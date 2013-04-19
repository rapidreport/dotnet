Imports System.IO

Imports NPOI.HSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.customizer

Module ExamplePage

    Public Sub Run()
        Dim pages As ReportPages
        With Nothing
            Dim report As New Report(Json.Read("report\example_page1.rrpt"), New Customizer)
            report.Fill(New ReportDataSource(getDataTable))
            pages = report.GetPages()
        End With
        With Nothing
            Dim report As New Report(Json.Read("report\example_page3.rrpt"))
            report.Fill(DummyDataSource.GetInstance)
            pages.AddRange(report.GetPages)
        End With

        Using fs As New FileStream("output\example_page.pdf", IO.FileMode.Create)
            pages.Render(New PdfRenderer(fs))
        End Using

        Using fs As New FileStream("output\example_page.xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet("example_page")
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Dim printer As New Printer(pages)
        printer.DynamicPageSetting = True
        Dim preview As New FmPrintPreview(printer)
        preview.ShowDialog()
    End Sub

    Private Function getDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("GROUP_CD", GetType(String))
        ret.Columns.Add("DATA", GetType(String))
        ret.Rows.Add("A", "A-1")
        ret.Rows.Add("A", "A-2")
        ret.Rows.Add("A", "A-3")
        ret.Rows.Add("A", "A-4")
        ret.Rows.Add("A", "A-5")
        ret.Rows.Add("A", "A-6")
        ret.Rows.Add("A", "A-7")
        ret.Rows.Add("A", "A-8")
        ret.Rows.Add("A", "A-9")
        ret.Rows.Add("A", "A-10")
        ret.Rows.Add("A", "A-11")
        ret.Rows.Add("A", "A-12")
        ret.Rows.Add("A", "A-13")
        ret.Rows.Add("A", "A-14")
        ret.Rows.Add("A", "A-15")
        ret.Rows.Add("A", "A-16")
        ret.Rows.Add("A", "A-17")
        ret.Rows.Add("A", "A-18")
        ret.Rows.Add("A", "A-19")
        ret.Rows.Add("A", "A-20")
        ret.Rows.Add("B", "B-1")
        ret.Rows.Add("B", "B-2")
        ret.Rows.Add("B", "B-3")
        ret.Rows.Add("B", "B-4")
        ret.Rows.Add("B", "B-5")
        ret.Rows.Add("B", "B-6")
        ret.Rows.Add("B", "B-7")
        ret.Rows.Add("B", "B-8")
        ret.Rows.Add("B", "B-9")
        ret.Rows.Add("B", "B-10")
        Return ret
    End Function

    Private Class Customizer
        Inherits DefaultCustomizer
        Private reportDesign As ReportDesign
        Public Sub New()
            Me.reportDesign = New ReportDesign(Json.Read("report\example_page2.rrpt"))
        End Sub
        Public Overrides Sub PageAdded( _
          ByVal report As Report, _
          ByVal pages As ReportPages, _
          ByVal page As ReportPage)
            Dim g As Group = page.FindFinishedGroup("group_example")
            If g IsNot Nothing Then
                Dim _report As New Report(Me.reportDesign)
                _report.Fill(g.Data)
                pages.AddRange(_report.GetPages)
            End If
        End Sub
    End Class

End Module