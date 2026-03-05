Imports log4net

Public Module CliLogger
    Private ReadOnly _logger As ILog = LogManager.GetLogger("CliFileRequest")

    Public Sub LogInfo(msg As String)
        _logger.Info(msg)
    End Sub

    Public Sub LogException(ex As Exception)
        _logger.Error("CLI Exception", ex)
    End Sub
End Module
