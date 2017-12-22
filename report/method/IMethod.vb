Imports jp.co.systembase.report.component

Namespace method
    Public Interface IMethod
        Function GetAvaliableContext() As Report.EEvalContext
        Function Exec( _
          evaluator As Evaluator, _
          param As String, _
          scope As String, _
          unit As String)
    End Interface
End Namespace