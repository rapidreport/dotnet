Imports System.Drawing
Imports jp.co.systembase.NPOI.HSSF.UserModel
Imports jp.co.systembase.NPOI.HSSF.Util

Namespace component
    Public Class ColorPool

        Public Renderer As XlsRenderer
        Public Pool As New Dictionary(Of String, Short)

        Private palette As HSSFPalette
        Private index As Short

        Public Sub New(renderer As XlsRenderer)
            Me.Renderer = renderer
            Me.palette = CType(Me.Renderer.Workbook, HSSFWorkbook).GetCustomPalette
        End Sub

        Public Function GetIndex(v As String) As Short
            Dim c As Color = RenderUtil.GetColor(v)
            If Not c.Equals(Color.Empty) Then
                Return Me.GetIndex(c)
            End If
            Return -1
        End Function

        Public Function GetIndex(v As Color) As Short
            If Me.Renderer.Setting.CustomPalette Then
                Dim key As String = Me.colorToStr(v)
                If Not Me.Pool.ContainsKey(key) Then
                    If index = 64 Then
                        Return -1
                    End If
                    Me.palette.SetColorAtIndex(Me.index, v.R, v.G, v.B)
                    Me.Pool.Add(key, index)
                    Me.index += 1
                End If
                Return Me.Pool(key)
            Else
                Return ColorUtil.GetIndex(v)
            End If
        End Function

        Private Function colorToStr(v As Color) As String
            Return v.R & ":" & v.G & ":" & v.B
        End Function

    End Class
End Namespace