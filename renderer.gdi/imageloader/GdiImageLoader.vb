Imports jp.co.systembase.report.renderer

Namespace imageloader

    Public Class GdiImageLoader
        Implements IGdiImageLoader

        Public ImageMap As ImageMap

        Public Sub New()
            Me.new(New ImageMap)
        End Sub

        Public Sub New(imageMap As ImageMap)
            Me.ImageMap = imageMap
        End Sub

        Public Function GetImage(param As Object) As Image Implements IGdiImageLoader.GetImage
            Return Me.ImageMap(param)
        End Function

    End Class

End Namespace
