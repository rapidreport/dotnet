Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data

Namespace method

    Public Class FieldMethod
        Implements IMethod

        Public Function GetAvaliableContext() As Report.EEvalContext Implements IMethod.GetAvaliableContext
            Return Report.EEvalContext.ANY
        End Function

        Public Function Exec( _
          evaluator As Evaluator, _
          param As String, _
          scope As String, _
          unit As String) As Object Implements IMethod.Exec
            If param Is Nothing Then
                Return Nothing
            End If
            If scope Is Nothing And unit Is Nothing Then
                Return evaluator.BasicContext.DataRecord.Get(param)
            Else
                Dim data As ReportData = evaluator.GetData(scope, unit)
                If Not data.IsEmpty Then
                    Return data.GetRecord.Get(param)
                Else
                    Return Nothing
                End If
            End If
        End Function

    End Class

End Namespace