Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_5_2_SubData

    Public Overrides Function ToString() As String
        Return "5.2 サブデータ"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_2_subdata"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        Dim gp As New GroupDataProvider()
        gp.GroupDataMap.Add("sub", New ReportDataSource(_GetSubDataTable()))
        report.Fill(New ReportDataSource(_GetDataTable()), gp)

        Dim pages As ReportPages = report.GetPages()
        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function _GetDataTable() As DataTable
        Dim ret As New DataTable()
        ret.Columns.Add("key", GetType(Decimal))
        ret.Rows.Add(1)
        Return ret
    End Function

    Private Function _GetSubDataTable() As DataTable
        Dim ret As New DataTable()
        ret.Columns.Add("key", GetType(Decimal))
        ret.Rows.Add(2)
        Return ret
    End Function

End Class
