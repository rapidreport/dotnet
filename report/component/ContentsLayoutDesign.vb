Namespace component

    Public Class ContentsLayoutDesign

        Public Direction As Report.EDirection
        Public X As Single
        Public Y As Single
        Public Size As Single
        Public SpecSize As Boolean

        Public Sub New()
            Me.New(New Hashtable)
        End Sub

        Public Sub New(desc As Hashtable)
            If desc("direction") = "horizontal" Then
                Me.Direction = Report.EDirection.HORIZONTAL
            Else
                Me.Direction = Report.EDirection.VERTICAL
            End If
            If desc.ContainsKey("size") Then
                Me.Size = desc("size")
                Me.SpecSize = True
            Else
                Me.Size = 0
                Me.SpecSize = False
            End If
            Me.X = desc("x")
            Me.Y = desc("y")
        End Sub

        Public Function GetRegion(parentRegion As Region) As Region
            Dim ret As New Region
            ret.Top = parentRegion.Top + Me.Y
            ret.Left = parentRegion.Left + Me.X
            Select Case Me.Direction
                Case Report.EDirection.VERTICAL
                    ret.Bottom = ret.Top
                    ret.MaxBottom = parentRegion.MaxBottom
                    If Me.SpecSize Then
                        ret.Right = ret.Left + Me.Size
                    Else
                        ret.Right = parentRegion.Right
                    End If
                    ret.MaxRight = ret.Right
                Case Report.EDirection.HORIZONTAL
                    If Me.SpecSize Then
                        ret.Bottom = ret.Top + Me.Size
                    Else
                        ret.Bottom = parentRegion.Bottom
                    End If
                    ret.MaxBottom = ret.Bottom
                    ret.Right = ret.Left
                    ret.MaxRight = parentRegion.MaxRight
            End Select
            Return ret
        End Function

        Public Function GetContentRegion(sizeDesign As ContentSizeDesign, region As Region, lastRegion As Region) As Region
            Dim ret As New Region
            Select Case Me.Direction
                Case Report.EDirection.VERTICAL
                    If lastRegion Is Nothing Then
                        ret.Top = region.Top
                        ret.Left = region.Left
                    Else
                        ret.Top = lastRegion.Bottom
                        ret.Left = lastRegion.Left
                    End If
                    If sizeDesign.SpecInitial Then
                        If Not sizeDesign.RevInitial Then
                            ret.Bottom = ret.Top + sizeDesign.Initial
                        Else
                            ret.Bottom = region.MaxBottom - sizeDesign.Initial
                        End If
                    Else
                        ret.Bottom = ret.Top
                    End If
                    ret.Right = region.Right
                    If sizeDesign.NotExtendable Then
                        ret.MaxBottom = ret.Bottom
                    ElseIf sizeDesign.SpecMax Then
                        If Not sizeDesign.RevMax Then
                            ret.MaxBottom = ret.Top + sizeDesign.Max
                        Else
                            ret.MaxBottom = region.MaxBottom - sizeDesign.Max
                        End If
                    Else
                        ret.MaxBottom = region.MaxBottom
                    End If
                    If ret.MaxBottom > region.MaxBottom Then
                        ret.MaxBottom = region.MaxBottom
                    End If
                    ret.MaxRight = ret.Right
                Case Report.EDirection.HORIZONTAL
                    If lastRegion Is Nothing Then
                        ret.Top = region.Top
                        ret.Left = region.Left
                    Else
                        ret.Top = lastRegion.Top
                        ret.Left = lastRegion.Right
                    End If
                    ret.Bottom = region.Bottom
                    If sizeDesign.SpecInitial Then
                        If Not sizeDesign.RevInitial Then
                            ret.Right = ret.Left + sizeDesign.Initial
                        Else
                            ret.Right = region.MaxRight - sizeDesign.Initial
                        End If
                    Else
                        ret.Right = ret.Left
                    End If
                    ret.MaxBottom = ret.Bottom
                    If sizeDesign.NotExtendable Then
                        ret.MaxRight = ret.Right
                    ElseIf sizeDesign.SpecMax Then
                        If Not sizeDesign.RevMax Then
                            ret.MaxRight = ret.Left + sizeDesign.Max
                        Else
                            ret.MaxRight = region.MaxRight - sizeDesign.Max
                        End If
                    Else
                        ret.MaxRight = region.MaxRight
                    End If
                    If ret.MaxRight > region.MaxRight Then
                        ret.MaxRight = region.MaxRight
                    End If
            End Select
            If ret.Bottom < ret.Top Then
                ret.Bottom = ret.Top
            End If
            If ret.MaxBottom < ret.Top Then
                ret.MaxBottom = ret.Top
            End If
            If ret.Right < ret.Left Then
                ret.Right = ret.Left
            End If
            If ret.MaxRight < ret.Left Then
                ret.MaxRight = ret.Left
            End If
            Return ret
        End Function

    End Class

End Namespace