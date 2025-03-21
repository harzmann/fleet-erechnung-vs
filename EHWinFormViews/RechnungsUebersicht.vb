Imports System.Data
Imports System.IO
Imports System.Reflection
Imports System.Runtime.Loader
Imports System.Windows.Forms
Imports ehfleet_classlibrary
Imports EHFleetXRechnung.Viewer.GermanRadGridViewLocalization
Imports log4net
Imports log4net.Appender
Imports log4net.Repository
Imports Microsoft.VisualBasic.CompilerServices
Imports Stimulsoft.Base
Imports Stimulsoft.Report.Export
Imports Telerik.WinControls
Imports Telerik.WinControls.UI
Imports Telerik.WinControls.UI.Localization

Public Class RechnungsUebersicht

    Private ReadOnly _dbConnection As General.Database

    Private ReadOnly _xmlExporter As XRechnungExporter

    Private Shared ReadOnly _logger As ILog

    Private _rechnungsArt As RechnungsArt

    Public Property RechnungsArt As RechnungsArt
        Get
            Return _rechnungsArt
        End Get
        Set(value As RechnungsArt)
            _rechnungsArt = value
            'UpdateLogConfiguration()
        End Set
    End Property

    'Private Shared Sub LoadLicensingassembly()
    '    Dim currentNameSpace = GetType(ReportForm).Namespace
    '    Dim currentAssembly = GetType(ReportForm).Assembly
    '    Dim resources = currentAssembly.GetManifestResourceNames.ToHashSet
    '    Dim name = "Stimulsoft.Base.dll"

    '    If Not resources.Contains(name) Then Return

    '    Using stream = currentAssembly.GetManifestResourceStream(name)
    '        Dim assemblyContext = AssemblyLoadContext.GetLoadContext(currentAssembly)
    '        Dim assembly = assemblyContext.LoadFromStream(stream)
    '    End Using
    'End Sub

    Shared Sub New()
        _logger = LogManager.GetLogger(GetType(RechnungsUebersicht))
    End Sub

    Public Sub New(dbConnection As General.Database)
        _dbConnection = dbConnection
        Try
            _logger.Debug($"Instantiating {NameOf(RechnungsUebersicht)}")
            _xmlExporter = New XRechnungExporter(dbConnection)
            RadGridLocalizationProvider.CurrentProvider = New GermanRadGridLocalizationProvider
            ' This call is required by the designer.

            InitializeComponent()
            InitializeTelerikControls()
            WerkstattRechnungButton.CheckState = CheckState.Checked
            RechnungsArt = RechnungsArt.Werkstatt
            AddHandler DataGridView1.CellFormatting, Sub(sender, e)
                                                         If TypeOf e.Column Is GridViewDecimalColumn AndAlso e.Column.HeaderText = "Summe netto" Then
                                                             Dim numberColumn As GridViewDecimalColumn = DirectCast(e.Column, GridViewDecimalColumn)
                                                             numberColumn.FormatString = "{0:n} €"
                                                         End If

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
        Catch ex As Exception
            _logger.Error($"Error while creating {NameOf(RechnungsUebersicht)}", ex)
        End Try

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
        If RechnungsArt = RechnungsArt.Werkstatt Then
            WerkstattRechnungButton.CheckState = CheckState.Checked
            Return
        End If

        If WerkstattRechnungButton.CheckState = CheckState.Unchecked Then Return
        RechnungsArt = RechnungsArt.Werkstatt
        TankabrechnungButton.CheckState = CheckState.Unchecked
        ManuelleRechnungButton.CheckState = CheckState.Unchecked
        RefreshGrid()
    End Sub

    Private Sub TankabrechnungButton_CheckedChanged(sender As Object, e As EventArgs) Handles TankabrechnungButton.CheckStateChanged
        If RechnungsArt = RechnungsArt.Tanken Then
            TankabrechnungButton.CheckState = CheckState.Checked
            Return
        End If

        If TankabrechnungButton.CheckState = CheckState.Unchecked Then Return
        RechnungsArt = RechnungsArt.Tanken
        ManuelleRechnungButton.CheckState = CheckState.Unchecked
        WerkstattRechnungButton.CheckState = CheckState.Unchecked
        RefreshGrid()
    End Sub

    Private Sub ManuelleRechnungButton_CheckedChanged(sender As Object, e As EventArgs) Handles ManuelleRechnungButton.CheckStateChanged
        If RechnungsArt = RechnungsArt.Manuell Then
            ManuelleRechnungButton.CheckState = CheckState.Checked
            Return
        End If

        If ManuelleRechnungButton.CheckState = CheckState.Unchecked Then Return
        RechnungsArt = RechnungsArt.Manuell
        TankabrechnungButton.CheckState = CheckState.Unchecked
        WerkstattRechnungButton.CheckState = CheckState.Unchecked
        RefreshGrid()
    End Sub

    Private Sub UpdateLogConfiguration()
        Dim folder = _xmlExporter.GetExportPath(RechnungsArt)
        Dim repository As ILoggerRepository = LogManager.GetRepository()
        Dim appenders = repository.GetAppenders()
        Dim fileAppender As RollingFileAppender = appenders.FirstOrDefault(Function(a) a.GetType() = GetType(RollingFileAppender))
        If fileAppender Is Nothing Then Return

        Dim currentLogFile = fileAppender.File.Split(".").First()
        Dim fileName = Path.GetFileName(currentLogFile)
        Dim newFile = Path.Combine(folder, fileName)
        fileAppender.File = newFile
        fileAppender.ActivateOptions()
    End Sub

    Private Function RefreshGrid()
        Dim sql = GetSqlStatement(RechnungsArt)
        Dim dataTable = _dbConnection.FillDataTable(sql)
        If dataTable.Rows.Count = 0 OrElse dataTable.Columns.Count = 0 Then
            _logger.Fatal("Could not read from database")
            Throw New Exception("Datenbankverbindung konnte nicht aufgebaut werden.")
        End If

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
        Try
            Select Case e.Column.Name
                Case "Bericht"
                    _logger.Debug("Opening Report Form")
                    Dim reportForm = New ReportForm(_dbConnection, RechnungsArt, New List(Of Integer) From {rechnungsNummer})
                    reportForm.ShowDialog()
                Case "XML"
                    Dim filePath As String = _xmlExporter.GetExportFilePath(RechnungsArt, rechnungsNummer, "xml")
                    Using fileStream = File.Create(filePath)
                        Try
                            _xmlExporter.CreateBillXml(fileStream, RechnungsArt, rechnungsNummer)
                        Catch ex As Exception
                            MessageBox.Show("Speichern fehlgeschlagen!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        End Try

                    End Using

                    If _xmlExporter.IsSuccess Then
                        MessageBox.Show("Speichern erfolgreich!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        MessageBox.Show("Speichern fehlgeschlagen!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If

                Case "PDF"
                    Dim reportForm = New ReportForm(_dbConnection, RechnungsArt, New List(Of Integer) From {rechnungsNummer})
                    reportForm.SavePdf()
                    'MessageBox.Show("Speichern erfolgreich!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case "Validator"
                    Dim filePath As String = _xmlExporter.GetExportFilePath(RechnungsArt, rechnungsNummer, "xml")
                    Using fileStream = File.Create(filePath)
                        Try
                            _xmlExporter.CreateBillXml(fileStream, RechnungsArt, rechnungsNummer)
                        Catch ex As Exception
                            MessageBox.Show("Speichern fehlgeschlagen!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        End Try
                    End Using

                    _xmlExporter.Validate(filePath)
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

        Dim rechnungsNummern = DataGridView1.Rows.Where(Function(r) r.GetType() = GetType(GridDataRowElement)).Select(
            Function(rowInfo)
                Dim dataRow = dataTable.Rows(rowInfo.Index)
                Return Convert.ToInt32(dataRow.Item(0))
            End Function).ToList

        Try
            Dim reportForm = New ReportForm(_dbConnection, RechnungsArt, rechnungsNummern)
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
            Dim reportForm = New ReportForm(_dbConnection, RechnungsArt, DataGridView1.SelectedRows.Select(
                Function(rowInfo)
                    Dim dataRow = dataTable.Rows(rowInfo.Index)
                    Return Convert.ToInt32(dataRow.Item(0))
                End Function).ToList)

            reportForm.ShowDialog()
        Catch ex As Exception
            _logger.Error($"Failed to create {NameOf(ReportForm)}")
            MessageBox.Show("Unbekannter Fehler beim Anzeigen der Rechnung!" & vbCrLf & ex.Message, "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub SelectedPdfExportButton_Clicked(sender As Object, args As EventArgs) Handles SelectedPdfExportButton.Click
        Dim dataSource = CType(DataGridView1.DataSource, BindingSource)
        Dim dataTable = CType(dataSource.DataSource, DataTable)

        Try
            Dim reportForm = New ReportForm(_dbConnection, RechnungsArt, DataGridView1.SelectedRows.Select(
                Function(rowInfo)
                    Dim dataRow = dataTable.Rows(rowInfo.Index)
                    Return Convert.ToInt32(dataRow.Item(0))
                End Function).ToList)

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
                Dim filePath As String = _xmlExporter.GetExportFilePath(RechnungsArt, bill, "xml")
                Try
                    Using fileStream = File.Create(filePath)
                        _xmlExporter.CreateBillXml(fileStream, RechnungsArt, bill)
                    End Using
                Catch ex As Exception
                    _logger.Error($"Error saving bill xml file to {filePath}", ex)
                End Try
            Next

            If _xmlExporter.IsSuccess Then
                MessageBox.Show("Speichern erfolgreich!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MessageBox.Show("Speichern fehlgeschlagen!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            _logger.Error("Exception while saving bill xml files", ex)
            MessageBox.Show("Speichern fehlgeschlagen!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try
    End Sub

    Private Sub AllPdfExportButton_Clicked(sender As Object, args As EventArgs) Handles AllPdfExportButton.Click
        Dim dataSource = CType(DataGridView1.DataSource, BindingSource)
        Dim dataTable = CType(dataSource.DataSource, DataTable)

        Dim rechnungsNummern = DataGridView1.Rows.Where(Function(r) r.GetType() = GetType(GridViewDataRowInfo)).Select(
            Function(rowInfo)
                Dim dataRow = dataTable.Rows(rowInfo.Index)
                Return Convert.ToInt32(dataRow.Item(0))
            End Function).ToList

        Try
            Dim reportForm = New ReportForm(_dbConnection, RechnungsArt, rechnungsNummern)
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
                Dim filePath As String = _xmlExporter.GetExportFilePath(RechnungsArt, bill, "xml")
                Try
                    Using fileStream = File.Create(filePath)
                        _xmlExporter.CreateBillXml(fileStream, RechnungsArt, bill)
                        If _xmlExporter.IsSuccess = False Then
                            Dim Result As DialogResult
                            Result = MessageBox.Show("XML Export fehlgeschlagen!" & vbCrLf & "Soll Vorgang abgebrochen werden?", "Fleet Fuhrpark IM System", MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                            If Result = System.Windows.Forms.DialogResult.Yes Then
                                Exit For
                            End If
                        End If
                    End Using
                Catch ex As Exception
                    _logger.Error($"Error saving bill xml file to {filePath}", ex)
                End Try
            Next

            MessageBox.Show("XML Export abgeschlossen!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            _logger.Error("Exception while saving bill xml files", ex)
            MessageBox.Show("XML Export fehlgeschlagen!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try
    End Sub
End Class