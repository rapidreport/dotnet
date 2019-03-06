Namespace component

    Public Class MonospacedFontsDesign

        Public DefaultFont As ItemDesign = Nothing
        Public FontMap As New Dictionary(Of String, ItemDesign)

        Public Sub New()
            Me.New(New ArrayList)
        End Sub

        Public Sub New(desc As ArrayList)
            For Each d As Hashtable In desc
                If d.ContainsKey("name") Then
                    Me.FontMap.Add(d("name"), New ItemDesign(d))
                Else
                    Me.DefaultFont = New ItemDesign(d)
                End If
            Next
        End Sub

        Public Function [Get](fontDesign As FontDesign) As DetailDesign
            Dim item As ItemDesign = Me.DefaultFont
            If Me.FontMap.ContainsKey(fontDesign.Name) Then
                item = Me.FontMap(fontDesign.Name)
            End If
            If item Is Nothing Then
                Return Nothing
            End If
            If fontDesign.Bold Then
                Return item.Bold
            Else
                Return item.Regular
            End If
        End Function

        Public Class ItemDesign
            Public Regular As DetailDesign
            Public Bold As DetailDesign
            Public Sub New(desc As Hashtable)
                Me.Regular = New DetailDesign(desc("regular"))
                Me.Bold = New DetailDesign(desc("bold"))
            End Sub
        End Class

        Public Class DetailDesign
            Public HalfWidth As Single
            Public FullWidth As Single
            Public RowHeidht As Single

            Public Sub New(desc As Hashtable)
                Me.HalfWidth = desc("half_width")
                Me.FullWidth = desc("full_width")
                Me.RowHeidht = desc("row_height")
            End Sub

        End Class

    End Class

End Namespace
