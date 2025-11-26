Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ExportLogGridForm
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ExportLogGridForm))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.btnPrintGrid = New System.Windows.Forms.Button()
        Me.btnCopyClipboard = New System.Windows.Forms.Button()
        Me.ExportLogGrid = New System.Windows.Forms.DataGridView()
        Me.Panel1.SuspendLayout()
        CType(Me.ExportLogGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnPrintGrid)
        Me.Panel1.Controls.Add(Me.btnCopyClipboard)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 313)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(984, 48)
        Me.Panel1.TabIndex = 3
        '
        'btnPrintGrid
        '
        Me.btnPrintGrid.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnPrintGrid.Location = New System.Drawing.Point(131, 13)
        Me.btnPrintGrid.Margin = New System.Windows.Forms.Padding(2)
        Me.btnPrintGrid.Name = "btnPrintGrid"
        Me.btnPrintGrid.Size = New System.Drawing.Size(90, 24)
        Me.btnPrintGrid.TabIndex = 4
        Me.btnPrintGrid.Text = "Drucken"
        Me.btnPrintGrid.UseVisualStyleBackColor = True
        '
        'btnCopyClipboard
        '
        Me.btnCopyClipboard.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnCopyClipboard.Location = New System.Drawing.Point(11, 13)
        Me.btnCopyClipboard.Margin = New System.Windows.Forms.Padding(2)
        Me.btnCopyClipboard.Name = "btnCopyClipboard"
        Me.btnCopyClipboard.Size = New System.Drawing.Size(116, 24)
        Me.btnCopyClipboard.TabIndex = 3
        Me.btnCopyClipboard.Text = "Zwischenablage"
        Me.btnCopyClipboard.UseVisualStyleBackColor = True
        '
        'ExportLogGrid
        '
        Me.ExportLogGrid.AllowUserToAddRows = False
        Me.ExportLogGrid.AllowUserToDeleteRows = False
        Me.ExportLogGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.ExportLogGrid.DefaultCellStyle = DataGridViewCellStyle1
        Me.ExportLogGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ExportLogGrid.Location = New System.Drawing.Point(0, 0)
        Me.ExportLogGrid.Margin = New System.Windows.Forms.Padding(2)
        Me.ExportLogGrid.Name = "ExportLogGrid"
        Me.ExportLogGrid.ReadOnly = True
        Me.ExportLogGrid.Size = New System.Drawing.Size(984, 313)
        Me.ExportLogGrid.TabIndex = 4
        '
        'ExportLogGridForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(984, 361)
        Me.Controls.Add(Me.ExportLogGrid)
        Me.Controls.Add(Me.Panel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "ExportLogGridForm"
        Me.Text = "Export-Log"
        Me.Panel1.ResumeLayout(False)
        CType(Me.ExportLogGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel1 As Panel
    Friend WithEvents btnPrintGrid As Button
    Friend WithEvents btnCopyClipboard As Button
    Friend WithEvents ExportLogGrid As DataGridView
End Class
