Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]
    Public Class DigitOperator
        Implements IOperator
        Public Function Exec( _
          ByVal evaluator As Evaluator, _
          ByVal params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 2)
            Dim v As Object = evaluator.Eval(params(0))
            Dim d As Integer = evaluator.Eval(params(1))
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
                    Return _v.Substring(j, 1)
                End If
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace
