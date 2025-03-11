Imports System.IO
Imports System.Web.UI.WebControls
Imports ehfleet_classlibrary
Imports EHFleetXRechnung.Viewer
Imports log4net
Imports Microsoft.VisualBasic.ApplicationServices
Imports Telerik.WinControls.Export

Namespace My
    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.

    ' **NEW** ApplyApplicationDefaults: Raised when the application queries default values to be set for the application.

    ' Example:
    ' Private Sub MyApplication_ApplyApplicationDefaults(sender As Object, e As ApplyApplicationDefaultsEventArgs) Handles Me.ApplyApplicationDefaults
    '
    '   ' Setting the application-wide default Font:
    '   e.Font = New Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular)
    '
    '   ' Setting the HighDpiMode for the Application:
    '   e.HighDpiMode = HighDpiMode.PerMonitorV2
    '
    '   ' If a splash dialog is used, this sets the minimum display time:
    '   e.MinimumSplashScreenDisplayTime = 4000
    ' End Sub

    Partial Friend Class MyApplication

        Protected Overrides Function OnStartup(eventArgs As StartupEventArgs) As Boolean

            ' Log4Net Logdatei
            Dim logger As ILog = ConfigureLogging()
            'Dim readValue = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\VB and VBA Program Settings\EHFleet Fuhrpark IM System\Allgemein", "workdbcn", Nothing)
            'If readValue Is Nothing Then
            '    readValue = "Provider=MSOLEDBSQL;Data Source=.\SQLEXPRESS;Initial Catalog=EHFleet;Integrated Security=SSPI;"
            'End If

            'Try
            '    ' Startobjekt auf StartUp-Form setzen
            '    Dim Form = New EHFleetXRechnung.Viewer.RechnungsUebersicht(New General.Database(readValue.ToString))
            '    MainForm = Form
            '    Return MyBase.OnStartup(eventArgs)
            'Catch ex As Exception
            '    MessageBox.Show(ex.Message)
            '    Return False
            'End Try

            ' Direkter Aufruf Viewer-Klasse
            Dim Form As RechnungsUebersicht
            Dim connectionString = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\VB and VBA Program Settings\EHFleet Fuhrpark IM System\Allgemein", "workdbcn", Nothing)?.ToString
            If Not My.Settings.UseRegDbCnStr Then
                connectionString = My.Settings.AppDbCnStr
            End If

            If String.IsNullOrWhiteSpace(connectionString) Then
                MsgBox("Fleet XRechnung App konnte nicht gestartet werden. Es wurde keine Verbindungszeichenfolge (workdbcn) in der Registrierung gefunden. " &
                           "Bitte stellen Sie sicher, dass eine gültige Fleet Client Installation auf diesem System vorhanden ist.", MsgBoxStyle.Critical, "Fleet XRechnung App")
                Return False
            End If

            logger.Debug($"Using the following DB ConnectionString {connectionString}")
            Form = New RechnungsUebersicht(New General.Database(connectionString))
            MainForm = Form
            Return MyBase.OnStartup(eventArgs)
        End Function

        Private Function ConfigureLogging() As ILog

            ' Log4Net Logger konfigurieren
            Dim libraryConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logger.config")
            Dim logConfigFile = New FileInfo(libraryConfigPath)

            If logConfigFile.Exists Then
                log4net.GlobalContext.Properties("log4net:HostName") = Environment.MachineName
                log4net.GlobalContext.Properties("log4net:Login") = "unknown"
                log4net.Config.XmlConfigurator.Configure(logConfigFile)
            End If

            Return LogManager.GetLogger(GetType(Application))
        End Function
    End Class
End Namespace
