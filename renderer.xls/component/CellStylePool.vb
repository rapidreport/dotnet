Imports NPOI.HSSF.UserModel

Namespace component
    Public Class CellStylePool

        Public Renderer As XlsRenderer
        Public Map As New Dictionary(Of CellStyle, HSSFCellStyle)
        Public RevMap As New Dictionary(Of HSSFCellStyle, CellStyle)

        Public Sub New(ByVal renderer As XlsRenderer)
            Me.Renderer = renderer
        End Sub

        Public Function [Get](ByVal cellStyle As CellStyle) As HSSFCellStyle
            If Not Me.Map.ContainsKey(cellStyle) Then
                Me.Map.Add(cellStyle, Me.createStyle(cellStyle))
                Me.RevMap.Add(Me.Map(cellStyle), cellStyle)
            End If
            Return Me.Map(cellStyle)
        End Function

        Private Function createStyle(ByVal cellStyle As CellStyle) As HSSFCellStyle
            Dim ret As HSSFCellStyle = Me.Renderer.Workbook.CreateCellStyle()
            cellStyle.Fill(ret, Me.Renderer)
            Return ret
        End Function

    End Class
End Namespace