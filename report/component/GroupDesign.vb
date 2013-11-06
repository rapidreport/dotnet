Imports jp.co.systembase.report.data

Namespace component

    Public Class GroupDesign

        Public Desc As Hashtable = Nothing
        Public Caption As String = Nothing
        Public Id As String = Nothing
        Public DataSource As String = Nothing
        Public Keys As List(Of String)
        Public Detail As Boolean = False
        Public MaxCount As Integer = 0
        Public PageBreak As Boolean = False
        Public ResetPageCount As Boolean = False
        Public SortKeys As List(Of String) = Nothing
        Public Layout As GroupLayoutDesign = Nothing
        Public CustomFields As Dictionary(Of String, String) = Nothing
        Public CustomFieldsKeyList As List(Of String) = Nothing
        Public ContentDesigns As List(Of ContentDesign) = Nothing

        Public ReportDesign As ReportDesign
        Public ParentContentDesign As ContentDesign

        Public Sub New()
            Me.New(Nothing)
        End Sub

        Public Sub New(ByVal keys As List(Of String))
            Me.Detail = False
            Me.Keys = keys
        End Sub

        Public Sub New( _
          ByVal desc As Hashtable, _
          ByVal reportDesign As ReportDesign, _
          ByVal parentContentDesign As ContentDesign)
            Me.Desc = desc
            Me.ReportDesign = reportDesign
            Me.ParentContentDesign = parentContentDesign
            Me.LoadDesc()
            Me.LoadSubDesc()
        End Sub

        Public Sub LoadDesc()
            Me.Caption = Me.Desc("caption")
            Me.Id = Me.Desc("id")
            If Me.Desc.ContainsKey("keys") Then
                Me.Keys = New List(Of String)
                For Each k As String In Desc("keys")
                    Me.Keys.Add(k)
                Next
            Else
                Me.Keys = Nothing
            End If
            Me.DataSource = Me.Desc("data_source")
            Me.Detail = Me.Desc("detail")
            Me.MaxCount = Me.Desc("max_count")
            Me.PageBreak = Me.Desc("page_break")
            Me.ResetPageCount = Me.Desc("reset_page_count")
            If Desc.ContainsKey("sort_keys") Then
                Me.SortKeys = New List(Of String)
                For Each k As String In Me.Desc("sort_keys")
                    Me.SortKeys.Add(k)
                Next
            Else
                Me.SortKeys = Nothing
            End If
            If Desc.ContainsKey("layout") Then
                Me.Layout = New GroupLayoutDesign(Me.Desc("layout"))
            Else
                Me.Layout = New GroupLayoutDesign
            End If
            If Me.Desc.ContainsKey("custom_fields") Then
                Me.CustomFields = New Dictionary(Of String, String)
                Me.CustomFieldsKeyList = New List(Of String)
                For Each d As Hashtable In Me.Desc("custom_fields")
                    If d.ContainsKey("key") AndAlso d.ContainsKey("exp") Then
                        If Not Me.CustomFields.ContainsKey(d("key")) Then
                            Me.CustomFields.Add(d("key"), d("exp"))
                            Me.CustomFieldsKeyList.Add(d("key"))
                        End If
                    End If
                Next
            Else
                Me.CustomFields = Nothing
                Me.CustomFieldsKeyList = Nothing
            End If
        End Sub

        Public Sub LoadSubDesc()
            If Not Me.Desc.ContainsKey("contents") OrElse CType(Me.Desc("contents"), ArrayList).Count = 0 Then
                Throw New ArgumentException("group has no content")
            End If
            Me.ContentDesigns = New List(Of ContentDesign)
            For Each d As Hashtable In Desc("contents")
                Me.ContentDesigns.Add(New ContentDesign(d, Me))
            Next
        End Sub

        Public Function DataSplit(ByVal data As ReportData) As List(Of ReportData)
            Dim ret As New List(Of ReportData)
            If Me.unbreakable Then
                If data.Size > 0 Then
                    ret.Add(New ReportData(data))
                End If
            Else
                Dim i As Integer = 0
                Do While (i < data.Size)
                    Dim j As Integer = i + 1
                    Do While (j < data.Size)
                        If Me.isBreak(data, i, j) Then
                            Exit Do
                        End If
                        j += 1
                    Loop
                    ret.Add(ReportData.GetPartialData(data, i, j))
                    i = j
                Loop
            End If
            Return ret
        End Function

        Private Function unbreakable() As Boolean
            If Me.Detail Then
                Return False
            End If
            If Me.Keys IsNot Nothing AndAlso Me.Keys.Count > 0 Then
                Return False
            End If
            If Me.MaxCount > 0 Then
                Return False
            End If
            Return True
        End Function

        Private Function isBreak(ByVal data As ReportData, ByVal i As Integer, ByVal j As Integer) As Boolean
            If Me.Detail Then
                Return True
            ElseIf Me.Keys IsNot Nothing Then
                For Each key As String In Me.Keys
                    If Not ReportUtil.Eq(data.Get(i, key), data.Get(j, key)) Then
                        Return True
                    End If
                Next
            End If
            If Me.MaxCount > 0 AndAlso j - i >= Me.MaxCount Then
                Return True
            End If
            Return False
        End Function

        Public Function FindContentDesign(ByVal id As String) As ContentDesign
            If Me.ContentDesigns IsNot Nothing Then
                For Each cd As ContentDesign In Me.ContentDesigns
                    If id.Equals(cd.Id) Then
                        Return cd
                    ElseIf cd.GroupDesign IsNot Nothing Then
                        Dim ret As ContentDesign = cd.GroupDesign.FindContentDesign(id)
                        If ret IsNot Nothing Then
                            Return ret
                        End If
                    End If
                Next
            End If
            Return Nothing
        End Function

        Public Function FindGroupDesign(ByVal id As String) As GroupDesign
            If id.Equals(Me.Id) Then
                Return Me
            ElseIf Me.ContentDesigns IsNot Nothing Then
                For Each cd As ContentDesign In Me.ContentDesigns
                    If cd.GroupDesign IsNot Nothing Then
                        Dim ret As GroupDesign = cd.GroupDesign.FindGroupDesign(id)
                        If ret IsNot Nothing Then
                            Return ret
                        End If
                    End If
                Next
            End If
            Return Nothing
        End Function

        Public Function GetAggregateSrcContentDesign() As ContentDesign
            For Each d As ContentDesign In Me.ContentDesigns
                If d.AggregateSrc Then
                    Return d
                End If
            Next
            Return Nothing
        End Function

    End Class

End Namespace