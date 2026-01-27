Imports System.Windows.Forms
Imports System.Diagnostics
Imports System.Drawing.Printing
Imports System.Drawing

Public Class ExportLogGridForm
    Private _logEntries As List(Of ExportLogEntry)

    Public Sub New(logEntries As List(Of ExportLogEntry))
        InitializeComponent()
        _logEntries = logEntries

        ' Setup DataGridView columns and properties
        ExportLogGrid.AutoGenerateColumns = False
        ExportLogGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
        ExportLogGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.True

        ExportLogGrid.Columns.Clear()
        ExportLogGrid.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "RechnungsNummer",
            .HeaderText = "Rechnungs-Nr.",
            .Name = "colRechnungsNr"
        })
        ExportLogGrid.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "Status",
            .HeaderText = "Status",
            .Name = "colStatus"
        })
        ExportLogGrid.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "FehlerInfo",
            .HeaderText = "Ausgabe",
            .Name = "colFehlerInfo",
            .Width = 400,
            .DefaultCellStyle = New DataGridViewCellStyle With {.WrapMode = DataGridViewTriState.True}
        })
        ExportLogGrid.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "ExportFilePath",
            .HeaderText = "Exportdatei",
            .Name = "colExportFilePath",
            .Width = 350
        })
        ExportLogGrid.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "HtmlValidatorPath",
            .HeaderText = "Validator-Bericht",
            .Name = "colValidatorBericht",
            .Width = 350
        })
        ' Neue Spalten für E-Mail-Versand
        ExportLogGrid.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "EmailEmpfaenger",
            .HeaderText = "E-Mail-Empfänger",
            .Name = "colEmailEmpfaenger",
            .Width = 250
        })
        ExportLogGrid.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "EmailStatus",
            .HeaderText = "E-Mail-Status",
            .Name = "colEmailStatus",
            .Width = 120
        })
        ExportLogGrid.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "EmailFehlerInfo",
            .HeaderText = "E-Mail-Fehlerinfo",
            .Name = "colEmailFehlerInfo",
            .Width = 400,
            .DefaultCellStyle = New DataGridViewCellStyle With {.WrapMode = DataGridViewTriState.True}
        })

        ' Set monospaced font for "Ausgabe" column (for code/indentation)
        ExportLogGrid.Columns("colFehlerInfo").DefaultCellStyle.Font = New Font("Courier New", 9, FontStyle.Regular)
        ExportLogGrid.Columns("colEmailFehlerInfo").DefaultCellStyle.Font = New Font("Courier New", 9, FontStyle.Regular)

        ExportLogGrid.DataSource = _logEntries

        ' Zeilenfarben je Status
        AddHandler ExportLogGrid.RowPrePaint, AddressOf ExportLogGrid_RowPrePaint

        ' Inhalt von E-Mail-Empfänger trimmen und Leerzeilen entfernen
        AddHandler ExportLogGrid.CellFormatting, AddressOf ExportLogGrid_CellFormatting

    End Sub

    Private Sub ExportLogGrid_RowPrePaint(sender As Object, e As DataGridViewRowPrePaintEventArgs)
        Dim grid = CType(sender, DataGridView)
        Dim row = grid.Rows(e.RowIndex)
        Dim status = Convert.ToString(row.Cells("colStatus").Value)
        If status IsNot Nothing AndAlso status.ToLowerInvariant() = "erfolgreich" Then
            row.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220) ' hellgrün
        Else
            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220) ' hellrot
        End If
    End Sub

    Private Sub ExportLogGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles ExportLogGrid.CellContentClick
        If e.ColumnIndex = ExportLogGrid.Columns("colValidatorBericht").Index AndAlso e.RowIndex >= 0 Then
            Dim htmlPath = Convert.ToString(ExportLogGrid.Rows(e.RowIndex).Cells("colValidatorBericht").Value)
            If Not String.IsNullOrEmpty(htmlPath) AndAlso IO.File.Exists(htmlPath) Then
                Process.Start("explorer.exe", htmlPath)
            End If
        End If
    End Sub

    ' Trimmt und bereinigt den Inhalt der Spalte "E-Mail-Empfänger"
    Private Sub ExportLogGrid_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If ExportLogGrid.Columns(e.ColumnIndex).Name = "colEmailEmpfaenger" AndAlso e.Value IsNot Nothing Then
            Dim lines = e.Value.ToString().Split({vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)
            Dim trimmed = String.Join(Environment.NewLine, lines.Select(Function(l) l.Trim()).Where(Function(l) Not String.IsNullOrWhiteSpace(l)))
            e.Value = trimmed
        End If
    End Sub

    Private Sub btnCopyClipboard_Click(sender As Object, e As EventArgs) Handles btnCopyClipboard.Click
        ExportLogGrid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
        ExportLogGrid.SelectAll()
        Clipboard.SetDataObject(ExportLogGrid.GetClipboardContent())
        ExportLogGrid.ClearSelection()
        MessageBox.Show("Grid-Inhalt wurde in die Zwischenablage kopiert.", "Export-Log", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnPrintGrid_Click(sender As Object, e As EventArgs) Handles btnPrintGrid.Click
        Dim printDoc As New PrintDocument()
        printDoc.DefaultPageSettings.Landscape = True ' Querformat aktivieren
        AddHandler printDoc.PrintPage, AddressOf PrintPageHandler
        Dim printDialog As New PrintDialog()
        printDialog.Document = printDoc
        If printDialog.ShowDialog() = DialogResult.OK Then
            printDoc.Print()
        End If
    End Sub

    Private Sub PrintPageHandler(sender As Object, e As PrintPageEventArgs)
        ' Druck im Querformat, FehlerInfo, Exportdatei und Validator-Bericht jeweils in separater Zeile
        Dim fontNormal As New Font("Arial", 8)
        Dim fontCourier As New Font("Courier New", 6)
        Dim y As Integer = e.MarginBounds.Top
        Dim x As Integer = e.MarginBounds.Left
        Dim lineHeightNormal As Integer = fontNormal.Height + 4
        Dim lineHeightCourier As Integer = fontCourier.Height + 3

        ' Spaltennamen für Spezialzeilen
        Dim colFehlerInfo = ExportLogGrid.Columns("colFehlerInfo").Index
        Dim colExportFilePath = ExportLogGrid.Columns("colExportFilePath").Index
        Dim colValidatorBericht = ExportLogGrid.Columns("colValidatorBericht").Index

        ' Hauptzeile: alle Spalten außer FehlerInfo, Exportdatei, Validator-Bericht
        For Each col As DataGridViewColumn In ExportLogGrid.Columns
            If col.Name <> "colFehlerInfo" AndAlso col.Name <> "colExportFilePath" AndAlso col.Name <> "colValidatorBericht" Then
                e.Graphics.DrawString(col.HeaderText, fontNormal, Brushes.Black, x, y)
                x += 200
            End If
        Next
        y += lineHeightNormal
        x = e.MarginBounds.Left

        ' Zeilen drucken
        For Each row As DataGridViewRow In ExportLogGrid.Rows
            ' Hauptzeile: alle Spalten außer FehlerInfo, Exportdatei, Validator-Bericht
            For Each col As DataGridViewColumn In ExportLogGrid.Columns
                If col.Name <> "colFehlerInfo" AndAlso col.Name <> "colExportFilePath" AndAlso col.Name <> "colValidatorBericht" Then
                    Dim val = If(row.Cells(col.Index).Value, "").ToString()
                    e.Graphics.DrawString(val, fontNormal, Brushes.Black, x, y)
                    x += 200
                End If
            Next

            x = e.MarginBounds.Left
            y += lineHeightNormal

            ' FehlerInfo in separater Zeile, mehrzeilig berücksichtigen
            Dim fehlerInfoVal = If(row.Cells(colFehlerInfo).Value, "").ToString()
            Dim fehlerInfoLines = fehlerInfoVal.Split({vbCrLf, vbLf, vbCr}, StringSplitOptions.None)
            For Each line As String In fehlerInfoLines
                e.Graphics.DrawString("> " & line, fontCourier, Brushes.Black, x, y)
                y += lineHeightCourier
                ' Nur "Ausgabe: " vor der ersten Zeile, danach nur den Text
                If line Is fehlerInfoLines(0) Then
                    ' Remove "Ausgabe: " prefix for subsequent lines
                    x += 0 ' no change needed, just for clarity
                End If
            Next

            ' Exportdatei (XML-Pfad) in separater Zeile
            Dim xmlPathVal = If(row.Cells(colExportFilePath).Value, "").ToString()
            e.Graphics.DrawString("Exportdatei: " & xmlPathVal, fontNormal, Brushes.Black, x, y)
            y += lineHeightNormal

            ' Validator-Bericht (HTML-Pfad) in separater Zeile
            Dim htmlPathVal = If(row.Cells(colValidatorBericht).Value, "").ToString()
            e.Graphics.DrawString("Validator-Bericht: " & htmlPathVal, fontNormal, Brushes.Black, x, y)
            y += lineHeightNormal

            ' Abstand zur nächsten Zeile
            y += 4
            x = e.MarginBounds.Left
        Next
        e.HasMorePages = False
    End Sub

End Class
