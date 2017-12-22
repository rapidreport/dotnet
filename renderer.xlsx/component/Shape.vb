Imports NPOI.XSSF.UserModel

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer.xlsx.elementrenderer

Namespace component
    Public Class Shape

        Public Renderer As IShapeRenderer = Nothing
        Public Region As Region = Nothing
        Public CellRange As New CellRange

        Public Function GetXSSFClientAnchor(topRow As Integer) As XSSFClientAnchor
            Return New XSSFClientAnchor(0, 0, 0, 0, _
                                        Math.Min(Me.CellRange.Col1, Me.CellRange.Col2), _
                                        Math.Min(Me.CellRange.Row1, Me.CellRange.Row2) + topRow, _
                                        Math.Max(Me.CellRange.Col1, Me.CellRange.Col2), _
                                        Math.Max(Me.CellRange.Row1, Me.CellRange.Row2) + topRow)
        End Function

        Public Function IsInverted() As Boolean
            Return ((Me.CellRange.Col2 - Me.CellRange.Col1) * _
                    (Me.CellRange.Row2 - Me.CellRange.Row1)) < 0
        End Function

    End Class
End Namespace