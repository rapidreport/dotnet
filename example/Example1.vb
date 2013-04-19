Imports System.IO

Imports NPOI.HSSF.UserModel

Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.gdi.imageloader
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.pdf.imageloader
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.renderer.xls.imageloader

Module Example1

    Public Sub Run()
        Dim report As New Report(Json.Read("report\example1.rrpt"))
        report.Fill(New ReportDataSource(getDataTable))
        report.GlobalScope.Add("today", Today)
        Dim pages As ReportPages = report.GetPages()

        Dim imageMap As New ImageMap
        imageMap.Add("logo", New Bitmap("image\logo.png"))

        Using fs As New FileStream("output\example1.pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            renderer.ImageLoaderMap.Add("image", New PdfImageLoader(imageMap))
            pages.Render(renderer)
        End Using

        Using fs As New FileStream("output\example1.xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet("example1")
            renderer.ImageLoaderMap.Add("image", New XlsImageLoader(imageMap))
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        With Nothing
            Dim printer As New Printer(pages)
            printer.ImageLoaderMap.Add("image", New GdiImageLoader(imageMap))
            Dim preview As New FmPrintPreview(printer)
            preview.StartUpZoomFit = True
            preview.ShowDialog()
        End With
    End Sub

    Private Function getDataTable() As DataTable
        Dim ret As New DataTable
        ret.Columns.Add("PLACE", GetType(Decimal))
        ret.Columns.Add("TITLE", GetType(String))
        ret.Columns.Add("PRICE", GetType(Decimal))
        ret.Rows.Add(1, "Emacs辞典", 2980)
        ret.Rows.Add(1, "Rubyist Magazine 正しいRubyコードの書き方", 2600)
        ret.Rows.Add(1, "完全図解式ネットワーク再入門", 1480)
        ret.Rows.Add(1, "SQLクイックリファレンス", 2800)
        ret.Rows.Add(1, "MicrosoftSilverlight完全解説", 1905)
        ret.Rows.Add(1, "Eclipse3完全攻略", 2800)
        ret.Rows.Add(2, "Eclipse3.4プラグイン開発 徹底攻略", 3900)
        ret.Rows.Add(2, "dRubyによる分散・Webプログラミング", 3200)
        ret.Rows.Add(2, "デザインパターン", 4800)
        ret.Rows.Add(2, "Apacheハンドブック", 4600)
        ret.Rows.Add(2, "Ant", 2600)
        ret.Rows.Add(2, "HTML&XHTML", 5800)
        ret.Rows.Add(2, "JavaScript", 4200)
        ret.Rows.Add(2, "iText IN ACTION", 4000)
        ret.Rows.Add(2, "JQuery+JavaScript実践リファレンス", 2800)
        ret.Rows.Add(2, "基礎からのサーブレット/JSP", 2800)
        ret.Rows.Add(2, "RailsによるアジャイルWebアプリケーション開発", 3800)
        ret.Rows.Add(2, "Tomcatハンドブック", 4700)
        ret.Rows.Add(2, "入門UNIXシェルプログラミング", 3200)
        ret.Rows.Add(2, "プログラマのためのSQL", 4500)
        ret.Rows.Add(2, "PHP5徹底攻略", 2800)
        ret.Rows.Add(2, "JQueryクックブック", 3600)
        ret.Rows.Add(2, "PDFリファレンス", 6800)
        ret.Rows.Add(2, "プログラミングRuby 言語編", 3800)
        ret.Rows.Add(2, "プログラミングRuby ライブラリ編", 4200)
        ret.Rows.Add(2, "Effective Java", 3600)
        ret.Rows.Add(2, "Postfix実用ガイド", 3200)
        ret.Rows.Add(3, "LINQ実践サンプル集", 3800)
        ret.Rows.Add(3, "Cプログラミング診断室", 2427)
        ret.Rows.Add(3, "入門csh & tcsh", 2900)
        ret.Rows.Add(3, "ピープルウェア", 2200)
        ret.Rows.Add(3, "JavaからRubyへ", 2200)
        ret.Rows.Add(3, "ハッカーと画家", 2400)
        ret.Rows.Add(3, "EffectiveC++", 3800)
        ret.Rows.Add(3, "EffectiveSTL", 3200)
        ret.Rows.Add(3, "GNUソフトウェアプログラミング", 3200)
        ret.Rows.Add(3, "携帯端末用Web制作バイブル", 2980)
        ret.Rows.Add(3, "UNIXネットワーク管理者ハンドブック", 3980)
        ret.Rows.Add(3, "UNIXシステム管理者ハンドブック", 4800)
        ret.Rows.Add(3, "開発者ノートシリーズ Hibernate", 2400)
        ret.Rows.Add(3, "アジャイルプラクティス", 2400)
        ret.Rows.Add(3, "Joel on software", 2800)
        ret.Rows.Add(4, "オブジェクト指向入門", 4835)
        Return ret
    End Function


End Module
