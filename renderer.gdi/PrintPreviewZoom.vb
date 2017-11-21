﻿Public Class PrintPreviewZoom

    Public WithEvents PrintPreview As IPrintPreviewZoom
    Private _zoomTextChanged As Boolean = False

    Public Sub Init(ByVal printPreview As IPrintPreviewZoom)
        Me.PrintPreview = printPreview
        Me._UpdateReport()
    End Sub

    Private Sub _UpdateReport() Handles PrintPreview.UpdateReport
        Me.TxtZoom.Text = Fix(Me.PrintPreview.Zoom * 100)
        Me._zoomTextChanged = False
        If Me.PrintPreview.AutoZoomFit Then
            Me.BtnAutoZoomFit.BackColor = SystemColors.ControlDark
        Else
            Me.BtnAutoZoomFit.BackColor = SystemColors.Control
        End If
        If Me.PrintPreview.AutoZoomFitWidth Then
            Me.BtnAutoZoomFitWidth.BackColor = SystemColors.ControlDark
        Else
            Me.BtnAutoZoomFitWidth.BackColor = SystemColors.Control
        End If
    End Sub

    Private Sub _SetUpZoom()
        If IsNumeric(Me.TxtZoom.Text) Then
            Dim d As Decimal = Me.TxtZoom.Text
            Me.PrintPreview.Zoom = d / 100.0
        End If
    End Sub

    Private Sub TxtZoom_GotFocus(sender As Object, e As System.EventArgs) Handles TxtZoom.GotFocus
        Me._zoomTextChanged = False
    End Sub

    Private Sub TxtZoom_TextChanged(sender As Object, e As System.EventArgs) Handles TxtZoom.TextChanged
        Me._zoomTextChanged = True
    End Sub

    Private Sub TxtZoom_Validated(ByVal sender As Object, ByVal e As EventArgs) Handles TxtZoom.Validated
        If Me._zoomTextChanged Then
            Me._SetUpZoom()
        End If
    End Sub

    Private Sub TxtZoom_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TxtZoom.KeyDown
        If e.KeyCode.Equals(System.Windows.Forms.Keys.Enter) Then
            Me._SetUpZoom()
        End If
    End Sub

    Private Sub BtnZoomIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnZoomIn.Click
        Me.PrintPreview.ZoomIn()
    End Sub

    Private Sub BtnZoomOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnZoomOut.Click
        Me.PrintPreview.ZoomOut()
    End Sub

    Private Sub BtnAutoZoomFit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAutoZoomFit.Click
        Me.PrintPreview.AutoZoomFit = Not Me.PrintPreview.AutoZoomFit
    End Sub

    Private Sub BtnAutoZoomFitWidth_Click(sender As System.Object, e As System.EventArgs) Handles BtnAutoZoomFitWidth.Click
        Me.PrintPreview.AutoZoomFitWidth = Not Me.PrintPreview.AutoZoomFitWidth
    End Sub

    Public Function HandleMouseWheelEvent(ByVal e As MouseEventArgs) As Boolean
        If Me.TxtZoom.Focused Then
            If e.Delta > 0 Then
                Me.PrintPreview.ZoomIn()
            Else
                Me.PrintPreview.ZoomOut()
            End If
            Return True
        End If
        Return False
    End Function

End Class