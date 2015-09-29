Imports System.IO
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.pdf

Public Class Test_4_20_barcode

    Public Overrides Function ToString() As String
        Return "4.20 バーコード"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_20_barcode"

        Dim sw As New Stopwatch
        sw.Start()

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.GlobalScope.Add("time", Now)
        report.GlobalScope.Add("lang", "vb")
        report.Fill(DummyDataSource.GetInstance)

        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            pages.Render(renderer)
        End Using

        sw.Stop()
        MessageBox.Show(sw.ElapsedMilliseconds)

    End Sub

End Class
