Imports System.Drawing

Imports NPOI.HSSF.UserModel

Namespace imageloader
    Public Interface IXlsImageLoader
        Function GetImage(ByVal param As Object) As Image
    End Interface
End Namespace