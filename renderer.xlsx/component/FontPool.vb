Imports NPOI.XSSF.UserModel

Namespace component
    Public Class FontPool

        Public Renderer As XlsxRenderer
        Public Map As New Dictionary(Of FontStyle, XSSFFont)

        Public Sub New(renderer As XlsxRenderer)
            Me.Renderer = Renderer
        End Sub

        Public Function [Get](fontStyle As FontStyle)
            If Not Me.Map.ContainsKey(fontStyle) Then
                Me.Map.Add(fontStyle, Me.createFont(fontStyle))
            End If
            Return Me.Map(fontStyle)
        End Function

        Private Function createFont(fontStyle As FontStyle) As XSSFFont
            Dim ret As XSSFFont = Me.Renderer.Workbook.CreateFont()
            fontStyle.Fill(ret, Me.Renderer)
            Return ret
        End Function

    End Class
End Namespace