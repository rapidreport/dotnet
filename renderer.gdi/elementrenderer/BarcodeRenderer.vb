Imports ZXing
Imports ZXing.Common
Imports ZXing.QrCode

Imports jp.co.systembase.barcode
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer
Imports ZXing.QrCode.Internal

Namespace elementrenderer

    Public Class BarcodeRenderer
        Implements IElementRenderer

        Public Sub Render( _
          env As RenderingEnv, _
          reportDesign As ReportDesign, _
          region As Region, _
          design As ElementDesign, _
          data As Object) Implements IElementRenderer.Render
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
                    Case "gs1_128"
                        code = "(99)99999999"
                        If design.Get("gs1_conveni") Then
                            code = "(91)999999999999999999999999999999999999999999"
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
                        Dim barcode As New Ean8
                        If design.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        barcode.Render(g, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight, code)
                    Case "code39"
                        Dim barcode As New Code39
                        If design.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        If design.Get("generate_checksum") Then
                            barcode.GenerateCheckSum = True
                        End If
                        barcode.Render(g, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight, code)
                    Case "codabar"
                        Dim barcode As New Codabar
                        If design.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        If design.Get("generate_checksum") Then
                            barcode.GenerateCheckSum = True
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
                            barcode.WithStartStopText = True
                        End If
                        barcode.Render(g, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight, startCode & code & stopCode)
                    Case "itf"
                        Dim barcode As New Itf
                        If design.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        If design.Get("generate_checksum") Then
                            barcode.GenerateCheckSum = True
                        End If
                        barcode.Render(g, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight, code)
                    Case "code128"
                        Dim barcode As New Code128
                        If design.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        barcode.Render(g, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight, code)
                    Case "gs1_128"
                        Dim barcode As New Gs1_128
                        If design.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        If design.Get("gs1_conveni") Then
                            barcode.ConveniFormat = True
                        End If
                        barcode.Render(g, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight, code)
                    Case "yubin"
                        Dim barcode As New Yubin
                        barcode.Render(g, _region.Left, _region.Top, _region.GetWidth, _region.GetHeight, code)
                    Case "qrcode"
                        Dim w As New QRCodeWriter
                        Dim h As New Dictionary(Of EncodeHintType, Object)
                        If Not design.IsNull("qr_charset") Then
                            h.Add(EncodeHintType.CHARACTER_SET, design.Get("qr_charset"))
                        Else
                            h.Add(EncodeHintType.CHARACTER_SET, "SJIS")
                        End If
                        If Not design.IsNull("qr_correction_level") Then
                            Select Case design.Get("qr_correction_level")
                                Case "L"
                                    h.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.L)
                                Case "Q"
                                    h.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.Q)
                                Case "H"
                                    h.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H)
                                Case Else
                                    h.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.M)
                            End Select
                        Else
                            h.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.M)
                        End If
                        h.Add(EncodeHintType.DISABLE_ECI, True)
                        Dim bm As BitMatrix
                        bm = w.encode(code, BarcodeFormat.QR_CODE, 0, 0, h)
                        Dim mw As Single = _region.GetWidth / bm.Width
                        Dim mh As Single = _region.GetHeight / bm.Height
                        For y As Integer = 0 To bm.Height - 1
                            For x As Integer = 0 To bm.Width - 1
                                If bm(x, y) Then
                                    g.FillRectangle(Brushes.Black, _region.Left + mw * x, _region.Top + mh * y, mw, mh)
                                End If
                            Next
                        Next
                    Case Else
                        Dim barcode As New Ean13
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