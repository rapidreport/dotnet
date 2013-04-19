Imports jp.co.systembase.report.component

Namespace expression
    Public Class ImmediateExpression
        Implements IExpression
        Public Value As Object
        Public Sub New(ByVal value As Object)
            Me.Value = value
        End Sub
        Public Function Eval(ByVal evaluator As Evaluator) As Object Implements IExpression.Eval
            Return Me.Value
        End Function
    End Class
End Namespace