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
            Dim rule As Boolean = False
            If params.Count >= 4 Then
                rule = evaluator.Eval(params(3))
            End If
            Dim sp As New TextSplitterByWidth(w, rule)
            Return sp.GetLine(str, i)
        End Function
    End Class
End Namespace