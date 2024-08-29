Imports System.IO
Imports ehfleet_classlibrary
Imports Stimulsoft.Report

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

        Dim dataTable = DataConnection.FillDataTable($"SELECT [Text] FROM [EHFleet].[dbo].[Parameter] where ParameterNr = {reportParameterId}")
        Dim reportPath = String.Empty

        If dataTable.Rows.Count > 0 Then
            reportPath = dataTable.Rows(0).Item(0).ToString()
        End If

        'Dim sql = GetSqlStatement(rechnungsArt, rechnungsNummer)

        'dataTable = DataConnection.FillDataTable(sql)

        StiViewerControl1.Report = StiReport.CreateNewReport
        If File.Exists(reportPath) Then
            StiViewerControl1.Report.Load(reportPath)
        End If

        Dim db = StiViewerControl1.Report.Dictionary.Databases(0)
        DirectCast(db, Stimulsoft.Report.Dictionary.StiSqlDatabase).ConnectionString = DataConnection.ConnectionString
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

    Private Function GetSqlStatement(rechnungsArt As RechnungsArt, rechnungsNummer As Integer) As String
        Select Case rechnungsArt
            Case RechnungsArt.Werkstatt
                Return $"select * from [abfr_wareport] where [ID] = {rechnungsNummer}"
            Case RechnungsArt.Tanken
                Return $"select * from [abfr_tankreport] where [RechnungsNr] = {rechnungsNummer}"
            Case RechnungsArt.Manuell
                Return $"select * from [abfr_mrvkreport] where [RechnungsNr] = {rechnungsNummer}"
        End Select

        Return String.Empty
    End Function
End Class
