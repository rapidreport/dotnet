Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.textformatter
Imports jp.co.systembase.report.renderer.gdi.elementrenderer

Public Class GdiRendererSetting
    Implements ICloneable

    Public DummyElementRenderer As IElementRenderer
    Public DefaultFont As String
    Public ElementRendererMap As New Dictionary(Of String, IElementRenderer)
    Public FontMap As New Dictionary(Of String, String)

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
    End Sub

    Private Sub New(ByVal setting As GdiRendererSetting)
        Me.DummyElementRenderer = setting.DummyElementRenderer
        For Each k As String In setting.ElementRendererMap.Keys
            Me.ElementRendererMap.Add(k, setting.ElementRendererMap(k))
        Next
        Me.DefaultFont = setting.DefaultFont
        For Each k As String In setting.FontMap.Keys
            Me.FontMap.Add(k, setting.FontMap(k))
        Next
    End Sub

    Public Function GetElementRenderer(ByVal key As String) As IElementRenderer
        If key IsNot Nothing AndAlso Me.ElementRendererMap.ContainsKey(key) Then
            Return Me.ElementRendererMap(key)
        Else
            Return Me.DummyElementRenderer
        End If
    End Function

    Public Function GetFont(ByVal key As String) As String
        If key IsNot Nothing AndAlso Me.FontMap.ContainsKey(key) Then
            Return Me.FontMap(key)
        Else
            Return Me.DefaultFont
        End If
    End Function

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return New GdiRendererSetting(Me)
    End Function

End Class
