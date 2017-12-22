Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Interface IElementRenderer
        Sub Render( _
          env As RenderingEnv, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object)
    End Interface

End Namespace
