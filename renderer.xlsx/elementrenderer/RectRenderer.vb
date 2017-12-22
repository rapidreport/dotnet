Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer.xlsx.component

Namespace elementrenderer
    Public Class RectRenderer
        Implements IElementRenderer

        Public Sub Collect( _
          renderer As XlsxRenderer, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object) Implements IElementRenderer.Collect
            Dim _region As Region = region.ToPointScale(reportDesign)
            If _region.GetWidth <= 0 Or _region.GetHeight <= 0 Then
                Exit Sub
            End If
            Dim grid As New Grid
            grid.Region = _region
            With Nothing
                Dim bs As BorderStyle = BorderStyle.GetInstance(design, reportDesign)
                If bs IsNot Nothing Then
                    If Not design.Get("hide_top") Then
                        grid.Style.TopBorder = bs
                    End If
                    If Not design.Get("hide_bottom") AndAlso _
                      Math.Abs(_region.GetHeight) > RowColUtil.TOLERANCE Then
                        grid.Style.BottomBorder = bs
                    End If
                    If Not design.Get("hide_left") Then
                        grid.Style.LeftBorder = bs
                    End If
                    If Not design.Get("hide_right") AndAlso _
                      Math.Abs(_region.GetHeight) > RowColUtil.TOLERANCE Then
                        grid.Style.RightBorder = bs
                    End If
                End If
            End With
            If Not design.IsNull("fill_color") Then
                grid.Style.FillColor = design.Get("fill_color")
            End If
            renderer.CurrentPage.Grids.Add(grid)
        End Sub

    End Class
End Namespace
