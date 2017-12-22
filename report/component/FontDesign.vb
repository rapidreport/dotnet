Namespace component
    Public Class FontDesign

        Public Name As String = "gothic"
        Public Size As Single = 10
        Public Bold As Boolean = False
        Public Italic As Boolean = False
        Public Underline As Boolean = False

        Public Sub New(desc As Hashtable)
            Me.new(New ElementDesign(desc))
        End Sub

        Public Sub New(desc As ElementDesign)
            If Not desc.IsNull("name") Then
                Me.Name = desc.Get("name")
            End If
            If Not desc.IsNull("size") Then
                Me.Size = desc.Get("size")
            End If
            Me.Bold = desc.Get("bold")
            Me.Italic = desc.Get("italic")
            Me.Underline = desc.Get("underline")
        End Sub

    End Class
End Namespace

