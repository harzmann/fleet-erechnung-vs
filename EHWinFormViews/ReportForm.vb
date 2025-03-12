Imports System.CodeDom
Imports System.IO
Imports System.Reflection
Imports System.Runtime.Loader
Imports System.Windows.Forms
Imports ehfleet_classlibrary
Imports log4net
Imports Stimulsoft.Base
Imports Stimulsoft.Base.Licenses
Imports Stimulsoft.Report
Imports Stimulsoft.Report.Dictionary
Imports Stimulsoft.Report.Export
Imports Stimulsoft.Report.Helpers
Imports Telerik.WinControls
Imports Telerik.WinControls.UI

Public Class ReportForm
    Private _dataConnection As General.Database

    Private ReadOnly _xmlExporter As XRechnungExporter

    Private ReadOnly _logger As ILog

    Private ReadOnly _rechnungsNummern As List(Of Integer)

    Private ReadOnly _billType As RechnungsArt

    Private _saveSinglePdf As Boolean

    Public Property DataConnection As General.Database
        Get
            Return _dataConnection
        End Get
        Set(value As General.Database)
            _dataConnection = value
        End Set
    End Property

    Shared Sub New()
        'StiLicense.LoadFromFile(Path.Combine(Path.GetDirectoryName(GetType(ReportForm).Assembly.Location), "license.key"))

    End Sub


    Private Shared Sub InstallLicense(key As String)
        Try
            StiLicense.Key = key
        Catch ex As Exception
            Dim licenseKey = StiLicenseKey.Get(key)
            Dim licensingType = GetType(StiLicense)
            Dim keyProperty = licensingType.GetProperty("LicenseKey", System.Reflection.BindingFlags.NonPublic Xor System.Reflection.BindingFlags.Static)
            Dim keyField = licensingType.GetField("key", System.Reflection.BindingFlags.NonPublic Xor System.Reflection.BindingFlags.Static)
            If keyProperty Is Nothing Or keyField Is Nothing Then Return

            keyProperty.SetValue(Nothing, licenseKey)
            keyField.SetValue(Nothing, key)
        End Try

    End Sub

    Public Sub New(dbConnection As General.Database, billType As RechnungsArt, rechnungsNummern As List(Of Integer))
        _dataConnection = dbConnection
        _logger = LogManager.GetLogger(Me.GetType())
        _logger.Debug($"Instantiating {NameOf(ReportForm)}")
        _xmlExporter = New XRechnungExporter(dbConnection)
        _rechnungsNummern = rechnungsNummern
        _billType = billType

        InstallLicense("6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHk3LB9R6pWAk1QqUjOkeHv6OOlD/P9VbyZIvUMUf5DgXWtfTph4Mq1GsBBQJeFHAQDJ1Ji+ynEb8F5xLdDG8MOLdqZq+K3QgObIxz/RbC8Oh5REXsrIKWsaR6OK2DgwEnxih9DTLIPb+4wJCQFDY0bmb/fK14QgER+UHcTMKXYdxq/cRgZl7dOXtotRW0+K5K5mFnB33yWjmKvjvarO6+qe2J3k68rivqZy49c8KLZkzw/26buHe4W40ewCeS4xwVLzud5b9cSKiScLlJdTF2+xfAVItCN8HrEIEVmNAzHZOhrivhdshYKMznBAfjPOUETumlFsx32hbXe8riCArJt1ax25A4Fx0A01tEAgmdKbqA3YYqOWck5aGHEmNkiTbwtxMNnlOtbS8I4ywC/hDGrbC44d/N/g/tB1VskRzsP+kbw3kn7DKdW6VuIqWAyCSnoe6vuEdCQCf23Mn89ZojrXF8wsN667aB2wyB8Zefvp4U089UT/GlUrouhDXQcYHkwy+OC98JRW2gr+fze2N4t0mexeemkdqpW5g13tiREz9y+IBfFwHlekZo1OzcWYB92+qs5es415FXPXko8hYTHuZ1UxHi/TjQPRvKXgYbhdsqRM/npxed7pftyRS9KuGUhUG8aACWyceTGRCGnZTjLq")

        ' This call is required by the designer.
        InitializeComponent()

        Try
            Dim reportParameterId = GetReportParameterId(billType)
            Dim dataTable = DataConnection.FillDataTable($"SELECT [Text], [Aktiviert] FROM [EHFleet].[dbo].[Parameter] where ParameterNr = {reportParameterId}")
            Dim reportPath = String.Empty
            Dim isActive = False
            If dataTable.Rows.Count > 0 Then
                reportPath = dataTable.Rows(0).Item(0).ToString()
                isActive = dataTable.Rows(0).Item(1)
            Else
                _logger.Error($"Parameter {reportParameterId} could not be found in DB")
                MessageBox.Show($"Parameter {reportParameterId} nicht gefunden!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            If isActive AndAlso File.Exists(reportPath) Then
                StiViewerControl1.Report = StiReport.CreateNewReport
                StiViewerControl1.Report.Load(reportPath)
            Else
                _logger.Error($"Parameter {reportParameterId} not active or report file cannot be found ({reportPath})")
                Throw New Exception($"Report {reportPath} nicht gefunden")
            End If

            If StiViewerControl1.Report Is Nothing OrElse StiViewerControl1.Report.Dictionary.Databases Is Nothing Then
                _logger.Error($"Failed to open or load report {reportPath}")
                MessageBox.Show($"Bericht konnte nicht geladen werden ({reportPath})", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim relations = StiViewerControl1.Report.Dictionary.Relations.SaveToJsonObject(StiJsonSaveMode.Report)

            StiViewerControl1.Report.Dictionary.Databases.Clear()
            StiViewerControl1.Report.Dictionary.Databases.Add(New StiOleDbDatabase("OLE DB", DataConnection.ConnectionString))

            StiViewerControl1.Report.Dictionary.DataSources.Clear()
            Dim queries = GetSqlStatements(billType, rechnungsNummern)
            StiViewerControl1.Report.Dictionary.DataSources.AddRange(queries.Select(Function(statement)
                                                                                        Dim ds = (New StiOleDbSource("OLE DB", statement.Key, statement.Key, statement.Value))
                                                                                        ds.Dictionary = StiViewerControl1.Report.Dictionary
                                                                                        Return ds
                                                                                    End Function).ToArray)

            If relations IsNot Nothing Then
                StiViewerControl1.Report.Dictionary.Relations.LoadFromJsonObject(relations)
            End If

            StiViewerControl1.Report.Dictionary.RegRelations()
            StiViewerControl1.Report.Dictionary.SynchronizeRelations()
            StiViewerControl1.Report.Dictionary.Synchronize()

            AddHandler StiViewerControl1.ProcessExport, AddressOf HandleProcesExport

            StiViewerControl1.Report.Render()
        Catch ex As Exception
            _logger.Error($"Exception during {NameOf(ReportForm)} constructor", ex)
            Throw
        Finally
            _logger.Debug($"Leaving {NameOf(ReportForm)} constructor")
        End Try
    End Sub

    Public Sub SavePdf()
        _saveSinglePdf = True
        Dim pdfExportService = StiOptions.Services.Exports.FirstOrDefault(Function(s) s.ServiceName.ToLower().Contains("pdf"))
        StiViewerControl1.InvokeProcessExport(pdfExportService)
    End Sub

    Private Sub HandleProcesExport(sender As Object, args As EventArgs)
        ' Check if the user is trying to export as PDF
        Dim service = TryCast(sender, StiPdfExportService)
        If service Is Nothing Then
            RemoveHandler StiViewerControl1.ProcessExport, AddressOf HandleProcesExport
            StiViewerControl1.InvokeProcessExport(sender)
            AddHandler StiViewerControl1.ProcessExport, AddressOf HandleProcesExport
            Return
        End If

        If Not _saveSinglePdf Then
            Using stiFormRunner = StiGuiOptions.GetExportFormRunner("StiPdfExportSetupForm", StiGuiMode.Gdi, Me)
                AddHandler stiFormRunner.Complete,
                    Sub(form As IStiFormRunner, e As StiShowDialogCompleteEvetArgs)
                        Dim saveDialog = New SaveFileDialog
                        saveDialog.AddExtension = True
                        saveDialog.Filter = "pdf-Datei|*.pdf"
                        saveDialog.Title = "Bitte Speicherort auswählen."
                        saveDialog.DefaultExt = "pdf"
                        Dim result = saveDialog.ShowDialog()
                        If result = DialogResult.OK Then
                            Try
                                _logger.Debug($"Attempting to export bill pdf with embedded xml data to {saveDialog.FileName}")
                                Using stream = New FileStream(saveDialog.FileName, FileMode.Create)
                                    Dim settings = New StiPdfExportSettings()
                                    settings.PageRange = form("PagesRange")
                                    settings.ImageQuality = form("ImageQuality")
                                    settings.ImageCompressionMethod = form("ImageCompressionMethod")
                                    settings.ImageResolution = form("Resolution")
                                    settings.EmbeddedFonts = form("EmbeddedFonts") AndAlso form("EmbeddedFontsEnabled")
                                    settings.ExportRtfTextAsImage = form("ExportRtfTextAsImage")
                                    settings.PasswordInputUser = form("UserPassword")
                                    settings.PasswordInputOwner = form("OwnerPassword")
                                    settings.UserAccessPrivileges = form("UserAccessPrivileges")
                                    settings.KeyLength = form("EncryptionKeyLength")
                                    settings.GetCertificateFromCryptoUI = form("GetCertificateFromCryptoUI")
                                    settings.UseDigitalSignature = form("UseDigitalSignature")
                                    settings.SubjectNameString = form("SubjectNameString")
                                    settings.PdfComplianceMode = Export.StiPdfComplianceMode.A3
                                    settings.ImageFormat = form("ImageFormat")
                                    settings.DitheringType = form("MonochromeDitheringType")
                                    settings.AllowEditable = form("AllowEditable")
                                    settings.ImageResolutionMode = form("ImageResolutionMode")
                                    settings.CertificateThumbprint = form("CertificateThumbprint")

                                    For Each rechnungsNummer In _rechnungsNummern
                                        Using xmlStream = New MemoryStream
                                            _xmlExporter.CreateBillXml(xmlStream, _billType, rechnungsNummer)
                                            Dim billdate = _rechnungsNummern(rechnungsNummer)
                                            Dim formattedBillNumber = _xmlExporter.GetFormattedBillNumber(_billType, rechnungsNummer)
                                            Dim xmlFileName = $"{formattedBillNumber}_{billdate.ToString("yyyyMMdd_HHmmss")}"
                                            settings.EmbeddedFiles.Add(New StiPdfEmbeddedFileData(xmlFileName, $"XRechnung Nr. {formattedBillNumber}", xmlStream.GetBuffer(), "application/xml"))
                                        End Using
                                    Next

                                    service.ExportTo(StiViewerControl1.Report, stream, settings)

                                End Using

                            Catch ex As Exception
                                _logger.Error($"Exception while trying to save report to pdf file", ex)
                                MessageBox.Show("Fehler beim Speichern!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return
                            Finally
                                _logger.Debug("Finished exporting bill pdf.")
                            End Try

                            MessageBox.Show("Speichern erfolgreich!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                    End Sub
                stiFormRunner.ShowDialog()
            End Using
        Else
            Try
                For Each billNumber In _rechnungsNummern
                    Dim exportFilePath = _xmlExporter.GetExportFilePath(_billType, billNumber, "pdf")

                    _logger.Debug($"Attempting to export bill pdf with embedded xml data to {exportFilePath}")
                    Using stream = File.Create(exportFilePath)
                        Dim settings = New StiPdfExportSettings()
                        settings.PageRange = StiPagesRange.All
                        settings.ImageQuality = 75
                        settings.ImageCompressionMethod = StiPdfImageCompressionMethod.Jpeg
                        settings.ImageResolution = 100
                        'settings.EmbeddedFonts = Form("EmbeddedFonts") AndAlso Form("EmbeddedFontsEnabled")
                        settings.ExportRtfTextAsImage = False
                        'settings.PasswordInputUser = Form("UserPassword")
                        'settings.PasswordInputOwner = Form("OwnerPassword")
                        'settings.UserAccessPrivileges = Form("UserAccessPrivileges")
                        'settings.KeyLength = Form("EncryptionKeyLength")
                        'settings.GetCertificateFromCryptoUI = Form("GetCertificateFromCryptoUI")
                        'settings.UseDigitalSignature = Form("UseDigitalSignature")
                        'settings.SubjectNameString = Form("SubjectNameString")
                        settings.PdfComplianceMode = Export.StiPdfComplianceMode.A3
                        settings.ImageFormat = StiImageFormat.Color
                        settings.DitheringType = StiMonochromeDitheringType.None
                        settings.AllowEditable = False
                        settings.ImageResolutionMode = StiImageResolutionMode.Auto
                        'settings.CertificateThumbprint = Form("CertificateThumbprint")

                        Using xmlStream = New MemoryStream
                            _xmlExporter.CreateBillXml(xmlStream, _billType, billNumber)
                            Dim formattedBillNumber = _xmlExporter.GetFormattedBillNumber(_billType, billNumber)
                            Dim xmlFileName = _xmlExporter.GetExportFilePath(_billType, billNumber, "xml")
                            settings.EmbeddedFiles.Add(New StiPdfEmbeddedFileData(xmlFileName, $"XRechnung Nr. {formattedBillNumber}", xmlStream.GetBuffer(), "application/xml"))
                        End Using

                        service.ExportTo(StiViewerControl1.Report, stream, settings)
                    End Using
                Next
                MessageBox.Show("Speichern erfolgreich!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                _logger.Error($"Exception while trying to save report to pdf file", ex)
                MessageBox.Show("Fehler beim Speichern!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                _logger.Debug("Finished exporting bill pdf.")
                _saveSinglePdf = False
            End Try
        End If

    End Sub

    Private Function GetReportParameterId(rechnungsArt As RechnungsArt) As String
        Select Case rechnungsArt
            Case RechnungsArt.Werkstatt
                Return "210"
            Case RechnungsArt.Tanken
                Return "1210"
            Case RechnungsArt.Manuell
                Return "2210"
        End Select
        Return String.Empty
    End Function

    Private Function GetSqlStatements(rechnungsArt As RechnungsArt, rechnungsNummern As List(Of Integer)) As Dictionary(Of String, String)
        Dim inClausePlaceholders As String = String.Join(",", rechnungsNummern.Select(Function(v) $"{v}").ToArray())
        Select Case rechnungsArt
            Case RechnungsArt.Werkstatt
                Return New Dictionary(Of String, String) From
                    {
                        {"abfr_wavkreport", $"select * from abfr_wavkreport where RechnungsNr IN ({inClausePlaceholders})"},
                        {"abfr_subReport_Artikel", $"select * from abfr_wavkabrdetail WHERE RechnungsNr IN ({inClausePlaceholders}) AND ArtikelNr is not Null AND PersonalID is Null ORDER BY RechnungsDetailNr"},
                        {"abfr_subReport_Lohn", $"select * from abfr_wavkabrdetail WHERE RechnungsNr IN ({inClausePlaceholders}) AND PersonalID Is Not Null And ArtikelNr Is Null ORDER By RechnungsDetailNr"},
                        {"abfr_subReport_Sonstige", $"select * from abfr_wavkabrdetail WHERE RechnungsNr IN ({inClausePlaceholders}) AND PersonalID is Null AND ArtikelNr is Null ORDER BY RechnungsDetailNr"},
                        {"abfr_subReport_Kategorien", $"SELECT * FROM abfr_wavksummekat WHERE RechnungsNr IN ({inClausePlaceholders})"},
                        {"abfr_subReport_Mwst", $"SELECT * FROM abfr_wavkabrmwst WHERE RechnungsNr IN ({inClausePlaceholders})"}
                    }
            Case RechnungsArt.Tanken
                Return New Dictionary(Of String, String) From
                    {
                        {"abfr_tankreport", $"select * from abfr_tankreport WHERE RechnungsNr IN ({inClausePlaceholders})"},
                        {"abfr_subReport_Tankungen", $"SELECT * FROM abfr_tankabrdetail WHERE RechnungsNr IN ({inClausePlaceholders}) ORDER BY RechnungsDetailNr"},
                        {"abfr_subReport_Sonstige", $"SELECT * FROM TankabrechnungDetail WHERE RechnungsNr IN ({inClausePlaceholders}) AND SpritID is Null AND ArtikelNr is Null ORDER BY RechnungsDetailNr"},
                        {"abfr_subReport_Produktarten", $"SELECT * FROM abfr_tasummeprod WHERE RechnungsNr IN ({inClausePlaceholders})"},
                        {"abfr_subReport_Mwst", $"SELECT * FROM abfr_tankabrmwst WHERE RechnungsNr IN ({inClausePlaceholders})"}
                    }
            Case RechnungsArt.Manuell
                Return New Dictionary(Of String, String) From
                    {
                        {"abfr_mrvkreport", $"select * from abfr_mrvkreport WHERE RechnungsNr IN ({inClausePlaceholders})"},
                        {"abfr_subReport_Artikel", $"select * from abfr_mrvkabrdetail WHERE RechnungsNr IN ({inClausePlaceholders}) AND ArtikelNr is not Null AND PersonalID is Null ORDER BY RechnungsDetailNr"},
                        {"abfr_subReport_Lohn", $"select * from abfr_mrvkabrdetail WHERE RechnungsNr IN ({inClausePlaceholders}) AND PersonalID is not Null AND ArtikelNr is Null ORDER BY RechnungsDetailNr"},
                        {"abfr_subReport_Sonstige", $"SELECT * FROM abfr_mrvkabrdetail WHERE RechnungsNr IN ({inClausePlaceholders}) AND PersonalID is Null AND ArtikelNr is Null ORDER BY RechnungsDetailNr"},
                        {"abfr_subReport_Kategorien", $"SELECT * FROM abfr_mrvksummekat WHERE RechnungsNr IN ({inClausePlaceholders})"},
                        {"abfr_subReport_Mwst", $"SELECT * FROM abfr_mrvkabrmwst WHERE RechnungsNr IN ({inClausePlaceholders})"}
                    }
        End Select

        Return New Dictionary(Of String, String)
    End Function

End Class
