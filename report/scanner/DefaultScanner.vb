Imports jp.co.systembase.report.component

Namespace scanner
    Public Class DefaultScanner
        Implements IScanner

        Public Function BeforeGroups( _
          groups As Groups, _
          groupRange As GroupRange, _
          parentRegion As Region) As IScanner Implements IScanner.BeforeGroups
            Return Me
        End Function

        Public Overridable Sub AfterGroups( _
          groups As Groups, _
          groupRange As GroupRange, _
          parentRegion As Region, _
          region As Region, _
          broken As Boolean, _
          scanner As IScanner) Implements IScanner.AfterGroups
        End Sub

        Public Overridable Function BeforeGroup( _
          group As Group, _
          contentRange As ContentRange, _
          parentRegion As Region, _
          groupState As GroupState) As IScanner Implements IScanner.BeforeGroup
            Return Me
        End Function

        Public Overridable Sub AfterGroup( _
          group As Group, _
          contentRange As ContentRange, _
          parentRegion As Region, _
          groupState As GroupState, _
          region As Region, _
          scanner As IScanner) Implements IScanner.AfterGroup
        End Sub

        Public Overridable Function BeforeContent( _
          content As Content, _
          groupRange As GroupRange, _
          parentRegion As Region, _
          contentState As ContentState) As IScanner Implements IScanner.BeforeContent
            Return Me
        End Function

        Public Overridable Sub AfterContent( _
          content As Content, _
          groupRange As GroupRange, _
          parentRegion As Region, _
          contentState As ContentState, _
          region As Region, _
          scanner As IScanner) Implements IScanner.AfterContent
        End Sub

        Public Overridable Sub ScanSubContent( _
          content As Content, _
          parentRegion As Region, _
          contentState As ContentState, _
          region As Region, _
          paperRegion As Region) Implements IScanner.ScanSubContent
        End Sub

    End Class

End Namespace

