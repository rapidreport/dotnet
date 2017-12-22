Namespace component
    Public Class ElementDesigns
        Inherits List(Of ElementDesign)

        Private Sub New()
        End Sub

        Public Sub New(contentDesign As ContentDesign)
            If contentDesign.MergeContentId IsNot Nothing Then
                If Report.SharedContents.ContainsKey(contentDesign.MergeContentId) Then
                    Dim cd As ContentDesign = Report.SharedContents(contentDesign.MergeContentId)
                    If cd.Desc.ContainsKey("elements") Then
                        For Each e As Hashtable In cd.Desc("elements")
                            If e.ContainsKey("id") AndAlso "__trial__".Equals(e("id")) Then
                                Continue For
                            End If
                            Me.Add(New ElementDesign(e))
                        Next
                    End If
                End If
            End If
            If contentDesign.Desc.ContainsKey("elements") Then
                For Each e As Hashtable In contentDesign.Desc("elements")
                    Me.Add(New ElementDesign(e))
                Next
            End If
        End Sub

        Public Overloads Function Find(id As String) As ElementDesign
            For Each e As ElementDesign In Me
                If id.Equals(e.Get("id")) Then
                    Return e
                End If
            Next
            Return Nothing
        End Function

        Public Function SelectByType(type As String) As ElementDesigns
            Dim ret As New ElementDesigns
            For Each e As ElementDesign In Me
                If type.Equals(e.Get("type")) Then
                    ret.Add(e)
                End If
            Next
            Return ret
        End Function

    End Class
End Namespace