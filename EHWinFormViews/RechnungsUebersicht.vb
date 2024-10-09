Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports ehfleet_classlibrary
Imports EHWinFormViews.GermanRadGridViewLocalization
Imports ExcelLibrary.BinaryFileFormat
Imports Telerik.WinControls.UI
Imports Telerik.WinControls.UI.Localization

Public Class RechnungsUebersicht

    Private ReadOnly _dbConnection As General.Database

    Private ReadOnly _xmlExporter As XRechnungExporter

    Private _rechnungsArt As RechnungsArt

    Public Sub New(dbConnection As General.Database)
        'ScreenScaling.SetProcessDpiAwareness(_Process_DPI_Awareness.Process_DPI_Unaware)
        _dbConnection = dbConnection
        _xmlExporter = New XRechnungExporter(dbConnection)
        RadGridLocalizationProvider.CurrentProvider = New GermanRadGridLocalizationProvider
        ' This call is required by the designer.
        InitializeComponent()
        DataGridView1.SelectionMode = GridViewSelectionMode.FullRowSelect
        DataGridView1.MultiSelect = True

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
        buttonColumn.Name = "Aktion Pdf"
        buttonColumn.HeaderText = "Aktion Pdf"
        buttonColumn.DefaultText = "X Rechnung Pdf"
        buttonColumn.UseDefaultText = True
        DataGridView1.Columns.Add(buttonColumn)

        buttonColumn = New GridViewCommandColumn()
        buttonColumn.Name = "Aktion Xml"
        buttonColumn.HeaderText = "Aktion Xml"
        buttonColumn.DefaultText = "X Rechnung Xml"
        buttonColumn.UseDefaultText = True
        DataGridView1.Columns.Add(buttonColumn)
    End Function

    Private Sub DataGridView1_CellClick(sender As Object, e As GridViewCellEventArgs) Handles DataGridView1.CommandCellClick
        ' Check if the click is on a button column
        If e.RowIndex < 0 Then Return

        Dim dataSource = CType(DataGridView1.DataSource, BindingSource)
        Dim dataTable = CType(dataSource.DataSource, DataTable)
        Dim row = dataTable.Rows(e.RowIndex)
        Dim rechnungsNummer = Convert.ToInt32(row.Item(0))

        If e.ColumnIndex = DataGridView1.Columns("Aktion Pdf").Index Then
            ' Perform the action you want here

            Dim reportForm = New ReportForm(_dbConnection, _rechnungsArt, New List(Of Integer) From {rechnungsNummer})
            reportForm.ShowDialog()
        ElseIf e.ColumnIndex = DataGridView1.Columns("Aktion Xml").Index Then
            Dim fileDialog = New SaveFileDialog
            fileDialog.Title = "Bitte XRechnung Speicherort auswählen."
            fileDialog.Filter = "XRechnung|*.xml"
            fileDialog.AddExtension = True
            fileDialog.DefaultExt = "xml"
            Dim result = fileDialog.ShowDialog()
            If result <> DialogResult.OK Then Return
            Using fileStream = System.IO.File.OpenWrite(fileDialog.FileName)
                _xmlExporter.CreateBillXml(fileStream, _rechnungsArt, rechnungsNummer)
            End Using

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
                        {"Anmerkungen", "Anmerkungen"},
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
                        {"Anmerkungen", "Anmerkungen"},
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

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim dataSource = CType(DataGridView1.DataSource, BindingSource)
        Dim dataTable = CType(dataSource.DataSource, DataTable)

        Dim rechnungsNummern = DataGridView1.CurrentView.VisualRows.Where(Function(r) r.GetType() = GetType(GridDataRowElement)).Select(
        Function(rowInfo)
            Dim dataRow = dataTable.Rows(rowInfo.RowInfo.Index)
            Return Convert.ToInt32(dataRow.Item(0))
        End Function).ToList()

        Dim reportForm = New ReportForm(_dbConnection, _rechnungsArt, rechnungsNummern)
        reportForm.ShowDialog()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim dataSource = CType(DataGridView1.DataSource, BindingSource)
        Dim dataTable = CType(dataSource.DataSource, DataTable)
        Dim reportForm = New ReportForm(_dbConnection, _rechnungsArt, DataGridView1.SelectedRows.Select(Function(row)
                                                                                                            Dim dataRow = dataTable.Rows(row.Index)
                                                                                                            Return Convert.ToInt32(dataRow.Item(0))
                                                                                                        End Function).ToList())
        reportForm.ShowDialog()
    End Sub
End Class