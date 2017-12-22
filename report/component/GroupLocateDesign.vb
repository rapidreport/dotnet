Namespace component

    Public Class GroupLocateDesign

        Public X As Single
        Public Y As Single
        Public Count As Integer

        Public Sub New(desc As Hashtable)
            Me.X = desc("x")
            Me.Y = desc("y")
            Me.Count = desc("count")
        End Sub

        Public Function GetRegion(parentRegion As Region) As Region
            Dim ret As New Region
            ret.Top = parentRegion.Top + Me.Y
            ret.Left = parentRegion.Left + Me.X
            ret.Bottom = parentRegion.Bottom
            ret.Right = parentRegion.Right
            ret.MaxBottom = parentRegion.MaxBottom
            ret.MaxRight = parentRegion.MaxRight
            Return ret
        End Function

    End Class

End Namespace