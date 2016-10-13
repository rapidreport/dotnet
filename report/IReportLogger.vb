Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.component

Public Interface IReportLogger
    Sub EvaluateError(ByVal exp As String, ByVal ex As EvalException)
    Sub ElementRenderingError(
      ByVal contentDesign As ContentDesign,
      ByVal design As ElementDesign,
      ByVal ex As Exception)
    Sub UnknownFieldError(ByVal ex As UnknownFieldException)
End Interface
