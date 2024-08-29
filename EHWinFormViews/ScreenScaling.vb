Imports System.Runtime.InteropServices

Public Module ScreenScaling

    <DllImport("shcore.dll")>
    Public Function SetProcessDpiAwareness(ByVal value As _Process_DPI_Awareness) As Integer
    End Function

    Public Enum _Process_DPI_Awareness
        Process_DPI_Unaware = 0
        Process_System_DPI_Aware = 1
        Process_Per_Monitor_DPI_Aware = 2
    End Enum

End Module
