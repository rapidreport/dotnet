Imports System.Globalization
Imports jp.co.systembase.report.component

Public Class TextSplitterByDrawingWidth
    Inherits TextSplitter

    Private _TextDesign As TextDesign
    Private _Width As Single
    Private _ClipWidth As Single

    Public Sub New(textDesign As TextDesign, width As Single, clipWidth As Single)
        MyBase.New(True)
        _TextDesign = textDesign
        _Width = width
        _ClipWidth = clipWidth
    End Sub

    Protected Overrides Function _GetNextWidth(si As StringInfo) As Integer
        If _Width = 0 Then
            Return MyBase._GetNextWidth(si)
        End If
        Return _TextDesign.GetMonospacedDrawableLen(si, _Width)
    End Function

    Protected Overrides Function _ClipText(si As StringInfo, w As Integer) As String
        If _ClipWidth = 0 Then
            Return MyBase._ClipText(si, w)
        End If
        Return si.SubstringByTextElements(0, _TextDesign.GetMonospacedDrawableLen(si, _ClipWidth, w))
    End Function

End Class
