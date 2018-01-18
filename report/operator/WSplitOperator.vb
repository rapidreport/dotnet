Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class WSplitOperator
        Implements IOperator
        Public Function Exec( _
          evaluator As Evaluator, _
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 3)
            Dim str As String = ReportUtil.ObjectToString(evaluator.Eval(params(0)))
            If str Is Nothing Then
                Return Nothing
            End If
            Dim w As Integer = evaluator.Eval(params(1))
            Dim i As Integer = evaluator.Eval(params(2))
            Dim j As Integer = 0
            For Each t As String In ReportUtil.SplitLines(str)
                Dim b As Integer = 0
                Dim e As Integer = 0
                Do
                    b = e
                    e = ReportUtil.GetWIndex(t, b, w)
                    If j = i Then
                        Return ReportUtil.SubString(t, b, e - b)
                    End If
                    j += 1
                Loop While e < t.Length
            Next
            Return Nothing
        End Function
    End Class
End Namespace