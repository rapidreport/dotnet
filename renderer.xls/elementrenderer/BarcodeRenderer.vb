﻿Imports NPOI.HSSF.UserModel

Imports com.google.zxing
Imports com.google.zxing.common
Imports com.google.zxing.qrcode
Imports com.google.zxing.qrcode.decoder

Imports jp.co.systembase.barcode

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.renderer.xls.component
Imports jp.co.systembase.report.renderer

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
            Public ElementDesc As ElementDesign
            Public Sub New(ByVal elementDesc As ElementDesign, ByVal code As String)
                Me.Code = code
                Me.ElementDesc = elementDesc
            End Sub
            Public Sub Render(ByVal Page As Page, ByVal Shape As Shape) Implements IShapeRenderer.Render
                If Me.Code Is Nothing Then
                    Exit Sub
                End If
                Dim image As New Bitmap(CType(Shape.Region.GetWidth * 5, Integer), _
                                        CType(Shape.Region.GetHeight * 5, Integer))
                Dim g As Graphics = Graphics.FromImage(image)
                g.FillRectangle(Brushes.White, 0, 0, image.Width, image.Height)
                Dim type As String = ElementDesc.Get("barcode_type")
                Try
                    If type IsNot Nothing AndAlso type = "code39" Then
                        Dim barcode As New CCode39
                        If ElementDesc.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        If ElementDesc.Get("generate_checksum") Then
                            barcode.GenerateCheckSum = True
                        End If
                        barcode.Render(g, 0, 0, image.Width, image.Height, Me.Code)
                    ElseIf type IsNot Nothing AndAlso type = "ean8" Then
                        Dim barcode As New CEan8
                        If ElementDesc.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        barcode.Render(g, 0, 0, image.Width, image.Height, Me.Code)
                    ElseIf type IsNot Nothing AndAlso type = "code128" Then
                        Dim barcode As New CCode128
                        If ElementDesc.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        barcode.Render(g, 0, 0, image.Width, image.Height, Me.Code)
                    ElseIf type IsNot Nothing AndAlso type = "codabar" Then
                        Dim barcode As New CCodabar
                        If ElementDesc.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        If ElementDesc.Get("generate_checksum") Then
                            barcode.GenerateCheckSum = True
                        End If
                        Dim ss As String = "A"
                        If Not ElementDesc.IsNull("codabar_startstop_code") Then
                            ss = ElementDesc.Get("codabar_startstop_code")
                        End If
                        barcode.Render(g, 0, 0, image.Width, image.Height, ss & Me.Code & ss)
                    ElseIf type IsNot Nothing AndAlso type = "qrcode" Then
                        Dim w As New QRCodeWriter
                        Dim h As New Hashtable
                        If Not ElementDesc.IsNull("qr_charset") Then
                            h.Add(EncodeHintType.CHARACTER_SET, ElementDesc.Get("qr_charset"))
                        Else
                            h.Add(EncodeHintType.CHARACTER_SET, "shift_jis")
                        End If
                        If Not ElementDesc.IsNull("qr_correction_level") Then
                            Dim l As String = ElementDesc.Get("qr_correction_level")
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
                        Dim bm As ByteMatrix = w.encode(Me.Code, BarcodeFormat.QR_CODE, 0, 0, h)
                        Dim mw As Integer = image.Width / bm.Width
                        Dim mh As Integer = image.Width / bm.Height
                        For y As Integer = 0 To bm.Height - 1
                            For x As Integer = 0 To bm.Width - 1
                                If Not bm.Array(y)(x) Then
                                    g.FillRectangle(Brushes.Black, x * mw, y * mh, mw, mh)
                                End If
                            Next
                        Next
                    Else
                        Dim barcode As New CEan13
                        If ElementDesc.Get("without_text") Then
                            barcode.WithText = False
                        End If
                        barcode.Render(g, 0, 0, image.Width, image.Height, Me.Code)
                    End If
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