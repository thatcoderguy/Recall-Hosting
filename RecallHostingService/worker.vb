Public Class worker

    ''read database
    ''verify commands
    ''use classes to action
    Private objDB As data
    Private objFunctionDB As data
    Private objConfig As config
    Private objHosting As hosting
    Private objFTP As ftp
    Private objDomain As domainname
    Private objMailbox As mailboxadmin
    Private objSSL As ssl
    Private objDatabase As database
    Private blnQuit As Boolean
    Private blnWorking As Boolean
    'Private objUpdate As update

    ''new account setup e.g. create postoffce

    Public Sub New(ByRef configObject As config)

        ''store the config object (used to instantiate database objects)
        objConfig = configObject

        ''used for the main application to know if it should kill the workers or not
        blnQuit = False
        blnWorking = False

    End Sub

    Public Function isWorking() As Boolean
        Return blnWorking
    End Function

    Public Function doWork() As Boolean
        ''this is the bit that gets called from the thread
        Dim reader As Dictionary(Of String, String)

        ''instantiate a new database object for grabbing commands
        objDB = New data(objConfig.configValue("workhorsedatasource"), objConfig.configValue("workhorsedatabase"))

        ''let the service know this object is doing something when trying to stop
        blnWorking = True

        ''grab the next command from the database
        reader = objDB.getNextCommand()
 
        If reader IsNot Nothing Then

            Dim intAccountID As Integer = CInt(reader.Item("accountID"))
            Dim intCommandID As Integer = CInt(reader.Item("intCommandID"))

            ''PARAMETER NOTES:
            ''  parameter1 - action e.g. add/remove/modify

            Select Case reader.Item("commandName").ToString

                Case "mailbox"

                    Dim strDomainName As String = reader.Item("domainName").ToString

                    ''p1 = admin option/command
                    ''p2 = mailboxname

                    ''instantiate a new mailboxadmin object
                    objMailbox = New mailboxadmin(objConfig, intAccountID, intCommandID, reader.Item("commandName").ToString)

                    Select Case reader.Item("parameter1").ToString

                        Case "create mailbox"

                            ''p3 = mailboxsize
                            ''p4 = password

                            Try

                                ''check that the postoffice exsists - if not, create it
                                If objMailbox.doesPostofficeExist(strDomainName) Is Nothing Then

                                    If Not objMailbox.createPostoffice2(strDomainName) Then

                                        ''command failed
                                        objDB.commandFailed(intCommandID)

                                    End If

                                End If

                                ''try and create the mailbox
                                If objMailbox.createMailbox(strDomainName, reader.Item("parameter2").ToString, CInt(reader.Item("parameter3")), reader.Item("parameter4").ToString) Then

                                    ''mailbox created - archive the command
                                    objDB.archiveCommand(intCommandID)

                                Else

                                    ''command failed
                                    objDB.commandFailed(intCommandID)

                                End If


                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "createMailbox||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "create postoffice"

                            Try

                                ''check that the postoffice exsists - if not, create it
                                If objMailbox.doesPostofficeExist(strDomainName) Is Nothing Then

                                    If objMailbox.createPostoffice2(strDomainName) Then

                                        ''postoffice created... now add the domain
                                        ''domain added ok -archive command
                                        objDB.archiveCommand(intCommandID)

                                    Else

                                        ''command failed
                                        objDB.commandFailed(intCommandID)

                                    End If

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "createPostOffice||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try


                        Case "delete mailbox"

                            Try

                                ''try to delete mailbox
                                If objMailbox.deleteMailbox(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3").ToString) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "deleteMailbox||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "change mailbox password"

                            ''p3 = new password

                            Try

                                ''try to change mailbox password
                                If objMailbox.changeMailboxPassword(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3").ToString) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "changeMailboxPassword||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "change mailbox size"

                            ''p3 = new size

                            Try

                                ''try to change mailbox password
                                If objMailbox.changeMailboxSize(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3")) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "changeMailboxSize||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "change mailbox name"

                            ''p3 = new mailboxname

                            Try

                                ''try to change mailbox password
                                If objMailbox.changeMailboxName(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3").ToString) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "changeMailboxName||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "add redirector"

                            ''p3 = redirection email address
                            ''p4 = redirection status

                            Try

                                Dim blnKeepCopy As Boolean
                                blnKeepCopy = CBool(reader.Item("parameter4"))


                                If objMailbox.addMailboxRedirection(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3").ToString, blnKeepCopy) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If


                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "addMailboxRedirection||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "delete redirector"

                            ''p3 = redirection email address
                            ''p4 = redirection status
                            Try

                                Dim blnKeepCopy As Boolean
                                blnKeepCopy = CBool(reader.Item("parameter4"))

                                If objMailbox.removeMailboxRedirection(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3").ToString, blnKeepCopy) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "removeMailboxRedirection||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "add forwarder"

                            ''p3 = forwarder mailbox

                            Try

                                If objMailbox.addMailboxForwarder(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3").ToString) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "addMailboxForwarder||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "delete forwarder"

                            ''p3 = forwarder mailbox

                            Try

                                If objMailbox.removeMailboxForwarder(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3").ToString) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "removeMailboxForwarder||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "disable postoffice"

                            Try

                                If objMailbox.disablePostoffice(strDomainName) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If


                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "disablePostOffice||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "add pop connector"

                            ''p3 = username
                            ''p4 = password
                            ''p5 = server address
                            ''p6 = leave copy
                            ''p7 = pop port
                            ''p8 = use APOP

                            Try

                                If objMailbox.addPOPConnector(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3").ToString, reader.Item("parameter4").ToString, reader.Item("parameter5").ToString, CBool(reader.Item("parameter6")), CInt(reader.Item("parameter7").ToString), CBool(reader.Item("parameter8").ToString)) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "addPOPConnector||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "modify pop connector"

                            ''p3 = username
                            ''p4 = password
                            ''p5 = server address
                            ''p6 = leave copy
                            ''p7 = pop port
                            ''p8 = use APOP

                            ''p9 = new username
                            ''p10 = password
                            ''p11 = server address
                            ''p12 = leave copy
                            ''p13 = pop port
                            ''p14 = use APOP

                            Try

                                If objMailbox.updatePOPConnector(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3").ToString, reader.Item("parameter4").ToString, reader.Item("parameter5").ToString, CBool(reader.Item("parameter6")), CInt(reader.Item("parameter7").ToString), CBool(reader.Item("parameter8").ToString), reader.Item("parameter9").ToString, reader.Item("parameter10").ToString, reader.Item("parameter11").ToString, CBool(reader.Item("parameter12")), CInt(reader.Item("parameter13").ToString), CBool(reader.Item("parameter14").ToString)) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "updatePOPConnector||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "remove pop connector"

                            ''p3 = username
                            ''p4 = password
                            ''p5 = server address
                            ''p6 = leave copy
                            ''p7 = pop port
                            ''p8 = use APOP

                            Try

                                If objMailbox.removePOPConnector(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3").ToString, reader.Item("parameter4").ToString, reader.Item("parameter5").ToString, CBool(reader.Item("parameter6")), CInt(reader.Item("parameter7").ToString), CBool(reader.Item("parameter8").ToString)) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "removePOPConnector||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "add autoresponder"

                            ''p3 = subject
                            ''p4 = body text

                            Try
                                If objMailbox.manageAutoResponder(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3").ToString, reader.Item("parameter4").ToString, 1) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "manageAutoResponder||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "delete autoresponder"

                            ''p3 = subject
                            ''p4 = body text

                            Try
                                If objMailbox.manageAutoResponder(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3").ToString, reader.Item("parameter4").ToString, 0) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "manageAutoResponder||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "add webmail"

                            Try
                                If objMailbox.addWebMail(strDomainName) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "addWebMail||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "delete webmail"

                            Try
                                If objMailbox.removeWebMail(strDomainName) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "removeWebMail||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "add alias domain"

                            ''p3 = alias domain

                            Try
                                If objMailbox.addAliasDomain(strDomainName, reader.Item("parameter3").ToString) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "addAliasDomain||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "remove alias domain"

                            ''p3 = alias domain

                            Try
                                If objMailbox.removeAliasDomain(strDomainName, reader.Item("parameter3").ToString) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "removeAliasDomain||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "add catchall"


                        Case "remove catchall"

                        Case "add alias address"

                            ''p3 = alias address

                            Try
                                If objMailbox.addAliasAddress(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3").ToString) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "addAliasAddress||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "remove alias address"

                            ''p3 = alias address

                            Try
                                If objMailbox.removeAliasAddress(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3").ToString) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "removeAliasAddress||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "clean mailbox"

                            ''p3 = folder name
                            ''p4 = min file ages

                            Try
                                If objMailbox.purgeMailbox(strDomainName, reader.Item("parameter2").ToString, reader.Item("parameter3").ToString, CInt(reader.Item("parameter4").ToString)) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "purgeMailbox||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                        Case "mailbox usage"

                            Try
                                If objMailbox.mailboxUsage(strDomainName, reader.Item("parameter2").ToString) Then

                                    objDB.archiveCommand(intCommandID)

                                Else

                                    objDB.commandFailed(intCommandID)

                                End If

                            Catch ex As Exception

                                Dim objLog As Diagnostics.EventLog
                                objLog = New Diagnostics.EventLog
                                objLog.Source = "RecallHostingService"
                                EventLog.WriteEntry("RecallHostingService_log", "mailboxUsage||" & Err.Description, EventLogEntryType.Information)

                                objDB.commandFailed(intCommandID)

                            End Try

                    End Select

                    ''dispose first, to kill all db connections
                    objMailbox.Dispose()

                    ''destroy the mailbox admin object, to clean up.
                    objMailbox = Nothing

                Case "domain"

                    objDomain = New domainname

                    objDomain = Nothing

                Case "hosting"

                    objHosting = New hosting

                    objHosting = Nothing

                Case "ftp"

                    objFTP = New ftp

                    objFTP = Nothing

                Case "ssl"

                    objSSL = New ssl

                    objSSL = Nothing

                Case "database"

                    objDatabase = New database

                    objDatabase = Nothing

            End Select

        End If

        ''kills the connections and command object
        objDB.Dispose()

        blnWorking = False

    End Function

    Public Function fireWorker() As Boolean

        blnQuit = True

    End Function


    Public Function iQuit() As Boolean

        Return blnQuit

    End Function


End Class
