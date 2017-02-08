﻿Imports NPOI.SS.UserModel
Imports NPOI.XSSF.UserModel

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer.xlsx.component

Namespace elementrenderer
    Public Class CircleRenderer
        Implements IElementRenderer

        Public Sub Collect( _
          ByVal renderer As XlsxRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Collect
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
            Public Sub New(ByVal design As ElementDesign, _
                           ByVal reportDesign As ReportDesign)
                Me.Design = design
                Me.ReportDesign = reportDesign
            End Sub
            Public Sub Render(ByVal page As Page, ByVal shape As Shape) Implements IShapeRenderer.Render
                Dim lineWidth As Single = Me.ReportDesign.DefaultLineWidth
                If Not Design.IsNull("line_width") Then
                    lineWidth = Design.Get("line_width")
                    If lineWidth = 0 AndAlso Design.IsNull("fill_color") Then
                        Exit Sub
                    End If
                End If
                Dim s As XSSFSheet = page.Renderer.Sheet
                Dim p As XSSFDrawing = s.CreateDrawingPatriarch
                Dim sp As XSSFSimpleShape = p.CreateSimpleShape(shape.GetXSSFClientAnchor(page.TopRow))
                sp.ShapeType = ShapeTypes.Ellipse - 1
                If lineWidth > 0 Then
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
                Else
                    Dim t As Byte() = ColorUtil.GetTriplet(Me.Design.Get("fill_color"))
                    If t IsNot Nothing Then
                        sp.SetLineStyleColor(t(0), t(1), t(2))
                    End If
                    sp.LineWidth = 1
                End If
                If Not Me.Design.IsNull("fill_color") Then
                    Dim t As Byte() = ColorUtil.GetTriplet(Me.Design.Get("fill_color"))
                    If t IsNot Nothing Then
                        sp.SetFillColor(t(0), t(1), t(2))
                    Else
                        sp.IsNoFill = True
                    End If
                Else
                    sp.IsNoFill = True
                End If
                If Not Me.Design.IsNull("line_width") Then
                    sp.LineWidth = lineWidth
                End If
            End Sub
        End Class

    End Class
End Namespace