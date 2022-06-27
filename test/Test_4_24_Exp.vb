Imports System.IO

Imports jp.co.systembase.NPOI.HSSF.UserModel
Imports jp.co.systembase.NPOI.XSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.renderer.xlsx

Public Class Test_4_24_Exp

    Public Overrides Function ToString() As String
        Return "4.24 式"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_24_exp"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.GlobalScope.Add("time", Now)
        report.GlobalScope.Add("lang", "vb")
        report.Fill(New ReportDataSource(GetDataTable))

        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            pages.Render(renderer)
        End Using

        Using fs As New FileStream("out\" & name & ".xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet(name)
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Using fs As New FileStream("out\" & name & ".xlsx", IO.FileMode.Create)
            Dim workbook As New XSSFWorkbook
            Dim renderer As New XlsxRenderer(workbook)
            renderer.NewSheet(name)
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function GetDataTable() As DataTable
        Dim ret As New DataTable()

        ret.Columns.Add("日本語", GetType(String))
        ret.Columns.Add("半角 スペース", GetType(Date))
        ret.Columns.Add("全角　スペース", GetType(String))
        ret.Columns.Add("半角()", GetType(String))
        ret.Columns.Add("全角（）", GetType(String))
        ret.Columns.Add("円マーク\", GetType(String))
        ret.Columns.Add("改行" & vbCrLf & "改行", GetType(String))
        ret.Columns.Add("シングルクォート'", GetType(String))
        ret.Columns.Add("bumonCd", GetType(String))
        ret.Columns.Add("kaCd", GetType(String))
        ret.Columns.Add("kingaku(1)", GetType(Decimal))
        ret.Columns.Add("kingaku(2)", GetType(Decimal))
        ret.Columns.Add("kingaku(3)", GetType(Decimal))
        ret.Columns.Add("bool(1)", GetType(Boolean))
        ret.Columns.Add("kingaku1", GetType(Decimal))
        ret.Columns.Add("kingaku2", GetType(Decimal))
        ret.Columns.Add("kingaku3", GetType(Decimal))

        ret.Rows.Add("１番", DateTime.ParseExact("2013/03/01", "yyyy/MM/dd", Nothing),
            "株式会社　岩手商事", "半角カッコ()", "全角カッコ（）", "円マーク\", "改行" & vbCrLf & "した", "シングルクォート'",
            10, 1, 1000, 10, 3, True, 2000, 20, 6)
        ret.Rows.Add("１番", DateTime.ParseExact("2013/03/01", "yyyy/MM/dd", Nothing),
            "株式会社　岩手商事", "半角カッコ()", "全角カッコ（）", "円マーク\", "改行" & vbCrLf & "した", "シングルクォート'",
            10, 1, 990, 20, 4, False, 1980, 40, 8)
        ret.Rows.Add("１番", DateTime.ParseExact("2013/03/01", "yyyy/MM/dd", Nothing),
            "株式会社　岩手商事", "半角カッコ()", "全角カッコ（）", "円マーク\", "改行" & vbCrLf & "した", "シングルクォート'",
            10, 2, 990, 20, 4, False, 1980, 40, 8)
        ret.Rows.Add("１番", DateTime.ParseExact("2013/03/01", "yyyy/MM/dd", Nothing),
            "株式会社　岩手商事", "半角カッコ()", "全角カッコ（）", "円マーク\", "改行" & vbCrLf & "した", "シングルクォート'",
            10, 2, 990, 20, 4, False, 1980, 40, 8)
        ret.Rows.Add("１番", DateTime.ParseExact("2013/03/01", "yyyy/MM/dd", Nothing),
            "株式会社　岩手商事", "半角カッコ()", "全角カッコ（）", "円マーク\", "改行" & vbCrLf & "した", "シングルクォート'",
            20, 1, 980, 20, 4, False, 1960, 40, 8)
        ret.Rows.Add("１番", DateTime.ParseExact("2013/03/01", "yyyy/MM/dd", Nothing),
            "株式会社　岩手商事", "半角カッコ()", "全角カッコ（）", "円マーク\", "改行" & vbCrLf & "した", "シングルクォート'",
            20, 1, 970, 20, 4, False, 1940, 40, 8)
        Return ret
    End Function

End Class
