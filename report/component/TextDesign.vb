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
        End Sub

    End Class

End Namespace
