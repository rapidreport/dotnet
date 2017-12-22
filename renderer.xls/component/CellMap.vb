Namespace component
    Public Class CellMap

        Public Map As List(Of List(Of Cell))

        Public Sub New(rows As Integer, cols As Integer, page As Page)
            Me.Map = New List(Of List(Of Cell))
            For i As Integer = 0 To rows
                Me.Map.Add(New List(Of Cell))
                For j As Integer = 0 To cols
                    Me.Map(i).Add(Nothing)
                Next
            Next
            Me.fillGrids(page.Grids)
            Me.fillFields(page.Fields)
        End Sub

        Private Function createOrGetCell(row As Integer, col As Integer) As Cell
            Dim ret As Cell = Me.Map(row)(col)
            If ret Is Nothing Then
                ret = New Cell(row, col)
                Me.Map(row)(col) = ret
            End If
            Return ret
        End Function

        Private Function createOrGetCellStyle(row As Integer, col As Integer) As CellStyle
            Dim cell As Cell = Me.createOrGetCell(row, col)
            If cell.Style Is Nothing Then
                cell.Style = New CellStyle
            End If
            Return cell.Style
        End Function

        Private Sub fillGrids(grids As List(Of Grid))
            For Each grid As Grid In grids
                If grid.Style.LeftBorder IsNot Nothing Then
                    For r As Integer = grid.CellRange.Row1 To grid.CellRange.Row2 - 1
                        Me.createOrGetCellStyle(r, grid.CellRange.Col1).GridStyle.LeftBorder = grid.Style.LeftBorder
                    Next
                End If
                If grid.Style.RightBorder IsNot Nothing Then
                    For r As Integer = grid.CellRange.Row1 To grid.CellRange.Row2 - 1
                        Me.createOrGetCellStyle(r, grid.CellRange.Col2 - 1).GridStyle.RightBorder = grid.Style.RightBorder
                    Next
                End If
                If grid.Style.TopBorder IsNot Nothing Then
                    For c As Integer = grid.CellRange.Col1 To grid.CellRange.Col2 - 1
                        Me.createOrGetCellStyle(grid.CellRange.Row1, c).GridStyle.TopBorder = grid.Style.TopBorder
                    Next
                End If
                If grid.Style.BottomBorder IsNot Nothing Then
                    For c As Integer = grid.CellRange.Col1 To grid.CellRange.Col2 - 1
                        Me.createOrGetCellStyle(grid.CellRange.Row2 - 1, c).GridStyle.BottomBorder = grid.Style.BottomBorder
                    Next
                End If
                If grid.Style.FillColor IsNot Nothing Then
                    For r As Integer = grid.CellRange.Row1 To grid.CellRange.Row2 - 1
                        For c As Integer = grid.CellRange.Col1 To grid.CellRange.Col2 - 1
                            Me.createOrGetCellStyle(r, c).GridStyle.FillColor = grid.Style.FillColor
                        Next
                    Next
                End If
            Next
        End Sub

        Private Sub fillFields(fields As List(Of Field))
            For Each field As Field In fields
                Me.createOrGetCellStyle(field.CellRange.Row1, field.CellRange.Col1).FieldStyle = field.Style
                Dim cell As Cell = Me.createOrGetCell(field.CellRange.Row1, field.CellRange.Col1)
                For r As Integer = field.CellRange.Row1 To field.CellRange.Row2 - 1
                    For c As Integer = field.CellRange.Col1 To field.CellRange.Col2 - 1
                        Dim _cell As Cell = Me.createOrGetCell(r, c)
                        Me.relocate(_cell)
                        _cell.MergedCell = cell
                    Next
                Next
                If TypeOf field.Data Is String Then
                    Dim t As String = field.Data
                    t = t.Replace(vbCrLf, vbLf)
                    cell.Data = t
                Else
                    cell.Data = field.Data
                End If
                cell.Range.Row2 = field.CellRange.Row2
                cell.Range.Col2 = field.CellRange.Col2
            Next
        End Sub

        Private Sub relocate(cell As Cell)
            If cell.MergedCell Is cell Then
                Dim r2 As Integer = cell.Range.Row2
                Dim c2 As Integer = cell.Range.Col2
                For r As Integer = cell.Range.Row1 To r2 - 1
                    For c As Integer = cell.Range.Col1 To c2 - 1
                        Me.createOrGetCell(r, c).Reset()
                    Next
                Next
            Else
                Dim _cell As Cell = cell.MergedCell
                If cell.Range.Row1 > _cell.Range.Row1 Then
                    For r As Integer = cell.Range.Row1 To _cell.Range.Row2 - 1
                        For c As Integer = _cell.Range.Col1 To _cell.Range.Col2 - 1
                            Me.createOrGetCell(r, c).Reset()
                        Next
                    Next
                    _cell.Range.Row2 = cell.Range.Row1
                Else
                    For c As Integer = cell.Range.Col1 To _cell.Range.Col2 - 1
                        For r As Integer = _cell.Range.Row1 To _cell.Range.Row2 - 1
                            Me.createOrGetCell(r, c).Reset()
                        Next
                    Next
                    _cell.Range.Col2 = cell.Range.Col1
                End If
            End If
        End Sub

    End Class
End Namespace