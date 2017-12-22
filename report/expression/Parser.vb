Imports System.Text
Imports System.Text.RegularExpressions

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.operator
Imports jp.co.systembase.report.method

Namespace expression
    Public Class Parser

        Public Event CreateMethod(method As IMethod, methodKey As String, param As String, scope As String, unit As String)

        Private Const LITERAL_NULL As String = "null"
        Private Const LITERAL_TRUE As String = "true"
        Private Const LITERAL_FALSE As String = "false"

        Private Shared REGEX_NUMBER As New Regex("^-?[0-9.]+$")
        Private Shared REGEX_DATE As New Regex("^#([0-9]{4})([0-9]{2})([0-9]{2})$")
        Private Shared REGEX_DATETIME As New Regex("^#([0-9]{4})([0-9]{2})([0-9]{2})([0-9]{2})([0-9]{2})([0-9]{2})$")
        Private Shared REGEX_METHOD As New Regex("^([^.#@]*)?(\.((\'.*\')|([^.#@]*)))?(@([^.#@]*))?(#([^.#@]*))?$")

        Private index As Integer
        Private setting As ReportSetting

        Public Sub New(setting As ReportSetting)
            Me.setting = setting
        End Sub

        Public Function Parse(source As String) As IExpression
            If source Is Nothing Then
                Return Nothing
            End If
            Dim _source As String = source.Replace(vbCrLf, " ").Trim
            If _source.Length = 0 Then
                Return Nothing
            End If
            Me.index = 0
            Dim ret As IExpression = Me._Parse_aux(_source)
            If Me.index < _source.Length Then
                Throw New EvalException("'" & _source.Substring(Me.index) & "' は予期せぬ文字です : " & source)
            End If
            Return ret
        End Function

        Private Function _Parse_aux(source As String) As IExpression
            If source(Me.index) = "("c Then
                Return Me._Parse_operator(source)
            Else
                Dim token As String = Me.nextToken(source)
                If token.Equals(LITERAL_NULL) Then
                    Return New ImmediateExpression(Nothing)
                ElseIf token.Equals(LITERAL_TRUE) Then
                    Return New ImmediateExpression(True)
                ElseIf token.Equals(LITERAL_FALSE) Then
                    Return New ImmediateExpression(False)
                ElseIf token.StartsWith("'") Then
                    Return New ImmediateExpression(evalString(token))
                End If
                With Nothing
                    Dim m As Match = REGEX_NUMBER.Match(token)
                    If m.Success Then
                        Return New ImmediateExpression(CType(token, Decimal))
                    End If
                End With
                With Nothing
                    Dim m As Match = REGEX_DATE.Match(token)
                    If m.Success Then
                        Dim year As Integer = CType(m.Groups(1).ToString, Integer)
                        Dim month As Integer = CType(m.Groups(2).ToString, Integer)
                        Dim day As Integer = CType(m.Groups(3).ToString, Integer)
                        Return New ImmediateExpression(New DateTime(year, month, day, 0, 0, 0))
                    End If
                End With
                With Nothing
                    Dim m As Match = REGEX_DATETIME.Match(token)
                    If m.Success Then
                        Dim year As Integer = CType(m.Groups(1).ToString, Integer)
                        Dim month As Integer = CType(m.Groups(2).ToString, Integer)
                        Dim day As Integer = CType(m.Groups(3).ToString, Integer)
                        Dim hour As Integer = CType(m.Groups(4).ToString, Integer)
                        Dim minute As Integer = CType(m.Groups(5).ToString, Integer)
                        Dim second As Integer = CType(m.Groups(6).ToString, Integer)
                        Return New ImmediateExpression(New DateTime(year, month, day, hour, minute, second))
                    End If
                End With
                With Nothing
                    Dim m As Match = REGEX_METHOD.Match(token)
                    If m.Success Then
                        Dim methodKey As String = m.Groups(1).ToString.Trim
                        Dim method As IMethod = Nothing
                        Dim param As String = Nothing
                        Dim param1 As String = m.Groups(4).ToString.Trim
                        Dim param2 As String = m.Groups(5).ToString.Trim
                        Dim scope As String = m.Groups(7).ToString.Trim
                        Dim unit As String = m.Groups(9).ToString.Trim
                        If String.IsNullOrEmpty(methodKey) Then
                            methodKey = Nothing
                        End If
                        If Not String.IsNullOrEmpty(param1) Then
                            param = evalString(param1)
                        ElseIf Not String.IsNullOrEmpty(param2) Then
                            param = param2
                        End If
                        If String.IsNullOrEmpty(param) Then
                            param = Nothing
                        End If
                        If m.Groups(6).Length = 0 Then
                            scope = Nothing
                        End If
                        If m.Groups(8).Length = 0 Then
                            unit = Nothing
                        End If
                        method = Me.setting.GetMethod(methodKey)
                        If method Is Nothing Then
                            Throw New EvalException("メソッド '" & methodKey & "' は見つかりません : " & source)
                        End If
                        RaiseEvent CreateMethod(method, methodKey, param, scope, unit)
                        Return New MethodExpression(method, param, scope, unit)
                    End If
                End With
                Return Nothing
            End If
        End Function

        Private Function _Parse_operator(source As String) As IExpression
            Me.index += 1
            Me.skipSpace(source)
            Dim operatorKey As String = nextToken(source)
            If operatorKey.Length = 0 Then
                Throw New EvalException("オペレータがありません : " & source)
            End If
            Dim [operator] As IOperator = Me.setting.GetOperator(operatorKey)
            If [operator] Is Nothing Then
                Throw New EvalException("オペレータ '" & operatorKey & "' は見つかりません : " & source)
            End If
            Dim params As New List(Of IExpression)
            Do
                Me.skipSpace(source)
                If Me.index = source.Length Then
                    Throw New EvalException("')' がありません : " & source)
                End If

                If source(Me.index) = ")"c Then
                    Me.index += 1
                    Return New OperatorExpression([operator], params)
                End If
                params.Add(Me._Parse_aux(source))
            Loop
        End Function

        Private Sub skipSpace(source As String)
            Do While Me.index < source.Length AndAlso source(Me.index) = " "c
                Me.index += 1
            Loop
        End Sub

        Private Function nextToken(source As String) As String
            Dim i As Integer = Me.index
            Dim escaped As Boolean = False
            Dim quoted As Boolean = False
            Do While i < source.Length
                Dim c As Char = source(i)
                If Not escaped And Not quoted Then
                    If c = " "c Or c = ")"c Then
                        Exit Do
                    End If
                End If
                If Not escaped Then
                    If c = "\" Then
                        escaped = True
                    Else
                        If Not quoted Then
                            If c = "'" Then
                                quoted = True
                            End If
                        Else
                            If c = "'" Then
                                quoted = False
                            End If
                        End If
                    End If
                Else
                    escaped = False
                End If
                i += 1
            Loop
            Dim ret As String = ""
            If i > Me.index Then
                ret = source.Substring(Me.index, i - Me.index)
                Me.index = i
            End If
            Return ret
        End Function

        Private Function evalString(token As String) As String
            Dim i As Integer = 1
            Dim j As Integer = 1
            Dim sb As New StringBuilder
            Dim escaped As Boolean = False
            Do
                If j >= token.Length Then
                    Throw New EvalException("文字列が閉じられていません : " & token)
                End If
                Dim c As Char = token(j)
                If Not escaped Then
                    If c = "'"c Then
                        If j > i Then
                            sb.Append(token.Substring(i, j - i))
                            i = j
                        End If
                        Exit Do
                    ElseIf c = "\"c Then
                        If j > i Then
                            sb.Append(token.Substring(i, j - i))
                            i = j
                        End If
                        escaped = True
                    End If
                Else
                    Select Case c
                        Case "'"
                            sb.Append("'")
                        Case "\"
                            sb.Append("\")
                        Case "n"
                            sb.Append(vbCrLf)
                        Case Else
                            Throw New EvalException("不正なエスケープ文字です: \" & c & " (有効なもの \' \\ \n) : " & token)
                    End Select
                    escaped = False
                    i = j + 1
                End If
                j += 1
            Loop
            If i + 1 < token.Length Then
                Throw New EvalException("'" & token.Substring(i + 1, 1) & "'は予期せぬ文字です : " & token)
            End If
            Return sb.ToString
        End Function

    End Class
End Namespace