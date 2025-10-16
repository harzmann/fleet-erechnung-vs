Imports System.Data
Imports System.Windows.Forms
Imports ehfleet_classlibrary

Public Class ParameterEditorForm
    Private ReadOnly _dataConnection As General.Database

    Public Sub New(dataConnection As General.Database)
        InitializeComponent()
        _dataConnection = dataConnection
    End Sub

    Private Sub ParameterEditorForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        EnsureParameterExists(1)
        EnsureParameterExists(11)
        EnsureParameterExists(21)
        EnsureParameterExists(211)
        EnsureParameterExists(1211)
        EnsureParameterExists(2211)
        LoadAllParameterValues()
    End Sub

    Private Sub EnsureParameterExists(parameterNr As Integer)
        Dim dt = _dataConnection.FillDataTable($"SELECT COUNT(*) FROM Parameter WHERE ParameterNr = {parameterNr}")
        If Convert.ToInt32(dt.Rows(0)(0)) = 0 Then
            Dim sql = "INSERT INTO Parameter (ParameterNr, Aktiviert) VALUES (?, -1)"
            Dim cmd = _dataConnection.CreateCommand(sql)
            cmd.Parameters.AddWithValue("@ParameterNr", parameterNr)
            cmd.ExecuteNonQuery()
        End If
    End Sub

    ' --- Einfache Getter/Setter für Parameterwerte ---
    Private Function GetParameterValue(parameterNr As Integer, fieldName As String) As String
        Dim dt = _dataConnection.FillDataTable($"SELECT [{fieldName}] FROM Parameter WHERE ParameterNr = {parameterNr}")
        If dt.Rows.Count > 0 AndAlso Not IsDBNull(dt.Rows(0).Item(fieldName)) Then
            Return dt.Rows(0).Item(fieldName).ToString()
        End If
        Return ""
    End Function

    Private Sub SetParameterValue(parameterNr As Integer, fieldName As String, value As String)
        Dim sql = $"UPDATE Parameter SET [{fieldName}]=? WHERE ParameterNr=?"
        Dim cmd = _dataConnection.CreateCommand(sql)
        cmd.Parameters.AddWithValue("@" & fieldName, value)
        cmd.Parameters.AddWithValue("@ParameterNr", parameterNr)
        cmd.ExecuteNonQuery()
    End Sub

    ' --- Vereinheitlichtes Laden aller Parameterwerte für alle Tabs ---
    Private Sub LoadAllParameterValues()
        ' Tab 1: ParameterNr 1
        txtFirma.Text = GetParameterValue(1, "Firma")
        txtName.Text = GetParameterValue(1, "Name")
        txtStrasse.Text = GetParameterValue(1, "Straße")
        txtPLZ.Text = GetParameterValue(1, "PLZ")
        txtOrt.Text = GetParameterValue(1, "Ort")
        txtTelefon.Text = GetParameterValue(1, "Telefon")
        txtSteuernummer.Text = GetParameterValue(1, "Steuernummer")
        ' Tab TA: ParameterNr 11
        txtFirma_TA.Text = GetParameterValue(11, "Firma")
        txtName_TA.Text = GetParameterValue(11, "Name")
        txtStrasse_TA.Text = GetParameterValue(11, "Strasse")
        txtPLZ_TA.Text = GetParameterValue(11, "PLZ")
        txtOrt_TA.Text = GetParameterValue(11, "Ort")
        txtTelefon_TA.Text = GetParameterValue(11, "Telefon")
        txtSteuernummer_TA.Text = GetParameterValue(11, "Steuernummer")
        ' Tab MR: ParameterNr 21
        txtFirma_MR.Text = GetParameterValue(21, "Firma")
        txtName_MR.Text = GetParameterValue(21, "Name")
        txtStrasse_MR.Text = GetParameterValue(21, "Strasse")
        txtPLZ_MR.Text = GetParameterValue(21, "PLZ")
        txtOrt_MR.Text = GetParameterValue(21, "Ort")
        txtTelefon_MR.Text = GetParameterValue(21, "Telefon")
        txtSteuernummer_MR.Text = GetParameterValue(21, "Steuernummer")
        ' Tab 2: ParameterNr 211
        txtModul1_211.Text = GetParameterValue(211, "Modul1")
        txtModul2_211.Text = GetParameterValue(211, "Modul2")
        txtModul3_211.Text = GetParameterValue(211, "Modul3")
        txtAusgabepfad_211.Text = GetParameterValue(211, "Ausgabepfad")
        ' Tab 3: ParameterNr 1211
        txtModul1_1211.Text = GetParameterValue(1211, "Modul1")
        txtModul2_1211.Text = GetParameterValue(1211, "Modul2")
        txtModul3_1211.Text = GetParameterValue(1211, "Modul3")
        txtAusgabepfad_1211.Text = GetParameterValue(1211, "Ausgabepfad")
        ' Tab 4: ParameterNr 2211
        txtModul1_2211.Text = GetParameterValue(2211, "Modul1")
        txtModul2_2211.Text = GetParameterValue(2211, "Modul2")
        txtModul3_2211.Text = GetParameterValue(2211, "Modul3")
        txtAusgabepfad_2211.Text = GetParameterValue(2211, "Ausgabepfad")
    End Sub

    ' --- Vereinheitlichtes Speichern aller Parameterwerte für alle Tabs ---
    Private Sub SaveAllParameterValues()
        ' Tab 1: ParameterNr 1
        SetParameterValue(1, "Firma", txtFirma.Text)
        SetParameterValue(1, "Name", txtName.Text)
        SetParameterValue(1, "Straße", txtStrasse.Text)
        SetParameterValue(1, "PLZ", txtPLZ.Text)
        SetParameterValue(1, "Ort", txtOrt.Text)
        SetParameterValue(1, "Telefon", txtTelefon.Text)
        SetParameterValue(1, "Steuernummer", txtSteuernummer.Text)
        ' Tab TA: ParameterNr 11
        SetParameterValue(11, "Firma", txtFirma_TA.Text)
        SetParameterValue(11, "Name", txtName_TA.Text)
        SetParameterValue(11, "Strasse", txtStrasse_TA.Text)
        SetParameterValue(11, "PLZ", txtPLZ_TA.Text)
        SetParameterValue(11, "Ort", txtOrt_TA.Text)
        SetParameterValue(11, "Telefon", txtTelefon_TA.Text)
        SetParameterValue(11, "Steuernummer", txtSteuernummer_TA.Text)
        ' Tab MR: ParameterNr 21
        SetParameterValue(21, "Firma", txtFirma_MR.Text)
        SetParameterValue(21, "Name", txtName_MR.Text)
        SetParameterValue(21, "Strasse", txtStrasse_MR.Text)
        SetParameterValue(21, "PLZ", txtPLZ_MR.Text)
        SetParameterValue(21, "Ort", txtOrt_MR.Text)
        SetParameterValue(21, "Telefon", txtTelefon_MR.Text)
        SetParameterValue(21, "Steuernummer", txtSteuernummer_MR.Text)
        ' Tab 2: ParameterNr 211
        SetParameterValue(211, "Modul1", txtModul1_211.Text)
        SetParameterValue(211, "Modul2", txtModul2_211.Text)
        SetParameterValue(211, "Modul3", txtModul3_211.Text)
        SetParameterValue(211, "Ausgabepfad", txtAusgabepfad_211.Text)
        ' Tab 3: ParameterNr 1211
        SetParameterValue(1211, "Modul1", txtModul1_1211.Text)
        SetParameterValue(1211, "Modul2", txtModul2_1211.Text)
        SetParameterValue(1211, "Modul3", txtModul3_1211.Text)
        SetParameterValue(1211, "Ausgabepfad", txtAusgabepfad_1211.Text)
        ' Tab 4: ParameterNr 2211
        SetParameterValue(2211, "Modul1", txtModul1_2211.Text)
        SetParameterValue(2211, "Modul2", txtModul2_2211.Text)
        SetParameterValue(2211, "Modul3", txtModul3_2211.Text)
        SetParameterValue(2211, "Ausgabepfad", txtAusgabepfad_2211.Text)
    End Sub

    Private Sub btnSaveAll_Click(sender As Object, e As EventArgs) Handles btnSaveAll.Click
        SaveAllParameterValues()
        MessageBox.Show("Alle Daten gespeichert.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ' --- Validierung für Ausgabepfad (max. 255 Zeichen, plausibler Pfad) ---
    Private Function ValidateAusgabepfad(pfad As String) As Boolean
        If String.IsNullOrWhiteSpace(pfad) OrElse pfad.Length > 255 Then Return False
        Try
            Dim invalidChars = IO.Path.GetInvalidPathChars()
            If pfad.IndexOfAny(invalidChars) >= 0 Then Return False
            ' Optional: Existenz prüfen, aber nicht erzwingen:
            ' If Not IO.Directory.Exists(pfad) Then Return False
            Return True
        Catch
            Return False
        End Try
    End Function

    ' --- Eingabefeld-Validierung für IBAN/BIC/Telefon etc. ---
    Private Sub txtModul2_211_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtModul2_211.Validating, txtModul2_1211.Validating, txtModul2_2211.Validating
        Dim tb = DirectCast(sender, TextBox)
        If tb.Text.Length > 0 AndAlso tb.Text.Length < 15 Then
            MessageBox.Show("Bitte geben Sie eine gültige IBAN (mind. 15 Zeichen) ein.", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            e.Cancel = True
        End If
    End Sub

    Private Sub txtModul3_211_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtModul3_211.Validating, txtModul3_1211.Validating, txtModul3_2211.Validating
        Dim tb = DirectCast(sender, TextBox)
        If tb.Text.Length > 0 AndAlso (tb.Text.Length < 8 OrElse tb.Text.Length > 11) Then
            MessageBox.Show("Bitte geben Sie eine gültige BIC (8 oder 11 Zeichen) ein.", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            e.Cancel = True
        End If
    End Sub

End Class
