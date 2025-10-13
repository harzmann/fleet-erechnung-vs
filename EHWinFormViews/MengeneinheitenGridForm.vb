Imports System.Data
Imports System.Windows.Forms
Imports Telerik.WinControls.UI
Imports ehfleet_classlibrary.General

Public Class MengeneinheitenGridForm
    Inherits Form

    Private WithEvents grid As RadGridView
    Private ReadOnly _dataConnection As Database

    Public Sub New(dataConnection As Database)
        _dataConnection = dataConnection
        InitializeComponent()
        LoadMengeneinheiten()
    End Sub

    Private Sub InitializeComponent()
        Dim TableViewDefinition1 As Telerik.WinControls.UI.TableViewDefinition = New Telerik.WinControls.UI.TableViewDefinition()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SteuersaetzeGridForm))
        Me.grid = New Telerik.WinControls.UI.RadGridView()
        CType(Me.grid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.grid.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'grid
        '
        Me.grid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grid.Location = New System.Drawing.Point(0, 0)
        '
        '
        '
        Me.grid.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill
        Me.grid.MasterTemplate.ViewDefinition = TableViewDefinition1
        Me.grid.Name = "grid"
        Me.grid.Size = New System.Drawing.Size(900, 400)
        Me.grid.TabIndex = 0
        '
        'MengeneinheitenGridForm
        '
        Me.ClientSize = New System.Drawing.Size(900, 400)
        Me.Controls.Add(Me.grid)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "MengeneinheitenGridForm"
        Me.Text = "Mengeneinheiten"
        CType(Me.grid.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.grid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Private Sub LoadMengeneinheiten()
        Dim dt = _dataConnection.FillDataTable("SELECT [ID],[MengeneinheitK],[MengeneinheitL],[ERechnung_UnitCode],[ERechnung_UnitConversion],[LetzterZugriff_Benutzer],[LetzterZugriff_Datum] FROM Mengeneinheit")
        grid.DataSource = dt

        grid.AutoGenerateColumns = False
        grid.Columns.Clear()

        grid.Columns.Add(New GridViewTextBoxColumn("ID") With {.FieldName = "ID", .[ReadOnly] = True})
        grid.Columns.Add(New GridViewTextBoxColumn("MengeneinheitK") With {.FieldName = "MengeneinheitK", .[ReadOnly] = True})
        grid.Columns.Add(New GridViewTextBoxColumn("MengeneinheitL") With {.FieldName = "MengeneinheitL", .[ReadOnly] = True})

        grid.Columns.Add(New GridViewTextBoxColumn("ERechnung_UnitCode") With {.FieldName = "ERechnung_UnitCode"})

        ' Use GridViewDecimalColumn for ERechnung_UnitConversion with decimal(18,6) settings
        Dim decCol As New GridViewDecimalColumn("ERechnung_UnitConversion")
        decCol.FieldName = "ERechnung_UnitConversion"
        decCol.DecimalPlaces = 6
        decCol.Minimum = Decimal.Parse("-999999999999.999999")
        decCol.Maximum = Decimal.Parse("999999999999.999999")
        decCol.FormatString = "{0:N6}"
        decCol.DataType = GetType(Decimal)
        grid.Columns.Add(decCol)

        ' Set yellow background for editable columns via CellFormatting event
        AddHandler grid.CellFormatting, AddressOf Grid_CellFormatting

        ' Set yellow background for editable columns
        For Each colName In New String() {"ERechnung_UnitCode", "ERechnung_UnitConversion"}
            grid.Columns(colName).ReadOnly = False
            grid.Columns(colName).HeaderText = colName
            grid.Columns(colName).AllowFiltering = True
            grid.Columns(colName).AllowSort = True
        Next

        ' Set all other columns as read-only, enable sorting/filtering
        For Each col In grid.Columns
            If Not {"ERechnung_UnitCode", "ERechnung_UnitConversion"}.Contains(col.Name) Then
                col.ReadOnly = True
            End If
            col.AllowFiltering = True
            col.AllowSort = True
        Next

        grid.AllowAddNewRow = False
        grid.AllowDeleteRow = False
        grid.AllowEditRow = True
        grid.ShowFilteringRow = True
        grid.EnableSorting = True
    End Sub

    ' Cell formatting for yellow background on editable columns
    Private Sub Grid_CellFormatting(sender As Object, e As CellFormattingEventArgs)
        Dim editableCols = New String() {"ERechnung_UnitCode", "ERechnung_UnitConversion"}
        If editableCols.Contains(e.Column.Name) Then
            e.CellElement.BackColor = Drawing.Color.Yellow
            e.CellElement.DrawFill = True
        Else
            e.CellElement.ResetValue(Telerik.WinControls.Primitives.FillPrimitive.BackColorProperty, Telerik.WinControls.ValueResetFlags.Local)
            e.CellElement.DrawFill = False
        End If
    End Sub

    Private Sub grid_CellValueChanged(sender As Object, e As GridViewCellEventArgs) Handles grid.CellValueChanged
        Dim editableCols = New String() {"ERechnung_UnitCode", "ERechnung_UnitConversion"}
        If editableCols.Contains(e.Column.Name) Then
            Dim row = grid.Rows(e.RowIndex)
            Dim id = row.Cells("ID").Value
            Dim ucode = row.Cells("ERechnung_UnitCode").Value
            Dim uconv = row.Cells("ERechnung_UnitConversion").Value
            Dim sql = "UPDATE Mengeneinheit SET ERechnung_UnitCode = ?, ERechnung_UnitConversion = ?, LetzterZugriff_Benutzer = ?, LetzterZugriff_Datum = ? WHERE ID = ?"
            Dim cmd = _dataConnection.CreateCommand(sql)
            cmd.Parameters.AddWithValue("@ucode", ucode)
            cmd.Parameters.AddWithValue("@uconv", uconv)
            cmd.Parameters.AddWithValue("@benutzer", "EHFleetXRechnung")
            cmd.Parameters.AddWithValue("@datum", DateTime.Now)
            cmd.Parameters.AddWithValue("@id", id)
            cmd.ExecuteNonQuery()
        End If
    End Sub

    ' Validation: Only allow decimal(18,6) in ERechnung_UnitConversion
    Private Sub grid_CellValidating(sender As Object, e As CellValidatingEventArgs) Handles grid.CellValidating
        If e.Column IsNot Nothing AndAlso e.Column.Name = "ERechnung_UnitConversion" Then
            Dim valueStr = If(e.Value, "").ToString()
            Dim decValue As Decimal
            If Not Decimal.TryParse(valueStr, decValue) Then
                e.Cancel = True
                grid.Rows(e.RowIndex).ErrorText = "Bitte geben Sie einen gültigen Dezimalwert ein."
                Return
            End If
            ' Check for decimal(18,6) precision
            Dim parts = valueStr.Split("."c)
            If parts.Length = 2 AndAlso parts(1).Length > 6 Then
                e.Cancel = True
                grid.Rows(e.RowIndex).ErrorText = "Maximal 6 Nachkommastellen erlaubt."
                Return
            End If
            If decValue > Decimal.Parse("999999999999.999999") OrElse decValue < Decimal.Parse("-999999999999.999999") Then
                e.Cancel = True
                grid.Rows(e.RowIndex).ErrorText = "Wert außerhalb des Bereichs für decimal(18,6)."
                Return
            End If
            grid.Rows(e.RowIndex).ErrorText = ""
        End If
    End Sub

    Private Sub grid_Click(sender As Object, e As EventArgs) Handles grid.Click

    End Sub
End Class
