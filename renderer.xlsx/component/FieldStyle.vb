Imports NPOI.XSSF.UserModel
Imports NPOI.SS.UserModel

Imports jp.co.systembase.report.component

Namespace component
    Public Class FieldStyle

        Public FontStyle As FontStyle
        Public TextDesign As TextDesign

        Public Sub New(textDesign As TextDesign)
            Me.FontStyle = New FontStyle(textDesign)
            Me.TextDesign = textDesign
        End Sub

        Public Sub Fill(cellStyle As XSSFCellStyle, renderer As XlsxRenderer)
            cellStyle.SetFont(renderer.FontPool.Get(Me.FontStyle))
            If Not Me.TextDesign.Vertical And Me.TextDesign.Distribute Then
                cellStyle.Alignment = HorizontalAlignment.Distributed
            Else
                Select Case Me.TextDesign.HAlign
                    Case Report.EHAlign.LEFT
                        cellStyle.Alignment = HorizontalAlignment.Left
                    Case Report.EHAlign.CENTER
                        cellStyle.Alignment = HorizontalAlignment.Center
                    Case Report.EHAlign.RIGHT
                        cellStyle.Alignment = HorizontalAlignment.Right
                End Select
            End If
            If Me.TextDesign.Vertical And Me.TextDesign.Distribute Then
                cellStyle.VerticalAlignment = VerticalAlignment.Distributed
            Else
                Select Case Me.TextDesign.VAlign
                    Case Report.EVAlign.TOP
                        cellStyle.VerticalAlignment = VerticalAlignment.Top
                    Case Report.EVAlign.CENTER
                        cellStyle.VerticalAlignment = VerticalAlignment.Center
                    Case Report.EVAlign.BOTTOM
                        cellStyle.VerticalAlignment = VerticalAlignment.Bottom
                End Select
            End If
            If Me.TextDesign.Wrap Then
                cellStyle.WrapText = True
            End If
            If Me.TextDesign.ShrinkToFit Then
                cellStyle.ShrinkToFit = True
            End If
            If Me.TextDesign.Vertical Then
                cellStyle.Rotation = &HFF
            End If
            If Me.TextDesign.XlsFormat IsNot Nothing Then
                Dim dataFormat As Short = renderer.Workbook.CreateDataFormat().GetFormat(Me.TextDesign.XlsFormat)
                cellStyle.DataFormat = dataFormat
            Else
                cellStyle.DataFormat = &H31
            End If
        End Sub

        Public Overrides Function GetHashCode() As Integer
            Dim ret As Integer = Me.FontStyle.GetHashCode
            ret = ret Xor Me.TextDesign.HAlign.GetHashCode
            ret = ret Xor Me.TextDesign.VAlign.GetHashCode
            ret = ret Xor IIf(Me.TextDesign.Wrap, 1, 0)
            ret = ret Xor IIf(Me.TextDesign.ShrinkToFit, 2, 0)
            ret = ret Xor IIf(Me.TextDesign.Vertical, 4, 0)
            ret = ret Xor IIf(Me.TextDesign.Distribute, 8, 0)
            If Me.TextDesign.XlsFormat IsNot Nothing Then
                ret = ret Xor Me.TextDesign.XlsFormat.GetHashCode
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
            If Not TypeOf obj Is FieldStyle Then
                Return False
            End If
            Dim _obj As FieldStyle = obj
            If Not Me.FontStyle.Equals(_obj.FontStyle) Then
                Return False
            End If
            If Me.TextDesign.HAlign <> _obj.TextDesign.HAlign Then
                Return False
            End If
            If Me.TextDesign.VAlign <> _obj.TextDesign.VAlign Then
                Return False
            End If
            If Me.TextDesign.Wrap <> _obj.TextDesign.Wrap Then
                Return False
            End If
            If Me.TextDesign.ShrinkToFit <> _obj.TextDesign.ShrinkToFit Then
                Return False
            End If
            If Me.TextDesign.Vertical <> _obj.TextDesign.Vertical Then
                Return False
            End If
            If Me.TextDesign.Distribute <> _obj.TextDesign.Distribute Then
                Return False
            End If
            If Me.TextDesign.XlsFormat Is Nothing Then
                If _obj.TextDesign.XlsFormat IsNot Nothing Then
                    Return False
                End If
            ElseIf Me.TextDesign.XlsFormat <> _obj.TextDesign.XlsFormat Then
                Return False
            End If
            Return True
        End Function

    End Class
End Namespace
