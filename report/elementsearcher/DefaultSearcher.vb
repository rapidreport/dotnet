Imports jp.co.systembase.report.component

Namespace elementseacher
    Public Class DefaultSearcher
        Implements IElementSearcher
        Public Function SearchByKeyword( _
          reportDesign As ReportDesign, _
          design As ElementDesign, _
          data As Object, _
          keyword As String) As Boolean Implements IElementSearcher.SearchByKeyword
            Return False
        End Function
    End Class
End Namespace