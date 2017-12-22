Imports jp.co.systembase.report.component

Namespace data.internal

    Public Class WrapperDataSource
        Implements IReportDataSource

        Public DataList As New List(Of ReportData)

        Public Function [Get](i As Integer, key As String) As Object Implements IReportDataSource.Get
            Return Me.DataList(i).GetRecord.Get(key)
        End Function

        Public Function Size() As Integer Implements IReportDataSource.Size
            Return Me.DataList.Count
        End Function

    End Class

End Namespace