Namespace component

    Public Class GroupRange

        Public Groups As Groups = Nothing
        Public First As ContentHistory = Nothing
        Public Last As ContentHistory = Nothing

        Public Sub New(groups As Groups)
            Me.New(groups, _
                   groups.GetNextContentHistory(Nothing), _
                   Nothing)
        End Sub

        Public Sub New( _
          groups As Groups, _
          first As ContentHistory, _
          last As ContentHistory)
            Me.Groups = groups
            Me.First = first
            Me.Last = last
        End Sub

        Public Function GetSubRange(g As Group) As ContentRange
            Dim _first As ContentHistory = Nothing
            Dim _last As ContentHistory = Nothing
            If Me.First Is Nothing Then
            ElseIf Me.First.Group.Index > g.Index Then
            ElseIf Me.Last IsNot Nothing AndAlso Me.Last.Group.Index < g.Index Then
            Else
                If Me.First.Group.Index < g.Index Then
                    _first = ContentHistory.GetInstance( _
                      g.GetNextContent(Nothing), _
                      g.ParentGroups)
                Else
                    _first = Me.First
                End If
                If Me.Last IsNot Nothing AndAlso Me.Last.Group Is g Then
                    _last = Me.Last
                End If
            End If
            Return New ContentRange(g, _first, _last)
        End Function

        Public Function GetGroupCount() As Integer
            If Me.First Is Nothing Then
                Return 0
            ElseIf Me.Last Is Nothing Then
                Return Me.Groups.Groups.Count - Me.First.Group.Index
            Else
                Return Me.Last.Group.Index - Me.First.Group.Index + 1
            End If
        End Function

        Public Function GetGroup(i As Integer) As Group
            Return Me.Groups.Groups(Me.First.Group.Index + i)
        End Function

        Public Function ContainsFirst() As Boolean
            If Me.First Is Nothing Then
                Return False
            Else
                Return Me.First.Group.Index = 0
            End If
        End Function

        Public Function ContainsLast() As Boolean
            If Me.First Is Nothing Then
                Return False
            ElseIf Me.Last Is Nothing Then
                Return True
            Else
                Return Me.Last.Group.Index = Me.Groups.Groups.Count - 1
            End If
        End Function

        Public Function Unbreakable() As Boolean
            If Me.Last IsNot Nothing Then
                Dim ch As ContentHistory = Me.Last
                Do While ch IsNot Nothing
                    If ch.ContentLast Then
                        Return ch.Content.Design.Unbreakable
                    End If
                    ch = ch.Child
                Loop
            End If
            Return False
        End Function

    End Class

End Namespace