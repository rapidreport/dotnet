Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Interface IOperator
        Function Exec( _
          ByVal evaluator As Evaluator, _
          ByVal params As List(Of IExpression)) As Object
    End Interface
End Namespace

