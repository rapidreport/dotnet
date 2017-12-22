Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class Round5EOperator
        Implements IOperator
        Public Function Exec( _
          evaluator As Evaluator, _
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 1)
            Dim o As Object = evaluator.Eval(params(0))
            If o Is Nothing Then
                Return Nothing
            End If
            Dim digit As Integer = 0
            If params.Count >= 2 Then
                digit = evaluator.Eval(params(1))
            End If
            Return ReportUtil.Round5E(o, digit)
        End Function
    End Class
End Namespace
