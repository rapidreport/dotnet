Imports System.Globalization

Public Class TextSplitter

    Private Const WORD_CHARS As String =
        "0123456789" &
        "abcdefghijklmnopqrstuvwxyz" &
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ" &
        "ﾞﾟｰ･｡@#$%^&*_+|~=\`<>""':;,.,"
    Private Const OPEN_CHARS As String =
        "｢({[""'（［｛〈《「『【〔"
    Private Const CLOSE_CHARS As String =
        "｣)}]""':;,.,–/）］｝〉》」』】〕" &
        ""”’；：。、．,・ゝゞー" &
        "ァィゥェォッャュョヮヵヶ" &
        "ぁぃぅぇぉっゃゅょゎゕゖ" &
        "ㇰㇱㇲㇳㇴㇵㇶㇷㇸㇹㇷ゚ㇺㇻㇼㇽㇾㇿ々〻〜"

    Private _BreakRule As Boolean

    Public Sub New()
        Me.New(False)
    End Sub

    Public Sub New(breakRule As Boolean)
        Me._BreakRule = breakRule
    End Sub

    Public Function GetLines(text As String) As List(Of String)
        Return _GetLines(text, -1)
    End Function

    Public Function GetLine(text As String, i As Integer) As String
        Dim l As List(Of String) = Me._GetLines(text, i)
        If l.Count > i Then
            Return l(i)
        Else
            Return Nothing
        End If
    End Function

    Private Function _GetLines(text As String, limit As Integer) As List(Of String)
        Dim lf As String = vbLf
        Dim cr As String = vbCr
        Dim ret As New List(Of String)
        If text Is Nothing Then
            ret.Add(Nothing)
        Else
            For Each t As String In text.Split(lf)
                Me._Split(ret, t.Replace(cr, ""), limit)
                If limit >= 0 AndAlso ret.Count > limit Then
                    Exit For
                End If
            Next
        End If
        Return ret
    End Function

    Private Sub _Split(l As List(Of String), text As String, limit As Integer)
        If text.Length = 0 Then
            l.Add("")
        Else
            Dim t As String = text
            Do
                Dim si As StringInfo = New StringInfo(t)
                Dim w As Integer = Me._GetNextWidth(si)
                If w = 0 Then
                    Exit Do
                End If
                If Me._BreakRule Then
                    w = _GetNextOnRule(si, w)
                End If
                l.Add(_ClipText(si, w))
                If limit >= 0 AndAlso l.Count > limit Then
                    Exit Do
                End If
                If si.LengthInTextElements > w Then
                    t = si.SubstringByTextElements(w)
                    If Not Report.Compatibility._4_37_SplittedTextNoTrim Then
                        t = t.TrimStart
                    End If
                Else
                    Exit Do
                End If
            Loop
        End If
    End Sub

    Protected Overridable Function _GetNextWidth(si As StringInfo) As Integer
        Return si.LengthInTextElements
    End Function

    Protected Overridable Function _ClipText(si As StringInfo, w As Integer) As String
        Return si.SubstringByTextElements(0, w)
    End Function

    Private Function _GetNextOnRule(si As StringInfo, w As Integer) As Integer
        If w = si.LengthInTextElements Then
            Return w
        End If
        Dim _w As Integer = w
        Do While _w > 0
            Dim cp As Char = si.SubstringByTextElements(_w - 1)
            Dim cn As Char = si.SubstringByTextElements(_w)
            If WORD_CHARS.Contains(cp) And WORD_CHARS.Contains(cn) Then
            ElseIf OPEN_CHARS.Contains(cp) Then
            ElseIf CLOSE_CHARS.Contains(cn) Then
            Else
                Return _w
            End If
            _w -= 1
        Loop
        Return w
    End Function

End Class