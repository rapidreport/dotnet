Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.expression

Namespace component
    Public Class DataCache
        Private _CustomField As _
          New Dictionary(Of ReportData,  _
                Dictionary(Of String,  _
                  Dictionary(Of Integer, Object)))
        Private _Summary As _
          New Dictionary(Of IReportDataSource,  _
                Dictionary(Of String,  _
                  Dictionary(Of Integer, Decimal)))
        Private _Count As _
          New Dictionary(Of IReportDataSource,  _
                Dictionary(Of String,  _
                  Dictionary(Of Integer, Integer)))
        Private _CustomFieldSummary As _
          New Dictionary(Of ReportData,  _
            Dictionary(Of String,  _
              Dictionary(Of Integer, Decimal)))
        Private _CustomFieldCount As _
          New Dictionary(Of ReportData,  _
            Dictionary(Of String,  _
              Dictionary(Of Integer, Integer)))
        Public Expression As New Dictionary(Of String, IExpression)
        Public Function CustomField( _
          data As ReportData, _
          key As String) As Dictionary(Of Integer, Object)
            If Not Me._CustomField.ContainsKey(data) Then
                Me._CustomField.Add(data, _
                  New Dictionary(Of String,  _
                          Dictionary(Of Integer, Object)))
            End If
            If Not Me._CustomField(data).ContainsKey(key) Then
                Me._CustomField(data).Add(key, _
                  New Dictionary(Of Integer, Object))
            End If
            Return Me._CustomField(data)(key)
        End Function
        Public Function Summary( _
          dataSource As IReportDataSource, _
          key As String) As Dictionary(Of Integer, Decimal)
            If Not Me._Summary.ContainsKey(dataSource) Then
                Me._Summary.Add(dataSource, _
                  New Dictionary(Of String,  _
                        Dictionary(Of Integer, Decimal)))
            End If
            If Not Me._Summary(dataSource).ContainsKey(key) Then
                Me._Summary(dataSource).Add(key, _
                  New Dictionary(Of Integer, Decimal))
            End If
            Return Me._Summary(dataSource)(key)
        End Function
        Public Function Count( _
          dataSource As IReportDataSource, _
          key As String) As Dictionary(Of Integer, Integer)
            If Not Me._Count.ContainsKey(dataSource) Then
                Me._Count.Add(dataSource, _
                  New Dictionary(Of String,  _
                        Dictionary(Of Integer, Integer)))
            End If
            If Not Me._Count(dataSource).ContainsKey(key) Then
                Me._Count(dataSource).Add(key, _
                  New Dictionary(Of Integer, Integer))
            End If
            Return Me._Count(dataSource)(key)
        End Function
        Public Function CustomFieldSummary( _
          data As ReportData, _
          key As String) As Dictionary(Of Integer, Decimal)
            If Not Me._CustomFieldSummary.ContainsKey(data) Then
                Me._CustomFieldSummary.Add(data, _
                  New Dictionary(Of String,  _
                          Dictionary(Of Integer, Decimal)))
            End If
            If Not Me._CustomFieldSummary(data).ContainsKey(key) Then
                Me._CustomFieldSummary(data).Add(key, _
                  New Dictionary(Of Integer, Decimal))
            End If
            Return Me._CustomFieldSummary(data)(key)
        End Function
        Public Function CustomFieldCount( _
          data As ReportData, _
          key As String) As Dictionary(Of Integer, Integer)
            If Not Me._CustomFieldCount.ContainsKey(data) Then
                Me._CustomFieldCount.Add(data, _
                  New Dictionary(Of String,  _
                          Dictionary(Of Integer, Integer)))
            End If
            If Not Me._CustomFieldCount(data).ContainsKey(key) Then
                Me._CustomFieldCount(data).Add(key, _
                  New Dictionary(Of Integer, Integer))
            End If
            Return Me._CustomFieldCount(data)(key)
        End Function
    End Class
End Namespace