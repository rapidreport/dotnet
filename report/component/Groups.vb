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

        Public Sub New( _
          ByVal design As GroupDesign, _
          ByVal report As Report, _
          ByVal parentContent As Content)
            Me.Design = design
            Me.Report = report
            Me.ParentContent = parentContent
        End Sub

        Public Sub Fill(ByVal dataSourceGroup As Group)
            Me.DataSourceGroup = dataSourceGroup
            Me.Fill(New ReportData(Me.DataSourceGroup.Data))
        End Sub

        Public Sub Fill(ByVal data As ReportData)
            Me.Groups = New List(Of Group)
            Dim _data As ReportData = data
            If Not Me.Report.InDesigner AndAlso Me.Design.BlankData Then
                _data = New ReportData(BlankDataSource.GetInstance, _
                                       _data.Report, _
                                       _data.Group)
            End If
            If Me.Report.GroupDataProvider IsNot Nothing Then
                Dim dataSource As IReportDataSource = _
                  Me.Report.GroupDataProvider.GetGroupDataSource(Me, data)
                If dataSource IsNot data Then
                    _data = New ReportData(dataSource, data.Report, data.Group)
                    Me.DataOverridden = True
                End If
            End If
            If Me.Design.SortKeys IsNot Nothing Then
                _data = New ReportData( _
                  New SortedDataSource(_data, Me.Design.SortKeys), _
                  _data.Report, _
                  _data.Group)
                Me.DataOverridden = True
            End If
            Dim index As Integer = 0
            Dim dataList As List(Of ReportData)
            If Me.Design.SplitString IsNot Nothing Then
                dataList = Me.Design.SplitString.Split(_data)
            Else
                dataList = Me.Design.DataSplit(_data)
            End If
            For Each d As ReportData In dataList
                Dim g As New Group(Me, index)
                Me.Groups.Add(g)
                g.Fill(d)
                index += 1
            Next
        End Sub

        Public Function Scan( _
          ByVal scanner As IScanner, _
          ByVal groupRange As GroupRange, _
          ByVal paperRegion As Region) As Region
            Return Me.Scan(scanner, groupRange, paperRegion, paperRegion, Nothing)
        End Function

        Public Function Scan( _
          ByVal scanner As IScanner, _
          ByVal groupRange As GroupRange, _
          ByVal paperRegion As Region, _
          ByVal parentRegion As Region, _
          ByVal parentState As ContentState) As Region
            Dim _scanner As IScanner = scanner.BeforeGroups(Me, groupRange, parentRegion)
            Dim region As Region = parentRegion
            Dim i As Integer = 0
            Dim isFirst As Boolean = True
            Dim layoutCount As Integer = Me.Design.Layout.GetCount
            Dim lastIndex As Integer
            Dim lastIndex2 As Integer
            Dim lastRegion As Region = Nothing
            If layoutCount = 0 And Me.Design.Layout.Blank Then
                layoutCount = Me.getInitialGroupCount(parentRegion)
            End If
            If layoutCount > 0 Then
                If Me.Design.Layout.Blank Then
                    lastIndex2 = layoutCount
                    lastIndex = Math.Min(lastIndex2, groupRange.GetGroupCount)
                Else
                    lastIndex = Math.Min(groupRange.GetGroupCount, layoutCount)
                    lastIndex2 = lastIndex
                End If
            Else
                lastIndex = groupRange.GetGroupCount
                lastIndex2 = lastIndex
            End If
            Do
                If i = lastIndex2 Then
                    Exit Do
                End If
                Dim g As Group
                Dim contentRange As ContentRange
                If i < groupRange.GetGroupCount Then
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
                groupState.Blank = (i >= groupRange.GetGroupCount)
                groupState.BlankFirst = (i = groupRange.GetGroupCount)
                groupState.BlankLast = groupState.Blank And groupState.Last2
                Dim groupRegion As Region = Me.Design.Layout.GetGroupRegion(parentRegion, lastRegion, i)
                lastRegion = g.Scan(_scanner, contentRange, paperRegion, groupRegion, groupState)
                region = extendRegion(region, lastRegion)
                isFirst = False
                i += 1
            Loop
            Dim broken As Boolean = (layoutCount > 0 And layoutCount < groupRange.GetGroupCount)
            scanner.AfterGroups(Me, groupRange, parentRegion, region, broken, _scanner)
            Return region
        End Function

        Private Function extendRegion(ByVal region As Region, ByVal groupRegion As Region) As Region
            Dim ret As New Region
            ret.Top = region.Top
            ret.Left = region.Left
            ret.MaxBottom = region.MaxBottom
            ret.MaxRight = region.MaxRight
            ret.Bottom = Math.Max(region.Bottom, groupRegion.Bottom)
            ret.Right = Math.Max(region.Right, groupRegion.Right)
            Return ret
        End Function

        Private Function getInitialGroupCount(ByVal parentRegion As Region) As Integer
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
            Dim ret As New Group(Me, -1)
            Dim g As Group = Nothing
            If Me.ParentContent IsNot Nothing Then
                g = Me.ParentContent.ParentGroup
            End If
            ret.Fill(New ReportData(DummyDataSource.GetInstance, Me.Report, g))
            Return ret
        End Function

        Public Function GetNextContent(ByVal ch As ContentHistory) As Content
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

        Public Function GetNextContentHistory(ByVal ch As ContentHistory) As ContentHistory
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