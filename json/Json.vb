Imports Newtonsoft.Json
Imports System.IO

Public Module Json

    Public Sub Write(data As Hashtable, path As String)
        Using w As New StreamWriter(path)
            Write(data, w)
        End Using
    End Sub

    Public Sub Write(data As Hashtable, writer As TextWriter)
        Using w As New JsonTextWriter(writer)
            Write(data, w)
        End Using
    End Sub

    Public Sub Write(data As Hashtable, writer As JsonWriter)
        writeHash(data, writer)
    End Sub

    Public Function Read(path As String) As Hashtable
        Using reader As New StreamReader(path)
            Return Read(reader)
        End Using
    End Function

    Public Function Read(reader As TextReader) As Hashtable
        Return Read(New JsonTextReader(reader))
    End Function

    Public Function Read(reader As JsonReader) As Hashtable
        If reader.Read Then
            Return readHash(reader)
        Else
            Return Nothing
        End If
    End Function

    Private Sub writeNode(data As Object, writer As JsonWriter)
        If TypeOf data Is Hashtable Then
            writeHash(data, writer)
        ElseIf TypeOf data Is ArrayList Then
            writeArray(data, writer)
        ElseIf data Is Nothing Then
            writer.WriteNull()
        Else
            writer.WriteValue(data)
        End If
    End Sub

    Private Sub writeHash(data As Hashtable, writer As JsonWriter)
        writer.WriteStartObject()
        For Each k As String In data.Keys
            writer.WritePropertyName(k)
            writeNode(data(k), writer)
        Next
        writer.WriteEndObject()
    End Sub

    Private Sub writeArray(data As ArrayList, writer As JsonWriter)
        writer.WriteStartArray()
        For Each v As Object In data
            writeNode(v, writer)
        Next
        writer.WriteEndArray()
    End Sub

    Private Function readNode(reader As JsonReader) As Object
        Select Case reader.TokenType
            Case JsonToken.StartArray
                Return readArray(reader)
            Case JsonToken.StartObject
                Return readHash(reader)
            Case Else
                Return reader.Value
        End Select
    End Function

    Private Function readArray(reader As JsonReader) As ArrayList
        Dim ret As New ArrayList
        Do While reader.Read
            If reader.TokenType = JsonToken.EndArray Then
                Return ret
            End If
            ret.Add(readNode(reader))
        Loop
        Return ret
    End Function

    Private Function readHash(reader As JsonReader) As Hashtable
        Dim ret As New Hashtable
        Do While reader.Read
            Dim key As Object = Nothing
            If reader.TokenType = JsonToken.EndObject Then
                Return ret
            End If
            If reader.TokenType = JsonToken.PropertyName Then
                key = reader.Value
                reader.Read()
            End If
            ret.Add(key, readNode(reader))
        Loop
        Return ret
    End Function

End Module