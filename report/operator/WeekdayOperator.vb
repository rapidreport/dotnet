Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class WeekdayOperator
        Implements IOperator
        Public Function Exec(
          evaluator As Evaluator,
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 1)
            Dim o As Object = evaluator.Eval(params(0))
            If o IsNot Nothing Then
                Dim v As DateTime = o
                Return CInt(v.DayOfWeek)
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace