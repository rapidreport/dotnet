﻿Imports System.IO

Imports jp.co.systembase.NPOI.HSSF.UserModel
Imports jp.co.systembase.NPOI.XSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.renderer.xlsx

Public Class Test_4_21_LocateCount

    Public Overrides Function ToString() As String
        Return "4.21 絶対座標配置(並べる数)"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_21_locate_count"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(getDataTable))
        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            pages.Render(renderer)
        End Using

        Using fs As New FileStream("out\" & name & ".xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet(name)
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Using fs As New FileStream("out\" & name & ".xlsx", IO.FileMode.Create)
            Dim workbook As New XSSFWorkbook
            Dim renderer As New XlsxRenderer(workbook)
            renderer.NewSheet(name)
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function getDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("no", GetType(String))
        ret.Rows.Add("010")
        ret.Rows.Add("020")
        ret.Rows.Add("030")
        ret.Rows.Add("040")
        ret.Rows.Add("050")
        ret.Rows.Add("060")
        ret.Rows.Add("060")
        ret.Rows.Add("070")
        Return ret
    End Function

End Class
