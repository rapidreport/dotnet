Imports jp.co.systembase.report.data

Namespace component
    Public Class UnknownFieldException
        Inherits Exception
        Public dataSource As IReportDataSource
        Public key As String
        Public i As Integer
        Public Sub New(ByVal dataSource As IReportDataSource, ByVal i As Integer, ByVal key As String)
            MyBase.New("不明な列です：" & key & " 行番号：" & i)
            Me.dataSource = dataSource
            Me.key = key
            Me.i = i
        End Sub
    End Class
End Namespace
