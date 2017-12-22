Public Class PrintPreviewMultiPage

    Public WithEvents PrintPreview As IPrintPreviewMultiPage

    Public Sub Init(printPreview As IPrintPreviewZoom)
        Me.PrintPreview = printPreview
        Me._UpdateReport()
    End Sub

    Private Sub _UpdateReport() Handles PrintPreview.UpdateReport
        Me.BtnMultiPage.BackColor =
            IIf(Me.PrintPreview.MultiPage, SystemColors.ControlDark, SystemColors.Control)
    End Sub

    Private Sub BtnMultiPage_Click(sender As Object, e As EventArgs) Handles BtnMultiPage.Click
        Me.PrintPreview.MultiPage = Not Me.PrintPreview.MultiPage
    End Sub

End Class
