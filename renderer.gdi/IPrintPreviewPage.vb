Public Interface IPrintPreviewPage
    Event UpdateReport()
    Property PageCount() As Integer
    Function PageCountTotal() As Integer
    Sub PrevPage()
    Sub NextPage()
    Sub FirstPage()
    Sub LastPage()
End Interface