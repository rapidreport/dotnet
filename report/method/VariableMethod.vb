Imports jp.co.systembase.report.component

Namespace method

    Public Class VariableMethod
        Implements IMethod

        Public Function GetAvaliableContext() As Report.EEvalContext Implements IMethod.GetAvaliableContext
            Return Report.EEvalContext.PAGE
        End Function

        Public Function Exec( _
          ByVal evaluator As Evaluator, _
          ByVal param As String, _
          ByVal scope As String, _
          ByVal unit As String) As Object Implements IMethod.Exec
            With evaluator.PageContext
                If .Variables.ContainsKey(param) Then
                    Return .Variables(param)
                Else
                    Return Nothing
                End If
            End With
        End Function

    End Class

End Namespace