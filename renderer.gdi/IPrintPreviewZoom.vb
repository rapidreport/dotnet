Public Interface IPrintPreviewZoom
    Event UpdateReport()
    Event Resize()
    Event ZoomInOrOut(zoomIn As Boolean)
    Property Zoom() As Decimal
    Sub ZoomIn()
    Sub ZoomOut()
    Sub ZoomFit()
    Sub ZoomFitWidth()
End Interface