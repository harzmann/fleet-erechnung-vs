Module RechnungsSqlHelper

    Public Function GetSqlStatement(rechnungsArt As RechnungsArt) As String
        Select Case rechnungsArt
            Case RechnungsArt.Werkstatt
                Dim columnMapping As New Dictionary(Of String, String) From
                    {
                        {"RechnungsNr", "'RG-Nr.'"},
                        {"FORMAT (Rechnungsdatum, 'dd.MM.yyyy')", "'RG-Datum'"},
                        {"WANr", "'WA-Kurzbezeichnung'"},
                        {"FORMAT (Datum, 'dd.MM.yyyy')", "'WA-Datum'"},
                        {"Auftragsart", "'WA-Art'"},
                        {"Belegart", "Belegart"},
                        {"KundenNr", "'KD-Nr.'"},
                        {"DebitorNr", "'Debitor-Nr.'"},
                        {"Firma", "Firma"},
                        {"LeitwegeID", "'Leitweg-Id'"},
                        {"EmailRechnung", "'EmailRechnung'"},
                        {"Summe", "'Summe netto'"},
                        {"Exportiert", "X"},
                        {"Gebucht", "B"},
                        {"ERechnung_Locked", "L"}
                    }

                Return $"select {String.Join(",", columnMapping.Select(Function(map) $"{map.Key} as {map.Value}"))} from [abfr_wavkliste] where Storno=0"
            Case RechnungsArt.Tanken
                Dim columnMapping As New Dictionary(Of String, String) From
                    {
                        {"RechnungsNr", "'RG-Nr.'"},
                        {"FORMAT (Rechnungsdatum, 'dd.MM.yyyy')", "'RG-Datum'"},
                        {"Anmerkungen", "Anmerkungen"},
                        {"Belegart", "Belegart"},
                        {"KundenNr", "'KD-Nr.'"},
                        {"DebitorNr", "'Debitor-Nr.'"},
                        {"Firma", "Firma"},
                        {"LeitwegeID", "'Leitweg-Id'"},
                        {"EmailRechnung", "'EmailRechnung'"},
                        {"Summe", "'Summe netto'"},
                        {"Exportiert", "X"},
                        {"Gebucht", "B"},
                        {"ERechnung_Locked", "L"}
                    }

                Return $"select {String.Join(",", columnMapping.Select(Function(map) $"{map.Key} as {map.Value}"))} from [abfr_tavkliste] where Storno=0"
            Case RechnungsArt.Manuell
                Dim columnMapping As New Dictionary(Of String, String) From
                    {
                        {"RechnungsNr", "'RG-Nr.'"},
                        {"FORMAT (Rechnungsdatum, 'dd.MM.yyyy')", "'RG-Datum'"},
                        {"Anmerkungen", "Anmerkungen"},
                        {"Belegart", "Belegart"},
                        {"KundenNr", "'KD-Nr.'"},
                        {"DebitorNr", "'Debitor-Nr.'"},
                        {"Firma", "Firma"},
                        {"LeitwegeID", "'Leitweg-Id'"},
                        {"EmailRechnung", "'EmailRechnung'"},
                        {"Summe", "'Summe netto'"},
                        {"Exportiert", "X"},
                        {"Gebucht", "B"},
                        {"ERechnung_Locked", "L"}
                    }

                Return $"select {String.Join(",", columnMapping.Select(Function(map) $"{map.Key} as {map.Value}"))} from [abfr_mrvkliste] where Storno=0"
        End Select

        Return String.Empty

    End Function

End Module
