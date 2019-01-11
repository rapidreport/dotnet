Public Class TextSplitterByWidth
    Inherits TextSplitter
    Private _Width As Integer
    Public Sub New(width As Integer, breakRule As Boolean)
        MyBase.New(breakRule)
        Me._Width = width
    End Sub
    Protected Overrides Function _GetNextWidth(text As String) As Integer
        If Me._Width = 0 Then
            Return MyBase._GetNextWidth(text)
        End If
        Return ReportUtil.GetWIndex(text, Me._Width)
    End Function
End Class