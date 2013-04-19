Public Class PrintPreviewZoom

    Public WithEvents PrintPreview As IPrintPreviewZoom

    Public Sub Init(ByVal printPreview As IPrintPreviewZoom)
        Me.PrintPreview = printPreview
    End Sub

    Private Sub TxtZoom_Validated(ByVal sender As Object, ByVal e As EventArgs) Handles TxtZoom.Validated
        Me.zoom()
    End Sub

    Private Sub TxtZoom_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TxtZoom.KeyDown
        If e.KeyCode.Equals(System.Windows.Forms.Keys.Enter) Then
            Me.zoom()
        End If
    End Sub

    Private Sub BtnZoomIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnZoomIn.Click
        Me.PrintPreview.ZoomIn()
    End Sub

    Private Sub BtnZoomOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnZoomOut.Click
        Me.PrintPreview.ZoomOut()
    End Sub

    Private Sub BtnZoomFit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnZoomFit.Click
        Me.PrintPreview.ZoomFit()
    End Sub

    Private Sub PrintPreview_UpdateReport() Handles PrintPreview.UpdateReport
        Me.TxtZoom.Text = Fix(Me.PrintPreview.Zoom * 100)
    End Sub

    Private Sub zoom()
        If IsNumeric(Me.TxtZoom.Text) Then
            Dim d As Decimal = Me.TxtZoom.Text
            Me.PrintPreview.Zoom = d / 100.0
        End If
    End Sub

    Public Function HandleMouseWheelEvent(ByVal e As MouseEventArgs) As Boolean
        If Me.TxtZoom.Focused Then
            If e.Delta > 0 Then
                Me.PrintPreview.ZoomIn()
            Else
                Me.PrintPreview.ZoomOut()
            End If
            Return True
        Else
            Return False
        End If
    End Function

End Class