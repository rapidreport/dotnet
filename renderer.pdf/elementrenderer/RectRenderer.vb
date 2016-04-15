Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Class RectRenderer
        Implements IElementRenderer

        Public Sub Render( _
          ByVal renderer As PdfRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Render
            Dim _region As Region = region.ToPointScale(reportDesign)
            Dim cb As PdfContentByte = renderer.Writer.DirectContent
            Dim x1 As Single = renderer.Trans.X(_region.Left)
            Dim x2 As Single = renderer.Trans.X(_region.Right)
            Dim y1 As Single = renderer.Trans.Y(_region.Bottom)
            Dim y2 As Single = renderer.Trans.Y(_region.Top)
            If x2 < x1 OrElse y2 < y1 Then
                Exit Sub
            End If
            Dim rd As Single = 0
            Dim t As Boolean = Not design.Get("hide_top")
            Dim b As Boolean = Not design.Get("hide_bottom")
            Dim l As Boolean = Not design.Get("hide_left")
            Dim r As Boolean = Not design.Get("hide_right")
            If Not design.IsNull("round") Then
                rd = design.Get("round")
            End If
            If t And b And l And r Then
                cb.SaveState()
                Try
                    Dim stroke As Boolean = Me.setupStroke(cb, design, reportDesign)
                    Dim fill As Boolean = Me.setupFill(cb, design)
                    If stroke Or fill Then
                        If rd = 0 Then
                            cb.Rectangle(x1, y1, x2 - x1, y2 - y1)
                        Else
                            cb.RoundRectangle(x1, y1, x2 - x1, y2 - y1, rd)
                        End If
                        If stroke And fill Then
                            cb.FillStroke()
                        ElseIf stroke Then
                            cb.Stroke()
                        ElseIf fill Then
                            cb.Fill()
                        End If
                    End If
                Finally
                    cb.RestoreState()
                End Try
            Else
                Dim _rd As Single = rd * 0.4477
                cb.SaveState()
                Try
                    If Me.setupFill(cb, design) Then
                        cb.MoveTo(x1 + IIf(b And l, rd, 0), y1)
                        cb.LineTo(x2 - IIf(b And r, rd, 0), y1)
                        If rd > 0 And b And r Then
                            cb.CurveTo(x2 - _rd, y1, x2, y1 + _rd, x2, y1 + rd)
                        End If
                        cb.LineTo(x2, y2 - IIf(t And r, rd, 0))
                        If rd > 0 And t And r Then
                            cb.CurveTo(x2, y2 - _rd, x2 - _rd, y2, x2 - rd, y2)
                        End If
                        cb.LineTo(x1 + IIf(t And l, rd, 0), y2)
                        If rd > 0 And t And l Then
                            cb.CurveTo(x1 + _rd, y2, x1, y2 - _rd, x1, y2 - rd)
                        End If
                        cb.LineTo(x1, y1 + IIf(b And l, rd, 0))
                        If rd > 0 And b And l Then
                            cb.CurveTo(x1, y1 + _rd, x1 + _rd, y1, x1 + rd, y1)
                        End If
                        cb.Fill()
                    End If
                Finally
                    cb.RestoreState()
                End Try
                If t Or b Or l Or r Then
                    cb.SaveState()
                    If Me.setupStroke(cb, design, reportDesign) Then
                        Dim lw As Single = 0
                        If Not design.IsNull("line_width") Then
                            lw = design.Get("line_width") / 2
                        End If
                        cb.MoveTo(x1 + IIf(l, rd, -lw), y1)
                        If b Then
                            cb.LineTo(x2 + IIf(r, -rd, lw), y1)
                        Else
                            cb.MoveTo(x2, y1 - lw)
                        End If
                        If rd > 0 And b And r Then
                            cb.CurveTo(x2 - _rd, y1, x2, y1 + _rd, x2, y1 + rd)
                        End If
                        If r Then
                            cb.LineTo(x2, y2 + IIf(t, -rd, lw))
                        Else
                            cb.MoveTo(x2 + lw, y2)
                        End If
                        If rd > 0 And t And r Then
                            cb.CurveTo(x2, y2 - _rd, x2 - _rd, y2, x2 - rd, y2)
                        End If
                        If t Then
                            cb.LineTo(x1 + If(l, rd, -lw), y2)
                        Else
                            cb.MoveTo(x1, y2 + lw)
                        End If
                        If rd > 0 And t And l Then
                            cb.CurveTo(x1 + _rd, y2, x1, y2 - _rd, x1, y2 - rd)
                        End If
                        If l Then
                            cb.LineTo(x1, y1 + IIf(b, rd, -lw))
                        Else
                            cb.MoveTo(x1, y1 - lw)
                        End If
                        If rd > 0 And b And l Then
                            cb.CurveTo(x1, y1 + _rd, x1 + _rd, y1, x1 + rd, y1)
                        End If
                        cb.Stroke()
                    End If
                    cb.RestoreState()
                End If
            End If
        End Sub

        Private Function setupStroke( _
          ByVal cb As PdfContentByte, _
          ByVal design As ElementDesign, _
          ByVal reportDesign As ReportDesign) As Boolean
            Dim lw As Single = reportDesign.DefaultLineWidth
            If Not design.IsNull("line_width") Then
                lw = design.Get("line_width")
                If lw = 0 Then
                    Return False
                End If
            End If
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
            Return True
        End Function

        Private Function setupFill(ByVal cb As PdfContentByte, ByVal d As ElementDesign) As Boolean
            If Not d.IsNull("fill_color") Then
                Dim c As Color = PdfRenderUtil.GetColor(d.Get("fill_color"))
                If c IsNot Nothing Then
                    cb.SetColorFill(c)
                    Return True
                End If
            End If
            Return False
        End Function

    End Class

End Namespace