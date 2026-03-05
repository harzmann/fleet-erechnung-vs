Public Module CliFileRequestValidator

    Public Function Validate(request As CliFileRequest, ByRef payload As InvoicePayload) As Integer
        Dim validTypes = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {"WA", "TA", "MR"}
        If request Is Nothing OrElse String.IsNullOrWhiteSpace(request.Cmd) Then Return 2
        payload = InvoicePayload.FromJsonElement(request.Payload)
        If Not validTypes.Contains(payload.InvoiceType) Then Return 3
        If payload.InvoiceNo <= 0 Then Return 3
        If request.Cmd.StartsWith("invoice.email", StringComparison.OrdinalIgnoreCase) Then
            If String.IsNullOrWhiteSpace(payload.To) Then
                ' TODO: interne Empfängerermittlung
                Return 3
            End If
        End If
        Select Case request.Cmd.ToLowerInvariant()
            Case "invoice.printpreview", "invoice.export.xml", "invoice.export.hybridpdf", "invoice.email.xml", "invoice.email.hybridpdf"
                Return 0
            Case Else
                Return 2
        End Select
    End Function

End Module
