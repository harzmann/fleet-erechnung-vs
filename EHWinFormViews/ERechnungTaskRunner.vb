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

        Dim allowDuplicates As Boolean = plan.IncludeDuplicates
        Dim onlyNewSinceLastRun As Boolean = plan.OnlyNewSinceLastRun
        _log.Info($"IncludeDuplicates={allowDuplicates}")
        _log.Info($"OnlyNewSinceLastRun={onlyNewSinceLastRun}")

        Try
            Dim dom As String = Normalize(plan.Domain)
            Dim act As String = Normalize(plan.Action)

            Dim dt = LoadInvoices(dom)

            If onlyNewSinceLastRun Then
                Dim lastOkEndedUtc As DateTime? = _runRepo.GetLastSuccessfulEndedUtc(taskId)
                If lastOkEndedUtc.HasValue Then
                    _log.Info($"Delta-Filter aktiv: nur Rechnungen neu seit letztem OK-Lauf (EndedUtc UTC)={lastOkEndedUtc.Value:O}")
                    dt = ApplyOnlyNewSinceLastRunFilter(dt, lastOkEndedUtc.Value)
                Else
                    _log.Info("Delta-Filter aktiv, aber kein vorheriger erfolgreicher Task-Lauf gefunden -> exportiere/versende alle passenden Rechnungen.")
                End If
            End If

            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                _log.Info("Keine Datensätze zum Versand/Export gefunden.")
                Return True
            End If

            Dim mailer = New XRechnungEmail(_db)
            Dim exporter As New XRechnungExporter(_db)

            Dim sent As Integer = 0
            Dim failed As Integer = 0
            Dim skippedDup As Integer = 0
            Dim skippedNoRecipient As Integer = 0

            For Each row As DataRow In dt.Rows
                Dim bill As Integer = Convert.ToInt32(row(0))

                ' EmailEmpfänger nur relevant bei EMAIL_* Actions
                Dim empfaenger As String = ""
                If dt.Columns.Contains("EmailRechnung") Then
                    empfaenger = Convert.ToString(row("EmailRechnung"))
                End If
                empfaenger = If(empfaenger, "").Trim()

                Dim logEntry As New ExportLogEntry With {
                    .RechnungsNummer = bill,
                    .EmailEmpfaenger = empfaenger,
                    .Timestamp = DateTime.UtcNow
                }

                Dim isEmailAction As Boolean = (act = "EMAIL_PDF" OrElse act = "EMAIL_XML")

                ' Für EMAIL_* muss ein Empfänger vorhanden sein
                If isEmailAction AndAlso String.IsNullOrWhiteSpace(empfaenger) Then
                    logEntry.Status = "Übersprungen (kein Empfänger)"
                    logEntry.EmailStatus = "Übersprungen (kein Empfänger)"
                    logEntry.EmailFehlerInfo = "Kein Empfänger (EmailRechnung leer)."
                    skippedNoRecipient += 1
                    PersistLogSafe(runId, logEntry)
                    Continue For
                End If

                ' Standard: nur nicht festgeschriebene Rechnungen (Duplikate nur wenn erlaubt)
                Dim isIssued As Boolean = exporter.IsRechnungIssued(bill, ParseDomainToRechnungsArt(dom))
                If isIssued AndAlso Not allowDuplicates Then
                    logEntry.Status = "Übersprungen (Duplikat)"
                    logEntry.FehlerInfo = "Festgeschriebene Rechnung wurde übersprungen (Duplikate nicht aktiviert)."
                    If isEmailAction Then
                        logEntry.EmailStatus = "Übersprungen (Duplikat)"
                        logEntry.EmailFehlerInfo = "Duplikatversand nicht aktiviert."
                    End If
                    skippedDup += 1
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
                    If isEmailAction Then logEntry.EmailStatus = "FEHLER"
                    logEntry.FehlerInfo = If(String.IsNullOrWhiteSpace(logEntry.FehlerInfo), exOne.ToString(), logEntry.FehlerInfo & Environment.NewLine & exOne.ToString())
                    If isEmailAction AndAlso String.IsNullOrWhiteSpace(logEntry.EmailFehlerInfo) Then
                        logEntry.EmailFehlerInfo = exOne.Message
                    End If
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
            _log.Info($"ERechnungTaskRunner DONE taskId={taskId} runId={runId} ok={ok} sent={sent} failed={failed} skippedDup={skippedDup} skippedNoRecipient={skippedNoRecipient} total={dt.Rows.Count}")
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
        Directory.CreateDirectory(Path.GetDirectoryName(xmlFilePath))

        If logEntry IsNot Nothing Then
            logEntry.ExportFilePath = xmlFilePath
            If String.IsNullOrWhiteSpace(logEntry.Status) Then logEntry.Status = "EXPORT_XML"
            If logEntry.Timestamp = DateTime.MinValue Then logEntry.Timestamp = DateTime.UtcNow
        End If

        If Not exporter.IsRechnungIssued(rechnungsNummer, billType) Then
            Using fs = File.Create(xmlFilePath)
                exporter.CreateBillXml(fs, billType, rechnungsNummer, True, logEntry)
            End Using
        Else
            Using fs = File.Create(xmlFilePath)
                exporter.ExportFinalizedXmlDuplicate(fs, billType, rechnungsNummer, logEntry)
            End Using
        End If

        If Not exporter.IsSuccess Then
            If logEntry IsNot Nothing AndAlso String.IsNullOrWhiteSpace(logEntry.FehlerInfo) Then
                logEntry.FehlerInfo = "Export XML fehlgeschlagen."
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

        Directory.CreateDirectory(Path.GetDirectoryName(xmlFilePath))
        Directory.CreateDirectory(Path.GetDirectoryName(hybridPdfPath))

        If logEntry IsNot Nothing Then
            logEntry.ExportFilePath = hybridPdfPath
            If String.IsNullOrWhiteSpace(logEntry.Status) Then logEntry.Status = "EXPORT_PDF"
            If logEntry.Timestamp = DateTime.MinValue Then logEntry.Timestamp = DateTime.UtcNow
        End If

        Dim xmlOk As Boolean = ExportXmlOnly(exporter, billType, rechnungsNummer, logEntry)
        If Not xmlOk Then Return False

        Dim xmlBytes As Byte() = File.ReadAllBytes(xmlFilePath)

        Dim pdfBytes As Byte() = exporter.GetPdfRawFromBlob(rechnungsNummer, billType)
        If pdfBytes Is Nothing OrElse pdfBytes.Length = 0 Then
            If logEntry IsNot Nothing Then
                logEntry.Status = "Fehler"
                logEntry.FehlerInfo = "PDF-Blob ist leer oder nicht vorhanden."
            End If
            Return False
        End If

        Using msPdf As New MemoryStream(pdfBytes)
            Using msXml As New MemoryStream(xmlBytes)
                Using outFs As FileStream = File.Create(hybridPdfPath)
                    exporter.CreateHybridPdfA_ZUGFeRD(msPdf, msXml, outFs, rechnungsNummer, billType, logEntry)
                End Using
            End Using
        End Using

        If logEntry IsNot Nothing AndAlso String.IsNullOrWhiteSpace(logEntry.Status) Then
            logEntry.Status = "Erfolgreich"
        End If

        Return True
    End Function

    ''' <summary>
    ''' Filtert auf Rechnungen, deren Rechnungsdatum nach dem letzten erfolgreichen Task-Ende liegt.
    ''' Rechnungsdatum ist lokale Zeit (kein UTC).
    ''' </summary>
    Private Function ApplyOnlyNewSinceLastRunFilter(dt As DataTable, lastEndedUtc As DateTime) As DataTable
        If dt Is Nothing Then Return dt

        Const colName As String = "Rechnungsdatum"

        If Not dt.Columns.Contains(colName) Then
            _log.Warn($"Delta-Filter: Spalte '{colName}' nicht in Invoice-Query gefunden -> Filter wird übersprungen.")
            Return dt
        End If

        ' lastEndedUtc kommt aus dbo.ERechnungTaskRun (UTC). Rechnungsdatum ist lokal -> Cutoff lokal.
        Dim cutoffLocal As DateTime
        Try
            cutoffLocal = lastEndedUtc.ToLocalTime()
        Catch
            cutoffLocal = lastEndedUtc
        End Try

        Dim out As DataTable = dt.Clone()
        Dim kept As Integer = 0
        Dim dropped As Integer = 0

        For Each r As DataRow In dt.Rows
            Dim v As Object = r(colName)
            Dim d As DateTime
            Dim ok As Boolean = False

            If v IsNot Nothing AndAlso v IsNot DBNull.Value Then
                If TypeOf v Is DateTime Then
                    d = DirectCast(v, DateTime)
                    ok = True
                Else
                    ok = DateTime.TryParse(Convert.ToString(v).Trim(), d)
                End If
            End If

            ' Sicherheitsverhalten: wenn Datum nicht parsebar -> NICHT wegfiltern
            If Not ok Then
                out.ImportRow(r)
                kept += 1
            ElseIf d > cutoffLocal Then
                out.ImportRow(r)
                kept += 1
            Else
                dropped += 1
            End If
        Next

        _log.Info($"Delta-Filter: Spalte='{colName}', CutoffLocal='{cutoffLocal:O}', kept={kept}, dropped={dropped}")
        Return out
    End Function


End Class
