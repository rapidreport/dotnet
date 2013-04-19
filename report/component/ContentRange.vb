Imports jp.co.systembase.report.component

Namespace component

    Public Class ContentRange

        Public Group As Group = Nothing
        Public First As ContentHistory = Nothing
        Public Last As ContentHistory = Nothing

        Public Sub New( _
          ByVal group As Group)
            Me.New( _
              group, _
              ContentHistory.GetInstance(group.GetNextContent(Nothing), group.ParentGroups), _
              Nothing)
        End Sub

        Public Sub New( _
          ByVal group As Group, _
          ByVal first As ContentHistory, _
          ByVal last As ContentHistory)
            Me.Group = group
            Me.First = first
            Me.Last = last
        End Sub

        Public Function IsFirst(ByVal c As Content) As Boolean
            Return (Me.First IsNot Nothing) AndAlso (c Is Me.First.Content)
        End Function

        Public Function IsLast(ByVal c As Content) As Boolean
            Return (Me.Last IsNot Nothing) AndAlso (c Is Me.Last.Content)
        End Function

        Public Function GetSubRange(ByVal c As Content) As GroupRange
            If c.Groups Is Nothing Then
                Return Nothing
            End If
            Dim _first As ContentHistory = Nothing
            Dim _last As ContentHistory = Nothing
            If Me.First Is Nothing Then
            ElseIf Me.First.Content.GetIndex > c.GetIndex Then
            ElseIf Me.Last IsNot Nothing AndAlso Me.Last.Content.GetIndex < c.GetIndex Then
            Else
                If Me.First.Content.GetIndex < c.GetIndex Then
                    _first = ContentHistory.GetInstance(c.GetNextContent(Nothing), c.Groups)
                Else
                    _first = Me.First.Child
                End If
                If Me.Last IsNot Nothing AndAlso Me.Last.Content Is c Then
                    _last = Me.Last.Child
                End If
            End If
            Return New GroupRange(c.Groups, _first, _last)
        End Function

    End Class

End Namespace