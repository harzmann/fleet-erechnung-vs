Imports Telerik.WinControls.UI

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class RechnungsUebersicht
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim TableViewDefinition1 As Telerik.WinControls.UI.TableViewDefinition = New Telerik.WinControls.UI.TableViewDefinition()
        Me.ManuelleRechnungButton = New System.Windows.Forms.RadioButton()
        Me.TankabrechnungButton = New System.Windows.Forms.RadioButton()
        Me.WerkstattRechnungButton = New System.Windows.Forms.RadioButton()
        Me.DataGridView1 = New Telerik.WinControls.UI.RadGridView()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridView1.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ManuelleRechnungButton
        '
        Me.ManuelleRechnungButton.Appearance = System.Windows.Forms.Appearance.Button
        Me.ManuelleRechnungButton.AutoSize = True
        Me.ManuelleRechnungButton.Location = New System.Drawing.Point(12, 68)
        Me.ManuelleRechnungButton.Name = "ManuelleRechnungButton"
        Me.ManuelleRechnungButton.Size = New System.Drawing.Size(136, 26)
        Me.ManuelleRechnungButton.TabIndex = 6
        Me.ManuelleRechnungButton.TabStop = True
        Me.ManuelleRechnungButton.Text = "Manuelle Rechnung"
        Me.ManuelleRechnungButton.UseVisualStyleBackColor = True
        '
        'TankabrechnungButton
        '
        Me.TankabrechnungButton.Appearance = System.Windows.Forms.Appearance.Button
        Me.TankabrechnungButton.Location = New System.Drawing.Point(12, 40)
        Me.TankabrechnungButton.Name = "TankabrechnungButton"
        Me.TankabrechnungButton.Size = New System.Drawing.Size(136, 26)
        Me.TankabrechnungButton.TabIndex = 5
        Me.TankabrechnungButton.TabStop = True
        Me.TankabrechnungButton.Text = "Tankabrechnung"
        Me.TankabrechnungButton.UseVisualStyleBackColor = True
        '
        'WerkstattRechnungButton
        '
        Me.WerkstattRechnungButton.Appearance = System.Windows.Forms.Appearance.Button
        Me.WerkstattRechnungButton.Location = New System.Drawing.Point(12, 12)
        Me.WerkstattRechnungButton.Name = "WerkstattRechnungButton"
        Me.WerkstattRechnungButton.Size = New System.Drawing.Size(136, 26)
        Me.WerkstattRechnungButton.TabIndex = 4
        Me.WerkstattRechnungButton.TabStop = True
        Me.WerkstattRechnungButton.Text = "Werkstattrechnung"
        Me.WerkstattRechnungButton.UseVisualStyleBackColor = True
        '
        'DataGridView1
        '
        Me.DataGridView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridView1.EnableGestures = False
        Me.DataGridView1.Location = New System.Drawing.Point(17, 105)
        Me.DataGridView1.Margin = New System.Windows.Forms.Padding(8)
        '
        '
        '
        Me.DataGridView1.MasterTemplate.AllowAddNewRow = False
        Me.DataGridView1.MasterTemplate.AllowCellContextMenu = False
        Me.DataGridView1.MasterTemplate.AllowDeleteRow = False
        Me.DataGridView1.MasterTemplate.AllowEditRow = False
        Me.DataGridView1.MasterTemplate.AllowSearchRow = True
        Me.DataGridView1.MasterTemplate.ClipboardPasteMode = Telerik.WinControls.UI.GridViewClipboardPasteMode.Disable
        Me.DataGridView1.MasterTemplate.EnableFiltering = True
        Me.DataGridView1.MasterTemplate.ShowGroupedColumns = True
        Me.DataGridView1.MasterTemplate.ViewDefinition = TableViewDefinition1
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(802, 370)
        Me.DataGridView1.TabIndex = 7
        '
        'RechnungsUebersicht
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1108, 607)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.ManuelleRechnungButton)
        Me.Controls.Add(Me.TankabrechnungButton)
        Me.Controls.Add(Me.WerkstattRechnungButton)
        Me.Name = "RechnungsUebersicht"
        Me.Text = "RechnungsUebersicht"
        CType(Me.DataGridView1.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ManuelleRechnungButton As Windows.Forms.RadioButton
    Friend WithEvents TankabrechnungButton As Windows.Forms.RadioButton
    Friend WithEvents WerkstattRechnungButton As Windows.Forms.RadioButton
    Public WithEvents DataGridView1 As RadGridView
End Class
