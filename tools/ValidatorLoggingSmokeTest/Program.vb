Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Security.AccessControl
Imports System.Security.Principal
Imports EHFleetXRechnung.Viewer

Module Program
    Sub Main()
        Dim programLogs = New DirectoryInfo(Path.Combine(Path.GetDirectoryName(GetType(LoggingConfiguration).Assembly.Location), "logs"))
        programLogs.Create()
        Dim deny = New FileSystemAccessRule(WindowsIdentity.GetCurrent().User, FileSystemRights.Write, InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Deny)
        Dim acl = programLogs.GetAccessControl()
        acl.AddAccessRule(deny)
        programLogs.SetAccessControl(acl)
        Dim fixture = Path.Combine(Path.GetTempPath(), "EHFleetValidatorFixture-" & Guid.NewGuid().ToString("N") & ".xml")
        Try
            LoggingConfiguration.Configure()
            If LoggingConfiguration.LogDirectory = programLogs.FullName Then Throw New Exception("Fallback not used")
            Dim exporter = DirectCast(RuntimeHelpers.GetUninitializedObject(GetType(XRechnungExporter)), XRechnungExporter)
            GetType(XRechnungExporter).GetField("_reportsFolderCleaned", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic).SetValue(exporter, True)
            File.WriteAllText(fixture, "<Invoice xmlns=""urn:oasis:names:specification:ubl:schema:xsd:Invoice-2""/>")
            Dim entry As New ExportLogEntry()
            Dim result = exporter.Validate(fixture, silent:=True, logEntry:=entry)
            If result Then Throw New Exception("Invalid fixture unexpectedly accepted")
            If Not File.Exists(entry.HtmlValidatorPath) Then Throw New Exception("Validator did not produce a report: " & entry.FehlerInfo)
            If Path.GetDirectoryName(entry.HtmlValidatorPath) <> Path.Combine(LoggingConfiguration.LogDirectory, "html") Then Throw New Exception("Wrong report folder")
            Console.WriteLine("PASS shipped viewer and Java validator wrote HTML report despite denied program logs: " & entry.HtmlValidatorPath)
        Finally
            acl.RemoveAccessRuleSpecific(deny)
            programLogs.SetAccessControl(acl)
            File.Delete(fixture)
            Dim html = Path.Combine(LoggingConfiguration.LogDirectory, "html")
            If Directory.Exists(html) Then
                For Each report In Directory.GetFiles(html, Path.GetFileNameWithoutExtension(fixture) & "*")
                    File.Delete(report)
                Next
            End If
        End Try
    End Sub
End Module
