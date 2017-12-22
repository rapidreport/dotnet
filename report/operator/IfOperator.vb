Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class IfOperator
        Implements IOperator
        Public Function Exec( _
          evaluator As Evaluator, _
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 2)
            If ReportUtil.Condition(evaluator.Eval(params(0))) Then
                Return evaluator.Eval(params(1))
            Else
                If params.Count >= 3 Then
                    Return evaluator.Eval(params(2))
                Else
                    Return Nothing
                End If
            End If
        End Function
    End Class
End Namespace