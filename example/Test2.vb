Imports System.IO

Imports NPOI.HSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.xls

Module Test2

    Public Sub Run()
        Dim report As New Report(Json.Read("report\test2.rrpt"))
        report.Fill(New ReportDataSource(getDataTable))
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("output\test2.pdf", IO.FileMode.Create)
            pages.Render(New PdfRenderer(fs))
        End Using

        Using fs As New FileStream("output\test2.xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet("test2")
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function getDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("CODE39", GetType(String))
        ret.Columns.Add("CODE128", GetType(String))
        ret.Columns.Add("EAN8", GetType(String))
        ret.Columns.Add("EAN13", GetType(String))
        ret.Columns.Add("CODABAR", GetType(String))
        ret.Columns.Add("QRCODE", GetType(String))
        ret.Rows.Add( _
          "12345678", _
          "12345abcABC", _
          "8765432", _
          "321098765432", _
          "12345678", _
          "株式会社システムベース: http://www.systembase.co.jp/")
        Return ret
    End Function

End Module

