Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.data.internal

Namespace data
    Public Class GroupDataProvider
        Implements IGroupDataProvider

        Private Class _KeyList
            Inherits List(Of Object)
            Public Overrides Function Equals(ByVal obj As Object) As Boolean
                If Not TypeOf obj Is _KeyList Then
                    Return False
                End If
                Dim l As _KeyList = obj
                If Me.Count <> l.Count Then
                    Return False
                End If
                For i As Integer = 0 To Me.Count - 1
                    Dim v1 As Object = Me(i)
                    Dim v2 As Object = l(i)
                    If v1 Is Nothing Then
                        If v2 IsNot Nothing Then
                            Return False
                        End If
                    Else
                        If Not v1.Equals(v2) Then
                            Return False
                        End If
                    End If
                Next
                Return True
            End Function
            Public Overrides Function GetHashCode() As Integer
                Dim ret As Integer = 0
                For Each v As Object In Me
                    ret <<= 1
                    If v IsNot Nothing Then
                        ret = ret Xor v.GetHashCode
                    End If
                Next
                Return ret
            End Function
        End Class

        Public GroupDataMap As GroupDataMap

        Private groupDataCache As New Dictionary(Of String, Dictionary(Of _KeyList, IReportDataSource))
        Private keyNamesCache As New Dictionary(Of GroupDesign, List(Of String))

        Public Sub New()
            Me.new(New GroupDataMap)
        End Sub

        Public Sub New(ByVal groupDataMap As GroupDataMap)
            Me.GroupDataMap = groupDataMap
        End Sub

        Public Function GetGroupDataSource(ByVal groups As Groups, ByVal data As ReportData) As IReportDataSource Implements IGroupDataProvider.GetGroupDataSource
            Dim groupId As String = groups.Design.Id
            If groupId IsNot Nothing AndAlso Me.GroupDataMap.ContainsKey(groupId) Then
                Select Case Me.GroupDataMap.GetDataMode(groupId)
                    Case Report.EGroupDataMode.NO_SPLIT
                        Return Me.GroupDataMap.GetDataSource(groupId)
                    Case Else
                        Dim keyNames As List(Of String) = Me.getKeyNames(groups.Design)
                        If keyNames.Count = 0 Then
                            Return Me.GroupDataMap.GetDataSource(groupId)
                        Else
                            If Not Me.groupDataCache.ContainsKey(groupId) Then
                                Me.groupDataCache.Add(
                                  groupId,
                                  splitData(keyNames,
                                            Me.GroupDataMap.GetDataSource(groupId),
                                            data.Report.Design.Setting.Logger))
                            End If
                            Dim keys As _KeyList = getKeys(keyNames, data)
                            If Me.groupDataCache(groupId).ContainsKey(keys) Then
                                Return Me.groupDataCache(groupId)(keys)
                            Else
                                Select Case Me.GroupDataMap.GetDataMode(groupId)
                                    Case Report.EGroupDataMode.DEFAULT_DUMMY
                                        Return DummyDataSource.GetInstance
                                    Case Else
                                        Return BlankDataSource.GetInstance
                                End Select
                            End If
                        End If
                End Select
            Else
                Return data
            End If
        End Function

        Private Function getKeyNames(ByVal groupDesign As GroupDesign) As List(Of String)
            If Not Me.keyNamesCache.ContainsKey(groupDesign) Then
                Dim keyNames As New List(Of String)
                If groupDesign.ParentContentDesign IsNot Nothing Then
                    getKeyNames_aux(groupDesign.ParentContentDesign.ParentGroupDesign, keyNames)
                End If
                Me.keyNamesCache.Add(groupDesign, keyNames)
            End If
            Return Me.keyNamesCache(groupDesign)
        End Function

        Private Shared Sub getKeyNames_aux(ByVal groupDesign As GroupDesign, ByVal keyNames As List(Of String))
            If groupDesign.ParentContentDesign IsNot Nothing Then
                getKeyNames_aux(groupDesign.ParentContentDesign.ParentGroupDesign, keyNames)
            End If
            If Not groupDesign.Detail AndAlso groupDesign.Keys IsNot Nothing Then
                keyNames.AddRange(groupDesign.Keys)
            End If
        End Sub

        Private Shared Function getKeys( _
          ByVal keyNames As List(Of String), _
          ByVal data As ReportData) As _KeyList
            Dim ret As New _KeyList
            Dim r As ReportDataRecord = data.GetRecord
            For Each k As String In keyNames
                ret.Add(ReportUtil.Regularize(r.Get(k)))
            Next
            Return ret
        End Function

        Private Shared Function getKeys(
          ByVal keyNames As List(Of String),
          ByVal dataSource As IReportDataSource,
          ByVal index As Integer,
          ByVal logger As IReportLogger) As _KeyList
            Dim ret As New _KeyList
            For Each k As String In keyNames
                Try
                    ret.Add(ReportUtil.Regularize(dataSource.Get(index, k)))
                Catch ex As UnknownFieldException
                    If logger IsNot Nothing Then
                        logger.UnknownFieldError(ex)
                    End If
                    ret.Add(Nothing)
                End Try
            Next
            Return ret
        End Function

        Private Shared Function splitData(
          ByVal keyNames As List(Of String),
          ByVal dataSource As IReportDataSource,
          ByVal logger As IReportLogger) As Dictionary(Of _KeyList, IReportDataSource)
            Dim ret As New Dictionary(Of _KeyList, IReportDataSource)
            Dim keys As _KeyList = Nothing
            Dim beginIndex As Integer = 0
            For i As Integer = 0 To dataSource.Size - 1
                Dim _keys As _KeyList = getKeys(keyNames, dataSource, i, logger)
                If Not _keys.Equals(keys) Then
                    If keys IsNot Nothing Then
                        ret.Add(keys, New SubDataSource(dataSource, beginIndex, i))
                    End If
                    keys = _keys
                    beginIndex = i
                End If
            Next
            If keys IsNot Nothing AndAlso beginIndex < dataSource.Size Then
                ret.Add(keys, New SubDataSource(dataSource, beginIndex, dataSource.Size))
            End If
            Return ret
        End Function

    End Class
End Namespace