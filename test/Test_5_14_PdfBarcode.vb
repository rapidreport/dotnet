Imports System.IO

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.pdf

Class Test_5_14_PdfBarcode

    Public Overrides Function ToString() As String
        Return "5.14 PDFバーコード"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_14_pdfbarcode"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance)
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            pages.Render(renderer)
        End Using
    End Sub

End Class
