Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data

Namespace method

    Public Class RowCountMethod
        Implements IMethod

        Public Function GetAvaliableContext() As Report.EEvalContext Implements IMethod.GetAvaliableContext
            Return Report.EEvalContext.ANY
        End Function

        Public Function Exec( _
          ByVal evaluator As Evaluator, _
          ByVal param As String, _
          ByVal scope As String, _
          ByVal unit As String) As Object Implements IMethod.Exec
            Dim data As ReportData = evaluator.GetData(scope, unit)
            If param IsNot Nothing Then
                Return data.GetCount(param)
            Else
                Return data.Size
            End If
        End Function

    End Class

End Namespace
