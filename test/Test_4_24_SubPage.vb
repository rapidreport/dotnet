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

Public Class Test_4_24_SubPage

    Public Overrides Function ToString() As String
        Return "4.24 サブページ"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_24_subpage"

        Dim subReport As New Report(Json.Read("rrpt\" & name & "2.rrpt"))
        subReport.Fill(New ReportDataSource(GetDataTable))
        Dim subPages As ReportPages = subReport.GetPages()

        Dim report As New Report(Json.Read("rrpt\" & name & "1.rrpt"))
        report.AddSubPages("subpage", subPages)
        report.Fill(New SubPageDataSource(subPages, "group1", "page1", "page2"))
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
        ret.Columns.Add("g1", GetType(String))
        ret.Columns.Add("g2", GetType(String))
        ret.Columns.Add("num", GetType(Decimal))
        ret.Rows.Add("A", "A1", 10)
        ret.Rows.Add("A", "A1", 10)
        ret.Rows.Add("A", "A1", 10)
        ret.Rows.Add("A", "A1", 10)
        ret.Rows.Add("A", "A1", 10)
        ret.Rows.Add("A", "A1", 10)
        ret.Rows.Add("A", "A1", 10)
        ret.Rows.Add("A", "A1", 10)
        ret.Rows.Add("A", "A1", 10)
        ret.Rows.Add("A", "A1", 10)
        ret.Rows.Add("A", "A2", 100)
        ret.Rows.Add("A", "A2", 100)
        ret.Rows.Add("A", "A2", 100)
        ret.Rows.Add("A", "A2", 100)
        ret.Rows.Add("A", "A2", 100)
        ret.Rows.Add("A", "A2", 100)
        ret.Rows.Add("A", "A2", 100)
        ret.Rows.Add("A", "A2", 100)
        ret.Rows.Add("A", "A2", 100)
        ret.Rows.Add("A", "A2", 100)
        ret.Rows.Add("B", "B1", 20)
        ret.Rows.Add("B", "B1", 20)
        ret.Rows.Add("B", "B1", 20)
        ret.Rows.Add("B", "B1", 20)
        ret.Rows.Add("B", "B1", 20)
        ret.Rows.Add("B", "B1", 20)
        ret.Rows.Add("B", "B1", 20)
        ret.Rows.Add("B", "B1", 20)
        ret.Rows.Add("B", "B1", 20)
        ret.Rows.Add("B", "B1", 20)
        ret.Rows.Add("B", "B2", 200)
        ret.Rows.Add("B", "B2", 200)
        ret.Rows.Add("B", "B2", 200)
        ret.Rows.Add("B", "B2", 200)
        ret.Rows.Add("B", "B2", 200)
        ret.Rows.Add("B", "B2", 200)
        ret.Rows.Add("B", "B2", 200)
        ret.Rows.Add("B", "B2", 200)
        ret.Rows.Add("B", "B2", 200)
        ret.Rows.Add("B", "B2", 200)
        Return ret
    End Function
End Class
