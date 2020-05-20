Imports System.Globalization
Imports jp.co.systembase.report.Report

Namespace component

    Public Class TextDesign

        Public Font As FontDesign
        Public HAlign As EHAlign = EHAlign.LEFT
        Public VAlign As EVAlign = EVAlign.TOP
        Public Color As String = Nothing
        Public Wrap As Boolean = False
        Public Vertical As Boolean = False
        Public Distribute As Boolean = False
        Public ShrinkToFit As Boolean = False
        Public DecimalPlace As Integer = 0
        Public CharSpacing As Single = 0
        Public XlsFormat As String = Nothing
        Public MonospacedFont As MonospacedFontsDesign.DetailDesign = Nothing

        Public Sub New(reportDesign As ReportDesign, design As ElementDesign)
            If design.IsNull("font") Then
                Me.Font = reportDesign.DefaultFontDesign
            Else
                Me.Font = New FontDesign(design.Child("font"))
            End If
            If Not design.IsNull("halign") Then
                Select Case design.Get("halign")
                    Case "left"
                        Me.HAlign = EHAlign.LEFT
                    Case "center"
                        Me.HAlign = EHAlign.CENTER
                    Case "right"
                        Me.HAlign = EHAlign.RIGHT
                End Select
            End If
            If Not design.IsNull("valign") Then
                Select Case design.Get("valign")
                    Case "top"
                        Me.VAlign = EVAlign.TOP
                    Case "center"
                        Me.VAlign = EVAlign.CENTER
                    Case "bottom"
                        Me.VAlign = EVAlign.BOTTOM
                End Select
            End If
            If Not design.IsNull("decimal_place") Then
                Me.DecimalPlace = design.Get("decimal_place")
            End If
            Me.Color = design.Get("color")
            Me.Wrap = design.Get("wrap")
            Me.Vertical = design.Get("vertical")
            Me.Distribute = design.Get("distribute")
            Me.ShrinkToFit = design.Get("shrink_to_fit")
            Me.XlsFormat = design.Get("xls_format")
            If Not design.IsNull("char_spacing") Then
                Me.CharSpacing = design.Get("char_spacing")
            Else
                Me.CharSpacing = reportDesign.DefaultCharSpacing
            End If
            Me.MonospacedFont = reportDesign.MonospacedFontsDesign.Get(Me.Font)
        End Sub

        Public Function GetMonospacedWidth(si As StringInfo) As Single
            Return GetMonospacedWidth(si, Font.Size)
        End Function
        Public Function GetMonospacedWidth(si As StringInfo, fontSize As Single) As Single
            Dim ret As Single = fontSize / 3
            For i As Integer = 0 To si.LengthInTextElements - 1
                ret += MonoWidth(si.SubstringByTextElements(i, 1), fontSize)
            Next
            Return ret
        End Function

        Public Function GetPdfCharSpacing(si As StringInfo, fontSize As Single) As Single
            Dim cs As Single = 0
            Dim c As Integer = 0
            For i As Integer = 0 To si.LengthInTextElements - 1
                If ReportUtil.IsSingleChar(si.SubstringByTextElements(i, 1)) Then
                    cs += _MonoHalfWidth() - 0.5
                Else
                    cs += _MonoFullWidth() - 1.0
                End If
                c += 1
            Next
            If c > 0 Then
                Return (cs / c) * fontSize
            Else
                Return 0
            End If
        End Function

        Public Function GetMonospacedFitFontSize(texts As List(Of String), width As Single, minSize As Single) As Single
            Dim w As Single = 0
            For Each t As String In texts
                w = Math.Max(GetMonospacedWidth(New StringInfo(t)), w)
            Next
            If w <= width Then
                Return Font.Size
            Else
                Return Math.Max(ReportUtil.RoundDown(Font.Size * width / w, -1), minSize)
            End If
        End Function

        Public Function GetMonospacedDrawableLen(si As StringInfo, width As Single) As Integer
            Return GetMonospacedDrawableLen(si, width, -1)
        End Function
        Public Function GetMonospacedDrawableLen(si As StringInfo, width As Single, maxLen As Integer) As Integer
            Dim w As Single = Font.Size / 3.0
            For i As Integer = 0 To si.LengthInTextElements - 1
                w += MonoWidth(si.SubstringByTextElements(i, 1), Font.Size)
                If (i > 0 And w >= width) Or (i = maxLen) Then
                    Return i
                End If
            Next
            Return si.LengthInTextElements
        End Function

        Private Function _MonoHalfWidth() As Single
            Return MonospacedFont.HalfWidth * (1 + CharSpacing)
        End Function

        Private Function _MonoFullWidth() As Single
            Return MonospacedFont.FullWidth * (1 + CharSpacing)
        End Function

        Public Function MonoWidth(c As String, fontSize As Single)
            If ReportUtil.IsSingleChar(c) Then
                Return _MonoHalfWidth() * fontSize
            Else
                Return _MonoFullWidth() * fontSize
            End If
        End Function

    End Class

End Namespace
