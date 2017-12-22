Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data

Namespace method

    Public Class RowCountAtMethod
        Implements IMethod

        Public Function GetAvaliableContext() As Report.EEvalContext Implements IMethod.GetAvaliableContext
            Return Report.EEvalContext.PAGE
        End Function

        Public Function Exec( _
          evaluator As Evaluator, _
          param As String, _
          scope As String, _
          unit As String) As Object Implements IMethod.Exec
            Dim data As ReportData = evaluator.GetPresentData(scope, unit)
            If param IsNot Nothing Then
                Return data.GetCount(param)
            Else
                Return data.Size
            End If
        End Function

    End Class

End Namespace
