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
        Public XlsFormat As String = Nothing
        Public MonospacedFont As MonospacedFontDesign.DetailDesign = Nothing

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
            Me.MonospacedFont = reportDesign.MonospacedFontDesign.Get(Me.Font)
        End Sub

        Public Function GetMonospacedWidth(si As StringInfo) As Single
            Dim ret As Single = Font.Size / 3
            For i As Integer = 0 To si.LengthInTextElements - 1
                If IsSingleChar(si.SubstringByTextElements(i, 1)) Then
                    ret += MonospacedFont.HalfWidth * Font.Size
                Else
                    ret += MonospacedFont.FullWidth * Font.Size
                End If
            Next
            Return ret
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
            Return GetMonospacedDrawableLen(si, width, si.LengthInTextElements)
        End Function
        Public Function GetMonospacedDrawableLen(si As StringInfo, width As Single, maxLen As Integer) As Integer
            Dim ret As Integer = 0
            Dim w As Single = Font.Size / 3.0
            For i As Integer = 0 To si.LengthInTextElements - 1
                If ReportUtil.IsSingleChar(si.SubstringByTextElements(i, 1)) Then
                    w += MonospacedFont.HalfWidth * Font.Size
                Else
                    w += MonospacedFont.FullWidth * Font.Size
                End If
                If (i > 1 And w >= width) Or (i = maxLen) Then
                    Return i
                End If
            Next
            Return si.LengthInTextElements
        End Function

    End Class

End Namespace
