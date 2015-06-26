Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer

Namespace search.elementseacher
    Public Class FieldSearcher
        Implements IElementSearcher
        Public Function Search( _
          reportDesign As ReportDesign, _
          design As ElementDesign, _
          data As Object, _
          keyword As String) As Boolean Implements IElementSearcher.Search
            Dim text As String = RenderUtil.Format(reportDesign, design.Child("formatter"), data)
            If text IsNot Nothing Then
                Return text.IndexOf(keyword) >= 0
            Else
                Return False
            End If
        End Function
    End Class
End Namespace