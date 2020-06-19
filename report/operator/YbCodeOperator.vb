Imports System.Text.RegularExpressions
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]

    Public Class YbCodeOperator
        Implements IOperator
        Public Function Exec(
          evaluator As Evaluator,
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 1)
            Dim ret = ""
            With Nothing
                Dim v = evaluator.Eval(params(0))
                If v IsNot Nothing Then
                    Dim t = StrConv(v.ToString(), VbStrConv.Narrow)
                    ret = Regex.Replace(t, "\D", "")
                End If
            End With
            If params.Count >= 2 Then
                Dim v = evaluator.Eval(params(1))
                If v IsNot Nothing Then
                    Dim t = StrConv(v.ToString(), VbStrConv.Narrow)
                    t = Regex.Replace(t, "^\D+|\D+$", "")
                    ret &= Regex.Replace(t, "\D+", "-")
                End If
            End If
            Return ret
        End Function
    End Class

End Namespace


