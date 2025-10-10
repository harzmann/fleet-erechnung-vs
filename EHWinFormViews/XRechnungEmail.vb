Imports System.Net.Mail
Imports System.IO
Imports ehfleet_classlibrary
Imports log4net
Imports log4net.Config
Imports log4net.Appender
Imports log4net.Repository

Public Class XRechnungEmail
    Private ReadOnly _dbConnection As General.Database
    Private ReadOnly _xmlExporter As XRechnungExporter

    ' SMTP-spezifischer Logger
    Private Shared ReadOnly _smtpLogger As ILog

    Shared Sub New()
        ' Logger für SMTP-Logdatei initialisieren
        ' Konfiguration analog zu logger.config, aber mit anderem Dateinamen
        Dim repository As ILoggerRepository = LogManager.GetRepository()
        Dim smtpAppenderName As String = "EHFleetXRechnungSmtpAppender"
        Dim logFilePath As String = "logs/EHFleetXRechnungSmtp.log"

        ' Prüfe, ob Appender bereits existiert
        Dim existing = repository.GetAppenders().FirstOrDefault(Function(a) a.Name = smtpAppenderName)
        If existing Is Nothing Then
            Dim appender As New RollingFileAppender()
            appender.Name = smtpAppenderName
            appender.File = logFilePath
            appender.AppendToFile = True
            appender.MaxSizeRollBackups = 5
            appender.MaximumFileSize = "10MB"
            appender.RollingStyle = RollingFileAppender.RollingMode.Size
            appender.StaticLogFileName = True
            appender.Layout = New log4net.Layout.PatternLayout("%date [%thread] %-5level %logger - %message%newline")
            appender.LockingModel = New log4net.Appender.FileAppender.MinimalLock()
            appender.ActivateOptions()
            log4net.Config.BasicConfigurator.Configure(repository, appender)
        End If
        _smtpLogger = LogManager.GetLogger(smtpAppenderName)
    End Sub

    Public Sub New(dbConnection As General.Database)
        _dbConnection = dbConnection
        _xmlExporter = New XRechnungExporter(dbConnection)
    End Sub

    ''' <summary>
    ''' Sendet die XRechnung-XML für die angegebene Rechnungsnummer an die übergebene E-Mail-Adresse.
    ''' SMTP-Konfiguration wird aus Parameter 6101 geladen.
    ''' Ausführliches Logging in logs/EHFleetXRechnungSmtp.log.
    ''' </summary>
    Public Function SendXRechnungXml(rechnungsArt As RechnungsArt, rechnungsNummer As Integer, empfaengerEmail As String) As Boolean
        _smtpLogger.Info($"Starte Versand für Rechnung {rechnungsNummer} an {empfaengerEmail}")

        ' SMTP Parameter auslesen
        Dim paramHelper = New General.Parameter(_dbConnection)
        Dim smtpParam = paramHelper.GetParameter(6101)
        If Not smtpParam.IsSuccessful Then
            _smtpLogger.Error("Parameter 6101 konnte nicht ausgelesen werden oder ist nicht angelegt.")
            Return False
        End If

        Dim smtpServer = smtpParam.Modul5
        Dim smtpUser = smtpParam.Modul6
        Dim smtpPassword = smtpParam.Modul7
        Dim smtpPortStr = smtpParam.Modul8

        ' Validierung der SMTP-Daten
        If String.IsNullOrWhiteSpace(smtpServer) OrElse
           String.IsNullOrWhiteSpace(smtpUser) OrElse
           String.IsNullOrWhiteSpace(smtpPassword) OrElse
           String.IsNullOrWhiteSpace(smtpPortStr) Then
            _smtpLogger.Error("Ungültige SMTP-Daten: Mindestens ein Wert ist leer.")
            Return False
        End If

        Dim smtpPort As Integer
        If Not Integer.TryParse(smtpPortStr, smtpPort) OrElse smtpPort <= 0 Then
            _smtpLogger.Error($"Ungültiger SMTP-Port: '{smtpPortStr}'")
            Return False
        End If

        _smtpLogger.Info($"SMTP-Konfiguration: Server={smtpServer}, User={smtpUser}, Port={smtpPort}")

        ' XML erzeugen
        Dim tempFile As String = Path.GetTempFileName()
        Try
            Using fs = File.Create(tempFile)
                _xmlExporter.CreateBillXml(fs, rechnungsArt, rechnungsNummer)
            End Using
        Catch ex As Exception
            _smtpLogger.Error($"Fehler beim Erstellen der XML-Datei: {ex.Message}")
            Return False
        End Try

        If Not _xmlExporter.IsSuccess Then
            _smtpLogger.Error("XML-Erstellung fehlgeschlagen (IsSuccess=False).")
            Return False
        End If

        ' E-Mail versenden
        Try
            Dim mail As New MailMessage()
            mail.To.Add(empfaengerEmail)
            mail.Subject = $"XRechnung XML für Rechnung {rechnungsNummer}"
            mail.Body = "Im Anhang finden Sie die XRechnung XML-Datei."
            mail.Attachments.Add(New Attachment(tempFile))

            Using smtp As New SmtpClient(smtpServer, smtpPort)
                smtp.EnableSsl = True
                smtp.Credentials = New Net.NetworkCredential(smtpUser, smtpPassword)
                _smtpLogger.Info($"Sende E-Mail an {empfaengerEmail} ...")
                smtp.Send(mail)
            End Using

            _smtpLogger.Info($"E-Mail für Rechnung {rechnungsNummer} erfolgreich versendet.")
            Return True
        Catch ex As Exception
            _smtpLogger.Error($"Fehler beim E-Mail-Versand: {ex.Message}")
            Return False
        Finally
            Try
                File.Delete(tempFile)
                _smtpLogger.Info($"Temporäre XML-Datei gelöscht: {tempFile}")
            Catch ex As Exception
                _smtpLogger.Warn($"Konnte temporäre Datei nicht löschen: {tempFile} ({ex.Message})")
            End Try
        End Try
    End Function

    ' ...existing code...
End Class
