Imports System.IO
Imports iTextSharp.text.pdf
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.renderer.xlsx
Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel

Public Class Test_4_36_SurrogatePair

    Public Overrides Function ToString() As String
        Return "4.36 サロゲートペア"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_36_surrogate_pair"

        Dim fontPath As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Fonts)

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance)
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim setting As PdfRendererSetting = New PdfRendererSetting()
            setting.FontMap("ipamjm") = BaseFont.CreateFont(fontPath & "\ipamjm.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED)
            Dim renderer As PdfRenderer = New PdfRenderer(fs, setting)
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

        With Nothing
            Dim setting As New GdiRendererSetting
            setting.FontMap.Add("ipamjm", "IPAmj明朝")
            Dim preview As New FmPrintPreview(New Printer(pages, setting))
            preview.ShowDialog()
        End With
    End Sub

End Class
