Imports System.IO

Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf


Public Class Test_4_31_ShrinkFontSizeStep

    Public Overrides Function ToString() As String
        Return "4.31 縮小して全体を表示のステップ変更"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_31_shrink_font_size_step"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.GlobalScope.Add("time", Now)
        report.GlobalScope.Add("lang", "vb")
        report.Fill(DummyDataSource.GetInstance)

        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & "_0.1.pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            renderer.Setting.ShrinkFontSizeStep = 0.1
            pages.Render(renderer)
        End Using
        Using fs As New FileStream("out\" & name & "_0.5.pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            renderer.Setting.ShrinkFontSizeStep = 0.5
            pages.Render(renderer)
        End Using

        With Nothing
            Dim setting As New GdiRendererSetting
            setting.ShrinkFontSizeStep = 0.1
            Dim preview As New FmPrintPreview(New Printer(pages, setting))
            preview.ShowDialog()
        End With
        With Nothing
            Dim setting As New GdiRendererSetting
            setting.ShrinkFontSizeStep = 0.5
            Dim preview As New FmPrintPreview(New Printer(pages, setting))
            preview.ShowDialog()
        End With
    End Sub

End Class

