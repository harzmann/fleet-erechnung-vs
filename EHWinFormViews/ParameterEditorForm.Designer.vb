Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ParameterEditorForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ParameterEditorForm))
        Me.tabControl1 = New System.Windows.Forms.TabControl()
        Me.tab1 = New System.Windows.Forms.TabPage()
        Me.lblFirma = New System.Windows.Forms.Label()
        Me.txtFirma = New System.Windows.Forms.TextBox()
        Me.lblName = New System.Windows.Forms.Label()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.lblStrasse = New System.Windows.Forms.Label()
        Me.txtStrasse = New System.Windows.Forms.TextBox()
        Me.lblPLZ = New System.Windows.Forms.Label()
        Me.txtPLZ = New System.Windows.Forms.TextBox()
        Me.lblOrt = New System.Windows.Forms.Label()
        Me.txtOrt = New System.Windows.Forms.TextBox()
        Me.lblTelefon = New System.Windows.Forms.Label()
        Me.txtTelefon = New System.Windows.Forms.TextBox()
        Me.lblSteuernummer = New System.Windows.Forms.Label()
        Me.txtSteuernummer = New System.Windows.Forms.TextBox()
        Me.tabTA = New System.Windows.Forms.TabPage()
        Me.lblFirma_TA = New System.Windows.Forms.Label()
        Me.txtFirma_TA = New System.Windows.Forms.TextBox()
        Me.lblName_TA = New System.Windows.Forms.Label()
        Me.txtName_TA = New System.Windows.Forms.TextBox()
        Me.lblStrasse_TA = New System.Windows.Forms.Label()
        Me.txtStrasse_TA = New System.Windows.Forms.TextBox()
        Me.lblPLZ_TA = New System.Windows.Forms.Label()
        Me.txtPLZ_TA = New System.Windows.Forms.TextBox()
        Me.lblOrt_TA = New System.Windows.Forms.Label()
        Me.txtOrt_TA = New System.Windows.Forms.TextBox()
        Me.lblTelefon_TA = New System.Windows.Forms.Label()
        Me.txtTelefon_TA = New System.Windows.Forms.TextBox()
        Me.lblSteuernummer_TA = New System.Windows.Forms.Label()
        Me.txtSteuernummer_TA = New System.Windows.Forms.TextBox()
        Me.tabMR = New System.Windows.Forms.TabPage()
        Me.lblFirma_MR = New System.Windows.Forms.Label()
        Me.txtFirma_MR = New System.Windows.Forms.TextBox()
        Me.lblName_MR = New System.Windows.Forms.Label()
        Me.txtName_MR = New System.Windows.Forms.TextBox()
        Me.lblStrasse_MR = New System.Windows.Forms.Label()
        Me.txtStrasse_MR = New System.Windows.Forms.TextBox()
        Me.lblPLZ_MR = New System.Windows.Forms.Label()
        Me.txtPLZ_MR = New System.Windows.Forms.TextBox()
        Me.lblOrt_MR = New System.Windows.Forms.Label()
        Me.txtOrt_MR = New System.Windows.Forms.TextBox()
        Me.lblTelefon_MR = New System.Windows.Forms.Label()
        Me.txtTelefon_MR = New System.Windows.Forms.TextBox()
        Me.lblSteuernummer_MR = New System.Windows.Forms.Label()
        Me.txtSteuernummer_MR = New System.Windows.Forms.TextBox()
        Me.tab2 = New System.Windows.Forms.TabPage()
        Me.lblModul1_211 = New System.Windows.Forms.Label()
        Me.txtModul1_211 = New System.Windows.Forms.TextBox()
        Me.lblModul2_211 = New System.Windows.Forms.Label()
        Me.txtModul2_211 = New System.Windows.Forms.TextBox()
        Me.lblModul3_211 = New System.Windows.Forms.Label()
        Me.txtModul3_211 = New System.Windows.Forms.TextBox()
        Me.lblAusgabepfad_211 = New System.Windows.Forms.Label()
        Me.txtAusgabepfad_211 = New System.Windows.Forms.TextBox()
        Me.tab3 = New System.Windows.Forms.TabPage()
        Me.lblModul1_1211 = New System.Windows.Forms.Label()
        Me.txtModul1_1211 = New System.Windows.Forms.TextBox()
        Me.lblModul2_1211 = New System.Windows.Forms.Label()
        Me.txtModul2_1211 = New System.Windows.Forms.TextBox()
        Me.lblModul3_1211 = New System.Windows.Forms.Label()
        Me.txtModul3_1211 = New System.Windows.Forms.TextBox()
        Me.lblAusgabepfad_1211 = New System.Windows.Forms.Label()
        Me.txtAusgabepfad_1211 = New System.Windows.Forms.TextBox()
        Me.tab4 = New System.Windows.Forms.TabPage()
        Me.lblModul1_2211 = New System.Windows.Forms.Label()
        Me.txtModul1_2211 = New System.Windows.Forms.TextBox()
        Me.lblModul2_2211 = New System.Windows.Forms.Label()
        Me.txtModul2_2211 = New System.Windows.Forms.TextBox()
        Me.lblModul3_2211 = New System.Windows.Forms.Label()
        Me.txtModul3_2211 = New System.Windows.Forms.TextBox()
        Me.lblAusgabepfad_2211 = New System.Windows.Forms.Label()
        Me.txtAusgabepfad_2211 = New System.Windows.Forms.TextBox()
        Me.btnSaveAll = New System.Windows.Forms.Button()
        Me.tabControl1.SuspendLayout()
        Me.tab1.SuspendLayout()
        Me.tabTA.SuspendLayout()
        Me.tabMR.SuspendLayout()
        Me.tab2.SuspendLayout()
        Me.tab3.SuspendLayout()
        Me.tab4.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabControl1
        '
        Me.tabControl1.Controls.Add(Me.tab1)
        Me.tabControl1.Controls.Add(Me.tabTA)
        Me.tabControl1.Controls.Add(Me.tabMR)
        Me.tabControl1.Controls.Add(Me.tab2)
        Me.tabControl1.Controls.Add(Me.tab3)
        Me.tabControl1.Controls.Add(Me.tab4)
        Me.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabControl1.Location = New System.Drawing.Point(0, 0)
        Me.tabControl1.Name = "tabControl1"
        Me.tabControl1.SelectedIndex = 0
        Me.tabControl1.Size = New System.Drawing.Size(343, 320)
        Me.tabControl1.TabIndex = 0
        '
        'tab1
        '
        Me.tab1.Controls.Add(Me.lblFirma)
        Me.tab1.Controls.Add(Me.txtFirma)
        Me.tab1.Controls.Add(Me.lblName)
        Me.tab1.Controls.Add(Me.txtName)
        Me.tab1.Controls.Add(Me.lblStrasse)
        Me.tab1.Controls.Add(Me.txtStrasse)
        Me.tab1.Controls.Add(Me.lblPLZ)
        Me.tab1.Controls.Add(Me.txtPLZ)
        Me.tab1.Controls.Add(Me.lblOrt)
        Me.tab1.Controls.Add(Me.txtOrt)
        Me.tab1.Controls.Add(Me.lblTelefon)
        Me.tab1.Controls.Add(Me.txtTelefon)
        Me.tab1.Controls.Add(Me.lblSteuernummer)
        Me.tab1.Controls.Add(Me.txtSteuernummer)
        Me.tab1.Location = New System.Drawing.Point(4, 22)
        Me.tab1.Name = "tab1"
        Me.tab1.Padding = New System.Windows.Forms.Padding(3)
        Me.tab1.Size = New System.Drawing.Size(335, 294)
        Me.tab1.TabIndex = 0
        Me.tab1.Text = "Firmenstammdaten"
        Me.tab1.UseVisualStyleBackColor = True
        '
        'lblFirma
        '
        Me.lblFirma.AutoSize = True
        Me.lblFirma.Location = New System.Drawing.Point(17, 17)
        Me.lblFirma.Name = "lblFirma"
        Me.lblFirma.Size = New System.Drawing.Size(32, 13)
        Me.lblFirma.TabIndex = 0
        Me.lblFirma.Text = "Firma"
        '
        'txtFirma
        '
        Me.txtFirma.Location = New System.Drawing.Point(103, 15)
        Me.txtFirma.Name = "txtFirma"
        Me.txtFirma.Size = New System.Drawing.Size(172, 20)
        Me.txtFirma.TabIndex = 1
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Location = New System.Drawing.Point(17, 43)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(35, 13)
        Me.lblName.TabIndex = 2
        Me.lblName.Text = "Name"
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(103, 41)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(172, 20)
        Me.txtName.TabIndex = 3
        '
        'lblStrasse
        '
        Me.lblStrasse.AutoSize = True
        Me.lblStrasse.Location = New System.Drawing.Point(17, 69)
        Me.lblStrasse.Name = "lblStrasse"
        Me.lblStrasse.Size = New System.Drawing.Size(38, 13)
        Me.lblStrasse.TabIndex = 4
        Me.lblStrasse.Text = "Straﬂe"
        '
        'txtStrasse
        '
        Me.txtStrasse.Location = New System.Drawing.Point(103, 67)
        Me.txtStrasse.Name = "txtStrasse"
        Me.txtStrasse.Size = New System.Drawing.Size(172, 20)
        Me.txtStrasse.TabIndex = 5
        '
        'lblPLZ
        '
        Me.lblPLZ.AutoSize = True
        Me.lblPLZ.Location = New System.Drawing.Point(17, 95)
        Me.lblPLZ.Name = "lblPLZ"
        Me.lblPLZ.Size = New System.Drawing.Size(27, 13)
        Me.lblPLZ.TabIndex = 6
        Me.lblPLZ.Text = "PLZ"
        '
        'txtPLZ
        '
        Me.txtPLZ.Location = New System.Drawing.Point(103, 93)
        Me.txtPLZ.Name = "txtPLZ"
        Me.txtPLZ.Size = New System.Drawing.Size(172, 20)
        Me.txtPLZ.TabIndex = 7
        '
        'lblOrt
        '
        Me.lblOrt.AutoSize = True
        Me.lblOrt.Location = New System.Drawing.Point(17, 121)
        Me.lblOrt.Name = "lblOrt"
        Me.lblOrt.Size = New System.Drawing.Size(21, 13)
        Me.lblOrt.TabIndex = 8
        Me.lblOrt.Text = "Ort"
        '
        'txtOrt
        '
        Me.txtOrt.Location = New System.Drawing.Point(103, 119)
        Me.txtOrt.Name = "txtOrt"
        Me.txtOrt.Size = New System.Drawing.Size(172, 20)
        Me.txtOrt.TabIndex = 9
        '
        'lblTelefon
        '
        Me.lblTelefon.AutoSize = True
        Me.lblTelefon.Location = New System.Drawing.Point(17, 147)
        Me.lblTelefon.Name = "lblTelefon"
        Me.lblTelefon.Size = New System.Drawing.Size(43, 13)
        Me.lblTelefon.TabIndex = 10
        Me.lblTelefon.Text = "Telefon"
        '
        'txtTelefon
        '
        Me.txtTelefon.Location = New System.Drawing.Point(103, 145)
        Me.txtTelefon.Name = "txtTelefon"
        Me.txtTelefon.Size = New System.Drawing.Size(172, 20)
        Me.txtTelefon.TabIndex = 11
        '
        'lblSteuernummer
        '
        Me.lblSteuernummer.AutoSize = True
        Me.lblSteuernummer.Location = New System.Drawing.Point(17, 173)
        Me.lblSteuernummer.Name = "lblSteuernummer"
        Me.lblSteuernummer.Size = New System.Drawing.Size(42, 13)
        Me.lblSteuernummer.TabIndex = 12
        Me.lblSteuernummer.Text = "USt.-ID"
        '
        'txtSteuernummer
        '
        Me.txtSteuernummer.Location = New System.Drawing.Point(103, 171)
        Me.txtSteuernummer.Name = "txtSteuernummer"
        Me.txtSteuernummer.Size = New System.Drawing.Size(172, 20)
        Me.txtSteuernummer.TabIndex = 13
        '
        'tabTA
        '
        Me.tabTA.Controls.Add(Me.lblFirma_TA)
        Me.tabTA.Controls.Add(Me.txtFirma_TA)
        Me.tabTA.Controls.Add(Me.lblName_TA)
        Me.tabTA.Controls.Add(Me.txtName_TA)
        Me.tabTA.Controls.Add(Me.lblStrasse_TA)
        Me.tabTA.Controls.Add(Me.txtStrasse_TA)
        Me.tabTA.Controls.Add(Me.lblPLZ_TA)
        Me.tabTA.Controls.Add(Me.txtPLZ_TA)
        Me.tabTA.Controls.Add(Me.lblOrt_TA)
        Me.tabTA.Controls.Add(Me.txtOrt_TA)
        Me.tabTA.Controls.Add(Me.lblTelefon_TA)
        Me.tabTA.Controls.Add(Me.txtTelefon_TA)
        Me.tabTA.Controls.Add(Me.lblSteuernummer_TA)
        Me.tabTA.Controls.Add(Me.txtSteuernummer_TA)
        Me.tabTA.Location = New System.Drawing.Point(4, 22)
        Me.tabTA.Name = "tabTA"
        Me.tabTA.Padding = New System.Windows.Forms.Padding(3)
        Me.tabTA.Size = New System.Drawing.Size(335, 251)
        Me.tabTA.TabIndex = 1
        Me.tabTA.Text = "Firmenstammdaten TA"
        Me.tabTA.UseVisualStyleBackColor = True
        '
        'lblFirma_TA
        '
        Me.lblFirma_TA.AutoSize = True
        Me.lblFirma_TA.Location = New System.Drawing.Point(17, 17)
        Me.lblFirma_TA.Name = "lblFirma_TA"
        Me.lblFirma_TA.Size = New System.Drawing.Size(32, 13)
        Me.lblFirma_TA.TabIndex = 0
        Me.lblFirma_TA.Text = "Firma"
        '
        'txtFirma_TA
        '
        Me.txtFirma_TA.Location = New System.Drawing.Point(103, 15)
        Me.txtFirma_TA.Name = "txtFirma_TA"
        Me.txtFirma_TA.Size = New System.Drawing.Size(172, 20)
        Me.txtFirma_TA.TabIndex = 1
        '
        'lblName_TA
        '
        Me.lblName_TA.AutoSize = True
        Me.lblName_TA.Location = New System.Drawing.Point(17, 43)
        Me.lblName_TA.Name = "lblName_TA"
        Me.lblName_TA.Size = New System.Drawing.Size(35, 13)
        Me.lblName_TA.TabIndex = 2
        Me.lblName_TA.Text = "Name"
        '
        'txtName_TA
        '
        Me.txtName_TA.Location = New System.Drawing.Point(103, 41)
        Me.txtName_TA.Name = "txtName_TA"
        Me.txtName_TA.Size = New System.Drawing.Size(172, 20)
        Me.txtName_TA.TabIndex = 3
        '
        'lblStrasse_TA
        '
        Me.lblStrasse_TA.AutoSize = True
        Me.lblStrasse_TA.Location = New System.Drawing.Point(17, 69)
        Me.lblStrasse_TA.Name = "lblStrasse_TA"
        Me.lblStrasse_TA.Size = New System.Drawing.Size(38, 13)
        Me.lblStrasse_TA.TabIndex = 4
        Me.lblStrasse_TA.Text = "Straﬂe"
        '
        'txtStrasse_TA
        '
        Me.txtStrasse_TA.Location = New System.Drawing.Point(103, 67)
        Me.txtStrasse_TA.Name = "txtStrasse_TA"
        Me.txtStrasse_TA.Size = New System.Drawing.Size(172, 20)
        Me.txtStrasse_TA.TabIndex = 5
        '
        'lblPLZ_TA
        '
        Me.lblPLZ_TA.AutoSize = True
        Me.lblPLZ_TA.Location = New System.Drawing.Point(17, 95)
        Me.lblPLZ_TA.Name = "lblPLZ_TA"
        Me.lblPLZ_TA.Size = New System.Drawing.Size(27, 13)
        Me.lblPLZ_TA.TabIndex = 6
        Me.lblPLZ_TA.Text = "PLZ"
        '
        'txtPLZ_TA
        '
        Me.txtPLZ_TA.Location = New System.Drawing.Point(103, 93)
        Me.txtPLZ_TA.Name = "txtPLZ_TA"
        Me.txtPLZ_TA.Size = New System.Drawing.Size(172, 20)
        Me.txtPLZ_TA.TabIndex = 7
        '
        'lblOrt_TA
        '
        Me.lblOrt_TA.AutoSize = True
        Me.lblOrt_TA.Location = New System.Drawing.Point(17, 121)
        Me.lblOrt_TA.Name = "lblOrt_TA"
        Me.lblOrt_TA.Size = New System.Drawing.Size(21, 13)
        Me.lblOrt_TA.TabIndex = 8
        Me.lblOrt_TA.Text = "Ort"
        '
        'txtOrt_TA
        '
        Me.txtOrt_TA.Location = New System.Drawing.Point(103, 119)
        Me.txtOrt_TA.Name = "txtOrt_TA"
        Me.txtOrt_TA.Size = New System.Drawing.Size(172, 20)
        Me.txtOrt_TA.TabIndex = 9
        '
        'lblTelefon_TA
        '
        Me.lblTelefon_TA.AutoSize = True
        Me.lblTelefon_TA.Location = New System.Drawing.Point(17, 147)
        Me.lblTelefon_TA.Name = "lblTelefon_TA"
        Me.lblTelefon_TA.Size = New System.Drawing.Size(43, 13)
        Me.lblTelefon_TA.TabIndex = 10
        Me.lblTelefon_TA.Text = "Telefon"
        '
        'txtTelefon_TA
        '
        Me.txtTelefon_TA.Location = New System.Drawing.Point(103, 145)
        Me.txtTelefon_TA.Name = "txtTelefon_TA"
        Me.txtTelefon_TA.Size = New System.Drawing.Size(172, 20)
        Me.txtTelefon_TA.TabIndex = 11
        '
        'lblSteuernummer_TA
        '
        Me.lblSteuernummer_TA.AutoSize = True
        Me.lblSteuernummer_TA.Location = New System.Drawing.Point(17, 173)
        Me.lblSteuernummer_TA.Name = "lblSteuernummer_TA"
        Me.lblSteuernummer_TA.Size = New System.Drawing.Size(42, 13)
        Me.lblSteuernummer_TA.TabIndex = 12
        Me.lblSteuernummer_TA.Text = "USt.-ID"
        '
        'txtSteuernummer_TA
        '
        Me.txtSteuernummer_TA.Location = New System.Drawing.Point(103, 171)
        Me.txtSteuernummer_TA.Name = "txtSteuernummer_TA"
        Me.txtSteuernummer_TA.Size = New System.Drawing.Size(172, 20)
        Me.txtSteuernummer_TA.TabIndex = 13
        '
        'tabMR
        '
        Me.tabMR.Controls.Add(Me.lblFirma_MR)
        Me.tabMR.Controls.Add(Me.txtFirma_MR)
        Me.tabMR.Controls.Add(Me.lblName_MR)
        Me.tabMR.Controls.Add(Me.txtName_MR)
        Me.tabMR.Controls.Add(Me.lblStrasse_MR)
        Me.tabMR.Controls.Add(Me.txtStrasse_MR)
        Me.tabMR.Controls.Add(Me.lblPLZ_MR)
        Me.tabMR.Controls.Add(Me.txtPLZ_MR)
        Me.tabMR.Controls.Add(Me.lblOrt_MR)
        Me.tabMR.Controls.Add(Me.txtOrt_MR)
        Me.tabMR.Controls.Add(Me.lblTelefon_MR)
        Me.tabMR.Controls.Add(Me.txtTelefon_MR)
        Me.tabMR.Controls.Add(Me.lblSteuernummer_MR)
        Me.tabMR.Controls.Add(Me.txtSteuernummer_MR)
        Me.tabMR.Location = New System.Drawing.Point(4, 22)
        Me.tabMR.Name = "tabMR"
        Me.tabMR.Padding = New System.Windows.Forms.Padding(3)
        Me.tabMR.Size = New System.Drawing.Size(335, 251)
        Me.tabMR.TabIndex = 2
        Me.tabMR.Text = "Firmenstammdaten MR"
        Me.tabMR.UseVisualStyleBackColor = True
        '
        'lblFirma_MR
        '
        Me.lblFirma_MR.AutoSize = True
        Me.lblFirma_MR.Location = New System.Drawing.Point(17, 17)
        Me.lblFirma_MR.Name = "lblFirma_MR"
        Me.lblFirma_MR.Size = New System.Drawing.Size(32, 13)
        Me.lblFirma_MR.TabIndex = 0
        Me.lblFirma_MR.Text = "Firma"
        '
        'txtFirma_MR
        '
        Me.txtFirma_MR.Location = New System.Drawing.Point(103, 15)
        Me.txtFirma_MR.Name = "txtFirma_MR"
        Me.txtFirma_MR.Size = New System.Drawing.Size(172, 20)
        Me.txtFirma_MR.TabIndex = 1
        '
        'lblName_MR
        '
        Me.lblName_MR.AutoSize = True
        Me.lblName_MR.Location = New System.Drawing.Point(17, 43)
        Me.lblName_MR.Name = "lblName_MR"
        Me.lblName_MR.Size = New System.Drawing.Size(35, 13)
        Me.lblName_MR.TabIndex = 2
        Me.lblName_MR.Text = "Name"
        '
        'txtName_MR
        '
        Me.txtName_MR.Location = New System.Drawing.Point(103, 41)
        Me.txtName_MR.Name = "txtName_MR"
        Me.txtName_MR.Size = New System.Drawing.Size(172, 20)
        Me.txtName_MR.TabIndex = 3
        '
        'lblStrasse_MR
        '
        Me.lblStrasse_MR.AutoSize = True
        Me.lblStrasse_MR.Location = New System.Drawing.Point(17, 69)
        Me.lblStrasse_MR.Name = "lblStrasse_MR"
        Me.lblStrasse_MR.Size = New System.Drawing.Size(38, 13)
        Me.lblStrasse_MR.TabIndex = 4
        Me.lblStrasse_MR.Text = "Straﬂe"
        '
        'txtStrasse_MR
        '
        Me.txtStrasse_MR.Location = New System.Drawing.Point(103, 67)
        Me.txtStrasse_MR.Name = "txtStrasse_MR"
        Me.txtStrasse_MR.Size = New System.Drawing.Size(172, 20)
        Me.txtStrasse_MR.TabIndex = 5
        '
        'lblPLZ_MR
        '
        Me.lblPLZ_MR.AutoSize = True
        Me.lblPLZ_MR.Location = New System.Drawing.Point(17, 95)
        Me.lblPLZ_MR.Name = "lblPLZ_MR"
        Me.lblPLZ_MR.Size = New System.Drawing.Size(27, 13)
        Me.lblPLZ_MR.TabIndex = 6
        Me.lblPLZ_MR.Text = "PLZ"
        '
        'txtPLZ_MR
        '
        Me.txtPLZ_MR.Location = New System.Drawing.Point(103, 93)
        Me.txtPLZ_MR.Name = "txtPLZ_MR"
        Me.txtPLZ_MR.Size = New System.Drawing.Size(172, 20)
        Me.txtPLZ_MR.TabIndex = 7
        '
        'lblOrt_MR
        '
        Me.lblOrt_MR.AutoSize = True
        Me.lblOrt_MR.Location = New System.Drawing.Point(17, 121)
        Me.lblOrt_MR.Name = "lblOrt_MR"
        Me.lblOrt_MR.Size = New System.Drawing.Size(21, 13)
        Me.lblOrt_MR.TabIndex = 8
        Me.lblOrt_MR.Text = "Ort"
        '
        'txtOrt_MR
        '
        Me.txtOrt_MR.Location = New System.Drawing.Point(103, 119)
        Me.txtOrt_MR.Name = "txtOrt_MR"
        Me.txtOrt_MR.Size = New System.Drawing.Size(172, 20)
        Me.txtOrt_MR.TabIndex = 9
        '
        'lblTelefon_MR
        '
        Me.lblTelefon_MR.AutoSize = True
        Me.lblTelefon_MR.Location = New System.Drawing.Point(17, 147)
        Me.lblTelefon_MR.Name = "lblTelefon_MR"
        Me.lblTelefon_MR.Size = New System.Drawing.Size(43, 13)
        Me.lblTelefon_MR.TabIndex = 10
        Me.lblTelefon_MR.Text = "Telefon"
        '
        'txtTelefon_MR
        '
        Me.txtTelefon_MR.Location = New System.Drawing.Point(103, 145)
        Me.txtTelefon_MR.Name = "txtTelefon_MR"
        Me.txtTelefon_MR.Size = New System.Drawing.Size(172, 20)
        Me.txtTelefon_MR.TabIndex = 11
        '
        'lblSteuernummer_MR
        '
        Me.lblSteuernummer_MR.AutoSize = True
        Me.lblSteuernummer_MR.Location = New System.Drawing.Point(17, 173)
        Me.lblSteuernummer_MR.Name = "lblSteuernummer_MR"
        Me.lblSteuernummer_MR.Size = New System.Drawing.Size(42, 13)
        Me.lblSteuernummer_MR.TabIndex = 12
        Me.lblSteuernummer_MR.Text = "USt.-ID"
        '
        'txtSteuernummer_MR
        '
        Me.txtSteuernummer_MR.Location = New System.Drawing.Point(103, 171)
        Me.txtSteuernummer_MR.Name = "txtSteuernummer_MR"
        Me.txtSteuernummer_MR.Size = New System.Drawing.Size(172, 20)
        Me.txtSteuernummer_MR.TabIndex = 13
        '
        'tab2
        '
        Me.tab2.Controls.Add(Me.lblModul1_211)
        Me.tab2.Controls.Add(Me.txtModul1_211)
        Me.tab2.Controls.Add(Me.lblModul2_211)
        Me.tab2.Controls.Add(Me.txtModul2_211)
        Me.tab2.Controls.Add(Me.lblModul3_211)
        Me.tab2.Controls.Add(Me.txtModul3_211)
        Me.tab2.Controls.Add(Me.lblAusgabepfad_211)
        Me.tab2.Controls.Add(Me.txtAusgabepfad_211)
        Me.tab2.Location = New System.Drawing.Point(4, 22)
        Me.tab2.Name = "tab2"
        Me.tab2.Padding = New System.Windows.Forms.Padding(3)
        Me.tab2.Size = New System.Drawing.Size(335, 251)
        Me.tab2.TabIndex = 3
        Me.tab2.Text = "ERechnung WA"
        Me.tab2.UseVisualStyleBackColor = True
        '
        'lblModul1_211
        '
        Me.lblModul1_211.AutoSize = True
        Me.lblModul1_211.Location = New System.Drawing.Point(17, 26)
        Me.lblModul1_211.Name = "lblModul1_211"
        Me.lblModul1_211.Size = New System.Drawing.Size(80, 13)
        Me.lblModul1_211.TabIndex = 0
        Me.lblModul1_211.Text = "Absender eMail"
        '
        'txtModul1_211
        '
        Me.txtModul1_211.Location = New System.Drawing.Point(103, 23)
        Me.txtModul1_211.MaxLength = 100
        Me.txtModul1_211.Name = "txtModul1_211"
        Me.txtModul1_211.Size = New System.Drawing.Size(172, 20)
        Me.txtModul1_211.TabIndex = 1
        '
        'lblModul2_211
        '
        Me.lblModul2_211.AutoSize = True
        Me.lblModul2_211.Location = New System.Drawing.Point(17, 61)
        Me.lblModul2_211.Name = "lblModul2_211"
        Me.lblModul2_211.Size = New System.Drawing.Size(32, 13)
        Me.lblModul2_211.TabIndex = 2
        Me.lblModul2_211.Text = "IBAN"
        '
        'txtModul2_211
        '
        Me.txtModul2_211.Location = New System.Drawing.Point(103, 58)
        Me.txtModul2_211.MaxLength = 34
        Me.txtModul2_211.Name = "txtModul2_211"
        Me.txtModul2_211.Size = New System.Drawing.Size(172, 20)
        Me.txtModul2_211.TabIndex = 3
        '
        'lblModul3_211
        '
        Me.lblModul3_211.AutoSize = True
        Me.lblModul3_211.Location = New System.Drawing.Point(17, 95)
        Me.lblModul3_211.Name = "lblModul3_211"
        Me.lblModul3_211.Size = New System.Drawing.Size(63, 13)
        Me.lblModul3_211.TabIndex = 4
        Me.lblModul3_211.Text = "BIC/SWIFT"
        '
        'txtModul3_211
        '
        Me.txtModul3_211.Location = New System.Drawing.Point(103, 93)
        Me.txtModul3_211.MaxLength = 11
        Me.txtModul3_211.Name = "txtModul3_211"
        Me.txtModul3_211.Size = New System.Drawing.Size(172, 20)
        Me.txtModul3_211.TabIndex = 5
        '
        'lblAusgabepfad_211
        '
        Me.lblAusgabepfad_211.AutoSize = True
        Me.lblAusgabepfad_211.Location = New System.Drawing.Point(20, 130)
        Me.lblAusgabepfad_211.Name = "lblAusgabepfad_211"
        Me.lblAusgabepfad_211.Size = New System.Drawing.Size(73, 13)
        Me.lblAusgabepfad_211.TabIndex = 6
        Me.lblAusgabepfad_211.Text = "Ausgabepfad:"
        '
        'txtAusgabepfad_211
        '
        Me.txtAusgabepfad_211.Location = New System.Drawing.Point(103, 127)
        Me.txtAusgabepfad_211.MaxLength = 255
        Me.txtAusgabepfad_211.Name = "txtAusgabepfad_211"
        Me.txtAusgabepfad_211.Size = New System.Drawing.Size(172, 20)
        Me.txtAusgabepfad_211.TabIndex = 7
        '
        'tab3
        '
        Me.tab3.Controls.Add(Me.lblModul1_1211)
        Me.tab3.Controls.Add(Me.txtModul1_1211)
        Me.tab3.Controls.Add(Me.lblModul2_1211)
        Me.tab3.Controls.Add(Me.txtModul2_1211)
        Me.tab3.Controls.Add(Me.lblModul3_1211)
        Me.tab3.Controls.Add(Me.txtModul3_1211)
        Me.tab3.Controls.Add(Me.lblAusgabepfad_1211)
        Me.tab3.Controls.Add(Me.txtAusgabepfad_1211)
        Me.tab3.Location = New System.Drawing.Point(4, 22)
        Me.tab3.Name = "tab3"
        Me.tab3.Padding = New System.Windows.Forms.Padding(3)
        Me.tab3.Size = New System.Drawing.Size(335, 251)
        Me.tab3.TabIndex = 4
        Me.tab3.Text = "ERechnung TA"
        Me.tab3.UseVisualStyleBackColor = True
        '
        'lblModul1_1211
        '
        Me.lblModul1_1211.AutoSize = True
        Me.lblModul1_1211.Location = New System.Drawing.Point(17, 26)
        Me.lblModul1_1211.Name = "lblModul1_1211"
        Me.lblModul1_1211.Size = New System.Drawing.Size(80, 13)
        Me.lblModul1_1211.TabIndex = 0
        Me.lblModul1_1211.Text = "Absender eMail"
        '
        'txtModul1_1211
        '
        Me.txtModul1_1211.Location = New System.Drawing.Point(103, 23)
        Me.txtModul1_1211.MaxLength = 100
        Me.txtModul1_1211.Name = "txtModul1_1211"
        Me.txtModul1_1211.Size = New System.Drawing.Size(172, 20)
        Me.txtModul1_1211.TabIndex = 1
        '
        'lblModul2_1211
        '
        Me.lblModul2_1211.AutoSize = True
        Me.lblModul2_1211.Location = New System.Drawing.Point(17, 61)
        Me.lblModul2_1211.Name = "lblModul2_1211"
        Me.lblModul2_1211.Size = New System.Drawing.Size(32, 13)
        Me.lblModul2_1211.TabIndex = 2
        Me.lblModul2_1211.Text = "IBAN"
        '
        'txtModul2_1211
        '
        Me.txtModul2_1211.Location = New System.Drawing.Point(103, 58)
        Me.txtModul2_1211.MaxLength = 34
        Me.txtModul2_1211.Name = "txtModul2_1211"
        Me.txtModul2_1211.Size = New System.Drawing.Size(172, 20)
        Me.txtModul2_1211.TabIndex = 3
        '
        'lblModul3_1211
        '
        Me.lblModul3_1211.AutoSize = True
        Me.lblModul3_1211.Location = New System.Drawing.Point(17, 95)
        Me.lblModul3_1211.Name = "lblModul3_1211"
        Me.lblModul3_1211.Size = New System.Drawing.Size(63, 13)
        Me.lblModul3_1211.TabIndex = 4
        Me.lblModul3_1211.Text = "BIC/SWIFT"
        '
        'txtModul3_1211
        '
        Me.txtModul3_1211.Location = New System.Drawing.Point(103, 93)
        Me.txtModul3_1211.MaxLength = 11
        Me.txtModul3_1211.Name = "txtModul3_1211"
        Me.txtModul3_1211.Size = New System.Drawing.Size(172, 20)
        Me.txtModul3_1211.TabIndex = 5
        '
        'lblAusgabepfad_1211
        '
        Me.lblAusgabepfad_1211.AutoSize = True
        Me.lblAusgabepfad_1211.Location = New System.Drawing.Point(20, 130)
        Me.lblAusgabepfad_1211.Name = "lblAusgabepfad_1211"
        Me.lblAusgabepfad_1211.Size = New System.Drawing.Size(73, 13)
        Me.lblAusgabepfad_1211.TabIndex = 6
        Me.lblAusgabepfad_1211.Text = "Ausgabepfad:"
        '
        'txtAusgabepfad_1211
        '
        Me.txtAusgabepfad_1211.Location = New System.Drawing.Point(103, 127)
        Me.txtAusgabepfad_1211.MaxLength = 255
        Me.txtAusgabepfad_1211.Name = "txtAusgabepfad_1211"
        Me.txtAusgabepfad_1211.Size = New System.Drawing.Size(172, 20)
        Me.txtAusgabepfad_1211.TabIndex = 7
        '
        'tab4
        '
        Me.tab4.Controls.Add(Me.lblModul1_2211)
        Me.tab4.Controls.Add(Me.txtModul1_2211)
        Me.tab4.Controls.Add(Me.lblModul2_2211)
        Me.tab4.Controls.Add(Me.txtModul2_2211)
        Me.tab4.Controls.Add(Me.lblModul3_2211)
        Me.tab4.Controls.Add(Me.txtModul3_2211)
        Me.tab4.Controls.Add(Me.lblAusgabepfad_2211)
        Me.tab4.Controls.Add(Me.txtAusgabepfad_2211)
        Me.tab4.Location = New System.Drawing.Point(4, 22)
        Me.tab4.Name = "tab4"
        Me.tab4.Padding = New System.Windows.Forms.Padding(3)
        Me.tab4.Size = New System.Drawing.Size(335, 251)
        Me.tab4.TabIndex = 5
        Me.tab4.Text = "ERechnung MR"
        Me.tab4.UseVisualStyleBackColor = True
        '
        'lblModul1_2211
        '
        Me.lblModul1_2211.AutoSize = True
        Me.lblModul1_2211.Location = New System.Drawing.Point(17, 26)
        Me.lblModul1_2211.Name = "lblModul1_2211"
        Me.lblModul1_2211.Size = New System.Drawing.Size(80, 13)
        Me.lblModul1_2211.TabIndex = 0
        Me.lblModul1_2211.Text = "Absender eMail"
        '
        'txtModul1_2211
        '
        Me.txtModul1_2211.Location = New System.Drawing.Point(103, 23)
        Me.txtModul1_2211.MaxLength = 100
        Me.txtModul1_2211.Name = "txtModul1_2211"
        Me.txtModul1_2211.Size = New System.Drawing.Size(172, 20)
        Me.txtModul1_2211.TabIndex = 1
        '
        'lblModul2_2211
        '
        Me.lblModul2_2211.AutoSize = True
        Me.lblModul2_2211.Location = New System.Drawing.Point(17, 61)
        Me.lblModul2_2211.Name = "lblModul2_2211"
        Me.lblModul2_2211.Size = New System.Drawing.Size(32, 13)
        Me.lblModul2_2211.TabIndex = 2
        Me.lblModul2_2211.Text = "IBAN"
        '
        'txtModul2_2211
        '
        Me.txtModul2_2211.Location = New System.Drawing.Point(103, 58)
        Me.txtModul2_2211.MaxLength = 34
        Me.txtModul2_2211.Name = "txtModul2_2211"
        Me.txtModul2_2211.Size = New System.Drawing.Size(172, 20)
        Me.txtModul2_2211.TabIndex = 3
        '
        'lblModul3_2211
        '
        Me.lblModul3_2211.AutoSize = True
        Me.lblModul3_2211.Location = New System.Drawing.Point(17, 95)
        Me.lblModul3_2211.Name = "lblModul3_2211"
        Me.lblModul3_2211.Size = New System.Drawing.Size(63, 13)
        Me.lblModul3_2211.TabIndex = 4
        Me.lblModul3_2211.Text = "BIC/SWIFT"
        '
        'txtModul3_2211
        '
        Me.txtModul3_2211.Location = New System.Drawing.Point(103, 93)
        Me.txtModul3_2211.MaxLength = 11
        Me.txtModul3_2211.Name = "txtModul3_2211"
        Me.txtModul3_2211.Size = New System.Drawing.Size(172, 20)
        Me.txtModul3_2211.TabIndex = 5
        '
        'lblAusgabepfad_2211
        '
        Me.lblAusgabepfad_2211.AutoSize = True
        Me.lblAusgabepfad_2211.Location = New System.Drawing.Point(20, 130)
        Me.lblAusgabepfad_2211.Name = "lblAusgabepfad_2211"
        Me.lblAusgabepfad_2211.Size = New System.Drawing.Size(73, 13)
        Me.lblAusgabepfad_2211.TabIndex = 6
        Me.lblAusgabepfad_2211.Text = "Ausgabepfad:"
        '
        'txtAusgabepfad_2211
        '
        Me.txtAusgabepfad_2211.Location = New System.Drawing.Point(103, 127)
        Me.txtAusgabepfad_2211.MaxLength = 255
        Me.txtAusgabepfad_2211.Name = "txtAusgabepfad_2211"
        Me.txtAusgabepfad_2211.Size = New System.Drawing.Size(172, 20)
        Me.txtAusgabepfad_2211.TabIndex = 7
        '
        'btnSaveAll
        '
        Me.btnSaveAll.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.btnSaveAll.Location = New System.Drawing.Point(0, 292)
        Me.btnSaveAll.Name = "btnSaveAll"
        Me.btnSaveAll.Size = New System.Drawing.Size(343, 28)
        Me.btnSaveAll.TabIndex = 1
        Me.btnSaveAll.Text = "Speichern"
        '
        'ParameterEditorForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(343, 320)
        Me.Controls.Add(Me.btnSaveAll)
        Me.Controls.Add(Me.tabControl1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "ParameterEditorForm"
        Me.Text = "Verk‰ufer"
        Me.tabControl1.ResumeLayout(False)
        Me.tab1.ResumeLayout(False)
        Me.tab1.PerformLayout()
        Me.tabTA.ResumeLayout(False)
        Me.tabTA.PerformLayout()
        Me.tabMR.ResumeLayout(False)
        Me.tabMR.PerformLayout()
        Me.tab2.ResumeLayout(False)
        Me.tab2.PerformLayout()
        Me.tab3.ResumeLayout(False)
        Me.tab3.PerformLayout()
        Me.tab4.ResumeLayout(False)
        Me.tab4.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tabControl1 As TabControl
    Friend WithEvents tab1 As TabPage
    Friend WithEvents tabTA As TabPage
    Friend WithEvents tabMR As TabPage
    Friend WithEvents tab2 As TabPage
    Friend WithEvents tab3 As TabPage
    Friend WithEvents tab4 As TabPage

    ' Tab 1 Controls
    Friend WithEvents txtFirma As TextBox
    Friend WithEvents txtName As TextBox
    Friend WithEvents txtStrasse As TextBox
    Friend WithEvents txtPLZ As TextBox
    Friend WithEvents txtOrt As TextBox
    Friend WithEvents txtTelefon As TextBox
    Friend WithEvents txtSteuernummer As TextBox
    Friend WithEvents lblFirma As Label
    Friend WithEvents lblName As Label
    Friend WithEvents lblStrasse As Label
    Friend WithEvents lblPLZ As Label
    Friend WithEvents lblOrt As Label
    Friend WithEvents lblTelefon As Label
    Friend WithEvents lblSteuernummer As Label

    ' Tab TA Controls
    Friend WithEvents txtFirma_TA As TextBox
    Friend WithEvents txtName_TA As TextBox
    Friend WithEvents txtStrasse_TA As TextBox
    Friend WithEvents txtPLZ_TA As TextBox
    Friend WithEvents txtOrt_TA As TextBox
    Friend WithEvents txtTelefon_TA As TextBox
    Friend WithEvents txtSteuernummer_TA As TextBox
    Friend WithEvents lblFirma_TA As Label
    Friend WithEvents lblName_TA As Label
    Friend WithEvents lblStrasse_TA As Label
    Friend WithEvents lblPLZ_TA As Label
    Friend WithEvents lblOrt_TA As Label
    Friend WithEvents lblTelefon_TA As Label
    Friend WithEvents lblSteuernummer_TA As Label

    ' Tab MR Controls
    Friend WithEvents txtFirma_MR As TextBox
    Friend WithEvents txtName_MR As TextBox
    Friend WithEvents txtStrasse_MR As TextBox
    Friend WithEvents txtPLZ_MR As TextBox
    Friend WithEvents txtOrt_MR As TextBox
    Friend WithEvents txtTelefon_MR As TextBox
    Friend WithEvents txtSteuernummer_MR As TextBox
    Friend WithEvents lblFirma_MR As Label
    Friend WithEvents lblName_MR As Label
    Friend WithEvents lblStrasse_MR As Label
    Friend WithEvents lblPLZ_MR As Label
    Friend WithEvents lblOrt_MR As Label
    Friend WithEvents lblTelefon_MR As Label
    Friend WithEvents lblSteuernummer_MR As Label

    ' Tab 2 Controls
    Friend WithEvents txtModul1_211 As TextBox
    Friend WithEvents txtModul2_211 As TextBox
    Friend WithEvents txtModul3_211 As TextBox
    Friend WithEvents lblModul1_211 As Label
    Friend WithEvents lblModul2_211 As Label
    Friend WithEvents lblModul3_211 As Label
    Friend WithEvents lblAusgabepfad_211 As Label
    Friend WithEvents txtAusgabepfad_211 As TextBox

    ' Tab 3 Controls
    Friend WithEvents txtModul1_1211 As TextBox
    Friend WithEvents txtModul2_1211 As TextBox
    Friend WithEvents txtModul3_1211 As TextBox
    Friend WithEvents lblModul1_1211 As Label
    Friend WithEvents lblModul2_1211 As Label
    Friend WithEvents lblModul3_1211 As Label
    Friend WithEvents lblAusgabepfad_1211 As Label
    Friend WithEvents txtAusgabepfad_1211 As TextBox

    ' Tab 4 Controls
    Friend WithEvents txtModul1_2211 As TextBox
    Friend WithEvents txtModul2_2211 As TextBox
    Friend WithEvents txtModul3_2211 As TextBox
    Friend WithEvents lblModul1_2211 As Label
    Friend WithEvents lblModul2_2211 As Label
    Friend WithEvents lblModul3_2211 As Label
    Friend WithEvents lblAusgabepfad_2211 As Label
    Friend WithEvents txtAusgabepfad_2211 As TextBox

    ' Central Save Button
    Friend WithEvents btnSaveAll As Button

End Class
