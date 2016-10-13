﻿Imports System.Text
Imports System.Text.RegularExpressions

Imports jp.co.systembase.report.expression
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.method
Imports jp.co.systembase.report.operator
Imports jp.co.systembase.report.scanner

Namespace component

    Public Class Evaluator

        Public TraverseMode As Boolean = False
        Public BasicContext As BasicContextClass = Nothing
        Public ContentContext As ContentContextClass = Nothing
        Public PageContext As PageContextClass = Nothing

        Public Class BasicContextClass
            Public Report As Report
            Public Data As ReportData
            Public DataRecord As ReportDataRecord
        End Class

        Public Class ContentContextClass
            Public Content As Content
            Public ContentState As ContentState
        End Class

        Public Class PageContextClass
            Public Page As ReportPage
            Public Pages As ReportPages
            Public DataContainer As DataContainer
            Public Variables As New Dictionary(Of String, Object)
        End Class

        Public Sub New()
            Me.TraverseMode = True
        End Sub

        Public Sub New(ByVal report As Report, ByVal data As ReportData, ByVal dataRecord As ReportDataRecord)
            Me.BasicContext = New BasicContextClass
            Me.BasicContext.Report = report
            Me.BasicContext.Data = data
            Me.BasicContext.DataRecord = dataRecord
        End Sub

        Public Sub New(ByVal content As Content, ByVal contentState As ContentState)
            Me.BasicContext = New BasicContextClass
            Me.BasicContext.Report = content.GetReport
            Me.BasicContext.Data = content.GetData
            Me.BasicContext.DataRecord = content.GetData.GetRecord
            Me.ContentContext = New ContentContextClass
            Me.ContentContext.Content = content
            Me.ContentContext.ContentState = contentState
        End Sub

        Public Sub New( _
          ByVal page As ReportPage, _
          ByVal pages As ReportPages, _
          ByVal contentInstance As ContentInstance, _
          ByVal scanner As RenderingScanner)
            Me.New(page, pages, contentInstance.Content, contentInstance.ContentState, scanner.DataContainer)
        End Sub

        Public Sub New( _
          ByVal page As ReportPage, _
          ByVal pages As ReportPages, _
          ByVal content As Content, _
          ByVal contentState As ContentState, _
          ByVal dataContainer As DataContainer)
            Me.BasicContext = New BasicContextClass
            Me.BasicContext.Report = content.GetReport
            Me.BasicContext.Data = content.GetData
            Me.BasicContext.DataRecord = content.GetData.GetRecord
            Me.ContentContext = New ContentContextClass
            Me.ContentContext.Content = content
            Me.ContentContext.ContentState = contentState
            Me.PageContext = New PageContextClass
            Me.PageContext.Page = page
            Me.PageContext.Pages = pages
            Me.PageContext.DataContainer = dataContainer
            With ContentContext.Content.Design
                If .Variables IsNot Nothing Then
                    For Each k As String In .Variables.Keys
                        Me.PageContext.Variables.Add(k, Me.EvalTry(.Variables(k)))
                    Next
                End If
            End With
        End Sub

        Public Function EvalTry(ByVal exp As String) As Object
            Try
                Return Me.Eval(exp)
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Function Eval(ByVal exp As String) As Object
            Try
                If exp Is Nothing Then
                    Return Nothing
                End If
                Dim _exp As String = exp.Trim
                If _exp.Length = 0 Then
                    Return Nothing
                End If
                If Not Me.BasicContext.Data.DataCache.Expression.ContainsKey(_exp) Then
                    Dim parser As New Parser(Me.BasicContext.Report.Design.Setting)
                    Me.BasicContext.Data.DataCache.Expression.Add(_exp, parser.Parse(_exp))
                End If
                Return Me.Eval(Me.BasicContext.Data.DataCache.Expression(_exp))
            Catch ex As Exception
                Dim _ex As EvalException
                If TypeOf ex Is EvalException Then
                    _ex = ex
                Else
                    _ex = New EvalException("式の評価中にエラーが発生しました : " & exp, ex)
                End If
                Dim logger As IReportLogger = Me.BasicContext.Report.Design.Setting.Logger
                If logger IsNot Nothing Then
                    logger.EvaluateError(exp, _ex)
                End If
                Throw _ex
            End Try
        End Function

        Public Function Eval(ByVal exp As IExpression) As Object
            Return exp.Eval(Me)
        End Function

        Public Function GetData(ByVal scope As String, ByVal unit As String) As ReportData
            Dim scopeData As ReportData = Me.BasicContext.Data.FindScope(scope)
            If scopeData Is Nothing Then
                Throw New ArgumentException("集計範囲が不正です" & IIf(Not scope.Equals(""), ": " & scope, ""))
            End If
            If unit Is Nothing Then
                Return scopeData
            Else
                Dim unitGroupDesign As GroupDesign = scopeData.FindUnit(unit)
                If unitGroupDesign Is Nothing Then
                    Throw New ArgumentException("集計単位が不正です" & IIf(Not unit.Equals(""), ": " & unit, ""))
                End If
                Dim dataSource As IReportDataSource = scopeData.GetWrapperDataSource(unitGroupDesign)
                Dim indexRange As IndexRange = scopeData.GetDataIndexRange(unitGroupDesign)
                Dim dataCache As DataCache = Me.BasicContext.Report.DataCache
                Dim setting As ReportSetting = Me.BasicContext.Report.Design.Setting
                If indexRange IsNot Nothing Then
                    Return New ReportData(dataSource, indexRange.BeginIndex, indexRange.EndIndex, dataCache, setting)
                Else
                    Return New ReportData(dataSource, dataCache, setting)
                End If
            End If
        End Function

        Public Function GetPageData(ByVal scope As String, ByVal unit As String) As ReportData
            Return Me.PageContext.DataContainer.GetPageData(Me.ContentContext.Content, scope)
        End Function

        Public Function GetPresentData(ByVal scope As String, ByVal unit As String) As ReportData
            Return Me.PageContext.DataContainer.GetPresentData(Me.ContentContext.Content, scope)
        End Function

        Public Sub ValidateParamCount(params As List(Of IExpression), count As Integer)
            If params.Count < count Then
                Throw New ArgumentException("引数が足りません")
            End If
        End Sub

    End Class

End Namespace