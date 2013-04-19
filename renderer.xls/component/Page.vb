Imports jp.co.systembase.report.component

Namespace component
    Public Class Page

        Public Renderer As XlsRenderer
        Public Grids As New List(Of Grid)
        Public Fields As New List(Of Field)
        Public Shapes As New List(Of Shape)
        Public TopRow As Integer = 0

        Public Sub New( _
          ByVal renderer As XlsRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal paperRegion As Region)
            Me.Renderer = renderer
            Dim grid As New Grid
            grid.Region = paperRegion.ToPointScale(reportDesign)
            Me.Grids.Add(grid)
        End Sub

    End Class
End Namespace