Module Test_0_5_Data

    Public Function GetDataTable() As DataTable
        Dim ret As New DataTable()
        ret.Columns.Add("bumonCd", GetType(Decimal))
        ret.Columns.Add("bumon", GetType(String))
        ret.Columns.Add("uriageDate", GetType(DateTime))
        ret.Columns.Add("denpyoNo", GetType(Decimal))
        ret.Columns.Add("shohinCd", GetType(String))
        ret.Columns.Add("shohin", GetType(String))
        ret.Columns.Add("tanka", GetType(Decimal))
        ret.Columns.Add("suryo", GetType(Decimal))
        For i As Integer = 1 To 100
            For j As Integer = 1 To 50
                ret.Rows.Add(i, "部門" & i, _
                    DateTime.ParseExact("2013/02/01", "yyyy/MM/dd", Nothing), _
                    j, "PC00001", "ノートパソコン", 70000, 10)
                ret.Rows.Add(i, "部門" & i, _
                    DateTime.ParseExact("2013/02/01", "yyyy/MM/dd", Nothing), _
                    j, "DP00002", "モニター", 25000, 10)
                ret.Rows.Add(i, "部門" & i, _
                    DateTime.ParseExact("2013/02/01", "yyyy/MM/dd", Nothing), _
                    j, "PR00003", "プリンタ", 20000, 2)
                ret.Rows.Add(i, "部門" & i, _
                    DateTime.ParseExact("2013/02/10", "yyyy/MM/dd", Nothing), _
                    j, "PR00003", "プリンタ", 20000, 3)
            Next
        Next
        Return ret
    End Function

End Module
