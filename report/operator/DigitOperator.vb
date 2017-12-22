Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class DigitOperator
        Implements IOperator
        Public Function Exec( _
          evaluator As Evaluator, _
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 2)
            Dim v As Object = evaluator.Eval(params(0))
            Dim d As Integer = evaluator.Eval(params(1))
            Dim ns As String = Nothing
            If params.Count >= 3 Then
                ns = evaluator.Eval(params(2))
            End If
            If v IsNot Nothing Then
                Dim _v As String = v.ToString
                Dim l As Integer = _v.Length
                Dim i As Integer = _v.IndexOf(".")
                Dim j As Integer = IIf(i < 0, l, i) - d - IIf(d >= 0, 1, 0)
                If j < 0 Then
                    Return Nothing
                ElseIf j >= l Then
                    Return "0"
                Else
                    Dim c As String = _v.Substring(j, 1)
                    If ns IsNot Nothing AndAlso c.Equals("-") Then
                        Return ns
                    Else
                        Return c
                    End If
                End If
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace
