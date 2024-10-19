Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports ehfleet_classlibrary
Imports log4net.Config

<Assembly: AssemblyKeyFile("..\\EHFleet.snk")>

<ComVisible(True), Guid("1E240738-5D71-4046-A7A1-3C961E4B2E10"), InterfaceType(ComInterfaceType.InterfaceIsDual)>
Public Interface IHelloWorld
    <DispId(1)> Function SayHello() As String

End Interface

<ComVisible(True), Guid("58C78CCF-40C4-40FF-8577-0B033231B2E6"), ClassInterface(ClassInterfaceType.None)>
Public Class HelloWorld
    Implements IHelloWorld

    Public Function SayHello() As String Implements IHelloWorld.SayHello
        Return "Hello World"
    End Function
End Class

<ComVisible(True), Guid("3D853E7B-01DA-4944-8E65-5E36B501E889"), InterfaceType(ComInterfaceType.InterfaceIsDual)>
Public Interface IWindowWrapper
    <DispId(1)> Sub ShowWindow(connectionString As String, login As String)
End Interface

<ComVisible(True), Guid("5C2AD12F-2282-4CFF-AC95-6FFC57F074D1"), ClassInterface(ClassInterfaceType.None)>
Public Class WindowWrapper
    'Public Const ClassId As String = "3D853E7B-01DA-4944-8E65-5E36B501E889"
    Implements IWindowWrapper

    Private Sub ConfigureLogging(login As String)
        Dim libraryConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EHFleetXRechnung.Viewer.dll.config")
        'MsgBox(libraryConfigPath)
        ' ToDo: Pfad setzen auf Installationspfad der Anwendung
        Dim logConfigFile = New FileInfo(libraryConfigPath)
        'MsgBox(logConfigFile.ToString)

        If logConfigFile.Exists Then
            log4net.GlobalContext.Properties("log4net:HostName") = Environment.MachineName
            log4net.GlobalContext.Properties("log4net:Login") = login
            XmlConfigurator.Configure(logConfigFile)
        End If
    End Sub

    Private Sub IWindowWrapper_ShowWindow(connectionString As String, login As String) Implements IWindowWrapper.ShowWindow
        ConfigureLogging(login)
        Dim form = New RechnungsUebersicht(New General.Database(connectionString))
        form.ShowDialog()
    End Sub

End Class
