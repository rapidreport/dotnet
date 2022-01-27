Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.textformatter
Imports jp.co.systembase.report.expression

Namespace elementrenderer

    Public Class TextRenderer
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
                text = design.Get("text")
                Dim textProcessor As New EmbeddedTextProcessor
                Dim mocks As List(Of String) = textProcessor.ExtractMocks(text)
                If mocks IsNot Nothing Then
                    text = textProcessor.EmbedMock(text, GdiRenderUtil.GetMockDataList(mocks))
                End If
            Else
                text = design.Get("text")
                If data IsNot Nothing Then
                    Dim textProcessor As New EmbeddedTextProcessor
                    text = textProcessor.EmbedData(reportDesign, design.Child("formatter"), text, data)
                End If
            End If
            If text Is Nothing Then
                Return
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
