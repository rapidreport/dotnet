Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.textformatter
Imports jp.co.systembase.report.renderer

Namespace elementrenderer

    Public Class FieldRenderer
        Implements IElementRenderer

        Public Sub Render( _
          ByVal env As RenderingEnv, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Render
            If Not design.IsNull("rect") Then
                env.Setting.GetElementRenderer("rect").Render( _
                    env, _
                    reportDesign, _
                    region, _
                    design.Child("rect"), _
                    Nothing)
            End If
            Dim text As String
            If env.InDesigner Then
                If Not design.IsNull("mock") Then
                    Dim mockData As Object = GdiRenderUtil.GetMockData(design.Get("mock"))
                    text = RenderUtil.Format(reportDesign, design.Child("formatter"), mockData)
                Else
                    text = design.Get("exp")
                    design.Put("decimal_place", 0)
                End If
            Else
                text = RenderUtil.Format(reportDesign, design.Child("formatter"), data)
            End If
            If text Is Nothing Then
                Exit Sub
            End If
            Dim gdiText As GdiText = _GetGdiText()
            gdiText.Initialize(env, reportDesign, region, design, text)
            gdiText.Draw()
        End Sub

        Protected Overridable Function _GetGdiText() As GdiText
            Return New GdiText()
        End Function

    End Class

End Namespace