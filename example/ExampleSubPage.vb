Imports System.IO

Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.gdi.imageloader
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.pdf.imageloader
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.renderer.xls.imageloader
Imports jp.co.systembase.report.renderer.xlsx
Imports jp.co.systembase.report.renderer.xlsx.imageloader

Module ExampleSubPage

    Public Sub Run()
        Dim subReport As New Report(Json.Read("report\example_subpage2.rrpt"))
        subReport.Fill(New ReportDataSource(getDataTable))
        Dim subPages As ReportPages = subReport.GetPages()
        Dim report As New Report(Json.Read("report\example_subpage1.rrpt"))
        report.AddSubPages("subpage", subPages)
        report.Fill(New SubPageDataSource(subPages, "group1", "page1", "page2"))
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("output\example_subpage.pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            pages.Render(renderer)
        End Using

        Using fs As New FileStream("output\example_subpage.xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet("example_subpage")
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Using fs As New FileStream("output\example_subpage.xlsx", IO.FileMode.Create)
            Dim workbook As New XSSFWorkbook
            Dim renderer As New XlsxRenderer(workbook)
            renderer.NewSheet("example_subpage")
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        With Nothing
            Dim printer As New Printer(pages)
            Dim preview As New FmPrintPreview(printer)
            preview.StartUpZoomFit = True
            preview.ShowDialog()
        End With

    End Sub

    Private Function getDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("g1", GetType(String))
        ret.Columns.Add("g2", GetType(String))
        ret.Columns.Add("num", GetType(Decimal))
        ret.Rows.Add("A", "A1", 123)
        ret.Rows.Add("A", "A1", 456)
        ret.Rows.Add("A", "A1", 200)
        ret.Rows.Add("A", "A1", 100)
        ret.Rows.Add("A", "A1", 99)
        ret.Rows.Add("A", "A1", 88)
        ret.Rows.Add("A", "A1", 77)
        ret.Rows.Add("A", "A1", 230)
        ret.Rows.Add("A", "A2", 109)
        ret.Rows.Add("A", "A2", 10)
        ret.Rows.Add("A", "A3", 120)
        ret.Rows.Add("A", "A3", 63)
        ret.Rows.Add("A", "A4", 30)
        ret.Rows.Add("A", "A4", 97)
        ret.Rows.Add("B", "B1", 10)
        ret.Rows.Add("B", "B2", 22)
        ret.Rows.Add("B", "B2", 44)
        Return ret
    End Function


End Module
