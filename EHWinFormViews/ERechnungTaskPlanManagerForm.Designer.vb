Option Strict On
Option Infer On

Imports System.ComponentModel
Imports System.Windows.Forms

Partial Class ERechnungTaskPlanManagerForm

    Private components As IContainer

    Friend WithEvents splitMain As SplitContainer
    Friend WithEvents gridTasks As DataGridView

    Friend WithEvents grpDetails As GroupBox
    Friend WithEvents lblTaskId As Label
    Friend WithEvents txtTaskId As TextBox
    Friend WithEvents lblTaskName As Label
    Friend WithEvents txtTaskName As TextBox
    Friend WithEvents lblDomain As Label
    Friend WithEvents cmbDomain As ComboBox
    Friend WithEvents lblAction As Label
    Friend WithEvents cmbAction As ComboBox
    Friend WithEvents chkEnabled As CheckBox

    Friend WithEvents lblScheduleType As Label
    Friend WithEvents cmbScheduleType As ComboBox
    Friend WithEvents lblEveryN As Label
    Friend WithEvents numEveryN As NumericUpDown
    Friend WithEvents lblStartTime As Label
    Friend WithEvents dtpStartTime As DateTimePicker
    Friend WithEvents lblWeekdays As Label
    Friend WithEvents clbWeekdays As CheckedListBox

    Friend WithEvents grpStatus As GroupBox
    Friend WithEvents lblSchedulerRunningTitle As Label
    Friend WithEvents lblSchedulerRunning As Label
    Friend WithEvents lblLastRunTitle As Label
    Friend WithEvents lblLastRunUtc As Label
    Friend WithEvents lblLastResultTitle As Label
    Friend WithEvents lblLastResult As Label

    Friend WithEvents pnlButtons As FlowLayoutPanel
    Friend WithEvents btnNeu As Button
    Friend WithEvents btnSpeichern As Button
    Friend WithEvents btnLoeschen As Button
    Friend WithEvents btnWindowsTaskAktualisieren As Button
    Friend WithEvents btnStarten As Button
    Friend WithEvents btnBeenden As Button
    Friend WithEvents btnAktualisieren As Button
    Friend WithEvents btnLogs As Button

    Friend WithEvents tmrStatus As Timer

    <DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    <DebuggerNonUserCode()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ERechnungTaskPlanManagerForm))
        Me.splitMain = New System.Windows.Forms.SplitContainer()
        Me.gridTasks = New System.Windows.Forms.DataGridView()
        Me.pnlButtons = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnNeu = New System.Windows.Forms.Button()
        Me.btnSpeichern = New System.Windows.Forms.Button()
        Me.btnLoeschen = New System.Windows.Forms.Button()
        Me.btnWindowsTaskAktualisieren = New System.Windows.Forms.Button()
        Me.btnStarten = New System.Windows.Forms.Button()
        Me.btnBeenden = New System.Windows.Forms.Button()
        Me.btnAktualisieren = New System.Windows.Forms.Button()
        Me.btnLogs = New System.Windows.Forms.Button()
        Me.grpStatus = New System.Windows.Forms.GroupBox()
        Me.lblSchedulerRunningTitle = New System.Windows.Forms.Label()
        Me.lblSchedulerRunning = New System.Windows.Forms.Label()
        Me.lblLastRunTitle = New System.Windows.Forms.Label()
        Me.lblLastRunUtc = New System.Windows.Forms.Label()
        Me.lblLastResultTitle = New System.Windows.Forms.Label()
        Me.lblLastResult = New System.Windows.Forms.Label()
        Me.grpDetails = New System.Windows.Forms.GroupBox()
        Me.lblTaskId = New System.Windows.Forms.Label()
        Me.txtTaskId = New System.Windows.Forms.TextBox()
        Me.lblTaskName = New System.Windows.Forms.Label()
        Me.txtTaskName = New System.Windows.Forms.TextBox()
        Me.lblDomain = New System.Windows.Forms.Label()
        Me.cmbDomain = New System.Windows.Forms.ComboBox()
        Me.lblAction = New System.Windows.Forms.Label()
        Me.cmbAction = New System.Windows.Forms.ComboBox()
        Me.chkEnabled = New System.Windows.Forms.CheckBox()
        Me.lblScheduleType = New System.Windows.Forms.Label()
        Me.cmbScheduleType = New System.Windows.Forms.ComboBox()
        Me.lblEveryN = New System.Windows.Forms.Label()
        Me.numEveryN = New System.Windows.Forms.NumericUpDown()
        Me.lblStartTime = New System.Windows.Forms.Label()
        Me.dtpStartTime = New System.Windows.Forms.DateTimePicker()
        Me.lblWeekdays = New System.Windows.Forms.Label()
        Me.clbWeekdays = New System.Windows.Forms.CheckedListBox()
        Me.tmrStatus = New System.Windows.Forms.Timer(Me.components)
        CType(Me.splitMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitMain.Panel1.SuspendLayout()
        Me.splitMain.Panel2.SuspendLayout()
        Me.splitMain.SuspendLayout()
        CType(Me.gridTasks, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlButtons.SuspendLayout()
        Me.grpStatus.SuspendLayout()
        Me.grpDetails.SuspendLayout()
        CType(Me.numEveryN, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'splitMain
        '
        Me.splitMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitMain.Location = New System.Drawing.Point(0, 0)
        Me.splitMain.Name = "splitMain"
        '
        'splitMain.Panel1
        '
        Me.splitMain.Panel1.Controls.Add(Me.gridTasks)
        '
        'splitMain.Panel2
        '
        Me.splitMain.Panel2.Controls.Add(Me.pnlButtons)
        Me.splitMain.Panel2.Controls.Add(Me.grpStatus)
        Me.splitMain.Panel2.Controls.Add(Me.grpDetails)
        Me.splitMain.Size = New System.Drawing.Size(1250, 720)
        Me.splitMain.SplitterDistance = 670
        Me.splitMain.TabIndex = 0
        '
        'gridTasks
        '
        Me.gridTasks.AllowUserToAddRows = False
        Me.gridTasks.AllowUserToDeleteRows = False
        Me.gridTasks.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridTasks.Location = New System.Drawing.Point(0, 0)
        Me.gridTasks.MultiSelect = False
        Me.gridTasks.Name = "gridTasks"
        Me.gridTasks.ReadOnly = True
        Me.gridTasks.RowHeadersVisible = False
        Me.gridTasks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.gridTasks.Size = New System.Drawing.Size(670, 720)
        Me.gridTasks.TabIndex = 0
        '
        'pnlButtons
        '
        Me.pnlButtons.Controls.Add(Me.btnNeu)
        Me.pnlButtons.Controls.Add(Me.btnSpeichern)
        Me.pnlButtons.Controls.Add(Me.btnLoeschen)
        Me.pnlButtons.Controls.Add(Me.btnStarten)
        Me.pnlButtons.Controls.Add(Me.btnBeenden)
        Me.pnlButtons.Controls.Add(Me.btnAktualisieren)
        Me.pnlButtons.Controls.Add(Me.btnLogs)
        Me.pnlButtons.Controls.Add(Me.btnWindowsTaskAktualisieren)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlButtons.Location = New System.Drawing.Point(0, 660)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Size = New System.Drawing.Size(576, 60)
        Me.pnlButtons.TabIndex = 0
        '
        'btnNeu
        '
        Me.btnNeu.Location = New System.Drawing.Point(3, 3)
        Me.btnNeu.Name = "btnNeu"
        Me.btnNeu.Size = New System.Drawing.Size(100, 23)
        Me.btnNeu.TabIndex = 0
        Me.btnNeu.Text = "Neu"
        '
        'btnSpeichern
        '
        Me.btnSpeichern.Location = New System.Drawing.Point(109, 3)
        Me.btnSpeichern.Name = "btnSpeichern"
        Me.btnSpeichern.Size = New System.Drawing.Size(100, 23)
        Me.btnSpeichern.TabIndex = 1
        Me.btnSpeichern.Text = "Speichern"
        '
        'btnLoeschen
        '
        Me.btnLoeschen.Location = New System.Drawing.Point(215, 3)
        Me.btnLoeschen.Name = "btnLoeschen"
        Me.btnLoeschen.Size = New System.Drawing.Size(100, 23)
        Me.btnLoeschen.TabIndex = 2
        Me.btnLoeschen.Text = "Löschen"
        '
        'btnWindowsTaskAktualisieren
        '
        Me.btnWindowsTaskAktualisieren.Location = New System.Drawing.Point(215, 32)
        Me.btnWindowsTaskAktualisieren.Name = "btnWindowsTaskAktualisieren"
        Me.btnWindowsTaskAktualisieren.Size = New System.Drawing.Size(100, 23)
        Me.btnWindowsTaskAktualisieren.TabIndex = 3
        Me.btnWindowsTaskAktualisieren.Text = "Windows-Task aktualisieren"
        '
        'btnStarten
        '
        Me.btnStarten.Location = New System.Drawing.Point(321, 3)
        Me.btnStarten.Name = "btnStarten"
        Me.btnStarten.Size = New System.Drawing.Size(100, 23)
        Me.btnStarten.TabIndex = 4
        Me.btnStarten.Text = "Starten"
        '
        'btnBeenden
        '
        Me.btnBeenden.Location = New System.Drawing.Point(427, 3)
        Me.btnBeenden.Name = "btnBeenden"
        Me.btnBeenden.Size = New System.Drawing.Size(100, 23)
        Me.btnBeenden.TabIndex = 5
        Me.btnBeenden.Text = "Beenden"
        '
        'btnAktualisieren
        '
        Me.btnAktualisieren.Location = New System.Drawing.Point(3, 32)
        Me.btnAktualisieren.Name = "btnAktualisieren"
        Me.btnAktualisieren.Size = New System.Drawing.Size(100, 23)
        Me.btnAktualisieren.TabIndex = 6
        Me.btnAktualisieren.Text = "Aktualisieren"
        '
        'btnLogs
        '
        Me.btnLogs.Location = New System.Drawing.Point(109, 32)
        Me.btnLogs.Name = "btnLogs"
        Me.btnLogs.Size = New System.Drawing.Size(100, 23)
        Me.btnLogs.TabIndex = 7
        Me.btnLogs.Text = "Logs"
        '
        'grpStatus
        '
        Me.grpStatus.Controls.Add(Me.lblSchedulerRunningTitle)
        Me.grpStatus.Controls.Add(Me.lblSchedulerRunning)
        Me.grpStatus.Controls.Add(Me.lblLastRunTitle)
        Me.grpStatus.Controls.Add(Me.lblLastRunUtc)
        Me.grpStatus.Controls.Add(Me.lblLastResultTitle)
        Me.grpStatus.Controls.Add(Me.lblLastResult)
        Me.grpStatus.Dock = System.Windows.Forms.DockStyle.Top
        Me.grpStatus.Location = New System.Drawing.Point(0, 415)
        Me.grpStatus.Name = "grpStatus"
        Me.grpStatus.Size = New System.Drawing.Size(576, 140)
        Me.grpStatus.TabIndex = 1
        Me.grpStatus.TabStop = False
        Me.grpStatus.Text = "Status"
        '
        'lblSchedulerRunningTitle
        '
        Me.lblSchedulerRunningTitle.AutoSize = True
        Me.lblSchedulerRunningTitle.Location = New System.Drawing.Point(14, 28)
        Me.lblSchedulerRunningTitle.Name = "lblSchedulerRunningTitle"
        Me.lblSchedulerRunningTitle.Size = New System.Drawing.Size(114, 13)
        Me.lblSchedulerRunningTitle.TabIndex = 0
        Me.lblSchedulerRunningTitle.Text = "Task läuft (Scheduler):"
        '
        'lblSchedulerRunning
        '
        Me.lblSchedulerRunning.AutoSize = True
        Me.lblSchedulerRunning.Location = New System.Drawing.Point(170, 28)
        Me.lblSchedulerRunning.Name = "lblSchedulerRunning"
        Me.lblSchedulerRunning.Size = New System.Drawing.Size(10, 13)
        Me.lblSchedulerRunning.TabIndex = 1
        Me.lblSchedulerRunning.Text = "-"
        '
        'lblLastRunTitle
        '
        Me.lblLastRunTitle.AutoSize = True
        Me.lblLastRunTitle.Location = New System.Drawing.Point(14, 58)
        Me.lblLastRunTitle.Name = "lblLastRunTitle"
        Me.lblLastRunTitle.Size = New System.Drawing.Size(97, 13)
        Me.lblLastRunTitle.TabIndex = 2
        Me.lblLastRunTitle.Text = "Letzter Lauf (UTC):"
        '
        'lblLastRunUtc
        '
        Me.lblLastRunUtc.AutoSize = True
        Me.lblLastRunUtc.Location = New System.Drawing.Point(170, 58)
        Me.lblLastRunUtc.Name = "lblLastRunUtc"
        Me.lblLastRunUtc.Size = New System.Drawing.Size(10, 13)
        Me.lblLastRunUtc.TabIndex = 3
        Me.lblLastRunUtc.Text = "-"
        '
        'lblLastResultTitle
        '
        Me.lblLastResultTitle.AutoSize = True
        Me.lblLastResultTitle.Location = New System.Drawing.Point(14, 88)
        Me.lblLastResultTitle.Name = "lblLastResultTitle"
        Me.lblLastResultTitle.Size = New System.Drawing.Size(88, 13)
        Me.lblLastResultTitle.TabIndex = 4
        Me.lblLastResultTitle.Text = "Letztes Ergebnis:"
        '
        'lblLastResult
        '
        Me.lblLastResult.AutoSize = True
        Me.lblLastResult.Location = New System.Drawing.Point(170, 88)
        Me.lblLastResult.Name = "lblLastResult"
        Me.lblLastResult.Size = New System.Drawing.Size(10, 13)
        Me.lblLastResult.TabIndex = 5
        Me.lblLastResult.Text = "-"
        '
        'grpDetails
        '
        Me.grpDetails.Controls.Add(Me.lblTaskId)
        Me.grpDetails.Controls.Add(Me.txtTaskId)
        Me.grpDetails.Controls.Add(Me.lblTaskName)
        Me.grpDetails.Controls.Add(Me.txtTaskName)
        Me.grpDetails.Controls.Add(Me.lblDomain)
        Me.grpDetails.Controls.Add(Me.cmbDomain)
        Me.grpDetails.Controls.Add(Me.lblAction)
        Me.grpDetails.Controls.Add(Me.cmbAction)
        Me.grpDetails.Controls.Add(Me.chkEnabled)
        Me.grpDetails.Controls.Add(Me.lblScheduleType)
        Me.grpDetails.Controls.Add(Me.cmbScheduleType)
        Me.grpDetails.Controls.Add(Me.lblEveryN)
        Me.grpDetails.Controls.Add(Me.numEveryN)
        Me.grpDetails.Controls.Add(Me.lblStartTime)
        Me.grpDetails.Controls.Add(Me.dtpStartTime)
        Me.grpDetails.Controls.Add(Me.lblWeekdays)
        Me.grpDetails.Controls.Add(Me.clbWeekdays)
        Me.grpDetails.Dock = System.Windows.Forms.DockStyle.Top
        Me.grpDetails.Location = New System.Drawing.Point(0, 0)
        Me.grpDetails.Name = "grpDetails"
        Me.grpDetails.Size = New System.Drawing.Size(576, 415)
        Me.grpDetails.TabIndex = 2
        Me.grpDetails.TabStop = False
        Me.grpDetails.Text = "Task-Details"
        '
        'lblTaskId
        '
        Me.lblTaskId.AutoSize = True
        Me.lblTaskId.Location = New System.Drawing.Point(14, 28)
        Me.lblTaskId.Name = "lblTaskId"
        Me.lblTaskId.Size = New System.Drawing.Size(48, 13)
        Me.lblTaskId.TabIndex = 0
        Me.lblTaskId.Text = "Task-ID:"
        '
        'txtTaskId
        '
        Me.txtTaskId.Location = New System.Drawing.Point(170, 24)
        Me.txtTaskId.Name = "txtTaskId"
        Me.txtTaskId.ReadOnly = True
        Me.txtTaskId.Size = New System.Drawing.Size(120, 20)
        Me.txtTaskId.TabIndex = 1
        '
        'lblTaskName
        '
        Me.lblTaskName.AutoSize = True
        Me.lblTaskName.Location = New System.Drawing.Point(14, 62)
        Me.lblTaskName.Name = "lblTaskName"
        Me.lblTaskName.Size = New System.Drawing.Size(60, 13)
        Me.lblTaskName.TabIndex = 2
        Me.lblTaskName.Text = "Taskname:"
        '
        'txtTaskName
        '
        Me.txtTaskName.Location = New System.Drawing.Point(170, 58)
        Me.txtTaskName.Name = "txtTaskName"
        Me.txtTaskName.Size = New System.Drawing.Size(360, 20)
        Me.txtTaskName.TabIndex = 3
        '
        'lblDomain
        '
        Me.lblDomain.AutoSize = True
        Me.lblDomain.Location = New System.Drawing.Point(14, 96)
        Me.lblDomain.Name = "lblDomain"
        Me.lblDomain.Size = New System.Drawing.Size(50, 13)
        Me.lblDomain.TabIndex = 4
        Me.lblDomain.Text = "Domäne:"
        '
        'cmbDomain
        '
        Me.cmbDomain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbDomain.Location = New System.Drawing.Point(170, 92)
        Me.cmbDomain.Name = "cmbDomain"
        Me.cmbDomain.Size = New System.Drawing.Size(200, 21)
        Me.cmbDomain.TabIndex = 5
        '
        'lblAction
        '
        Me.lblAction.AutoSize = True
        Me.lblAction.Location = New System.Drawing.Point(14, 130)
        Me.lblAction.Name = "lblAction"
        Me.lblAction.Size = New System.Drawing.Size(40, 13)
        Me.lblAction.TabIndex = 6
        Me.lblAction.Text = "Aktion:"
        '
        'cmbAction
        '
        Me.cmbAction.Location = New System.Drawing.Point(170, 126)
        Me.cmbAction.Name = "cmbAction"
        Me.cmbAction.Size = New System.Drawing.Size(200, 21)
        Me.cmbAction.TabIndex = 7
        '
        'chkEnabled
        '
        Me.chkEnabled.AutoSize = True
        Me.chkEnabled.Location = New System.Drawing.Point(170, 160)
        Me.chkEnabled.Name = "chkEnabled"
        Me.chkEnabled.Size = New System.Drawing.Size(50, 17)
        Me.chkEnabled.TabIndex = 8
        Me.chkEnabled.Text = "Aktiv"
        '
        'lblScheduleType
        '
        Me.lblScheduleType.AutoSize = True
        Me.lblScheduleType.Location = New System.Drawing.Point(14, 194)
        Me.lblScheduleType.Name = "lblScheduleType"
        Me.lblScheduleType.Size = New System.Drawing.Size(48, 13)
        Me.lblScheduleType.TabIndex = 9
        Me.lblScheduleType.Text = "Zeitplan:"
        '
        'cmbScheduleType
        '
        Me.cmbScheduleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbScheduleType.Location = New System.Drawing.Point(170, 190)
        Me.cmbScheduleType.Name = "cmbScheduleType"
        Me.cmbScheduleType.Size = New System.Drawing.Size(200, 21)
        Me.cmbScheduleType.TabIndex = 10
        '
        'lblEveryN
        '
        Me.lblEveryN.AutoSize = True
        Me.lblEveryN.Location = New System.Drawing.Point(14, 228)
        Me.lblEveryN.Name = "lblEveryN"
        Me.lblEveryN.Size = New System.Drawing.Size(64, 13)
        Me.lblEveryN.TabIndex = 11
        Me.lblEveryN.Text = "Intervall (N):"
        '
        'numEveryN
        '
        Me.numEveryN.Location = New System.Drawing.Point(170, 224)
        Me.numEveryN.Maximum = New Decimal(New Integer() {999, 0, 0, 0})
        Me.numEveryN.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.numEveryN.Name = "numEveryN"
        Me.numEveryN.Size = New System.Drawing.Size(80, 20)
        Me.numEveryN.TabIndex = 12
        Me.numEveryN.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'lblStartTime
        '
        Me.lblStartTime.AutoSize = True
        Me.lblStartTime.Location = New System.Drawing.Point(14, 262)
        Me.lblStartTime.Name = "lblStartTime"
        Me.lblStartTime.Size = New System.Drawing.Size(48, 13)
        Me.lblStartTime.TabIndex = 13
        Me.lblStartTime.Text = "Startzeit:"
        '
        'dtpStartTime
        '
        Me.dtpStartTime.Location = New System.Drawing.Point(170, 258)
        Me.dtpStartTime.Name = "dtpStartTime"
        Me.dtpStartTime.Size = New System.Drawing.Size(200, 20)
        Me.dtpStartTime.TabIndex = 14
        '
        'lblWeekdays
        '
        Me.lblWeekdays.AutoSize = True
        Me.lblWeekdays.Location = New System.Drawing.Point(14, 292)
        Me.lblWeekdays.Name = "lblWeekdays"
        Me.lblWeekdays.Size = New System.Drawing.Size(72, 13)
        Me.lblWeekdays.TabIndex = 15
        Me.lblWeekdays.Text = "Wochentage:"
        '
        'clbWeekdays
        '
        Me.clbWeekdays.Location = New System.Drawing.Point(170, 292)
        Me.clbWeekdays.Name = "clbWeekdays"
        Me.clbWeekdays.Size = New System.Drawing.Size(200, 109)
        Me.clbWeekdays.TabIndex = 16
        '
        'tmrStatus
        '
        Me.tmrStatus.Interval = 2000
        '
        'ERechnungTaskPlanManagerForm
        '
        Me.ClientSize = New System.Drawing.Size(1250, 720)
        Me.Controls.Add(Me.splitMain)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "ERechnungTaskPlanManagerForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "E-Rechnung Task Manager"
        Me.splitMain.Panel1.ResumeLayout(False)
        Me.splitMain.Panel2.ResumeLayout(False)
        CType(Me.splitMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitMain.ResumeLayout(False)
        CType(Me.gridTasks, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlButtons.ResumeLayout(False)
        Me.grpStatus.ResumeLayout(False)
        Me.grpStatus.PerformLayout()
        Me.grpDetails.ResumeLayout(False)
        Me.grpDetails.PerformLayout()
        CType(Me.numEveryN, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

End Class
