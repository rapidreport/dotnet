Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer

Namespace elementpreprocessor
    Public Interface IElementPreprocessor
        Function DoProcess( _
          renderer As IRenderer, _
          region As Region, _
          evaluator As Evaluator, _
          design As ElementDesign) As Object
    End Interface
End Namespace