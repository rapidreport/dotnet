Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.textformatter
Imports jp.co.systembase.report.expression

Namespace elementrenderer

    Public Class TextRenderer
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
                Return
            End If
            Dim _region = _GetRegion(reportDesign, region, design)
            Dim gdiText As GdiText = _GetGdiText(env, reportDesign, _region, design, text)
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

        Protected Overridable Function _EmbedMock(text As String, reportDesign As ReportDesign, formatterDesign As ElementDesign) As String
            Try
                Dim textProcessor As New EmbeddedTextProcessor
                Dim mocks As List(Of String) = textProcessor.ExtractMocks(text)
                If mocks IsNot Nothing Then
                    Dim _mocks As New ArrayList
                    For Each m In mocks
                        Dim _m As String = m.ToString()
                        If Not m.StartsWith("#{") Then
                            _m = RenderUtil.Format(reportDesign, formatterDesign, GetMockData(m))
                        End If
                        _mocks.Add(_m)
                    Next
                    Return textProcessor.EmbedMock(text, _mocks)
                End If
            Catch ex As Exception
            End Try
            Return text
        End Function

        Protected Overridable Function _GetText(env As RenderingEnv, reportDesign As ReportDesign, design As ElementDesign, data As Object) As String
            If env.InDesigner Then
                Return _EmbedMock(design.Get("text"), reportDesign, design.Child("formatter"))
            Else
                Dim ret As String = design.Get("text")
                If data IsNot Nothing Then
                    Dim textProcessor As New EmbeddedTextProcessor
                    ret = textProcessor.EmbedData(reportDesign, design.Child("formatter"), ret, data)
                End If
                Return ret
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
