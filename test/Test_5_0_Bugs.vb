﻿Imports System.IO
Imports iTextSharp.text.pdf
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf

Public Class Test_5_0_Bugs

    Public Overrides Function ToString() As String
        Return "5.0 バグ"
    End Function

    Public Sub Run()
        Dim name As String = "test_5_0_bugs"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(DummyDataSource.GetInstance())

        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            pages.Render(renderer)
        End Using

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()

    End Sub

End Class
