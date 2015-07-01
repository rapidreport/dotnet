Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace search.searchobject
    Public Class TextSearchObject
        Implements ISearchObject
        Public Function GetText( _
          reportDesign As ReportDesign, _
          design As ElementDesign, _
          data As Object) As String Implements ISearchObject.GetText
            Dim ret As String = design.Get("text")
            If data IsNot Nothing Then
                Dim textProcessor As New EmbeddedTextProcessor
                ret = textProcessor.EmbedData(reportDesign, design.Child("formatter"), ret, data)
            End If
            Return ret
        End Function
    End Class
End Namespace