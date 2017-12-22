Imports NPOI.HSSF.UserModel

Namespace component
    Public Class CellStylePool

        Public Renderer As XlsRenderer
        Public Map As New Dictionary(Of CellStyle, HSSFCellStyle)

        Public Sub New(renderer As XlsRenderer)
            Me.Renderer = renderer
        End Sub

        Public Function [Get](cellStyle As CellStyle) As HSSFCellStyle
            If Not Me.Map.ContainsKey(cellStyle) Then
                Me.Map.Add(cellStyle, Me.createStyle(cellStyle))
            End If
            Return Me.Map(cellStyle)
        End Function

        Private Function createStyle(cellStyle As CellStyle) As HSSFCellStyle
            Dim ret As HSSFCellStyle = Me.Renderer.Workbook.CreateCellStyle()
            cellStyle.Fill(ret, Me.Renderer)
            Return ret
        End Function

    End Class
End Namespace