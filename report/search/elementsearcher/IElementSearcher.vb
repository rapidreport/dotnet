Imports jp.co.systembase.report.component

Namespace search.elementseacher
    Public Interface IElementSearcher
        Function Search( _
          ByVal reportDesign As ReportDesign, _
          ByVal design As ElementDesign, _
          ByVal data As Object, _
          ByVal keyword As String) As Boolean
    End Interface
End Namespace