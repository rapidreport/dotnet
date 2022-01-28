Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_5_12_SortKeys

    Public Overrides Function ToString() As String
        Return "5.12 ソートキーの昇順/降順"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_12_sortkeys"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(_GetDataTable()))

        Dim pages As ReportPages = report.GetPages()
        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function _GetDataTable() As DataTable
        Dim ret As New DataTable()
        Dim l() = New Object() {Nothing, 1, 2}
        ret.Columns.Add("a", GetType(Object))
        ret.Columns.Add("b", GetType(Object))
        ret.Columns.Add("c", GetType(Object))
        ret.Columns.Add("d", GetType(Object))
        For a = 0 To 2
            For b = 0 To 2
                For c = 0 To 2
                    For d = 0 To 2
                        ret.Rows.Add(l(a), l(b), l(c), l(d))
                    Next
                Next
            Next
        Next
        Return ret
    End Function

End Class
