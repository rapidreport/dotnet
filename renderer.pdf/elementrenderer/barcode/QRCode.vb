Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports com.google.zxing
Imports com.google.zxing.common
Imports com.google.zxing.qrcode

Imports jp.co.systembase.report.component

Namespace barcode

    Public Module QRCode

        Public Function GetImage( _
          ByVal cb As PdfContentByte, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal code As String) As Image
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
            Return Image.GetInstance(tmp)
        End Function

    End Module

End Namespace
