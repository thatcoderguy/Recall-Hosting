Imports MailEnable.Administration
Imports System.IO
Imports Microsoft.Web.Administration

Public Class mailboxadmin
    Implements IDisposable

    Private objDB As data
    Private commandID As Integer
    Private accountID As Integer
    Private commandName As String
    Private disposed As Boolean
    Private strMailEnableLocation As String
    Private strIISIPAddress As String

    Public Sub New(ByRef objConfig As config, ByVal intAccountID As Integer, ByVal intCommandID As Integer, ByVal strCommandName As String)
        ''use the copy of the database layer class from the worker thread
        ''this is used for logging and error capture

        objDB = New data(objConfig.configValue("workhorsedatasource"), objConfig.configValue("workhorsedatabase"))
        strMailEnableLocation = objConfig.configValue("mailenablelocation")
        strIISIPAddress = objConfig.configValue("iisipaddress")

        'this data is used when logging
        commandID = intCommandID
        accountID = intAccountID
        commandName = strCommandName
        disposed = False

    End Sub

    Public Function createPostoffice2(ByVal strDomain As String) As Boolean
        ''yeah.... the original method didnt work
        ''I suspect it was too modularised, so this version will create everything
        ''but this means the modularisation can still be kept (because its useful)

        Try

            Dim objPO As Postoffice

            objDB.writeInformationToLog("Attempting to create Postoffice", commandID, "Domain = " & strDomain)

            ''check if the postoffice already exists
            If doesPostofficeExist(strDomain) Is Nothing Then

                ''setup the postoffice details
                objPO = New Postoffice
                objPO.Account = strDomain
                objPO.Status = 1
                objPO.Name = strDomain

                If objPO.AddPostoffice() = 1 Then

                    objDB.writeInformationToLog("Postoffice added OK, attempting to add Postmaster", commandID, "Domain = " & strDomain)

                    Dim objMailbox As Mailbox
                    objMailbox = New Mailbox

                    objMailbox.Postoffice = strDomain
                    objMailbox.MailboxName = "Postmaster"
                    objMailbox.Limit = 51200 ''50MB
                    objMailbox.RedirectAddress = ""
                    objMailbox.RedirectStatus = 0
                    objMailbox.Status = 1

                    If objMailbox.AddMailbox() = 1 Then

                        objDB.writeInformationToLog("Postmaster mailbox added, attempting to create login", commandID, "Domain = " & strDomain)

                        Dim objLogin As Login
                        objLogin = New Login

                        objLogin.Account = strDomain
                        objLogin.Description = "Postmaster Mailbox"
                        objLogin.Password = "DnxMZPuTzdZOIMOZY8z" ''so people cant guess the postmaster password
                        objLogin.Rights = "USER"
                        objLogin.Status = 1
                        objLogin.UserName = "Postmaster@" & strDomain

                        If objLogin.AddLogin() = 1 Then

                            objDB.writeInformationToLog("Postmaster login added, attempting to create domain", commandID, "Domain = " & strDomain)

                            Dim objDomain As Domain
                            objDomain = New Domain

                            objDomain.AccountName = strDomain
                            objDomain.DomainName = strDomain
                            objDomain.Status = 1
                            objDomain.DomainRedirectionHosts = ""
                            objDomain.DomainRedirectionStatus = 0

                            If objDomain.AddDomain() = 1 Then

                                objDB.writeInformationToLog("Domain created, attempting to create address maps", commandID, "Domain = " & strDomain)

                                Dim objAddressMap As AddressMap
                                objAddressMap = New AddressMap

                                objAddressMap.Account = strDomain
                                objAddressMap.DestinationAddress = "[SF:" & strDomain & "/Postmaster]"
                                objAddressMap.SourceAddress = "[SMTP:Postmaster@" & strDomain & "]"
                                objAddressMap.Scope = "0"

                                If Not objAddressMap.AddAddressMap() = 1 Then

                                    ''log error
                                    objDB.writeInformationToLog("Could not create postmaster address map", commandID, "Domain = " & strDomain)
                                    objDB.writeErrorToDatabase(accountID, "Could not create postmaster address map", commandID, 0, "createpostoffice", "mailbox", commandID)
                                    Return False

                                End If

                                objDB.writeInformationToLog("Postmaster address map created", commandID, "Domain = " & strDomain)

                                objAddressMap = New AddressMap

                                objAddressMap.Account = strDomain
                                objAddressMap.DestinationAddress = "[SF:" & strDomain & "/Postmaster]"
                                objAddressMap.SourceAddress = "[SMTP:Abuse@" & strDomain & "]"
                                objAddressMap.Scope = "0"

                                If Not objAddressMap.AddAddressMap() = 1 Then

                                    ''log error
                                    objDB.writeInformationToLog("Could not create abuse address map", commandID, "Domain = " & strDomain)
                                    objDB.writeErrorToDatabase(accountID, "Could not create abuse address map", 0, 0, "createpostoffice", "mailbox", commandID)
                                    Return False

                                End If

                                objDB.writeInformationToLog("Abuse address map created", commandID, "Domain = " & strDomain)

                                objAddressMap = New AddressMap

                                objAddressMap.Account = strDomain
                                objAddressMap.DestinationAddress = "[SF:" & strDomain & "/Postmaster]"
                                objAddressMap.SourceAddress = "[SMTP:*@" & strDomain & "]"
                                objAddressMap.Scope = "0"

                                If Not objAddressMap.AddAddressMap() = 1 Then

                                    ''log error
                                    objDB.writeInformationToLog("Could not create catchall address map", commandID, "Domain = " & strDomain)
                                    objDB.writeErrorToDatabase(accountID, "Could not create catchall address map", 0, 0, "createpostoffice", "mailbox", commandID)
                                    Return False

                                End If

                                objDB.writeInformationToLog("Catchall address map created", commandID, "Domain = " & strDomain)

                                Return True

                            Else

                                ''log error
                                objDB.writeInformationToLog("Could not create domain", commandID, "Domain = " & strDomain)
                                objDB.writeErrorToDatabase(accountID, "Could not create domain", 0, 0, "createpostoffice", "mailbox", commandID)
                                Return False

                            End If

                        Else

                            ''log error
                            objDB.writeInformationToLog("Could not create Postmaster login", commandID, "Domain = " & strDomain)
                            objDB.writeErrorToDatabase(accountID, "Could not create Postmaster login", 0, 0, "createpostoffice", "mailbox", commandID)
                            Return False

                        End If

                    Else

                        ''log error
                        objDB.writeInformationToLog("Could not create Postmaster mailbox", commandID, "Domain = " & strDomain)
                        objDB.writeErrorToDatabase(accountID, "Could not create Postmaster mailbox", 0, 0, "createpostoffice", "mailbox", commandID)
                        Return False

                    End If


                Else

                    ''log error
                    objDB.writeInformationToLog("Could not create Postoffice", commandID, "Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not create Postoffice", 0, 0, "createpostoffice", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice already exists", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Postoffice already exists", 0, 0, "createpostoffice", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Probelm in createPostOffice2||" & Err.Description, EventLogEntryType.Information)

        End Try

        Return False

    End Function

    Public Function createPostoffice(ByVal strDomain As String) As Boolean
        Dim objPO As Postoffice

        ''check if the postoffice already exists
        If doesPostofficeExist(strDomain) Is Nothing Then

            ''setup the postoffice details
            objPO = New Postoffice
            objPO.Account = strDomain
            objPO.Status = 1
            objPO.Name = strDomain

            ''create the postoffice
            If objPO.AddPostoffice() = 1 Then

                objDB.writeInformationToLog("Postoffice Created", commandID, "Domain created = " & strDomain)
                Return True

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice Creation Failed", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Postoffice Creation Failed", 0, 0, "createpostoffice", "mailbox", commandID)
                Return False

            End If

        Else

            ''log error
            objDB.writeInformationToLog("Postoffice already exists", commandID, "Domain = " & strDomain)
            Return True
            'objDB.writeErrorToDatabase(accountID, "ostoffice already exists", 0, 0, "createpostoffice", "mailbox", commandID)

        End If

    End Function

    Public Function doesPostofficeExist(ByVal strDomain As String) As Postoffice
        ''modulerized to make logging and error capture easier

        Try

            Dim objPostoffice As Postoffice

            objPostoffice = New Postoffice
            objPostoffice.Name = strDomain
            objPostoffice.Account = strDomain

            objDB.writeInformationToLog("Checking to see if Postoffice exists", commandID, "Domain checked = " & strDomain)

            ''check if office exists
            If objPostoffice.GetPostoffice() = 1 Then

                objDB.writeInformationToLog("Postoffice does exist", commandID, "Domain checked = " & strDomain)
                Return objPostoffice

            Else

                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain checked = " & strDomain)
                Return Nothing

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in doesPostOfficeExist||" & Err.Description, EventLogEntryType.Information)

        End Try

        Return Nothing

    End Function

    Public Function doesDomainExist(ByVal strDomain As String) As Domain
        ''modulerized to make logging and error capture easier

        Try

            Dim objDomain As Domain

            objDomain = New Domain
            objDomain.AccountName = strDomain
            objDomain.DomainName = strDomain
            objDomain.Status = -1
            objDomain.DomainRedirectionHosts = ""
            objDomain.DomainRedirectionStatus = -1

            objDB.writeInformationToLog("Checking to see if Domain exists", commandID, "Domain checked = " & strDomain)

            ''check if office exists
            If objDomain.GetDomain() = 1 Then

                objDB.writeInformationToLog("Domain does exist", commandID, "Domain checked = " & strDomain)
                Return objDomain

            Else

                objDB.writeInformationToLog("Domain does NOT exist", commandID, "Domain checked = " & strDomain)
                Return Nothing

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in doesDomainExist||" & Err.Description, EventLogEntryType.Information)

        End Try

        Return Nothing

    End Function

    Public Function doesMailboxExist(ByVal strDomain As String, ByVal strMailboxName As String) As Mailbox

        Try

            Dim objMailbox As New Mailbox

            ''the easiest way to get the mailbox name
            strMailboxName = Split(strMailboxName, "@")(0)

            ''setup mailbox details
            objMailbox.Postoffice = strDomain
            objMailbox.MailboxName = strMailboxName
            objMailbox.Size = -4

            objDB.writeInformationToLog("Checking to see if Mailbox exists", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

            ''check if mailbox exists
            If objMailbox.GetMailbox() = 1 Then

                objDB.writeInformationToLog("Mailbox does exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                Return objMailbox

            Else

                objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                Return Nothing

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in doesMailboxExist||" & Err.Description, EventLogEntryType.Information)

        End Try

        Return Nothing

    End Function

    Public Function doesLoginExist(ByVal strDomain As String, ByVal strMailboxName As String) As Login

        Try

            Dim objLogin As Login
            objLogin = New Login

            ''the easiest way to get the mailbox name
            strMailboxName = Split(strMailboxName, "@")(0)

            objLogin.Account = strDomain
            objLogin.UserName = strMailboxName & "@" & strDomain
            objLogin.Description = ""
            objLogin.Password = ""
            objLogin.Rights = ""
            objLogin.Status = 1

            objDB.writeInformationToLog("Checking to see if login exists", commandID, "Username = " & strMailboxName & "@" & strDomain & "||Domain = " & strDomain)

            ''check if mailbox exists
            If objLogin.GetLogin() = 1 Then

                objDB.writeInformationToLog("Login does exist", commandID, "Username = " & strMailboxName & "@" & strDomain & "||Domain = " & strDomain)
                Return objLogin

            Else

                objDB.writeInformationToLog("Login does NOT exist", commandID, "Username = " & strMailboxName & "@" & strDomain & "||Domain = " & strDomain)
                Return Nothing

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in doesLoginExist||" & Err.Description, EventLogEntryType.Information)

        End Try

        Return Nothing

    End Function

    Public Function disablePostoffice(ByVal strDomain As String) As Boolean

        Try

            ''this is used if some asshole doesnt pay his godamn bills!
            Dim objPostoffice As Postoffice

            ''retreive postoffice
            objPostoffice = doesPostofficeExist(strDomain)

            If Not objPostoffice Is Nothing Then

                ''disable the postoffice
                objPostoffice.EditPostoffice(strDomain, 0, objPostoffice.Account)
                objDB.writeInformationToLog("Postoffice Disabled", commandID, "Domain checked = " & strDomain)
                Return True

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Postoffice doesn NOT exist", 0, 0, "disablepostoffice", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in disablePostoffice||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function addDomainToPostOffice(ByVal strDomain As String) As Boolean

        Try

            Dim objDomain As Domain

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog()

            objDB.writeInformationToLog("Attempting to add Domain", commandID, "Domain = " & strDomain)

            ''try and grab the postoffice
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                ''try and grab the domain
                objDomain = doesDomainExist(strDomain)

                If objDomain Is Nothing Then

                    ''setup the domain
                    objDomain = New Domain
                    objDomain.DomainName = strDomain
                    objDomain.Status = 1
                    objDomain.DomainRedirectionHosts = ""
                    objDomain.DomainRedirectionStatus = 0

                    objDB.writeInformationToLog("Checking to see if domain exists", commandID, "Domain = " & strDomain)

                    ''try and add the domain
                    If objDomain.AddDomain() = 1 Then

                        ''now that the domain has been added, we have to add postmaster, catchall and abuse addresses

                        objDB.writeInformationToLog("Domain added, attempting to add address map", commandID, "Domain = " & strDomain)

                        Dim objAddressMap As AddressMap

                        'add postmaster
                        objAddressMap = New AddressMap
                        objAddressMap.Account = strDomain
                        objAddressMap.DestinationAddress = "[SF:" & strDomain & "/postmaster]"
                        objAddressMap.SourceAddress = "[SMTP:postmaster@" & strDomain & "]"
                        objAddressMap.Scope = "0"

                        If objAddressMap.AddAddressMap() = 1 Then
                            objDB.writeInformationToLog("Postmaster address added", commandID, "Domain = " & strDomain)
                        Else
                            objDB.writeInformationToLog("Could not add postmaster address", commandID, "Domain = " & strDomain)
                        End If

                        objAddressMap = Nothing

                        'add abuse
                        objAddressMap = New AddressMap
                        objAddressMap.Account = strDomain
                        objAddressMap.DestinationAddress = "[SF:" & strDomain & "/abuse]"
                        objAddressMap.SourceAddress = "[SMTP:abuse@" & strDomain & "]"
                        objAddressMap.Scope = "0"

                        If objAddressMap.AddAddressMap() = 1 Then
                            objDB.writeInformationToLog("Abuse address added", commandID, "Domain = " & strDomain)
                        Else
                            objDB.writeInformationToLog("Could not add abuse address", commandID, "Domain = " & strDomain)
                        End If

                        objAddressMap = Nothing

                        'add catchall
                        objAddressMap = New AddressMap
                        objAddressMap.Account = strDomain
                        objAddressMap.DestinationAddress = "[SF:" & strDomain & "/catchall]"
                        objAddressMap.SourceAddress = "[SMTP:*@" & strDomain & "]"
                        objAddressMap.Scope = "0"

                        If objAddressMap.AddAddressMap() = 1 Then
                            objDB.writeInformationToLog("Catchall address added", commandID, "Domain = " & strDomain)
                        Else
                            objDB.writeInformationToLog("Could not add catchall address", commandID, "Domain = " & strDomain)
                        End If

                        objAddressMap = Nothing

                        objDB.writeInformationToLog("Domain created", commandID, "Domain = " & strDomain)
                        Return True
                    Else
                        ''log error
                        objDB.writeInformationToLog("Domain was not created", commandID, "Domain = " & strDomain)
                        objDB.writeErrorToDatabase(accountID, "Could not create domain", 0, 0, "addDomainToPostOffice", "mailbox", commandID)
                        Return False
                    End If

                Else
                    ''log error
                    objDB.writeInformationToLog("Domain already exists", commandID, "Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "domain already exists", 0, 0, "addDomainToPostOffice", "mailbox", commandID)
                    Return False
                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Postoffice doesnt exist", 0, 0, "addDomainToPostOffice", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in addDomainToPostoffice||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function createMailbox(ByVal strDomain As String, ByVal strMailboxName As String, ByVal intMailBoxSize As Integer, ByVal strPassword As String) As Boolean

        Try

            Dim objMailbox As Mailbox
            Dim objDomain As Domain

            ''the easiest way to get the mailbox name
            strMailboxName = Split(strMailboxName, "@")(0)

            'if the postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                objMailbox = doesMailboxExist(strDomain, strMailboxName)

                'does the mailbox already exist?
                If Not objMailbox Is Nothing Then

                    ''log error
                    objDB.writeInformationToLog("Mailbox already exists", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not create Mailbox - Mailbox already exists", 0, 0, "createmailbox", "mailbox", commandID)
                    Return False

                Else

                    objMailbox = New Mailbox

                    ''setup mailbox details
                    objMailbox.Postoffice = strDomain
                    objMailbox.MailboxName = strMailboxName
                    objMailbox.Limit = intMailBoxSize
                    objMailbox.Status = 1
                    objMailbox.Size = 0
                    objMailbox.RedirectStatus = 0
                    objMailbox.RedirectAddress = ""

                    If objMailbox.AddMailbox() = 1 Then

                        objDB.writeInformationToLog("Mailbox created", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        ''the mailbox was added, now we need a login and an address map

                        Dim objLogin As Login
                        objLogin = New Login

                        objLogin.Account = strDomain
                        objLogin.UserName = strMailboxName & "@" & strDomain
                        objLogin.LastAttempt = -1
                        objLogin.LoginAttempts = -1
                        objLogin.LastSuccessfulLogin = -1
                        objLogin.Password = ""
                        objLogin.Rights = ""
                        objLogin.Status = -1

                        ''does the login already exist
                        If objLogin.GetLogin() = 1 Then

                            ''log error
                            objDB.writeInformationToLog("Login was not created", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                            objDB.writeErrorToDatabase(accountID, "Login already exists", 0, 0, "createmailbox", "mailbox", commandID)
                            Return False

                        Else

                            ''setup login details
                            objLogin.Account = strDomain
                            objLogin.UserName = strMailboxName & "@" & strDomain
                            objLogin.LastAttempt = 0
                            objLogin.LoginAttempts = 0
                            objLogin.LastSuccessfulLogin = 0
                            objLogin.Password = strPassword
                            objLogin.Rights = "USER"
                            objLogin.Status = 1

                            If objLogin.AddLogin() = 1 Then

                                objDB.writeInformationToLog("Login created", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

                                If addAddressMap(strDomain, strMailboxName, strMailboxName) Then

                                    objDB.writeInformationToLog("Address map created for primary domain", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

                                    objDomain = New Domain
                                    objDomain.AccountName = strDomain   ''postoffice name
                                    objDomain.DomainName = ""
                                    objDomain.Status = -1
                                    objDomain.DomainRedirectionHosts = ""
                                    objDomain.DomainRedirectionStatus = -1

                                    If objDomain.FindFirstDomain = 1 Then

                                        objDB.writeInformationToLog("Attempting to create address maps for all domains", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

                                        'reset to find the next domain
                                        objDomain.AccountName = strDomain   ''postoffice name
                                        objDomain.DomainName = ""
                                        objDomain.Status = -1
                                        objDomain.DomainRedirectionHosts = ""
                                        objDomain.DomainRedirectionStatus = -1

                                        Do While objDomain.FindNextDomain = 1

                                            ''login now created, so we need to create the address map
                                            If addExternalAddressMap(strDomain, strMailboxName, strMailboxName & "@" & objDomain.DomainName, False) Then

                                                objDB.writeInformationToLog("address map created", commandID, "Mailbox = " & strMailboxName & "||Domain = " & objDomain.DomainName)

                                            Else

                                                ''log error
                                                objDB.writeInformationToLog("Could not create address map", commandID, "Mailbox = " & strMailboxName & "||Domain = " & objDomain.DomainName)
                                                objDB.writeErrorToDatabase(accountID, "Could not create address map", 0, 0, "createmailbox", "mailbox", commandID)

                                            End If

                                            'reset to find the next domain
                                            objDomain.AccountName = strDomain   ''postoffice name
                                            objDomain.DomainName = ""
                                            objDomain.Status = -1
                                            objDomain.DomainRedirectionHosts = ""
                                            objDomain.DomainRedirectionStatus = -1

                                        Loop

                                        Return True

                                    Else

                                        ''log error
                                        objDB.writeInformationToLog("Could not create primary domain address map", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                                        objDB.writeErrorToDatabase(accountID, "Could not create primary domain address map", 0, 0, "createmailbox", "mailbox", commandID)
                                        Return False

                                    End If

                                Else

                                    ''log error
                                    objDB.writeInformationToLog("Could not find any domains", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                                    objDB.writeErrorToDatabase(accountID, "Could not find any domains", 0, 0, "createmailbox", "mailbox", commandID)
                                    Return False

                                End If

                            Else

                            ''log error
                            objDB.writeInformationToLog("Could not create login", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                            objDB.writeErrorToDatabase(accountID, "Could not create mailnox login", 0, 0, "createmailbox", "mailbox", commandID)
                            Return False

                        End If

                        End If

                    Else

                        ''log error
                        objDB.writeInformationToLog("Mailbox was not created", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        objDB.writeErrorToDatabase(accountID, "Could not create Mailbox", 0, 0, "createmailbox", "mailbox", commandID)
                        Return False

                    End If

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not create Mailbox - Postoffice doesnt exist", 0, 0, "createmailbox", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in createMailbox||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function changeMailboxPassword(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strNewPassword As String) As Boolean

        Try

            Dim objLogin As Login

            objDB.writeInformationToLog("Attempting to change mailbox password", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

            ''check that postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                ''check that mailbox exists
                If Not doesMailboxExist(strDomain, strMailboxName) Is Nothing Then

                    objLogin = doesLoginExist(strDomain, strMailboxName)

                    ''check that login does exist
                    If Not objLogin Is Nothing Then

                        ''attempt to change password
                        If objLogin.EditLogin(objLogin.UserName, objLogin.Status, strNewPassword, objLogin.Account, objLogin.Description, objLogin.LoginAttempts, objLogin.LastAttempt, objLogin.LastSuccessfulLogin, objLogin.Rights) = 1 Then

                            ''yay, it worked
                            objDB.writeInformationToLog("Mailbox password changed", commandID, "Username = " & strMailboxName & "@" & strDomain & "||Domain = " & strDomain)
                            Return True

                        Else

                            ''log error
                            objDB.writeInformationToLog("Could not change mailbox password", commandID, "Username = " & strMailboxName & "@" & strDomain & "||Domain = " & strDomain)
                            objDB.writeErrorToDatabase(accountID, "Could not change mailbox password", 0, 0, "changemailboxpassword", "mailbox", commandID)
                            Return False

                        End If

                    Else

                        ''log error
                        objDB.writeInformationToLog("Login does NOT exist", commandID, "Username = " & strMailboxName & "@" & strDomain & "||Domain = " & strDomain)
                        objDB.writeErrorToDatabase(accountID, "Could not change Mailbox password - Login does NOT exist", 0, 0, "changemailboxpassword", "mailbox", commandID)
                        Return False

                    End If

                Else

                    ''log error
                    objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not change Mailbox password - Mailbox does NOT exist", 0, 0, "changemailboxpassword", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not change Mailbox password - Postoffice doesnt exist", 0, 0, "changemailboxpassword", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in changeMailboxPassword||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function changeMailboxSize(ByVal strDomain As String, ByVal strMailboxName As String, ByVal intNewMailBoxSize As Integer) As Boolean

        Try

            Dim objMailbox As Mailbox

            objDB.writeInformationToLog("Attempting to change mailbox size", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

            ''check that postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                objMailbox = doesMailboxExist(strDomain, strMailboxName)

                ''check that mailbox exists
                If Not objMailbox Is Nothing Then

                    ''attempt to change the mailbox size
                    If objMailbox.EditMailbox(objMailbox.Postoffice, objMailbox.MailboxName, objMailbox.RedirectAddress, objMailbox.RedirectStatus, objMailbox.Status, intNewMailBoxSize, objMailbox.Size) = 1 Then

                        ''yay, it worked
                        objDB.writeInformationToLog("Mailbox size updated", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        Return True

                    Else

                        ''log error
                        objDB.writeInformationToLog("Could not change mailbox size", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        objDB.writeErrorToDatabase(accountID, "Could not change mailbox size", 0, 0, "changeMailboxSize", "mailbox", commandID)
                        Return False

                    End If

                Else

                    ''log error
                    objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not change Mailbox size - Mailbox does NOT exist", 0, 0, "changeMailboxSize", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not change Mailbox size - Postoffice doesnt exist", 0, 0, "changeMailboxSize", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in changeMailboxSize||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function deleteMailbox(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strForwardName As String) As Boolean

        Try

            Dim objMailbox As Mailbox

            objDB.writeInformationToLog("Attempting to delete mailbox", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

            ''check that postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                objMailbox = doesMailboxExist(strDomain, strMailboxName)

                ''check that mailbox exists
                If Not objMailbox Is Nothing Then

                    ''attempt to remove mailbox
                    If objMailbox.RemoveMailbox() = 1 Then

                        objDB.writeInformationToLog("Mailbox deleted, now attempting to remove address map", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

                        ''the easiest way to get the mailbox name
                        strMailboxName = Split(strMailboxName, "@")(0)

                        If removeAddressMap(strDomain, strMailboxName, strForwardName, True) Then

                            ''yay, it worked!
                            objDB.writeInformationToLog("Address map deleted", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                            Return True

                        Else

                            ''log error
                            objDB.writeInformationToLog("Could not delete address map", commandID, "Mailbox = " & strMailboxName & "@" & strDomain & "||Domain = " & strDomain)
                            objDB.writeErrorToDatabase(accountID, "Could not delete address map", 0, 0, "deleteMailbox", "mailbox", commandID)
                            Return False

                        End If

                    Else

                        ''log error
                        objDB.writeInformationToLog("Could not delete mailbox", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        objDB.writeErrorToDatabase(accountID, "Could not delete Mailbox", 0, 0, "deleteMailbox", "mailbox", commandID)
                        Return False

                    End If

                Else

                    ''log error
                    objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not delete Mailbox - Mailbox does NOT exist", 0, 0, "deleteMailbox", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not delete Mailbox - Postoffice doesnt exist", 0, 0, "deleteMailbox", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in deleteMailbox||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function changeMailboxName(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strNewMailboxName As String) As Boolean

        Try

            Dim objMailbox As Mailbox
            Dim objLogin As Login
            Dim objAddressMap As AddressMap

            objDB.writeInformationToLog("Attempting to change mailbox name", commandID, "Mailbox = " & strMailboxName & "||New Name=" & strNewMailboxName & "||Domain = " & strDomain)

            ''check that postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                objMailbox = doesMailboxExist(strDomain, strMailboxName)

                ''check that mailbox exists
                If Not objMailbox Is Nothing Then

                    strNewMailboxName = Split(strNewMailboxName, "@")(0)

                    ''attempt to rename mailbox
                    If objMailbox.EditMailbox(objMailbox.Postoffice, strNewMailboxName, objMailbox.RedirectAddress, objMailbox.RedirectStatus, objMailbox.Status, objMailbox.Limit, objMailbox.Status) = 1 Then

                        objAddressMap = New AddressMap
                        objAddressMap.Account = strDomain
                        objAddressMap.DestinationAddress = "" '"[SF:" & strDomain & "/" & strMailboxName & "]"
                        objAddressMap.SourceAddress = "[SMTP:" & strNewMailboxName & "@" & strDomain & "]"
                        objAddressMap.Scope = ""

                        ''start iterating through and deleteing them all
                        If objAddressMap.FindFirstAddressMap() = 1 Then

                            Do
                                ''attempt to remove address map
                                If objAddressMap.EditAddressMap(objAddressMap.Account, objAddressMap.SourceAddress, "[SF:" & strDomain & "/" & strNewMailboxName & "]", objAddressMap.Scope, objAddressMap.SourceAddress) = 1 Then

                                    ''yay, it worked!
                                    objDB.writeInformationToLog("Address map deleted", commandID, "Source=" & objAddressMap.SourceAddress & "||Destination = " & strMailboxName & "@" & strDomain & "||Domain = " & strDomain)

                                Else

                                    ''log error
                                    objDB.writeInformationToLog("Could not delete address map", commandID, "Source=" & objAddressMap.SourceAddress & "||Destination = " & strMailboxName & "@" & strDomain & "||Domain = " & strDomain)
                                    objDB.writeErrorToDatabase(accountID, "Could not delete address map", 0, 0, "changeMailboxName", "mailbox", commandID)
                                    Return False

                                End If

                                ''we want to remove address map(s) for this mailbox
                                objAddressMap.Account = strDomain
                                objAddressMap.DestinationAddress = ""
                                objAddressMap.SourceAddress = "[SMTP:" & strNewMailboxName & "@" & strDomain & "]"
                                objAddressMap.Scope = ""

                            Loop While objAddressMap.FindNextAddressMap() = 1

                            objDB.writeInformationToLog("All address maps updated", commandID, "Mailbox = " & strMailboxName & "@" & strDomain & "||Domain = " & strDomain)

                            ''this should only return true if all the address maps have been removed
                            Return True

                        End If

                        objLogin = doesLoginExist(strDomain, strMailboxName)

                        ''check that login does exist
                        If Not objLogin Is Nothing Then

                            ''attempt to change password
                            If objLogin.EditLogin(strNewMailboxName, objLogin.Status, objLogin.Password, objLogin.Account, objLogin.Description, objLogin.LoginAttempts, objLogin.LastAttempt, objLogin.LastSuccessfulLogin, objLogin.Rights) = 1 Then

                                ''yay, it worked
                                objDB.writeInformationToLog("Mailbox password changed", commandID, "Username = " & strMailboxName & "@" & strDomain & "||Domain = " & strDomain)
                                Return True

                            Else

                                ''log error
                                objDB.writeInformationToLog("Could not change mailbox password", commandID, "Username = " & strMailboxName & "@" & strDomain & "||Domain = " & strDomain)
                                objDB.writeErrorToDatabase(accountID, "Could not change mailbox password", 0, 0, "changemailboxpassword", "mailbox", commandID)
                                Return False

                            End If

                        Else

                            ''log error
                            objDB.writeInformationToLog("Login does NOT exist", commandID, "Username = " & strMailboxName & "@" & strDomain & "||Domain = " & strDomain)
                            objDB.writeErrorToDatabase(accountID, "Could not change Mailbox password - Login does NOT exist", 0, 0, "changemailboxpassword", "mailbox", commandID)
                            Return False

                        End If

                        objDB.writeInformationToLog("Mailbox update, now attempting to update address map", commandID, "Mailbox = " & strMailboxName & "||New Name=" & strNewMailboxName & "||Domain = " & strDomain)

                    Else

                        ''log error
                        objDB.writeInformationToLog("Could not update mailbox name", commandID, "Mailbox = " & strMailboxName & "||New Name=" & strNewMailboxName & "||Domain = " & strDomain)
                        objDB.writeErrorToDatabase(accountID, "Could not update Mailbox name", 0, 0, "changeMailboxName", "mailbox", commandID)
                        Return False

                    End If

                Else

                    ''log error
                    objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not update Mailbox - Mailbox does NOT exist", 0, 0, "changeMailboxName", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not update Mailbox - Postoffice doesnt exist", 0, 0, "changeMailboxName", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in changeMailboxName||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Private Function addAddressMap(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strForwarderName As String) As Boolean

        Try

            Dim objAddressMap As AddressMap
            objAddressMap = New AddressMap

            strMailboxName = Split(strMailboxName, "@")(0)
            strForwarderName = Split(strForwarderName, "@")(0)

            objAddressMap.Account = strDomain
            objAddressMap.DestinationAddress = ""
            objAddressMap.Scope = ""
            objAddressMap.SourceAddress = "[SMTP:" & strForwarderName & "@" & strDomain & "]"

            'does the addreses map already exist?
            If objAddressMap.GetAddressMap() = 1 Then

                ''log error
                objDB.writeInformationToLog("Address map already exists", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                Return False

            Else

                objAddressMap.Account = strDomain
                objAddressMap.DestinationAddress = "[SF:" & strDomain & "/" & strMailboxName & "]"
                objAddressMap.SourceAddress = "[SMTP:" & strForwarderName & "@" & strDomain & "]"
                objAddressMap.Scope = "0"

                ''add address map
                If objAddressMap.AddAddressMap() = 1 Then

                    objDB.writeInformationToLog("Address Map created", commandID, "Source=" & strForwarderName & "||Destination = " & strMailboxName & "||Domain = " & strDomain)
                    Return True

                Else

                    ''log error
                    objDB.writeInformationToLog("Could not add address map", commandID, "Source=" & strForwarderName & "||Destination = " & strMailboxName & "||Domain = " & strDomain)
                    Return False

                End If

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in addAddressMap||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function addExternalAddressMap(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strForwarderName As String, ByVal blnForwarderAddress As Boolean) As Boolean

        Try

            Dim objAddressMap As AddressMap
            objAddressMap = New AddressMap
            Dim strExternalDomain As String

            strExternalDomain = ""
            strMailboxName = Split(strMailboxName, "@")(0)
            strExternalDomain = Split(strForwarderName, "@")(1)
            If strExternalDomain = "" Then strExternalDomain = strDomain
            strForwarderName = Split(strForwarderName, "@")(0)

            objAddressMap.Account = strDomain
            If blnForwarderAddress Then
                objAddressMap.DestinationAddress = "[SMTP:" & strForwarderName & "@" & strExternalDomain & "]"
                objAddressMap.SourceAddress = "[SMTP:" & strMailboxName & "@" & strDomain & "]"
            Else
                objAddressMap.DestinationAddress = "[SF:" & strDomain & "/" & strMailboxName & "]"
                objAddressMap.SourceAddress = "[SMTP:" & strForwarderName & "@" & strExternalDomain & "]"
            End If
            objAddressMap.Scope = ""

            'does the addreses map already exist?
            If objAddressMap.GetAddressMap() = 1 Then

                ''log error
                objDB.writeInformationToLog("Address map already exists", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                Return False

            Else

                objAddressMap.Account = strDomain
                If blnForwarderAddress Then
                    objAddressMap.DestinationAddress = "[SMTP:" & strForwarderName & "@" & strExternalDomain & "]"
                    objAddressMap.SourceAddress = "[SMTP:" & strMailboxName & "@" & strDomain & "]"
                Else
                    objAddressMap.DestinationAddress = "[SF:" & strDomain & "/" & strMailboxName & "]"
                    objAddressMap.SourceAddress = "[SMTP:" & strForwarderName & "@" & strExternalDomain & "]"
                End If
                objAddressMap.Scope = "0"

                ''add address map
                If objAddressMap.AddAddressMap() = 1 Then

                    objDB.writeInformationToLog("Address Map created", commandID, "Source=" & strForwarderName & "@" & strExternalDomain & "||Destination = " & strMailboxName & "||Domain = " & strDomain)
                    Return True

                Else

                    ''log error
                    objDB.writeInformationToLog("Could not add address map", commandID, "Source=" & strForwarderName & "@" & strExternalDomain & "||Destination = " & strMailboxName & "||Domain = " & strDomain)
                    Return False

                End If

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in addExternalAddressMap||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function removeAddressMap(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strForwardMailboxName As String, ByVal blnRemoveAllMaps As Boolean) As Boolean

        Try

            Dim objAddressMap As AddressMap
            objAddressMap = New AddressMap

            ''the easiest way to get the mailbox name
            strForwardMailboxName = Split(strForwardMailboxName, "@")(0)
            strMailboxName = Split(strMailboxName, "@")(0)

            If Not blnRemoveAllMaps Then

                ''we want to remove address map(s) for this mailbox
                objAddressMap = New AddressMap
                objAddressMap.Account = strDomain
                objAddressMap.DestinationAddress = ""
                objAddressMap.SourceAddress = "[SMTP:" & strForwardMailboxName & "@" & strDomain & "]"
                objAddressMap.Scope = ""

                ''just delete the one address map

                If strMailboxName = strForwardMailboxName Then

                    ''log error
                    objDB.writeInformationToLog("I'm not going to delete the mailbox's own address map!", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    Return False

                    ''find the first address map
                ElseIf objAddressMap.GetAddressMap() = 1 Then

                    ''attempt to remove address map
                    If objAddressMap.RemoveAddressMap() = 1 Then

                        ''yay, it worked!
                        objDB.writeInformationToLog("Address map deleted", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        Return True

                    Else

                        ''log error
                        objDB.writeInformationToLog("Could not delete address map", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        Return False

                    End If

                Else

                    ''log error
                    objDB.writeInformationToLog("Could not find address map", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    Return False

                End If

            Else

                ''we want to remove address map(s) for this mailbox
                objAddressMap = New AddressMap
                objAddressMap.Account = strDomain
                objAddressMap.DestinationAddress = "[SF:" & strDomain & "/" & strMailboxName & "]"
                objAddressMap.SourceAddress = ""
                objAddressMap.Scope = ""

                objDB.writeInformationToLog("Attempting to remove all address maps", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

                ''start iterating through and deleteing them all
                If objAddressMap.FindFirstAddressMap() = 1 Then

                    Do
                        ''attempt to remove address map
                        If objAddressMap.RemoveAddressMap() = 1 Then

                            ''yay, it worked!
                            objDB.writeInformationToLog("Address map deleted", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                            '  Return True

                        Else

                            ''log error
                            objDB.writeInformationToLog("Could not delete address map", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                            'Return False

                        End If

                        ''we want to remove address map(s) for this mailbox
                        objAddressMap.Account = strDomain
                        objAddressMap.DestinationAddress = "[SF:" & strDomain & "/" & strMailboxName & "]"
                        objAddressMap.SourceAddress = ""
                        objAddressMap.Scope = ""

                    Loop While objAddressMap.FindFirstAddressMap() = 1

                    ''this should only return true if all the address maps have been removed
                    Return True

                End If

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in removeAddressMap||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function removeExternalAddressMap(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strForwardMailboxName As String, ByVal strMethodName As String, ByVal blnRemoveAllMaps As Boolean) As Boolean

        Try

            Dim objAddressMap As AddressMap
            objAddressMap = New AddressMap

            ''the easiest way to get the mailbox name
            strForwardMailboxName = Split(strForwardMailboxName, "@")(0)
            strMailboxName = Split(strMailboxName, "@")(0)

            If Not blnRemoveAllMaps Then

                ''we want to remove address map(s) for this mailbox
                objAddressMap = New AddressMap
                objAddressMap.Account = strDomain
                objAddressMap.DestinationAddress = ""
                objAddressMap.SourceAddress = "[SMTP:" & strMailboxName & "@" & strDomain & "]"
                objAddressMap.Scope = ""

                ''just delete the one address map

                If strMailboxName = strForwardMailboxName Then

                    ''log error
                    objDB.writeInformationToLog("I'm not going to delete the mailbox's own address map!", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    Return False

                    ''find the first address map
                ElseIf objAddressMap.GetAddressMap() = 1 Then

                    ''attempt to remove address map
                    If objAddressMap.RemoveAddressMap() = 1 Then

                        ''yay, it worked!
                        objDB.writeInformationToLog("Address map deleted", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        Return True

                    Else

                        ''log error
                        objDB.writeInformationToLog("Could not delete address map", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        Return False

                    End If

                Else

                    ''log error
                    objDB.writeInformationToLog("Could not find address map", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    'objDB.writeErrorToDatabase(accountID, "Could not delete address map", 0, 0, strMethodName, "mailbox", commandID)
                    Return False

                End If

            Else

                ''we want to remove address map(s) for this mailbox
                objAddressMap = New AddressMap
                objAddressMap.Account = strDomain
                objAddressMap.DestinationAddress = "[SMTP:" & strForwardMailboxName & "@" & strDomain & "]"
                objAddressMap.SourceAddress = "[SMTP:" & strMailboxName & "@" & strDomain & "]"
                objAddressMap.Scope = ""

                ''start iterating through and deleteing them all
                If objAddressMap.FindFirstAddressMap() = 1 Then

                    Do
                        ''attempt to remove address map
                        If objAddressMap.RemoveAddressMap() = 1 Then

                            ''yay, it worked!
                            objDB.writeInformationToLog("Address map deleted", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                            '  Return True

                        Else

                            ''log error
                            objDB.writeInformationToLog("Could not delete address map", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                            ' objDB.writeErrorToDatabase(accountID, "Could not delete address map", 0, 0, strMethodName, "mailbox", commandID)
                            Return False

                        End If

                        ''we want to remove address map(s) for this mailbox
                        objAddressMap.Account = strDomain
                        objAddressMap.DestinationAddress = "[SMTP:" & strForwardMailboxName & "@" & strDomain & "]"
                        objAddressMap.SourceAddress = "[SMTP:" & strMailboxName & "@" & strDomain & "]"
                        objAddressMap.Scope = ""

                    Loop While objAddressMap.FindNextAddressMap() = 1

                    ''this should only return true if all the address maps have been removed
                    Return True

                End If

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in removeAddressMap||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function addMailboxRedirection(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strRedirectorMailboxName As String, ByVal blnKeepCopy As Boolean) As Boolean

        Dim strRedirectorDomain As String
        strMailboxName = Split(strMailboxName, "@")(0)
        strRedirectorDomain = Split(strRedirectorMailboxName, "@")(1)
        strRedirectorMailboxName = Split(strRedirectorMailboxName, "@")(0)

        Try

            Dim objMailbox As Mailbox

            objDB.writeInformationToLog("Attempting to add redirector", commandID, "Redirector = " & strRedirectorMailboxName & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)

            ''check that postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                objMailbox = doesMailboxExist(strDomain, strMailboxName)

                ''check that mailbox exists
                If Not objMailbox Is Nothing Then

                    ''append the address to the end of the existing forwarders
                    Dim strExistingRedirectors As String
                    strExistingRedirectors = objMailbox.RedirectAddress
                    If strExistingRedirectors = "" Then
                        strExistingRedirectors = strExistingRedirectors & "[SMTP:" & strRedirectorMailboxName & "@" & strRedirectorDomain & "]"
                    Else
                        strExistingRedirectors = strExistingRedirectors & ";[SMTP:" & strRedirectorMailboxName & "@" & strRedirectorDomain & "]"
                    End If

                    ''set the forwarding status
                    ''1 - forward & dont keep copy
                    ''2 - forward & keep copy
                    Dim intRedirectorStatus As Integer
                    If blnKeepCopy Then
                        intRedirectorStatus = 2
                    Else
                        intRedirectorStatus = 1
                    End If

                    ''update the forwarders for the mailbox
                    If objMailbox.EditMailbox(objMailbox.Postoffice, objMailbox.MailboxName, strExistingRedirectors, intRedirectorStatus, objMailbox.Status, objMailbox.Limit, objMailbox.Size) = 1 Then

                        objDB.writeInformationToLog("Redirector added", commandID, "Redirector = " & strRedirectorMailboxName & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        Return True

                    Else

                        ''log error
                        objDB.writeInformationToLog("Could not add redirector", commandID, "Redirector = " & strRedirectorMailboxName & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        objDB.writeErrorToDatabase(accountID, "Could not add redirector", 0, 0, "addMailboxRedirection", "mailbox", commandID)
                        Return False

                    End If

                Else

                    ''log error
                    objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not add redirector - Mailbox does NOT exist", 0, 0, "addMailboxRedirection", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not add redirector - Postoffice doesnt exist", 0, 0, "addMailboxRedirection", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in addMailboxRedirection||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function removeMailboxRedirection(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strRedirectorMailboxName As String, ByVal blnKeepCopy As Boolean) As Boolean

        Dim strRedirectorDomain As String

        strMailboxName = Split(strMailboxName, "@")(0)
        strRedirectorDomain = Split(strRedirectorMailboxName, "@")(1)
        strRedirectorMailboxName = Split(strRedirectorMailboxName, "@")(0)

        Try

            Dim objMailbox As Mailbox

            objDB.writeInformationToLog("Attempting to remove redirector", commandID, "Redirector = " & strRedirectorMailboxName & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)

            ''check that postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                objMailbox = doesMailboxExist(strDomain, strMailboxName)

                ''check that mailbox exists
                If Not objMailbox Is Nothing Then

                    Dim strExistingRedirectors As String
                    strExistingRedirectors = objMailbox.RedirectAddress
                    strExistingRedirectors = Replace(";" & strExistingRedirectors, ";[SMTP:" & strRedirectorMailboxName & "@" & strRedirectorDomain & "]", "")
                    If Left(strExistingRedirectors, 1) = ";" Then strExistingRedirectors = Right(strExistingRedirectors, Len(strExistingRedirectors) - 1)

                    ''set the forwarding status
                    ''1 - forward & dont keep copy
                    ''2 - forward & keep copy
                    Dim intRedirectorStatus As Integer

                    ''if there arent any addresses forwarders then disable forwarding
                    If strExistingRedirectors = "" Then
                        intRedirectorStatus = 0
                    Else
                        If blnKeepCopy Then
                            intRedirectorStatus = 2
                        Else
                            intRedirectorStatus = 1
                        End If
                    End If

                    If objMailbox.EditMailbox(objMailbox.Postoffice, objMailbox.MailboxName, strExistingRedirectors, intRedirectorStatus, objMailbox.Status, objMailbox.Limit, objMailbox.Size) = 1 Then

                        objDB.writeInformationToLog("Redirector removed", commandID, "Redirector = " & strRedirectorMailboxName & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        Return True

                    Else

                        ''log error
                        objDB.writeInformationToLog("Could not removed redirector", commandID, "Redirector = " & strRedirectorMailboxName & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        objDB.writeErrorToDatabase(accountID, "Could not removed redirector", 0, 0, "removeMailboxRedirection", "mailbox", commandID)
                        Return False

                    End If

                Else

                    ''log error
                    objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "CCould not removed redirector - Mailbox does NOT exist", 0, 0, "removeMailboxRedirection", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not removed redirector - Postoffice doesnt exist", 0, 0, "removeMailboxRedirection", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in removeMailboxRedirection||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function removeMailboxForwarder(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strForwardMailboxName As String) As Boolean

        strMailboxName = Split(strMailboxName, "@")(0)
        strForwardMailboxName = Split(strForwardMailboxName, "@")(0)

        Try

            objDB.writeInformationToLog("Attempting to remove fowarder", commandID, "Forwarder = " & strForwardMailboxName & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)

            ''check that postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                ''check that mailbox exists
                If doesMailboxExist(strDomain, strMailboxName) Is Nothing Then

                    If removeExternalAddressMap(strDomain, strMailboxName, strForwardMailboxName, "removeMailboxForwarder", False) Then

                        ''yay, forwarder removed
                        objDB.writeInformationToLog("Fowarder removed", commandID, "Forward=" & strForwardMailboxName & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        Return True

                    Else

                        ''log error
                        objDB.writeInformationToLog("Could not remove forwarder", commandID, "Forward=" & strForwardMailboxName & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        objDB.writeErrorToDatabase(accountID, "Could not remove forwarder", 0, 0, "removeMailboxForwarder", "mailbox", commandID)
                        Return False

                    End If

                Else

                    ''log error
                    objDB.writeInformationToLog("Mailbox already exists", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not create forwarder - Mailbox already exists", 0, 0, "removeMailboxForwarder", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not create forwarder - Postoffice doesnt exist", 0, 0, "removeMailboxForwarder", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in removeMailboxForwarder||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function addMailboxForwarder(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strForwardMailboxName As String) As Boolean

        Try

            objDB.writeInformationToLog("Attempting to add fowarder", commandID, "Forwarder = " & strForwardMailboxName & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)

            ''check that postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                ''check that mailbox exists
                If doesMailboxExist(strDomain, strMailboxName) Is Nothing Then

                    objDB.writeInformationToLog("Attemping to add external address map", commandID, "Forward=" & strForwardMailboxName & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Attemping to add external address map", 0, 0, "addMailboxForwarder", "mailbox", commandID)

                    If addExternalAddressMap(strDomain, strMailboxName, strForwardMailboxName, True) Then

                        ''yay, forwarder added
                        objDB.writeInformationToLog("Fowarder added", commandID, "Forward=" & strForwardMailboxName & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        Return True

                    Else

                        ''log error
                        objDB.writeInformationToLog("Could not create forwarder", commandID, "Forward=" & strForwardMailboxName & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        objDB.writeErrorToDatabase(accountID, "Could not create forwarder", 0, 0, "addMailboxForwarder", "mailbox", commandID)
                        Return False

                    End If

                Else

                    ''log error
                    objDB.writeInformationToLog("Mailbox does exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not create forwarder - Mailbox already exists", 0, 0, "addMailboxForwarder", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not create forwarder - Postoffice doesnt exist", 0, 0, "addMailboxForwarder", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in addMailboxForwarder||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function addPOPConnector(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strUsername As String, ByVal strPassword As String, ByVal strMailserverAddress As String, ByVal blnLeaveCopy As Boolean, ByVal intPOPPort As Integer, ByVal blnAPOP As Boolean) As Boolean

        Try

            Dim objPOPConnector As POPRetriever

            objDB.writeInformationToLog("Attempting to create POP connector", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

            ''check that postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                ''check that mailbox exists
                If Not doesMailboxExist(strDomain, strMailboxName) Is Nothing Then

                    objPOPConnector = New POPRetriever

                    ''see if a pop receiver already exists
                    objPOPConnector.APOP = -1
                    objPOPConnector.Status = -1
                    objPOPConnector.UserName = strUsername
                    objPOPConnector.Port = -1
                    objPOPConnector.Password = ""
                    objPOPConnector.MailServer = strMailserverAddress
                    objPOPConnector.LocalPostoffice = strDomain
                    objPOPConnector.LocalMailbox = ""
                    objPOPConnector.LeaveOnServer = -1
                    objPOPConnector.Host = ""
                    objPOPConnector.DownloadNewOnly = -1

                    If Not objPOPConnector.GetPOPRetreiver() = 1 Then

                        objPOPConnector = New POPRetriever

                        ''setup pop retreiver
                        objPOPConnector.APOP = CInt(blnAPOP)
                        objPOPConnector.Status = 1
                        objPOPConnector.UserName = strUsername
                        objPOPConnector.Port = intPOPPort
                        objPOPConnector.Password = strPassword
                        objPOPConnector.MailServer = strMailserverAddress
                        objPOPConnector.LocalPostoffice = strDomain
                        objPOPConnector.LocalMailbox = strMailboxName
                        objPOPConnector.LeaveOnServer = CInt(blnLeaveCopy)
                        objPOPConnector.Host = ""
                        objPOPConnector.DownloadNewOnly = 1

                        If objPOPConnector.AddPOPRetreiver() = 1 Then

                            objDB.writeInformationToLog("POP retriever created", commandID, "Username = " & strUsername & "||Mailserver = " & strMailserverAddress & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                            Return True

                        Else

                            ''log error
                            objDB.writeInformationToLog("Could not create POP retriever", commandID, "Username = " & strUsername & "||Mailserver = " & strMailserverAddress & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                            objDB.writeErrorToDatabase(accountID, "Could not create POP retreiver", 0, 0, "addPOPConnector", "mailbox", commandID)
                            Return False

                        End If

                    Else

                        ''dont put this into the error database - this can "leagally" happen

                        ''log error
                        objDB.writeInformationToLog("POP Retriever already exists", commandID, "Username = " & strUsername & "||Mailserver = " & strMailserverAddress & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        Return False

                    End If

                Else

                    ''log error
                    objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not create POP retreiver - Mailbox does NOT exist", 0, 0, "addPOPConnector", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not create POP retreiver - Postoffice doesnt exist", 0, 0, "addPOPConnector", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in addPOPConnector||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function removePOPConnector(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strUsername As String, ByVal strPassword As String, ByVal strMailserverAddress As String, ByVal blnLeaveCopy As Boolean, ByVal intPOPPort As Integer, ByVal blnAPOP As Boolean) As Boolean

        Try

            Dim objPOPConnector As POPRetriever

            objDB.writeInformationToLog("Attempting to delete POP connector", commandID, "Username = " & strUsername & "||Mailserver = " & strMailserverAddress & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)

            ''check that postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                ''check that mailbox exists
                If Not doesMailboxExist(strDomain, strMailboxName) Is Nothing Then

                    objPOPConnector = New POPRetriever

                    ''see if a pop receiver already exists
                    objPOPConnector.APOP = -1
                    objPOPConnector.Status = -1
                    objPOPConnector.UserName = strUsername
                    objPOPConnector.Port = intPOPPort
                    objPOPConnector.Password = ""
                    objPOPConnector.MailServer = strMailserverAddress
                    objPOPConnector.LocalPostoffice = strDomain
                    objPOPConnector.LocalMailbox = strMailboxName
                    objPOPConnector.LeaveOnServer = -1
                    objPOPConnector.Host = ""
                    objPOPConnector.DownloadNewOnly = -1

                    objDB.writeInformationToLog("Attempting to find POP connector", commandID, "Username = " & strUsername & "||Mailserver = " & strMailserverAddress & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)


                    If objPOPConnector.GetPOPRetreiver() = 1 Then

                        If objPOPConnector.RemovePOPRetreiver() = 1 Then

                            objDB.writeInformationToLog("POP retriever deleted", commandID, "Username = " & strUsername & "||Mailserver = " & strMailserverAddress & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                            Return True

                        Else

                            ''log error
                            objDB.writeInformationToLog("Could not delete POP retriever", commandID, "Username = " & strUsername & "||Mailserver = " & strMailserverAddress & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                            objDB.writeErrorToDatabase(accountID, "Could delete POP retreiver", 0, 0, "removePOPConnector", "mailbox", commandID)
                            Return False

                        End If

                    Else

                        ''dont put this into the error database - this can "leagally" happen

                        ''log error
                        objDB.writeInformationToLog("POP Retriever does not exist", commandID, "Username = " & strUsername & "||Mailserver = " & strMailserverAddress & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        Return False

                    End If

                Else

                    ''log error
                    objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could create POP retreiver - Mailbox does NOT exist", 0, 0, "removePConnector", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not create POP retreiver - Postoffice doesnt exist", 0, 0, "removePOPConnector", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in removePOPConnector||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function updatePOPConnector(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strUsername As String, ByVal strPassword As String, ByVal strMailserverAddress As String, ByVal blnLeaveCopy As Boolean, ByVal intPOPPort As Integer, ByVal blnAPOP As Boolean, ByVal strNewUsername As String, ByVal strNewPassword As String, ByVal strNewMailserverAddress As String, ByVal blnNewLeaveCopy As Boolean, ByVal intNewPOPPort As Integer, ByVal blnNewAPOP As Boolean) As Boolean

        ''remove the connector, then add another with the new details *simples*

        objDB.writeInformationToLog("Attempting to update POP connector", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

        objDB.writeInformationToLog("Attempting to delete POP retriever", commandID, "Username = " & strUsername & "||Mailserver = " & strMailserverAddress & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)

        If removePOPConnector(strDomain, strMailboxName, strUsername, strPassword, strMailserverAddress, blnLeaveCopy, intPOPPort, blnAPOP) Then

            objDB.writeInformationToLog("Attempting to add POP retriever", commandID, "Username = " & strNewUsername & "||Mailserver = " & strNewMailserverAddress & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)

            If addPOPConnector(strDomain, strMailboxName, strNewUsername, strNewPassword, strNewMailserverAddress, blnNewLeaveCopy, intNewPOPPort, blnNewAPOP) Then

                objDB.writeInformationToLog("POP connector details updated", commandID, "Username = " & strNewUsername & "||Mailserver = " & strNewMailserverAddress & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                Return True

            End If

        End If

    End Function

    Public Function manageAutoResponder(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strSubjectText As String, ByVal strBodyText As String, ByVal blnEnabled As Boolean) As Boolean

        Dim arrLines(3) As String
        Dim intLineIndex As Integer = 0
        Dim intCurrentLineIndex As Integer = 0

        Try

            Dim objMailbox As Mailbox

            objDB.writeInformationToLog("Attempting to create auto responder", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

            ''check that postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                objMailbox = doesMailboxExist(strDomain, strMailboxName)

                ''check that mailbox exists
                If Not objMailbox Is Nothing Then

                    ''check for blocking file
                    While File.Exists(strMailEnableLocation & "\Postoffices\" & strDomain & "\MAILROOT\" & strMailboxName & "\AUTORESPOND.BLK")
                    End While

                    ''create blocking file
                    Dim objWFile As New StreamWriter(strMailEnableLocation & "\Postoffices\" & strDomain & "\MAILROOT\" & strMailboxName & "\AUTORESPOND.BLK")
                    objWFile.Close()

                    ''create config file
                    If Not File.Exists(strMailEnableLocation & "\Postoffices\" & strDomain & "\MAILROOT\" & strMailboxName & "\AUTORESPOND.CF_") Then
                        objWFile = New StreamWriter(strMailEnableLocation & "\Postoffices\" & strDomain & "\MAILROOT\" & strMailboxName & "\AUTORESPOND.CF_")
                        objWFile.Close()
                    End If

                    ''write new details to config file
                    objWFile = New StreamWriter(strMailEnableLocation & "\Postoffices\" & strDomain & "\MAILROOT\" & strMailboxName & "\AUTORESPOND.CF_")

                    objWFile.WriteLine("From: " & strMailboxName & "@" & strDomain)
                    objWFile.WriteLine("Subject: " & strSubjectText)
                    objWFile.WriteLine("Reply-To: <" & strMailboxName & "@" & strDomain & ">")
                    objWFile.WriteLine("")
                    objWFile.WriteLine(strBodyText)
                    objWFile.Flush()
                    objWFile.Close()
                    objWFile.Dispose()

                    ''are we enabling or disabling the autoresponder?
                    If blnEnabled Then

                        ''if auto responder is enabled, then copy config file
                        File.Copy(strMailEnableLocation & "\Postoffices\" & strDomain & "\MAILROOT\" & strMailboxName & "\AUTORESPOND.CF_", strMailEnableLocation & "\Postoffices\" & strDomain & "\MAILROOT\" & strMailboxName & "\AUTORESPOND.CFG", True)

                    Else

                        If File.Exists(strMailEnableLocation & "\Postoffices\" & strDomain & "\MAILROOT\" & strMailboxName & "\AUTORESPOND.CFG") Then

                            ''if auto responder is disabled, then delete copied config file
                            File.Delete(strMailEnableLocation & "\Postoffices\" & strDomain & "\MAILROOT\" & strMailboxName & "\AUTORESPOND.CFG")

                        End If

                    End If

                    ''delete blocking file
                    File.Delete(strMailEnableLocation & "\Postoffices\" & strDomain & "\MAILROOT\" & strMailboxName & "\AUTORESPOND.BLK")

                    Return True

                Else

                    ''log error
                    objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not create auto responder - Mailbox does NOT exist", 0, 0, "manageAutoResponder", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not create auto responder - Postoffice doesnt exist", 0, 0, "manageAutoResponder", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in managerAutoResponder||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function addWebMail(ByVal strDomain As String) As Boolean

        Try

            ''check that postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                Dim objSite As Site
                Dim svrMgr As New ServerManager

                Try

                    ''try and retrieve the default webmail site
                    objSite = svrMgr.Sites("MailEnable WebMail")

                Catch ex As Exception

                    ''log error
                    objDB.writeInformationToLog("Server object failed", commandID, "Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Server Object Failed", 0, 0, "addWebMail", "mailbox", commandID)
                    Return False

                End Try


                If Not objSite Is Nothing Then

                    'wait for blocking file to be removed
                    While File.Exists(strMailEnableLocation & "\config\SITE-BASES.BLK")
                    End While

                    ''create blocking file
                    Dim objWFile As New StreamWriter(strMailEnableLocation & "\config\SITE-BASES.BLK")
                    objWFile.Close()

                    'wait for blocking file to be removed
                    While File.Exists(strMailEnableLocation & "\config\SITE-POSTOFFICE.BLK")
                    End While

                    ''create blocking file
                    objWFile = New StreamWriter(strMailEnableLocation & "\config\SITE-POSTOFFICE.BLK")
                    objWFile.Close()

                    ''append domain to the postoffice file
                    objWFile = New StreamWriter(strMailEnableLocation & "\config\SITE-POSTOFFICE.TAB", True)
                    objWFile.WriteLine("webmail." & strDomain & vbTab & strDomain)
                    objWFile.Close()

                    ''append domain skin setting to file
                    objWFile = New StreamWriter(strMailEnableLocation & "\config\SITE-BASES.TAB", True)
                    objWFile.WriteLine("webmail." & strDomain & vbTab & "HooDoo")
                    objWFile.Close()
                    objWFile.Dispose()

                    ''delete block files
                    File.Delete(strMailEnableLocation & "\config\SITE-BASES.BLK")
                    File.Delete(strMailEnableLocation & "\config\SITE-POSTOFFICE.BLK")

                    ''create host header for webmail IIS record
                    Dim objBind As Binding
                    objBind = objSite.Bindings.CreateElement()
                    objBind.Protocol = "http"
                    objBind.BindingInformation = strIISIPAddress & ":80:" & "webmail." & strDomain
                    objSite.Bindings.Add(objBind)

                    ''save the changes to IIS
                    svrMgr.CommitChanges()

                    svrMgr.Dispose()

                    Return True

                Else

                    ''log error
                    objDB.writeInformationToLog("Could not find default webmail site", commandID, "Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not find default webmail site", 0, 0, "addWebMail", "mailbox", commandID)

                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not add webmail - Postoffice doesnt exist", 0, 0, "addWebMail", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in addWebMail||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function removeWebMail(ByVal strDomain As String) As Boolean

        Try

            ''check that postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                Dim objSite As Site
                Dim svrMgr As New ServerManager

                ''try and retrieve the default webmail site
                objSite = svrMgr.Sites("MailEnable WebMail")

                If Not objSite Is Nothing Then


                    'wait for blocking file to be removed
                    While File.Exists(strMailEnableLocation & "\config\SITE-BASES.BLK")
                    End While

                    ''create blocking file (not only for blocking but also as a temp location
                    Dim objWFile = New StreamWriter(strMailEnableLocation & "\config\SITE-BASES.BLK", False)

                    ''open the current postoffice file
                    Dim objRFile As New StreamReader(strMailEnableLocation & "\config\SITE-BASES.TAB")
                    Dim strFileContentLine As String

                    ''loop through the files data
                    While Not objRFile.Peek = -1

                        strFileContentLine = objRFile.ReadLine()

                        ''if the current line doesnt contain what we're trying to remove
                        ''then write the line into the block file
                        If Not strFileContentLine = "webmail." & strDomain & vbTab & "HooDoo" Then
                            objWFile.WriteLine(strFileContentLine)
                        End If

                    End While

                    ''close files
                    objRFile.Close()
                    objRFile.Dispose()
                    objWFile.Close()

                    'wait for blocking file to be removed
                    While File.Exists(strMailEnableLocation & "\config\SITE-POSTOFFICE.BLK")
                    End While

                    ''create blocking file (not only for blocking but also as a temp location
                    objWFile = New StreamWriter(strMailEnableLocation & "\config\SITE-POSTOFFICE.BLK", False)

                    ''open the current postoffice file
                    objRFile = New StreamReader(strMailEnableLocation & "\config\SITE-POSTOFFICE.TAB")

                    ''loop through the files data
                    While Not objRFile.Peek = -1

                        strFileContentLine = objRFile.ReadLine()

                        ''if the current line doesnt contain what we're trying to remove
                        ''then write the line into the block file
                        If Not strFileContentLine = "webmail." & strDomain & vbTab & strDomain Then
                            objWFile.WriteLine(strFileContentLine)
                        End If

                    End While

                    ''close files
                    objRFile.Close()
                    objRFile.Dispose()
                    objWFile.Close()

                    ''overwrite existing TAB files
                    File.Copy(strMailEnableLocation & "\config\SITE-BASES.BLK", strMailEnableLocation & "\config\SITE-BASES.TAB", True)
                    File.Copy(strMailEnableLocation & "\config\SITE-POSTOFFICE.BLK", strMailEnableLocation & "\config\SITE-POSTOFFICE.TAB", True)

                    ''delete block files
                    File.Delete(strMailEnableLocation & "\config\SITE-BASES.BLK")
                    File.Delete(strMailEnableLocation & "\config\SITE-POSTOFFICE.BLK")

                    ''find and remove host header
                    For Each binding In objSite.Bindings
                        If binding.BindingInformation = strIISIPAddress & ":80:webmail." & strDomain Then
                            binding.Delete()
                        End If
                    Next

                    ''save the changes to IIS
                    svrMgr.CommitChanges()

                    svrMgr.Dispose()

                    Return True

                Else

                    ''log error
                    objDB.writeInformationToLog("Could not find default webmail site", commandID, "Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not find default webmail site", 0, 0, "addWebMail", "mailbox", commandID)

                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not manage webmail - Postoffice doesnt exist", 0, 0, "addWebMail", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in addWebMail||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function addAliasDomain(ByVal strDomain As String, ByVal strAliasDomain As String) As Boolean

        Try

            objDB.writeInformationToLog("Attempting to create alias domain", commandID, "Domain = " & strDomain & "||Alias = " & strAliasDomain)

            ''check if the postoffice already exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                If doesDomainExist(strAliasDomain) Is Nothing Then

                    Dim objDomain As Domain
                    objDomain = New Domain

                    objDomain.AccountName = strDomain
                    objDomain.DomainName = strAliasDomain
                    objDomain.Status = 1
                    objDomain.DomainRedirectionHosts = ""
                    objDomain.DomainRedirectionStatus = 0

                    ''try and add the domain
                    If objDomain.AddDomain() = 1 Then

                        ''now that the alias domain has been added, we have to add address maps to all the mailboxes
                        objDB.writeInformationToLog("Alias Domain added, attempting to add address maps to all mailboxes", commandID, "Domain = " & strDomain)

                        Dim objMailbox As New Mailbox
                        Dim objAliasAddressMap As AddressMap

                        objMailbox.Postoffice = strDomain
                        objMailbox.MailboxName = ""
                        objMailbox.Limit = -1
                        objMailbox.RedirectAddress = ""
                        objMailbox.RedirectStatus = -1
                        objMailbox.Size = -1
                        objMailbox.Status = -1

                        If objMailbox.FindFirstMailbox() = 1 Then

                            Do

                                ''for debugging, log the mailboxes  found
                                objDB.writeInformationToLog("Found mailbox: " & objMailbox.MailboxName, commandID, "Domain = " & strDomain)

                                objAliasAddressMap = New AddressMap
                                objAliasAddressMap.Account = strDomain
                                objAliasAddressMap.DestinationAddress = "[SF:" & strDomain & "/" & objMailbox.MailboxName & "]"
                                objAliasAddressMap.Scope = "0"
                                objAliasAddressMap.SourceAddress = "[SMTP:" & objMailbox.MailboxName & "@" & strAliasDomain & "]"
                                objAliasAddressMap.Status = 1

                                If objAliasAddressMap.AddAddressMap = 1 Then

                                    ''for debugging, log the mailboxes  found
                                    objDB.writeInformationToLog("Added address map for: " & objMailbox.MailboxName & "@" & strAliasDomain, commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)

                                Else

                                    ''log error
                                    objDB.writeInformationToLog("Failed to add address map for: " & objMailbox.MailboxName & "@" & strAliasDomain, commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)
                                    objDB.writeErrorToDatabase(accountID, "Failed to add address map", 0, 0, "addAliasDomain", "mailbox", commandID)

                                End If

                            Loop While objMailbox.FindNextMailbox = 1


                            objAliasAddressMap = New AddressMap
                            objAliasAddressMap.Account = strDomain
                            objAliasAddressMap.DestinationAddress = "[SF:" & strDomain & "/postmaster]"
                            objAliasAddressMap.Scope = "0"
                            objAliasAddressMap.SourceAddress = "[SMTP:*@" & strAliasDomain & "]"
                            objAliasAddressMap.Status = 1

                            If objAliasAddressMap.AddAddressMap = 1 Then

                                ''for debugging, log the mailboxes  found
                                objDB.writeInformationToLog("Added address map for: *@" & strAliasDomain, commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)

                            Else

                                ''log error
                                objDB.writeInformationToLog("Failed to add address map for: *@" & strAliasDomain, commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)
                                objDB.writeErrorToDatabase(accountID, "Failed to add address map", 0, 0, "addAliasDomain", "mailbox", commandID)

                            End If

                            objAliasAddressMap = New AddressMap
                            objAliasAddressMap.Account = strDomain
                            objAliasAddressMap.DestinationAddress = "[SF:" & strDomain & "/postmaster]"
                            objAliasAddressMap.Scope = "0"
                            objAliasAddressMap.SourceAddress = "[SMTP:abuse@" & strAliasDomain & "]"
                            objAliasAddressMap.Status = 1

                            If objAliasAddressMap.AddAddressMap = 1 Then

                                ''for debugging, log the mailboxes  found
                                objDB.writeInformationToLog("Added address map for: abuse@" & strAliasDomain, commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)

                            Else

                                ''log error
                                objDB.writeInformationToLog("Failed to add address map for: abuse@" & strAliasDomain, commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)
                                objDB.writeErrorToDatabase(accountID, "Failed to add address map", 0, 0, "addAliasDomain", "mailbox", commandID)


                            End If

                            objAliasAddressMap = Nothing
                            objMailbox = Nothing

                            Return True

                        Else

                            ''log error
                            objDB.writeInformationToLog("Could not find first mailbox", commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)
                            objDB.writeErrorToDatabase(accountID, "Could not find first mailbox", 0, 0, "addAliasDomain", "mailbox", commandID)
                            Return False

                        End If

                    Else

                        ''log error
                        objDB.writeInformationToLog("Domain was not created", commandID, "Domain = " & strDomain)
                        objDB.writeErrorToDatabase(accountID, "Could not create domain", 0, 0, "addAliasDomain", "mailbox", commandID)
                        Return False

                    End If

                Else

                    ''log error
                    objDB.writeInformationToLog("Domain already exists", commandID, "Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not add alias domain - domain already exists", 0, 0, "addAliasDomain", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not add alias domsin - Postoffice doesnt exist", 0, 0, "addAliasDomain", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in addAliasDomain||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function removeAliasDomain(ByVal strDomain As String, ByVal strAliasDomain As String) As Boolean

        Try

            objDB.writeInformationToLog("Attempting to remove alias domain", commandID, "Domain = " & strDomain & "||Alias = " & strAliasDomain)

            ''check if the postoffice already exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                Dim objMailbox As New Mailbox
                Dim objAliasAddressMap As AddressMap

                objMailbox.Postoffice = strDomain
                objMailbox.MailboxName = ""
                objMailbox.Limit = -1
                objMailbox.RedirectAddress = ""
                objMailbox.RedirectStatus = -1
                objMailbox.Size = -1
                objMailbox.Status = -1

                If objMailbox.FindFirstMailbox() = 1 Then

                    Do

                        ''for debugging, log the mailboxes  found
                        objDB.writeInformationToLog("Found mailbox: " & objMailbox.MailboxName, commandID, "Domain = " & strDomain)

                        objAliasAddressMap = New AddressMap
                        objAliasAddressMap.Account = strDomain
                        objAliasAddressMap.DestinationAddress = "[SF:" & strDomain & "/" & objMailbox.MailboxName & "]"
                        objAliasAddressMap.Scope = "0"
                        objAliasAddressMap.SourceAddress = "[SMTP:" & objMailbox.MailboxName & "@" & strAliasDomain & "]"
                        objAliasAddressMap.Status = 1

                        If objAliasAddressMap.RemoveAddressMap = 1 Then

                            ''for debugging, log the mailboxes  found
                            objDB.writeInformationToLog("Removed address map for: " & objMailbox.MailboxName & "@" & strAliasDomain, commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)

                        Else

                            ''log error
                            objDB.writeInformationToLog("Failed to remove address map for: " & objMailbox.MailboxName & "@" & strAliasDomain, commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)
                            objDB.writeErrorToDatabase(accountID, "Failed to remove address map", 0, 0, "addAliasDomain", "mailbox", commandID)

                        End If

                    Loop While objMailbox.FindNextMailbox = 1


                    objAliasAddressMap = New AddressMap
                    objAliasAddressMap.Account = strDomain
                    objAliasAddressMap.DestinationAddress = "[SF:" & strDomain & "/postmaster]"
                    objAliasAddressMap.Scope = "0"
                    objAliasAddressMap.SourceAddress = "[SMTP:*@" & strAliasDomain & "]"
                    objAliasAddressMap.Status = 1

                    If objAliasAddressMap.RemoveAddressMap = 1 Then

                        ''for debugging, log the mailboxes  found
                        objDB.writeInformationToLog("Removed address map for: *@" & strAliasDomain, commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)

                    Else

                        ''log error
                        objDB.writeInformationToLog("Failed to add address map for: *@" & strAliasDomain, commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)
                        objDB.writeErrorToDatabase(accountID, "Failed to add address map", 0, 0, "addAliasDomain", "mailbox", commandID)

                    End If

                    objAliasAddressMap = New AddressMap
                    objAliasAddressMap.Account = strDomain
                    objAliasAddressMap.DestinationAddress = "[SF:" & strDomain & "/postmaster]"
                    objAliasAddressMap.Scope = "0"
                    objAliasAddressMap.SourceAddress = "[SMTP:abuse@" & strAliasDomain & "]"
                    objAliasAddressMap.Status = 1

                    If objAliasAddressMap.RemoveAddressMap = 1 Then

                        ''for debugging, log the mailboxes  found
                        objDB.writeInformationToLog("Removed address map for: abuse@" & strAliasDomain, commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)

                    Else

                        ''log error
                        objDB.writeInformationToLog("Failed to add address map for: abuse@" & strAliasDomain, commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)
                        objDB.writeErrorToDatabase(accountID, "Failed to add address map", 0, 0, "removeAliasDomain", "mailbox", commandID)


                    End If

                    Dim objDomain As New Domain
                    objDomain = New Domain
                    objDomain.AccountName = strDomain
                    objDomain.DomainName = strAliasDomain
                    objDomain.Status = -1
                    objDomain.DomainRedirectionHosts = ""
                    objDomain.DomainRedirectionStatus = -1

                    If objDomain.RemoveDomain = 1 Then

                        ''for debugging, log the mailboxes  found
                        objDB.writeInformationToLog("Removed alias domain", commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)

                    Else

                        ''log error
                        objDB.writeInformationToLog("Could not remove alias domain", commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)
                        objDB.writeErrorToDatabase(accountID, "Could remove alias domain", 0, 0, "addAliasDomain", "mailbox", commandID)
                        Return False

                    End If

                    objAliasAddressMap = Nothing
                    objMailbox = Nothing
                    objDomain = Nothing

                    Return True

                Else

                    ''log error
                    objDB.writeInformationToLog("Could not find first mailbox", commandID, "Domain = " & strDomain & "||Alias Domain = " & strAliasDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not find first mailbox", 0, 0, "removeAliasDomain", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not remove alias domain - Postoffice doesnt exist", 0, 0, "addAliasDomain", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in removeAliasDomain||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function addCatchall(ByVal strDomain As String) As Boolean

    End Function

    Public Function removedCatchall(ByVal strDomain As String) As Boolean

    End Function

    Public Function mailboxUsage(ByVal strDomain As String, ByVal strMailboxName As String) As Boolean

        Try

            objDB.writeInformationToLog("Attempting to return mailbox usage", commandID, "Domain = " & strDomain & "||Mailbox = " & strMailboxName)

            ''check if the postoffice already exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                Try

                    Dim objMailbox As New Mailbox

                    ''the easiest way to get the mailbox name
                    strMailboxName = Split(strMailboxName, "@")(0)

                    ''setup mailbox details
                    objMailbox.Postoffice = strDomain
                    objMailbox.MailboxName = strMailboxName
                    objMailbox.Size = -1
                    objDB.writeInformationToLog("Checking to see if Mailbox exists", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

                    ''check if mailbox exists
                    If objMailbox.GetMailbox() = 1 Then

                        objDB.writeInformationToLog("Updated mailbox size file", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

                        Return True

                    Else

                        objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        Return False

                    End If

                Catch ex As Exception

                    objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not retrieve mailbox usage - Mailbox doesnt exist", 0, 0, "mailboxUsage", "mailbox", commandID)
                    Return False

                End Try

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not remove alias domain - Postoffice doesnt exist", 0, 0, "mailboxUsage", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in mailboxUsage||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function purgeMailbox(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strFolderName As String, ByVal minFileAge As Integer) As Boolean

        Try

            objDB.writeInformationToLog("Attempting to return mailbox usage", commandID, "Domain = " & strDomain & "||Mailbox = " & strMailboxName)

            ''check if the postoffice already exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                Try

                    Dim objMailbox As New Mailbox

                    ''the easiest way to get the mailbox name
                    strMailboxName = Split(strMailboxName, "@")(0)

                    ''setup mailbox details
                    objMailbox.Postoffice = strDomain
                    objMailbox.MailboxName = strMailboxName
                    objMailbox.Size = -1
                    objDB.writeInformationToLog("Checking to see if Mailbox exists", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

                    ''check if mailbox exists
                    If objMailbox.GetMailbox() = 1 Then

                        objDB.writeInformationToLog("Updated mailbox size file", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)

                        Return True

                    Else

                        objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        Return False

                    End If

                Catch ex As Exception

                    objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not retrieve mailbox usage - Mailbox doesnt exist", 0, 0, "mailboxUsage", "mailbox", commandID)
                    Return False

                End Try

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not remove alias domain - Postoffice doesnt exist", 0, 0, "mailboxUsage", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in mailboxUsage||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function addAliasAddress(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strAliasAddress As String) As Boolean

        strMailboxName = Split(strMailboxName, "@")(0)
        strAliasAddress = Split(strAliasAddress, "@")(0)

        Try

            objDB.writeInformationToLog("Attempting to add alias address", commandID, "AliasAddress = " & strAliasAddress & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)

            ''check that postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                ''check that mailbox exists
                If Not doesMailboxExist(strDomain, strMailboxName) Is Nothing Then

                    If addAddressMap(strDomain, strMailboxName, strAliasAddress) Then

                        ''alias address added
                        Return True

                    Else

                        ''log error
                        objDB.writeInformationToLog("Could not create alias address map", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        objDB.writeErrorToDatabase(accountID, "Could not add alias address", 0, 0, "addAliasAddress", "mailbox", commandID)
                        Return False

                    End If

                Else

                    ''log error
                    objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not add alias address - Mailbox does NOT exist", 0, 0, "addAliasAddress", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not add alias address - Postoffice doesnt exist", 0, 0, "addAliasAddress", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in addAliasAddress||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

    End Function

    Public Function removeAliasAddress(ByVal strDomain As String, ByVal strMailboxName As String, ByVal strAliasAddress As String) As Boolean

        Try

            objDB.writeInformationToLog("Attempting to remove alias address", commandID, "AliasAddress = " & strAliasAddress & "||Mailbox = " & strMailboxName & "||Domain = " & strDomain)

            ''check that postoffice exists
            If Not doesPostofficeExist(strDomain) Is Nothing Then

                ''check that mailbox exists
                If Not doesMailboxExist(strDomain, strMailboxName) Is Nothing Then

                    If removeAddressMap(strDomain, strMailboxName, strAliasAddress, False) Then

                        ''alias address removed
                        Return True

                    Else

                        ''log error
                        objDB.writeInformationToLog("Could not remove alias address map", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                        objDB.writeErrorToDatabase(accountID, "Could not remove alias address", 0, 0, "removeAliasAddress", "mailbox", commandID)
                        Return False

                    End If

                Else

                    ''log error
                    objDB.writeInformationToLog("Mailbox does NOT exist", commandID, "Mailbox = " & strMailboxName & "||Domain = " & strDomain)
                    objDB.writeErrorToDatabase(accountID, "Could not remove alias address - Mailbox does NOT exist", 0, 0, "removeAliasAddress", "mailbox", commandID)
                    Return False

                End If

            Else

                ''log error
                objDB.writeInformationToLog("Postoffice does NOT exist", commandID, "Domain = " & strDomain)
                objDB.writeErrorToDatabase(accountID, "Could not remove alias address - Postoffice doesnt exist", 0, 0, "removerAliasAddress", "mailbox", commandID)
                Return False

            End If

        Catch ex As Exception

            Dim objLog As Diagnostics.EventLog
            objLog = New Diagnostics.EventLog
            objLog.Source = "RecallHostingService"
            EventLog.WriteEntry("RecallHostingService_log", "Problem in removeAliasAddress||" & Err.Description, EventLogEntryType.Information)

            Return False

        End Try

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
            objDB.Dispose()

            Dispose()
            MyBase.Finalize()
        End If
    End Sub

    Protected Overrides Sub Finalize()
        ' Simply call Dispose(False).
        Dispose(False)
    End Sub


End Class
