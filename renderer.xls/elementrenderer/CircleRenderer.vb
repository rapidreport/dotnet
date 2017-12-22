Imports NPOI.HSSF.UserModel

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer.xls.component

Namespace elementrenderer
    Public Class CircleRenderer
        Implements IElementRenderer

        Public Sub Collect( _
          renderer As XlsRenderer, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object) Implements IElementRenderer.Collect
            Dim _region As Region = region.ToPointScale(reportDesign)
            Dim shape As New Shape
            shape.Renderer = New CircleShapeRenderer(design, reportDesign)
            shape.Region = _region
            renderer.CurrentPage.Shapes.Add(shape)
        End Sub

        Public Class CircleShapeRenderer
            Implements IShapeRenderer
            Public Design As ElementDesign
            Public ReportDesign As ReportDesign
            Public Sub New(design As ElementDesign, _
                           reportDesign As ReportDesign)
                Me.Design = design
                Me.ReportDesign = reportDesign
            End Sub
            Public Sub Render(Page As Page, Shape As Shape) Implements IShapeRenderer.Render
                Dim lineWidth As Single = Me.ReportDesign.DefaultLineWidth
                If Not Design.IsNull("line_width") Then
                    lineWidth = Design.Get("line_width")
                    If lineWidth = 0 AndAlso Design.IsNull("fill_color") Then
                        Exit Sub
                    End If
                End If
                Dim s As HSSFSheet = Page.Renderer.Sheet
                Dim p As HSSFPatriarch = s.DrawingPatriarch
                Dim sp As HSSFSimpleShape = p.CreateSimpleShape(Shape.GetHSSFClientAnchor(Page.TopRow))
                sp.ShapeType = HSSFSimpleShape.OBJECT_TYPE_OVAL
                If Not Me.Design.IsNull("color") Then
                    Dim c As Color = ColorUtil.GetColor(Me.Design.Get("color"))
                    If Not c.Equals(Color.Empty) Then
                        sp.SetLineStyleColor(c.R, c.G, c.B)
                    End If
                End If
                If Not Me.Design.IsNull("fill_color") Then
                    Dim c As Color = ColorUtil.GetColor(Me.Design.Get("fill_color"))
                    If Not c.Equals(Color.Empty) Then
                        sp.SetFillColor(c.R, c.G, c.B)
                    End If
                Else
                    sp.IsNoFill = True
                End If
                If Not Me.Design.IsNull("line_width") Then
                    sp.LineWidth = HSSFSimpleShape.LINEWIDTH_ONE_PT * lineWidth
                End If
                If lineWidth = 0 Then
                    sp.LineStyle = NPOI.SS.UserModel.LineStyle.None
                ElseIf Not Me.Design.IsNull("line_style") Then
                    Dim ls As String = Me.Design.Get("line_style")
                    If ls = "dot" Then
                        sp.LineStyle = NPOI.SS.UserModel.LineStyle.DotSys
                    ElseIf ls = "dash" Then
                        sp.LineStyle = NPOI.SS.UserModel.LineStyle.DashSys
                    ElseIf ls = "dashdot" Then
                        sp.LineStyle = NPOI.SS.UserModel.LineStyle.DashDotSys
                    End If
                End If
            End Sub
        End Class

    End Class
End Namespace