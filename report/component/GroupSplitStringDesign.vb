Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.data.internal

Public Class GroupSplitStringDesign

    Public Key As String
    Public Exp As String
    Public Width As Integer

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
    End Sub

    Public Function Split(data As ReportData) As List(Of ReportData)
        Dim ret As New List(Of ReportData)
        Dim t As String = (New Evaluator(data)).EvalTry(Me.Exp)

        If t Is Nothing Then
            ret.Add(New ReportData(New SplitStringDataSource(data, Me.Key, Nothing), data.Report, data.Group))
        Else
            For Each _t As String In ReportUtil.SplitLines(t)
                If Me.Width = 0 Then
                    ret.Add(New ReportData(New SplitStringDataSource(data, Me.Key, _t), data.Report, data.Group))
                Else
                    Dim b As Integer = 0
                    Dim e As Integer = 0
                    Do
                        b = e
                        e = ReportUtil.GetWIndex(_t, b, Me.Width)
                        ret.Add(New ReportData(
                                New SplitStringDataSource(data, Me.Key, ReportUtil.SubString(_t, b, e - b)),
                                data.Report, data.Group))
                    Loop While e < _t.Length
                End If
            Next
        End If

        Return ret
    End Function

End Class
