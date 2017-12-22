Namespace component

    Public Class GroupMarginDesign

        Public Top As Single
        Public Left As Single
        Public Bottom As Single
        Public Right As Single

        Public Sub New()
            Me.new(New Hashtable)
        End Sub

        Public Sub New(desc As Hashtable)
            Me.Top = desc("top")
            Me.Left = desc("left")
            Me.Bottom = desc("bottom")
            Me.Right = desc("right")
        End Sub

        Public Function GetRegion(region As Region) As Region
            Dim ret As New Region
            ret.Top = region.Top + Me.Top
            ret.Left = region.Left + Me.Left
            ret.Bottom = region.Bottom - Me.Bottom
            ret.Right = region.Right - Me.Right
            ret.MaxBottom = region.MaxBottom - Me.Bottom
            ret.MaxRight = region.MaxRight - Me.Right
            Return ret
        End Function

        Public Function GetExtendedRegion( _
          region As Region, _
          subRegion As Region) As Region
            Dim ret As New Region
            ret.Top = region.Top
            ret.Left = region.Left
            ret.MaxBottom = region.MaxBottom
            ret.MaxRight = region.MaxRight
            With Nothing
                Dim b As Single = subRegion.Bottom + Me.Bottom
                If b > region.Bottom Then
                    ret.Bottom = b
                Else
                    ret.Bottom = region.Bottom
                End If
            End With
            With Nothing
                Dim r As Single = subRegion.Right + Me.Right
                If r > region.Right Then
                    ret.Right = r
                Else
                    ret.Right = region.Right
                End If
            End With
            Return ret
        End Function

    End Class
End Namespace

