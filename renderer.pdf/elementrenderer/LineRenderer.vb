Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Class LineRenderer
        Implements IElementRenderer

        Public Sub Render( _
          ByVal renderer As PdfRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Render
            Dim _region As Region = region.ToPointScale(reportDesign)
            Dim cb As PdfContentByte = renderer.Writer.DirectContent()
            Dim lw As Single = reportDesign.DefaultLineWidth
            If Not design.IsNull("line_width") Then
                lw = design.Get("line_width")
            End If
            If lw = 0 Then
                Exit Sub
            End If
            cb.SaveState()
            Try
                cb.SetLineWidth(lw)
                If Not design.IsNull("color") Then
                    Dim c As Color = PdfRenderUtil.GetColor(design.Get("color"))
                    If c IsNot Nothing Then
                        cb.SetColorStroke(c)
                    End If
                End If
                If Not design.IsNull("line_pattern") Then
                    Dim pt As String() = CType(design.Get("line_pattern"), String).Split(",")
                    Dim ptl As New List(Of Single)
                    For Each v As String In pt
                        If Char.IsDigit(v) AndAlso v > 0 Then
                            ptl.Add(v)
                        End If
                    Next
                    If ptl.Count > 0 Then
                        If ptl.Count Mod 2 = 1 Then
                            ptl.Add(0)
                        End If
                        cb.SetLineDash(ptl.ToArray, 0)
                    End If
                ElseIf Not design.IsNull("line_style") Then
                    Select Case design.Get("line_style")
                        Case "dot"
                            cb.SetLineDash(New Single() {1 * lw, 1 * lw}, 0)
                        Case "dash"
                            cb.SetLineDash(New Single() {3 * lw, 1 * lw}, 0)
                        Case "dashdot"
                            cb.SetLineDash(New Single() {3 * lw, 1 * lw, 1 * lw, 1 * lw}, 0)
                    End Select
                End If
                If design.Get("doublet") Then
                    Dim r As Single = Math.Atan2(_region.GetHeight, _region.GetWidth) + Math.PI / 2
                    Dim dx As Single = Math.Cos(r) * Math.Max(lw, 0.7F)
                    Dim dy As Single = Math.Sin(r) * Math.Max(lw, 0.7F)
                    cb.MoveTo(renderer.Trans.X(_region.Left + dx), _
                              renderer.Trans.Y(_region.Top + dy))
                    cb.LineTo(renderer.Trans.X(_region.Right + dx), _
                              renderer.Trans.Y(_region.Bottom + dy))
                    cb.Stroke()
                    cb.MoveTo(renderer.Trans.X(_region.Left - dx), _
                              renderer.Trans.Y(_region.Top - dy))
                    cb.LineTo(renderer.Trans.X(_region.Right - dx), _
                              renderer.Trans.Y(_region.Bottom - dy))
                    cb.Stroke()
                Else
                    Dim startArrow As Boolean = design.Get("start_cap") = "arrow"
                    Dim endArrow As Boolean = design.Get("end_cap") = "arrow"
                    Dim r As Single = Math.Atan2(_region.GetHeight, _region.GetWidth)
                    Dim w As Single = lw + 2
                    If startArrow Then
                        cb.MoveTo(renderer.Trans.X(_region.Left + tx(r, w * 2, 0)), _
                                  renderer.Trans.Y(_region.Top + ty(r, w * 2, 0)))
                    Else
                        cb.MoveTo(renderer.Trans.X(_region.Left), _
                                  renderer.Trans.Y(_region.Top))
                    End If
                    If endArrow Then
                        cb.LineTo(renderer.Trans.X(_region.Right + tx(r, -w * 2, 0)), _
                                  renderer.Trans.Y(_region.Bottom + ty(r, -w * 2, 0)))
                    Else
                        cb.LineTo(renderer.Trans.X(_region.Right), _
                                  renderer.Trans.Y(_region.Bottom))
                    End If
                    cb.Stroke()
                    If startArrow Then
                        cb.MoveTo(renderer.Trans.X(_region.Left + tx(r, w * 2, -w)), _
                                  renderer.Trans.Y(_region.Top + ty(r, w * 2, -w)))
                        cb.LineTo(renderer.Trans.X(_region.Left), _
                                  renderer.Trans.Y(_region.Top))
                        cb.LineTo(renderer.Trans.X(_region.Left + tx(r, w * 2, w)), _
                                  renderer.Trans.Y(_region.Top + ty(r, w * 2, w)))
                        cb.Fill()
                    End If
                    If endArrow Then
                        cb.MoveTo(renderer.Trans.X(_region.Right + tx(r, -w * 2, -w)), _
                                  renderer.Trans.Y(_region.Bottom + ty(r, -w * 2, -w)))
                        cb.LineTo(renderer.Trans.X(_region.Right), _
                                  renderer.Trans.Y(_region.Bottom))
                        cb.LineTo(renderer.Trans.X(_region.Right + tx(r, -w * 2, w)), _
                                  renderer.Trans.Y(_region.Bottom + ty(r, -w * 2, w)))
                        cb.Fill()
                    End If
                End If
            Finally
                cb.RestoreState()
            End Try
        End Sub

        Private Function tx(ByVal r As Single, ByVal x As Single, ByVal y As Single) As Single
            If x <> 0 Or y <> 0 Then
                Return x * Math.Cos(r) - y * Math.Sin(r)
            Else
                Return 0
            End If
        End Function

        Private Function ty(ByVal r As Single, ByVal x As Single, ByVal y As Single) As Single
            If x <> 0 Or y <> 0 Then
                Return x * Math.Sin(r) + y * Math.Cos(r)
            Else
                Return 0
            End If
        End Function

    End Class

End Namespace
