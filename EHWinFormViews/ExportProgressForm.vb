Imports System.Windows.Forms

Public Class ExportProgressForm
    Public Property CancelRequested As Boolean = False

    Public Sub New(totalCount As Integer)
        InitializeComponent()
        ProgressBar1.Minimum = 0
        ProgressBar1.Maximum = totalCount
        ProgressBar1.Value = 0
        LabelStatus.Text = ""
        LabelCount.Text = $"0 / {totalCount}"
        CancelRequested = False
    End Sub

    Public Sub UpdateProgress(currentIndex As Integer, totalCount As Integer, currentRechnung As Integer)
        If ProgressBar1.InvokeRequired Then
            ProgressBar1.Invoke(Sub() UpdateProgress(currentIndex, totalCount, currentRechnung))
        Else
            ProgressBar1.Value = currentIndex
            LabelCount.Text = $"{currentIndex} / {totalCount}"
            LabelStatus.Text = $"Exportiere Rechnung {currentRechnung}..."
        End If
    End Sub

    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        CancelRequested = True
        ButtonCancel.Enabled = False
        LabelStatus.Text = "Abbruch wird durchgeführt..."
    End Sub
End Class
