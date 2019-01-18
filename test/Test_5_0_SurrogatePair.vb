Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_5_0_SurrogatePair

    Public Overrides Function ToString() As String
        Return "5.0 サロゲートペア"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_0_surrogate_pair"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance())

        Dim pages As ReportPages = report.GetPages()

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub


End Class
