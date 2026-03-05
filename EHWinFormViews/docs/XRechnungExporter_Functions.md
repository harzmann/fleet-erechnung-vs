# Funktionen im Projekt EHFleetXRechnung.Viewer

## InvoiceActions (Modul)

### Public Sub ShowPrintPreview
**Parameter:**  
- `invoiceType As String`  
- `invoiceNo As Long`  
- `db As General.Database`  
**Beschreibung:**  
Öffnet die Druckvorschau (ReportForm) für die angegebene Rechnungsart und Rechnungsnummer. Startet ggf. die MessageLoop.

---

### Public Async Function ExportXmlAsync
**Parameter:**  
- `invoiceType As String`  
- `invoiceNo As Long`  
- `db As General.Database`  
**Rückgabe:**  
- `Task(Of String)`  
**Beschreibung:**  
Exportiert die Rechnung als XRechnung-XML. (Noch nicht implementiert.)

---

### Public Async Function ExportHybridPdfAsync
**Parameter:**  
- `invoiceType As String`  
- `invoiceNo As Long`  
- `db As General.Database`  
**Rückgabe:**  
- `Task(Of String)`  
**Beschreibung:**  
Exportiert die Rechnung als HybridPDF (PDF mit eingebetteter XRechnung-XML). (Noch nicht implementiert.)

---

### Public Async Function EmailXmlAsync
**Parameter:**  
- `invoiceType As String`  
- `invoiceNo As Long`  
- `toAddress As String`  
- `db As General.Database`  
**Rückgabe:**  
- `Task`  
**Beschreibung:**  
Versendet die Rechnung als XRechnung-XML per E-Mail. (Noch nicht implementiert.)

---

### Public Async Function EmailHybridPdfAsync
**Parameter:**  
- `invoiceType As String`  
- `invoiceNo As Long`  
- `toAddress As String`  
- `db As General.Database`  
**Rückgabe:**  
- `Task`  
**Beschreibung:**  
Versendet die Rechnung als HybridPDF per E-Mail. (Noch nicht implementiert.)

---

### Private Function MapInvoiceTypeToEnum
**Parameter:**  
- `value As String`  
**Rückgabe:**  
- `RechnungsArt`  
**Beschreibung:**  
Wandelt den String-Code (WA/TA/MR) in das RechnungsArt-Enum um.

---

## XRechnungExporter (Klasse)

### Public Function CreateBillXml
**Parameter:**  
- `xmlStream As Stream`  
- `billType As RechnungsArt`  
- `rechnungsNummer As Integer`  
- `Optional storeXmlAndUpdate As Boolean = False`  
- `Optional logEntry As ExportLogEntry = Nothing`  
**Rückgabe:**  
- `Boolean`  
**Beschreibung:**  
Erzeugt eine XRechnung-XML für die angegebene Rechnung und schreibt sie in den Stream. Optional wird die XML validiert, gespeichert und der Status in der Datenbank aktualisiert.

---

### Public Function ValidateBuyerData
**Parameter:**  
- `buyerData As Dictionary(Of String, String)`  
**Rückgabe:**  
- `Boolean`  
**Beschreibung:**  
Prüft, ob die Käuferdaten für die XRechnung vollständig sind (LandISO und EmailRechnung sind Pflicht).

---

### Public Function Validate
**Parameter:**  
- `file As String`  
- `Optional silent As Boolean = False`  
- `Optional logEntry As ExportLogEntry = Nothing`  
**Rückgabe:**  
- `Boolean`  
**Beschreibung:**  
Validiert eine XRechnung-XML-Datei mit externer Java-Validierungssoftware.

---

### Public Function GetExportPath
**Parameter:**  
- `billType As RechnungsArt`  
**Rückgabe:**  
- `String`  
**Beschreibung:**  
Liefert den Export-Ordnerpfad für die angegebene Rechnungsart.

---

### Public Function GetExportFilePath
**Parameter:**  
- `billType As RechnungsArt`  
- `rechnungsNummer As Integer`  
- `extension As String`  
**Rückgabe:**  
- `String`  
**Beschreibung:**  
Liefert den vollständigen Dateipfad für den Export (inkl. Dateiname).

---

### Public Function GetFormattedBillNumber
**Parameter:**  
- `billNumber As Integer`  
- `billType As RechnungsArt`  
**Rückgabe:**  
- `String`  
**Beschreibung:**  
Formatiert die Rechnungsnummer mit Präfix (z.B. "WA-000123").

---

### Public Function GetMeasurementCode
**Parameter:**  
- `type As String`  
- `quantity As Decimal`  
- `unitPrice As Decimal`  
- `Optional roundQtyDecimals As Integer = 2`  
- `Optional unitPriceDecimals As Integer = 2`  
**Rückgabe:**  
- `MeasurementCode`  
**Beschreibung:**  
Ermittelt Mengeneinheit, Umrechnungsfaktor und rechnet Menge/Preis um.

