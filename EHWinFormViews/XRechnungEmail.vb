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

    ' SMTP-Konfiguration aus ParameterNr=6101 laden, mit Logging und Betreff-Template
    Private Function GetSmtpConfigFromParameter() As (FromAddress As String, SmtpServer As String, SmtpUser As String, SmtpPassword As String, Port As Integer, SubjectTemplate As String)
        _smtpLogger.Info("Lese SMTP-Konfiguration aus ParameterNr=6101 ...")
        Try
            Dim sql As String = "SELECT * FROM Parameter WHERE ParameterNr = 6101"
            Dim dt As DataTable = _dbConnection.FillDataTable(sql)
            If dt.Rows.Count = 0 Then
                _smtpLogger.Error("SMTP-Parameter (Nr. 6101) nicht gefunden.")
                Throw New Exception("SMTP-Parameter (Nr. 6101) nicht gefunden.")
            End If
            Dim row = dt.Rows(0)
            Dim fromAddress As String = If(Not IsDBNull(row("Modul4")), row("Modul4").ToString(), "")
            Dim smtpServer As String = If(Not IsDBNull(row("Modul5")), row("Modul5").ToString(), "")
            Dim smtpUser As String = If(Not IsDBNull(row("Modul6")), row("Modul6").ToString(), "")
            Dim smtpPassword As String = If(Not IsDBNull(row("Modul7")), row("Modul7").ToString(), "")
            Dim port As Integer = 25
            Dim portStr As String = If(Not IsDBNull(row("Modul8")), row("Modul8").ToString().Trim(), "")
            If Not String.IsNullOrWhiteSpace(portStr) Then
                If Not Integer.TryParse(portStr, port) Then
                    port = 25
                End If
            Else
                port = 25
            End If
            Dim subjectTemplate As String = ""
            If row.Table.Columns.Contains("Text") AndAlso Not IsDBNull(row("Text")) Then
                subjectTemplate = row("Text").ToString()
            End If
            _smtpLogger.Info($"SMTP-Konfiguration geladen: From={fromAddress}, Server={smtpServer}, User={smtpUser}, Port={port}, SubjectTemplate={subjectTemplate}")
            Return (fromAddress, smtpServer, smtpUser, smtpPassword, port, subjectTemplate)
        Catch ex As Exception
            _smtpLogger.Error("Fehler beim Laden der SMTP-Konfiguration: " & ex.Message)
            Throw
        End Try
    End Function

    ' Neue Funktion: Body-Template für E-Mail aus ParameterNr=6102 laden
    Private Function GetMailBodyTemplateFromParameter() As String
        _smtpLogger.Info("Lese Body-Template aus ParameterNr=6102 ...")
        Try
            Dim sql As String = "SELECT * FROM Parameter WHERE ParameterNr = 6102"
            Dim dt As DataTable = _dbConnection.FillDataTable(sql)
            If dt.Rows.Count = 0 Then
                _smtpLogger.Info("Body-Template-Parameter (Nr. 6102) nicht gefunden.")
                Return ""
            End If
            Dim row = dt.Rows(0)
            ' Aktiv-Flag prüfen: z.B. Feld "Aktiv" = True oder 1, falls vorhanden
            Dim isActive As Boolean = True
            If row.Table.Columns.Contains("Aktiv") AndAlso Not IsDBNull(row("Aktiv")) Then
                Dim aktivValue = row("Aktiv").ToString().Trim().ToLower()
                isActive = (aktivValue = "1" OrElse aktivValue = "true" OrElse aktivValue = "ja" OrElse aktivValue = "yes")
            End If
            Dim bodyTemplate As String = ""
            If row.Table.Columns.Contains("Text") AndAlso Not IsDBNull(row("Text")) Then
                bodyTemplate = row("Text").ToString()
            End If
            If isActive AndAlso Not String.IsNullOrWhiteSpace(bodyTemplate) Then
                _smtpLogger.Info("Body-Template aus ParameterNr=6102 wird verwendet.")
                Return bodyTemplate
            End If
            _smtpLogger.Info("Body-Template aus ParameterNr=6102 ist nicht aktiv oder leer.")
            Return ""
        Catch ex As Exception
            _smtpLogger.Error("Fehler beim Laden des Body-Templates: " & ex.Message)
            Return ""
        End Try
    End Function

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
        Dim xmlBytes() As Byte = Nothing
        Dim blobId As Guid = Guid.Empty

        If logEntry Is Nothing Then logEntry = New ExportLogEntry()
        logEntry.RechnungsNummer = rechnungsNummer
        logEntry.EmailEmpfaenger = empfaengerEmail

        Try
            _smtpLogger.Info($"[START] Sende XRechnung-XML: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}")

            ' SMTP-Konfiguration laden
            Dim smtpConfig = GetSmtpConfigFromParameter()
            If String.IsNullOrWhiteSpace(smtpConfig.SmtpServer) OrElse String.IsNullOrWhiteSpace(smtpConfig.FromAddress) Then
                Throw New Exception("SMTP-Konfiguration (Server/Absender) fehlt in Parameter 6101.")
            End If

            ' Betreff ermitteln und Platzhalter ersetzen
            Dim formattedInvoiceNumber As String = _xmlExporter.GetFormattedBillNumber(rechnungsNummer, billType)
            Dim subjectTemplate As String = smtpConfig.SubjectTemplate
            If String.IsNullOrWhiteSpace(subjectTemplate) Then
                subjectTemplate = "Elektronischer Rechnungsversand Beleg: [RGNR]"
            End If
            Dim mailSubject As String = subjectTemplate.Replace("[RGNR]", formattedInvoiceNumber)

            ' Body-Template aus Parameter 6102 laden und ggf. verwenden
            Dim mailBody As String = "Ihre elektronische Rechnung im XRechnungs-Format im Anhang."
            Dim paramBody As String = GetMailBodyTemplateFromParameter()
            If Not String.IsNullOrWhiteSpace(paramBody) Then
                mailBody = paramBody
            End If

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
                ' Dateiname für Anhang nach Schema von GetExportFilePath
                Dim xmlAttachmentName As String = Path.GetFileName(xmlFilePath)
                mail.Attachments.Add(New System.Net.Mail.Attachment(New MemoryStream(xmlBytes), xmlAttachmentName, "text/xml"))
                mail.From = New System.Net.Mail.MailAddress(smtpConfig.FromAddress)
                Dim smtp As New System.Net.Mail.SmtpClient()
                smtp.Host = smtpConfig.SmtpServer
                smtp.Port = smtpConfig.Port
                smtp.EnableSsl = True ' Optional: ggf. aus Parametern steuerbar machen
                If Not String.IsNullOrWhiteSpace(smtpConfig.SmtpUser) Then
                    smtp.Credentials = New System.Net.NetworkCredential(smtpConfig.SmtpUser, smtpConfig.SmtpPassword)
                End If
                Try
                    _smtpLogger.Info($"Sende E-Mail: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}, Betreff={mailSubject}")
                    ' Akzeptiere alle Zertifikate (nur für Testzwecke, nicht für Produktion!)
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = Function(sender, certificate, chain, sslPolicyErrors) True
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

    ''' <summary>
    ''' Sendet das Hybrid-PDF für die angegebene Rechnungsnummer an die übergebene E-Mail-Adresse.
    ''' SMTP-Konfiguration wird aus Parameter 6101 geladen.
    ''' Body-Template analog zu XML-Versand.
    ''' Rechnung wird wie bei XML zuerst festgeschrieben, PDF-Blob wird bevorzugt verwendet.
    ''' </summary>
    Public Function SendXRechnungPdf(
        billType As RechnungsArt,
        rechnungsNummer As Integer,
        empfaengerEmail As String,
        Optional logEntry As ExportLogEntry = Nothing
    ) As Boolean
        Dim exporter As New XRechnungExporter(_dbConnection)
        Dim xmlFilePath As String = exporter.GetExportFilePath(billType, rechnungsNummer, "xml")
        Dim hybridPdfPath As String = exporter.GetExportFilePath(billType, rechnungsNummer, "hybrid.pdf")
        Dim pdfFestgeschrieben As Boolean = False
        Dim mailSent As Boolean = False
        Dim mailRawId As Guid = Guid.NewGuid()
        Dim mailVersandId As Guid = Guid.NewGuid()
        Dim mailError As String = ""
        Dim mailStatus As String = ""
        Dim mailBody As String = "Ihre elektronische Rechnung als Hybrid-PDF im Anhang."
        Dim pdfBytes() As Byte = Nothing
        Dim blobId As Guid = Guid.Empty

        If logEntry Is Nothing Then logEntry = New ExportLogEntry()
        logEntry.RechnungsNummer = rechnungsNummer
        logEntry.EmailEmpfaenger = empfaengerEmail

        Try
            _smtpLogger.Info($"[START] Sende Hybrid-PDF: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}")

            ' SMTP-Konfiguration laden
            Dim smtpConfig = GetSmtpConfigFromParameter()
            If String.IsNullOrWhiteSpace(smtpConfig.SmtpServer) OrElse String.IsNullOrWhiteSpace(smtpConfig.FromAddress) Then
                Throw New Exception("SMTP-Konfiguration (Server/Absender) fehlt in Parameter 6101.")
            End If

            ' Betreff ermitteln und Platzhalter ersetzen
            Dim formattedInvoiceNumber As String = _xmlExporter.GetFormattedBillNumber(rechnungsNummer, billType)
            Dim subjectTemplate As String = smtpConfig.SubjectTemplate
            If String.IsNullOrWhiteSpace(subjectTemplate) Then
                subjectTemplate = "Elektronischer Rechnungsversand Beleg: [RGNR]"
            End If
            Dim mailSubject As String = subjectTemplate.Replace("[RGNR]", formattedInvoiceNumber)

            ' Body-Template aus Parameter 6102 laden und ggf. verwenden
            Dim paramBody As String = GetMailBodyTemplateFromParameter()
            If Not String.IsNullOrWhiteSpace(paramBody) Then
                mailBody = paramBody
            End If

            ' 1. Festschreiben oder Duplikat erzeugen (wie bei XML)
            If Not exporter.IsRechnungIssued(rechnungsNummer) Then
                Using fs = File.Create(xmlFilePath)
                    exporter.CreateBillXml(fs, billType, rechnungsNummer, True, logEntry)
                End Using
                If Not exporter.IsSuccess Then
                    _smtpLogger.Error($"Festschreibung fehlgeschlagen: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}")
                    logEntry.Status = "Fehler"
                    logEntry.EmailStatus = "Festschreibung fehlgeschlagen"
                    logEntry.EmailFehlerInfo = mailError
                    Return False
                End If
                pdfFestgeschrieben = True
            Else
                ' Duplikat erzeugen (XML, damit PDF-Blob-Logik identisch bleibt)
                Using fs = File.Create(xmlFilePath)
                    exporter.ExportFinalizedXmlDuplicate(fs, billType, rechnungsNummer, logEntry)
                End Using
                pdfFestgeschrieben = True
            End If

            ' 1a. Ermittle BlobId aus passender Blob-Tabelle (wie bei XML)
            If pdfFestgeschrieben Then
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

            ' 2. PDF-Blob prüfen und ggf. erzeugen (wie ExportSelectedPdf)
            If pdfFestgeschrieben Then
                pdfBytes = exporter.GetPdfRawFromBlob(rechnungsNummer, billType)
                If pdfBytes Is Nothing OrElse pdfBytes.Length = 0 Then
                    ' Kein PDF im Blob, neu erzeugen und speichern
                    Using pdfMemStream As New MemoryStream()
                        Dim reportForm = New ReportForm(_dbConnection, billType, New List(Of Integer) From {rechnungsNummer})
                        reportForm.SavePdf(pdfMemStream)
                        exporter.StorePdfToBlob(pdfMemStream.ToArray(), rechnungsNummer, billType)
                        pdfBytes = pdfMemStream.ToArray()
                    End Using
                End If
            End If

            If pdfBytes Is Nothing OrElse pdfBytes.Length = 0 Then
                _smtpLogger.Error($"Hybrid-PDF nicht gefunden oder leer: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}")
                logEntry.Status = "Fehler"
                logEntry.EmailStatus = "Hybrid-PDF nicht gefunden"
                logEntry.EmailFehlerInfo = mailError
                Return False
            End If

            ' 3. Versand nur wenn PDF festgeschrieben/dupliziert
            _smtpLogger.Info($"Bereite E-Mail-Versand vor: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}")
            Dim mail As New System.Net.Mail.MailMessage()
            mail.To.Add(empfaengerEmail)
            mail.Subject = mailSubject
            mail.Body = mailBody
            ' Dateiname für Anhang nach Schema von GetExportFilePath
            Dim pdfAttachmentName As String = Path.GetFileName(hybridPdfPath)
            mail.Attachments.Add(New System.Net.Mail.Attachment(New MemoryStream(pdfBytes), pdfAttachmentName, "application/pdf"))
            mail.From = New System.Net.Mail.MailAddress(smtpConfig.FromAddress)
            Dim smtp As New System.Net.Mail.SmtpClient()
            smtp.Host = smtpConfig.SmtpServer
            smtp.Port = smtpConfig.Port
            smtp.EnableSsl = True ' Optional: ggf. aus Parametern steuerbar machen
            If Not String.IsNullOrWhiteSpace(smtpConfig.SmtpUser) Then
                smtp.Credentials = New System.Net.NetworkCredential(smtpConfig.SmtpUser, smtpConfig.SmtpPassword)
            End If
            Try
                _smtpLogger.Info($"Sende E-Mail: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}, Betreff={mailSubject}")
                System.Net.ServicePointManager.ServerCertificateValidationCallback = Function(sender, certificate, chain, sslPolicyErrors) True
                smtp.Send(mail)
                mailSent = True
                mailStatus = "Erfolgreich"
                logEntry.EmailStatus = "Erfolgreich"
                logEntry.EmailFehlerInfo = mailError
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

            ' 4. Versand-Logging in DB (identisch wie in SendXRechnungXml)
            If mailSent Then
                _smtpLogger.Info($"Logge Versand in DB: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}, Status={mailStatus}")
                Dim now As DateTime = DateTime.Now
                Dim versandId As Long
                Dim sqlVersand As String = ""
                Dim sqlMailRaw As String = ""
                Dim kanal As String = "E-Mail"
                Dim betreff As String = mailSubject
                Dim nachrichtKurz As String = mailBody
                Dim attachmentHashes As String = ""
                Dim providerMessageId As String = mailRawId.ToString()
                Dim sentAt As DateTime = now
                Dim deliveredAt As Object = DBNull.Value
                Dim lastError As String = If(logEntry.EmailFehlerInfo, "")
                Dim detailsJson As String = ""
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

                Using cmd2 As New OleDb.OleDbCommand(sqlMailRaw, _dbConnection.cn)
                    cmd2.Parameters.AddWithValue("@VersandId", versandId)
                    cmd2.Parameters.AddWithValue("@MimeRaw", pdfBytes)
                    cmd2.ExecuteNonQuery()
                End Using
            End If

            logEntry.Status = "Erfolgreich"
            logEntry.EmailStatus = mailStatus
            logEntry.EmailFehlerInfo = mailError
            _smtpLogger.Info($"[END] Sende Hybrid-PDF: RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}, Status={mailStatus}")
            Return mailSent

        Catch ex As Exception
            logEntry.Status = "Fehler"
            logEntry.EmailStatus = "Fehler"
            logEntry.EmailFehlerInfo = ex.Message
            _smtpLogger.Error($"Unerwarteter Fehler (Hybrid-PDF): RechnungsNr={rechnungsNummer}, Empfaenger={empfaengerEmail}, Fehler={ex.Message}")
            Return False
        End Try
    End Function

End Class
