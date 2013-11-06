Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class YearOperator
        Implements IOperator
        Public Function Exec( _
          ByVal evaluator As Evaluator, _
          ByVal params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 1)
            Dim o As Object = evaluator.Eval(params(0))
            If o IsNot Nothing Then
                Dim v As DateTime = o
                Return v.Year
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace