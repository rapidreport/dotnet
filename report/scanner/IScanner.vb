Imports jp.co.systembase.report.component

Namespace scanner

    Public Interface IScanner
        Function BeforeContent( _
          ByVal content As Content, _
          ByVal groupRange As GroupRange, _
          ByVal parentRegion As Region, _
          ByVal contentState As ContentState) As IScanner
        Sub AfterContent( _
          ByVal content As Content, _
          ByVal groupRange As GroupRange, _
          ByVal parentRegion As Region, _
          ByVal contentState As ContentState, _
          ByVal region As Region, _
          ByVal scanner As IScanner)
        Function BeforeGroups( _
          ByVal groups As Groups, _
          ByVal groupRange As GroupRange, _
          ByVal parentRegion As Region) As IScanner
        Sub AfterGroups( _
          ByVal groups As Groups, _
          ByVal groupRange As GroupRange, _
          ByVal parentRegion As Region, _
          ByVal region As Region, _
          ByVal broken As Boolean, _
          ByVal scanner As IScanner)
        Function BeforeGroup( _
          ByVal group As Group, _
          ByVal contentRange As ContentRange, _
          ByVal parentContext As Region, _
          ByVal groupState As GroupState) As IScanner
        Sub AfterGroup( _
          ByVal group As Group, _
          ByVal contentRange As ContentRange, _
          ByVal parentRegion As Region, _
          ByVal groupState As GroupState, _
          ByVal region As Region, _
          ByVal scanner As IScanner)
        Sub ScanSubContent( _
          ByVal content As Content, _
          ByVal parentRegion As Region, _
          ByVal contentState As ContentState, _
          ByVal region As Region, _
          ByVal paperRegion As Region)
    End Interface

End Namespace