Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Interface IElementRenderer
        Enum ELineStyle
            DOT = 7
            DASH = 6
            DASHDOT = 8
        End Enum
        Sub Collect( _
          renderer As XlsxRenderer, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object)
    End Interface

End Namespace

