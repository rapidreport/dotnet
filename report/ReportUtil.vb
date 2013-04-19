Public Module ReportUtil

    Private Const SINGLE_CHARS As String = _
      "0123456789" & _
      "abcdefghijklmnopqrstuvwxyz" & _
      "ABCDEFGHIJKLMNOPQRSTUVWXYZ" & _
      "ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜｦﾝｧｨｩｪｫｯｬｭｮ" & _
      "ﾞﾟｰ｢｣･､｡ !@#$%^&*()_+|~-=\`{}[]:"";'<>?,./" & vbCrLf & vbTab

    Public Function RoundDown(ByVal v As Decimal, ByVal digit As Integer) As Decimal
        Dim d As Decimal = (10 ^ (-digit))
        Dim tmp As Decimal = roundFix(Math.Abs(v) * d)
        Return Math.Sign(v) * tmp / d
    End Function

    Public Function RoundUp(ByVal v As Decimal, ByVal digit As Integer) As Decimal
        Dim d As Decimal = (10 ^ (-digit))
        Dim tmp As Decimal = roundFix(Math.Abs(v) * d + 0.9D)
        Return Math.Sign(v) * tmp / d
    End Function

    Public Function RoundUp2(ByVal v As Decimal, ByVal digit As Integer) As Decimal
        Dim d As Decimal = (10 ^ (-digit))
        Dim tmp As Decimal = roundFix(Math.Abs(v) * d + 0.9999999999D)
        Return Math.Sign(v) * tmp / d
    End Function

    Public Function Round5(ByVal v As Decimal, ByVal digit As Integer) As Decimal
        Dim d As Decimal = (10 ^ (-digit))
        Dim tmp As Decimal = roundFix(Math.Abs(v) * d + 0.5D)
        Return Math.Sign(v) * tmp / d
    End Function

    Public Function Round6(ByVal v As Decimal, ByVal digit As Integer) As Decimal
        Dim d As Decimal = (10 ^ (-digit))
        Dim tmp As Decimal = roundFix(Math.Abs(v) * d + 0.4D)
        Return Math.Sign(v) * tmp / d
    End Function

    Private Function roundFix(ByVal v As Decimal) As Decimal
        Dim s As String = v.ToString
        Dim i As Integer = s.IndexOf(".")
        If i >= 0 Then
            Return s.Substring(0, i)
        Else
            Return v
        End If
    End Function

    Public Function Eq(ByVal v1 As Object, ByVal v2 As Object) As Boolean
        If v1 Is Nothing Then
            Return v2 Is Nothing
        Else
            Return Regularize(v1).Equals(Regularize(v2))
        End If
    End Function

    Public Function Condition(ByVal v As Object) As Boolean
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

    Public Function Regularize(ByVal v As Object) As Object
        If TypeOf v Is Integer OrElse _
           TypeOf v Is Long OrElse _
           TypeOf v Is Single OrElse _
           TypeOf v Is Double OrElse _
           TypeOf v Is Byte Then
            Return CType(v, Decimal)
        End If
        Return v
    End Function

    Public Function ToPoint(ByVal scaleUnit As Report.EScaleUnit, ByVal v As Single) As Single
        Select Case scaleUnit
            Case Report.EScaleUnit.MM
                Return v * 2.835
            Case Report.EScaleUnit.INCH
                Return v * 72
        End Select
        Return v
    End Function

    Public Function PointTo(ByVal scaleUnit As Report.EScaleUnit, ByVal v As Single) As Single
        Select Case scaleUnit
            Case Report.EScaleUnit.MM
                Return v * 0.3528
            Case Report.EScaleUnit.INCH
                Return v * 0.01389
        End Select
        Return v
    End Function

    Public Function SubString(ByVal str As String, ByVal begin As Integer) As String
        Dim b As Integer = begin
        If b < 0 Then
            b = str.Length + b
            If b < 0 Then
                b = 0
            End If
        End If
        If b >= str.Length Then
            Return Nothing
        Else
            Return str.Substring(b)
        End If
    End Function

    Public Function SubString(ByVal str As String, ByVal begin As Integer, ByVal len As Integer) As String
        Dim b As Integer = begin
        Dim l As Integer = len
        If b < 0 Then
            b = str.Length + b
            If b < 0 Then
                l += b
                b = 0
            End If
        End If
        If l <= 0 Or b >= str.Length Then
            Return Nothing
        ElseIf b + l > str.Length Then
            Return str.Substring(b)
        Else
            Return str.Substring(b, l)
        End If
    End Function

    Public Function WStrinLen(ByVal str As String) As Integer
        Dim ret As Integer = 0
        For i As Integer = 0 To str.Length - 1
            Dim c As Char = str.Chars(i)
            If SINGLE_CHARS.IndexOf(c) >= 0 Then
                ret += 1
            Else
                ret += 2
            End If
        Next
        Return ret
    End Function

    Public Function WSubString(ByVal str As String, ByVal begin As Integer) As String
        Dim b As Integer
        If begin >= 0 Then
            b = getWIndex(str, 0, begin)
        Else
            b = getWRevIndex(str, str.Length, -begin)
        End If
        If b >= str.Length Then
            Return Nothing
        Else
            Return str.Substring(b)
        End If
    End Function

    Public Function WSubString(ByVal str As String, ByVal begin As Integer, ByVal len As Integer) As String
        Dim b As Integer
        Dim e As Integer
        If begin >= 0 Then
            b = getWIndex(str, 0, begin)
            e = getWIndex(str, b, len)
        Else
            Dim _len As Integer = Math.Min(-begin, len)
            e = getWRevIndex(str, str.Length, -(begin + _len))
            b = getWRevIndex(str, e, _len)
        End If
        If e <= b Or b >= str.Length Then
            Return Nothing
        ElseIf e >= str.Length Then
            Return str.Substring(b)
        Else
            Return str.Substring(b, e - b)
        End If
    End Function

    Private Function getWIndex(ByVal str As String, ByVal base As Integer, ByVal w As Integer) As Integer
        Dim _w As Integer = 0
        For i As Integer = base To str.Length - 1
            Dim c As Char = str.Chars(i)
            If SINGLE_CHARS.IndexOf(c) >= 0 Then
                _w += 1
            Else
                _w += 2
            End If
            If _w > w Then
                Return i
            End If
        Next
        Return str.Length
    End Function

    Private Function getWRevIndex(ByVal str As String, ByVal base As Integer, ByVal w As Integer) As Integer
        Dim _w As Integer = 0
        For i As Integer = base - 1 To 0 Step -1
            Dim c As Char = str.Chars(i)
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

    Public Function ObjectToString(ByVal o As Object) As String
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