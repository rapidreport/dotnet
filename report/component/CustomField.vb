﻿Imports jp.co.systembase.report.data

Namespace component
    Public Class CustomField

        Public Class Stack

            Public Class Node
                Public Caller As Node
                Public Field As CustomField
                Public Sub New(ByVal caller As Node, ByVal field As CustomField)
                    Me.Caller = caller
                    Me.Field = field
                End Sub
            End Class

            Public Top As Node = Nothing

            Private Sub detectCircularReference(ByVal field As CustomField)
                Dim n As Node = Me.Top
                Do While n IsNot Nothing
                    If n.Field.Key.Equals(field.Key) And n.Field.Data Is field.Data Then
                        Throw New EvalException("循環参照が含まれています : " & n.Field.Key)
                    End If
                    n = n.Caller
                Loop
            End Sub

            Public Sub Push(ByVal field As CustomField)
                Me.detectCircularReference(field)
                Me.Top = New Node(Me.Top, field)
            End Sub

            Public Sub Pop()
                Me.Top = Me.Top.Caller
            End Sub

        End Class

        Public Key As String
        Public Exp As String
        Public Report As Report
        Public Data As ReportData

        Public Sub New( _
          ByVal key As String, _
          ByVal exp As String, _
          ByVal report As Report, _
          ByVal data As ReportData)
            Me.Key = key
            Me.Exp = exp
            Me.Report = report
            Me.Data = data
        End Sub

        Public Function [Get](ByVal i As Integer) As Object
            If Not TypeOf Me.Data.DataSource Is INoCache Then
                With Me.Report.DataCache.CustomField(Me.Data, Key)
                    If .ContainsKey(i) Then
                        Return .Item(i)
                    Else
                        Dim ret As Object = Me.evalTry(i)
                        .Add(i, ret)
                        Return ret
                    End If
                End With
            Else
                Return Me.evalTry(i)
            End If
        End Function

        Private Function evalTry(ByVal i As Integer) As Object
            Dim r As New ReportDataRecord(Me.Data, i)
            Return New Evaluator(Me.Report, Me.Data, r).EvalTry(Me.Exp)
        End Function

    End Class
End Namespace