Imports System.Globalization
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
    Public GaijiFont As BaseFont
    Public IsMonospaced As Boolean
    Public TextMatrix As List(Of Single) = Nothing

    Protected Const TOLERANCE As Single = 0.1F
    Protected Const OFFSET_Y As Single = -0.5F
    Protected Const MARGIN_X As Single = 2.0F
    Protected Const MARGIN_BOTTOM As Single = 2.0F

    Private Const VERTICAL_ROTATE_CHARS As String = "…‥｜ーｰ(){}[]<>（）｛｝「」＜＞←→↓⇒⇔↑＝≒"
    Private Const VERTICAL_ROTATE2_CHARS As String = "～"
    Private Const VERTICAL_SHIFT_CHARS As String = "。、，"

    Private Shared _VerticalRotateCharsMap As Dictionary(Of String, Boolean)
    Private Shared _VerticalRotate2CharsMap As Dictionary(Of String, Boolean)
    Private Shared _VerticalShiftCharsMap As Dictionary(Of String, Boolean)

    Shared Sub New()
        _VerticalRotateCharsMap = New Dictionary(Of String, Boolean)
        _VerticalRotate2CharsMap = New Dictionary(Of String, Boolean)
        _VerticalShiftCharsMap = New Dictionary(Of String, Boolean)
        For i As Integer = 0 To VERTICAL_ROTATE_CHARS.Length - 1
            _VerticalRotateCharsMap.Add(VERTICAL_ROTATE_CHARS.Chars(i), True)
        Next
        For i As Integer = 0 To VERTICAL_ROTATE2_CHARS.Length - 1
            _VerticalRotate2CharsMap.Add(VERTICAL_ROTATE2_CHARS.Chars(i), True)
        Next
        For i As Integer = 0 To VERTICAL_SHIFT_CHARS.Length - 1
            _VerticalShiftCharsMap.Add(VERTICAL_SHIFT_CHARS.Chars(i), True)
        Next
    End Sub

    Public Overridable Sub Initialize(
      renderer As PdfRenderer,
      reportDesign As ReportDesign,
      region As Region,
      design As ElementDesign,
      text As String)
        Me.Renderer = renderer
        Me.Region = region.ToPointScale(reportDesign)
        Me.TextDesign = New TextDesign(reportDesign, design)
        Me.Text = text
        Me.ContentByte = Me.Renderer.Writer.DirectContent
        Me.Font = Me.Renderer.Setting.GetFont(TextDesign.Font.Name)
        Me.GaijiFont = Me.Renderer.Setting.GetGaijiFont(TextDesign.Font.Name)
        If Not Report.Compatibility._4_37_Typeset Then
            Me.IsMonospaced = Me.TextDesign.MonospacedFont IsNot Nothing And _IsMonospacedFont()
        Else
            Me.IsMonospaced = False
        End If
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
        Dim texts As List(Of String) = (New TextSplitter).GetLines(Me.Text)
        Dim y As Single = 0
        Select Case TextDesign.VAlign
            Case Report.EVAlign.TOP
                If Not Report.Compatibility._4_37_Typeset Then
                    y = fontSize * 0.125
                Else
                    y = 0
                End If
            Case Report.EVAlign.CENTER
                If Not Report.Compatibility._4_37_Typeset Then
                    y = (Region.GetHeight + fontSize * 0.125 - fontSize * texts.Count) / 2
                Else
                    y = (Region.GetHeight - fontSize * texts.Count) / 2
                End If
                y = Math.Max(y, 0)
            Case Report.EVAlign.BOTTOM
                If Not Report.Compatibility._4_37_Typeset Then
                    y = Region.GetHeight - fontSize * texts.Count
                Else
                    y = Region.GetHeight - (fontSize * texts.Count) - MARGIN_BOTTOM
                End If
                y = Math.Max(y, 0)
        End Select
        If Report.Compatibility._4_37_Typeset Then
            y += OFFSET_Y
        End If
        Dim rows As Integer = Fix((Region.GetHeight + TOLERANCE) / fontSize)
        For i As Integer = 0 To Math.Max(Math.Min(texts.Count, rows) - 1, 0)
            Dim si As StringInfo = New StringInfo(texts(i))
            Dim mx As Single
            If Not Report.Compatibility._4_37_Typeset Then
                mx = fontSize / 3
            Else
                mx = MARGIN_X * 2
            End If
            Dim m As List(Of Single) = _GetDistributeMap(Region.GetWidth - mx, si.LengthInTextElements, fontSize)
            _Draw_Preprocess()
            ContentByte.BeginText()
            ContentByte.SetFontAndSize(Font, fontSize)
            For j As Integer = 0 To si.LengthInTextElements - 1
                Dim c As String = si.SubstringByTextElements(j, 1)
                _DrawChar(fontSize, c, m(j) - _GetTextWidth(fontSize, c) / 2 + mx / 2, y)
            Next
            ContentByte.EndText()
            If TextDesign.Font.Underline Then
                Dim lw As Single = (fontSize / 13.4) * Renderer.Setting.UnderlineWidthCoefficient
                _DrawUnderline(fontSize, mx / 2, y, Region.GetWidth - mx, lw)
            End If
            y += fontSize
        Next
    End Sub

    Protected Overridable Sub _Draw_DistributeVertical()
        Dim fontSize As Single = TextDesign.Font.Size
        Dim texts As List(Of String) = (New TextSplitter).GetLines(Me.Text)
        Dim x As Single = 0
        Dim mx As Single
        If Not Report.Compatibility._4_37_Typeset Then
            mx = fontSize / 6
        Else
            mx = MARGIN_X
        End If
        Select Case TextDesign.HAlign
            Case Report.EHAlign.LEFT
                x = fontSize * (texts.Count - 1) + fontSize / 2 + mx
                x = Math.Min(x, Region.GetWidth - fontSize / 2 - mx)
            Case Report.EHAlign.CENTER
                x = (Region.GetWidth + (texts.Count - 1) * fontSize) / 2
                x = Math.Min(x, Region.GetWidth - fontSize / 2 - mx)
            Case Report.EHAlign.RIGHT
                x = Region.GetWidth - fontSize / 2 - mx
        End Select
        Dim cols As Integer = Fix(((Region.GetWidth - mx * 2) + TOLERANCE) / fontSize)
        For i As Integer = 0 To Math.Max(Math.Min(texts.Count, cols) - 1, 0)
            Dim si As StringInfo = New StringInfo(texts(i))
            If TextDesign.Font.Underline Then
                Dim lw As Single = (fontSize / 13.4) * Renderer.Setting.UnderlineWidthCoefficient
                _DrawVerticalUnderLine(fontSize, x + fontSize / 2, 0, Region.GetHeight, lw)
            End If
            With Nothing
                Dim my As Single
                If Not Report.Compatibility._4_37_Typeset Then
                    my = 0
                Else
                    my = MARGIN_BOTTOM
                End If
                Dim m As List(Of Single) = _GetDistributeMap(Region.GetHeight - my, si.LengthInTextElements, fontSize)
                _Draw_Preprocess()
                ContentByte.SetFontAndSize(Font, fontSize)
                ContentByte.BeginText()
                For j As Integer = 0 To si.LengthInTextElements - 1
                    _DrawVerticalChar(fontSize, si.SubstringByTextElements(j, 1), x, m(j) - fontSize / 2 + OFFSET_Y)
                Next
                ContentByte.EndText()
            End With
            x -= fontSize
        Next
    End Sub

    Protected Overridable Sub _Draw_Vertical()
        _Draw_Vertical_Aux(TextDesign.Font.Size, (New TextSplitter).GetLines(Me.Text))
    End Sub

    Protected Overridable Sub _Draw_VerticalShrink()
        Dim fontSize As Single = TextDesign.Font.Size
        Dim texts As List(Of String) = (New TextSplitter).GetLines(Me.Text)
        With Nothing
            Dim m As Integer = 0
            For Each t As String In texts
                Dim si As StringInfo = New StringInfo(t)
                If m < si.LengthInTextElements Then
                    m = si.LengthInTextElements
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
        Dim l As Integer = Fix((Me.Region.GetHeight + TOLERANCE) / Me.TextDesign.Font.Size)
        _Draw_Vertical_Aux(TextDesign.Font.Size, (New TextSplitterByLen(l)).GetLines(Me.Text))
    End Sub

    Protected Overridable Sub _Draw_Fixdec()
        Dim fd As New _FixDec(Me)
        If IsMonospaced Then
            Dim texts As List(Of String) =
                (New TextSplitterByDrawingWidth(TextDesign, 0, Region.GetWidth)).GetLines(fd.GetFullText(True))
            _Draw_Monospaced(TextDesign.Font.Size, texts)
        Else
            fd.DrawText(TextDesign.Font.Size)
        End If
    End Sub

    Protected Overridable Sub _Draw_FixdecShrink()
        Dim fd As New _FixDec(Me)
        If IsMonospaced Then
            Dim texts As List(Of String) = (New TextSplitter(True)).GetLines(fd.GetFullText(True))
            Dim fontSize As Single = _GetFitFontSize(texts)
            _Draw_Monospaced(fontSize, texts)
        Else
            Dim texts As List(Of String) = (New TextSplitter(True)).GetLines(fd.GetFullText(False))
            Dim fontSize As Single = _GetFitFontSize(texts)
            fd.DrawText(fontSize)
        End If
    End Sub

    Protected Overridable Sub _Draw_Shrink()
        If IsMonospaced Then
            Dim texts As List(Of String) = (New TextSplitter).GetLines(Me.Text)
            Dim fontSize As Single = TextDesign.GetMonospacedFitFontSize(texts, Region.GetWidth, Renderer.Setting.ShrinkFontSizeMin)
            _Draw_Monospaced(fontSize, texts)
        Else
            Dim texts As List(Of String) = (New TextSplitter).GetLines(Me.Text)
            Dim fontSize As Single = _GetFitFontSize(texts)
            _Draw_Aux(fontSize, texts)
        End If
    End Sub

    Protected Overridable Sub _Draw_Wrap()
        If IsMonospaced Then
            _Draw_Monospaced(TextDesign.Font.Size, (New TextSplitterByDrawingWidth(TextDesign, Region.GetWidth, 0)).GetLines(Me.Text))
        Else
            _Draw_Aux(TextDesign.Font.Size, (New _TextSplitterByPdfWidth(Me)).GetLines(Me.Text))
        End If
    End Sub

    Protected Overridable Sub _Draw()
        If IsMonospaced Then
            Dim texts As List(Of String) =
                (New TextSplitterByDrawingWidth(TextDesign, 0, Region.GetWidth)).GetLines(Me.Text)
            _Draw_Monospaced(TextDesign.Font.Size, texts)
        Else
            _Draw_Aux(TextDesign.Font.Size, (New TextSplitter).GetLines(Me.Text))
        End If
    End Sub

    Protected Overridable Sub _Draw_Aux(
      fontSize As Single,
      texts As List(Of String))
        Dim y As Single = 0
        Select Case TextDesign.VAlign
            Case Report.EVAlign.TOP
                y = 0
            Case Report.EVAlign.CENTER
                If Not Report.Compatibility._4_37_Typeset Then
                    y = (Region.GetHeight - fontSize * texts.Count - fontSize * 0.125) / 2
                Else
                    y = (Region.GetHeight - fontSize * texts.Count) / 2
                End If
                y = Math.Max(y, 0)
            Case Report.EVAlign.BOTTOM
                If Not Report.Compatibility._4_37_Typeset Then
                    y = Region.GetHeight - fontSize * texts.Count - fontSize * 0.125
                Else
                    y = Region.GetHeight - fontSize * texts.Count - MARGIN_BOTTOM
                End If
                y = Math.Max(y, 0)
        End Select
        If Report.Compatibility._4_37_Typeset Then
            y += OFFSET_Y
        End If
        Dim rows As Integer = Fix((Region.GetHeight + TOLERANCE) / fontSize)
        For i As Integer = 0 To Math.Max(Math.Min(texts.Count, rows) - 1, 0)
            Dim t As String = texts(i)
            Dim w As Single = _GetTextWidth(fontSize, t)

            With Nothing
                Dim si As StringInfo = New StringInfo(t)
                Dim rw As Single
                If Not Report.Compatibility._4_37_Typeset Then
                    rw = Region.GetWidth - fontSize / 3
                Else
                    rw = Region.GetWidth - MARGIN_X * 2
                End If
                If w > rw Then
                    Dim _t As String = ""
                    Dim _w As Single = 0
                    For j As Integer = 1 To si.LengthInTextElements
                        Dim __t As String = si.SubstringByTextElements(0, j)
                        Dim __w As Single = _GetTextWidth(fontSize, __t)
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
                    If Not Report.Compatibility._4_37_Typeset Then
                        x = fontSize / 6
                    Else
                        x = MARGIN_X
                    End If
                Case Report.EHAlign.CENTER
                    If Not Report.Compatibility._4_37_Typeset Then
                        x = (Region.GetWidth - w) / 2
                        x = Math.Max(x, fontSize / 6)
                    Else
                        x = (Region.GetWidth - w) / 2
                        x = Math.Max(x, MARGIN_X)
                    End If
                Case Report.EHAlign.RIGHT
                    If Not Report.Compatibility._4_37_Typeset Then
                        x = Region.GetWidth - w - fontSize / 6
                        x = Math.Max(x, fontSize / 6)
                    Else
                        x = Region.GetWidth - w - MARGIN_X
                        x = Math.Max(x, MARGIN_X)
                    End If
            End Select
            _Draw_Preprocess()
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

    Protected Overridable Sub _Draw_Monospaced(
      fontSize As Single,
      texts As List(Of String))
        Dim y As Single = 0
        Dim h As Single = TextDesign.MonospacedFont.RowHeidht * fontSize
        Select Case TextDesign.VAlign
            Case Report.EVAlign.TOP
                y = 0
            Case Report.EVAlign.CENTER
                y = (Region.GetHeight - h * texts.Count - fontSize * 0.125) / 2
                y = Math.Max(y, 0)
            Case Report.EVAlign.BOTTOM
                y = Region.GetHeight - h * texts.Count - fontSize * 0.125
                y = Math.Max(y, 0)
        End Select
        Dim rows As Integer = Fix((Region.GetHeight + TOLERANCE) / h)
        For i As Integer = 0 To Math.Max(Math.Min(texts.Count, rows) - 1, 0)
            Dim t As String = texts(i)
            Dim si As New StringInfo(t)
            Dim w As Single = TextDesign.GetMonospacedWidth(si, fontSize)
            Dim cs As Single = TextDesign.GetPdfCharSpacing(si, fontSize)
            Dim x As Single = 0
            Select Case TextDesign.HAlign
                Case Report.EHAlign.LEFT
                    x = fontSize / 6
                Case Report.EHAlign.CENTER
                    x = (Region.GetWidth - w) / 2 + fontSize / 6
                    x = Math.Max(x, fontSize / 6)
                Case Report.EHAlign.RIGHT
                    x = Region.GetWidth - w + fontSize / 6
                    x = Math.Max(x, fontSize / 6)
            End Select
            _Draw_Preprocess()
            ContentByte.SetFontAndSize(Font, fontSize)
            ContentByte.BeginText()
            _DrawText(fontSize, t, x, y, cs)
            ContentByte.EndText()
            If TextDesign.Font.Underline Then
                Dim lw As Single = (fontSize / 13.4) * Renderer.Setting.UnderlineWidthCoefficient
                _DrawUnderline(fontSize, x, y, w, lw)
            End If
            y += h
        Next
    End Sub

    Protected Overridable Sub _Draw_Vertical_Aux(
      fontSize As Single,
      texts As List(Of String))
        Dim x As Single = 0
        Dim mx As Single
        If Not Report.Compatibility._4_37_Typeset Then
            mx = fontSize / 6
        Else
            mx = MARGIN_X
        End If
        Select Case TextDesign.HAlign
            Case Report.EHAlign.LEFT
                x = fontSize * (texts.Count - 1) + fontSize / 2 + mx
                x = Math.Min(x, Region.GetWidth - fontSize / 2 - mx)
            Case Report.EHAlign.CENTER
                x = (Region.GetWidth + (texts.Count - 1) * fontSize) / 2
                x = Math.Min(x, Region.GetWidth - fontSize / 2 - mx)
            Case Report.EHAlign.RIGHT
                x = Region.GetWidth - fontSize / 2 - mx
        End Select
        Dim cols As Integer = Fix(((Region.GetWidth - mx * 2) + TOLERANCE) / fontSize)
        Dim rows As Integer = Fix((Region.GetHeight + TOLERANCE) / fontSize)
        For i As Integer = 0 To Math.Max(Math.Min(texts.Count, cols) - 1, 0)
            Dim si As StringInfo = New StringInfo(texts(i))
            Dim y As Single = 0
            Select Case TextDesign.VAlign
                Case Report.EVAlign.TOP
                    y = 0
                Case Report.EVAlign.CENTER
                    y = (Region.GetHeight - fontSize * si.LengthInTextElements) / 2
                    y = Math.Max(y, 0)
                Case Report.EVAlign.BOTTOM
                    If Not Report.Compatibility._4_37_Typeset Then
                        y = Region.GetHeight - fontSize * si.LengthInTextElements
                    Else
                        y = Region.GetHeight - fontSize * si.LengthInTextElements - MARGIN_BOTTOM
                    End If
                    y = Math.Max(y, 0)
            End Select
            y += OFFSET_Y
            Dim _yc As Integer = Math.Min(si.LengthInTextElements, rows)
            If TextDesign.Font.Underline Then
                Dim lw As Single = (fontSize / 13.4) * Renderer.Setting.UnderlineWidthCoefficient
                _DrawVerticalUnderLine(fontSize, x + fontSize / 2, y, _yc * fontSize, lw)
            End If
            _Draw_Preprocess()
            ContentByte.SetFontAndSize(Font, fontSize)
            ContentByte.BeginText()
            For j As Integer = 0 To _yc - 1
                _DrawVerticalChar(fontSize, si.SubstringByTextElements(j, 1), x, y)
                y += fontSize
            Next
            ContentByte.EndText()
            x -= fontSize
        Next
    End Sub

    Protected Overridable Sub _SetTextMatrix(
      fontSize As Single,
      x As Single,
      y As Single)
        Dim trans As PdfRenderer.TransClass = Renderer.Trans
        Dim _x As Single = Region.Left + x
        Dim _y As Single = Region.Top + y + fontSize
        If Not Report.Compatibility._4_37_Typeset Then
            _y -= fontSize * 0.133
        Else
            _y -= fontSize / 13.4
        End If
        If TextMatrix IsNot Nothing Then
            ContentByte.SetTextMatrix(TextMatrix(0), TextMatrix(1), TextMatrix(2), TextMatrix(3),
                                      trans.X(_x), trans.Y(_y))
        Else
            If TextDesign.Font.Italic Then
                If TextDesign.Vertical Then
                    ContentByte.SetTextMatrix(1, -0.3, 0, 1, trans.X(_x), trans.Y(_y))
                Else
                    ContentByte.SetTextMatrix(1, 0, 0.3, 1, trans.X(_x), trans.Y(_y))
                End If
            Else
                ContentByte.SetTextMatrix(trans.X(_x), trans.Y(_y))
            End If
        End If
    End Sub

    Protected Overridable Sub _SetRotateTextMatrix(
      fontSize As Single,
      x As Single,
      y As Single)
        Dim trans As PdfRenderer.TransClass = Renderer.Trans
        Dim _x As Single = Region.Left + x
        Dim _y As Single = Region.Top + y + fontSize
        If TextDesign.Font.Italic Then
            _x -= fontSize / 13.4
            _y += fontSize / 4
            ContentByte.SetTextMatrix(0, -1, 1, -0.3, trans.X(_x), trans.Y(_y))
        Else
            ContentByte.SetTextMatrix(0, -1, 1, 0, trans.X(_x), trans.Y(_y))
        End If
    End Sub

    Protected Overridable Sub _SetRotate2TextMatrix(
      fontSize As Single,
      x As Single,
      y As Single)
        Dim trans As PdfRenderer.TransClass = Renderer.Trans
        Dim _x As Single = Region.Left + x
        Dim _y As Single = Region.Top + y + fontSize
        If TextDesign.Font.Italic Then
            _x -= fontSize / 13.4
            _y += fontSize / 2
            ContentByte.SetTextMatrix(0, -1, -1, 0.3, trans.X(_x), trans.Y(_y))
        Else
            ContentByte.SetTextMatrix(0, -1, -1, 0, trans.X(_x), trans.Y(_y))
        End If
    End Sub

    Protected Overridable Sub _DrawText(
      fontSize As Single,
      text As String,
      x As Single,
      y As Single)
        _DrawText(fontSize, text, x, y, 0)
    End Sub

    Protected Overridable Sub _DrawText(
      fontSize As Single,
      text As String,
      x As Single,
      y As Single,
      charSpacing As Single)
        Dim _texts As List(Of String) = Nothing
        If Renderer.Setting.GaijiFont IsNot Nothing OrElse GaijiFont IsNot Nothing Then
            _texts = _DetectGaiji(text)
        End If
        If _texts Is Nothing Then
            If charSpacing > 0 Then
                ContentByte.SetCharacterSpacing(charSpacing)
            End If
            _SetTextMatrix(fontSize, x, y)
            ContentByte.ShowText(text)
        Else
            Dim gaiji As Boolean = False
            Dim _x As Single = x
            For Each t As String In _texts
                Dim si As StringInfo = New StringInfo(t)
                If si.LengthInTextElements > 0 Then
                    If Not gaiji Then
                        If charSpacing > 0 Then
                            ContentByte.SetCharacterSpacing(charSpacing)
                        End If
                        _SetTextMatrix(fontSize, _x, y)
                        ContentByte.ShowText(si.String)
                        _x += Font.GetWidthPoint(si.String, fontSize) + si.LengthInTextElements * charSpacing
                    Else
                        For Each c As Char In si.String
                            Dim f As BaseFont = Renderer.Setting.GaijiFont
                            If GaijiFont IsNot Nothing AndAlso GaijiFont.GetWidth(c) > 0 Then
                                f = GaijiFont
                            End If
                            If f Is Nothing Then
                                f = Font
                            End If
                            ContentByte.SetFontAndSize(f, fontSize)
                            _SetTextMatrix(fontSize, _x, y)
                            ContentByte.ShowText(c)
                            _x += fontSize + charSpacing
                        Next
                        ContentByte.SetFontAndSize(Font, fontSize)
                    End If
                End If
                gaiji = Not gaiji
            Next
        End If
    End Sub

    Protected Overridable Sub _DrawChar(
      fontSize As Single,
      c As String,
      x As Single,
      y As Single)
        Dim gaiji As Boolean = False
        If Renderer.Setting.GaijiFont IsNot Nothing AndAlso _IsGaiji(c) Then
            ContentByte.SetFontAndSize(Renderer.Setting.GaijiFont, fontSize)
            gaiji = True
        End If
        _SetTextMatrix(fontSize, x, y)
        ContentByte.ShowText(c)
        If gaiji Then
            ContentByte.SetFontAndSize(Font, fontSize)
        End If
    End Sub

    Protected Overridable Sub _DrawVerticalChar(
      fontSize As Single,
      c As String,
      x As Single,
      y As Single)
        Dim gaiji As Boolean = False
        If Renderer.Setting.GaijiFont IsNot Nothing AndAlso _IsGaiji(c) Then
            ContentByte.SetFontAndSize(Renderer.Setting.GaijiFont, fontSize)
            gaiji = True
        End If
        If _VerticalRotateCharsMap.ContainsKey(c) Then
            _SetRotateTextMatrix(
              fontSize, x - fontSize / 3, y - _GetTextWidth(fontSize, c))
        ElseIf _VerticalRotate2CharsMap.ContainsKey(c) Then
            _SetRotate2TextMatrix(
              fontSize, x + fontSize / 3, y - _GetTextWidth(fontSize, c))
        ElseIf _VERTICALSHIFTCHARSmap.ContainsKey(c) Then
            Dim d As Single = -_GetTextWidth(fontSize, c) / 2
            If TextDesign.Font.Italic Then
                d += fontSize / 4
            End If
            _SetTextMatrix(fontSize, x, y + d)
        Else
            _SetTextMatrix(
              fontSize, x - _GetTextWidth(fontSize, c) / 2, y)
        End If
        ContentByte.ShowText(c)
        If gaiji Then
            ContentByte.SetFontAndSize(Font, fontSize)
        End If
    End Sub

    Protected Overridable Sub _DrawUnderline(
      fontSize As Single,
      x As Single,
      y As Single,
      width As Single,
      lineWidth As Single)
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

    Protected Overridable Sub _DrawVerticalUnderLine(
      fontSize As Single,
      x As Single,
      y As Single,
      h As Single,
      lineWidth As Single)
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

    Protected Shared Function _GetDistributeMap(w As Single, c As Integer, fontSize As Single) As List(Of Single)
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

    Protected Function _GetFitFontSize(texts As List(Of String)) As Single
        Dim t As String = Nothing
        Dim w As Single = 0
        Dim rw As Single = Region.GetWidth - MARGIN_X * 2
        For Each _t As String In texts
            Dim _w As Single = _GetTextWidth(TextDesign.Font.Size, _t)
            If w < _w Then
                w = _w
                t = _t
            End If
        Next
        If w <= rw Then
            Return TextDesign.Font.Size
        End If
        Dim _i As Integer = 0
        Do While Renderer.Setting.ShrinkFontSizeMin + _i * Renderer.Setting.ShrinkFontSizeStep < TextDesign.Font.Size
            _i += 1
        Loop
        For i As Integer = _i - 1 To 1 Step -1
            Dim s As Single = Renderer.Setting.ShrinkFontSizeMin + i * Renderer.Setting.ShrinkFontSizeStep
            If _GetTextWidth(s, t) <= rw Then
                Return s
            End If
        Next
        Return Renderer.Setting.ShrinkFontSizeMin
    End Function

    Protected Function _GetTextWidth(fontSize As Single, text As String) As Single
        Dim _texts As List(Of String) = Nothing
        If Renderer.Setting.GaijiFont IsNot Nothing OrElse GaijiFont IsNot Nothing Then
            _texts = _DetectGaiji(text)
        End If
        Dim ret As Single = 0
        If _texts Is Nothing Then
            ret = Font.GetWidthPoint(text, fontSize)
        Else
            Dim g As Boolean = False
            For Each t As String In _texts
                Dim si As StringInfo = New StringInfo(t)
                If si.LengthInTextElements > 0 Then
                    If Not g Then
                        ret += Font.GetWidthPoint(si.String, fontSize)
                    Else
                        ret += si.LengthInTextElements * fontSize
                    End If
                End If
                g = Not g
            Next
        End If
        If TextDesign.Font.Italic Then
            ret += fontSize / 6
        End If
        Return ret
    End Function

    Protected Shared Function _DetectGaiji(text As String) As List(Of String)
        Dim ret As List(Of String) = Nothing
        Dim g As Boolean = False
        Dim last As Integer = 0
        Dim si As StringInfo = New StringInfo(text)
        For i As Integer = 0 To si.LengthInTextElements - 1
            If _IsGaiji(si.SubstringByTextElements(i, 1)) Then
                If Not g Then
                    If ret Is Nothing Then
                        ret = New List(Of String)
                    End If
                    ret.Add(si.SubstringByTextElements(last, i - last))
                    last = i
                    g = True
                End If
            Else
                If g Then
                    ret.Add(si.SubstringByTextElements(last, i - last))
                    last = i
                    g = False
                End If
            End If
        Next
        If ret IsNot Nothing Then
            ret.Add(si.SubstringByTextElements(last))
        End If
        Return ret
    End Function

    Protected Shared Function _IsGaiji(c As Char) As Boolean
        Dim a As Integer = AscW(c)
        Return a >= &HE000 And a <= &HF8FF
    End Function

    Protected Class _TextSplitterByPdfWidth
        Inherits TextSplitter
        Private _PdfText As PdfText
        Public Sub New(pdfText As PdfText)
            MyBase.New(Not Report.Compatibility._4_34_PdfWrapNoRule)
            Me._PdfText = pdfText
        End Sub
        Protected Overrides Function _GetNextWidth(si As StringInfo) As Integer
            Dim cw As Single = Me._PdfText.Region.GetWidth - MARGIN_X * 2
            Dim fontSize As Single = Me._PdfText.TextDesign.Font.Size
            If Me._PdfText._GetTextWidth(fontSize, si.String) > cw + TOLERANCE Then
                Dim i As Integer
                For i = 2 To si.LengthInTextElements
                    If Me._PdfText._GetTextWidth(fontSize, si.SubstringByTextElements(0, i)) > cw + TOLERANCE Then
                        Return i - 1
                    End If
                Next
            End If
            Return si.LengthInTextElements
        End Function
    End Class

    Protected Overridable Function _IsMonospacedFont() As Boolean
        If Not Renderer.monospacedFontCache.ContainsKey(Font) Then
            Renderer.monospacedFontCache.Add(Font,
              ReportUtil.RoundDown(Me.Font.GetWidthPoint("i", 1), -3) =
              ReportUtil.RoundDown(Me.Font.GetWidthPoint("W", 1), -3))
        End If
        Return Renderer.monospacedFontCache(Font)
    End Function

    Protected Class _FixDec

        Public PdfText As PdfText
        Public Text1 As String = ""
        Public Text2 As String = ""
        Public Text3 As String = ""

        Public Sub New(pdfText As PdfText)
            Me.PdfText = pdfText
            Dim si As StringInfo = New StringInfo(Me.PdfText.Text)
            With Regex.Match(si.String, "([^0-9]*)$")
                Me.Text3 = .Groups(0).Value
                si.String = si.SubstringByTextElements(0, si.LengthInTextElements - Me.Text3.Length)
            End With
            With Regex.Match(si.String, "(\.[0-9]*)?$")
                Me.Text2 = .Groups(0).Value
                Me.Text1 = si.SubstringByTextElements(0, si.LengthInTextElements - Me.Text2.Length)
            End With
        End Sub

        Public Function GetFullText2(space As Boolean) As String
            Dim ret As String = Me.Text2
            If ret.Length = 0 Then
                ret = IIf(space, " ", ".")
            End If
            Return ret.PadRight(Me.PdfText.TextDesign.DecimalPlace + 1, IIf(space, " ", "0"))
        End Function

        Public Function GetFullText(space As Boolean) As String
            Return Me.Text1 & Me.GetFullText2(space) & Me.Text3
        End Function

        Public Sub DrawText(fontSize As Single)
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
                Dim t As String = Me.Text1 & Me.GetFullText2(False)
                Dim ft As String = t & Me.Text3
                Dim w As Single = PdfText._GetTextWidth(fontSize, t)
                Dim fw As Single = PdfText._GetTextWidth(fontSize, ft)
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
                PdfText._Draw_Preprocess()
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

        Protected Overridable Sub _DrawText_Aux(
          fontSize As Single,
          x As Single,
          y As Single,
          text As String)
            Dim _x As Single = Math.Max(x, MARGIN_X)
            Dim w As Single = PdfText.Region.GetWidth - _x - MARGIN_X
            Dim si As StringInfo = New StringInfo(text)
            If w < 0 Then
                Exit Sub
            End If
            If PdfText._GetTextWidth(fontSize, si.String) > w + TOLERANCE Then
                Dim _t As String = ""
                Dim __t As String = ""
                For i As Integer = 1 To si.LengthInTextElements
                    __t = si.SubstringByTextElements(0, i)
                    If PdfText._GetTextWidth(fontSize, __t) <= w + TOLERANCE Then
                        _t = __t
                    Else
                        Exit For
                    End If
                Next
                si.String = _t
            End If
            If si.LengthInTextElements > 0 Then
                PdfText._DrawText(fontSize, si.String, _x, y)
            End If
        End Sub

    End Class


End Class
