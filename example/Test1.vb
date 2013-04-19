Imports System.IO

Imports NPOI.HSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.xls

Module Test1

    Public Sub Run()
        Dim sw As New Stopwatch
        sw.Start()
        Dim report As New Report(Json.Read("report\test1.rrpt"))
        report.Fill(New ReportDataSource(getDataTable))
        Debug.Print("fill: " & sw.ElapsedMilliseconds)
        Dim pages As ReportPages = report.GetPages()
        'Debug.Print("paging: " & sw.ElapsedMilliseconds)
        'Using fs As New FileStream("output\test1.pdf", IO.FileMode.Create)
        '    pages.Render(New CPdfRenderer(fs))
        'End Using
        'Debug.Print("render: " & sw.ElapsedMilliseconds)
        'Using fs As New FileStream("output\test1.xls", IO.FileMode.Create)
        '    Dim workbook As New HSSFWorkbook
        '    Dim renderer As New CXlsRenderer(workbook, "test1")
        '    pages.Render(renderer)
        '    workbook.Write(fs)
        'End Using
        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function getDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("ID1", GetType(Decimal))
        ret.Columns.Add("ID2", GetType(Decimal))
        ret.Columns.Add("ID3", GetType(Decimal))
        ret.Columns.Add("NUM", GetType(Decimal))
        ret.Columns.Add("TXT1", GetType(String))
        ret.Columns.Add("TXT2", GetType(String))
        For i1 As Integer = 1 To 10
            For i2 As Integer = 1 To 10
                For i3 As Integer = 1 To 100
                    Dim t As String = i1 & i2 & i3
                    ret.Rows.Add(i1, i2, i3, i3, _
                                 "hogehogehogehogehogehogehogehogehogehoge" & t, _
                                 "fugafugafugafugafugafugafugafugafugafuga" & t)
                Next
            Next
        Next
        Return ret
    End Function

End Module

