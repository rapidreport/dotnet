Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class AbsOperator
        Implements IOperator
        Public Function Exec(
          evaluator As Evaluator,
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 1)
            Dim v As Object = evaluator.Eval(params(0))
            If v IsNot Nothing Then
                Return Math.Abs(v)
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace