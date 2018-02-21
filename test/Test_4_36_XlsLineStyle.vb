Imports System.IO
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.renderer.xlsx
Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel

Public Class Test_4_36_XlsLineStyle

    Public Overrides Function ToString() As String
        Return "4.36 Excel線スタイル"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_36_xls_line_style"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance)
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet(name)
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Using fs As New FileStream("out\" & name & ".xlsx", IO.FileMode.Create)
            Dim workbook As New XSSFWorkbook
            Dim renderer As New XlsxRenderer(workbook)
            renderer.NewSheet(name)
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

    End Sub

End Class
