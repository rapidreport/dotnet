Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Interface IElementRenderer
        Sub Render( _
          ByVal renderer As PdfRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object)
    End Interface

End Namespace
