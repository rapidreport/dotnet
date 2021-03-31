Imports System.IO
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf

Public Class Test_5_10_BackSlash

    Public Overrides Function ToString() As String
        Return "5.10 バックスラッシュ"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_10_backslash"
        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(getDataTable))

        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim setting As New PdfRendererSetting
            setting.ReplaceBackslashToYen = True
            Dim renderer As New PdfRenderer(fs, setting)
            pages.Render(renderer)
        End Using
    End Sub

    Private Function getDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("v", GetType(String))

        ret.Rows.Add("1234567890\234567890")
        ret.Rows.Add("1234567\90\234567890")
        ret.Rows.Add("1234567\90\23456\890")
        ret.Rows.Add("\1\2\3\4\5\6\7\8\9\0111")
        ret.Rows.Add("123456789012345678901234567890")

        Return ret
    End Function

End Class
