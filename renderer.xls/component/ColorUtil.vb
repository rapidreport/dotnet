Imports System.Drawing
Imports NPOI.HSSF.Util

Namespace component
    Module ColorUtil

        Public Function GetColor(v As String) As Color
            If Not String.IsNullOrEmpty(v) Then
                If v.StartsWith("#") AndAlso v.Length = 7 Then
                    Dim _v As String = v.Substring(1).ToLower
                    For i As Integer = 0 To 5
                        If "0123456789abcdef".IndexOf(_v(i)) < 0 Then
                            Return Color.Empty
                        End If
                    Next
                    Return Color.FromArgb( _
                      Convert.ToInt32(_v.Substring(0, 2), 16), _
                      Convert.ToInt32(_v.Substring(2, 2), 16), _
                      Convert.ToInt32(_v.Substring(4, 2), 16))
                Else
                    If Array.IndexOf(RenderUtil.COLOR_NAMES, v.ToLower) >= 0 Then
                        Return Color.FromName(v)
                    End If
                End If
            End If
            Return Color.Empty
        End Function

        Public Function GetIndex(v As String) As Short
            Return GetIndex(GetColor(v))
        End Function

        Public Function GetIndex(color As Color) As Short
            Dim ret As Short = 0
            Dim diff As Integer = &HFFFFFF
            For Each c As _ColorData In getColorTable()
                Dim _diff As Integer = c.GetDiff(color)
                If _diff < diff Then
                    ret = c.Index
                    diff = _diff
                End If
            Next
            Return ret
        End Function

        Private Class _ColorData
            Public Index As Short
            Public Triplet As Byte()
            Public Sub New(index As Short, triplet As Byte())
                Me.Index = index
                Me.Triplet = triplet
            End Sub
            Public Function GetDiff(color As Color) As Integer
                Dim r As Integer = CType(Me.Triplet(0), Integer) - color.R
                Dim g As Integer = CType(Me.Triplet(1), Integer) - color.G
                Dim b As Integer = CType(Me.Triplet(2), Integer) - color.B
                Return r * r + g * g + b * b
            End Function
        End Class

        Private colorTable As List(Of _ColorData) = Nothing
        Private Function getColorTable() As List(Of _ColorData)
            If colorTable Is Nothing Then
                colorTable = New List(Of _ColorData)
                colorTable.Add(New _ColorData(HSSFColor.Aqua.Index, HSSFColor.Aqua.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Black.Index, HSSFColor.Black.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Blue.Index, HSSFColor.Blue.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.BlueGrey.Index, HSSFColor.BlueGrey.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.BrightGreen.Index, HSSFColor.BrightGreen.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Brown.Index, HSSFColor.Brown.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Coral.Index, HSSFColor.Coral.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.CornflowerBlue.Index, HSSFColor.CornflowerBlue.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.DarkBlue.Index, HSSFColor.DarkBlue.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.DarkGreen.Index, HSSFColor.DarkGreen.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.DarkRed.Index, HSSFColor.DarkRed.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.DarkTeal.Index, HSSFColor.DarkTeal.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.DarkYellow.Index, HSSFColor.DarkYellow.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Gold.Index, HSSFColor.Gold.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Green.Index, HSSFColor.Green.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Grey25Percent.Index, HSSFColor.Grey25Percent.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Grey40Percent.Index, HSSFColor.Grey40Percent.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Grey50Percent.Index, HSSFColor.Grey50Percent.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Grey80Percent.Index, HSSFColor.Grey80Percent.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Indigo.Index, HSSFColor.Indigo.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Lavender.Index, HSSFColor.Lavender.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.LemonChiffon.Index, HSSFColor.LemonChiffon.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.LightBlue.Index, HSSFColor.LightBlue.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.LightCornflowerBlue.Index, HSSFColor.LightCornflowerBlue.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.LightGreen.Index, HSSFColor.LightGreen.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.LightOrange.Index, HSSFColor.LightOrange.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.LightTurquoise.Index, HSSFColor.LightTurquoise.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.LightYellow.Index, HSSFColor.LightYellow.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Lime.Index, HSSFColor.Lime.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Maroon.Index, HSSFColor.Maroon.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.OliveGreen.Index, HSSFColor.OliveGreen.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Orange.Index, HSSFColor.Orange.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Orchid.Index, HSSFColor.Orchid.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.PaleBlue.Index, HSSFColor.PaleBlue.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Pink.Index, HSSFColor.Pink.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Plum.Index, HSSFColor.Plum.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Red.Index, HSSFColor.Red.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Rose.Index, HSSFColor.Rose.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.RoyalBlue.Index, HSSFColor.RoyalBlue.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.SeaGreen.Index, HSSFColor.SeaGreen.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.SkyBlue.Index, HSSFColor.SkyBlue.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Tan.Index, HSSFColor.Tan.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Teal.Index, HSSFColor.Teal.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Turquoise.Index, HSSFColor.Turquoise.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Violet.Index, HSSFColor.Violet.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.White.Index, HSSFColor.White.Triplet))
                colorTable.Add(New _ColorData(HSSFColor.Yellow.Index, HSSFColor.Yellow.Triplet))
            End If
            Return colorTable
        End Function

    End Module
End Namespace