Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer

Namespace search.searchobject
    Public Class FieldSearchObject
        Implements ISearchObject
        Public Function GetText( _
          reportDesign As ReportDesign, _
          design As ElementDesign, _
          data As Object) As String Implements ISearchObject.GetText
            Return RenderUtil.Format(reportDesign, design.Child("formatter"), data)
        End Function
    End Class
End Namespace