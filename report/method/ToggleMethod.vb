Imports jp.co.systembase.report.component

Namespace method

    Public Class ToggleMethod
        Implements IMethod

        Public Function GetAvaliableContext() As Report.EEvalContext Implements IMethod.GetAvaliableContext
            Return Report.EEvalContext.PAGE
        End Function

        Public Function Exec( _
          ByVal evaluator As Evaluator, _
          ByVal param As String, _
          ByVal scope As String, _
          ByVal unit As String) As Object Implements IMethod.Exec
            With evaluator.PageContext.Page
                .ToggleValue = Not .ToggleValue
                Return .ToggleValue
            End With
        End Function

    End Class

End Namespace