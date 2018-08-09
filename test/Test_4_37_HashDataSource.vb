Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_4_37_HashDataSource

    Public Overrides Function ToString() As String
        Return "4.37 Hashtableデータソース"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_37_hash_datasource"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(GetData()))

        Dim pages As ReportPages = report.GetPages()

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function GetData() As IList
        Dim ret As New ArrayList
        For i As Integer = 1 To 10
            Dim r As New Hashtable
            r("hoge") = "ほげ" & i
            r("fuga") = "ふが" & i
            r("num1") = 10 * i
            r("num2") = 100 * i
            r("Date1") = (New Date(2000, 1, 1)).AddDays(i)
            r("Date2") = (New Date(2010, 1, 1)).AddDays(i)
            ret.Add(r)
        Next
        Return ret
    End Function

End Class
