Imports System.Globalization

Public Class TextSplitterByWidth
    Inherits TextSplitter
    Private _Width As Integer
    Public Sub New(width As Integer, breakRule As Boolean)
        MyBase.New(breakRule)
        Me._Width = width
    End Sub
    Protected Overrides Function _GetNextWidth(si As StringInfo) As Integer
        If Me._Width = 0 Then
            Return MyBase._GetNextWidth(si)
        End If
        Return ReportUtil.GetWIndex(si, Me._Width)
    End Function
End Class