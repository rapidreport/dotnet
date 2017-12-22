Imports jp.co.systembase.report.component

Namespace method

    Public Class PageMethod
        Implements IMethod

        Public Function GetAvaliableContext() As Report.EEvalContext Implements IMethod.GetAvaliableContext
            Return Report.EEvalContext.PAGE
        End Function

        Public Function Exec( _
          evaluator As Evaluator, _
          param As String, _
          scope As String, _
          unit As String) As Object Implements IMethod.Exec
            With evaluator.PageContext.Page
                If .PageScope.ContainsKey(param) Then
                    Return .PageScope(param)
                Else
                    Return Nothing
                End If
            End With
        End Function

    End Class

End Namespace