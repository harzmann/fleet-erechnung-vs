Imports System.Net.Mail
Imports System.Data
Imports System.IO
Imports ehfleet_classlibrary
Imports log4net
Imports log4net.Config
Imports log4net.Appender
Imports log4net.Repository
'Imports EHWinFormViews

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
    ''' Erweiterte Funktion: ExportLogEntry und Versand-Logging
    ''' </summary>
    Public Function SendXRechnungXml(
        billType As RechnungsArt,
        rechnungsNummer As Integer,
        empfaengerEmail As String,
        Optional logEntry As ExportLogEntry = Nothing
    ) As Boolean
        Dim exporter As New XRechnungExporter(_dbConnection)
        Dim xmlFilePath As String = exporter.GetExportFilePath(billType, rechnungsNummer, "xml")
        Dim xmlFestgeschrieben As Boolean = False
        Dim mailSent As Boolean = False
        Dim mailRawId As Guid = Guid.NewGuid()
        Dim mailVersandId As Guid = Guid.NewGuid()
        Dim mailError As String = ""
        Dim mailStatus As String = ""
        Dim mailSubject As String = $"XRechnung {rechnungsNummer}"
        Dim mailBody As String = "Ihre elektronische Rechnung im XRechnungs-Format im Anhang."
        Dim xmlBytes() As Byte = Nothing
        Dim blobId As Guid = Guid.Empty

        If logEntry Is Nothing Then logEntry = New ExportLogEntry()
        logEntry.RechnungsNummer = rechnungsNummer
        logEntry.EmailEmpfaenger = empfaengerEmail

        Try
            _smtpLogger.Info($"[START] Sende XRechnung-XML: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}")

            ' 1. Festschreiben oder Duplikat erzeugen
            If Not exporter.IsRechnungIssued(rechnungsNummer) Then
                Using fs = File.Create(xmlFilePath)
                    exporter.CreateBillXml(fs, billType, rechnungsNummer, True, logEntry)
                End Using
                If Not exporter.IsSuccess Then
                    _smtpLogger.Error($"Festschreibung fehlgeschlagen: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}")
                    logEntry.Status = "Fehler"
                    logEntry.EmailStatus = "Festschreibung fehlgeschlagen"
                    Return False
                End If
                xmlFestgeschrieben = True
                xmlBytes = File.ReadAllBytes(xmlFilePath)
            Else
                ' Duplikat erzeugen
                Using fs = File.Create(xmlFilePath)
                    exporter.ExportFinalizedXmlDuplicate(fs, billType, rechnungsNummer, logEntry)
                End Using
                xmlFestgeschrieben = True
                xmlBytes = File.ReadAllBytes(xmlFilePath)
            End If

            ' 1a. Ermittle BlobId aus passender Blob-Tabelle
            If xmlFestgeschrieben Then
                Select Case billType
                    Case RechnungsArt.Werkstatt
                        Using cmdBlob As New OleDb.OleDbCommand("SELECT TOP 1 BlobId FROM RechnungBlob WHERE RechnungsNr = ?", _dbConnection.cn)
                            cmdBlob.Parameters.AddWithValue("@RechnungsNr", rechnungsNummer)
                            Dim result = cmdBlob.ExecuteScalar()
                            If result IsNot Nothing AndAlso Guid.TryParse(result.ToString(), blobId) Then
                                blobId = CType(result, Guid)
                            End If
                        End Using
                    Case RechnungsArt.Tanken
                        Using cmdBlob As New OleDb.OleDbCommand("SELECT TOP 1 BlobId FROM TankabrechnungBlob WHERE RechnungsNr = ?", _dbConnection.cn)
                            cmdBlob.Parameters.AddWithValue("@RechnungsNr", rechnungsNummer)
                            Dim result = cmdBlob.ExecuteScalar()
                            If result IsNot Nothing AndAlso Guid.TryParse(result.ToString(), blobId) Then
                                blobId = CType(result, Guid)
                            End If
                        End Using
                    Case RechnungsArt.Manuell
                        Using cmdBlob As New OleDb.OleDbCommand("SELECT TOP 1 BlobId FROM ManuelleRechnungBlob WHERE RechnungsNr = ?", _dbConnection.cn)
                            cmdBlob.Parameters.AddWithValue("@RechnungsNr", rechnungsNummer)
                            Dim result = cmdBlob.ExecuteScalar()
                            If result IsNot Nothing AndAlso Guid.TryParse(result.ToString(), blobId) Then
                                blobId = CType(result, Guid)
                            End If
                        End Using
                End Select
            End If

            ' 2. Versand nur wenn festgeschrieben/dupliziert
            If xmlFestgeschrieben AndAlso xmlBytes IsNot Nothing AndAlso xmlBytes.Length > 0 Then
                _smtpLogger.Info($"Bereite E-Mail-Versand vor: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}")
                Dim mail As New System.Net.Mail.MailMessage()
                mail.To.Add(empfaengerEmail)
                mail.Subject = mailSubject
                mail.Body = mailBody
                mail.Attachments.Add(New System.Net.Mail.Attachment(New MemoryStream(xmlBytes), "xrechnung.xml", "text/xml"))
                Dim smtp As New System.Net.Mail.SmtpClient()
                Try
                    _smtpLogger.Info($"Sende E-Mail: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}, Betreff={mailSubject}")
                    smtp.Send(mail)
                    mailSent = True
                    mailStatus = "Erfolgreich"
                    logEntry.EmailStatus = "Erfolgreich"
                    _smtpLogger.Info($"E-Mail erfolgreich gesendet: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}")
                Catch ex As Exception
                    mailError = ex.Message
                    mailStatus = "Fehler"
                    logEntry.EmailStatus = "Fehler"
                    logEntry.EmailFehlerInfo = ex.Message
                    logEntry.Status = "Fehler"
                    _smtpLogger.Error($"Fehler beim E-Mail-Versand: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}, Fehler={ex.Message}")
                    Return False
                End Try
            Else
                _smtpLogger.Error($"XML nicht festgeschrieben: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}")
                logEntry.Status = "Fehler"
                logEntry.EmailStatus = "XML nicht festgeschrieben"
                Return False
            End If

            ' 3. Versand-Logging in DB
            If mailSent Then
                _smtpLogger.Info($"Logge Versand in DB: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}, Status={mailStatus}")
                Dim now As DateTime = DateTime.Now
                Dim versandId As Long
                ' Die VersandId ist BIGINT IDENTITY, daher Insert + SCOPE_IDENTITY() nötig
                Dim sqlVersand As String = ""
                Dim sqlMailRaw As String = ""
                Dim kanal As String = "E-Mail"
                Dim betreff As String = mailSubject
                Dim nachrichtKurz As String = mailBody
                Dim attachmentHashes As String = "" ' Optional: Hashes der Attachments, falls vorhanden
                Dim providerMessageId As String = mailRawId.ToString() ' Mapping: MessageId → ProviderMessageId
                Dim sentAt As DateTime = now
                Dim deliveredAt As Object = DBNull.Value ' Nicht gesetzt
                Dim lastError As String = If(logEntry.EmailFehlerInfo, "")
                Dim detailsJson As String = "" ' Optional: weitere Details als JSON
                Dim createdAt As DateTime = now
                Dim createdBy As String = Environment.UserName

                Select Case billType
                    Case RechnungsArt.Werkstatt
                        sqlVersand = "INSERT INTO RechnungVersand (RechnungsNr, BlobId, Kanal, Status, EmpfaengerAdresse, Betreff, NachrichtKurz, AttachmentHashes, ProviderMessageId, SentAt, DeliveredAt, LastError, DetailsJson, CreatedAt, CreatedBy) " &
                                     "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
                        sqlMailRaw = "INSERT INTO RechnungVersandMailRaw (VersandId, MimeRaw) VALUES (?, ?)"
                    Case RechnungsArt.Tanken
                        sqlVersand = "INSERT INTO TankabrechnungVersand (TankabrechnungNr, BlobId, Kanal, Status, EmpfaengerAdresse, Betreff, NachrichtKurz, AttachmentHashes, ProviderMessageId, SentAt, DeliveredAt, LastError, DetailsJson, CreatedAt, CreatedBy) " &
                                     "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
                        sqlMailRaw = "INSERT INTO TankabrechnungVersandMailRaw (VersandId, MimeRaw) VALUES (?, ?)"
                    Case RechnungsArt.Manuell
                        sqlVersand = "INSERT INTO ManuelleRechnungVersand (RechnungsNr, BlobId, Kanal, Status, EmpfaengerAdresse, Betreff, NachrichtKurz, AttachmentHashes, ProviderMessageId, SentAt, DeliveredAt, LastError, DetailsJson, CreatedAt, CreatedBy) " &
                                     "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
                        sqlMailRaw = "INSERT INTO ManuelleRechnungVersandMailRaw (VersandId, MimeRaw) VALUES (?, ?)"
                End Select

                ' Insert in Versand-Tabelle und erhalte VersandId (IDENTITY)
                Using cmd1 As New OleDb.OleDbCommand(sqlVersand & ";SELECT @@IDENTITY", _dbConnection.cn)
                    Select Case billType
                        Case RechnungsArt.Werkstatt
                            cmd1.Parameters.AddWithValue("@RechnungsNr", rechnungsNummer)
                        Case RechnungsArt.Tanken
                            cmd1.Parameters.AddWithValue("@TankabrechnungNr", rechnungsNummer)
                        Case RechnungsArt.Manuell
                            cmd1.Parameters.AddWithValue("@ManuelleRechnungNr", rechnungsNummer)
                    End Select
                    cmd1.Parameters.AddWithValue("@BlobId", blobId.ToString())
                    cmd1.Parameters.AddWithValue("@Kanal", kanal)
                    cmd1.Parameters.AddWithValue("@Status", mailStatus)
                    cmd1.Parameters.AddWithValue("@EmpfaengerAdresse", empfaengerEmail)
                    cmd1.Parameters.AddWithValue("@Betreff", betreff)
                    cmd1.Parameters.AddWithValue("@NachrichtKurz", nachrichtKurz)
                    cmd1.Parameters.AddWithValue("@AttachmentHashes", attachmentHashes)
                    cmd1.Parameters.AddWithValue("@ProviderMessageId", providerMessageId)
                    cmd1.Parameters.AddWithValue("@SentAt", sentAt)
                    cmd1.Parameters.AddWithValue("@DeliveredAt", deliveredAt)
                    cmd1.Parameters.AddWithValue("@LastError", lastError)
                    cmd1.Parameters.AddWithValue("@DetailsJson", detailsJson)
                    cmd1.Parameters.AddWithValue("@CreatedAt", createdAt)
                    cmd1.Parameters.AddWithValue("@CreatedBy", createdBy)
                    Dim obj = cmd1.ExecuteScalar()
                    versandId = Convert.ToInt64(obj)
                End Using

                ' Insert in MailRaw-Tabelle (VersandId, MimeRaw)
                Using cmd2 As New OleDb.OleDbCommand(sqlMailRaw, _dbConnection.cn)
                    cmd2.Parameters.AddWithValue("@VersandId", versandId)
                    cmd2.Parameters.AddWithValue("@MimeRaw", xmlBytes)
                    cmd2.ExecuteNonQuery()
                End Using
            End If

            logEntry.Status = "Erfolgreich"
            logEntry.EmailStatus = mailStatus
            _smtpLogger.Info($"[END] Sende XRechnung-XML: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}, Status={mailStatus}")
            Return mailSent

        Catch ex As Exception
            logEntry.Status = "Fehler"
            logEntry.EmailStatus = "Fehler"
            logEntry.EmailFehlerInfo = ex.Message
            _smtpLogger.Error($"Unerwarteter Fehler: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}, Fehler={ex.Message}")
            Return False
        End Try
    End Function

End Class
