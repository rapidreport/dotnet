Imports jp.co.systembase.report.component

Namespace data

    Public Class SubPageDataSource
        Implements IReportDataSource

        Public SubPageKeys As List(Of String)

        Private recordList As New List(Of data.ReportDataRecord)
        Private pageIndexList As New List(Of Dictionary(Of String, Integer))

        Private currentRecord As data.ReportDataRecord = Nothing
        Private currentPageIndex As Dictionary(Of String, Integer) = Nothing
        Private currentSubPageIndex As Integer = 0

        Public Sub New(pages As ReportPages, pageGroupId As String, ParamArray subPageKeys() As String)
            Me.SubPageKeys = New List(Of String)(subPageKeys)
            For i As Integer = 0 To pages.Count - 1
                Dim page As ReportPage = pages(i)
                If pageGroupId IsNot Nothing Then
                    Dim g As Group = page.FindStartedGroup(pageGroupId)
                    If g IsNot Nothing Then
                        Me.addBreak(g.Data.GetRecord)
                    End If
                End If
                If Me.currentRecord Is Nothing Then
                    Me.addBreak(page.Report.Data.GetRecord)
                End If
                Me.addSubPage(i)
            Next
        End Sub

        Private Sub addSubPage(pageIndex As Integer)
            If Me.currentSubPageIndex = 0 Then
                Me.recordList.Add(Me.currentRecord)
                Me.pageIndexList.Add(Me.currentPageIndex)
            End If
            Me.currentPageIndex.Add(Me.SubPageKeys(Me.currentSubPageIndex), pageIndex)
            Me.currentSubPageIndex += 1
            If Me.currentSubPageIndex = Me.SubPageKeys.Count Then
                Me.addBreak(Me.currentRecord)
            End If
        End Sub

        Private Sub addBreak(record As data.ReportDataRecord)
            Me.currentRecord = record
            Me.currentPageIndex = New Dictionary(Of String, Integer)
            Me.currentSubPageIndex = 0
        End Sub

        Public Function [Get](i As Integer, key As String) As Object Implements data.IReportDataSource.[Get]
            If Me.SubPageKeys.Contains(key) Then
                If Me.pageIndexList(i).ContainsKey(key) Then
                    Return Me.pageIndexList(i)(key)
                Else
                    Return Nothing
                End If
            Else
                Return Me.recordList(i).[Get](key)
            End If
        End Function

        Public Function Size() As Integer Implements data.IReportDataSource.Size
            Return Me.recordList.Count
        End Function

    End Class

End Namespace
