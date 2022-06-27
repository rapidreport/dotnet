Imports System.Drawing

Imports jp.co.systembase.NPOI.XSSF.UserModel

Namespace imageloader
    Public Interface IXlsxImageLoader
        Function GetImage(param As Object) As Image
    End Interface
End Namespace