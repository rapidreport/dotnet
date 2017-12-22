Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class OrOperator
        Implements IOperator
        Public Function Exec( _
          evaluator As Evaluator, _
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 2)
            For i As Integer = 0 To params.Count - 1
                If ReportUtil.Condition(evaluator.Eval(params(i))) Then
                    Return True
                End If
            Next
            Return False
        End Function
    End Class
End Namespace