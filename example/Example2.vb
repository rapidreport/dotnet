Imports System.IO

Imports NPOI.HSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.xls

Module Example2

    Public Sub Run()
        Dim report As New Report(Json.Read("report\example2.rrpt"))
        Dim dataProvider As New GroupDataProvider
        dataProvider.GroupDataMap.Add("group_shonin", New ReportDataSource(getSubDataTable))
        report.Fill(New ReportDataSource(getDataTable), dataProvider)
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("output\example2.pdf", IO.FileMode.Create)
            pages.Render(New PdfRenderer(fs))
        End Using

        Using fs As New FileStream("output\example2.xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet("example2")
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function getDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("HAT_ID", GetType(Integer))
        ret.Columns.Add("TOKUI_NM", GetType(String))
        ret.Columns.Add("TOKUI_TANTO_NM", GetType(String))
        ret.Columns.Add("HIN_NM", GetType(String))
        ret.Columns.Add("HIN_CD", GetType(String))
        ret.Columns.Add("SURYO", GetType(Decimal))
        ret.Columns.Add("TANKA", GetType(Decimal))
        ret.Columns.Add("SHUKKABI", GetType(Date))
        ret.Rows.Add(1, "HOGE精密", "担当太郎", "パイロットパンチ", "AAA-BBB-CCC-DDD-1000", 1, 600, DateTime.ParseExact("2011/06/07", "yyyy/MM/dd", Nothing))
        ret.Rows.Add(1, "HOGE精密", "担当太郎", "ガイドプレート", "AIUEO-999.999", 5, 1050, DateTime.ParseExact("2011/06/15", "yyyy/MM/dd", Nothing))
        ret.Rows.Add(1, "HOGE精密", "担当太郎", "イジェクタピン", "1234-5678-9999", 1, 7340, DateTime.ParseExact("2011/06/13", "yyyy/MM/dd", Nothing))
        ret.Rows.Add(2, "FUGA機械", "担当花子", "ブロックダイ", "9999-8888-7777", 10, 1600, DateTime.ParseExact("2011/06/10", "yyyy/MM/dd", Nothing))
        ret.Rows.Add(2, "FUGA機械", "担当花子", "ブランジャ", "ZZZZZ-YYYYY-XXXXX", 5, 800, DateTime.ParseExact("2011/06/10", "yyyy/MM/dd", Nothing))
        Return ret
    End Function

    Private Function getSubDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("HAT_ID", GetType(Integer))
        ret.Columns.Add("SHONIN_NM", GetType(String))
        ret.Rows.Add(1, "承認一郎")
        ret.Rows.Add(1, "承認次郎")
        ret.Rows.Add(1, "承認三郎")
        ret.Rows.Add(1, "承認四郎")
        ret.Rows.Add(2, "承認花子")
        Return ret
    End Function

End Module
