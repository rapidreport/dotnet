Imports System.Globalization

Public Class TextSplitterByLen
    Inherits TextSplitter
    Private _Length As Integer

    Public Sub New(length As Integer)
        MyBase.New(False)
        Me._Length = length
    End Sub
    Protected Overrides Function _GetNextWidth(si As StringInfo) As Integer
        If _Length = 0 Then
            Return MyBase._GetNextWidth(si)
        End If
        Return Math.Min(Me._Length, si.LengthInTextElements)
    End Function
End Class
