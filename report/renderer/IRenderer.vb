Imports jp.co.systembase.report.component

Namespace renderer

    Public Interface IRenderer
        Sub BeginReport(ByVal reportDesign As ReportDesign)
        Sub EndReport(ByVal reportDesign As ReportDesign)
        Sub BeginPage(ByVal reportDesign As ReportDesign, ByVal pageIndex As Integer, ByVal paperRegion As Region)
        Sub EndPage(ByVal reportDesign As ReportDesign)
        Sub RenderElement(ByVal reportDesign As ReportDesign, ByVal region As Region, ByVal design As ElementDesign, ByVal data As Object)
    End Interface

End Namespace