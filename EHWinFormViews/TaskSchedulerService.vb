Imports log4net
Imports Microsoft.Win32.TaskScheduler
Imports System.IO

Public Class TaskSchedulerService
    Private ReadOnly _folderName As String
    Private ReadOnly _exePath As String

    Private Shared ReadOnly _logger As ILog = LogManager.GetLogger(GetType(TaskSchedulerService))

    Public Sub New(exePath As String, Optional folderName As String = "\EHFleetXRechnung")
        _exePath = exePath
        _folderName = folderName
    End Sub

    Public Function EnsureTask(taskName As String,
                               description As String,
                               args As String,
                               trigger As Trigger,
                               Optional runAsHighest As Boolean = True) As String

        _logger.Info($"EnsureTask START: {taskName}")
        _logger.Debug($"ExePath={_exePath}")
        _logger.Debug($"Args={args}")
        _logger.Debug($"Folder={_folderName}")

        If Not File.Exists(_exePath) Then
            _logger.Error("EXE nicht gefunden: " & _exePath)
            Throw New FileNotFoundException("EXE nicht gefunden", _exePath)
        End If

        Dim workDir As String = Path.GetDirectoryName(_exePath)
        _logger.Debug($"WorkingDirectory={workDir}")

        Using ts As New TaskService()
            Dim folder = GetOrCreateFolder(ts, _folderName)

            Dim td = ts.NewTask()
            td.RegistrationInfo.Description = description

            td.Principal.LogonType = TaskLogonType.InteractiveToken
            If runAsHighest Then
                td.Principal.RunLevel = TaskRunLevel.Highest
            End If

            td.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew
            td.Settings.StopIfGoingOnBatteries = False
            td.Settings.DisallowStartIfOnBatteries = False
            td.Settings.StartWhenAvailable = True

            td.Triggers.Clear()
            td.Triggers.Add(trigger)

            td.Actions.Clear()
            ' WICHTIG: "Start in" / WorkingDirectory setzen (sonst oft 0x3)
            td.Actions.Add(New ExecAction(_exePath, args, workDir))

            Dim regTask = folder.RegisterTaskDefinition(
                taskName,
                td,
                TaskCreation.CreateOrUpdate,
                Nothing,
                Nothing,
                td.Principal.LogonType)

            _logger.Info($"EnsureTask DONE: {regTask.Path}")
            Return regTask.Path
        End Using
    End Function

    Public Sub RunNow(taskPathOrName As String)
        _logger.Info("RunNow: " & taskPathOrName)
        Using ts As New TaskService()
            Dim t = FindTask(ts, taskPathOrName)
            If t Is Nothing Then
                _logger.Warn("Task nicht gefunden: " & taskPathOrName)
                Throw New Exception("Task nicht gefunden: " & taskPathOrName)
            End If
            t.Run()
        End Using
    End Sub

    Public Sub StopTask(taskPathOrName As String)
        _logger.Info("StopTask: " & taskPathOrName)
        Using ts As New TaskService()
            Dim t = FindTask(ts, taskPathOrName)
            If t Is Nothing Then
                _logger.Warn("Task nicht gefunden: " & taskPathOrName)
                Throw New Exception("Task nicht gefunden: " & taskPathOrName)
            End If
            t.Stop()
        End Using
    End Sub

    Public Function IsRunning(taskPathOrName As String) As Boolean
        Try
            Using ts As New TaskService()
                Dim t = FindTask(ts, taskPathOrName)
                If t Is Nothing Then Return False
                Return t.State = TaskState.Running
            End Using
        Catch ex As Exception
            _logger.Warn("IsRunning failed for " & taskPathOrName, ex)
            Return False
        End Try
    End Function

    ' 🔄 Snapshot (State, LastRunTime, LastTaskResult)
    Public Function GetSnapshot(taskPathOrName As String) As TaskSnapshot
        Using ts As New TaskService()
            Dim t = FindTask(ts, taskPathOrName)
            If t Is Nothing Then Return Nothing

            Dim s As New TaskSnapshot()
            s.Path = t.Path
            s.Name = t.Name
            s.State = t.State.ToString()
            s.LastRunTime = If(t.LastRunTime = DateTime.MinValue, CType(Nothing, DateTime?), t.LastRunTime)
            s.LastTaskResult = t.LastTaskResult
            s.NextRunTime = If(t.NextRunTime = DateTime.MinValue, CType(Nothing, DateTime?), t.NextRunTime)
            Return s
        End Using
    End Function

    Public Function BuildTrigger(scheduleType As String,
                                 everyN As Integer,
                                 startTime As TimeSpan,
                                 weekdaysMask As Integer) As Trigger

        scheduleType = If(scheduleType, "").Trim().ToUpperInvariant()

        Select Case scheduleType
            Case "MINUTELY"
                Dim trig = New TimeTrigger()
                trig.StartBoundary = DateTime.Now.AddMinutes(1)
                trig.Repetition.Interval = TimeSpan.FromMinutes(Math.Max(1, everyN))
                trig.Repetition.Duration = TimeSpan.FromDays(3650)
                Return trig

            Case "HOURLY"
                Dim trig = New TimeTrigger()
                trig.StartBoundary = DateTime.Now.AddMinutes(1)
                trig.Repetition.Interval = TimeSpan.FromHours(Math.Max(1, everyN))
                trig.Repetition.Duration = TimeSpan.FromDays(3650)
                Return trig

            Case "DAILY"
                Dim dt As New DailyTrigger()
                dt.DaysInterval = Math.Max(1, everyN)
                dt.StartBoundary = DateTime.Today.Add(startTime)
                If dt.StartBoundary < DateTime.Now Then dt.StartBoundary = dt.StartBoundary.AddDays(1)
                Return dt

            Case "WEEKDAYS"
                Dim wt As New WeeklyTrigger()
                wt.WeeksInterval = Math.Max(1, everyN)
                wt.StartBoundary = DateTime.Today.Add(startTime)
                If wt.StartBoundary < DateTime.Now Then wt.StartBoundary = wt.StartBoundary.AddDays(1)
                wt.DaysOfWeek = DecodeDaysOfWeek(weekdaysMask)
                Return wt

            Case Else
                Throw New Exception("Unbekannter ScheduleType: " & scheduleType)
        End Select
    End Function

    Private Function DecodeDaysOfWeek(mask As Integer) As DaysOfTheWeek
        Dim d As DaysOfTheWeek = 0
        If (mask And 1) <> 0 Then d = d Or DaysOfTheWeek.Monday
        If (mask And 2) <> 0 Then d = d Or DaysOfTheWeek.Tuesday
        If (mask And 4) <> 0 Then d = d Or DaysOfTheWeek.Wednesday
        If (mask And 8) <> 0 Then d = d Or DaysOfTheWeek.Thursday
        If (mask And 16) <> 0 Then d = d Or DaysOfTheWeek.Friday
        If (mask And 32) <> 0 Then d = d Or DaysOfTheWeek.Saturday
        If (mask And 64) <> 0 Then d = d Or DaysOfTheWeek.Sunday
        Return d
    End Function

    Public Sub DeleteTask(taskPathOrName As String)
        _logger.Info("DeleteTask: " & taskPathOrName)
        Using ts As New TaskService()
            Dim folder = GetOrCreateFolder(ts, _folderName)
            Dim t = FindTask(ts, taskPathOrName)
            If t Is Nothing Then Return
            folder.DeleteTask(t.Name, False)
        End Using
    End Sub

    Private Function GetOrCreateFolder(ts As TaskService, folderPath As String) As TaskFolder
        If String.IsNullOrWhiteSpace(folderPath) Then Return ts.RootFolder

        ' Normalize: "\EHFleetXRechnung"  (root beginnt immer mit \)
        Dim path As String = folderPath.Trim()
        If Not path.StartsWith("\") Then path = "\" & path

        ' GetFolder wirft Exception wenn nicht vorhanden -> abfangen
        Try
            Dim existing = ts.GetFolder(path)
            If existing IsNot Nothing Then Return existing
        Catch ex As Exception
            _logger.Debug("GetFolder not found: " & path, ex)
        End Try

        ' Ordner erstellen (nur 1 Ebene, wie "\EHFleetXRechnung")
        Dim name = path.Trim("\"c)
        Try
            Return ts.RootFolder.CreateFolder(name)
        Catch ex As Exception
            ' Kann parallel erstellt worden sein -> nochmal versuchen
            _logger.Warn("CreateFolder failed, retry GetFolder: " & path, ex)
            Return ts.GetFolder(path)
        End Try
    End Function

    Private Function FindTask(ts As TaskService, taskPathOrName As String) As Task
        If String.IsNullOrWhiteSpace(taskPathOrName) Then Return Nothing

        Dim key As String = taskPathOrName.Trim()

        ' Wenn voller Pfad ("\EHFleetXRechnung\Task-XYZ") → direkt
        If key.StartsWith("\") Then
            Try
                Return ts.GetTask(key)
            Catch ex As Exception
                _logger.Debug("GetTask failed: " & key, ex)
                Return Nothing
            End Try
        End If

        ' Sonst im configured folder suchen
        Dim folder As TaskFolder = Nothing
        Try
            folder = GetOrCreateFolder(ts, _folderName)
        Catch ex As Exception
            _logger.Debug("Folder access failed: " & _folderName, ex)
            Return Nothing
        End Try

        Try
            ' folder.Tasks("name") kann werfen, wenn nicht vorhanden
            Return folder.Tasks(key)
        Catch ex As Exception
            _logger.Debug("Task not found in folder: " & key, ex)
            Return Nothing
        End Try
    End Function

End Class

Public Class TaskSnapshot
    Public Property Path As String
    Public Property Name As String
    Public Property State As String
    Public Property LastRunTime As DateTime?
    Public Property NextRunTime As DateTime?
    Public Property LastTaskResult As Integer
End Class
