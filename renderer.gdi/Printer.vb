Imports System.Drawing.Printing
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer.gdi.imageloader

Public Class Printer

    Public Shared ShowStatusDialog As Boolean = True

    Public Setting As GdiRendererSetting
    Public ImageLoaderMap As New Dictionary(Of String, IGdiImageLoader)
    Public PageIndex As Integer = 0
    Public PrintDialog As New PrintDialog
    Public WithEvents PrintDocument As New PrintDocument
    Public DynamicPageSetting As Boolean = False
    Public PrintedPages As New List(Of Integer)
    Public PreviewBackgroundImage As Image = Nothing
    Public PreviewBackgroundSetting As New PreviewBackgroundSettingClass

    Private _Pages As ReportPages
    Private _HardMarginMap As Dictionary(Of ReportDesign, PointF)
    Private _CurrentPageCount As Integer = 0

    Public Class PreviewBackgroundSettingClass
        Public X As Decimal = 0
        Public Y As Decimal = 0
        Public Scale As Decimal = 1
        Public Alpha As Decimal = 0.5
    End Class

    Public Sub New(pages As ReportPages)
        Me.New(pages, New GdiRendererSetting)
    End Sub

    Public Sub New(pages As ReportPages, setting As GdiRendererSetting)
        With Me.PrintDialog
            .Document = Me.PrintDocument
            .AllowSomePages = True
            .UseEXDialog = True
        End With
        If Not ShowStatusDialog Then
            Me.PrintDocument.PrintController = New StandardPrintController
        End If
        Me.Pages = pages
        Me.Setting = setting
    End Sub

    Public Property Pages() As ReportPages
        Get
            Return Me._Pages
        End Get
        Set(value As ReportPages)
            If value.Count = 0 Then
                Throw New RenderException("ページがありません")
            End If
            Me._Pages = value
            Me.Pages.SetUpCountingPages()
            With Me.PrintDialog
                .PrinterSettings.FromPage = 1
                .PrinterSettings.ToPage = 1
                .PrinterSettings.MinimumPage = 1
                .PrinterSettings.MaximumPage = Me.Pages.Count
            End With
            If Me.Pages.Report.Design.Caption IsNot Nothing Then
                Me.PrintDocument.DocumentName = Me.Pages.Report.Design.Caption
            End If
            GdiRenderUtil.SetUpPrinterSetting(
              Me.PrintDocument.DefaultPageSettings, Me.Pages.Report.Design)
            GdiRenderUtil.SetUpPageSettings(
              Me.PrintDocument.DefaultPageSettings, Me.Pages.Report.Design)
        End Set
    End Property

    Public Property CurrentPageCount As Integer
        Get
            Return Me._CurrentPageCount
        End Get
        Set(value As Integer)
            Me._CurrentPageCount = value
            Me.PrintDialog.AllowCurrentPage = (Me.CurrentPageCount > 0)
        End Set
    End Property

    Private Sub PrintDocument_BeginPrint(sender As Object, e As PrintEventArgs) Handles PrintDocument.BeginPrint
        Me.PageIndex = 0
        Me._HardMarginMap = New Dictionary(Of ReportDesign, PointF)
    End Sub

    Private Sub PrintDocument_QueryPageSettings(sender As Object, e As QueryPageSettingsEventArgs) Handles PrintDocument.QueryPageSettings
        Select Case e.PageSettings.PrinterSettings.PrintRange
            Case PrintRange.SomePages
                Do While Me.PageIndex < e.PageSettings.PrinterSettings.FromPage - 1
                    Me.PageIndex += 1
                Loop
            Case PrintRange.CurrentPage
                Me.PageIndex = Me.CurrentPageCount - 1
        End Select
        Dim page As ReportPage = Me.Pages(Me.PageIndex)
        If Me.DynamicPageSetting Then
            GdiRenderUtil.SetUpPageSettings(e.PageSettings, page.Report.Design)
        End If
    End Sub

    Private Sub PrintDocument_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDocument.PrintPage
        Dim page As ReportPage = Me.Pages(PageIndex)
        If Not Me._HardMarginMap.ContainsKey(page.Report.Design) Then
            Me._HardMarginMap.Add(
                page.Report.Design,
                New PointF(e.PageSettings.HardMarginX * 72 / 100,
                           e.PageSettings.HardMarginY * 72 / 100))
        End If
        GdiRenderUtil.SetUpGraphics(e.Graphics, page.Report.Design, Me.PageIndex, Me._HardMarginMap(page.Report.Design))
        Me._Render(e.Graphics, page)
        If Not Me.PrintedPages.Contains(Me.PageIndex) Then
            Me.PrintedPages.Add(Me.PageIndex)
        End If
        Me.PageIndex += 1
        If Me.PageIndex = Me.Pages.Count Then
            e.HasMorePages = False
        Else
            Select Case e.PageSettings.PrinterSettings.PrintRange
                Case PrintRange.SomePages
                    If Me.PageIndex = e.PageSettings.PrinterSettings.ToPage Then
                        e.HasMorePages = False
                    Else
                        e.HasMorePages = True
                    End If
                Case PrintRange.CurrentPage
                    e.HasMorePages = False
                Case Else
                    e.HasMorePages = True
            End Select
        End If
    End Sub

    Private Sub _Render(graphics As Graphics, page As ReportPage)
        Dim renderer As IRenderer = New GdiRenderer(New RenderingEnv(graphics, Me))
        page.Render(renderer, Me.Pages)
    End Sub

    Public Function GetPixelSize(pageIndex As Integer) As Size
        Dim page As ReportPage = Me.Pages(pageIndex)
        With page.Report.Design.PaperDesign.GetActualSize.ToPoint(page.Report.Design.PaperDesign)
            Using tmp As New Bitmap(1, 1)
                Using g As Graphics = Graphics.FromImage(tmp)
                    Return New Size(GdiRenderUtil.ToPixelX(g, .Width),
                                    GdiRenderUtil.ToPixelY(g, .Height))
                End Using
            End Using
        End With
    End Function

    Public Function GetImage(pageIndex As Integer, scaleX As Single, scaleY As Single) As Image
        Dim size As Size = GetPixelSize(pageIndex)
        Dim ret As New Bitmap(CType(size.Width * scaleX, Integer),
                              CType(size.Height * scaleY, Integer))
        Using g As Graphics = Graphics.FromImage(ret)
            g.ScaleTransform(scaleX, scaleY)
            g.FillRectangle(Brushes.White, 0, 0, size.Width, size.Height)
            Me.Render(g, pageIndex)
        End Using
        Return ret
    End Function

    Public Sub Render(graphics As Graphics, pageIndex As Integer)
        Dim page As ReportPage = Me.Pages(pageIndex)
        GdiRenderUtil.SetUpGraphics(graphics, page.Report.Design, Me.PageIndex, New PointF(0, 0))
        Me._Render(graphics, page)
    End Sub

    Public Function GetImage(pageIndex As Integer) As Image
        Return GetImage(pageIndex, 1.0, 1.0)
    End Function

    Public Shared Sub DrawBackgroundImage(graphics As Graphics, image As Image, setting As PreviewBackgroundSettingClass, paperDesign As PaperDesign)
        Dim pageSize As PaperSizeDesign = paperDesign.GetActualSize.ToPoint(paperDesign)
        Dim scale As Single = Math.Min((pageSize.Width / image.Width),
                                       (pageSize.Height / image.Height)) * setting.Scale
        Dim cm As New Imaging.ColorMatrix()
        cm.Matrix00 = 1
        cm.Matrix11 = 1
        cm.Matrix22 = 1
        cm.Matrix33 = setting.Alpha
        cm.Matrix44 = 1
        Dim ia As New Imaging.ImageAttributes()
        ia.SetColorMatrix(cm)
        graphics.DrawImage(image,
                           New Rectangle(paperDesign.ToPoint(setting.X), paperDesign.ToPoint(setting.Y),
                                         image.Width * scale, image.Height * scale),
                           0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia)
    End Sub

End Class