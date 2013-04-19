Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.scanner

Namespace component

    Public Class Content

        Public Design As ContentDesign
        Public Groups As Groups = Nothing
        Public SubContents As List(Of Content) = Nothing

        Public ParentGroup As Group
        Public BaseContent As Content

        Public Sub New( _
          ByVal design As ContentDesign, _
          ByVal parentGroup As Group)
            Me.New(design, parentGroup, Nothing)
        End Sub

        Public Sub New( _
          ByVal design As ContentDesign, _
          ByVal parentGroup As Group, _
          ByVal baseContent As Content)
            Me.Design = design
            Me.ParentGroup = parentGroup
            Me.BaseContent = baseContent
        End Sub

        Public Sub Fill(ByVal data As ReportData)
            If Me.Design.GroupDesign IsNot Nothing Then
                Me.Groups = New Groups(Me.Design.GroupDesign, Me.GetReport, Me)
                Me.Groups.Fill(data)
            End If
            If Me.Design.SubContentDesigns IsNot Nothing Then
                Me.SubContents = New List(Of Content)
                For Each cd As ContentDesign In Me.Design.SubContentDesigns
                    Dim c As New Content(cd, Me.ParentGroup, Me)
                    Me.SubContents.Add(c)
                    c.Fill(data)
                Next
            End If
        End Sub

        Public Function Scan( _
          ByVal scanner As IScanner, _
          ByVal groupRange As GroupRange, _
          ByVal paperRegion As Region, _
          ByVal parentRegion As Region, _
          ByVal region As Region, _
          ByVal contentState As ContentState, _
          ByVal evaluator As Evaluator) As Region
            Dim _scanner As IScanner = scanner.BeforeContent(Me, groupRange, parentRegion, contentState)
            Dim _region As Region = region
            If Me.GetReport.Customizer IsNot Nothing Then
                _region = Me.GetReport.Customizer.ContentRegion(Me, evaluator, _region)
            End If
            If _region IsNot Nothing AndAlso Not contentState.GroupState.Blank Then
                If Me.Groups IsNot Nothing AndAlso groupRange IsNot Nothing Then
                    _region = Me.Groups.Scan(_scanner, groupRange, paperRegion, _region, contentState)
                End If
            End If
            scanner.AfterContent(Me, groupRange, parentRegion, contentState, _region, _scanner)
            scanner.ScanSubContent(Me, parentRegion, contentState, _region, paperRegion)
            Return _region
        End Function

        Public Function GetNextContent(ByVal ch As ContentHistory) As Content
            If ch Is Nothing Then
                If Me.Groups Is Nothing OrElse Me.Groups.Groups.Count = 0 Then
                    Return Me
                Else
                    Return Me.Groups.GetNextContent(Nothing)
                End If
            Else
                If Me.Groups Is Nothing OrElse Me.Groups.Groups.Count = 0 Then
                    Return Nothing
                Else
                    Return Me.Groups.GetNextContent(ch.Child)
                End If
            End If
        End Function

        Public Function GetIndex() As Integer
            If Me.BaseContent IsNot Nothing Then
                Return Me.BaseContent.GetIndex
            Else
                Return Me.ParentGroup.Contents.IndexOf(Me)
            End If
        End Function

        Public Function GetSubIndex() As Integer
            If Me.BaseContent IsNot Nothing Then
                Me.BaseContent.SubContents.IndexOf(Me)
            Else
                Return -1
            End If
        End Function

        Public Function GetData() As ReportData
            Return Me.ParentGroup.Data
        End Function

        Public Function GetGroups() As Groups
            Return Me.ParentGroup.ParentGroups
        End Function

        Public Function GetReport() As Report
            Return Me.ParentGroup.ParentGroups.Report
        End Function

        Public Overrides Function ToString() As String
            Dim ret As String = Me.ParentGroup.ToString
            If Me.Design.Caption IsNot Nothing Then
                ret &= Me.Design.Caption
            ElseIf Me.Design.Id IsNot Nothing Then
                ret &= Me.Design.Id
            Else
                ret &= "(content)"
            End If
            ret &= "[" & Me.GetIndex & "]"

            Return ret
        End Function

    End Class

End Namespace