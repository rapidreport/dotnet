﻿Imports System.Windows.Forms
Imports System.Drawing
Imports System.ComponentModel

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer

Public Class PrintPreview
    Inherits Control
    Implements IPrintPreviewPage
    Implements IPrintPreviewZoom

    Public Event UpdateReportPage() Implements IPrintPreviewPage.UpdateReport
    Public Event UpdateReportZoom() Implements IPrintPreviewZoom.UpdateReport
    Public Event Rendering(ByVal sender As Object, ByVal g As Graphics, ByRef cancel As Boolean)
    Public Event Rendered(ByVal sender As Object, ByVal g As Graphics)

    Public PageBuffer As Bitmap = Nothing
    Public WithEvents VScrollBar As New VScrollBar
    Public WithEvents HScrollBar As New HScrollBar

    Private _Printer As Printer = Nothing
    Private _Zoom As Decimal = 1D
    Private _PageCount As Integer = 1

    Private ZOOM_DEGREE() As Decimal = {0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.25, 1.5, 1.75, 2.0, 2.5, 3.0}
    Private Const MARGIN_VIEW As Integer = 8
    Private Const SCROLLBAR_WIDTH As Integer = 16
    Private Const ZOOM_MIN As Decimal = 0.3
    Private Const ZOOM_MAX As Decimal = 3.0

    Private _Gripping As Boolean = False
    Private _GripLocation As Point

    Public Class CRenderBlock
        Implements IDisposable
        Public PrintPreview As PrintPreview
        Public Sub New(ByVal printPreview As PrintPreview)
            Me.PrintPreview = printPreview
        End Sub
        Private disposedValue As Boolean = False
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                Me.PrintPreview._RenderBlock = Nothing
                Me.PrintPreview.Render()
            End If
            Me.disposedValue = True
        End Sub

