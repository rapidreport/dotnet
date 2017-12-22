Imports System.Globalization
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer

Namespace textformatter

    Public Module TextFormatterUtil

        Public Function Format(v As Object, design As ElementDesign) As String
            Dim _v As Object = ReportUtil.Regularize(v)
            If _v Is Nothing Then
                Return Nothing
            ElseIf TypeOf v Is Boolean Then
                If v Then
                    Return "true"
                Else
                    Return "false"
                End If
            ElseIf TypeOf _v Is Date Then
                Dim formatString As String = Nothing
                If Not design.IsNull("format") Then
                    formatString = design.Get("format")
                End If
                Return FormatDate(CType(_v, Date), formatString)
            ElseIf TypeOf _v Is Decimal Then
                Dim formatString As String = Nothing
                If Not design.IsNull("format") Then
                    formatString = design.Get("format")
                End If
                Return FormatNumber(_v, formatString)
            End If
            Return _v.ToString
        End Function

        Public Function FormatList(list As List(Of Object), separator As String, desc As ElementDesign) As String
            Dim result As New List(Of String)
            For Each o As Object In list
                result.Add(TextFormatterUtil.Format(o, desc))
            Next
            Return String.Join(separator, result.ToArray)
        End Function

        Public Function FormatDate(d As Date, formatString As String) As String
            If formatString Is Nothing Then
                Return FormatDate(d, "yyyy/MM/dd")
            End If

            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim ret As String = ""

            Do While j < formatString.Length
                Dim t As String = formatString.Substring(j)
                Dim p As String = Nothing
                Dim w As Integer = 0
                If t.StartsWith("yyyy") Then
                    p = d.Year.ToString
                    w = 4
                ElseIf t.StartsWith("yy") Then
                    p = d.Year.ToString.Substring(2, 2)
                    w = 2
                ElseIf t.StartsWith("MMMM") Then
                    p = getMonthEnL(d.Month)
                    w = 4
                ElseIf t.StartsWith("MMM") Then
                    p = getMonthEn(d.Month)
                    w = 3
                ElseIf t.StartsWith("MM") Then
                    p = d.Month.ToString.PadLeft(2, "0")
                    w = 2
                ElseIf t.StartsWith("M") Then
                    p = d.Month.ToString
                    w = 1
                ElseIf t.StartsWith("dddd") Then
                    p = getDayOfWeekL(d.DayOfWeek)
                    w = 4
                ElseIf t.StartsWith("ddd") Then
                    p = getDayOfWeek(d.DayOfWeek)
                    w = 3
                ElseIf t.StartsWith("AAA") Then
                    p = getDayOfWeekJ(d.DayOfWeek)
                    w = 3
                ElseIf t.StartsWith("dd") Then
                    p = d.Day.ToString.PadLeft(2, "0")
                    w = 2
                ElseIf t.StartsWith("d") Then
                    p = d.Day.ToString
                    w = 1
                ElseIf t.StartsWith("hh") Then
                    p = d.Hour.ToString.PadLeft(2, "0")
                    w = 2
                ElseIf t.StartsWith("h") Then
                    p = d.Hour.ToString
                    w = 1
                ElseIf t.StartsWith("mm") Then
                    p = d.Minute.ToString.PadLeft(2, "0")
                    w = 2
                ElseIf t.StartsWith("m") Then
                    p = d.Minute.ToString
                    w = 1
                ElseIf t.StartsWith("ss") Then
                    p = d.Second.ToString.PadLeft(2, "0")
                    w = 2
                ElseIf t.StartsWith("s") Then
                    p = d.Second.ToString
                    w = 1
                ElseIf t.StartsWith("nn") Then
                    Dim jcal As New JapaneseCalendar
                    p = jcal.GetYear(d).ToString.PadLeft(2, "0")
                    w = 2
                ElseIf t.StartsWith("n") Then
                    Dim jcal As New JapaneseCalendar
                    p = jcal.GetYear(d).ToString
                    w = 1
                ElseIf t.StartsWith("NN") Then
                    Dim jcal As New JapaneseCalendar
                    Dim year As Integer = jcal.GetYear(d)
                    p = IIf(year = 1, "元", year.ToString.PadLeft(2, "0"))
                    w = 2
                ElseIf t.StartsWith("N") Then
                    Dim jcal As New JapaneseCalendar
                    Dim year As Integer = jcal.GetYear(d)
                    p = IIf(year = 1, "元", year.ToString)
                    w = 1
                ElseIf t.StartsWith("gg") Then
                    Dim culture As CultureInfo = New CultureInfo("ja-JP", True)
                    culture.DateTimeFormat.Calendar = New JapaneseCalendar
                    p = d.ToString("gg", culture)
                    w = 2
                End If
                If p IsNot Nothing Then
                    If i < j Then
                        ret &= formatString.Substring(i, j - i)
                    End If
                    ret &= p
                    j += w
                    i = j
                Else
                    j += 1
                End If
            Loop
            If i < formatString.Length Then
                ret &= formatString.Substring(i)
            End If

            Return ret
        End Function

        Private Function getDayOfWeek(d As DayOfWeek) As String
            Select Case d
                Case DayOfWeek.Sunday
                    Return "Sun"
                Case DayOfWeek.Monday
                    Return "Mon"
                Case DayOfWeek.Tuesday
                    Return "Tue"
                Case DayOfWeek.Wednesday
                    Return "Wed"
                Case DayOfWeek.Thursday
                    Return "Thu"
                Case DayOfWeek.Friday
                    Return "Fri"
                Case DayOfWeek.Saturday
                    Return "Sat"
            End Select
            Return Nothing
        End Function

        Private Function getDayOfWeekL(d As DayOfWeek) As String
            Select Case d
                Case DayOfWeek.Sunday
                    Return "Sunday"
                Case DayOfWeek.Monday
                    Return "Monday"
                Case DayOfWeek.Tuesday
                    Return "Tuesday"
                Case DayOfWeek.Wednesday
                    Return "Wednesday"
                Case DayOfWeek.Thursday
                    Return "Thursday"
                Case DayOfWeek.Friday
                    Return "Friday"
                Case DayOfWeek.Saturday
                    Return "Saturday"
            End Select
            Return Nothing
        End Function

        Private Function getDayOfWeekJ(d As DayOfWeek) As String
            Select Case d
                Case DayOfWeek.Sunday
                    Return "日"
                Case DayOfWeek.Monday
                    Return "月"
                Case DayOfWeek.Tuesday
                    Return "火"
                Case DayOfWeek.Wednesday
                    Return "水"
                Case DayOfWeek.Thursday
                    Return "木"
                Case DayOfWeek.Friday
                    Return "金"
                Case DayOfWeek.Saturday
                    Return "土"
            End Select
            Return Nothing
        End Function

        Private Function getMonthEn(m As Integer) As String
            Select Case m
                Case 1
                    Return "Jan"
                Case 2
                    Return "Feb"
                Case 3
                    Return "Mar"
                Case 4
                    Return "Apr"
                Case 5
                    Return "May"
                Case 6
                    Return "Jun"
                Case 7
                    Return "Jul"
                Case 8
                    Return "Aug"
                Case 9
                    Return "Sep"
                Case 10
                    Return "Oct"
                Case 11
                    Return "Nov"
                Case 12
                    Return "Dec"
            End Select
            Return Nothing
        End Function

        Private Function getMonthEnL(m As Integer) As String
            Select Case m
                Case 1
                    Return "January"
                Case 2
                    Return "February"
                Case 3
                    Return "March"
                Case 4
                    Return "April"
                Case 5
                    Return "May"
                Case 6
                    Return "June"
                Case 7
                    Return "July"
                Case 8
                    Return "August"
                Case 9
                    Return "September"
                Case 10
                    Return "October"
                Case 11
                    Return "November"
                Case 12
                    Return "December"
            End Select
            Return Nothing
        End Function

        Public Function FormatNumber(v As Decimal, formatString As String) As String
            If String.IsNullOrEmpty(formatString) Then
                Return v.ToString
            End If
            With Nothing
                Dim i As Integer = formatString.IndexOf(";")
                If i = -1 Then
                    Return _FormatNumber(v, formatString)
                Else
                    If v >= 0 Then
                        Return _FormatNumber(v, formatString.Substring(0, i))
                    Else
                        Return _FormatNumber(Math.Abs(v), formatString.Substring(i + 1))
                    End If
                End If
            End With
        End Function

        Private Function _FormatNumber(v As Decimal, formatString As String) As String
            If String.IsNullOrEmpty(formatString) Then
                Return v.ToString
            End If
            Dim ret As String = ""
            Dim b As Integer = -1
            Dim e As Integer = -1
            For i As Integer = 0 To formatString.Length - 1
                If b = -1 Then
                    If "0#,.".IndexOf(formatString.Chars(i)) >= 0 Then
                        b = i
                        If b > 0 Then
                            ret = formatString.Substring(0, b)
                        End If
                    End If
                ElseIf e = -1 Then
                    If "0#,.".IndexOf(formatString.Chars(i)) = -1 Then
                        e = i
                        ret &= _FormatNumber_Aux(v, formatString.Substring(b, e - b))
                    End If
                End If
            Next
            If b = -1 Then
                ret = formatString
            ElseIf e = -1 Then
                ret &= _FormatNumber_Aux(v, formatString.Substring(b))
            ElseIf e < formatString.Length Then
                ret &= formatString.Substring(e)
            End If
            Return ret
        End Function

        Private Function _FormatNumber_Aux(v As Decimal, formatString As String) As String
            Dim zeroLength As Integer = 0
            Dim decZeroLength As Integer = 0
            Dim decLength As Integer = 0
            Dim camma As Integer = 0
            Dim negative As Boolean = False
            Dim intValue As String = Nothing
            Dim decValue As String = Nothing
            With Nothing
                Dim i As Integer = formatString.IndexOf(".")
                Dim _intFormat As String
                If i >= 0 Then
                    Dim _decFormat As String = formatString.Substring(i + 1)
                    Dim z As Boolean = True
                    For j As Integer = 0 To _decFormat.Length - 1
                        Dim c As Char = _decFormat.Chars(j)
                        If c.Equals("#"c) Then
                            z = False
                        End If
                        If c.Equals("#"c) Or c.Equals("0"c) Then
                            decLength += 1
                            If z Then
                                decZeroLength += 1
                            End If
                        End If
                    Next
                    _intFormat = formatString.Substring(0, i)
                Else
                    _intFormat = formatString
                End If
                With Nothing
                    Dim z As Boolean = True
                    For j As Integer = 0 To _intFormat.Length - 1
                        Dim c As Char = _intFormat.Chars(_intFormat.Length - j - 1)
                        If c.Equals(","c) And camma = 0 Then
                            camma = j
                        End If
                        If c.Equals("#"c) Then
                            z = False
                        End If
                        If z And c.Equals("0"c) Then
                            zeroLength += 1
                        End If
                    Next
                End With
            End With
            With Nothing
                Dim _intValue As String = Nothing
                With Nothing
                    Dim __v As Decimal = ReportUtil.Round5(v, -decLength)
                    negative = (__v < 0)
                    Dim _v As String = Math.Abs(__v).ToString
                    Dim i As Integer = _v.IndexOf(".")
                    If i >= 0 Then
                        _intValue = _v.Substring(0, i)
                        decValue = _v.Substring(i + 1)
                    Else
                        _intValue = _v
                        decValue = ""
                    End If
                    If _intValue.Equals("0"c) Then
                        _intValue = ""
                    End If
                End With
                decValue = decValue.PadRight(decZeroLength, "0")
                With Nothing
                    _intValue = _intValue.PadLeft(zeroLength, "0")
                    If camma > 0 Then
                        Dim l As Integer = _intValue.Length
                        Dim i As Integer = l Mod camma
                        If i > 0 Then
                            intValue = _intValue.Substring(0, i)
                        Else
                            intValue = ""
                        End If
                        Do While i + camma <= l
                            If intValue.Length > 0 Then
                                intValue &= ","
                            End If
                            intValue &= _intValue.Substring(i, camma)
                            i += camma
                        Loop
                    Else
                        intValue = _intValue
                    End If
                End With
            End With
            Dim ret As String = ""
            If negative Then
                ret = "-"
            End If
            If Not String.IsNullOrEmpty(intValue) Then
                ret &= intValue
            End If
            If Not String.IsNullOrEmpty(decValue) Then
                ret &= "." & decValue
            End If
            Return ret
        End Function

    End Module

End Namespace