Namespace component
    Public Class ContentInstance
        Public Content As Content
        Public Region As Region
        Public ContentState As ContentState
        Public Sub New( _
           ByVal content As Content, _
           ByVal region As Region, _
           ByVal contentState As ContentState)
            Me.Region = region
            Me.Content = content
            Me.ContentState = contentState
        End Sub
    End Class
End Namespace