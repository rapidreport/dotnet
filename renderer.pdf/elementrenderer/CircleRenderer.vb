Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Class CircleRenderer
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
            Try
                Dim stroke As Boolean = True
                Dim fill As Boolean = False
                Dim lw As Single = reportDesign.DefaultLineWidth
                If Not design.IsNull("line_width") Then
                    lw = design.Get("line_width")
                    If lw = 0 Then
                        stroke = False
                    End If
                End If
                If stroke Then
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
                End If
                If Not design.IsNull("fill_color") Then
                    Dim c As Color = PdfRenderUtil.GetColor(design.Get("fill_color"))
                    If c IsNot Nothing Then
                        fill = True
                        cb.SetColorFill(c)
                    End If
                End If
                If stroke Or fill Then
                    cb.Ellipse( _
                      renderer.Trans.X(_region.Left), _
                      renderer.Trans.Y(_region.Bottom), _
                      renderer.Trans.X(_region.Right), _
                      renderer.Trans.Y(_region.Top))
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
        End Sub

    End Class

End Namespace
