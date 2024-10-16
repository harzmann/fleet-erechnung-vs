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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RechnungsUebersicht))
        Dim TableViewDefinition7 As Telerik.WinControls.UI.TableViewDefinition = New Telerik.WinControls.UI.TableViewDefinition()
        Me.PanelMenu = New System.Windows.Forms.Panel()
        Me.GroupBoxBerichte = New System.Windows.Forms.GroupBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.GroupBoxRechnungsart = New System.Windows.Forms.GroupBox()
        Me.ManuelleRechnungButton = New System.Windows.Forms.RadioButton()
        Me.TankabrechnungButton = New System.Windows.Forms.RadioButton()
        Me.WerkstattRechnungButton = New System.Windows.Forms.RadioButton()
        Me.PanelDatagrid = New System.Windows.Forms.Panel()
        Me.DataGridView1 = New Telerik.WinControls.UI.RadGridView()
        Me.ImageListIcons32 = New System.Windows.Forms.ImageList(Me.components)
        Me.PanelMenu.SuspendLayout()
        Me.GroupBoxBerichte.SuspendLayout()
        Me.GroupBoxRechnungsart.SuspendLayout()
        Me.PanelDatagrid.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridView1.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelMenu
        '
        Me.PanelMenu.Controls.Add(Me.GroupBoxBerichte)
        Me.PanelMenu.Controls.Add(Me.GroupBoxRechnungsart)
        Me.PanelMenu.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelMenu.Location = New System.Drawing.Point(0, 0)
        Me.PanelMenu.Margin = New System.Windows.Forms.Padding(2)
        Me.PanelMenu.Name = "PanelMenu"
        Me.PanelMenu.Size = New System.Drawing.Size(1262, 118)
        Me.PanelMenu.TabIndex = 13
        '
        'GroupBoxBerichte
        '
        Me.GroupBoxBerichte.Controls.Add(Me.Button2)
        Me.GroupBoxBerichte.Controls.Add(Me.Button1)
        Me.GroupBoxBerichte.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBoxBerichte.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBoxBerichte.Location = New System.Drawing.Point(260, 0)
        Me.GroupBoxBerichte.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBoxBerichte.Name = "GroupBoxBerichte"
        Me.GroupBoxBerichte.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBoxBerichte.Size = New System.Drawing.Size(239, 118)
        Me.GroupBoxBerichte.TabIndex = 12
        Me.GroupBoxBerichte.TabStop = False
        Me.GroupBoxBerichte.Text = "Berichte"
        '
        'Button2
        '
        Me.Button2.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button2.Location = New System.Drawing.Point(4, 21)
        Me.Button2.Margin = New System.Windows.Forms.Padding(2)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(220, 40)
        Me.Button2.TabIndex = 11
        Me.Button2.Text = "Alle anzeigen"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.Location = New System.Drawing.Point(5, 61)
        Me.Button1.Margin = New System.Windows.Forms.Padding(2)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(220, 40)
        Me.Button1.TabIndex = 10
        Me.Button1.Text = "Ausgewählte anzeigen"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'GroupBoxRechnungsart
        '
        Me.GroupBoxRechnungsart.Controls.Add(Me.ManuelleRechnungButton)
        Me.GroupBoxRechnungsart.Controls.Add(Me.TankabrechnungButton)
        Me.GroupBoxRechnungsart.Controls.Add(Me.WerkstattRechnungButton)
        Me.GroupBoxRechnungsart.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBoxRechnungsart.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBoxRechnungsart.Location = New System.Drawing.Point(0, 0)
        Me.GroupBoxRechnungsart.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBoxRechnungsart.Name = "GroupBoxRechnungsart"
        Me.GroupBoxRechnungsart.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBoxRechnungsart.Size = New System.Drawing.Size(260, 118)
        Me.GroupBoxRechnungsart.TabIndex = 11
        Me.GroupBoxRechnungsart.TabStop = False
        Me.GroupBoxRechnungsart.Text = "Rechnungsart"
        '
        'ManuelleRechnungButton
        '
        Me.ManuelleRechnungButton.Appearance = System.Windows.Forms.Appearance.Button
        Me.ManuelleRechnungButton.Font = New System.Drawing.Font("Segoe UI", 10.2!)
        Me.ManuelleRechnungButton.Image = CType(resources.GetObject("ManuelleRechnungButton.Image"), System.Drawing.Image)
        Me.ManuelleRechnungButton.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.ManuelleRechnungButton.Location = New System.Drawing.Point(168, 21)
        Me.ManuelleRechnungButton.Margin = New System.Windows.Forms.Padding(2)
        Me.ManuelleRechnungButton.Name = "ManuelleRechnungButton"
        Me.ManuelleRechnungButton.Size = New System.Drawing.Size(80, 80)
        Me.ManuelleRechnungButton.TabIndex = 9
        Me.ManuelleRechnungButton.TabStop = True
        Me.ManuelleRechnungButton.Text = "MR"
        Me.ManuelleRechnungButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.ManuelleRechnungButton.UseVisualStyleBackColor = True
        '
        'TankabrechnungButton
        '
        Me.TankabrechnungButton.Appearance = System.Windows.Forms.Appearance.Button
        Me.TankabrechnungButton.Font = New System.Drawing.Font("Segoe UI", 10.2!)
        Me.TankabrechnungButton.Image = CType(resources.GetObject("TankabrechnungButton.Image"), System.Drawing.Image)
        Me.TankabrechnungButton.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.TankabrechnungButton.Location = New System.Drawing.Point(86, 21)
        Me.TankabrechnungButton.Margin = New System.Windows.Forms.Padding(2)
        Me.TankabrechnungButton.Name = "TankabrechnungButton"
        Me.TankabrechnungButton.Size = New System.Drawing.Size(80, 80)
        Me.TankabrechnungButton.TabIndex = 8
        Me.TankabrechnungButton.TabStop = True
        Me.TankabrechnungButton.Text = "TA"
        Me.TankabrechnungButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.TankabrechnungButton.UseVisualStyleBackColor = True
        '
        'WerkstattRechnungButton
        '
        Me.WerkstattRechnungButton.Appearance = System.Windows.Forms.Appearance.Button
        Me.WerkstattRechnungButton.Font = New System.Drawing.Font("Segoe UI", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WerkstattRechnungButton.Image = CType(resources.GetObject("WerkstattRechnungButton.Image"), System.Drawing.Image)
        Me.WerkstattRechnungButton.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.WerkstattRechnungButton.Location = New System.Drawing.Point(5, 21)
        Me.WerkstattRechnungButton.Margin = New System.Windows.Forms.Padding(2)
        Me.WerkstattRechnungButton.Name = "WerkstattRechnungButton"
        Me.WerkstattRechnungButton.Size = New System.Drawing.Size(80, 80)
        Me.WerkstattRechnungButton.TabIndex = 7
        Me.WerkstattRechnungButton.TabStop = True
        Me.WerkstattRechnungButton.Text = "WA"
        Me.WerkstattRechnungButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.WerkstattRechnungButton.UseVisualStyleBackColor = True
        '
        'PanelDatagrid
        '
        Me.PanelDatagrid.Controls.Add(Me.DataGridView1)
        Me.PanelDatagrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDatagrid.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PanelDatagrid.Location = New System.Drawing.Point(0, 118)
        Me.PanelDatagrid.Margin = New System.Windows.Forms.Padding(2)
        Me.PanelDatagrid.Name = "PanelDatagrid"
        Me.PanelDatagrid.Size = New System.Drawing.Size(1262, 603)
        Me.PanelDatagrid.TabIndex = 14
        '
        'DataGridView1
        '
        Me.DataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridView1.EnableGestures = False
        Me.DataGridView1.Font = New System.Drawing.Font("Segoe UI", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DataGridView1.Location = New System.Drawing.Point(0, 0)
        Me.DataGridView1.Margin = New System.Windows.Forms.Padding(0)
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
        Me.DataGridView1.MasterTemplate.ViewDefinition = TableViewDefinition7
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(1262, 603)
        Me.DataGridView1.TabIndex = 13
        '
        'ImageListIcons32
        '
        Me.ImageListIcons32.ImageStream = CType(resources.GetObject("ImageListIcons32.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageListIcons32.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageListIcons32.Images.SetKeyName(0, "approval_32x32.png")
        Me.ImageListIcons32.Images.SetKeyName(1, "car-repair_32x32.png")
        Me.ImageListIcons32.Images.SetKeyName(2, "file_32x32.png")
        Me.ImageListIcons32.Images.SetKeyName(3, "garage_32x32.png")
        Me.ImageListIcons32.Images.SetKeyName(4, "gasoline_32x32.png")
        Me.ImageListIcons32.Images.SetKeyName(5, "pdf_32x32.png")
        Me.ImageListIcons32.Images.SetKeyName(6, "service_32x32.png")
        Me.ImageListIcons32.Images.SetKeyName(7, "trust_32x32.png")
        '
        'RechnungsUebersicht
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(120.0!, 120.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(1262, 721)
        Me.Controls.Add(Me.PanelDatagrid)
        Me.Controls.Add(Me.PanelMenu)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "RechnungsUebersicht"
        Me.Text = "FLEET XRechnung"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.PanelMenu.ResumeLayout(False)
        Me.GroupBoxBerichte.ResumeLayout(False)
        Me.GroupBoxRechnungsart.ResumeLayout(False)
        Me.PanelDatagrid.ResumeLayout(False)
        CType(Me.DataGridView1.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelMenu As Windows.Forms.Panel
    Friend WithEvents GroupBoxRechnungsart As Windows.Forms.GroupBox
    Friend WithEvents ManuelleRechnungButton As Windows.Forms.RadioButton
    Friend WithEvents TankabrechnungButton As Windows.Forms.RadioButton
    Friend WithEvents WerkstattRechnungButton As Windows.Forms.RadioButton
    Friend WithEvents GroupBoxBerichte As Windows.Forms.GroupBox
    Friend WithEvents Button2 As Windows.Forms.Button
    Friend WithEvents Button1 As Windows.Forms.Button
    Friend WithEvents PanelDatagrid As Windows.Forms.Panel
    Public WithEvents DataGridView1 As RadGridView
    Friend WithEvents ImageListIcons32 As Windows.Forms.ImageList
End Class
