Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_5_12_TextMock

    Public Overrides Function ToString() As String
        Return "5.12 Text要素のMock"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_12_textmock"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance())

        Dim pages As ReportPages = report.GetPages()
        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

End Class
