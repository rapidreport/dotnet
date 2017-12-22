Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports jp.co.systembase.report.component

Namespace barcode

    Public Module Gs1_128

        Public Function GetImage( _
          cb As PdfContentByte, _
          region As Region, _
          design As ElementDesign, _
          code As String) As Image

            Dim barcode As New jp.co.systembase.barcode.Gs1_128
            If design.Get("without_text") Then
                barcode.WithText = False
            End If
            If design.Get("gs1_conveni") Then
                barcode.ConveniFormat = True
            End If

            Dim w As Single = region.GetWidth
            Dim h As Single = region.GetHeight
            Dim _h As Single = h
            If barcode.WithText Then
                If barcode.ConveniFormat Then
                    _h *= 0.5F
                Else
                    _h *= 0.7F
                End If
            End If
            If w <= 0 Or h <= 0 Then
                Return Nothing
            End If

            Dim _data As String = code
            barcode.Validate(_data)
            If barcode.ConveniFormat Then
                _data = barcode.PreprocessConveniData(_data)
            End If
            Dim ps As List(Of Integer) = _
                barcode.GetCodePoints(barcode.TrimData(_data), jp.co.systembase.barcode.Code128.ECodeType.C)
            Dim tmp As PdfTemplate = cb.CreateTemplate(region.GetWidth, region.GetHeight)
            tmp.SetColorFill(Color.BLACK)
            Dim mw As Single = w / ((ps.Count + 1) * 11 + 13)
            Dim draw As Boolean = True
            Dim x As Single = 0
            For Each c As Byte In barcode.Encode(ps)
                Dim dw As Single = c * mw
                If draw Then
                    tmp.Rectangle(x, tmp.Height - _h, dw, _h)
                End If
                draw = Not draw
                x += dw
            Next
            tmp.Fill()

            If barcode.WithText Then
                If barcode.ConveniFormat Then
                    Dim t As String = barcode.ConveniDisplayFormat(_data)
                    Dim t1 As String = t.Substring(0, 33)
                    Dim t2 As String = t.Substring(33)
                    tmp.BeginText()
                    Dim fs As Single = BarcodeUtil.GetFontSize(tmp, t1)
                    BarcodeUtil.SetFont(tmp, fs)
                    tmp.SetTextMatrix(0, tmp.Height - (_h + fs))
                    tmp.ShowText(t1)
                    tmp.SetTextMatrix(0, tmp.Height - (_h + fs * 2))
                    tmp.ShowText(t2)
                    tmp.EndText()
                Else
                    Dim t As String = barcode.DisplayFormat(_data)
                    BarcodeUtil.RenderText(tmp, t)
                End If
            End If

            Return Image.GetInstance(tmp)
        End Function

    End Module

End Namespace
