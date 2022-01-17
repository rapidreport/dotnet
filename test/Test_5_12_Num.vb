Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_5_12_Num

    Public Overrides Function ToString() As String
        Return "5.12 numオペレータ"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_12_num"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(_GetDataTable()))

        Dim pages As ReportPages = report.GetPages()
        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function _GetDataTable() As DataTable
        Dim ret As New DataTable()
        ret.Columns.Add("v", GetType(String))
        ret.Rows.Add("1")
        ret.Rows.Add("2")
        ret.Rows.Add("3")
        Return ret
    End Function

End Class
