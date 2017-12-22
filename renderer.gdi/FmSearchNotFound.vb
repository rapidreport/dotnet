Public Class FmSearchNotFound

    Public Forward As Boolean
    Public Retry As Boolean = False

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(panel As PrintPreviewSearchPanel, forward As Boolean, defaultClose As Boolean)
        InitializeComponent()
        Me.Forward = forward
        If Me.Forward Then
            Me.LblMessage.Text = "最後まで検索しましたが、見つかりませんでした。"
            Me.BtnRetry.Text = "先頭から再検索"
        Else
            Me.LblMessage.Text = "先頭まで検索しましたが、見つかりませんでした。"
            Me.BtnRetry.Text = "最後から再検索"
        End If
        If defaultClose Then
            Me.BtnClose.TabIndex = 0
            Me.BtnRetry.TabIndex = 1
        Else
            Me.BtnRetry.TabIndex = 0
            Me.BtnClose.TabIndex = 1
        End If
    End Sub

    Private Sub BtnRetry_Click(sender As System.Object, e As System.EventArgs) Handles BtnRetry.Click
        Me.Retry = True
        Me.Close()
    End Sub

    Private Sub FmSearchNotFound_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Escape
                Me.Close()
        End Select
    End Sub

    Private Sub BtnClose_Click(sender As System.Object, e As System.EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub

End Class