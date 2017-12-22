Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer

Namespace customizer
    Public Interface IReportCustomizer
        Function ContentRegion( _
          content As Content, _
          evaluator As Evaluator, _
          region As Region) As Region
        Sub PageAdded( _
          report As Report, _
          pages As ReportPages, _
          page As ReportPage)
        Sub RenderContent( _
          content As Content, _
          evaluator As Evaluator, _
          region As Region, _
          elementDesigns As ElementDesigns)
    End Interface
End Namespace

