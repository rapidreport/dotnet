Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class MinOperator
        Implements IOperator
        Public Function Exec( _
          evaluator As Evaluator, _
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 1)
            Dim min As Object = Nothing
            For i As Integer = 0 To params.Count - 1
                Dim v As Object = evaluator.Eval(params(i))
                If v IsNot Nothing Then
                    If min Is Nothing OrElse min > v Then
                        min = v
                    End If
                End If
            Next
            Return min
        End Function
    End Class
End Namespace