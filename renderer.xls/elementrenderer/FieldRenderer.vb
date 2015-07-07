Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer.xls.component

Namespace elementrenderer
    Public Class FieldRenderer
        Implements IElementRenderer

        Public Sub Collect( _
          ByVal renderer As XlsRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Collect
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
            Dim field As New Field
            field.Region = _region
            field.Style = New FieldStyle(New TextDesign(reportDesign, design))
            If data IsNot Nothing Then
                If field.Style.TextDesign.XlsFormat IsNot Nothing Then
                    field.Data = data
                Else
                    field.Data = RenderUtil.Format(reportDesign, design.Child("formatter"), data)
                End If
            End If
            renderer.CurrentPage.Fields.Add(field)
        End Sub

    End Class
End Namespace