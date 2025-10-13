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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RechnungsUebersicht))
        Dim TableViewDefinition1 As Telerik.WinControls.UI.TableViewDefinition = New Telerik.WinControls.UI.TableViewDefinition()
        Me.PanelMenu = New System.Windows.Forms.Panel()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.VerkaeuferButton = New Telerik.WinControls.UI.RadButton()
        Me.SteuersaetzeButton = New Telerik.WinControls.UI.RadButton()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.RadButton1 = New Telerik.WinControls.UI.RadButton()
        Me.RadButton2 = New Telerik.WinControls.UI.RadButton()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.SelectedXmlEmailButton = New Telerik.WinControls.UI.RadButton()
        Me.SelectedPdfEmailButton = New Telerik.WinControls.UI.RadButton()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.AllXmlExportButton = New Telerik.WinControls.UI.RadButton()
        Me.AllPdfExportButton = New Telerik.WinControls.UI.RadButton()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.SelectedXmlExportButton = New Telerik.WinControls.UI.RadButton()
        Me.SelectedPdfExportButton = New Telerik.WinControls.UI.RadButton()
        Me.GroupBoxBerichte = New System.Windows.Forms.GroupBox()
        Me.Button2 = New Telerik.WinControls.UI.RadButton()
        Me.Button1 = New Telerik.WinControls.UI.RadButton()
        Me.GroupBoxRechnungsart = New System.Windows.Forms.GroupBox()
        Me.ManuelleRechnungButton = New Telerik.WinControls.UI.RadToggleButton()
        Me.TankabrechnungButton = New Telerik.WinControls.UI.RadToggleButton()
        Me.WerkstattRechnungButton = New Telerik.WinControls.UI.RadToggleButton()
        Me.PanelDatagrid = New System.Windows.Forms.Panel()
        Me.DataGridView1 = New Telerik.WinControls.UI.RadGridView()
        Me.CrystalTheme1 = New Telerik.WinControls.Themes.CrystalTheme()
        Me.MengeneinheitenButton = New Telerik.WinControls.UI.RadButton()
        Me.PanelMenu.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        CType(Me.VerkaeuferButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SteuersaetzeButton, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        CType(Me.RadButton1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadButton2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox6.SuspendLayout()
        CType(Me.SelectedXmlEmailButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SelectedPdfEmailButton, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        CType(Me.AllXmlExportButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AllPdfExportButton, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        CType(Me.SelectedXmlExportButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SelectedPdfExportButton, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBoxBerichte.SuspendLayout()
        CType(Me.Button2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Button1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBoxRechnungsart.SuspendLayout()
        CType(Me.ManuelleRechnungButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TankabrechnungButton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WerkstattRechnungButton, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelDatagrid.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridView1.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.MengeneinheitenButton, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelMenu
        '
        Me.PanelMenu.Controls.Add(Me.GroupBox7)
        Me.PanelMenu.Controls.Add(Me.GroupBox4)
        Me.PanelMenu.Controls.Add(Me.GroupBox1)
        Me.PanelMenu.Controls.Add(Me.GroupBoxBerichte)
        Me.PanelMenu.Controls.Add(Me.GroupBoxRechnungsart)
        Me.PanelMenu.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelMenu.Location = New System.Drawing.Point(0, 0)
        Me.PanelMenu.Margin = New System.Windows.Forms.Padding(2)
        Me.PanelMenu.Name = "PanelMenu"
        Me.PanelMenu.Size = New System.Drawing.Size(1365, 94)
        Me.PanelMenu.TabIndex = 13
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.MengeneinheitenButton)
        Me.GroupBox7.Controls.Add(Me.VerkaeuferButton)
        Me.GroupBox7.Controls.Add(Me.SteuersaetzeButton)
        Me.GroupBox7.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBox7.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox7.Location = New System.Drawing.Point(979, 0)
        Me.GroupBox7.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox7.Size = New System.Drawing.Size(268, 94)
        Me.GroupBox7.TabIndex = 15
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "Stammdaten"
        '
        'VerkaeuferButton
        '
        Me.VerkaeuferButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.VerkaeuferButton.ImageAlignment = System.Drawing.ContentAlignment.TopCenter
        Me.VerkaeuferButton.Location = New System.Drawing.Point(4, 17)
        Me.VerkaeuferButton.Margin = New System.Windows.Forms.Padding(2)
        Me.VerkaeuferButton.Name = "VerkaeuferButton"
        Me.VerkaeuferButton.Padding = New System.Windows.Forms.Padding(2)
        Me.VerkaeuferButton.Size = New System.Drawing.Size(107, 21)
        Me.VerkaeuferButton.TabIndex = 14
        Me.VerkaeuferButton.Text = "Verkäufer"
        Me.VerkaeuferButton.TextAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.VerkaeuferButton.ThemeName = "Crystal"
        '
        'SteuersaetzeButton
        '
        Me.SteuersaetzeButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SteuersaetzeButton.ImageAlignment = System.Drawing.ContentAlignment.TopCenter
        Me.SteuersaetzeButton.Location = New System.Drawing.Point(4, 42)
        Me.SteuersaetzeButton.Margin = New System.Windows.Forms.Padding(2)
        Me.SteuersaetzeButton.Name = "SteuersaetzeButton"
        Me.SteuersaetzeButton.Padding = New System.Windows.Forms.Padding(2)
        Me.SteuersaetzeButton.Size = New System.Drawing.Size(107, 21)
        Me.SteuersaetzeButton.TabIndex = 13
        Me.SteuersaetzeButton.Text = "Steuersätze"
        Me.SteuersaetzeButton.TextAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.SteuersaetzeButton.ThemeName = "Crystal"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.GroupBox5)
        Me.GroupBox4.Controls.Add(Me.GroupBox6)
        Me.GroupBox4.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBox4.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox4.Location = New System.Drawing.Point(711, 0)
        Me.GroupBox4.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox4.Size = New System.Drawing.Size(268, 94)
        Me.GroupBox4.TabIndex = 14
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Email"
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.RadButton1)
        Me.GroupBox5.Controls.Add(Me.RadButton2)
        Me.GroupBox5.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBox5.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox5.Location = New System.Drawing.Point(134, 18)
        Me.GroupBox5.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox5.Size = New System.Drawing.Size(132, 74)
        Me.GroupBox5.TabIndex = 15
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Alle"
        '
        'RadButton1
        '
        Me.RadButton1.Font = New System.Drawing.Font("Segoe UI", 10.2!)
        Me.RadButton1.ImageAlignment = System.Drawing.ContentAlignment.TopCenter
        Me.RadButton1.Location = New System.Drawing.Point(68, 19)
        Me.RadButton1.Margin = New System.Windows.Forms.Padding(2)
        Me.RadButton1.Name = "RadButton1"
        Me.RadButton1.Padding = New System.Windows.Forms.Padding(2)
        Me.RadButton1.Size = New System.Drawing.Size(60, 50)
        Me.RadButton1.TabIndex = 13
        Me.RadButton1.Text = "XML"
        Me.RadButton1.TextAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.RadButton1.ThemeName = "Crystal"
        '
        'RadButton2
        '
        Me.RadButton2.Font = New System.Drawing.Font("Segoe UI", 10.2!)
        Me.RadButton2.ImageAlignment = System.Drawing.ContentAlignment.TopCenter
        Me.RadButton2.Location = New System.Drawing.Point(4, 19)
        Me.RadButton2.Margin = New System.Windows.Forms.Padding(2)
        Me.RadButton2.Name = "RadButton2"
        Me.RadButton2.Padding = New System.Windows.Forms.Padding(2)
        Me.RadButton2.Size = New System.Drawing.Size(60, 50)
        Me.RadButton2.TabIndex = 12
        Me.RadButton2.Text = "PDF"
        Me.RadButton2.TextAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.RadButton2.ThemeName = "Crystal"
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.SelectedXmlEmailButton)
        Me.GroupBox6.Controls.Add(Me.SelectedPdfEmailButton)
        Me.GroupBox6.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBox6.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox6.Location = New System.Drawing.Point(2, 18)
        Me.GroupBox6.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox6.Size = New System.Drawing.Size(132, 74)
        Me.GroupBox6.TabIndex = 14
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Ausgewählte"
        '
        'SelectedXmlEmailButton
        '
        Me.SelectedXmlEmailButton.Font = New System.Drawing.Font("Segoe UI", 10.2!)
        Me.SelectedXmlEmailButton.ImageAlignment = System.Drawing.ContentAlignment.TopCenter
        Me.SelectedXmlEmailButton.Location = New System.Drawing.Point(68, 19)
        Me.SelectedXmlEmailButton.Margin = New System.Windows.Forms.Padding(2)
        Me.SelectedXmlEmailButton.Name = "SelectedXmlEmailButton"
        Me.SelectedXmlEmailButton.Padding = New System.Windows.Forms.Padding(2)
        Me.SelectedXmlEmailButton.Size = New System.Drawing.Size(60, 50)
        Me.SelectedXmlEmailButton.TabIndex = 11
        Me.SelectedXmlEmailButton.Text = "XML"
        Me.SelectedXmlEmailButton.TextAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.SelectedXmlEmailButton.ThemeName = "Crystal"
        '
        'SelectedPdfEmailButton
        '
        Me.SelectedPdfEmailButton.Font = New System.Drawing.Font("Segoe UI", 10.2!)
        Me.SelectedPdfEmailButton.ImageAlignment = System.Drawing.ContentAlignment.TopCenter
        Me.SelectedPdfEmailButton.Location = New System.Drawing.Point(4, 19)
        Me.SelectedPdfEmailButton.Margin = New System.Windows.Forms.Padding(2)
        Me.SelectedPdfEmailButton.Name = "SelectedPdfEmailButton"
        Me.SelectedPdfEmailButton.Padding = New System.Windows.Forms.Padding(2)
        Me.SelectedPdfEmailButton.Size = New System.Drawing.Size(60, 50)
        Me.SelectedPdfEmailButton.TabIndex = 10
        Me.SelectedPdfEmailButton.Text = "PDF"
        Me.SelectedPdfEmailButton.TextAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.SelectedPdfEmailButton.ThemeName = "Crystal"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.GroupBox3)
        Me.GroupBox1.Controls.Add(Me.GroupBox2)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBox1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(443, 0)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Size = New System.Drawing.Size(268, 94)
        Me.GroupBox1.TabIndex = 13
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Export"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.AllXmlExportButton)
        Me.GroupBox3.Controls.Add(Me.AllPdfExportButton)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBox3.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.Location = New System.Drawing.Point(134, 18)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Size = New System.Drawing.Size(132, 74)
        Me.GroupBox3.TabIndex = 15
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Alle"
        '
        'AllXmlExportButton
        '
        Me.AllXmlExportButton.Font = New System.Drawing.Font("Segoe UI", 10.2!)
        Me.AllXmlExportButton.ImageAlignment = System.Drawing.ContentAlignment.TopCenter
        Me.AllXmlExportButton.Location = New System.Drawing.Point(68, 19)
        Me.AllXmlExportButton.Margin = New System.Windows.Forms.Padding(2)
        Me.AllXmlExportButton.Name = "AllXmlExportButton"
        Me.AllXmlExportButton.Padding = New System.Windows.Forms.Padding(2)
        Me.AllXmlExportButton.Size = New System.Drawing.Size(60, 50)
        Me.AllXmlExportButton.TabIndex = 13
        Me.AllXmlExportButton.Text = "XML"
        Me.AllXmlExportButton.TextAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.AllXmlExportButton.ThemeName = "Crystal"
        '
        'AllPdfExportButton
        '
        Me.AllPdfExportButton.Font = New System.Drawing.Font("Segoe UI", 10.2!)
        Me.AllPdfExportButton.ImageAlignment = System.Drawing.ContentAlignment.TopCenter
        Me.AllPdfExportButton.Location = New System.Drawing.Point(4, 19)
        Me.AllPdfExportButton.Margin = New System.Windows.Forms.Padding(2)
        Me.AllPdfExportButton.Name = "AllPdfExportButton"
        Me.AllPdfExportButton.Padding = New System.Windows.Forms.Padding(2)
        Me.AllPdfExportButton.Size = New System.Drawing.Size(60, 50)
        Me.AllPdfExportButton.TabIndex = 12
        Me.AllPdfExportButton.Text = "PDF"
        Me.AllPdfExportButton.TextAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.AllPdfExportButton.ThemeName = "Crystal"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.SelectedXmlExportButton)
        Me.GroupBox2.Controls.Add(Me.SelectedPdfExportButton)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBox2.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox2.Location = New System.Drawing.Point(2, 18)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox2.Size = New System.Drawing.Size(132, 74)
        Me.GroupBox2.TabIndex = 14
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Ausgewählte"
        '
        'SelectedXmlExportButton
        '
        Me.SelectedXmlExportButton.Font = New System.Drawing.Font("Segoe UI", 10.2!)
        Me.SelectedXmlExportButton.ImageAlignment = System.Drawing.ContentAlignment.TopCenter
        Me.SelectedXmlExportButton.Location = New System.Drawing.Point(68, 19)
        Me.SelectedXmlExportButton.Margin = New System.Windows.Forms.Padding(2)
        Me.SelectedXmlExportButton.Name = "SelectedXmlExportButton"
        Me.SelectedXmlExportButton.Padding = New System.Windows.Forms.Padding(2)
        Me.SelectedXmlExportButton.Size = New System.Drawing.Size(60, 50)
        Me.SelectedXmlExportButton.TabIndex = 11
        Me.SelectedXmlExportButton.Text = "XML"
        Me.SelectedXmlExportButton.TextAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.SelectedXmlExportButton.ThemeName = "Crystal"
        '
        'SelectedPdfExportButton
        '
        Me.SelectedPdfExportButton.Font = New System.Drawing.Font("Segoe UI", 10.2!)
        Me.SelectedPdfExportButton.ImageAlignment = System.Drawing.ContentAlignment.TopCenter
        Me.SelectedPdfExportButton.Location = New System.Drawing.Point(4, 19)
        Me.SelectedPdfExportButton.Margin = New System.Windows.Forms.Padding(2)
        Me.SelectedPdfExportButton.Name = "SelectedPdfExportButton"
        Me.SelectedPdfExportButton.Padding = New System.Windows.Forms.Padding(2)
        Me.SelectedPdfExportButton.Size = New System.Drawing.Size(60, 50)
        Me.SelectedPdfExportButton.TabIndex = 10
        Me.SelectedPdfExportButton.Text = "PDF"
        Me.SelectedPdfExportButton.TextAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.SelectedPdfExportButton.ThemeName = "Crystal"
        '
        'GroupBoxBerichte
        '
        Me.GroupBoxBerichte.Controls.Add(Me.Button2)
        Me.GroupBoxBerichte.Controls.Add(Me.Button1)
        Me.GroupBoxBerichte.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBoxBerichte.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBoxBerichte.Location = New System.Drawing.Point(257, 0)
        Me.GroupBoxBerichte.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBoxBerichte.Name = "GroupBoxBerichte"
        Me.GroupBoxBerichte.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBoxBerichte.Size = New System.Drawing.Size(186, 94)
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
        Me.Button2.Size = New System.Drawing.Size(178, 21)
        Me.Button2.TabIndex = 11
        Me.Button2.Text = "Alle anzeigen"
        Me.Button2.ThemeName = "Crystal"
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.Location = New System.Drawing.Point(4, 46)
        Me.Button1.Margin = New System.Windows.Forms.Padding(2)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(178, 21)
        Me.Button1.TabIndex = 10
        Me.Button1.Text = "Ausgewählte anzeigen"
        Me.Button1.TextWrap = True
        Me.Button1.ThemeName = "Crystal"
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
        Me.GroupBoxRechnungsart.Size = New System.Drawing.Size(257, 94)
        Me.GroupBoxRechnungsart.TabIndex = 11
        Me.GroupBoxRechnungsart.TabStop = False
        Me.GroupBoxRechnungsart.Text = "Rechnungsart"
        '
        'ManuelleRechnungButton
        '
        Me.ManuelleRechnungButton.Font = New System.Drawing.Font("Segoe UI", 10.2!)
        Me.ManuelleRechnungButton.Image = CType(resources.GetObject("ManuelleRechnungButton.Image"), System.Drawing.Image)
        Me.ManuelleRechnungButton.ImageAlignment = System.Drawing.ContentAlignment.TopCenter
        Me.ManuelleRechnungButton.Location = New System.Drawing.Point(172, 17)
        Me.ManuelleRechnungButton.Margin = New System.Windows.Forms.Padding(2)
        Me.ManuelleRechnungButton.Name = "ManuelleRechnungButton"
        Me.ManuelleRechnungButton.Padding = New System.Windows.Forms.Padding(5)
        Me.ManuelleRechnungButton.Size = New System.Drawing.Size(80, 70)
        Me.ManuelleRechnungButton.TabIndex = 9
        Me.ManuelleRechnungButton.Text = "MR"
        Me.ManuelleRechnungButton.TextAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.ManuelleRechnungButton.ThemeName = "Crystal"
        '
        'TankabrechnungButton
        '
        Me.TankabrechnungButton.Image = CType(resources.GetObject("TankabrechnungButton.Image"), System.Drawing.Image)
        Me.TankabrechnungButton.ImageAlignment = System.Drawing.ContentAlignment.TopCenter
        Me.TankabrechnungButton.Location = New System.Drawing.Point(88, 17)
        Me.TankabrechnungButton.Margin = New System.Windows.Forms.Padding(2)
        Me.TankabrechnungButton.Name = "TankabrechnungButton"
        Me.TankabrechnungButton.Padding = New System.Windows.Forms.Padding(5)
        Me.TankabrechnungButton.Size = New System.Drawing.Size(80, 70)
        Me.TankabrechnungButton.TabIndex = 8
        Me.TankabrechnungButton.Text = "TA"
        Me.TankabrechnungButton.TextAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.TankabrechnungButton.ThemeName = "Crystal"
        '
        'WerkstattRechnungButton
        '
        Me.WerkstattRechnungButton.Font = New System.Drawing.Font("Segoe UI", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WerkstattRechnungButton.Image = CType(resources.GetObject("WerkstattRechnungButton.Image"), System.Drawing.Image)
        Me.WerkstattRechnungButton.ImageAlignment = System.Drawing.ContentAlignment.TopCenter
        Me.WerkstattRechnungButton.Location = New System.Drawing.Point(4, 17)
        Me.WerkstattRechnungButton.Margin = New System.Windows.Forms.Padding(2)
        Me.WerkstattRechnungButton.Name = "WerkstattRechnungButton"
        Me.WerkstattRechnungButton.Padding = New System.Windows.Forms.Padding(5)
        Me.WerkstattRechnungButton.Size = New System.Drawing.Size(80, 70)
        Me.WerkstattRechnungButton.TabIndex = 7
        Me.WerkstattRechnungButton.Text = "WA"
        Me.WerkstattRechnungButton.TextAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.WerkstattRechnungButton.ThemeName = "Crystal"
        '
        'PanelDatagrid
        '
        Me.PanelDatagrid.Controls.Add(Me.DataGridView1)
        Me.PanelDatagrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDatagrid.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PanelDatagrid.Location = New System.Drawing.Point(0, 94)
        Me.PanelDatagrid.Margin = New System.Windows.Forms.Padding(2)
        Me.PanelDatagrid.Name = "PanelDatagrid"
        Me.PanelDatagrid.Size = New System.Drawing.Size(1365, 483)
        Me.PanelDatagrid.TabIndex = 14
        '
        'DataGridView1
        '
        Me.DataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridView1.EnableGestures = False
        Me.DataGridView1.Font = New System.Drawing.Font("Segoe UI", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DataGridView1.Location = New System.Drawing.Point(0, 0)
        Me.DataGridView1.Margin = New System.Windows.Forms.Padding(12)
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
        Me.DataGridView1.Size = New System.Drawing.Size(1365, 483)
        Me.DataGridView1.TabIndex = 13
        Me.DataGridView1.ThemeName = "Crystal"
        '
        'MengeneinheitenButton
        '
        Me.MengeneinheitenButton.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MengeneinheitenButton.ImageAlignment = System.Drawing.ContentAlignment.TopCenter
        Me.MengeneinheitenButton.Location = New System.Drawing.Point(4, 67)
        Me.MengeneinheitenButton.Margin = New System.Windows.Forms.Padding(2)
        Me.MengeneinheitenButton.Name = "MengeneinheitenButton"
        Me.MengeneinheitenButton.Padding = New System.Windows.Forms.Padding(2)
        Me.MengeneinheitenButton.Size = New System.Drawing.Size(107, 21)
        Me.MengeneinheitenButton.TabIndex = 15
        Me.MengeneinheitenButton.Text = "Mengeneinheiten"
        Me.MengeneinheitenButton.TextAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.MengeneinheitenButton.ThemeName = "Crystal"
        '
        'RechnungsUebersicht
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(1365, 577)
        Me.Controls.Add(Me.PanelDatagrid)
        Me.Controls.Add(Me.PanelMenu)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "RechnungsUebersicht"
        Me.Text = "FLEET XRechnung"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.PanelMenu.ResumeLayout(False)
        Me.GroupBox7.ResumeLayout(False)
        CType(Me.VerkaeuferButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SteuersaetzeButton, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        CType(Me.RadButton1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadButton2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox6.ResumeLayout(False)
        CType(Me.SelectedXmlEmailButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SelectedPdfEmailButton, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        CType(Me.AllXmlExportButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AllPdfExportButton, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        CType(Me.SelectedXmlExportButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SelectedPdfExportButton, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBoxBerichte.ResumeLayout(False)
        CType(Me.Button2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Button1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBoxRechnungsart.ResumeLayout(False)
        CType(Me.ManuelleRechnungButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TankabrechnungButton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WerkstattRechnungButton, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelDatagrid.ResumeLayout(False)
        CType(Me.DataGridView1.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.MengeneinheitenButton, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelMenu As System.Windows.Forms.Panel
    Friend WithEvents GroupBoxRechnungsart As System.Windows.Forms.GroupBox
    Friend WithEvents ManuelleRechnungButton As Telerik.WinControls.UI.RadToggleButton
    Friend WithEvents TankabrechnungButton As Telerik.WinControls.UI.RadToggleButton
    Friend WithEvents WerkstattRechnungButton As Telerik.WinControls.UI.RadToggleButton
    Friend WithEvents GroupBoxBerichte As System.Windows.Forms.GroupBox
    Friend WithEvents Button2 As Telerik.WinControls.UI.RadButton
    Friend WithEvents Button1 As Telerik.WinControls.UI.RadButton
    Friend WithEvents PanelDatagrid As System.Windows.Forms.Panel
    Public WithEvents DataGridView1 As RadGridView
    Friend WithEvents CrystalTheme1 As Telerik.WinControls.Themes.CrystalTheme
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents SelectedPdfExportButton As RadButton
    Friend WithEvents AllXmlExportButton As RadButton
    Friend WithEvents AllPdfExportButton As RadButton
    Friend WithEvents SelectedXmlExportButton As RadButton
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents RadButton1 As RadButton
    Friend WithEvents RadButton2 As RadButton
    Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
    Friend WithEvents SelectedXmlEmailButton As RadButton
    Friend WithEvents SelectedPdfEmailButton As RadButton
    Friend WithEvents GroupBox7 As System.Windows.Forms.GroupBox
    Friend WithEvents SteuersaetzeButton As RadButton
    Friend WithEvents VerkaeuferButton As RadButton
    Friend WithEvents MengeneinheitenButton As RadButton
End Class
