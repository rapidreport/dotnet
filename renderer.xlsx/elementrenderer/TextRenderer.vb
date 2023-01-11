Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.textformatter
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.expression
Imports jp.co.systembase.report.renderer.xlsx.component

Namespace elementrenderer
    Public Class TextRenderer
        Implements IElementRenderer

        Public Overridable Sub Collect(
          renderer As XlsxRenderer,
          reportDesign As ReportDesign,
          region As Region,
          design As ElementDesign,
          data As Object) Implements IElementRenderer.Collect
            _RenderRect(renderer, reportDesign, region, design)
            Dim _region As Region = region.ToPointScale(reportDesign)
            If _region.GetWidth <= 0 Or _region.GetHeight <= 0 Then
                Exit Sub
            End If
            Dim field As New Field
            field.Region = _region
            field.Style = New FieldStyle(New TextDesign(reportDesign, design))
            field.Data = _GetText(reportDesign, design, data)
            renderer.CurrentPage.Fields.Add(field)
        End Sub

        Protected Overridable Sub _RenderRect(renderer As XlsxRenderer, reportDesign As ReportDesign, region As Region, design As ElementDesign)
            If Not design.IsNull("rect") Then
                renderer.Setting.GetElementRenderer("rect").Collect(
                  renderer,
                  reportDesign,
                  region,
                  design.Child("rect"),
                  Nothing)
            End If
        End Sub

        Protected Overridable Function _GetText(reportDesign As ReportDesign, design As ElementDesign, data As Object) As String
            Dim ret As String = design.Get("text")
            If data IsNot Nothing Then
                Dim textProcessor As New EmbeddedTextProcessor
                ret = textProcessor.EmbedData(reportDesign, design.Child("formatter"), ret, data)
            End If
            Return ret
        End Function

    End Class
End Namespace
