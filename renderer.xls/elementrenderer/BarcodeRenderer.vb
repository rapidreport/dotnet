Imports NPOI.HSSF.UserModel

Imports ZXing
Imports ZXing.Common
Imports ZXing.QrCode
Imports ZXing.QrCode.Internal

Imports jp.co.systembase.barcode
Imports jp.co.systembase.barcode.Barcode
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.xls.component

Namespace elementrenderer
    Public Class BarcodeRenderer
        Implements IElementRenderer

        Public Sub Collect( _
          ByVal renderer As XlsRenderer, _
          ByVal reportDesign As ReportDesign, _
          ByVal region As Region, _
          ByVal design As ElementDesign, _
          ByVal data As Object) Implements IElementRenderer.Collect
            Dim _region As Region = region.ToPointScale(reportDesign)
            Dim code As String = RenderUtil.Format(reportDesign, design.Child("formatter"), data)
            Dim shape As New Shape
            shape.Region = _region
            shape.Renderer = New BarcodeShapeRenderer(design, code)
            renderer.CurrentPage.Shapes.Add(shape)
        End Sub

        Public Class BarcodeShapeRenderer
            Implements IShapeRenderer
            Public Code As String
            Public Design As ElementDesign
            Public Sub New(ByVal design As ElementDesign, ByVal code As String)
                Me.Code = code
                Me.Design = design
            End Sub
            Public Sub Render(ByVal Page As Page, ByVal Shape As Shape) Implements IShapeRenderer.Render
                If Me.Code Is Nothing Then
                    Exit Sub
                End If
                Const scale As Integer = 10
                Dim width As Integer = CType(Shape.Region.GetWidth * scale, Integer)
                Dim height As Integer = CType(Shape.Region.GetHeight * scale, Integer)
                If width = 0 OrElse height = 0 Then
                    Exit Sub
                End If
                Dim image As New Bitmap(width, height)
                Dim g As Graphics = Graphics.FromImage(image)
                g.FillRectangle(Brushes.White, 0, 0, image.Width, image.Height)
                Dim type As String = Design.Get("barcode_type")
                Try
                    Select Case type
                        Case "ean8"
                            Dim barcode As New Ean8
                            If Design.Get("without_text") Then
                                barcode.WithText = False
                            End If
                            barcode.Render(g, 0, 0, image.Width, image.Height, Me.Code)
                        Case "code39"
                            Dim barcode As New Code39
                            If Design.Get("without_text") Then
                                barcode.WithText = False
                            End If
                            If Design.Get("generate_checksum") Then
                                barcode.GenerateCheckSum = True
                            End If
                            barcode.Render(g, 0, 0, image.Width, image.Height, Me.Code)
                        Case "codabar"
                            Dim barcode As New Codabar
                            If Design.Get("without_text") Then
                                barcode.WithText = False
                            End If
                            If Design.Get("generate_checksum") Then
                                barcode.GenerateCheckSum = True
                            End If
                            Dim ss As String = "A"
                            If Not Design.IsNull("codabar_startstop_code") Then
                                ss = Design.Get("codabar_startstop_code")
                            End If
                            If Design.Get("codabar_startstop_show") Then
                                barcode.WithStartStopText = True
                            End If
                            barcode.Render(g, 0, 0, image.Width, image.Height, ss & Me.Code & ss)
                        Case "itf"
                            Dim barcode As New Itf
                            If Design.Get("without_text") Then
                                barcode.WithText = False
                            End If
                            If Design.Get("generate_checksum") Then
                                barcode.GenerateCheckSum = True
                            End If
                            barcode.Render(g, 0, 0, image.Width, image.Height, Me.Code)
                        Case "code128"
                            Dim barcode As New Code128
                            If Design.Get("without_text") Then
                                barcode.WithText = False
                            End If
                            barcode.Render(g, 0, 0, image.Width, image.Height, Me.Code)
                        Case "gs1_128"
                            Dim barcode As New Gs1_128
                            If Design.Get("without_text") Then
                                barcode.WithText = False
                            End If
                            If Design.Get("gs1_conveni") Then
                                barcode.ConveniFormat = True
                            End If
                            barcode.Render(g, 0, 0, image.Width, image.Height, Me.Code)
                        Case "yubin"
                            Dim barcode As New Yubin
                            barcode.Render(g, 0, 0, image.Width, image.Height, Me.Code)
                        Case "qrcode"
                            Dim w As New QRCodeWriter
                            Dim h As New Dictionary(Of EncodeHintType, Object)
                            If Not Design.IsNull("qr_charset") Then
                                h.Add(EncodeHintType.CHARACTER_SET, Design.Get("qr_charset"))
                            Else
                                h.Add(EncodeHintType.CHARACTER_SET, "SJIS")
                            End If
                            If Not Design.IsNull("qr_correction_level") Then
                                Dim l As String = Design.Get("qr_correction_level")
                                If l = "L" Then
                                    h.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.L)
                                ElseIf l = "Q" Then
                                    h.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.Q)
                                ElseIf l = "H" Then
                                    h.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H)
                                Else
                                    h.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.M)
                                End If
                            Else
                                h.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.M)
                            End If
                            h.Add(EncodeHintType.DISABLE_ECI, True)
                            Dim bm As BitMatrix = w.encode(Me.Code, BarcodeFormat.QR_CODE, 0, 0, h)
                            Dim mw As Integer = Fix(image.Width / bm.Width)
                            Dim mh As Integer = Fix(image.Height / bm.Height)
                            Dim mgw As Integer = (image.Width - (mw * bm.Width)) / 2
                            Dim mgh As Integer = (image.Height - (mh * bm.Height)) / 2
                            For y As Integer = 0 To bm.Height - 1
                                For x As Integer = 0 To bm.Width - 1
                                    If bm(x, y) Then
                                        g.FillRectangle(Brushes.Black, mgw + x * mw, mgh + y * mh, mw, mh)
                                    End If
                                Next
                            Next
                        Case Else
                            Dim barcode As New Ean13
                            If Design.Get("without_text") Then
                                barcode.WithText = False
                            End If
                            barcode.Render(g, 0, 0, image.Width, image.Height, Me.Code)
                    End Select
                    Dim p As HSSFPatriarch = Page.Renderer.Sheet.DrawingPatriarch
                    Dim index As Integer = Page.Renderer.Workbook.AddPicture( _
                      (New ImageConverter).ConvertTo(image, GetType(Byte())), _
                      NPOI.SS.UserModel.PictureType.PNG)
                    p.CreatePicture(Shape.GetHSSFClientAnchor(Page.TopRow), index)
                Catch ex As Exception
                End Try
            End Sub
        End Class

    End Class
End Namespace