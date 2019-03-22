Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Class ImageRenderer
        Implements IElementRenderer

        Public Sub Render( _
          env As RenderingEnv, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object) Implements IElementRenderer.Render
            Dim img As Image = Nothing
            If Not env.InDesigner Then
                If Not design.IsNull("key") And data IsNot Nothing Then
                    Dim key As String = design.Get("key")
                    If env.ImageLoaderMap.ContainsKey(key) Then
                        img = env.ImageLoaderMap(key).GetImage(data)
                    End If
                End If
            End If
            If img Is Nothing Then
                img = reportDesign.GetImage(design.Base, "image")
            End If
            If img Is Nothing Then
                If env.InDesigner Then
                    img = My.Resources.MockImage
                Else
                    Exit Sub
                End If
            End If
            Dim _region As Region = region.ToPointScale(reportDesign)
            Dim g As Graphics = env.Graphics
            Dim w As Single = img.Width
            Dim h As Single = img.Height
            Dim r As Single = 1.0F
            If Not Report.Compatibility._4_37_ImagePixelScale Then
                Dim dpix As Single = img.HorizontalResolution
                Dim dpiy As Single = img.HorizontalResolution
                w *= 72.0F / IIf(dpix > 0, dpix, 96)
                h *= 72.0F / IIf(dpiy > 0, dpiy, 96)
            End If
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
            g.DrawImage(img, l, t, w, h)
        End Sub

    End Class

End Namespace
