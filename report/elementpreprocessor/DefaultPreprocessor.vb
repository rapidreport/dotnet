Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.expression

Namespace elementpreprocessor
    Public Class DefaultPreprocessor
        Implements IElementPreprocessor
        Public Function DoProcess( _
          renderer As IRenderer, _
          region As Region, _
          evaluator As Evaluator, _
          design As ElementDesign) As Object Implements IElementPreprocessor.DoProcess
            If Not design.IsNull("exp") Then
                Return evaluator.EvalTry(design.Get("exp"))
            ElseIf Not design.IsNull("text") Then
                Dim textProcessor As New EmbeddedTextProcessor
                Dim expressions As List(Of String) = textProcessor.ExtractExpressions(design.Get("text"))
                If expressions IsNot Nothing Then
                    Dim ret As New ArrayList
                    For Each exp As String In expressions
                        ret.Add(evaluator.EvalTry(exp))
                    Next
                    Return ret
                End If
            End If
            Return Nothing
        End Function
    End Class
End Namespace