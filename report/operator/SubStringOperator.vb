﻿Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class SubStringOperator
        Implements IOperator
        Public Function Exec( _
          ByVal evaluator As Evaluator, _
          ByVal params As List(Of IExpression)) As Object Implements IOperator.Exec
            Dim str As String = ReportUtil.ObjectToString(evaluator.Eval(params(0)))
            If str Is Nothing Then
                Return Nothing
            End If
            Dim b As Integer = evaluator.Eval(params(1))
            If params.Count >= 3 Then
                Dim l As Integer = evaluator.Eval(params(2))
                Return ReportUtil.SubString(str, b, l)
            Else
                Return ReportUtil.SubString(str, b)
            End If
        End Function
    End Class
End Namespace