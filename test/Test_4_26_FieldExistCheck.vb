Imports System.IO

Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.renderer.xlsx

Imports iTextSharp.text.pdf

Public Class Test_4_26_FieldExistCheck
    Public Overrides Function ToString() As String
        Return "4.26 存在しないフィールドを参照したらエラー"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_26_field_exist_check"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(GetDataTable))
        Dim pages As ReportPages = report.GetPages()

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Public Function GetDataTable() As DataTable
        Dim ret As New DataTable()
        ret.Columns.Add("field1", GetType(String))
        ret.Rows.Add("正常に文字列が出力")
        Return ret
    End Function

End Class
