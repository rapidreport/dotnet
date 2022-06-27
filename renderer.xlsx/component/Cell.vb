Imports jp.co.systembase.NPOI.XSSF.UserModel
Imports jp.co.systembase.NPOI.SS.Util

Namespace component
    Public Class Cell

        Public Style As CellStyle = Nothing
        Public Data As Object = Nothing
        Public Range As New CellRange
        Public MergedCell As Cell = Nothing

        Public Sub New(row As Integer, col As Integer)
            Me.Range.Row1 = row
            Me.Range.Col1 = col
            Me.Reset()
        End Sub

        Public Sub Reset()
            Me.Data = Nothing
            Me.Range.Row2 = Me.Range.Row1 + 1
            Me.Range.Col2 = Me.Range.Col1 + 1
            Me.MergedCell = Me
        End Sub

        Public Sub Render(page As Page)
            Dim s As XSSFSheet = page.Renderer.Sheet
            Dim hssfCell As XSSFCell = Nothing
            If Me.Style IsNot Nothing Then
                Dim cellStyle As XSSFCellStyle = page.Renderer.CellStylePool.Get(Me.Style)
                hssfCell = s.GetRow(page.TopRow + Me.Range.Row1).CreateCell(Me.Range.Col1)
                hssfCell.CellStyle = cellStyle
            End If
            If Me.MergedCell Is Me AndAlso _
              (Me.Range.Row1 < Me.Range.Row2 - 1 OrElse Me.Range.Col1 < Me.Range.Col2 - 1) Then
                s.AddMergedRegion(New CellRangeAddress( _
                                  page.TopRow + Me.Range.Row1, _
                                  page.TopRow + Me.Range.Row2 - 1, _
                                  Me.Range.Col1, _
                                  Me.Range.Col2 - 1))
            End If
            If hssfCell Is Nothing Then
                hssfCell = s.GetRow(page.TopRow + Me.Range.Row1).CreateCell(Me.Range.Col1)
            End If
            If TypeOf Me.Data Is String Then
                hssfCell.SetCellValue(CType(Me.Data, String))
            ElseIf TypeOf Me.Data Is Short OrElse _
              TypeOf Me.Data Is Integer OrElse _
              TypeOf Me.Data Is Long OrElse _
              TypeOf Me.Data Is UShort OrElse _
              TypeOf Me.Data Is UInteger OrElse _
              TypeOf Me.Data Is ULong OrElse _
              TypeOf Me.Data Is Single OrElse _
              TypeOf Me.Data Is Double OrElse _
              TypeOf Me.Data Is Byte OrElse _
              TypeOf Me.Data Is Decimal Then
                hssfCell.SetCellValue(CType(Me.Data, Double))
            ElseIf TypeOf Me.Data Is Boolean Then
                hssfCell.SetCellValue(CType(Me.Data, Boolean))
            ElseIf TypeOf Me.Data Is Date Then
                hssfCell.SetCellValue(CType(Me.Data, Date))
            End If
        End Sub

    End Class
End Namespace