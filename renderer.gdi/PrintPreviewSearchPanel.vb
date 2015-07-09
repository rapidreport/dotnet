Imports jp.co.systembase.report.search

Public Class PrintPreviewSearchPanel

    Public PrintPreview As IPrintPreviewSearch

    Private _lastPageIndex As Integer = -1
    Private _lastElementIndex As Integer = -1
    Private _SearchResultsMap As Dictionary(Of Integer, List(Of SearchRenderer.Result))

    Public Sub Init(printPreview As IPrintPreviewSearch)
        Me.PrintPreview = printPreview
    End Sub

    Public Sub DoOpen()
        Me.Reset()
        Me.TxtKeyword.Text = ""
        Me.Show()
        Me.TxtKeyword.Focus()
    End Sub

    Public Sub DoClose()
        Me.Reset()
        Me.Hide()
    End Sub

    Public Sub Reset()
        Me._lastPageIndex = -1
        Me._lastElementIndex = -1
        Me._SearchResultsMap = Nothing
        If Me.PrintPreview IsNot Nothing Then
            Me.PrintPreview.Release()
        End If
    End Sub

    Private Sub BtnNext_Click(sender As System.Object, e As System.EventArgs) Handles BtnNext.Click
        Me.Search(True)
    End Sub

    Private Sub BtnPrev_Click(sender As System.Object, e As System.EventArgs) Handles BtnPrev.Click
        Me.Search(False)
    End Sub

    Private Sub TxtKeyword_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtKeyword.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter
                If e.Modifiers = Keys.Shift Then
                    Me.Search(False)
                Else
                    Me.Search(True)
                End If
        End Select
    End Sub

    Private Sub TxtKeyword_TextChanged(sender As Object, e As System.EventArgs) Handles TxtKeyword.TextChanged
        If Me._SearchResultsMap IsNot Nothing Then
            Me.Reset()
        End If
    End Sub

    Public Sub Search(forward As Boolean)
        Me.TxtKeyword.Focus()
        Dim k As String = Me.TxtKeyword.Text
        If k IsNot Nothing Then
            k = k.Trim
        End If
        If String.IsNullOrEmpty(k) Then
            Exit Sub
        End If
        Dim retry As Boolean = False
        Do
            Dim pageIndex As Integer = Me.PrintPreview.GetPageIndex
            Dim found As Boolean = False
            Dim entireSearched As Boolean = False
            If forward Then
                If retry Then
                    entireSearched = True
                    found = Me.SearchForward(k, 0)
                ElseIf Me._SearchResultsMap IsNot Nothing AndAlso Me._lastPageIndex >= 0 Then
                    found = Me.SearchForward(k, Me._lastPageIndex, Me._lastElementIndex)
                Else
                    entireSearched = (pageIndex = 0)
                    found = Me.SearchForward(k, pageIndex)
                End If
            Else
                If retry Then
                    entireSearched = True
                    found = Me.SearchPrev(k, Me.PrintPreview.GetPages.Count - 1)
                ElseIf Me._SearchResultsMap IsNot Nothing AndAlso Me._lastPageIndex >= 0 Then
                    found = Me.SearchPrev(k, Me._lastPageIndex, Me._lastElementIndex)
                ElseIf pageIndex = 0 Then
                    entireSearched = True
                    found = Me.SearchPrev(k, Me.PrintPreview.GetPages.Count - 1)
                Else
                    entireSearched = (pageIndex = Me.PrintPreview.GetPages.Count - 1)
                    found = Me.SearchPrev(k, pageIndex)
                End If
            End If
            retry = False
            If Not found Then
                Dim f As New FmSearchNotFound(Me, forward, entireSearched)
                f.Location = Me.Parent.PointToScreen(New Point(Me.Right - f.Size.Width, Me.Bottom))
                f.ShowDialog()
                retry = f.Retry
            End If
        Loop While retry
    End Sub

    Public Function SearchForward(keyword As String, pageIndex As Integer) As Boolean
        Return Me.SearchForward(keyword, pageIndex, -1)
    End Function

    Public Function SearchForward(keyword As String, pageIndex As Integer, elementIndex As Integer) As Boolean
        Try
            Me.TopLevelControl.Cursor = Cursors.WaitCursor
            Dim pi As Integer = pageIndex
            Dim ei As Integer = elementIndex + 1
            Do
                Dim rs As List(Of SearchRenderer.Result) = Me.GetSearchResults(keyword, pi)
                If ei >= rs.Count Then
                    pi += 1
                    If pi >= Me.PrintPreview.GetPages.Count Then
                        Exit Do
                    Else
                        ei = 0
                    End If
                Else
                    Me.PrintPreview.Focus(pi, rs(ei).Region.ToPointScale(Me.PrintPreview.GetPages(pi).Report.Design))
                    Me._lastPageIndex = pi
                    Me._lastElementIndex = ei
                    Return True
                End If
            Loop
        Finally
            Me.TopLevelControl.Cursor = Cursors.Default
        End Try
        Return False
    End Function

    Public Function SearchPrev(keyword As String, pageIndex As Integer) As Boolean
        Dim rs As List(Of SearchRenderer.Result) = Me.GetSearchResults(keyword, pageIndex)
        Return Me.SearchPrev(keyword, pageIndex, rs.Count)
    End Function

    Public Function SearchPrev(keyword As String, pageIndex As Integer, elementIndex As Integer) As Boolean
        Try
            Me.TopLevelControl.Cursor = Cursors.WaitCursor
            Dim pi As Integer = pageIndex
            Dim ei As Integer = elementIndex - 1
            Do
                If ei < 0 Then
                    pi -= 1
                    If pi < 0 Then
                        Exit Do
                    Else
                        ei = Me.GetSearchResults(keyword, pi).Count - 1
                    End If
                Else
                    Dim r As component.Region = Me.GetSearchResults(keyword, pi)(ei).Region
                    Dim p As ReportPage = Me.PrintPreview.GetPages(pi)
                    Me.PrintPreview.Focus(pi, r.ToPointScale(p.Report.Design))
                    Me._lastPageIndex = pi
                    Me._lastElementIndex = ei
                    Return True
                End If
            Loop
        Finally
            Me.TopLevelControl.Cursor = Cursors.Default
        End Try
        Return False
    End Function

    Public Function GetSearchResults(keyword As String, pageIndex As Integer) As List(Of SearchRenderer.Result)
        If Me._SearchResultsMap Is Nothing Then
            Me._SearchResultsMap = New Dictionary(Of Integer, List(Of SearchRenderer.Result))
        End If
        If Not Me._SearchResultsMap.ContainsKey(pageIndex) Then
            Dim renderer As New SearchRenderer(keyword)
            Dim pages As ReportPages = Me.PrintPreview.GetPages
            pages(pageIndex).Render(renderer, pages)
            Me._SearchResultsMap.Add(pageIndex, renderer.Results)
        End If
        Return Me._SearchResultsMap(pageIndex)
    End Function

End Class
