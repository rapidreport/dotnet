Public Class PrintPreviewSearch

    Public WithEvents PrintPreview As IPrintPreviewSearch
    Public SearchPanel As PrintPreviewSearchPanel

    Public Sub Init(printPreview As IPrintPreviewSearch, searchPanel As PrintPreviewSearchPanel)
        Me.PrintPreview = printPreview
        Me.SearchPanel = searchPanel
        Me.SearchPanel.Visible = False
    End Sub

    Private Sub BtnSearch_Click(sender As System.Object, e As System.EventArgs) Handles BtnSearch.Click
        If Not Me.SearchPanel.Visible Then
            Me.PanelShow()
        Else
            Me.PanelHide()
        End If
    End Sub

    Public Sub PanelShow()
        Me.SearchPanel.Visible = True
        Me.SearchPanel.Focus()
        Me.BtnSearch.BackColor = SystemColors.ControlDark
    End Sub

    Public Sub PanelHide()
        Me.SearchPanel.Visible = False
        Me.BtnSearch.BackColor = SystemColors.Control
    End Sub

End Class
