Imports System.Drawing
Imports NPOI.HSSF.Util

Namespace component
    Module ColorUtil

        Public Function GetColor(ByVal v As String) As Color
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

        Public Function GetIndex(ByVal v As String) As Short
            Return GetIndex(GetColor(v))
        End Function

        Public Function GetIndex(ByVal color As Color) As Short
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
            Public Triplet As Short()
            Public Sub New(ByVal index As Short, ByVal triplet As Short())
                Me.Index = index
                Me.Triplet = triplet
            End Sub
            Public Function GetDiff(ByVal color As Color) As Integer
                Dim r As Integer = Me.Triplet(0) - color.R
                Dim g As Integer = Me.Triplet(1) - color.G
                Dim b As Integer = Me.Triplet(2) - color.B
                Return r * r + g * g + b * b
            End Function
        End Class

        Private colorTable As List(Of _ColorData) = Nothing
        Private Function getColorTable() As List(Of _ColorData)
            If colorTable Is Nothing Then
                colorTable = New List(Of _ColorData)
                colorTable.Add(New _ColorData(HSSFColor.AQUA.index, HSSFColor.AQUA.triplet))
                colorTable.Add(New _ColorData(HSSFColor.BLACK.index, HSSFColor.BLACK.triplet))
                colorTable.Add(New _ColorData(HSSFColor.BLUE.index, HSSFColor.BLUE.triplet))
                colorTable.Add(New _ColorData(HSSFColor.BLUE_GREY.index, HSSFColor.BLUE_GREY.triplet))
                colorTable.Add(New _ColorData(HSSFColor.BRIGHT_GREEN.index, HSSFColor.BRIGHT_GREEN.triplet))
                colorTable.Add(New _ColorData(HSSFColor.BROWN.index, HSSFColor.BROWN.triplet))
                colorTable.Add(New _ColorData(HSSFColor.CORAL.index, HSSFColor.CORAL.triplet))
                colorTable.Add(New _ColorData(HSSFColor.CORNFLOWER_BLUE.index, HSSFColor.CORNFLOWER_BLUE.triplet))
                colorTable.Add(New _ColorData(HSSFColor.DARK_BLUE.index, HSSFColor.DARK_BLUE.triplet))
                colorTable.Add(New _ColorData(HSSFColor.DARK_GREEN.index, HSSFColor.DARK_GREEN.triplet))
                colorTable.Add(New _ColorData(HSSFColor.DARK_RED.index, HSSFColor.DARK_RED.triplet))
                colorTable.Add(New _ColorData(HSSFColor.DARK_TEAL.index, HSSFColor.DARK_TEAL.triplet))
                colorTable.Add(New _ColorData(HSSFColor.DARK_YELLOW.index, HSSFColor.DARK_YELLOW.triplet))
                colorTable.Add(New _ColorData(HSSFColor.GOLD.index, HSSFColor.GOLD.triplet))
                colorTable.Add(New _ColorData(HSSFColor.GREEN.index, HSSFColor.GREEN.triplet))
                colorTable.Add(New _ColorData(HSSFColor.GREY_25_PERCENT.index, HSSFColor.GREY_25_PERCENT.triplet))
                colorTable.Add(New _ColorData(HSSFColor.GREY_40_PERCENT.index, HSSFColor.GREY_40_PERCENT.triplet))
                colorTable.Add(New _ColorData(HSSFColor.GREY_50_PERCENT.index, HSSFColor.GREY_50_PERCENT.triplet))
                colorTable.Add(New _ColorData(HSSFColor.GREY_80_PERCENT.index, HSSFColor.GREY_80_PERCENT.triplet))
                colorTable.Add(New _ColorData(HSSFColor.INDIGO.index, HSSFColor.INDIGO.triplet))
                colorTable.Add(New _ColorData(HSSFColor.LAVENDER.index, HSSFColor.LAVENDER.triplet))
                colorTable.Add(New _ColorData(HSSFColor.LEMON_CHIFFON.index, HSSFColor.LEMON_CHIFFON.triplet))
                colorTable.Add(New _ColorData(HSSFColor.LIGHT_BLUE.index, HSSFColor.LIGHT_BLUE.triplet))
                colorTable.Add(New _ColorData(HSSFColor.LIGHT_CORNFLOWER_BLUE.index, HSSFColor.LIGHT_CORNFLOWER_BLUE.triplet))
                colorTable.Add(New _ColorData(HSSFColor.LIGHT_GREEN.index, HSSFColor.LIGHT_GREEN.triplet))
                colorTable.Add(New _ColorData(HSSFColor.LIGHT_ORANGE.index, HSSFColor.LIGHT_ORANGE.triplet))
                colorTable.Add(New _ColorData(HSSFColor.LIGHT_TURQUOISE.index, HSSFColor.LIGHT_TURQUOISE.triplet))
                colorTable.Add(New _ColorData(HSSFColor.LIGHT_YELLOW.index, HSSFColor.LIGHT_YELLOW.triplet))
                colorTable.Add(New _ColorData(HSSFColor.LIME.index, HSSFColor.LIME.triplet))
                colorTable.Add(New _ColorData(HSSFColor.MAROON.index, HSSFColor.MAROON.triplet))
                colorTable.Add(New _ColorData(HSSFColor.OLIVE_GREEN.index, HSSFColor.OLIVE_GREEN.triplet))
                colorTable.Add(New _ColorData(HSSFColor.ORANGE.index, HSSFColor.ORANGE.triplet))
                colorTable.Add(New _ColorData(HSSFColor.ORCHID.index, HSSFColor.ORCHID.triplet))
                colorTable.Add(New _ColorData(HSSFColor.PALE_BLUE.index, HSSFColor.PALE_BLUE.triplet))
                colorTable.Add(New _ColorData(HSSFColor.PINK.index, HSSFColor.PINK.triplet))
                colorTable.Add(New _ColorData(HSSFColor.PLUM.index, HSSFColor.PLUM.triplet))
                colorTable.Add(New _ColorData(HSSFColor.RED.index, HSSFColor.RED.triplet))
                colorTable.Add(New _ColorData(HSSFColor.ROSE.index, HSSFColor.ROSE.triplet))
                colorTable.Add(New _ColorData(HSSFColor.ROYAL_BLUE.index, HSSFColor.ROYAL_BLUE.triplet))
                colorTable.Add(New _ColorData(HSSFColor.SEA_GREEN.index, HSSFColor.SEA_GREEN.triplet))
                colorTable.Add(New _ColorData(HSSFColor.SKY_BLUE.index, HSSFColor.SKY_BLUE.triplet))
                colorTable.Add(New _ColorData(HSSFColor.TAN.index, HSSFColor.TAN.triplet))
                colorTable.Add(New _ColorData(HSSFColor.TEAL.index, HSSFColor.TEAL.triplet))
                colorTable.Add(New _ColorData(HSSFColor.TURQUOISE.index, HSSFColor.TURQUOISE.triplet))
                colorTable.Add(New _ColorData(HSSFColor.VIOLET.index, HSSFColor.VIOLET.triplet))
                colorTable.Add(New _ColorData(HSSFColor.WHITE.index, HSSFColor.WHITE.triplet))
                colorTable.Add(New _ColorData(HSSFColor.YELLOW.index, HSSFColor.YELLOW.triplet))
            End If
            Return colorTable
        End Function

    End Module
End Namespace