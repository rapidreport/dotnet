Imports jp.co.systembase.report.component

Namespace search.searchobject
    Public Interface ISearchObject
        Function GetText( _
          reportDesign As ReportDesign, _
          design As ElementDesign, _
          data As Object) As String
    End Interface
End Namespace