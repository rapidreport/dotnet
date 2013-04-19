Public Interface IPrintPreviewPage
    Event UpdateReport()
    Property PageCount() As Integer
    Function GetPageCountTotal() As Integer
End Interface