Imports System.IO
Imports System.Windows.Forms
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

    Public Sub New(dbConnection As General.Database, rechnungsArt As RechnungsArt, rechnungsNummer As Integer)
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

        StiViewerControl1.Report = StiReport.CreateNewReport
        If isActive AndAlso File.Exists(reportPath) Then
            StiViewerControl1.Report.Load(reportPath)
        End If

        If StiViewerControl1.Report Is Nothing OrElse StiViewerControl1.Report.Dictionary.Databases Is Nothing Then
            MessageBox.Show("Bericht konnte nicht gefunden oder gelade werden")
            Return
        End If

        StiViewerControl1.Report.Dictionary.Databases.Clear()
        StiViewerControl1.Report.Dictionary.Databases.Add(New Dictionary.StiOleDbDatabase("OLE DB", DataConnection.ConnectionString))

        StiViewerControl1.Report.Dictionary.DataSources.Clear()
        Dim queries = GetSqlStatements(rechnungsArt, rechnungsNummer)
        StiViewerControl1.Report.Dictionary.DataSources.AddRange(queries.Select(Function(statement)
                                                                                    Dim ds = (New StiOleDbSource("OLE DB", statement.Key, statement.Key, statement.Value))
                                                                                    ds.Dictionary = StiViewerControl1.Report.Dictionary
                                                                                    Return ds
                                                                                End Function).ToArray)
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


    Private Function GetSqlStatements(rechnungsArt As RechnungsArt, rechnungsNummer As Integer) As Dictionary(Of String, String)
        Select Case rechnungsArt
            Case RechnungsArt.Werkstatt
                Return New Dictionary(Of String, String) From
                    {
                        {"abfr_wavkreport", $"select * from abfr_wavkreport where RechnungsNr = {rechnungsNummer}"},
                        {"abfr_subReport_Artikel", $"select * from abfr_wavkabrdetail WHERE RechnungsNr = {rechnungsNummer} AND ArtikelNr is not Null AND PersonalID is Null ORDER BY RechnungsDetailNr"},
                        {"abfr_subReport_Teile", $"select * from abfr_wavkabrdetail WHERE RechnungsNr = {rechnungsNummer} AND ArtikelNr is not Null AND PersonalID is Null ORDER BY RechnungsDetailNr"}
                    }
            Case RechnungsArt.Tanken
                Return New Dictionary(Of String, String) From
                    {
                        {"abfr_tankreport", $"select * from abfr_tankreport where RechnungsNr = {rechnungsNummer}"},
                        {"abfr_subReport_Artikel", $"select * from abfr_tankabrdetail WHERE RechnungsNr = {rechnungsNummer} ORDER BY Tankdatum"}
                    }
            Case RechnungsArt.Manuell
                Return New Dictionary(Of String, String) From
                    {
                        {"abfr_mrvkreport", $"select * from abfr_mrvkreport where RechnungsNr = {rechnungsNummer}"},
                        {"abfr_subReport_Artikel", $"select * from abfr_mrvkabrdetail where RechnungsNr = {rechnungsNummer} AND ArtikelNr is not Null AND PersonalID is Null ORDER BY ArtikelNr"}
                    }
        End Select

        Return New Dictionary(Of String, String)
    End Function

End Class
