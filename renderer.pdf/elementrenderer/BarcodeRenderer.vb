Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports com.google.zxing
Imports com.google.zxing.common
Imports com.google.zxing.qrcode

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer

Namespace elementrenderer

    Public Class BarcodeRenderer
        Implements IElementRenderer

        Public Sub Render( _
          ByVal renderer As PdfRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Render
            Dim code As String = RenderUtil.Format(reportDesign, design.Child("formatter"), data)
            If code Is Nothing Then
                Exit Sub
            End If
            Dim _region As Region = region.ToPointScale(reportDesign)
            Dim image As Image = Nothing
            Dim cb As PdfContentByte = renderer.Writer.DirectContent
            Dim type As String = design.Get("barcode_type")
            Try
                If type IsNot Nothing AndAlso type.Equals("code39") Then
                    Dim bc As New Barcode39
                    bc.Code = code
                    If design.Get("without_text") Then
                        bc.Font = Nothing
                    End If
                    If design.Get("generate_checksum") Then
                        bc.GenerateChecksum = True
                    End If
                    image = bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                ElseIf type IsNot Nothing AndAlso type.Equals("ean8") Then
                    Dim bc As New BarcodeEAN
                    bc.CodeType = Barcode.EAN8
                    If design.Get("without_text") Then
                        bc.Font = Nothing
                    End If
                    If code.Length = 7 Then
                        bc.Code = code & BarcodeEAN.CalculateEANParity(code)
                        image = bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                    ElseIf code.Length = 8 Then
                        bc.Code = code
                        image = bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                    End If
                ElseIf type IsNot Nothing AndAlso type.Equals("code128") Then
                    Dim bc As New Barcode128
                    If design.Get("without_text") Then
                        bc.Font = Nothing
                    End If
                    bc.Code = code
                    image = bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                ElseIf type IsNot Nothing AndAlso type.Equals("codabar") Then
                    Dim bc As New BarcodeCodabar
                    If design.Get("without_text") Then
                        bc.Font = Nothing
                    End If
                    If design.Get("generate_checksum") Then
                        bc.GenerateChecksum = True
                    End If
                    Dim ss As String = "A"
                    If Not design.IsNull("codabar_startstop_code") Then
                        ss = design.Get("codabar_startstop_code")
                    End If
                    bc.Code = ss & code & ss
                    image = bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                ElseIf type IsNot Nothing AndAlso type.Equals("qrcode") Then
                    Dim w As New QRCodeWriter
                    Dim h As New Hashtable
                    If Not design.IsNull("qr_charset") Then
                        h.Add(EncodeHintType.CHARACTER_SET, design.Get("qr_charset"))
                    Else
                        h.Add(EncodeHintType.CHARACTER_SET, "shift_jis")
                    End If
                    If Not design.IsNull("qr_correction_level") Then
                        Select Case design.Get("qr_correction_level")
                            Case "L"
                                h.Add(EncodeHintType.ERROR_CORRECTION, decoder.ErrorCorrectionLevel.L)
                            Case "Q"
                                h.Add(EncodeHintType.ERROR_CORRECTION, decoder.ErrorCorrectionLevel.Q)
                            Case "H"
                                h.Add(EncodeHintType.ERROR_CORRECTION, decoder.ErrorCorrectionLevel.H)
                            Case Else
                                h.Add(EncodeHintType.ERROR_CORRECTION, decoder.ErrorCorrectionLevel.M)
                        End Select
                    Else
                        h.Add(EncodeHintType.ERROR_CORRECTION, decoder.ErrorCorrectionLevel.M)
                    End If
                    Dim bm As ByteMatrix = w.encode(code, BarcodeFormat.QR_CODE, 0, 0, h)
                    Dim tmp As PdfTemplate = cb.CreateTemplate(bm.Width, bm.Height)
                    For y As Integer = 0 To bm.Height - 1
                        For x As Integer = 0 To bm.Width - 1
                            If Not bm.Array(y)(x) Then
                                tmp.Rectangle(x, bm.Height - y, 1, 1)
                            End If
                        Next
                    Next
                    tmp.Fill()
                    image = image.GetInstance(tmp)
                Else
                    Dim bc As New BarcodeEAN
                    bc.CodeType = Barcode.EAN13
                    If design.Get("without_text") Then
                        bc.Font = Nothing
                    End If
                    If code.Length = 12 Then
                        bc.Code = code & BarcodeEAN.CalculateEANParity(code)
                        image = bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                    ElseIf code.Length = 13 Then
                        bc.Code = code
                        image = bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                    End If
                End If
            Catch ex As Exception
            End Try
            If image IsNot Nothing Then
                image.ScaleAbsolute(_region.GetWidth - 2, _region.GetHeight - 2)
                image.SetAbsolutePosition(renderer.Trans.X(_region.Left + 1), renderer.Trans.Y(_region.Bottom + 1))
                cb.AddImage(image)
            End If
        End Sub

    End Class

End Namespace