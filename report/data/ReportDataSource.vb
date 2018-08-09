Imports System.Reflection

Namespace data

    Public Class ReportDataSource
        Implements IReportDataSource

        Public Data As DataTable = Nothing
        Public ListData As IList = Nothing

        Private _PropertyCache As Dictionary(Of Type, Dictionary(Of String, PropertyInfo))
        Private _FieldCache As Dictionary(Of Type, Dictionary(Of String, FieldInfo))

        Public Sub New(dataTable As DataTable)
            Me.Data = dataTable
        End Sub

        Public Sub New(data As IList)
            Me.ListData = data
            Me._PropertyCache = New Dictionary(Of Type, Dictionary(Of String, PropertyInfo))
            Me._FieldCache = New Dictionary(Of Type, Dictionary(Of String, FieldInfo))
        End Sub

        Public Overridable Function [Get](i As Integer, key As String) As Object Implements IReportDataSource.[Get]
            If Me.Data IsNot Nothing Then
                Return _GetFromDataTable(i, key)
            Else
                Return _GetFromListData(i, key)
            End If
        End Function

        Private Function _GetFromDataTable(i As Integer, key As String) As Object
            If Me.Data.Columns.Contains(key) Then
                Dim v As Object = Me.Data.Rows(i).Item(key)
                If TypeOf v Is DBNull Then
                    Return Nothing
                Else
                    Return v
                End If
            Else
                Throw New UnknownFieldException(Me, i, key)
            End If
        End Function

        Private Function _GetFromListData(i As Integer, key As String) As Object
            Dim r As Object = Me.ListData(i)
            If TypeOf r Is Hashtable Then
                Dim h As Hashtable = r
                If h.ContainsKey(key) Then
                    Return h(key)
                Else
                    Throw New UnknownFieldException(Me, i, key)
                End If
            Else
                Dim t As Type = r.GetType()
                Me._CreateCache(t)
                If Me._PropertyCache(t).ContainsKey(key) Then
                    Return Me._PropertyCache(t)(key).GetValue(r, Nothing)
                End If
                If Me._FieldCache(t).ContainsKey(key) Then
                    Return Me._FieldCache(t)(key).GetValue(r)
                End If
                Throw New UnknownFieldException(Me, i, key)
            End If
        End Function

        Private Sub _CreateCache(t As Type)
            If Me._PropertyCache.ContainsKey(t) Then
                Exit Sub
            End If
            Dim pc As New Dictionary(Of String, PropertyInfo)
            Dim fc As New Dictionary(Of String, FieldInfo)
            Dim ps() As PropertyInfo = t.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
            Dim fs() As FieldInfo = t.GetFields(BindingFlags.Public Or BindingFlags.Instance)
            For Each p As PropertyInfo In ps
                Dim n As String = p.Name
                Dim _n As String = n.Substring(0, 1).ToLower & n.Substring(1)
                If Not pc.ContainsKey(n) Then
                    pc.Add(n, p)
                End If
                If Not pc.ContainsKey(_n) Then
                    pc.Add(_n, p)
                End If
            Next
            For Each f As FieldInfo In fs
                Dim n As String = f.Name
                Dim _n As String = n.Substring(0, 1).ToLower & n.Substring(1)
                If Not fc.ContainsKey(n) Then
                    fc.Add(n, f)
                End If
                If Not fc.ContainsKey(_n) Then
                    fc.Add(_n, f)
                End If
            Next
            Me._PropertyCache.Add(t, pc)
            Me._FieldCache.Add(t, fc)
        End Sub

        Public Overridable Function Size() As Integer Implements IReportDataSource.Size
            If Me.Data IsNot Nothing Then
                Return Me._SizeFromDataTable()
            Else
                Return Me._SizeFromListData()
            End If
        End Function

        Private Function _SizeFromDataTable() As Integer
            Return Me.Data.Rows.Count
        End Function

        Private Function _SizeFromListData() As Integer
            Return Me.ListData.Count
        End Function

    End Class

End Namespace

