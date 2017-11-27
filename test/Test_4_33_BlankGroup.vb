Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_4_33_BlankGroup

    Public Overrides Function ToString() As String
        Return "4.33 ブランク行"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_33_blank_group"
        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance)

        Dim pages As ReportPages = report.GetPages()
        With Nothing
            Dim printer As New Printer(pages)
            Dim preview As New FmPrintPreview(printer)
            preview.ShowDialog()
        End With

    End Sub

End Class
