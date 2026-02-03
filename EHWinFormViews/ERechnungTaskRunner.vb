Imports System
Imports System.Data
Imports System.IO
Imports ehfleet_classlibrary
Imports log4net

Public Class ERechnungTaskRunner
    Private ReadOnly _db As General.Database
    Private ReadOnly _log As ILog
    Private ReadOnly _planRepo As ERechnungTaskPlanRepository
    Private ReadOnly _runRepo As ERechnungTaskRunRepository

    Public Sub New(dbConnection As General.Database, logger As ILog, runRepo As ERechnungTaskRunRepository)
        _db = dbConnection
        _log = logger
        _planRepo = New ERechnungTaskPlanRepository(_db)
        _runRepo = runRepo
    End Sub

    Public Function RunTaskById(taskId As Integer, runId As Integer) As Boolean
        Dim plan = _planRepo.GetById(taskId)
        If plan Is Nothing Then Throw New Exception("TaskPlan nicht gefunden in dbo.ERechnungTaskPlan: " & taskId)

        If Not plan.Enabled Then
            _log.Info($"TaskId={taskId} ist deaktiviert – Abbruch.")
            Return True
        End If

        _log.Info($"ERechnungTaskRunner START taskId={taskId} runId={runId} Domain={plan.Domain} Action={plan.Action}")

        Try
            Dim dom As String = Normalize(plan.Domain)
            Dim act As String = Normalize(plan.Action)

            Dim dt = LoadInvoices(dom)
            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                _log.Info("Keine Datensätze zum Versand gefunden.")
                Return True
            End If

            Dim mailer = New XRechnungEmail(_db)
            Dim exporter As New XRechnungExporter(_db)

            Dim sent As Integer = 0
            Dim failed As Integer = 0

            For Each row As DataRow In dt.Rows
                Dim bill As Integer = Convert.ToInt32(row(0))
                Dim empfaenger As String = If(dt.Columns.Contains("EmailRechnung"), Convert.ToString(row("EmailRechnung")), "")
                empfaenger = If(empfaenger, "").Trim()

                Dim logEntry As New ExportLogEntry With {.RechnungsNummer = bill, .EmailEmpfaenger = empfaenger, .Timestamp = DateTime.UtcNow}

                If String.IsNullOrWhiteSpace(empfaenger) Then
                    logEntry.Status = "SKIPPED"
                    logEntry.EmailStatus = "SKIPPED"
                    logEntry.EmailFehlerInfo = "Kein Empfänger (EmailRechnung leer)."
                    failed += 1
                    PersistLogSafe(runId, logEntry)
                    Continue For
                End If

                Dim singleOk As Boolean
                Try
                    Select Case act
                        Case "EMAIL_PDF"
                            singleOk = mailer.SendXRechnungPdf(ParseDomainToRechnungsArt(dom), bill, empfaenger, logEntry)

                        Case "EMAIL_XML"
                            singleOk = mailer.SendXRechnungXml(ParseDomainToRechnungsArt(dom), bill, empfaenger, logEntry)

                        Case "EXPORT_XML"
                            singleOk = ExportXmlOnly(exporter, ParseDomainToRechnungsArt(dom), bill, logEntry)

                        Case "EXPORT_PDF"
                            singleOk = ExportPdfOnly(exporter, ParseDomainToRechnungsArt(dom), bill, logEntry)

                        Case Else
                            Throw New Exception("Unbekannte Action: " & act)
                    End Select
                Catch exOne As Exception
                    logEntry.Status = "FEHLER"
                    logEntry.EmailStatus = "FEHLER"
                    logEntry.EmailFehlerInfo = exOne.Message
                    logEntry.FehlerInfo = If(String.IsNullOrWhiteSpace(logEntry.FehlerInfo), exOne.ToString(), logEntry.FehlerInfo & Environment.NewLine & exOne.ToString())
                    singleOk = False
                End Try

                If singleOk Then
                    sent += 1
                Else
                    failed += 1
                End If

                PersistLogSafe(runId, logEntry)
            Next

            Dim ok As Boolean = (failed = 0)
            _log.Info($"ERechnungTaskRunner DONE taskId={taskId} runId={runId} ok={ok} sent={sent} failed={failed} total={dt.Rows.Count}")
            Return ok

        Catch ex As Exception
            _log.Error($"ERechnungTaskRunner FAILED taskId={taskId} runId={runId}: {ex.Message}", ex)
            Throw
        Finally
            _log.Info($"ERechnungTaskRunner END taskId={taskId} runId={runId}")
        End Try
    End Function

    Private Sub PersistLogSafe(runId As Integer, logEntry As ExportLogEntry)
        If _runRepo Is Nothing OrElse runId <= 0 Then Return

        Try
            Dim validatorText As String = ExtractValidatorText(logEntry)
            Dim validatorHtml As String = ReadAllTextIfExists(logEntry.HtmlValidatorPath)

            _runRepo.InsertLog(runId, logEntry, validatorText:=validatorText, validatorHtml:=validatorHtml)
        Catch ex As Exception
            _log.Error($"PersistRunLog FAILED runId={runId} bill={logEntry.RechnungsNummer}: {ex.Message}", ex)
        End Try
    End Sub

    Private Function ExtractValidatorText(e As ExportLogEntry) As String
        ' Exporter schreibt StdOut/StdErr bereits in FehlerInfo – wir spiegeln das 1:1 als ValidatorText
        Return If(e Is Nothing, "", If(e.FehlerInfo, ""))
    End Function

    Private Function ReadAllTextIfExists(path As String) As String
        Try
            If String.IsNullOrWhiteSpace(path) Then Return ""
            If Not File.Exists(path) Then Return ""
            Return File.ReadAllText(path)
        Catch
            Return ""
        End Try
    End Function

    Private Function Normalize(s As String) As String
        Return If(s, "").Trim().ToUpperInvariant()
    End Function

    Private Function LoadInvoices(domain As String) As DataTable
        Dim ra = ParseDomainToRechnungsArt(domain)
        Dim sql = RechnungsSqlHelper.GetSqlStatement(ra)
        Return _db.FillDataTable(sql)
    End Function

    Private Function ParseDomainToRechnungsArt(domain As String) As RechnungsArt
        Select Case Normalize(domain)
            Case "WERKSTATT", "RECHNUNG"
                Return RechnungsArt.Werkstatt
            Case "TANK", "TANKABRECHNUNG"
                Return RechnungsArt.Tanken
            Case "MANUELL", "MANUELLERECHNUNG"
                Return RechnungsArt.Manuell
            Case Else
                Throw New Exception("Unbekannte Domain: " & domain)
        End Select
    End Function

    Private Function ExportXmlOnly(exporter As XRechnungExporter,
                               billType As RechnungsArt,
                               rechnungsNummer As Integer,
                               logEntry As ExportLogEntry) As Boolean

        Dim xmlFilePath As String = exporter.GetExportFilePath(billType, rechnungsNummer, "xml")
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(xmlFilePath))

        If logEntry IsNot Nothing Then
            logEntry.ExportFilePath = ""
            logEntry.Status = "EXPORT_XML"
        End If

        ' Festschreiben / Duplikat
        If Not exporter.IsRechnungIssued(rechnungsNummer) Then
            Using fs = System.IO.File.Create(xmlFilePath)
                exporter.CreateBillXml(fs, billType, rechnungsNummer, True, logEntry)
            End Using
            If exporter.IsSuccess = False Then
                File.Delete(xmlFilePath)
            Else
                logEntry.ExportFilePath = xmlFilePath
            End If
        Else
            Using fs = System.IO.File.Create(xmlFilePath)
                exporter.ExportFinalizedXmlDuplicate(fs, billType, rechnungsNummer, logEntry)
            End Using
        End If

        If Not exporter.IsSuccess Then
            If logEntry IsNot Nothing Then
                If String.IsNullOrWhiteSpace(logEntry.FehlerInfo) Then
                    logEntry.FehlerInfo = "Export XML fehlgeschlagen."
                End If
            End If
            Return False
        End If

        If logEntry IsNot Nothing AndAlso String.IsNullOrWhiteSpace(logEntry.Status) Then
            logEntry.Status = "Erfolgreich"
        End If

        Return True
    End Function


    Private Function ExportPdfOnly(exporter As XRechnungExporter,
                               billType As RechnungsArt,
                               rechnungsNummer As Integer,
                               logEntry As ExportLogEntry) As Boolean

        Dim xmlFilePath As String = exporter.GetExportFilePath(billType, rechnungsNummer, "xml")
        Dim hybridPdfPath As String = exporter.GetExportFilePath(billType, rechnungsNummer, "hybrid.pdf")

        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(xmlFilePath))
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(hybridPdfPath))

        If logEntry IsNot Nothing Then
            logEntry.ExportFilePath = hybridPdfPath
            logEntry.Status = "EXPORT_PDF"
        End If

        ' 1) XML sicher erzeugen (Festschreiben / Duplikat)
        Dim xmlOk As Boolean = ExportXmlOnly(exporter, billType, rechnungsNummer, logEntry)
        If Not xmlOk Then Return False

        Dim xmlBytes As Byte() = System.IO.File.ReadAllBytes(xmlFilePath)

        ' 2) Original-PDF aus Blob holen
        Dim pdfBytes As Byte() = exporter.GetPdfRawFromBlob(rechnungsNummer, billType)
        If pdfBytes Is Nothing OrElse pdfBytes.Length = 0 Then
            If logEntry IsNot Nothing Then
                logEntry.Status = "Fehler"
                logEntry.FehlerInfo = "PDF-Blob ist leer oder nicht vorhanden."
            End If
            Return False
        End If

        ' 3) Hybrid-PDF erzeugen
        Using msPdf As New System.IO.MemoryStream(pdfBytes)
            Using msXml As New System.IO.MemoryStream(xmlBytes)
                Using outFs As System.IO.FileStream = System.IO.File.Create(hybridPdfPath)
                    exporter.CreateHybridPdfA_ZUGFeRD(msPdf, msXml, outFs, rechnungsNummer, billType, logEntry)
                End Using
            End Using
        End Using

        If logEntry IsNot Nothing AndAlso String.IsNullOrWhiteSpace(logEntry.Status) Then
            logEntry.Status = "Erfolgreich"
        End If

        Return True
    End Function

End Class
