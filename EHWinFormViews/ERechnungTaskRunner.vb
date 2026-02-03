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

            Dim sent As Integer = 0
            Dim failed As Integer = 0

            For Each row As DataRow In dt.Rows
                Dim bill As Integer = Convert.ToInt32(row(0))
                Dim empfaenger As String = If(dt.Columns.Contains("EmailRechnung"), Convert.ToString(row("EmailRechnung")), "")
                empfaenger = If(empfaenger, "").Trim()

                Dim logEntry As New ExportLogEntry With {
                    .RechnungsNummer = bill,
                    .EmailEmpfaenger = empfaenger
                }

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
End Class
