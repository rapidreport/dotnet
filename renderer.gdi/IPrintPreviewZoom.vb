Public Interface IPrintPreviewZoom
    Event UpdateReport()
    Property Zoom() As Decimal
    Sub ZoomIn()
    Sub ZoomOut()
    Sub ZoomFit()
End Interface