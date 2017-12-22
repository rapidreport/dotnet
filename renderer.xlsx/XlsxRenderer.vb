Imports NPOI.XSSF.UserModel
Imports NPOI.SS.UserModel

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.xlsx.component
Imports jp.co.systembase.report.renderer.xlsx.imageloader

Public Class XlsxRenderer
    Implements IRenderer

    Public Setting As XlsxRendererSetting
    Public ImageLoaderMap As Dictionary(Of Object, IXlsxImageLoader) = Nothing
    Public Workbook As XSSFWorkbook
    Public Sheet As XSSFSheet = Nothing
    Public Pages As List(Of Page) = Nothing
    Public CurrentPage As Page = Nothing

    Public ImagePool As Dictionary(Of Image, Integer)
    Public CellStylePool As CellStylePool
    Public FontPool As FontPool

    Public Sub New(workbook As XSSFWorkbook)
        Me.New(workbook, New XlsxRendererSetting)
    End Sub

    Public Sub New(workbook As XSSFWorkbook, setting As XlsxRendererSetting)
        Me.Workbook = workbook
        Me.Setting = setting
        Me.CellStylePool = New CellStylePool(Me)
        Me.FontPool = New FontPool(Me)
    End Sub

    Public Sub NewSheet(sheetName As String)
        Me.Sheet = Me.Workbook.CreateSheet(sheetName)
        Me.Pages = New List(Of Page)
        Me.CurrentPage = Nothing
        Me.ImageLoaderMap = New Dictionary(Of Object, IXlsxImageLoader)
        Me.Sheet.CreateDrawingPatriarch()
        Me.ImagePool = New Dictionary(Of Image, Integer)
    End Sub

    Public Sub BeginReport(reportDesign As ReportDesign) Implements IRenderer.BeginReport
        Dim ps As XSSFPrintSetup = Me.Sheet.PrintSetup
        Select Case reportDesign.PaperDesign.PaperType
            Case Report.EPaperType.A3
                ps.PaperSize = PaperSize.A3 + 1
            Case Report.EPaperType.A4
                ps.PaperSize = PaperSize.A4 + 1
            Case Report.EPaperType.A5
                ps.PaperSize = PaperSize.A5 + 1
            Case Report.EPaperType.B4
                ps.PaperSize = PaperSize.B4 + 1
            Case Report.EPaperType.B5
                ps.PaperSize = PaperSize.B5 + 1
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

    Public Sub EndReport(reportDesign As ReportDesign) Implements IRenderer.EndReport
        Dim cols As List(Of Single) = RowColUtil.CreateCols(reportDesign, Me)
        Dim colWidths As List(Of Integer) = RowColUtil.CreateColWidths(cols, 1.25F * Me.Setting.ColWidthCoefficent)
        For i As Integer = 0 To colWidths.Count - 1
            Me.Sheet.SetColumnWidth(i, colWidths(i))
        Next
        With Nothing
            Dim topRow As Integer = 0
            For Each page As Page In Me.Pages
                page.TopRow = topRow
                Dim rows As List(Of Single) = RowColUtil.CreateRows(reportDesign, page)
                Dim rowHeights As List(Of Integer) = RowColUtil.CreateRowHeights(rows, 1.46F * Me.Setting.RowHeightCoefficent)
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

    Public Sub BeginPage(reportDesign As ReportDesign, pageIndex As Integer, paperRegion As Region) Implements IRenderer.BeginPage
        Me.CurrentPage = New Page(Me, reportDesign, paperRegion)
        Me.Pages.Add(Me.CurrentPage)
    End Sub

    Public Sub EndPage(reportDesign As ReportDesign) Implements IRenderer.EndPage
    End Sub

    Public Sub RenderElement( _
      reportDesign As ReportDesign, _
      region As Region, _
      design As ElementDesign, _
      data As Object) Implements IRenderer.RenderElement
        Me.Setting.GetElementRenderer(design.Get("type")) _
          .Collect(Me, reportDesign, region, design, data)
    End Sub

    Public Function GetImageIndex(image As Image) As Integer
        If image Is Nothing Then
            Return -1
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
