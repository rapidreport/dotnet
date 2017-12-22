Namespace data.internal

    Public Class SubDataSource
        Implements IReportDataSource

        Private dataSource As IReportDataSource
        Private beginIndex As Integer
        Private endIndex As Integer

        Public Sub New( _
          dataSource As IReportDataSource, _
          beginIndex As Integer, _
          endIndex As Integer)
            Me.dataSource = dataSource
            Me.beginIndex = beginIndex
            Me.endIndex = endIndex
        End Sub

        Public Function [Get](i As Integer, key As String) As Object Implements IReportDataSource.Get
            If Me.beginIndex < 0 OrElse i < 0 OrElse i >= Me.Size Then
                Throw New ArgumentOutOfRangeException
            End If
            Return Me.dataSource.Get(Me.beginIndex + i, key)
        End Function

        Public Function Size() As Integer Implements IReportDataSource.Size
            Return Me.endIndex - Me.beginIndex
        End Function

    End Class

End Namespace