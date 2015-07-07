Imports System.IO

Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.xls

Public Class Test_0_5_XLS
    Implements ITest

    Public Overrides Function ToString() As String
        Return "0.5 1000ページ(XLS)"
    End Function

    Public Sub Run() Implements ITest.Run
        Dim name As String = "test_0_5"

        Dim sw As New Stopwatch
        sw.Start()

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.GlobalScope.Add("time", Now)
        report.GlobalScope.Add("lang", "vb")
        report.Fill(New ReportDataSource(Test_0_5_Data.GetDataTable))

        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet(name)
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        sw.Stop()
        MessageBox.Show(sw.ElapsedMilliseconds)

    End Sub

End Class
