﻿Imports jp.co.systembase.report.component
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
            Dim gdiText As GdiText = _GetGdiText()
            gdiText.Initialize(env, reportDesign, region, design, text)
            gdiText.Draw()
        End Sub

        Protected Overridable Function _GetGdiText() As GdiText
            Return New GdiText()
        End Function

    End Class

End Namespace
