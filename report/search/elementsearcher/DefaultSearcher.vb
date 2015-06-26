Imports jp.co.systembase.report.component

Namespace search.elementseacher
    Public Class DefaultSearcher
        Implements IElementSearcher
        Public Function Search( _
          reportDesign As ReportDesign, _
          design As ElementDesign, _
          data As Object, _
          keyword As String) As Boolean Implements IElementSearcher.Search
            Return False
        End Function
    End Class
End Namespace