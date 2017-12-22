Public Class PrintPreviewPage

    Public WithEvents PrintPreview As IPrintPreviewPage

    Public Sub Init(printPreview As IPrintPreviewPage)
        Me.PrintPreview = printPreview
        Me._UpdateReport()
    End Sub

    Private _UpdateFlag As Boolean = False
    Private Sub _UpdateReport() Handles PrintPreview.UpdateReport
        Try
            Me._UpdateFlag = True
            Me.TxtPageTotal.Text = Me.PrintPreview.PageCountTotal
            Me.TxtPage.Text = Me.PrintPreview.PageCount
            Me.SlPage.MinValue = 1
            Me.SlPage.MaxValue = Me.PrintPreview.PageCountTotal
            Me.SlPage.ChangeUnit = Math.Max(1, Me.SlPage.MaxValue / 10)
            Me.SlPage.Value = Me.PrintPreview.PageCount
        Finally
            Me._UpdateFlag = False
        End Try
    End Sub

    Private Sub _SetUpPageCount()
        If IsNumeric(Me.TxtPage.Text) Then
            Me.PrintPreview.PageCount = CType(Me.TxtPage.Text, Integer)
        End If
    End Sub

    Private Sub TxtPage_Validated(sender As Object, e As System.EventArgs) Handles TxtPage.Validated
        Me._SetUpPageCount()
    End Sub

    Private Sub TxtPage_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtPage.KeyDown
        If e.KeyCode.Equals(Windows.Forms.Keys.Enter) Then
            Me._SetUpPageCount()
        End If
    End Sub

    Private Sub BtnPrev_Click(sender As System.Object, e As System.EventArgs) Handles BtnPrev.Click
        Me.PrintPreview.PrevPage()
    End Sub

    Private Sub BtnNext_Click(sender As System.Object, e As System.EventArgs) Handles BtnNext.Click
        Me.PrintPreview.NextPage()
    End Sub

    Private Sub BtnFirst_Click(sender As System.Object, e As System.EventArgs) Handles BtnFirst.Click
        Me.PrintPreview.FirstPage()
    End Sub

    Private Sub BtnLast_Click(sender As System.Object, e As System.EventArgs) Handles BtnLast.Click
        Me.PrintPreview.LastPage()
    End Sub

    Private Sub SlPage_ValueChanged(sender As Object) Handles SlPage.ValueChanged
        If Me._UpdateFlag Then
            Exit Sub
        End If
        Me.TxtPage.Text = Me.SlPage.Value
        Me._SetUpPageCount()
    End Sub

    Public Function HandleMouseWheelEvent(e As MouseEventArgs) As Boolean
        If Me.TxtPage.Focused Then
            If e.Delta > 0 Then
                Me.PrintPreview.NextPage()
            Else
                Me.PrintPreview.PrevPage()
            End If
            Return True
        End If
        Return False
    End Function

End Class