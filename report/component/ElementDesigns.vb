Namespace component
    Public Class ElementDesigns
        Inherits List(Of ElementDesign)

        Private Sub New()
        End Sub

        Public Sub New(ByVal contentDesign As ContentDesign)
            If contentDesign.Desc.ContainsKey("elements") Then
                For Each e As Hashtable In contentDesign.Desc("elements")
                    Me.Add(New ElementDesign(e))
                Next
            End If
        End Sub

        Public Overloads Function Find(ByVal id As String) As ElementDesign
            For Each e As ElementDesign In Me
                If id.Equals(e.Get("id")) Then
                    Return e
                End If
            Next
            Return Nothing
        End Function

        Public Function SelectByType(ByVal type As String) As ElementDesigns
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