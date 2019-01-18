Imports jp.co.systembase.report.component

Namespace scanner

    Public Interface IScanner
        Function BeforeContent(
          content As Content,
          groupRange As GroupRange,
          parentRegion As Region,
          contentState As ContentState) As IScanner
        Sub AfterContent(
          content As Content,
          groupRange As GroupRange,
          parentRegion As Region,
          contentState As ContentState,
          region As Region,
          scanner As IScanner)
        Function BeforeGroups(
          groups As Groups,
          groupRange As GroupRange,
          parentRegion As Region) As IScanner
        Sub AfterGroups(
          groups As Groups,
          groupRange As GroupRange,
          parentRegion As Region,
          region As Region,
          broken As Boolean,
          scanner As IScanner)
        Function BeforeGroup(
          group As Group,
          contentRange As ContentRange,
          parentContext As Region,
          groupState As GroupState) As IScanner
        Sub AfterGroup(
          group As Group,
          contentRange As ContentRange,
          parentRegion As Region,
          groupState As GroupState,
          region As Region,
          scanner As IScanner)
        Sub ScanSubContent(
          content As Content,
          parentRegion As Region,
          contentState As ContentState,
          region As Region,
          paperRegion As Region,
          background As Boolean)
    End Interface

End Namespace