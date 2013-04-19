Imports System.IO

Imports NPOI.HSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.xls

Module ExampleCross

    Public Sub Run()
        Dim report As New Report(Json.Read("report\example_cross.rrpt"))
        report.Fill(New ReportDataSource(getDataTable))
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("output\example_cross.pdf", IO.FileMode.Create)
            pages.Render(New PdfRenderer(fs))
        End Using

        Using fs As New FileStream("output\example_cross.xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet("example_cross")
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function getDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("ITEM", GetType(Decimal))
        ret.Columns.Add("ITEM_NAME", GetType(String))
        ret.Columns.Add("OFFICE", GetType(Decimal))
        ret.Columns.Add("OFFICE_NAME", GetType(String))
        ret.Columns.Add("NUM", GetType(Decimal))
        ret.Rows.Add(1, "科目1", 1, "事業所1", 0)
        ret.Rows.Add(1, "科目1", 2, "事業所2", 10)
        ret.Rows.Add(1, "科目1", 3, "事業所3", 0)
        ret.Rows.Add(1, "科目1", 4, "事業所4", 100)
        ret.Rows.Add(2, "科目2", 1, "事業所1", 999)
        ret.Rows.Add(2, "科目2", 2, "事業所2", 555)
        ret.Rows.Add(2, "科目2", 3, "事業所3", 0)
        ret.Rows.Add(2, "科目2", 4, "事業所4", 0)
        ret.Rows.Add(3, "科目3", 1, "事業所1", 12345)
        ret.Rows.Add(3, "科目3", 2, "事業所2", 3456)
        ret.Rows.Add(3, "科目3", 3, "事業所3", 4567)
        ret.Rows.Add(3, "科目3", 4, "事業所4", 56789)
        Return ret
    End Function

End Module

