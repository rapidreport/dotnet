Imports System.Globalization
Imports jp.co.systembase.report.component

Public Class TextSplitterByDrawingWidth
    Inherits TextSplitter

    Private _TextDesign As TextDesign
    Private _WrapWidth As Single
    Private _ClipWidth As Single

    Public Sub New(textDesign As TextDesign, wrapWidth As Single, clipWidth As Single)
        MyBase.New(True)
        _TextDesign = textDesign
        _WrapWidth = wrapWidth
        _ClipWidth = clipWidth
    End Sub

    Protected Overrides Function _GetNextWidth(si As StringInfo) As Integer
        If _WrapWidth = 0 Then
            Return MyBase._GetNextWidth(si)
        End If
        Return _TextDesign.GetMonospacedDrawableLen(si, _WrapWidth)
    End Function

    Protected Overrides Function _ClipText(si As StringInfo, w As Integer) As String
        If _ClipWidth = 0 Then
            Return MyBase._ClipText(si, w)
        End If
        Dim _w = _TextDesign.GetMonospacedDrawableLen(si, _ClipWidth, w)
        Return si.SubstringByTextElements(0, _w)
    End Function

End Class
