Module Main

    Public Tests As New List(Of Object)

    Public Sub Main()
        System.Windows.Forms.Application.EnableVisualStyles()

        Tests.Add(New Test_0_1)
        Tests.Add(New Test_0_2)
        Tests.Add(New Test_0_3)
        Tests.Add(New Test_0_4)
        Tests.Add(New Test_0_5_PDF)
        Tests.Add(New Test_0_5_XLS)
        Tests.Add(New Test_0_5_XLSX)
        Tests.Add(New Test_4_17)
        Tests.Add(New Test_4_18_Trim)
        Tests.Add(New Test_4_18_MExp)
        Tests.Add(New Test_4_20_Code128)
        Tests.Add(New Test_4_20_FormatNull)
        Tests.Add(New Test_4_20_MergeContent)
        Tests.Add(New Test_4_21_LocateCount)
        Tests.Add(New Test_4_21_MergeContentLogo)
        Tests.Add(New Test_4_23_Rect)
        Tests.Add(New Test_4_24_Exp)
        Tests.Add(New Test_4_24_SubPage)
        Tests.Add(New Test_4_25_Page)
        Tests.Add(New Test_4_26_UnknownField)

        Application.Run(New FmTest)
    End Sub

End Module
