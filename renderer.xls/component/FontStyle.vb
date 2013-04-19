Imports NPOI.HSSF.UserModel
Imports NPOI.SS.UserModel

Imports jp.co.systembase.report.component

Namespace component
    Public Class FontStyle

        Public Setting As TextDesign

        Public Sub New(ByVal setting As TextDesign)
            Me.Setting = setting
        End Sub

        Public Sub Fill(ByVal font As HSSFFont, ByVal renderer As XlsRenderer)
            font.FontName = renderer.Setting.GetFont(Me.Setting.Font.Name)
            font.FontHeightInPoints = Me.Setting.Font.Size
            If Me.Setting.Font.Bold Then
                font.Boldweight = FontBoldWeight.BOLD
            End If
            If Me.Setting.Font.Italic Then
                font.IsItalic = True
            End If
            If Me.Setting.Font.Underline Then
                font.Underline = FontUnderlineType.SINGLE
            End If
            If Me.Setting.Color IsNot Nothing Then
                Dim i As Short = renderer.ColorPool.GetIndex(Me.Setting.Color)
                If i >= 0 Then
                    font.Color = i
                End If
            End If
        End Sub

        Public Overrides Function GetHashCode() As Integer
            Dim ret As Integer = IIf(Me.Setting.Font.Name Is Nothing, Me.Setting.Font.Name, 0)
            ret = ret Xor Me.Setting.Font.Size.GetHashCode
            ret = ret Xor IIf(Me.Setting.Font.Bold, 1, 0)
            ret = ret Xor IIf(Me.Setting.Font.Italic, 2, 0)
            ret = ret Xor IIf(Me.Setting.Font.Underline, 4, 0)
            If Me.Setting.Color IsNot Nothing Then
                ret = ret Xor Me.Setting.Color.GetHashCode
            End If
            Return ret
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Me Is obj Then
                Return True
            End If
            If obj Is Nothing Then
                Return False
            End If
            If Not TypeOf obj Is FontStyle Then
                Return False
            End If
            Dim _obj As FontStyle = obj
            If Me.Setting.Font.Name Is Nothing Then
                If _obj.Setting.Font.Name IsNot Nothing Then
                    Return False
                End If
            ElseIf Me.Setting.Font.Name <> _obj.Setting.Font.Name Then
                Return False
            End If
            If Me.Setting.Font.Size <> _obj.Setting.Font.Size Then
                Return False
            End If
            If Me.Setting.Font.Bold <> _obj.Setting.Font.Bold Then
                Return False
            End If
            If Me.Setting.Font.Italic <> _obj.Setting.Font.Italic Then
                Return False
            End If
            If Me.Setting.Font.Underline <> _obj.Setting.Font.Underline Then
                Return False
            End If
            If Me.Setting.Color Is Nothing Then
                If _obj.Setting.Color IsNot Nothing Then
                    Return False
                End If
            ElseIf Me.Setting.Color <> _obj.Setting.Color Then
                Return False
            End If
            Return True
        End Function

    End Class
End Namespace
