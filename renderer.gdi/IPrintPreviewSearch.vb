Imports jp.co.systembase.report.component

Public Interface IPrintPreviewSearch
    Function GetPageIndex()
    Sub Focus(pageIndex As Integer, region As Region)
    Sub Release()
End Interface
