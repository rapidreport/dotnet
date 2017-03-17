Imports System.IO
Imports iTextSharp.text.pdf
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.pdf

Public Class Test_4_29_Gaiji

    Public Overrides Function ToString() As String
        Return "4.29 特定フォントにリンクした外字"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_29_gaiji"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance)

        Dim pages As ReportPages = report.GetPages()
        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            renderer.Setting.GaijiFont = BaseFont.CreateFont("rrpt\font\eudc.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED)
            renderer.Setting.GaijiFontMap.Add("gothic", BaseFont.CreateFont("rrpt\font\msgothic.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED))
            pages.Render(renderer)
        End Using

    End Sub

End Class
