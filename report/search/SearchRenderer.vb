Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.search.searchobject
Imports jp.co.systembase.report.renderer

Namespace search
    Public Class SearchRenderer
        Implements IRenderer

        Public Class Result
            Public ElementDesign As ElementDesign
            Public Region As Region
        End Class

        Public Keyword As String
        Public Results As New List(Of Result)

        Public Sub New(keyword As String)
            Me.Keyword = keyword
        End Sub

        Public Sub BeginPage(reportDesign As ReportDesign, pageIndex As Integer, paperRegion As Region) Implements IRenderer.BeginPage
        End Sub
        Public Sub BeginReport(reportDesign As ReportDesign) Implements IRenderer.BeginReport
        End Sub
        Public Sub EndPage(reportDesign As ReportDesign) Implements IRenderer.EndPage
        End Sub
        Public Sub EndReport(reportDesign As ReportDesign) Implements IRenderer.EndReport
        End Sub

        Public Sub RenderElement(reportDesign As ReportDesign, region As Region, design As ElementDesign, data As Object) Implements IRenderer.RenderElement
            If design.Get("id") = "__trial__" Then
                Exit Sub
            End If
            Dim searchObject As ISearchObject = reportDesign.Setting.GetSearchObject(design.Get("type"))
            Dim t As String = searchObject.GetText(reportDesign, design, data)
            If t IsNot Nothing AndAlso t.ToLower.IndexOf(Me.Keyword.ToLower) >= 0 Then
                Dim r As New Result
                r.ElementDesign = design
                r.Region = region
                Me.Results.Add(r)
            End If
        End Sub

    End Class
End Namespace

