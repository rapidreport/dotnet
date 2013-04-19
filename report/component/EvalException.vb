Namespace component
    Public Class EvalException
        Inherits Exception
        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(ByVal message As String, ByVal cause As Exception)
            MyBase.New(message, cause)
        End Sub
    End Class
End Namespace
