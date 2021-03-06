﻿Imports System.IO
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.pdf

Public Class Test_4_36_GroupsSortKey
    Public Overrides Function ToString() As String
        Return "4.36 GroupDataProviderとソートキー"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_36_groups_sortkey"

        Dim setting As New ReportSetting

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))

        Dim provider As New GroupDataProvider
        provider.GroupDataMap.Add("group_id", New ReportDataSource(GetGroupDataTable))

        report.Fill(New ReportDataSource(GetDataTable), provider)

        Dim pages As ReportPages = report.GetPages()
        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            pages.Render(renderer)
        End Using
    End Sub

    Private Function GetDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("group_key", GetType(String))
        ret.Rows.Add("b")
        ret.Rows.Add("a")
        Return ret
    End Function

    Private Function GetGroupDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("group_key", GetType(String))
        ret.Columns.Add("value", GetType(String))
        ret.Rows.Add("b", "b-2")
        ret.Rows.Add("b", "b-1")
        ret.Rows.Add("a", "a-1")
        ret.Rows.Add("a", "a-3")
        ret.Rows.Add("a", "a-2")
        Return ret
    End Function

End Class
