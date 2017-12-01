Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_4_33_Crosstab

    Public Overrides Function ToString() As String
        Return "4.33 クロス集計表"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_33_crosstab"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(GetDataTable))

        Dim pages As ReportPages = report.GetPages()
        With Nothing
            Dim printer As New Printer(pages)
            Dim preview As New FmPrintPreview(printer)
            preview.ShowDialog()
        End With
    End Sub

    Public Function GetDataTable() As DataTable
        Dim ret As New DataTable()
        Dim branchNms() As String =
            {"北上本店", "東京支店", "盛岡営業所", "秋田営業所",
            "仙台営業所", "山形営業所", "福島営業所"}
        Dim periodNms() As String =
            {"2010年上期", "2010年下期", "2011年上期", "2011年下期",
             "2012年上期", "2012年下期", "2013年上期", "2013年下期",
             "2014年上期", "2014年下期", "2015年上期", "2015年下期",
             "2016年上期", "2016年下期"}
        ret.Columns.Add("branch_cd", GetType(Integer))
        ret.Columns.Add("branch_nm", GetType(String))
        ret.Columns.Add("period_cd", GetType(Integer))
        ret.Columns.Add("period_nm", GetType(String))
        ret.Columns.Add("amount", GetType(Decimal))
        For i As Integer = 0 To 13
            For j As Integer = 0 To IIf(i <= 3, 4, 6)
                ret.Rows.Add(j + 1, branchNms(j), i + 1, periodNms(i), 10000 + i * 100 + j * 10)
            Next
        Next
        Return ret
    End Function

End Class
