Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_5_19_Sub_Variables

    Public Overrides Function ToString() As String
        Return "5.19 sub/変数"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_19_sub_variables"

        ' jp.co.systembase.report.Report.Compatibility._5_18_Sub = True

        Dim Report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        Report.Fill(DummyDataSource.GetInstance())

        Dim pages As ReportPages = report.GetPages()
        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

End Class
