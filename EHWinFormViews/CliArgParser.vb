Option Strict On
Option Explicit On

Public Module CliArgParser

    Public Function ParseRequestPath(args As String()) As String
        If args Is Nothing OrElse args.Length = 0 Then Return Nothing

        For i As Integer = 0 To args.Length - 1
            Dim arg As String = args(i)
            If String.IsNullOrWhiteSpace(arg) Then Continue For

            If arg.StartsWith("--request", StringComparison.OrdinalIgnoreCase) Then

                ' Variante: --request="C:\x\req.json" oder --request=C:\x\req.json
                If arg.Contains("="c) Then
                    Dim parts As String() = arg.Split({"="c}, 2) ' <-- FIX: Char() + count
                    If parts.Length = 2 Then
                        Return parts(1).Trim(""""c)
                    End If

                    ' Variante: --request "C:\x\req.json"
                ElseIf i + 1 < args.Length Then
                    Return args(i + 1).Trim(""""c)
                End If
            End If
        Next

        Return Nothing
    End Function

End Module