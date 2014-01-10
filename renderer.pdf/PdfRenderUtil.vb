﻿Imports System.Text.RegularExpressions
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports jp.co.systembase.report.component

Public Module PdfRenderUtil

    Private Const TOLERANCE As Single = 0.1F
    Private Const OFFSET_Y As Single = -0.5F
    Private Const MARGIN_X As Single = 2.0F
    Private Const MARGIN_BOTTOM As Single = 2.0F
    Private Const SHRINK_FONT_SIZE_MIN As Single = 4.0F

    Private Const VERTICAL_ROTATE_CHARS As String = "～…‥｜ーｰ(){}[]<>（）｛｝「」＜＞"
    Private Const VERTICAL_SHIFT_CHARS As String = "。、"

    Public Sub DrawText( _
      ByVal renderer As PdfRenderer, _
      ByVal region As Region, _
      ByVal textDesign As TextDesign, _
      ByVal text As String)
        If String.IsNullOrEmpty(text) Then
            Exit Sub
        End If
        If region.GetWidth <= 0 OrElse region.GetHeight <= 0 Then
            Exit Sub
        End If
        Dim cb As PdfContentByte = renderer.Writer.DirectContent
        cb.SaveState()
        Try
            If textDesign.Color IsNot Nothing Then
                Dim c As Color = GetColor(textDesign.Color)
                If c IsNot Nothing Then
                    cb.SetColorFill(c)
                    cb.SetColorStroke(c)
                End If
            End If
            Dim font As BaseFont = renderer.Setting.GetFont(textDesign.Font.Name)
            If textDesign.Font.Bold Then
                cb.SetTextRenderingMode(PdfContentByte.TEXT_RENDER_MODE_FILL_STROKE)
                If Not Report.Compatibility._4_6_PdfFontBold Then
                    cb.SetLineWidth(textDesign.Font.Size * 0.01)
                End If
            Else
                cb.SetTextRenderingMode(PdfContentByte.TEXT_RENDER_MODE_FILL)
            End If
            If textDesign.Distribute Then
                If textDesign.Vertical Then
                    _drawText_distribute_vertical(cb, region, renderer.Trans, textDesign, font, renderer.Setting.GaijiFont, text)
                Else
                    _drawText_distribute(cb, region, renderer.Trans, textDesign, font, renderer.Setting.GaijiFont, text)
                End If
            ElseIf textDesign.Vertical Then
                If textDesign.ShrinkToFit Then
                    _drawText_vertical_shrink(cb, region, renderer.Trans, textDesign, font, renderer.Setting.GaijiFont, text)
                ElseIf textDesign.Wrap Then
                    _drawText_vertical_wrap(cb, region, renderer.Trans, textDesign, font, renderer.Setting.GaijiFont, text)
                Else
                    _drawText_vertical(cb, region, renderer.Trans, textDesign, font, renderer.Setting.GaijiFont, text)
                End If
            ElseIf textDesign.DecimalPlace > 0 Then
                If textDesign.ShrinkToFit Then
                    _drawText_fixdec_shrink(cb, region, renderer.Trans, textDesign, font, renderer.Setting.GaijiFont, text)
                Else
                    _drawText_fixdec(cb, region, renderer.Trans, textDesign, font, renderer.Setting.GaijiFont, text)
                End If
            ElseIf textDesign.ShrinkToFit Then
                _drawText_shrink(cb, region, renderer.Trans, textDesign, font, renderer.Setting.GaijiFont, text)
            ElseIf textDesign.Wrap Then
                _drawText_wrap(cb, region, renderer.Trans, textDesign, font, renderer.Setting.GaijiFont, text)
            Else
                _drawText(cb, region, renderer.Trans, textDesign, font, renderer.Setting.GaijiFont, text)
            End If
        Finally
            cb.RestoreState()
        End Try
    End Sub

    Private Sub _drawText_distribute( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal text As String)
        Dim fontSize As Single = textDesign.Font.Size
        Dim y As Single = 0
        Select Case textDesign.VAlign
            Case Report.EVAlign.TOP
                y = 0
            Case Report.EVAlign.CENTER
                y = (region.GetHeight - fontSize) / 2
            Case Report.EVAlign.BOTTOM
                y = region.GetHeight - fontSize - MARGIN_BOTTOM
        End Select
        y += OFFSET_Y
        With Nothing
            Dim m As List(Of Single) = getDistributeMap(region.GetWidth - MARGIN_X * 2, text.Length, fontSize)
            cb.BeginText()
            cb.SetFontAndSize(font, fontSize)
            For i As Integer = 0 To text.Length - 1
                Dim c As String = text(i)
                showText(cb, region, trans, textDesign, font, gaijiFont, fontSize, _
                         c, m(i) - getTextWidth(font, gaijiFont, textDesign, fontSize, c) / 2 + MARGIN_X, y)
            Next
            cb.EndText()
        End With
        If textDesign.Font.Underline Then
            drawUnderline(cb, region, trans, fontSize, MARGIN_X, y, region.GetWidth - MARGIN_X * 2)
        End If
    End Sub

    Private Sub _drawText_distribute_vertical( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal text As String)
        Dim fontSize As Single = textDesign.Font.Size
        Dim x As Single = 0
        Select Case textDesign.HAlign
            Case Report.EHAlign.LEFT
                x = fontSize / 2 + MARGIN_X
            Case Report.EHAlign.CENTER
                x = region.GetWidth / 2
            Case Report.EHAlign.RIGHT
                x = region.GetWidth - fontSize / 2 - MARGIN_X
        End Select
        If textDesign.Font.Underline Then
            drawVerticalUnderLine(cb, region, trans, fontSize, x + fontSize / 2, 0, region.GetHeight)
        End If
        With Nothing
            Dim m As List(Of Single) = getDistributeMap(region.GetHeight - MARGIN_BOTTOM, text.Length, fontSize)
            cb.SetFontAndSize(font, fontSize)
            cb.BeginText()
            For i As Integer = 0 To text.Length - 1
                showVerticalChar(cb, region, trans, textDesign, font, gaijiFont, fontSize, text(i), _
                                 x, m(i) - fontSize / 2 + OFFSET_Y)
            Next
            cb.EndText()
        End With
    End Sub

    Private Sub _drawText_vertical( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal text As String)
        Dim texts As List(Of String) = splitVerticalText(region, textDesign, text, False)
        _drawText_vertical_aux(cb, region, trans, textDesign, font, gaijiFont, textDesign.Font.Size, texts)
    End Sub

    Private Sub _drawText_vertical_shrink( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal text As String)
        Dim fontSize As Single = textDesign.Font.Size
        Dim texts As List(Of String) = splitVerticalText(region, textDesign, text, False)
        With Nothing
            Dim m As Integer = 0
            For Each t As String In texts
                If m < t.Length Then
                    m = t.Length
                End If
            Next
            If m > 0 Then
                Dim _fontSize As Single = Math.Max((region.GetHeight - MARGIN_BOTTOM) / m, SHRINK_FONT_SIZE_MIN)
                fontSize = Math.Min(fontSize, _fontSize)
            End If
        End With
        _drawText_vertical_aux(cb, region, trans, textDesign, font, gaijiFont, fontSize, texts)
    End Sub

    Private Sub _drawText_vertical_wrap( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal text As String)
        Dim texts As List(Of String) = splitVerticalText(region, textDesign, text, True)
        _drawText_vertical_aux(cb, region, trans, textDesign, font, gaijiFont, textDesign.Font.Size, texts)
    End Sub

    Private Sub _drawText_fixdec( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal text As String)
        Dim fd As New _FixDec(text)
        fd.DrawText(cb, region, trans, textDesign, font, gaijiFont, textDesign.Font.Size)
    End Sub

    Private Sub _drawText_fixdec_shrink( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal text As String)
        Dim fd As New _FixDec(text)
        Dim texts As New List(Of String)
        texts.Add(fd.GetFullText(textDesign.DecimalPlace))
        Dim fontSize As Single = getFitFontSize(region, font, gaijiFont, textDesign, textDesign.Font.Size, texts)
        fd.DrawText(cb, region, trans, textDesign, font, gaijiFont, fontSize)
    End Sub

    Private Sub _drawText_shrink( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal text As String)
        Dim texts As List(Of String) = splitText(region, textDesign, font, gaijiFont, text, False)
        Dim fontSize As Single = getFitFontSize(region, font, gaijiFont, textDesign, textDesign.Font.Size, texts)
        _drawText_aux(cb, region, trans, textDesign, font, gaijiFont, fontSize, texts)
    End Sub

    Private Sub _drawText_wrap( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal text As String)
        Dim texts As List(Of String) = splitText(region, textDesign, font, gaijiFont, text, True)
        _drawText_aux(cb, region, trans, textDesign, font, gaijiFont, textDesign.Font.Size, texts)
    End Sub

    Private Sub _drawText( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal text As String)
        Dim texts As List(Of String) = splitText(region, textDesign, font, gaijiFont, text, False)
        _drawText_aux(cb, region, trans, textDesign, font, gaijiFont, textDesign.Font.Size, texts)
    End Sub

    Private Sub _drawText_aux( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal fontSize As Single, _
      ByVal texts As List(Of String))
        Dim y As Single = 0
        Select Case textDesign.VAlign
            Case Report.EVAlign.TOP
                y = 0
            Case Report.EVAlign.CENTER
                y = (region.GetHeight - fontSize * texts.Count) / 2
                y = Math.Max(y, 0)
            Case Report.EVAlign.BOTTOM
                y = region.GetHeight - fontSize * texts.Count - MARGIN_BOTTOM
                y = Math.Max(y, 0)
        End Select
        y += OFFSET_Y
        Dim rows As Integer = Fix((region.GetHeight + TOLERANCE) / fontSize)
        For i As Integer = 0 To Math.Max(Math.Min(texts.Count, rows) - 1, 0)
            Dim t As String = texts(i)
            Dim w As Single = getTextWidth(font, gaijiFont, textDesign, fontSize, t)
            With Nothing
                Dim rw As Single = region.GetWidth - MARGIN_X * 2
                If w > rw Then
                    Dim _t As String = ""
                    Dim _w As Single = 0
                    For j As Integer = 1 To t.Length
                        Dim __t As String = t.Substring(0, j)
                        Dim __w As Single = getTextWidth(font, gaijiFont, textDesign, fontSize, __t)
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
            Select Case textDesign.HAlign
                Case Report.EHAlign.LEFT
                    x = MARGIN_X
                Case Report.EHAlign.CENTER
                    x = (region.GetWidth - w) / 2
                    x = Math.Max(x, MARGIN_X)
                Case Report.EHAlign.RIGHT
                    x = region.GetWidth - w - MARGIN_X
                    x = Math.Max(x, MARGIN_X)
            End Select
            cb.SetFontAndSize(font, fontSize)
            cb.BeginText()
            showText(cb, region, trans, textDesign, font, gaijiFont, fontSize, t, x, y)
            cb.EndText()
            If textDesign.Font.Underline Then
                drawUnderline(cb, region, trans, fontSize, x, y, w)
            End If
            y += fontSize
        Next
    End Sub

    Private Sub _drawText_vertical_aux( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal fontSize As Single, _
      ByVal texts As List(Of String))
        Dim x As Single = 0
        Select Case textDesign.HAlign
            Case Report.EHAlign.LEFT
                x = fontSize * (texts.Count - 1) + fontSize / 2 + MARGIN_X
                x = Math.Min(x, region.GetWidth - fontSize / 2 - MARGIN_X)
            Case Report.EHAlign.CENTER
                x = (region.GetWidth + (texts.Count - 1) * fontSize) / 2
                x = Math.Min(x, region.GetWidth - fontSize / 2 - MARGIN_X)
            Case Report.EHAlign.RIGHT
                x = region.GetWidth - fontSize / 2 - MARGIN_X
        End Select
        Dim cols As Integer = Fix(((region.GetWidth - MARGIN_X * 2) + TOLERANCE) / fontSize)
        Dim rows As Integer = Fix((region.GetHeight + TOLERANCE) / fontSize)
        For i As Integer = 0 To Math.Max(Math.Min(texts.Count, cols) - 1, 0)
            Dim t As String = texts(i)
            Dim y As Single = 0
            Select Case textDesign.VAlign
                Case Report.EVAlign.TOP
                    y = 0
                Case Report.EVAlign.CENTER
                    y = (region.GetHeight - fontSize * t.Length) / 2
                    y = Math.Max(y, 0)
                Case Report.EVAlign.BOTTOM
                    y = region.GetHeight - fontSize * t.Length - MARGIN_BOTTOM
                    y = Math.Max(y, 0)
            End Select
            y += OFFSET_Y
            Dim _yc As Integer = Math.Min(t.Length, rows)
            If textDesign.Font.Underline Then
                drawVerticalUnderLine(cb, region, trans, fontSize, x + fontSize / 2, y, _yc * fontSize)
            End If
            cb.SetFontAndSize(font, fontSize)
            cb.BeginText()
            For j As Integer = 0 To _yc - 1
                showVerticalChar(cb, region, trans, textDesign, font, gaijiFont, fontSize, t(j), x, y)
                y += fontSize
            Next
            cb.EndText()
            x -= fontSize
        Next
    End Sub

    Private Class _FixDec

        Public Text1 As String = ""
        Public Text2 As String = ""
        Public Text3 As String = ""

        Public Sub New(ByVal text As String)
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

        Public Sub DrawText( _
          ByVal cb As PdfContentByte, _
          ByVal region As Region, _
          ByVal trans As PdfRenderer.TransClass, _
          ByVal textDesign As TextDesign, _
          ByVal font As BaseFont, _
          ByVal gaijiFont As BaseFont, _
          ByVal fontSize As Single)
            Dim y As Single = 0
            Select Case textDesign.VAlign
                Case Report.EVAlign.TOP
                    y = 0
                Case Report.EVAlign.CENTER
                    y = (region.GetHeight - fontSize) / 2
                Case Report.EVAlign.BOTTOM
                    y = region.GetHeight - fontSize - MARGIN_BOTTOM
            End Select
            y += OFFSET_Y
            With Nothing
                Dim t As String = Me.Text1 & Me.GetFullText2(textDesign.DecimalPlace)
                Dim ft As String = t & Me.Text3
                Dim w As Single = getTextWidth(font, gaijiFont, textDesign, fontSize, t)
                Dim fw As Single = getTextWidth(font, gaijiFont, textDesign, fontSize, ft)
                Dim x As Single = 0
                Select Case textDesign.HAlign
                    Case Report.EHAlign.LEFT
                        x = MARGIN_X
                    Case Report.EHAlign.CENTER
                        x = (region.GetWidth - fw) / 2
                        x = Math.Max(x, MARGIN_X)
                    Case Report.EHAlign.RIGHT
                        x = region.GetWidth - fw - MARGIN_X
                        x = Math.Max(x, MARGIN_X)
                End Select
                cb.SetFontAndSize(font, fontSize)
                cb.BeginText()
                drawText_aux(cb, region, trans, textDesign, font, gaijiFont, fontSize, x, y, Me.Text1 & Me.Text2)
                If Me.Text3.Length > 0 Then
                    drawText_aux(cb, region, trans, textDesign, font, gaijiFont, fontSize, x + w, y, Me.Text3)
                End If
                cb.EndText()
                If textDesign.Font.Underline Then
                    drawUnderline(cb, region, trans, fontSize, x, y, fw)
                End If
            End With
        End Sub

        Private Sub drawText_aux( _
          ByVal cb As PdfContentByte, _
          ByVal region As Region, _
          ByVal trans As PdfRenderer.TransClass, _
          ByVal textDesign As TextDesign, _
          ByVal font As BaseFont, _
          ByVal gaijiFont As BaseFont, _
          ByVal fontSize As Single, _
          ByVal x As Single, _
          ByVal y As Single, _
          ByVal text As String)
            Dim _x As Single = Math.Max(x, MARGIN_X)
            Dim w As Single = region.GetWidth - _x - MARGIN_X
            Dim t As String = text
            If w < 0 Then
                Exit Sub
            End If
            If getTextWidth(font, gaijiFont, textDesign, fontSize, t) > w + TOLERANCE Then
                Dim _t As String = ""
                Dim __t As String = ""
                For i As Integer = 1 To t.Length
                    __t = t.Substring(0, i)
                    If getTextWidth(font, gaijiFont, textDesign, fontSize, __t) <= w + TOLERANCE Then
                        _t = __t
                    Else
                        Exit For
                    End If
                Next
                t = _t
            End If
            If t.Length > 0 Then
                showText(cb, region, trans, textDesign, font, gaijiFont, fontSize, t, _x, y)
            End If
        End Sub

    End Class

    Private Function getDistributeMap(ByVal w As Single, ByVal c As Integer, ByVal fontSize As Single) As List(Of Single)
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

    Private Function getFitFontSize( _
      ByVal region As Region, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal textDesign As TextDesign, _
      ByVal fontSize As Single, _
      ByVal texts As List(Of String)) As Single
        Dim t As String = Nothing
        Dim w As Single = 0
        Dim rw As Single = region.GetWidth - MARGIN_X * 2
        For Each _t As String In texts
            Dim _w As Single = getTextWidth(font, gaijiFont, textDesign, fontSize, _t)
            If w < _w Then
                w = _w
                t = _t
            End If
        Next
        If w <= rw Then
            Return fontSize
        End If
        Dim _i As Integer = 0
        Do While SHRINK_FONT_SIZE_MIN + _i * 0.5 < fontSize
            _i += 1
        Loop
        For i As Integer = _i - 1 To 1 Step -1
            Dim s As Single = SHRINK_FONT_SIZE_MIN + i * 0.5
            If getTextWidth(font, gaijiFont, textDesign, s, t) <= rw Then
                Return s
            End If
        Next
        Return SHRINK_FONT_SIZE_MIN
    End Function

    Private Function splitText( _
      ByVal region As Region, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal text As String, _
      ByVal wrap As Boolean) As List(Of String)
        Dim fontSize As Single = textDesign.Font.Size
        Dim cw As Single = region.GetWidth - MARGIN_X * 2
        Dim ret As New List(Of String)
        For Each t As String In text.Split(vbCr)
            t = t.Replace(vbLf, "")
            If wrap AndAlso getTextWidth(font, gaijiFont, textDesign, fontSize, t) > cw + TOLERANCE Then
                Dim _t As String = t
                Do While Not String.IsNullOrEmpty(_t)
                    Dim i As Integer
                    For i = 2 To _t.Length
                        If getTextWidth(font, gaijiFont, textDesign, fontSize, _t.Substring(0, i)) > cw + TOLERANCE Then
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

    Private Function splitVerticalText( _
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

    Private Function getTextWidth( _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal textDesign As TextDesign, _
      ByVal fontSize As Single, _
      ByVal text As String) As Single
        Dim _texts As List(Of String) = Nothing
        If gaijiFont IsNot Nothing Then
            _texts = detectGaiji(text)
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

    Private Sub setTextMatrix( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal fontSize As Single, _
      ByVal x As Single, _
      ByVal y As Single)
        Dim _x As Single = region.Left + x
        Dim _y As Single = (region.Top + y + fontSize) - (fontSize / 13.4)
        If textDesign.Font.Italic Then
            cb.SetTextMatrix(1, 0, 0.3, 1, trans.X(_x), trans.Y(_y))
        Else
            cb.SetTextMatrix(trans.X(_x), trans.Y(_y))
        End If
    End Sub

    Private Sub setRotateTextMatrix( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal fontSize As Single, _
      ByVal x As Single, _
      ByVal y As Single)
        Dim _x As Single = region.Left + x
        Dim _y As Single = (region.Top + y + fontSize) - (fontSize / 13.4)
        If textDesign.Font.Italic Then
            cb.SetTextMatrix(-0.3, -1, 1, 0, trans.X(_x), trans.Y(_y))
        Else
            cb.SetTextMatrix(0, -1, 1, 0, trans.X(_x), trans.Y(_y))
        End If
    End Sub

    Private Sub showText( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal fontSize As Single, _
      ByVal text As String, _
      ByVal x As Single, _
      ByVal y As Single)
        Dim _texts As List(Of String) = Nothing
        If gaijiFont IsNot Nothing Then
            _texts = detectGaiji(text)
        End If
        If _texts Is Nothing Then
            setTextMatrix(cb, region, trans, textDesign, fontSize, x, y)
            cb.ShowText(text)
        Else
            Dim gaiji As Boolean = False
            Dim _x As Single = x
            For Each t As String In _texts
                If t.Length > 0 Then
                    If Not gaiji Then
                        setTextMatrix(cb, region, trans, textDesign, fontSize, _x, y)
                        cb.ShowText(t)
                        _x += font.GetWidthPoint(t, fontSize)
                    Else
                        cb.SetFontAndSize(gaijiFont, fontSize)
                        For Each c As Char In t
                            setTextMatrix(cb, region, trans, textDesign, fontSize, _x, y)
                            cb.ShowText(c)
                            _x += fontSize
                        Next
                        cb.SetFontAndSize(font, fontSize)
                    End If
                End If
                gaiji = Not gaiji
            Next
        End If
    End Sub

    Private Sub showVerticalChar( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal textDesign As TextDesign, _
      ByVal font As BaseFont, _
      ByVal gaijiFont As BaseFont, _
      ByVal fontSize As Single, _
      ByVal c As String, _
      ByVal x As Single, _
      ByVal y As Single)
        Dim gaiji As Boolean = False
        If gaijiFont IsNot Nothing AndAlso isGaiji(c) Then
            cb.SetFontAndSize(gaijiFont, fontSize)
            gaiji = True
        End If
        If VERTICAL_ROTATE_CHARS.IndexOf(c) >= 0 Then
            setRotateTextMatrix(cb, region, trans, textDesign, _
                                fontSize, x - fontSize / 3, _
                                y - getTextWidth(font, gaijiFont, textDesign, fontSize, c))
        ElseIf VERTICAL_SHIFT_CHARS.IndexOf(c) >= 0 Then
            setTextMatrix(cb, region, trans, textDesign, fontSize, x, y - fontSize / 2)
        Else
            setTextMatrix(cb, region, trans, textDesign, _
                          fontSize, x - getTextWidth(font, gaijiFont, textDesign, fontSize, c) / 2, _
                          y)
        End If
        cb.ShowText(c)
        If gaiji Then
            cb.SetFontAndSize(font, fontSize)
        End If
    End Sub

    Private Sub drawUnderline( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal fontSize As Single, _
      ByVal x As Single, _
      ByVal y As Single, _
      ByVal width As Single)
        Dim lw As Single = fontSize / 13.4
        Dim _x1 As Single = region.Left + x
        Dim _x2 As Single = _x1 + width
        Dim _y As Single = region.Top + y + fontSize - OFFSET_Y
        _x1 = Math.Max(_x1, region.Left + MARGIN_X)
        _x2 = Math.Min(_x2, region.Right - MARGIN_X)
        If _x1 < _x2 Then
            cb.SetLineWidth(lw)
            cb.MoveTo(trans.X(_x1), trans.Y(_y))
            cb.LineTo(trans.X(_x2), trans.Y(_y))
            cb.Stroke()
        End If
    End Sub

    Private Sub drawVerticalUnderLine( _
      ByVal cb As PdfContentByte, _
      ByVal region As Region, _
      ByVal trans As PdfRenderer.TransClass, _
      ByVal fontSize As Single, _
      ByVal x As Single, _
      ByVal y As Single, _
      ByVal h As Single)
        Dim lw As Single = fontSize / 13.4
        Dim _x As Single = region.Left + x
        Dim _y As Single = (region.Top + y) - OFFSET_Y
        cb.SetLineWidth(lw)
        cb.MoveTo(trans.X(_x), trans.Y(_y))
        cb.LineTo(trans.X(_x), trans.Y(_y + h))
        cb.Stroke()
    End Sub

    Private Function detectGaiji(ByVal text As String) As List(Of String)
        Dim ret As List(Of String) = Nothing
        Dim g As Boolean = False
        Dim last As Integer = 0
        For i As Integer = 0 To text.Length - 1
            If isGaiji(text.Chars(i)) Then
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

    Private Function isGaiji(ByVal c As Char) As Boolean
        Dim a As Integer = AscW(c)
        Return a >= &HE000 And a <= &HF8FF
    End Function

    Public Function GetColor(ByVal v As String) As Color
        If Not String.IsNullOrEmpty(v) Then
            If v.StartsWith("#") Then
                Dim _v As String = v.Substring(1).ToLower
                For i As Integer = 0 To 5
                    If "0123456789abcdef".IndexOf(_v(i)) < 0 Then
                        Return Nothing
                    End If
                Next
                Return New Color(Convert.ToInt32(_v.Substring(0, 2), 16), _
                                 Convert.ToInt32(_v.Substring(2, 2), 16), _
                                 Convert.ToInt32(_v.Substring(4, 2), 16))
            Else
                If Array.IndexOf(RenderUtil.COLOR_NAMES, v.ToLower) >= 0 Then
                    Dim c As System.Drawing.Color = System.Drawing.Color.FromName(v)
                    Return New Color(c.R, c.G, c.B)
                End If
            End If
        End If
        Return Nothing
    End Function

End Module