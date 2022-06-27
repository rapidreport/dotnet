Imports System.IO

Imports jp.co.systembase.NPOI.HSSF.UserModel
Imports jp.co.systembase.NPOI.XSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.xlsx

Public Class Test_0_5_XLSX

    Public Overrides Function ToString() As String
        Return "0.5 1000ページ(XLSX)"
    End Function

    Public Sub Run()
        Dim name As String = "test_0_5"

        Dim sw As New Stopwatch
        sw.Start()

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.GlobalScope.Add("time", Now)
        report.GlobalScope.Add("lang", "vb")
        report.Fill(New ReportDataSource(Test_0_5_Data.GetDataTable))

        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".xlsx", IO.FileMode.Create)
            Dim workbook As New XSSFWorkbook
            Dim renderer As New XlsxRenderer(workbook)
            renderer.NewSheet(name)
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        sw.Stop()
        MessageBox.Show(sw.ElapsedMilliseconds)

    End Sub

End Class
