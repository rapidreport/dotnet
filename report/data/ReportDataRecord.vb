Imports jp.co.systembase.report.component

Namespace data

    Public Class ReportDataRecord

        Public data As ReportData
        Public i As Integer

        Public Sub New(ByVal data As ReportData, ByVal i As Integer)
            Me.data = data
            Me.i = i
        End Sub

        Public Function [Get](ByVal key As String) As Object
            Return Me.data.Get(Me.i, key)
        End Function

    End Class

End Namespace
