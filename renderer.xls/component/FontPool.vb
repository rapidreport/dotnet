Imports NPOI.HSSF.UserModel

Namespace component
    Public Class FontPool

        Public Renderer As XlsRenderer
        Public Map As New Dictionary(Of FontStyle, HSSFFont)

        Public Sub New(ByVal renderer As XlsRenderer)
            Me.Renderer = Renderer
        End Sub

        Public Function [Get](ByVal fontStyle As FontStyle)
            If Not Me.Map.ContainsKey(fontStyle) Then
                Me.Map.Add(fontStyle, Me.createFont(fontStyle))
            End If
            Return Me.Map(fontStyle)
        End Function

        Private Function createFont(ByVal fontStyle As FontStyle) As HSSFFont
            Dim ret As HSSFFont = Me.Renderer.Workbook.CreateFont()
            fontStyle.Fill(ret, Me.Renderer)
            Return ret
        End Function

    End Class
End Namespace