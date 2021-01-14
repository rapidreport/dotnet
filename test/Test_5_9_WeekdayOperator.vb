Imports System.IO
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf

Public Class Test_5_9_WeekdayOperator

    Public Overrides Function ToString() As String
        Return "5.9 オペレータ曜日"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_9_weekdayoperator"
        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(getDataTable))

        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            pages.Render(renderer)
        End Using

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function getDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("v", GetType(Date))
        ret.Columns.Add("answer", GetType(Integer))

        Dim day As Date = Date.Today
        For i As Integer = 0 To 6
            ret.Rows.Add(day, day.DayOfWeek)
            day = day.AddDays(1)
        Next
        Return ret
    End Function

End Class
