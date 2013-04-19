Imports jp.co.systembase.report.component

Namespace textformatter
    Public Class DefaultTextFormatter
        Implements ITextFormatter
        Public Function Format( _
          ByVal v As Object, _
          ByVal design As ElementDesign) As String Implements ITextFormatter.Format
            Return TextFormatterUtil.Format(v, design)
        End Function
    End Class
End Namespace
