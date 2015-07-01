Imports jp.co.systembase.report.component

Public Interface IPrintPreviewSearch
    Function GetPages() As ReportPages
    Function GetPageIndex() As Integer
    Sub Focus(pageIndex As Integer, region As Region)
    Sub Release()
End Interface
