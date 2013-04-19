Imports jp.co.systembase.report.component

Public Interface IReportLogger
    Sub EvaluateError(ByVal exp As String, ByVal ex As EvalException)
    Sub ElementRenderingError( _
      ByVal contentDesign As ContentDesign, _
      ByVal design As ElementDesign, _
      ByVal ex As Exception)
End Interface
