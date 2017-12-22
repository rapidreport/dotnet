Imports jp.co.systembase.report.component

Namespace method
    Public Class CurrentTimeMethod
        Implements IMethod
        Public Function GetAvaliableContext() As Report.EEvalContext Implements IMethod.GetAvaliableContext
            Return Report.EEvalContext.ANY
        End Function
        Public Function Exec( _
          evaluator As Evaluator, _
          param As String, _
          scope As String, _
          unit As String) As Object Implements IMethod.Exec
            Return evaluator.BasicContext.Report.CurrentTime
        End Function
    End Class
End Namespace
