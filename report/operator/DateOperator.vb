Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace [operator]

    Public Class DateOperator
        Implements IOperator

        Public Function Exec(
          evaluator As Evaluator,
          params As List(Of IExpression)) As Object Implements IOperator.Exec
            evaluator.ValidateParamCount(params, 1)
            Dim o As Object = evaluator.Eval(params(0))
            If TypeOf o Is String Then
                Dim l = _Extract(o)
                If l.Count = 1 Then
                    Dim v As String = l(0)
                    If v.Length = 6 Then
                        Return New Date(
                            _Year(v.Substring(0, 2)),
                            v.Substring(2, 2),
                            v.Substring(4, 2),
                            0, 0, 0)
                    ElseIf v.Length = 8 Then
                        Return New Date(
                            _Year(v.Substring(0, 4)),
                            v.Substring(4, 2),
                            v.Substring(6, 2),
                            0, 0, 0)
                    ElseIf v.Length = 12 Then
                        Return New Date(
                            _Year(v.Substring(0, 2)),
                            v.Substring(2, 2),
                            v.Substring(4, 2),
                            v.Substring(6, 2),
                            v.Substring(8, 2),
                            v.Substring(10, 2))
                    ElseIf v.Length = 14 Then
                        Return New Date(
                            _Year(v.Substring(0, 4)),
                            v.Substring(4, 2),
                            v.Substring(6, 2),
                            v.Substring(8, 2),
                            v.Substring(10, 2),
                            v.Substring(12, 2))
                    End If
                End If
                If l.Count > 0 Then
                    Dim y = _Year(l(0))
                    Dim mn = 0
                    Dim d = 1
                    Dim h = 0
                    Dim m = 0
                    Dim s = 0
                    If l.Count > 1 Then
                        mn = l(1)
                    End If
                    If l.Count > 2 Then
                        d = l(2)
                    End If
                    If l.Count > 3 Then
                        h = l(3)
                    End If
                    If l.Count > 4 Then
                        m = l(4)
                    End If
                    If l.Count > 5 Then
                        s = l(5)
                    End If
                    Return New Date(y, mn, d, h, m, s)
                End If
            ElseIf params.Count = 3 Then
                Return New Date(
                    _Year(evaluator.Eval(params(0))),
                    evaluator.Eval(params(1)),
                    evaluator.Eval(params(2)),
                    0, 0, 0)
            Else
                Dim y = _Year(evaluator.Eval(params(0)))
                Dim mn = 0
                Dim d = 1
                Dim h = 0
                Dim m = 0
                Dim s = 0
                If params.Count > 1 Then
                    mn = evaluator.Eval(params(1))
                End If
                If params.Count > 2 Then
                    d = evaluator.Eval(params(2))
                End If
                If params.Count > 3 Then
                    h = evaluator.Eval(params(3))
                End If
                If params.Count > 4 Then
                    m = evaluator.Eval(params(4))
                End If
                If params.Count > 5 Then
                    s = evaluator.Eval(params(5))
                End If
                Return New Date(y, mn, d, h, m, s)
            End If
            Return Nothing
        End Function

        Private Function _Year(v As Integer) As Integer
            If v < 50 Then
                Return v + 2000
            ElseIf v < 100 Then
                Return v + 1900
            Else
                Return v
            End If
        End Function

        Private Function _Extract(v As String) As List(Of String)
            Dim ret = New List(Of String)()
            Dim l = 0
            For i = 0 To v.Length() - 1
                Dim c = v(i)
                If c < "0"c Or c > "9"c Then
                    If i > l Then
                        ret.Add(v.Substring(l, i - l))
                    End If
                    l = i + 1
                End If
            Next
            If v.Length > l Then
                ret.Add(v.Substring(l, v.Length - l))
            End If
            Return ret
        End Function

    End Class

End Namespace
