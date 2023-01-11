Imports jp.co.systembase.NPOI.SS.UserModel
Imports jp.co.systembase.NPOI.XSSF.UserModel

Imports ZXing
Imports ZXing.Common
Imports ZXing.QrCode
Imports ZXing.QrCode.Internal

Imports jp.co.systembase.barcode
Imports jp.co.systembase.barcode.Barcode
Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.xlsx.component

Namespace elementrenderer
    Public Class BarcodeRenderer
        Implements IElementRenderer

        Public Overridable Sub Collect(
          renderer As XlsxRenderer,
          reportDesign As ReportDesign,
          region As Region,
          design As ElementDesign,
          data As Object) Implements IElementRenderer.Collect
            renderer.CurrentPage.Shapes.Add(New Shape With {
                .Region = region.ToPointScale(reportDesign),
                .Renderer = New BarcodeShapeRenderer(design, _GetCode(reportDesign, design, data))
            })
        End Sub

        Protected Overridable Function _GetCode(reportDesign As ReportDesign, design As ElementDesign, data As Object) As String
            Return RenderUtil.Format(reportDesign, design.Child("formatter"), data)
        End Function

        Public Class BarcodeShapeRenderer
            Implements IShapeRenderer

            Public Code As String
            Public Design As ElementDesign

            Public Sub New(design As ElementDesign, code As String)
                Me.Code = code
                Me.Design = design
            End Sub

            Public Overridable Sub Render(page As Page, shape As Shape) Implements IShapeRenderer.Render
                If Me.Code Is Nothing Then
                    Exit Sub
                End If
                Const scale As Integer = 10
                Dim width As Integer = CType(shape.Region.GetWidth * scale, Integer)
                Dim height As Integer = CType(shape.Region.GetHeight * scale, Integer)
                If width = 0 OrElse height = 0 Then
                    Exit Sub
                End If
                Using image As New Bitmap(width, height)
                    Try
                        _RenderBarcode(image)
                        Dim p As IDrawing = page.Renderer.Sheet.CreateDrawingPatriarch
                        Dim index As Integer = page.Renderer.Workbook.AddPicture(
                          (New ImageConverter).ConvertTo(image, GetType(Byte())),
                          NPOI.SS.UserModel.PictureType.PNG)
                        p.CreatePicture(shape.GetXSSFClientAnchor(page.TopRow), index)
                    Catch ex As Exception
                    End Try
                End Using
            End Sub

            Protected Overridable Sub _RenderBarcode(image As Bitmap)
                Using g As Graphics = Graphics.FromImage(image)
                    g.FillRectangle(Brushes.White, 0, 0, image.Width, image.Height)
                    Dim type As String = Design.Get("barcode_type")
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
                            Dim startCode As String = "A"
                            Dim stopCode As String = "A"
                            If Not Design.IsNull("codabar_startstop_code") Then
                                Dim ss As String = Design.Get("codabar_startstop_code")
                                If ss.Length = 1 Then
                                    startCode = ss
                                    stopCode = ss
                                ElseIf ss.Length > 1 Then
                                    startCode = ss(0)
                                    stopCode = ss(1)
                                End If
                            End If
                            If Design.Get("codabar_startstop_show") Then
                                barcode.WithStartStopText = True
                            End If
                            barcode.Render(g, 0, 0, image.Width, image.Height, startCode & Me.Code & stopCode)
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
                End Using
            End Sub

        End Class

    End Class
End Namespace