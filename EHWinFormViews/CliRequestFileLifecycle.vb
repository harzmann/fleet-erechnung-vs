Imports System.IO
Imports System.Text.Json

Public Module CliRequestFileLifecycle
    Public Sub MarkProcessing(requestPath As String)
        Try
            Dim processingPath = Path.Combine(Path.GetDirectoryName(requestPath), Path.GetFileNameWithoutExtension(requestPath) & ".processing")
            File.Move(requestPath, processingPath)
        Catch ex As Exception
            CliLogger.LogException(ex)
        End Try
    End Sub

    Public Sub DeleteRequestFile(requestPath As String)
        Try
            If File.Exists(requestPath) Then File.Delete(requestPath)
        Catch ex As Exception
            CliLogger.LogException(ex)
        End Try
    End Sub

    Public Sub WriteErrorFile(requestPath As String, errMsg As String)
        Try
            Dim errPath = Path.Combine(Path.GetDirectoryName(requestPath), Path.GetFileNameWithoutExtension(requestPath) & ".err.json")
            Dim errorObj = New With {.error = errMsg}
            File.WriteAllText(errPath, JsonSerializer.Serialize(errorObj))
        Catch ex As Exception
            CliLogger.LogException(ex)
        End Try
    End Sub
End Module
