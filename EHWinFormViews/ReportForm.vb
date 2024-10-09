Imports System.IO
Imports System.Net.Security
Imports System.Runtime.Serialization
Imports System.Windows.Forms
Imports System.Xml.Serialization
Imports AngleSharp.Css
Imports ehfleet_classlibrary
Imports EHFleetXRechnung.Schemas
Imports QiHe.CodeLib
Imports Stimulsoft.Base
Imports Stimulsoft.Blockly.Blocks.Controls
Imports Stimulsoft.Controls.Wpf.ControlsV3
Imports Stimulsoft.Editor
Imports Stimulsoft.Report
Imports Stimulsoft.Report.Dictionary
Imports Stimulsoft.Report.Export
Imports Stimulsoft.Report.Helpers
Imports Stimulsoft.Report.Viewer
Imports Telerik.WinControls.UI
Imports Telerik.Windows.Documents.Flow.Model

Public Class ReportForm
    Private _dataConnection As General.Database

    Private _xmlExporter As XRechnungExporter

    Public Property DataConnection As General.Database
        Get
            Return _dataConnection
        End Get
        Set(value As General.Database)
            _dataConnection = value
        End Set
    End Property

    Shared Sub New()
        Stimulsoft.Base.StiLicense.LoadFromFile(Path.Combine(Path.GetDirectoryName(GetType(ReportForm).Assembly.Location), "license.key"))
    End Sub

    Public Sub New(dbConnection As General.Database, billType As RechnungsArt, rechnungsNummern As List(Of Integer))
        _dataConnection = dbConnection
        _xmlExporter = New XRechnungExporter(dbConnection)

        ' This call is required by the designer.
        InitializeComponent()

        Dim reportParameterId = GetReportParameterId(billType)

        Dim dataTable = DataConnection.FillDataTable($"SELECT [Text], [Aktiviert] FROM [EHFleet].[dbo].[Parameter] where ParameterNr = {reportParameterId}")
        Dim reportPath = String.Empty
        Dim isActive = False
        If dataTable.Rows.Count > 0 Then
            reportPath = dataTable.Rows(0).Item(0).ToString()
            isActive = dataTable.Rows(0).Item(1)
        End If

        If isActive AndAlso File.Exists(reportPath) Then
            StiViewerControl1.Report = StiReport.CreateNewReport
            StiViewerControl1.Report.Load(reportPath)
        End If

        If StiViewerControl1.Report Is Nothing OrElse StiViewerControl1.Report.Dictionary.Databases Is Nothing Then
            Throw New Exception("Bericht konnte nicht gefunden oder geladen werden")
            Return
        End If

        Dim relations = StiViewerControl1.Report.Dictionary.Relations.SaveToJsonObject(Stimulsoft.Base.StiJsonSaveMode.Report)

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

        AddHandler StiViewerControl1.ProcessExport,
            Sub(sender As Object, args As EventArgs)
                ' Check if the user is trying to export as PDF
                Dim service = TryCast(sender, StiPdfExportService)
                If service Is Nothing Then Return
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

                                    For Each rechnungsNummer In rechnungsNummern
                                        Using xmlStream = New MemoryStream
                                            _xmlExporter.CreateBillXml(xmlStream, billType, rechnungsNummer)
                                            settings.EmbeddedFiles.Add(New StiPdfEmbeddedFileData($"XRechnung_{rechnungsNummer}.xml", $"XRechnung Nr. {rechnungsNummer}", xmlStream.GetBuffer(), "application/xml"))
                                        End Using
                                    Next

                                    service.ExportTo(StiViewerControl1.Report, stream, settings)

                                End Using

                            Catch ex As Exception
                                MessageBox.Show("Fehler beim Speichern!")
                                Return
                            End Try

                            MessageBox.Show("Speichern erfolgreich!")
                        End If
                    End Sub

                    stiFormRunner.ShowDialog()


                End Using


                'service.Export(StiViewerControl1.Report)
                'If viewer.expo = StiExportFormat.Pdf Then
                '    ' Cancel the default export operation
                '    e.Cancel = True

                '    ' Get the report
                '    Dim report = viewer.Report

                '    ' Define custom PDF export settings (like embedding XML for XRechnung)
                '    Dim pdfSettings As New StiPdfExportSettings()
                '    pdfSettings.PdfACompliance = StiPdfACompliance.PdfA3B

                '    ' Assume xmlPath is the path to your XRechnung XML file
                '    pdfSettings.EmbeddedFiles.Add(New StiPdfEmbeddedFile("XRechnung.xml", xmlPath, True))

                '    ' Manually export the report with the custom settings
                '    report.ExportDocument(StiExportFormat.Pdf, "output.pdf", pdfSettings)
                'End If
            End Sub

        StiViewerControl1.Report.Render()
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
