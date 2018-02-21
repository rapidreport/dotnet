Imports NPOI.SS.UserModel
Imports NPOI.XSSF.UserModel

Namespace component
    Public Class GridStyle

        Public TopBorder As BorderStyle = Nothing
        Public BottomBorder As BorderStyle = Nothing
        Public LeftBorder As BorderStyle = Nothing
        Public RightBorder As BorderStyle = Nothing

        Public FillColor As String = Nothing

        Public Sub Fill(cellStyle As XSSFCellStyle, renderer As XlsxRenderer)
            If Me.LeftBorder IsNot Nothing Then
                Dim bs As BorderStyle = Me.LeftBorder
                cellStyle.BorderLeft = toNPOIBorderStyle(bs.LineStyle)
                If bs.LineColor IsNot Nothing Then
                    Dim c As XSSFColor = ColorUtil.GetColor(bs.LineColor)
                    If c IsNot Nothing Then
                        cellStyle.SetLeftBorderColor(c)
                    End If
                End If
            End If
            If Me.RightBorder IsNot Nothing Then
                Dim bs As BorderStyle = Me.RightBorder
                cellStyle.BorderRight = toNPOIBorderStyle(bs.LineStyle)
                If bs.LineColor IsNot Nothing Then
                    Dim c As XSSFColor = ColorUtil.GetColor(bs.LineColor)
                    If c IsNot Nothing Then
                        cellStyle.SetRightBorderColor(c)
                    End If
                End If
            End If
            If Me.TopBorder IsNot Nothing Then
                Dim bs As BorderStyle = Me.TopBorder
                cellStyle.BorderTop = toNPOIBorderStyle(bs.LineStyle)
                If bs.LineColor IsNot Nothing Then
                    Dim c As XSSFColor = ColorUtil.GetColor(bs.LineColor)
                    If c IsNot Nothing Then
                        cellStyle.SetTopBorderColor(c)
                    End If
                End If
            End If
            If Me.BottomBorder IsNot Nothing Then
                Dim bs As BorderStyle = Me.BottomBorder
                cellStyle.BorderBottom = toNPOIBorderStyle(bs.LineStyle)
                If bs.LineColor IsNot Nothing Then
                    Dim c As XSSFColor = ColorUtil.GetColor(bs.LineColor)
                    If c IsNot Nothing Then
                        cellStyle.SetBottomBorderColor(c)
                    End If
                End If
            End If
            If Me.FillColor IsNot Nothing Then
                Dim c As XSSFColor = ColorUtil.GetColor(Me.FillColor)
                If c IsNot Nothing Then
                    cellStyle.FillPattern = FillPattern.SolidForeground
                    cellStyle.SetFillForegroundColor(c)
                End If
            End If
        End Sub

        Private Shared Function toNPOIBorderStyle(ls As BorderStyle.ELineStyle) As NPOI.SS.UserModel.BorderStyle
            Select Case ls
                Case BorderStyle.ELineStyle.THIN
                    Return NPOI.SS.UserModel.BorderStyle.Thin
                Case BorderStyle.ELineStyle.HAIR
                    Return NPOI.SS.UserModel.BorderStyle.Hair
                Case BorderStyle.ELineStyle.DOT
                    Return NPOI.SS.UserModel.BorderStyle.Dotted
                Case BorderStyle.ELineStyle.DASH
                    Return NPOI.SS.UserModel.BorderStyle.Dashed
                Case BorderStyle.ELineStyle.DASHDOT
                    Return NPOI.SS.UserModel.BorderStyle.DashDot
                Case BorderStyle.ELineStyle.DASHDOTDOT
                    Return NPOI.SS.UserModel.BorderStyle.DashDotDot
                Case BorderStyle.ELineStyle.MEDIUM
                    Return NPOI.SS.UserModel.BorderStyle.Medium
                Case BorderStyle.ELineStyle.MEDIUM_DASH
                    Return NPOI.SS.UserModel.BorderStyle.MediumDashed
                Case BorderStyle.ELineStyle.MEDIUM_DASHDOT
                    Return NPOI.SS.UserModel.BorderStyle.MediumDashDot
                Case BorderStyle.ELineStyle.MEDIUM_DASHDOTDOT
                    Return NPOI.SS.UserModel.BorderStyle.MediumDashDotDot
                Case BorderStyle.ELineStyle.SLANTED_DASHDOT
                    Return NPOI.SS.UserModel.BorderStyle.SlantedDashDot
                Case BorderStyle.ELineStyle.THICK
                    Return NPOI.SS.UserModel.BorderStyle.Thick
                Case BorderStyle.ELineStyle.DOUBLE
                    Return NPOI.SS.UserModel.BorderStyle.Double
            End Select
            Return NPOI.SS.UserModel.BorderStyle.None
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim ret As Integer = 0
            If Me.TopBorder IsNot Nothing Then
                ret = Me.TopBorder.GetHashCode
            End If
            If Me.BottomBorder IsNot Nothing Then
                ret = ret Xor Me.BottomBorder.GetHashCode Xor &HFF
            End If
            If Me.LeftBorder IsNot Nothing Then
                ret = ret Xor Me.LeftBorder.GetHashCode Xor &HFF00
            End If
            If Me.RightBorder IsNot Nothing Then
                ret = ret Xor Me.RightBorder.GetHashCode Xor &HFF0000
            End If
            If Me.FillColor IsNot Nothing Then
                ret = ret Xor Me.FillColor.GetHashCode
            End If
            Return ret
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If Me Is obj Then
                Return True
            End If
            If obj Is Nothing Then
                Return False
            End If
            If Not TypeOf obj Is GridStyle Then
                Return False
            End If
            Dim _obj As GridStyle = obj
            If Me.TopBorder Is Nothing Then
                If _obj.TopBorder IsNot Nothing Then
                    Return False
                End If
            ElseIf Not Me.TopBorder.Equals(_obj.TopBorder) Then
                Return False
            End If
            If Me.BottomBorder Is Nothing Then
                If _obj.BottomBorder IsNot Nothing Then
                    Return False
                End If
            ElseIf Not Me.BottomBorder.Equals(_obj.BottomBorder) Then
                Return False
            End If
            If Me.LeftBorder Is Nothing Then
                If _obj.LeftBorder IsNot Nothing Then
                    Return False
                End If
            ElseIf Not Me.LeftBorder.Equals(_obj.LeftBorder) Then
                Return False
            End If
            If Me.RightBorder Is Nothing Then
                If _obj.RightBorder IsNot Nothing Then
                    Return False
                End If
            ElseIf Not Me.RightBorder.Equals(_obj.RightBorder) Then
                Return False
            End If
            If Me.FillColor Is Nothing Then
                If _obj.FillColor IsNot Nothing Then
                    Return False
                End If
            ElseIf Me.FillColor <> _obj.FillColor Then
                Return False
            End If
            Return True
        End Function

    End Class
End Namespace

