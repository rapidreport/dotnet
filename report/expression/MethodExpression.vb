Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.method

Namespace expression
    Public Class MethodExpression
        Implements IExpression
        Public Method As IMethod
        Public Param As String
        Public Scope As String
        Public Unit As String
        Public Sub New( _
          method As IMethod, _
          param As String, _
          scope As String, _
          unit As String)
            Me.Method = method
            Me.Param = param
            Me.Scope = scope
            Me.Unit = unit
        End Sub
        Public Function Eval(evaluator As Evaluator) As Object Implements IExpression.Eval
            If Not evaluator.TraverseMode Then
                If Me.Unit IsNot Nothing AndAlso Not evaluator.BasicContext.Data.IsFilled Then
                    Throw New EvalException("'#'付きのメソッドは report.fill() の実行後にのみ呼び出すことができます")
                End If
                Dim avail As Boolean = True
                Select Case Method.GetAvaliableContext
                    Case Report.EEvalContext.CONTENT
                        avail = evaluator.ContentContext IsNot Nothing
                    Case Report.EEvalContext.PAGE
                        avail = evaluator.PageContext IsNot Nothing
                End Select
                If Not avail Then
                    Throw New EvalException("メソッド '" & Me.Method.GetType.FullName & "' は、このコンテキストでは呼び出すことができません")
                End If
            End If
            Return Method.Exec(evaluator, Me.Param, Me.Scope, Me.Unit)
        End Function
    End Class
End Namespace
