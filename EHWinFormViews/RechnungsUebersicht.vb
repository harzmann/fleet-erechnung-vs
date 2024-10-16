Imports System.IO
Imports System.Windows.Forms
Imports ehfleet_classlibrary
Imports EHFleetXRechnung.Viewer.GermanRadGridViewLocalization
Imports log4net
Imports Stimulsoft.Report.Export
Imports Telerik.WinControls
Imports Telerik.WinControls.UI
Imports Telerik.WinControls.UI.Localization

Public Class RechnungsUebersicht

    Private ReadOnly _dbConnection As General.Database

    Private ReadOnly _xmlExporter As XRechnungExporter

    Private ReadOnly _logger As ILog

    Private _rechnungsArt As RechnungsArt

    Public Sub New(dbConnection As General.Database)
        _dbConnection = dbConnection
        _logger = LogManager.GetLogger(Me.GetType())
        _logger.Debug($"Instantiating {NameOf(RechnungsUebersicht)}")
        _xmlExporter = New XRechnungExporter(dbConnection)
        RadGridLocalizationProvider.CurrentProvider = New GermanRadGridLocalizationProvider
        ' This call is required by the designer.

        InitializeComponent()
        InitializeTelerikControls()
        WerkstattRechnungButton.CheckState = CheckState.Checked
        AddHandler DataGridView1.CellFormatting, Sub(sender, e)
                                                     If TypeOf e.Column Is GridViewCommandColumn AndAlso e.RowIndex >= 0 Then
                                                         Dim commandCell As GridCommandCellElement = TryCast(e.CellElement, GridCommandCellElement)
                                                         If commandCell IsNot Nothing AndAlso commandCell.Children.Count > 0 Then
                                                             Dim buttonElement As RadButtonElement = TryCast(commandCell.Children(0), RadButtonElement)
                                                             If buttonElement IsNot Nothing Then
                                                                 commandCell.FitToSizeMode = RadFitToSizeMode.FitToParentBounds
                                                                 commandCell.AutoSize = True
                                                                 commandCell.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter
                                                                 commandCell.Alignment = System.Drawing.ContentAlignment.MiddleCenter
                                                                 buttonElement.AutoSize = True
                                                                 buttonElement.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter
                                                                 buttonElement.Alignment = System.Drawing.ContentAlignment.MiddleCenter
                                                                 buttonElement.SvgImage = GetSvgImage(e.Column.Name, New System.Drawing.Size(30, 30))
                                                             End If
                                                         End If
                                                     End If
                                                 End Sub

        DataGridView1.SelectionMode = GridViewSelectionMode.FullRowSelect
        DataGridView1.MultiSelect = True
        DataGridView1.VirtualMode = False
        RefreshGrid()

        _logger.Debug($"Leaving {NameOf(RechnungsUebersicht)} constructor")
    End Sub

    Private Sub InitializeTelerikControls()
        WerkstattRechnungButton.SvgImage = GetSvgImage("werkstatt", New System.Drawing.Size(35, 35))
        TankabrechnungButton.SvgImage = GetSvgImage("tanken", New System.Drawing.Size(35, 35))
        ManuelleRechnungButton.SvgImage = GetSvgImage("manuell", New System.Drawing.Size(35, 35))
        SelectedXmlExportButton.SvgImage = GetSvgImage("xml", New System.Drawing.Size(25, 25))
        AllXmlExportButton.SvgImage = GetSvgImage("xml", New System.Drawing.Size(25, 25))
        SelectedPdfExportButton.SvgImage = GetSvgImage("pdf", New System.Drawing.Size(25, 25))
        AllPdfExportButton.SvgImage = GetSvgImage("pdf", New System.Drawing.Size(25, 25))
    End Sub

    Private Function GetSvgImage(name As String, desiredSize As System.Drawing.Size) As RadSvgImage
        Dim currentNameSpace = Me.GetType.Namespace
        Dim currentAssembly = Me.GetType.Assembly
        Dim resources = currentAssembly.GetManifestResourceNames.ToHashSet
        Select Case name.ToLower()
            Case "pdf"
                name = $"{currentNameSpace}.pdf.svg"
            Case "bericht"
                name = $"{currentNameSpace}.approval.svg"
            Case "xml"
                name = $"{currentNameSpace}.file.svg"
            Case "validator"
                name = $"{currentNameSpace}.trust.svg"
            Case "werkstatt"
                name = $"{currentNameSpace}.garage.svg"
            Case "tanken"
                name = $"{currentNameSpace}.gasoline.svg"
            Case "manuell"
                name = $"{currentNameSpace}.car-repair.svg"
        End Select

        If Not resources.Contains(name) Then Return Nothing

        Using stream = currentAssembly.GetManifestResourceStream(name)
            Dim image = RadSvgImage.FromStream(stream)
            image.Size = desiredSize
            Return image
        End Using
    End Function

    Private Sub WerkstattRechnungButton_CheckedChanged(sender As Object, e As EventArgs) Handles WerkstattRechnungButton.CheckStateChanged
        If _rechnungsArt = RechnungsArt.Werkstatt Then
            WerkstattRechnungButton.CheckState = CheckState.Checked
            Return
        End If

        If WerkstattRechnungButton.CheckState = CheckState.Unchecked Then Return
        _rechnungsArt = RechnungsArt.Werkstatt
        TankabrechnungButton.CheckState = CheckState.Unchecked
        ManuelleRechnungButton.CheckState = CheckState.Unchecked
        RefreshGrid()
    End Sub

    Private Sub TankabrechnungButton_CheckedChanged(sender As Object, e As EventArgs) Handles TankabrechnungButton.CheckStateChanged
        If _rechnungsArt = RechnungsArt.Tanken Then
            TankabrechnungButton.CheckState = CheckState.Checked
            Return
        End If

        If TankabrechnungButton.CheckState = CheckState.Unchecked Then Return
        _rechnungsArt = RechnungsArt.Tanken
        ManuelleRechnungButton.CheckState = CheckState.Unchecked
        WerkstattRechnungButton.CheckState = CheckState.Unchecked
        RefreshGrid()
    End Sub

    Private Sub ManuelleRechnungButton_CheckedChanged(sender As Object, e As EventArgs) Handles ManuelleRechnungButton.CheckStateChanged
        If _rechnungsArt = RechnungsArt.Manuell Then
            ManuelleRechnungButton.CheckState = CheckState.Checked
            Return
        End If

        If ManuelleRechnungButton.CheckState = CheckState.Unchecked Then Return
        _rechnungsArt = RechnungsArt.Manuell
        TankabrechnungButton.CheckState = CheckState.Unchecked
        WerkstattRechnungButton.CheckState = CheckState.Unchecked
        RefreshGrid()
    End Sub

    Private Function RefreshGrid()

        Dim sql = GetSqlStatement(_rechnungsArt)
        Dim dataTable = _dbConnection.FillDataTable(sql)
        Dim dataTableBindingSource = New BindingSource()

        DataGridView1.Columns.Clear()
        DataGridView1.AutoSizeRows = False
        DataGridView1.TableElement.RowHeight = 40
        DataGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None
        dataTableBindingSource.DataSource = dataTable
        DataGridView1.DataSource = dataTableBindingSource
        DataGridView1.BestFitColumns(BestFitColumnMode.DisplayedCells)

        Dim buttonColumn As New GridViewCommandColumn()
        buttonColumn.Name = "Bericht"
        buttonColumn.HeaderText = "Bericht"
        buttonColumn.DefaultText = "Bericht"
        buttonColumn.UseDefaultText = False
        buttonColumn.ImageLayout = ImageLayout.Center
        buttonColumn.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter
        buttonColumn.Width = 80
        DataGridView1.Columns.Add(buttonColumn)

        buttonColumn = New GridViewCommandColumn()
        buttonColumn.Name = "XML"
        buttonColumn.HeaderText = "XML"
        buttonColumn.DefaultText = "XRechnung XML"
        buttonColumn.UseDefaultText = False
        buttonColumn.ImageLayout = ImageLayout.Center
        buttonColumn.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter
        buttonColumn.Width = 80
        DataGridView1.Columns.Add(buttonColumn)

        buttonColumn = New GridViewCommandColumn()
        buttonColumn.Name = "PDF"
        buttonColumn.HeaderText = "PDF"
        buttonColumn.DefaultText = "XRechnung Hybrid"
        buttonColumn.UseDefaultText = False
        buttonColumn.ImageLayout = ImageLayout.Center
        buttonColumn.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter
        buttonColumn.Width = 80
        DataGridView1.Columns.Add(buttonColumn)

        buttonColumn = New GridViewCommandColumn()
        buttonColumn.Name = "Validator"
        buttonColumn.HeaderText = "Validator"
        buttonColumn.DefaultText = "XRechnung Validator"
        buttonColumn.UseDefaultText = False
        buttonColumn.ImageLayout = ImageLayout.Center
        buttonColumn.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter
        buttonColumn.Width = 80
        DataGridView1.Columns.Add(buttonColumn)

    End Function

    Private Sub DataGridView1_CellClick(sender As Object, e As GridViewCellEventArgs) Handles DataGridView1.CellClick
        ' Check if the click is on a button column
        If e.RowIndex < 0 Then Return

        Dim dataSource = CType(DataGridView1.DataSource, BindingSource)
        Dim dataTable = CType(dataSource.DataSource, DataTable)
        Dim row = dataTable.Rows(e.RowIndex)
        Dim rechnungsNummer = Convert.ToInt32(row.Item(0))
        Dim billDate = Convert.ToDateTime(row.Item(1))

        Try
            Select Case e.Column.Name
                Case "Bericht"
                    Dim reportForm = New ReportForm(_dbConnection, _rechnungsArt, New Dictionary(Of Integer, Date) From {{rechnungsNummer, billDate}})
                    reportForm.ShowDialog()
                Case "XML"
                    Dim filePath As String = _xmlExporter.GetExportFilePath(_rechnungsArt, rechnungsNummer, billDate)
                    Using fileStream = System.IO.File.Create(filePath)
                        Try
                            _xmlExporter.CreateBillXml(fileStream, _rechnungsArt, rechnungsNummer)
                        Catch ex As Exception
                            MessageBox.Show("Speichern fehlgeschlagen!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        End Try

                    End Using

                    MessageBox.Show("Speichern erfolgreich!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case "PDF"
                    Dim reportForm = New ReportForm(_dbConnection, _rechnungsArt, New Dictionary(Of Integer, Date) From {{rechnungsNummer, billDate}})
                    reportForm.SavePdf()
                Case "Validator"
                    Dim fileDialog = New SaveFileDialog
                    fileDialog.Title = "Bitte XRechnung Speicherort auswählen."
                    fileDialog.Filter = "XRechnung|*.xml"
                    fileDialog.AddExtension = True
                    fileDialog.DefaultExt = "xml"
                    Dim result = fileDialog.ShowDialog()
                    If result <> DialogResult.OK Then Return
                    Using fileStream = File.Create(fileDialog.FileName)
                        Try
                            _xmlExporter.CreateBillXml(fileStream, _rechnungsArt, rechnungsNummer)
                        Catch ex As Exception
                            MessageBox.Show("Speichern fehlgeschlagen!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        End Try
                    End Using

                    _xmlExporter.Validate(fileDialog.FileName)
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GetSqlStatement(rechnungsArt As RechnungsArt) As String
        Select Case rechnungsArt
            Case RechnungsArt.Werkstatt
                Dim columnMapping As New Dictionary(Of String, String) From
                    {
                        {"RechnungsNr", "'RG-Nr.'"},
                        {"FORMAT (Rechnungsdatum, 'dd.MM.yyyy')", "'RG-Datum'"},
                        {"WANr", "'WA-Kurzbezeichnung'"},
                        {"FORMAT (Datum, 'dd.MM.yyyy')", "'WA-Datum'"},
                        {"Auftragsart", "'WA-Art'"},
                        {"Belegart", "Belegart"},
                        {"KundenNr", "'KD-Nr.'"},
                        {"DebitorNr", "'Debitor-Nr.'"},
                        {"Firma", "Firma"},
                        {"LeitwegeID", "'Leitweg-Id'"},
                        {"EmailRechnung", "'EmailRechnung'"},
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
                        {"FORMAT (Rechnungsdatum, 'dd.MM.yyyy')", "'RG-Datum'"},
                        {"Anmerkungen", "Anmerkungen"},
                        {"Belegart", "Belegart"},
                        {"KundenNr", "'KD-Nr.'"},
                        {"DebitorNr", "'Debitor-Nr.'"},
                        {"Firma", "Firma"},
                        {"LeitwegeID", "'Leitweg-Id'"},
                        {"EmailRechnung", "'EmailRechnung'"},
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
                        {"FORMAT (Rechnungsdatum, 'dd.MM.yyyy')", "'RG-Datum'"},
                        {"Anmerkungen", "Anmerkungen"},
                        {"Belegart", "Belegart"},
                        {"KundenNr", "'KD-Nr.'"},
                        {"DebitorNr", "'Debitor-Nr.'"},
                        {"Firma", "Firma"},
                        {"LeitwegeID", "'Leitweg-Id'"},
                        {"EmailRechnung", "'EmailRechnung'"},
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

        Dim rechnungsNummern = DataGridView1.Rows.Where(Function(r) r.GetType() = GetType(GridDataRowElement)).ToDictionary(
            Function(rowInfo)
                Dim dataRow = dataTable.Rows(rowInfo.Index)
                Return Convert.ToInt32(dataRow.Item(0))
            End Function,
            Function(rowInfo)
                Dim dataRow = dataTable.Rows(rowInfo.Index)
                Return Convert.ToDateTime(dataRow.Item(1))
            End Function)

        Try
            Dim reportForm = New ReportForm(_dbConnection, _rechnungsArt, rechnungsNummern)
            reportForm.ShowDialog()
        Catch ex As Exception
            _logger.Error($"Failed to create {NameOf(ReportForm)}")
            MessageBox.Show("Unbekannter Fehler beim Anzeigen der Rechnung!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim dataSource = CType(DataGridView1.DataSource, BindingSource)
        Dim dataTable = CType(dataSource.DataSource, DataTable)

        Try
            Dim reportForm = New ReportForm(_dbConnection, _rechnungsArt, DataGridView1.SelectedRows.ToDictionary(
                Function(rowInfo)
                    Dim dataRow = dataTable.Rows(rowInfo.Index)
                    Return Convert.ToInt32(dataRow.Item(0))
                End Function,
                Function(rowInfo)
                    Dim dataRow = dataTable.Rows(rowInfo.Index)
                    Return Convert.ToDateTime(dataRow.Item(1))
                End Function))

            reportForm.ShowDialog()
        Catch ex As Exception
            _logger.Error($"Failed to create {NameOf(ReportForm)}")
            MessageBox.Show("Unbekannter Fehler beim Anzeigen der Rechnung!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub SelectedPdfExportButton_Clicked(sender As Object, args As EventArgs) Handles SelectedPdfExportButton.Click
        Dim dataSource = CType(DataGridView1.DataSource, BindingSource)
        Dim dataTable = CType(dataSource.DataSource, DataTable)

        Try
            Dim reportForm = New ReportForm(_dbConnection, _rechnungsArt, DataGridView1.SelectedRows.ToDictionary(
                Function(rowInfo)
                    Dim dataRow = dataTable.Rows(rowInfo.Index)
                    Return Convert.ToInt32(dataRow.Item(0))
                End Function,
                Function(rowInfo)
                    Dim dataRow = dataTable.Rows(rowInfo.Index)
                    Return Convert.ToDateTime(dataRow.Item(1))
                End Function))

            reportForm.SavePdf()
        Catch ex As Exception
            _logger.Error($"Failed to create {NameOf(ReportForm)}")
            MessageBox.Show("Unbekannter Fehler beim Anzeigen der Rechnung!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SelectedXmlExportButton_Clicked(sender As Object, args As EventArgs) Handles SelectedXmlExportButton.Click
        Dim dataSource = CType(DataGridView1.DataSource, BindingSource)
        Dim dataTable = CType(dataSource.DataSource, DataTable)

        Try
            Dim bills = DataGridView1.SelectedRows.ToDictionary(
                Function(rowInfo)
                    Dim dataRow = dataTable.Rows(rowInfo.Index)
                    Return Convert.ToInt32(dataRow.Item(0))
                End Function,
                Function(rowInfo)
                    Dim dataRow = dataTable.Rows(rowInfo.Index)
                    Return Convert.ToDateTime(dataRow.Item(1))
                End Function)

            For Each bill In bills.Keys
                Dim filePath As String = _xmlExporter.GetExportFilePath(_rechnungsArt, bill, bills(bill))
                Try
                    Using fileStream = File.Create(filePath)
                        _xmlExporter.CreateBillXml(fileStream, _rechnungsArt, bill)
                    End Using
                Catch ex As Exception
                    _logger.Error($"Error saving bill xml file to {filePath}", ex)
                End Try
            Next
        Catch ex As Exception
            _logger.Error("Exception while saving bill xml files", ex)
            MessageBox.Show("Speichern fehlgeschlagen!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try
    End Sub

    Private Sub AllPdfExportButton_Clicked(sender As Object, args As EventArgs) Handles AllPdfExportButton.Click
        Dim dataSource = CType(DataGridView1.DataSource, BindingSource)
        Dim dataTable = CType(dataSource.DataSource, DataTable)

        Dim rechnungsNummern = DataGridView1.Rows.Where(Function(r) r.GetType() = GetType(GridDataRowElement)).ToDictionary(
            Function(rowInfo)
                Dim dataRow = dataTable.Rows(rowInfo.Index)
                Return Convert.ToInt32(dataRow.Item(0))
            End Function,
            Function(rowInfo)
                Dim dataRow = dataTable.Rows(rowInfo.Index)
                Return Convert.ToDateTime(dataRow.Item(1))
            End Function)

        Try
            Dim reportForm = New ReportForm(_dbConnection, _rechnungsArt, rechnungsNummern)
            reportForm.SavePdf()
        Catch ex As Exception
            _logger.Error($"Failed to create {NameOf(ReportForm)}")
            MessageBox.Show("Unbekannter Fehler beim Anzeigen der Rechnung!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AllXmlExportButton_Clicked(sender As Object, args As EventArgs) Handles AllXmlExportButton.Click
        Dim dataSource = CType(DataGridView1.DataSource, BindingSource)
        Dim dataTable = CType(dataSource.DataSource, DataTable)

        Try
            Dim bills = DataGridView1.Rows.ToDictionary(
                Function(rowInfo)
                    Dim dataRow = dataTable.Rows(rowInfo.Index)
                    Return Convert.ToInt32(dataRow.Item(0))
                End Function,
                Function(rowInfo)
                    Dim dataRow = dataTable.Rows(rowInfo.Index)
                    Return Convert.ToDateTime(dataRow.Item(1))
                End Function)

            For Each bill In bills.Keys
                Dim filePath As String = _xmlExporter.GetExportFilePath(_rechnungsArt, bill, bills(bill))
                Try
                    Using fileStream = File.Create(filePath)
                        _xmlExporter.CreateBillXml(fileStream, _rechnungsArt, bill)
                    End Using
                Catch ex As Exception
                    _logger.Error($"Error saving bill xml file to {filePath}", ex)
                End Try
            Next

            MessageBox.Show("Speichern erfolgreich!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            _logger.Error("Exception while saving bill xml files", ex)
            MessageBox.Show("Speichern fehlgeschlagen!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try
    End Sub
End Class