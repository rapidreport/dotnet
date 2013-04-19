Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data

Namespace method

    Public Class FieldSumMethod
        Implements IMethod

        Public Function GetAvaliableContext() As Report.EEvalContext Implements IMethod.GetAvaliableContext
            Return Report.EEvalContext.ANY
        End Function

        Public Function Exec( _
          ByVal evaluator As Evaluator, _
          ByVal param As String, _
          ByVal scope As String, _
          ByVal unit As String) As Object Implements IMethod.Exec
            If param Is Nothing Then
                Return Nothing
            End If
            Return evaluator.GetData(scope, unit).GetSummary(param)
        End Function

    End Class

End Namespace