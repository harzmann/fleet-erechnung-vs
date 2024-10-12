Imports System.Globalization
Imports System.IO
Imports System.Threading
Imports ehfleet_classlibrary
Imports s2industries.ZUGFeRD

Public Class XRechnungExporter
    Private _dataConnection As General.Database

    Public Sub New(dataConnection As General.Database)
        _dataConnection = dataConnection
    End Sub

    Public Sub CreateBillXml(xmlStream As Stream, billType As RechnungsArt, rechnungsNummer As Integer)
        'Dim serializer = New XmlSerializer(GetType(Invoice))
        Dim sqls = GetSqlStatements(billType, New List(Of Integer) From {rechnungsNummer})
        Dim items = GetItemsFromQuery(GetSqlStatementForBill(billType, New List(Of Integer) From {rechnungsNummer})).FirstOrDefault
        If items Is Nothing Then Return

        Dim vatData = GetItemsFromQuery(GetSqlStatementForVat(billType, New List(Of Integer) From {rechnungsNummer}))
        Dim sellerData = GetSellerParameter(billType)
        Dim additionalSellerData = GetAdditionalSellerParameter(billType)
        Dim buyerData = GetBuyerData(items("KundenNr"))
        Dim xRechnung = New InvoiceDescriptor()
        xRechnung.InvoiceNo = rechnungsNummer.ToString()
        xRechnung.Currency = CType([Enum].Parse(GetType(CurrencyCodes), sellerData("Währung")), CurrencyCodes)
        Dim countryCode = CType([Enum].Parse(GetType(CountryCodes), buyerData("LandISO")), CountryCodes)
        xRechnung.SetBuyer(items("Firma"), items("Postleitzahl"), items("Ort"), items("Rechnungsadresse"), countryCode, items("KundenNr"))
        xRechnung.AddBuyerTaxRegistration(items("UStIdNr"), TaxRegistrationSchemeID.VA)
        xRechnung.Buyer.CountrySubdivisionName = items("FirmaOderAbteilung")
        xRechnung.Buyer.ID.ID = items("KundenNr")
        xRechnung.Buyer.ID.SchemeID = GlobalIDSchemeIdentifiers.CompanyNumber
        Dim leitwegId = buyerData("LeitwegeID")
        If String.IsNullOrWhiteSpace(leitwegId) Then
            xRechnung.SetBuyerElectronicAddress(buyerData("EmailRechnung"), ElectronicAddressSchemeIdentifiers.EM)
        Else
            xRechnung.SetBuyerElectronicAddress(buyerData("LeitwegeID"), ElectronicAddressSchemeIdentifiers.LeitwegID)
        End If

        xRechnung.SetBuyerContact(items("Kontakt"), items("Firma"), items("EmailAdresse"), items("Telefonnummer"))
        Dim referenceColumn = ""
        Select Case billType
            Case RechnungsArt.Werkstatt
                referenceColumn = "WABez"
            Case RechnungsArt.Tanken
                referenceColumn = "Anmerkungen"
            Case RechnungsArt.Manuell
                referenceColumn = "Anmerkungen"
        End Select

        xRechnung.PaymentReference = items(referenceColumn)
        xRechnung.InvoiceDate = items("Rechnungsdatum")

        Dim dueDate = ""
        Dim daysToDueDate = 0
        Try
            Dim billDate = DateTime.Parse(items("Rechnungsdatum"))
            daysToDueDate = Integer.Parse(items("NettoTage"))
            dueDate = billDate.AddDays(daysToDueDate)
        Catch ex As Exception
            dueDate = items("Rechnungsdatum")
        End Try

        Dim skontoDays = 0
        Decimal.TryParse(items("SkontoTage"), skontoDays)
        Dim skontoRate = 0
        Decimal.TryParse(items("SkontoProzent"), skontoRate)

        If Not String.IsNullOrWhiteSpace(skontoDays) AndAlso Not String.IsNullOrWhiteSpace(skontoRate) Then
            xRechnung.AddTradePaymentTerms($"#SKONTO#TAGE={skontoDays}#PROZENT={skontoRate:F2}#", dueDate)
        End If

        'xRechnung.AddTradePaymentTerms($"Fällig {daysToDueDate} Tage nach Rechnungsstellung", dueDate)
        xRechnung.SetPaymentMeans(PaymentMeansTypeCodes.CreditTransfer)
        xRechnung.AddCreditorFinancialAccount(additionalSellerData("Modul2"), additionalSellerData("Modul3"), Nothing, Nothing, additionalSellerData("Modul4"), additionalSellerData("Modul4"))

        xRechnung.SetSeller(sellerData("Firma"), sellerData("PLZ"), sellerData("Ort"), sellerData("Straße"), CountryCodes.DE, buyerData("LieferantenNr"))
        xRechnung.Seller.ID.ID = buyerData("LieferantenNr")
        xRechnung.Seller.ID.SchemeID = GlobalIDSchemeIdentifiers.CompanyNumber
        xRechnung.AddSellerTaxRegistration(sellerData("Steuernummer"), TaxRegistrationSchemeID.FC)
        xRechnung.SetSellerContact(sellerData("Name"), sellerData("Firma"), additionalSellerData("Modul1"), sellerData("Telefon"), sellerData("Telefax"))
        xRechnung.SetSellerElectronicAddress(additionalSellerData("Modul1"), ElectronicAddressSchemeIdentifiers.EM)

        Dim poReference As String = String.Empty
        items.TryGetValue("WAID", poReference)
        If String.IsNullOrWhiteSpace(poReference) Then items.TryGetValue("KostenstelleKunde", poReference)
        xRechnung.ReferenceOrderNo = poReference
        xRechnung.OrderNo = poReference
        xRechnung.ActualDeliveryDate = items("Lieferdatum")

        'add line items
        For Each query In sqls
            Dim rows = GetItemsFromQuery(query)
            If Not rows.Any() Then Continue For

            For Each lineItemData In rows
                If Not lineItemData.Any() Then Continue For

                If billType <> RechnungsArt.Tanken Then
                    Dim itemName = lineItemData("Artikelbez")
                    If String.IsNullOrWhiteSpace(itemName) Then itemName = lineItemData("FahrerName")
                    If String.IsNullOrWhiteSpace(itemName) Then itemName = lineItemData("Bemerkung")
                    Dim lineItem = xRechnung.AddTradeLineItem(itemName)
                    lineItem.SellerAssignedID = lineItemData("ArtikelNr")
                    lineItem.BilledQuantity = Decimal.Parse(lineItemData("Menge"))
                    lineItem.UnitQuantity = 1
                    Dim unit = lineItemData("ArtikelMEH")
                    If String.IsNullOrWhiteSpace(unit) Then
                        unit = lineItemData("PersonalMEH")
                    End If

                    lineItem.UnitCode = GetMeasurementCode(unit)

                    lineItem.LineTotalAmount = Decimal.Parse(lineItemData("Gesamtpreis"))

                    If Not String.IsNullOrWhiteSpace(lineItemData("Datum")) Then
                        lineItem.BillingPeriodStart = DateTime.Parse(lineItemData("Datum"))
                        lineItem.BillingPeriodEnd = DateTime.Parse(lineItemData("Datum"))
                    End If

                    Dim baseAmount = Decimal.Parse(lineItemData("EPreis"))
                    If Not String.IsNullOrWhiteSpace(lineItemData("Rabatt")) Then
                        Dim percentage = Decimal.Parse(lineItemData("Rabatt"))
                        Dim discount = lineItem.BilledQuantity * baseAmount * percentage
                        'discount = (lineItem.BilledQuantity * baseAmount - lineItem.LineTotalAmount) / lineItem.BilledQuantity

                        Dim actualAmount = baseAmount - (discount / lineItem.BilledQuantity)
                        lineItem.GrossUnitPrice = baseAmount
                        lineItem.NetUnitPrice = actualAmount
                        lineItem.AddTradeAllowanceCharge(True, xRechnung.Currency, Nothing, discount, "Discount")
                    Else
                        lineItem.GrossUnitPrice = baseAmount
                        lineItem.NetUnitPrice = baseAmount
                    End If

                    lineItem.TaxPercent = Decimal.Parse(lineItemData("MwStProzent"))
                    lineItem.TaxCategoryCode = TaxCategoryCodes.S
                    lineItem.TaxType = TaxTypes.VAT
                    lineItem.Description = lineItemData("ArtikelbezLang")
                    If String.IsNullOrWhiteSpace(lineItem.Description) Then lineItem.Description = lineItemData("FahrerName")
                    If String.IsNullOrWhiteSpace(lineItem.Description) Then lineItem.Description = lineItemData("Bemerkung")
                Else
                    Dim lineItem = xRechnung.AddTradeLineItem(lineItemData("Bezeichnung"))
                    lineItem.SellerAssignedID = lineItemData("SpritID")
                    lineItem.BilledQuantity = Decimal.Parse(lineItemData("Menge"))
                    lineItem.UnitQuantity = 1
                    lineItem.UnitCode = GetMeasurementCode(lineItemData("Mengeneinheit"))
                    lineItem.LineTotalAmount = Decimal.Parse(lineItemData("Gesamtpreis"))
                    lineItem.BillingPeriodStart = DateTime.Parse(lineItemData("Tankdatum"))
                    lineItem.BillingPeriodEnd = DateTime.Parse(lineItemData("Tankdatum"))
                    Dim baseAmount = Decimal.Parse(lineItemData("EPreis"))
                    If Not String.IsNullOrWhiteSpace(lineItemData("Rabatt")) Then
                        Dim percentage = Decimal.Parse(lineItemData("Rabatt"))
                        Dim discount = lineItem.BilledQuantity * baseAmount * percentage
                        'discount = (lineItem.BilledQuantity * baseAmount - lineItem.LineTotalAmount) / lineItem.BilledQuantity

                        Dim actualAmount = lineItem.LineTotalAmount / lineItem.BilledQuantity
                        lineItem.GrossUnitPrice = baseAmount
                        lineItem.NetUnitPrice = actualAmount
                        lineItem.AddTradeAllowanceCharge(True, xRechnung.Currency, Nothing, discount, "Discount")
                    Else
                        lineItem.GrossUnitPrice = baseAmount
                        lineItem.NetUnitPrice = baseAmount
                    End If
                    lineItem.TaxCategoryCode = TaxCategoryCodes.S
                    lineItem.TaxPercent = Decimal.Parse(lineItemData("MwStProzent"))
                    lineItem.TaxType = TaxTypes.VAT
                    lineItem.Description = lineItemData("Bezeichnung")
                End If
            Next
        Next

        'Dim discountSum = xRechnung.TradeLineItems.Sum(Function(item) item.GetTradeAllowanceCharges().Where(Function(x) Not x.ChargeIndicator).Sum(Function(x) x.ActualAmount))
        Dim lineNetSum = xRechnung.TradeLineItems.Sum(Function(item) item.LineTotalAmount)
        Dim vatAmount As Decimal
        Dim netSum As Decimal
        items.TryGetValue("Mehrwertsteuer", vatAmount)
        items.TryGetValue("Summe", netSum)

        xRechnung.DuePayableAmount = vatAmount + netSum
        xRechnung.TaxBasisAmount = netSum
        xRechnung.TaxTotalAmount = vatAmount
        xRechnung.LineTotalAmount = netSum
        xRechnung.GrandTotalAmount = netSum + vatAmount

        Dim taxPercentages = xRechnung.TradeLineItems.GroupBy(Function(item) item.TaxPercent)
        Dim taxEntries = taxPercentages.Select(Function(percentage)
                                                   Dim entry = New Tax()
                                                   entry.Percent = percentage.Key
                                                   entry.CategoryCode = TaxCategoryCodes.S
                                                   entry.BasisAmount = percentage.Sum(Function(item) item.LineTotalAmount)
                                                   Return entry
                                               End Function)

        xRechnung.Taxes.AddRange(taxEntries)

        Dim currentCulture = Thread.CurrentThread.CurrentCulture
        Dim currentUiCulture = Thread.CurrentThread.CurrentUICulture

        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture

        xRechnung.Save(xmlStream, ZUGFeRDVersion.Version23, Profile.XRechnung, ZUGFeRDFormats.UBL)

        Thread.CurrentThread.CurrentCulture = currentCulture
        Thread.CurrentThread.CurrentUICulture = currentUiCulture
    End Sub

    Private Function GetMeasurementCode(type As String) As QuantityCodes
        Select Case type
            Case "Stk"
                Return QuantityCodes.C62
            Case "Stk."
                Return QuantityCodes.C62
            Case ""
                Return QuantityCodes.C62
            Case "L"
                Return QuantityCodes.LTR
            Case "AW"
                Return QuantityCodes.ACT
            Case Else
                Return QuantityCodes.Unknown

        End Select
    End Function

    Private Function GetItemsFromQuery(sql As String) As List(Of Dictionary(Of String, String))
        Dim table = _dataConnection.FillDataTable(sql)
        If table.Rows.Count = 0 Then Return New List(Of Dictionary(Of String, String))
        Return table.Rows.OfType(Of DataRow).Select(
            Function(row) table.Columns.OfType(Of DataColumn).ToDictionary(Function(column) column.ColumnName.Replace(" ", ""), Function(column) row(column.ColumnName).ToString())).ToList
    End Function

    Private Function GetSellerParameter(rechnungsArt As RechnungsArt) As Dictionary(Of String, String)
        Dim sql = GetSellerParameterSql(rechnungsArt)
        Dim dataTable = _dataConnection.FillDataTable(sql)
        Return dataTable.Columns.OfType(Of DataColumn).ToDictionary(Function(column) column.ColumnName, Function(column) dataTable.Rows.OfType(Of DataRow)().First()(column.ColumnName).ToString)
    End Function

    Private Function GetAdditionalSellerParameter(rechnungsArt As RechnungsArt) As Dictionary(Of String, String)
        Dim sql = GetAdditionalSellerParameterSql(rechnungsArt)
        Dim dataTable = _dataConnection.FillDataTable(sql)
        Return dataTable.Columns.OfType(Of DataColumn).ToDictionary(Function(column) column.ColumnName, Function(column) dataTable.Rows.OfType(Of DataRow)().First()(column.ColumnName).ToString)
    End Function

    Private Function GetBuyerData(kundenNr As String) As Dictionary(Of String, String)
        Dim sql = $"select * from Kunden where KundenNr = {kundenNr}"
        Dim dataTable = _dataConnection.FillDataTable(sql)
        Return dataTable.Columns.OfType(Of DataColumn).ToDictionary(Function(column) column.ColumnName, Function(column) dataTable.Rows.OfType(Of DataRow)().First()(column.ColumnName).ToString)
    End Function

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
    End Function


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
    End Function

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
End Class
