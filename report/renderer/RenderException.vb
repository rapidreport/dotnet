Namespace renderer

    Public Class RenderException
        Inherits Exception
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, cause As Exception)
            MyBase.New(message, cause)
        End Sub
    End Class

End Namespace
