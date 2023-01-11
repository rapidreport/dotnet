Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Class FieldRenderer
        Implements IElementRenderer

        Public Overridable Sub Render(
          env As RenderingEnv,
          reportDesign As ReportDesign,
          region As Region,
          design As ElementDesign,
          data As Object) Implements IElementRenderer.Render
            _RenderRect(env, reportDesign, region, design)
            Dim text = _GetText(env, reportDesign, design, data)
            If text Is Nothing Then
                Exit Sub
            End If
            Dim _region = _GetRegion(reportDesign, region, design)
            Dim gdiText = _GetGdiText(env, reportDesign, _region, design, text)
            gdiText.Initialize(env, reportDesign, _region, design, text)
            gdiText.Draw()
        End Sub

        Protected Overridable Sub _RenderRect(env As RenderingEnv, reportDesign As ReportDesign, region As Region, design As ElementDesign)
            If Not design.IsNull("rect") Then
                env.Setting.GetElementRenderer("rect").Render(
                    env,
                    reportDesign,
                    region,
                    design.Child("rect"),
                    Nothing)
            End If
        End Sub

        Protected Overridable Function _GetText(env As RenderingEnv, reportDesign As ReportDesign, design As ElementDesign, data As Object) As String
            If env.InDesigner Then
                If Not design.IsNull("mock") Then
                    Dim mockData As Object = GdiRenderUtil.GetMockData(design.Get("mock"))
                    Return RenderUtil.Format(reportDesign, design.Child("formatter"), mockData)
                Else
                    design.Put("decimal_place", 0)
                    Return design.Get("exp")
                End If
            Else
                Return RenderUtil.Format(reportDesign, design.Child("formatter"), data)
            End If
        End Function

        Protected Overridable Function _GetRegion(reportDesign As ReportDesign, region As Region, design As ElementDesign) As Region
            Dim ret = region
            If Not design.IsNull("margin") Then
                Dim m = design.Child("margin")
                ret = New Region(region, m.Get("left"), m.Get("top"), m.Get("right"), m.Get("bottom"))
            End If
            Return ret
        End Function

        Protected Overridable Function _GetGdiText(env As RenderingEnv, reportDesign As ReportDesign, region As Region, design As ElementDesign, text As String) As GdiText
            Return _GetGdiText()
        End Function

        Protected Overridable Function _GetGdiText() As GdiText
            Return New GdiText()
        End Function

    End Class

End Namespace