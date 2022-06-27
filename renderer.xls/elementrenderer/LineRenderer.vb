Imports jp.co.systembase.NPOI.HSSF.UserModel

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer.xls.component

Namespace elementrenderer
    Public Class LineRenderer
        Implements IElementRenderer

        Public Sub Collect( _
          renderer As XlsRenderer, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object) Implements IElementRenderer.Collect
            Dim _region As Region = region.ToPointScale(reportDesign)
            If Math.Abs(_region.GetWidth) < RowColUtil.TOLERANCE Then
                Dim bs As BorderStyle = BorderStyle.GetInstance(design, reportDesign)
                If bs IsNot Nothing Then
                    Dim grid As New Grid
                    If _region.Left > RowColUtil.TOLERANCE Then
                        grid.Style.RightBorder = bs
                    Else
                        grid.Style.LeftBorder = bs
                    End If
                    grid.Region = New Region
                    grid.Region.Top = Math.Min(_region.Top, _region.Bottom)
                    grid.Region.Bottom = Math.Max(_region.Top, _region.Bottom)
                    grid.Region.Left = _region.Left
                    grid.Region.Right = _region.Left
                    renderer.CurrentPage.Grids.Add(grid)
                End If
            ElseIf Math.Abs(_region.GetHeight) < RowColUtil.TOLERANCE Then
                Dim bs As BorderStyle = BorderStyle.GetInstance(design, reportDesign)
                If bs IsNot Nothing Then
                    Dim grid As New Grid
                    If _region.Top > RowColUtil.TOLERANCE Then
                        grid.Style.BottomBorder = bs
                    Else
                        grid.Style.TopBorder = bs
                    End If
                    grid.Region = New Region
                    grid.Region.Top = _region.Top
                    grid.Region.Bottom = _region.Top
                    grid.Region.Left = Math.Min(_region.Left, _region.Right)
                    grid.Region.Right = Math.Max(_region.Left, _region.Right)
                    renderer.CurrentPage.Grids.Add(grid)
                End If
            Else
                Dim shape As New Shape()
                shape.Renderer = New LineShapeRenderer(design, reportDesign)
                shape.Region = _region
                renderer.CurrentPage.Shapes.Add(shape)
            End If
        End Sub

        Public Class LineShapeRenderer
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
                    If lineWidth = 0 Then
                        Return
                    End If
                End If
                Dim s As HSSFSheet = Page.Renderer.Sheet
                Dim p As HSSFPatriarch = s.DrawingPatriarch
                Dim sp As HSSFSimpleShape = p.CreateSimpleShape(Shape.GetHSSFClientAnchor(Page.TopRow))
                sp.ShapeType = HSSFSimpleShape.OBJECT_TYPE_LINE
                If Not Me.Design.IsNull("color") Then
                    Dim c As System.Drawing.Color = RenderUtil.GetColor(Me.Design.Get("color"))
                    If Not c.Equals(Color.Empty) Then
                        sp.SetLineStyleColor(c.R, c.G, c.B)
                    End If
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