Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class SubOperator
        Implements IOperator
        Public Function Exec( _
          evaluator As Evaluator, _
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 1)
            If params.Count = 1 Then
                Dim o As Object = evaluator.Eval(params(0))
                If o Is Nothing Then
                    Return Nothing
                End If
                Return -o
            Else
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
                    End If
                    ret -= o
                Next
                Return ret
            End If
        End Function
    End Class
End Namespace