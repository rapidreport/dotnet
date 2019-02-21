Imports System.Drawing

Namespace component

    Public Class Region

        Public Top As Single = 0
        Public Left As Single = 0
        Public Bottom As Single = 0
        Public Right As Single = 0
        Public MaxBottom As Single = 0
        Public MaxRight As Single = 0

        Public Sub New()
        End Sub

        Public Sub New(region As Region)
            Me.Top = region.Top
            Me.Left = region.Left
            Me.Bottom = region.Bottom
            Me.Right = region.Right
            Me.MaxBottom = region.MaxBottom
            Me.MaxRight = region.MaxRight
        End Sub

        Public Sub New(region As Region, ml As Single, mt As Single, mr As Single, mb As Single)
            Me.Top = region.Top + mt
            Me.Left = region.Left + ml
            Me.Bottom = region.Bottom - mb
            Me.Right = region.Right - mr
            Me.MaxBottom = region.MaxBottom
            Me.MaxRight = region.MaxRight
        End Sub

        Public Function GetMergedRegion(region As Region) As Region
            Dim ret As New Region
            If Me.Top < region.Top Then
                ret.Top = Me.Top
            Else
                ret.Top = region.Top
            End If
            If Me.Left < region.Left Then
                ret.Left = Me.Left
            Else
                ret.Left = region.Left
            End If
            If Me.Bottom > region.Bottom Then
                ret.Bottom = Me.Bottom
            Else
                ret.Bottom = region.Bottom
            End If
            If Me.Right > region.Right Then
                ret.Right = Me.Right
            Else
                ret.Right = region.Right
            End If
            If Me.MaxBottom > region.MaxBottom Then
                ret.MaxBottom = Me.MaxBottom
            Else
                ret.MaxBottom = region.MaxBottom
            End If
            If Me.MaxRight > region.MaxRight Then
                ret.MaxRight = Me.MaxRight
            Else
                ret.MaxRight = region.MaxRight
            End If
            Return ret
        End Function

        Public Function GetWidth() As Single
            Return Me.Right - Me.Left
        End Function

        Public Function GetHeight() As Single
            Return Me.Bottom - Me.Top
        End Function

        Public Function GetMaxWidth() As Single
            Return Me.MaxRight - Me.Left
        End Function

        Public Function GetMaxHeight() As Single
            Return Me.MaxBottom - Me.Top
        End Function

        Public Sub SetWidth(w As Single)
            Me.Right = Me.Left + w
        End Sub

        Public Sub SetHeight(h As Single)
            Me.Bottom = Me.Top + h
        End Sub

        Public Function ToPointScale(reportDesign As ReportDesign) As Region
            Return Me.ToPointScale(reportDesign.PaperDesign.ScaleUnit)
        End Function

        Public Function ToPointScale(scaleUnit As Report.EScaleUnit) As Region
            Dim ret As New Region
            ret.Top = ReportUtil.ToPoint(scaleUnit, Me.Top)
            ret.Left = ReportUtil.ToPoint(scaleUnit, Me.Left)
            ret.Bottom = ReportUtil.ToPoint(scaleUnit, Me.Bottom)
            ret.Right = ReportUtil.ToPoint(scaleUnit, Me.Right)
            ret.MaxBottom = ReportUtil.ToPoint(scaleUnit, Me.MaxBottom)
            ret.MaxRight = ReportUtil.ToPoint(scaleUnit, Me.MaxRight)
            Return ret
        End Function

        Public Function IsVOverflowed()
            Return (Me.Bottom > Me.MaxBottom)
        End Function

        Public Function IsHOverflowed()
            Return (Me.Right > Me.MaxRight)
        End Function

        Public Function ClipMargin() As Region
            Dim ret As New Region
            ret.Top = 0
            ret.Left = 0
            ret.Bottom = Me.GetHeight
            ret.Right = Me.GetWidth
            ret.MaxBottom = ret.Bottom
            ret.MaxRight = ret.Right
            Return ret
        End Function

        Public Function ToRectangleF() As RectangleF
            Return ToRectangleF(0, 0)
        End Function

        Public Function ToRectangleF(offsetX As Single, offsetY As Single) As RectangleF
            Return New RectangleF(Me.Left + offsetX, Me.Top + offsetY,
                                  Me.GetWidth - offsetX, Me.GetHeight - offsetY)
        End Function

        Public Overrides Function ToString() As String
            Return "(" & Me.Left & ", " & Me.Top & ") - " &
                   "(" & Me.Right & ", " & Me.Bottom & ") - " &
                   "(" & Me.MaxRight & ", " & Me.MaxBottom & ")"
        End Function

    End Class

End Namespace