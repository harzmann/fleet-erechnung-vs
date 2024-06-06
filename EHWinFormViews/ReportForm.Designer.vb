<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ReportForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.StiViewerControl1 = New Stimulsoft.Report.Viewer.StiViewerControl()
        Me.SuspendLayout()
        '
        'StiViewerControl1
        '
        Me.StiViewerControl1.AllowDrop = True
        Me.StiViewerControl1.Location = New System.Drawing.Point(13, 13)
        Me.StiViewerControl1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.StiViewerControl1.Name = "StiViewerControl1"
        Me.StiViewerControl1.Report = Nothing
        Me.StiViewerControl1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.StiViewerControl1.ShowZoom = True
        Me.StiViewerControl1.Size = New System.Drawing.Size(892, 545)
        Me.StiViewerControl1.TabIndex = 0
        '
        'ReportForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(915, 575)
        Me.Controls.Add(Me.StiViewerControl1)
        Me.Name = "ReportForm"
        Me.Text = "ReportForm"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents StiViewerControl1 As Stimulsoft.Report.Viewer.StiViewerControl
End Class
