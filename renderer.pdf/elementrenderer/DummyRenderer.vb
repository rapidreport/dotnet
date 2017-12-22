Imports iTextSharp.text.pdf

Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Class DummyRenderer
        Implements IElementRenderer

        Public Sub Render( _
          renderer As PdfRenderer, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object) Implements IElementRenderer.Render
        End Sub

    End Class

End Namespace
