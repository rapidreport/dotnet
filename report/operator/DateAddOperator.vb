Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class DateAddOperator
        Implements IOperator
        Public Function Exec(
          evaluator As Evaluator,
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 3)
            Dim o As Object = evaluator.Eval(params(0))
            Dim u As String = evaluator.Eval(params(1))
            Dim d As Integer = evaluator.Eval(params(2))
            If o IsNot Nothing Then
                Dim dt As DateTime = o
                Select Case u
                    Case "y"
                        Return dt.AddYears(d)
                    Case "M"
                        Return dt.AddMonths(d)
                    Case "d"
                        Return dt.AddDays(d)
                    Case "h"
                        Return dt.AddHours(d)
                    Case "m"
                        Return dt.AddMinutes(d)
                    Case "s"
                        Return dt.AddSeconds(d)
                    Case Else
                        Return dt
                End Select
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace
