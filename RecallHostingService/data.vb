Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Collections
Imports System

Public Class data
    Implements IDisposable

    Private sqlcon As SqlConnection
    Private sqlcom As SqlCommand
    Private disposed As Boolean

    Public Sub New(ByVal strDatasource As String, ByVal strDatabase As String)
        sqlcon = New SqlConnection

        ''instantiate a connection
        sqlcon.ConnectionString = "Data Source=" & strDatasource & ";Initial Catalog=" & strDatabase & ";Integrated Security=SSPI;"
        sqlcon.Open()

        disposed = False

    End Sub

    Public Function defragDatabase() As Boolean
        ''stop workers

    End Function

    Public Function getNumberOfPendingCommands() As Integer
        Dim reader As SqlDataReader
        Dim intCount As Integer

        ''instantiate a command object
        sqlcom = New SqlCommand
        sqlcom.Connection = sqlcon
        intCount = 0

        sqlcom.CommandText = "sp_getNumberOfQueuedCommands"
        reader = sqlcom.ExecuteReader

        If reader.Read Then

            intCount = CInt(reader("queuedcommands").ToString)
            reader.Close()

        End If

        reader = Nothing
        sqlcom.Dispose()

        Return intCount

    End Function

    Public Function removeDirtyStringData(ByVal strValue As String) As String
        strValue = strValue.Replace("'", "''")
        Return strValue
    End Function

    Public Function removeDirtyNumericData(ByVal strValue As String) As String

        If strValue = "NULL" Then Return strValue
        strValue = Regex.Replace(strValue, "[^\d]", "")
        Return strValue

    End Function

    Public Sub writeErrorToDatabase(ByVal intAccountNumber As String, ByVal strErrorDescription As String, _
                                    ByVal intErrorCode As Integer, ByVal intErrorLine As Integer, _
                                    ByVal strFunctionName As String, ByVal strClassName As String, _
                                    ByVal intCommandID As String)

        ''instantiate a command object
        sqlcom = New SqlCommand
        sqlcom.Connection = sqlcon

        If intAccountNumber = "" Or intAccountNumber = "0" Then intAccountNumber = "NULL"
        If intCommandID = "" Or intCommandID = "0" Then intCommandID = "NULL"
        sqlcom.CommandText = "sp_inserterror '" & removeDirtyStringData(strErrorDescription) & "'," & _
                            removeDirtyNumericData(intCommandID) & "," & removeDirtyNumericData(intAccountNumber) & "," & _
                            removeDirtyNumericData(intErrorCode.ToString) & "," & removeDirtyNumericData(intErrorLine.ToString) & ",'" & _
                            removeDirtyStringData(strFunctionName) & "','" & removeDirtyStringData(strClassName) & "'"


        sqlcom.ExecuteNonQuery()
        sqlcom.Dispose()

    End Sub

    Public Sub writeInformationToLog(ByVal strCommandName As String, ByVal intCommandID As Integer, ByVal strDataDump As String)

        ''instantiate a command object
        sqlcom = New SqlCommand
        sqlcom.Connection = sqlcon

        sqlcom.CommandText = "sp_createlogentry " & removeDirtyNumericData(intCommandID.ToString) & _
                                ",'" & removeDirtyStringData(strCommandName) & "','" & removeDirtyStringData(strDataDump) & "'"

        sqlcom.ExecuteNonQuery()

        sqlcom.Dispose()

    End Sub

    Public Function getNextCommand() As Dictionary(Of String, String)
        ''grab the next command in the queue, and lock it

        Dim reader As SqlDataReader
        Dim dicCommand As Dictionary(Of String, String)
        dicCommand = New Dictionary(Of String, String)

        ''instantiate a command object
        sqlcom = New SqlCommand
        sqlcom.Connection = sqlcon

        sqlcom.CommandText = "sp_getNextCommand"
        reader = sqlcom.ExecuteReader

        If reader.Read Then

            dicCommand.Add("intCommandID", reader.Item("intCommandID"))
            dicCommand.Add("commandName", reader.Item("commandName"))
            dicCommand.Add("accountID", reader.Item("accountID"))
            dicCommand.Add("domainName", reader.Item("domainName"))
            dicCommand.Add("parameter1", reader.Item("parameter1"))
            dicCommand.Add("parameter2", reader.Item("parameter2"))
            dicCommand.Add("parameter3", reader.Item("parameter3"))
            dicCommand.Add("parameter4", reader.Item("parameter4"))
            dicCommand.Add("parameter5", reader.Item("parameter5"))
            dicCommand.Add("parameter6", reader.Item("parameter6"))
            dicCommand.Add("parameter7", reader.Item("parameter7"))
            dicCommand.Add("parameter8", reader.Item("parameter8"))
            dicCommand.Add("parameter9", reader.Item("parameter9"))
            dicCommand.Add("parameter10", reader.Item("parameter10"))
            dicCommand.Add("parameter11", reader.Item("parameter11"))
            dicCommand.Add("parameter12", reader.Item("parameter12"))
            dicCommand.Add("parameter13", reader.Item("parameter13"))
            dicCommand.Add("parameter14", reader.Item("parameter14"))
            dicCommand.Add("parameter15", reader.Item("parameter15"))
            dicCommand.Add("parameter16", reader.Item("parameter16"))
            dicCommand.Add("parameter17", reader.Item("parameter17"))
            dicCommand.Add("parameter18", reader.Item("parameter18"))
            dicCommand.Add("parameter19", reader.Item("parameter19"))
            dicCommand.Add("parameter20", reader.Item("parameter20"))
            reader.Close()

        Else

            dicCommand = Nothing

        End If

        reader = Nothing
        sqlcom.Dispose()

        Return dicCommand

    End Function

    Public Function archiveCommand(ByVal intCommandID As Integer) As Boolean
        ''archive the command after processing

        ''instantiate a command object
        sqlcom = New SqlCommand
        sqlcom.Connection = sqlcon

        sqlcom.CommandText = "sp_archivecommand " & intCommandID.ToString
        If sqlcom.ExecuteNonQuery() > 0 Then
            sqlcom.Dispose()
            Return True
        Else
            sqlcom.Dispose()
            Return False
        End If

    End Function

    Public Function commandFailed(ByVal intCommandID As Integer) As Boolean
        ''update failcount - archive if failed 3 times

        ''instantiate a command object
        sqlcom = New SqlCommand
        sqlcom.Connection = sqlcon

        sqlcom.CommandText = "sp_commandfailed " & intCommandID.ToString
        If sqlcom.ExecuteNonQuery() > 0 Then
            sqlcom.Dispose()
            Return True
        Else
            sqlcom.Dispose()
            Return False
        End If

    End Function

    ' Implement IDisposable.
    Public Overloads Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overridable Overloads Sub Dispose(ByVal disposing As Boolean)
        If disposed = False Then
            If disposing Then
                ' Free other state (managed objects).
                disposed = True
            End If
            ' Free your own state (unmanaged objects).
            'sqlcom.Dispose()
            sqlcon.Close()
            sqlcon.Dispose()

            Dispose()
            MyBase.Finalize()
        End If
    End Sub

    Protected Overrides Sub Finalize()
        ' Simply call Dispose(False).
        Dispose(False)
    End Sub

End Class
