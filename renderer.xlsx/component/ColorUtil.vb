Imports System.Drawing
Imports NPOI.XSSF.Util
Imports NPOI.XSSF.UserModel

Namespace component
    Module ColorUtil

        Public Function GetTriplet(v As String) As Byte()
            If Not String.IsNullOrEmpty(v) Then
                If v.StartsWith("#") AndAlso v.Length = 7 Then
                    Dim _v As String = v.Substring(1).ToLower
                    For i As Integer = 0 To 5
                        If "0123456789abcdef".IndexOf(_v(i)) < 0 Then
                            Return Nothing
                        End If
                    Next
                    Return New Byte() {
                      Convert.ToInt32(_v.Substring(0, 2), 16), _
                      Convert.ToInt32(_v.Substring(2, 2), 16), _
                      Convert.ToInt32(_v.Substring(4, 2), 16)}
                Else
                    If Array.IndexOf(RenderUtil.COLOR_NAMES, v.ToLower) >= 0 Then
                        Dim c As Color = Color.FromName(v)
                        Return New Byte() {c.R, c.G, c.B}
                    End If
                End If
            End If
            Return Nothing
        End Function

        Public Function GetColor(v As String) As XSSFColor
            Dim t() As Byte = GetTriplet(v)
            If t IsNot Nothing Then
                Return New XSSFColor(t)
            Else
                Return Nothing
            End If
        End Function

        Public Function GetFontColor(v As String) As XSSFColor
            Dim t() As Byte = GetTriplet(v)
            If t IsNot Nothing Then
                If t(0) = 255 AndAlso t(1) = 255 AndAlso t(2) = 255 Then
                    Return New XSSFColor(New Byte() {254, 254, 254})
                ElseIf t(0) = 0 AndAlso t(1) = 0 AndAlso t(2) = 0 Then
                    Return Nothing
                Else
                    Return New XSSFColor(t)
                End If
            Else
                Return Nothing
            End If
        End Function

    End Module
End Namespace