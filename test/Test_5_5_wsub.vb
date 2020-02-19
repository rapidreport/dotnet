Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_5_5_Wsub

    Public Overrides Function ToString() As String
        Return "5.5 wsub"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_5_wsub"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance())

        Dim pages As ReportPages = report.GetPages()
        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

End Class
