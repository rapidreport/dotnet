Imports System.IO
Imports iTextSharp.text.pdf
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.pdf

Public Class Test_4_35_PdfVertical

    Public Overrides Function ToString() As String
        Return "4.35 Pdf縦書き"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_35_pdf_vertical"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance)
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            pages.Render(renderer)
        End Using

    End Sub

End Class
