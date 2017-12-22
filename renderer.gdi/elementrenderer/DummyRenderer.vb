Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Class DummyRenderer
        Implements IElementRenderer

        Public Sub Render( _
          env As RenderingEnv, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object) Implements IElementRenderer.Render
            If env.InDesigner Then
                Dim _region As Region = region.ToPointScale(reportDesign)
                Dim g As Graphics = env.Graphics
                Using p As New Pen(Color.Red)
                    p.DashStyle = Drawing2D.DashStyle.Dot
                    g.DrawRectangle( _
                      p, _
                      New Rectangle( _
                        _region.Left, _
                        _region.Top, _
                        _region.GetWidth, _
                        _region.GetHeight))
                    g.DrawLine( _
                      p, _
                      _region.Left, _
                      _region.Top, _
                      _region.Right, _
                      _region.Bottom)
                    g.DrawLine( _
                      p, _
                      _region.Left, _
                      _region.Bottom, _
                      _region.Right, _
                      _region.Top)
                End Using
                g.DrawString( _
                  design.Get("type"), _
                  SystemFonts.DefaultFont, _
                  Brushes.Red, _
                  Math.Min(_region.Left, _region.Right), _
                  Math.Min(_region.Top, _region.Bottom))
            End If
        End Sub

    End Class

End Namespace
