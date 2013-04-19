Public Class FmPrintPreview

    Public PrintExecuted As Boolean = False
    Public StartUpZoomFit As Boolean = False

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Public Sub New(ByVal printer As Printer)
        Me.InitializeComponent()
        Me.PrintPreview.Printer = printer
    End Sub

    Private Sub FmPrintPreview_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Using Me.PrintPreview.RenderBlock
            Me.PrintPreviewPage.Init(Me.PrintPreview)
            Me.PrintPreviewZoom.Init(Me.PrintPreview)
            If Me.StartUpZoomFit Then
                Me.PrintPreview.ZoomFit()
            End If
        End Using
    End Sub

    Private Sub FmPrintPreview_MouseWheel(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Me.MouseWheel
        Dim handled As Boolean = False
        If Me.ActiveControl Is Me.PrintPreviewPage Then
            handled = Me.PrintPreviewPage.HandleMouseWheelEvent(e)
        ElseIf Me.ActiveControl Is Me.PrintPreviewZoom Then
            handled = Me.PrintPreviewZoom.HandleMouseWheelEvent(e)
        End If
        If Not handled Then
            Me.PrintPreview.HandleMouseWheelEvent(e)
        End If
    End Sub

    Private Sub FmPrintPreview_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.P
                If e.Modifiers = Keys.Control Then
                    Me.Print()
                End If
            Case Keys.Escape
                Me.Close()
            Case Else
                Me.PrintPreview.HandleKeyDownEvent(e)
        End Select
    End Sub

    Private Sub BtnPrint_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnPrint.Click
        Me.Print()
    End Sub

    Private Sub BtnClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub

    Public Sub Print()
        If Me.PrintPreview.Printer.PrintDialog.ShowDialog = DialogResult.OK Then
            Me.PrintPreview.Printer.PrintDocument.Print()
            Me.PrintExecuted = True
        End If
    End Sub

End Class
