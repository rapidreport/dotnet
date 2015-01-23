Public Class PrintPreviewZoom

    Public WithEvents PrintPreview As IPrintPreviewZoom

    Public Sub Init(ByVal printPreview As IPrintPreviewZoom)
        Me.PrintPreview = printPreview
    End Sub

    Private _zoomTextChanged As Boolean = False

    Private Sub TxtZoom_GotFocus(sender As Object, e As System.EventArgs) Handles TxtZoom.GotFocus
        Me._zoomTextChanged = False
    End Sub

    Private Sub TxtZoom_TextChanged(sender As Object, e As System.EventArgs) Handles TxtZoom.TextChanged
        Me._zoomTextChanged = True
    End Sub

    Private Sub TxtZoom_Validated(ByVal sender As Object, ByVal e As EventArgs) Handles TxtZoom.Validated
        If Me._zoomTextChanged Then
            Me.zoom()
        End If
    End Sub

    Private Sub TxtZoom_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TxtZoom.KeyDown
        If e.KeyCode.Equals(System.Windows.Forms.Keys.Enter) Then
            Me.zoom()
        End If
    End Sub

    Private Sub BtnZoomIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnZoomIn.Click
        Me.PrintPreview.ZoomIn()
        Me.AutoFit = False
        Me.AutoFitWidth = False
    End Sub

    Private Sub BtnZoomOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnZoomOut.Click
        Me.PrintPreview.ZoomOut()
        Me.AutoFit = False
        Me.AutoFitWidth = False
    End Sub

    Private Sub BtnZoomFit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnZoomFit.Click
        Me.PrintPreview.ZoomFit()
        Me.AutoFit = Not Me.AutoFit
    End Sub

    Private Sub BtnZoomFitWidth_Click(sender As System.Object, e As System.EventArgs) Handles BtnZoomFitWidth.Click
        Me.PrintPreview.ZoomFitWidth()
        Me.AutoFitWidth = Not Me.AutoFitWidth
    End Sub

    Private Sub PrintPreview_UpdateReport() Handles PrintPreview.UpdateReport
        Me.TxtZoom.Text = Fix(Me.PrintPreview.Zoom * 100)
    End Sub

    Private Sub PrintPreview_Resize() Handles PrintPreview.Resize
        If Me.AutoFit Then
            Me.PrintPreview.ZoomFit()
        ElseIf Me.AutoFitWidth Then
            Me.PrintPreview.ZoomFitWidth()
        End If
    End Sub

    Private Sub PrintPreview_ZoomInOrOut(zoomIn As Boolean) Handles PrintPreview.ZoomInOrOut
        If zoomIn Then
            Me.PrintPreview.ZoomIn()
        Else
            Me.PrintPreview.ZoomOut()
        End If
        Me.AutoFit = False
        Me.AutoFitWidth = False
    End Sub

    Private Sub zoom()
        If IsNumeric(Me.TxtZoom.Text) Then
            Dim d As Decimal = Me.TxtZoom.Text
            Me.PrintPreview.Zoom = d / 100.0
            Me.AutoFit = False
            Me.AutoFitWidth = False
        End If
    End Sub

    Public Function HandleMouseWheelEvent(ByVal e As MouseEventArgs) As Boolean
        If Me.TxtZoom.Focused Then
            If e.Delta > 0 Then
                Me.PrintPreview.ZoomIn()
            Else
                Me.PrintPreview.ZoomOut()
            End If
            Me.AutoFit = False
            Me.AutoFitWidth = False
            Return True
        Else
            Return False
        End If
    End Function

    Private _AutoFit As Boolean = False
    Public Property AutoFit As Boolean
        Set(value As Boolean)
            If value Then
                Me.AutoFitWidth = False
            End If
            Me._AutoFit = value
            If Me.AutoFit Then
                Me.BtnZoomFit.BackColor = SystemColors.ControlDark
            Else
                Me.BtnZoomFit.BackColor = SystemColors.Control
            End If
        End Set
        Get
            Return Me._AutoFit
        End Get
    End Property

    Private _AutoFitWidth As Boolean = False
    Public Property AutoFitWidth As Boolean
        Set(value As Boolean)
            If value Then
                Me.AutoFit = False
            End If
            Me._AutoFitWidth = value
            If Me.AutoFitWidth Then
                Me.BtnZoomFitWidth.BackColor = SystemColors.ControlDark
            Else
                Me.BtnZoomFitWidth.BackColor = SystemColors.Control
            End If
        End Set
        Get
            Return Me._AutoFitWidth
        End Get
    End Property

End Class