Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.data.internal
Imports jp.co.systembase.report.scanner

Namespace component

    Public Class Groups

        Public Design As GroupDesign = Nothing
        Public Report As Report = Nothing
        Public ParentContent As Content = Nothing
        Public Groups As List(Of Group) = Nothing
        Public DataOverridden As Boolean = False
        Public DataSourceGroup As Group = Nothing

        Public Sub New(
          design As GroupDesign,
          report As Report,
          parentContent As Content)
            Me.Design = design
            Me.Report = report
            Me.ParentContent = parentContent
        End Sub

        Public Sub Fill(dataSourceGroup As Group)
            Me.DataSourceGroup = dataSourceGroup
            Me.Fill(New ReportData(Me.DataSourceGroup.Data))
        End Sub

        Public Sub Fill(data As ReportData)
            Me.Groups = New List(Of Group)
            Dim _data As ReportData = data
            If Me.Report.InDesigner Then
                Me.AddGroup(New ReportData(_data))
            Else
                If Me.Design.BlankData Then
                    _data = New ReportData(BlankDataSource.GetInstance, _data.Report, _data.Group)
                End If
                With Nothing
                    Dim dataSource As IReportDataSource = _data
                    If Me.Report.GroupDataProvider IsNot Nothing Then
                        dataSource = Me.Report.GroupDataProvider.GetGroupDataSource(Me, data)
                    End If
                    If Me.Design.SortKeys IsNot Nothing Then
                        dataSource = New SortedDataSource(dataSource, Me.Design.SortKeys, _data.Context.GetLogger)
                    End If
                    If dataSource IsNot data Then
                        _data = New ReportData(dataSource, data.Report, data.Group)
                        Me.DataOverridden = True
                    End If
                End With
                If Not Crosstab.Fill(Me, _data) Then
                    Dim dataList As List(Of ReportData)
                    If Me.Design.SplitString IsNot Nothing Then
                        dataList = Me.Design.SplitString.Split(_data)
                    Else
                        dataList = Me.Design.DataSplit(_data)
                    End If
                    For Each d As ReportData In dataList
                        Me.AddGroup(d)
                    Next
                End If
            End If
        End Sub

        Public Sub AddGroup(data As ReportData)
            Me.AddGroup(data, Nothing)
        End Sub
        Public Sub AddGroup(data As ReportData, crosstabState As Crosstab.State)
            Dim g As New Group(Me, Me.Groups.Count, crosstabState)
            Me.Groups.Add(g)
            g.Fill(data, True)
        End Sub

        Public Function Scan(
          scanner As IScanner,
          groupRange As GroupRange,
          paperRegion As Region) As Region
            Return Me.Scan(scanner, groupRange, paperRegion, paperRegion, Nothing, New Evaluator(Me.Report.Data))
        End Function

        Public Function Scan(
          scanner As IScanner,
          groupRange As GroupRange,
          paperRegion As Region,
          parentRegion As Region,
          parentState As ContentState,
          evaluator As Evaluator) As Region
            Dim _scanner As IScanner = scanner.BeforeGroups(Me, groupRange, parentRegion)
            Dim region As Region = parentRegion
            Dim i As Integer = 0
            Dim isFirst As Boolean = True
            Dim filledCount As Integer = groupRange.GetGroupCount
            evaluator.GroupLayoutFilledCount = filledCount
            Dim layoutCount As Integer = Me.Design.Layout.GetCount(IIf(Me.Report.InDesigner, Nothing, evaluator))
            Dim lastIndex As Integer
            Dim lastIndex2 As Integer
            Dim lastRegion As Region = Nothing
            If Me.Design.Layout.Blank AndAlso layoutCount = 0 Then
                layoutCount = Me.getDefaultGroupCount(parentRegion)
            End If
            If parentState IsNot Nothing AndAlso parentState.GroupState.Blank Then
                If layoutCount > 0 Then
                    lastIndex = 0
                    lastIndex2 = IIf(Me.Design.Layout.Blank, layoutCount, 1)
                Else
                    lastIndex = 0
                    lastIndex2 = 1
                End If
            Else
                If layoutCount > 0 Then
                    lastIndex = Math.Min(filledCount, layoutCount)
                    lastIndex2 = IIf(Me.Design.Layout.Blank, layoutCount, lastIndex)
                Else
                    lastIndex = filledCount
                    lastIndex2 = lastIndex
                End If
            End If
            Do
                If i = lastIndex2 Then
                    Exit Do
                End If
                Dim g As Group
                Dim contentRange As ContentRange
                If i < lastIndex Then
                    g = groupRange.GetGroup(i)
                    contentRange = groupRange.GetSubRange(g)
                Else
                    g = Me.createBlankGroup
                    contentRange = New ContentRange(g)
                End If
                Dim groupState As New GroupState(parentState)
                groupState.First = isFirst
                groupState.Last = (i = lastIndex - 1)
                groupState.Last2 = (i = lastIndex2 - 1)
                groupState.Index = i
                groupState.GroupFirst = groupRange.ContainsFirst And isFirst
                groupState.GroupLast = groupState.Last And groupRange.ContainsLast
                groupState.GroupLast2 = groupState.Last2 And groupRange.ContainsLast
                groupState.GroupIndex = g.Index
                groupState.Blank = (i >= lastIndex)
                groupState.BlankFirst = (i = lastIndex)
                groupState.BlankLast = groupState.Blank And groupState.Last2
                Dim groupRegion As Region = Me.Design.Layout.GetGroupRegion(parentRegion, lastRegion, i)
                lastRegion = g.Scan(_scanner, contentRange, paperRegion, groupRegion, groupState)
                region = extendRegion(region, lastRegion)
                isFirst = False
                i += 1
            Loop
            Dim broken As Boolean = (layoutCount > 0 And layoutCount < filledCount)
            scanner.AfterGroups(Me, groupRange, parentRegion, region, broken, _scanner)
            Return region
        End Function

        Private Function extendRegion(region As Region, groupRegion As Region) As Region
            Dim ret As New Region
            ret.Top = region.Top
            ret.Left = region.Left
            ret.MaxBottom = region.MaxBottom
            ret.MaxRight = region.MaxRight
            ret.Bottom = Math.Max(region.Bottom, groupRegion.Bottom)
            ret.Right = Math.Max(region.Right, groupRegion.Right)
            Return ret
        End Function

        Private Function getDefaultGroupCount(parentRegion As Region) As Integer
            Dim ret As Integer = 0
            Dim u As Single = 0
            If Me.Design.ContentDesigns.Count = 1 Then
                Dim d As ContentDesign = Me.Design.ContentDesigns(0)
                If d.GroupDesign Is Nothing AndAlso Not d.Size.RevInitial Then
                    u = d.Size.Initial
                End If
            End If
            If u > 0 Then
                Dim t As Single = 0
                Dim _t As Single = 0
                Select Case Me.Design.Layout.Direction
                    Case Report.EDirection.VERTICAL
                        t = parentRegion.GetMaxHeight
                    Case Report.EDirection.HORIZONTAL
                        t = parentRegion.GetMaxWidth
                End Select
                Do
                    _t += u
                    If _t <= t Then
                        ret += 1
                    Else
                        Exit Do
                    End If
                Loop
            End If
            Return ret
        End Function

        Private Function createBlankGroup() As Group
            Dim ret As New Group(Me, -1, Nothing)
            Dim g As Group = Nothing
            If Me.ParentContent IsNot Nothing Then
                g = Me.ParentContent.ParentGroup
            End If
            ret.Fill(New ReportData(DummyDataSource.GetInstance, Me.Report, g), False)
            Return ret
        End Function

        Public Function GetNextContent(ch As ContentHistory) As Content
            If Me.Groups.Count = 0 Then
                Return Nothing
            End If
            If ch Is Nothing Then
                Return Me.Groups(0).GetNextContent(Nothing)
            Else
                Dim c As Content = ch.Group.GetNextContent(ch)
                If c IsNot Nothing Then
                    Return c
                Else
                    Dim i As Integer = ch.Group.Index
                    If i < Me.Groups.Count - 1 Then
                        Return Me.Groups(i + 1).GetNextContent(Nothing)
                    Else
                        Return Nothing
                    End If
                End If
            End If
        End Function

        Public Function GetNextContentHistory(ch As ContentHistory) As ContentHistory
            Dim c As Content = Me.GetNextContent(ch)
            If c IsNot Nothing Then
                Return ContentHistory.GetInstance(c, Me)
            Else
                Return Nothing
            End If
        End Function

        Public Overrides Function ToString() As String
            Dim p As String
            If Me.ParentContent IsNot Nothing Then
                p = Me.ParentContent.ToString & ":"
            Else
                p = Me.Report.ToString & ":"
            End If
            If Me.Design.Caption IsNot Nothing Then
                Return p & Me.Design.Caption
            ElseIf Me.Design.Id IsNot Nothing Then
                Return p & Me.Design.Id
            Else
                Return p & "(group)"
            End If
        End Function

    End Class

End Namespace