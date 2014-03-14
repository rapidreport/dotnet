Imports System.IO
Imports System.Drawing

Imports jp.co.systembase.report.component

Public Class ReportDesign

    Public Desc As Hashtable = Nothing
    Public Setting As ReportSetting

    Public Caption As String = Nothing
    Public Id As String = Nothing
    Public PaperDesign As PaperDesign = Nothing
    Public DefaultFont As TextDesign.FontDesign = Nothing
    Public DefaultLineWidth As Single = 0
    Public PageCapacity As Integer = 0
    Public ResetPageCount As Boolean = False
    Public CustomFields As Dictionary(Of String, String) = Nothing
    Public CustomFieldsKeyList As List(Of String) = Nothing
    Public PaperName As String = Nothing
    Public PrinterName As String = Nothing
    Public GroupDesign As GroupDesign = Nothing
    Public Memo As String = Nothing

    Private imageCache As New Dictionary(Of Hashtable, Dictionary(Of String, Image))

    Public Sub New(ByVal desc As Hashtable)
        Me.New(desc, New ReportSetting)
    End Sub

    Public Sub New(ByVal desc As Hashtable, ByVal setting As ReportSetting)
        Me.Desc = desc
        Me.Setting = setting
        Me.LoadDesc()
        Me.LoadSubDesc()
    End Sub

    Public Sub LoadDesc()
        Me.Caption = Me.Desc("caption")
        Me.Id = Me.Desc("id")
        If Me.Desc.ContainsKey("paper") Then
            Me.PaperDesign = New PaperDesign(Me.Desc("paper"))
        Else
            Me.PaperDesign = New PaperDesign
        End If
        Me.DefaultFont = New TextDesign.FontDesign(CType(Me.Desc("font"), Hashtable))
        If Me.Desc.ContainsKey("line_width") Then
            Me.DefaultLineWidth = Me.Desc("line_width")
        Else
            Me.DefaultLineWidth = 1.0F
        End If
        Me.PageCapacity = Me.Desc("page_capacity")
        Me.ResetPageCount = Me.Desc("reset_page_count")
        If Me.Desc.ContainsKey("custom_fields") Then
            Me.CustomFields = New Dictionary(Of String, String)
            Me.CustomFieldsKeyList = New List(Of String)
            For Each d As Hashtable In Me.Desc("custom_fields")
                If d.ContainsKey("key") AndAlso d.ContainsKey("exp") Then
                    If Not Me.CustomFields.ContainsKey(d("key")) Then
                        Me.CustomFields.Add(d("key"), d("exp"))
                        Me.CustomFieldsKeyList.Add(d("key"))
                    End If
                End If
            Next
        Else
            Me.CustomFields = Nothing
            Me.CustomFieldsKeyList = Nothing
        End If
        Me.PrinterName = Desc("printer_name")
        Me.PaperName = Desc("paper_name")
        Me.Memo = Desc("memo")
    End Sub

    Public Sub LoadSubDesc()
        If Not Me.Desc.ContainsKey("group") Then
            Throw New ArgumentException("report has no group")
        End If
        Me.GroupDesign = New GroupDesign(Desc("group"), Me, Nothing)
    End Sub

    Public Function FindContentDesign(ByVal id As String) As ContentDesign
        If Me.GroupDesign IsNot Nothing Then
            Dim ret As ContentDesign = Me.GroupDesign.FindContentDesign(id)
            If ret IsNot Nothing Then
                Return ret
            End If
        End If
        Return Nothing
    End Function

    Public Function FindGroupDesign(ByVal id As String) As GroupDesign
        If Me.GroupDesign IsNot Nothing Then
            Dim ret As GroupDesign = Me.GroupDesign.FindGroupDesign(id)
            If ret IsNot Nothing Then
                Return ret
            End If
        End If
        Return Nothing
    End Function

    Public Function GetImage(ByVal desc As Hashtable, ByVal key As String) As Image
        If Not Me.ImageCache.ContainsKey(desc) OrElse Not Me.ImageCache(desc).ContainsKey(key) Then
            Me.CreateImageCache(desc, key)
        End If
        Return Me.ImageCache(desc)(key)
    End Function

    Public Sub CreateImageCache(ByVal desc As Hashtable, ByVal key As String)
        Dim img As Image = Nothing
        If desc.ContainsKey(key) Then
            Try
                Dim imageBytes() As Byte = Convert.FromBase64String(desc(key))
                img = Image.FromStream(New MemoryStream(imageBytes, 0, imageBytes.Length), True)
            Catch ex As Exception
            End Try
        End If
        Me.CreateImageCache(desc, key, img)
    End Sub

    Public Sub CreateImageCache(ByVal desc As Hashtable, ByVal key As String, ByVal image As Image)
        If Not Me.imageCache.ContainsKey(desc) Then
            Me.imageCache.Add(desc, New Dictionary(Of String, Image))
        End If
        If Me.imageCache(desc).ContainsKey(key) Then
            Me.imageCache(desc).Remove(key)
        End If
        Me.imageCache(desc).Add(key, image)
    End Sub

    Public Sub RemoveImageCache(ByVal desc As Hashtable, ByVal key As String)
        If Not Me.imageCache.ContainsKey(desc) Then
            Exit Sub
        End If
        If Me.imageCache(desc).ContainsKey(key) Then
            Me.imageCache(desc).Remove(key)
        End If
    End Sub

    Public Sub RemoveImageCache(ByVal desc As Hashtable)
        If Me.imageCache.ContainsKey(desc) Then
            Me.imageCache.Remove(desc)
        End If
    End Sub

End Class
