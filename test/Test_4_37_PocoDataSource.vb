Imports System.IO
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer.gdi

Public Class Test_4_37_PocoDataSource

    Public Overrides Function ToString() As String
        Return "4.37 Pocoデータソース"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_37_poco_datasource"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(GetData()))

        Dim pages As ReportPages = report.GetPages()

        Dim preview As New FmPrintPreview(New Printer(pages))
        preview.ShowDialog()
    End Sub

    Private Function GetData() As IList
        Dim ret As New ArrayList
        For i As Integer = 1 To 10
            Dim r As New Data
            r.Hoge = "ほげ" & i
            r.Fuga = "ふが" & i
            r.FooBar = "ふーばー" & i
            r.BarFoo = "ばーふー" & i
            r.Num1 = 10 * i
            r.Num2 = 100 * i
            r.Date1 = (New Date(2000, 1, 1)).AddDays(i)
            r.Date2 = (New Date(2010, 1, 1)).AddDays(i)
            ret.Add(r)
        Next
        Return ret
    End Function

    Private Class Data
        Public Property Hoge As String
        Public Fuga As String
        Public Property FooBar As String
        Public BarFoo As String
        Public Property Num1 As Decimal
        Public Num2 As Decimal
        Public Property Date1 As Date
        Public Date2 As Date
    End Class

End Class
