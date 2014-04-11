Imports System.IO

Public Class config

    Private dicConfig As Dictionary(Of String, String)

    Public Sub New(ByVal strConfigFile As String)
        Dim fileReader As StreamReader
        Dim strFileLine As String
        Dim arrTmpConfigLine(1) As String

        dicConfig = New Dictionary(Of String, String)
        fileReader = New StreamReader(strConfigFile)

        While Not fileReader.Peek = -1

            strFileLine = fileReader.ReadLine()
            arrTmpConfigLine = Split(strFileLine, vbTab)

            If Not dicConfig.ContainsKey(arrTmpConfigLine(0)) Then
                dicConfig.Add(arrTmpConfigLine(0), arrTmpConfigLine(1))
            End If

        End While

    End Sub

    Public Function configValue(ByVal strConfigRef As String)

        If dicConfig.ContainsKey(strConfigRef) Then
            Return dicConfig(strConfigRef)
        Else
            Return ""
        End If

    End Function

End Class
