Imports System.IO
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf

Public Class Test_4_35_Wrap

    Public Overrides Function ToString() As String
        Return "4.35 改行"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_35_wrap"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance)
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            pages.Render(renderer)
        End Using

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

End Class
