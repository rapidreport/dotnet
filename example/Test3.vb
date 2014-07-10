Imports System.IO

Imports iTextSharp.text.pdf
Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.renderer.xlsx

Module Test3

    Public Sub Run()
        Dim report As New Report(Json.Read("report\test3.rrpt"))
        report.Fill(DummyDataSource.GetInstance)
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("output\test3.pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            'renderer.Setting.GaijiFont = BaseFont.CreateFont("EUDC.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED)
            pages.Render(renderer)
        End Using

        Using fs As New FileStream("output\test3.xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet("test3")
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Using fs As New FileStream("output\test3.xlsx", IO.FileMode.Create)
            Dim workbook As New XSSFWorkbook
            Dim renderer As New XlsxRenderer(workbook)
            renderer.NewSheet("test3")
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

End Module
