Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports ehfleet_classlibrary
Imports log4net.Config

<Assembly: AssemblyKeyFile("..\\EHFleet.snk")>
<ComVisible(True), Guid("3D853E7B-01DA-4944-8E65-5E36B501E889"), InterfaceType(ComInterfaceType.InterfaceIsDual)>
Public Interface IShowWindow
    <DispId(1)> Sub ShowWindow(connectionString As String, login As String)
End Interface

<ComVisible(True), Guid("19E46122-C584-4DED-A53E-B23A2F0B3AA0"), ClassInterface(ClassInterfaceType.None)>
Public Class WindowWrapper
    'Public Const ClassId As String = "3D853E7B-01DA-4944-8E65-5E36B501E889"

    Implements IShowWindow

    Public Sub ShowWindow(connectionString As String, login As String) Implements IShowWindow.ShowWindow
        ConfigureLogging(login)
        Dim form = New RechnungsUebersicht(New General.Database(connectionString))
        form.ShowDialog()
    End Sub

    Private Sub ConfigureLogging(login As String)
        Dim libraryConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logger.config")
        Dim logConfigFile = New FileInfo(libraryConfigPath)

        If logConfigFile.Exists Then
            log4net.GlobalContext.Properties("log4net:HostName") = Environment.MachineName
            log4net.GlobalContext.Properties("log4net:Login") = login
            XmlConfigurator.Configure(logConfigFile)
        End If
    End Sub

End Class
