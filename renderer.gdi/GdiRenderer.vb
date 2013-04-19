Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer.gdi.imageloader

Public Class GdiRenderer
    Implements IRenderer

    Public Env As RenderingEnv

    Public Sub New(ByVal env As RenderingEnv)
        Me.Env = env
    End Sub

    Public Sub BeginReport(ByVal reportDesign As ReportDesign) Implements IRenderer.BeginReport
    End Sub

    Public Sub BeginPage(ByVal reportDesign As ReportDesign, ByVal pageIndex As Integer, ByVal paperRegion As Region) Implements IRenderer.BeginPage
    End Sub

    Public Sub EndReport(ByVal reportDesign As ReportDesign) Implements IRenderer.EndReport
    End Sub

    Public Sub EndPage(ByVal reportDesign As ReportDesign) Implements IRenderer.EndPage
    End Sub

    Public Sub RenderElement( _
      ByVal reportDesign As ReportDesign, _
      ByVal region As Region, _
      ByVal design As ElementDesign, _
      ByVal data As Object) Implements IRenderer.RenderElement
        Me.Env.Setting.GetElementRenderer(design.Get("type")) _
          .Render(Me.Env, reportDesign, region, design, data)
    End Sub

End Class
