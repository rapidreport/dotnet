﻿Imports jp.co.systembase.report.component

Namespace elementrenderer
    Public Class DummyRenderer
        Implements IElementRenderer
        Public Sub Collect( _
          ByVal renderer As XlsxRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Collect
        End Sub
    End Class
End Namespace