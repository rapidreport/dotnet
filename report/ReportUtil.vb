Imports System.Globalization

Public Module ReportUtil

    Private Const SINGLE_CHARS As String =
      "0123456789" &
      "abcdefghijklmnopqrstuvwxyz" &
      "ABCDEFGHIJKLMNOPQRSTUVWXYZ" &
      "ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜｦﾝｧｨｩｪｫｯｬｭｮ" &
      "ﾞﾟｰ｢｣･､｡ !@#$%^&*()_+|~-=\`{}[]:"";'<>?,./" & vbCrLf & vbTab

    Public Function RoundDown(v As Decimal, digit As Integer) As Decimal
        Dim d As Decimal = (10 ^ (-digit))
        Dim tmp As Decimal = Decimal.Truncate(Math.Abs(v) * d)
        Return Math.Sign(v) * tmp / d
    End Function

    Public Function RoundUp(v As Decimal, digit As Integer) As Decimal
        Dim d As Decimal = (10 ^ (-digit))
        Dim tmp As Decimal = Decimal.Truncate(Math.Abs(v) * d + 0.9D)
        Return Math.Sign(v) * tmp / d
    End Function

    Public Function RoundUp2(v As Decimal, digit As Integer) As Decimal
        Dim d As Decimal = (10 ^ (-digit))
        Dim _v As Decimal = Math.Abs(v) * d
        Dim tmp As Decimal = Decimal.Truncate(_v)
        If _v > tmp Then
            tmp += 1
        End If
        Return Math.Sign(v) * tmp / d
    End Function

    Public Function Round5(v As Decimal, digit As Integer) As Decimal
        Dim d As Decimal = (10 ^ (-digit))
        Dim tmp As Decimal = Decimal.Truncate(Math.Abs(v) * d + 0.5D)
        Return Math.Sign(v) * tmp / d
    End Function

    Public Function Round5E(v As Decimal, digit As Integer) As Decimal
        Dim d As Decimal = (10 ^ (-digit))
        Dim _v As Decimal = Math.Abs(v) * d
        Dim _tv As Decimal = Decimal.Truncate(_v)
        Dim tmp As Decimal
        If _v - _tv = 0.5D Then
            tmp = _tv
            If _tv Mod 2 Then
                tmp += 1
            End If
        Else
            tmp = Decimal.Truncate(_v + 0.5D)
        End If
        Return Math.Sign(v) * tmp / d
    End Function

    Public Function Round6(v As Decimal, digit As Integer) As Decimal
        Dim d As Decimal = (10 ^ (-digit))
        Dim tmp As Decimal = Decimal.Truncate(Math.Abs(v) * d + 0.4D)
        Return Math.Sign(v) * tmp / d
    End Function

    Public Function Eq(v1 As Object, v2 As Object) As Boolean
        If v1 Is Nothing Then
            Return v2 Is Nothing
        Else
            Return Regularize(v1).Equals(Regularize(v2))
        End If
    End Function

    Public Function Condition(v As Object) As Boolean
        If TypeOf v Is Boolean Then
            Return v
        Else
            Dim _v As Object = Regularize(v)
            If TypeOf _v Is Decimal Then
                Return Not _v.Equals(0D)
            End If
        End If
        Return False
    End Function

    Public Function Regularize(v As Object) As Object
        If TypeOf v Is Integer OrElse
           TypeOf v Is Long OrElse
           TypeOf v Is Short OrElse
           TypeOf v Is Single OrElse
           TypeOf v Is Double OrElse
           TypeOf v Is Byte Then
            Return CType(v, Decimal)
        End If
        Return v
    End Function

    Public Function ToPoint(scaleUnit As Report.EScaleUnit, v As Single) As Single
        Select Case scaleUnit
            Case Report.EScaleUnit.MM
                Return v * 2.835
            Case Report.EScaleUnit.INCH
                Return v * 72
        End Select
        Return v
    End Function

    Public Function PointTo(scaleUnit As Report.EScaleUnit, v As Single) As Single
        Select Case scaleUnit
            Case Report.EScaleUnit.MM
                Return v * 0.3528
            Case Report.EScaleUnit.INCH
                Return v * 0.01389
        End Select
        Return v
    End Function

    Public Function SubString(str As String, begin As Integer) As String
        Dim si As StringInfo = New StringInfo(str)
        Dim b As Integer = begin
        If b < 0 Then
            b = si.LengthInTextElements + b
            If b < 0 Then
                b = 0
            End If
        End If
        If b >= si.LengthInTextElements Then
            Return Nothing
        Else
            Return str.Substring(b)
        End If
    End Function

    Public Function SubString(str As String, begin As Integer, len As Integer) As String
        Dim si As StringInfo = New StringInfo(str)
        Dim b As Integer = begin
        Dim l As Integer = len
        If b < 0 Then
            b = si.LengthInTextElements + b
            If b < 0 Then
                l += b
                b = 0
            End If
        End If
        If l <= 0 Or b >= si.LengthInTextElements Then
            Return Nothing
        ElseIf b + l > si.LengthInTextElements Then
            Return str.Substring(b)
        Else
            Return str.Substring(b, l)
        End If
    End Function

    Public Function WStrinLen(str As String) As Integer
        Dim si As StringInfo = New StringInfo(str)
        Dim ret As Integer = 0
        For i As Integer = 0 To si.LengthInTextElements - 1
            Dim c As Char = si.SubstringByTextElements(i, 1)
            If SINGLE_CHARS.IndexOf(c) >= 0 Then
                ret += 1
            Else
                ret += 2
            End If
        Next
        Return ret
    End Function

    Public Function WSubString(str As String, begin As Integer) As String
        Dim si As StringInfo = New StringInfo(str)
        Dim b As Integer
        If begin >= 0 Then
            b = GetWIndex(str, 0, begin)
        Else
            b = GetWRevIndex(str, si.LengthInTextElements, -begin)
        End If
        If b >= si.LengthInTextElements Then
            Return Nothing
        Else
            Return str.Substring(b)
        End If
    End Function

    Public Function WSubString(str As String, begin As Integer, len As Integer) As String
        Dim si As StringInfo = New StringInfo(str)
        Dim b As Integer
        Dim e As Integer
        If begin >= 0 Then
            b = GetWIndex(str, 0, begin)
            e = GetWIndex(str, b, len)
        Else
            Dim _len As Integer = Math.Min(-begin, len)
            e = GetWRevIndex(str, si.LengthInTextElements, -(begin + _len))
            b = GetWRevIndex(str, e, _len)
        End If
        If e <= b Or b >= si.LengthInTextElements Then
            Return Nothing
        ElseIf e >= si.LengthInTextElements Then
            Return str.Substring(b)
        Else
            Return str.Substring(b, e - b)
        End If
    End Function

    Public Function GetWIndex(str As String, base As Integer, w As Integer) As Integer
        Dim si As StringInfo = New StringInfo(str)
        Dim _w As Integer = 0
        For i As Integer = base To si.LengthInTextElements - 1
            Dim c As Char = si.SubstringByTextElements(i, 1)
            If SINGLE_CHARS.IndexOf(c) >= 0 Then
                _w += 1
            Else
                _w += 2
            End If
            If _w > w Then
                Return i
            End If
        Next
        Return si.LengthInTextElements
    End Function

    Public Function GetWRevIndex(str As String, base As Integer, w As Integer) As Integer
        Dim si As StringInfo = New StringInfo(str)
        Dim _w As Integer = 0
        For i As Integer = base - 1 To 0 Step -1
            Dim c As Char = si.SubstringByTextElements(i, 1)
            If SINGLE_CHARS.IndexOf(c) >= 0 Then
                _w += 1
            Else
                _w += 2
            End If
            If _w > w Then
                Return i + 1
            End If
        Next
        Return 0
    End Function

    Public Function ObjectToString(o As Object) As String
        If TypeOf o Is String Then
            Return o
        Else
            Dim _o As Object = ReportUtil.Regularize(o)
            If TypeOf _o Is Decimal Then
                Return _o.ToString
            End If
        End If
        Return Nothing
    End Function

End Module