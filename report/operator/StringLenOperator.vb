Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class StringLenOperator
        Implements IOperator
        Public Function Exec( _
          ByVal evaluator As Evaluator, _
          ByVal params As List(Of IExpression)) As Object Implements IOperator.Exec
            Dim str As String = ReportUtil.ObjectToString(evaluator.Eval(params(0)))
            If str Is Nothing Then
                Return Nothing
            End If
            Return str.Length
        End Function
    End Class
End Namespace
