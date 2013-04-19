﻿Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class GreaterOperator
        Implements IOperator
        Public Function Exec( _
          ByVal evaluator As Evaluator, _
          ByVal params As List(Of IExpression)) As Object Implements IOperator.Exec
            Return evaluator.Eval(params(0)) > evaluator.Eval(params(1))
        End Function
    End Class
End Namespace