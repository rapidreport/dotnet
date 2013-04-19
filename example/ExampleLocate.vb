Imports System.IO

Imports NPOI.HSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.xls

Module ExampleLocate

    Public Sub Run()
        Dim report As New Report(Json.Read("report\example_locate.rrpt"))
        report.Fill(New ReportDataSource(getDataTable))
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("output\example_locate.pdf", IO.FileMode.Create)
            pages.Render(New PdfRenderer(fs))
        End Using

        Using fs As New FileStream("output\example_locate.xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet("example_locate")
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function getDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("DATA", GetType(String))
        ret.Rows.Add("データ1")
        ret.Rows.Add("データ2")
        ret.Rows.Add("データ3")
        ret.Rows.Add("データ4")
        ret.Rows.Add("データ5")
        Return ret
    End Function

End Module
