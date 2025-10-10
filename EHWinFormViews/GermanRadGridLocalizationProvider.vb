Imports Telerik.WinControls.UI.Localization

Namespace GermanRadGridViewLocalization
    Public Class GermanRadGridLocalizationProvider
        Inherits RadGridLocalizationProvider

        Public Overrides Function GetLocalizedString(id As String) As String
            Select Case id
                Case RadGridStringId.ConditionalFormattingPleaseSelectValidCellValue
                    Return "Bitte wählen Sie einen gültigen Wert aus"
                Case RadGridStringId.ConditionalFormattingPleaseSetValidCellValue
                    Return "Bitte legen Sie einen gültigen Zellenwert fest"
                Case RadGridStringId.ConditionalFormattingPleaseSetValidCellValues
                    Return "Bitte legen Sie gültige Zellenwerte fest"
                Case RadGridStringId.ConditionalFormattingPleaseSetValidExpression
                    Return "Bitte legen Sie einen gültigen Ausdruck fest"
                Case RadGridStringId.ConditionalFormattingItem
                    Return "Artikel"
                Case RadGridStringId.ConditionalFormattingInvalidParameters
                    Return "Ungültige Parameter"

                Case RadGridStringId.FilterFunctionBetween
                    Return "Zwischen"
                Case RadGridStringId.FilterFunctionContains
                    Return "Enthält"
                Case RadGridStringId.FilterFunctionDoesNotContain
                    Return "Enthält nicht"
                Case RadGridStringId.FilterFunctionEndsWith
                    Return "Endet mit"
                Case RadGridStringId.FilterFunctionEqualTo
                    Return "Ist Gleich"
                Case RadGridStringId.FilterFunctionGreaterThan
                    Return "Größer als"
                Case RadGridStringId.FilterFunctionGreaterThanOrEqualTo
                    Return "Größer oder gleich"
                Case RadGridStringId.FilterFunctionIsEmpty
                    Return "Ist leer"
                Case RadGridStringId.FilterFunctionIsNull
                    Return "Ist null"
                Case RadGridStringId.FilterFunctionLessThan
                    Return "Kleiner als"
                Case RadGridStringId.FilterFunctionLessThanOrEqualTo
                    Return "Kleiner oder gleich"
                Case RadGridStringId.FilterFunctionNoFilter
                    Return "Kein Filter"
                Case RadGridStringId.FilterFunctionNotBetween
                    Return "Nicht dazwischen"
                Case RadGridStringId.FilterFunctionNotEqualTo
                    Return "Nicht gleich"
                Case RadGridStringId.FilterFunctionNotIsEmpty
                    Return "Ist nicht leer"
                Case RadGridStringId.FilterFunctionNotIsNull
                    Return "Ist nicht null"
                Case RadGridStringId.FilterFunctionStartsWith
                    Return "Beginnt mit"
                Case RadGridStringId.FilterFunctionCustom
                    Return "Benutzerdefinierter Filter"

                Case RadGridStringId.FilterOperatorBetween
                    Return "Zwischen"
                Case RadGridStringId.FilterOperatorContains
                    Return "Enthält"
                Case RadGridStringId.FilterOperatorDoesNotContain
                    Return "Enthält nicht"
                Case RadGridStringId.FilterOperatorEndsWith
                    Return "Endet mit"
                Case RadGridStringId.FilterOperatorEqualTo
                    Return "Ist gleich"
                Case RadGridStringId.FilterOperatorGreaterThan
                    Return "Größer als"
                Case RadGridStringId.FilterOperatorGreaterThanOrEqualTo
                    Return "Größer oder gleich"
                Case RadGridStringId.FilterOperatorIsEmpty
                    Return "Ist leer"
                Case RadGridStringId.FilterOperatorIsNull
                    Return "Ist null"
                Case RadGridStringId.FilterOperatorLessThan
                    Return "Weniger als"
                Case RadGridStringId.FilterOperatorLessThanOrEqualTo
                    Return "Kleiner oder gleich"
                Case RadGridStringId.FilterOperatorNoFilter
                    Return "Kein Filter"
                Case RadGridStringId.FilterOperatorNotBetween
                    Return "Nicht dazwischen"
                Case RadGridStringId.FilterOperatorNotEqualTo
                    Return "Ist nicht Gleich"
                Case RadGridStringId.FilterOperatorNotIsEmpty
                    Return "Ist nicht Leer"
                Case RadGridStringId.FilterOperatorNotIsNull
                    Return "Ist nicht Null"
                Case RadGridStringId.FilterOperatorStartsWith
                    Return "Beginnt mit"
                Case RadGridStringId.FilterOperatorIsLike
                    Return "Ähnlich"
                Case RadGridStringId.FilterOperatorNotIsLike
                    Return "Nicht ähnlich"
                Case RadGridStringId.FilterOperatorIsContainedIn
                    Return "Enthalten in"
                Case RadGridStringId.FilterOperatorNotIsContainedIn
                    Return "Nicht enthalten in"
                Case RadGridStringId.FilterOperatorCustom
                    Return "Benutzerdefiniert"

                Case RadGridStringId.CustomFilterMenuItem
                    Return "Benutzerdefiniert"
                Case RadGridStringId.CustomFilterDialogCaption
                    Return "RadGridView Filter Dialog [{0}]"
                Case RadGridStringId.CustomFilterDialogLabel
                    Return "Zeilen anzeigen die:"
                Case RadGridStringId.CustomFilterDialogRbAnd
                    Return "UND"
                Case RadGridStringId.CustomFilterDialogRbOr
                    Return "ODER"
                Case RadGridStringId.CustomFilterDialogBtnOk
                    Return "Ok"
                Case RadGridStringId.CustomFilterDialogBtnCancel
                    Return "Abbrechen"
                Case RadGridStringId.CustomFilterDialogCheckBoxNot
                    Return "Nicht"
                Case RadGridStringId.CustomFilterDialogTrue
                    Return "Wahr"
                Case RadGridStringId.CustomFilterDialogFalse
                    Return "Falsch"

                Case RadGridStringId.FilterMenuBlanks
                    Return "Leer"
                Case RadGridStringId.FilterMenuAvailableFilters
                    Return "Verfügbare Filter"
                Case RadGridStringId.FilterMenuSearchBoxText
                    Return "Suche..."
                Case RadGridStringId.FilterMenuClearFilters
                    Return "Filter löschen"
                Case RadGridStringId.FilterMenuButtonOK
                    Return "OK"
                Case RadGridStringId.FilterMenuButtonCancel
                    Return "Abbrechen"
                Case RadGridStringId.FilterMenuSelectionAll
                    Return "Alle"
                Case RadGridStringId.FilterMenuSelectionAllSearched
                    Return "Alle such Ergebnisse"
                Case RadGridStringId.FilterMenuSelectionNull
                    Return "Null"
                Case RadGridStringId.FilterMenuSelectionNotNull
                    Return "Nicht Null"

                Case RadGridStringId.FilterFunctionSelectedDates
                    Return "Filter nach spezifischem Datum:"
                Case RadGridStringId.FilterFunctionToday
                    Return "Heute"
                Case RadGridStringId.FilterFunctionYesterday
                    Return "Gestern"
                Case RadGridStringId.FilterFunctionDuringLast7days
                    Return "In den letzten 7 Tagen"

                Case RadGridStringId.FilterLogicalOperatorAnd
                    Return "UND"
                Case RadGridStringId.FilterLogicalOperatorOr
                    Return "ODER"
                Case RadGridStringId.FilterCompositeNotOperator
                    Return "NICHT"

                Case RadGridStringId.DeleteRowMenuItem
                    Return "Zeile löschen"
                Case RadGridStringId.SortAscendingMenuItem
                    Return "Aufsteigend sortieren"
                Case RadGridStringId.SortDescendingMenuItem
                    Return "Absteigend sortieren"
                Case RadGridStringId.ClearSortingMenuItem
                    Return "Sortierung entfernen"
                Case RadGridStringId.ConditionalFormattingMenuItem
                    Return "Bedingte Formatierung"
                Case RadGridStringId.GroupByThisColumnMenuItem
                    Return "Nach dieser Spalte gruppieren"
                Case RadGridStringId.UngroupThisColumn
                    Return "Gruppierung dieser Spalte aufheben"
                Case RadGridStringId.ColumnChooserMenuItem
                    Return "Spalten auswahl"
                Case RadGridStringId.HideMenuItem
                    Return "Spalte verbergen"
                Case RadGridStringId.HideGroupMenuItem
                    Return "Gruppe verbergen"
                Case RadGridStringId.UnpinMenuItem
                    Return "Fixierung lösen"
                Case RadGridStringId.UnpinRowMenuItem
                    Return "Fixierung lösen"
                Case RadGridStringId.PinMenuItem
                    Return "Fixierungs status"
                Case RadGridStringId.PinAtLeftMenuItem
                    Return "Links fixieren"
                Case RadGridStringId.PinAtRightMenuItem
                    Return "Rechts fixieren"
                Case RadGridStringId.PinAtBottomMenuItem
                    Return "Unten fixieren"
                Case RadGridStringId.PinAtTopMenuItem
                    Return "Oben fixieren"
                Case RadGridStringId.BestFitMenuItem
                    Return "Optimale Spaltenbreite"
                Case RadGridStringId.PasteMenuItem
                    Return "Einfügen"
                Case RadGridStringId.EditMenuItem
                    Return "Editieren"
                Case RadGridStringId.ClearValueMenuItem
                    Return "Wert entfernen"
                Case RadGridStringId.CopyMenuItem
                    Return "Kopieren"
                Case RadGridStringId.CutMenuItem
                    Return "Ausschneiden"
                Case RadGridStringId.AddNewRowString
                    Return "Klicken Sie hier, um eine neue Zeile einzufügen."

                Case RadGridStringId.ConditionalFormattingSortAlphabetically
                    Return "Spalten alphabetisch sortieren"
                Case RadGridStringId.ConditionalFormattingCaption
                    Return "Bedingte Formatierungsregeln Manager"
                Case RadGridStringId.ConditionalFormattingLblColumn
                    Return "Formatiere nur Zellen mit"
                Case RadGridStringId.ConditionalFormattingLblName
                    Return "Regelname"
                Case RadGridStringId.ConditionalFormattingLblType
                    Return "Zellenwert"
                Case RadGridStringId.ConditionalFormattingLblValue1
                    Return "Wert 1"
                Case RadGridStringId.ConditionalFormattingLblValue2
                    Return "Wert 2"
                Case RadGridStringId.ConditionalFormattingGrpConditions
                    Return "Regeln"
                Case RadGridStringId.ConditionalFormattingGrpProperties
                    Return "Regel Eigenschaften"
                Case RadGridStringId.ConditionalFormattingChkApplyToRow
                    Return "Diese Formatierung für die gesamte Zeile anwenden"
                Case RadGridStringId.ConditionalFormattingChkApplyOnSelectedRows
                    Return "Für ausgewählte Zeilen anwenden"
                Case RadGridStringId.ConditionalFormattingBtnAdd
                    Return "Neue Regel hinzufügen"
                Case RadGridStringId.ConditionalFormattingBtnRemove
                    Return "Entfernen"
                Case RadGridStringId.ConditionalFormattingBtnOK
                    Return "OK"
                Case RadGridStringId.ConditionalFormattingBtnCancel
                    Return "Abbrechen"
                Case RadGridStringId.ConditionalFormattingBtnApply
                    Return "Anwenden"
                Case RadGridStringId.ConditionalFormattingRuleAppliesOn
                    Return "Regel anwenden auf"

                Case RadGridStringId.ConditionalFormattingCondition
                    Return "Bedingung"
                Case RadGridStringId.ConditionalFormattingExpression
                    Return "Ausdruck"
                Case RadGridStringId.ConditionalFormattingChooseOne
                    Return "[Wählen Sie eine aus]"
                Case RadGridStringId.ConditionalFormattingEqualsTo
                    Return "gleich"
                Case RadGridStringId.ConditionalFormattingIsNotEqualTo
                    Return "ungleich"
                Case RadGridStringId.ConditionalFormattingStartsWith
                    Return "beginnt mit"
                Case RadGridStringId.ConditionalFormattingEndsWith
                    Return "endet mit"
                Case RadGridStringId.ConditionalFormattingContains
                    Return "enthält"
                Case RadGridStringId.ConditionalFormattingDoesNotContain
                    Return "enthält nicht"
                Case RadGridStringId.ConditionalFormattingIsGreaterThan
                    Return "größer als"
                Case RadGridStringId.ConditionalFormattingIsGreaterThanOrEqual
                    Return "größer gleich"
                Case RadGridStringId.ConditionalFormattingIsLessThan
                    Return "kleiner als"
                Case RadGridStringId.ConditionalFormattingIsLessThanOrEqual
                    Return "kleiner gleich"
                Case RadGridStringId.ConditionalFormattingIsBetween
                    Return "zwischen"
                Case RadGridStringId.ConditionalFormattingIsNotBetween
                    Return "nicht zwischen"
                Case RadGridStringId.ConditionalFormattingLblFormat
                    Return "Format"
                    'Case RadGridStringId.ConditionalFormattingLblPreview
                    '    Return "Vorschau"
                    'Case RadGridStringId.ConditionalFormattingLblValue
                    '    Return "Wert"
                    'Case RadGridStringId.ConditionalFormattingLblSecondValue
                    '    Return "Zweiter Wert"
                    'Case RadGridStringId.ConditionalFormattingGrpType
                    '    Return "Regel Typ"
                    'Case RadGridStringId.ConditionalFormattingGrpDetination
                    '    Return "Ziel Spalten"
                    'Case RadGridStringId.ConditionalFormattingGrpExpression
                    Return "Ausdruck"
                Case RadGridStringId.ConditionalFormattingGrpProperties
                    Return "Eigenschaften"
                    'Case RadGridStringId.ConditionalFormattingLblBetween
                    '    Return "zwischen"
                    'Case RadGridStringId.ConditionalFormattingLblEqual
                    '    Return "gleich"
                    'Case RadGridStringId.ConditionalFormattingLblNotEqual
                    '    Return "ungleich"
                    'Case RadGridStringId.ConditionalFormattingLblGreater
                    '    Return "größer"
                    'Case RadGridStringId.ConditionalFormattingLblGreaterEqual
                    '    Return "größer gleich"
                    'Case RadGridStringId.ConditionalFormattingLblLess
                    '    Return "kleiner"
                    'Case RadGridStringId.ConditionalFormattingLblLessEqual
                    '    Return "kleiner gleich"
                    'Case RadGridStringId.ConditionalFormattingLblContain
                    '    Return "enthält"
                    'Case RadGridStringId.ConditionalFormattingLblNotContain
                    '    Return "enthält nicht"
                    'Case RadGridStringId.ConditionalFormattingLblStarts
                    '    Return "beginnt mit"
                    'Case RadGridStringId.ConditionalFormattingLblEnds
                    '    Return "endet mit"
                Case RadGridStringId.GroupingPanelDefaultMessage
                    Return "Ziehen Sie eine Spalte hierher, um nach dieser Spalte zu gruppieren"
                Case RadGridStringId.NoDataText
                    Return "Keine Daten zum Anzeigen vorhanden"
                Case RadGridStringId.SearchRowTextBoxNullText
                    Return "Suchtext eingeben"
                Case RadGridStringId.SearchRowChooseColumns
                    Return "In Spalten suchen"
                Case RadGridStringId.SearchRowMatchCase
                Case RadGridStringId.SearchRowSearchFromCurrentPosition
                    Return "Gross- und Kleinschreibung beachten"
                Case Else
                    Return MyBase.GetLocalizedString(id)
            End Select

            Return MyBase.GetLocalizedString(id)
        End Function
    End Class
End Namespace
