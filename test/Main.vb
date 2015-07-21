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

        Application.Run(New FmTest)
    End Sub

End Module
