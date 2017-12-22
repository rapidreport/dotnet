Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Interface IOperator
        Function Exec( _
          evaluator As Evaluator, _
          params As List(Of IExpression)) As Object
    End Interface
End Namespace

