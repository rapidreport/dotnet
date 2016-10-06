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

Public Class Test_4_26_FieldExistCheck
    Public Overrides Function ToString() As String
        Return "4.26 存在しないフィールドを参照したらエラー"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_26_field_exist_check"

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
        ret.Rows.Add("正常に文字列が出力 頁1")
        ret.Rows.Add("正常に文字列が出力 頁2")
        Return ret
    End Function

    Public Class Logger
        Implements IReportLogger
        Public ErrorStringBuilder As New StringBuilder
        Public Sub EvaluateError(exp As String, ex As EvalException) Implements IReportLogger.EvaluateError
            ErrorStringBuilder.AppendLine(ex.Message)
            If ex.InnerException IsNot Nothing Then
                ErrorStringBuilder.AppendLine(" " & ex.InnerException.GetType.ToString & ":" & ex.InnerException.Message)
            End If
        End Sub
        Public Sub ElementRenderingError(contentDesign As ContentDesign, design As ElementDesign, ex As Exception) Implements IReportLogger.ElementRenderingError
        End Sub
    End Class

End Class

