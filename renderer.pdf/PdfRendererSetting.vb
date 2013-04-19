﻿Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports jp.co.systembase.report.renderer.pdf.elementrenderer

Public Class PdfRendererSetting
    Implements ICloneable

    Public DummyElementRenderer As IElementRenderer
    Public DefaultFont As BaseFont
    Public ElementRendererMap As New Dictionary(Of String, IElementRenderer)
    Public FontMap As New Dictionary(Of String, BaseFont)
    Public ReplaceBackslashToYen As Boolean
    Private Shared _loaded As Boolean = False

    Public Sub New()
        If Not _loaded Then
            BaseFont.AddToResourceSearch(My.Application.Info.DirectoryPath & "\iTextAsian.dll")
            _loaded = True
        End If
        Me.DummyElementRenderer = New DummyRenderer
        Me.ElementRendererMap.Add("rect", New RectRenderer)
        Me.ElementRendererMap.Add("circle", New CircleRenderer)
        Me.ElementRendererMap.Add("line", New LineRenderer)
        Me.ElementRendererMap.Add("field", New FieldRenderer)
        Me.ElementRendererMap.Add("text", New TextRenderer)
        Me.ElementRendererMap.Add("barcode", New BarcodeRenderer)
        Me.ElementRendererMap.Add("image", New ImageRenderer)
        Me.ElementRendererMap.Add("subpage", New SubPageRenderer)
        Me.DefaultFont = BaseFont.CreateFont("HeiseiKakuGo-W5", "UniJIS-UCS2-H", BaseFont.NOT_EMBEDDED)
        Me.FontMap.Add("gothic", Me.DefaultFont)
        Me.FontMap.Add("mincho", BaseFont.CreateFont("HeiseiMin-W3", "UniJIS-UCS2-H", BaseFont.NOT_EMBEDDED))
        Me.ReplaceBackslashToYen = False
    End Sub

    Private Sub New(ByVal setting As PdfRendererSetting)
        Me.DummyElementRenderer = setting.DummyElementRenderer
        For Each k As String In setting.ElementRendererMap.Keys
            Me.ElementRendererMap.Add(k, setting.ElementRendererMap(k))
        Next
        Me.DefaultFont = setting.DefaultFont
        For Each k As String In setting.FontMap.Keys
            Me.FontMap.Add(k, setting.FontMap(k))
        Next
        Me.ReplaceBackslashToYen = setting.ReplaceBackslashToYen
    End Sub

    Public Function GetElementRenderer(ByVal key As String) As IElementRenderer
        If Not String.IsNullOrEmpty(key) AndAlso Me.ElementRendererMap.ContainsKey(key) Then
            Return Me.ElementRendererMap(key)
        Else
            Return Me.DummyElementRenderer
        End If
    End Function

    Public Function GetFont(ByVal key As String) As BaseFont
        If Not String.IsNullOrEmpty(key) AndAlso Me.FontMap.ContainsKey(key) Then
            Return Me.FontMap(key)
        Else
            Return Me.DefaultFont
        End If
    End Function

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return New PdfRendererSetting(Me)
    End Function

End Class
