Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer

Namespace elementpreprocessor
    Public Class SubPagePreprocessor
        Implements IElementPreprocessor
        Public Function DoProcess( _
          ByVal renderer As IRenderer, _
          ByVal region As Region, _
          ByVal evaluator As Evaluator, _
          ByVal design As ElementDesign) As Object Implements IElementPreprocessor.DoProcess
            Dim k As String
            Dim i As Object
            If design.IsNull("key") OrElse design.IsNull("exp") Then
                Return Nothing
            End If
            k = design.Get("key")
            i = evaluator.EvalTry(design.Get("exp"))
            If i Is Nothing Then
                Return Nothing
            End If
            evaluator.BasicContext.Report.RenderSubPage(renderer, region, k, i)
            Return Nothing
        End Function
    End Class
End Namespace
