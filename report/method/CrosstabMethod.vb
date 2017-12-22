Imports jp.co.systembase.report.component

Namespace method

    Public Class CrosstabMethod
        Implements IMethod

        Public Function GetAvaliableContext() As Report.EEvalContext Implements IMethod.GetAvaliableContext
            Return Report.EEvalContext.ANY
        End Function

        Public Function Exec(
          evaluator As Evaluator,
          param As String,
          scope As String,
          unit As String) As Object Implements IMethod.Exec
            If param Is Nothing Then
                Return Nothing
            End If
            With evaluator.BasicContext.Data
                If .Group IsNot Nothing AndAlso .Group.CrosstabState IsNot Nothing Then
                    Select Case param
                        Case "v_index"
                            Return .Group.CrosstabState.VIndex
                        Case "h_index"
                            Return .Group.CrosstabState.HIndex
                        Case "v_last"
                            Return .Group.CrosstabState.VLast
                        Case "h_last"
                            Return .Group.CrosstabState.HLast
                    End Select
                End If
            End With
            Return Nothing
        End Function

    End Class

End Namespace
