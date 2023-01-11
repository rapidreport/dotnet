Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer

Namespace elementrenderer

    Public Class BarcodeRenderer
        Implements IElementRenderer

        Public Overridable Sub Render(
          renderer As PdfRenderer,
          reportDesign As ReportDesign,
          region As Region,
          design As ElementDesign,
          data As Object) Implements IElementRenderer.Render
            Dim code = _GetCode(reportDesign, design, data)
            If code Is Nothing Then
                Exit Sub
            End If
            Try
                Dim _region As Region = region.ToPointScale(reportDesign)
                Dim cb = renderer.Writer.DirectContent
                Dim image As Image = _CreateImage(cb, _region, design, code)
                If image IsNot Nothing Then
                    If Not Report.Compatibility._5_13_PdfBarcode Then
                        image.ScaleAbsolute(_region.GetWidth - 4, _region.GetHeight - 4)
                        image.SetAbsolutePosition(renderer.Trans.X(_region.Left + 2), renderer.Trans.Y(_region.Bottom - 2))
                    Else
                        image.ScaleAbsolute(_region.GetWidth - 2, _region.GetHeight - 2)
                        image.SetAbsolutePosition(renderer.Trans.X(_region.Left + 1), renderer.Trans.Y(_region.Bottom + 1))
                    End If
                    cb.AddImage(image)
                End If
            Catch ex As Exception
            End Try
        End Sub

        Protected Overridable Function _GetCode(reportDesign As ReportDesign, design As ElementDesign, data As Object) As String
            Return RenderUtil.Format(reportDesign, design.Child("formatter"), data)
        End Function

        Protected Overridable Function _CreateImage(cb As PdfContentByte, region As Region, design As ElementDesign, code As String) As Image
            Dim type As String = design.Get("barcode_type")
            Select Case type
                Case "ean8"
                    Dim bc As New BarcodeEAN
                    bc.CodeType = iTextSharp.text.pdf.Barcode.EAN8
                    If design.Get("without_text") Then
                        bc.Font = Nothing
                    End If
                    If code.Length = 7 Then
                        bc.Code = code & BarcodeEAN.CalculateEANParity(code)
                        Return bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                    ElseIf code.Length = 8 Then
                        bc.Code = code
                        Return bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                    End If
                Case "code39"
                    Dim bc As New Barcode39
                    bc.Code = code
                    If design.Get("without_text") Then
                        bc.Font = Nothing
                    End If
                    If design.Get("generate_checksum") Then
                        bc.GenerateChecksum = True
                    End If
                    Return bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                Case "codabar"
                    Dim bc As New BarcodeCodabar
                    If design.Get("without_text") Then
                        bc.Font = Nothing
                    End If
                    If design.Get("generate_checksum") Then
                        bc.GenerateChecksum = True
                    End If
                    Dim startCode As String = "A"
                    Dim stopCode As String = "A"
                    If Not design.IsNull("codabar_startstop_code") Then
                        Dim ss As String = design.Get("codabar_startstop_code")
                        If ss.Length = 1 Then
                            startCode = ss
                            stopCode = ss
                        ElseIf ss.Length > 1 Then
                            startCode = ss(0)
                            stopCode = ss(1)
                        End If
                    End If
                    If design.Get("codabar_startstop_show") Then
                        bc.StartStopText = True
                    End If
                    bc.Code = startCode & code & stopCode
                    Return bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                Case "itf"
                    Dim bc As New BarcodeInter25
                    If design.Get("without_text") Then
                        bc.Font = Nothing
                    End If
                    If design.Get("generate_checksum") Then
                        bc.GenerateChecksum = True
                    End If
                    bc.Code = code
                    If bc.GenerateChecksum Then
                        If bc.Code.Length Mod 2 = 0 Then
                            bc.Code = "0" & bc.Code
                        End If
                    Else
                        If bc.Code.Length Mod 2 = 1 Then
                            bc.Code = "0" & bc.Code
                        End If
                    End If
                    Return bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                Case "code128"
                    Dim bc As New Barcode128
                    If design.Get("without_text") Then
                        bc.Font = Nothing
                    End If
                    bc.Code = code
                    Return bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                Case "gs1_128"
                    Return barcode.Gs1_128.GetImage(cb, region, design, code)
                Case "yubin"
                    Return barcode.Yubin.GetImage(cb, region, design, code)
                Case "qrcode"
                    Return barcode.QRCode.GetImage(cb, region, design, code)
                Case Else
                    Dim bc As New BarcodeEAN
                    bc.CodeType = iTextSharp.text.pdf.Barcode.EAN13
                    If design.Get("without_text") Then
                        bc.Font = Nothing
                    End If
                    If code.Length = 12 Then
                        bc.Code = code & BarcodeEAN.CalculateEANParity(code)
                        Return bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                    ElseIf code.Length = 13 Then
                        bc.Code = code
                        Return bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                    End If
            End Select
            Return Nothing
        End Function

    End Class

End Namespace