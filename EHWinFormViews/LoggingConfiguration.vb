Imports System.IO
Imports System.Security
Imports log4net
Imports log4net.Appender
Imports log4net.Config
Imports log4net.Layout

''' <summary>Gemeinsamer, beschreibbarer Ablageort für Logs und Validator-Berichte.</summary>
Public NotInheritable Class LoggingConfiguration
    Private Shared ReadOnly _directory As New Lazy(Of String)(Function() SelectWritableDirectory(
        Path.Combine(Path.GetDirectoryName(GetType(LoggingConfiguration).Assembly.Location), "logs"),
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "eh-systemhaus", "EHFleetXRechnung", "logs"),
        Path.Combine(Path.GetTempPath(), "eh-systemhaus", "EHFleetXRechnung", "logs")))
    Private Shared ReadOnly _sync As New Object()

    Private Sub New()
    End Sub

    Public Shared ReadOnly Property LogDirectory As String
        Get
            Return _directory.Value
        End Get
    End Property

    ' Tatsächlich schreiben statt ACLs auszuwerten: funktioniert auch auf Netzlaufwerken.
    Friend Shared Function SelectWritableDirectory(ParamArray candidates As String()) As String
        Dim lastError As Exception = Nothing
        For Each candidate In candidates
            Try
                Dim directory = Path.GetFullPath(candidate)
                System.IO.Directory.CreateDirectory(directory)
                Using probe As New FileStream(Path.Combine(directory, ".write-test-" & Guid.NewGuid().ToString("N")),
                                              FileMode.CreateNew, FileAccess.Write, FileShare.None, 1, FileOptions.DeleteOnClose)
                    probe.WriteByte(0)
                    probe.Flush()
                End Using
                Return directory
            Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException OrElse TypeOf ex Is SecurityException
                lastError = ex
            End Try
        Next
        Throw New IOException("Kein beschreibbares Logverzeichnis verfügbar (Programmverzeichnis, LocalAppData und Temp).", lastError)
    End Function

    Public Shared Sub Configure(Optional login As String = Nothing)
        SyncLock _sync
            GlobalContext.Properties("EHFleetLogDirectory") = LogDirectory
            GlobalContext.Properties("log4net:HostName") = Environment.MachineName
            If login IsNot Nothing Then GlobalContext.Properties("log4net:Login") = login
            Dim repository = LogManager.GetRepository()
            If Not repository.Configured Then
                Dim config = New FileInfo(Path.Combine(Path.GetDirectoryName(GetType(LoggingConfiguration).Assembly.Location), "logger.config"))
                If config.Exists Then
                    XmlConfigurator.Configure(repository, config)
                Else
                    ' Auch ohne Konfigurationsdatei ein dauerhaftes Fehlerprotokoll bereitstellen.
                    Dim appender As New RollingFileAppender With {
                        .Name = "RollingFileAppender", .File = Path.Combine(LogDirectory, "EHFleetXRechnungViewer"),
                        .AppendToFile = True, .RollingStyle = RollingFileAppender.RollingMode.Date,
                        .StaticLogFileName = False, .DatePattern = "'.'yyyy-MM-dd'.log'",
                        .LockingModel = New FileAppender.MinimalLock(),
                        .Layout = New PatternLayout("%date %-5level %logger - %message%newline")}
                    appender.ActivateOptions()
                    BasicConfigurator.Configure(repository, appender)
                End If
                LogManager.GetLogger(GetType(LoggingConfiguration)).Info("Logverzeichnis: " & LogDirectory)
            End If
        End SyncLock
    End Sub
End Class
