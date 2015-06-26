Public Class PrintPreviewSearch

    Public SearchPanel As PrintPreviewSearchPanel

    Public Sub Init(pages As ReportPages, printPreview As IPrintPreviewSearch, searchPanel As PrintPreviewSearchPanel)
        Me.SearchPanel = searchPanel
        Me.SearchPanel.Init(pages, printPreview)
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
        Me.SearchPanel.DoOpen()
        Me.BtnSearch.BackColor = SystemColors.ControlDark
    End Sub

    Public Sub PanelHide()
        Me.SearchPanel.DoClose()
        Me.BtnSearch.BackColor = SystemColors.Control
    End Sub

End Class
