Imports System.IO
Imports iTextSharp.text.pdf
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf

Public Class Test_5_6_CharSpacing

    Public Overrides Function ToString() As String
        Return "5.6 文字間隔(+縦寄せ)"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_6_charspacing"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance())

        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            renderer.Setting.FontMap("gothic") = BaseFont.CreateFont("C:\Windows\Fonts\msgothic.ttc,0", BaseFont.IDENTITY_H, BaseFont.EMBEDDED)
            pages.Render(renderer)
        End Using

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

End Class
