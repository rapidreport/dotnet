Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Class ImageRenderer
        Implements IElementRenderer

        Public Sub Render( _
          ByVal renderer As PdfRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Render
            Dim img As Image = Nothing
            If Not design.IsNull("key") AndAlso data IsNot Nothing Then
                Dim key As String = design.Get("key")
                If renderer.ImageLoaderMap.ContainsKey(key) Then
                    img = renderer.ImageLoaderMap(key).GetImage(data)
                End If
            End If
            If img Is Nothing Then
                img = renderer.GetImage(reportDesign, design.Base, "image")
            End If
            If img Is Nothing Then
                Exit Sub
            End If
            Dim _region As Region = region.ToPointScale(reportDesign)
            Dim cb As PdfContentByte = renderer.Writer.DirectContent
            Dim w As Single = img.Width
            Dim h As Single = img.Height
            Dim r As Single = 1.0F
            If w > _region.GetWidth Or h > _region.GetHeight Then
                r = Math.Min(_region.GetWidth / w, _region.GetHeight / h)
            End If
            If design.Get("fit") AndAlso _
              (w < _region.GetWidth And h < _region.GetHeight) Then
                r = Math.Min(_region.GetWidth / w, _region.GetHeight / h)
            End If
            w *= r
            h *= r
            Dim t As Single = _region.Top
            Dim l As Single = _region.Left
            If Not design.IsNull("valign") Then
                Select Case design.Get("valign")
                    Case "center"
                        t = _region.Top + (_region.GetHeight - h) / 2
                    Case "bottom"
                        t = _region.Bottom - h
                End Select
            End If
            If Not design.IsNull("halign") Then
                Select Case design.Get("halign")
                    Case "center"
                        l = _region.Left + (_region.GetWidth - w) / 2
                    Case "right"
                        l = _region.Right - w
                End Select
            End If
            cb.AddImage(img, w, 0, 0, h, renderer.Trans.X(l), renderer.Trans.Y(t + h))
        End Sub

    End Class

End Namespace
