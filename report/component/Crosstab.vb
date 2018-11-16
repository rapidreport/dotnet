
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.data.internal

Namespace component
    Public Module Crosstab

        Public Class Setting

            Public VKeys As List(Of String) = Nothing
            Public HKeys As List(Of String) = Nothing
            Public VCount As Integer = 0
            Public HCount As Integer = 0

            Public Sub New(groupDesign As GroupDesign)
                Me._Init(groupDesign)
            End Sub

            Private Sub _Init(groupDesign As GroupDesign)
                Select Case groupDesign.CrosstabPartType
                    Case Report.ECrosstabPartType.VDETAIL
                        Me.VKeys = groupDesign.Keys
                        If groupDesign.Layout IsNot Nothing Then
                            Me.VCount = groupDesign.Layout.MaxCount
                        End If
                    Case Report.ECrosstabPartType.HDETAIL
                        Me.HKeys = groupDesign.Keys
                        If groupDesign.Layout IsNot Nothing Then
                            Me.HCount = groupDesign.Layout.MaxCount
                        End If
                End Select
                For Each cd As ContentDesign In groupDesign.ContentDesigns
                    If cd.GroupDesign IsNot Nothing Then
                        Me._Init(cd.GroupDesign)
                    End If
                    If cd.SubContentDesigns IsNot Nothing Then
                        For Each scd As ContentDesign In cd.SubContentDesigns
                            If scd.GroupDesign IsNot Nothing Then
                                Me._Init(scd.GroupDesign)
                            End If
                        Next
                    End If
                Next
            End Sub

            Public Function IsValid() As Boolean
                Return _
                  Me.VKeys IsNot Nothing AndAlso Me.HKeys IsNot Nothing AndAlso
                  Me.VCount > 0 AndAlso Me.HCount > 0
            End Function

        End Class

        Public Class DataSet
            Public OrgData As ReportData
            Public Captions As List(Of CrosstabDataSource)
            Public Summaries As List(Of CrosstabDataSource)
            Public VDetails As List(Of CrosstabDataSource)
            Public HDetails As Dictionary(Of CrosstabDataSource, List(Of CrosstabDataSource))
        End Class

        Public Class State

            Public Setting As Setting
            Public DataSet As DataSet
            Public VIndex As Integer
            Public HIndex As Integer
            Public VLast As Boolean
            Public HLast As Boolean

            Public Sub New(setting As Setting, dataSet As DataSet, vIndex As Integer, hIndex As Integer, vLast As Boolean, hLast As Boolean)
                Me.Setting = setting
                Me.DataSet = dataSet
                Me.VIndex = vIndex
                Me.HIndex = hIndex
                Me.VLast = vLast
                Me.HLast = hLast
            End Sub

        End Class

        Public Function Fill(groups As Groups, data As ReportData) As Boolean
            If groups.Report.InDesigner Then
                Return False
            End If
            Select Case groups.Design.CrosstabPartType
                Case Report.ECrosstabPartType.ROOT
                    Return _Fill_Root(groups, data)
                Case Report.ECrosstabPartType.CAPTION
                    Return _Fill_Caption(groups, data)
                Case Report.ECrosstabPartType.VDETAIL
                    Return _Fill_VDetail(groups, data)
                Case Report.ECrosstabPartType.HDETAIL
                    Return _Fill_HDetail(groups, data)
                Case Report.ECrosstabPartType.SUMMARY
                    Return _Fill_Summary(groups, data)
            End Select
            Return False
        End Function

        Private Function _Fill_Root(groups As Groups, data As ReportData) As Boolean
            Dim setting As New Setting(groups.Design)
            If Not setting.IsValid Then
                Return False
            End If
            Dim ds As DataSet = _CreateDataSet(groups, setting, data)
            Dim vc As Integer = Fix((ds.VDetails.Count + setting.VCount - 1) / setting.VCount)
            Dim hc As Integer = Fix((ds.Captions.Count + setting.HCount - 1) / setting.HCount)
            For v As Integer = 0 To vc - 1
                For h As Integer = 0 To hc - 1
                    groups.AddGroup(New ReportData(ds.OrgData), New State(setting, ds, v, h, v = vc - 1, h = hc - 1))
                Next
            Next
            Return True
        End Function

        Private Function _Fill_Caption(groups As Groups, data As ReportData) As Boolean
            Dim state As State = _GetState(groups)
            If state Is Nothing Then
                Return False
            End If
            Dim b As Integer = state.HIndex * state.Setting.HCount
            For i As Integer = 0 To state.Setting.HCount - 1
                Dim _i As Integer = i + b
                If _i >= state.DataSet.Captions.Count Then
                    Exit For
                End If
                groups.AddGroup(New ReportData(state.DataSet.Captions(_i), data.Report, data.Group))
            Next
            Return True
        End Function

        Private Function _Fill_VDetail(groups As Groups, data As ReportData) As Boolean
            Dim state As State = _GetState(groups)
            If state Is Nothing Then
                Return False
            End If
            Dim b As Integer = state.VIndex * state.Setting.VCount
            For i As Integer = 0 To state.Setting.VCount - 1
                Dim _i As Integer = i + b
                If _i >= state.DataSet.VDetails.Count Then
                    Exit For
                End If
                groups.AddGroup(New ReportData(state.DataSet.VDetails(_i), data.Report, data.Group))
            Next
            Return True
        End Function

        Private Function _Fill_HDetail(groups As Groups, data As ReportData) As Boolean
            Dim state As State = _GetState(groups)
            If state Is Nothing Then
                Return False
            End If
            If TypeOf data.DataSource Is CrosstabDataSource Then
                Dim row As List(Of CrosstabDataSource) = state.DataSet.HDetails(data.DataSource)
                Dim b As Integer = state.HIndex * state.Setting.HCount
                For i As Integer = 0 To state.Setting.HCount - 1
                    Dim _i As Integer = i + b
                    If _i >= row.Count Then
                        Exit For
                    End If
                    groups.AddGroup(New ReportData(row(_i), data.Report, data.Group))
                Next
            End If
            Return True
        End Function

        Private Function _Fill_Summary(groups As Groups, data As ReportData) As Boolean
            Dim state As State = _GetState(groups)
            If state Is Nothing Then
                Return False
            End If
            Dim b As Integer = state.HIndex * state.Setting.HCount
            For i As Integer = 0 To state.Setting.HCount - 1
                Dim _i As Integer = i + b
                If _i >= state.DataSet.Summaries.Count Then
                    Exit For
                End If
                groups.AddGroup(New ReportData(state.DataSet.Summaries(_i), data.Report, data.Group))
            Next
            Return True
        End Function

        Private Function _GetState(groups As Groups) As State
            If groups.ParentContent IsNot Nothing Then
                Return groups.ParentContent.ParentGroup.CrosstabState
            End If
            Return Nothing
        End Function

        Private Function _CreateDataSet(groups As Groups, setting As Setting, data As ReportData) As DataSet
            Dim ret As New DataSet
            Dim keys As New List(Of String)
            keys.AddRange(setting.VKeys)
            keys.AddRange(setting.HKeys)
            Dim vSorted As New CrosstabDataSource(data, keys)
            Dim hSorted As New CrosstabDataSource(data, setting.HKeys)
            Dim hMasterList As New List(Of CrosstabDataSource)
            Dim hMasterMap As New Dictionary(Of CrosstabDataSource, Integer)
            ret.OrgData = data
            With Nothing
                Dim master As CrosstabDataSource = Nothing
                If groups.Design.Id IsNot Nothing Then
                    Dim d As IReportDataSource = groups.Report.GetCrosstabCaptionDataSource(groups.Design.Id)
                    If d IsNot Nothing Then
                        master = New CrosstabDataSource(d)
                    End If
                End If
                If master Is Nothing Then
                    master = hSorted
                End If
                ret.Captions = _DataSplit(master, setting.HKeys)
                For Each d As CrosstabDataSource In ret.Captions
                    Dim _d As CrosstabDataSource = d.Dummy(setting.HKeys)
                    hMasterMap.Add(_d, hMasterList.Count)
                    hMasterList.Add(_d)
                Next
            End With
            ret.VDetails = _DataSplit(vSorted, setting.VKeys)
            ret.HDetails = New Dictionary(Of CrosstabDataSource, List(Of CrosstabDataSource))
            For Each d As CrosstabDataSource In ret.VDetails
                Dim row As New List(Of CrosstabDataSource)(hMasterList)
                For Each _d As CrosstabDataSource In _DataSplit(d, setting.HKeys)
                    If hMasterMap.ContainsKey(_d) Then
                        row(hMasterMap(_d)) = _d
                    End If
                Next
                ret.HDetails.Add(d, row)
            Next
            ret.Summaries = New List(Of CrosstabDataSource)(hMasterList)
            For Each d As CrosstabDataSource In _DataSplit(hSorted, setting.HKeys)
                If hMasterMap.ContainsKey(d) Then
                    ret.Summaries(hMasterMap(d)) = d
                End If
            Next
            Return ret
        End Function

        Private Function _KeysEq(keys As List(Of String), data1 As CrosstabDataSource, i1 As Integer, data2 As CrosstabDataSource, i2 As Integer) As Boolean
            For Each key As String In keys
                If Not ReportUtil.Eq(data1.Get(i1, key), data2.Get(i2, key)) Then
                    Return False
                End If
            Next
            Return True
        End Function

        Private Function _DataSplit(data As CrosstabDataSource, keys As List(Of String)) As List(Of CrosstabDataSource)
            Dim ret As New List(Of CrosstabDataSource)
            Dim i As Integer = 0
            Do While (i < data.Size)
                Dim j As Integer = i + 1
                Do While (j < data.Size)
                    If Not _KeysEq(keys, data, i, data, j) Then
                        Exit Do
                    End If
                    j += 1
                Loop
                ret.Add(data.Part(i, j).SetKeys(keys))
                i = j
            Loop
            Return ret
        End Function

    End Module
End Namespace