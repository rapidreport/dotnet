Imports jp.co.systembase.report.component

Namespace scanner
    Public Class DefaultScanner
        Implements IScanner

        Public Function BeforeGroups( _
          ByVal groups As Groups, _
          ByVal groupRange As GroupRange, _
          ByVal parentRegion As Region) As IScanner Implements IScanner.BeforeGroups
            Return Me
        End Function

        Public Overridable Sub AfterGroups( _
          ByVal groups As Groups, _
          ByVal groupRange As GroupRange, _
          ByVal parentRegion As Region, _
          ByVal region As Region, _
          ByVal broken As Boolean, _
          ByVal scanner As IScanner) Implements IScanner.AfterGroups
        End Sub

        Public Overridable Function BeforeGroup( _
          ByVal group As Group, _
          ByVal contentRange As ContentRange, _
          ByVal parentRegion As Region, _
          ByVal groupState As GroupState) As IScanner Implements IScanner.BeforeGroup
            Return Me
        End Function

        Public Overridable Sub AfterGroup( _
          ByVal group As Group, _
          ByVal contentRange As ContentRange, _
          ByVal parentRegion As Region, _
          ByVal groupState As GroupState, _
          ByVal region As Region, _
          ByVal scanner As IScanner) Implements IScanner.AfterGroup
        End Sub

        Public Overridable Function BeforeContent( _
          ByVal content As Content, _
          ByVal groupRange As GroupRange, _
          ByVal parentRegion As Region, _
          ByVal contentState As ContentState) As IScanner Implements IScanner.BeforeContent
            Return Me
        End Function

        Public Overridable Sub AfterContent( _
          ByVal content As Content, _
          ByVal groupRange As GroupRange, _
          ByVal parentRegion As Region, _
          ByVal contentState As ContentState, _
          ByVal region As Region, _
          ByVal scanner As IScanner) Implements IScanner.AfterContent
        End Sub

        Public Overridable Sub ScanSubContent( _
          ByVal content As Content, _
          ByVal parentRegion As Region, _
          ByVal contentState As ContentState, _
          ByVal region As Region, _
          ByVal paperRegion As Region) Implements IScanner.ScanSubContent
        End Sub

    End Class

End Namespace

