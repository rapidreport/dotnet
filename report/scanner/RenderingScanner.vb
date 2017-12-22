Imports jp.co.systembase.report.component

Namespace scanner

    Public Class RenderingScanner
        Inherits DefaultScanner

        Public ContentInstances As New List(Of ContentInstance)
        Public DataContainer As DataContainer
        Public DisplayedGroups As Dictionary(Of String, Group)

        Public Sub New()
            Me.DataContainer = New DataContainer
            Me.DisplayedGroups = New Dictionary(Of String, Group)
        End Sub

        Public Sub New(parent As RenderingScanner)
            Me.DataContainer = parent.DataContainer
            Me.DisplayedGroups = parent.DisplayedGroups
        End Sub

        Public Overrides Function BeforeGroup( _
          group As component.Group, _
          contentRange As ContentRange, _
          parentRegion As Region, _
          groupState As GroupState) As IScanner
            Me.DataContainer.InitializeData(group)
            With Nothing
                Dim id As String = group.GetDesign.Id
                If id IsNot Nothing AndAlso Not Me.DisplayedGroups.ContainsKey(id) Then
                    Me.DisplayedGroups.Add(id, group)
                End If
            End With
            Return Me
        End Function

        Public Overrides Function BeforeContent( _
          content As Content, _
          groupRange As GroupRange, _
          parentRegion As Region, _
          contentState As ContentState) As IScanner
            Return New RenderingScanner(Me)
        End Function

        Public Overrides Sub AfterContent( _
          content As Content, _
          groupRange As GroupRange, _
          parentRegion As Region, _
          contentState As ContentState, _
          region As Region, _
          scanner As IScanner)
            If region IsNot Nothing Then
                Me.ContentInstances.Add(New ContentInstance(content, region, contentState))
                Me.ContentInstances.AddRange(CType(scanner, RenderingScanner).ContentInstances)
            End If
            If content.BaseContent Is Nothing AndAlso contentState.Intrinsic Then
                Me.DataContainer.UpdateData(content)
            End If
        End Sub

        Public Overrides Sub ScanSubContent( _
          content As Content, _
          parentRegion As Region, _
          contentState As ContentState, _
          region As Region, _
          paperRegion As Region)
            If region IsNot Nothing AndAlso content.SubContents IsNot Nothing Then
                Dim _region As New Region(region)
                _region.MaxBottom = _region.Bottom
                _region.MaxRight = _region.Right
                For Each c As Content In content.SubContents
                    Dim evaluator As New Evaluator(c, contentState)
                    If Not c.GetReport.InDesigner AndAlso c.Design.VisibilityCond IsNot Nothing Then
                        If Not ReportUtil.Condition(evaluator.EvalTry(c.Design.VisibilityCond)) Then
                            Continue For
                        End If
                    End If
                    Dim gr As GroupRange = Nothing
                    If c.Groups IsNot Nothing Then
                        gr = New GroupRange(c.Groups)
                    End If
                    c.Scan(Me, gr, paperRegion, parentRegion, _
                           c.Design.Layout.GetRegion(_region), contentState, evaluator)
                Next
            End If
        End Sub

    End Class

End Namespace