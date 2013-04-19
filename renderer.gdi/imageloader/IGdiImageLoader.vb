Imports System.Drawing

Namespace imageloader

    Public Interface IGdiImageLoader
        Function GetImage(ByVal param As Object) As Image
    End Interface

End Namespace
