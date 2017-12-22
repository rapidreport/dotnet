Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Class CircleRenderer
        Implements IElementRenderer

        Public Sub Render( _
          env As RenderingEnv, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object) Implements IElementRenderer.Render
            Dim _region As Region = region.ToPointScale(reportDesign)
            Dim g As Graphics = env.Graphics
            If Not design.IsNull("fill_color") Then
                Dim color As Color = GdiRenderUtil.GetColor(design.Get("fill_color"))
                If Not color.Equals(System.Drawing.Color.Empty) Then
                    Using b As New SolidBrush(color)
                        g.FillEllipse(b, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight)
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
                        g.DrawEllipse(p, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight)
                    End Using
                End If
            End With
        End Sub

    End Class
End Namespace