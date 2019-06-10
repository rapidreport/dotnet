Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_5_2_Sort

    Public Overrides Function ToString() As String
        Return "5.2 ソート"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_2_sort"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(_GetDataTable()))

        Dim pages As ReportPages = report.GetPages()
        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function _GetDataTable() As DataTable
        Dim ret As New DataTable()
        ret.Columns.Add("col1", GetType(Decimal))
        ret.Columns.Add("col2", GetType(Decimal))
        ret.Rows.Add(1, 10)
        ret.Rows.Add(2, 20)
        ret.Rows.Add(3, 30)
        ret.Rows.Add(4, 20)
        ret.Rows.Add(5, 10)
        ret.Rows.Add(6, 30)
        ret.Rows.Add(7, 10)
        ret.Rows.Add(8, 30)
        ret.Rows.Add(9, 20)
        ret.Rows.Add(10, 30)
        ret.Rows.Add(11, 20)
        ret.Rows.Add(12, 10)
        Return ret
    End Function

End Class
