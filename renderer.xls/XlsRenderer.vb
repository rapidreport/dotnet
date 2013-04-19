Imports NPOI.HSSF.UserModel
Imports NPOI.SS.UserModel

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.xls.component
Imports jp.co.systembase.report.renderer.xls.imageloader

Public Class XlsRenderer
    Implements IRenderer

    Public Setting As XlsRendererSetting
    Public ImageLoaderMap As Dictionary(Of Object, IXlsImageLoader) = Nothing
    Public Workbook As HSSFWorkbook
    Public Sheet As HSSFSheet = Nothing
    Public Pages As List(Of Page) = Nothing
    Public CurrentPage As Page = Nothing

    Public ImagePool As Dictionary(Of Image, Integer)
    Public CellStylePool As CellStylePool
    Public FontPool As FontPool
    Public ColorPool As ColorPool

    Public Sub New(ByVal workbook As HSSFWorkbook)
        Me.New(workbook, New XlsRendererSetting)
    End Sub

    Public Sub New(ByVal workbook As HSSFWorkbook, ByVal setting As XlsRendererSetting)
        Me.Workbook = workbook
        Me.Setting = setting
        Me.CellStylePool = New CellStylePool(Me)
        Me.FontPool = New FontPool(Me)
        Me.ColorPool = New ColorPool(Me)
    End Sub

    Public Sub NewSheet(ByVal sheetName As String)
        Me.Sheet = Me.Workbook.CreateSheet(sheetName)
        Me.Pages = New List(Of Page)
        Me.CurrentPage = Nothing
        Me.ImageLoaderMap = New Dictionary(Of Object, IXlsImageLoader)
        Me.Sheet.CreateDrawingPatriarch()
        Me.ImagePool = New Dictionary(Of Image, Integer)
    End Sub

    Public Sub BeginReport(ByVal reportDesign As ReportDesign) Implements IRenderer.BeginReport
        Dim ps As HSSFPrintSetup = Me.Sheet.PrintSetup
        Select Case reportDesign.PaperDesign.PaperType
            Case Report.EPaperType.A3
                ps.PaperSize = PaperSize.A3
            Case Report.EPaperType.A4
                ps.PaperSize = PaperSize.A4
            Case Report.EPaperType.A5
                ps.PaperSize = PaperSize.A5
            Case Report.EPaperType.B4
                ps.PaperSize = PaperSize.B4
            Case Report.EPaperType.B5
                ps.PaperSize = PaperSize.B5
        End Select
        ps.Landscape = reportDesign.PaperDesign.Landscape
        ps.HeaderMargin = 0
        ps.FooterMargin = 0
        ps.VResolution = Me.Setting.VResolution
        ps.HResolution = Me.Setting.HResolution
        With Nothing
            Dim m As PaperMarginDesign = reportDesign.PaperDesign.Margin.ToPoint(reportDesign.PaperDesign)
            Me.Sheet.SetMargin(MarginType.TopMargin, m.Top / 72)
            Me.Sheet.SetMargin(MarginType.BottomMargin, m.Bottom / 72)
            Me.Sheet.SetMargin(MarginType.LeftMargin, m.Left / 72)
            Me.Sheet.SetMargin(MarginType.RightMargin, m.Right / 72)
        End With
    End Sub

    Public Sub EndReport(ByVal reportDesign As ReportDesign) Implements IRenderer.EndReport
        Dim cols As List(Of Single) = RowColUtil.CreateCols(reportDesign, Me)
        Dim colWidths As List(Of Integer) = RowColUtil.CreateColWidths(cols, 1.26F * Me.Setting.ColWidthCoefficent)
        For i As Integer = 0 To colWidths.Count - 1
            Me.Sheet.SetColumnWidth(i, colWidths(i))
        Next
        With Nothing
            Dim topRow As Integer = 0
            For Each page As Page In Me.Pages
                page.TopRow = topRow
                Dim rows As List(Of Single) = RowColUtil.CreateRows(reportDesign, page)
                Dim rowHeights As List(Of Integer) = RowColUtil.CreateRowHeights(rows, 1.36F * Me.Setting.RowHeightCoefficent)
                For i As Integer = 0 To rowHeights.Count - 1
                    Me.Sheet.CreateRow(topRow + i).Height = rowHeights(i)
                Next
                Dim cellMap As New CellMap(rowHeights.Count, colWidths.Count, page)
                For r As Integer = 0 To rowHeights.Count - 1
                    For c As Integer = 0 To colWidths.Count - 1
                        Dim cell As Cell = cellMap.Map(r)(c)
                        If cell IsNot Nothing Then
                            cell.Render(page)
                        End If
                    Next
                Next
                For Each shape As Shape In page.Shapes
                    shape.Renderer.Render(page, shape)
                Next
                topRow += rowHeights.Count
                Me.Sheet.SetRowBreak(topRow - 1)
            Next
            Me.Sheet.Workbook.SetPrintArea(Me.Sheet.Workbook.GetSheetIndex(Me.Sheet), 0, colWidths.Count - 1, 0, topRow - 1)
            Me.Sheet.FitToPage = False
        End With
    End Sub

    Public Sub BeginPage(ByVal reportDesign As ReportDesign, ByVal pageIndex As Integer, ByVal paperRegion As Region) Implements IRenderer.BeginPage
        Me.CurrentPage = New Page(Me, reportDesign, paperRegion)
        Me.Pages.Add(Me.CurrentPage)
    End Sub

    Public Sub EndPage(ByVal reportDesign As ReportDesign) Implements IRenderer.EndPage
    End Sub

    Public Sub RenderElement( _
      ByVal reportDesign As ReportDesign, _
      ByVal region As Region, _
      ByVal design As ElementDesign, _
      ByVal data As Object) Implements IRenderer.RenderElement
        Me.Setting.GetElementRenderer(design.Get("type")) _
          .Collect(Me, reportDesign, region, design, data)
    End Sub

    Public Function GetImageIndex(ByVal image As Image) As Integer
        If image Is Nothing Then
            Return 0
        End If
        If Not Me.ImagePool.ContainsKey(image) Then
            Dim index As Integer = Me.Workbook.AddPicture( _
                  (New ImageConverter).ConvertTo(image, GetType(Byte())), _
                  NPOI.SS.UserModel.PictureType.PNG)
            Me.ImagePool.Add(image, index)
        End If
        Return Me.ImagePool(image)
    End Function

End Class
