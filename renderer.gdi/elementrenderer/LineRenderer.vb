Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Class LineRenderer
        Implements IElementRenderer

        Public Sub Render( _
          env As RenderingEnv, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object) Implements IElementRenderer.Render
            Dim _region As Region = region.ToPointScale(reportDesign)
            Dim g As Graphics = env.Graphics
            Dim c As Color = Color.Black
            Dim lw As Single = reportDesign.DefaultLineWidth
            If Not design.IsNull("color") Then
                c = GdiRenderUtil.GetColor(design.Get("color"), c)
            End If
            If Not design.IsNull("line_width") Then
                lw = design.Get("line_width")
            End If
            If lw > 0 Then
                Using p As New Pen(c, lw)
                    If Not design.IsNull("line_pattern") Then
                        Dim pt As String() = CType(design.Get("line_pattern"), String).Split(",")
                        Dim ptl As New List(Of Single)
                        For Each v As String In pt
                            If Char.IsDigit(v) AndAlso v > 0 Then
                                ptl.Add(v)
                            End If
                        Next
                        If ptl.Count > 1 Then
                            p.DashPattern = ptl.ToArray
                            p.DashStyle = Drawing2D.DashStyle.Custom
                        End If
                    ElseIf Not design.IsNull("line_style") Then
                        Select Case design.Get("line_style")
                            Case "dot"
                                p.DashStyle = Drawing2D.DashStyle.Dot
                            Case "dash"
                                p.DashStyle = Drawing2D.DashStyle.Dash
                            Case "dashdot"
                                p.DashStyle = Drawing2D.DashStyle.DashDot
                        End Select
                    End If
                    If design.Get("doublet") Then
                        Dim r As Double = Math.Atan2(_region.GetHeight, _region.GetWidth) + Math.PI / 2
                        Dim dx As Single = Math.Cos(r) * Math.Max(lw, 0.7F)
                        Dim dy As Single = Math.Sin(r) * Math.Max(lw, 0.7F)
                        g.DrawLine(p, _region.Left + dx, _region.Top + dy, _region.Right + dx, _region.Bottom + dy)
                        g.DrawLine(p, _region.Left - dx, _region.Top - dy, _region.Right - dx, _region.Bottom - dy)
                    Else
                        Dim startArrow As Boolean = design.Get("start_cap") = "arrow"
                        Dim endArrow As Boolean = design.Get("end_cap") = "arrow"
                        Dim r As Single = Math.Atan2(_region.GetHeight, _region.GetWidth)
                        Dim w As Single = lw + 2
                        g.DrawLine(p, _
                                   _region.Left + IIf(startArrow, tx(r, w * 2, 0), 0), _
                                   _region.Top + IIf(startArrow, ty(r, w * 2, 0), 0), _
                                   _region.Right + IIf(endArrow, tx(r, -w * 2, 0), 0), _
                                   _region.Bottom + IIf(endArrow, ty(r, -w * 2, 0), 0))
                        If startArrow Then
                            Using b As New SolidBrush(c)
                                g.FillPolygon(b, New PointF() { _
                                              New PointF(_region.Left + tx(r, w * 2, -w), _region.Top + ty(r, w * 2, -w)), _
                                              New PointF(_region.Left, _region.Top), _
                                              New PointF(_region.Left + tx(r, w * 2, w), _region.Top + ty(r, w * 2, w))})
                            End Using
                        End If
                        If endArrow Then
                            Using b As New SolidBrush(c)
                                g.FillPolygon(b, New PointF() { _
                                              New PointF(_region.Right + tx(r, -w * 2, -w), _region.Bottom + ty(r, -w * 2, -w)), _
                                              New PointF(_region.Right, _region.Bottom), _
                                              New PointF(_region.Right + tx(r, -w * 2, w), _region.Bottom + ty(r, -w * 2, w))})
                            End Using
                        End If
                    End If
                End Using
            End If
        End Sub

        Private Function tx(r As Single, x As Single, y As Single) As Single
            If x <> 0 Or y <> 0 Then
                Return x * Math.Cos(r) - y * Math.Sin(r)
            Else
                Return 0
            End If
        End Function

        Private Function ty(r As Single, x As Single, y As Single) As Single
            If x <> 0 Or y <> 0 Then
                Return x * Math.Sin(r) + y * Math.Cos(r)
            Else
                Return 0
            End If
        End Function

    End Class

End Namespace