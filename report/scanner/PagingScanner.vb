Imports jp.co.systembase.report.component

Namespace scanner

    Public Class PagingScanner
        Inherits DefaultScanner

        Public AppearedGroups As New Dictionary(Of GroupDesign, Group)
        Public StartedGroups As New Dictionary(Of GroupDesign, Group)
        Public FinishedGroups As New Dictionary(Of GroupDesign, Group)
        Public Broken As Boolean = False
        Public Weight As Integer = 0

        Public Overrides Sub AfterContent( _
          content As Content, _
          groupRange As GroupRange, _
          parentRegion As Region, _
          contentState As ContentState, _
          region As Region, _
          scanner As IScanner)
            If region IsNot Nothing Then
                If region.IsVOverflowed Then
                    Me.Broken = True
                End If
                If region.IsHOverflowed Then
                    Me.Broken = True
                End If
                Me.Weight += content.Design.Weight
            End If
        End Sub

        Public Overrides Sub AfterGroup( _
          group As Group, _
          contentRange As ContentRange, _
          parentRegion As Region, _
          groupState As GroupState, _
          region As Region, _
          scanner As IScanner)
            Dim gd As GroupDesign = group.GetDesign
            If gd.PageBreak AndAlso Me.AppearedGroups.ContainsKey(gd) Then
                Me.Broken = True
            End If
            If Not Me.AppearedGroups.ContainsKey(gd) Then
                Me.AppearedGroups.Add(gd, group)
            End If
            If contentRange.First.GroupFirst Then
                If Not Me.StartedGroups.ContainsKey(gd) Then
                    Me.StartedGroups.Add(gd, group)
                End If
            End If
            If contentRange.Last Is Nothing OrElse _
               contentRange.Last.GroupLast Then
                If Not Me.FinishedGroups.ContainsKey(gd) Then
                    Me.FinishedGroups.Add(gd, group)
                End If
            End If
        End Sub

        Public Overrides Sub AfterGroups( _
          groups As Groups, _
          groupRange As GroupRange, _
          parentRegion As Region, _
          region As Region, _
          broken As Boolean, _
          scanner As IScanner)
            If Not groups.Design.Layout.ClipOverflow AndAlso broken Then
                Me.Broken = True
            End If
        End Sub

    End Class

End Namespace
