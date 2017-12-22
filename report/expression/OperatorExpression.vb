Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.operator

Namespace expression
    Public Class OperatorExpression
        Implements IExpression
        Public [Operator] As IOperator
        Public Params As List(Of IExpression)
        Public Sub New( _
          [operator] As IOperator, _
          params As List(Of IExpression))
            Me.Operator = [operator]
            Me.Params = params
        End Sub
        Public Function Eval(evaluator As Evaluator) As Object Implements IExpression.Eval
            Return Me.Operator.Exec(evaluator, Me.Params)
        End Function
    End Class
End Namespace