Imports System.Data
Imports System.Windows.Forms
Imports Telerik.WinControls.UI
Imports ehfleet_classlibrary.General

Public Class SteuersaetzeGridForm
    Inherits Form

    Private WithEvents grid As RadGridView
    Private ReadOnly _dataConnection As Database

    Public Sub New(dataConnection As Database)
        _dataConnection = dataConnection
        InitializeComponent()
        LoadSteuersaetze()
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
        'SteuersaetzeGridForm
        '
        Me.ClientSize = New System.Drawing.Size(900, 400)
        Me.Controls.Add(Me.grid)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "SteuersaetzeGridForm"
        Me.Text = "Steuersätze"
        CType(Me.grid.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.grid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Private Sub LoadSteuersaetze()
        Dim dt = _dataConnection.FillDataTable("SELECT [ID],[Matchcode],[Kurzbezeichnung],[MwSt],[ERechnung_BT151],[ERechnung_BT120],[ERechnung_BT121],[LetzterZugriff_Benutzer],[LetzterZugriff_Datum] FROM Steuersätze")
        grid.DataSource = dt

        grid.AutoGenerateColumns = False
        grid.Columns.Clear()

        grid.Columns.Add(New GridViewTextBoxColumn("ID") With {.FieldName = "ID", .[ReadOnly] = True})
        grid.Columns.Add(New GridViewTextBoxColumn("Matchcode") With {.FieldName = "Matchcode", .[ReadOnly] = True})
        grid.Columns.Add(New GridViewTextBoxColumn("Kurzbezeichnung") With {.FieldName = "Kurzbezeichnung", .[ReadOnly] = True})
        grid.Columns.Add(New GridViewDecimalColumn("MwSt") With {.FieldName = "MwSt", .[ReadOnly] = True})

        grid.Columns.Add(New GridViewTextBoxColumn("ERechnung_BT151") With {.FieldName = "ERechnung_BT151"})
        grid.Columns.Add(New GridViewTextBoxColumn("ERechnung_BT120") With {.FieldName = "ERechnung_BT120"})
        grid.Columns.Add(New GridViewTextBoxColumn("ERechnung_BT121") With {.FieldName = "ERechnung_BT121"})

        ' Set yellow background for editable columns via CellFormatting event
        AddHandler grid.CellFormatting, AddressOf Grid_CellFormatting

        ' Set yellow background for editable columns
        For Each colName In New String() {"ERechnung_BT151", "ERechnung_BT120", "ERechnung_BT121"}
            grid.Columns(colName).ReadOnly = False
            grid.Columns(colName).HeaderText = colName
            grid.Columns(colName).AllowFiltering = True
            grid.Columns(colName).AllowSort = True
        Next

        ' Set all other columns as read-only, enable sorting/filtering
        For Each col In grid.Columns
            If Not {"ERechnung_BT151", "ERechnung_BT120", "ERechnung_BT121"}.Contains(col.Name) Then
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
        Dim editableCols = New String() {"ERechnung_BT151", "ERechnung_BT120", "ERechnung_BT121"}
        If editableCols.Contains(e.Column.Name) Then
            e.CellElement.BackColor = Drawing.Color.Yellow
            e.CellElement.DrawFill = True
        Else
            e.CellElement.ResetValue(Telerik.WinControls.Primitives.FillPrimitive.BackColorProperty, Telerik.WinControls.ValueResetFlags.Local)
            e.CellElement.DrawFill = False
        End If
    End Sub

    Private Sub grid_CellValueChanged(sender As Object, e As GridViewCellEventArgs) Handles grid.CellValueChanged
        Dim editableCols = New String() {"ERechnung_BT151", "ERechnung_BT120", "ERechnung_BT121"}
        If editableCols.Contains(e.Column.Name) Then
            Dim row = grid.Rows(e.RowIndex)
            Dim id = row.Cells("ID").Value
            Dim bt151 = row.Cells("ERechnung_BT151").Value
            Dim bt120 = row.Cells("ERechnung_BT120").Value
            Dim bt121 = row.Cells("ERechnung_BT121").Value
            Dim sql = "UPDATE Steuersätze SET ERechnung_BT151 = ?, ERechnung_BT120 = ?, ERechnung_BT121 = ?, LetzterZugriff_Benutzer = ?, LetzterZugriff_Datum = ? WHERE ID = ?"
            Dim cmd = _dataConnection.CreateCommand(sql)
            cmd.Parameters.AddWithValue("@bt151", bt151)
            cmd.Parameters.AddWithValue("@bt120", bt120)
            cmd.Parameters.AddWithValue("@bt121", bt121)
            cmd.Parameters.AddWithValue("@benutzer", "EHFleetXRechnung")
            cmd.Parameters.AddWithValue("@datum", DateTime.Now)
            cmd.Parameters.AddWithValue("@id", id)
            cmd.ExecuteNonQuery()
        End If
    End Sub

    Private Sub grid_Click(sender As Object, e As EventArgs) Handles grid.Click

    End Sub
End Class
