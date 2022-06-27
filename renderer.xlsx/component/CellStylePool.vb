Imports jp.co.systembase.NPOI.XSSF.UserModel

Namespace component
    Public Class CellStylePool

        Public Renderer As XlsxRenderer
        Public Map As New Dictionary(Of CellStyle, XSSFCellStyle)

        Private l As New List(Of XSSFCellStyle)

        Public Sub New(renderer As XlsxRenderer)
            Me.Renderer = renderer
        End Sub

        Public Function [Get](cellStyle As CellStyle) As XSSFCellStyle            
            If Not Me.Map.ContainsKey(cellStyle) Then
                Me.Map.Add(cellStyle, Me.createStyle(cellStyle))
            End If
            Return Me.Map(cellStyle)
        End Function

        Private Function createStyle(cellStyle As CellStyle) As XSSFCellStyle
            Dim ret As XSSFCellStyle = Me.Renderer.Workbook.CreateCellStyle()
            cellStyle.Fill(ret, Me.Renderer)
            Return ret
        End Function

    End Class
End Namespace