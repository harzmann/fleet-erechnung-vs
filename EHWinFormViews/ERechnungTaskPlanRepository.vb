Option Explicit On
Option Infer On

Imports System.Data
Imports System.Data.OleDb
Imports ehfleet_classlibrary

' ==========================================================
' Datenmodell
' ==========================================================
Public Class ERechnungTaskPlanRow
    Public Property TaskId As Integer
    Public Property Domain As String
    Public Property Action As String
    Public Property Enabled As Boolean
    Public Property ScheduleType As String
    Public Property EveryN As Integer
    Public Property StartTime As TimeSpan?
    Public Property WeekdaysMask As Integer
    Public Property LastRunUtc As DateTime?
    Public Property LastResult As String
    Public Property LastRunOk As Boolean?
    Public Property TaskName As String
    Public Property IncludeDuplicates As Boolean
    Public Property OnlyNewSinceLastRun As Boolean
End Class

' ==========================================================
' Repository für dbo.ERechnungTaskPlan (OLE DB)
'  - nutzt _db.cn als OleDbConnection
'  - OleDb verwendet Parameter positionsbasiert (?)
' ==========================================================
Public Class ERechnungTaskPlanRepository

    Private ReadOnly _db As General.Database

    Public Sub New(db As General.Database)
        _db = db
    End Sub

    Private Function Conn() As OleDbConnection
        Return DirectCast(_db.cn, OleDbConnection)
    End Function

    ' ----------------------------------------------------------
    ' Alle Tasks laden
    ' ----------------------------------------------------------
    Public Function GetAll() As DataTable
        Dim sql As String =
"SELECT
    TaskId,
    TaskName,
    Domain,
    Action,
    Enabled,
    ScheduleType,
    EveryN,
    StartTime,
    WeekdaysMask,
    IncludeDuplicates,
    OnlyNewSinceLastRun,
    LastRunUtc,
    LastResult,
    LastRunOk
FROM dbo.ERechnungTaskPlan
ORDER BY TaskId DESC;"

        Using cmd As New OleDbCommand(sql, Conn())
            Using da As New OleDbDataAdapter(cmd)
                Dim dt As New DataTable()
                da.Fill(dt)
                Return dt
            End Using
        End Using
    End Function

    ' ----------------------------------------------------------
    ' Einzelnen Task laden
    ' ----------------------------------------------------------
    Public Function GetById(taskId As Integer) As ERechnungTaskPlanRow
        Dim sql As String =
"SELECT TOP 1
    TaskId,
    Domain,
    Action,
    Enabled,
    ScheduleType,
    EveryN,
    StartTime,
    WeekdaysMask,
    IncludeDuplicates,
    OnlyNewSinceLastRun,
    LastRunUtc,
    LastResult,
    LastRunOk,
    TaskName
FROM dbo.ERechnungTaskPlan
WHERE TaskId = ?;"

        Using cmd As New OleDbCommand(sql, Conn())
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = taskId})
            Using da As New OleDbDataAdapter(cmd)
                Dim dt As New DataTable()
                da.Fill(dt)
                If dt.Rows.Count = 0 Then Return Nothing
                Return MapRow(dt.Rows(0))
            End Using
        End Using
    End Function

    ' ----------------------------------------------------------
    ' Insert
    ' ----------------------------------------------------------
    Public Function Insert(row As ERechnungTaskPlanRow) As Integer
        Dim sql As String =
"INSERT INTO dbo.ERechnungTaskPlan
(
    Domain,
    Action,
    Enabled,
    ScheduleType,
    EveryN,
    StartTime,
    WeekdaysMask,
    TaskName,
    IncludeDuplicates,
    OnlyNewSinceLastRun
)
VALUES
(
    ?, ?, ?, ?, ?, ?, ?, ?, ?, ?
);
SELECT CAST(SCOPE_IDENTITY() AS int);"

        Using cmd As New OleDbCommand(sql, Conn())
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = row.Domain})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = row.Action})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Boolean, .Value = row.Enabled})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = row.ScheduleType})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = row.EveryN})

            If row.StartTime.HasValue Then
                cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.DBTime, .Value = row.StartTime.Value})
            Else
                cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.DBTime, .Value = DBNull.Value})
            End If

            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = row.WeekdaysMask})

            If String.IsNullOrWhiteSpace(row.TaskName) Then
                cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = DBNull.Value})
            Else
                cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = row.TaskName})
            End If

            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Boolean, .Value = row.IncludeDuplicates})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Boolean, .Value = row.OnlyNewSinceLastRun})

            Return Convert.ToInt32(cmd.ExecuteScalar())
        End Using
    End Function

    ' ----------------------------------------------------------
    ' Update
    ' ----------------------------------------------------------
    Public Sub Update(row As ERechnungTaskPlanRow)
        Dim sql As String =
