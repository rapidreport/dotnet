Namespace data.internal

    Public Class CrosstabDataSource
        Implements IReportDataSource

        Private _DataSource As IReportDataSource = Nothing
        Private _Indexes As List(Of Integer) = Nothing
        Private _Keys As List(Of String) = Nothing
        Private _Mask As Boolean = False

        Public Sub New(dataSource As IReportDataSource)
            Me._DataSource = dataSource
            Me._Indexes = New List(Of Integer)
            For i As Integer = 0 To Me._DataSource.Size - 1
                Me._Indexes.Add(i)
            Next
        End Sub

        Public Sub New(dataSource As IReportDataSource, keys As List(Of String))
            Me.New(dataSource)
            Me._Indexes.Sort(New RecordComparer(Me._DataSource, keys, Nothing))
        End Sub

        Private Sub New()
        End Sub

        Public Function Part(i As Integer, j As Integer) As CrosstabDataSource
            Dim ret As New CrosstabDataSource
            ret._DataSource = Me._DataSource
            ret._Indexes = Me._Indexes.GetRange(i, j - i)
            Return ret
        End Function

        Public Function Dummy(keys As List(Of String)) As CrosstabDataSource
            Dim ret As New CrosstabDataSource
            ret._DataSource = Me._DataSource
            ret._Indexes = Me._Indexes.GetRange(0, 1)
            ret.SetKeys(keys)
            ret._Mask = True
            Return ret
        End Function

        Public Function SetKeys(keys As List(Of String)) As CrosstabDataSource
            Me._Keys = keys
            Return Me
        End Function

        Public Function Size() As Integer Implements IReportDataSource.Size
            Return Me._Indexes.Count
        End Function

        Public Function [Get](i As Integer, key As String) As Object Implements IReportDataSource.[Get]
            If Me._Mask Then
                If Not Me._Keys.Contains(key) Then
                    Return Nothing
                End If
            End If
            Return Me._DataSource.Get(Me._Indexes(i), key)
        End Function

        Public Overrides Function GetHashCode() As Integer
            If Me._Keys IsNot Nothing Then
                Dim ret As Integer = 0
                For i As Integer = 0 To Me._Keys.Count - 1
                    Dim v As Object = Me.Get(0, Me._Keys(i))
                    If v IsNot Nothing Then
                        ret = ret Xor (v.GetHashCode() << i)
                    End If
                Next
                Return ret
            Else
                Return MyBase.GetHashCode()
            End If
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If Me._Keys IsNot Nothing Then
                If obj Is Me Then
                    Return True
                ElseIf Not TypeOf obj Is CrosstabDataSource Then
                    Return False
                Else
                    For i As Integer = 0 To Me._Keys.Count - 1
                        Dim v1 As Object = Me.Get(0, Me._Keys(i))
                        Dim v2 As Object = CType(obj, CrosstabDataSource).Get(0, Me._Keys(i))
                        If v1 Is Nothing Then
                            If v2 IsNot Nothing Then
                                Return False
                            End If
                        ElseIf Not v1.Equals(v2) Then
                            Return False
                        End If
                    Next
                End If
                Return True
            Else
                Return MyBase.Equals(obj)
            End If
        End Function

    End Class

End Namespace
