Imports iTextSharp.text
Imports iTextSharp.text.pdf

Namespace barcode

    Public Module BarcodeUtil

        Public Sub RenderText(tmp As PdfTemplate, text As String)
            tmp.BeginText()
            Dim fs As Single = GetFontSize(tmp, text)
            SetFont(tmp, fs)
            tmp.SetTextMatrix((tmp.Width - (text.Length + 2) * fs / 2) / 2, tmp.Height - (tmp.Height * 0.7 + fs))
            tmp.ShowText(text)
            tmp.EndText()
        End Sub

        Public Function GetFontSize(tmp As PdfTemplate, text As String) As Single
            Dim ret As Single = tmp.Height * 0.3
            Return Math.Min(ret, ((tmp.Width * 0.9) / text.Length) * 2)
        End Function

        Public Sub SetFont(tmp As PdfTemplate, fontSize As Single)
            Dim f As BaseFont = BaseFont.CreateFont("Helvetica", "winansi", False)
            tmp.SetFontAndSize(f, fontSize)
        End Sub

    End Module

End Namespace