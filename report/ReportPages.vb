Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer

Public Class ReportPages
    Inherits List(Of ReportPage)

    Public Report As Report

    Public Sub New(report As Report)
        Me.Report = report
    End Sub

    Public Sub Render(renderer As IRenderer)
        If Me.Count = 0 Then
            Throw New RenderException("ページがありません")
        End If
        Try
            Me.SetUpCountingPages()
            renderer.BeginReport(Me.Report.Design)
            For Each page As ReportPage In Me
                page.Render(renderer, Me)
            Next
            renderer.EndReport(Me.Report.Design)
        Catch ex As Exception
            If TypeOf ex Is RenderException Then
                Throw ex
            Else
                Dim message As String = "帳票のレンダリング中にエラーが発生しました"
                Throw New RenderException(message, ex)
            End If
        End Try
    End Sub

    Public Sub SetUpCountingPages()
        Dim countingPages As List(Of ReportPage) = Nothing
        For Each p As ReportPage In Me
            If countingPages Is Nothing OrElse p.ResetPageCount Then
                countingPages = New List(Of ReportPage)
            End If
            countingPages.Add(p)
            p.CountingPages = countingPages
        Next
    End Sub

End Class
