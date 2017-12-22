Namespace component
    Public Class ContentInstance

        Public Content As Content
        Public Region As Region
        Public ContentState As ContentState

        Public Sub New( _
           content As Content, _
           region As Region, _
           contentState As ContentState)
            Me.Region = region
            Me.Content = content
            Me.ContentState = contentState
        End Sub

        Public Function GetElementDesigns(evaluator As Evaluator) As ElementDesigns
            Dim ret As New ElementDesigns(Me.Content.Design)
            Dim report As Report = evaluator.BasicContext.Report
            _customizeElements(ret, evaluator)
            If report.Customizer IsNot Nothing Then
                report.Customizer.RenderContent(Me.Content, evaluator, Me.Region, ret)
            End If
            Return ret
        End Function

        Private Sub _customizeElements(elements As ElementDesigns, evaluator As Evaluator)
            For Each e As ElementDesign In elements
                If e.Base.Contains("customize") Then
                    For Each d As Hashtable In e.Base("customize")
                        Dim ce As New ElementDesign(d)
                        If Not ce.IsNull("property") AndAlso _
                           Not ce.IsNull("cond") AndAlso _
                           Not ce.IsNull("exp") Then
                            If evaluator.EvalTry(ce.Get("cond")) Then
                                Dim _e As ElementDesign = e
                                Dim ps As String() = CType(ce.Get("property"), String).Split(".")
                                For i As Integer = 0 To ps.Length - 2
                                    _e = _e.Child(ps(i).Trim)
                                Next
                                Try
                                    _e.Put(ps(ps.Length - 1), evaluator.Eval(ce.Get("exp")))
                                Catch ex As Exception
                                End Try
                            End If
                        End If
                    Next
                End If
            Next
        End Sub

    End Class
End Namespace