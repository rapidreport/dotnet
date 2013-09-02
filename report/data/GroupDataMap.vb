Namespace data

    Public Class GroupDataMap

        Private dataMap As New Dictionary(Of String, IReportDataSource)
        Private modeMap As New Dictionary(Of String, Report.EGroupDataMode)

        Public Sub Add(ByVal key As String, ByVal dataSource As IReportDataSource)
            Me.Add(key, dataSource, Report.EGroupDataMode.DEFAULT_BLANK)
        End Sub

        Public Sub Add(ByVal key As String, ByVal dataSource As IReportDataSource, ByVal mode As Report.EGroupDataMode)
            Me.dataMap.Add(key, dataSource)
            Me.modeMap.Add(key, mode)
        End Sub

        Public Function ContainsKey(ByVal key As String) As Boolean
            Return Me.dataMap.ContainsKey(key)
        End Function

        Public Function GetDataSource(ByVal key As String) As IReportDataSource
            Return Me.dataMap(key)
        End Function

        Public Function GetDataMode(ByVal key As String) As Report.EGroupDataMode
            Return Me.modeMap(key)
        End Function

    End Class

End Namespace
