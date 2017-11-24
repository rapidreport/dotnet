Imports System.Windows.Forms
Imports System.Drawing
Imports System.ComponentModel

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.gdi

Public Class PrintPreview
    Inherits Control
    Implements IPrintPreviewPage
    Implements IPrintPreviewZoom
    Implements IPrintPreviewSearch
    Implements IPrintPreviewMultiPage

    Public Event UpdateReportPage() Implements IPrintPreviewPage.UpdateReport
    Public Event UpdateReportZoom() Implements IPrintPreviewZoom.UpdateReport
    Public Event UpdateReportMultiPage() Implements IPrintPreviewMultiPage.UpdateReport
    Public Event Rendering(ByVal sender As Object, ByVal g As Graphics, ByRef cancel As Boolean)
    Public Event Rendered(ByVal sender As Object, ByVal g As Graphics)
    Public PageBuffers As List(Of Bitmap) = Nothing
    Public WithEvents VScrollBar As New VScrollBar
    Public WithEvents HScrollBar As New HScrollBar

    Private _Printer As Printer = Nothing
    Private _Zoom As Decimal = 1D
    Private _PageCount As Integer = 1

    Private ZOOM_DEGREE() As Decimal = {0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.25, 1.5, 1.75, 2.0, 2.5, 3.0}
    Private Const MULTI_PAGE_COUNT As Integer = 2
    Private Const MARGIN_VIEW As Integer = 8
    Private Const BORDER_VIEW As Integer = 2
    Private Const SCROLLBAR_WIDTH As Integer = 16
    Private Const ZOOM_MIN As Decimal = 0.3
    Private Const ZOOM_MAX As Decimal = 3.0
    Private Const BUFFERSIZE_MAX As Decimal = 5000

    Private _Gripping As Boolean = False
    Private _GripLocation As Point

    Private _ReportMargin As SizeF

    Private _FocusPageIndex As Integer = -1
    Private _FocusRegion As component.Region

    Public Class RenderBlockClass
        Implements IDisposable

        Public ScrollPreserve As Boolean
        Public PrintPreview As PrintPreview

        Private _ScrollV As Integer
        Private _ScrollH As Integer

        Public Sub New(ByVal printPreview As PrintPreview, scrollPreserve As Boolean)
            Me.PrintPreview = printPreview
            Me.ScrollPreserve = scrollPreserve
            If Me.ScrollPreserve Then
                Me._ScrollV = Me.PrintPreview.VScrollBar.Value
                Me._ScrollH = Me.PrintPreview.HScrollBar.Value
            End If
        End Sub

        Private disposedValue As Boolean = False
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                Me.PrintPreview._RenderBlock = Nothing
                Me.PrintPreview.Render()
                If Me.ScrollPreserve Then
                    Me.PrintPreview._SetScrollValue(Me.PrintPreview.VScrollBar, _ScrollV)
                    Me.PrintPreview._SetScrollValue(Me.PrintPreview.HScrollBar, _ScrollH)
                End If
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

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(False)>
    Public Property Printer() As Printer
        Get
            Return Me._Printer
        End Get
        Set(ByVal value As Printer)
            Me._Printer = value
            If Me.PageCount > Me.PageCountTotal Then
                Me.PageCount = Me.PageCountTotal
            End If
        End Set
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(False)>
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
                If Me.Printer IsNot Nothing AndAlso Me._PageCount > Me.PageCountTotal Then
                    Me._PageCount = Me.PageCountTotal
                End If
                Me._UpdatePageZoomStatus()
            End Using
        End Set
    End Property

    Public Sub PrevPage() Implements IPrintPreviewPage.PrevPage
        Me.PageCount -= IIf(Me.MultiPage, MULTI_PAGE_COUNT, 1)
    End Sub

    Public Sub NextPage() Implements IPrintPreviewPage.NextPage
        Dim c As Integer = IIf(Me.MultiPage, MULTI_PAGE_COUNT, 1)
        If Me.PageCount + c - 1 < Me.PageCountTotal Then
            Me.PageCount += c
        End If
    End Sub

    Public Sub FirstPage() Implements IPrintPreviewPage.FirstPage
        Me.PageCount = 1
    End Sub

    Public Sub LastPage() Implements IPrintPreviewPage.LastPage
        Me.PageCount = Me.PageCountTotal - (IIf(Me.MultiPage, MULTI_PAGE_COUNT, 1) - 1)
    End Sub

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(False)>
    Public Property Zoom() As Decimal Implements IPrintPreviewZoom.Zoom
        Get
            Return Me._Zoom
        End Get
        Set(ByVal value As Decimal)
            Me.AutoZoomFit = False
            Me.AutoZoomFitWidth = False
            Me.MultiPage = False
            Me._UpdatePageZoomStatus()
            Me._SetZoom(value)
        End Set
    End Property

    Private Sub _SetZoom(zoom As Decimal)
        Using Me.RenderBlock
            Me._Zoom = zoom
            If Me._Zoom < ZOOM_MIN Then
                Me._Zoom = ZOOM_MIN
            End If
            If Me._Zoom > ZOOM_MAX Then
                Me._Zoom = ZOOM_MAX
            End If
        End Using
    End Sub

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

    Public Sub ZoomFit()
        Dim page As ReportPage = Me.Printer.Pages(Me.PageCount - 1)
        Dim paperSize As PaperSizeDesign = page.Report.Design.PaperDesign.GetActualSize.ToPoint(page.Report.Design.PaperDesign)
        Me._SetZoom(Math.Min((Me.Width - (MARGIN_VIEW * 2)) / Me.ToPixelX(paperSize.Width),
                             (Me.Height - (MARGIN_VIEW * 2)) / Me.ToPixelY(paperSize.Height)))
    End Sub

    Public Sub ZoomFitWidth()
        Dim page As ReportPage = Me.Printer.Pages(Me.PageCount - 1)
        Dim paperSize As PaperSizeDesign = page.Report.Design.PaperDesign.GetActualSize.ToPoint(page.Report.Design.PaperDesign)
        Dim z1 As Decimal = Math.Min((Me.Width - (MARGIN_VIEW * 2)) / Me.ToPixelX(paperSize.Width),
                                     (Me.Height - (MARGIN_VIEW * 2)) / Me.ToPixelY(paperSize.Height))
        Dim z2 As Decimal = (Me.Width - (MARGIN_VIEW * 3) - SCROLLBAR_WIDTH) / Me.ToPixelX(paperSize.Width)
        Me._SetZoom(Math.Max(z1, z2))
    End Sub

    Public Sub ZoomMultiPage()
        Dim w As Integer
        Dim h As Integer
        With Nothing
            Dim page As ReportPage = Me.Printer.Pages(Me.PageCount - 1)
            Dim paperSize As PaperSizeDesign = page.Report.Design.PaperDesign.GetActualSize.ToPoint(page.Report.Design.PaperDesign)
            w = Me.ToPixelX(paperSize.Width)
            h = Me.ToPixelY(paperSize.Height)
        End With
        If Me.PageCount < Me.PageCountTotal - 1 Then
            Dim page As ReportPage = Me.Printer.Pages(Me.PageCount)
            Dim paperSize As PaperSizeDesign = page.Report.Design.PaperDesign.GetActualSize.ToPoint(page.Report.Design.PaperDesign)
            w += BORDER_VIEW + Me.ToPixelX(paperSize.Width)
            h = Math.Max(h, Me.ToPixelY(paperSize.Height))
        Else
            w += BORDER_VIEW + w
        End If
        Me._SetZoom(Math.Min((Me.Width - SCROLLBAR_WIDTH - MARGIN_VIEW) / w,
                             (Me.Height - (MARGIN_VIEW * 2)) / h))
    End Sub

    Private _AutoZoomFit As Boolean = False
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(False)>
    Public Property AutoZoomFit As Boolean Implements IPrintPreviewZoom.AutoZoomFit
        Set(value As Boolean)
            If value Then
                Me.AutoZoomFitWidth = False
                Me.MultiPage = False
            End If
            Me._AutoZoomFit = value
            Me._UpdatePageZoomStatus()
        End Set
        Get
            Return Me._AutoZoomFit
        End Get
    End Property

    Private _AutoZoomFitWidth As Boolean = False
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(False)>
    Public Property AutoZoomFitWidth As Boolean Implements IPrintPreviewZoom.AutoZoomFitWidth
        Set(value As Boolean)
            If value Then
                Me.AutoZoomFit = False
                Me.MultiPage = False
            End If
            Me._AutoZoomFitWidth = value
            Me._UpdatePageZoomStatus()
        End Set
        Get
            Return Me._AutoZoomFitWidth
        End Get
    End Property

    Private _MultiPage As Boolean = False
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(False)>
    Public Property MultiPage As Boolean Implements IPrintPreviewMultiPage.MultiPage
        Set(value As Boolean)
            If value Then
                Me.AutoZoomFit = False
                Me.AutoZoomFitWidth = False
            End If
            Me._MultiPage = value
            Me._UpdatePageZoomStatus()
        End Set
        Get
            Return Me._MultiPage
        End Get
    End Property

    Public Function PageCountTotal() As Integer Implements IPrintPreviewPage.PageCountTotal
        Return Me.Printer.Pages.Count
    End Function

    Private Sub PrintPreview_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseClick
        Me.Focus()
    End Sub

    Private Sub PrintPreview_Resize(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Resize
        Me._UpdatePageZoomStatus()
    End Sub

    Private Sub _UpdatePageZoomStatus()
        Dim rendered As Boolean = False
        If Me.AutoZoomFit Then
            Me.ZoomFit()
            rendered = True
        ElseIf Me.AutoZoomFitWidth Then
            Me.ZoomFitWidth()
            rendered = True
        ElseIf Me.MultiPage Then
            Me.ZoomMultiPage()
            rendered = True
        End If
        Me._ScrollBarUpdate()
        If Not rendered Then
            Me._RaiseUpdateReport()
        End If
    End Sub

    Private Sub _RaiseUpdateReport()
        RaiseEvent UpdateReportPage()
        RaiseEvent UpdateReportZoom()
        RaiseEvent UpdateReportMultiPage()
    End Sub

    Public Sub ScrollOrPageChange(ByVal delta As Integer)
        If delta > 0 Then
            If Not Me.VScrollBar.Visible OrElse Me.VScrollBar.Value = 0 Then
                Me.PrevPage()
            Else
                Dim v As Integer = Me.VScrollBar.Value - Me.VScrollBar.SmallChange
                If v < 0 Then
                    v = 0
                End If
                Me.VScrollBar.Value = v
            End If
        Else
            Dim max As Integer = Me.VScrollBar.Maximum - Me.VScrollBar.LargeChange
            If Not Me.VScrollBar.Visible OrElse Me.VScrollBar.Value = max Then
                Me.NextPage()
            Else
                Dim v As Integer = Me.VScrollBar.Value + Me.VScrollBar.SmallChange
                If v > max Then
                    v = max
                End If
                Me.VScrollBar.Value = v
            End If
        End If
    End Sub

    Private _RenderBlock As RenderBlockClass = Nothing
    Public Function RenderBlock() As RenderBlockClass
        Return RenderBlock(False)
    End Function

    Public Function RenderBlock(scrollPreserve As Boolean) As RenderBlockClass
        If Me._RenderBlock Is Nothing Then
            Me._RenderBlock = New RenderBlockClass(Me, scrollPreserve)
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

        Me._ClearPageBuffer()

        For i As Integer = 0 To IIf(Me.MultiPage, MULTI_PAGE_COUNT, 1) - 1
            If Me.PageCount + i > Me.PageCountTotal Then
                Exit For
            End If
            Dim page As ReportPage = Me.Printer.Pages(Me.PageCount + i - 1)
            Dim pageSize As PaperSizeDesign = page.Report.Design.PaperDesign.GetActualSize.ToPoint(page.Report.Design.PaperDesign)
            Dim zoom As Decimal = Me.Zoom
            Dim buf As Bitmap
            With Nothing
                Dim w As Integer = Me.ToPixelX(pageSize.Width)
                Dim h As Integer = Me.ToPixelY(pageSize.Height)
                zoom = Math.Min(zoom, BUFFERSIZE_MAX / w)
                zoom = Math.Min(zoom, BUFFERSIZE_MAX / h)
                buf = New Bitmap(CType(w * zoom, Integer), CType(h * zoom, Integer))
            End With

            Using g As Graphics = Graphics.FromImage(buf)
                g.PageUnit = GraphicsUnit.Point
                g.ScaleTransform(zoom, zoom)
                g.FillRectangle(Brushes.White, 0, 0, pageSize.Width, pageSize.Height)
                If Me.Printer.PreviewBackgroundImage IsNot Nothing Then

                    Dim scale As Single = Math.Min((pageSize.Width / Me.Printer.PreviewBackgroundImage.Width) * Me.Printer.PreviewBackgroundSetting.Scale,
                                               (pageSize.Height / Me.Printer.PreviewBackgroundImage.Height) * Me.Printer.PreviewBackgroundSetting.Scale)

                    Dim cm As New Imaging.ColorMatrix()
                    cm.Matrix00 = 1
                    cm.Matrix11 = 1
                    cm.Matrix22 = 1
                    cm.Matrix33 = Me.Printer.PreviewBackgroundSetting.Alpha
                    cm.Matrix44 = 1
                    Dim ia As New Imaging.ImageAttributes()
                    ia.SetColorMatrix(cm)
                    g.DrawImage(Me.Printer.PreviewBackgroundImage, New Rectangle(Me.Printer.PreviewBackgroundSetting.X, Me.Printer.PreviewBackgroundSetting.Y,
                                                                             Me.Printer.PreviewBackgroundImage.Width * scale,
                                                                             Me.Printer.PreviewBackgroundImage.Height * scale),
                            0, 0, Me.Printer.PreviewBackgroundImage.Width, Me.Printer.PreviewBackgroundImage.Height, GraphicsUnit.Pixel, ia)
                End If
                With Nothing
                    Dim m As PaperMarginDesign = page.Report.Design.PaperDesign.Margin.ToPoint(page.Report.Design.PaperDesign)
                    If ((Me.PageCount - 1) Mod 2) And m.OddReverse Then
                        Me._ReportMargin = New SizeF(m.Right, m.Top)
                    Else
                        Me._ReportMargin = New SizeF(m.Left, m.Top)
                    End If
                    g.TranslateTransform(Me._ReportMargin.Width, Me._ReportMargin.Height)
                End With
                Dim cancel As Boolean = False
                RaiseEvent Rendering(Me, g, cancel)
                If Not cancel Then
                    Dim r As New GdiRenderer(New RenderingEnv(g, Me.Printer))
                    page.Render(r, Me.Printer.Pages)
                    RaiseEvent Rendered(Me, g)
                End If
            End Using
            Me.PageBuffers.Add(buf)
        Next
        Me.VScrollBar.Value = 0
        Me.HScrollBar.Value = 0
        Me._ScrollBarUpdate()
        Me._RaiseUpdateReport()
    End Sub

    Private Sub _ClearPageBuffer()
        If Me.PageBuffers IsNot Nothing Then
            For Each buf As Bitmap In Me.PageBuffers
                buf.Dispose()
            Next
        End If
        Me.PageBuffers = New List(Of Bitmap)
    End Sub

    Private Sub _ScrollBarUpdate()
        Me.VScrollBar.Visible = False
        Me.HScrollBar.Visible = False
        If Me.PageBuffers Is Nothing Then
            Return
        End If
        With Me.ClientRectangle
            If .Width = 0 Or .Height = 0 Then
                Return
            End If
        End With
        Dim w As Integer
        Dim h As Integer
        With Me._EntireBufferSize(False)
            w = .Width
            h = .Height
        End With
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

    Private Sub _ViewUpdate(ByVal g As Graphics)
        If Me.PageBuffers Is Nothing Then
            Exit Sub
        End If
        Using b As New SolidBrush(Color.DimGray)
            g.FillRectangle(b, 0, 0, Me.Width, Me.Height)
        End Using
        For i As Integer = 0 To IIf(Me.MultiPage, MULTI_PAGE_COUNT, 1) - 1
            If i >= Me.PageBuffers.Count Then
                Exit For
            End If
            Dim buf As Bitmap = Me.PageBuffers(i)
            Dim base As Point = Me._BaseLocation(i)
            Dim x As Integer = base.X
            Dim y As Integer = base.Y
            x -= Me.HScrollBar.Value
            y -= Me.VScrollBar.Value
            g.DrawImage(buf, x, y)
            Using p As New Pen(Color.Black)
                g.DrawRectangle(p, x - 1, y - 1, buf.Width + 1, buf.Height + 1)
                g.DrawRectangle(p, 0, 0, Me.Width - 1, Me.Height - 1)
            End Using
            If Me._FocusPageIndex >= 0 AndAlso Me.PageCount + i - 1 = Me._FocusPageIndex Then
                Dim r As Rectangle = _ToRect(i, Me._FocusRegion)
                Using p As New Pen(Color.FromArgb(128, Color.OrangeRed), (1 + Me.Zoom) * 2)
                    g.DrawRectangle(p, r.Left - Me.HScrollBar.Value, r.Top - Me.VScrollBar.Value, r.Width, r.Height)
                End Using
            End If
            If Me.VScrollBar.Visible AndAlso Me.HScrollBar.Visible Then
                Using b As New SolidBrush(System.Drawing.SystemColors.Control)
                    g.FillRectangle(b, Me.Width - SCROLLBAR_WIDTH, Me.Height - SCROLLBAR_WIDTH, SCROLLBAR_WIDTH, SCROLLBAR_WIDTH)
                End Using
            End If
        Next
    End Sub

    Private Function _EntireBufferSize(fillWidth As Boolean) As Size
        Dim w As Integer = MARGIN_VIEW * 2
        Dim h As Integer = 0
        For i As Integer = 0 To IIf(Me.MultiPage, MULTI_PAGE_COUNT, 1) - 1
            Dim buf As Bitmap
            If i < Me.PageBuffers.Count Then
                buf = Me.PageBuffers(i)
            ElseIf fillWidth Then
                buf = Me.PageBuffers(0)
            Else
                Exit For
            End If
            If i > 0 Then
                w += BORDER_VIEW
            End If
            w += buf.Width
            h = Math.Max(buf.Height + MARGIN_VIEW * 2, h)
        Next
        Return New Size(w, h)
    End Function

    Private Function _BaseLocation(i As Integer) As Point
        Dim es As Size = Me._EntireBufferSize(True)
        Dim x As Integer = (Me.Width - es.Width) / 2
        Dim y As Integer = (Me.Height - es.Height) / 2
        If x < MARGIN_VIEW Then
            x = MARGIN_VIEW
        End If
        If y < MARGIN_VIEW Then
            y = MARGIN_VIEW
        End If
        For j As Integer = 0 To i - 1
            Dim buf As Bitmap = Me.PageBuffers(j)
            x += BORDER_VIEW + buf.Width
        Next
        Return New Point(x, y)
    End Function

    Private Function _ToRect(i As Integer, region As component.Region) As Rectangle
        Dim base As Point = Me._BaseLocation(i)
        Return New Rectangle(
          ToPixelX(Me._ReportMargin.Width + region.Left) * Me.Zoom + base.X,
          ToPixelY(Me._ReportMargin.Height + region.Top) * Me.Zoom + base.Y,
          ToPixelX(region.GetWidth) * Me.Zoom, ToPixelX(region.GetHeight) * Me.Zoom)
    End Function

    Private Sub PrintPreview_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint
        If Not Me.DesignMode Then
            Me._ViewUpdate(e.Graphics)
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

    Private Sub PrintPreview_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        If Me.HScrollBar.Visible Or Me.VScrollBar.Visible Then
            Me._Gripping = True
            Me.Cursor = New Cursor(My.Resources.hand_grip.Handle)
            Me._GripLocation = e.Location
        End If
    End Sub

    Private Sub PrintPreview_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        If Me._Gripping Then
            Me._Gripping = False
            Me.Cursor = New Cursor(My.Resources.hand.Handle)
        End If
    End Sub

    Private Sub PrintPreview_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        If Me._Gripping Then
            Dim dx As Integer = Me._GripLocation.X - e.Location.X
            Dim dy As Integer = Me._GripLocation.Y - e.Location.Y
            If Me.HScrollBar.Visible Then
                Me.HScrollBar.Value = Math.Min(Math.Max(Me.HScrollBar.Value + dx, 0),
                                               Me.HScrollBar.Maximum - Me.HScrollBar.LargeChange)
            End If
            If Me.VScrollBar.Visible Then
                Me.VScrollBar.Value = Math.Min(Math.Max(Me.VScrollBar.Value + dy, 0),
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
                Me.NextPage()
            Case Keys.PageUp
                Me.PrevPage()
            Case Keys.Home
                Me.FirstPage()
            Case Keys.End
                Me.LastPage()
        End Select
    End Sub

    Public Sub ScrollTo(i As Integer, region As component.Region)
        Me.ScrollTo(_ToRect(i, region))
    End Sub

    Public Sub ScrollTo(rect As Rectangle)
        Dim m As Integer = 10
        Dim w As Integer = Me.Width
        Dim h As Integer = Me.Height
        If Me.VScrollBar.Visible Then
            w -= 16
        End If
        If Me.HScrollBar.Visible Then
            h -= 16
        End If
        If rect.Left - Me.HScrollBar.Value < m Then
            _SetScrollValue(Me.HScrollBar, rect.Left - m)
        ElseIf rect.Right - Me.HScrollBar.Value > w - m Then
            _SetScrollValue(Me.HScrollBar, Math.Min(rect.Right - w + m, rect.Left - m))
        End If
        If rect.Top - Me.VScrollBar.Value < m Then
            _SetScrollValue(Me.VScrollBar, rect.Top - m)
        ElseIf rect.Bottom - Me.VScrollBar.Value > h - m Then
            _SetScrollValue(Me.VScrollBar, Math.Min(rect.Bottom - h + m, rect.Top - m))
        End If
    End Sub

    Private Sub _SetScrollValue(scrollBar As ScrollBar, v As Integer)
        If scrollBar.Visible Then
            Dim _v As Integer = Math.Max(Math.Min(v, scrollBar.Maximum), scrollBar.Minimum)
            scrollBar.Value = v
        End If
    End Sub

    Public Sub SetFocusRegion(pageIndex As Integer, region As component.Region)
        Using Me.RenderBlock(True)
            Me._FocusPageIndex = pageIndex
            Me._FocusRegion = region
        End Using
    End Sub

    Public Sub ClearFocusRegion()
        If Me._FocusRegion Is Nothing Then
            Exit Sub
        End If
        Using Me.RenderBlock(True)
            Me._FocusPageIndex = -1
            Me._FocusRegion = Nothing
        End Using
    End Sub

    Public Function GetPages() As ReportPages Implements IPrintPreviewSearch.GetPages
        Return Me.Printer.Pages
    End Function

    Public Function _GetSearchPageIndex() As Integer Implements IPrintPreviewSearch.GetPageIndex
        Return Me.PageCount - 1
    End Function

    Private Sub _SearchFocus(pageIndex As Integer, region As component.Region) Implements IPrintPreviewSearch.Focus
        Dim pi As Integer = pageIndex
        Dim bi As Integer = 0
        If Me.MultiPage Then
            Dim _pi As Integer = Me.PageCount - 1
            If pi >= _pi And pi < _pi + MULTI_PAGE_COUNT Then
                pi = _pi
                bi = _pi - pi
            Else
                pi = Fix(pageIndex / MULTI_PAGE_COUNT) * MULTI_PAGE_COUNT
                bi = pageIndex - pi
            End If
        End If
        Using Me.RenderBlock(Me.PageCount = pi + 1)
            Me.SetFocusRegion(pageIndex, region)
            Me.PageCount = pi + 1
        End Using
        Me.ScrollTo(bi, region)
    End Sub

    Private Sub _SearchRelease() Implements IPrintPreviewSearch.Release
        Me.ClearFocusRegion()
    End Sub

End Class