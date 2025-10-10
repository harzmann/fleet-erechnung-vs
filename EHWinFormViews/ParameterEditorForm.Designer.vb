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
        Me.btnSave1 = New System.Windows.Forms.Button()
        Me.tab2 = New System.Windows.Forms.TabPage()
        Me.lblModul1_211 = New System.Windows.Forms.Label()
        Me.txtModul1_211 = New System.Windows.Forms.TextBox()
        Me.lblModul2_211 = New System.Windows.Forms.Label()
        Me.txtModul2_211 = New System.Windows.Forms.TextBox()
        Me.lblModul3_211 = New System.Windows.Forms.Label()
        Me.txtModul3_211 = New System.Windows.Forms.TextBox()
        Me.btnSave211 = New System.Windows.Forms.Button()
        Me.tab3 = New System.Windows.Forms.TabPage()
        Me.lblModul1_1211 = New System.Windows.Forms.Label()
        Me.txtModul1_1211 = New System.Windows.Forms.TextBox()
        Me.lblModul2_1211 = New System.Windows.Forms.Label()
        Me.txtModul2_1211 = New System.Windows.Forms.TextBox()
        Me.lblModul3_1211 = New System.Windows.Forms.Label()
        Me.txtModul3_1211 = New System.Windows.Forms.TextBox()
        Me.btnSave1211 = New System.Windows.Forms.Button()
        Me.tab4 = New System.Windows.Forms.TabPage()
        Me.lblModul1_2211 = New System.Windows.Forms.Label()
        Me.txtModul1_2211 = New System.Windows.Forms.TextBox()
        Me.lblModul2_2211 = New System.Windows.Forms.Label()
        Me.txtModul2_2211 = New System.Windows.Forms.TextBox()
        Me.lblModul3_2211 = New System.Windows.Forms.Label()
        Me.txtModul3_2211 = New System.Windows.Forms.TextBox()
        Me.btnSave2211 = New System.Windows.Forms.Button()
        Me.tabControl1.SuspendLayout()
        Me.tab1.SuspendLayout()
        Me.tab2.SuspendLayout()
        Me.tab3.SuspendLayout()
        Me.tab4.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabControl1
        '
        Me.tabControl1.Controls.Add(Me.tab1)
        Me.tabControl1.Controls.Add(Me.tab2)
        Me.tabControl1.Controls.Add(Me.tab3)
        Me.tabControl1.Controls.Add(Me.tab4)
        Me.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabControl1.Location = New System.Drawing.Point(0, 0)
        Me.tabControl1.Name = "tabControl1"
        Me.tabControl1.SelectedIndex = 0
        Me.tabControl1.Size = New System.Drawing.Size(343, 277)
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
        Me.tab1.Controls.Add(Me.btnSave1)
        Me.tab1.Location = New System.Drawing.Point(4, 22)
        Me.tab1.Name = "tab1"
        Me.tab1.Padding = New System.Windows.Forms.Padding(3)
        Me.tab1.Size = New System.Drawing.Size(335, 251)
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
        'btnSave1
        '
        Me.btnSave1.Location = New System.Drawing.Point(103, 204)
        Me.btnSave1.Name = "btnSave1"
        Me.btnSave1.Size = New System.Drawing.Size(86, 23)
        Me.btnSave1.TabIndex = 14
        Me.btnSave1.Text = "Speichern"
        Me.btnSave1.UseVisualStyleBackColor = True
        '
        'tab2
        '
        Me.tab2.Controls.Add(Me.lblModul1_211)
        Me.tab2.Controls.Add(Me.txtModul1_211)
        Me.tab2.Controls.Add(Me.lblModul2_211)
        Me.tab2.Controls.Add(Me.txtModul2_211)
        Me.tab2.Controls.Add(Me.lblModul3_211)
        Me.tab2.Controls.Add(Me.txtModul3_211)
        Me.tab2.Controls.Add(Me.btnSave211)
        Me.tab2.Location = New System.Drawing.Point(4, 22)
        Me.tab2.Name = "tab2"
        Me.tab2.Padding = New System.Windows.Forms.Padding(3)
        Me.tab2.Size = New System.Drawing.Size(335, 251)
        Me.tab2.TabIndex = 1
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
        Me.txtModul3_211.Name = "txtModul3_211"
        Me.txtModul3_211.Size = New System.Drawing.Size(172, 20)
        Me.txtModul3_211.TabIndex = 5
        '
        'btnSave211
        '
        Me.btnSave211.Location = New System.Drawing.Point(103, 130)
        Me.btnSave211.Name = "btnSave211"
        Me.btnSave211.Size = New System.Drawing.Size(86, 23)
        Me.btnSave211.TabIndex = 6
        Me.btnSave211.Text = "Speichern"
        Me.btnSave211.UseVisualStyleBackColor = True
        '
        'tab3
        '
        Me.tab3.Controls.Add(Me.lblModul1_1211)
        Me.tab3.Controls.Add(Me.txtModul1_1211)
        Me.tab3.Controls.Add(Me.lblModul2_1211)
        Me.tab3.Controls.Add(Me.txtModul2_1211)
        Me.tab3.Controls.Add(Me.lblModul3_1211)
        Me.tab3.Controls.Add(Me.txtModul3_1211)
        Me.tab3.Controls.Add(Me.btnSave1211)
        Me.tab3.Location = New System.Drawing.Point(4, 22)
        Me.tab3.Name = "tab3"
        Me.tab3.Padding = New System.Windows.Forms.Padding(3)
        Me.tab3.Size = New System.Drawing.Size(335, 251)
        Me.tab3.TabIndex = 2
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
        Me.txtModul3_1211.Name = "txtModul3_1211"
        Me.txtModul3_1211.Size = New System.Drawing.Size(172, 20)
        Me.txtModul3_1211.TabIndex = 5
        '
        'btnSave1211
        '
        Me.btnSave1211.Location = New System.Drawing.Point(103, 130)
        Me.btnSave1211.Name = "btnSave1211"
        Me.btnSave1211.Size = New System.Drawing.Size(86, 23)
        Me.btnSave1211.TabIndex = 6
        Me.btnSave1211.Text = "Speichern"
        Me.btnSave1211.UseVisualStyleBackColor = True
        '
        'tab4
        '
        Me.tab4.Controls.Add(Me.lblModul1_2211)
        Me.tab4.Controls.Add(Me.txtModul1_2211)
        Me.tab4.Controls.Add(Me.lblModul2_2211)
        Me.tab4.Controls.Add(Me.txtModul2_2211)
        Me.tab4.Controls.Add(Me.lblModul3_2211)
        Me.tab4.Controls.Add(Me.txtModul3_2211)
        Me.tab4.Controls.Add(Me.btnSave2211)
        Me.tab4.Location = New System.Drawing.Point(4, 22)
        Me.tab4.Name = "tab4"
        Me.tab4.Padding = New System.Windows.Forms.Padding(3)
        Me.tab4.Size = New System.Drawing.Size(335, 251)
        Me.tab4.TabIndex = 3
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
        Me.txtModul3_2211.Name = "txtModul3_2211"
        Me.txtModul3_2211.Size = New System.Drawing.Size(172, 20)
        Me.txtModul3_2211.TabIndex = 5
        '
        'btnSave2211
        '
        Me.btnSave2211.Location = New System.Drawing.Point(103, 130)
        Me.btnSave2211.Name = "btnSave2211"
        Me.btnSave2211.Size = New System.Drawing.Size(86, 23)
        Me.btnSave2211.TabIndex = 6
        Me.btnSave2211.Text = "Speichern"
        Me.btnSave2211.UseVisualStyleBackColor = True
        '
        'ParameterEditorForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(343, 277)
        Me.Controls.Add(Me.tabControl1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "ParameterEditorForm"
        Me.Text = "Verk‰ufer"
        Me.tabControl1.ResumeLayout(False)
        Me.tab1.ResumeLayout(False)
        Me.tab1.PerformLayout()
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
    Friend WithEvents btnSave1 As Button

    ' Tab 2 Controls
    Friend WithEvents txtModul1_211 As TextBox
    Friend WithEvents txtModul2_211 As TextBox
    Friend WithEvents txtModul3_211 As TextBox
    Friend WithEvents lblModul1_211 As Label
    Friend WithEvents lblModul2_211 As Label
    Friend WithEvents lblModul3_211 As Label
    Friend WithEvents btnSave211 As Button

    ' Tab 3 Controls
    Friend WithEvents txtModul1_1211 As TextBox
    Friend WithEvents txtModul2_1211 As TextBox
    Friend WithEvents txtModul3_1211 As TextBox
    Friend WithEvents lblModul1_1211 As Label
    Friend WithEvents lblModul2_1211 As Label
    Friend WithEvents lblModul3_1211 As Label
    Friend WithEvents btnSave1211 As Button

    ' Tab 4 Controls
    Friend WithEvents txtModul1_2211 As TextBox
    Friend WithEvents txtModul2_2211 As TextBox
    Friend WithEvents txtModul3_2211 As TextBox
    Friend WithEvents lblModul1_2211 As Label
    Friend WithEvents lblModul2_2211 As Label
    Friend WithEvents lblModul3_2211 As Label
    Friend WithEvents btnSave2211 As Button

End Class
