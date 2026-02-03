Imports System.IO
Imports System.Diagnostics
Imports System.Data.OleDb
Imports ehfleet_classlibrary
Imports EHFleetXRechnung.Viewer
Imports log4net
Imports log4net.Config
Imports System.Data

Public Class StartUp

    Private bConfig As Boolean = False
    Private iStartUpCounter As Integer = 14
    Private logger As ILog

    Private Sub ConfigureLogging()
        Dim libraryConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logger.config")
        Dim logConfigFile = New FileInfo(libraryConfigPath)
        logger = LogManager.GetLogger(GetType(StartUp))
        If logConfigFile.Exists Then
            log4net.GlobalContext.Properties("log4net:HostName") = Environment.MachineName
            XmlConfigurator.Configure(logConfigFile)
        End If
    End Sub

    Private Sub butStart_Click(sender As Object, e As EventArgs) Handles butStart.Click

        Dim Form As RechnungsUebersicht
        Dim dbConnection As General.Database = Nothing

        ' Lesen AppDbCnStr aus Registry
        Dim readValue = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\VB and VBA Program Settings\EHFleet Fuhrpark IM System\Allgemein", "workdbcn", Nothing)

        ' Timer stoppen
        tmrStartUp.Enabled = False

        ConfigureLogging()
        If My.Settings.UseRegDbCnStr = True Then
            ' Start Anwendung mit DB-Verbindung aus Registry
            If readValue Is Nothing Then
                MsgBox("Fleet XRechnung App konnte nicht gestartet werden. Es wurde keine Verbindungszeichenfolge (workdbcn) in der Registrierung gefunden. " &
                       "Bitte stellen Sie sicher, dass eine gültige Fleet Client Installation auf diesem System vorhanden ist.", MsgBoxStyle.Critical, "Fleet XRechnung App")
            Else
                dbConnection = New General.Database(readValue.ToString)
            End If
        Else
            ' Start Anwendung mit DB-Verbindung aus App.config
            dbConnection = New General.Database(My.Settings.AppDbCnStr.ToString)
        End If

        If dbConnection IsNot Nothing AndAlso dbConnection.cn.State = Data.ConnectionState.Open Then
            logger.Debug($"Using DB connection string: {dbConnection.cn.ConnectionString}")
            Form = New RechnungsUebersicht(dbConnection)
            Me.Hide()
            Form.ShowDialog()
        Else
            MsgBox("Verbindung zur Datenbank konnte nicht geöffnet werden!" & vbCrLf & dbConnection.cn.ConnectionString, MsgBoxStyle.Critical)
        End If

        Me.Close()

    End Sub

    Private Sub StartUp_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Headless/CLI Mode (Windows Task Scheduler)
        If HandleCliMode() Then
            Return
        End If

        ' Konfiguration aus My.Settings anzeigen
        chkUseRegDbCnStr.Checked = My.Settings.UseRegDbCnStr
        If chkUseRegDbCnStr.Checked = True Then
            txtAppDbCnStr.Text = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\VB and VBA Program Settings\EHFleet Fuhrpark IM System\Allgemein", "workdbcn", Nothing).ToString
        Else
            txtAppDbCnStr.Text = My.Settings.AppDbCnStr
        End If

        ' StartUp Timer starten
        tmrStartUp.Enabled = True

        ' Status
        Dim FilePath As String = Application.StartupPath & "\EHFleetXRechnung.Viewer.dll"
        lblStatus.Text = "Version: App (" & My.Application.Info.Version.ToString & ") - Viewer (" & System.Diagnostics.FileVersionInfo.GetVersionInfo(FilePath).FileVersion & ")"
    End Sub

    Private Sub RadButton1_Click(sender As Object, e As EventArgs) Handles butConfig.Click

        ' Konfigurieren und Abspeichern
        bConfig = Not bConfig
        chkUseRegDbCnStr.Enabled = bConfig
        chkUseRegDbCnStr.Visible = bConfig
        If chkUseRegDbCnStr.Checked = False Then
            txtAppDbCnStr.Enabled = bConfig
            txtAppDbCnStr.Visible = bConfig
            lblAppDbCnStr.Visible = bConfig
        End If
        If bConfig = True Then
            Me.Size = New Size(375, 300)
            tmrStartUp.Enabled = False
            butConfig.Text = "Speichern"
        Else
            My.Settings.UseRegDbCnStr = chkUseRegDbCnStr.Checked
            If chkUseRegDbCnStr.Checked = False Then
                My.Settings.AppDbCnStr = txtAppDbCnStr.Text
            End If
            My.Settings.Save()
            Me.Size = New Size(375, 225)
            butConfig.Text = "Konfigurieren"
        End If

    End Sub

    ' =========================================================================================
    ' CLI / Headless Mode
    '   EHFleetXRechnung.App.exe --run-task <TaskId>
    '
    ' - AppLock (sp_getapplock) verhindert parallele Runs pro TaskId (Server-weit)
    ' - RunId + EndRun persistieren in dbo.ERechnungTaskRun / dbo.ERechnungTaskRunLog
    ' - ExitCode:
    '     0 = OK
    '     2 = parallel lock (already running)
    '     3 = exception / crash
    ' =========================================================================================
    Private Function HandleCliMode() As Boolean
        Dim args = Environment.GetCommandLineArgs()
        Dim idx = Array.FindIndex(args, Function(a) String.Equals(a, "--run-task", StringComparison.OrdinalIgnoreCase))
        If idx < 0 Then Return False
        If idx + 1 >= args.Length Then Return False

        Dim taskId As Integer
        If Not Integer.TryParse(args(idx + 1), taskId) Then Return False

        ' Logging muss in CLI zwingend aktiv sein
        ConfigureLogging()
        logger.Info($"CLI Mode START --run-task {taskId}")
        logger.Info("Args=" & String.Join(" ", args))

        Dim db As General.Database = Nothing
        Dim exitCode As Integer = 0
        Dim ok As Boolean = False
        Dim summary As String = ""
        Dim durationMs As Integer = 0
        Dim runId As Integer = 0

        Dim sw As Stopwatch = Stopwatch.StartNew()

        Try
            db = OpenDatabaseFromSettingsOrRegistry()
            logger.Debug($"Using DB connection string: {db.cn.ConnectionString}")

            ' AppLock (no parallel run per TaskId)
            Dim lockAcquired As Boolean = AcquireAppLock(db, $"EHFleetXRechnung.Task.{taskId}")
            If Not lockAcquired Then
                exitCode = 2
                summary = "Parallel-Guard: Task läuft bereits (sp_getapplock)."
                logger.Warn(summary)
                Return True
            End If

            ' RunId erzeugen
            Dim runRepo As New ERechnungTaskRunRepository(db)
            runId = runRepo.StartRun(taskId, Environment.MachineName, Environment.UserDomainName & "\" & Environment.UserName)
            logger.Info($"Run START RunId={runId} TaskId={taskId}")

            ' Plan-Status in ERechnungTaskPlan spiegeln
            UpdateTaskPlanLastFields(db, taskId, lastRunOk:=Nothing, lastResult:="RUNNING", setLastRunUtc:=True)

            ' Task ausführen (Business)
            Dim runner As New ERechnungTaskRunner(db, logger, runRepo)
            ok = runner.RunTaskById(taskId, runId)

            ' ExitCode 0/1 nach ok
            exitCode = If(ok, 0, 1)
            summary = If(ok, "OK", "FAILED (fachlich): siehe RunLog")

        Catch ex As Exception
            ok = False
            exitCode = 3
            summary = "FAILED: " & ex.Message
            logger.Error(summary, ex)

        Finally
            sw.Stop()
            durationMs = CInt(Math.Min(Integer.MaxValue, sw.ElapsedMilliseconds))

            Try
                If db IsNot Nothing Then
                    ' EndRun persistieren
                    Try
                        Dim runRepo As New ERechnungTaskRunRepository(db)
                        If runId > 0 Then
                            runRepo.EndRun(runId, ok, exitCode, durationMs, summary)
                            logger.Info($"Run END RunId={runId} ok={ok} exit={exitCode} durMs={durationMs}")
                        End If
                    Catch ex2 As Exception
                        logger.Error("EndRun FAILED", ex2)
                    End Try

                    ' Plan-Status final spiegeln
                    UpdateTaskPlanLastFields(db, taskId, ok, $"{summary} | dur={sw.Elapsed.TotalSeconds:0.0}s | exit={exitCode} (0x{exitCode:X})", True)
                End If
            Catch ex3 As Exception
                logger.Error("Finalize CLI state FAILED", ex3)
            End Try

            ' AppLock freigeben
            Try
                If db IsNot Nothing Then
                    ReleaseAppLock(db, $"EHFleetXRechnung.Task.{taskId}")
                End If
            Catch ex4 As Exception
                logger.Debug("ReleaseAppLock failed", ex4)
            End Try
        End Try

        logger.Info($"CLI Mode END --run-task {taskId} ok={ok} exit={exitCode} durMs={durationMs}")
        Environment.ExitCode = exitCode
        Environment.Exit(exitCode)
        Return True
    End Function

    Private Function OpenDatabaseFromSettingsOrRegistry() As General.Database
        Dim readValue = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\VB and VBA Program Settings\EHFleet Fuhrpark IM System\Allgemein", "workdbcn", Nothing)

        If My.Settings.UseRegDbCnStr = True Then
            If readValue Is Nothing Then
                Throw New Exception("Keine Verbindungszeichenfolge (workdbcn) in der Registry gefunden.")
            End If
            Return New General.Database(readValue.ToString())
        End If

        Return New General.Database(My.Settings.AppDbCnStr.ToString())
    End Function

    Private Sub UpdateTaskPlanLastFields(db As General.Database, taskId As Integer, lastRunOk As Boolean?, lastResult As String, setLastRunUtc As Boolean)
        Dim safe As String = If(lastResult, "")
        safe = If(safe.Length > 200, safe.Substring(0, 200), safe)

        Dim sql As String =
            "UPDATE dbo.ERechnungTaskPlan " &
            "SET " &
            If(setLastRunUtc, "LastRunUtc = SYSUTCDATETIME(), ", "") &
            "LastResult = ?, " &
            "LastRunOk = ? " &
            "WHERE TaskId = ?;"

        Using cmd As New OleDb.OleDbCommand(sql, DirectCast(db.cn, OleDb.OleDbConnection))
            cmd.Parameters.AddWithValue("@p1", safe)
            If lastRunOk.HasValue Then
                cmd.Parameters.AddWithValue("@p2", If(lastRunOk.Value, 1, 0))
            Else
                cmd.Parameters.AddWithValue("@p2", DBNull.Value)
            End If
            cmd.Parameters.AddWithValue("@p3", taskId)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Function AcquireAppLock(db As General.Database, resourceName As String) As Boolean
        Dim sql As String = "DECLARE @r int; EXEC @r = sp_getapplock @Resource=?, @LockMode='Exclusive', @LockOwner='Session', @LockTimeout=0; SELECT @r;"
        Using cmd As New OleDb.OleDbCommand(sql, DirectCast(db.cn, OleDb.OleDbConnection))
            cmd.Parameters.AddWithValue("@p1", resourceName)
            Dim r As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            logger.Debug($"sp_getapplock resource={resourceName} result={r}")
            Return (r >= 0)
        End Using
    End Function

    Private Sub ReleaseAppLock(db As General.Database, resourceName As String)
        Dim sql As String = "DECLARE @r int; EXEC @r = sp_releaseapplock @Resource=?, @LockOwner='Session'; SELECT @r;"
        Using cmd As New OleDb.OleDbCommand(sql, DirectCast(db.cn, OleDb.OleDbConnection))
            cmd.Parameters.AddWithValue("@p1", resourceName)
            Dim r As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            logger.Debug($"sp_releaseapplock resource={resourceName} result={r}")
        End Using
    End Sub

End Class
