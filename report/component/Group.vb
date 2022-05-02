Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.scanner

Namespace component

    Public Class Group

        Public Data As ReportData = Nothing
        Public Contents As List(Of Content) = Nothing
        Public ParentGroups As Groups = Nothing
        Public Index As Integer
        Public TraversalIndex As Integer = -1

        Public DataIndexRangeMap As New Dictionary(Of GroupDesign, IndexRange)

        Public CrosstabState As Crosstab.State = Nothing

        Public Sub New(parentGroups As Groups, index As Integer, crosstabState As Crosstab.State)
            Me.ParentGroups = parentGroups
            Me.Index = index
            If crosstabState IsNot Nothing Then
                Me.CrosstabState = crosstabState
            Else
                If Me.ParentGroups.ParentContent IsNot Nothing Then
                    Me.CrosstabState = Me.ParentGroups.ParentContent.ParentGroup.CrosstabState
                End If
            End If
        End Sub

        Public Sub Fill(data As ReportData, updateContext As Boolean)
            Me.Data = data
            Me.Data.Group = Me
            If updateContext Then
                Me.Data.UpdateContext()
            End If
            Me.Contents = New List(Of Content)
            For Each cd As ContentDesign In Me.GetDesign.ContentDesigns
                If Not Me.GetReport.InDesigner AndAlso cd.ExistenceCond IsNot Nothing Then
                    Dim evaluator As New Evaluator(Me.GetReport, Me.Data, Me.Data.GetRecord)
                    If Not ReportUtil.Condition(evaluator.EvalTry(cd.ExistenceCond)) Then
                        Continue For
                    End If
                End If
                Dim c As New Content(cd, Me)
                Me.Contents.Add(c)
                c.Fill(Me.Data)
            Next
            If Me.Contents.Count = 0 Then
                Dim cd As New ContentDesign(New Hashtable, Me.GetDesign)
                Dim c As New Content(cd, Me)
                Me.Contents.Add(c)
                c.Fill(New ReportData(DummyDataSource.GetInstance, Me))
            End If
        End Sub

        Public Function Scan(
          scanner As IScanner,
          contentRange As ContentRange,
          paperRegion As Region,
          parentRegion As Region,
          groupState As GroupState) As Region
            Dim _scanner As IScanner = scanner.BeforeGroup(Me, contentRange, parentRegion, groupState)
            Dim contentsRegion As Region = Me.GetDesign.Layout.GetGroupInitialRegion(parentRegion)
            Dim region As Region = contentsRegion
            Dim firstPage As Boolean = False
            Dim lastPage As Boolean = False
            If contentRange.First IsNot Nothing Then
                If contentRange.First.Content Is Me.Contents(0) And
                  contentRange.First.ContentFirst Then
                    firstPage = True
                End If
                If contentRange.Last Is Nothing Then
                    lastPage = True
                ElseIf contentRange.Last.Content Is Me.Contents(Me.Contents.Count - 1) And
                      contentRange.Last.ContentLast Then
                    lastPage = True
                End If
            End If
            Dim _contents As New List(Of Content)
            Dim _contentStates As New List(Of ContentState)
            With Nothing
                Dim intrinsic As Boolean = False
                For Each content As Content In Me.Contents
                    If contentRange.IsFirst(content) Then
                        intrinsic = True
                    End If
                    If intrinsic Or content.Design.EveryPage Then
                        Dim contentState As New ContentState(groupState, content)
                        contentState.Intrinsic = intrinsic
                        contentState.FirstPage = firstPage
                        contentState.LastPage = lastPage
                        _contents.Add(content)
                        _contentStates.Add(contentState)
                    End If
                    If contentRange.IsLast(content) Then
                        intrinsic = False
                    End If
                Next
                If _contentStates.Count > 0 Then
                    _contentStates(0).Header = True
                    _contentStates(_contentStates.Count - 1).Footer = True
                End If
            End With
            With Nothing
                Dim lastRegion As Region = Nothing
                For i As Integer = 0 To _contents.Count - 1
                    Dim content As Content = _contents(i)
                    Dim contentState As ContentState = _contentStates(i)
                    Dim evaluator As New Evaluator(content, contentState)
                    If Not Me.GetReport.InDesigner AndAlso content.Design.VisibilityCond IsNot Nothing Then
                        If Not ReportUtil.Condition(evaluator.EvalTry(content.Design.VisibilityCond)) Then
                            Continue For
                        End If
                    End If
                    Dim groupRange As GroupRange
                    If contentState.Intrinsic Then
                        groupRange = contentRange.GetSubRange(content)
                    ElseIf content.Groups IsNot Nothing Then
                        If Report.Compatibility._4_37_EveryPageAllData Then
                            If content.Design.EveryPageBlankGroup Then
                                groupRange = New GroupRange(content.Groups, Nothing, Nothing)
                            Else
                                groupRange = New GroupRange(content.Groups)
                            End If
                        Else
                            groupRange = New GroupRange(content.Groups, Nothing, Nothing)
                        End If
                    Else
                        groupRange = Nothing
                    End If
                    Dim contentRegion As Region = Me.GetDesign.Layout.GetContentRegion(content.Design.Size, contentsRegion, lastRegion,
                                                                                       IIf(Me.GetReport.InDesigner, Nothing, evaluator))
                    lastRegion = content.Scan(_scanner, groupRange, paperRegion, contentsRegion, contentRegion, contentState, evaluator)
                    If lastRegion IsNot Nothing Then
                        region = lastRegion.GetMergedRegion(region)
                    End If
                Next
            End With
            scanner.AfterGroup(Me, contentRange, parentRegion, groupState, region, _scanner)
            Return region
        End Function

        Public Function GetAggregateSrcContent() As Content
            For Each c As Content In Me.Contents
                If c.Design.AggregateSrc Then
                    Return c
                End If
            Next
            Return Nothing
        End Function

        Public Function GetNextContent(ch As ContentHistory) As Content
            If ch Is Nothing Then
                Return Me.Contents(0).GetNextContent(Nothing)
            Else
                Dim c As Content = ch.Content.GetNextContent(ch)
                If c IsNot Nothing Then
                    Return c
                Else
                    Dim i As Integer = ch.Content.GetIndex
                    If i < Me.Contents.Count - 1 Then
                        Return Me.Contents(i + 1).GetNextContent(Nothing)
                    Else
                        Return Nothing
                    End If
                End If
            End If
        End Function

        Public Function GetDesign() As GroupDesign
            Return Me.ParentGroups.Design
        End Function

        Public Function GetReport() As Report
            Return Me.ParentGroups.Report
        End Function

        Public Overrides Function ToString() As String
            Return Me.ParentGroups.ToString & "[" & Me.Index & "]"
        End Function

    End Class

End Namespace
