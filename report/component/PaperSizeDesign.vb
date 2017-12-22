Namespace component

    Public Class PaperSizeDesign

        Public Width As Single
        Public Height As Single

        Public Sub New()
        End Sub

        Public Sub New(desc As Hashtable)
            Me.Width = CType(desc("width"), Single)
            Me.Height = CType(desc("height"), Single)
        End Sub

        Public Sub New( _
          scaleUnit As Report.EScaleUnit, _
          paperType As Report.EPaperType)
            Select Case scaleUnit
                Case Report.EScaleUnit.POINT
                    Me.initialize_point(paperType)
                Case Report.EScaleUnit.MM
                    Me.initialize_mm(paperType)
                Case Report.EScaleUnit.INCH
                    Me.initialize_inch(paperType)
            End Select
        End Sub

        Private Sub initialize_point(paperType As Report.EPaperType)
            Select Case paperType
                Case Report.EPaperType.A3
                    Me.Width = 842
                    Me.Height = 1192
                Case Report.EPaperType.A4
                    Me.Width = 595
                    Me.Height = 842
                Case Report.EPaperType.A5
                    Me.Width = 420
                    Me.Height = 595
                Case Report.EPaperType.B4
                    Me.Width = 728
                    Me.Height = 1031
                Case Report.EPaperType.B5
                    Me.Width = 515
                    Me.Height = 728
            End Select
        End Sub

        Private Sub initialize_mm(paperType As Report.EPaperType)
            Select Case paperType
                Case Report.EPaperType.A3
                    Me.Width = 297
                    Me.Height = 420
                Case Report.EPaperType.A4
                    Me.Width = 210
                    Me.Height = 297
                Case Report.EPaperType.A5
                    Me.Width = 148
                    Me.Height = 210
                Case Report.EPaperType.B4
                    Me.Width = 257
                    Me.Height = 364
                Case Report.EPaperType.B5
                    Me.Width = 182
                    Me.Height = 257
            End Select
        End Sub

        Private Sub initialize_inch(paperType As Report.EPaperType)
            Select Case paperType
                Case Report.EPaperType.A3
                    Me.Width = 11.69
                    Me.Height = 16.54
                Case Report.EPaperType.A4
                    Me.Width = 8.27
                    Me.Height = 11.69
                Case Report.EPaperType.A5
                    Me.Width = 5.83
                    Me.Height = 8.27
                Case Report.EPaperType.B4
                    Me.Width = 10.12
                    Me.Height = 14.33
                Case Report.EPaperType.B5
                    Me.Width = 7.17
                    Me.Height = 10.12
            End Select
        End Sub

        Public Function ToPoint(paperDesign As PaperDesign) As PaperSizeDesign
            Return Me.ToPoint(paperDesign.ScaleUnit)
        End Function

        Public Function ToPoint(scaleUnit As Report.EScaleUnit) As PaperSizeDesign
            Dim ret As New PaperSizeDesign
            ret.Width = ReportUtil.ToPoint(scaleUnit, Me.Width)
            ret.Height = ReportUtil.ToPoint(scaleUnit, Me.Height)
            Return ret
        End Function

    End Class

End Namespace
