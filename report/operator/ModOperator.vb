Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class ModOperator
        Implements IOperator
        Public Function Exec( _
          evaluator As Evaluator, _
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 2)
            Dim v1 As Object = evaluator.Eval(params(0))
            Dim v2 As Object = evaluator.Eval(params(1))
            If v1 Is Nothing Or v2 Is Nothing Then
                Return Nothing
            Else
                Return v1 Mod v2
            End If
        End Function
    End Class
End Namespace