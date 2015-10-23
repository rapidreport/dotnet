Imports System.IO
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.pdf
Imports NPOI.HSSF.UserModel
Imports jp.co.systembase.report.renderer.xls
Imports NPOI.XSSF.UserModel
Imports jp.co.systembase.report.renderer.xlsx
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_4_20_MergeContent

    Public Overrides Function ToString() As String
        Return "4.20 差込コンテント"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_20_merge_content"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(GetDataTable))

        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            renderer.Setting.ReplaceBackslashToYen = True
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

    Private Function getDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("mitsumoriNo", GetType(Decimal))
        ret.Columns.Add("mitsumoriDate", GetType(Date))
        ret.Columns.Add("tokuisaki1", GetType(String))
        ret.Columns.Add("tokuisaki2", GetType(String))
        ret.Columns.Add("hinmei", GetType(String))
        ret.Columns.Add("irisu", GetType(Decimal))
        ret.Columns.Add("hakosu", GetType(Decimal))
        ret.Columns.Add("tani", GetType(String))
        ret.Columns.Add("tanka", GetType(Decimal))
        ret.Rows.Add(101, DateTime.ParseExact("2013/03/01", "yyyy/MM/dd", Nothing), _
            "株式会社 岩手商事", "北上支社", _
            "ノートパソコン", 1, 10, "台", 70000)
        ret.Rows.Add(101, DateTime.ParseExact("2013/03/01", "yyyy/MM/dd", Nothing), _
            "株式会社 岩手商事", "北上支社", _
            "モニター", 1, 10, "台", 20000)
        ret.Rows.Add(101, DateTime.ParseExact("2013/03/01", "yyyy/MM/dd", Nothing), _
            "株式会社 岩手商事", "北上支社", _
            "プリンタ", 1, 2, "台", 25000)
        ret.Rows.Add(101, DateTime.ParseExact("2013/03/01", "yyyy/MM/dd", Nothing), _
            "株式会社 岩手商事", "北上支社", _
            "トナーカートリッジ", 2, 2, "本", 5000)
        Return ret
    End Function

End Class