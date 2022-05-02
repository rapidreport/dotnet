Imports System.IO

Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.renderer.xlsx
Imports iTextSharp.text.pdf

Class Test_5_13_Unit

    Public Overrides Function ToString() As String
        Return "5.13 集計単位"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_13_unit"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance)
        Dim pages As ReportPages = report.GetPages()

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

End Class
