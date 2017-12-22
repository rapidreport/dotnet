Namespace component

    Public Class ContentHistory

        Public Report As Report = Nothing
        Public Group As Group = Nothing
        Public Content As Content = Nothing
        Public GroupFirst As Boolean = False
        Public GroupLast As Boolean = False
        Public ContentFirst As Boolean = False
        Public ContentLast As Boolean = False
        Public Child As ContentHistory = Nothing

        Private Sub New()
        End Sub

        Public Shared Function GetInstance(content As Content, rootGroups As Groups) As ContentHistory
            Dim ret As ContentHistory = Nothing
            Dim c As Content = content
            Do While c IsNot Nothing
                If rootGroups IsNot Nothing AndAlso c.Groups Is rootGroups Then
                    Exit Do
                End If
                Dim t As ContentHistory = New ContentHistory
                t.Content = c
                t.Group = c.ParentGroup
                t.Report = c.GetReport
                If ret Is Nothing OrElse _
                   (ret.GroupFirst AndAlso c.Groups.Groups(0) Is ret.Group) Then
                    t.ContentFirst = True
                End If
                If ret Is Nothing OrElse _
                   (ret.GroupLast AndAlso c.Groups.Groups(c.Groups.Groups.Count - 1) Is ret.Group) Then
                    t.ContentLast = True
                End If
                If t.ContentFirst AndAlso c.ParentGroup.Contents(0) Is c Then
                    t.GroupFirst = True
                End If
                If t.ContentLast AndAlso c.ParentGroup.Contents(c.ParentGroup.Contents.Count - 1) Is c Then
                    t.GroupLast = True
                End If
                t.Child = ret
                ret = t
                c = c.ParentGroup.ParentGroups.ParentContent
            Loop
            Return ret
        End Function

        Public Function GetLeaf() As ContentHistory
            Dim ret As ContentHistory = Me
            Do While ret.Child IsNot Nothing
                ret = ret.Child
            Loop
            Return ret
        End Function

        Public Overrides Function ToString() As String
            Dim ret As String = ""
            Dim ch As ContentHistory = Me
            Do While (ch IsNot Nothing)
                ret &= "group: " & ch.Group.ToString & " "
                If ch.GroupFirst Then
                    ret &= "(group first) "
                Else
                    ret &= "              "
                End If
                If ch.GroupLast Then
                    ret &= "(group last) "
                Else
                    ret &= "             "
                End If
                ret &= vbCrLf
                ret &= "content: " & ch.Content.ToString & " "
                If ch.ContentFirst Then
                    ret &= "(content first) "
                Else
                    ret &= "                "
                End If
                If ch.ContentLast Then
                    ret &= "(content last) "
                Else
                    ret &= "               "
                End If
                ret &= vbCrLf
                ch = ch.Child
            Loop
            Return ret
        End Function

    End Class

End Namespace
