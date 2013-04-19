Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class MaxOperator
        Implements IOperator
        Public Function Exec( _
          ByVal evaluator As Evaluator, _
          ByVal params As List(Of IExpression)) As Object Implements IOperator.Exec
            Dim max As Object = Nothing
            For i As Integer = 0 To params.Count - 1
                Dim v As Object = evaluator.Eval(params(i))
                If v IsNot Nothing Then
                    If max Is Nothing OrElse max < v Then
                        max = v
                    End If
                End If
            Next
            Return max
        End Function
    End Class
End Namespace