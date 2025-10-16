Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Threading
Imports System.Windows.Forms
Imports ehfleet_classlibrary
Imports log4net
Imports s2industries.ZUGFeRD
Imports Stimulsoft.Database

Public Class XRechnungExporter
    Private ReadOnly _dataConnection As General.Database

    Private Shared ReadOnly _logger As ILog

    Private ReadOnly _exportPaths As Dictionary(Of RechnungsArt, String) = New Dictionary(Of RechnungsArt, String)

    Public IsSuccess As Boolean = False
    Public Cancel As Boolean = False

    Public Structure MeasurementCode
        Public UnitCode As QuantityCodes
        Public Quantity As Decimal           ' umgerechnete Menge
        Public UnitPrice As Decimal          ' umgerechneter Einzelpreis (je neuer Einheit)
        Public ConversionFactor As Decimal   ' verwendeter Umrechnungsfaktor
        Public OriginalUnitPrice As Decimal  ' übergebener Originalpreis (Info)

        Public Sub New(unitCode As QuantityCodes,
                   quantity As Decimal,
                   unitPrice As Decimal,
                   factor As Decimal,
                   Optional originalUnitPrice As Decimal = 0D)
            Me.UnitCode = unitCode
            Me.Quantity = quantity
            Me.UnitPrice = unitPrice
            Me.ConversionFactor = If(factor = 0D, 1D, factor)
            Me.OriginalUnitPrice = originalUnitPrice
        End Sub
    End Structure

    Shared Sub New()
        _logger = LogManager.GetLogger(GetType(XRechnungExporter))
    End Sub

    Public Sub New(dataConnection As General.Database)

        _dataConnection = dataConnection
        _logger.Debug($"Instantiating {NameOf(XRechnungExporter)}")

        _exportPaths = [Enum].GetValues(
            GetType(RechnungsArt)).OfType(Of RechnungsArt).ToDictionary(
                Function(billType) billType,
                    Function(billType)
                        Dim sellerData = GetAdditionalSellerParameter(billType)
                        Dim path = String.Empty
                        sellerData.TryGetValue("Ausgabepfad", path)
                        Return path
                    End Function)

        _logger.Debug($"Leaving {NameOf(XRechnungExporter)} constructor")

    End Sub


    ' ========================================================
    ' ==== CreateBillXml: refaktoriert in Helper-Methoden ====
    ' ========================================================

    Public Function CreateBillXml(xmlStream As Stream,
                              billType As RechnungsArt,
                              rechnungsNummer As Integer,
                              Optional storeXmlAndUpdate As Boolean = False) As Boolean

        _logger.Debug($"Entering {NameOf(CreateBillXml)}({xmlStream.GetType().Name}, {billType}, {rechnungsNummer}, storeXmlAndUpdate={storeXmlAndUpdate})")
        IsSuccess = False

        Try
            ' --- Daten laden ---
            Dim sqls = GetSqlStatements(billType, New List(Of Integer) From {rechnungsNummer})
            Dim items = GetItemsFromQuery(GetSqlStatementForBill(billType, New List(Of Integer) From {rechnungsNummer})).FirstOrDefault
            If items Is Nothing Then
                _logger.Warn($"No data found for Bill {billType}-{rechnungsNummer}")
                Return False
            End If

            Dim sellerData = GetSellerParameter(billType)
            Dim additionalSellerData = GetAdditionalSellerParameter(billType)
            Dim buyerData = GetBuyerData(GetDataFromColumn(items, "KundenNr"))

            If Not ValidateBuyerData(buyerData) Then
                _logger.Warn($"Validation for Buyer failed - Matchcode: " & GetSafe(buyerData, "Matchcode"))
                Return False
            End If

            ' --- Beleg erzeugen ---
            Dim xRechnung = New InvoiceDescriptor()

            Dim billTypeText As String = ""
            Dim formattedInvoiceNumber As String = Sale.Invoicing.FormatInvoiceNumber(_dataConnection, billTypeText, rechnungsNummer)
            Dim orderNo As String = DetermineOrderNo(billType, items, formattedInvoiceNumber)

            FillHeader(xRechnung, items, orderNo, formattedInvoiceNumber)
            FillSeller(xRechnung, sellerData, additionalSellerData, buyerData)
            FillBuyer(xRechnung, items, buyerData, formattedInvoiceNumber)
            AddPaymentTerms(xRechnung, items)
            AddPaymentMeans(xRechnung, additionalSellerData)

            ' --- Positionen & Aggregation ---
            Dim sumNetLines As Decimal = 0D
            Dim taxGroups As New Dictionary(Of String, TaxAgg)(StringComparer.OrdinalIgnoreCase)

            _logger.Debug("Filling line items")
            For Each query In sqls
                Dim rows = GetItemsFromQuery(query)
                If Not rows.Any() Then Continue For

                For Each lineItemData In rows
                    If Not lineItemData.Any() Then Continue For

                    Select Case billType
                        Case RechnungsArt.Werkstatt
                            ' Stamm-Artikel
                            TryAddLine(xRechnung, lineItemData, "Matchcode", "Artikelbez", "ArtikelMEH", sumNetLines, taxGroups)
                            ' Personal
                            TryAddLine(xRechnung, lineItemData, "FahrerNr", "FahrerName", "PersonalMEH", sumNetLines, taxGroups)
                            ' Manueller Artikel
                            TryAddLine(xRechnung, lineItemData, "freieArtikelNr", "freieArtikelbez", "freieArtikeleinheit", sumNetLines, taxGroups)
                        Case RechnungsArt.Tanken
                            ' Tankbuchungen
                            TryAddLine(xRechnung, lineItemData, "Bezeichnung", "SpritID", "Mengeneinheit", sumNetLines, taxGroups)

                    End Select
                Next
            Next

            ' --- Steueraufschlüsselung + Summen ---
            AddTaxBreakdownAndTotals(xRechnung, sumNetLines, taxGroups)

            ' --- XML schreiben ---
            xRechnung.Save(xmlStream, ZUGFeRDVersion.Version23, Profile.XRechnung, ZUGFeRDFormats.UBL)

            ' --- Optional: Speichern/Status updaten ---
            If storeXmlAndUpdate Then
                Dim xmlBytes As Byte() = StreamToBytes(xmlStream)
                Dim format As String = ZUGFeRDFormats.UBL.ToString
                Dim profil As String = Profile.XRechnung.ToString

                Dim tempFile As String = Path.GetTempFileName()
                Try
                    File.WriteAllBytes(tempFile, xmlBytes)
                    Dim isValid As Boolean = Validate(tempFile, silent:=True)
                    File.Delete(tempFile)
                    If Not isValid Then
                        _logger.Warn("XML-Validierung fehlgeschlagen, Speicherung wird abgebrochen.")
                        Return False
                    End If
                Catch ex As Exception
                    _logger.Error("Fehler bei der temporären XML-Validierung: " & ex.Message)
                    If File.Exists(tempFile) Then
                        Try : File.Delete(tempFile) : Catch : End Try
                    End If
                    Return False
                End Try

                StoreXmlAndUpdateStatus(xmlBytes, rechnungsNummer, format, profil)
            End If

            IsSuccess = True
            Return True

        Catch ex As Exception
            _logger.Error($"Exception during {NameOf(CreateBillXml)}", ex)
            MessageBox.Show("Speichern fehlgschlagen!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            _logger.Debug($"Leaving {NameOf(CreateBillXml)}")
        End Try
    End Function


    ' =======================
    ' ====== Helpers ========
    ' =======================

    ' --- Bestimme die Auftrags-/Referenznummer ---
    Private Function DetermineOrderNo(billType As RechnungsArt, items As Object, formattedInvoiceNumber As String) As String
        Select Case billType
            Case RechnungsArt.Werkstatt
                Dim waId As Integer = SafeInt(items, "WAID")
                If waId > 0 Then
                    Return "Werkstattauftrag: " & waId.ToString()
                End If
                Return formattedInvoiceNumber
            Case RechnungsArt.Tanken, RechnungsArt.Manuell
                Return formattedInvoiceNumber
            Case Else
                Return formattedInvoiceNumber
        End Select
    End Function

    ' --- Kopf füllen ---
    Private Sub FillHeader(x As InvoiceDescriptor, items As Object, orderNo As String, invoiceNo As String)
        _logger.Debug("Filling invoice header data")
        x.InvoiceNo = invoiceNo
        x.InvoiceDate = SafeDate(items, "Rechnungsdatum")
        x.Currency = CurrencyCodes.EUR
        x.OrderNo = orderNo
        x.ActualDeliveryDate = SafeDate(items, "Lieferdatum")
    End Sub

    ' --- Verkäufer füllen ---
    Private Sub FillSeller(x As InvoiceDescriptor, seller As Object, addSeller As Object, buyer As Object)
        _logger.Debug("Filling seller data")
        x.SetSeller(
            name:=GetDataFromColumn(seller, "Firma"),
            postcode:=GetDataFromColumn(seller, "PLZ"),
            city:=GetDataFromColumn(seller, "Ort"),
            street:=GetDataFromColumn(seller, "Straße"),
            country:=CountryCodes.DE,
            id:=GetSafe(buyer, "LieferantenNr")
        )
        x.AddSellerTaxRegistration(GetDataFromColumn(seller, "Steuernummer"), TaxRegistrationSchemeID.VA)
        x.SetSellerElectronicAddress(
            address:=GetDataFromColumn(addSeller, "Modul1"),
            electronicAddressSchemeID:=ElectronicAddressSchemeIdentifiers.EM
        )

        ' --- SELLER CONTACT (BG-6) ---
        ' BR-DE-2 verlangt BG-6 – wir setzen mind. den Namen; E-Mail/Telefon falls vorhanden
        x.SetSellerContact(
            name:=GetDataFromColumn(seller, "Firma"),
            emailAddress:=GetDataFromColumn(addSeller, "Modul1"),
            phoneno:=GetDataFromColumn(seller, "Telefon")
        )

    End Sub

    ' --- Käufer füllen ---
    Private Sub FillBuyer(x As InvoiceDescriptor, items As Object, buyer As Object, paymentRef As String)
        _logger.Debug("Filling buyer data")
        Dim country As CountryCodes = CountryCodes.DE
        Try
            country = CType([Enum].Parse(GetType(CountryCodes), GetSafe(buyer, "LandISO")), CountryCodes)
        Catch
        End Try

        x.SetBuyer(
            name:=GetDataFromColumn(items, "Firma"),
            postcode:=GetDataFromColumn(items, "Postleitzahl"),
            city:=GetDataFromColumn(items, "Ort"),
            street:=GetDataFromColumn(items, "Rechnungsadresse"),
            country:=country,
            id:=GetDataFromColumn(items, "KundenNr"),
            countrySubdivisonName:=GetDataFromColumn(items, "FirmaOderAbteilung")
        )

        ' Umsatzste-ID (BT-15 / BT-48)
        x.AddBuyerTaxRegistration(no:=GetDataFromColumn(items, "UStIdNr"), schemeID:=TaxRegistrationSchemeID.VA)

        ' E-Mail für die Rechnung (BT-14)
        x.SetBuyerElectronicAddress(address:=GetSafe(buyer, "EmailRechnung"), electronicAddressSchemeID:=ElectronicAddressSchemeIdentifiers.EM)

        ' Zahlungsreferenz (BT-83)
        x.PaymentReference = paymentRef

        ' Buyer Reference (BT-10)
        Dim buyerRef As String = GetSafe(buyer, "LeitwegeID")
        If buyerRef = "" Then buyerRef = paymentRef
        x.ReferenceOrderNo = buyerRef

        ' Deliver-to Country (BT-80) über ShipTo setzen
        Try
            Dim shipTo As New Party()
            shipTo.Postcode = GetDataFromColumn(items, "Postleitzahl")
            shipTo.City = GetDataFromColumn(items, "Ort")
            shipTo.Street = GetDataFromColumn(items, "Rechnungsadresse")
            shipTo.Country = country
            x.ShipTo = shipTo
        Catch ex As Exception
            _logger.Warn($"ShipTo/BT-80 konnte nicht gesetzt werden: {ex.Message}")
        End Try

    End Sub

    ' --- Zahlungsbedingungen ---
    Private Sub AddPaymentTerms(x As InvoiceDescriptor, items As Object)
        _logger.Debug("Filling payment terms")
        Dim billDate As Date = SafeDate(items, "Rechnungsdatum")

        Dim netDays As Integer = SafeInt(items, "NettoTage")
        Dim dueNet As Date = billDate.AddDays(netDays)
        x.AddTradePaymentTerms($"Zahlbar innerhalb {netDays} Tagen netto bis {dueNet:dd.MM.yyyy}", dueNet)

        _logger.Debug("Filling payment terms for Skonto")
        Dim skDays As Integer = SafeInt(items, "SkontoTage")
        Dim skPct As Decimal = SafeDec(items, "SkontoProzent")
        If skDays > 0 AndAlso skPct > 0D Then
            Dim dueSk As Date = billDate.AddDays(skDays)
            x.AddTradePaymentTerms($"{skPct:0.##}% Skonto innerhalb {skDays} Tagen bis {dueSk:dd.MM.yyyy}",
                               dueSk, PaymentTermsType.Skonto, skDays, skPct)
        End If
    End Sub

    ' --- Zahlungsinfos (Bank) ---
    Private Sub AddPaymentMeans(x As InvoiceDescriptor, addSeller As Object)
        _logger.Debug("Filling payment information")
        x.SetPaymentMeans(PaymentMeansTypeCodes.CreditTransfer)
        x.AddCreditorFinancialAccount(
            iban:=GetDataFromColumn(addSeller, "Modul2"),
            bic:=GetDataFromColumn(addSeller, "Modul3"),
            bankName:=GetDataFromColumn(addSeller, "Modul4")
        )
    End Sub

    ' --- Struktur zur Steuer-Aggregation ---
    Private Structure TaxAgg
        Public Basis As Decimal
        Public Percent As Decimal
        Public Category As TaxCategoryCodes
        Public ReasonCode As String
        Public ReasonText As String
    End Structure

    ' --- Zeile hinzufügen & aggregieren ---
    Private Sub TryAddLine(x As InvoiceDescriptor,
                       line As Object,
                       nameKey As String,
                       descKey As String,
                       unitKey As String,
                       ByRef sumNetLines As Decimal,
                       ByRef taxGroups As Dictionary(Of String, TaxAgg))

        Dim name As String = GetSafe(line, nameKey)
        If String.IsNullOrWhiteSpace(name) Then Exit Sub

        ' Grundbeschreibung aus Datenquelle
        Dim desc As String = GetSafe(line, descKey)

        ' Kontext-Erkennung
        Dim isPersonal As Boolean =
        String.Equals(unitKey, "PersonalMEH", StringComparison.OrdinalIgnoreCase) _
        OrElse Not String.IsNullOrWhiteSpace(GetSafe(line, "FahrerNr"))

        ' Tanken erkennen über typische Felder aus abfr_tankabrdetail
        Dim isTanken As Boolean =
        Not String.IsNullOrWhiteSpace(GetSafe(line, "Tankdatum")) _
        OrElse Not String.IsNullOrWhiteSpace(GetSafe(line, "Produktart")) _
        OrElse Not String.IsNullOrWhiteSpace(GetSafe(line, "KFZNr")) _
        OrElse Not String.IsNullOrWhiteSpace(GetSafe(line, "Kennzeichen")) _
        OrElse Not String.IsNullOrWhiteSpace(GetSafe(line, "Kilometerstand"))

        ' Metadaten-Block für Description aufbauen
        Dim metaLines As New List(Of String)

        If isPersonal Then
            ' Werkstatt/Personal: Datum aus abfr_wavkabrdetail
            Dim d = GetSafe(line, "Datum")
            If Not String.IsNullOrWhiteSpace(d) Then
                Dim dTxt As String = d
                ' optional versuchen zu formatieren
                Try
                    Dim dt As DateTime
                    If DateTime.TryParse(d, dt) Then
                        dTxt = dt.ToString("dd.MM.yyyy")
                    End If
                Catch
                End Try
                metaLines.Add(";Datum: " & dTxt)
            End If
        End If

        If isTanken Then
            ' Tanken: Zusatzinfos aus abfr_tankabrdetail
            Dim tankdatum = GetSafe(line, "Tankdatum")
            If Not String.IsNullOrWhiteSpace(tankdatum) Then
                Try
                    Dim dt As DateTime
                    If DateTime.TryParse(tankdatum, dt) Then
                        tankdatum = dt.ToString("dd.MM.yyyy HH:mm:ss")
                    End If
                Catch
                End Try
            End If

            Dim km = GetSafe(line, "Kilometerstand")
            ' Reihenfolge wie gewünscht
            Dim produktart = GetSafe(line, "Produktart")
            Dim bez = GetSafe(line, "Bezeichnung")
            Dim kfzNr = GetSafe(line, "KFZNr")
            Dim kz = GetSafe(line, "Kennzeichen")

            ' --- Semikolon-getrennte Felder ohne führendes Semikolon ---
            If Not String.IsNullOrWhiteSpace(produktart) Then metaLines.Add("Produktart: " & produktart)
            If Not String.IsNullOrWhiteSpace(bez) Then metaLines.Add("Bezeichnung: " & bez)
            If Not String.IsNullOrWhiteSpace(kfzNr) Then metaLines.Add("KFZ-Nr.: " & kfzNr)
            If Not String.IsNullOrWhiteSpace(kz) Then metaLines.Add("Kennzeichen: " & kz)
            If Not String.IsNullOrWhiteSpace(tankdatum) Then metaLines.Add("Tankdatum: " & tankdatum)
            If Not String.IsNullOrWhiteSpace(km) Then metaLines.Add("Kilometerstand: " & km)
        End If

        ' Metadaten an Description anhängen (weitere cbc:Description wäre auch möglich)
        If metaLines.Count > 0 Then
            Dim metaBlock As String
            metaBlock = String.Join(";", metaLines)
            If String.IsNullOrWhiteSpace(desc) Then
                desc = metaBlock
            Else
                desc &= Environment.NewLine & metaBlock
            End If
        End If

        ' Mess-/Preis-Umrechnung
        ' Standard: 2 Nachkommastellen; für Tanken Preis = 4, Menge = 3,
        Dim qtyDecimals As Integer = 2
        Dim unitPriceDecimals As Integer = If(isTanken, 4, 2)
        If isTanken Then
            qtyDecimals = 3
        End If

        ' Menge/Preis aus Quelle
        Dim qtyRaw As Decimal = SafeDec(line, "Menge")
        Dim unitPriceRaw As Decimal = SafeDec(line, "EPreis")
        Dim lineNet As Decimal = SafeDec(line, "Gesamtpreis")

        ' Unit ermitteln
        Dim unitKeyValue As String = GetSafe(line, unitKey)

        ' MeasurementCode inkl. umgerechneter Menge & Einzelpreis holen
        Dim mc As MeasurementCode = GetMeasurementCode(
            type:=unitKeyValue,
            quantity:=qtyRaw,
            unitPrice:=unitPriceRaw,
            roundQtyDecimals:=qtyDecimals,
            unitPriceDecimals:=unitPriceDecimals
        )

        ' Steuer / Kategorie
        Dim cat As TaxCategoryCodes = ParseTaxCategoryCode(GetSafe(line, "ERechnung_BT151"))
        Dim vatPct As Decimal = SafeDec(line, "MwStProzent")

        ' Position hinzufügen
        Dim li = x.AddTradeLineItem(
            name:=name,
            description:=desc,
            unitCode:=mc.UnitCode,
            unitQuantity:=1D,
            billedQuantity:=mc.Quantity,
            netUnitPrice:=mc.UnitPrice,
            lineTotalAmount:=lineNet,
            taxType:=TaxTypes.VAT,
            categoryCode:=cat,
            taxPercent:=vatPct
        )

        ' Exemption normalisieren
        Dim exText As String = GetSafe(line, "ERechnung_BT120").Trim()
        Dim exCode As String = GetSafe(line, "ERechnung_BT121").Trim()
        ApplyExemptionDefaults(cat, vatPct, exText, exCode)

        ' Positionsrabatt explizit ausweisen, falls Basis > Zeilennetto
        AddLineAllowanceIfAny(li, mc.Quantity, mc.UnitPrice, lineNet, GetSafe(line, "Rabatt"))

        ' Aggregation
        sumNetLines += lineNet

        Dim key As String = $"{CInt(cat)}|{vatPct:0.####}|{exCode}|{exText}"
        If taxGroups.ContainsKey(key) Then
            Dim tg = taxGroups(key)
            tg.Basis += lineNet
            taxGroups(key) = tg
        Else
            taxGroups(key) = New TaxAgg With {
            .Basis = lineNet,
            .Percent = vatPct,
            .Category = cat,
            .ReasonCode = exCode,
            .ReasonText = exText
        }
        End If
    End Sub

    ' --- Defaults für Befreiungsgründe setzen (EU-IGL/Reverse Charge etc.) ---
    Private Sub ApplyExemptionDefaults(ByRef cat As TaxCategoryCodes,
                                   ByRef percent As Decimal,
                                   ByRef reasonText As String,
                                   ByRef reasonCode As String)
        Select Case cat
            Case TaxCategoryCodes.K ' Intra-Community supply
                If String.IsNullOrWhiteSpace(reasonText) Then reasonText = "Intra-Community supply"
                If String.IsNullOrWhiteSpace(reasonCode) Then reasonCode = "VATEX-EU-IC"
                percent = 0D
            Case TaxCategoryCodes.AE ' Reverse charge
                If String.IsNullOrWhiteSpace(reasonText) Then reasonText = "Reverse charge"
                If String.IsNullOrWhiteSpace(reasonCode) Then reasonCode = "VATEX-EU-AE"
                percent = 0D
            Case TaxCategoryCodes.G ' Export outside EU
                If String.IsNullOrWhiteSpace(reasonText) Then reasonText = "Export outside the EU"
                If String.IsNullOrWhiteSpace(reasonCode) Then reasonCode = "VATEX-EU-G"
                percent = 0D
            Case TaxCategoryCodes.O ' Not subject to VAT
                If String.IsNullOrWhiteSpace(reasonText) Then reasonText = "Not subject to VAT"
                If String.IsNullOrWhiteSpace(reasonCode) Then reasonCode = "VATEX-EU-O"
                percent = 0D
            Case Else
                ' S, Z, E etc. unverändert
        End Select
    End Sub

    ' --- Positions-ALLOWANCE (Rabatt) hinzufügen, wenn vorhanden ---
    Private Sub AddLineAllowanceIfAny(li As TradeLineItem,
                                  qty As Decimal,
                                  unitNet As Decimal,
                                  lineNet As Decimal,
                                  rabattField As String)
        Dim baseAmount As Decimal = Round2(qty * unitNet)
        Dim allowanceAmount As Decimal = Math.Max(0D, Round2(baseAmount - lineNet))
        If allowanceAmount <= 0D Then Exit Sub

        Dim reason As String = "Positionsrabatt"
        Dim pctText As String = FormatPctFromRaw(rabattField)
        If pctText <> "" Then
            reason &= $" {pctText}%"
        End If

        Dim pctNullable As Decimal? = If(baseAmount > 0D, Round2(allowanceAmount / baseAmount * 100D), CType(Nothing, Decimal?))

        ' Wichtig: Allowance (Rabatt), nicht Charge!
        li.AddSpecifiedTradeAllowance(
            currency:=CurrencyCodes.EUR,
            basisAmount:=baseAmount,
            actualAmount:=allowanceAmount,
            chargePercentage:=pctText,
            reason:=reason
        )
    End Sub

    ' --- VAT Breakdown + Summen setzen ---
    Private Sub AddTaxBreakdownAndTotals(x As InvoiceDescriptor,
                                     sumNetLines As Decimal,
                                     taxGroups As Dictionary(Of String, TaxAgg))

        Dim totalVAT As Decimal = 0D

        For Each tg In taxGroups.Values
            Dim basis As Decimal = Round2(tg.Basis)
            Dim pct As Decimal = tg.Percent
            Dim taxAmt As Decimal = Round2(basis * pct / 100D)

            ' Steuerbefreiung parsen - nullable
            Dim exCodeNullable As System.Nullable(Of TaxExemptionReasonCodes) = ParseExemptionReasonCodeNullable(tg.ReasonCode)

            ' Steueraufschlüsselung hinzufügen
            x.AddApplicableTradeTax(
                basisAmount:=basis,
                percent:=pct,
                taxAmount:=taxAmt,
                typeCode:=TaxTypes.VAT,
                categoryCode:=tg.Category,
                exemptionReasonCode:=exCodeNullable,
                exemptionReason:=tg.ReasonText
            )

            totalVAT += taxAmt
        Next

        Dim netRounded As Decimal = Round2(sumNetLines)
        Dim grand As Decimal = Round2(netRounded + totalVAT)

        x.SetTotals(
            lineTotalAmount:=netRounded,
            allowanceTotalAmount:=0D, ' Belegebene nicht genutzt
            taxBasisAmount:=netRounded,
            taxTotalAmount:=totalVAT,
            grandTotalAmount:=grand,
            totalPrepaidAmount:=0D,
            duePayableAmount:=grand,
            roundingAmount:=0D
        )
    End Sub

    ' Sicherer Integer-Getter mit Fallback
    Private Function D(obj As Object) As Decimal
        If obj Is Nothing OrElse IsDBNull(obj) Then Return 0D
        Dim s = Convert.ToString(obj, Globalization.CultureInfo.InvariantCulture)
        Dim v As Decimal
        If Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, v) Then
            Return v
        End If
        Return 0D
    End Function

    ' Sicherer String-Getter mit Fallback
    Private Function S(obj As Object, Optional fallback As String = "") As String
        If obj Is Nothing OrElse IsDBNull(obj) Then Return fallback
        Dim val = Convert.ToString(obj)
        If String.IsNullOrWhiteSpace(val) Then Return fallback
        Return val
    End Function

    ' Einfache SQL-Escaping-Funktion - verhindert SQL-Fehler bei Quotes
    Private Function SqlEsc(ByVal s As String) As String
        If s Is Nothing Then Return ""
        Return s.Replace("'", "''")
    End Function

    ' Dictionary-Helper: Verschiedene Dictionary-Typen in ein IDictionary(Of String, Object) umwandeln
    Private Function ToObjectDict(input As Object) As IDictionary(Of String, Object)
        If input Is Nothing Then
            Return New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
        End If

        Dim od = TryCast(input, IDictionary(Of String, Object))
        If od IsNot Nothing Then Return od

        Dim sd = TryCast(input, IDictionary(Of String, String))
        If sd IsNot Nothing Then
            Dim res As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
            For Each kv In sd
                res(kv.Key) = kv.Value
            Next
            Return res
        End If

        Dim nd = TryCast(input, IDictionary) ' non-generic fallback
        If nd IsNot Nothing Then
            Dim res As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
            For Each key In nd.Keys
                res(CStr(key)) = nd(key)
            Next
            Return res
        End If

        Throw New InvalidCastException("Unsupported dictionary type for ToObjectDict")
    End Function

    ' Sicherer String-Getter für Dictionary-Objekte mit Fallback
    Private Function GetSafe(d As Object, key As String) As String
        Dim dd = ToObjectDict(d)
        If Not dd.ContainsKey(key) OrElse dd(key) Is Nothing Then Return ""
        Return CStr(dd(key))
    End Function

    ' Sicherer Integer-Getter für Dictionary-Objekte mit Fallback
    Private Function SafeInt(d As Object, key As String) As Integer
        Dim s = GetSafe(d, key)
        Dim i As Integer
        If Integer.TryParse(s, i) Then Return i
        Return 0
    End Function

    ' Sicherer Decimal-Getter für Dictionary-Objekte mit Fallback
    Private Function SafeDec(d As Object, key As String) As Decimal
        Dim s = GetSafe(d, key)
        Dim v As Decimal
        If Decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, v) Then Return v
        If Decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, v) Then Return v
        Return 0D
    End Function

    ' Sicherer Date-Getter für Dictionary-Objekte mit Fallback
    Private Function SafeDate(d As Object, key As String) As Date
        Dim s = GetSafe(d, key)
        Dim dt As Date
        If Date.TryParse(s, dt) Then Return dt
        Return Date.Today
    End Function

    ' Runden auf 2 Dezimalstellen (MidpointRounding.AwayFromZero)
    Private Function Round2(v As Decimal) As Decimal
        Return Math.Round(v, 2, MidpointRounding.AwayFromZero)
    End Function

    ' Prozentformat aus "raw" parsen (z.B. "0.19", "19", "19.00", "0,19", "19,00", "0.19123", "0,19123", "0.19%", "19%")
    Private Function FormatPctFromRaw(raw As String) As String
        If String.IsNullOrWhiteSpace(raw) Then Return ""
        Dim v As Decimal
        If Not Decimal.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, v) Then
            If Not Decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, v) Then
                Return ""
            End If
        End If
        If v <= 1D Then v *= 100D
        Return v.ToString("0.##", CultureInfo.InvariantCulture)
    End Function

    ' QuantityCodes-Enum aus String parsen (inkl. Aliasse)
    Private Function ToQuantityCode(code As String) As QuantityCodes
        If String.IsNullOrWhiteSpace(code) Then Return QuantityCodes.C62

        ' Normalisieren (Leerzeichen/Trennzeichen raus, Großschreibung)
        Dim c As String = code.Trim().ToUpperInvariant() _
                          .Replace(" ", "") _
                          .Replace("-", "") _
                          .Replace(".", "") _
                          .Replace("/", "")

        ' Häufige Aliasse/Schreibweisen
        Select Case c
            Case "L", "LT", "LTR", "LITER" : Return QuantityCodes.LTR
            Case "H", "HR", "HUR" : Return QuantityCodes.HUR
            Case "KG", "KGM" : Return QuantityCodes.KGM
            Case "KWH", "KW_H" : Return QuantityCodes.KWH
            Case "PC", "PCS", "PCE", "STK", "ST", "STUECK", "STÜCK"
                Return QuantityCodes.C62
        End Select

        ' Nur Namen zulassen (nicht numerische Strings):
        ' Ist der (normalisierte) Name in der Enum definiert?
        If [Enum].IsDefined(GetType(QuantityCodes), c) Then
            ' Sicheres Parse (ohne Try/Catch und ohne numerische Interpretation)
            Return DirectCast([Enum].Parse(GetType(QuantityCodes), c, ignoreCase:=True), QuantityCodes)
        End If

        ' Fallback, wenn unbekannt
        Return QuantityCodes.C62
    End Function

    ' Stream in Byte-Array konvertieren
    Private Function StreamToBytes(s As Stream) As Byte()
        If TypeOf s Is MemoryStream Then
            Return DirectCast(s, MemoryStream).ToArray()
        End If
        Dim ms As New MemoryStream()
        If s.CanSeek Then s.Position = 0
        s.CopyTo(ms)
        Return ms.ToArray()
    End Function

    ' Steuerschlüssel-Code parsen (Enum), Default S
    Private Function ParseTaxCategoryCode(code As String) As TaxCategoryCodes
        Try
            Return CType([Enum].Parse(GetType(TaxCategoryCodes), code, True), TaxCategoryCodes)
        Catch
            Return TaxCategoryCodes.S
        End Try
    End Function

    ' Steuerbefreiungsgrund-Code parsen (Enum), Nullable
    Private Function ParseExemptionReasonCodeNullable(code As String) As System.Nullable(Of TaxExemptionReasonCodes)
        If String.IsNullOrWhiteSpace(code) Then Return Nothing

        Dim c As String = code.Trim()

        ' Nur wenn der String exakt einem Enum-Namen entspricht:
        If [Enum].IsDefined(GetType(TaxExemptionReasonCodes), c) Then
            Return CType([Enum].Parse(GetType(TaxExemptionReasonCodes), c, True), TaxExemptionReasonCodes)
        End If

        Return Nothing
    End Function

    ' --- Validierung der Käuferdaten (LandISO muss gesetzt sein) ---
    Public Function ValidateBuyerData(buyerData As Dictionary(Of String, String)) As Boolean

        ValidateBuyerData = True
        ' LandISO ist Pflichtfeld
        If buyerData("LandISO") = "" Then
            MessageBox.Show("Kennzeichen Land ISO fehlt bei Kunde: " & buyerData("Matchcode"), "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
            _logger.Debug($"Empty value for LandISO - Matchcode: " & buyerData("Matchcode"))
            ValidateBuyerData = False
        End If
        ' E-Mail für Rechnung ist Pflichtfeld
        If buyerData("EmailRechnung") = "" Then
            MessageBox.Show("EMail für Rechnung fehlt bei Kunde: " & buyerData("Matchcode"), "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
            _logger.Debug($"Empty value for EmailRechnung - Matchcode: " & buyerData("Matchcode"))
            ValidateBuyerData = False
        End If

    End Function

    ' --- Validierung der XML-Datei mittels externer Java-Anwendung ---
    Public Function Validate(file As String, Optional silent As Boolean = False) As Boolean
        _logger.Debug($"Entering {NameOf(Validate)}({file}, silent={silent})")
        Dim validationSuccess As Boolean = False
        Try
            Dim currentFolder = Path.GetDirectoryName(Me.GetType().Assembly.Location)
            Dim validatorFolder = Path.Combine(currentFolder, "validator")
            Dim reportFolder = Path.Combine(currentFolder, "logs", "html")
            Directory.CreateDirectory(reportFolder)
            CleanValidationReports(reportFolder)

            Dim configurationFolder = Path.Combine(validatorFolder, "configuration")
            Dim validatorFileName = Path.Combine(validatorFolder, "validator-1.5.2-standalone.jar")
            Dim configFile = Path.Combine(configurationFolder, "EN16931-UBL-validation.xslt")
            Dim scenarioFile = Path.Combine(configurationFolder, "scenarios.xml")

            If Not IO.File.Exists(validatorFileName) OrElse Not Directory.Exists(configurationFolder) OrElse Not IO.File.Exists(configFile) OrElse Not IO.File.Exists(scenarioFile) Then
                _logger.Error($"Validator or configuration not found.")
                If Not silent Then
                    MessageBox.Show("Validator nicht installiert!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
                Return False
            End If

            Dim javaFolder = Environment.ExpandEnvironmentVariables("JRE_HOME")
            If String.IsNullOrWhiteSpace(javaFolder) Then
                _logger.Error("Java JRE not installed")
                If Not silent Then
                    MessageBox.Show("Jave JRE nicht installiert!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
                Return False
            End If

            Dim arguments = $"-jar ""{validatorFileName}"" -s ""{scenarioFile}"" -r ""{configurationFolder}"" -d ""{file}"" -h"
            Dim pInfo = New ProcessStartInfo()
            pInfo.UseShellExecute = False
            pInfo.FileName = "java"
            pInfo.Arguments = arguments
            pInfo.WorkingDirectory = Path.Combine(currentFolder, "logs", "html")
            pInfo.CreateNoWindow = True

            ' Redirect output and error streams
            pInfo.RedirectStandardOutput = True
            pInfo.RedirectStandardError = True

            Dim p = New Process()
            p.StartInfo = pInfo

            Dim stdOutputBuilder As New System.Text.StringBuilder()
            Dim stdErrorBuilder As New System.Text.StringBuilder()
            Dim outputWaitHandle As New Threading.AutoResetEvent(False)
            Dim errorWaitHandle As New Threading.AutoResetEvent(False)

            AddHandler p.OutputDataReceived, Sub(sender, e)
                                                 If e.Data Is Nothing Then
                                                     outputWaitHandle.Set()
                                                 Else
                                                     stdOutputBuilder.AppendLine(e.Data)
                                                 End If
                                             End Sub
            AddHandler p.ErrorDataReceived, Sub(sender, e)
                                                If e.Data Is Nothing Then
                                                    errorWaitHandle.Set()
                                                Else
                                                    stdErrorBuilder.AppendLine(e.Data)
                                                End If
                                            End Sub

            Cursor.Current = Cursors.WaitCursor
            p.Start()
            p.BeginOutputReadLine()
            p.BeginErrorReadLine()
            p.WaitForExit()
            Cursor.Current = Cursors.Default

            ' Warten bis alle Ausgaben gelesen wurden (max. 5 Sekunden je Stream)
            outputWaitHandle.WaitOne(5000)
            errorWaitHandle.WaitOne(5000)

            Dim stdOutput As String = stdOutputBuilder.ToString()
            Dim stdError As String = stdErrorBuilder.ToString()

            ' Log process output
            If Not String.IsNullOrWhiteSpace(stdOutput) Then
                _logger.Info($"Validator Output: {stdOutput}")
            End If
            If Not String.IsNullOrWhiteSpace(stdError) Then
                _logger.Error($"Validator Error: {stdError}")
            End If

            If p.ExitCode <> 0 Then
                validationSuccess = False
                If Not silent Then
                    Dim result = MessageBox.Show("Validierung ist fehlgeschlagen.", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)

                    Dim reporFile = Directory.EnumerateFiles(reportFolder, "*.html").FirstOrDefault()
                    If reporFile Is Nothing Then
                        MessageBox.Show("Es wurde keine Reportdatei gefunden!" & vbCrLf &
                                        "Bitte überprüfen Sie das Logfile für weitere Ausgaben.", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return False
                    End If

                    Process.Start("explorer.exe", reporFile)
                End If
            Else
                validationSuccess = True
                If Not silent Then
                    MessageBox.Show("Validierung erfolgreich!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        Catch ex As Exception
            _logger.Error($"Exception during {NameOf(Validate)}", ex)
            validationSuccess = False
        Finally
            _logger.Debug($"Leaving {NameOf(Validate)}")
        End Try
        Return validationSuccess
    End Function

    ' --- Exportpfad für Belegart holen ---
    Public Function GetExportPath(billType As RechnungsArt) As String
        Dim path = String.Empty
        If Not _exportPaths.TryGetValue(billType, path) OrElse String.IsNullOrWhiteSpace(path) Then
            Throw New Exception($"Exportpfad nicht definiert!")
        End If

        Return IO.Path.Combine(path, billType.ToString())
    End Function

    ' --- Vollständigen Export-Dateipfad holen (inkl. Dateiname) ---
    Public Function GetExportFilePath(billType As RechnungsArt, rechnungsNummer As Integer, extension As String) As String
        Dim exportPath = GetExportPath(billType)
        Dim formattedBillNumber = GetFormattedBillNumber(rechnungsNummer, billType)
        Dim fileName = $"{formattedBillNumber}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.{extension}"
        Dim filePath = Path.Combine(exportPath, fileName)
        Directory.CreateDirectory(exportPath)
        Return filePath
    End Function

    ' --- Formatierten Belegnummer-String holen (z.B. "WA-000123") ---
    Public Function GetFormattedBillNumber(billNumber As Integer, billType As RechnungsArt)
        Return Sale.Invoicing.FormatInvoiceNumber(_dataConnection, GetBillTypeText(billType), billNumber)
    End Function

    ' --- Belegart-Text für Belegnummern-Präfix holen (z.B. "WA" für Werkstatt) ---
    Private Function GetBillTypeText(billType As RechnungsArt) As String
        Dim billTypeText = String.Empty
        Dim referenceColumn = ""
        Select Case billType
            Case RechnungsArt.Werkstatt
                billTypeText = "WA"
            Case RechnungsArt.Tanken
                billTypeText = "TA"
            Case RechnungsArt.Manuell
                billTypeText = "MR"
        End Select

        Return billTypeText
    End Function

    ' --- Wert aus Dictionary holen mit Fallback + Warnung im Log ---
    Private Function GetDataFromColumn(data As Dictionary(Of String, String), column As String, Optional defaultValue As String = "") As String
        Dim value = defaultValue
        If Not data.TryGetValue(column, value) AndAlso Not String.IsNullOrWhiteSpace(defaultValue) Then
            _logger.Warn($"Column {column} does not exist, using default value {defaultValue}")
        End If

        If String.IsNullOrWhiteSpace(value) Then Return defaultValue
        Return value
    End Function

    ' --- Alte HTML-Reports löschen ---
    Private Sub CleanValidationReports(reportFolder As String)
        Dim files = Directory.EnumerateFiles(reportFolder, "*.html").ToList
        files.ForEach(Sub(f) File.Delete(f))
    End Sub

    ' --- Mengeneinheit-Code + Umrechnungsfaktor aus DB holen & Menge umrechnen ---
    Public Function GetMeasurementCode(type As String,
                                   quantity As Decimal,
                                   unitPrice As Decimal,
                                   Optional roundQtyDecimals As Integer = 2,
                                   Optional unitPriceDecimals As Integer = 2) As MeasurementCode
        ' Fallbacks
        Dim unitCodeStr As String = "C62"  ' DB liefert Strings, wir mappen gleich ins Enum
        Dim factor As Decimal = 1D

        Try
            Dim sql As String =
            "SELECT TOP 1 ERechnung_UnitCode, ERechnung_UnitConversion " &
            "FROM dbo.Mengeneinheit " &
            "WHERE MengeneinheitK = '" & SqlEsc(type) & "'"

            Dim dt As DataTable = _dataConnection.FillDataTable(sql)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim r As DataRow = dt.Rows(0)
                unitCodeStr = S(r("ERechnung_UnitCode"), "C62")
                factor = D(r("ERechnung_UnitConversion"))
                If factor = 0D Then factor = 1D
            End If
        Catch ex As Exception
            _logger.Warn($"GetMeasurementCode: Fallback für type={type}: {ex.Message}")
            unitCodeStr = "C62" : factor = 1D
        End Try

        ' Menge umrechnen
        Dim qtyConv As Decimal = quantity * factor
        If roundQtyDecimals = 2 Then
            qtyConv = Round2(qtyConv)
        Else
            qtyConv = Math.Round(qtyConv, roundQtyDecimals, MidpointRounding.AwayFromZero)
        End If

        ' Einzelpreis gegenläufig umrechnen (Preis je neuer Einheit)
        Dim priceConv As Decimal = If(factor = 0D, unitPrice, unitPrice / factor)
        If unitPriceDecimals = 2 Then
            priceConv = Round2(priceConv)
        Else
            priceConv = Math.Round(priceConv, unitPriceDecimals, MidpointRounding.AwayFromZero)
        End If

        Dim unitEnum As QuantityCodes = ToQuantityCode(unitCodeStr)
        Return New MeasurementCode(unitEnum, qtyConv, priceConv, factor, unitPrice)
    End Function

    ' --- Elemente aus SQL-Query holen ---
    Private Function GetItemsFromQuery(sql As String) As List(Of Dictionary(Of String, String))
        Dim table = _dataConnection.FillDataTable(sql)
        If table.Rows.Count = 0 Then Return New List(Of Dictionary(Of String, String))
        Return table.Rows.OfType(Of Data.DataRow).Select(
            Function(row) table.Columns.OfType(Of Data.DataColumn).ToDictionary(Function(column) column.ColumnName.Replace(" ", ""), Function(column) row(column.ColumnName).ToString())).ToList
    End Function

    ' --- Verkäuferdaten holen ---
    Private Function GetSellerParameter(rechnungsArt As RechnungsArt) As Dictionary(Of String, String)
        Dim sql = GetSellerParameterSql(rechnungsArt)
        Dim dataTable = _dataConnection.FillDataTable(sql)
        If dataTable.Rows.Count = 0 Then
            _logger.Error($"Could not fetch seller parameters from DB ({sql})")
            Return New Dictionary(Of String, String)
        End If
        Return dataTable.Columns.OfType(Of Data.DataColumn).ToDictionary(Function(column) column.ColumnName, Function(column) dataTable.Rows.OfType(Of Data.DataRow)().First()(column.ColumnName).ToString)
    End Function

    ' --- Zusätzliche Verkäuferdaten holen ---
    Private Function GetAdditionalSellerParameter(rechnungsArt As RechnungsArt) As Dictionary(Of String, String)
        Dim sql = GetAdditionalSellerParameterSql(rechnungsArt)
        Dim dataTable = _dataConnection.FillDataTable(sql)
        If dataTable.Rows.Count = 0 Then
            _logger.Error($"Could not fetch additional seller parameters from DB ({sql})")
            Return New Dictionary(Of String, String)
        End If
        Return dataTable.Columns.OfType(Of Data.DataColumn).ToDictionary(Function(column) column.ColumnName, Function(column) dataTable.Rows.OfType(Of Data.DataRow)().First()(column.ColumnName).ToString)
    End Function

    ' -- Zusätzliche Verkäuferdaten aus Parameter holen ---
    Private Function GetAdditionalSellerParameterSql(rechnungsArt As RechnungsArt) As String
        Dim parameterNumber = ""
        Select Case rechnungsArt
            Case RechnungsArt.Werkstatt
                parameterNumber = "211"
            Case RechnungsArt.Tanken
                parameterNumber = "1211"
            Case RechnungsArt.Manuell
                parameterNumber = "2211"
        End Select

        Return $"select * from Parameter where ParameterNr = {parameterNumber}"
    End Function

    ' -- Zahlungsreferenz holen ---
    Private Function GetSellerParameterSql(rechnungsArt As RechnungsArt) As String
        Dim parameterNumber = ""
        Select Case rechnungsArt
            Case RechnungsArt.Werkstatt
                parameterNumber = "1"
            Case RechnungsArt.Tanken
                parameterNumber = "11"
            Case RechnungsArt.Manuell
                parameterNumber = "21"
        End Select

        Return $"select * from Parameter where ParameterNr = {parameterNumber}"
    End Function

    ' --- Käuferdaten holen ---
    Private Function GetBuyerData(kundenNr As String) As Dictionary(Of String, String)
        Dim sql = $"select * from Kunden where KundenNr = {kundenNr}"
        Dim dataTable = _dataConnection.FillDataTable(sql)
        If dataTable.Rows.Count = 0 Then
            _logger.Error($"Could not fetch customer parameters from DB ({sql})")
            Return New Dictionary(Of String, String)
        End If
        Return dataTable.Columns.OfType(Of Data.DataColumn).ToDictionary(Function(column) column.ColumnName, Function(column) dataTable.Rows.OfType(Of Data.DataRow)().First()(column.ColumnName).ToString)
    End Function

    ' --- SQL-Statements für Rechnungskopf, Details, Steuerdaten ---
    Private Function GetSqlStatementForVat(rechnungsArt As RechnungsArt, rechnungsNummern As List(Of Integer)) As String
        Dim inClausePlaceholders As String = String.Join(",", rechnungsNummern.Select(Function(v) $"{v}").ToArray())
        Select Case rechnungsArt
            Case RechnungsArt.Werkstatt
                Return $"SELECT * FROM abfr_wavkabrmwst WHERE RechnungsNr IN ({inClausePlaceholders})"
            Case RechnungsArt.Tanken
                Return $"SELECT * FROM abfr_tankabrmwst WHERE RechnungsNr IN ({inClausePlaceholders})"
            Case RechnungsArt.Manuell
                Return $"SELECT * FROM abfr_mrvkabrmwst WHERE RechnungsNr IN ({inClausePlaceholders})"
        End Select

        Return ""
    End Function

    ' --- SQL-Statement für Rechnungskopf holen ---
    Private Function GetSqlStatementForBill(rechnungsArt As RechnungsArt, rechnungsNummern As List(Of Integer)) As String
        Dim inClausePlaceholders As String = String.Join(",", rechnungsNummern.Select(Function(v) $"{v}").ToArray())
        Select Case rechnungsArt
            Case RechnungsArt.Werkstatt
                Return $"select * from abfr_wavkreport where RechnungsNr IN ({inClausePlaceholders})"
            Case RechnungsArt.Tanken
                Return $"select * from abfr_tankreport WHERE RechnungsNr IN ({inClausePlaceholders})"
            Case RechnungsArt.Manuell
                Return $"select * from abfr_mrvkreport WHERE RechnungsNr IN ({inClausePlaceholders})"
        End Select

        Return ""
    End Function

    ' --- SQL-Statements für Rechnungsdetails holen ---
    Private Function GetSqlStatements(rechnungsArt As RechnungsArt, rechnungsNummern As List(Of Integer)) As List(Of String)
        Dim inClausePlaceholders As String = String.Join(",", rechnungsNummern.Select(Function(v) $"{v}").ToArray())
        Select Case rechnungsArt
            Case RechnungsArt.Werkstatt
                Return New List(Of String) From
                    {
                        $"select * from abfr_wavkabrdetail WHERE RechnungsNr IN ({inClausePlaceholders}) AND ArtikelNr is not Null AND PersonalID is Null ORDER BY RechnungsDetailNr",
                        $"select * from abfr_wavkabrdetail WHERE RechnungsNr IN ({inClausePlaceholders}) AND PersonalID Is Not Null And ArtikelNr Is Null ORDER By RechnungsDetailNr",
                        $"select * from abfr_wavkabrdetail WHERE RechnungsNr IN ({inClausePlaceholders}) AND PersonalID is Null AND ArtikelNr is Null ORDER BY RechnungsDetailNr"
                    }
            Case RechnungsArt.Tanken
                Return New List(Of String) From
                    {
                        $"SELECT * FROM abfr_tankabrdetail WHERE RechnungsNr IN ({inClausePlaceholders}) ORDER BY RechnungsDetailNr",
                        $"SELECT * FROM TankabrechnungDetail WHERE RechnungsNr IN ({inClausePlaceholders}) AND SpritID is Null AND ArtikelNr is Null ORDER BY RechnungsDetailNr"
                    }
            Case RechnungsArt.Manuell
                Return New List(Of String) From
                    {
                        $"select * from abfr_mrvkabrdetail WHERE RechnungsNr IN ({inClausePlaceholders}) AND ArtikelNr is not Null AND PersonalID is Null ORDER BY RechnungsDetailNr",
                        $"select * from abfr_mrvkabrdetail WHERE RechnungsNr IN ({inClausePlaceholders}) AND PersonalID is not Null AND ArtikelNr is Null ORDER BY RechnungsDetailNr",
                        $"SELECT * FROM abfr_mrvkabrdetail WHERE RechnungsNr IN ({inClausePlaceholders}) AND PersonalID is Null AND ArtikelNr is Null ORDER BY RechnungsDetailNr"
                    }
        End Select

        Return New List(Of String)
    End Function

    ''' <summary>
    ''' Prüft, ob das Feld Status in der Tabelle RechnungKopf für die angegebene Rechnungsnummer den Wert "Issued" hat.
    ''' </summary>
    Public Function IsRechnungIssued(rechnungsNummer As Integer) As Boolean
        Try
            Dim sql = $"SELECT COUNT(*) FROM RechnungKopf WHERE RechnungsNr = {rechnungsNummer} AND Status = 'Issued'"
            Dim dt = _dataConnection.FillDataTable(sql)
            If dt.Rows.Count > 0 AndAlso Convert.ToInt32(dt.Rows(0).Item(0)) > 0 Then
                Return True
            End If
        Catch ex As Exception
            _logger.Error($"Fehler bei Status-Prüfung für Rechnung {rechnungsNummer}: {ex.Message}")
        End Try
        Return False
    End Function

    ''' <summary>
    ''' Gibt das Feld PDF_Raw aus der Tabelle RechnungBlob für die angegebene Rechnungsnummer als Byte-Array zurück.
    ''' </summary>
    Public Function GetPdfRawFromBlob(rechnungsNummer As Integer) As Byte()
        Try
            Dim sql = $"SELECT PDF_Raw FROM RechnungBlob WHERE RechnungsNr = {rechnungsNummer}"
            Dim dt = _dataConnection.FillDataTable(sql)
            If dt.Rows.Count > 0 AndAlso Not IsDBNull(dt.Rows(0)("PDF_Raw")) Then
                ' PDF_Raw ist VARBINARY(MAX), daher als Byte-Array zurückgeben
                Return CType(dt.Rows(0)("PDF_Raw"), Byte())
            End If
        Catch ex As Exception
            _logger.Error($"Fehler beim Auslesen von PDF_Raw für Rechnung {rechnungsNummer}: {ex.Message}")
        End Try
        Return Nothing
    End Function

    ''' <summary>
    ''' Gibt das Feld XML_Raw aus der Tabelle RechnungBlob für die angegebene Rechnungsnummer als Byte-Array zurück.
    ''' </summary>
    Public Function GetXmlRawFromBlob(rechnungsNummer As Integer) As Byte()
        Try
            Dim sql = $"SELECT XML_Raw FROM RechnungBlob WHERE RechnungsNr = {rechnungsNummer}"
            Dim dt = _dataConnection.FillDataTable(sql)
            If dt.Rows.Count > 0 AndAlso Not IsDBNull(dt.Rows(0)("XML_Raw")) Then
                ' XML_Raw ist VARBINARY(MAX), daher als Byte-Array zurückgeben
                Return CType(dt.Rows(0)("XML_Raw"), Byte())
            End If
        Catch ex As Exception
            _logger.Error($"Fehler beim Auslesen von XML_Raw für Rechnung {rechnungsNummer}: {ex.Message}")
        End Try
        Return Nothing
    End Function

    ''' <summary>
    ''' Speichert den XML-Inhalt in RechnungBlob.XML_Raw, erstellt eine Signatur in RechnungSignature,
    ''' und aktualisiert RechnungKopf (Status, ERechnung-Felder).
    ''' </summary>
    ''' <param name="xmlBytes">XML-Inhalt als Byte-Array</param>
    ''' <param name="rechnungsNummer">Rechnungsnummer</param>
    ''' <param name="format">ERechnung Format (z.B. 'UBL')</param>
    ''' <param name="profil">ERechnung Profil (z.B. 'XRechnung')</param>
    Public Sub StoreXmlAndUpdateStatus(xmlBytes As Byte(), rechnungsNummer As Integer, format As String, profil As String)
        Try
            ' 1. GUID erzeugen
            Dim blobId As Guid = Guid.NewGuid()
            Dim createdAt As DateTime = DateTime.Now
            Dim createdBy As String = Environment.UserName

            ' 2. XML in RechnungBlob speichern (INSERT, nicht UPDATE)
            ' XML_SHA256 - Blob-Variante = technischer Fingerabdruck des gespeicherten XML (Integritäts- und Dublettenprüfung).
            Dim sqlBlob = "INSERT INTO RechnungBlob " &
                "(BlobId, RechnungsNr, Format, EN16931Profil, XML_Raw, CreatedAt, CreatedBy) " &
                "VALUES (?, ?, ?, ?, ?, ?, ?)"
            Dim cmdBlob As New OleDb.OleDbCommand(sqlBlob, _dataConnection.cn)
            cmdBlob.Parameters.AddWithValue("@BlobId", blobId.ToString())
            cmdBlob.Parameters.AddWithValue("@RechnungsNr", rechnungsNummer)
            cmdBlob.Parameters.AddWithValue("@Format", format)
            cmdBlob.Parameters.AddWithValue("@EN16931Profil", profil)
            cmdBlob.Parameters.AddWithValue("@XML_Raw", xmlBytes)
            cmdBlob.Parameters.AddWithValue("@CreatedAt", createdAt)
            cmdBlob.Parameters.AddWithValue("@CreatedBy", createdBy)
            cmdBlob.ExecuteNonQuery()
            _logger.Info($"E-Rechnung XML für Rechnung {rechnungsNummer} in RechnungBlob gespeichert (BlobId: {blobId})")

            ' 3. Signatur generieren und speichern (SHA256-Hash)
            ' XML_SHA256 - Signature-Variante = rechtlich/kryptographischer Fingerabdruck (Beweis, dass die Signatur sich genau auf dieses XML bezieht).
            Dim xmlSha256 As String
            Using sha = System.Security.Cryptography.SHA256.Create()
                xmlSha256 = BitConverter.ToString(sha.ComputeHash(xmlBytes)).Replace("-", "").ToLowerInvariant()
            End Using

            Dim signatureAlgo As String = "SHA256"
            Dim signatureBytes As Byte() = New Byte() {} ' Falls keine echte Signatur vorhanden, leer
            Dim certThumbprint As String = "" ' Falls kein Zertifikat, leer
            Dim signedAt As DateTime = DateTime.Now
            Dim signedBy As String = Environment.UserName ' oder leer, falls nicht verfügbar

            Dim sqlSignature = "INSERT INTO RechnungSignature " &
                "(RechnungNr, BlobId, XML_SHA256, SignatureAlgo, SignatureBytes, CertThumbprint, SignedAt, SignedBy) " &
                "VALUES (?, ?, ?, ?, ?, ?, ?, ?)"
            Dim cmdSignature As New OleDb.OleDbCommand(sqlSignature, _dataConnection.cn)
            cmdSignature.Parameters.AddWithValue("@RechnungNr", rechnungsNummer)
            cmdSignature.Parameters.AddWithValue("@BlobId", blobId.ToString())
            cmdSignature.Parameters.AddWithValue("@XML_SHA256", xmlSha256)
            cmdSignature.Parameters.AddWithValue("@SignatureAlgo", signatureAlgo)
            cmdSignature.Parameters.AddWithValue("@SignatureBytes", signatureBytes)
            cmdSignature.Parameters.AddWithValue("@CertThumbprint", certThumbprint)
            cmdSignature.Parameters.AddWithValue("@SignedAt", signedAt)
            cmdSignature.Parameters.AddWithValue("@SignedBy", signedBy)
            cmdSignature.ExecuteNonQuery()

            ' 4. RechnungKopf aktualisieren (alle ERechnung-Felder)
            ' XML parsen: als UTF-8 String speichern, damit TRY_CAST(@XML_Text AS XML) in SQL funktioniert
            Dim xmlParsedString As String = System.Text.Encoding.UTF8.GetString(xmlBytes)

            Dim sqlKopf = "UPDATE RechnungKopf SET " &
                          "Status = 'Issued', " &
                          "Locked = ?, " &
                          "IssueTimestamp = ?, " &
                          "ERechnung_Format = ?, " &
                          "ERechnung_EN16931Profil = ?, " &
                          "ERechnung_XMLParsed = ?, " &
                          "ERechnung_Valid = ?, " &
                          "ERechnung_ValReport = ?, " &
                          "ERechnung_ReceivedAt = ?, " &
                          "ERechnung_ReportedAt = ? " &
                          "WHERE RechnungsNr = ?"
            Dim cmdKopf As New OleDb.OleDbCommand(sqlKopf, _dataConnection.cn)
            cmdKopf.Parameters.AddWithValue("@Locked", True)
            cmdKopf.Parameters.AddWithValue("@IssueTimestamp", createdAt)
            cmdKopf.Parameters.AddWithValue("@ERechnung_Format", format)
            cmdKopf.Parameters.AddWithValue("@ERechnung_EN16931Profil", profil)
            cmdKopf.Parameters.AddWithValue("@ERechnung_XMLParsed", xmlParsedString) ' Geparste XML-Daten als String speichern
            cmdKopf.Parameters.AddWithValue("@ERechnung_Valid", DBNull.Value)
            cmdKopf.Parameters.AddWithValue("@ERechnung_ValReport", DBNull.Value)
            cmdKopf.Parameters.AddWithValue("@ERechnung_ReceivedAt", createdAt)
            cmdKopf.Parameters.AddWithValue("@ERechnung_ReportedAt", DBNull.Value)
            cmdKopf.Parameters.AddWithValue("@nr", rechnungsNummer)
            cmdKopf.ExecuteNonQuery()
        Catch ex As Exception
            _logger.Error($"Fehler beim Speichern der XML und Aktualisieren des Status für Rechnung {rechnungsNummer}: {ex.Message}")
            MessageBox.Show("Fehler beim Speichern der E-Rechnung!", "Fleet Fuhrpark IM System", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


End Class
