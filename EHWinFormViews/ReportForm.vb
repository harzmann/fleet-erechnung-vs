Imports System.IO
Imports System.Windows.Forms
Imports AngleSharp.Css
Imports ehfleet_classlibrary
Imports Stimulsoft.Blockly.Blocks.Controls
Imports Stimulsoft.Report
Imports Stimulsoft.Report.Dictionary

Public Class ReportForm
    Private _dataConnection As General.Database

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

    Public Sub New(dbConnection As General.Database, rechnungsArt As RechnungsArt, rechnungsNummern As List(Of Integer))
        _dataConnection = dbConnection

        ' This call is required by the designer.
        InitializeComponent()

        Dim reportParameterId = GetReportParameterId(rechnungsArt)

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
        Dim queries = GetSqlStatements(rechnungsArt, rechnungsNummern)
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
