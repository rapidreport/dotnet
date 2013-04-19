﻿Imports iTextSharp.text.pdf

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.textformatter
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.expression

Namespace elementrenderer

    Public Class TextRenderer
        Implements IElementRenderer

        Public Sub Render( _
          ByVal renderer As PdfRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Render
            Dim text As String = design.Get("text")
            If data IsNot Nothing Then
                Dim textProcessor As New EmbeddedTextProcessor
                text = textProcessor.EmbedData(reportDesign, design.Child("formatter"), text, data)
            End If
            If text Is Nothing Then
                Return
            End If
            If renderer.Setting.ReplaceBackslashToYen Then
                text = text.Replace("\", ChrW(&HA5))
            End If
            Dim _region As Region = region.ToPointScale(reportDesign)
            PdfRenderUtil.DrawText( _
              renderer, _
              _region, _
              New TextDesign(reportDesign, design), _
              text)
        End Sub
    End Class

End Namespace