Imports System.IO
Imports System.Security.AccessControl
Imports System.Security.Principal
Imports log4net

Module Program
    Private Sub Check(condition As Boolean, description As String)
        If Not condition Then Throw New Exception(description)
        Console.WriteLine("PASS " & description)
    End Sub

    Sub Main()
        Dim root = Path.Combine(Path.GetTempPath(), "EHFleetLoggingTest-" & Guid.NewGuid().ToString("N"))
        Dim candidates = {Path.Combine(root, "program"), Path.Combine(root, "local"), Path.Combine(root, "temp")}
        For Each folder In candidates
            Directory.CreateDirectory(folder)
        Next
        Dim deny = New FileSystemAccessRule(WindowsIdentity.GetCurrent().User, FileSystemRights.Write,
                                           InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit,
                                           PropagationFlags.None, AccessControlType.Deny)
        Try
            Check(LoggingConfiguration.SelectWritableDirectory(candidates) = candidates(0), "writable program directory")
            For i = 0 To 2
                Dim directoryInfo As New DirectoryInfo(candidates(i))
                Dim acl = directoryInfo.GetAccessControl()
                acl.AddAccessRule(deny)
                directoryInfo.SetAccessControl(acl)
                If i < 2 Then
                    Check(LoggingConfiguration.SelectWritableDirectory(candidates) = candidates(i + 1), "ACL denied: fallback " & (i + 1))
                Else
                    Dim failed As Boolean = False
                    Try
                        LoggingConfiguration.SelectWritableDirectory(candidates)
                    Catch ex As IOException
                        failed = True
                    End Try
                    Check(failed, "all paths denied: explicit error")
                End If
            Next
        Finally
            For Each folder In candidates
                Dim directoryInfo As New DirectoryInfo(folder)
                Dim acl = directoryInfo.GetAccessControl()
                acl.RemoveAccessRuleSpecific(deny)
                directoryInfo.SetAccessControl(acl)
            Next
        End Try

        ' Exercise the actual XML and selected path, including SMTP and HTML reports.
        Dim programLogs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs")
        Dim programDirectory As New DirectoryInfo(programLogs)
        programDirectory.Create()
        Dim programAcl = programDirectory.GetAccessControl()
        programAcl.AddAccessRule(deny)
        programDirectory.SetAccessControl(programAcl)
        Try
            LoggingConfiguration.Configure("logging-test")
            Check(LoggingConfiguration.LogDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                  "eh-systemhaus", "EHFleetXRechnung", "logs"), "production LocalAppData fallback")
            Dim repository = LogManager.GetRepository()
            Check(repository.Configured, "logger.config loaded")
            Dim marker = "fallback-test-" & Guid.NewGuid().ToString("N")
            LogManager.GetLogger("ReleaseSmoke").Info(marker)
            LogManager.GetLogger("EHFleetXRechnungSmtpAppender").Info(marker & "-SMTP")
            Dim appenders = repository.GetAppenders().OfType(Of log4net.Appender.FileAppender)().ToArray()
            For Each fileAppender In appenders
                Check(Path.GetDirectoryName(fileAppender.File) = LoggingConfiguration.LogDirectory, "appender path " & fileAppender.Name)
                Check(File.ReadAllText(fileAppender.File, fileAppender.Encoding).Contains(marker), "log written " & fileAppender.Name)
            Next
            Check(appenders.Length = 2, "both appenders verified")
            Dim report = Path.Combine(LoggingConfiguration.LogDirectory, "html", marker & ".html")
            Directory.CreateDirectory(Path.GetDirectoryName(report))
            File.WriteAllText(report, "<html>test</html>")
            Check(File.Exists(report), "validator report directory writable")
            File.Delete(report)
            Check(Not Directory.EnumerateFiles(LoggingConfiguration.LogDirectory, ".write-test-*").Any(), "write probes removed")
            LogManager.Shutdown()
        Finally
            programAcl.RemoveAccessRuleSpecific(deny)
            programDirectory.SetAccessControl(programAcl)
        End Try
    End Sub
End Module
