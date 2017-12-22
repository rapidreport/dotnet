Namespace component
    Public Class ContentState
        Public GroupState As GroupState
        Public Content As Content
        Public Intrinsic As Boolean = False
        Public FirstPage As Boolean = False
        Public LastPage As Boolean = False
        Public Header As Boolean = False
        Public Footer As Boolean = False

        Public Sub New(groupState As GroupState, content As Content)
            Me.GroupState = groupState
            Me.Content = content
        End Sub

        Public Function FindScope(id As String) As ContentState
            If id Is Nothing Then
                Return Me
            ElseIf id.Equals("") Then
                If Me.GroupState.ParentState IsNot Nothing Then
                    Return Me.GroupState.ParentState
                End If
            Else
                Dim ret As ContentState = Me
                Do While ret IsNot Nothing
                    If ret.Content.ParentGroup.GetDesign.Id = id Then
                        Return ret
                    End If
                    ret = ret.GroupState.ParentState
                Loop
            End If
            Throw New ArgumentException(id & " is not found")
        End Function

    End Class
End Namespace