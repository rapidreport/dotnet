Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer.gdi.imageloader

Public Class GdiRenderer
    Implements IRenderer

    Public Env As RenderingEnv

    Public Sub New(env As RenderingEnv)
        Me.Env = env
    End Sub

    Public Sub BeginReport(reportDesign As ReportDesign) Implements IRenderer.BeginReport
    End Sub

    Public Sub BeginPage(reportDesign As ReportDesign, pageIndex As Integer, paperRegion As Region) Implements IRenderer.BeginPage
    End Sub

    Public Sub EndReport(reportDesign As ReportDesign) Implements IRenderer.EndReport
    End Sub

    Public Sub EndPage(reportDesign As ReportDesign) Implements IRenderer.EndPage
    End Sub

    Public Sub RenderElement( _
      reportDesign As ReportDesign, _
      region As Region, _
      design As ElementDesign, _
      data As Object) Implements IRenderer.RenderElement
        Me.Env.Setting.GetElementRenderer(design.Get("type")) _
          .Render(Me.Env, reportDesign, region, design, data)
    End Sub

End Class
