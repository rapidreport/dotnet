Imports jp.co.systembase.report.component

Namespace method

    Public Class PageCountMethod
        Implements IMethod

        Public Function GetAvaliableContext() As Report.EEvalContext Implements IMethod.GetAvaliableContext
            Return Report.EEvalContext.PAGE
        End Function

        Public Function Exec( _
          evaluator As Evaluator, _
          param As String, _
          scope As String, _
          unit As String) As Object Implements IMethod.Exec
            Dim p As ReportPage = evaluator.PageContext.Page
            Select Case param
                Case "entire"
                    Return evaluator.PageContext.Pages.IndexOf(p) + 1
                Case "entire_total"
                    Return evaluator.PageContext.Pages.Count
                Case "total"
                    Return p.CountingPages.Count
                Case Else
                    Return p.CountingPages.IndexOf(p) + 1
            End Select
        End Function

    End Class

End Namespace