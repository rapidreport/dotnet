Imports iTextSharp.text.pdf

Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Class DummyRenderer
        Implements IElementRenderer

        Public Sub Render( _
          ByVal renderer As PdfRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Render
            Dim _region As Region = region.ToPointScale(reportDesign)
            Dim cb As PdfContentByte = renderer.Writer.DirectContent
            cb.SaveState()
            With Nothing
                Dim t As String = design.Get("type")
                If t IsNot Nothing Then
                    cb.SetFontAndSize(renderer.Setting.DefaultFont, 10)
                    cb.SetTextRenderingMode(PdfContentByte.TEXT_RENDER_MODE_FILL)
                    cb.BeginText()
                    cb.SetTextMatrix(renderer.Trans.X(_region.Left), renderer.Trans.Y(_region.Top + 10))
                    cb.ShowText(t)
                    cb.EndText()
                End If
            End With
            cb.SetLineDash(New Single() {1, 2}, 0)
            cb.SetColorStroke(iTextSharp.text.Color.RED)
            cb.Rectangle(renderer.Trans.X(_region.Left), renderer.Trans.Y(_region.Bottom), _region.GetWidth, _region.GetHeight)
            cb.Stroke()
            cb.MoveTo(renderer.Trans.X(_region.Left), renderer.Trans.Y(_region.Top))
            cb.LineTo(renderer.Trans.X(_region.Right), renderer.Trans.Y(_region.Bottom))
            cb.Stroke()
            cb.MoveTo(renderer.Trans.X(_region.Left), renderer.Trans.Y(_region.Bottom))
            cb.LineTo(renderer.Trans.X(_region.Right), renderer.Trans.Y(_region.Top))
            cb.Stroke()
            cb.RestoreState()
        End Sub

    End Class

End Namespace
