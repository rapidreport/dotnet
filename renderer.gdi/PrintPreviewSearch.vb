Public Class PrintPreviewSearch

    Public SearchPanel As PrintPreviewSearchPanel

    Public Sub Init(printPreview As IPrintPreviewSearch, searchPanel As PrintPreviewSearchPanel)
        Me.SearchPanel = searchPanel
        Me.SearchPanel.Init(printPreview)
        Me.SearchPanel.DoClose()
    End Sub

    Private Sub BtnSearch_Click(sender As System.Object, e As System.EventArgs) Handles BtnSearch.Click
        If Not Me.SearchPanel.Visible Then
            Me.PanelShow()
        Else
            Me.PanelHide()
        End If
    End Sub

    Public Sub PanelShow()
        If Me.SearchPanel IsNot Nothing Then
            Me.SearchPanel.DoOpen()
            Me.BtnSearch.BackColor = SystemColors.ControlDark
        End If
    End Sub

    Public Sub PanelHide()
        If Me.SearchPanel IsNot Nothing Then
            Me.SearchPanel.DoClose()
            Me.BtnSearch.BackColor = SystemColors.Control
        End If
    End Sub

End Class
