Imports System.Text.RegularExpressions

Imports jp.co.systembase.report.component

Public Class GdiText

    Public Graphics As Graphics
    Public Region As Region
    Public Setting As GdiRendererSetting
    Public TextDesign As TextDesign
    Public Text As String

    Protected Const TOLERANCE As Single = 0.1F
    Protected Const VERTICAL_ROTATE_CHARS As String = "～…‥｜ーｰ(){}[]<>（）｛｝「」＜＞。、"

    Public Overridable Sub Initialize( _
      ByVal env As RenderingEnv, _
      ByVal reportDesign As ReportDesign, _
      ByVal region As Region, _
      ByVal design As ElementDesign, _
      ByVal text As String)
        Me.Graphics = env.Graphics
        Me.Region = region.ToPointScale(reportDesign)
        Me.Setting = env.Setting
        Me.TextDesign = New TextDesign(reportDesign, design)
        Me.Text = text
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
        Dim font As Font = _GetFont(Me.Setting, Me.TextDesign, True)
        Dim texts As List(Of String) = _SplitByCr(Me.Region, font, Me.Text, False)
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
            Dim t As String = texts(i)
            Using b As New SolidBrush(color)
                Dim mx As Single = font.Size / 6
                Dim m As List(Of Single) = _GetDistributeMap(Region.GetWidth - mx * 2, t.Length, font)
                For j As Integer = 0 To t.Length - 1
                    Graphics.DrawString(t(j), font, b, Region.Left + m(j) + mx, Region.Top + y, sf)
                Next
            End Using
            If TextDesign.Font.Underline Then
                Using p As New Pen(color)
                    p.Width = TextDesign.Font.Size / 13.4F
                    Graphics.DrawLine( _
                      p, _
                      Region.Left + font.Size / 6, _
                      Region.Top + y + (font.Size - p.Width), _
                      Region.Right - font.Size / 6, _
                      Region.Top + y + (font.Size - p.Width))
                End Using
            End If
            y += font.Size
        Next
    End Sub

    Protected Overridable Sub _Draw_DistributeVertical()
        Dim font As Font = _GetFont(Setting, TextDesign, True)
        Dim texts As List(Of String) = _SplitByCr(Region, font, Text, False)
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
            Using b As New SolidBrush(color)
                Dim m As List(Of Single) = _GetDistributeMap(Region.GetHeight, t.Length, font)
                For j As Integer = 0 To t.Length - 1
                    Dim c As String = t(j)
                    Graphics.DrawString(c, font, b, Region.Left + x - font.Size / 6, Region.Top + m(j) + font.Size / 10, sf)
                Next
            End Using
            If TextDesign.Font.Underline Then
                Using p As New Pen(color)
                    p.Width = TextDesign.Font.Size / 13.4F
                    Me.Graphics.DrawLine( _
                      p, _
                      Region.Left + x + font.Size / 2 + p.Width, _
                      Region.Top, _
                      Region.Left + x + font.Size / 2 + p.Width, _
                      Region.Bottom)
                End Using
            End If
            x -= font.Size
        Next
    End Sub

    Protected Overridable Sub _Draw_Vertical()
        Dim font As Font = _GetFont(Setting, TextDesign, True)
        Dim texts As List(Of String) = _SplitByCr(Region, font, Text, False)
        _Draw_Vertical_Aux(texts, font)
    End Sub

    Protected Overridable Sub _Draw_VerticalShrink()
        Dim font As Font = _GetFont(Setting, TextDesign, True)
        Dim texts As List(Of String) = _SplitByCr(Region, font, Text, False)
        With Nothing
            Dim m As Integer = 0
            For Each t As String In texts
                If m < t.Length Then
                    m = t.Length
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
        Dim font As Font = _GetFont(Setting, TextDesign, True)
        Dim texts As List(Of String) = _SplitByCr(Region, font, Text, True)
        _Draw_Vertical_Aux(texts, font)
    End Sub

    Protected Overridable Sub _Draw_Fixdec()
        Dim fd As New _FixDec(Me)
        Dim font As Font = _GetFont(Setting, TextDesign, True)
        fd.DrawText(font)
    End Sub

    Protected Overridable Sub _Draw_FixdecShrink()
        Dim fd As New _FixDec(Me)
        Dim font As Font = _GetFont(Setting, TextDesign, True)
        font = _GetFitFont(Graphics, Region, Setting, fd.GetFullText(TextDesign.DecimalPlace), font,
                           Me._GetStringFormat(StringAlignment.Near, StringAlignment.Near, 0))
        fd.DrawText(font)
    End Sub

    Protected Overridable Sub _Draw_Wrap()
        Dim font As Font = _GetFont(Setting, TextDesign, False)
        Dim color As Color = GdiRenderUtil.GetColor(TextDesign.Color, Drawing.Color.Black)
        Dim r As New RectangleF(Region.Left, Region.Top, Region.GetWidth, Region.GetHeight)
        Dim sf As StringFormat = _GetStringFormat(
            _ToStringAlignment(TextDesign.HAlign),
            _ToStringAlignment(TextDesign.VAlign), 0)
        Using b As New SolidBrush(color)
            Graphics.DrawString(Text, font, b, r, sf)
        End Using
    End Sub

    Protected Overridable Sub _Draw_Shrink()
        Dim font As Font = _GetFont(Setting, TextDesign, False)
        font = _GetFitFont(Graphics, Region, Setting, Text, font,
                           Me._GetStringFormat(StringAlignment.Near, StringAlignment.Near, 0))
        Dim color As Color = GdiRenderUtil.GetColor(TextDesign.Color, Drawing.Color.Black)
        Dim r As New RectangleF(Region.Left, Region.Top, Region.GetWidth, Region.GetHeight)
        Dim sf As StringFormat = _GetStringFormat(
            _ToStringAlignment(TextDesign.HAlign),
            _ToStringAlignment(TextDesign.VAlign),
            StringFormatFlags.NoWrap)
        Using b As New SolidBrush(color)
            Graphics.DrawString(Text, font, b, r, sf)
        End Using
    End Sub

    Protected Overridable Sub _Draw()
        Dim font As Font = _GetFont(Setting, TextDesign, False)
        Dim color As Color = GdiRenderUtil.GetColor(TextDesign.Color, Drawing.Color.Black)
        Dim r As New RectangleF(Region.Left, Region.Top, Region.GetWidth, Region.GetHeight)
        Dim sf As StringFormat = _GetStringFormat(
            _ToStringAlignment(TextDesign.HAlign),
            _ToStringAlignment(TextDesign.VAlign),
            StringFormatFlags.NoWrap)
        Using b As New SolidBrush(color)
            Graphics.DrawString(Text, font, b, r, sf)
        End Using
    End Sub

    Protected Overridable Sub _Draw_Vertical_Aux( _
      ByVal texts As List(Of String), _
      ByVal font As Font)
        Dim color As Color = GdiRenderUtil.GetColor(TextDesign.Color, Drawing.Color.Black)
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
        Dim sf As StringFormat = _GetStringFormat(
            _ToStringAlignment(Report.EHAlign.LEFT),
            _ToStringAlignment(Report.EVAlign.CENTER),
            StringFormatFlags.DirectionVertical)
        Dim cw As Single = Region.GetWidth - mx * 2
        Dim ch As Single = Region.GetHeight
        Dim xc As Integer = Fix((cw + TOLERANCE) / font.Size)
        Dim yc As Integer = Fix((ch + TOLERANCE) / font.Size)
        Using b As New SolidBrush(color)
            For i As Integer = 0 To Math.Min(texts.Count, xc) - 1
                Dim t As String = texts(i)
                Dim y As Single = 0
                Select Case TextDesign.VAlign
                    Case Report.EVAlign.TOP
                        y = 0
                    Case Report.EVAlign.CENTER
                        y = (Region.GetHeight - font.Size * t.Length) / 2
                        y = Math.Max(y, 0)
                    Case Report.EVAlign.BOTTOM
                        y = Region.GetHeight - font.Size * t.Length
                        y = Math.Max(y, 0)
                End Select
                Dim _yc As Integer = Math.Min(t.Length, yc)
                If TextDesign.Font.Underline Then
                    Using p As New Pen(color)
                        p.Width = TextDesign.Font.Size / 13.4F
                        Graphics.DrawLine(
                          p,
                          Region.Left + x + font.Size / 2,
                          Region.Top + y,
                          Region.Left + x + font.Size / 2,
                          Region.Top + y + font.Size * _yc)
                    End Using
                End If
                For j As Integer = 0 To _yc - 1
                    Dim c As String = t(j)
                    Graphics.DrawString(c, font, b, Region.Left + x - font.Size / 6, Region.Top + y - font.Size / 10, sf)
                    y += font.Size
                Next
                x -= font.Size
            Next
        End Using
    End Sub

    Protected Overridable Function _GetStringFormat(
      alignment As StringAlignment,
      lineAlignment As StringAlignment,
      formatFlags As StringFormatFlags) As StringFormat
        Dim ret As New StringFormat()
        ret.Alignment = alignment
        ret.LineAlignment = lineAlignment
        ret.FormatFlags = formatFlags
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

    Protected Shared Function _GetFont( _
      ByVal setting As GdiRendererSetting, _
      ByVal textDesign As TextDesign, _
      ByVal ignoreUnderline As Boolean) As Font
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

    Protected Shared Function _ToStringAlignment( _
      ByVal align As Report.EHAlign) As StringAlignment
        Select Case align
            Case Report.EHAlign.LEFT
                Return StringAlignment.Near
            Case Report.EHAlign.CENTER
                Return StringAlignment.Center
            Case Report.EHAlign.RIGHT
                Return StringAlignment.Far
        End Select
    End Function

    Protected Shared Function _ToStringAlignment( _
      ByVal align As Report.EVAlign) As StringAlignment
        Select Case align
            Case Report.EVAlign.TOP
                Return StringAlignment.Near
            Case Report.EVAlign.CENTER
                Return StringAlignment.Center
            Case Report.EVAlign.BOTTOM
                Return StringAlignment.Far
        End Select
    End Function

    Protected Shared Function _GetFitFont( _
      ByVal g As Graphics, _
      ByVal region As Region, _
      ByVal setting As GdiRendererSetting, _
      ByVal text As String, _
      ByVal baseFont As Font, _
      ByVal stringFormat As StringFormat) As Font
        If g.MeasureString(text, baseFont, 100000, stringFormat).Width <= region.GetWidth Then
            Return baseFont
        End If
        Dim _i As Integer = 0
        Do While setting.ShrinkFontSizeMin + _i * setting.ShrinkFontSizeStep < baseFont.Size
            _i += 1
        Loop
        For i As Integer = _i - 1 To 1 Step -1
            Dim s As Single = setting.ShrinkFontSizeMin + i * setting.ShrinkFontSizeStep
            Dim font As New Font(baseFont.Name, s, baseFont.Style)
            If g.MeasureString(text, font, 100000, stringFormat).Width <= region.GetWidth Then
                Return font
            End If
        Next
        Return New Font(baseFont.Name, setting.ShrinkFontSizeMin, baseFont.Style)
    End Function

    Protected Shared Function _GetDistributeMap(ByVal w As Single, ByVal c As Integer, ByVal font As Font) As List(Of Single)
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

    Protected Shared Function _SplitByCr( _
      ByVal region As Region, _
      ByVal font As Font, _
      ByVal text As String, _
      ByVal wrap As Boolean) As List(Of String)
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

    Protected Class _FixDec

        Public GdiText As GdiText
        Public Text1 As String
        Public Text2 As String
        Public Text3 As String

        Public Sub New(ByVal gdiText As GdiText)
            Me.GdiText = gdiText
            Dim t As String = Me.GdiText.Text
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

        Public Sub DrawText(ByVal font As Font)
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
                Dim t As String = Me.Text1 & Me.GetFullText2(GdiText.TextDesign.DecimalPlace)
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
                With Nothing
                    Dim r As New RectangleF( _
                      GdiText.Region.Left + x, _
                      GdiText.Region.Top + y, _
                      GdiText.Region.GetWidth - x, _
                      GdiText.Region.GetHeight - y)
                    GdiText.Graphics.DrawString(Me.Text1 & Me.Text2, font, b, r, sf)
                End With
                If Me.Text3.Length > 0 Then
                    Dim _x As Single = x + w - font.Size / 3
                    If _x < GdiText.Region.GetWidth Then
                        Dim r As New RectangleF( _
                          GdiText.Region.Left + _x, _
                          GdiText.Region.Top + y, _
                          GdiText.Region.GetWidth - _x, _
                          GdiText.Region.GetHeight - y)
                        GdiText.Graphics.DrawString(Me.Text3, font, b, r, sf)
                    End If
                End If
                If GdiText.TextDesign.Font.Underline Then
                    Dim _x As Single = x + fw
                    _x = Math.Min(_x, GdiText.Region.GetWidth)
                    Dim m As Single = font.Size / 6
                    Using p As New Pen(color)
                        p.Width = font.Size / 13.4F
                        GdiText.Graphics.DrawLine( _
                          p, _
                          GdiText.Region.Left + x + m, _
                          GdiText.Region.Top + y + (font.Size - p.Width), _
                          GdiText.Region.Left + _x - m, _
                          GdiText.Region.Top + y + (font.Size - p.Width))
                    End Using
                End If
            End Using
        End Sub

    End Class

End Class
