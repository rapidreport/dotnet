Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.elementpreprocessor
Imports jp.co.systembase.report.scanner
Imports jp.co.systembase.report.renderer

Public Class ReportPage

    Public Report As Report
    Public Range As GroupRange
    Public ToggleValue As Boolean = False
    Public PageScope As Dictionary(Of String, Object)
    Public ResetPageCount As Boolean = False

    Public AppearedGroups As Dictionary(Of GroupDesign, Group)
    Public StartedGroups As Dictionary(Of GroupDesign, Group)
    Public FinishedGroups As Dictionary(Of GroupDesign, Group)

    Public CountingPages As List(Of ReportPage) = Nothing

    Public Sub New( _
      report As Report, _
      range As GroupRange, _
      scanner As PagingScanner)
        Me.Report = report
        Me.Range = range
        Me.PageScope = New Dictionary(Of String, Object)
        Me.AppearedGroups = scanner.AppearedGroups
        Me.StartedGroups = scanner.StartedGroups
        Me.FinishedGroups = scanner.FinishedGroups
    End Sub

    Public Sub New(page As ReportPage)
        Me.Report = page.Report
        Me.Range = page.Range
        Me.PageScope = New Dictionary(Of String, Object)(page.PageScope)
        Me.AppearedGroups = page.AppearedGroups
        Me.StartedGroups = page.StartedGroups
        Me.FinishedGroups = page.FinishedGroups
    End Sub

    Public Sub Render(renderer As IRenderer, pages As ReportPages)
        Dim paperRegion As Region = Me.Report.Design.PaperDesign.GetRegion
        Try
            renderer.BeginPage(Me.Report.Design, pages.IndexOf(Me), paperRegion)
            Dim scanner As New RenderingScanner
            Me.Report.Groups.Scan(scanner, Me.Range, paperRegion)
            Me._Render_aux(renderer, pages, scanner, False)
            renderer.EndPage(Me.Report.Design)
        Catch ex As Exception
            If TypeOf ex Is RenderException Then
                Throw ex
            Else
                Dim message As String = "an error occurred while rendering page[" & pages.IndexOf(Me) & "]"
                Throw New RenderException(message, ex)
            End If
        End Try
    End Sub

    Public Sub RenderSubPage(renderer As IRenderer, pages As ReportPages, paperRegion As Region)
        Dim scanner As New RenderingScanner
        Me.Report.Groups.Scan(scanner, Me.Range, paperRegion)
        Me._Render_aux(renderer, pages, scanner, True)
    End Sub

    Private Sub _Render_aux( _
      renderer As IRenderer, _
      pages As ReportPages, _
      scanner As RenderingScanner, _
      subPage As Boolean)
        Me.ToggleValue = False
        Dim elementsMap As New Dictionary(Of ContentInstance, ElementDesigns)
        Dim evaluatorMap As New Dictionary(Of ContentInstance, Evaluator)
        For Each instance As ContentInstance In scanner.ContentInstances
            Dim evaluator As New Evaluator(Me, pages, instance, scanner)
            elementsMap.Add(instance, instance.GetElementDesigns(evaluator))
            evaluatorMap.Add(instance, evaluator)
            evaluator.EvalTry("debug")
        Next
        For Each instance As ContentInstance In scanner.ContentInstances
            Me._RenderContent(renderer, instance, elementsMap(instance), evaluatorMap(instance), subPage, True)
        Next
        For Each instance As ContentInstance In scanner.ContentInstances
            Me._RenderContent(renderer, instance, elementsMap(instance), evaluatorMap(instance), subPage, False)
        Next
    End Sub

    Private Sub _RenderContent( _
      renderer As IRenderer, _
      instance As ContentInstance, _
      elements As ElementDesigns, _
      evaluator As Evaluator, _
      subPage As Boolean, _
      background As Boolean)
        For Each design As ElementDesign In elements
            If subPage AndAlso design.Get("id") = "__trial__" Then
                Continue For
            End If
            If design.Get("background") <> background Then
                Continue For
            End If
            If Not Me.Report.InDesigner AndAlso Not design.IsVisible(evaluator) Then
                Continue For
            End If
            Try
                Dim data As Object = Nothing
                Dim region As Region = design.GetRegion(instance.Region)
                Dim preprocessor As IElementPreprocessor = Me.Report.Design.Setting.GetElementPreprocessor(design.Get("type"))
                If preprocessor IsNot Nothing Then
                    data = preprocessor.DoProcess(renderer, region, evaluator, design)
                End If
                renderer.RenderElement(Me.Report.Design, region, design, data)
            Catch ex As Exception
                If Me.Report.Design.Setting.Logger IsNot Nothing Then
                    Me.Report.Design.Setting.Logger.ElementRenderingError( _
                      instance.Content.Design, _
                      design, _
                      ex)
                End If
            End Try
        Next
    End Sub

    Public Function FindAppearedGroup(id As String) As Group
        Return findGroup(id, Me.AppearedGroups)
    End Function

    Public Function FindStartedGroup(id As String) As Group
        Return findGroup(id, Me.StartedGroups)
    End Function

    Public Function FindFinishedGroup(id As String) As Group
        Return findGroup(id, Me.FinishedGroups)
    End Function

    Private Shared Function findGroup( _
      id As String, _
      groups As Dictionary(Of GroupDesign, Group)) As Group
        For Each gd As GroupDesign In groups.Keys
            If id.Equals(gd.Id) Then
                Return groups(gd)
            End If
        Next
        Return Nothing
    End Function

End Class
