Imports System.Globalization
Imports System.Text.RegularExpressions

Imports jp.co.systembase.report.component

Public Class GdiText

    Public Graphics As Graphics
    Public Region As Region
    Public Setting As GdiRendererSetting
    Public TextDesign As TextDesign
    Public Text As String
    Public IsMonospaced As Boolean

    Protected Const TOLERANCE As Single = 0.1F

    Public Overridable Sub Initialize(
      env As RenderingEnv,
      reportDesign As ReportDesign,
      region As Region,
      design As ElementDesign,
      text As String)
        Me.Graphics = env.Graphics
        Me.Region = region.ToPointScale(reportDesign)
        Me.Setting = env.Setting
        Me.TextDesign = New TextDesign(reportDesign, design)
        Me.Text = text
        Me.IsMonospaced =
            Me.TextDesign.MonospacedFont IsNot Nothing And
            env.IsMonospacedFont(Me.TextDesign.Font.Name)
    End Sub

    Public Overridable Sub Draw()
        If Me._IsEmpty Then
            Return
        End If
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
    End Sub

    Protected Overridable Sub _Draw_Distribute()
        Dim font As Font = _GetFont()
        Dim texts As List(Of String) = (New TextSplitter()).GetLines(Me.Text)
        Dim color As Color = GdiRenderUtil.GetColor(Me.TextDesign.Color, Drawing.Color.Black)
        Dim sf As StringFormat = _GetStringFormat(
            _ToStringAlignment(Report.EHAlign.CENTER),
            _ToStringAlignment(Report.EVAlign.TOP), 0)
        Dim y As Single = 0
        Select Case Me.TextDesign.VAlign
            Case Report.EVAlign.TOP
                y = 0
            Case Report.EVAlign.CENTER
                y = (Region.GetHeight - (font.Size * texts.Count)) / 2
                y = Math.Max(y, 0)
            Case Report.EVAlign.BOTTOM
                y = Region.GetHeight - (font.Size * texts.Count) - font.Size / 8
                y = Math.Max(y, 0)
        End Select
        Dim yc As Integer = Fix((Me.Region.GetHeight + TOLERANCE) / font.Size)
        For i As Integer = 0 To Math.Max(Math.Min(texts.Count, yc) - 1, 0)
            Dim si As StringInfo = New StringInfo(texts(i))
            Dim t As String = texts(i)
            Using b As New SolidBrush(color)
                Dim mx As Single = font.Size / 6
                Dim m As List(Of Single) = _GetDistributeMap(Region.GetWidth - mx * 2, si.LengthInTextElements, font)
                For j As Integer = 0 To si.LengthInTextElements - 1
                    Graphics.DrawString(si.SubstringByTextElements(j, 1), font, b, Region.Left + m(j) + mx, Region.Top + y, sf)
                Next
            End Using
            If TextDesign.Font.Underline Then
                Using p As New Pen(color)
                    p.Width = TextDesign.Font.Size / 13.4F
                    Graphics.DrawLine(
                      p,
                      Region.Left + font.Size / 6,
                      Region.Top + y + (font.Size - p.Width),
                      Region.Right - font.Size / 6,
                      Region.Top + y + (font.Size - p.Width))
                End Using
            End If
            y += font.Size
        Next
    End Sub

    Protected Overridable Sub _Draw_DistributeVertical()
        Dim font As Font = _GetFont()
        Dim texts As List(Of String) = (New TextSplitter()).GetLines(Me.Text)
        Dim color As Color = GdiRenderUtil.GetColor(TextDesign.Color, Drawing.Color.Black)
        Dim sf As StringFormat = _GetStringFormat(
            _ToStringAlignment(Report.EHAlign.CENTER),
            _ToStringAlignment(Report.EVAlign.CENTER),
            StringFormatFlags.DirectionVertical)
        Dim mx As Single = font.Size / 6
        Dim x As Single = 0
        Select Case TextDesign.HAlign
            Case Report.EHAlign.LEFT
                x = font.Size * (texts.Count - 1) + font.Size / 2 + mx
                x = Math.Min(x, Region.GetWidth - font.Size / 2 - mx)
            Case Report.EHAlign.CENTER
                x = (Region.GetWidth + font.Size * (texts.Count - 1)) / 2
                x = Math.Min(x, Region.GetWidth - font.Size / 2 - mx)
            Case Report.EHAlign.RIGHT
                x = Region.GetWidth - font.Size / 2 - mx
        End Select
        Dim xc As Integer = Fix((Region.GetWidth - mx * 2 + TOLERANCE) / font.Size)
        For i As Integer = 0 To Math.Max(Math.Min(xc, texts.Count) - 1, 0)
            Dim t As String = texts(i)
            Dim si As StringInfo = New StringInfo(texts(i))
            Using b As New SolidBrush(color)
                Dim m As List(Of Single) = _GetDistributeMap(Region.GetHeight, si.LengthInTextElements, font)
                For j As Integer = 0 To si.LengthInTextElements - 1
                    Dim c As String = si.SubstringByTextElements(j, 1)
                    Graphics.DrawString(c, font, b, Region.Left + x - font.Size / 6, Region.Top + m(j) + font.Size / 10, sf)
                Next
            End Using
            If TextDesign.Font.Underline Then
                Using p As New Pen(color)
                    p.Width = TextDesign.Font.Size / 13.4F
                    Me.Graphics.DrawLine(
                      p,
                      Region.Left + x + font.Size / 2 + p.Width,
                      Region.Top,
                      Region.Left + x + font.Size / 2 + p.Width,
                      Region.Bottom)
                End Using
            End If
            x -= font.Size
        Next
    End Sub

    Protected Overridable Sub _Draw_Vertical()
        Dim texts As List(Of String) = (New TextSplitter()).GetLines(Me.Text)
        _Draw_Vertical_Aux(texts, _GetFont())
    End Sub

    Protected Overridable Sub _Draw_VerticalShrink()
        Dim texts As List(Of String) = (New TextSplitter()).GetLines(Me.Text)
        Dim font As Font = _GetFont()
        With Nothing
            Dim m As Integer = 0
            For Each t As String In texts
                Dim si As StringInfo = New StringInfo(t)
                If m < si.LengthInTextElements Then
                    m = si.LengthInTextElements
                End If
            Next
            If m > 0 Then
                Dim fontSize As Single = Math.Max(Region.GetHeight / m, Setting.ShrinkFontSizeMin)
                If font.Size > fontSize Then
                    font = New Font(font.Name, fontSize, font.Style)
                End If
            End If
        End With
        _Draw_Vertical_Aux(texts, font)
    End Sub

    Protected Overridable Sub _Draw_VerticalWrap()
        Dim font As Font = _GetFont()
        Dim l As Integer = Fix((Region.GetHeight + TOLERANCE) / font.Size)
        _Draw_Vertical_Aux((New TextSplitterByLen(l)).GetLines(Me.Text), font)
    End Sub

    Protected Overridable Sub _Draw_Fixdec()
        Dim fd As New _FixDec(Me)
        fd.DrawText(_GetFont())
    End Sub

    Protected Overridable Sub _Draw_FixdecShrink()
        Dim fd As New _FixDec(Me)
        fd.DrawText(_GetFitFont(fd.GetFullText()))
    End Sub

    Protected Overridable Sub _Draw_Wrap()
        If Me.IsMonospaced Then
            Dim sp As New TextSplitterByDrawingWidth(TextDesign, Region.GetWidth, 0)
            _Draw_Monospaced(sp.GetLines(Text), _GetFont())
        Else
            Using b As New SolidBrush(GdiRenderUtil.GetColor(TextDesign.Color, Drawing.Color.Black))
                Graphics.DrawString(Text, _GetFont(), b, Region.ToRectangleF, _GetStringFormat(0))
            End Using
        End If
    End Sub

    Protected Overridable Sub _Draw_Shrink()
        If Me.IsMonospaced Then
            Dim texts As List(Of String) = (New TextSplitter(True)).GetLines(Text)
            _Draw_Monospaced(texts, _GetFont(TextDesign.GetMonospacedFitFontSize(texts, Region.GetWidth, Setting.ShrinkFontSizeMin)))
        Else
            Using b As New SolidBrush(GdiRenderUtil.GetColor(TextDesign.Color, Drawing.Color.Black))
                Graphics.DrawString(Text, _GetFitFont(Text), b, Region.ToRectangleF, _GetStringFormat(StringFormatFlags.NoWrap))
            End Using
        End If
    End Sub

    Protected Overridable Sub _Draw()
        If Me.IsMonospaced Then
            Dim sp As New TextSplitterByDrawingWidth(TextDesign, 0, Region.GetWidth)
            _Draw_Monospaced(sp.GetLines(Me.Text), _GetFont())
        Else
            Using b As New SolidBrush(GdiRenderUtil.GetColor(TextDesign.Color, Drawing.Color.Black))
                Graphics.DrawString(Text, _GetFont(), b, Region.ToRectangleF, _GetStringFormat(StringFormatFlags.NoWrap))
            End Using
        End If
    End Sub

    Protected Overridable Sub _Draw_Monospaced(texts As List(Of String), font As Font)
        Dim sf As StringFormat = _GetStringFormat(_ToStringAlignment(TextDesign.HAlign), StringAlignment.Near, StringFormatFlags.NoWrap)
        Dim y As Single = 0
        Select Case TextDesign.VAlign
            Case Report.EVAlign.TOP
                y = 0
            Case Report.EVAlign.CENTER
                y = (Region.GetHeight -
                  (0.125 + TextDesign.MonospacedFont.RowHeidht * texts.Count) * font.Size) / 2
            Case Report.EVAlign.BOTTOM
                y = Region.GetHeight -
                  (0.125 + TextDesign.MonospacedFont.RowHeidht * texts.Count) * font.Size
        End Select
        Using b As New SolidBrush(GdiRenderUtil.GetColor(TextDesign.Color, Drawing.Color.Black))
            For Each t As String In texts
                Graphics.DrawString(t, font, b, Region.ToRectangleF(0, y), sf)
                y += TextDesign.MonospacedFont.RowHeidht * font.Size
            Next
        End Using
    End Sub

    Protected Overridable Sub _Draw_Vertical_Aux(texts As List(Of String), Font As Font)
        Dim color As Color = GdiRenderUtil.GetColor(TextDesign.Color, Drawing.Color.Black)
        Dim mx As Single = Font.Size / 6
        Dim x As Single = 0
        Select Case TextDesign.HAlign
            Case Report.EHAlign.LEFT
                x = Font.Size * (texts.Count - 1) + Font.Size / 2 + mx
                x = Math.Min(x, Region.GetWidth - Font.Size / 2 - mx)
            Case Report.EHAlign.CENTER
                x = (Region.GetWidth + Font.Size * (texts.Count - 1)) / 2
                x = Math.Min(x, Region.GetWidth - Font.Size / 2 - mx)
            Case Report.EHAlign.RIGHT
                x = Region.GetWidth - Font.Size / 2 - mx
        End Select
        Dim sf As StringFormat = _GetStringFormat(
            _ToStringAlignment(Report.EHAlign.LEFT),
            _ToStringAlignment(Report.EVAlign.CENTER),
            StringFormatFlags.DirectionVertical)
        Dim cw As Single = Region.GetWidth - mx * 2
        Dim ch As Single = Region.GetHeight
        Dim xc As Integer = Fix((cw + TOLERANCE) / Font.Size)
        Dim yc As Integer = Fix((ch + TOLERANCE) / Font.Size)
        Using b As New SolidBrush(color)
            For i As Integer = 0 To Math.Min(texts.Count, xc) - 1
                Dim t As String = texts(i)
                Dim si As New StringInfo(t)
                Dim y As Single = 0
                Select Case TextDesign.VAlign
                    Case Report.EVAlign.TOP
                        y = 0
                    Case Report.EVAlign.CENTER
                        y = (Region.GetHeight - Font.Size * si.LengthInTextElements) / 2
                        y = Math.Max(y, 0)
                    Case Report.EVAlign.BOTTOM
                        y = Region.GetHeight - Font.Size * si.LengthInTextElements
                        y = Math.Max(y, 0)
                End Select
                Dim _yc As Integer = Math.Min(si.LengthInTextElements, yc)
                If TextDesign.Font.Underline Then
                    Using p As New Pen(color)
                        p.Width = TextDesign.Font.Size / 13.4F
                        Graphics.DrawLine(
                          p,
                          Region.Left + x + Font.Size / 2,
                          Region.Top + y,
                          Region.Left + x + Font.Size / 2,
                          Region.Top + y + Font.Size * _yc)
                    End Using
                End If
                For j As Integer = 0 To _yc - 1
                    Dim c As String = si.SubstringByTextElements(j, 1)
                    Graphics.DrawString(c, Font, b, Region.Left + x - Font.Size / 6, Region.Top + y - Font.Size / 10, sf)
                    y += Font.Size
                Next
                x -= Font.Size
            Next
        End Using
    End Sub

    Protected Overridable Function _GetStringFormat(formatFlags As StringFormatFlags)
        Return _GetStringFormat(_ToStringAlignment(TextDesign.HAlign), _ToStringAlignment(TextDesign.VAlign), formatFlags)
    End Function

    Protected Overridable Function _GetStringFormat(
      alignment As StringAlignment,
      lineAlignment As StringAlignment,
      formatFlags As StringFormatFlags) As StringFormat
        Dim ret As New StringFormat()
        ret.Alignment = alignment
        ret.LineAlignment = lineAlignment
        ret.FormatFlags = formatFlags
        If Not Report.Compatibility._4_34_GdiTextNotMeasureTrailingSpaces Then
            ret.FormatFlags = ret.FormatFlags Or StringFormatFlags.MeasureTrailingSpaces
        End If
        Return ret
    End Function

    Protected Overridable Function _IsEmpty() As Boolean
        If String.IsNullOrEmpty(Me.Text) Then
            Return True
        End If
        If Me.Region.GetWidth <= 0 OrElse Me.Region.GetHeight <= 0 Then
            Return True
        End If
        Return False
    End Function

    Protected Overridable Function _GetFont() As Font
        Return _GetFont(TextDesign.Font.Size)
    End Function

    Protected Overridable Function _GetFont(fontSize As Single) As Font
        Dim style As FontStyle = FontStyle.Regular
        If TextDesign.Font.Bold Then
            style = style Or FontStyle.Bold
        End If
        If TextDesign.Font.Italic Then
            style = style Or FontStyle.Italic
        End If
        If TextDesign.Font.Underline Then
            If Not (TextDesign.Distribute Or TextDesign.Vertical Or TextDesign.DecimalPlace > 0) Then
                style = style Or FontStyle.Underline
            End If
        End If
        Return New Font(Setting.GetFont(TextDesign.Font.Name), fontSize, style)
    End Function

    Protected Shared Function _ToStringAlignment(
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

    Protected Shared Function _ToStringAlignment(
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

    Protected Overridable Function _GetFitFont(text As String) As Font
        Dim f As Font = _GetFont()
        Dim sf As StringFormat = _GetStringFormat(StringAlignment.Near, StringAlignment.Near, 0)
        If Graphics.MeasureString(text, f, 100000, sf).Width <= Region.GetWidth Then
            Return f
        End If
        Dim _i As Integer = 0
        Do While Setting.ShrinkFontSizeMin + _i * Setting.ShrinkFontSizeStep < f.Size
            _i += 1
        Loop
        For i As Integer = _i - 1 To 1 Step -1
            Dim s As Single = Setting.ShrinkFontSizeMin + i * Setting.ShrinkFontSizeStep
            Dim font As New Font(f.Name, s, f.Style)
            If Graphics.MeasureString(text, font, 100000, sf).Width <= Region.GetWidth Then
                Return font
            End If
        Next
        Return New Font(f.Name, Setting.ShrinkFontSizeMin, f.Style)
    End Function

    Protected Overridable Function _GetDistributeMap(w As Single, c As Integer, font As Font) As List(Of Single)
        Dim ret As New List(Of Single)
        If c = 1 Then
            ret.Add(w / 2)
        Else
            Dim t As Single = font.Size / 2
            Do
                ret.Add(t)
                t += (w - font.Size - 2) / (c - 1)
            Loop While t < w And ret.Count < c
        End If
        Return ret
    End Function

    Protected Class _FixDec

        Public GdiText As GdiText
        Public Text1 As String
        Public Text2 As String
        Public Text3 As String

        Public Sub New(gdiText As GdiText)
            Me.GdiText = gdiText
            Dim t As String = Me.GdiText.Text
            If gdiText.IsMonospaced Then
                Dim si As New StringInfo(Me.GdiText.Text)
                t = si.SubstringByTextElements(0, gdiText.TextDesign.GetMonospacedDrawableLen(si, Me.GdiText.Region.GetWidth))
            End If
            With Regex.Match(t, "([^0-9]*)$")
                Me.Text3 = .Groups(0).Value
                t = t.Substring(0, t.Length - Me.Text3.Length)
            End With
            With Regex.Match(t, "(\.[0-9]*)?$")
                Me.Text2 = .Groups(0).Value
                Me.Text1 = t.Substring(0, t.Length - Me.Text2.Length)
            End With
        End Sub

        Public Function GetFullText2() As String
            Dim ret As String = Me.Text2
            If ret.Length = 0 Then
                ret = "."
            End If
            Return ret.PadRight(Me.GdiText.TextDesign.DecimalPlace + 1, "0")
        End Function

        Public Function GetFullText() As String
            Return Me.Text1 & Me.GetFullText2() & Me.Text3
        End Function

        Public Sub DrawText(font As Font)
            Dim color As Color = GdiRenderUtil.GetColor(GdiText.TextDesign.Color, Drawing.Color.Black)
            Dim ls As New SizeF(10000, 10000)
            Dim y As Single = 0
            Select Case GdiText.TextDesign.VAlign
                Case Report.EVAlign.TOP
                    y = 0
                Case Report.EVAlign.CENTER
                    y = (GdiText.Region.GetHeight - font.Size - font.Size / 8) / 2
                Case Report.EVAlign.BOTTOM
                    y = GdiText.Region.GetHeight - font.Size - font.Size / 8
            End Select
            Using b As New SolidBrush(color)
                Dim sf As StringFormat = GdiText._GetStringFormat(
                    StringAlignment.Near,
                    StringAlignment.Near,
                    StringFormatFlags.NoWrap)
                Dim t As String = Me.Text1 & Me.GetFullText2()
                Dim ft As String = t & Me.Text3
                Dim w As Single = GdiText.Graphics.MeasureString(t, font, ls, sf).Width
                Dim fw As Single = GdiText.Graphics.MeasureString(ft, font, ls, sf).Width
                Dim x As Single = 0
                Select Case GdiText.TextDesign.HAlign
                    Case Report.EHAlign.LEFT
                        x = 0
                    Case Report.EHAlign.CENTER
                        x = (GdiText.Region.GetWidth - fw) / 2
                        x = Math.Max(x, 0)
                    Case Report.EHAlign.RIGHT
                        x = GdiText.Region.GetWidth - fw
                        x = Math.Max(x, 0)
                End Select
                GdiText.Graphics.DrawString(Me.Text1 & Me.Text2, font, b, GdiText.Region.ToRectangleF(x, y), sf)
                If Me.Text3.Length > 0 Then
                    Dim _x As Single = x + w - font.Size / 3
                    If _x < GdiText.Region.GetWidth Then
                        GdiText.Graphics.DrawString(Me.Text3, font, b, GdiText.Region.ToRectangleF(_x, y), sf)
                    End If
                End If
                If GdiText.TextDesign.Font.Underline Then
                    Dim _x As Single = x + fw
                    _x = Math.Min(_x, GdiText.Region.GetWidth)
                    Dim m As Single = font.Size / 6
                    Using p As New Pen(color)
                        p.Width = font.Size / 13.4F
                        GdiText.Graphics.DrawLine(
                          p,
                          GdiText.Region.Left + x + m,
                          GdiText.Region.Top + y + (font.Size - p.Width),
                          GdiText.Region.Left + _x - m,
                          GdiText.Region.Top + y + (font.Size - p.Width))
                    End Using
                End If
            End Using
        End Sub

    End Class

End Class
