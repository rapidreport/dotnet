Imports jp.co.systembase.report.renderer.gdi.imageloader

Public Class RenderingEnv
    Public Graphics As Graphics
    Public Setting As GdiRendererSetting
    Public ImageLoaderMap As Dictionary(Of String, IGdiImageLoader)
    Public InDesigner As Boolean = False
    Public Sub New(g As Graphics, printer As Printer)
        Me.New(g, printer.Setting, printer.ImageLoaderMap)
    End Sub
    Public Sub New( _
      g As Graphics, _
      setting As GdiRendererSetting, _
      imageLoaderMap As Dictionary(Of String, IGdiImageLoader))
        Me.Graphics = g
        Me.Setting = setting
        Me.ImageLoaderMap = imageLoaderMap
    End Sub
End Class
