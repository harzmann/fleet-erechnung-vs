Option Strict On
Option Infer On

Imports System.Data
Imports System.Windows.Forms
Imports ehfleet_classlibrary

Partial Public Class ERechnungTaskPlanManagerForm
    Inherits Form

    Private ReadOnly _db As General.Database
    Private ReadOnly _repo As ERechnungTaskPlanRepository
    Private ReadOnly _ts As ERechnungTaskSchedulerFacade
    Private ReadOnly _runRepo As ERechnungTaskRunRepository

    Public Sub New(db As General.Database)
        InitializeComponent()

        _db = db
        _repo = New ERechnungTaskPlanRepository(_db)
        _ts = New ERechnungTaskSchedulerFacade(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
        _runRepo = New ERechnungTaskRunRepository(_db)

        ' Defaults
        cmbDomain.Items.Clear()
        cmbDomain.Items.AddRange(New Object() {"Rechnung", "Tankabrechnung", "ManuelleRechnung"})
        cmbDomain.SelectedIndex = 0

        cmbAction.Items.Clear()
        cmbAction.Items.AddRange(New Object() {"EMAIL_PDF", "EMAIL_XML", "EXPORT_PDF", "EXPORT_XML"})
        cmbAction.SelectedIndex = 0

        cmbScheduleType.Items.Clear()
        cmbScheduleType.Items.AddRange(New Object() {"MINUTELY", "HOURLY", "DAILY", "WEEKDAYS"})
        cmbScheduleType.SelectedItem = "DAILY"

        dtpStartTime.Format = DateTimePickerFormat.Time
        dtpStartTime.ShowUpDown = True
        dtpStartTime.Value = Date.Today.AddHours(2)

        ' Weekdays
        If clbWeekdays.Items.Count = 0 Then
            clbWeekdays.Items.AddRange(New Object() {"Mo", "Di", "Mi", "Do", "Fr", "Sa", "So"})
        End If

        LoadGrid()
        tmrStatus.Start()
    End Sub

    Private Sub LoadGrid()
        Dim dt = _repo.GetAll()
        gridTasks.AutoGenerateColumns = True 'False
        gridTasks.DataSource = dt

        If gridTasks.Rows.Count > 0 Then
            gridTasks.Rows(0).Selected = True
        End If

        RefreshStatus()
    End Sub

    Private Function TryGetTimeSpan(value As Object) As TimeSpan?
        If value Is Nothing OrElse value Is DBNull.Value Then Return Nothing

        If TypeOf value Is TimeSpan Then
            Return DirectCast(value, TimeSpan)
        End If

        If TypeOf value Is DateTime Then
            Return DirectCast(value, DateTime).TimeOfDay
        End If

        Dim s As String = Convert.ToString(value).Trim()
        If s.Length = 0 Then Return Nothing

        Dim ts As TimeSpan
        If TimeSpan.TryParse(s, ts) Then
            Return ts
        End If

        Dim dt As DateTime
        If DateTime.TryParse(s, dt) Then
            Return dt.TimeOfDay
        End If

        Return Nothing
    End Function

    Private Function CurrentTaskId() As Integer
        Dim id As Integer = 0
        If Integer.TryParse(If(txtTaskId.Text, "").Trim(), id) Then Return id

        If gridTasks.CurrentRow Is Nothing OrElse gridTasks.CurrentRow.DataBoundItem Is Nothing Then Return 0
        Dim drv = TryCast(gridTasks.CurrentRow.DataBoundItem, DataRowView)
        If drv Is Nothing Then Return 0
        Return Convert.ToInt32(drv.Row.Field(Of Integer)("TaskId"))
    End Function

    Private Sub gridTasks_SelectionChanged(sender As Object, e As EventArgs) Handles gridTasks.SelectionChanged
        If gridTasks.CurrentRow Is Nothing OrElse gridTasks.CurrentRow.DataBoundItem Is Nothing Then Return
        Dim drv = TryCast(gridTasks.CurrentRow.DataBoundItem, DataRowView)
        If drv Is Nothing Then Return
        Dim r = drv.Row

        txtTaskId.Text = r.Field(Of Integer)("TaskId").ToString()
        cmbDomain.Text = r.Field(Of String)("Domain")
        cmbAction.Text = r.Field(Of String)("Action")

        chkIncludeDuplicates.Checked = r.Field(Of Boolean)("IncludeDuplicates")
        chkOnlyNewSinceLastRun.Checked = r.Field(Of Boolean)("OnlyNewSinceLastRun")
        chkEnabled.Checked = r.Field(Of Boolean)("Enabled")

        txtTaskName.Text = If(r.IsNull("TaskName"), "", r.Field(Of String)("TaskName"))
        cmbScheduleType.SelectedItem = r.Field(Of String)("ScheduleType")
        numEveryN.Value = Math.Max(1D, Convert.ToDecimal(r.Field(Of Integer)("EveryN")))

        Dim tsOpt As TimeSpan? = TryGetTimeSpan(r("StartTime"))
        If tsOpt.HasValue Then
            dtpStartTime.Value = Date.Today.Add(tsOpt.Value)
        Else
            dtpStartTime.Value = Date.Today.AddHours(2)
        End If

        SetWeekdaysChecked(r.Field(Of Integer)("WeekdaysMask"))

        RefreshStatus()
    End Sub

    Private Sub btnNeu_Click(sender As Object, e As EventArgs) Handles btnNeu.Click
        ClearEditor()
    End Sub

    Private Sub ClearEditor()
        txtTaskId.Text = ""
        txtTaskName.Text = ""
        cmbDomain.SelectedItem = "Rechnung"
        cmbAction.Text = "EMAIL_PDF"
        chkIncludeDuplicates.Checked = False
        chkOnlyNewSinceLastRun.Checked = False
        chkEnabled.Checked = True
        cmbScheduleType.SelectedItem = "DAILY"
        numEveryN.Value = 1
        dtpStartTime.Value = Date.Today.AddHours(2)

        For i = 0 To clbWeekdays.Items.Count - 1
            clbWeekdays.SetItemChecked(i, False)
        Next

        lblSchedulerRunning.Text = "-"
        lblLastRunUtc.Text = "-"
        lblLastResult.Text = "-"
    End Sub

    Private Function ReadEditor() As ERechnungTaskPlanRow
        Dim x As New ERechnungTaskPlanRow()
        x.TaskId = If(String.IsNullOrWhiteSpace(If(txtTaskId.Text, "").Trim()), 0, Convert.ToInt32(txtTaskId.Text))
        x.TaskName = If(txtTaskName.Text, "").Trim()
        x.Domain = If(cmbDomain.SelectedItem, "").ToString().Trim()
        x.Action = If(cmbAction.SelectedItem, "").ToString().Trim()
        x.IncludeDuplicates = chkIncludeDuplicates.Checked
        x.OnlyNewSinceLastRun = chkOnlyNewSinceLastRun.Checked
        x.Enabled = chkEnabled.Checked
        x.ScheduleType = Convert.ToString(cmbScheduleType.SelectedItem)
        x.EveryN = Convert.ToInt32(numEveryN.Value)
        x.StartTime = dtpStartTime.Value.TimeOfDay
        x.WeekdaysMask = GetWeekdaysMask()
        Return x
    End Function

    Private Sub btnSpeichern_Click(sender As Object, e As EventArgs) Handles btnSpeichern.Click
        Dim plan = ReadEditor()

        If String.IsNullOrWhiteSpace(plan.Domain) OrElse String.IsNullOrWhiteSpace(plan.Action) Then
            MessageBox.Show("Domäne und Aktion dürfen nicht leer sein.", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If plan.TaskId = 0 Then
            plan.TaskId = _repo.Insert(plan)
            txtTaskId.Text = plan.TaskId.ToString()
        Else
            _repo.Update(plan)
        End If

        ' TaskName generieren/säubern + speichern
        plan.TaskName = _ts.EnsureTaskName(plan)
        txtTaskName.Text = plan.TaskName
        _repo.Update(plan)

        ' Nach dem Speichern: Windows-Task automatisch erstellen/aktualisieren
        Try
            If plan.Enabled Then
                _ts.EnsureWindowsTask(plan)   ' <- muss intern Trigger bauen + EnsureTask() rufen
            Else
                ' Optional: wenn deaktiviert, Windows-Task entfernen oder disabled setzen
                _ts.DeleteTask(plan)
            End If
        Catch ex As Exception
            ' Nur melden – DB ist bereits gespeichert
            MessageBox.Show("Windows-Task konnte nicht angelegt/aktualisiert werden:" & Environment.NewLine & ex.Message,
                    "Aufgabenplanung", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try

        LoadGrid()
        SelectRow(plan.TaskId)
        RefreshStatus()
    End Sub

    Private Sub btnWindowsTaskAktualisieren_Click(sender As Object, e As EventArgs) Handles btnWindowsTaskAktualisieren.Click
        Dim id = CurrentTaskId()
        If id = 0 Then
            MessageBox.Show("Bitte zuerst speichern, damit eine TaskId existiert.", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim plan = _repo.GetById(id)
        If plan Is Nothing Then Return

        plan.TaskName = _ts.EnsureTaskName(plan)
        _repo.Update(plan)

        _ts.EnsureWindowsTask(plan)
        RefreshStatus()
    End Sub

    Private Sub btnStarten_Click(sender As Object, e As EventArgs) Handles btnStarten.Click
        Dim id = CurrentTaskId()
        If id = 0 Then Return

        Dim plan = _repo.GetById(id)
        If plan Is Nothing Then Return

        ' Sicherstellen, dass Task registriert ist
        plan.TaskName = _ts.EnsureTaskName(plan)
        _repo.Update(plan)
        _ts.EnsureWindowsTask(plan)

        _ts.RunNow(plan)
        RefreshStatus()
    End Sub

    Private Sub btnBeenden_Click(sender As Object, e As EventArgs) Handles btnBeenden.Click
        Dim id = CurrentTaskId()
        If id = 0 Then Return
        Dim plan = _repo.GetById(id)
        If plan Is Nothing Then Return

        _ts.StopTask(plan)
        RefreshStatus()
    End Sub

    Private Sub btnLoeschen_Click(sender As Object, e As EventArgs) Handles btnLoeschen.Click
        Dim id = CurrentTaskId()
        If id = 0 Then Return

        If MessageBox.Show("Task wirklich löschen (inkl. Windows Task)?", "Bestätigung", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Return
        End If

        Dim plan = _repo.GetById(id)
        If plan IsNot Nothing Then
            Try
                _ts.StopTask(plan)
            Catch
            End Try
            Try
                _ts.DeleteTask(plan)
            Catch
            End Try
        End If

        _repo.Delete(id)
        LoadGrid()
        ClearEditor()
    End Sub

    Private Sub btnAktualisieren_Click(sender As Object, e As EventArgs) Handles btnAktualisieren.Click
        LoadGrid()
    End Sub

    Private Sub btnLogs_Click(sender As Object, e As EventArgs) Handles btnLogs.Click
        Dim plan = GetSelectedPlan()
        If plan Is Nothing Then Return

        Dim runs = _runRepo.GetRuns(plan.TaskId)
        If runs.Rows.Count = 0 Then
            MessageBox.Show("Für diese TaskId sind noch keine Runs vorhanden.", "Logs", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Default: letzter Run
        Dim runId As Integer = Convert.ToInt32(runs.Rows(0)("RunId"))

        ' OPTIONAL: wenn du eine Auswahl willst, kann man hier einfach per InputBox die RunId abfragen
        ' runId = Convert.ToInt32(InputBox("RunId auswählen:", "Logs", runId.ToString()))

        Dim entries = _runRepo.GetLogsAsEntries(runId, materializeValidatorHtmlToTemp:=True)
        Dim f As New ExportLogGridForm(entries)
        f.Text = $"Versand-Log (RunId={runId}, TaskId={plan.TaskId})"
        f.ShowDialog(Me)
    End Sub

    Private Sub tmrStatus_Tick(sender As Object, e As EventArgs) Handles tmrStatus.Tick
        RefreshStatus()
    End Sub

    Private Sub RefreshStatus()
        Dim id = CurrentTaskId()
        If id = 0 Then
            lblSchedulerRunning.Text = "-"
            Return
        End If

        Dim plan = _repo.GetById(id)
        If plan Is Nothing Then Return

        lblSchedulerRunning.Text = If(_ts.IsRunning(plan), "JA", "NEIN")

        lblLastRunUtc.Text = If(plan.LastRunUtc.HasValue, plan.LastRunUtc.Value.ToString("yyyy-MM-dd HH:mm:ss"), "-")
        lblLastResult.Text = If(plan.LastResult, "-")
    End Sub

    Private Sub SelectRow(taskId As Integer)
        For Each r As DataGridViewRow In gridTasks.Rows
            Dim drv = TryCast(r.DataBoundItem, DataRowView)
            If drv Is Nothing Then Continue For
            If Convert.ToInt32(drv.Row.Field(Of Integer)("TaskId")) = taskId Then
                r.Selected = True
                gridTasks.CurrentCell = r.Cells(0)
                Exit For
            End If
        Next
    End Sub

    Private Function GetWeekdaysMask() As Integer
        ' Bitmask: Mo=1, Di=2, Mi=4, Do=8, Fr=16, Sa=32, So=64
        Dim mask As Integer = 0
        For i = 0 To clbWeekdays.Items.Count - 1
            If clbWeekdays.GetItemChecked(i) Then
                mask = mask Or (1 << i)
            End If
        Next
        Return mask
    End Function

    Private Sub SetWeekdaysChecked(mask As Integer)
        For i = 0 To clbWeekdays.Items.Count - 1
            Dim isSet = (mask And (1 << i)) <> 0
            clbWeekdays.SetItemChecked(i, isSet)
        Next
    End Sub

    Private Function GetSelectedPlan() As ERechnungTaskPlanRow
        ' 1) Wenn Grid selektiert ist: TaskId aus Grid-Row lesen
        If gridTasks IsNot Nothing AndAlso gridTasks.SelectedRows IsNot Nothing AndAlso gridTasks.SelectedRows.Count > 0 Then
            Dim row = gridTasks.SelectedRows(0)
            If row IsNot Nothing AndAlso row.Cells IsNot Nothing Then
                Dim v = row.Cells(0).Value
                If v IsNot Nothing AndAlso v IsNot DBNull.Value Then
                    Dim id As Integer
                    If Integer.TryParse(Convert.ToString(v), id) Then
                        Return _repo.GetById(id)
                    End If
                End If
            End If
        End If

        ' 2) Fallback: wenn du ein Textfeld für TaskId hast (z.B. txtTaskId)
        If txtTaskId IsNot Nothing Then
            Dim id As Integer
            If Integer.TryParse(txtTaskId.Text, id) Then
                Return _repo.GetById(id)
            End If
        End If

        Return Nothing
    End Function

End Class