Public Class FmSearchNotFound

    Public Keyword As String
    Public Forward As Boolean

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(ByVal keyword As String, ByVal forward As Boolean)
        InitializeComponent()
        Me.Keyword = keyword
        Me.Forward = forward
        Me.LblKeyword.Text = Me.Keyword
        If Me.Forward Then
            Me.LblMessage.Text = "を、最後まで検索しましたが、見つかりませんでした。"
            Me.BtnRetry.Text = "先頭から検索"
        Else
            Me.LblMessage.Text = "を、先頭まで検索しましたが、見つかりませんでした。"
            Me.BtnRetry.Text = "最後から検索"
        End If
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