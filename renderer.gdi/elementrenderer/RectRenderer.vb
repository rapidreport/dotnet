Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Class RectRenderer
        Implements IElementRenderer

        Public Sub Render( _
          env As RenderingEnv, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object) Implements IElementRenderer.Render
            Dim _region As Region = region.ToPointScale(reportDesign)
            If _region.GetWidth < 0 Or _region.GetHeight < 0 Then
                Exit Sub
            End If
            Dim g As Graphics = env.Graphics
            Dim rd As Single = 0
            Dim t As Boolean = True
            Dim b As Boolean = True
            Dim l As Boolean = True
            Dim r As Boolean = True
            If Not design.IsNull("round") Then
                rd = design.Get("round")
            End If
            If design.Get("hide_top") Then
                t = False
            End If
            If design.Get("hide_bottom") Then
                b = False
            End If
            If design.Get("hide_left") Then
                l = False
            End If
            If design.Get("hide_right") Then
                r = False
            End If
            If Not design.IsNull("fill_color") Then
                Dim color As Color = GdiRenderUtil.GetColor(design.Get("fill_color"))
                If Not color.Equals(System.Drawing.Color.Empty) Then
                    Using br As New SolidBrush(color)
                        g.FillRectangle(br, _region.Left + rd, _region.Top + rd, _region.GetWidth - rd * 2, _region.GetHeight - rd * 2)
                        If rd > 0 Then
                            With Nothing
                                Dim _l As Single = _region.Left + IIf(l And t, rd, 0)
                                Dim _r As Single = _region.Right - IIf(r And t, rd, 0)
                                g.FillRectangle(br, _l, _region.Top, _r - _l, rd)
                            End With
                            With Nothing
                                Dim _l As Single = _region.Left + IIf(l And b, rd, 0)
                                Dim _r As Single = _region.Right - IIf(r And b, rd, 0)
                                g.FillRectangle(br, _l, _region.Bottom - rd, _r - _l, rd)
                            End With
                            With Nothing
                                Dim _t As Single = _region.Top + IIf(t And l, rd, 0)
                                Dim _b As Single = _region.Bottom - IIf(b And l, rd, 0)
                                g.FillRectangle(br, _region.Left, _t, rd, _b - _t)
                            End With
                            With Nothing
                                Dim _t As Single = _region.Top + IIf(t And r, rd, 0)
                                Dim _b As Single = _region.Bottom - IIf(b And r, rd, 0)
                                g.FillRectangle(br, _region.Right - rd, _t, rd, _b - _t)
                            End With
                            If t And l Then
                                g.FillPie(br, _region.Left, _region.Top, rd * 2, rd * 2, 180, 90)
                            End If
                            If t And r Then
                                g.FillPie(br, _region.Right - rd * 2, _region.Top, rd * 2, rd * 2, 270, 90)
                            End If
                            If b And l Then
                                g.FillPie(br, _region.Left, _region.Bottom - rd * 2, rd * 2, rd * 2, 90, 90)
                            End If
                            If b And r Then
                                g.FillPie(br, _region.Right - rd * 2, _region.Bottom - rd * 2, rd * 2, rd * 2, 0, 90)
                            End If
                        End If
                    End Using
                End If
            End If
            With Nothing
                Dim color As Color = color.Black
                Dim lineWidth As Single = reportDesign.DefaultLineWidth
                If Not design.IsNull("color") Then
                    color = GdiRenderUtil.GetColor(design.Get("color"), color)
                End If
                If Not design.IsNull("line_width") Then
                    lineWidth = design.Get("line_width")
                End If
                If lineWidth > 0 Then
                    Using p As New Pen(color, lineWidth)
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
                        If t And b And l And r And rd = 0 Then
                            g.DrawRectangle(p, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight)
                        Else
                            If t Then
                                Dim _l As Single = _region.Left + IIf(l, rd, 0) - lineWidth / 2
                                Dim _r As Single = _region.Right - IIf(r, rd, 0) + lineWidth / 2
                                g.DrawLine(p, _l, _region.Top, _r, _region.Top)
                            End If
                            If b Then
                                Dim _l As Single = _region.Left + IIf(l, rd, 0) - lineWidth / 2
                                Dim _r As Single = _region.Right - IIf(r, rd, 0) + lineWidth / 2
                                g.DrawLine(p, _l, _region.Bottom, _r, _region.Bottom)
                            End If
                            If l Then
                                Dim _t As Single = _region.Top + IIf(t, rd, 0) - lineWidth / 2
                                Dim _b As Single = _region.Bottom - IIf(b, rd, 0) + lineWidth / 2
                                g.DrawLine(p, _region.Left, _t, _region.Left, _b)
                            End If
                            If r Then
                                Dim _t As Single = _region.Top + IIf(t, rd, 0) - lineWidth / 2
                                Dim _b As Single = _region.Bottom - IIf(b, rd, 0) + lineWidth / 2
                                g.DrawLine(p, _region.Right, _t, _region.Right, _b)
                            End If
                            If rd > 0 Then
                                If t And l Then
                                    g.DrawArc(p, _region.Left, _region.Top, rd * 2, rd * 2, 180, 90)
                                End If
                                If t And r Then
                                    g.DrawArc(p, _region.Right - rd * 2, _region.Top, rd * 2, rd * 2, 270, 90)
                                End If
                                If b And l Then
                                    g.DrawArc(p, _region.Left, _region.Bottom - rd * 2, rd * 2, rd * 2, 90, 90)
                                End If
                                If b And r Then
                                    g.DrawArc(p, _region.Right - rd * 2, _region.Bottom - rd * 2, rd * 2, rd * 2, 0, 90)
                                End If
                            End If
                        End If
                    End Using
                End If
            End With
        End Sub

    End Class

End Namespace