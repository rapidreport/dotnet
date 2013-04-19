Imports System.IO

Imports NPOI.HSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.customizer

Module ExampleRegion

    Public Sub Run()
        Dim design As New ReportDesign(Json.Read("report\example_region.rrpt"))
        design.PaperDesign.Margin.Left = 50
        Dim report As New Report(design, New Customizer)
        report.Fill(New ReportDataSource(getDataTable))
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("output\example_region.pdf", IO.FileMode.Create)
            pages.Render(New PdfRenderer(fs))
        End Using

        Using fs As New FileStream("output\example_region.xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet("example_region")
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function getDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("HEIGHT", GetType(Decimal))
        ret.Rows.Add(20)
        ret.Rows.Add(30)
        ret.Rows.Add(40)
        ret.Rows.Add(50)
        ret.Rows.Add(60)
        ret.Rows.Add(70)
        ret.Rows.Add(80)
        ret.Rows.Add(90)
        ret.Rows.Add(100)
        Return ret
    End Function

    Private Class Customizer
        Inherits DefaultCustomizer
        Public Overrides Function ContentRegion( _
          ByVal content As Content, _
          ByVal evaluator As Evaluator, _
          ByVal region As Region) As Region
            If "content_example".Equals(content.Design.Id) Then
                Dim ret As New Region(region)
                ret.SetHeight(evaluator.EvalTry(".HEIGHT"))
                Return ret
            Else
                Return region
            End If
        End Function
    End Class

End Module