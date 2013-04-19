Public Class PrintPreviewPage

    Public WithEvents PrintPreview As IPrintPreviewPage
    Private initializing As Boolean = False

    Public Sub Init(ByVal printPreview As IPrintPreviewPage)
        Me.initializing = True
        Try
            Me.PrintPreview = printPreview
            Me._UpdateReport()
        Finally
            Me.initializing = False
        End Try
        Me.setPage()
    End Sub

    Private _UpdateFlag As Boolean = False
    Private Sub _UpdateReport() Handles PrintPreview.UpdateReport
        Try
            Me._UpdateFlag = True
            Me.TxtPageTotal.Text = Me.PrintPreview.GetPageCountTotal
            Me.TxtPage.Text = Me.PrintPreview.PageCount
            Me.SlPage.MinValue = 1
            Me.SlPage.MaxValue = Me.PrintPreview.GetPageCountTotal
            Me.SlPage.ChangeUnit = Math.Max(1, Me.SlPage.MaxValue / 10)
            Me.SlPage.Value = Me.PrintPreview.PageCount
        Finally
            Me._UpdateFlag = False
        End Try
    End Sub

    Private Sub TxtPage_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles TxtPage.Validated
        Me.setPage()
    End Sub

    Private Sub TxtPage_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TxtPage.KeyDown
        If e.KeyCode.Equals(Windows.Forms.Keys.Enter) Then
            Me.setPage()
        End If
    End Sub

    Private Sub BtnPrev_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPrev.Click
        Me.prevPage()
    End Sub

    Private Sub BtnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNext.Click
        Me.nextPage()
    End Sub

    Private Sub BtnBegin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnBegin.Click
        Me.PrintPreview.PageCount = 1
    End Sub

    Private Sub BtnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnEnd.Click
        Me.PrintPreview.PageCount = Me.PrintPreview.GetPageCountTotal
    End Sub

    Private Sub prevPage()
        Dim i As Integer = Me.PrintPreview.PageCount
        If i > 1 Then
            Me.PrintPreview.PageCount = i - 1
        End If
    End Sub

    Private Sub nextPage()
        Dim i As Integer = Me.PrintPreview.PageCount
        If i < Me.PrintPreview.GetPageCountTotal Then
            Me.PrintPreview.PageCount = i + 1
        End If
    End Sub

    Private Sub setPage()
        If Me.initializing Then
            Exit Sub
        End If
        If IsNumeric(Me.TxtPage.Text) Then
            Dim i As Integer = Me.TxtPage.Text
            Me.PrintPreview.PageCount = i
        End If
    End Sub

    Private Sub SlPage_ValueChanged(ByVal sender As Object) Handles SlPage.ValueChanged
        If Me._UpdateFlag Then
            Exit Sub
        End If
        Me.TxtPage.Text = Me.SlPage.Value
        Me.setPage()
    End Sub

    Public Function HandleMouseWheelEvent(ByVal e As MouseEventArgs) As Boolean
        If Me.TxtPage.Focused Then
            If e.Delta > 0 Then
                Me.nextPage()
            Else
                Me.prevPage()
            End If
            Return True
        Else
            Return False
        End If
    End Function

End Class