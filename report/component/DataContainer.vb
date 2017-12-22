Imports jp.co.systembase.report.data

Namespace component

    Public Class DataContainer

        Public Class PageRange

            Public BeginIndex As Integer = -1
            Public EndIndex As Integer = -1
            Public PastIndex As Integer = -1

            Public Sub UpdateRange(data As ReportData)
                If Me.BeginIndex = -1 Then
                    Me.BeginIndex = data.BeginIndex
                End If
                Me.EndIndex = data.EndIndex
                Me.PastIndex = Me.EndIndex
            End Sub

            Public Sub UpdatePast(index As Integer)
                Me.PastIndex = index
            End Sub

            Public Function GetPresentIndex(prior As Boolean) As Integer
                If prior And Me.BeginIndex > -1 Then
                    Return Me.BeginIndex
                Else
                    Return Me.PastIndex
                End If
            End Function

        End Class

        Public PageRangeMap As New Dictionary(Of ReportData, PageRange)

        Public Sub InitializeData(g As Group)
            If Not Me.PageRangeMap.ContainsKey(g.GetReport.Data) Then
                Me.PageRangeMap.Add(g.GetReport.Data, New PageRange)
            End If
            Me.PageRangeMap.Add(g.Data, New PageRange)
        End Sub

        Public Sub UpdateData(c As Content)
            Dim leaf As Boolean = isLeafContent(c)
            Dim pre As Boolean = isPriorContent(c)
            Dim past As Boolean = isPosteriorContent(c)
            Dim data As ReportData = c.GetData
            Dim _data As ReportData = data
            Do While _data IsNot Nothing
                If Not Me.PageRangeMap.ContainsKey(_data) Then
                    Exit Do
                End If
                If Not _data.HasSameSource(data) Then
                    Exit Do
                End If
                If leaf Then
                    Me.PageRangeMap(_data).UpdateRange(data)
                ElseIf pre Then
                    Me.PageRangeMap(_data).UpdatePast(data.BeginIndex)
                ElseIf past Then
                    Me.PageRangeMap(_data).UpdatePast(data.EndIndex)
                End If
                If Not _data.IsAggregateSrc Then
                    Exit Do
                End If
                _data = _data.GetParentData
            Loop
        End Sub

        Private Shared Function isLeafContent(c As Content) As Boolean
            If Not c.Design.AggregateSrc Then
                Return False
            End If
            Dim gs As Groups = c.Groups
            If gs Is Nothing Then
                Return True
            End If
            If gs.DataOverridden Then
                Return True
            End If
            If gs.Design.GetAggregateSrcContentDesign() Is Nothing Then
                Return True
            End If
            Return False
        End Function

        Private Shared Function isPriorContent(c As Content) As Boolean
            Dim ac As Content = c.ParentGroup.GetAggregateSrcContent
            Return ac IsNot Nothing AndAlso c.GetIndex < ac.GetIndex
        End Function

        Private Shared Function isPosteriorContent(c As Content) As Boolean
            Dim ac As Content = c.ParentGroup.GetAggregateSrcContent
            Return ac IsNot Nothing AndAlso c.GetIndex > ac.GetIndex
        End Function

        Public Function GetPresentData(content As Content, scope As String) As ReportData
            Dim scopeData As ReportData = content.GetData.FindScope(scope)
            If scopeData Is Nothing Then
                Throw New ArgumentException("invalid scope" & IIf(Not scope.Equals(""), ": " & scope, ""))
            End If
            If Not scopeData.HasSameSource(content.GetData) Then
                Throw New ArgumentException("data was overridden")
            End If
            If Not Me.PageRangeMap.ContainsKey(content.GetData) Then
                Return ReportData.GetEmptyData(scopeData)
            End If
            Return New ReportData(scopeData, scopeData.BeginIndex, _
              Me.PageRangeMap(content.GetData).GetPresentIndex(isPriorContent(content)))
        End Function

        Public Function GetPageData(content As Content, scope As String) As ReportData
            Dim scopeData As ReportData = content.GetData.FindScope(scope)
            If scopeData Is Nothing Then
                Throw New ArgumentException("invalid scope" & IIf(Not scope.Equals(""), ": " & scope, ""))
            End If
            If Not scopeData.HasSameSource(content.GetData) Then
                Throw New ArgumentException("data was overridden")
            End If
            If Not Me.PageRangeMap.ContainsKey(scopeData) Then
                Return ReportData.GetEmptyData(scopeData)
            End If
            Dim pageRange As PageRange = Me.PageRangeMap(scopeData)
            Return New ReportData(scopeData, pageRange.BeginIndex, pageRange.EndIndex)
        End Function

    End Class

End Namespace