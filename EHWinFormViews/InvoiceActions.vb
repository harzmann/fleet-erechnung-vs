Option Strict On
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports ehfleet_classlibrary
Imports log4net

Public Module InvoiceActions

    Private ReadOnly _logger As ILog = LogManager.GetLogger(GetType(InvoiceActions))

    Public Sub ShowPrintPreview(invoiceType As String, invoiceNo As Long, db As General.Database)
        ' UI Pflicht: ReportForm öffnen
        Dim billType As RechnungsArt = MapInvoiceTypeToEnum(invoiceType)
        Dim nums As New List(Of Integer) From {CInt(invoiceNo)}

        ' Wenn noch keine MessageLoop läuft: selbst starten.
        If Not Application.MessageLoop Then
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            Application.Run(New ReportForm(db, billType, nums))
            Return
        End If

        ' Wenn UI schon läuft: modal öffnen.
        Using frm As New ReportForm(db, billType, nums)
            frm.ShowDialog()
        End Using
    End Sub

    Public Async Function ExportXmlAsync(invoiceType As String, invoiceNo As Long, db As General.Database) As Task(Of String)
        Throw New NotImplementedException("ExportXmlAsync not implemented.")
    End Function

    Public Async Function ExportHybridPdfAsync(invoiceType As String, invoiceNo As Long, db As General.Database) As Task(Of String)
        Throw New NotImplementedException("ExportHybridPdfAsync not implemented.")
    End Function

    Public Async Function EmailXmlAsync(invoiceType As String, invoiceNo As Long, toAddress As String, db As General.Database) As Task
        Throw New NotImplementedException("EmailXmlAsync not implemented.")
    End Function

    Public Async Function EmailHybridPdfAsync(invoiceType As String, invoiceNo As Long, toAddress As String, db As General.Database) As Task
        Throw New NotImplementedException("EmailHybridPdfAsync not implemented.")
    End Function

    Private Function MapInvoiceTypeToEnum(value As String) As RechnungsArt
        Select Case value.Trim().ToUpperInvariant()
            Case "WA" : Return RechnungsArt.Werkstatt
            Case "TA" : Return RechnungsArt.Tanken
            Case "MR" : Return RechnungsArt.Manuell
            Case Else
                Throw New ArgumentOutOfRangeException(NameOf(value), $"Unbekannte Rechnungsart: '{value}' (erwartet WA/TA/MR)")
        End Select
    End Function

End Module