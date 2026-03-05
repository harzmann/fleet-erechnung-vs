Imports System.Text.Json
Public Class CliFileRequest
    Public Property Version As Integer
    Public Property RequestId As String
    Public Property CreatedUtc As DateTime
    Public Property User As String
    Public Property Cmd As String
    Public Property Payload As JsonElement
End Class

Public Class InvoicePayload
    Public Property InvoiceType As String
    Public Property InvoiceNo As Long
    Public Property Silent As Boolean?
    Public Property [To] As String

    Public Shared Function FromJsonElement(elem As JsonElement) As InvoicePayload
        Dim opts As New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True}
        Return JsonSerializer.Deserialize(Of InvoicePayload)(elem.GetRawText(), opts)
    End Function
End Class
