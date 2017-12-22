Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.component

Public Interface IReportLogger
    Sub EvaluateError(exp As String, ex As EvalException)
    Sub ElementRenderingError(
      contentDesign As ContentDesign,
      design As ElementDesign,
      ex As Exception)
    Sub UnknownFieldError(ex As UnknownFieldException)
End Interface
