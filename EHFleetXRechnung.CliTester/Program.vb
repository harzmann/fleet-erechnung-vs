Option Strict On
Option Explicit On

Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Text.Json
Imports System.Collections.Generic

Namespace EHFleetXRechnung.CliTester

    Module Program

        Sub Main(args As String())

            Console.WriteLine("---------------------------")
            Console.WriteLine("EHFleetXRechnung CLI Tester")
            Console.WriteLine("---------------------------")

            If args.Length < 4 Then
                ShowHelp()
                Return
            End If

            Dim exePath As String = args(0)
            Dim cmd As String = args(1)
            Dim invoiceType As String = args(2)
            Dim invoiceNo As Integer = Integer.Parse(args(3))

            Dim toAddress As String = Nothing

            If args.Length >= 5 Then
                toAddress = args(4)
            End If

            If Not File.Exists(exePath) Then
                Console.WriteLine("ERROR: Application not found:")
                Console.WriteLine(exePath)
                Return
            End If

            Dim requestFile = CreateRequestFile(cmd, invoiceType, invoiceNo, toAddress)

            Console.WriteLine("Request file created:")
            Console.WriteLine(requestFile)
            Console.WriteLine()

            Dim psi As New ProcessStartInfo()
            psi.FileName = exePath
            psi.Arguments = $"--request ""{requestFile}"""
            psi.UseShellExecute = False

            Console.WriteLine("Starting application...")
            Console.WriteLine()

            Dim proc = Process.Start(psi)
            proc.WaitForExit()

            Console.WriteLine()
            Console.WriteLine("Application finished")
            Console.WriteLine("ExitCode: " & proc.ExitCode)

        End Sub


        Private Function CreateRequestFile(cmd As String,
                                           invoiceType As String,
                                           invoiceNo As Integer,
                                           toAddress As String) As String

            Dim requestId As String = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")

            Dim payload As New Dictionary(Of String, Object)
            payload("invoiceType") = invoiceType
            payload("invoiceNo") = invoiceNo

            If Not String.IsNullOrWhiteSpace(toAddress) Then
                payload("to") = toAddress
            End If

            Dim request As New Dictionary(Of String, Object)
            request("version") = 1
            request("requestId") = requestId
            request("createdUtc") = DateTime.UtcNow
            request("user") = Environment.UserName
            request("cmd") = cmd
            request("payload") = payload

            Dim json As String = JsonSerializer.Serialize(request, New JsonSerializerOptions With {
                .WriteIndented = True
            })

            Dim dir As String = Path.Combine(Environment.CurrentDirectory, "requests")

            If Not Directory.Exists(dir) Then
                Directory.CreateDirectory(dir)
            End If

            Dim filePath As String = Path.Combine(dir, $"req_{requestId}.json")

            File.WriteAllText(filePath, json)

            Return filePath

        End Function


        Private Sub ShowHelp()

            Console.WriteLine()
            Console.WriteLine("Usage:")
            Console.WriteLine()
            Console.WriteLine("EHFleetXRechnung.CliTester <AppPath> <cmd> <type> <invoiceNo> [email]")
            Console.WriteLine()

            Console.WriteLine("Commands:")
            Console.WriteLine("invoice.printpreview")
            Console.WriteLine("invoice.export.xml")
            Console.WriteLine("invoice.export.hybridpdf")
            Console.WriteLine("invoice.email.xml")
            Console.WriteLine("invoice.email.hybridpdf")

            Console.WriteLine()
            Console.WriteLine("Examples:")
            Console.WriteLine()

            Console.WriteLine("PrintPreview:")
            Console.WriteLine("EHFleetXRechnung.CliTester ""C:\Apps\EHFleet\EHFleetXRechnung.exe"" invoice.printpreview WA 4711")

            Console.WriteLine()
            Console.WriteLine("Export XML:")
            Console.WriteLine("EHFleetXRechnung.CliTester ""C:\Apps\EHFleet\EHFleetXRechnung.exe"" invoice.export.xml WA 4711")

            Console.WriteLine()
            Console.WriteLine("Email HybridPDF:")
            Console.WriteLine("EHFleetXRechnung.CliTester ""C:\Apps\EHFleet\EHFleetXRechnung.exe"" invoice.email.hybridpdf WA 4711 kunde@firma.de")

            Console.WriteLine()

        End Sub

    End Module

End Namespace