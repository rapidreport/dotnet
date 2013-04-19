Imports jp.co.systembase.report.component

Namespace data

    Public Interface IReportDataSource
        Function [Get](ByVal i As Integer, ByVal key As String) As Object
        Function Size() As Integer
    End Interface

End Namespace
