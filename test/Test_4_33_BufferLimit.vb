Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_4_33_BufferLimit

    Public Overrides Function ToString() As String
        Return "4.33 バッファサイズのリミット"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_33_buffer_limit"

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
