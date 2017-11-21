Public Interface IPrintPreviewZoom
    Event UpdateReport()
    Property Zoom() As Decimal
    Sub ZoomIn()
    Sub ZoomOut()
    Property AutoZoomFit As Boolean
    Property AutoZoomFitWidth As Boolean
End Interface