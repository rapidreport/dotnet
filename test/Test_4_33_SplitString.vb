Imports System.IO
Imports jp.co.systembase.json
Imports jp.co.systembase.report
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.gdi
Imports jp.co.systembase.report.renderer.pdf
Imports jp.co.systembase.report.renderer.xls
Imports jp.co.systembase.report.renderer.xlsx
Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel

Public Class Test_4_33_SplitString

    Public Overrides Function ToString() As String
        Return "4.33 文字列分割"
    End Function

    Public Sub Run()
        Dim name As String = "test_4_33_split_string"

        Dim report As New Report(Json.Read("rrpt\" & name & ".rrpt"))
        report.Fill(New ReportDataSource(Me.GetDataTable()))

        Dim pages As ReportPages = report.GetPages()

        Using fs As New FileStream("out\" & name & ".pdf", IO.FileMode.Create)
            Dim renderer As New PdfRenderer(fs)
            pages.Render(renderer)
        End Using

        Using fs As New FileStream("out\" & name & ".xls", IO.FileMode.Create)
            Dim workbook As New HSSFWorkbook
            Dim renderer As New XlsRenderer(workbook)
            renderer.NewSheet(name)
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        Using fs As New FileStream("out\" & name & ".xlsx", IO.FileMode.Create)
            Dim workbook As New XSSFWorkbook
            Dim renderer As New XlsxRenderer(workbook)
            renderer.NewSheet(name)
            pages.Render(renderer)
            workbook.Write(fs)
        End Using

        With Nothing
            Dim preview As New FmPrintPreview(New Printer(pages))
            preview.ShowDialog()
        End With
    End Sub

    Public Function GetDataTable() As DataTable
        Dim ret As New DataTable()
        ret.Columns.Add("text", GetType(String))

        ret.Rows.Add("遠野郷は今の陸中上閉伊郡の西の半分、山々にて取り囲かこまれたる平地なり。" & vbCrLf &
                     "新町村にては、遠野、土淵、附馬牛、松崎、青笹、上郷、小友、綾織、鱒沢、宮守、達曾部の一町十ヶ村に分かつ。" & vbCrLf &
                     "近代或いは西閉伊郡とも称し、中古にはまた遠野保とも呼べり。今日郡役所のある遠野町はすなわち一郷の町場にして、南部家一万石の城下なり。" & vbCrLf &
                     "城を横田城ともいう。" & vbCrLf &
                     "この地へ行くには花巻の停車場にて汽車を下おり、北上川を渡り、その川の支流猿さるヶ石川の渓を伝いて、東の方へ入ること十三里、遠野の町に至る。" & vbCrLf &
                     "山奥には珍しき繁華の地なり。伝えいう、遠野郷の地大昔はすべて一円の湖水なりしに、その水猿ヶ石川となりて人界に流れ出でしより、自然にかくのごとき邑落をなせしなりと。" & vbCrLf &
                     "されば谷川のこの猿ヶ石に落合うもの甚はなはだ多く、俗に七内八崎ありと称す。内は沢または谷のことにて、奥州の地名には多くあり。")
        ret.Rows.Add("遠野の町は南北の川の落合にあり。" & vbCrLf &
                     "以前は七七十里とて、七つの渓谷おのおの七十里の奥より売買の貨物を聚め、その市の日は馬千匹、人千人の賑わしさなりき。" & vbCrLf &
                     "四方の山々の中に最も秀でたるを早池峯という、北の方附馬牛の奥にあり。東の方には六角牛山立てり。石神という山は附馬牛と達曾部との間にありて、その高さ前の二つよりも劣れり。" & vbCrLf &
                     "大昔に女神あり、三人の娘を伴ないてこの高原に来たり、今の来内村の伊豆権現の社あるところに宿りし夜、今夜よき夢を見たらん娘によき山を与うべしと母の神の語りて寝たりしに、夜深く天より霊華降りて姉の姫の胸の上に止りしを、末の姫眼覚めて窃にこれを取り、わが胸の上に載せたりしかば、ついに最も美しき早池峯の山を得、姉たちは六角牛と石神とを得たり。若き三人の女神おのおの三の山に住し今もこれを領したもう故に、遠野の女どもはその妬を畏れて今もこの山には遊ばずといえり。")
        ret.Rows.Add("山々の奥には山人住めり。栃内村和野の佐々木嘉兵衛という人は今も七十余にて生存せり。" & vbCrLf &
                     "この翁若かりしころ猟をして山奥に入りしに、遥かなる岩の上に美しき女一人ありて、長き黒髪を梳りていたり。" & vbCrLf &
                     "顔の色きわめて白し。不敵の男なれば直に銃を差し向けて打ち放せしに弾に応じて倒れたり。" & vbCrLf &
                     "そこに馳けつけて見れば、身のたけ高き女にて、解きたる黒髪はまたそのたけよりも長かりき。" & vbCrLf &
                     "のちの験にせばやと思いてその髪をいささか切り取り、これを綰ねて懐に入れ、やがて家路に向いしに、道の程にて耐えがたく睡眠を催しければ、しばらく物蔭に立寄りてまどろみたり。" & vbCrLf &
                     "その間夢と現との境のようなる時に、これも丈の高き男一人近よりて懐中に手を差し入れ、かの綰ねたる黒髪を取り返し立ち去ると見ればたちまち睡は覚めたり。" & vbCrLf &
                     "山男なるべしといえり。")
        ret.Rows.Add("山口村の吉兵衛という家の主人、根子立という山に入り、笹を苅りて束となし担ぎて立上らんとする時、笹原の上を風の吹き渡るに心づきて見れば、奥の方なる林の中より若き女の穉児を負おいたるが笹原の上を歩みて此方へ来るなり。" & vbCrLf &
                     "きわめてあでやかなる女にて、これも長き黒髪を垂れたり。" & vbCrLf &
                     "児を結いつけたる紐は藤の蔓にて、着たる衣類は世の常の縞物なれど、裾のあたりぼろぼろに破れたるを、いろいろの木の葉などを添えて綴りたり。" & vbCrLf &
                     "足は地に着くとも覚えず。事もなげに此方に近より、男のすぐ前を通りて何方へか行き過ぎたり。" & vbCrLf &
                     "この人はその折の怖ろしさより煩い始めて、久しく病みてありしが、近きころ亡せたり。")
        ret.Rows.Add("遠野郷より海岸の田ノ浜、吉利吉里などへ越ゆるには、昔より笛吹峠という山路あり。" & vbCrLf &
                     "山口村より六角牛の方へ入り路のりも近かりしかど、近年この峠を越ゆる者、山中にて必ず山男山女に出逢うより、誰もみな怖ろしがりて次第に往来も稀になりしかば、ついに別の路を境木峠という方に開き、和山を馬次場として今は此方ばかりを越ゆるようになれり。" & vbCrLf &
                     "二里以上の迂路なり。")
        ret.Rows.Add("遠野郷にては豪農のことを今でも長者という。" & vbCrLf &
                     "青笹村大字糠前の長者の娘、ふと物に取り隠されて年久しくなりしに、同じ村の何某という猟師、或る日山に入りて一人の女に遭う。" & vbCrLf &
                     "怖ろしくなりてこれを撃たんとせしに、何おじではないか、ぶつなという。" & vbCrLf &
                     "驚きてよく見れば彼の長者がまな娘なり。" & vbCrLf &
                     "何故なにゆえにこんな処ところにはおるぞと問えば、或る物に取られて今はその妻となれり。" & vbCrLf &
                     "子もあまた生みたれど、すべて夫が食い尽して一人此のごとくあり。" & vbCrLf &
                     "おのれはこの地に一生涯を送ることなるべし。" & vbCrLf &
                     "人にも言うな。" & vbCrLf &
                     "御身も危うければ疾く帰れというままに、その在所をも問い明らめずして遁げ還れりという。")
        Return ret
    End Function

End Class
