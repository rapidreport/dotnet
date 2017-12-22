Public Class FmPrintPreview

    Public PrintExecuted As Boolean = False

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Public Sub New(printer As Printer)
        Me.InitializeComponent()
        Me.PrintPreview.Printer = printer
    End Sub

    Private Sub FmPrintPreview_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Me.PrintPreview.Printer Is Nothing Then
            Exit Sub
        End If
        Using Me.PrintPreview.RenderBlock
            Me.PrintPreviewPage.Init(Me.PrintPreview)
            Me.PrintPreviewSearch.Init(Me.PrintPreview, Me.PrintPreviewSearchPanel)
            Me.PrintPreviewZoom.Init(Me.PrintPreview)
            Me.PrintPreviewMultiPage.Init(Me.PrintPreview)
            If Me.StartUpZoomFit Then
                Me.PrintPreview.AutoZoomFit = True
            ElseIf Me.StartUpZoomFitWidth Then
                Me.PrintPreview.AutoZoomFitWidth = True
            End If
        End Using
        If Report.Compatibility._4_32_PreviewMultiPageDisabled Then
            Me.MultiPageEnabled = False
        End If
        If Report.Compatibility._4_15_PreviewSearchDisabled Then
            Me.SearchEnabled = False
        End If
    End Sub

    Private Sub FmPrintPreview_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel
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

    Private Sub FmPrintPreview_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.P
                If e.Modifiers = Keys.Control Then
                    Me.Print()
                End If
            Case Keys.Escape
                If Me.PrintPreviewSearchPanel.Visible Then
                    Me.PrintPreviewSearch.PanelHide()
                Else
                    Me.Close()
                End If
            Case Else
                Me.PrintPreview.HandleKeyDownEvent(e)
        End Select
    End Sub

    Private Sub BtnPrint_Click(sender As Object, e As EventArgs) Handles BtnPrint.Click
        Me.Print()
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub

    Public Sub Print()
        Me.PrintPreview.Printer.CurrentPageCount = Me.PrintPreview.PageCount
        If Me.PrintPreview.Printer.PrintDialog.ShowDialog = DialogResult.OK Then
            Me.PrintPreview.Printer.PrintDocument.Print()
            Me.PrintExecuted = True
        End If
    End Sub

    Private _StartUpZoomFit As Boolean = False
    Public Property StartUpZoomFit As Boolean
        Set(value As Boolean)
            If value Then
                Me.StartUpZoomFitWidth = False
            End If
            Me._StartUpZoomFit = value
        End Set
        Get
            Return Me._StartUpZoomFit
        End Get
    End Property

    Private _StartUpZoomFitWidth As Boolean = False
    Public Property StartUpZoomFitWidth As Boolean
        Set(value As Boolean)
            If value Then
                Me.StartUpZoomFit = False
            End If
            Me._StartUpZoomFitWidth = value
        End Set
        Get
            Return Me._StartUpZoomFitWidth
        End Get
    End Property

    Private _MultiPageEnabled As Boolean = True
    Public Property MultiPageEnabled As Boolean
        Set(value As Boolean)
            Me._MultiPageEnabled = value
            If Me.MultiPageEnabled Then
                Me.PrintPreviewMultiPage.Show()
            Else
                Me.PrintPreviewMultiPage.Hide()
            End If
            Me._ReLayout()
        End Set
        Get
            Return Me._MultiPageEnabled
        End Get
    End Property

    Private _SearchEnabled As Boolean = True
    Public Property SearchEnabled As Boolean
        Set(value As Boolean)
            Me._SearchEnabled = value
            If Me.SearchEnabled Then
                Me.PrintPreviewSearch.Show()
            Else
                Me.PrintPreviewSearch.PanelHide()
                Me.PrintPreviewSearch.Hide()
            End If
            Me._ReLayout()
        End Set
        Get
            Return _SearchEnabled
        End Get
    End Property

    Private Sub _ReLayout()
        If Me.MultiPageEnabled Then
            With Me.PrintPreviewMultiPage
                Me.PrintPreviewZoom.Location = New Point(.Left + .Width + 6, 5)
            End With
        Else
            With Me.PrintPreviewPage
                Me.PrintPreviewZoom.Location = New Point(.Left + .Width + 6, 5)
            End With
        End If
        If Me.SearchEnabled Then
            With Me.PrintPreviewZoom
                Me.PrintPreviewSearch.Location = New Point(.Left + .Width + 6, 5)
            End With
            With Me.PrintPreviewSearch
                Me.BtnClose.Location = New Point(.Left + .Width + 6, 5)
            End With
        Else
            With Me.PrintPreviewZoom
                Me.BtnClose.Location = New Point(.Left + .Width + 6, 5)
            End With
        End If
    End Sub

End Class
