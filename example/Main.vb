Imports iTextSharp.text.pdf

Imports jp.co.systembase.report
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.pdf

Module Main

    Public Sub main()
        System.Windows.Forms.Application.EnableVisualStyles()
        'CPdfRenderer.DefaultSetting.FontMap("gothic") = _
        '  BaseFont.CreateFont("c:\windows\fonts\msgothic.ttc,0", BaseFont.IDENTITY_H, BaseFont.EMBEDDED)
        'CPdfRenderer.DefaultSetting.FontMap("mincho") = _
        '  BaseFont.CreateFont("c:\windows\fonts\msmincho.ttc,0", BaseFont.IDENTITY_H, BaseFont.EMBEDDED)
        Application.Run(New FmMenu)
    End Sub

End Module