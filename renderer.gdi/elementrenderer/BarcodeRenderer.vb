Imports com.google.zxing
Imports com.google.zxing.common
Imports com.google.zxing.qrcode

Imports jp.co.systembase.barcode
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer

Namespace elementrenderer

    Public Class BarcodeRenderer
        Implements IElementRenderer

        Public Sub Render( _
          ByVal env As RenderingEnv, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Render
            Dim code As String = Nothing
            If env.InDesigner Then
                Select Case design.Get("barcode_type")
                    Case "ean8"
                        code = "99999999"
                    Case "code39"
                        code = "99999999"
                    Case "code128"
                        code = "99999999"
                    Case "codabar"
                        code = "99999999"
                    Case "qrcode"
                        code = "QRCODEQRCODEQRCODEQRCODEQRCODE"
                    Case "yubin"
                        code = "99999999999999999999"
                    Case "itf"
                        code = "99999999999999"
                    Case "gs1128"
                        code = "#{99}99999999"
                        If design.Get("convenience_format") Then
                            code = "#{91}999999999999999999999999999999999999999999"
                        End If
                    Case Else
                        code = "9999999999999"
                End Select
            Else
                Dim fd As ElementDesign = design.Child("formatter")
                code = reportDesign.Setting.GetTextFormatter(fd.Get("type")).Format(data, fd)
            End If
            If code Is Nothing Then
                Exit Sub
            End If
            Dim _region As Region = region.ToPointScale(reportDesign)
            Dim g As Graphics = env.Graphics
            Try
                Select Case design.Get("barcode_type")
                    Case "ean8"
                        Dim barcode As New CEan8
                        If design.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        barcode.Render(g, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight, code)
                    Case "code39"
                        Dim barcode As New CCode39
                        If design.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        If design.Get("generate_checksum") Then
                            barcode.GenerateCheckSum = True
                        End If
                        barcode.Render(g, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight, code)
                    Case "code128"
                        Dim barcode As New CCode128
                        If design.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        barcode.Render(g, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight, code)
                    Case "codabar"
                        Dim barcode As New CCodabar
                        If design.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        If design.Get("generate_checksum") Then
                            barcode.GenerateCheckSum = True
                        End If
                        Dim ss As String = "A"
                        If Not design.IsNull("codabar_startstop_code") Then
                            ss = design.Get("codabar_startstop_code")
                        End If
                        barcode.Render(g, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight, ss & code & ss)
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
                        Dim bm As ByteMatrix
                        bm = w.encode(code, BarcodeFormat.QR_CODE, 0, 0, h)
                        Dim mw As Single = _region.GetWidth / bm.Width
                        Dim mh As Single = _region.GetHeight / bm.Height
                        For y As Integer = 0 To bm.Height - 1
                            For x As Integer = 0 To bm.Width - 1
                                If Not bm.Array(y)(x) Then
                                    g.FillRectangle(Brushes.Black, _region.Left + mw * x, _region.Top + mh * y, mw, mh)
                                End If
                            Next
                        Next
                    Case "yubin"
                        Dim barcode As New CYubinCustomer
                        Dim pt As Single = 10.0F
                        If Not design.IsNull("point") Then
                            pt = design.Get("point")
                        End If
                        barcode.Render(g, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight, pt, code)
                    Case "itf"
                        Dim barcode As New CItf
                        If design.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        If design.Get("generate_checksum") Then
                            barcode.GenerateCheckSum = True
                        End If
                        barcode.Render(g, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight, code)
                    Case "gs1128"
                        Dim barcode As New CGs1128
                        If design.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        If design.Get("convenience_format") Then
                            barcode.ConvenienceFormat = True
                        End If
                        barcode.Render(g, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight, code)
                    Case Else
                        Dim barcode As New CEan13
                        If design.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        barcode.Render(g, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight, code)
                End Select
            Catch ex As Exception
            End Try
        End Sub

    End Class

End Namespace