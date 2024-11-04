Imports System.IO
Imports ehfleet_classlibrary
Imports EHFleetXRechnung.Viewer
Imports Microsoft.VisualBasic.ApplicationServices

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
            ConfigureLogging()
            Dim readValue = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\VB and VBA Program Settings\EHFleet Fuhrpark IM System\Allgemein", "workdbcn", Nothing)
            If readValue Is Nothing Then
                readValue = "Provider=MSOLEDBSQL;Data Source=.\SQLEXPRESS;Initial Catalog=EHFleet;Integrated Security=SSPI;"
            End If

            Try
                Dim Form = New EHFleetXRechnung.Viewer.RechnungsUebersicht(New General.Database(readValue.ToString))
                MainForm = Form
                Return MyBase.OnStartup(eventArgs)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                Return False
            End Try
        End Function

        Private Sub ConfigureLogging()
            Dim libraryConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logger.config")
            Dim logConfigFile = New FileInfo(libraryConfigPath)

            If logConfigFile.Exists Then
                log4net.GlobalContext.Properties("log4net:HostName") = Environment.MachineName
                log4net.Config.XmlConfigurator.Configure(logConfigFile)
            End If
        End Sub
    End Class
End Namespace
