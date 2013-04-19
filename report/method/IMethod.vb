Imports jp.co.systembase.report.component

Namespace method
    Public Interface IMethod
        Function GetAvaliableContext() As Report.EEvalContext
        Function Exec( _
          ByVal evaluator As Evaluator, _
          ByVal param As String, _
          ByVal scope As String, _
          ByVal unit As String)
    End Interface
End Namespace