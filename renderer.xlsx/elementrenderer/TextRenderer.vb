Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.textformatter
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.expression
Imports jp.co.systembase.report.renderer.xlsx.component

Namespace elementrenderer
    Public Class TextRenderer
        Implements IElementRenderer

        Public Sub Collect( _
          renderer As XlsxRenderer, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object) Implements IElementRenderer.Collect
            If Not design.IsNull("rect") Then
                renderer.Setting.GetElementRenderer("rect").Collect( _
                  renderer, _
                  reportDesign, _
                  region, _
                  design.Child("rect"), _
                  Nothing)
            End If
            Dim _region As Region = region.ToPointScale(reportDesign)
            If _region.GetWidth <= 0 Or _region.GetHeight <= 0 Then
                Exit Sub
            End If
            Dim text As String = design.Get("text")
            If data IsNot Nothing Then
                Dim textProcessor As New EmbeddedTextProcessor
                text = textProcessor.EmbedData(reportDesign, design.Child("formatter"), text, data)
            End If
            Dim field As New Field
            field.Region = _region
            field.Style = New FieldStyle(New TextDesign(reportDesign, design))
            field.Data = text
            renderer.CurrentPage.Fields.Add(field)
        End Sub

    End Class
End Namespace
