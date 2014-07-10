﻿Imports NPOI.XSSF.UserModel

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer.xlsx.component

Namespace elementrenderer
    Public Class LineRenderer
        Implements IElementRenderer

        Public Sub Collect( _
          ByVal renderer As XlsxRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Collect
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
            Public Sub New(ByVal design As ElementDesign, _
                           ByVal reportDesign As ReportDesign)
                Me.Design = design
                Me.ReportDesign = reportDesign
            End Sub
            Public Sub Render(ByVal page As Page, ByVal shape As Shape) Implements IShapeRenderer.Render
                Dim lineWidth As Single = Me.ReportDesign.DefaultLineWidth
                If Not Design.IsNull("line_width") Then
                    lineWidth = Design.Get("line_width")
                    If lineWidth = 0 Then
                        Return
                    End If
                End If
                Dim s As XSSFSheet = page.Renderer.Sheet
                Dim p As XSSFDrawing = s.CreateDrawingPatriarch
                Dim sp As XSSFSimpleShape = p.CreateSimpleShape(shape.GetXSSFClientAnchor(page.TopRow))
                If shape.IsInverted Then
                    sp.SetShapeType(NPOI.SS.UserModel.ShapeTypes.LineInv - 1)
                Else
                    sp.SetShapeType(NPOI.SS.UserModel.ShapeTypes.Line - 1)
                End If
                sp.SetLineStyleColor(0, 0, 0)
                sp.LineWidth = lineWidth
                If Not Me.Design.IsNull("color") Then
                    Dim t As Byte() = ColorUtil.GetTriplet(Me.Design.Get("color"))
                    If t IsNot Nothing Then
                        sp.SetLineStyleColor(t(0), t(1), t(2))
                    End If
                End If
                If Not Me.Design.IsNull("line_style") Then
                    Dim ls As String = Me.Design.Get("line_style")
                    If ls = "dot" Then
                        sp.LineStyle = IElementRenderer.ELineStyle.DOT
                    ElseIf ls = "dash" Then
                        sp.LineStyle = IElementRenderer.ELineStyle.DASH
                    ElseIf ls = "dashdot" Then
                        sp.LineStyle = IElementRenderer.ELineStyle.DASHDOT
                    End If
                End If
            End Sub
        End Class

    End Class
End Namespace