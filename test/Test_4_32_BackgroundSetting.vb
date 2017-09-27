Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_4_32_BackgroundSetting

    Public Overrides Function ToString() As String
        Return "4.32 プレビュー時の背景画像"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_32_background_setting"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance)

        Dim pages As ReportPages = report.GetPages()
        With Nothing
            Dim printer As New Printer(pages)
            printer.PreviewBackgroundImage = Image.FromFile("./img/background_setting.png")
            printer.PreviewBackgroundSetting.Alpha = 0.2
            printer.PreviewBackgroundSetting.Scale = 2
            printer.PreviewBackgroundSetting.X = 20
            printer.PreviewBackgroundSetting.Y = 50

            Dim preview As New FmPrintPreview(printer)
            preview.ShowDialog()
        End With
    End Sub

End Class