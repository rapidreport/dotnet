Namespace component

    Public Class ContentSizeDesign

        Public Initial As Single
        Public Max As Single
        Public InitialExp As String
        Public MaxExp As String
        Public SpecInitial As Boolean
        Public SpecMax As Boolean
        Public RevInitial As Boolean
        Public RevMax As Boolean
        Public NotExtendable As Boolean

        Public Sub New()
            Me.new(New Hashtable)
        End Sub

        Public Sub New(desc As Hashtable)
            If desc.ContainsKey("initial") Then
                Me.Initial = desc("initial")
                Me.SpecInitial = True
            Else
                Me.Initial = 0
                Me.SpecInitial = False
            End If
            If desc.ContainsKey("max") Then
                Me.Max = desc("max")
                Me.SpecMax = True
            Else
                Me.Max = 0
                Me.RevMax = False
            End If
            Me.InitialExp = desc("initial_exp")
            Me.MaxExp = desc("max_exp")
            Me.RevInitial = desc("rev_initial")
            Me.RevMax = desc("rev_max")
            Me.NotExtendable = desc("not_extendable")
        End Sub

    End Class

End Namespace
