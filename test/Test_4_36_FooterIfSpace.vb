Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_4_36_FooterIfSpace

    Public Overrides Function ToString() As String
        Return "4.36 隙間があればフッター出力(page_group_count版)"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_36_footer_if_space"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(GetDataTable))
        Dim pages As ReportPages = report.GetPages()

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.StartUpZoomFit = True
        preview.ShowDialog()
    End Sub

    Private Function GetDataTable() As DataTable
        Dim ret As New DataTable()
        ret.Columns.Add("k", GetType(Decimal))
        ret.Columns.Add("v", GetType(Decimal))

        For i As Integer = 1 To 5
            For j As Integer = 1 To 3 + i
                ret.Rows.Add(i, j)
            Next
        Next
        Return ret
    End Function

End Class
