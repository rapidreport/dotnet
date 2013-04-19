Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer

Namespace elementpreprocessor
    Public Interface IElementPreprocessor
        Function DoProcess( _
          ByVal renderer As IRenderer, _
          ByVal region As Region, _
          ByVal evaluator As Evaluator, _
          ByVal design As ElementDesign) As Object
    End Interface
End Namespace