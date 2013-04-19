Namespace component

    Public Class GroupLayoutDesign

        Public Direction As Report.EDirection = Report.EDirection.VERTICAL
        Public Size As Single = 0
        Public SpecSize As Boolean = False
        Public X As Single = 0
        Public Y As Single = 0
        Public MaxCount As Integer = 0
        Public Blank As Boolean = False
        Public ClipOverflow As Boolean = False
        Public Locates As List(Of GroupLocateDesign) = Nothing

        Public Sub New()
            Me.new(New Hashtable)
        End Sub

        Public Sub New(ByVal desc As Hashtable)
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
            Me.MaxCount = desc("max_count")
            Me.Blank = desc("blank")
            Me.ClipOverflow = desc("clip_overflow")
            If desc.ContainsKey("locates") Then
                Me.Locates = New List(Of GroupLocateDesign)
                For Each d As Hashtable In desc("locates")
                    Me.Locates.Add(New GroupLocateDesign(d))
                Next
            Else
                Me.Locates = Nothing
            End If
        End Sub

        Public Function GetCount() As Integer
            If Me.IsLocateEnabled Then
                Return Me.Locates.Count
            Else
                Return Me.MaxCount
            End If
        End Function

        Public Function IsLocateEnabled() As Boolean
            Return Me.Locates IsNot Nothing AndAlso Me.Locates.Count > 0
        End Function

        Public Function GetGroupRegion( _
          ByVal parentRegion As Region, _
          ByVal lastRegion As Region, _
          ByVal i As Integer) As Region
            If Me.IsLocateEnabled Then
                Return Me.Locates(i).GetRegion(parentRegion)
            Else
                Dim ret As New Region
                ret.Top = parentRegion.Top + Me.Y
                ret.Left = parentRegion.Left + Me.X
                ret.Bottom = parentRegion.Bottom
                ret.Right = parentRegion.Right
                ret.MaxBottom = parentRegion.MaxBottom
                ret.MaxRight = parentRegion.MaxRight
                If lastRegion IsNot Nothing Then
                    Select Case Me.Direction
                        Case Report.EDirection.VERTICAL
                            ret.Top = lastRegion.Bottom
                        Case Report.EDirection.HORIZONTAL
                            ret.Left = lastRegion.Right
                    End Select
                End If
                Return ret
            End If
        End Function

        Public Function GetGroupInitialRegion(ByVal parentRegion As Region) As Region
            Dim ret As New Region
            ret.Top = parentRegion.Top
            ret.Left = parentRegion.Left
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

        Public Function GetContentRegion(ByVal sizeDesign As ContentSizeDesign, ByVal contentsRegion As Region, ByVal lastRegion As Region) As Region
            Dim ret As New Region
            Select Case Me.Direction
                Case Report.EDirection.VERTICAL
                    If lastRegion Is Nothing Then
                        ret.Top = contentsRegion.Top
                        ret.Left = contentsRegion.Left
                    Else
                        ret.Top = lastRegion.Bottom
                        ret.Left = lastRegion.Left
                    End If
                    If sizeDesign.SpecInitial Then
                        If Not sizeDesign.RevInitial Then
                            ret.Bottom = ret.Top + sizeDesign.Initial
                        Else
                            ret.Bottom = contentsRegion.MaxBottom - sizeDesign.Initial
                        End If
                    Else
                        ret.Bottom = ret.Top
                    End If
                    ret.Right = contentsRegion.Right
                    If sizeDesign.NotExtendable Then
                        ret.MaxBottom = ret.Bottom
                    ElseIf sizeDesign.SpecMax Then
                        If Not sizeDesign.RevMax Then
                            ret.MaxBottom = ret.Top + sizeDesign.Max
                        Else
                            ret.MaxBottom = contentsRegion.MaxBottom - sizeDesign.Max
                        End If
                    Else
                        ret.MaxBottom = contentsRegion.MaxBottom
                    End If
                    If ret.MaxBottom > contentsRegion.MaxBottom Then
                        ret.MaxBottom = contentsRegion.MaxBottom
                    End If
                    ret.MaxRight = ret.Right
                Case Report.EDirection.HORIZONTAL
                    If lastRegion Is Nothing Then
                        ret.Top = contentsRegion.Top
                        ret.Left = contentsRegion.Left
                    Else
                        ret.Top = lastRegion.Top
                        ret.Left = lastRegion.Right
                    End If
                    ret.Bottom = contentsRegion.Bottom
                    If sizeDesign.SpecInitial Then
                        If Not sizeDesign.RevInitial Then
                            ret.Right = ret.Left + sizeDesign.Initial
                        Else
                            ret.Right = contentsRegion.MaxRight - sizeDesign.Initial
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
                            ret.MaxRight = contentsRegion.MaxRight - sizeDesign.Max
                        End If
                    Else
                        ret.MaxRight = contentsRegion.MaxRight
                    End If
                    If ret.MaxRight > contentsRegion.MaxRight Then
                        ret.MaxRight = contentsRegion.MaxRight
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