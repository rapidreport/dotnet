Imports System.Drawing

Namespace imageloader

    Public Interface IGdiImageLoader
        Function GetImage(param As Object) As Image
    End Interface

End Namespace
