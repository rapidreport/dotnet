Imports jp.co.systembase.report
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.method

Public Class PageGroupCountMethod
    Implements IMethod

    Public Function GetAvaliableContext() As Report.EEvalContext Implements IMethod.GetAvaliableContext
        Return Report.EEvalContext.ANY
    End Function

    Public Function Exec(evaluator As Evaluator, param As String, scope As String, unit As String) As Object Implements IMethod.Exec
        If evaluator.GroupLayoutFilledCount < 0 Then
            Throw New EvalException("page_group_countメソッドはgroup.layout.内でのみ呼び出すことができます。")
        End If
        Return evaluator.GroupLayoutFilledCount
    End Function

End Class