"UPDATE dbo.ERechnungTaskPlan
SET
    Domain        = ?,
    Action        = ?,
    Enabled       = ?,
    ScheduleType  = ?,
    EveryN        = ?,
    StartTime     = ?,
    WeekdaysMask  = ?,
    TaskName      = ?,
    IncludeDuplicates = ?,
    OnlyNewSinceLastRun = ?
WHERE TaskId     = ?;"

        Using cmd As New OleDbCommand(sql, Conn())
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = row.Domain})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = row.Action})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Boolean, .Value = row.Enabled})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = row.ScheduleType})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = row.EveryN})

            If row.StartTime.HasValue Then
                cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.DBTime, .Value = row.StartTime.Value})
            Else
                cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.DBTime, .Value = DBNull.Value})
            End If

            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = row.WeekdaysMask})

            If String.IsNullOrWhiteSpace(row.TaskName) Then
                cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = DBNull.Value})
            Else
                cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = row.TaskName})
            End If

            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Boolean, .Value = row.IncludeDuplicates})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Boolean, .Value = row.OnlyNewSinceLastRun})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = row.TaskId})

            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' ----------------------------------------------------------
    ' Delete
    ' ----------------------------------------------------------
    Public Sub Delete(taskId As Integer)
        Dim sql As String = "DELETE FROM dbo.ERechnungTaskPlan WHERE TaskId = ?;"

        Using cmd As New OleDbCommand(sql, Conn())
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = taskId})
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' ----------------------------------------------------------
    ' Mapping DataRow → Objekt (robust für OleDb)
    ' ----------------------------------------------------------
    Private Function MapRow(r As DataRow) As ERechnungTaskPlanRow
        Dim x As New ERechnungTaskPlanRow()

        x.TaskId = Convert.ToInt32(r("TaskId"))
        x.Domain = Convert.ToString(r("Domain"))
        x.Action = Convert.ToString(r("Action"))
        x.Enabled = Convert.ToBoolean(r("Enabled"))
        x.ScheduleType = Convert.ToString(r("ScheduleType"))
        x.EveryN = Convert.ToInt32(r("EveryN"))
        x.WeekdaysMask = Convert.ToInt32(r("WeekdaysMask"))

        If r.IsNull("StartTime") Then
            x.StartTime = Nothing
        Else
            Try
                x.StartTime = TimeSpan.Parse(Convert.ToString(r("StartTime")))
            Catch
                x.StartTime = Nothing
            End Try
        End If

        If r.IsNull("LastRunUtc") Then
            x.LastRunUtc = Nothing
        Else
            Dim v As Object = r("LastRunUtc")
            If TypeOf v Is DateTime Then
                x.LastRunUtc = DirectCast(v, DateTime)
            Else
                Dim s As String = Convert.ToString(v).Trim()
                Dim dt As DateTime
                If DateTime.TryParse(s, dt) Then
                    x.LastRunUtc = dt
                Else
                    x.LastRunUtc = Nothing
                End If
            End If
        End If

        x.LastResult = If(r.IsNull("LastResult"), Nothing, Convert.ToString(r("LastResult")))

        If r.IsNull("LastRunOk") Then
            x.LastRunOk = Nothing
        Else
            Try
                x.LastRunOk = Convert.ToBoolean(r("LastRunOk"))
            Catch
                x.LastRunOk = Nothing
            End Try
        End If

        x.TaskName = If(r.IsNull("TaskName"), Nothing, Convert.ToString(r("TaskName")))

        If r.Table.Columns.Contains("IncludeDuplicates") AndAlso Not r.IsNull("IncludeDuplicates") Then
            x.IncludeDuplicates = Convert.ToBoolean(r("IncludeDuplicates"))
        Else
            x.IncludeDuplicates = False
        End If

        If r.Table.Columns.Contains("OnlyNewSinceLastRun") AndAlso Not r.IsNull("OnlyNewSinceLastRun") Then
            x.OnlyNewSinceLastRun = Convert.ToBoolean(r("OnlyNewSinceLastRun"))
        Else
            x.OnlyNewSinceLastRun = False
        End If

        Return x
    End Function

End Class
