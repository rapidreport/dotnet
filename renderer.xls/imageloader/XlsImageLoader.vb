﻿Imports System.Drawing

Imports NPOI.HSSF.UserModel

Imports jp.co.systembase.report.renderer

Namespace imageloader
    Public Class XlsImageLoader
        Implements IXlsImageLoader

        Public ImageMap As ImageMap

        Private pool As New Dictionary(Of Image, Integer)

        Public Sub New()
            Me.New(New ImageMap)
        End Sub

        Public Sub New(imageMap As ImageMap)
            Me.ImageMap = imageMap
        End Sub

        Public Overridable Function GetImage(param As Object) As Image Implements IXlsImageLoader.GetImage
            Return Me.ImageMap(param)
        End Function

    End Class
End Namespace