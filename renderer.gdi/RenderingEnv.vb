Imports jp.co.systembase.report.renderer.gdi.imageloader

Public Class RenderingEnv
    Public Graphics As Graphics
    Public Setting As GdiRendererSetting
    Public ImageLoaderMap As Dictionary(Of String, IGdiImageLoader)
    Public InDesigner As Boolean = False
    Public Sub New(ByVal g As Graphics, ByVal printer As Printer)
        Me.New(g, printer.Setting, printer.ImageLoaderMap)
    End Sub
    Public Sub New( _
      ByVal g As Graphics, _
      ByVal setting As GdiRendererSetting, _
      ByVal imageLoaderMap As Dictionary(Of String, IGdiImageLoader))
        Me.Graphics = g
        Me.Setting = setting
        Me.ImageLoaderMap = imageLoaderMap
    End Sub
End Class
