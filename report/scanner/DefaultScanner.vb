Imports jp.co.systembase.report.component

Namespace scanner
    Public Class DefaultScanner
        Implements IScanner

        Public Function BeforeGroups(
          groups As Groups,
          groupRange As GroupRange,
          parentRegion As Region) As IScanner Implements IScanner.BeforeGroups
            Return Me
        End Function

        Public Overridable Sub AfterGroups(
          groups As Groups,
          groupRange As GroupRange,
          parentRegion As Region,
          region As Region,
          broken As Boolean,
          scanner As IScanner) Implements IScanner.AfterGroups
        End Sub

        Public Overridable Function BeforeGroup(
          group As Group,
          contentRange As ContentRange,
          parentRegion As Region,
          groupState As GroupState) As IScanner Implements IScanner.BeforeGroup
            Return Me
        End Function

        Public Overridable Sub AfterGroup(
          group As Group,
          contentRange As ContentRange,
          parentRegion As Region,
          groupState As GroupState,
          region As Region,
          scanner As IScanner) Implements IScanner.AfterGroup
        End Sub

        Public Overridable Function BeforeContent(
          content As Content,
          groupRange As GroupRange,
          parentRegion As Region,
          contentState As ContentState) As IScanner Implements IScanner.BeforeContent
            Return Me
        End Function

        Public Overridable Sub AfterContent(
          content As Content,
          groupRange As GroupRange,
          parentRegion As Region,
          contentState As ContentState,
          region As Region,
          scanner As IScanner) Implements IScanner.AfterContent
        End Sub

        Public Overridable Sub ScanSubContent(
          content As Content,
          parentRegion As Region,
          contentState As ContentState,
          region As Region,
          paperRegion As Region,
          background As Boolean) Implements IScanner.ScanSubContent
        End Sub

    End Class

End Namespace

