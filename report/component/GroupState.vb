Imports jp.co.systembase.report.component

Namespace component
    Public Class GroupState
        Public ParentState As ContentState
        Public First As Boolean = False
        Public Last As Boolean = False
        Public Last2 As Boolean = False
        Public Index As Integer = 0
        Public GroupFirst As Boolean = False
        Public GroupLast As Boolean = False
        Public GroupLast2 As Boolean = False
        Public GroupIndex As Integer = 0
        Public Blank As Boolean = False
        Public BlankFirst As Boolean = False
        Public BlankLast As Boolean = False
        Public Sub New(parentState As ContentState)
            Me.ParentState = parentState
        End Sub
    End Class
End Namespace