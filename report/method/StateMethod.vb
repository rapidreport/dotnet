Imports jp.co.systembase.report.component

Namespace method

    Public Class StateMethod
        Implements IMethod

        Public Function GetAvaliableContext() As Report.EEvalContext Implements IMethod.GetAvaliableContext
            Return Report.EEvalContext.CONTENT
        End Function

        Public Function Exec( _
          evaluator As Evaluator, _
          param As String, _
          scope As String, _
          unit As String) As Object Implements IMethod.Exec
            If param Is Nothing Then
                Return Nothing
            End If
            Dim state As ContentState = evaluator.ContentContext.ContentState.FindScope(scope)
            Select Case param
                Case "first"
                    Return state.GroupState.First
                Case "last"
                    Return state.GroupState.Last
                Case "last2"
                    Return state.GroupState.Last2
                Case "index"
                    Return state.GroupState.Index
                Case "group_first"
                    Return state.GroupState.GroupFirst
                Case "group_last"
                    Return state.GroupState.GroupLast
                Case "group_last2"
                    Return state.GroupState.GroupLast2
                Case "group_index"
                    Return state.GroupState.GroupIndex
                Case "first_page"
                    Return state.FirstPage
                Case "last_page"
                    Return state.LastPage
                Case "intrinsic"
                    Return state.Intrinsic
                Case "blank"
                    Return state.GroupState.Blank
                Case "blank_first"
                    Return state.GroupState.BlankFirst
                Case "blank_last"
                    Return state.GroupState.BlankLast
                Case "header"
                    Return state.Header
                Case "footer"
                    Return state.Footer
            End Select
            Return Nothing
        End Function

    End Class

End Namespace