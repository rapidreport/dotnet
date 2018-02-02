Public Class TextSplitterByLen
    Inherits TextSplitter
    Private _Length As Integer
    Public Sub New(length As Integer)
        MyBase.New(False)
        Me._Length = length
    End Sub
    Protected Overrides Function _GetNextWidth(text As String) As Integer
        If _Length = 0 Then
            Return MyBase._GetNextWidth(text)
        End If
        Return Math.Min(Me._Length, text.Length)
    End Function
End Class
