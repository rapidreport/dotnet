Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer

Namespace customizer
    Public Interface IReportCustomizer
        Function ContentRegion( _
          ByVal content As Content, _
          ByVal evaluator As Evaluator, _
          ByVal region As Region) As Region
        Sub PageAdded( _
          ByVal report As Report, _
          ByVal pages As ReportPages, _
          ByVal page As ReportPage)
        Sub RenderContent( _
          ByVal content As Content, _
          ByVal evaluator As Evaluator, _
          ByVal region As Region, _
          ByVal elementDesigns As ElementDesigns)
    End Interface
End Namespace

