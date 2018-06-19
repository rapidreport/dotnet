Imports jp.co.systembase.report.component

Namespace method
    Public Interface IMethod
        Function GetAvaliableContext() As Report.EEvalContext
        Function Exec(
          evaluator As Evaluator,
          param As String,
          scope As String,
          unit As String)
    End Interface
End Namespace