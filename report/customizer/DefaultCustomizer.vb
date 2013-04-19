Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer

Namespace customizer
    Public Class DefaultCustomizer
        Implements IReportCustomizer
        Public Overridable Function ContentRegion( _
          ByVal content As Content, _
          ByVal evaluator As Evaluator, _
          ByVal region As Region) As Region Implements IReportCustomizer.ContentRegion
            Return region
        End Function
        Public Overridable Sub PageAdded( _
          ByVal report As Report, _
          ByVal pages As ReportPages, _
          ByVal page As ReportPage) Implements IReportCustomizer.PageAdded
        End Sub
        Public Overridable Sub RenderContent( _
          ByVal content As Content, _
          ByVal evaluator As Evaluator, _
          ByVal region As Region, _
          ByVal elementDesigns As ElementDesigns) Implements IReportCustomizer.RenderContent
        End Sub
    End Class
End Namespace