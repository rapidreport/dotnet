Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_4_28_MonthEn

    Public Overrides Function ToString() As String
        Return "4.28 月英語表記"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_28_month_en"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(GetDataTable))
        Dim pages As ReportPages = report.GetPages()

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Public Function GetDataTable() As DataTable
        Dim ret As New DataTable()
        ret.Columns.Add("date", GetType(Date))
        ret.Rows.Add(New DateTime(2010, 1, 2, 1, 2, 3))
        ret.Rows.Add(New DateTime(2011, 2, 4, 2, 3, 4))
        ret.Rows.Add(New DateTime(2012, 3, 6, 3, 4, 5))
        ret.Rows.Add(New DateTime(2013, 4, 8, 4, 5, 6))
        ret.Rows.Add(New DateTime(2014, 5, 10, 5, 6, 7))
        ret.Rows.Add(New DateTime(2015, 6, 12, 6, 7, 8))
        ret.Rows.Add(New DateTime(2016, 7, 14, 7, 8, 9))
        ret.Rows.Add(New DateTime(2017, 8, 16, 8, 9, 10))
        ret.Rows.Add(New DateTime(2018, 9, 18, 9, 10, 11))
        ret.Rows.Add(New DateTime(2019, 10, 20, 10, 11, 12))
        ret.Rows.Add(New DateTime(2020, 11, 22, 11, 12, 13))
        ret.Rows.Add(New DateTime(2021, 12, 24, 12, 13, 14))
        Return ret
    End Function

End Class
