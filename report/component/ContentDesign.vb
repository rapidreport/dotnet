Namespace component

    Public Class ContentDesign

        Public Desc As Hashtable = Nothing
        Public Caption As String = Nothing
        Public Id As String = Nothing
        Public Size As ContentSizeDesign = Nothing
        Public Layout As SubContentLayoutDesign = Nothing
        Public Background As Boolean = False
        Public AggregateSrc As Boolean = False
        Public EveryPage As Boolean = False
        Public EveryPageBlankGroup As Boolean = False
        Public ExistenceCond As String = Nothing
        Public VisibilityCond As String = Nothing
        Public Unbreakable As Boolean = False
        Public Weight As Integer = 0
        Public Variables As Dictionary(Of String, String) = Nothing
        Public VariablesKeyList As List(Of String) = Nothing
        Public MergeContentId As String = Nothing
        Public GroupDesign As GroupDesign = Nothing
        Public SubContentDesigns As List(Of ContentDesign) = Nothing

        Public ParentGroupDesign As GroupDesign
        Public BaseContentDesign As ContentDesign

        Public Sub New( _
          desc As Hashtable, _
          parentGroupDesign As GroupDesign)
            Me.New(desc, parentGroupDesign, Nothing)
        End Sub

        Public Sub New( _
          desc As Hashtable, _
          parentGroupDesign As GroupDesign, _
          baseContentDesign As ContentDesign)
            Me.Desc = desc
            Me.ParentGroupDesign = parentGroupDesign
            Me.BaseContentDesign = baseContentDesign
            Me.LoadDesc()
            Me.LoadSubDesc()
        End Sub

        Public Sub LoadDesc()
            Me.Caption = Me.Desc("caption")
            Me.Id = Me.Desc("id")
            If Me.Desc.ContainsKey("size") Then
                Me.Size = New ContentSizeDesign(Me.Desc("size"))
            Else
                Me.Size = New ContentSizeDesign
            End If
            If Me.Desc.Contains("layout") Then
                Me.Layout = New SubContentLayoutDesign(Me.Desc("layout"))
            Else
                Me.Layout = New SubContentLayoutDesign()
            End If
            Me.Background = Me.Desc("background")
            Me.AggregateSrc = Me.Desc("aggregate_src")
            Me.EveryPage = Me.Desc("every_page")
            Me.EveryPageBlankGroup = Me.Desc("every_page_blank_group")
            Me.ExistenceCond = Me.Desc("existence_cond")
            Me.VisibilityCond = Me.Desc("visibility_cond")
            Me.Unbreakable = Me.Desc("unbreakable")
            Me.Weight = Me.Desc("weight")
            If Me.Desc.ContainsKey("variables") Then
                Me.Variables = New Dictionary(Of String, String)
                Me.VariablesKeyList = New List(Of String)
                For Each d As Hashtable In Me.Desc("variables")
                    If d.ContainsKey("key") AndAlso d.ContainsKey("exp") Then
                        If Not Me.Variables.ContainsKey(d("key")) Then
                            Me.Variables.Add(d("key"), d("exp"))
                            Me.VariablesKeyList.Add(d("key"))
                        End If
                    End If
                Next
            Else
                Me.Variables = Nothing
                Me.VariablesKeyList = Nothing
            End If
            Me.MergeContentId = Me.Desc("merge_content_id")
        End Sub

        Public Sub LoadSubDesc()
            If Me.Desc.ContainsKey("group") Then
                Me.GroupDesign = New GroupDesign(Me.Desc("group"), Me.ParentGroupDesign.ReportDesign, Me)
            Else
                Me.GroupDesign = Nothing
            End If
            If Me.Desc.ContainsKey("sub") Then
                Me.SubContentDesigns = New List(Of ContentDesign)
                For Each d As Hashtable In Me.Desc("sub")
                    Me.SubContentDesigns.Add(New ContentDesign(d, Me.ParentGroupDesign, Me))
                Next
            Else
                Me.SubContentDesigns = Nothing
            End If
        End Sub

        Public Function GetIndex() As Integer
            If Me.BaseContentDesign IsNot Nothing Then
                Return Me.BaseContentDesign.GetIndex()
            Else
                Return Me.ParentGroupDesign.ContentDesigns.IndexOf(Me)
            End If
        End Function

        Public Function GetSubIndex() As Integer
            If Me.BaseContentDesign IsNot Nothing Then
                Return Me.BaseContentDesign.SubContentDesigns.IndexOf(Me)
            Else
                Return -1
            End If
        End Function

        Public Function GetReportDesign() As ReportDesign
            Return Me.ParentGroupDesign.ReportDesign
        End Function

        Public Function FindContentDesign(id As String) As ContentDesign
            If id.Equals(Me.Id) Then
                Return Me
            ElseIf Me.GroupDesign IsNot Nothing Then
                Dim ret As ContentDesign = Me.GroupDesign.FindContentDesign(id)
                If ret IsNot Nothing Then
                    Return ret
                ElseIf Me.SubContentDesigns IsNot Nothing Then
                    For Each cd As ContentDesign In Me.SubContentDesigns
                        ret = cd.FindContentDesign(id)
                        If ret IsNot Nothing Then
                            Return ret
                        End If
                    Next
                End If
            End If
            Return Nothing
        End Function

        Public Function FindGroupDesign(id As String) As GroupDesign
            If Me.GroupDesign IsNot Nothing Then
                Dim ret As GroupDesign = Me.GroupDesign.FindGroupDesign(id)
                If ret IsNot Nothing Then
                    Return ret
                End If
            End If
            Return Nothing
        End Function

    End Class

End Namespace