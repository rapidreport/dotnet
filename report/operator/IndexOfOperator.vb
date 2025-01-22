Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class IndexOfOperator
        Implements IOperator
        Public Function Exec(
          evaluator As Evaluator,
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 2)
            Dim str = ReportUtil.ObjectToString(evaluator.Eval(params(0)))
            Dim searchStr = ReportUtil.ObjectToString(evaluator.Eval(params(1)))
            If str Is Nothing Or searchStr Is Nothing Then
                Return -1
            End If
            Return str.IndexOf(searchStr)
        End Function
    End Class
End Namespace
