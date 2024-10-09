Imports ehfleet_classlibrary
Imports EHFleetXRechnung.Schemas
Imports Stimulsoft.Controls.Win.DotNetBar
Imports Stimulsoft.Report.Export
Imports System.IO
Imports System.Xml.Serialization

Public Class XRechnungExporter
    Private _dataConnection As General.Database

    Public Sub New(dataConnection As General.Database)
        _dataConnection = dataConnection
    End Sub

    Public Sub CreateBillXml(xmlStream As Stream, billType As RechnungsArt, rechnungsNummer As Integer)
        Dim serializer = New XmlSerializer(GetType(Invoice))
        Dim sqls = GetSqlStatements(billType, New List(Of Integer) From {rechnungsNummer})
        Dim items = GetItemsFromQuery(GetSqlStatementForBill(billType, New List(Of Integer) From {rechnungsNummer}))
        Dim vatData = GetItemsFromQuery(GetSqlStatementForVat(billType, New List(Of Integer) From {rechnungsNummer}))
        Dim sellerData = GetSellerParameter(billType)
        Dim buyerData = GetBuyerData(items("KundenNr"))
        Dim xRechnung = New Invoice()
        xRechnung.Invoicenumber.Value = rechnungsNummer.ToString()
        xRechnung.Invoicecurrencycode.Value = sellerData("Währung")
        xRechnung.Buyer.BuyerName.Value = items("Firma")
        xRechnung.Buyer.BuyerPostalAddress.BuyerAddressline1.Value = items("Rechnungsadresse")
        'xRechnung.Buyer.BuyerPostalAddress.BuyerAddressline2.Value = items("")
        'xRechnung.Buyer.BuyerPostalAddress.BuyerAddressline3.Value = items("")
        xRechnung.Buyer.BuyerPostalAddress.BuyerCity.Value = items("Ort")
        xRechnung.Buyer.BuyerPostalAddress.BuyerCountrySubdivision.Value = items("FirmaOderAbteilung")
        xRechnung.Buyer.BuyerPostalAddress.BuyerPostCode.Value = items("Postleitzahl")
        xRechnung.Buyer.BuyerPostalAddress.BuyerCountryCode.Value = buyerData("LandISO")

        xRechnung.Buyer.BuyerIdentifier.Value = items("KundenNr")
        xRechnung.Buyer.BuyerVATidentifier.Value = items("UStIdNr")
        xRechnung.Buyer.BuyerContact.BuyerContactpoint.Value = items("Kontakt")
        xRechnung.Buyer.BuyerContact.BuyerContactEmailAddress.Value = items("EmailAdresse")
        xRechnung.Buyer.BuyerContact.BuyerContactTelephonenumber.Value = items("Telefonnummer")
        xRechnung.Buyer.BuyerElectronicAddress.Value = buyerData("EmailRechnung")
        'xRechnung.Buyer.BuyerLegalRegistrationIdentifier.Value = items("")
        xRechnung.BuyerAccountingReference.Value = items("KostenstelleKunde")
        Dim referenceColumn = ""
        Select Case billType
            Case RechnungsArt.Werkstatt
                referenceColumn = "WABez"
            Case RechnungsArt.Tanken
                referenceColumn = "Anmerkungen"
            Case RechnungsArt.Manuell
                referenceColumn = "Anmerkungen"
        End Select

        xRechnung.BuyerReference.Value = items(referenceColumn)
        xRechnung.InvoiceIssueDate.Value = items("Rechnungsdatum")
        'xRechnung.Payee.PayeeIdentifier.Value = items("DebitorNr")
        'xRechnung.Payee.PayeeLegalRegistrationIdentifier.Value = items("")
        xRechnung.Payee.PayeeName.Value = sellerData("Firma") 'Auftraggnehmer, Zahlungsempfaenger

        Dim dueDate = ""
        Try
            Dim billDate = DateTime.Parse(items("Rechnungsdatum"))
            Dim daysToDueDate = Integer.Parse(items("NettoTage"))
            dueDate = billDate.AddDays(daysToDueDate)
        Catch ex As Exception
            dueDate = items("Rechnungsdatum")
        End Try

        xRechnung.PaymentDueDate.Value = dueDate
        Dim payment = New CreditTransfer()
        payment.PaymentAccountIdentifier.Value = sellerData("Modul2") 'IBAN
        xRechnung.PaymentInstruction.Paymentmeanstext.Value = "IBAN"
        xRechnung.PaymentInstruction.Paymentmeanstypecode.Value = "58"

        xRechnung.PaymentInstruction.CreditTransfer.Add(payment)

        xRechnung.Seller.SellerName.Value = sellerData("Firma") 'Auftraggnehmer
        xRechnung.Seller.SellerVATIdentifier.Value = sellerData("Steuernummer")
        'xRechnung.Seller.SellerLegalRegistrationIdentifier.Value = sellerData("")
        'xRechnung.Seller.SellerContact.SellerContactPoint.Value = sellerData("")
        'xRechnung.Seller.SellerContact.SellerContactEmailAddress.Value = sellerData("")
        xRechnung.Seller.SellerContact.SellerContactTelephoneNumber.Value = sellerData("Telefon")
        'xRechnung.Seller.SellerTradingname.Value = sellerData("Firma")
        xRechnung.Seller.SellerPostaladdress.SellerAddressline1.Value = sellerData("Straße")
        'xRechnung.Seller.SellerPostaladdress.SellerAddressline2.Value = sellerData("")
        'xRechnung.Seller.SellerPostaladdress.SellerAddressline3.Value = sellerData("")
        xRechnung.Seller.SellerPostaladdress.SellerCity.Value = sellerData("Ort")
        xRechnung.Seller.SellerPostaladdress.SellerPostcode.Value = sellerData("PLZ")
        'xRechnung.Seller.SellerPostaladdress.SellerCountrySubdivision.Value = sellerData("")
        xRechnung.Seller.SellerPostaladdress.SellerCountryCode.Value = "DE"

        Dim poReference As String = String.Empty
        items.TryGetValue("WANr", poReference)
        xRechnung.PurchaseOrderReference.Value = poReference
        xRechnung.ValueAddedTaxPointDate.Value = items("Rechnungsdatum")
        'xRechnung.Salesorderreference.Value = items("")
        xRechnung.DeliveryInformation.ActualDeliveryDate.Value = items("Lieferdatum")
        xRechnung.DeliveryInformation.DeliverToAddress.DeliverToAddressline1.Value = items("Rechnungsadresse")
        'xRechnung.DeliveryInformation.DeliverToAddress.Delivertoaddressline2 = items("")
        'xRechnung.DeliveryInformation.DeliverToAddress.Delivertoaddressline3 = items("")
        xRechnung.DeliveryInformation.DeliverToAddress.DeliverToCity.Value = items("Ort")
        'xRechnung.DeliveryInformation.DeliverToAddress.DelivertoCountrySubdivision = items("")
        xRechnung.DeliveryInformation.DeliverToAddress.DelivertoPostCode.Value = items("Postleitzahl")
        xRechnung.DeliveryInformation.DeliverToAddress.DelivertoCountryCode.Value = buyerData("LandISO")

        Dim vatAmount As Decimal
        Dim netSum As Decimal
        items.TryGetValue("Mehrwertsteuer", vatAmount)
        items.TryGetValue("Summe", netSum)

        xRechnung.DOCUMENTTOTALS.Amountdueforpayment.Value = (vatAmount + netSum).ToString("F2")
        xRechnung.DOCUMENTTOTALS.InvoicetotalamountwithoutVAT.Value = items("Summe")
        xRechnung.DOCUMENTTOTALS.InvoicetotalVATamount.Value = items("Mehrwertsteuer")
        'add line items
        For Each query In sqls
            items = GetItemsFromQuery(query)
            If Not items.Any() Then Continue For
            Dim lineItem = New InvoiceLine
            If billType <> RechnungsArt.Tanken Then
                lineItem.Invoicelineidentifier.Value = items("ArtikelNr")
                lineItem.Invoicelinenote.Value = items("Artikelbez")
                lineItem.Invoicedquantity.Value = items("Menge")
                lineItem.Invoicedquantityunitofmeasurecode.Value = GetMeasurementCode(items("ArtikelMEH"))
                lineItem.Invoicelinenetamount.Value = items("Gesamtpreis")
                lineItem.INVOICELINEPERIOD.Invoicelineperiodstartdate.Value = items("Datum")
                lineItem.INVOICELINEPERIOD.Invoicelineperiodenddate.Value = items("Datum")
                Dim rabatt = New InvoiceLineAllowances
                rabatt.Invoicelineallowanceamount.Value = items("Rabatt")
                lineItem.INVOICELINEALLOWANCES.Add(rabatt)
                lineItem.PRICEDETAILS.Itemgrossprice.Value = items("EPreis")
                Dim lineVat = New LineVatInformation
                lineVat.InvoiceditemVATrate.Value = items("MwStProzent")
                lineVat.InvoiceditemVATcategorycode.Value = "S"
                lineItem.LINEVATINFORMATION.Add(lineVat)
                lineItem.ITEMINFORMATION.Itemname.Value = items("Artikelbez")
                lineItem.ITEMINFORMATION.Itemdescription.Value = items("ArtikelbezLang")
                lineItem.ITEMINFORMATION.ItemSellersidentifier.Value = items("ArtikelNr")
                lineItem.ITEMINFORMATION.Itemcountryoforigin.Value = "DE"
            Else
                lineItem.Invoicelineidentifier.Value = items("SpritID")
                lineItem.Invoicelinenote.Value = items("Bezeichnung")
                lineItem.Invoicedquantity.Value = items("Menge")
                lineItem.Invoicedquantityunitofmeasurecode.Value = GetMeasurementCode(items("Mengeneinheit"))
                lineItem.Invoicelinenetamount.Value = items("Gesamtpreis")
                lineItem.INVOICELINEPERIOD.Invoicelineperiodstartdate.Value = items("Tankdatum")
                lineItem.INVOICELINEPERIOD.Invoicelineperiodenddate.Value = items("Tankdatum")
                Dim rabatt = New InvoiceLineAllowances
                rabatt.Invoicelineallowanceamount.Value = items("Rabatt")
                lineItem.INVOICELINEALLOWANCES.Add(rabatt)
                lineItem.PRICEDETAILS.Itemgrossprice.Value = items("EPreis")
                Dim lineVat = New LineVatInformation
                lineVat.InvoiceditemVATrate.Value = items("MwStProzent")
                lineVat.InvoiceditemVATcategorycode.Value = "S"
                lineItem.LINEVATINFORMATION.Add(lineVat)
                lineItem.ITEMINFORMATION.Itemname.Value = items("Bezeichnung")
                lineItem.ITEMINFORMATION.Itemdescription.Value = items("Bezeichnung")
                lineItem.ITEMINFORMATION.ItemSellersidentifier.Value = items("SpritID")
                lineItem.ITEMINFORMATION.Itemcountryoforigin.Value = "DE"
            End If

            xRechnung.INVOICELINE.Add(lineItem)
        Next

        serializer.Serialize(xmlStream, xRechnung)
    End Sub

    Private Function GetMeasurementCode(type As String) As String
        Select Case type
            Case "Stk"
                Return "H87"
            Case "L"
                Return "LTR"
            Case Else
                Return type

        End Select
    End Function

    Private Function GetItemsFromQuery(sql As String) As Dictionary(Of String, String)
        Dim table = _dataConnection.FillDataTable(sql)
        If table.Rows.Count = 0 Then Return New Dictionary(Of String, String)()
        Return table.Columns.OfType(Of DataColumn).ToDictionary(Function(column) column.ColumnName.Replace(" ", ""), Function(column) table.Rows.OfType(Of DataRow)().First()(column.ColumnName).ToString())
    End Function

    Private Function GetSellerParameter(rechnungsArt As RechnungsArt) As Dictionary(Of String, String)
        Dim sql = GetSellerParameterSql(rechnungsArt)
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
