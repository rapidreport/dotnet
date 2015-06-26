Imports jp.co.systembase.report.search

Public Class PrintPreviewSearchPanel

    Public Pages As ReportPages
    Public PrintPreview As IPrintPreviewSearch

    Private _lastPageIndex As Integer = -1
    Private _lastElementIndex As Integer = -1
    Private _SearchResultsMap As Dictionary(Of Integer, List(Of SearchRenderer.Result))

    Public Sub Init(pages As ReportPages, printPreview As IPrintPreviewSearch)
        Me.Pages = pages
        Me.PrintPreview = printPreview
    End Sub

    Public Sub DoOpen()
        Me.Reset()
        Me.TxtKeyword.Text = ""
        Me.Show()
        Me.TxtKeyword.Focus()
    End Sub

    Public Sub DoClose()
        Me.PrintPreview.Release()
        Me.Hide()
    End Sub

    Public Sub Reset()
        Me._lastPageIndex = -1
        Me._lastElementIndex = -1
        Me._SearchResultsMap = New Dictionary(Of Integer, List(Of SearchRenderer.Result))
    End Sub

    Private Sub BtnNext_Click(sender As System.Object, e As System.EventArgs) Handles BtnNext.Click
        Me._Search(True)
    End Sub

    Private Sub BtnPrev_Click(sender As System.Object, e As System.EventArgs) Handles BtnPrev.Click
        Me._Search(False)
    End Sub

    Private Sub TxtKeyword_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TxtKeyword.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter
                If e.Modifiers = Keys.Shift Then
                    Me._Search(False)
                Else
                    Me._Search(True)
                End If
        End Select
    End Sub

    Private Sub TxtKeyword_TextChanged(sender As Object, e As System.EventArgs) Handles TxtKeyword.TextChanged
        If Me._lastPageIndex >= 0 Then
            Me.Reset()
            Me.PrintPreview.Release()
        End If
    End Sub

    Private Sub _Search(forward As Boolean)
        Dim k As String = Me.TxtKeyword.Text
        If k IsNot Nothing Then
            k = k.Trim
        End If
        If String.IsNullOrEmpty(k) Then
            Exit Sub
        End If
        Dim pageIndex As Integer = Me.PrintPreview.GetPageIndex
        If forward Then
            If Me._lastPageIndex >= 0 Then
                Me.SearchForward(k, Me._lastPageIndex, Me._lastElementIndex)
            Else
                Me.SearchForward(k, pageIndex)
            End If
        Else
            If Me._lastPageIndex >= 0 Then
                Me.SearchPrev(k, Me._lastPageIndex, Me._lastElementIndex)
            ElseIf pageIndex = 0 Then
                Me.SearchForward(k, Me.Pages.Count - 1)
            Else
                Me.SearchForward(k, pageIndex)
            End If
        End If
    End Sub

    Public Sub SearchForward(keyword As String, pageIndex As Integer)
        Me.SearchForward(keyword, pageIndex, -1)
    End Sub

    Public Sub SearchForward(keyword As String, pageIndex As Integer, elementIndex As Integer)
        Dim pi As Integer = pageIndex
        Dim ei As Integer = elementIndex + 1
        Dim found As Boolean = False
        Do
            Dim rs As List(Of SearchRenderer.Result) = Me.GetSearchResults(keyword, pi)
            If ei >= rs.Count Then
                pi += 1
                If pi >= Me.Pages.Count Then
                    Exit Do
                Else
                    ei = 0
                End If
            Else
                Me.PrintPreview.Focus(pi, rs(ei).Region.ToPointScale(Me.Pages(pi).Report.Design))
                Me._lastPageIndex = pi
                Me._lastElementIndex = ei
                found = True
                Exit Do
            End If
        Loop
    End Sub

    Public Sub SearchPrev(keyword As String, pageIndex As Integer)
        Dim rs As List(Of SearchRenderer.Result) = Me.GetSearchResults(keyword, pageIndex)
        Me.SearchPrev(keyword, pageIndex, rs.Count)
    End Sub

    Public Sub SearchPrev(keyword As String, pageIndex As Integer, elementIndex As Integer)
        Dim pi As Integer = pageIndex
        Dim ei As Integer = elementIndex - 1
        Dim found As Boolean = False
        Do
            If ei < 0 Then
                pi -= 1
                If pi < 0 Then
                    Exit Do
                Else
                    ei = Me.GetSearchResults(keyword, pi).Count - 1
                End If
            Else
                Me.PrintPreview.Focus(pi, Me.GetSearchResults(keyword, pi)(ei).Region.ToPointScale(Me.Pages(pi).Report.Design))
                Me._lastPageIndex = pi
                Me._lastElementIndex = ei
                found = True
                Exit Do
            End If
        Loop
    End Sub

    Public Function GetSearchResults(keyword As String, pageIndex As Integer) As List(Of SearchRenderer.Result)
        If Not Me._SearchResultsMap.ContainsKey(pageIndex) Then
            Dim renderer As New SearchRenderer(keyword)
            Me.Pages(pageIndex).Render(renderer, Me.Pages)
            Me._SearchResultsMap.Add(pageIndex, renderer.Results)
        End If
        Return Me._SearchResultsMap(pageIndex)
    End Function

End Class
