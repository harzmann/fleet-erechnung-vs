Option Strict On
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.IO
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
        Return Await Task.Run(Function()
                                  Dim billType As RechnungsArt = MapInvoiceTypeToEnum(invoiceType)
                                  Dim exporter As New XRechnungExporter(db)
                                  Dim logEntry As New ExportLogEntry With {
                .RechnungsNummer = CInt(invoiceNo),
                .Timestamp = DateTime.UtcNow
            }
                                  Dim xmlFilePath As String = exporter.GetExportFilePath(billType, CInt(invoiceNo), "xml")
                                  Directory.CreateDirectory(Path.GetDirectoryName(xmlFilePath))

                                  Try
                                      If Not exporter.IsRechnungIssued(CInt(invoiceNo), billType) Then
                                          Using fs = File.Create(xmlFilePath)
                                              exporter.CreateBillXml(fs, billType, CInt(invoiceNo), True, logEntry)
                                          End Using
                                      Else
                                          Using fs = File.Create(xmlFilePath)
                                              exporter.ExportFinalizedXmlDuplicate(fs, billType, CInt(invoiceNo), logEntry)
                                          End Using
                                      End If

                                      If Not exporter.IsSuccess Then
                                          Throw New Exception("Export XML fehlgeschlagen.")
                                      End If

                                      Return xmlFilePath
                                  Catch ex As Exception
                                      _logger.Error("Fehler beim ExportXmlAsync", ex)
                                      Throw
                                  End Try
                              End Function)
    End Function

    Public Async Function ExportHybridPdfAsync(invoiceType As String, invoiceNo As Long, db As General.Database) As Task(Of String)
        Return Await Task.Run(Function()
                                  Dim billType As RechnungsArt = MapInvoiceTypeToEnum(invoiceType)
                                  Dim exporter As New XRechnungExporter(db)
                                  Dim logEntry As New ExportLogEntry With {
                .RechnungsNummer = CInt(invoiceNo),
                .Timestamp = DateTime.UtcNow
            }
                                  Dim xmlFilePath As String = exporter.GetExportFilePath(billType, CInt(invoiceNo), "xml")
                                  Dim hybridPdfPath As String = exporter.GetExportFilePath(billType, CInt(invoiceNo), "hybrid.pdf")
                                  Directory.CreateDirectory(Path.GetDirectoryName(xmlFilePath))
                                  Directory.CreateDirectory(Path.GetDirectoryName(hybridPdfPath))

                                  Try
                                      ' Erst XML exportieren
                                      If Not exporter.IsRechnungIssued(CInt(invoiceNo), billType) Then
                                          Using fs = File.Create(xmlFilePath)
                                              exporter.CreateBillXml(fs, billType, CInt(invoiceNo), True, logEntry)
                                          End Using
                                      Else
                                          Using fs = File.Create(xmlFilePath)
                                              exporter.ExportFinalizedXmlDuplicate(fs, billType, CInt(invoiceNo), logEntry)
                                          End Using
                                      End If

                                      If Not exporter.IsSuccess Then
                                          Throw New Exception("Export XML für Hybrid-PDF fehlgeschlagen.")
                                      End If

                                      Dim xmlBytes As Byte() = File.ReadAllBytes(xmlFilePath)
                                      Dim pdfBytes As Byte() = exporter.GetPdfRawFromBlob(CInt(invoiceNo), billType)
                                      If pdfBytes Is Nothing OrElse pdfBytes.Length = 0 Then
                                          Throw New Exception("PDF-Blob ist leer oder nicht vorhanden.")
                                      End If

                                      Using msPdf As New MemoryStream(pdfBytes)
                                          Using msXml As New MemoryStream(xmlBytes)
                                              Using outFs As FileStream = File.Create(hybridPdfPath)
                                                  exporter.CreateHybridPdfA_ZUGFeRD(msPdf, msXml, outFs, CInt(invoiceNo), billType, logEntry)
                                              End Using
                                          End Using
                                      End Using

                                      Return hybridPdfPath
                                  Catch ex As Exception
                                      _logger.Error("Fehler beim ExportHybridPdfAsync", ex)
                                      Throw
                                  End Try
                              End Function)
    End Function

    Public Async Function EmailXmlAsync(invoiceType As String, invoiceNo As Long, toAddress As String, db As General.Database) As Task
        Await Task.Run(Sub()
                           Dim billType As RechnungsArt = MapInvoiceTypeToEnum(invoiceType)
                           Dim mailer As New XRechnungEmail(db)
                           Dim logEntry As New ExportLogEntry With {
                .RechnungsNummer = CInt(invoiceNo),
                .EmailEmpfaenger = toAddress,
                .Timestamp = DateTime.UtcNow
            }
                           Try
                               Dim ok = mailer.SendXRechnungXml(billType, CInt(invoiceNo), toAddress, logEntry)
                               If Not ok Then
                                   Throw New Exception("E-Mail Versand der XRechnung-XML fehlgeschlagen.")
                               End If
                           Catch ex As Exception
                               _logger.Error("Fehler beim EmailXmlAsync", ex)
                               Throw
                           End Try
                       End Sub)
    End Function

    Public Async Function EmailHybridPdfAsync(invoiceType As String, invoiceNo As Long, toAddress As String, db As General.Database) As Task
        Await Task.Run(Sub()
                           Dim billType As RechnungsArt = MapInvoiceTypeToEnum(invoiceType)
                           Dim mailer As New XRechnungEmail(db)
                           Dim logEntry As New ExportLogEntry With {
                .RechnungsNummer = CInt(invoiceNo),
                .EmailEmpfaenger = toAddress,
                .Timestamp = DateTime.UtcNow
            }
                           Try
                               Dim ok = mailer.SendXRechnungPdf(billType, CInt(invoiceNo), toAddress, logEntry)
                               If Not ok Then
                                   Throw New Exception("E-Mail Versand des Hybrid-PDF fehlgeschlagen.")
                               End If
                           Catch ex As Exception
                               _logger.Error("Fehler beim EmailHybridPdfAsync", ex)
                               Throw
                           End Try
                       End Sub)
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