Namespace component

    Public Class ElementDesign

        Public Base As Hashtable
        Public Modify As Hashtable
        Public RootDesign As ElementDesign
        Public Modified As Boolean = False

        Public Sub New(type As String)
            Me.Base = New Hashtable
            Me.Modify = New Hashtable
            Me.RootDesign = Me
            Me.Base.Add("type", type)
        End Sub

        Public Sub New(base As Hashtable)
            Me.Base = base
            Me.Modify = New Hashtable
            Me.RootDesign = Me
        End Sub

        Private Sub New(base As Hashtable, modify As Hashtable, rootDesign As ElementDesign)
            Me.Base = base
            Me.Modify = modify
            Me.RootDesign = rootDesign
        End Sub

        Public Function IsNull(key As String) As Boolean
            If Me.Modify.ContainsKey(key) Then
                Return Me.Modify(key) Is Nothing
            End If
            If Me.Base IsNot Nothing Then
                Return Me.Base(key) Is Nothing
            End If
            Return True
        End Function

        Public Function [Get](key As String) As Object
            If Me.Modify.ContainsKey(key) Then
                Return Me.Modify(key)
            End If
            If Me.Base IsNot Nothing Then
                Return Me.Base(key)
            End If
            Return Nothing
        End Function

        Public Sub Put(key As String, value As Object)
            Me.Modify(key) = value
            Me.RootDesign.Modified = True
        End Sub

        Public Function Child(key As String) As ElementDesign
            If Not Me.Modify.ContainsKey(key) Then
                Me.Modify(key) = New Hashtable
            End If
            Return New ElementDesign(Me.Base(key), Me.Modify(key), Me.RootDesign)
        End Function

        Public Function GetRegion(contentRegion As Region) As Region
            Return New ElementLayoutDesign(Me.Child("layout")).GetRegion(contentRegion)
        End Function

        Public Function IsVisible(evaluator As Evaluator) As Boolean
            If Not Me.IsNull("visibility_cond") Then
                If Not ReportUtil.Condition(evaluator.EvalTry(Me.Get("visibility_cond"))) Then
                    Return False
                End If
            End If
            Return True
        End Function

        Public Overrides Function ToString() As String
            Dim ret As String = ""
            If Not Me.IsNull("caption") Then
                ret = Me.Get("caption")
            ElseIf Not Me.IsNull("id") Then
                ret = Me.Get("id")
            End If
            Return ret & "(" & Me.Get("type") & ")"
        End Function

    End Class

End Namespace
