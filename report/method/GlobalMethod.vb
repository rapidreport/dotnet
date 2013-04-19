Imports jp.co.systembase.report.component

Namespace method

    Public Class GlobalMethod
        Implements IMethod

        Public Function GetAvaliableContext() As Report.EEvalContext Implements IMethod.GetAvaliableContext
            Return Report.EEvalContext.ANY
        End Function

        Public Function Exec( _
          ByVal evaluator As Evaluator, _
          ByVal param As String, _
          ByVal scope As String, _
          ByVal unit As String) As Object Implements IMethod.Exec
            With evaluator.BasicContext.Report
                If .GlobalScope.ContainsKey(param) Then
                    Return .GlobalScope(param)
                Else
                    Return Nothing
                End If
            End With
        End Function

    End Class

End Namespace