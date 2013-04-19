Imports System.IO

Imports NPOI.HSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.customizer

Module ExampleRender

    Public Sub Run()
        Dim report As New Report(Json.Read("report\example_render.rrpt"), New Customizer)
        report.Fill(New ReportDataSource(getDataTable))
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("output\example_render.pdf", IO.FileMode.Create)
            pages.Render(New PdfRenderer(fs))
        End Using

        Using fs As New FileStream("output\example_render.xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet("example_render")
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function getDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("NUM", GetType(Decimal))
        ret.Rows.Add(50)
        ret.Rows.Add(40)
        ret.Rows.Add(30)
        ret.Rows.Add(20)
        ret.Rows.Add(10)
        ret.Rows.Add(0)
        ret.Rows.Add(-10)
        ret.Rows.Add(-20)
        ret.Rows.Add(-30)
        ret.Rows.Add(-40)
        ret.Rows.Add(-50)
        Return ret
    End Function

    Private Class Customizer
        Inherits DefaultCustomizer
        Public Overrides Sub RenderContent( _
          ByVal content As Content, _
          ByVal evaluator As Evaluator, _
          ByVal region As Region, _
          ByVal elementDesigns As ElementDesigns)
            If "content_example".Equals(content.Design.Id) Then
                Dim e As ElementDesign = elementDesigns.Find("graph")
                Dim num As Decimal = evaluator.EvalTry(".NUM")
                If num >= 0 Then
                    e.Child("layout").Put("x1", 100)
                    e.Child("layout").Put("x2", 100 + num)
                    e.Put("fill_color", "lightblue")
                Else
                    e.Child("layout").Put("x1", 100 + num)
                    e.Child("layout").Put("x2", 100)
                    e.Put("fill_color", "pink")
                End If
            End If
        End Sub
    End Class

End Module
