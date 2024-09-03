Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports ehfleet_classlibrary
Imports EHWinFormViews.GermanRadGridViewLocalization
Imports Telerik.WinControls.UI
Imports Telerik.WinControls.UI.Localization

Public Class RechnungsUebersicht

    Private ReadOnly _dbConnection As General.Database

    Private _rechnungsArt As RechnungsArt

    Public Sub New(dbConnection As General.Database)
        'ScreenScaling.SetProcessDpiAwareness(_Process_DPI_Awareness.Process_DPI_Unaware)
        _dbConnection = dbConnection
        RadGridLocalizationProvider.CurrentProvider = New GermanRadGridLocalizationProvider
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Sub WerkstattRechnungButton_CheckedChanged(sender As Object, e As EventArgs) Handles WerkstattRechnungButton.CheckedChanged
        _rechnungsArt = RechnungsArt.Werkstatt
        RefreshGrid()
    End Sub

    Private Sub TankabrechnungButton_CheckedChanged(sender As Object, e As EventArgs) Handles TankabrechnungButton.CheckedChanged
        _rechnungsArt = RechnungsArt.Tanken
        RefreshGrid()
    End Sub

    Private Sub ManuelleRechnungButton_CheckedChanged(sender As Object, e As EventArgs) Handles ManuelleRechnungButton.CheckedChanged
        _rechnungsArt = RechnungsArt.Manuell
        RefreshGrid()
    End Sub

    Private Function RefreshGrid()
        DataGridView1.Columns.Clear()
        Dim sql = GetSqlStatement(_rechnungsArt)
        Dim dataTable = _dbConnection.FillDataTable(sql)

        Dim dataTableBindingSource = New Windows.Forms.BindingSource()
        dataTableBindingSource.DataSource = dataTable
        DataGridView1.DataSource = dataTableBindingSource

        Dim buttonColumn As New GridViewCommandColumn()
        buttonColumn.Name = "Aktion"
        buttonColumn.HeaderText = "Aktion"
        buttonColumn.DefaultText = "X Rechnung"
        buttonColumn.UseDefaultText = True

        ' Add the button column to the DataGridView
        DataGridView1.Columns.Add(buttonColumn)
    End Function

    Private Sub DataGridView1_CellClick(sender As Object, e As GridViewCellEventArgs) Handles DataGridView1.CommandCellClick
        ' Check if the click is on a button column
        If e.ColumnIndex = DataGridView1.Columns("Aktion").Index AndAlso e.RowIndex >= 0 Then
            ' Perform the action you want here
            Dim dataSource = CType(DataGridView1.DataSource, BindingSource)
            Dim dataTable = CType(dataSource.DataSource, DataTable)
            Dim row = dataTable.Rows(e.RowIndex)
            Dim rechnungsNummer = Convert.ToInt32(row.Item(0))
            Dim reportForm = New ReportForm(_dbConnection, _rechnungsArt, rechnungsNummer)
            reportForm.ShowDialog()
        End If
    End Sub


    Private Function GetSqlStatement(rechnungsArt As RechnungsArt) As String
        Select Case rechnungsArt
            Case RechnungsArt.Werkstatt
                Dim columnMapping As New Dictionary(Of String, String) From
                    {
                        {"RechnungsNr", "'RG-Nr.'"},
                        {"Rechnungsdatum", "'RG-Datum'"},
                        {"WANr", "'WA-Kurzbezeichnung'"},
                        {"Datum", "'WA-Datum'"},
                        {"Auftragsart", "'WA-Art'"},
                        {"Belegart", "Belegart"},
                        {"KundenNr", "'KD-Nr.'"},
                        {"DebitorNr", "'Debitor-Nr.'"},
                        {"Firma", "Firma"},
                        {"Summe", "'Summe netto'"},
                        {"Exportiert", "X"},
                        {"Gebucht", "B"},
                        {"Storno", "S"}
                    }

                Return $"select {String.Join(",", columnMapping.Select(Function(map) $"{map.Key} as {map.Value}"))} from [abfr_wavkliste]"
            Case RechnungsArt.Tanken
                Dim columnMapping As New Dictionary(Of String, String) From
                    {
                        {"RechnungsNr", "'RG-Nr.'"},
                        {"Rechnungsdatum", "'RG-Datum'"},
                        {"Rechnungstext", "Anmerkungen"},
                        {"Belegart", "Belegart"},
                        {"KundenNr", "'KD-Nr.'"},
                        {"DebitorNr", "'Debitor-Nr.'"},
                        {"Firma", "Firma"},
                        {"Summe", "'Summe netto'"},
                        {"Exportiert", "X"},
                        {"Gebucht", "B"},
                        {"Storno", "S"}
                    }

                Return $"select {String.Join(",", columnMapping.Select(Function(map) $"{map.Key} as {map.Value}"))} from [abfr_tavkliste]"
            Case RechnungsArt.Manuell
                Dim columnMapping As New Dictionary(Of String, String) From
                    {
                        {"RechnungsNr", "'RG-Nr.'"},
                        {"Rechnungsdatum", "'RG-Datum'"},
                        {"Rechnungstext", "Anmerkungen"},
                        {"Belegart", "Belegart"},
                        {"KundenNr", "'KD-Nr.'"},
                        {"DebitorNr", "'Debitor-Nr.'"},
                        {"Firma", "Firma"},
                        {"Summe", "'Summe netto'"},
                        {"Exportiert", "X"},
                        {"Gebucht", "B"},
                        {"Storno", "S"}
                    }

                Return $"select {String.Join(",", columnMapping.Select(Function(map) $"{map.Key} as {map.Value}"))} from [abfr_mrvkliste]"
        End Select

        Return String.Empty
    End Function

End Class