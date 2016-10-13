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
Imports System.Text

Public Class Test_4_26_UnknownField
    Public Overrides Function ToString() As String
        Return "4.26 不明な列エラー"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_26_unknownfield"

        Dim setting As New ReportSetting
        Dim logger As New Logger
        setting.Logger = logger

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"), setting)
        report.Fill(New ReportDataSource(GetDataTable))
        Dim pages As ReportPages = report.GetPages()

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()

        MessageBox.Show(logger.ErrorStringBuilder.ToString)
    End Sub

    Public Function GetDataTable() As DataTable
        Dim ret As New DataTable()
        ret.Columns.Add("field1", GetType(String))
        ret.Rows.Add("ページ1")
        ret.Rows.Add("ページ2")
        Return ret
    End Function

    Public Class Logger
        Implements IReportLogger
        Public ErrorStringBuilder As New StringBuilder
        Public Sub EvaluateError(exp As String, ex As EvalException) Implements IReportLogger.EvaluateError
        End Sub
        Public Sub ElementRenderingError(contentDesign As ContentDesign, design As ElementDesign, ex As Exception) Implements IReportLogger.ElementRenderingError
        End Sub
        Public Sub UnknownFieldError(ex As UnknownFieldException) Implements IReportLogger.UnknownFieldError
            ErrorStringBuilder.AppendLine(ex.Message)
        End Sub
    End Class

End Class
