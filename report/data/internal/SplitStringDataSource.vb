Namespace data.internal

    Public Class SplitStringDataSource
        Implements IReportDataSource

        Private reportData As ReportData
        Private key As String
        Private value As String

        Public Sub New(data As ReportData, key As String, value As String)
            Me.reportData = data
            Me.key = key
            Me.value = value
        End Sub

        Public Function [Get](i As Integer, key As String) As Object Implements IReportDataSource.Get
            If Me.key.Equals(key) Then
                Return Me.value
            Else
                Return Me.reportData.Get(i, key)
            End If
        End Function

        Public Function Size() As Integer Implements IReportDataSource.Size
            Return Me.reportData.Size
        End Function
    End Class

End Namespace

