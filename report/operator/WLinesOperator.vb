Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class WLinesOperator
        Implements IOperator
        Public Function Exec(
          evaluator As Evaluator,
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 2)
            Dim str As String = ReportUtil.ObjectToString(evaluator.Eval(params(0)))
            If str Is Nothing Then
                Return Nothing
            End If
            Dim w As Integer = evaluator.Eval(params(1))
            Dim rule As Boolean = False
            If params.Count >= 3 Then
                rule = evaluator.Eval(params(2))
            End If
            Dim sp As New TextSplitterByWidth(w, rule)
            Return sp.GetLines(str).Count
        End Function
    End Class
End Namespace