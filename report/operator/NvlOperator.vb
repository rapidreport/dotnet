Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class NvlOperator
        Implements IOperator
        Public Function Exec( _
          evaluator As Evaluator, _
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 1)
            For i As Integer = 0 To params.Count - 1
                Dim v As Object = evaluator.Eval(params(i))
                If v IsNot Nothing Then
                    Return v
                End If
            Next
            Return Nothing
        End Function
    End Class
End Namespace