Imports System.Drawing.Printing
Imports System.Text.RegularExpressions

Imports jp.co.systembase.report.component

Public Module GdiRenderUtil

    Private Const TOLERANCE As Single = 0.1F

    Private Const VERTICAL_ROTATE_CHARS As String = "～…‥｜ーｰ(){}[]<>（）｛｝「」＜＞。、"

    <Obsolete("GdiTextクラスを利用して下さい。")> _
    Public Sub DrawText( _
      g As Graphics, _
      region As Region, _
      setting As GdiRendererSetting, _
      textDesign As TextDesign, _
      text As String)
        If String.IsNullOrEmpty(text) Then
            Exit Sub
        End If
        If region.GetWidth <= 0 OrElse region.GetHeight <= 0 Then
            Exit Sub
        End If
        If textDesign.Distribute Then
            If textDesign.Vertical Then
                _drawText_distribute_vertical(g, region, setting, textDesign, text)
            Else
                _drawText_distribute(g, region, setting, textDesign, text)
            End If
        ElseIf textDesign.Vertical Then
            If textDesign.ShrinkToFit Then
                _drawText_vertical_shrink(g, region, setting, textDesign, text)
            ElseIf textDesign.Wrap Then
                _drawText_vertical_wrap(g, region, setting, textDesign, text)
            Else
                _drawText_vertical(g, region, setting, textDesign, text)
            End If
        ElseIf textDesign.DecimalPlace > 0 Then
            If textDesign.ShrinkToFit Then
                _drawText_fixdec_shrink(g, region, setting, textDesign, text)
            Else
                _drawText_fixdec(g, region, setting, textDesign, text)
            End If
        ElseIf textDesign.ShrinkToFit Then
            _drawText_shrink(g, region, setting, textDesign, text)
        ElseIf textDesign.Wrap Then
            _drawText_wrap(g, region, setting, textDesign, text)
        Else
            _drawText(g, region, setting, textDesign, text)
        End If
    End Sub

    Private Sub _drawText_distribute( _
      g As Graphics, _
      region As Region, _
      setting As GdiRendererSetting, _
      textDesign As TextDesign, _
      text As String)
        Dim font As Font = getFont(setting, textDesign, True)
        Dim texts As List(Of String) = splitByCr(region, font, text, False)
        Dim color As Color = GetColor(textDesign.Color, Drawing.Color.Black)
        Dim sf As New StringFormat
        sf.Alignment = toStringAlignment(Report.EHAlign.CENTER)
        sf.LineAlignment = toStringAlignment(Report.EVAlign.TOP)
        Dim y As Single = 0
        Select Case textDesign.VAlign
            Case Report.EVAlign.TOP
                y = 0
            Case Report.EVAlign.CENTER
                y = (region.GetHeight - (font.Size * texts.Count)) / 2
                y = Math.Max(y, 0)
            Case Report.EVAlign.BOTTOM
                y = region.GetHeight - (font.Size * texts.Count) - font.Size / 8
                y = Math.Max(y, 0)
        End Select
        Dim yc As Integer = Fix((region.GetHeight + TOLERANCE) / font.Size)
        For i As Integer = 0 To Math.Max(Math.Min(texts.Count, yc) - 1, 0)
            Dim t As String = texts(i)
            Using b As New SolidBrush(color)
                Dim mx As Single = font.Size / 6
                Dim m As List(Of Single) = getDistributeMap(region.GetWidth - mx * 2, t.Length, font)
                For j As Integer = 0 To t.Length - 1
                    g.DrawString(t(j), font, b, region.Left + m(j) + mx, region.Top + y, sf)
                Next
            End Using
            If textDesign.Font.Underline Then
                Using p As New Pen(color)
                    p.Width = textDesign.Font.Size / 13.4F
                    g.DrawLine( _
                      p, _
                      region.Left + font.Size / 6, _
                      region.Top + y + (font.Size - p.Width), _
                      region.Right - font.Size / 6, _
                      region.Top + y + (font.Size - p.Width))
                End Using
            End If
            y += font.Size
        Next
    End Sub

    Private Sub _drawText_distribute_vertical( _
      g As Graphics, _
      region As Region, _
      setting As GdiRendererSetting, _
      textDesign As TextDesign, _
      text As String)
        Dim font As Font = getFont(setting, textDesign, True)
        Dim texts As List(Of String) = splitByCr(region, font, text, False)
        Dim color As Color = GetColor(textDesign.Color, Drawing.Color.Black)
        Dim sf As New StringFormat
        Dim sfr As New StringFormat
        sf.Alignment = toStringAlignment(Report.EHAlign.CENTER)
        sf.LineAlignment = toStringAlignment(Report.EVAlign.CENTER)
        sfr.Alignment = toStringAlignment(Report.EHAlign.CENTER)
        sfr.LineAlignment = toStringAlignment(Report.EVAlign.CENTER)
        sfr.FormatFlags = StringFormatFlags.DirectionVertical
        Dim mx As Single = font.Size / 6
        Dim x As Single = 0
        Select Case textDesign.HAlign
            Case Report.EHAlign.LEFT
                x = font.Size * (texts.Count - 1) + font.Size / 2 + mx
                x = Math.Min(x, region.GetWidth - font.Size / 2 - mx)
            Case Report.EHAlign.CENTER
                x = (region.GetWidth + font.Size * (texts.Count - 1)) / 2
                x = Math.Min(x, region.GetWidth - font.Size / 2 - mx)
            Case Report.EHAlign.RIGHT
                x = region.GetWidth - font.Size / 2 - mx
        End Select
        Dim xc As Integer = Fix((region.GetWidth - mx * 2 + TOLERANCE) / font.Size)
        For i As Integer = 0 To Math.Max(Math.Min(xc, texts.Count) - 1, 0)
            Dim t As String = texts(i)
            Using b As New SolidBrush(color)
                Dim m As List(Of Single) = getDistributeMap(region.GetHeight, t.Length, font)
                For j As Integer = 0 To t.Length - 1
                    Dim c As String = t(j)
                    If VERTICAL_ROTATE_CHARS.IndexOf(c) >= 0 Then
                        g.DrawString(c, font, b, region.Left + x - font.Size / 6, region.Top + m(j), sfr)
                    Else
                        g.DrawString(c, font, b, region.Left + x, region.Top + m(j), sf)
                    End If
                Next
            End Using
            If textDesign.Font.Underline Then
                Using p As New Pen(color)
                    p.Width = textDesign.Font.Size / 13.4F
                    g.DrawLine( _
                      p, _
                      region.Left + x + font.Size / 2 + p.Width, _
                      region.Top, _
                      region.Left + x + font.Size / 2 + p.Width, _
                      region.Bottom)
                End Using
            End If
            x -= font.Size
        Next
    End Sub

    Private Sub _drawText_vertical( _
      g As Graphics, _
      region As Region, _
      setting As GdiRendererSetting, _
      textDesign As TextDesign, _
      text As String)
        Dim font As Font = getFont(setting, textDesign, True)
        Dim texts As List(Of String) = splitByCr(region, font, text, False)
        _drawText_vertical_aux(g, region, textDesign, texts, font)
    End Sub

    Private Sub _drawText_vertical_shrink( _
      g As Graphics, _
      region As Region, _
      setting As GdiRendererSetting, _
      textDesign As TextDesign, _
      text As String)
        Dim font As Font = getFont(setting, textDesign, True)
        Dim texts As List(Of String) = splitByCr(region, font, text, False)
        With Nothing
            Dim m As Integer = 0
            For Each t As String In texts
                If m < t.Length Then
                    m = t.Length
                End If
            Next
            If m > 0 Then
                Dim fontSize As Single = Math.Max(region.GetHeight / m, setting.ShrinkFontSizeMin)
                If font.Size > fontSize Then
                    font = New Font(font.Name, fontSize, font.Style)
                End If
            End If
        End With
        _drawText_vertical_aux(g, region, textDesign, texts, font)
    End Sub

    Private Sub _drawText_vertical_wrap( _
      g As Graphics, _
      region As Region, _
      setting As GdiRendererSetting, _
      textDesign As TextDesign, _
      text As String)
        Dim font As Font = getFont(setting, textDesign, True)
        Dim texts As List(Of String) = splitByCr(region, font, text, True)
        _drawText_vertical_aux(g, region, textDesign, texts, font)
    End Sub

    Private Sub _drawText_fixdec( _
      g As Graphics, _
      region As Region, _
      setting As GdiRendererSetting, _
      textDesign As TextDesign, _
      text As String)
        Dim fd As New _FixDec(text)
        Dim font As Font = getFont(setting, textDesign, True)
        fd.DrawText(g, region, setting, textDesign, font)
    End Sub

    Private Sub _drawText_fixdec_shrink( _
      g As Graphics, _
      region As Region, _
      setting As GdiRendererSetting, _
      textDesign As TextDesign, _
      text As String)
        Dim fd As New _FixDec(text)
        Dim font As Font = getFont(setting, textDesign, True)
        font = getFitFont(g, region, setting, fd.GetFullText(textDesign.DecimalPlace), font)
        fd.DrawText(g, region, setting, textDesign, font)
    End Sub

    Private Sub _drawText_wrap( _
      g As Graphics, _
      region As Region, _
      setting As GdiRendererSetting, _
      textDesign As TextDesign, _
      text As String)
        Dim font As Font = getFont(setting, textDesign, False)
        Dim color As Color = GetColor(textDesign.Color, Drawing.Color.Black)
        Dim r As New RectangleF(region.Left, region.Top, region.GetWidth, region.GetHeight)
        Dim sf As New StringFormat
        sf.Alignment = toStringAlignment(textDesign.HAlign)
        sf.LineAlignment = toStringAlignment(textDesign.VAlign)
        Using b As New SolidBrush(color)
            g.DrawString(text, font, b, r, sf)
        End Using
    End Sub

    Private Sub _drawText_shrink( _
      g As Graphics, _
      region As Region, _
      setting As GdiRendererSetting, _
      textDesign As TextDesign, _
      text As String)
        Dim font As Font = getFont(setting, textDesign, False)
        font = getFitFont(g, region, setting, text, font)
        Dim color As Color = GetColor(textDesign.Color, Drawing.Color.Black)
        Dim r As New RectangleF(region.Left, region.Top, region.GetWidth, region.GetHeight)
        Dim sf As New StringFormat
        sf.Alignment = toStringAlignment(textDesign.HAlign)
        sf.LineAlignment = toStringAlignment(textDesign.VAlign)
        sf.FormatFlags = StringFormatFlags.NoWrap
        Using b As New SolidBrush(color)
            g.DrawString(text, font, b, r, sf)
        End Using
    End Sub

    Private Sub _drawText( _
      g As Graphics, _
      region As Region, _
      setting As GdiRendererSetting, _
      textDesign As TextDesign, _
      text As String)
        Dim font As Font = getFont(setting, textDesign, False)
        Dim color As Color = GetColor(textDesign.Color, Drawing.Color.Black)
        Dim r As New RectangleF(region.Left, region.Top, region.GetWidth, region.GetHeight)
        Dim sf As New StringFormat
        sf.Alignment = toStringAlignment(textDesign.HAlign)
        sf.LineAlignment = toStringAlignment(textDesign.VAlign)
        sf.FormatFlags = StringFormatFlags.NoWrap
        Using b As New SolidBrush(color)
            g.DrawString(text, font, b, r, sf)
        End Using
    End Sub

    Private Sub _drawText_vertical_aux( _
      g As Graphics, _
      region As Region, _
      textDesign As TextDesign, _
      texts As List(Of String), _
      font As Font)
        Dim color As Color = GetColor(textDesign.Color, Drawing.Color.Black)
        Dim mx As Single = font.Size / 6
        Dim x As Single = 0
        Select Case textDesign.HAlign
            Case Report.EHAlign.LEFT
                x = font.Size * (texts.Count - 1) + font.Size / 2 + mx
                x = Math.Min(x, region.GetWidth - font.Size / 2 - mx)
            Case Report.EHAlign.CENTER
                x = (region.GetWidth + font.Size * (texts.Count - 1)) / 2
                x = Math.Min(x, region.GetWidth - font.Size / 2 - mx)
            Case Report.EHAlign.RIGHT
                x = region.GetWidth - font.Size / 2 - mx
        End Select
        Dim sf As New StringFormat
        Dim sfr As New StringFormat
        sf.Alignment = toStringAlignment(Report.EHAlign.CENTER)
        sf.LineAlignment = toStringAlignment(Report.EVAlign.TOP)
        sfr.Alignment = toStringAlignment(Report.EHAlign.LEFT)
        sfr.LineAlignment = toStringAlignment(Report.EVAlign.CENTER)
        sfr.FormatFlags = StringFormatFlags.DirectionVertical
        Dim cw As Single = region.GetWidth - mx * 2
        Dim ch As Single = region.GetHeight
        Dim xc As Integer = Fix((cw + TOLERANCE) / font.Size)
        Dim yc As Integer = Fix((ch + TOLERANCE) / font.Size)
        Using b As New SolidBrush(color)
            For i As Integer = 0 To Math.Min(texts.Count, xc) - 1
                Dim t As String = texts(i)
                Dim y As Single = 0
                Select Case textDesign.VAlign
                    Case Report.EVAlign.TOP
                        y = 0
                    Case Report.EVAlign.CENTER
                        y = (region.GetHeight - font.Size * t.Length) / 2
                        y = Math.Max(y, 0)
                    Case Report.EVAlign.BOTTOM
                        y = region.GetHeight - font.Size * t.Length
                        y = Math.Max(y, 0)
                End Select
                Dim _yc As Integer = Math.Min(t.Length, yc)
                If textDesign.Font.Underline Then
                    Using p As New Pen(color)
                        p.Width = textDesign.Font.Size / 13.4F
                        g.DrawLine( _
                          p, _
                          region.Left + x + font.Size / 2, _
                          region.Top + y, _
                          region.Left + x + font.Size / 2, _
                          region.Top + y + font.Size * _yc)
                    End Using
                End If
                For j As Integer = 0 To _yc - 1
                    Dim c As String = t(j)
                    If VERTICAL_ROTATE_CHARS.IndexOf(c) >= 0 Then
                        g.DrawString(c, font, b, region.Left + x - font.Size / 6, region.Top + y, sfr)
                    Else
                        g.DrawString(c, font, b, region.Left + x, region.Top + y, sf)
                    End If
                    y += font.Size
                Next
                x -= font.Size
            Next
        End Using
    End Sub

    Private Function getDistributeMap(w As Single, c As Integer, font As Font) As List(Of Single)
        Dim ret As New List(Of Single)
        If c = 1 Then
            ret.Add(w / 2)
        Else
            Dim t As Single = 1 + font.Size / 2
            Do
                ret.Add(t)
                t += (w - font.Size - 2) / (c - 1)
            Loop While t < w And ret.Count < c
        End If
        Return ret
    End Function

    Private Function splitByCr( _
      region As Region, _
      font As Font, _
      text As String, _
      wrap As Boolean) As List(Of String)
        Dim h As Single = region.GetHeight
        Dim yc As Integer = Fix((h + TOLERANCE) / font.Size)
        Dim ret As New List(Of String)
        For Each t As String In text.Split(vbCr)
            t = t.Replace(vbLf, "")
            If wrap Then
                Do
                    If t.Length > yc Then
                        ret.Add(t.Substring(0, yc))
                        t = t.Substring(yc)
                    Else
                        ret.Add(t)
                        Exit Do
                    End If
                Loop
            Else
                ret.Add(t)
            End If
        Next
        Return ret
    End Function

    Private Class _FixDec

        Public Text1 As String
        Public Text2 As String
        Public Text3 As String

        Public Sub New(text As String)
            Dim t As String = text
            With Regex.Match(t, "([^0-9]*)$")
                Me.Text3 = .Groups(0).Value
                t = t.Substring(0, t.Length - Me.Text3.Length)
            End With
            With Regex.Match(t, "(\.[0-9]*)?$")
                Me.Text2 = .Groups(0).Value
                Me.Text1 = t.Substring(0, t.Length - Me.Text2.Length)
            End With
        End Sub

        Public Function GetFullText2(width As Integer) As String
            Dim ret As String = Me.Text2
            If ret.Length = 0 Then
                ret = "."
            End If
            Return ret.PadRight(width + 1, "0")
        End Function

        Public Function GetFullText(width As Integer) As String
            Return Me.Text1 & Me.GetFullText2(width) & Me.Text3
        End Function

        Public Sub DrawText( _
          g As Graphics, _
          region As Region, _
          setting As GdiRendererSetting, _
          textDesign As TextDesign, _
          font As Font)
            Dim color As Color = GetColor(textDesign.Color, Drawing.Color.Black)
            Dim ls As New SizeF(10000, 10000)
            Dim y As Single = 0
            Select Case textDesign.VAlign
                Case Report.EVAlign.TOP
                    y = 0
                Case Report.EVAlign.CENTER
                    y = (region.GetHeight - font.Size - font.Size / 8) / 2
                Case Report.EVAlign.BOTTOM
                    y = region.GetHeight - font.Size - font.Size / 8
            End Select
            Using b As New SolidBrush(color)
                Dim sf As New StringFormat
                sf.Alignment = StringAlignment.Near
                sf.LineAlignment = StringAlignment.Near
                sf.FormatFlags = StringFormatFlags.NoWrap
                Dim t As String = Me.Text1 & Me.GetFullText2(textDesign.DecimalPlace)
                Dim ft As String = t & Me.Text3
                Dim w As Single = g.MeasureString(t, font, ls, sf).Width
                Dim fw As Single = g.MeasureString(ft, font, ls, sf).Width
                Dim x As Single = 0
                Select Case textDesign.HAlign
                    Case Report.EHAlign.LEFT
                        x = 0
                    Case Report.EHAlign.CENTER
                        x = (region.GetWidth - fw) / 2
                        x = Math.Max(x, 0)
                    Case Report.EHAlign.RIGHT
                        x = region.GetWidth - fw
                        x = Math.Max(x, 0)
                End Select
                With Nothing
                    Dim r As New RectangleF( _
                      region.Left + x, _
                      region.Top + y, _
                      region.GetWidth - x, _
                      region.GetHeight - y)
                    g.DrawString(Me.Text1 & Me.Text2, font, b, r, sf)
                End With
                If Me.Text3.Length > 0 Then
                    Dim _x As Single = x + w - font.Size / 3
                    If _x < region.GetWidth Then
                        Dim r As New RectangleF( _
                          region.Left + _x, _
                          region.Top + y, _
                          region.GetWidth - _x, _
                          region.GetHeight - y)
                        g.DrawString(Me.Text3, font, b, r, sf)
                    End If
                End If
                If textDesign.Font.Underline Then
                    Dim _x As Single = x + fw
                    _x = Math.Min(_x, region.GetWidth)
                    Dim m As Single = font.Size / 6
                    Using p As New Pen(color)
                        p.Width = font.Size / 13.4F
                        g.DrawLine( _
                          p, _
                          region.Left + x + m, _
                          region.Top + y + (font.Size - p.Width), _
                          region.Left + _x - m, _
                          region.Top + y + (font.Size - p.Width))
                    End Using
                End If
            End Using
        End Sub

    End Class

    Private Function getFont( _
      setting As GdiRendererSetting, _
      textDesign As TextDesign, _
      ignoreUnderline As Boolean) As Font
        Dim style As FontStyle = FontStyle.Regular
        If textDesign.Font.Bold Then
            style = style Or FontStyle.Bold
        End If
        If textDesign.Font.Italic Then
            style = style Or FontStyle.Italic
        End If
        If textDesign.Font.Underline Then
            If Not ignoreUnderline Then
                style = style Or FontStyle.Underline
            End If
        End If
        Return New Font( _
          setting.GetFont(textDesign.Font.Name), _
          textDesign.Font.Size, _
          style)
    End Function

    Private Function toStringAlignment( _
      align As Report.EHAlign) As StringAlignment
        Select Case align
            Case Report.EHAlign.LEFT
                Return StringAlignment.Near
            Case Report.EHAlign.CENTER
                Return StringAlignment.Center
            Case Report.EHAlign.RIGHT
                Return StringAlignment.Far
        End Select
    End Function

    Private Function toStringAlignment( _
      align As Report.EVAlign) As StringAlignment
        Select Case align
            Case Report.EVAlign.TOP
                Return StringAlignment.Near
            Case Report.EVAlign.CENTER
                Return StringAlignment.Center
            Case Report.EVAlign.BOTTOM
                Return StringAlignment.Far
        End Select
    End Function

    Private Function getFitFont( _
      g As Graphics, _
      region As Region, _
      setting As GdiRendererSetting, _
      text As String, _
      baseFont As Font) As Font
        Dim ls As New SizeF(10000, 10000)
        If g.MeasureString(text, baseFont, ls).Width <= region.GetWidth Then
            Return baseFont
        End If
        Dim _i As Integer = 0
        Do While setting.ShrinkFontSizeMin + _i * setting.ShrinkFontSizeStep < baseFont.Size
            _i += 1
        Loop
        For i As Integer = _i - 1 To 1 Step -1
            Dim s As Single = setting.ShrinkFontSizeMin + i * setting.ShrinkFontSizeStep
            Dim font As New Font(baseFont.Name, s, baseFont.Style)
            If g.MeasureString(text, font, ls).Width <= region.GetWidth Then
                Return font
            End If
        Next
        Return New Font(baseFont.Name, setting.ShrinkFontSizeMin, baseFont.Style)
    End Function

    Public Function GetColor(v As String) As Color
        Return GetColor(v, Color.Empty)
    End Function

    Public Function GetColor(v As String, defaultColor As Color) As Color
        If Not String.IsNullOrEmpty(v) Then
            If v.StartsWith("#") AndAlso v.Length = 7 Then
                Dim _v As String = v.Substring(1).ToLower
                For i As Integer = 0 To 5
                    If "0123456789abcdef".IndexOf(_v(i)) < 0 Then
                        Return defaultColor
                    End If
                Next
                Return Color.FromArgb( _
                  Convert.ToInt32(_v.Substring(0, 2), 16), _
                  Convert.ToInt32(_v.Substring(2, 2), 16), _
                  Convert.ToInt32(_v.Substring(4, 2), 16))
            Else
                If Array.IndexOf(renderer.RenderUtil.COLOR_NAMES, v.ToLower) >= 0 Then
                    Return Color.FromName(v)
                End If
            End If
        End If
        Return defaultColor
    End Function

    Public Function GetMockData(desc As String) As Object
        Try
            Dim m As Match = Regex.Match(desc.ToUpper, "([SWDT])(-?[0-9]+)?(\.([0-9]+))?")
            If m.Success Then
                Dim l As Integer = 0
                Dim f As Integer = 0
                If Not String.IsNullOrEmpty(m.Groups(2).ToString) Then
                    l = CType(m.Groups(2).ToString, Integer)
                End If
                If Not String.IsNullOrEmpty(m.Groups(4).ToString) Then
                    f = CType(m.Groups(4).ToString, Integer)
                End If
                Select Case m.Groups(1).ToString
                    Case "S"
                        l = Math.Abs(l)
                        If l = 1 Then
                            Return "X"
                        ElseIf l > 1 Then
                            Return "X" & "".PadRight(l - 2, "#") & "X"
                        End If
                    Case "W"
                        l = Math.Abs(l)
                        If l = 1 Then
                            Return "Ｘ"
                        ElseIf l > 1 Then
                            Return "Ｘ" & "".PadRight(l - 2, "＃") & "Ｘ"
                        End If
                    Case "D"
                        Dim t As String = ""
                        If l < 0 Then
                            t = "-"
                        End If
                        l = Math.Abs(l)
                        If f > 0 Then
                            If l - f > 0 Then
                                t &= "9".PadRight(l - f, "0")
                                t &= "." & "9".PadLeft(f, "0")
                            End If
                        Else
                            If l = 1 Then
                                t &= "9"
                            Else
                                t &= "9" & "".PadRight(l - 2, "0") & "9"
                            End If
                        End If
                        Return CType(t, Decimal)
                    Case "T"
                        Return New DateTime(2000, 12, 31, 23, 59, 59)
                End Select
            End If
        Catch ex As Exception
        End Try
        Return desc
    End Function

    Public Sub SetUpPrinterSetting(pageSettings As PageSettings, reportDesign As ReportDesign)
        If reportDesign.PrinterName IsNot Nothing Then
            pageSettings.PrinterSettings.PrinterName = reportDesign.PrinterName
        End If
    End Sub

    Public Sub SetUpPageSettings(pageSettings As PageSettings, reportDesign As ReportDesign)
        Dim kind As PaperKind = PaperKind.Custom
        Select Case reportDesign.PaperDesign.PaperType
            Case Report.EPaperType.A3
                kind = PaperKind.A3
            Case Report.EPaperType.A4
                kind = PaperKind.A4
            Case Report.EPaperType.A5
                kind = PaperKind.A5
            Case Report.EPaperType.B4
                kind = PaperKind.B4
            Case Report.EPaperType.B5
                kind = PaperKind.B5
        End Select
        With Nothing
            Dim pageSizeIsFound As Boolean = False
            If reportDesign.PaperName IsNot Nothing Then
                For Each size As PaperSize In pageSettings.PrinterSettings.PaperSizes
                    If reportDesign.PaperName.Equals(size.PaperName) Then
                        pageSettings.PaperSize = size
                        pageSizeIsFound = True
                        Exit For
                    End If
                Next
            End If
            If Not pageSizeIsFound Then
                If Not kind.Equals(PaperKind.Custom) Then
                    For Each size As PaperSize In pageSettings.PrinterSettings.PaperSizes
                        If size.Kind.Equals(kind) Then
                            pageSettings.PaperSize = size
                            pageSizeIsFound = True
                            Exit For
                        End If
                    Next
                End If
            End If
            If Not pageSizeIsFound Then
                With reportDesign.PaperDesign.GetActualSize.ToPoint(reportDesign.PaperDesign)
                    pageSettings.PaperSize = _
                      New PaperSize("_custom_", (.Width * 100 / 72), (.Height * 100 / 72))
                End With
            End If
        End With
        If reportDesign.PaperSource IsNot Nothing Then
            For Each source As PaperSource In pageSettings.PrinterSettings.PaperSources
                If reportDesign.PaperSource.Equals(source.SourceName) Then
                    pageSettings.PaperSource = source
                End If
            Next
        End If
        pageSettings.Landscape = reportDesign.PaperDesign.Landscape
    End Sub

    Public Sub SetUpGraphics( _
      g As Graphics, _
      reportDesign As ReportDesign, _
      pageIndex As Integer, _
      hardMargin As PointF)
        g.PageUnit = GraphicsUnit.Point
        Dim m As PaperMarginDesign = reportDesign.PaperDesign.Margin.ToPoint(reportDesign.PaperDesign)
        If (pageIndex Mod 2) And m.OddReverse Then
            g.TranslateTransform(m.Right - hardMargin.X, m.Top - hardMargin.Y)
        Else
            g.TranslateTransform(m.Left - hardMargin.X, m.Top - hardMargin.Y)
        End If
    End Sub

    Public Function ToPixelX(g As Graphics, v As Decimal) As Integer
        Return (v * g.DpiX) / 72
    End Function

    Public Function ToPixelY(g As Graphics, v As Decimal) As Integer
        Return (v * g.DpiY) / 72
    End Function

    Public Function ToPointX(g As Graphics, v As Integer) As Decimal
        Return (72 * v) / g.DpiY
    End Function

    Public Function ToPointY(g As Graphics, v As Integer) As Decimal
        Return (72 * v) / g.DpiY
    End Function

End Module