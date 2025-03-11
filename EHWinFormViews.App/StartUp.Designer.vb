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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(StartUp))
        Me.butStart = New Telerik.WinControls.UI.RadButton()
        Me.lblAppDbCnStr = New Telerik.WinControls.UI.RadLabel()
        Me.txtAppDbCnStr = New Telerik.WinControls.UI.RadTextBox()
        Me.chkUseRegDbCnStr = New Telerik.WinControls.UI.RadCheckBox()
        Me.butConfig = New Telerik.WinControls.UI.RadButton()
        Me.tmrStartUp = New System.Windows.Forms.Timer(Me.components)
        Me.lblStatus = New Telerik.WinControls.UI.RadLabel()
        Me.RadPictureBox1 = New Telerik.WinControls.UI.RadPictureBox()
        CType(Me.butStart, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lblAppDbCnStr, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtAppDbCnStr, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.chkUseRegDbCnStr, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.butConfig, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lblStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadPictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'butStart
        '
        Me.butStart.Location = New System.Drawing.Point(12, 109)
        Me.butStart.Name = "butStart"
        Me.butStart.Size = New System.Drawing.Size(110, 24)
        Me.butStart.TabIndex = 0
        Me.butStart.Text = "Start (5)"
        '
        'lblAppDbCnStr
        '
        Me.lblAppDbCnStr.Location = New System.Drawing.Point(15, 169)
        Me.lblAppDbCnStr.Name = "lblAppDbCnStr"
        Me.lblAppDbCnStr.Size = New System.Drawing.Size(133, 18)
        Me.lblAppDbCnStr.TabIndex = 1
        Me.lblAppDbCnStr.Text = "Verbindungszeichenfolge"
        '
        'txtAppDbCnStr
        '
        Me.txtAppDbCnStr.Enabled = False
        Me.txtAppDbCnStr.Location = New System.Drawing.Point(15, 187)
        Me.txtAppDbCnStr.Name = "txtAppDbCnStr"
        Me.txtAppDbCnStr.Size = New System.Drawing.Size(265, 20)
        Me.txtAppDbCnStr.TabIndex = 2
        '
        'chkUseRegDbCnStr
        '
        Me.chkUseRegDbCnStr.Enabled = False
        Me.chkUseRegDbCnStr.Location = New System.Drawing.Point(15, 151)
        Me.chkUseRegDbCnStr.Name = "chkUseRegDbCnStr"
        Me.chkUseRegDbCnStr.Size = New System.Drawing.Size(241, 18)
        Me.chkUseRegDbCnStr.TabIndex = 4
        Me.chkUseRegDbCnStr.Text = "Verwende DB-Verbindung aus Registrierung"
        '
        'butConfig
        '
        Me.butConfig.Location = New System.Drawing.Point(170, 109)
        Me.butConfig.Name = "butConfig"
        Me.butConfig.Size = New System.Drawing.Size(110, 24)
        Me.butConfig.TabIndex = 5
        Me.butConfig.Text = "Konfiguration"
        '
        'tmrStartUp
        '
        Me.tmrStartUp.Interval = 1000
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = False
        Me.lblStatus.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStatus.Location = New System.Drawing.Point(12, 85)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(268, 18)
        Me.lblStatus.TabIndex = 6
        Me.lblStatus.Text = "Status"
        Me.lblStatus.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        '
        'RadPictureBox1
        '
        Me.RadPictureBox1.Image = CType(resources.GetObject("RadPictureBox1.Image"), System.Drawing.Image)
        Me.RadPictureBox1.Location = New System.Drawing.Point(12, 3)
        Me.RadPictureBox1.Name = "RadPictureBox1"
        Me.RadPictureBox1.Size = New System.Drawing.Size(268, 76)
        Me.RadPictureBox1.TabIndex = 7
        '
        'StartUp
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(8, 16)
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(292, 144)
        Me.Controls.Add(Me.RadPictureBox1)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.txtAppDbCnStr)
        Me.Controls.Add(Me.lblAppDbCnStr)
        Me.Controls.Add(Me.butConfig)
        Me.Controls.Add(Me.chkUseRegDbCnStr)
        Me.Controls.Add(Me.butStart)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "StartUp"
        Me.Text = "FLEET XRechnung"
        CType(Me.butStart, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lblAppDbCnStr, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtAppDbCnStr, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.chkUseRegDbCnStr, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.butConfig, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lblStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadPictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

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

