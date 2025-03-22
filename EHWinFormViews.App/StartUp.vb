Imports System.IO
Imports ehfleet_classlibrary
Imports EHFleetXRechnung.Viewer
Imports log4net
Imports log4net.Config

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
        If chkUseRegDbCnStr.Checked = False Then
            txtAppDbCnStr.Enabled = bConfig
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

    Private Sub chkUseRegDbCnStr_ToggleStateChanged(sender As Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles chkUseRegDbCnStr.ToggleStateChanged

        ' Eingabe Verbindungszeichenfolge aktivieren/deaktivieren
        If args.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On Then
            txtAppDbCnStr.Enabled = False
            txtAppDbCnStr.Text = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\VB and VBA Program Settings\EHFleet Fuhrpark IM System\Allgemein", "workdbcn", Nothing).ToString
        Else
            txtAppDbCnStr.Enabled = True
            txtAppDbCnStr.Text = My.Settings.AppDbCnStr
        End If

    End Sub

    Private Sub tmrStartUp_Tick(sender As Object, e As EventArgs) Handles tmrStartUp.Tick

        ' Anwendung nach 5s starten
        butStart.Text = "Start (" + iStartUpCounter.ToString + ")"
        iStartUpCounter -= 1
        If iStartUpCounter = 0 Then
            tmrStartUp.Enabled = False
            butStart_Click(sender, e)
        End If

    End Sub

    Private Sub StartUp_Activated(sender As Object, e As EventArgs) Handles Me.Activated

        Me.Size = New Size(375, 225)

    End Sub
End Class
