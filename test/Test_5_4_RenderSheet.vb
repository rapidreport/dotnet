Imports System.IO
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.renderer.xlsx
Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel

Public Class Test_5_4_RenderSheet

    Public Overrides Function ToString() As String
        Return "5.4 シート出力"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_4_rendersheet"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(_GetDataTable()))

        Using fs As New FileStream("out\" & name & ".xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet(name)
            renderer.RenderSheet(report)
            workbook.Write(fs)
        End Using

        Using fs As New FileStream("out\" & name & ".xlsx", IO.FileMode.Create)
            Dim workbook As New XSSFWorkbook
            Dim renderer As New XlsxRenderer(workbook)
            renderer.NewSheet(name)
            renderer.RenderSheet(report)
            workbook.Write(fs)
        End Using

        Dim pages = report.GetPages()
        Using fs As New FileStream("out\" & name & "_pages.xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet(name)
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Using fs As New FileStream("out\" & name & "_pages.xlsx", IO.FileMode.Create)
            Dim workbook As New XSSFWorkbook
            Dim renderer As New XlsxRenderer(workbook)
            renderer.NewSheet(name)
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

    End Sub

    Private Function _GetDataTable() As DataTable
        Dim ret As New DataTable()
        ret.Columns.Add("col1", GetType(String))
        ret.Columns.Add("col2", GetType(Date))
        ret.Columns.Add("col3", GetType(Decimal))
        ret.Columns.Add("col4", GetType(Decimal))
        Dim d = New Date(2020, 1, 1)
        For i = 0 To 300
            ret.Rows.Add("行" & i, d.AddDays(i), i * 50, i * 15 + i * 0.01)
        Next
        Return ret
    End Function

End Class
