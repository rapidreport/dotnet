Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports jp.co.systembase.report.component

Namespace elementrenderer

    Public Class FieldRenderer
        Implements IElementRenderer

        Public Overridable Sub Render(
          renderer As PdfRenderer,
          reportDesign As ReportDesign,
          region As Region,
          design As ElementDesign,
          data As Object) Implements IElementRenderer.Render
            _RenderRect(renderer, reportDesign, region, design)
            Dim text = _GetText(reportDesign, design, data)
            If text Is Nothing Then
                Exit Sub
            End If
            If renderer.Setting.ReplaceBackslashToYen Then
                text = text.Replace("\", ChrW(&HA5))
            End If
            Dim _region = _GetRegion(reportDesign, region, design)
            Dim pdfText = _GetPdfText(renderer, reportDesign, _region, design, text)
            pdfText.Initialize(renderer, reportDesign, _region, design, text)
            pdfText.Draw()
        End Sub

        Protected Overridable Sub _RenderRect(renderer As PdfRenderer, reportDesign As ReportDesign, region As Region, design As ElementDesign)
            If Not design.IsNull("rect") Then
                renderer.Setting.GetElementRenderer("rect").Render(
                  renderer,
                  reportDesign,
                  region,
                  design.Child("rect"),
                  Nothing)
            End If
        End Sub

        Protected Overridable Function _GetText(reportDesign As ReportDesign, design As ElementDesign, data As Object) As String
            Return RenderUtil.Format(reportDesign, design.Child("formatter"), data)
        End Function

        Protected Overridable Function _GetRegion(reportDesign As ReportDesign, region As Region, design As ElementDesign) As Region
            Dim ret = region
            If Not design.IsNull("margin") Then
                Dim m = design.Child("margin")
                ret = New Region(region, m.Get("left"), m.Get("top"), m.Get("right"), m.Get("bottom"))
            End If
            Return ret
        End Function

        Protected Overridable Function _GetPdfText(renderer As PdfRenderer, reportDesign As ReportDesign, region As Region, design As ElementDesign, text As String) As PdfText
            Return _GetPdfText()
        End Function

        Protected Overridable Function _GetPdfText() As PdfText
            Return New PdfText()
        End Function

    End Class

End Namespace
