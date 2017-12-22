Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class DivOperator
        Implements IOperator
        Public Function Exec( _
          evaluator As Evaluator, _
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 2)
            Dim ret As Decimal
            With Nothing
                Dim o As Object = evaluator.Eval(params(0))
                If o Is Nothing Then
                    Return Nothing
                End If
                ret = o
            End With
            For i As Integer = 1 To params.Count - 1
                Dim o As Object = evaluator.Eval(params(i))
                If o Is Nothing Then
                    Return Nothing
                ElseIf o = 0 Then
                    Return Nothing
                Else
                    ret /= o
                End If
            Next
            Return ret
        End Function
    End Class
End Namespace