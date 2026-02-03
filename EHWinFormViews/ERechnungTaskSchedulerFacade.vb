Imports System
Imports Microsoft.Win32.TaskScheduler

' ==========================================================
' Facade zwischen UI / DB und TaskSchedulerService
' ==========================================================
Public Class ERechnungTaskSchedulerFacade

    Private ReadOnly _svc As TaskSchedulerService

    Public Sub New(exePath As String)
        _svc = New TaskSchedulerService(exePath)
    End Sub

    ' ----------------------------------------------------------
    ' Windows Task anlegen / aktualisieren
    ' ----------------------------------------------------------
    Public Sub EnsureWindowsTask(plan As ERechnungTaskPlanRow)
        If plan Is Nothing Then Throw New ArgumentNullException(NameOf(plan))

        Dim taskName As String = EnsureTaskName(plan)
        Dim args As String = "--run-task " & plan.TaskId.ToString()

        Dim trigger As Trigger =
            _svc.BuildTrigger(
                plan.ScheduleType,
                plan.EveryN,
                plan.StartTime,
                plan.WeekdaysMask
            )

        Dim description As String =
            $"EHFleet ERechnung Dispatch | TaskId={plan.TaskId} | Domain={plan.Domain} | Action={plan.Action}"

        _svc.EnsureTask(
            taskName,
            description,
            args,
            trigger,
            runAsHighest:=True
        )
    End Sub

    ' ----------------------------------------------------------
    ' Task manuell starten
    ' ----------------------------------------------------------
    Public Sub RunNow(plan As ERechnungTaskPlanRow)
        If plan Is Nothing Then Return
        If String.IsNullOrWhiteSpace(plan.TaskName) Then
            plan.TaskName = EnsureTaskName(plan)
        End If

        _svc.RunNow(plan.TaskName)
    End Sub

    ' ----------------------------------------------------------
    ' Task stoppen  (STOP ist VB-Schlüsselwort!)
    ' ----------------------------------------------------------
    Public Sub StopTask(plan As ERechnungTaskPlanRow)
        If plan Is Nothing Then Return
        If String.IsNullOrWhiteSpace(plan.TaskName) Then Return

        _svc.StopTask(plan.TaskName)
    End Sub

    ' ----------------------------------------------------------
    ' Task löschen
    ' ----------------------------------------------------------
    Public Sub DeleteTask(plan As ERechnungTaskPlanRow)
        If plan Is Nothing Then Return
        If String.IsNullOrWhiteSpace(plan.TaskName) Then Return

        _svc.DeleteTask(plan.TaskName)
    End Sub

    ' ----------------------------------------------------------
    ' Status: läuft gerade?
    ' ----------------------------------------------------------
    Public Function IsRunning(plan As ERechnungTaskPlanRow) As Boolean
        If plan Is Nothing Then Return False
        If String.IsNullOrWhiteSpace(plan.TaskName) Then Return False

        Return _svc.IsRunning(plan.TaskName)
    End Function

    ' ----------------------------------------------------------
    ' TaskName generieren / bereinigen
    ' ----------------------------------------------------------
    Public Function EnsureTaskName(plan As ERechnungTaskPlanRow) As String
        If plan Is Nothing Then Throw New ArgumentNullException(NameOf(plan))

        If Not String.IsNullOrWhiteSpace(plan.TaskName) Then
            plan.TaskName = SanitizeTaskName(plan.TaskName)
            Return plan.TaskName
        End If

        ' ⚠️ KEIN Trim() auf TaskId (Long/Integer!)
        Dim name As String =
            "ERechnungTask_" &
            plan.TaskId.ToString() & "_" &
            plan.Domain & "_" &
            plan.Action

        plan.TaskName = SanitizeTaskName(name)
        Return plan.TaskName
    End Function

    Private Function SanitizeTaskName(value As String) As String
        Dim s As String = If(value, "").Trim()

        If s.Length = 0 Then Return "ERechnungTask"

        ' Ungültige Zeichen für Task Scheduler ersetzen
        s = s.Replace("\", "_") _
             .Replace("/", "_") _
             .Replace(":", "_") _
             .Replace("*", "_") _
             .Replace("?", "_") _
             .Replace("""", "_") _
             .Replace("<", "_") _
             .Replace(">", "_") _
             .Replace("|", "_")

        ' Länge begrenzen (Task Scheduler mag keine extrem langen Namen)
        If s.Length > 180 Then
            s = s.Substring(0, 180)
        End If

        Return s
    End Function

End Class
