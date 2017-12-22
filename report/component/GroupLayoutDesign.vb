Namespace component

    Public Class GroupLayoutDesign

        Public Direction As Report.EDirection = Report.EDirection.VERTICAL
        Public Size As Single = 0
        Public SpecSize As Boolean = False
        Public X As Single = 0
        Public Y As Single = 0
        Public MaxCount As Integer = 0
        Public MaxCountExp As String = Nothing
        Public Blank As Boolean = False
        Public ClipOverflow As Boolean = False
        Public Locates As List(Of GroupLocateDesign) = Nothing

        Public Sub New()
            Me.new(New Hashtable)
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
            Me.MaxCount = desc("max_count")
            Me.MaxCountExp = desc("max_count_exp")
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

        Public Function GetCount(evaluator As Evaluator) As Integer
            If Me.IsLocateEnabled Then
                Dim ret As Integer = 0
                For Each l As GroupLocateDesign In Me.Locates
                    ret += Math.Max(1, l.Count)
                Next
                Return ret
            ElseIf evaluator IsNot Nothing AndAlso Me.MaxCountExp IsNot Nothing Then
                Try
                    Return evaluator.EvalTry(Me.MaxCountExp)
                Catch ex As Exception
                    Return 0
                End Try
            End If
            Return Me.MaxCount
        End Function

        Public Function IsLocateEnabled() As Boolean
            Return Me.Locates IsNot Nothing AndAlso Me.Locates.Count > 0
        End Function

        Public Function GetGroupRegion( _
          parentRegion As Region, _
          lastRegion As Region, _
          i As Integer) As Region
            If Me.IsLocateEnabled Then
                Dim _i As Integer = i
                For Each l As GroupLocateDesign In Me.Locates
                    If _i = 0 OrElse lastRegion Is Nothing Then
                        Return l.GetRegion(parentRegion)
                    Else
                        _i -= Math.Max(1, l.Count)
                        If _i < 0 Then
                            Dim ret As New Region
                            Select Case Me.Direction
                                Case Report.EDirection.VERTICAL
                                    ret.Top = lastRegion.Bottom
                                    ret.Left = lastRegion.Left
                                Case Report.EDirection.HORIZONTAL
                                    ret.Top = lastRegion.Top
                                    ret.Left = lastRegion.Right
                            End Select
                            ret.Bottom = lastRegion.Bottom
                            ret.Right = lastRegion.Right
                            ret.MaxBottom = lastRegion.MaxBottom
                            ret.MaxRight = lastRegion.MaxRight
                            Return ret
                        End If
                    End If
                Next
            End If
            With Nothing
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
            End With
        End Function

        Public Function GetGroupInitialRegion(parentRegion As Region) As Region
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

        Public Function GetContentRegion(sizeDesign As ContentSizeDesign, contentsRegion As Region, lastRegion As Region, evaluator As Evaluator) As Region
            Dim ret As New Region
            Dim init As Single = sizeDesign.Initial
            Dim max As Single = sizeDesign.Max
            Dim specInit As Single = sizeDesign.SpecInitial
            Dim specMax As Single = sizeDesign.SpecMax
            If evaluator IsNot Nothing Then
                If sizeDesign.InitialExp IsNot Nothing Then
                    Try
                        init = evaluator.EvalTry(sizeDesign.InitialExp)
                    Catch ex As Exception
                        init = 0
                    End Try
                    specInit = True
                End If
                If sizeDesign.MaxExp IsNot Nothing Then
                    Try
                        max = evaluator.EvalTry(sizeDesign.MaxExp)
                    Catch ex As Exception
                        max = 0
                    End Try
                    specMax = True
                End If
            End If
            Select Case Me.Direction
                Case Report.EDirection.VERTICAL
                    If lastRegion Is Nothing Then
                        ret.Top = contentsRegion.Top
                        ret.Left = contentsRegion.Left
                    Else
                        ret.Top = lastRegion.Bottom
                        ret.Left = lastRegion.Left
                    End If
                    If specInit Then
                        If Not sizeDesign.RevInitial Then
                            ret.Bottom = ret.Top + init
                        Else
                            ret.Bottom = contentsRegion.MaxBottom - init
                        End If
                    Else
                        ret.Bottom = ret.Top
                    End If
                    ret.Right = contentsRegion.Right
                    If sizeDesign.NotExtendable Then
                        ret.MaxBottom = ret.Bottom
                    ElseIf specMax Then
                        If Not sizeDesign.RevMax Then
                            ret.MaxBottom = ret.Top + max
                        Else
                            ret.MaxBottom = contentsRegion.MaxBottom - max
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
                    If specInit Then
                        If Not sizeDesign.RevInitial Then
                            ret.Right = ret.Left + init
                        Else
                            ret.Right = contentsRegion.MaxRight - init
                        End If
                    Else
                        ret.Right = ret.Left
                    End If
                    ret.MaxBottom = ret.Bottom
                    If sizeDesign.NotExtendable Then
                        ret.MaxRight = ret.Right
                    ElseIf specMax Then
                        If Not sizeDesign.RevMax Then
                            ret.MaxRight = ret.Left + max
                        Else
                            ret.MaxRight = contentsRegion.MaxRight - max
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