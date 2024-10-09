Imports ehfleet_classlibrary
Imports EHFleetXRechnung.Schemas
Imports s2industries.ZUGFeRD
Imports Stimulsoft.Controls.Win.DotNetBar
Imports Stimulsoft.Report
Imports Stimulsoft.Report.Export
Imports System.IO
Imports System.Xml.Serialization

Public Class XRechnungExporter
    Private _dataConnection As General.Database

    Public Sub New(dataConnection As General.Database)
        _dataConnection = dataConnection
    End Sub

    Public Sub CreateBillXml(xmlStream As Stream, billType As RechnungsArt, rechnungsNummer As Integer)
        'Dim serializer = New XmlSerializer(GetType(Invoice))
        Dim sqls = GetSqlStatements(billType, New List(Of Integer) From {rechnungsNummer})
        Dim items = GetItemsFromQuery(GetSqlStatementForBill(billType, New List(Of Integer) From {rechnungsNummer}))
        Dim vatData = GetItemsFromQuery(GetSqlStatementForVat(billType, New List(Of Integer) From {rechnungsNummer}))
        Dim sellerData = GetSellerParameter(billType)
        Dim buyerData = GetBuyerData(items("KundenNr"))
        Dim xRechnung = New InvoiceDescriptor()
        xRechnung.InvoiceNo = rechnungsNummer.ToString()
        xRechnung.Currency = CType([Enum].Parse(GetType(CurrencyCodes), sellerData("Währung")), CurrencyCodes)
        Dim countryCode = CType([Enum].Parse(GetType(CountryCodes), buyerData("LandISO")), CountryCodes)
        xRechnung.SetBuyer(items("Firma"), items("Postleitzahl"), items("Ort"), items("Rechnungsadresse"), countryCode, items("KundenNr"))
        xRechnung.AddBuyerTaxRegistration(items("UStIdNr"), TaxRegistrationSchemeID.VA)
        xRechnung.Buyer.CountrySubdivisionName = items("FirmaOderAbteilung")
        xRechnung.SetBuyerContact(items("Kontakt"), "", items("EmailAdresse"), items("Telefonnummer"))
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
        Try
            Dim billDate = DateTime.Parse(items("Rechnungsdatum"))
            Dim daysToDueDate = Integer.Parse(items("NettoTage"))
            dueDate = billDate.AddDays(daysToDueDate)
        Catch ex As Exception
            dueDate = items("Rechnungsdatum")
        End Try

        xRechnung.SetTradePaymentTerms("Fälligkeit", dueDate)
        xRechnung.SetPaymentMeans(PaymentMeansTypeCodes.CreditTransfer, "IBAN", sellerData("Modul2"))

        xRechnung.SetSeller(sellerData("Firma"), sellerData("PLZ"), sellerData("Ort"), sellerData("Straße"), CountryCodes.DE)
        xRechnung.AddSellerTaxRegistration(sellerData("Steuernummer"), TaxRegistrationSchemeID.FC)

        Dim poReference As String = String.Empty
        items.TryGetValue("WANr", poReference)
        xRechnung.ReferenceOrderNo = poReference
        xRechnung.ActualDeliveryDate = items("Lieferdatum")

        Dim vatAmount As Decimal
        Dim netSum As Decimal
        items.TryGetValue("Mehrwertsteuer", vatAmount)
        items.TryGetValue("Summe", netSum)

        xRechnung.DuePayableAmount = (vatAmount + netSum).ToString("F2")
        xRechnung.TaxBasisAmount = items("Summe")
        xRechnung.TaxTotalAmount = items("Mehrwertsteuer")
        'add line items
        For Each query In sqls
            items = GetItemsFromQuery(query)
            If Not items.Any() Then Continue For

            If billType <> RechnungsArt.Tanken Then
                Dim lineItem = xRechnung.AddTradeLineItem(items("Artikelbez"))
                lineItem.SellerAssignedID = items("ArtikelNr")
                lineItem.BilledQuantity = Decimal.Parse(items("Menge"))
                lineItem.UnitQuantity = Decimal.Parse(items("Menge"))
                If Not String.IsNullOrWhiteSpace(GetMeasurementCode(items("ArtikelMEH"))) Then
                    lineItem.UnitCode = CType([Enum].Parse(GetType(QuantityCodes), GetMeasurementCode(items("ArtikelMEH"))), QuantityCodes)
                End If

                lineItem.LineTotalAmount = Decimal.Parse(items("Gesamtpreis"))

                If Not String.IsNullOrWhiteSpace(items("Datum")) Then
                        lineItem.BillingPeriodStart = DateTime.Parse(items("Datum"))
                        lineItem.BillingPeriodEnd = DateTime.Parse(items("Datum"))
                    End If

                    Dim baseAmount = Decimal.Parse(items("EPreis"))
                    If Not String.IsNullOrWhiteSpace(items("Rabatt")) Then
                        Dim actualAmount = baseAmount - Decimal.Parse(items("Rabatt"))
                        lineItem.GrossUnitPrice = baseAmount
                        lineItem.NetUnitPrice = actualAmount
                        lineItem.AddTradeAllowanceCharge(True, xRechnung.Currency, baseAmount, actualAmount, "")
                    Else
                        lineItem.GrossUnitPrice = baseAmount
                        lineItem.NetUnitPrice = baseAmount
                    End If

                    lineItem.TaxPercent = Decimal.Parse(items("MwStProzent"))
                    lineItem.TaxType = TaxTypes.VAT
                    lineItem.Description = items("ArtikelbezLang")
                Else
                    Dim lineItem = xRechnung.AddTradeLineItem(items("Bezeichnung"))
                lineItem.SellerAssignedID = items("SpritID")
                lineItem.BilledQuantity = Decimal.Parse(items("Menge"))
                lineItem.UnitQuantity = Decimal.Parse(items("Menge"))
                lineItem.UnitCode = CType([Enum].Parse(GetType(QuantityCodes), GetMeasurementCode(items("Mengeneinheit"))), QuantityCodes)
                lineItem.LineTotalAmount = Decimal.Parse(items("Gesamtpreis"))
                lineItem.BillingPeriodStart = DateTime.Parse(items("Tankdatum"))
                lineItem.BillingPeriodEnd = DateTime.Parse(items("Tankdatum"))
                Dim baseAmount = Decimal.Parse(items("EPreis"))
                Dim actualAmount = baseAmount - Decimal.Parse(items("Rabatt"))
                lineItem.GrossUnitPrice = baseAmount
                lineItem.NetUnitPrice = actualAmount
                lineItem.AddTradeAllowanceCharge(True, xRechnung.Currency, baseAmount, actualAmount, "")
                lineItem.TaxPercent = Decimal.Parse(items("MwStProzent"))
                lineItem.TaxType = TaxTypes.VAT
                lineItem.Description = items("Bezeichnung")
            End If
        Next

        xRechnung.Save(xmlStream, ZUGFeRDVersion.Version22, Profile.XRechnung, ZUGFeRDFormats.UBL)
        'serializer.Serialize(xmlStream, xRechnung)
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
