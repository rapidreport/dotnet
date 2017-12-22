Namespace component

    Public Class PaperDesign

        Public PaperType As Report.EPaperType = Report.EPaperType.A4
        Public Size As PaperSizeDesign = Nothing
        Public Margin As PaperMarginDesign = Nothing
        Public ScaleUnit As Report.EScaleUnit = Report.EScaleUnit.POINT

        Public Landscape As Boolean = False

        Public Sub New()
            Me.new(New Hashtable)
        End Sub

        Public Sub New(desc As Hashtable)
            Select Case desc("scale_unit")
                Case "mm"
                    Me.ScaleUnit = Report.EScaleUnit.MM
                Case "inch"
                    Me.ScaleUnit = Report.EScaleUnit.INCH
                Case Else
                    Me.ScaleUnit = Report.EScaleUnit.POINT
            End Select
            Select Case desc("type")
                Case "a3"
                    Me.PaperType = Report.EPaperType.A3
                Case "a5"
                    Me.PaperType = Report.EPaperType.A5
                Case "b4"
                    Me.PaperType = Report.EPaperType.B4
                Case "b5"
                    Me.PaperType = Report.EPaperType.B5
                Case Else
                    Me.PaperType = Report.EPaperType.A4
            End Select
            Me.Size = New PaperSizeDesign(Me.ScaleUnit, Me.PaperType)
            If desc.ContainsKey("size") Then
                Dim s As PaperSizeDesign = New PaperSizeDesign(desc("size"))
                If s.Width > 0 And s.Height > 0 Then
                    If Me.ToPoint(s.Width) > Report.PAPER_WIDTH_MAX Then
                        Throw New ArgumentException("pagesize.width too large")
                    End If
                    If Me.ToPoint(s.Height) > Report.PAPER_HEIGHT_MAX Then
                        Throw New ArgumentException("pagesize.height too large")
                    End If
                    Me.PaperType = Report.EPaperType.CUSTOM
                    Me.Size = s
                End If
            End If
            Me.Landscape = desc("landscape")
            If desc.ContainsKey("margin") Then
                Me.Margin = New PaperMarginDesign(desc("margin"))
            Else
                Me.Margin = New PaperMarginDesign
            End If
        End Sub

        Public Function GetActualSize() As PaperSizeDesign
            Dim ret As New PaperSizeDesign
            If Me.Landscape Then
                ret.Width = Me.Size.Height
                ret.Height = Me.Size.Width
            Else
                ret.Width = Me.Size.Width
                ret.Height = Me.Size.Height
            End If
            Return ret
        End Function

        Public Function GetRegion() As Region
            Dim ret As New Region
            Dim paperSize As PaperSizeDesign = Me.GetActualSize
            ret.Top = 0
            ret.Left = 0
            ret.Bottom = paperSize.Height - Me.Margin.Top - Me.Margin.Bottom
            ret.Right = paperSize.Width - Me.Margin.Left - Me.Margin.Right
            ret.MaxBottom = ret.Bottom
            ret.MaxRight = ret.Right
            Return ret
        End Function

        Public Function ToPoint(v As Single) As Single
            Return ReportUtil.ToPoint(Me.ScaleUnit, v)
        End Function

    End Class
End Namespace