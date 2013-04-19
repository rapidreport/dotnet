Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.textformatter
Imports jp.co.systembase.report.expression

Namespace elementrenderer

    Public Class TextRenderer
        Implements IElementRenderer

        Public Sub Render( _
          ByVal env As RenderingEnv, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Render
            Dim text As String
            If env.InDesigner Then
                text = design.Get("text")
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
            Dim _region As Region = region.ToPointScale(reportDesign)
            GdiRenderUtil.DrawText( _
              env.Graphics, _
              _region, _
              env.Setting, _
              New TextDesign(reportDesign, design), _
              text)
        End Sub

    End Class

End Namespace
