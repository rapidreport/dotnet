Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Interface IElementRenderer
        Enum ELineStyle
            DOT = 7
            DASH = 6
            DASHDOT = 8
        End Enum
        Sub Collect( _
          ByVal renderer As XlsxRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object)
    End Interface

End Namespace

