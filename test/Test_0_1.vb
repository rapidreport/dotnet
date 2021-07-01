Imports System.IO

Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.renderer.xlsx
Imports iTextSharp.text.pdf

Public Class Test_0_1

    Public Overrides Function ToString() As String
        Return "0.1 基本機能"
    End Function

    Public Sub Run()
        Dim name As String = "test_0_1"

        Dim fontPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Fonts)

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.GlobalScope.Add("time", Now)
        report.GlobalScope.Add("lang", "vb")
        report.Fill(DummyDataSource.GetInstance)

        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            renderer.Setting.FontMap.Add("meiryo", BaseFont.CreateFont(fontPath & "\\meiryo.ttc,0", BaseFont.IDENTITY_H, BaseFont.EMBEDDED))
            pages.Render(renderer)
        End Using

        Using fs As New FileStream("out\" & name & ".xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.Setting.FontMap.Add("meiryo", "メイリオ")
            renderer.NewSheet(name)
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Using fs As New FileStream("out\" & name & ".xlsx", IO.FileMode.Create)
            Dim workbook As New XSSFWorkbook
            Dim renderer As New XlsxRenderer(workbook)
            renderer.Setting.FontMap.Add("meiryo", "メイリオ")
            renderer.NewSheet(name)
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Dim printer As New Printer(pages)
        printer.Setting.FontMap.Add("meiryo", "メイリオ")
        Dim preview As New FmPrintPreview(printer)
        preview.ShowDialog()
    End Sub

End Class
