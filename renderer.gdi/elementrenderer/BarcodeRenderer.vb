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

        Public Overridable Sub Render(
          env As RenderingEnv,
          reportDesign As ReportDesign,
          region As Region,
          design As ElementDesign,
          data As Object) Implements IElementRenderer.Render
            Dim code = _GetCode(env, reportDesign, design, data)
            If code Is Nothing Then
                Exit Sub
            End If
            Try
                _RenderBarcode(env.Graphics, region.ToPointScale(reportDesign), design, code)
            Catch ex As Exception
            End Try
        End Sub

        Protected Overridable Function _GetCode(env As RenderingEnv, reportDesign As ReportDesign, design As ElementDesign, data As Object) As String
            If env.InDesigner Then
                Return GdiRenderUtil.GetBarcodeMock(design)
            Else
                Return RenderUtil.Format(reportDesign, design.Child("formatter"), data)
            End If
        End Function

        Protected Overridable Sub _RenderBarcode(g As Graphics, region As Region, design As ElementDesign, code As String)
            Select Case design.Get("barcode_type")
                Case "ean8"
                    Dim barcode As New Ean8
                    If design.Get("without_text") Then
                        barcode.WithText = False
                    End If
                    barcode.Render(g, region.Left, region.Top, region.GetWidth, region.GetHeight, code)
                Case "code39"
                    Dim barcode As New Code39
                    If design.Get("without_text") Then
                        barcode.WithText = False
                    End If
                    If design.Get("generate_checksum") Then
                        barcode.GenerateCheckSum = True
                    End If
                    barcode.Render(g, region.Left, region.Top, region.GetWidth, region.GetHeight, code)
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
                    barcode.Render(g, region.Left, region.Top, region.GetWidth, region.GetHeight, startCode & code & stopCode)
                Case "itf"
                    Dim barcode As New Itf
                    If design.Get("without_text") Then
                        barcode.WithText = False
                    End If
                    If design.Get("generate_checksum") Then
                        barcode.GenerateCheckSum = True
                    End If
                    barcode.Render(g, region.Left, region.Top, region.GetWidth, region.GetHeight, code)
                Case "code128"
                    Dim barcode As New Code128
                    If design.Get("without_text") Then
                        barcode.WithText = False
                    End If
                    barcode.Render(g, region.Left, region.Top, region.GetWidth, region.GetHeight, code)
                Case "gs1_128"
                    Dim barcode As New Gs1_128
                    If design.Get("without_text") Then
                        barcode.WithText = False
                    End If
                    If design.Get("gs1_conveni") Then
                        barcode.ConveniFormat = True
                    End If
                    barcode.Render(g, region.Left, region.Top, region.GetWidth, region.GetHeight, code)
                Case "yubin"
                    Dim barcode As New Yubin
                    barcode.Render(g, region.Left, region.Top, region.GetWidth, region.GetHeight, code)
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
                    Dim mw As Single = region.GetWidth / bm.Width
                    Dim mh As Single = region.GetHeight / bm.Height
                    For y As Integer = 0 To bm.Height - 1
                        For x As Integer = 0 To bm.Width - 1
                            If bm(x, y) Then
                                g.FillRectangle(Brushes.Black, region.Left + mw * x, region.Top + mh * y, mw, mh)
                            End If
                        Next
                    Next
                Case Else
                    Dim barcode As New Ean13
                    If design.Get("without_text") Then
                        barcode.WithText = False
                    End If
                    barcode.Render(g, region.Left, region.Top, region.GetWidth, region.GetHeight, code)
            End Select
        End Sub

    End Class

End Namespace