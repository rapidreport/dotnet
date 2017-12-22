Imports jp.co.systembase.report.component

Namespace renderer

    Public Interface IRenderer
        Sub BeginReport(reportDesign As ReportDesign)
        Sub EndReport(reportDesign As ReportDesign)
        Sub BeginPage(reportDesign As ReportDesign, pageIndex As Integer, paperRegion As Region)
        Sub EndPage(reportDesign As ReportDesign)
        Sub RenderElement(reportDesign As ReportDesign, region As Region, design As ElementDesign, data As Object)
    End Interface

End Namespace