#Region " IDisposable Support "
        ' このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

    Public Sub New()
        Me.BackColor = System.Drawing.SystemColors.ControlDarkDark
        With Me.VScrollBar
            .Width = SCROLLBAR_WIDTH
            .Visible = False
            .Cursor = Cursors.Arrow
        End With
        With Me.HScrollBar
            .Height = SCROLLBAR_WIDTH
            .Visible = False
            .Cursor = Cursors.Arrow
        End With
        Me.Controls.Add(Me.VScrollBar)
        Me.Controls.Add(Me.HScrollBar)
        Me.DoubleBuffered = True
    End Sub

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(False)> _
    Public Property Printer() As Printer
        Get
            Return Me._Printer
        End Get
        Set(ByVal value As Printer)
            Me._Printer = value
            If Me.PageCount > Me.GetPageCountTotal Then
                Me.PageCount = Me.GetPageCountTotal
            End If
        End Set
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(False)> _
    Public Property PageCount() As Integer Implements IPrintPreviewPage.PageCount
        Get
            Return Me._PageCount
        End Get
        Set(ByVal value As Integer)
            Using Me.RenderBlock
                Me._PageCount = value
                If Me._PageCount < 1 Then
                    Me._PageCount = 1
                End If
                If Me.Printer IsNot Nothing AndAlso Me._PageCount > Me.GetPageCountTotal Then
                    Me._PageCount = Me.GetPageCountTotal
                End If
            End Using
        End Set
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(False)> _
    Public Property Zoom() As Decimal Implements IPrintPreviewZoom.Zoom
        Get
            Return Me._Zoom
        End Get
        Set(ByVal value As Decimal)
            Using Me.RenderBlock
                Me._Zoom = value
                If Me._Zoom < ZOOM_MIN Then
                    Me._Zoom = ZOOM_MIN
                End If
                If Me._Zoom > ZOOM_MAX Then
                    Me._Zoom = ZOOM_MAX
                End If
            End Using
        End Set
    End Property

    Public Sub ZoomIn() Implements IPrintPreviewZoom.ZoomIn
        Dim i As Integer
        For i = 0 To ZOOM_DEGREE.Length - 2
            If ZOOM_DEGREE(i) > Me.Zoom Then
                Exit For
            End If
        Next
        Me.Zoom = ZOOM_DEGREE(i)
    End Sub

    Public Sub ZoomOut() Implements IPrintPreviewZoom.ZoomOut
        Dim i As Integer
        For i = ZOOM_DEGREE.Length - 1 To 1 Step -1
            If ZOOM_DEGREE(i) < Me.Zoom Then
                Exit For
            End If
        Next
        Me.Zoom = ZOOM_DEGREE(i)
    End Sub

    Public Sub ZoomFit() Implements IPrintPreviewZoom.ZoomFit
        Dim page As ReportPage = Me.Printer.Pages(Me.PageCount - 1)
        Dim paperSize As PaperSizeDesign = page.Report.Design.PaperDesign.GetActualSize.ToPoint(page.Report.Design.PaperDesign)
        Dim x As Decimal = (Me.Width - (MARGIN_VIEW * 2)) / Me.ToPixelX(paperSize.Width)
        Dim y As Decimal = (Me.Height - (MARGIN_VIEW * 2)) / Me.ToPixelY(paperSize.Height)
        If x < y Then
            Me.Zoom = x
        Else
            Me.Zoom = y
        End If
    End Sub

    Public Function GetPageCountTotal() As Integer Implements IPrintPreviewPage.GetPageCountTotal
        Return Me.Printer.Pages.Count
    End Function

    Private Sub CPrintPreview_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseClick
        Me.Focus()
    End Sub

    Private Sub CPrintPreview_Resize(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Resize
        If Me.ClientRectangle.Width > 0 And Me.ClientRectangle.Height > 0 Then
            Me.scrollBarUpdate()
        End If
    End Sub

    Public Sub ScrollOrPageChange(ByVal delta As Integer)
        If delta > 0 Then
            If Not Me.VScrollBar.Visible OrElse _
               Me.VScrollBar.Value = 0 AndAlso Me.PageCount > 1 Then
                Me.PageCount -= 1
            Else
                Dim v As Integer = Me.VScrollBar.Value - Me.VScrollBar.SmallChange
                If v < 0 Then
                    v = 0
                End If
                Me.VScrollBar.Value = v
            End If
        Else
            Dim max As Integer = Me.VScrollBar.Maximum - Me.VScrollBar.LargeChange
            If Not Me.VScrollBar.Visible OrElse _
               Me.VScrollBar.Value = max AndAlso Me.PageCount < Me.Printer.Pages.Count Then
                Me.PageCount += 1
            Else
                Dim v As Integer = Me.VScrollBar.Value + Me.VScrollBar.SmallChange
                If v > max Then
                    v = max
                End If
                Me.VScrollBar.Value = v
            End If
        End If
    End Sub

    Private _RenderBlock As CRenderBlock = Nothing
    Public Function RenderBlock() As CRenderBlock
        If Me._RenderBlock Is Nothing Then
            Me._RenderBlock = New CRenderBlock(Me)
            Return Me._RenderBlock
        Else
            Return Nothing
        End If
    End Function

    Private Sub scrollBar_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles VScrollBar.ValueChanged, HScrollBar.ValueChanged
        Me.Invalidate()
    End Sub

    Public Sub Render()
        If Me.Printer Is Nothing Then
            Return
        End If
        Dim page As ReportPage = Me.Printer.Pages(Me.PageCount - 1)
        Dim pageSize As PaperSizeDesign = page.Report.Design.PaperDesign.GetActualSize.ToPoint(page.Report.Design.PaperDesign)
        Me.PageBuffer = New Bitmap(CType(Me.ToPixelX(pageSize.Width) * Me.Zoom, Integer), _
                                   CType(Me.ToPixelY(pageSize.Height) * Me.Zoom, Integer))
        Dim g As Graphics = Graphics.FromImage(Me.PageBuffer)
        g.PageUnit = GraphicsUnit.Point
        g.ScaleTransform(Me.Zoom, Me.Zoom)
        g.FillRectangle(Brushes.White, 0, 0, pageSize.Width, pageSize.Height)
        With Nothing
            Dim m As PaperMarginDesign = page.Report.Design.PaperDesign.Margin.ToPoint(page.Report.Design.PaperDesign)
            If ((Me.PageCount - 1) Mod 2) And m.OddReverse Then
                g.TranslateTransform(m.Right, m.Top)
            Else
                g.TranslateTransform(m.Left, m.Top)
            End If
        End With
        Dim cancel As Boolean = False
        RaiseEvent Rendering(Me, g, cancel)
        If Not cancel Then
            Dim r As New GdiRenderer(New RenderingEnv(g, Me.Printer))
            page.Render(r, Me.Printer.Pages)
            Me.VScrollBar.Value = 0
            Me.HScrollBar.Value = 0
            Me.scrollBarUpdate()
            RaiseEvent Rendered(Me, g)
        End If
    End Sub

    Private Sub scrollBarUpdate()
        Me.VScrollBar.Visible = False
        Me.HScrollBar.Visible = False
        If Me.PageBuffer Is Nothing Then
            Return
        End If
        Dim w As Integer = Me.PageBuffer.Width + MARGIN_VIEW * 2
        Dim h As Integer = Me.PageBuffer.Height + MARGIN_VIEW * 2
        If w > Me.Width Then
            Me.HScrollBar.Visible = True
        End If
        If h > Me.Height Then
            Me.VScrollBar.Visible = True
        End If
        If Me.VScrollBar.Visible Then
            w += SCROLLBAR_WIDTH
            If w > Me.Width Then
                Me.HScrollBar.Visible = True
            End If
        End If
        If Me.HScrollBar.Visible Then
            h += SCROLLBAR_WIDTH
            If h > Me.Height Then
                Me.VScrollBar.Visible = True
            End If
        End If
        Dim t As Integer = 0
        If Me.VScrollBar.Visible AndAlso Me.HScrollBar.Visible Then
            t = SCROLLBAR_WIDTH
        End If
        With Me.VScrollBar
            If .Visible Then
                .Location = New Point(Me.Width - SCROLLBAR_WIDTH, 0)
                .Height = Me.Height - t
                .Minimum = 0
                .Maximum = h
                .LargeChange = Me.Height
                .SmallChange = .LargeChange / 10 + 10
            Else
                .Minimum = 0
                .Maximum = 0
                .LargeChange = 0
                .SmallChange = 0
            End If
        End With
        With Me.HScrollBar
            If .Visible Then
                .Location = New Point(0, Me.Height - SCROLLBAR_WIDTH)
                .Width = Me.Width - t
                .Minimum = 0
                .Maximum = w
                .LargeChange = Me.Width
                .SmallChange = .LargeChange / 10 + 10
            Else
                .Minimum = 0
                .Maximum = 0
                .LargeChange = 0
                .SmallChange = 0
            End If
        End With
        If Me.VScrollBar.Visible Or Me.HScrollBar.Visible Then
            Me.Cursor = New Cursor(My.Resources.hand.Handle)
            Me._Gripping = False
        Else
            Me.Cursor = Cursors.Default
        End If
        Me.Invalidate()
    End Sub

    Private Sub viewUpdate(ByVal g As Graphics)
        If Me.PageBuffer Is Nothing Then
            Exit Sub
        End If
        Using b As New SolidBrush(Color.DimGray)
            g.FillRectangle(b, 0, 0, Me.Width, Me.Height)
        End Using
        Dim x As Integer = (Me.Width - Me.PageBuffer.Width) / 2
        Dim y As Integer = (Me.Height - Me.PageBuffer.Height) / 2
        If x < MARGIN_VIEW Then
            x = MARGIN_VIEW
        End If
        If y < MARGIN_VIEW Then
            y = MARGIN_VIEW
        End If
        x -= Me.HScrollBar.Value
        y -= Me.VScrollBar.Value
        g.DrawImage(Me.PageBuffer, x, y)
        Using p As New Pen(Color.Black)
            g.DrawRectangle(p, x - 1, y - 1, Me.PageBuffer.Width + 1, Me.PageBuffer.Height + 1)
            g.DrawRectangle(p, 0, 0, Me.Width - 1, Me.Height - 1)
        End Using
        If Me.VScrollBar.Visible AndAlso Me.HScrollBar.Visible Then
            Using b As New SolidBrush(System.Drawing.SystemColors.Control)
                g.FillRectangle(b, Me.Width - SCROLLBAR_WIDTH, Me.Height - SCROLLBAR_WIDTH, SCROLLBAR_WIDTH, SCROLLBAR_WIDTH)
            End Using
        End If
    End Sub

    Private Sub CPrintPreview_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint
        If Not Me.DesignMode Then
            Me.viewUpdate(e.Graphics)
            RaiseEvent UpdateReportPage()
            RaiseEvent UpdateReportZoom()
        End If
    End Sub

    Public Function ToPixelX(ByVal v As Decimal) As Integer
        Return GdiRenderUtil.ToPixelX(Me.CreateGraphics, v)
    End Function

    Public Function ToPixelY(ByVal v As Decimal) As Integer
        Return GdiRenderUtil.ToPixelY(Me.CreateGraphics, v)
    End Function

    Public Function ToPointX(ByVal v As Integer) As Decimal
        Return GdiRenderUtil.ToPointX(Me.CreateGraphics, v)
    End Function

    Public Function ToPointY(ByVal v As Integer) As Decimal
        Return GdiRenderUtil.ToPointY(Me.CreateGraphics, v)
    End Function

    Private Sub UPrintPreview_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        If Me.HScrollBar.Visible Or Me.VScrollBar.Visible Then
            Me._Gripping = True
            Me.Cursor = New Cursor(My.Resources.hand_grip.Handle)
            Me._GripLocation = e.Location
        End If
    End Sub

    Private Sub UPrintPreview_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        If Me._Gripping Then
            Me._Gripping = False
            Me.Cursor = New Cursor(My.Resources.hand.Handle)
        End If
    End Sub

    Private Sub UPrintPreview_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        If Me._Gripping Then
            Dim dx As Integer = Me._GripLocation.X - e.Location.X
            Dim dy As Integer = Me._GripLocation.Y - e.Location.Y
            If Me.HScrollBar.Visible Then
                Me.HScrollBar.Value = Math.Min(Math.Max(Me.HScrollBar.Value + dx, 0), _
                                               Me.HScrollBar.Maximum - Me.HScrollBar.LargeChange)
            End If
            If Me.VScrollBar.Visible Then
                Me.VScrollBar.Value = Math.Min(Math.Max(Me.VScrollBar.Value + dy, 0), _
                                               Me.VScrollBar.Maximum - Me.VScrollBar.LargeChange)
            End If
            Me._GripLocation = e.Location
        End If
    End Sub

    Public Sub HandleMouseWheelEvent(ByVal e As MouseEventArgs)
        If ModifierKeys = Keys.Control Then
            If e.Delta > 0 Then
                Me.ZoomIn()
            Else
                Me.ZoomOut()
            End If
        Else
            Me.ScrollOrPageChange(e.Delta)
        End If
    End Sub

    Public Sub HandleKeyDownEvent(ByVal e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.PageDown
                Me.PageCount += 1
            Case Keys.PageUp
                Me.PageCount -= 1
            Case Keys.Home
                Me.PageCount = 1
            Case Keys.End
                Me.PageCount = Me.GetPageCountTotal
        End Select
    End Sub

End Class