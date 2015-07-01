Imports jp.co.systembase.report.component

Namespace search.searchobject
    Public Interface ISearchObject
        Function GetText( _
          ByVal reportDesign As ReportDesign, _
          ByVal design As ElementDesign, _
          ByVal data As Object) As String
    End Interface
End Namespace