Imports System.IO
Imports ehfleet_classlibrary
Imports EHWinFormViews
Imports log4net.Config
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
            Dim Form = New RechnungsUebersicht(New General.Database("Provider=MSOLEDBSQL;Data Source=.\SQLEXPRESS;Initial Catalog=EHFleet;Integrated Security=SSPI;"))
            MainForm = Form
            Return MyBase.OnStartup(eventArgs)
        End Function

        Private Sub ConfigureLogging()
            Dim libraryConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EHFleetXRechnung.Viewer.dll.config")
            Dim logConfigFile = New FileInfo(libraryConfigPath)

            If logConfigFile.Exists Then
                log4net.GlobalContext.Properties("log4net:HostName") = Environment.MachineName
                XmlConfigurator.Configure(logConfigFile)
            End If
        End Sub
    End Class
End Namespace
