Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports ZXing
Imports ZXing.Common
Imports ZXing.QrCode
Imports ZXing.QrCode.Internal

Imports jp.co.systembase.report.component

Namespace barcode

    Public Module QRCode

        Public Function GetImage( _
          cb As PdfContentByte, _
          region As Region, _
          design As ElementDesign, _
          code As String) As Image
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
            Dim bm As BitMatrix = w.encode(code, BarcodeFormat.QR_CODE, 0, 0, h)
            Dim tmp As PdfTemplate = cb.CreateTemplate(bm.Width, bm.Height)
            tmp.SetColorFill(Color.BLACK)
            For y As Integer = 0 To bm.Height - 1
                For x As Integer = 0 To bm.Width - 1
                    If bm(x, y) Then
                        tmp.Rectangle(x, bm.Height - y, 1, 1)
                    End If
                Next
            Next
            tmp.Fill()
            Return Image.GetInstance(tmp)
        End Function

    End Module

End Namespace
