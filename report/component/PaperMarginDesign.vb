Namespace component

    Public Class PaperMarginDesign

        Public Top As Single = 0
        Public Left As Single = 0
        Public Bottom As Single = 0
        Public Right As Single = 0
        Public OddReverse As Boolean = False

        Public Sub New()
        End Sub

        Public Sub New(desc As Hashtable)
            Me.Top = desc("top")
            Me.Left = desc("left")
            Me.Bottom = desc("bottom")
            Me.Right = desc("right")
            Me.OddReverse = desc("odd_reverse")
        End Sub

        Public Function ToPoint(paperDesign As PaperDesign) As PaperMarginDesign
            Return Me.ToPoint(paperDesign.ScaleUnit)
        End Function

        Public Function ToPoint(scaleUnit As Report.EScaleUnit) As PaperMarginDesign
            Dim ret As New PaperMarginDesign
            ret.Top = ReportUtil.ToPoint(scaleUnit, Me.Top)
            ret.Left = ReportUtil.ToPoint(scaleUnit, Me.Left)
            ret.Bottom = ReportUtil.ToPoint(scaleUnit, Me.Bottom)
            ret.Right = ReportUtil.ToPoint(scaleUnit, Me.Right)
            ret.OddReverse = Me.OddReverse
            Return ret
        End Function

    End Class

End Namespace

