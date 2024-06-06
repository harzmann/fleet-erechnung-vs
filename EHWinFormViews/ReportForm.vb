Imports ehfleet_classlibrary

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

    Public Sub New(dbConnection As General.Database)
        _dataConnection = dbConnection

        ' This call is required by the designer.
        InitializeComponent()

        Dim dataTable = DataConnection.FillDataTable("SELECT * FROM Kunden")
        AddHandler Me.StiViewerControl1.RefreshControlState,
            Sub(sender As Object, e As EventArgs)
                StiViewerControl1.Report?.RegData(dataTable)
            End Sub

    End Sub
End Class