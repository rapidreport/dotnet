Imports jp.co.systembase.report.component

Namespace data.internal

    Public Class SortedDataSource
        Implements IReportDataSource

        Private reportData As ReportData
        Private keys As List(Of String)
        Private order As List(Of Integer)

        Public Sub New(ByVal reportData As ReportData, ByVal keys As List(Of String))
            Me.reportData = reportData
            Me.keys = keys
            Me.initialize()
        End Sub

        Private Sub initialize()
            Me.order = New List(Of Integer)
            For i As Integer = 0 To Me.reportData.Size - 1
                Me.order.Add(i)
            Next
            Me.order.Sort(New _comparer(Me.reportData, Me.keys))
        End Sub

        Private Class _comparer
            Implements IComparer(Of Integer)
            Public ReportData As ReportData
            Public Keys As List(Of String)
            Public Sub New(ByVal reportData As ReportData, ByVal keys As List(Of String))
                Me.ReportData = reportData
                Me.Keys = keys
            End Sub
            Public Function Compare(ByVal x As Integer, ByVal y As Integer) As Integer Implements IComparer(Of Integer).Compare
                For Each k As String In Me.Keys
                    Dim _x As Object = Me.ReportData.Get(x, k)
                    Dim _y As Object = Me.ReportData.Get(y, k)
                    If _x Is Nothing AndAlso _y Is Nothing Then
                        Continue For
                    End If
                    If _x Is Nothing Then
                        Return -1
                    End If
                    If _y Is Nothing Then
                        Return 1
                    End If
                    If _x < _y Then
                        Return -1
                    End If
                    If _x > _y Then
                        Return 1
                    End If
                Next
                Return 0
            End Function
        End Class

        Public Function Size() As Integer Implements IReportDataSource.Size
            Return Me.order.Count
        End Function

        Public Function [Get](ByVal i As Integer, ByVal key As String) As Object Implements IReportDataSource.[Get]
            Return Me.reportData.Get(Me.order(i), key)
        End Function

    End Class

End Namespace