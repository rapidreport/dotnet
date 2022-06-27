Imports System.IO
Imports iTextSharp.text.pdf
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.renderer.xlsx
Imports jp.co.systembase.NPOI.HSSF.UserModel
Imports jp.co.systembase.NPOI.XSSF.UserModel

Public Class Test_4_36_BoldUnderline

    Public Overrides Function ToString() As String
        Return "4.36 PDFで太字下線の不具合修正"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_36_bold_underline"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance)
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As PdfRenderer = New PdfRenderer(fs)
            pages.Render(renderer)
        End Using
    End Sub

End Class
