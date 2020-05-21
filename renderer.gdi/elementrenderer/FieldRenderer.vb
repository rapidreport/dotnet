Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Class FieldRenderer
        Implements IElementRenderer

        Public Sub Render( _
          env As RenderingEnv, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object) Implements IElementRenderer.Render
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
            Dim _region As Region = region
            If Not design.IsNull("margin") Then
                Dim m As ElementDesign = design.Child("margin")
                _region = New Region(region, m.Get("left"), m.Get("top"), m.Get("right"), m.Get("bottom"))
            End If
            Dim gdiText As GdiText = _GetGdiText(env, reportDesign, _region, design, text)
            gdiText.Initialize(env, reportDesign, _region, design, text)
            gdiText.Draw()
        End Sub

        Protected Overridable Function _GetGdiText(env As RenderingEnv, reportDesign As ReportDesign, region As Region, design As ElementDesign, text As String) As GdiText
            Return _GetGdiText()
        End Function

        Protected Overridable Function _GetGdiText() As GdiText
            Return New GdiText()
        End Function

    End Class

End Namespace