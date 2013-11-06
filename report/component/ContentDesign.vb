Namespace component

    Public Class ContentDesign

        Public Desc As Hashtable = Nothing
        Public Caption As String = Nothing
        Public Id As String = Nothing
        Public Size As ContentSizeDesign = Nothing
        Public AggregateSrc As Boolean = False
        Public EveryPage As Boolean = False
        Public VisibilityCond As String = Nothing
        Public Unbreakable As Boolean = False
        Public Weight As Integer = 0
        Public Variables As Dictionary(Of String, String) = Nothing
        Public VariablesKeyList As List(Of String) = Nothing
        Public GroupDesign As GroupDesign = Nothing
        Public SubContentDesigns As List(Of ContentDesign) = Nothing

        Public ParentGroupDesign As GroupDesign
        Public BaseContentDesign As ContentDesign

        Public Sub New( _
          ByVal desc As Hashtable, _
          ByVal parentGroupDesign As GroupDesign)
            Me.New(desc, parentGroupDesign, Nothing)
        End Sub

        Public Sub New( _
          ByVal desc As Hashtable, _
          ByVal parentGroupDesign As GroupDesign, _
          ByVal baseContentDesign As ContentDesign)
            Me.Desc = desc
            Me.ParentGroupDesign = parentGroupDesign
            Me.BaseContentDesign = baseContentDesign
            Me.LoadDesc()
            Me.LoadSubDesc()
        End Sub

        Public Sub LoadDesc()
            Me.Caption = Me.Desc("caption")
            Me.Id = Me.Desc("id")
            If Me.Desc.ContainsKey("size") Then
                Me.Size = New ContentSizeDesign(Me.Desc("size"))
            Else
                Me.Size = New ContentSizeDesign
            End If
            Me.AggregateSrc = Me.Desc("aggregate_src")
            Me.EveryPage = Me.Desc("every_page")
            Me.VisibilityCond = Me.Desc("visibility_cond")
            Me.Unbreakable = Me.Desc("unbreakable")
            Me.Weight = Me.Desc("weight")
            If Me.Desc.ContainsKey("variables") Then
                Me.Variables = New Dictionary(Of String, String)
                Me.VariablesKeyList = New List(Of String)
                For Each d As Hashtable In Me.Desc("variables")
                    If d.ContainsKey("key") AndAlso d.ContainsKey("exp") Then
                        If Not Me.Variables.ContainsKey(d("key")) Then
                            Me.Variables.Add(d("key"), d("exp"))
                            Me.VariablesKeyList.Add(d("key"))
                        End If
                    End If
                Next
            Else
                Me.Variables = Nothing
                Me.VariablesKeyList = Nothing
            End If
        End Sub

        Public Sub LoadSubDesc()
            If Me.Desc.ContainsKey("group") Then
                Me.GroupDesign = New GroupDesign(Me.Desc("group"), Me.ParentGroupDesign.ReportDesign, Me)
            Else
                Me.GroupDesign = Nothing
            End If
            If Me.Desc.ContainsKey("sub") Then
                Me.SubContentDesigns = New List(Of ContentDesign)
                For Each d As Hashtable In Me.Desc("sub")
                    Me.SubContentDesigns.Add(New ContentDesign(d, Me.ParentGroupDesign, Me))
                Next
            Else
                Me.SubContentDesigns = Nothing
            End If
        End Sub

        Public Function GetIndex() As Integer
            If Me.BaseContentDesign IsNot Nothing Then
                Return Me.BaseContentDesign.GetIndex()
            Else
                Return Me.ParentGroupDesign.ContentDesigns.IndexOf(Me)
            End If
        End Function

        Public Function GetSubIndex() As Integer
            If Me.BaseContentDesign IsNot Nothing Then
                Return Me.BaseContentDesign.SubContentDesigns.IndexOf(Me)
            Else
                Return -1
            End If
        End Function

        Public Function GetReportDesign() As ReportDesign
            Return Me.ParentGroupDesign.ReportDesign
        End Function

    End Class

End Namespace