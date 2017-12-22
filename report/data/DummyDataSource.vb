Namespace data

    Public Class DummyDataSource
        Implements IReportDataSource
        Implements INoCache

        Private Shared instance As New DummyDataSource

        Public Shared Function GetInstance() As DummyDataSource
            Return instance
        End Function

        Private Sub New()
        End Sub

        Public Function [Get](i As Integer, key As String) As Object Implements IReportDataSource.[Get]
            Return Nothing
        End Function

        Public Function Size() As Integer Implements IReportDataSource.Size
            Return 1
        End Function

    End Class

End Namespace
