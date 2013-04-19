Imports NPOI.HSSF.UserModel

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer.xls.elementrenderer

Namespace component
    Public Class Shape

        Public Renderer As IShapeRenderer = Nothing
        Public Region As Region = Nothing
        Public CellRange As New CellRange

        Public Function GetHSSFClientAnchor(ByVal topRow As Integer) As HSSFClientAnchor
            Return New HSSFClientAnchor(0, 0, 0, 0, _
                                        Me.CellRange.Col1, _
                                        Me.CellRange.Row1 + topRow, _
                                        Me.CellRange.Col2, _
                                        Me.CellRange.Row2 + topRow)
        End Function

    End Class
End Namespace