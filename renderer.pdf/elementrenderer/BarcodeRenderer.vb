Imports iTextSharp.text
Imports iTextSharp.text.pdf

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
                        If design.Get("codabar_startstop_show") Then
                            bc.StartStopText = True
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
                        image = barcode.Gs1_128.GetImage(cb, _region, design, code)
                    Case "yubin"
                        image = barcode.Yubin.GetImage(cb, _region, design, code)
                    Case "qrcode"
                        image = barcode.QRCode.GetImage(cb, _region, design, code)
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