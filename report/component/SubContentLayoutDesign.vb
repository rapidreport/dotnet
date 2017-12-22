Namespace component

    Public Class SubContentLayoutDesign

        Public X1 As Single
        Public Y1 As Single
        Public X2 As Single
        Public Y2 As Single
        Public SpecX1 As Boolean
        Public SpecY1 As Boolean
        Public SpecX2 As Boolean
        Public SpecY2 As Boolean
        Public RevX1 As Boolean
        Public RevY1 As Boolean
        Public RevX2 As Boolean
        Public RevY2 As Boolean

        Public Sub New()
            Me.new(New Hashtable)
        End Sub

        Public Sub New(desc As Hashtable)
            If desc.ContainsKey("x1") Then
                Me.X1 = desc("x1")
                Me.SpecX1 = True
            Else
                Me.X1 = 0
                Me.SpecX1 = False
            End If
            If desc.ContainsKey("y1") Then
                Me.Y1 = desc("y1")
                Me.SpecY1 = True
            Else
                Me.Y1 = 0
                Me.SpecY1 = False
            End If
            If desc.ContainsKey("x2") Then
                Me.X2 = desc("x2")
                Me.SpecX2 = True
            Else
                Me.X2 = 0
                Me.SpecX2 = False
            End If
            If desc.ContainsKey("y2") Then
                Me.Y2 = desc("y2")
                Me.SpecY2 = True
            Else
                Me.Y2 = 0
                Me.SpecY2 = False
            End If
            Me.RevX1 = desc("rev_x1")
            Me.RevY1 = desc("rev_y1")
            Me.RevX2 = desc("rev_x2")
            Me.RevY2 = desc("rev_y2")
        End Sub

        Public Function GetRegion(parentRegion As Region) As Region
            Dim ret As New Region
            If Me.SpecY1 Then
                If Not Me.RevY1 Then
                    ret.Top = parentRegion.Top + Me.Y1
                Else
                    ret.Top = parentRegion.Bottom - Me.Y1
                End If
            Else
                ret.Top = parentRegion.Top
            End If
            If Me.SpecX1 Then
                If Not Me.RevX1 Then
                    ret.Left = parentRegion.Left + Me.X1
                Else
                    ret.Left = parentRegion.Right - Me.X1
                End If
            Else
                ret.Left = parentRegion.Left
            End If
            If Me.SpecY2 Then
                If Not Me.RevY2 Then
                    ret.Bottom = parentRegion.Top + Me.Y2
                Else
                    ret.Bottom = parentRegion.Bottom - Me.Y2
                End If
            Else
                ret.Bottom = parentRegion.Bottom
            End If
            If Me.SpecX2 Then
                If Not Me.RevX2 Then
                    ret.Right = parentRegion.Left + Me.X2
                Else
                    ret.Right = parentRegion.Right - Me.X2
                End If
            Else
                ret.Right = parentRegion.Right
            End If
            ret.MaxBottom = ret.Bottom
            ret.MaxRight = ret.Right
            Return ret
        End Function

    End Class
End Namespace