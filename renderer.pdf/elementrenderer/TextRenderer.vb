Imports iTextSharp.text.pdf

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.textformatter
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.expression

Namespace elementrenderer

    Public Class TextRenderer
        Implements IElementRenderer

        Public Sub Render( _
          renderer As PdfRenderer, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object) Implements IElementRenderer.Render
            If Not design.IsNull("rect") Then
                renderer.Setting.GetElementRenderer("rect").Render( _
                  renderer, _
                  reportDesign, _
                  region, _
                  design.Child("rect"), _
                  Nothing)
            End If
            Dim text As String = design.Get("text")
            If data IsNot Nothing Then
                Dim textProcessor As New EmbeddedTextProcessor
                text = textProcessor.EmbedData(reportDesign, design.Child("formatter"), text, data)
            End If
            If text Is Nothing Then
                Return
            End If
            If renderer.Setting.ReplaceBackslashToYen Then
                text = text.Replace("\", ChrW(&HA5))
            End If
            Dim pdfText As New PdfText()
            pdfText.Initialize(renderer, reportDesign, region, design, text)
            pdfText.Draw()
        End Sub
    End Class

End Namespace