Imports System
Imports System.Data
Imports System.Linq

Public Class General
    Public Class Database
        Public Parameters As New DataTable()
        Public LastQuery As String
        Public Sub New()
            Parameters.Columns.Add("Aktiviert", GetType(Boolean))
            Parameters.Columns.Add("Text", GetType(String))
        End Sub
        Public Function FillDataTable(sql As String) As DataTable
            LastQuery = sql
            If sql.StartsWith("SELECT * FROM ") Then
                Dim invoice As New DataTable()
                invoice.Columns.Add("Rechnungsdatum", GetType(Date))
                invoice.Rows.Add(New Date(2026, 11, 20))
                Return invoice
            End If
            Return Parameters
        End Function
    End Class
End Class

Module Program
    Sub Check(actual As String, expected As String, scenario As String)
        If actual <> expected Then Throw New Exception(scenario & ": " & actual & " <> " & expected)
        Console.WriteLine("PASS " & scenario)
    End Sub
    Sub Main()
        Dim db As New General.Database()
        Dim types = {"WA", "TA", "MR"}
        Dim numbers = {156, 1156, 2156}
        For i = 0 To types.Length - 1
            Check(Sale.Invoicing.FormatInvoiceNumber(db, types(i), "72"), "72", "missing parameter " & types(i))
            Check(db.LastQuery, "select * from Parameter where ParameterNr = " & numbers(i), "parameter mapping " & types(i))
            db.Parameters.Rows.Add(False, "000000")
            Check(Sale.Invoicing.FormatInvoiceNumber(db, types(i), "72"), "72", "disabled parameter " & types(i))
            db.Parameters.Rows(0)("Aktiviert") = True
            Check(Sale.Invoicing.FormatInvoiceNumber(db, types(i), "72"), "000072", "enabled format " & types(i))
            db.Parameters.Rows(0)("Text") = "[RechnungsdatumYY]0000"
            Check(Sale.Invoicing.FormatInvoiceNumber(db, types(i), "72"), "260072", "date format " & types(i))
            db.Parameters.Rows.Clear()
        Next
        Check(Sale.Invoicing.FormatInvoiceNumber(db, "WA", "ABC-72"), "ABC-72", "non-numeric number")
        Check(Sale.Invoicing.FormatInvoiceNumber(db, "OTHER", "72"), "72", "other invoice type")
    End Sub
End Module
