Imports System.Drawing.Printing
Imports System.Drawing

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.textformatter
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.gdi.elementrenderer
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

    Private _Pages As ReportPages
    Private _HardMarginMap As Dictionary(Of ReportDesign, PointF)

    Public Sub New(ByVal pages As ReportPages)
        Me.New(pages, New GdiRendererSetting)
    End Sub

    Public Sub New(ByVal pages As ReportPages, ByVal setting As GdiRendererSetting)
        With Me.PrintDialog
            .Document = Me.PrintDocument
            If pages.Report.Design.Caption IsNot Nothing Then
                .Document.DocumentName = pages.Report.Design.Caption
            End If
            .AllowSomePages = True
            .UseEXDialog = True
        End With
        If Not ShowStatusDialog Then
            Me.PrintDocument.PrintController = New StandardPrintController
        End If
        Me.Pages = pages
        Me.Setting = Setting
    End Sub

    Public Property Pages() As ReportPages
        Get
            Return Me._Pages
        End Get
        Set(ByVal value As ReportPages)
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
            GdiRenderUtil.SetUpPrinterSetting( _
              Me.PrintDocument.DefaultPageSettings, Me.Pages.Report.Design)
            GdiRenderUtil.SetUpPageSettings( _
              Me.PrintDocument.DefaultPageSettings, Me.Pages.Report.Design)
        End Set
    End Property

    Private Sub PrintDocument_BeginPrint(ByVal sender As Object, ByVal e As PrintEventArgs) Handles PrintDocument.BeginPrint
        Me.PageIndex = 0
        Me._HardMarginMap = New Dictionary(Of ReportDesign, PointF)
    End Sub

    Private Sub PrintDocument_QueryPageSettings(ByVal sender As Object, ByVal e As QueryPageSettingsEventArgs) Handles PrintDocument.QueryPageSettings
        If e.PageSettings.PrinterSettings.PrintRange = PrintRange.SomePages Then
            Do While Me.PageIndex < e.PageSettings.PrinterSettings.FromPage - 1
                Me.PageIndex += 1
            Loop
        End If
        Dim page As ReportPage = Me.Pages(Me.PageIndex)
        If Me.DynamicPageSetting Then
            GdiRenderUtil.SetUpPageSettings(e.PageSettings, page.Report.Design)
        End If
    End Sub

    Private Sub PrintDocument_PrintPage(ByVal sender As Object, ByVal e As PrintPageEventArgs) Handles PrintDocument.PrintPage
        Dim page As ReportPage = Me.Pages(PageIndex)
        If Not Me._HardMarginMap.ContainsKey(page.Report.Design) Then
            Me._HardMarginMap.Add( _
                page.Report.Design, _
                New PointF(e.PageSettings.HardMarginX * 72 / 100, _
                           e.PageSettings.HardMarginY * 72 / 100))
        End If
        GdiRenderUtil.SetUpGraphics(e.Graphics, page.Report.Design, Me.PageIndex, Me._HardMarginMap(page.Report.Design))
        Me.Render(e.Graphics, page)
        If Not Me.PrintedPages.Contains(Me.PageIndex) Then
            Me.PrintedPages.Add(Me.PageIndex)
        End If
        Me.PageIndex += 1
        If Me.PageIndex = Me.Pages.Count Then
            e.HasMorePages = False
        ElseIf e.PageSettings.PrinterSettings.PrintRange = PrintRange.SomePages AndAlso _
          Me.PageIndex = e.PageSettings.PrinterSettings.ToPage Then
            e.HasMorePages = False
        Else
            e.HasMorePages = True
        End If
    End Sub

    Public Sub Render(ByVal g As Graphics, ByVal page As ReportPage)
        Dim renderer As IRenderer = New GdiRenderer(New RenderingEnv(g, Me))
        page.Render(renderer, Me.Pages)
    End Sub

    Public Function GetImage(ByVal pageIndex As Integer) As Image
        Dim w As Integer
        Dim h As Integer
        Dim page As ReportPage = Me.Pages(pageIndex)
        With page.Report.Design.PaperDesign.GetActualSize.ToPoint(page.Report.Design.PaperDesign)
            Dim g As Graphics = Graphics.FromImage(New Bitmap(1, 1))
            w = GdiRenderUtil.ToPixelX(g, .Width)
            h = GdiRenderUtil.ToPixelY(g, .Height)
        End With
        Dim ret As New Bitmap(w, h)
        With Nothing
            Dim g As Graphics = Graphics.FromImage(ret)
            g.FillRectangle(Brushes.White, 0, 0, w, h)
            GdiRenderUtil.SetUpGraphics(g, page.Report.Design, Me.PageIndex, New PointF(0, 0))
            Me.Render(g, page)
        End With
        Return ret
    End Function

End Class