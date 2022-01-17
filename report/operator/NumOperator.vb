Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class NumOperator
        Implements IOperator
        Public Function Exec(
          evaluator As Evaluator,
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 1)
            Dim v As Object = evaluator.Eval(params(0))
            If TypeOf v Is String Then
                Return CType(v, Decimal)
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace
