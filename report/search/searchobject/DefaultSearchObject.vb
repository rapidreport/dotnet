Imports jp.co.systembase.report.component

Namespace search.searchobject
    Public Class DefaultSearchObject
        Implements ISearchObject
        Public Function GetText( _
          reportDesign As ReportDesign, _
          design As ElementDesign, _
          data As Object) As String Implements ISearchObject.GetText
            Return Nothing
        End Function
    End Class
End Namespace