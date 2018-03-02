Imports jp.co.systembase.report.renderer.xlsx.elementrenderer

Public Class XlsxRendererSetting
    Implements ICloneable

    Public DummyElementRenderer As IElementRenderer
    Public DefaultFont As String
    Public ElementRendererMap As New Dictionary(Of String, IElementRenderer)
    Public FontMap As New Dictionary(Of String, String)
    Public CustomPalette As Boolean
    Public ColWidthMax As Single
    Public RowHeightMax As Single
    Public VResolution As Short
    Public HResolution As Short
    Public ColWidthCoefficent As Single
    Public RowHeightCoefficent As Single

    Public Sub New()
        Me.DummyElementRenderer = New DummyRenderer
        Me.ElementRendererMap.Add("rect", New RectRenderer)
        Me.ElementRendererMap.Add("circle", New CircleRenderer)
        Me.ElementRendererMap.Add("line", New LineRenderer)
        Me.ElementRendererMap.Add("text", New TextRenderer)
        Me.ElementRendererMap.Add("field", New FieldRenderer)
        Me.ElementRendererMap.Add("barcode", New BarcodeRenderer)
        Me.ElementRendererMap.Add("image", New ImageRenderer)
        Me.ElementRendererMap.Add("subpage", New SubPageRenderer)
        Me.DefaultFont = "ＭＳ ゴシック"
        Me.FontMap.Add("gothic", "ＭＳ ゴシック")
        Me.FontMap.Add("mincho", "ＭＳ 明朝")
        Me.ColWidthMax = 800.0F
        Me.RowHeightMax = 350.0F
        Me.VResolution = 600
        Me.HResolution = 600
        Me.ColWidthCoefficent = 1.0F
        Me.RowHeightCoefficent = 1.0F
    End Sub

    Private Sub New(setting As XlsxRendererSetting)
        Me.DummyElementRenderer = setting.DummyElementRenderer
        For Each k As String In setting.ElementRendererMap.Keys
            Me.ElementRendererMap.Add(k, setting.ElementRendererMap(k))
        Next
        Me.DefaultFont = setting.DefaultFont
        For Each k As String In setting.FontMap.Keys
            Me.FontMap.Add(k, setting.FontMap(k))
        Next
        Me.ColWidthMax = setting.ColWidthMax
        Me.RowHeightMax = setting.RowHeightMax
        Me.VResolution = setting.VResolution
        Me.HResolution = setting.HResolution
        Me.ColWidthCoefficent = setting.ColWidthCoefficent
        Me.RowHeightCoefficent = setting.RowHeightCoefficent
    End Sub

    Public Function GetElementRenderer(key As String) As IElementRenderer
        If key IsNot Nothing AndAlso Me.ElementRendererMap.ContainsKey(key) Then
            Return Me.ElementRendererMap(key)
        Else
            Return Me.DummyElementRenderer
        End If
    End Function

    Public Function GetFont(key As String) As String
        If key IsNot Nothing AndAlso Me.FontMap.ContainsKey(key) Then
            Return Me.FontMap(key)
        Else
            Return Me.DefaultFont
        End If
    End Function

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return New XlsxRendererSetting(Me)
    End Function

End Class
