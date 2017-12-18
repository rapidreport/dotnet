Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_4_34_DynamicContentSize

    Public Overrides Function ToString() As String
        Return "4.34 動的コンテントサイズ"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_34_dynamic_content_size"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(GetDataTable))
        Dim pages As ReportPages = report.GetPages()

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.StartUpZoomFit = True
        preview.ShowDialog()
    End Sub

    Private Function GetDataTable() As DataTable
        Dim ret As New DataTable()
        ret.Columns.Add("text1", GetType(String))
        ret.Columns.Add("text2", GetType(String))
        ret.Columns.Add("h", GetType(Decimal))
        ret.Rows.Add("1111" & vbCrLf & "2222" & vbCrLf & "3333", "1111" & vbCrLf & "2222", 40)
        ret.Rows.Add("1111" & vbCrLf & "2222", "1111" & vbCrLf & "2222" & vbCrLf & "3333", 60)
        Return ret
    End Function

End Class

