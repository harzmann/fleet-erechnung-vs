Imports System.Reflection
Imports System.Runtime.InteropServices
Imports ehfleet_classlibrary

<Assembly: AssemblyKeyFile("..\\EHFleet.snk")>

<ComVisible(True), Guid(WindowWrapper.ClassId)>
Public Class WindowWrapper
    Public Const ClassId As String = "3D853E7B-01DA-4944-8E65-5E36B501E889"

    Sub ShowWindow(connectionString As String)
        Dim form = New RechnungsUebersicht(New General.Database(connectionString))
        form.ShowDialog()
    End Sub
End Class
