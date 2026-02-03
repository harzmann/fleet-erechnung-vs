Option Strict On
Option Infer On

Imports System
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports ehfleet_classlibrary

Public Class ERechnungTaskRunRepository

    Private ReadOnly _db As General.Database

    Public Sub New(db As General.Database)
        _db = db
    End Sub

    Private Function Conn() As OleDbConnection
        Return DirectCast(_db.cn, OleDbConnection)
    End Function

    Public Function StartRun(taskId As Integer, hostName As String, windowsUser As String) As Integer
        Dim sql As String =
"INSERT INTO dbo.ERechnungTaskRun(TaskId, HostName, WindowsUser)
VALUES(?, ?, ?);
SELECT CAST(SCOPE_IDENTITY() AS int);"

        Using cmd As New OleDbCommand(sql, Conn())
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = taskId})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = If(hostName, "")})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = If(windowsUser, "")})
            Return Convert.ToInt32(cmd.ExecuteScalar())
        End Using
    End Function

    Public Sub EndRun(runId As Integer, ok As Boolean, exitCode As Integer, durationMs As Integer, summary As String, Optional schedulerResult As Integer? = Nothing)
        Dim sql As String =
"UPDATE dbo.ERechnungTaskRun
SET EndedUtc = SYSUTCDATETIME(),
    DurationMs = ?,
    ExitCode = ?,
    RunOk = ?,
    Summary = ?,
    SchedulerResult = ?
WHERE RunId = ?;"

        Using cmd As New OleDbCommand(sql, Conn())
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = durationMs})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = exitCode})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Boolean, .Value = ok})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = If(summary, "")})

            If schedulerResult.HasValue Then
                cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = schedulerResult.Value})
            Else
                cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = DBNull.Value})
            End If

            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = runId})
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Sub InsertLog(runId As Integer,
                         e As ExportLogEntry,
                         Optional validatorText As String = Nothing,
                         Optional validatorHtml As String = Nothing)

        Dim sql As String =
"INSERT INTO dbo.ERechnungTaskRunLog
(RunId, RechnungsNummer, Status, FehlerInfo, ExportFilePath, HtmlValidatorPath,
 EmailEmpfaenger, EmailStatus, EmailFehlerInfo,
 ValidatorText, ValidatorHtml)
VALUES
(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);"

        Using cmd As New OleDbCommand(sql, Conn())
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = runId})

            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = If(e Is Nothing, CType(DBNull.Value, Object), e.RechnungsNummer)})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = If(e?.Status, "")})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.LongVarWChar, .Value = If(e?.FehlerInfo, "")})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = If(e?.ExportFilePath, "")})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = If(e?.HtmlValidatorPath, "")})

            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = If(e?.EmailEmpfaenger, "")})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.VarWChar, .Value = If(e?.EmailStatus, "")})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.LongVarWChar, .Value = If(e?.EmailFehlerInfo, "")})

            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.LongVarWChar, .Value = If(validatorText, "")})
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.LongVarWChar, .Value = If(validatorHtml, "")})

            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Function GetRuns(taskId As Integer) As DataTable
        Dim sql As String =
"SELECT TOP 200
    RunId,
    StartedUtc,
    EndedUtc,
    DurationMs,
    RunOk,
    ExitCode,
    Summary
FROM dbo.ERechnungTaskRun
WHERE TaskId = ?
ORDER BY StartedUtc DESC;"

        Using cmd As New OleDbCommand(sql, Conn())
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = taskId})
            Using da As New OleDbDataAdapter(cmd)
                Dim dt As New DataTable()
                da.Fill(dt)
                Return dt
            End Using
        End Using
    End Function

    Public Function GetLogsAsEntries(runId As Integer, Optional materializeValidatorHtmlToTemp As Boolean = True) As List(Of ExportLogEntry)
        Dim sql As String =
"SELECT
    RechnungsNummer, Status, FehlerInfo, ExportFilePath, HtmlValidatorPath,
    EmailEmpfaenger, EmailStatus, EmailFehlerInfo,
    ValidatorText, ValidatorHtml
FROM dbo.ERechnungTaskRunLog
WHERE RunId = ?
ORDER BY CreatedUtc ASC;"

        Dim list As New List(Of ExportLogEntry)()

        Using cmd As New OleDbCommand(sql, Conn())
            cmd.Parameters.Add(New OleDbParameter With {.OleDbType = OleDbType.Integer, .Value = runId})
            Using da As New OleDbDataAdapter(cmd)
                Dim dt As New DataTable()
                da.Fill(dt)

                Dim tempRoot As String = Path.Combine(Path.GetTempPath(), "EHFleetXRechnung", "Run_" & runId.ToString())
                If materializeValidatorHtmlToTemp AndAlso Not Directory.Exists(tempRoot) Then
                    Directory.CreateDirectory(tempRoot)
                End If

                For Each r As DataRow In dt.Rows
                    Dim e As New ExportLogEntry()

                    If Not IsDBNull(r("RechnungsNummer")) Then e.RechnungsNummer = Convert.ToInt32(r("RechnungsNummer"))
                    e.Status = If(IsDBNull(r("Status")), "", Convert.ToString(r("Status")))
                    e.FehlerInfo = If(IsDBNull(r("FehlerInfo")), "", Convert.ToString(r("FehlerInfo")))
                    e.ExportFilePath = If(IsDBNull(r("ExportFilePath")), "", Convert.ToString(r("ExportFilePath")))
                    e.HtmlValidatorPath = If(IsDBNull(r("HtmlValidatorPath")), "", Convert.ToString(r("HtmlValidatorPath")))
                    e.EmailEmpfaenger = If(IsDBNull(r("EmailEmpfaenger")), "", Convert.ToString(r("EmailEmpfaenger")))
                    e.EmailStatus = If(IsDBNull(r("EmailStatus")), "", Convert.ToString(r("EmailStatus")))
                    e.EmailFehlerInfo = If(IsDBNull(r("EmailFehlerInfo")), "", Convert.ToString(r("EmailFehlerInfo")))

                    Dim vText As String = If(IsDBNull(r("ValidatorText")), "", Convert.ToString(r("ValidatorText")))
                    If vText.Length > 0 AndAlso e.FehlerInfo.Length = 0 Then
                        e.FehlerInfo = vText
                    End If

                    Dim vHtml As String = If(IsDBNull(r("ValidatorHtml")), "", Convert.ToString(r("ValidatorHtml")))
                    If materializeValidatorHtmlToTemp AndAlso vHtml.Length > 0 Then
                        Dim bill As String = If(e.RechnungsNummer > 0, e.RechnungsNummer.ToString(), Guid.NewGuid().ToString("N"))
                        Dim htmlPath = Path.Combine(tempRoot, $"validator_{bill}.html")
                        File.WriteAllText(htmlPath, vHtml)
                        e.HtmlValidatorPath = htmlPath
                    End If

                    list.Add(e)
                Next
            End Using
        End Using

        Return list
    End Function

End Class
