Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.data.internal

Public Class GroupSplitStringDesign

    Public Key As String
    Public Exp As String
    Public Width As Integer
    Public BreakRule As Boolean

    Public Sub New()
        Me.New(New Hashtable)
    End Sub

    Public Sub New(desc As Hashtable)
        Me.Key = desc("key")
        If desc.ContainsKey("exp") Then
            Me.Exp = desc("exp")
        Else
            Me.Exp = "." & Me.Key
        End If
        Me.Width = desc("width")
        Me.BreakRule = desc("break_rule")
    End Sub

    Public Function Split(data As ReportData) As List(Of ReportData)
        Dim ret As New List(Of ReportData)
        Dim t As String = (New Evaluator(data)).EvalTry(Me.Exp)
        Dim sp As New TextSplitterByWidth(Me.Width, Me.BreakRule)
        For Each _t As String In sp.GetLines(t)
            ret.Add(New ReportData(New SplitStringDataSource(data, Me.Key, _t), data.Report, data.Group))
        Next
        Return ret
    End Function

End Class
