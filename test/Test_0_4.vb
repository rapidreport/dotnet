Imports System.IO
Imports System.Drawing

Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.gdi.imageloader
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.pdf.imageloader
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.renderer.xls.imageloader
Imports jp.co.systembase.report.renderer.xlsx
Imports jp.co.systembase.report.renderer.xlsx.imageloader

Public Class Test_0_4
    Implements ITest

    Public Overrides Function ToString() As String
        Return "0.4 画像"
    End Function

    Private Function _GetImageMap() As ImageMap
        Dim map As New ImageMap()
        map.Add(1D, Image.FromFile("./img/logo.bmp"))
        map.Add(2D, Image.FromFile("./img/logo.gif"))
        map.Add(3D, Image.FromFile("./img/logo.jpg"))
        map.Add(4D, Image.FromFile("./img/logo.png"))
        Return map
    End Function

    Public Sub Run() Implements ITest.Run
        Dim name As String = "test_0_4"

        Dim imageMap As ImageMap = Me._GetImageMap()

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.GlobalScope.Add("time", Now)
        report.GlobalScope.Add("lang", "vb")
        report.Fill(DummyDataSource.GetInstance)

        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            renderer.ImageLoaderMap.Add("image", New PdfImageLoader(imageMap))
            pages.Render(renderer)
        End Using

        Using fs As New FileStream("out\" & name & ".xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet(name)
            renderer.ImageLoaderMap.Add("image", New XlsImageLoader(imageMap))
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Using fs As New FileStream("out\" & name & ".xlsx", IO.FileMode.Create)
            Dim workbook As New XSSFWorkbook
            Dim renderer As New XlsxRenderer(workbook)
            renderer.NewSheet(name)
            renderer.ImageLoaderMap.Add("image", New XlsxImageLoader(imageMap))
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        With Nothing
            Dim printer As New Printer(pages)
            printer.ImageLoaderMap.Add("image", New GdiImageLoader(imageMap))
            Dim preview As New FmPrintPreview(printer)
            preview.ShowDialog()
        End With

    End Sub

End Class