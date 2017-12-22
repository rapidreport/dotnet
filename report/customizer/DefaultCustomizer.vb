Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer

Namespace customizer
    Public Class DefaultCustomizer
        Implements IReportCustomizer
        Public Overridable Function ContentRegion( _
          content As Content, _
          evaluator As Evaluator, _
          region As Region) As Region Implements IReportCustomizer.ContentRegion
            Return region
        End Function
        Public Overridable Sub PageAdded( _
          report As Report, _
          pages As ReportPages, _
          page As ReportPage) Implements IReportCustomizer.PageAdded
        End Sub
        Public Overridable Sub RenderContent( _
          content As Content, _
          evaluator As Evaluator, _
          region As Region, _
          elementDesigns As ElementDesigns) Implements IReportCustomizer.RenderContent
        End Sub
    End Class
End Namespace