Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports jp.co.systembase.report.component

Namespace barcode

    Public Module Yubin

        Public Function GetImage( _
          cb As PdfContentByte, _
          region As Region, _
          design As ElementDesign, _
          code As String) As Image
            Dim bc As New jp.co.systembase.barcode.Yubin
            Dim codes As List(Of Byte) = bc.Encode(code)
            If codes Is Nothing Then
                Return Nothing
            End If
            Dim uw As Single = region.GetWidth / (codes.Count * 2)
            Dim x As Single = 0
            Dim y As Single = region.GetHeight / 2
            Dim tmp As PdfTemplate = cb.CreateTemplate(region.GetWidth, region.GetHeight)
            tmp.SetColorFill(Color.BLACK)
            For Each c As Byte In codes
                Dim by As Single = 0
                Dim bh As Single = 0
                Select Case c
                    Case 1
                        by = y - uw * 3
                        bh = uw * 6
                    Case 2
                        by = y - uw
                        bh = uw * 4
                    Case 3
                        by = y - uw * 3
                        bh = uw * 4
                    Case 4
                        by = y - uw
                        bh = uw * 2
                End Select
                tmp.Rectangle(x, by, uw, bh)
                x += uw * 2
            Next
            tmp.Fill()
            Return Image.GetInstance(tmp)

        End Function

    End Module

End Namespace