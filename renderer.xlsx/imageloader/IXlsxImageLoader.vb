Imports System.Drawing

Imports NPOI.XSSF.UserModel

Namespace imageloader
    Public Interface IXlsxImageLoader
        Function GetImage(param As Object) As Image
    End Interface
End Namespace