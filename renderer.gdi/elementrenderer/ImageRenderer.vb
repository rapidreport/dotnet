﻿Imports jp.co.systembase.report.component
Imports ZXing

Namespace elementrenderer

    Public Class ImageRenderer
        Implements IElementRenderer

        Public Sub Render(
          env As RenderingEnv,
          reportDesign As ReportDesign,
          region As Region,
          design As ElementDesign,
          data As Object) Implements IElementRenderer.Render
            Dim img As Image = _GetImage(env, reportDesign, design, data)
            If img Is Nothing Then
                Exit Sub
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
            If design.Get("fit") AndAlso
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

        Protected Overridable Function _GetImage(
          env As RenderingEnv,
          reportDesign As ReportDesign,
          design As ElementDesign,
          data As Object) As Image
            If Not env.InDesigner Then
                If Not design.IsNull("key") And data IsNot Nothing Then
                    Dim key As String = design.Get("key")
                    If env.ImageLoaderMap.ContainsKey(key) Then
                        Return env.ImageLoaderMap(key).GetImage(data)
                    End If
                End If
            End If
            Dim ret = reportDesign.GetImage(design.Base, "image")
            If ret Is Nothing And env.InDesigner Then
                ret = My.Resources.MockImage
            End If
            Return ret
        End Function

    End Class

End Namespace
