Imports NPOI.SS.UserModel

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer.xlsx.component
Imports jp.co.systembase.report.renderer.xlsx.imageloader

Namespace elementrenderer
    Public Class ImageRenderer
        Implements IElementRenderer

        Public Sub Collect( _
          ByVal renderer As XlsxRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Collect
            Dim _region As Region = region.ToPointScale(reportDesign)
            Dim image As Image = Nothing
            If Not design.IsNull("key") AndAlso data IsNot Nothing Then
                Dim key As String = design.Get("key")
                If renderer.ImageLoaderMap.ContainsKey(key) Then
                    image = renderer.ImageLoaderMap(key).GetImage(data)
                End If
            End If
            If image Is Nothing Then
                image = reportDesign.GetImage(design.Base, "image")
            End If
            Dim shape As New Shape
            shape.Region = _region
            shape.Renderer = New ImageShapeRenderer(image)
            renderer.CurrentPage.Shapes.Add(shape)
        End Sub

        Public Class ImageShapeRenderer
            Implements IShapeRenderer
            Public Image As Image
            Public Sub New(ByVal image As Image)
                Me.Image = image
            End Sub
            Public Sub Render(ByVal page As Page, ByVal shape As Shape) Implements IShapeRenderer.Render
                Dim index As Integer = page.Renderer.GetImageIndex(Me.Image)
                If index >= 0 Then
                    Dim p As IDrawing = page.Renderer.Sheet.CreateDrawingPatriarch
                    p.CreatePicture(shape.GetXSSFClientAnchor(page.TopRow), index)
                End If
            End Sub
        End Class

    End Class
End Namespace
