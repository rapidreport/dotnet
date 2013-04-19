﻿Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class SecondOperator
        Implements IOperator
        Public Function Exec( _
          ByVal evaluator As Evaluator, _
          ByVal params As List(Of IExpression)) As Object Implements IOperator.Exec
            Dim o As Object = evaluator.Eval(params(0))
            If o IsNot Nothing Then
                Dim v As DateTime = o
                Return v.Second
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace