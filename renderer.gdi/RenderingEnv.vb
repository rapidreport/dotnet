Imports jp.co.systembase.report.renderer.gdi.imageloader

Public Class RenderingEnv
    Public Graphics As Graphics
    Public Setting As GdiRendererSetting
    Public ImageLoaderMap As Dictionary(Of String, IGdiImageLoader)
    Public InDesigner As Boolean = False

    Private _IsMonospacedDefaultFont As Boolean = False
    Private _IsMonospacedFontMap As New Dictionary(Of String, Boolean)

    Public Sub New(g As Graphics, printer As Printer)
        Me.New(g, printer.Setting, printer.ImageLoaderMap)
    End Sub

    Public Sub New(
      g As Graphics,
      setting As GdiRendererSetting,
      imageLoaderMap As Dictionary(Of String, IGdiImageLoader))
        Me.Graphics = g
        Me.Setting = setting
        Me.ImageLoaderMap = imageLoaderMap
        Using _g As Graphics = Graphics.FromImage(New Bitmap(1, 1))
            _IsMonospacedDefaultFont = _IsMonospacedFont(_g, setting.DefaultFont)
            For Each f As String In setting.FontMap.Keys
                _IsMonospacedFontMap.Add(f, _IsMonospacedFont(_g, setting.FontMap(f)))
            Next
        End Using
    End Sub

    Private Function _IsMonospacedFont(g As Graphics, f As String) As Boolean
        Dim font As New Font(f, 1)
        Return ReportUtil.RoundDown(g.MeasureString("i", font).Width, -3) = ReportUtil.RoundDown(g.MeasureString("W", font).Width, -3)
    End Function

    Public Function IsMonospacedFont(fontName As String) As Boolean
        If _IsMonospacedFontMap.ContainsKey(fontName) Then
            Return _IsMonospacedFontMap(fontName)
        Else
            Return _IsMonospacedDefaultFont
        End If
    End Function

End Class
