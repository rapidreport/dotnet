Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data.internal

Namespace data

    Public Class ReportData
        Implements IReportDataSource

        Public DataSource As IReportDataSource = Nothing
        Public Report As Report = Nothing
        Public Group As Group = Nothing
        Public BeginIndex As Integer = -1
        Public EndIndex As Integer = -1
        Public DataCache As DataCache
        Public Logger As IReportLogger

        Public Sub New(ByVal dataSource As IReportDataSource, ByVal dataCache As DataCache, ByVal logger As IReportLogger)
            Me.initialize(dataSource, 0, dataSource.Size, Nothing, Nothing, dataCache, logger)
        End Sub

        Public Sub New(ByVal dataSource As IReportDataSource, ByVal beginIndex As Integer, ByVal endIndex As Integer, ByVal dataCache As DataCache, ByVal logger As IReportLogger)
            Me.initialize(dataSource, beginIndex, endIndex, Nothing, Nothing, dataCache, logger)
        End Sub

        Public Sub New(ByVal dataSource As IReportDataSource, ByVal group As Group)
            Me.initialize(dataSource, 0, dataSource.Size, group.GetReport, group, group.GetReport.DataCache, group.GetReport.Design.Setting.Logger)
        End Sub

        Public Sub New(ByVal dataSource As IReportDataSource, ByVal report As Report, ByVal group As Group)
            Me.initialize(dataSource, 0, dataSource.Size, report, group, report.DataCache, report.Design.Setting.Logger)
        End Sub

        Public Sub New(ByVal dataSource As IReportDataSource, ByVal beginIndex As Integer, ByVal endIndex As Integer, ByVal report As Report, ByVal group As Group)
            Me.initialize(dataSource, beginIndex, endIndex, report, group, report.DataCache, report.Design.Setting.Logger)
        End Sub

        Public Sub New(ByVal data As ReportData)
            Me.initialize(data.DataSource, data.BeginIndex, data.EndIndex, data.Report, data.Group, data.DataCache, data.Logger)
        End Sub

        Public Sub New(ByVal data As ReportData, ByVal beginIndex As Integer, ByVal endIndex As Integer)
            Me.initialize(data.DataSource, beginIndex, endIndex, data.Report, data.Group, data.DataCache, data.Logger)
        End Sub

        Public Shared Function GetPartialData(ByVal data As ReportData, ByVal beginIndex As Integer, ByVal endIndex As Integer)
            Return New ReportData(data, data.BeginIndex + beginIndex, data.BeginIndex + endIndex)
        End Function

        Public Shared Function GetEmptyData(ByVal data As ReportData) As ReportData
            Return GetEmptyData(data.DataSource, data.Report, data.Group)
        End Function

        Public Shared Function GetEmptyData(ByVal dataSource As IReportDataSource, ByVal report As Report, ByVal group As Group) As ReportData
            Return New ReportData(dataSource, -1, -1, report, group)
        End Function

        Private Sub initialize( _
          ByVal dataSource As IReportDataSource, _
          ByVal beginIndex As Integer, _
          ByVal endIndex As Integer, _
          ByVal report As Report, _
          ByVal group As Group, _
          ByVal dataCache As DataCache, _
          ByVal logger As IReportLogger)
            Me.DataSource = dataSource
            Me.Report = report
            Me.Group = group
            Me.DataCache = dataCache
            Me.Logger = logger
            If beginIndex >= 0 And beginIndex < endIndex Then
                Me.BeginIndex = beginIndex
                Me.EndIndex = endIndex
            Else
                Me.BeginIndex = -1
                Me.EndIndex = -1
            End If
        End Sub

        Public Sub SetGroup(ByVal group As Group)
            Me.Group = group
            Dim groupDesign As GroupDesign = group.GetDesign
            If Not Me.Report.WrapperDataSourceMap.ContainsKey(groupDesign) Then
                Me.Report.WrapperDataSourceMap.Add(groupDesign, New WrapperDataSource)
            End If
            Dim wrapperDataSource As WrapperDataSource = Me.Report.WrapperDataSourceMap(groupDesign)
            Dim index As Integer = wrapperDataSource.DataList.Count
            Me.Group.TraversalIndex = index
            With Nothing
                Dim g As Group = Me.Group
                Do
                    If g.ParentGroups.ParentContent Is Nothing Then
                        Exit Do
                    End If
                    g = g.ParentGroups.ParentContent.ParentGroup
                    If Not g.DataIndexRangeMap.ContainsKey(groupDesign) Then
                        g.DataIndexRangeMap.Add(groupDesign, New IndexRange)
                    End If
                    Dim indexRange As IndexRange = g.DataIndexRangeMap(groupDesign)
                    If indexRange.BeginIndex = -1 Then
                        indexRange.BeginIndex = index
                    End If
                    indexRange.EndIndex = index + 1
                Loop
            End With
            wrapperDataSource.DataList.Add(Me)
        End Sub

        Public Function HasSameSource(ByVal data As ReportData) As Boolean
            Return Me.DataSource Is data.DataSource
        End Function

        Public Function IsEmpty() As Boolean
            Return Not (Me.BeginIndex < Me.EndIndex)
        End Function

        Public Function Merge(ByVal data As ReportData) As ReportData
            If Not Me.HasSameSource(data) Then
                Return Me
            End If
            Dim bi As Integer = Me.BeginIndex
            Dim ei As Integer = Me.EndIndex
            If data.BeginIndex <> -1 Then
                If bi = -1 OrElse bi > data.BeginIndex Then
                    bi = data.BeginIndex
                End If
            End If
            If data.EndIndex <> -1 Then
                If ei = -1 OrElse ei < data.EndIndex Then
                    ei = data.EndIndex
                End If
            End If
            Return New ReportData(Me.DataSource, bi, ei, Me.Report, Me.Group)
        End Function

        Public Function [Get](ByVal i As Integer, ByVal key As String) As Object Implements IReportDataSource.[Get]
            If Me.BeginIndex < 0 OrElse i < 0 OrElse i >= Me.Size Then
                Throw New ArgumentOutOfRangeException
            End If
            Dim customField As CustomField = Me.findCustomField(key)
            Try
                If customField IsNot Nothing Then
                    If Me.Report IsNot Nothing Then
                        Me.Report.CustomFieldStack.Push(customField)
                    End If
                    Try
                        Return customField.Get(customField.Data.TransIndex(Me, i))
                    Finally
                        If Me.Report IsNot Nothing Then
                            Me.Report.CustomFieldStack.Pop()
                        End If
                    End Try
                Else
                    Return Me.DataSource.Get(i + Me.BeginIndex, key)
                End If
            Catch ex As EvalException
                If Me.Logger IsNot Nothing Then
                    Me.Logger.EvaluateError(key, ex)
                End If
                Return Nothing
            End Try
        End Function

        Public Function Size() As Integer Implements IReportDataSource.Size
            If Me.BeginIndex = -1 Then
                Return 0
            Else
                Return Me.EndIndex - Me.BeginIndex
            End If
        End Function

        Private Function findCustomField(ByVal key As String) As CustomField
            Dim g As Group = Me.Group
            Do While g IsNot Nothing AndAlso Me.HasSameSource(g.Data)
                Dim gd As GroupDesign = g.GetDesign
                If gd.CustomFields IsNot Nothing AndAlso gd.CustomFields.ContainsKey(key) Then
                    Return New CustomField( _
                      key, _
                      gd.CustomFields(key), _
                      Me.Report, _
                      g.Data)
                End If
                If g.ParentGroups.ParentContent IsNot Nothing Then
                    g = g.ParentGroups.ParentContent.ParentGroup
                ElseIf g.ParentGroups.DataSourceGroup IsNot Nothing Then
                    g = g.ParentGroups.DataSourceGroup
                Else
                    g = Nothing
                End If
            Loop
            If Me.Report IsNot Nothing AndAlso Me.HasSameSource(Me.Report.Data) Then
                Dim rd As ReportDesign = Me.Report.Design
                If rd.CustomFields IsNot Nothing AndAlso rd.CustomFields.ContainsKey(key) Then
                    Return New CustomField( _
                      key, _
                      rd.CustomFields(key), _
                      Me.Report, _
                      Me.Report.Data)
                End If
            End If
            Return Nothing
        End Function

        Public Function TransIndex(ByVal data As ReportData, ByVal i As Integer) As Integer
            Return i + (data.BeginIndex - Me.BeginIndex)
        End Function

        Public Function FindScope(ByVal scope As String) As ReportData
            If scope Is Nothing Then
                Return Me
            ElseIf scope.Equals("") Then
                Return Me.GetParentData
            Else
                Dim data As ReportData = Me
                Do While data IsNot Nothing
                    If scope.Equals(data.GetId) Then
                        Return data
                    End If
                    data = data.GetParentData
                Loop
                Return Nothing
            End If
        End Function

        Public Function FindUnit(ByVal unit As String) As GroupDesign
            If unit.Equals("") Then
                If Me.Group IsNot Nothing Then
                    Dim cd As ContentDesign = Me.Group.GetDesign.GetAggregateSrcContentDesign
                    If cd Is Nothing OrElse cd.GroupDesign Is Nothing Then
                        Return Nothing
                    End If
                    Return cd.GroupDesign
                Else
                    Return Me.Report.Groups.Design
                End If
            Else
                If Me.Group IsNot Nothing Then
                    Dim ret As GroupDesign = Me.Group.GetDesign.FindGroupDesign(unit)
                    If ret IsNot Nothing AndAlso ret Is Me.Group.GetDesign Then
                        ret = Nothing
                    End If
                    Return ret
                Else
                    Return Me.Report.Groups.Design.FindGroupDesign(unit)
                End If
            End If
        End Function

        Public Function GetParentData() As ReportData
            If Me.Group IsNot Nothing Then
                If Me.Group.ParentGroups.ParentContent IsNot Nothing Then
                    Return Me.Group.ParentGroups.ParentContent.ParentGroup.Data
                Else
                    Return Me.Group.ParentGroups.Report.Data
                End If
            Else
                Return Nothing
            End If
        End Function

        Private Class _SummaryResult
            Public Summary As Decimal
            Public Count As Integer
            Public Sub New(ByVal summary As Decimal, ByVal count As Integer)
                Me.Summary = summary
                Me.Count = count
            End Sub
        End Class

        Public Function GetCount(ByVal key As String) As Object
            Return Me.getSummary_Aux(key).Count
        End Function

        Public Function GetSummary(ByVal key As String) As Object
            With Me.getSummary_Aux(key)
                If .Count > 0 Then
                    Return .Summary
                Else
                    Return Nothing
                End If
            End With
        End Function

        Public Function GetAverage(ByVal key As String) As Object
            With Me.getSummary_Aux(key)
                If .Count > 0 Then
                    Return .Summary / .Count
                ElseIf Report.Compatibility._4_13_2_AverageZero Then
                    Return 0D
                Else
                    Return Nothing
                End If
            End With
        End Function

        Private Function getSummary_Aux(ByVal key As String) As _SummaryResult
            If Not TypeOf Me.DataSource Is INoCache AndAlso _
              Me.DataSource.Size > &HFF AndAlso Me.Size > &HFF Then
                Return Me.getSummary_Cache(key)
            Else
                Return Me.getSummary_NoCache(key)
            End If
        End Function

        Private Function getSummary_Cache(ByVal key As String) As _SummaryResult
            Dim summary As Decimal = 0
            Dim count As Integer = 0
            Dim summaryCache As Dictionary(Of Integer, Decimal)
            Dim countCache As Dictionary(Of Integer, Integer)
            Dim _beginIndex As Integer
            Dim _endIndex As Integer
            Dim customField As CustomField = Me.findCustomField(key)
            Try
                If Me.Report IsNot Nothing And customField IsNot Nothing Then
                    Me.Report.CustomFieldStack.Push(customField)
                End If
                With Nothing
                    Dim dc As DataCache = Me.DataCache
                    If customField IsNot Nothing Then
                        summaryCache = dc.CustomFieldSummary(customField.Data, key)
                        countCache = dc.CustomFieldCount(customField.Data, key)
                        _beginIndex = customField.Data.TransIndex(Me, 0)
                        _endIndex = customField.Data.TransIndex(Me, Me.Size)
                    Else
                        summaryCache = dc.Summary(Me.DataSource, key)
                        countCache = dc.Count(Me.DataSource, key)
                        _beginIndex = Me.BeginIndex
                        _endIndex = Me.EndIndex
                    End If
                End With
                Try
                    Dim segf As Integer = _beginIndex >> 8
                    Dim segt As Integer = _endIndex >> 8
                    For i As Integer = segf To segt
                        Dim offf As Integer = IIf(i = segf, _beginIndex And &HFF, 0)
                        Dim offt As Integer = IIf(i = segt, _endIndex And &HFF, &H100) - 1
                        Dim entire As Boolean = (offf = 0 And offt = &HFF)
                        If entire AndAlso summaryCache.ContainsKey(i) Then
                            summary += summaryCache(i)
                            count += countCache(i)
                        Else
                            Dim _summary As Decimal = 0
                            Dim _count As Integer = 0
                            For j As Integer = offf To offt
                                Dim o As Object = Nothing
                                Dim _i As Integer = (i << 8) Or j
                                If customField IsNot Nothing Then
                                    o = customField.Get(_i)
                                Else
                                    o = Me.DataSource.Get(_i, key)
                                End If
                                If o IsNot Nothing Then
                                    Dim _o As Object = ReportUtil.Regularize(o)
                                    If TypeOf _o Is Decimal Then
                                        _summary += _o
                                    End If
                                    _count += 1
                                End If
                            Next
                            summary += _summary
                            count += _count
                            If entire Then
                                summaryCache.Add(i, _summary)
                                countCache.Add(i, _count)
                            End If
                        End If
                    Next
                Finally
                    If Me.Report IsNot Nothing And customField IsNot Nothing Then
                        Me.Report.CustomFieldStack.Pop()
                    End If
                End Try
                Return New _SummaryResult(summary, count)
            Catch ex As EvalException
                If Me.Logger IsNot Nothing Then
                    Me.Logger.EvaluateError(key, ex)
                End If
                Return New _SummaryResult(0, 0)
            End Try
        End Function

        Private Function getSummary_NoCache(ByVal key As String) As _SummaryResult
            Dim summary As Decimal = 0
            Dim count As Integer = 0
            Dim customField As CustomField = Me.findCustomField(key)
            Try
                If Me.Report IsNot Nothing And customField IsNot Nothing Then
                    Me.Report.CustomFieldStack.Push(customField)
                End If
                Try
                    For i As Integer = 0 To Me.Size - 1
                        Dim o As Object
                        If customField IsNot Nothing Then
                            o = customField.Get(customField.Data.TransIndex(Me, i))
                        Else
                            o = Me.DataSource.Get(i + Me.BeginIndex, key)
                        End If
                        If o IsNot Nothing Then
                            Dim _o As Object = ReportUtil.Regularize(o)
                            If TypeOf _o Is Decimal Then
                                summary += _o
                            End If
                            count += 1
                        End If
                    Next
                Finally
                    If Me.Report IsNot Nothing And customField IsNot Nothing Then
                        Me.Report.CustomFieldStack.Pop()
                    End If
                End Try
                Return New _SummaryResult(summary, count)
            Catch ex As EvalException
                If Me.Logger IsNot Nothing Then
                    Me.Logger.EvaluateError(key, ex)
                End If
                Return New _SummaryResult(0, 0)
            End Try
        End Function

        Public Function GetId() As String
            If Me.Group IsNot Nothing Then
                Return Me.Group.GetDesign.Id
            ElseIf Me.Report IsNot Nothing Then
                Return Me.Report.Design.Id
            Else
                Return Nothing
            End If
        End Function

        Public Function GetTraversalIndex() As Integer
            If Me.Group IsNot Nothing Then
                Return Me.Group.TraversalIndex
            Else
                Return -1
            End If
        End Function

        Public Function IsFilled() As Boolean
            If Me.Report IsNot Nothing Then
                Return Me.Report.Filled
            Else
                Return False
            End If
        End Function

        Public Function IsAggregateSrc() As Boolean
            If Me.Group IsNot Nothing Then
                If Me.Group.ParentGroups.ParentContent IsNot Nothing Then
                    Return Me.Group.ParentGroups.ParentContent.Design.AggregateSrc
                Else
                    Return True
                End If
            ElseIf Me.Report IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Private _record As ReportDataRecord = Nothing
        Public Function GetRecord() As ReportDataRecord
            If Me.Size > 0 Then
                If Me._record Is Nothing Then
                    Me._record = New ReportDataRecord(Me, 0)
                End If
                Return Me._record
            Else
                Return Nothing
            End If
        End Function

        Public Function GetRecords() As IEnumerable
            Return New Records(Me)
        End Function

        Private Class Records
            Implements IEnumerable

            Private Data As ReportData

            Public Sub New(ByVal reportData As ReportData)
                Me.Data = reportData
            End Sub

            Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
                Return New RecordIterator(Me.Data)
            End Function

        End Class

        Private Class RecordIterator
            Implements IEnumerator

            Private Data As ReportData
            Private i As Integer

            Public Sub New(ByVal reportData As ReportData)
                Me.Data = reportData
                Me.i = -1
            End Sub

            Public ReadOnly Property Current() As Object Implements IEnumerator.Current
                Get
                    Return New ReportDataRecord(Me.Data, Me.i)
                End Get
            End Property

            Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
                Me.i += 1
                Return Me.i < Me.Data.Size
            End Function

            Public Sub Reset() Implements IEnumerator.Reset
                Throw New NotSupportedException
            End Sub

        End Class

        Private _emptyIndexRange As New IndexRange
        Public Function GetDataIndexRange(ByVal groupDesign As GroupDesign) As IndexRange
            If Me.Group IsNot Nothing Then
                If Me.Group.DataIndexRangeMap.ContainsKey(groupDesign) Then
                    Return Me.Group.DataIndexRangeMap(groupDesign)
                Else
                    Return Me._emptyIndexRange
                End If
            Else
                Return Nothing
            End If
        End Function

        Public Function GetWrapperDataSource(ByVal groupDesign As GroupDesign) As WrapperDataSource
            Return Me.Report.WrapperDataSourceMap(groupDesign)
        End Function

    End Class

End Namespace
