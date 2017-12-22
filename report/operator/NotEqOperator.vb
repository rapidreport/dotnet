Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class NotEqOperator
        Implements IOperator
        Public Function Exec( _
          evaluator As Evaluator, _
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 2)
            Dim v As Object = evaluator.Eval(params(0))
            For i As Integer = 1 To params.Count - 1
                If ReportUtil.Eq(v, evaluator.Eval(params(i))) Then
                    Return False
                End If
            Next
            Return True
        End Function
    End Class
End Namespace