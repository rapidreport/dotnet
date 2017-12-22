Imports jp.co.systembase.report.component

Namespace scanner

    Public Interface IScanner
        Function BeforeContent( _
          content As Content, _
          groupRange As GroupRange, _
          parentRegion As Region, _
          contentState As ContentState) As IScanner
        Sub AfterContent( _
          content As Content, _
          groupRange As GroupRange, _
          parentRegion As Region, _
          contentState As ContentState, _
          region As Region, _
          scanner As IScanner)
        Function BeforeGroups( _
          groups As Groups, _
          groupRange As GroupRange, _
          parentRegion As Region) As IScanner
        Sub AfterGroups( _
          groups As Groups, _
          groupRange As GroupRange, _
          parentRegion As Region, _
          region As Region, _
          broken As Boolean, _
          scanner As IScanner)
        Function BeforeGroup( _
          group As Group, _
          contentRange As ContentRange, _
          parentContext As Region, _
          groupState As GroupState) As IScanner
        Sub AfterGroup( _
          group As Group, _
          contentRange As ContentRange, _
          parentRegion As Region, _
          groupState As GroupState, _
          region As Region, _
          scanner As IScanner)
        Sub ScanSubContent( _
          content As Content, _
          parentRegion As Region, _
          contentState As ContentState, _
          region As Region, _
          paperRegion As Region)
    End Interface

End Namespace