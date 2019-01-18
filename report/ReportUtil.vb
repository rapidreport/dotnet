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
            b = Math.Max(si.LengthInTextElements + b, 0)
        End If
        If b >= si.LengthInTextElements Then
            Return Nothing
        Else
            Return si.SubstringByTextElements(b)
        End If
    End Function

    Public Function SubString(str As String, begin As Integer, len As Integer) As String
        Dim si As StringInfo = New StringInfo(str)
        Dim b As Integer = begin
        If b < 0 Then
            b = Math.Max(si.LengthInTextElements + b, 0)
        End If
        If len <= 0 Or b >= si.LengthInTextElements Then
            Return Nothing
        ElseIf b + len > si.LengthInTextElements Then
            Return si.SubstringByTextElements(b)
        Else
            Return si.SubstringByTextElements(b, len)
        End If
    End Function

    Public Function StringLen(str As String) As Integer
        Dim si As StringInfo = New StringInfo(str)
        Return si.LengthInTextElements
    End Function

    Public Function WStringLen(str As String) As Integer
        Dim si As StringInfo = New StringInfo(str)
        Dim ret As Integer = 0
        For i As Integer = 0 To si.LengthInTextElements - 1
            Dim c As String = si.SubstringByTextElements(i, 1)
            If c.Length = 1 AndAlso SINGLE_CHARS.IndexOf(c) >= 0 Then
                ret += 1
            Else
                ret += 2
            End If
        Next
        Return ret
    End Function

    Public Function WSubString(str As String, begin As Integer) As String
        Dim si As New StringInfo(str)
        If begin >= 0 Then
            Return si.SubstringByTextElements(GetWIndex(si, begin))
        Else
            Return si.SubstringByTextElements(GetWRevIndex(si, -begin))
        End If
    End Function

    Public Function WSubString(str As String, begin As Integer, len As Integer) As String
        If begin >= 0 Then
            Dim si1 As New StringInfo(str)
            Dim si2 As New StringInfo(si1.SubstringByTextElements(GetWIndex(si1, begin)))
            Return si2.SubstringByTextElements(0, GetWIndex(si2, len))
        Else
            Dim si1 As New StringInfo(str)
            Dim si2 As New StringInfo(si1.SubstringByTextElements(GetWRevIndex(si1, -begin)))
            Return si2.SubstringByTextElements(0, GetWIndex(si2, len))
        End If
    End Function

    Public Function GetWIndex(si As StringInfo, w As Integer) As Integer
        Dim _w As Integer = 0
        For i As Integer = 0 To si.LengthInTextElements - 1
            Dim c As String = si.SubstringByTextElements(i, 1)
            If c.Length = 1 AndAlso SINGLE_CHARS.IndexOf(c) >= 0 Then
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

    Public Function GetWRevIndex(si As StringInfo, w As Integer) As Integer
        Dim _w As Integer = 0
        For i As Integer = si.LengthInTextElements - 1 To 0 Step -1
            Dim c As String = si.SubstringByTextElements(i, 1)
            If c.Length = 1 AndAlso SINGLE_CHARS.IndexOf(c) >= 0 Then
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