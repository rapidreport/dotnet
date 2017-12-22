Imports jp.co.systembase.report.component

Namespace elementrenderer
    Public Class DummyRenderer
        Implements IElementRenderer
        Public Sub Collect( _
          renderer As XlsxRenderer, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object) Implements IElementRenderer.Collect
        End Sub
    End Class
End Namespace