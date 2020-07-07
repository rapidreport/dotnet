Imports System.IO
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.renderer.xlsx
Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel

Public Class Test_5_7_IsNumeric

    Public Overrides Function ToString() As String
        Return "5.7 IsNumeric"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_7_isnumeric"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(_GetDataTable()))

        Dim pages As ReportPages = report.GetPages()
        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function _GetDataTable() As DataTable
        Dim ret As New DataTable()

        ret.Columns.Add("vInteger", GetType(Integer))
        ret.Columns.Add("vUInteger", GetType(UInteger))

        ret.Columns.Add("vLong", GetType(Long))
        ret.Columns.Add("vULong", GetType(ULong))

        ret.Columns.Add("vSingle", GetType(Single))
        ret.Columns.Add("vDouble", GetType(Double))

        ret.Columns.Add("vDecimal", GetType(Decimal))

        ret.Columns.Add("vSByte", GetType(SByte))
        ret.Columns.Add("vByte", GetType(Byte))

        ret.Columns.Add("vShort", GetType(Short))
        ret.Columns.Add("vUShort", GetType(UShort))

        ret.Rows.Add(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
        ret.Rows.Add(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)

        Return ret
    End Function

End Class
