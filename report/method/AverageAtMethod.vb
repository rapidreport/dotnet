Imports jp.co.systembase.report.component

Namespace method

    Public Class AverageAtMethod
        Implements IMethod

        Public Function GetAvaliableContext() As Report.EEvalContext Implements IMethod.GetAvaliableContext
            Return Report.EEvalContext.PAGE
        End Function

        Public Function Exec( _
          evaluator As Evaluator, _
          param As String, _
          scope As String, _
          unit As String) As Object Implements IMethod.Exec
            If param Is Nothing Then
                Return Nothing
            End If
            Return evaluator.GetPresentData(scope, unit).GetAverage(param)
        End Function

    End Class

End Namespace
