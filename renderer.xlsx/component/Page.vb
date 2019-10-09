Imports jp.co.systembase.report.component

Namespace component
    Public Class Page

        Public Renderer As XlsxRenderer
        Public Grids As New List(Of Grid)
        Public Fields As New List(Of Field)
        Public Shapes As New List(Of Shape)
        Public TopRow As Integer = 0

        Public Sub New(
          renderer As XlsxRenderer,
          reportDesign As ReportDesign,
          paperRegion As Region)
            Me.Renderer = renderer
            If paperRegion IsNot Nothing Then
                Dim grid As New Grid
                grid.Region = paperRegion.ToPointScale(reportDesign)
                Me.Grids.Add(grid)
            End If
        End Sub

    End Class
End Namespace