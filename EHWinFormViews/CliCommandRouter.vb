Imports System.Threading.Tasks
Imports ehfleet_classlibrary

Public Module CliCommandRouter
    Public Async Function DispatchAsync(request As CliFileRequest, db As General.Database) As Task(Of Integer)
        Try
            Dim payload As InvoicePayload = Nothing
            Dim validationCode = CliFileRequestValidator.Validate(request, payload)
            If validationCode <> 0 Then Return validationCode

            Select Case request.Cmd.ToLowerInvariant()
                Case "invoice.printpreview"
                    ' Falls benötigt, db an InvoiceActions übergeben
                    InvoiceActions.ShowPrintPreview(payload.InvoiceType, payload.InvoiceNo, db)
                    Return 0
                Case "invoice.export.xml"
                    Await InvoiceActions.ExportXmlAsync(payload.InvoiceType, payload.InvoiceNo, db)
                    Return 0
                Case "invoice.export.hybridpdf"
                    Await InvoiceActions.ExportHybridPdfAsync(payload.InvoiceType, payload.InvoiceNo, db)
                    Return 0
                Case "invoice.email.xml"
                    Await InvoiceActions.EmailXmlAsync(payload.InvoiceType, payload.InvoiceNo, payload.To, db)
                    Return 0
                Case "invoice.email.hybridpdf"
                    Await InvoiceActions.EmailHybridPdfAsync(payload.InvoiceType, payload.InvoiceNo, payload.To, db)
                    Return 0
                Case Else
                    Return 2
            End Select
        Catch ex As Exception
            CliLogger.LogException(ex)
            Return 10
        End Try
    End Function

End Module
