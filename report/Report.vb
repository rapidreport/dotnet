﻿Imports System.IO

Imports jp.co.systembase.report.component
Imports jp.co.systembase.report.customizer
Imports jp.co.systembase.report.elementpreprocessor
Imports jp.co.systembase.report.textformatter
Imports jp.co.systembase.report.data
Imports jp.co.systembase.report.data.internal
Imports jp.co.systembase.report.method
Imports jp.co.systembase.report.operator
Imports jp.co.systembase.report.scanner
Imports jp.co.systembase.report.renderer

Public Class Report

    Public Const PAPER_WIDTH_MAX As Single = 4999
    Public Const PAPER_HEIGHT_MAX As Single = 4999

    Public Enum EDirection
        VERTICAL
        HORIZONTAL
    End Enum

    Public Enum EPaperType
        A3
        A4
        A5
        B4
        B5
        CUSTOM
    End Enum

    Public Enum EScaleUnit
        POINT
        MM
        INCH
    End Enum

    Public Enum EEvalContext
        ANY
        CONTENT
        PAGE
    End Enum

    Public Enum EHAlign
        LEFT
        CENTER
        RIGHT
    End Enum

    Public Enum EVAlign
        TOP
        CENTER
        BOTTOM
    End Enum

    Public Enum EGroupDataMode
        DEFAULT_BLANK
        DEFAULT_DUMMY
        NO_SPLIT
    End Enum

    Public Class Compatibility
        Public Shared _4_6_PdfFontBold As Boolean = False
        Public Shared _4_13_2_AverageZero As Boolean = False
        Public Shared _4_15_PreviewSearchDisabled As Boolean = False
    End Class

    Public Design As ReportDesign
    Public Customizer As IReportCustomizer
    Public Data As ReportData = Nothing
    Public GroupDataProvider As IGroupDataProvider = Nothing
    Public Groups As Groups = Nothing
    Public GlobalScope As New Dictionary(Of String, Object)
    Public DataCache As New DataCache
    Public Filled As Boolean = False
    Public InDesigner As Boolean = False
    Public CustomFieldStack As New CustomField.Stack

    Public WrapperDataSourceMap As New Dictionary(Of GroupDesign, WrapperDataSource)

    Private _SubPageMap As New Dictionary(Of String, ReportPages)

    Public Sub New(ByVal desc As Hashtable)
        Me.New(desc, CType(Nothing, IReportCustomizer))
    End Sub

    Public Sub New(ByVal desc As Hashtable, ByVal setting As ReportSetting)
        Me.New(desc, setting, Nothing)
    End Sub

    Public Sub New(ByVal design As ReportDesign)
        Me.New(design, Nothing)
    End Sub

    Public Sub New(ByVal desc As Hashtable, ByVal customizer As IReportCustomizer)
        Me.New(New ReportDesign(desc), customizer)
    End Sub

    Public Sub New(ByVal desc As Hashtable, ByVal setting As ReportSetting, ByVal customizer As IReportCustomizer)
        Me.New(New ReportDesign(desc, setting), customizer)
    End Sub

    Public Sub New(ByVal design As ReportDesign, ByVal customizer As IReportCustomizer)
        Me.Design = design
        Me.Customizer = customizer
    End Sub

    Public Sub Fill(ByVal dataSource As IReportDataSource)
        Me.Fill(dataSource, Nothing)
    End Sub

    Public Sub Fill(ByVal dataSource As IReportDataSource, ByVal groupDataProvider As IGroupDataProvider)
        Me.GroupDataProvider = groupDataProvider
        Me.Data = New ReportData(dataSource, Me, Nothing)
        Me.Groups = New Groups(Me.Design.GroupDesign, Me, Nothing)
        Me.Groups.Fill(Me.Data)
        Me.Filled = True
    End Sub

    Public Function GetPages() As ReportPages
        Dim ret As New ReportPages(Me)
        Dim fch As ContentHistory = Me.Groups.GetNextContentHistory(Nothing)
        Dim paperRegion As Region = Me.Design.PaperDesign.GetRegion
        Do While fch IsNot Nothing
            Dim range As GroupRange = Nothing
            Dim scanner As PagingScanner = Nothing
            Dim rangeAux As New GroupRange(Me.Groups, fch, fch)
            Dim scannerAux As New PagingScanner
            Me.Groups.Scan(scannerAux, rangeAux, paperRegion)
            Dim lch As ContentHistory = Me.Groups.GetNextContentHistory(fch)
            Do While lch IsNot Nothing
                Dim _range As New GroupRange(Me.Groups, fch, lch)
                Dim _scanner As New PagingScanner
                Me.Groups.Scan(_scanner, _range, paperRegion)
                If _scanner.Broken Then
                    Exit Do
                End If
                If Me.Design.PageCapacity > 0 AndAlso Me.Design.PageCapacity < _scanner.Weight Then
                    Exit Do
                End If
                lch = Me.Groups.GetNextContentHistory(lch)
                rangeAux = _range
                scannerAux = _scanner
                If Not _range.Unbreakable OrElse lch Is Nothing Then
                    range = rangeAux
                    scanner = scannerAux
                End If
            Loop
            If range Is Nothing Then
                range = rangeAux
                scanner = scannerAux
            End If
            Dim page As New ReportPage(Me, range, scanner)
            If ret.Count = 0 AndAlso page.Report.Design.ResetPageCount Then
                page.ResetPageCount = True
            Else
                For Each gd As GroupDesign In page.StartedGroups.Keys
                    If gd.ResetPageCount Then
                        page.ResetPageCount = True
                        Exit For
                    End If
                Next
            End If
            ret.Add(page)
            If Me.Customizer IsNot Nothing Then
                Me.Customizer.PageAdded(Me, ret, page)
            End If
            If range.Last Is Nothing Then
                fch = Nothing
            Else
                fch = Me.Groups.GetNextContentHistory(range.Last)
            End If
        Loop
        Return ret
    End Function

    Public Overrides Function ToString() As String
        If Me.Design.Caption IsNot Nothing Then
            Return Me.Design.Caption
        ElseIf Me.Design.Id IsNot Nothing Then
            Return Me.Design.Id
        Else
            Return "(report)"
        End If
    End Function

    Public Sub AddSubPages(ByVal key As String, ByVal pages As ReportPages)
        pages.SetUpCountingPages()
        Me._SubPageMap.Add(key, pages)
    End Sub

    Public Sub RenderSubPage(ByVal renderer As IRenderer, ByVal region As Region, ByVal key As String, ByVal index As Integer)
        If Not Me._SubPageMap.ContainsKey(key) Then
            Exit Sub
        End If
        Dim pages As ReportPages = Me._SubPageMap(key)
        pages(index).RenderSubPage(renderer, pages, region)
    End Sub

End Class
