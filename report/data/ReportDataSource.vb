Namespace data

    Public Class ReportDataSource
        Implements IReportDataSource

        Public Data As DataTable = Nothing

        Public Sub New(dataTable As DataTable)
            Me.Data = dataTable
        End Sub

        Public Overridable Function [Get](i As Integer, key As String) As Object Implements IReportDataSource.[Get]
            If Me.Data.Columns.Contains(key) Then
                Dim v As Object = Me.Data.Rows(i).Item(key)
                If TypeOf v Is DBNull Then
                    Return Nothing
                Else
                    Return v
                End If
            Else
                Throw New UnknownFieldException(Me, i, key)
            End If
        End Function

        Public Overridable Function Size() As Integer Implements IReportDataSource.Size
            Return Me.Data.Rows.Count
        End Function

    End Class

End Namespace

