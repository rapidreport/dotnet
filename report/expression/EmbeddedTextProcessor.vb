Imports System.Text

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.textformatter

Namespace expression
    Public Class EmbeddedTextProcessor

        Private index As Integer

        Public Function ExtractExpressions(source As String) As List(Of String)
            If source Is Nothing Then
                Return Nothing
            End If
            Me.index = 0
            Dim ret As List(Of String) = Nothing
            Do
                Me.nextText(source)
                If Me.index = source.Length Then
                    Exit Do
                End If
                If ret Is Nothing Then
                    ret = New List(Of String)
                End If
                ret.Add(Me.nextExpression(source))
            Loop
            Return ret
        End Function

        Public Function EmbedData( _
          reportDesign As ReportDesign, _
          formatterDesign As ElementDesign, _
          source As String, _
          data As ArrayList) As String
            If source Is Nothing Then
                Return Nothing
            End If
            Me.index = 0
            Dim sb As New StringBuilder
            Dim i As Integer = 0
            Do
                If Me.index = source.Length Then
                    Exit Do
                End If
                Dim t As String = Me.nextText(source)
                If t IsNot Nothing Then
                    sb.Append(t)
                End If
                If i < data.Count Then
                    Dim v As String = RenderUtil.Format(reportDesign, formatterDesign, data(i))
                    If v IsNot Nothing Then
                        sb.Append(v)
                    End If
                    Me.nextExpression(source)
                    i += 1
                End If
            Loop
            Return sb.ToString
        End Function

        Private Function nextText(source As String) As String
            If Me.index = source.Length Then
                Return Nothing
            End If
            Dim i As Integer = Me.index
            Do
                If i < source.Length - 1 AndAlso _
                  (source(i) = "#"c And source(i + 1) = "{"c) Then
                    Dim ret As String = source.Substring(Me.index, i - Me.index)
                    Me.index = i + 2
                    Return ret
                End If
                If i = source.Length Then
                    Dim ret As String = source.Substring(Me.index)
                    Me.index = i
                    Return ret
                End If
                i += 1
            Loop
        End Function

        Private Function nextExpression(source As String) As String
            Dim i As Integer = Me.index
            Dim quoted As Boolean = False
            Dim escaped As Boolean = False
            Do
                If i = source.Length Then
                    Throw New EvalException("埋め込まれた式が閉じられていません : " & source)
                End If
                If quoted Then
                    If escaped Then
                        escaped = False
                    Else
                        If source(i) = "'"c Then
                            quoted = False
                        ElseIf source(i) = "\"c Then
                            escaped = True
                        End If
                    End If
                Else
                    If source(i) = "}"c Then
                        Dim ret As String = source.Substring(Me.index, i - Me.index)
                        Me.index = i + 1
                        Return ret.Trim
                    ElseIf source(i) = "'"c Then
                        quoted = True
                    End If
                End If
                i += 1
            Loop
        End Function

    End Class
End Namespace