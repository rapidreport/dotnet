Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Class FieldRenderer
        Implements IElementRenderer

        Public Sub Render( _
          ByVal renderer As PdfRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Render
            Dim fd As ElementDesign = design.Child("formatter")
            Dim text As String = RenderUtil.Format(reportDesign, design.Child("formatter"), data)
            If text Is Nothing Then
                Exit Sub
            End If
            If renderer.Setting.ReplaceBackslashToYen Then
                text = text.Replace("\", ChrW(&HA5))
            End If
            Dim _region As Region = region.ToPointScale(reportDesign)
            PdfRenderUtil.DrawText( _
              renderer, _
              _region, _
              New TextDesign(reportDesign, design), _
              text)
        End Sub
    End Class

End Namespace
