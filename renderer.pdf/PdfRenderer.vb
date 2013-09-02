﻿Imports iTextSharp.text
Imports iTextSharp.text.pdf

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.textformatter
Imports jp.co.systembase.report.renderer
Imports jp.co.systembase.report.renderer.pdf.imageloader

Public Class PdfRenderer
    Implements IRenderer

    Public Setting As PdfRendererSetting
    Public ImageLoaderMap As New Dictionary(Of String, IPdfImageLoader)
    Public Document As Document
    Public Writer As PdfWriter
    Public Trans As TransClass

    Private imageCache As Dictionary(Of Hashtable, Dictionary(Of String, Image))

    Public Class TransClass
        Public PaperHeight As Single
        Public MarginLeft As Single
        Public MarginTop As Single
        Public Sub New(ByVal paperHeight As Single, ByVal marginLeft As Single, ByVal marginTop As Single)
            Me.PaperHeight = paperHeight
            Me.MarginLeft = marginLeft
            Me.MarginTop = marginTop
        End Sub
        Public Function X(ByVal _x As Single) As Single
            Return _x + Me.MarginLeft
        End Function
        Public Function Y(ByVal _y As Single) As Single
            Return Me.PaperHeight - (_y + Me.MarginTop)
        End Function
    End Class

    Public Sub New(ByVal os As System.IO.Stream)
        Me.new(os, New PdfRendererSetting)
    End Sub

    Public Sub New(ByVal os As System.IO.Stream, ByVal setting As PdfRendererSetting)
        Me.Setting = setting
        Me.Document = New Document
        Me.Writer = PdfWriter.GetInstance(Me.Document, os)
        Me.Writer.AddViewerPreference(PdfName.PRINTSCALING, PdfName.NONE)
    End Sub

    Public Sub BeginReport(ByVal reportDesign As ReportDesign) Implements IRenderer.BeginReport
        Me.ImageCache = New Dictionary(Of Hashtable, Dictionary(Of String, Image))
        Me.Document.Open()
    End Sub

    Public Sub EndReport(ByVal reportDesign As ReportDesign) Implements IRenderer.EndReport
        Me.Document.Close()
    End Sub

    Public Sub BeginPage(ByVal reportDesign As ReportDesign, ByVal pageIndex As Integer, ByVal paperRegion As Region) Implements IRenderer.BeginPage
        Dim pd As PaperDesign = reportDesign.PaperDesign
        Dim s As PaperSizeDesign = pd.GetActualSize.ToPoint(pd)
        Dim m As PaperMarginDesign = pd.Margin.ToPoint(pd)
        If m.OddReverse AndAlso (pageIndex Mod 2) Then
            Me.Trans = New TransClass(s.Height, m.Right, m.Top)
        Else
            Me.Trans = New TransClass(s.Height, m.Left, m.Top)
        End If
        Me.Document.SetPageSize(New RectangleReadOnly(s.Width, s.Height))
        Me.Document.SetMargins(0, 0, 0, 0)
        Me.Document.NewPage()
    End Sub

    Public Sub EndPage(ByVal reportDesign As ReportDesign) Implements IRenderer.EndPage
        Dim cb As PdfContentByte = Me.Writer.DirectContent
        cb.BeginText()
        cb.EndText()
    End Sub

    Public Sub RenderElement( _
      ByVal reportDesign As ReportDesign, _
      ByVal region As Region, _
      ByVal design As ElementDesign, _
      ByVal data As Object) Implements IRenderer.RenderElement
        Me.Setting.GetElementRenderer(design.Get("type")) _
          .Render(Me, reportDesign, region, design, data)
    End Sub

    Public Function GetImage(ByVal reportDesign As ReportDesign, ByVal desc As Hashtable, ByVal key As String) As Image
        If Not Me.imageCache.ContainsKey(desc) OrElse Not Me.imageCache(desc).ContainsKey(key) Then
            Me.createImageCache(reportDesign, desc, key)
        End If
        Return Me.imageCache(desc)(key)
    End Function

    Private Sub createImageCache(ByVal reportDesign As ReportDesign, ByVal desc As Hashtable, ByVal key As String)
        If Not Me.imageCache.ContainsKey(desc) Then
            Me.imageCache.Add(desc, New Dictionary(Of String, Image))
        End If
        If Me.imageCache(desc).ContainsKey(key) Then
            Me.imageCache(desc).Remove(key)
        End If
        If desc.ContainsKey(key) Then
            Dim img As System.Drawing.Image = reportDesign.GetImage(desc, key)
            If img IsNot Nothing Then
                Me.imageCache(desc).Add(key, Image.GetInstance(img, img.RawFormat))
            Else
                Me.imageCache(desc).Add(key, Nothing)
            End If
        Else
            Me.imageCache(desc).Add(key, Nothing)
        End If
    End Sub

End Class
