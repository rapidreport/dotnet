Namespace component
    Module RowColUtil

        Public Const TOLERANCE As Single = 1.0F

        Public Function CreateColWidths(cols As List(Of Single), coefficient As Single) As List(Of Integer)
            Dim ret As New List(Of Integer)
            Dim last As Integer = 0
            For i As Integer = 1 To cols.Count - 1
                Dim t As Integer = Fix(cols(i) * coefficient)
                ret.Add((t - last) * 36.55)
                last = t
            Next
            Return ret
        End Function

        Public Function CreateRowHeights(rows As List(Of Single), coefficient As Single) As List(Of Integer)
            Dim ret As New List(Of Integer)
            Dim last As Integer = 0
            For i As Integer = 1 To rows.Count - 1
                Dim t As Integer = Fix(rows(i) * coefficient)
                ret.Add((t - last) * 15)
                last = t
            Next
            Return ret
        End Function

        Public Function CreateCols(reportDesign As ReportDesign, renderer As XlsxRenderer)
            Dim l As New List(Of _IContainer)
            For Each page As Page In renderer.Pages
                For Each grid As Grid In page.Grids
                    l.Add(New LeftGridContainer(grid))
                    l.Add(New RightGridContainer(grid))
                Next
                For Each field As Field In page.Fields
                    l.Add(New LeftFieldContainer(field))
                    l.Add(New RightFieldContainer(field))
                Next
                For Each shape As Shape In page.Shapes
                    l.Add(New LeftShapeContainer(shape))
                    l.Add(New RightShapeContainer(shape))
                Next
            Next
            Return createRowCols_aux(l, renderer.Setting.ColWidthMax)
        End Function

        Public Function CreateRows(reportDesign As ReportDesign, page As Page)
            Dim l As New List(Of _IContainer)
            For Each grid As Grid In page.Grids
                l.Add(New TopGridContainer(grid))
                l.Add(New BottomGridContainer(grid))
            Next
            For Each field As Field In page.Fields
                l.Add(New TopFieldContainer(field))
                l.Add(New BottomFieldContainer(field))
            Next
            For Each shape As Shape In page.Shapes
                l.Add(New TopShapeContainer(shape))
                l.Add(New BottomShapeContainer(shape))
            Next
            Return createRowCols_aux(l, page.Renderer.Setting.RowHeightMax)
        End Function

        Private Function createRowCols_aux(l As List(Of _IContainer), max As Single)
            Dim ret As New List(Of Single)
            l.Sort(New _ContainerComparator)
            Dim pointPos As Single = 0
            Dim pos As Integer = 0
            ret.Add(pointPos)
            For Each c As _IContainer In l
                Dim _pointPos As Single = c.GetPointPos
                If _pointPos - pointPos < TOLERANCE Then
                    c.SetPos(pos)
                Else
                    Do While pointPos + max < _pointPos
                        pos += 1
                        pointPos += max
                        ret.Add(pointPos)
                    Loop
                    pos += 1
                    pointPos = _pointPos
                    c.SetPos(pos)
                    ret.Add(pointPos)
                End If
            Next
            Return ret
        End Function

        Private Interface _IContainer
            Function GetPointPos() As Single
            Sub SetPos(pos As Integer)
        End Interface

        Private Class _ContainerComparator
            Implements IComparer(Of _IContainer)
            Public Function Compare(o1 As _IContainer, o2 As _IContainer) As Integer Implements IComparer(Of _IContainer).Compare
                Dim v As Single = o1.GetPointPos - o2.GetPointPos
                If v < 0 Then
                    Return -1
                ElseIf v > 0 Then
                    Return 1
                Else
                    Return 0
                End If
            End Function
        End Class

        Private Class LeftGridContainer
            Implements _IContainer
            Private grid As Grid
            Public Sub New(grid As Grid)
                Me.grid = grid
            End Sub
            Public Function GetPointPos() As Single Implements _IContainer.GetPointPos
                Return Me.grid.Region.Left
            End Function
            Public Sub SetPos(pos As Integer) Implements _IContainer.SetPos
                Me.grid.CellRange.Col1 = pos
            End Sub
        End Class

        Private Class RightGridContainer
            Implements _IContainer
            Private grid As Grid
            Public Sub New(grid As Grid)
                Me.grid = grid
            End Sub
            Public Function GetPointPos() As Single Implements _IContainer.GetPointPos
                Return Me.grid.Region.Right
            End Function
            Public Sub SetPos(pos As Integer) Implements _IContainer.SetPos
                Me.grid.CellRange.Col2 = pos
            End Sub
        End Class

        Private Class TopGridContainer
            Implements _IContainer
            Private grid As Grid
            Public Sub New(grid As Grid)
                Me.grid = grid
            End Sub
            Public Function GetPointPos() As Single Implements _IContainer.GetPointPos
                Return Me.grid.Region.Top
            End Function
            Public Sub SetPos(pos As Integer) Implements _IContainer.SetPos
                Me.grid.CellRange.Row1 = pos
            End Sub
        End Class

        Private Class BottomGridContainer
            Implements _IContainer
            Private grid As Grid
            Public Sub New(grid As Grid)
                Me.grid = grid
            End Sub
            Public Function GetPointPos() As Single Implements _IContainer.GetPointPos
                Return Me.grid.Region.Bottom
            End Function
            Public Sub SetPos(pos As Integer) Implements _IContainer.SetPos
                Me.grid.CellRange.Row2 = pos
            End Sub
        End Class

        Private Class LeftFieldContainer
            Implements _IContainer
            Private field As Field
            Public Sub New(field As Field)
                Me.field = field
            End Sub
            Public Function GetPointPos() As Single Implements _IContainer.GetPointPos
                Return Me.field.Region.Left
            End Function
            Public Sub SetPos(pos As Integer) Implements _IContainer.SetPos
                Me.field.CellRange.Col1 = pos
            End Sub
        End Class

        Private Class RightFieldContainer
            Implements _IContainer
            Private field As Field
            Public Sub New(field As Field)
                Me.field = field
            End Sub
            Public Function GetPointPos() As Single Implements _IContainer.GetPointPos
                Return Me.field.Region.Right
            End Function
            Public Sub SetPos(pos As Integer) Implements _IContainer.SetPos
                Me.field.CellRange.Col2 = pos
            End Sub
        End Class

        Private Class TopFieldContainer
            Implements _IContainer
            Private field As Field
            Public Sub New(field As Field)
                Me.field = field
            End Sub
            Public Function GetPointPos() As Single Implements _IContainer.GetPointPos
                Return Me.field.Region.Top
            End Function
            Public Sub SetPos(pos As Integer) Implements _IContainer.SetPos
                Me.field.CellRange.Row1 = pos
            End Sub
        End Class

        Private Class BottomFieldContainer
            Implements _IContainer
            Private field As Field
            Public Sub New(field As Field)
                Me.field = field
            End Sub
            Public Function GetPointPos() As Single Implements _IContainer.GetPointPos
                Return Me.field.Region.Bottom
            End Function
            Public Sub SetPos(pos As Integer) Implements _IContainer.SetPos
                Me.field.CellRange.Row2 = pos
            End Sub
        End Class

        Private Class LeftShapeContainer
            Implements _IContainer
            Private shape As Shape
            Public Sub New(shape As Shape)
                Me.shape = shape
            End Sub
            Public Function GetPointPos() As Single Implements _IContainer.GetPointPos
                Return Me.shape.Region.Left
            End Function
            Public Sub SetPos(pos As Integer) Implements _IContainer.SetPos
                Me.shape.CellRange.Col1 = pos
            End Sub
        End Class

        Private Class RightShapeContainer
            Implements _IContainer
            Private shape As Shape
            Public Sub New(shape As Shape)
                Me.shape = shape
            End Sub
            Public Function GetPointPos() As Single Implements _IContainer.GetPointPos
                Return Me.shape.Region.Right
            End Function
            Public Sub SetPos(pos As Integer) Implements _IContainer.SetPos
                Me.shape.CellRange.Col2 = pos
            End Sub
        End Class

        Private Class TopShapeContainer
            Implements _IContainer
            Private shape As Shape
            Public Sub New(shape As Shape)
                Me.shape = shape
            End Sub
            Public Function GetPointPos() As Single Implements _IContainer.GetPointPos
                Return Me.shape.Region.Top
            End Function
            Public Sub SetPos(pos As Integer) Implements _IContainer.SetPos
                Me.shape.CellRange.Row1 = pos
            End Sub
        End Class

        Private Class BottomShapeContainer
            Implements _IContainer
            Private shape As Shape
            Public Sub New(shape As Shape)
                Me.shape = shape
            End Sub
            Public Function GetPointPos() As Single Implements _IContainer.GetPointPos
                Return Me.shape.Region.Bottom
            End Function
            Public Sub SetPos(pos As Integer) Implements _IContainer.SetPos
                Me.shape.CellRange.Row2 = pos
            End Sub
        End Class

    End Module
End Namespace