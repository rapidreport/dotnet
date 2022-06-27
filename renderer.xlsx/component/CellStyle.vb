Imports jp.co.systembase.NPOI.XSSF.UserModel

Namespace component
    Public Class CellStyle

        Public GridStyle As New GridStyle
        Public FieldStyle As FieldStyle = Nothing

        Public Sub Fill(cellStyle As XSSFCellStyle, renderer As XlsxRenderer)
            Me.GridStyle.Fill(cellStyle, renderer)
            If Me.FieldStyle IsNot Nothing Then
                Me.FieldStyle.Fill(cellStyle, renderer)
            End If
        End Sub

        Public Overrides Function GetHashCode() As Integer
            Dim ret As Integer = Me.GridStyle.GetHashCode
            If Me.FieldStyle IsNot Nothing Then
                ret = ret Xor Me.FieldStyle.GetHashCode
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
            If Not TypeOf obj Is CellStyle Then
                Return False
            End If
            Dim _obj As CellStyle = obj
            If Not Me.GridStyle.Equals(_obj.GridStyle) Then
                Return False
            End If
            If Me.FieldStyle Is Nothing Then
                If _obj.FieldStyle IsNot Nothing Then
                    Return False
                End If
            ElseIf Not Me.FieldStyle.Equals(_obj.FieldStyle) Then
                Return False
            End If
            Return True
        End Function

    End Class
End Namespace

