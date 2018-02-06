Imports System.IO
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.xlsx
Imports NPOI.XSSF.UserModel

Public Class Test_4_35_XlsxColor

    Public Overrides Function ToString() As String
        Return "4.35 Xlsx色設定"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_35_xlsx_color"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance)
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".xlsx", IO.FileMode.Create)
            Dim workbook As New XSSFWorkbook
            Dim renderer As New XlsxRenderer(workbook)
            renderer.NewSheet(name)
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

    End Sub

End Class
