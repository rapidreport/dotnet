Imports System.IO
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.pdf

Public Class Test_4_20_FormatNull

    Public Overrides Function ToString() As String
        Return "4.20 Null値に対するFormat"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_20_format_null"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(GetDataTable))

        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            pages.Render(renderer)
        End Using

    End Sub

    Private Function GetDataTable() As DataTable
        Dim ret As New DataTable()
        ret.Columns.Add("code", GetType(String))
        ret.Rows.Add("11111")
        ret.Rows.Add("22222")
        ret.Rows.Add("33333")
        Return ret
    End Function

End Class

