Imports jp.co.systembase.report.component

Namespace elementseacher
    Public Interface IElementSearcher
        Function SearchByKeyword( _
          ByVal reportDesign As ReportDesign, _
          ByVal design As ElementDesign, _
          ByVal data As Object, _
          ByVal keyword As String) As Boolean
    End Interface
End Namespace