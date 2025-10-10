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
        EnsureParameterExists(211)
        EnsureParameterExists(1211)
        EnsureParameterExists(2211)
        LoadParameterData()
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

    Private Sub LoadParameterData()
        ' Tab 1: ParameterNr 1
        Dim dt1 = _dataConnection.FillDataTable("SELECT * FROM Parameter WHERE ParameterNr = 1")
        If dt1.Rows.Count > 0 Then
            txtFirma.Text = dt1.Rows(0).Item("Firma").ToString()
            txtName.Text = dt1.Rows(0).Item("Name").ToString()
            txtStrasse.Text = dt1.Rows(0).Item("Straﬂe").ToString()
            txtPLZ.Text = dt1.Rows(0).Item("PLZ").ToString()
            txtOrt.Text = dt1.Rows(0).Item("Ort").ToString()
            txtTelefon.Text = dt1.Rows(0).Item("Telefon").ToString()
            txtSteuernummer.Text = dt1.Rows(0).Item("Steuernummer").ToString()
        End If

        ' Tab 2: ParameterNr 211
        Dim dt2 = _dataConnection.FillDataTable("SELECT * FROM Parameter WHERE ParameterNr = 211")
        If dt2.Rows.Count > 0 Then
            txtModul1_211.Text = dt2.Rows(0).Item("Modul1").ToString()
            txtModul2_211.Text = dt2.Rows(0).Item("Modul2").ToString()
            txtModul3_211.Text = dt2.Rows(0).Item("Modul3").ToString()
        End If

        ' Tab 3: ParameterNr 1211
        Dim dt3 = _dataConnection.FillDataTable("SELECT * FROM Parameter WHERE ParameterNr = 1211")
        If dt3.Rows.Count > 0 Then
            txtModul1_1211.Text = dt3.Rows(0).Item("Modul1").ToString()
            txtModul2_1211.Text = dt3.Rows(0).Item("Modul2").ToString()
            txtModul3_1211.Text = dt3.Rows(0).Item("Modul3").ToString()
        End If

        ' Tab 4: ParameterNr 2211
        Dim dt4 = _dataConnection.FillDataTable("SELECT * FROM Parameter WHERE ParameterNr = 2211")
        If dt4.Rows.Count > 0 Then
            txtModul1_2211.Text = dt4.Rows(0).Item("Modul1").ToString()
            txtModul2_2211.Text = dt4.Rows(0).Item("Modul2").ToString()
            txtModul3_2211.Text = dt4.Rows(0).Item("Modul3").ToString()
        End If
    End Sub

    Private Sub btnSave1_Click(sender As Object, e As EventArgs) Handles btnSave1.Click
        Dim sql = "UPDATE Parameter SET Firma=?, Name=?, Straﬂe=?, PLZ=?, Ort=?, Telefon=?, Steuernummer=? WHERE ParameterNr=1"
        Dim cmd = _dataConnection.CreateCommand(sql)
        cmd.Parameters.AddWithValue("@Firma", txtFirma.Text)
        cmd.Parameters.AddWithValue("@Name", txtName.Text)
        cmd.Parameters.AddWithValue("@Straﬂe", txtStrasse.Text)
        cmd.Parameters.AddWithValue("@PLZ", txtPLZ.Text)
        cmd.Parameters.AddWithValue("@Ort", txtOrt.Text)
        cmd.Parameters.AddWithValue("@Telefon", txtTelefon.Text)
        cmd.Parameters.AddWithValue("@Steuernummer", txtSteuernummer.Text)
        cmd.ExecuteNonQuery()
        MessageBox.Show("Daten gespeichert.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnSave211_Click(sender As Object, e As EventArgs) Handles btnSave211.Click
        Dim sql = "UPDATE Parameter SET Modul1=?, Modul2=?, Modul3=? WHERE ParameterNr=211"
        Dim cmd = _dataConnection.CreateCommand(sql)
        cmd.Parameters.AddWithValue("@Modul1", txtModul1_211.Text)
        cmd.Parameters.AddWithValue("@Modul2", txtModul2_211.Text)
        cmd.Parameters.AddWithValue("@Modul3", txtModul3_211.Text)
        cmd.ExecuteNonQuery()
        MessageBox.Show("Daten gespeichert.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnSave1211_Click(sender As Object, e As EventArgs) Handles btnSave1211.Click
        Dim sql = "UPDATE Parameter SET Modul1=?, Modul2=?, Modul3=? WHERE ParameterNr=1211"
        Dim cmd = _dataConnection.CreateCommand(sql)
        cmd.Parameters.AddWithValue("@Modul1", txtModul1_1211.Text)
        cmd.Parameters.AddWithValue("@Modul2", txtModul2_1211.Text)
        cmd.Parameters.AddWithValue("@Modul3", txtModul3_1211.Text)
        cmd.ExecuteNonQuery()
        MessageBox.Show("Daten gespeichert.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnSave2211_Click(sender As Object, e As EventArgs) Handles btnSave2211.Click
        Dim sql = "UPDATE Parameter SET Modul1=?, Modul2=?, Modul3=? WHERE ParameterNr=2211"
        Dim cmd = _dataConnection.CreateCommand(sql)
        cmd.Parameters.AddWithValue("@Modul1", txtModul1_2211.Text)
        cmd.Parameters.AddWithValue("@Modul2", txtModul2_2211.Text)
        cmd.Parameters.AddWithValue("@Modul3", txtModul3_2211.Text)
        cmd.ExecuteNonQuery()
        MessageBox.Show("Daten gespeichert.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

End Class
