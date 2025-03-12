<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class StartUp
    Inherits Telerik.WinControls.UI.RadForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(StartUp))
        butStart = New Telerik.WinControls.UI.RadButton()
        lblAppDbCnStr = New Telerik.WinControls.UI.RadLabel()
        txtAppDbCnStr = New Telerik.WinControls.UI.RadTextBox()
        chkUseRegDbCnStr = New Telerik.WinControls.UI.RadCheckBox()
        butConfig = New Telerik.WinControls.UI.RadButton()
        tmrStartUp = New Timer(components)
        lblStatus = New Telerik.WinControls.UI.RadLabel()
        RadPictureBox1 = New Telerik.WinControls.UI.RadPictureBox()
        CType(butStart, ComponentModel.ISupportInitialize).BeginInit()
        CType(lblAppDbCnStr, ComponentModel.ISupportInitialize).BeginInit()
        CType(txtAppDbCnStr, ComponentModel.ISupportInitialize).BeginInit()
        CType(chkUseRegDbCnStr, ComponentModel.ISupportInitialize).BeginInit()
        CType(butConfig, ComponentModel.ISupportInitialize).BeginInit()
        CType(lblStatus, ComponentModel.ISupportInitialize).BeginInit()
        CType(RadPictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' butStart
        ' 
        butStart.Location = New Point(12, 109)
        butStart.Name = "butStart"
        butStart.Size = New Size(110, 24)
        butStart.TabIndex = 0
        butStart.Text = "Start (5)"
        ' 
        ' lblAppDbCnStr
        ' 
        lblAppDbCnStr.Location = New Point(15, 169)
        lblAppDbCnStr.Name = "lblAppDbCnStr"
        lblAppDbCnStr.Size = New Size(133, 18)
        lblAppDbCnStr.TabIndex = 1
        lblAppDbCnStr.Text = "Verbindungszeichenfolge"
        ' 
        ' txtAppDbCnStr
        ' 
        txtAppDbCnStr.Enabled = False
        txtAppDbCnStr.Location = New Point(15, 187)
        txtAppDbCnStr.Name = "txtAppDbCnStr"
        txtAppDbCnStr.Size = New Size(265, 20)
        txtAppDbCnStr.TabIndex = 2
        ' 
        ' chkUseRegDbCnStr
        ' 
        chkUseRegDbCnStr.Enabled = False
        chkUseRegDbCnStr.Location = New Point(15, 151)
        chkUseRegDbCnStr.Name = "chkUseRegDbCnStr"
        chkUseRegDbCnStr.Size = New Size(241, 18)
        chkUseRegDbCnStr.TabIndex = 4
        chkUseRegDbCnStr.Text = "Verwende DB-Verbindung aus Registrierung"
        ' 
        ' butConfig
        ' 
        butConfig.Location = New Point(170, 109)
        butConfig.Name = "butConfig"
        butConfig.Size = New Size(110, 24)
        butConfig.TabIndex = 5
        butConfig.Text = "Konfiguration"
        ' 
        ' tmrStartUp
        ' 
        tmrStartUp.Interval = 1000
        ' 
        ' lblStatus
        ' 
        lblStatus.AutoSize = False
        lblStatus.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        lblStatus.Location = New Point(12, 85)
        lblStatus.Name = "lblStatus"
        lblStatus.Size = New Size(268, 18)
        lblStatus.TabIndex = 6
        lblStatus.Text = "Status"
        lblStatus.TextAlignment = ContentAlignment.MiddleCenter
        ' 
        ' RadPictureBox1
        ' 
        RadPictureBox1.Image = CType(resources.GetObject("RadPictureBox1.Image"), Image)
        RadPictureBox1.Location = New Point(12, 3)
        RadPictureBox1.Name = "RadPictureBox1"
        RadPictureBox1.Size = New Size(268, 76)
        RadPictureBox1.TabIndex = 7
        ' 
        ' StartUp
        ' 
        AutoScaleBaseSize = New Size(7, 15)
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.White
        ClientSize = New Size(292, 144)
        Controls.Add(RadPictureBox1)
        Controls.Add(lblStatus)
        Controls.Add(txtAppDbCnStr)
        Controls.Add(lblAppDbCnStr)
        Controls.Add(butConfig)
        Controls.Add(chkUseRegDbCnStr)
        Controls.Add(butStart)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "StartUp"
        Text = " "
        CType(butStart, ComponentModel.ISupportInitialize).EndInit()
        CType(lblAppDbCnStr, ComponentModel.ISupportInitialize).EndInit()
        CType(txtAppDbCnStr, ComponentModel.ISupportInitialize).EndInit()
        CType(chkUseRegDbCnStr, ComponentModel.ISupportInitialize).EndInit()
        CType(butConfig, ComponentModel.ISupportInitialize).EndInit()
        CType(lblStatus, ComponentModel.ISupportInitialize).EndInit()
        CType(RadPictureBox1, ComponentModel.ISupportInitialize).EndInit()
        CType(Me, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()

    End Sub

    Friend WithEvents butStart As Telerik.WinControls.UI.RadButton
    Friend WithEvents lblAppDbCnStr As Telerik.WinControls.UI.RadLabel
    Friend WithEvents txtAppDbCnStr As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents chkUseRegDbCnStr As Telerik.WinControls.UI.RadCheckBox
    Friend WithEvents butConfig As Telerik.WinControls.UI.RadButton
    Friend WithEvents tmrStartUp As Timer
    Friend WithEvents lblStatus As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadPictureBox1 As Telerik.WinControls.UI.RadPictureBox
End Class

