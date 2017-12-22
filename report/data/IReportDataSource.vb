Namespace data

    Public Interface IReportDataSource
        Function [Get](i As Integer, key As String) As Object
        Function Size() As Integer
    End Interface

End Namespace
