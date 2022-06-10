Imports System.Drawing
Imports NPOI.XSSF.Util
Imports NPOI.XSSF.UserModel

Namespace component
    Module ColorUtil

        Public Function GetColor(v As String) As XSSFColor
            Dim c = RenderUtil.GetColor(v)
            If Not c.IsEmpty Then
                Return New XSSFColor(c)
            Else
                Return Nothing
            End If
        End Function

        Public Function GetFontColor(v As String) As XSSFColor
            Dim c = RenderUtil.GetColor(v)
            If Not c.IsEmpty Then
                If c.R = 255 AndAlso c.G = 255 AndAlso c.B = 255 Then
                    Return New XSSFColor(New Byte() {254, 254, 254})
                ElseIf c.R = 0 AndAlso c.G = 0 AndAlso c.B = 0 Then
                    Return Nothing
                Else
                    Return New XSSFColor(c)
                End If
            Else
                Return Nothing
            End If
        End Function

    End Module
End Namespace