Imports jp.co.systembase.report.component

Namespace component

    Public Class BorderStyle

        Public Enum ELineStyle
            THIN
            DOT
            DASH
            DASHDOT
            MEDIUM
            MEDIUM_DOT
            MEDIUM_DASH
            MEDIUM_DASHDOT
            THICK
            [DOUBLE]
        End Enum

        Public LineStyle As ELineStyle = ELineStyle.THIN
        Public LineColor As String = Nothing

        Private Sub New()
        End Sub

        Public Shared Function GetInstance(ByVal design As ElementDesign, ByVal reportDesign As ReportDesign) As BorderStyle
            Dim lw As Single = reportDesign.DefaultLineWidth
            If Not design.IsNull("line_width") Then
                lw = design.Get("line_width")
            End If
            If lw = 0 Then
                Return Nothing
            End If
            Dim ret As New BorderStyle
            If design.Get("doublet") Then
                ret.LineStyle = ELineStyle.DOUBLE
            Else
                Dim ls As String = "solid"
                If Not design.IsNull("line_style") Then
                    ls = design.Get("line_style")
                End If

                If ls = "dot" Then
                    If lw >= 2.0F Then
                        ret.LineStyle = ELineStyle.MEDIUM_DOT
                    Else
                        ret.LineStyle = ELineStyle.DOT
                    End If
                ElseIf ls = "dash" Then
                    If lw >= 2.0F Then
                        ret.LineStyle = ELineStyle.MEDIUM_DASH
                    Else
                        ret.LineStyle = ELineStyle.DASH
                    End If
                ElseIf ls = "dashdot" Then
                    If lw >= 2.0F Then
                        ret.LineStyle = ELineStyle.MEDIUM_DASHDOT
                    Else
                        ret.LineStyle = ELineStyle.DASHDOT
                    End If
                Else
                    If lw >= 3.0F Then
                        ret.LineStyle = ELineStyle.THICK
                    ElseIf lw >= 2.0F Then
                        ret.LineStyle = ELineStyle.MEDIUM
                    Else
                        ret.LineStyle = ELineStyle.THIN
                    End If
                End If
            End If
            If Not design.IsNull("color") Then
                ret.LineColor = design.Get("color")
            End If
            Return ret
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim ret As Integer = Me.LineStyle.GetHashCode
            If Me.LineColor IsNot Nothing Then
                ret = ret Xor Me.LineColor.GetHashCode
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
            If Not TypeOf obj Is BorderStyle Then
                Return False
            End If
            Dim _obj As BorderStyle = obj
            If Me.LineStyle <> _obj.LineStyle Then
                Return False
            End If
            If Me.LineColor Is Nothing Then
                If _obj.LineColor IsNot Nothing Then
                    Return False
                End If
            ElseIf Me.LineColor <> _obj.LineColor Then
                Return False
            End If
            Return True
        End Function

    End Class

End Namespace