---

### Public Function IsRechnungIssued
**Parameter:**  
- `rechnungsNummer As Integer`  
- `billType As RechnungsArt`  
**Rückgabe:**  
- `Boolean`  
**Beschreibung:**  
Prüft, ob die Rechnung bereits den Status "Issued" hat.

---

### Public Function GetXmlRawFromBlob
**Parameter:**  
- `rechnungsNummer As Integer`  
- `billType As RechnungsArt`  
**Rückgabe:**  
- `Byte()`  
**Beschreibung:**  
Liest das XML-Raw-Feld aus der passenden Blob-Tabelle für die Rechnung.

---

### Public Sub StoreXmlAndUpdateStatus
**Parameter:**  
- `xmlBytes As Byte()`  
- `rechnungsNummer As Integer`  
- `format As String`  
- `profil As String`  
- `billType As RechnungsArt`  
**Beschreibung:**  
Speichert die XML in der Datenbank (Blob), erzeugt eine Signatur und aktualisiert den Kopfdatensatz.

---

### Public Sub ExportFinalizedXmlDuplicate
**Parameter:**  
- `exportStream As Stream`  
- `billType As RechnungsArt`  
- `rechnungsNummer As Integer`  
- `Optional logEntry As ExportLogEntry = Nothing`  
**Beschreibung:**  
Exportiert die bereits festgeschriebene Rechnung als XML-Duplikat aus der Datenbank.

---

### Public Function GetPdfRawFromBlob
**Parameter:**  
- `rechnungsNummer As Integer`  
- `billType As RechnungsArt`  
**Rückgabe:**  
- `Byte()`  
**Beschreibung:**  
Liest das PDF-Raw-Feld aus der passenden Blob-Tabelle für die Rechnung.

---

### Public Sub StorePdfToBlob
**Parameter:**  
- `pdfBytes As Byte()`  
- `rechnungsNummer As Integer`  
- `billType As RechnungsArt`  
**Beschreibung:**  
Speichert das PDF im Feld PDF_Raw in der passenden Blob-Tabelle.

---

### Public Sub CreateHybridPdfA_ZUGFeRD
**Parameter:**  
- `Optional pdfRaw As MemoryStream = Nothing`  
- `Optional xmlRaw As MemoryStream = Nothing`  
- `Optional outputStream As Stream = Nothing`  
- `Optional rechnungsNummer As Integer = 0`  
- `Optional billType As RechnungsArt = RechnungsArt.Werkstatt`  
- `Optional logEntry As ExportLogEntry = Nothing`  
**Beschreibung:**  
Erstellt eine Hybrid-PDF/A-3 Datei (ZUGFeRD/XRechnung) aus PDF und XML, optional mit Speicherung.

---

### Public Sub CreateHybridPdfA
**Parameter:**  
- `Optional pdfRaw As MemoryStream = Nothing`  
- `Optional xmlRaw As MemoryStream = Nothing`  
- `Optional outputStream As Stream = Nothing`  
- `Optional rechnungsNummer As Integer = 0`  
- `Optional billType As RechnungsArt = RechnungsArt.Werkstatt`  
- `Optional logEntry As ExportLogEntry = Nothing`  
**Beschreibung:**  
Erstellt eine Hybrid-PDF/A-3 Datei mit bytegenauer Einbettung der XML (PDF/A-3 Associated File).

---

## XRechnungEmail (Klasse)

### Public Function SendXRechnungXml
**Parameter:**  
- `billType As RechnungsArt`  
- `rechnungsNummer As Integer`  
- `empfaengerEmail As String`  
- `Optional logEntry As ExportLogEntry = Nothing`  
**Rückgabe:**  
- `Boolean`  
**Beschreibung:**  
Sendet die XRechnung-XML für die angegebene Rechnungsnummer und Rechnungsart an die übergebene E-Mail-Adresse. SMTP-Konfiguration und Betreff/Body-Template werden aus der Datenbank geladen. Versand und Logging erfolgen inklusive Datenbankprotokollierung.

---

### Public Function SendXRechnungPdf
**Parameter:**  
- `billType As RechnungsArt`  
- `rechnungsNummer As Integer`  
- `empfaengerEmail As String`  
- `Optional logEntry As ExportLogEntry = Nothing`  
**Rückgabe:**  
- `Boolean`  
**Beschreibung:**  
Sendet das Hybrid-PDF für die angegebene Rechnungsnummer und Rechnungsart an die übergebene E-Mail-Adresse. SMTP-Konfiguration und Body-Template werden aus der Datenbank geladen. Versand und Logging erfolgen inklusive Datenbankprotokollierung.

---
*(Private Hilfsfunktionen und interne Logik sind in der Klasse implementiert, aber hier nicht einzeln dokumentiert.)*

---