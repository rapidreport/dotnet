Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data

Imports jp.co.systembase.report.renderer.gdi

Public Class Test_4_25_Page

    Public Overrides Function ToString() As String
        Return "4.25 現在のページ印刷"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_25_page"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(GetDataTable))
        Dim pages As ReportPages = report.GetPages()

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.StartUpZoomFit = True
        preview.ShowDialog()
    End Sub

    Private Function GetDataTable() As DataTable
        Dim ret As New DataTable()
        ret.Columns.Add("num", GetType(Decimal))
        For i As Integer = 0 To 9
            ret.Rows.Add(i)
        Next
        Return ret
    End Function

End Class
