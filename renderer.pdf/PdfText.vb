Imports System.Text.RegularExpressions
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports jp.co.systembase.report.component

Public Class PdfText

    Public Renderer As PdfRenderer
    Public Region As Region
    Public TextDesign As TextDesign
    Public Text As String
    Public ContentByte As PdfContentByte
    Public Font As BaseFont
    Public TextMatrix As List(Of Single) = Nothing

    Protected Const TOLERANCE As Single = 0.1F
    Protected Const OFFSET_Y As Single = -0.5F
    Protected Const MARGIN_X As Single = 2.0F
    Protected Const MARGIN_BOTTOM As Single = 2.0F

    Protected Const VERTICAL_ROTATE_CHARS As String = "～…‥｜ーｰ(){}[]<>（）｛｝「」＜＞"
    Protected Const VERTICAL_SHIFT_CHARS As String = "。、"

    Public Sub New( _
      ByVal renderer As PdfRenderer, _
      ByVal region As Region, _
      ByVal textDesign As TextDesign, _
      ByVal text As String)
        Me.Renderer = renderer
        Me.Region = region
        Me.TextDesign = textDesign
        Me.Text = text
        Me.ContentByte = Me.Renderer.Writer.DirectContent
        Me.Font = Me.Renderer.Setting.GetFont(textDesign.Font.Name)
    End Sub

    Public Overridable Sub Draw()
        If Me._IsEmpty Then
            Return
        End If
        ContentByte.SaveState()
        Try
            _Draw_Preprocess()
            If TextDesign.Distribute Then
                If TextDesign.Vertical Then
                    _Draw_DistributeVertical()
                Else
                    _Draw_Distribute()
                End If
            ElseIf TextDesign.Vertical Then
                If TextDesign.ShrinkToFit Then
                    _Draw_VerticalShrink()
                ElseIf TextDesign.Wrap Then
                    _Draw_VerticalWrap()
                Else
                    _Draw_Vertical()
                End If
            ElseIf TextDesign.DecimalPlace > 0 Then
                If TextDesign.ShrinkToFit Then
                    _Draw_FixdecShrink()
                Else
                    _Draw_Fixdec()
                End If
            ElseIf TextDesign.ShrinkToFit Then
                _Draw_Shrink()
            ElseIf TextDesign.Wrap Then
                _Draw_Wrap()
            Else
                _Draw()
            End If
        Finally
            ContentByte.RestoreState()
        End Try
    End Sub

    Protected Overridable Sub _Draw_Preprocess()
        If TextDesign.Color IsNot Nothing Then
            Dim c As Color = GetColor(TextDesign.Color)
            If c IsNot Nothing Then
                ContentByte.SetColorFill(c)
                ContentByte.SetColorStroke(c)
            End If
        End If
        If TextDesign.Font.Bold Then
            ContentByte.SetTextRenderingMode(PdfContentByte.TEXT_RENDER_MODE_FILL_STROKE)
            If Not Report.Compatibility._4_6_PdfFontBold Then
                ContentByte.SetLineWidth(TextDesign.Font.Size * 0.01)
            End If
        Else
            ContentByte.SetTextRenderingMode(PdfContentByte.TEXT_RENDER_MODE_FILL)
        End If
    End Sub

    Protected Overridable Sub _Draw_Distribute()
        Dim fontSize As Single = TextDesign.Font.Size
        Dim texts As List(Of String) = _SplitByCr(Region, TextDesign, Text, False)
        Dim y As Single = 0
        Select Case textDesign.VAlign
            Case Report.EVAlign.TOP
                y = 0
            Case Report.EVAlign.CENTER
                y = (region.GetHeight - (fontSize * texts.Count)) / 2
                y = Math.Max(y, 0)
            Case Report.EVAlign.BOTTOM
                y = region.GetHeight - (fontSize * texts.Count) - MARGIN_BOTTOM
                y = Math.Max(y, 0)
        End Select
        y += OFFSET_Y
        Dim rows As Integer = Fix((region.GetHeight + TOLERANCE) / fontSize)
        For i As Integer = 0 To Math.Max(Math.Min(texts.Count, rows) - 1, 0)
            Dim t As String = texts(i)
            Dim m As List(Of Single) = _GetDistributeMap(Region.GetWidth - MARGIN_X * 2, t.Length, fontSize)
            ContentByte.BeginText()
            ContentByte.SetFontAndSize(Font, fontSize)
            For j As Integer = 0 To t.Length - 1
                Dim c As String = t(j)
                _DrawText(fontSize, c, m(j) - _GetTextWidth(Renderer.Setting, TextDesign, Font, fontSize, c) / 2 + MARGIN_X, y)
            Next
            ContentByte.EndText()
            If textDesign.Font.Underline Then
                Dim lw As Single = (fontSize / 13.4) * Renderer.Setting.UnderlineWidthCoefficient
                _DrawUnderline(fontSize, MARGIN_X, y, Region.GetWidth - MARGIN_X * 2, lw)
            End If
            y += fontSize
        Next
    End Sub

    Private Sub _Draw_DistributeVertical()
        Dim fontSize As Single = TextDesign.Font.Size
        Dim texts As List(Of String) = _SplitByCr(Region, TextDesign, Text, False)
        Dim x As Single = 0
        Select Case TextDesign.HAlign
            Case Report.EHAlign.LEFT
                x = fontSize * (texts.Count - 1) + fontSize / 2 + MARGIN_X
                x = Math.Min(x, Region.GetWidth - fontSize / 2 - MARGIN_X)
            Case Report.EHAlign.CENTER
                x = (Region.GetWidth + (texts.Count - 1) * fontSize) / 2
                x = Math.Min(x, Region.GetWidth - fontSize / 2 - MARGIN_X)
            Case Report.EHAlign.RIGHT
                x = Region.GetWidth - fontSize / 2 - MARGIN_X
        End Select
        Dim cols As Integer = Fix(((Region.GetWidth - MARGIN_X * 2) + TOLERANCE) / fontSize)
        For i As Integer = 0 To Math.Max(Math.Min(texts.Count, cols) - 1, 0)
            Dim t As String = texts(i)
            If TextDesign.Font.Underline Then
                Dim lw As Single = (fontSize / 13.4) * Renderer.Setting.UnderlineWidthCoefficient
                _DrawVerticalUnderLine(fontSize, x + fontSize / 2, 0, Region.GetHeight, lw)
            End If
            With Nothing
                Dim m As List(Of Single) = _GetDistributeMap(Region.GetHeight - MARGIN_BOTTOM, t.Length, fontSize)
                ContentByte.SetFontAndSize(Font, fontSize)
                ContentByte.BeginText()
                For j As Integer = 0 To t.Length - 1
                    _DrawVerticalChar(fontSize, t(j), x, m(j) - fontSize / 2 + OFFSET_Y)
                Next
                ContentByte.EndText()
            End With
            x -= fontSize
        Next
    End Sub

    Protected Overridable Sub _Draw_Vertical()
        Dim texts As List(Of String) = _SplitByCr(Region, TextDesign, Text, False)
        _Draw_Vertical_Aux(TextDesign.Font.Size, texts)
    End Sub

    Protected Overridable Sub _Draw_VerticalShrink()
        Dim fontSize As Single = TextDesign.Font.Size
        Dim texts As List(Of String) = _SplitByCr(Region, TextDesign, Text, False)
        With Nothing
            Dim m As Integer = 0
            For Each t As String In texts
                If m < t.Length Then
                    m = t.Length
                End If
            Next
            If m > 0 Then
                Dim _fontSize As Single = Math.Max((Region.GetHeight - MARGIN_BOTTOM) / m, Renderer.Setting.ShrinkFontSizeMin)
                fontSize = Math.Min(fontSize, _fontSize)
            End If
        End With
        _Draw_Vertical_Aux(fontSize, texts)
    End Sub

    Protected Overridable Sub _Draw_VerticalWrap()
        Dim texts As List(Of String) = _SplitByCr(Region, TextDesign, Text, True)
        _Draw_Vertical_Aux(TextDesign.Font.Size, texts)
    End Sub

    Protected Overridable Sub _Draw_Fixdec()
        Dim fd As New _FixDec(Me)
        fd.DrawText(TextDesign.Font.Size)
    End Sub

    Protected Overridable Sub _Draw_FixdecShrink()
        Dim fd As New _FixDec(Me)
        Dim texts As New List(Of String)
        texts.Add(fd.GetFullText(TextDesign.DecimalPlace))
        Dim fontSize As Single = _GetFitFontSize(Region, Renderer.Setting, Font, TextDesign, TextDesign.Font.Size, texts)
        fd.DrawText(fontSize)
    End Sub

    Protected Overridable Sub _Draw_Shrink()
        Dim texts As List(Of String) = _SplitText(Region, Renderer.Setting, TextDesign, Font, Text, False)
        Dim fontSize As Single = _GetFitFontSize(Region, Renderer.Setting, Font, TextDesign, TextDesign.Font.Size, texts)
        _Draw_Aux(fontSize, texts)
    End Sub

    Protected Overridable Sub _Draw_Wrap()
        Dim texts As List(Of String) = _SplitText(Region, Renderer.Setting, TextDesign, Font, Text, True)
        _Draw_Aux(TextDesign.Font.Size, texts)
    End Sub

    Protected Overridable Sub _Draw()
        Dim texts As List(Of String) = _SplitText(Region, Renderer.Setting, TextDesign, Font, Text, False)
        _Draw_Aux(TextDesign.Font.Size, texts)
    End Sub

    Protected Overridable Sub _Draw_Aux( _
      ByVal fontSize As Single, _
      ByVal texts As List(Of String))
        Dim y As Single = 0
        Select Case TextDesign.VAlign
            Case Report.EVAlign.TOP
                y = 0
            Case Report.EVAlign.CENTER
                y = (Region.GetHeight - fontSize * texts.Count) / 2
                y = Math.Max(y, 0)
            Case Report.EVAlign.BOTTOM
                y = Region.GetHeight - fontSize * texts.Count - MARGIN_BOTTOM
                y = Math.Max(y, 0)
        End Select
        y += OFFSET_Y
        Dim rows As Integer = Fix((Region.GetHeight + TOLERANCE) / fontSize)
        For i As Integer = 0 To Math.Max(Math.Min(texts.Count, rows) - 1, 0)
            Dim t As String = texts(i)
            Dim w As Single = _GetTextWidth(Renderer.Setting, TextDesign, Font, fontSize, t)
            With Nothing
                Dim rw As Single = Region.GetWidth - MARGIN_X * 2
                If w > rw Then
                    Dim _t As String = ""
                    Dim _w As Single = 0
                    For j As Integer = 1 To t.Length
                        Dim __t As String = t.Substring(0, j)
                        Dim __w As Single = _GetTextWidth(Renderer.Setting, TextDesign, Font, fontSize, __t)
                        If __w <= rw + TOLERANCE Then
                            _t = __t
                            _w = __w
                        Else
                            Exit For
                        End If
                    Next
                    t = _t
                    w = _w
                End If
            End With
            Dim x As Single = 0
            Select Case TextDesign.HAlign
                Case Report.EHAlign.LEFT
                    x = MARGIN_X
                Case Report.EHAlign.CENTER
                    x = (Region.GetWidth - w) / 2
                    x = Math.Max(x, MARGIN_X)
                Case Report.EHAlign.RIGHT
                    x = Region.GetWidth - w - MARGIN_X
                    x = Math.Max(x, MARGIN_X)
            End Select
            ContentByte.SetFontAndSize(Font, fontSize)
            ContentByte.BeginText()
            _DrawText(fontSize, t, x, y)
            ContentByte.EndText()
            If TextDesign.Font.Underline Then
                Dim lw As Single = (fontSize / 13.4) * Renderer.Setting.UnderlineWidthCoefficient
                _DrawUnderline(fontSize, x, y, w, lw)
            End If
            y += fontSize
        Next
    End Sub

    Protected Overridable Sub _Draw_Vertical_Aux( _
      ByVal fontSize As Single, _
      ByVal texts As List(Of String))
        Dim x As Single = 0
        Select Case TextDesign.HAlign
            Case Report.EHAlign.LEFT
                x = fontSize * (texts.Count - 1) + fontSize / 2 + MARGIN_X
                x = Math.Min(x, Region.GetWidth - fontSize / 2 - MARGIN_X)
            Case Report.EHAlign.CENTER
                x = (Region.GetWidth + (texts.Count - 1) * fontSize) / 2
                x = Math.Min(x, Region.GetWidth - fontSize / 2 - MARGIN_X)
            Case Report.EHAlign.RIGHT
                x = Region.GetWidth - fontSize / 2 - MARGIN_X
        End Select
        Dim cols As Integer = Fix(((Region.GetWidth - MARGIN_X * 2) + TOLERANCE) / fontSize)
        Dim rows As Integer = Fix((Region.GetHeight + TOLERANCE) / fontSize)
        For i As Integer = 0 To Math.Max(Math.Min(texts.Count, cols) - 1, 0)
            Dim t As String = texts(i)
            Dim y As Single = 0
            Select Case TextDesign.VAlign
                Case Report.EVAlign.TOP
                    y = 0
                Case Report.EVAlign.CENTER
                    y = (Region.GetHeight - fontSize * t.Length) / 2
                    y = Math.Max(y, 0)
                Case Report.EVAlign.BOTTOM
                    y = Region.GetHeight - fontSize * t.Length - MARGIN_BOTTOM
                    y = Math.Max(y, 0)
            End Select
            y += OFFSET_Y
            Dim _yc As Integer = Math.Min(t.Length, rows)
            If TextDesign.Font.Underline Then
                Dim lw As Single = (fontSize / 13.4) * Renderer.Setting.UnderlineWidthCoefficient
                _DrawVerticalUnderLine(fontSize, x + fontSize / 2, y, _yc * fontSize, lw)
            End If
            ContentByte.SetFontAndSize(Font, fontSize)
            ContentByte.BeginText()
            For j As Integer = 0 To _yc - 1
                _DrawVerticalChar(fontSize, t(j), x, y)
                y += fontSize
            Next
            ContentByte.EndText()
            x -= fontSize
        Next
    End Sub

    Protected Overridable Sub _SetTextMatrix( _
      ByVal fontSize As Single, _
      ByVal x As Single, _
      ByVal y As Single)
        Dim trans As PdfRenderer.TransClass = Renderer.Trans
        Dim _x As Single = Region.Left + x
        Dim _y As Single = (Region.Top + y + fontSize) - (fontSize / 13.4)
        If TextMatrix IsNot Nothing Then
            ContentByte.SetTextMatrix(TextMatrix(0), TextMatrix(1), TextMatrix(2), TextMatrix(3), _
                                      trans.X(_x), trans.Y(_y))
        Else
            If TextDesign.Font.Italic Then
                ContentByte.SetTextMatrix(1, 0, 0.3, 1, trans.X(_x), trans.Y(_y))
            Else
                ContentByte.SetTextMatrix(trans.X(_x), trans.Y(_y))
            End If
        End If
    End Sub

    Protected Overridable Sub _SetRotateTextMatrix( _
      ByVal fontSize As Single, _
      ByVal x As Single, _
      ByVal y As Single)
        Dim trans As PdfRenderer.TransClass = Renderer.Trans
        Dim _x As Single = Region.Left + x
        Dim _y As Single = (Region.Top + y + fontSize) - (fontSize / 13.4)
        If TextDesign.Font.Italic Then
            ContentByte.SetTextMatrix(-0.3, -1, 1, 0, trans.X(_x), trans.Y(_y))
        Else
            ContentByte.SetTextMatrix(0, -1, 1, 0, trans.X(_x), trans.Y(_y))
        End If
    End Sub

    Protected Overridable Sub _DrawText( _
      ByVal fontSize As Single, _
      ByVal text As String, _
      ByVal x As Single, _
      ByVal y As Single)
        Dim _texts As List(Of String) = Nothing
        If Renderer.Setting.GaijiFont IsNot Nothing Then
            _texts = _DetectGaiji(text)
        End If
        If _texts Is Nothing Then
            _SetTextMatrix(fontSize, x, y)
            ContentByte.ShowText(text)
        Else
            Dim gaiji As Boolean = False
            Dim _x As Single = x
            For Each t As String In _texts
                If t.Length > 0 Then
                    If Not gaiji Then
                        _SetTextMatrix(fontSize, _x, y)
                        ContentByte.ShowText(t)
                        _x += Font.GetWidthPoint(t, fontSize)
                    Else
                        ContentByte.SetFontAndSize(Renderer.Setting.GaijiFont, fontSize)
                        For Each c As Char In t
                            _SetTextMatrix(fontSize, _x, y)
                            ContentByte.ShowText(c)
                            _x += fontSize
                        Next
                        ContentByte.SetFontAndSize(Font, fontSize)
                    End If
                End If
                gaiji = Not gaiji
            Next
        End If
    End Sub

    Protected Overridable Sub _DrawVerticalChar( _
      ByVal fontSize As Single, _
      ByVal c As String, _
      ByVal x As Single, _
      ByVal y As Single)
        Dim gaiji As Boolean = False
        If Renderer.Setting.GaijiFont IsNot Nothing AndAlso _IsGaiji(c) Then
            ContentByte.SetFontAndSize(Renderer.Setting.GaijiFont, fontSize)
            gaiji = True
        End If
        If VERTICAL_ROTATE_CHARS.IndexOf(c) >= 0 Then
            _SetRotateTextMatrix( _
              fontSize, x - fontSize / 3, y - _GetTextWidth(Renderer.Setting, TextDesign, Font, fontSize, c))
        ElseIf VERTICAL_SHIFT_CHARS.IndexOf(c) >= 0 Then
            _SetTextMatrix(fontSize, x, y - fontSize / 2)
        Else
            _SetTextMatrix( _
              fontSize, x - _GetTextWidth(Renderer.Setting, TextDesign, Font, fontSize, c) / 2, y)
        End If
        ContentByte.ShowText(c)
        If gaiji Then
            ContentByte.SetFontAndSize(Font, fontSize)
        End If
    End Sub

    Protected Overridable Sub _DrawUnderline( _
      ByVal fontSize As Single, _
      ByVal x As Single, _
      ByVal y As Single, _
      ByVal width As Single, _
      ByVal lineWidth As Single)
        Dim trans As PdfRenderer.TransClass = Renderer.Trans
        Dim lw As Single = fontSize / 13.4
        Dim _x1 As Single = Region.Left + x
        Dim _x2 As Single = _x1 + width
        Dim _y As Single = Region.Top + y + fontSize - OFFSET_Y
        _x1 = Math.Max(_x1, Region.Left + MARGIN_X)
        _x2 = Math.Min(_x2, Region.Right - MARGIN_X)
        If _x1 < _x2 Then
            ContentByte.SetLineWidth(lineWidth)
            ContentByte.MoveTo(trans.X(_x1), trans.Y(_y))
            ContentByte.LineTo(trans.X(_x2), trans.Y(_y))
            ContentByte.Stroke()
        End If
    End Sub

    Protected Overridable Sub _DrawVerticalUnderLine( _
      ByVal fontSize As Single, _
      ByVal x As Single, _
      ByVal y As Single, _
      ByVal h As Single, _
      ByVal lineWidth As Single)
        Dim trans As PdfRenderer.TransClass = Renderer.Trans
        Dim _x As Single = Region.Left + x
        Dim _y As Single = (Region.Top + y) - OFFSET_Y
        ContentByte.SetLineWidth(lineWidth)
        ContentByte.MoveTo(trans.X(_x), trans.Y(_y))
        ContentByte.LineTo(trans.X(_x), trans.Y(_y + h))
        ContentByte.Stroke()
    End Sub

    Protected Overridable Function _IsEmpty() As Boolean
        If String.IsNullOrEmpty(Me.Text) Then
            Return True
        End If
        If Me.Region.GetWidth <= 0 OrElse Me.Region.GetHeight <= 0 Then
            Return True
        End If
        Return False
    End Function

    Protected Shared Function _GetDistributeMap(ByVal w As Single, ByVal c As Integer, ByVal fontSize As Single) As List(Of Single)
        Dim ret As New List(Of Single)
        If c = 1 Then
            ret.Add(w / 2)
        Else
            Dim t As Single = fontSize / 2
            Do
                ret.Add(t)
                t += (w - fontSize) / (c - 1)
            Loop While t < w And ret.Count < c
        End If
        Return ret
    End Function

    Protected Shared Function _GetFitFontSize( _
      ByVal region As Region, _
      ByVal setting As PdfRendererSetting, _
      ByVal font As BaseFont, _
      ByVal textDesign As TextDesign, _
      ByVal fontSize As Single, _
      ByVal texts As List(Of String)) As Single
        Dim t As String = Nothing
        Dim w As Single = 0
        Dim rw As Single = region.GetWidth - MARGIN_X * 2
        For Each _t As String In texts
            Dim _w As Single = _GetTextWidth(setting, textDesign, font, fontSize, _t)
            If w < _w Then
                w = _w
                t = _t
            End If
        Next
        If w <= rw Then
            Return fontSize
        End If
        Dim _i As Integer = 0
        Do While setting.ShrinkFontSizeMin + _i * 0.5 < fontSize
            _i += 1
        Loop
        For i As Integer = _i - 1 To 1 Step -1
            Dim s As Single = setting.ShrinkFontSizeMin + i * 0.5
            If _GetTextWidth(setting, textDesign, font, s, t) <= rw Then
                Return s
            End If
        Next
        Return setting.ShrinkFontSizeMin
    End Function

    Protected Shared Function _SplitText( _
      ByVal region As Region, _
      ByVal setting As PdfRendererSetting, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal text As String, _
      ByVal wrap As Boolean) As List(Of String)
        Dim fontSize As Single = textDesign.Font.Size
        Dim cw As Single = region.GetWidth - MARGIN_X * 2
        Dim ret As New List(Of String)
        For Each t As String In text.Split(vbCr)
            t = t.Replace(vbLf, "")
            If wrap AndAlso _GetTextWidth(setting, textDesign, font, fontSize, t) > cw + TOLERANCE Then
                Dim _t As String = t
                Do While Not String.IsNullOrEmpty(_t)
                    Dim i As Integer
                    For i = 2 To _t.Length
                        If _GetTextWidth(setting, textDesign, font, fontSize, _t.Substring(0, i)) > cw + TOLERANCE Then
                            Exit For
                        End If
                    Next
                    ret.Add(_t.Substring(0, i - 1))
                    _t = _t.Substring(i - 1)
                Loop
            Else
                ret.Add(t)
            End If
        Next
        Return ret
    End Function

    Protected Shared Function _SplitByCr( _
      ByVal region As Region, _
      ByVal textDesign As TextDesign, _
      ByVal text As String, _
      ByVal wrap As Boolean) As List(Of String)
        Dim fontSize As Single = textDesign.Font.Size
        Dim ch As Single = region.GetHeight
        Dim yc As Integer = Fix((ch + TOLERANCE) / fontSize)
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

    Protected Shared Function _GetTextWidth( _
      ByVal setting As PdfRendererSetting, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal fontSize As Single, _
      ByVal text As String) As Single
        Dim _texts As List(Of String) = Nothing
        If setting.GaijiFont IsNot Nothing Then
            _texts = _DetectGaiji(text)
        End If
        Dim ret As Single = 0
        If _texts Is Nothing Then
            ret = font.GetWidthPoint(text, fontSize)
        Else
            Dim g As Boolean = False
            For Each t As String In _texts
                If t.Length > 0 Then
                    If Not g Then
                        ret += font.GetWidthPoint(t, fontSize)
                    Else
                        ret += t.Length * fontSize
                    End If
                End If
                g = Not g
            Next
        End If
        If textDesign.Font.Italic Then
            ret += fontSize / 6
        End If
        Return ret
    End Function

    Protected Shared Function _DetectGaiji(ByVal text As String) As List(Of String)
        Dim ret As List(Of String) = Nothing
        Dim g As Boolean = False
        Dim last As Integer = 0
        For i As Integer = 0 To text.Length - 1
            If _IsGaiji(text.Chars(i)) Then
                If Not g Then
                    If ret Is Nothing Then
                        ret = New List(Of String)
                    End If
                    ret.Add(text.Substring(last, i - last))
                    last = i
                    g = True
                End If
            Else
                If g Then
                    ret.Add(text.Substring(last, i - last))
                    last = i
                    g = False
                End If
            End If
        Next
        If ret IsNot Nothing Then
            ret.Add(text.Substring(last))
        End If
        Return ret
    End Function

    Protected Shared Function _IsGaiji(ByVal c As Char) As Boolean
        Dim a As Integer = AscW(c)
        Return a >= &HE000 And a <= &HF8FF
    End Function

    Private Class _FixDec

        Public PdfText As PdfText
        Public Text1 As String = ""
        Public Text2 As String = ""
        Public Text3 As String = ""

        Public Sub New(ByVal pdfText As PdfText)
            Me.PdfText = pdfText
            Dim t As String = Me.PdfText.Text
            With Regex.Match(t, "([^0-9]*)$")
                Me.Text3 = .Groups(0).Value
                t = t.Substring(0, t.Length - Me.Text3.Length)
            End With
            With Regex.Match(t, "(\.[0-9]*)?$")
                Me.Text2 = .Groups(0).Value
                Me.Text1 = t.Substring(0, t.Length - Me.Text2.Length)
            End With
        End Sub

        Public Function GetFullText2(ByVal width As Integer) As String
            Dim ret As String = Me.Text2
            If ret.Length = 0 Then
                ret = "."
            End If
            Return ret.PadRight(width + 1, "0")
        End Function

        Public Function GetFullText(ByVal width As Integer) As String
            Return Me.Text1 & Me.GetFullText2(width) & Me.Text3
        End Function

        Public Sub DrawText(ByVal fontSize As Single)
            Dim y As Single = 0
            Select Case PdfText.TextDesign.VAlign
                Case Report.EVAlign.TOP
                    y = 0
                Case Report.EVAlign.CENTER
                    y = (PdfText.Region.GetHeight - fontSize) / 2
                Case Report.EVAlign.BOTTOM
                    y = PdfText.Region.GetHeight - fontSize - MARGIN_BOTTOM
            End Select
            y += OFFSET_Y
            With Nothing
                Dim t As String = Me.Text1 & Me.GetFullText2(PdfText.TextDesign.DecimalPlace)
                Dim ft As String = t & Me.Text3
                Dim w As Single = PdfText._GetTextWidth(PdfText.Renderer.Setting, PdfText.TextDesign, PdfText.Font, fontSize, t)
                Dim fw As Single = _GetTextWidth(PdfText.Renderer.Setting, PdfText.TextDesign, PdfText.Font, fontSize, ft)
                Dim x As Single = 0
                Select Case PdfText.TextDesign.HAlign
                    Case Report.EHAlign.LEFT
                        x = MARGIN_X
                    Case Report.EHAlign.CENTER
                        x = (PdfText.Region.GetWidth - fw) / 2
                        x = Math.Max(x, MARGIN_X)
                    Case Report.EHAlign.RIGHT
                        x = PdfText.Region.GetWidth - fw - MARGIN_X
                        x = Math.Max(x, MARGIN_X)
                End Select
                PdfText.ContentByte.SetFontAndSize(PdfText.Font, fontSize)
                PdfText.ContentByte.BeginText()
                _DrawText_Aux(fontSize, x, y, Me.Text1 & Me.Text2)
                If Me.Text3.Length > 0 Then
                    _DrawText_Aux(fontSize, x + w, y, Me.Text3)
                End If
                PdfText.ContentByte.EndText()
                If PdfText.TextDesign.Font.Underline Then
                    Dim lw As Single = (fontSize / 13.4) * PdfText.Renderer.Setting.UnderlineWidthCoefficient
                    PdfText._DrawUnderline(fontSize, x, y, fw, lw)
                End If
            End With
        End Sub

        Protected Overridable Sub _DrawText_Aux( _
          ByVal fontSize As Single, _
          ByVal x As Single, _
          ByVal y As Single, _
          ByVal text As String)
            Dim _x As Single = Math.Max(x, MARGIN_X)
            Dim w As Single = PdfText.Region.GetWidth - _x - MARGIN_X
            Dim t As String = text
            If w < 0 Then
                Exit Sub
            End If
            If _GetTextWidth(PdfText.Renderer.Setting, PdfText.TextDesign, PdfText.Font, fontSize, t) > w + TOLERANCE Then
                Dim _t As String = ""
                Dim __t As String = ""
                For i As Integer = 1 To t.Length
                    __t = t.Substring(0, i)
                    If _GetTextWidth(PdfText.Renderer.Setting, PdfText.TextDesign, PdfText.Font, fontSize, __t) <= w + TOLERANCE Then
                        _t = __t
                    Else
                        Exit For
                    End If
                Next
                t = _t
            End If
            If t.Length > 0 Then
                PdfText._DrawText(fontSize, t, _x, y)
            End If
        End Sub

    End Class

End Class
