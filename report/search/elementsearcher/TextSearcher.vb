﻿Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.expression

Namespace search.elementseacher
    Public Class TextSearcher
        Implements IElementSearcher
        Public Function Search( _
          reportDesign As ReportDesign, _
          design As ElementDesign, _
          data As Object, _
          keyword As String) As Boolean Implements IElementSearcher.Search
            Dim text As String = design.Get("text")
            If data IsNot Nothing Then
                Dim textProcessor As New EmbeddedTextProcessor
                text = textProcessor.EmbedData(reportDesign, design.Child("formatter"), text, data)
            End If
            If text IsNot Nothing Then
                Return text.ToLower.IndexOf(keyword.ToLower) >= 0
            Else
                Return False
            End If
        End Function
    End Class
End Namespace