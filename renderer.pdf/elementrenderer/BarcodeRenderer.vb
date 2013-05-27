﻿Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports com.google.zxing
Imports com.google.zxing.common
Imports com.google.zxing.qrcode

Imports jp.co.systembase.barcode
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.barcode.content
Imports jp.co.systembase.barcode.content.CBarContent
Imports jp.co.systembase.barcode.CBarcode

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
            Dim scaleMargin As Single = 2.0F
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
                ElseIf type IsNot Nothing AndAlso type.Equals("yubincustomer") Then
                    Dim bc As New CYubinCustomer
                    Dim pt As Single = 10.0F
                    If Not design.IsNull("point") Then
                        pt = design.Get("point")
                    End If
                    Dim tmp As PdfTemplate = cb.CreateTemplate(_region.GetWidth, _region.GetHeight)
                    Dim c As CBarContent = bc.CreateContent(0, 0, tmp.Width, tmp.Height, pt, code)
                    tmp.SetColorFill(Color.WHITE)
                    tmp.Rectangle(0, 0, tmp.Width, tmp.Height)
                    tmp.Fill()
                    tmp.SetColorFill(Color.BLACK)
                    Dim codes As New List(Of String)
                    For Each _code As String In bc.Encode(code)
                        For Each _c As Char In _code
                            codes.Add(_c)
                        Next
                    Next
                    For i As Integer = 0 To c.GetBars.Count - 1
                        Dim b As CBar = c.GetBars(i)
                        Dim y As Single = tmp.Height - b.GetHeight
                        Dim _type As String = codes(i)
                        If _type = "3" Then
                            y = tmp.Height - c.GetBars(0).GetHeight
                        ElseIf _type = "4" Then
                            y -= b.GetHeight
                        End If
                        tmp.Rectangle(b.GetX, y, b.GetWidth, b.GetHeight)
                    Next
                    tmp.Fill()
                    image = image.GetInstance(tmp)
                    scaleMargin = 0.0F
                ElseIf type IsNot Nothing AndAlso type.Equals("itf") Then
                    Dim bc As New CItf
                    If design.Get("without_text") Then
                        bc.WithText = False
                    End If
                    If design.Get("generate_checksum") Then
                        bc.GenerateCheckSum = True
                    End If
                    Dim tmp As PdfTemplate = cb.CreateTemplate(_region.GetWidth, _region.GetHeight)
                    Dim c As CBarContent = bc.CreateContent(0, 0, tmp.Width, tmp.Height, code)
                    tmp.SetColorFill(Color.WHITE)
                    tmp.Rectangle(0, 0, tmp.Width, tmp.Height)
                    tmp.Fill()
                    tmp.SetColorFill(Color.BLACK)
                    For Each b As CBar In c.GetBars
                        Dim y As Single = tmp.Height - b.GetY - b.GetHeight
                        tmp.Rectangle(b.GetX, y, b.GetWidth, b.GetHeight)
                    Next
                    tmp.Fill()
                    Dim t As CText = c.GetText
                    If Not t Is Nothing Then
                        tmp.BeginText()
                        Dim f As Font = FontFactory.GetFont(t.GetFont.Name, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED)
                        tmp.SetFontAndSize(f.GetCalculatedBaseFont(True), t.GetFont.Size)
                        Dim y As Single = tmp.Height - (t.GetY + t.GetFont.Size) + (t.GetFont.Size / 10)
                        tmp.ShowTextAligned(PdfContentByte.ALIGN_CENTER, t.GetCode, t.GetX, y, 0)
                        tmp.EndText()
                    End If
                    image = image.GetInstance(tmp)
                    scaleMargin = 0.0F
                ElseIf type IsNot Nothing AndAlso type.Equals("gs1128") Then
                    Dim bc As New CGs1128
                    If design.Get("without_text") Then
                        bc.WithText = False
                    End If
                    Dim tmp As PdfTemplate = cb.CreateTemplate(_region.GetWidth, _region.GetHeight)
                    Dim c As CBarContent = bc.CreateContent(0, 0, tmp.Width, tmp.Height, code)
                    tmp.SetColorFill(Color.WHITE)
                    tmp.Rectangle(0, 0, tmp.Width, tmp.Height)
                    tmp.Fill()
                    tmp.SetColorFill(Color.BLACK)
                    For Each b As CBar In c.GetBars
                        Dim y As Single = tmp.Height - b.GetY - b.GetHeight
                        tmp.Rectangle(b.GetX, y, b.GetWidth, b.GetHeight)
                    Next
                    tmp.Fill()
                    Dim t As CText = c.GetText
                    If Not t Is Nothing Then
                        tmp.BeginText()
                        Dim f As Font = FontFactory.GetFont(t.GetFont.Name, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED)
                        tmp.SetFontAndSize(f.GetCalculatedBaseFont(True), t.GetFont.Size)
                        Dim y As Single = tmp.Height - (t.GetY + t.GetFont.Size) + (t.GetFont.Size / 10)
                        tmp.ShowTextAligned(PdfContentByte.ALIGN_CENTER, t.GetCode, t.GetX, y, 0)
                        tmp.EndText()
                    End If
                    image = image.GetInstance(tmp)
                    scaleMargin = 0.0F
                Else
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
                End If
            Catch ex As Exception
            End Try
            If image IsNot Nothing Then
                image.ScaleAbsolute(_region.GetWidth - scaleMargin, _region.GetHeight - scaleMargin)
                image.SetAbsolutePosition(renderer.Trans.X(_region.Left + 1), renderer.Trans.Y(_region.Bottom + 1))
                cb.AddImage(image)
            End If
        End Sub

    End Class

End Namespace