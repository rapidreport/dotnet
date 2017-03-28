
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_4_29_Background

    Public Overrides Function ToString() As String
        Return "4.29 プレビュー時の背景画像"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_29_background"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance)

        Dim pages As ReportPages = report.GetPages()
        With Nothing
            Dim printer As New Printer(pages)
            printer.PreviewBackgroundImage = Image.FromFile("./img/background.png")
            Dim preview As New FmPrintPreview(printer)
            preview.ShowDialog()
        End With
    End Sub

End Class