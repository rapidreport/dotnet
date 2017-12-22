﻿Imports jp.co.systembase.report.component

Namespace method

    Public Class NextMethod
        Implements IMethod

        Public Function GetAvaliableContext() As Report.EEvalContext Implements IMethod.GetAvaliableContext
            Return Report.EEvalContext.CONTENT
        End Function

        Public Function Exec( _
          evaluator As Evaluator, _
          param As String, _
          scope As String, _
          unit As String) As Object Implements IMethod.Exec
            If param Is Nothing Then
                Return Nothing
            End If
            With evaluator.ContentContext
                Dim g As Group = .Content.ParentGroup
                If g IsNot Nothing Then
                    If g.Index < g.ParentGroups.Groups.Count - 1 Then
                        Return g.ParentGroups.Groups(g.Index + 1).Data.GetRecord.[Get](param)
                    End If
                End If
            End With
            Return Nothing
        End Function

    End Class

End Namespace