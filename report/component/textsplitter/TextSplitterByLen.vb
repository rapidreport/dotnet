Imports System.Globalization

Public Class TextSplitterByLen
    Inherits TextSplitter
    Private _Length As Integer
    Public Sub New(length As Integer)
        MyBase.New(False)
        Me._Length = length
    End Sub
    Protected Overrides Function _GetNextWidth(text As String) As Integer
        Dim si As StringInfo = New StringInfo(text)
        If _Length = 0 Then
            Return MyBase._GetNextWidth(text)
        End If
        Return Math.Min(Me._Length, si.LengthInTextElements)
    End Function
End Class
