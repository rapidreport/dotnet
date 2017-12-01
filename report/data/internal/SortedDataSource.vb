Imports jp.co.systembase.report.component

Namespace data.internal

    Public Class SortedDataSource
        Implements IReportDataSource

        Private _DataSource As IReportDataSource
        Private _Indexes As List(Of Integer)
        Private _Logger As IReportLogger

        Public Sub New(dataSource As IReportDataSource, keys As List(Of String), logger As IReportLogger)
            Me._DataSource = dataSource
            Me._Logger = logger
            Me._Indexes = New List(Of Integer)
            For i As Integer = 0 To Me._DataSource.Size - 1
                Me._Indexes.Add(i)
            Next
            Me._Indexes.Sort(New RecordComparer(Me._DataSource, keys, Me._Logger))
        End Sub

        Public Function Size() As Integer Implements IReportDataSource.Size
            Return Me._Indexes.Count
        End Function

        Public Function [Get](i As Integer, key As String) As Object Implements IReportDataSource.[Get]
            Try
                Return Me._DataSource.Get(Me._Indexes(i), key)
            Catch ex As UnknownFieldException
                If Me._Logger IsNot Nothing Then
                    Me._Logger.UnknownFieldError(ex)
                End If
                Return Nothing
            End Try
        End Function

    End Class

End Namespace