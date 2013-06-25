Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports com.google.zxing
Imports com.google.zxing.common
Imports com.google.zxing.qrcode

Imports jp.co.systembase.barcode
Imports jp.co.systembase.barcode.Barcode
Imports jp.co.systembase.barcode.content
Imports jp.co.systembase.barcode.content.BarContent
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
                Select Case type
                    Case "ean8"
                        Dim bc As New BarcodeEAN
                        bc.CodeType = iTextSharp.text.pdf.Barcode.EAN8
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
                    Case "code39"
                        Dim bc As New Barcode39
                        bc.Code = code
                        If design.Get("without_text") Then
                            bc.Font = Nothing
                        End If
                        If design.Get("generate_checksum") Then
                            bc.GenerateChecksum = True
                        End If
                        image = bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                    Case "codabar"
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
                        image = bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                    Case "code128"
                        Dim bc As New Barcode128
                        If design.Get("without_text") Then
                            bc.Font = Nothing
                        End If
                        bc.Code = code
                        image = bc.CreateImageWithBarcode(cb, Nothing, Nothing)
                    Case "gs1_128"
                        Dim bc As New Gs1_128
                        If design.Get("without_text") Then
                            bc.WithText = False
                        End If
                        If design.Get("conveni_format") Then
                            bc.ConveniFormat = True
                        End If
                        Dim tmp As PdfTemplate = cb.CreateTemplate(_region.GetWidth, _region.GetHeight)
                        Dim _image As New Drawing.Bitmap(CInt(_region.GetWidth), CInt(_region.GetHeight))
                        Dim g As Drawing.Graphics = Drawing.Graphics.FromImage(_image)
                        Dim c As BarContent = bc.CreateContent(g, 0, 0, tmp.Width, tmp.Height, code)
                        tmp.SetColorFill(Color.BLACK)
                        For Each b As Bar In c.GetBars
                            Dim y As Single = tmp.Height - b.GetY - b.GetHeight
                            tmp.Rectangle(b.GetX, y, b.GetWidth, b.GetHeight)
                        Next
                        tmp.Fill()
                        For Each t In c.GetText
                            tmp.BeginText()
                            Dim f As Font = FontFactory.GetFont(t.GetFont.Name, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED)
                            tmp.SetFontAndSize(f.GetCalculatedBaseFont(True), t.GetFont.Size)
                            Dim y As Single = tmp.Height - (t.GetY + t.GetFont.Size) + (t.GetFont.Size / 10)
                            Const margin As Single = 2.5F
                            tmp.ShowTextAligned(t.GetFormat.Alignment, t.GetCode, t.GetX + margin, y, 0)
                            tmp.EndText()
                        Next
                        image = image.GetInstance(tmp)
                    Case "yubin"
                        Dim bc As New Yubin
                        Dim codes As List(Of Byte) = bc.Encode(code)
                        Dim uw As Single = _region.GetWidth / (codes.Count * 2)
                        Dim x As Single = 0
                        Dim y As Single = _region.GetHeight / 2
                        If codes IsNot Nothing Then
                            Dim tmp As PdfTemplate = cb.CreateTemplate(_region.GetWidth, _region.GetHeight)
                            tmp.SetColorFill(Color.BLACK)
                            For Each c As Byte In codes
                                Dim by As Single = 0
                                Dim bh As Single = 0
                                Select Case c
                                    Case 1
                                        by = y - uw * 3
                                        bh = uw * 6
                                    Case 2
                                        by = y - uw
                                        bh = uw * 4
                                    Case 3
                                        by = y - uw * 3
                                        bh = uw * 4
                                    Case 4
                                        by = y - uw
                                        bh = uw * 2
                                End Select
                                tmp.Rectangle(x, by, uw, bh)
                                x += uw * 2
                            Next
                            tmp.Fill()
                            image = image.GetInstance(tmp)
                        End If
                    Case "qrcode"
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
                        tmp.SetColorFill(Color.BLACK)
                        For y As Integer = 0 To bm.Height - 1
                            For x As Integer = 0 To bm.Width - 1
                                If Not bm.Array(y)(x) Then
                                    tmp.Rectangle(x, bm.Height - y, 1, 1)
                                End If
                            Next
                        Next
                        tmp.Fill()
                        image = image.GetInstance(tmp)
                    Case Else
                        Dim bc As New BarcodeEAN
                        bc.CodeType = iTextSharp.text.pdf.Barcode.EAN13
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
                End Select
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