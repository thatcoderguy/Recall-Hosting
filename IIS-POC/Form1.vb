Imports Microsoft.Web.Administration
Imports System.IO
Imports System.Security.AccessControl
Imports System.Management
Imports Microsoft.Win32

Public Class Form1
    ''' <summary>
    ''' TO DO LIST
    ''' 1. NS Records                     DONE
    ''' 2. Delete/modify records          DONE
    ''' 3. set primary server of zone     DONE
    ''' 4. zone transfer records          DONE
    ''' 5. check if zone already exists   DONE
    ''' 6. remote domain setup            
    ''' 7. Delete/stop Zone               DONE
    ''' 8. Check if host header exists    DONE
    ''' 9. add host header                DONE
    ''' 10. delete header                 DONE
    ''' 11. add SSL cert to https header  50%
    ''' </summary>

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim strDomain As String
        Dim svrMgr As ServerManager
        Dim objSite As Site
        Dim appPool As ApplicationPool
        Dim config As Configuration
        Dim section As ConfigurationSection
        Dim attrib As ConfigurationAttribute

        strDomain = TextBox1.Text

        ''create folder
        Try
            Directory.CreateDirectory("E:\hosting\" & strDomain)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        svrMgr = New ServerManager()

        ''add iis entry
        Try
            svrMgr = New ServerManager()
            objSite = svrMgr.Sites.Add(strDomain, "http", "109.228.6.9:80:" & strDomain, "E:\hosting\" & strDomain)
            objSite.ServerAutoStart = True
            svrMgr.CommitChanges()

        Catch ex As Exception
            MsgBox("1 " & ex.Message)

        End Try

        ''create app pool
        Try

            objSite = svrMgr.Sites(strDomain)
            objSite.Name = strDomain
            objSite.Applications(0).VirtualDirectories(0).PhysicalPath = "E:\hosting\" & strDomain

            svrMgr.ApplicationPools.Add(strDomain)
            svrMgr.Sites(strDomain).Applications(0).ApplicationPoolName = strDomain

            appPool = svrMgr.ApplicationPools(strDomain)
            appPool.ManagedPipelineMode = ManagedPipelineMode.Integrated

            svrMgr.CommitChanges()

        Catch ex As Exception
            MsgBox("2 " & ex.Message)
        End Try

        Try

            Dim objConnOptions As ConnectionOptions
            objConnOptions = New ConnectionOptions()
            objConnOptions.Impersonation = ImpersonationLevel.Impersonate
            objConnOptions.Authentication = AuthenticationLevel.Connect
            objConnOptions.EnablePrivileges = True

            Dim objOptions As New ObjectGetOptions(Nothing, System.TimeSpan.MaxValue, True)
            Dim strIP() As String = {"109", "228", "6", "9"}
            Dim strMasterServers() As String = {"109.228.6.9"}
            Dim strNotifyServers() As String = {"212.227.52.140"}
            Dim strSecondaryServers() As String = {"212.227.52.140"}
            Dim objMgmntSearch As ManagementObjectSearcher
            Dim objMgmntCollection As ManagementObjectCollection
            Dim objMgmntObject As ManagementObject
            Dim objBind As Binding

            'objOptions.Timeout = System.TimeSpan.Parse("5000")
            Try
                Dim objMgmntScope As ManagementScope
                objMgmntScope = New ManagementScope("\\.\Root\MicrosoftDNS")
                objMgmntScope.Options = objConnOptions
                objMgmntScope.Connect()

                Try

                    Dim objMgmntPath = New ManagementPath("MicrosoftDNS_Zone")
                    Dim DnsZoneClass = New ManagementClass(objMgmntScope, objMgmntPath, objOptions)

                    Try
                        ' Obtain [in] parameters for the method
                        Dim inParamsZone As ManagementBaseObject = DnsZoneClass.GetMethodParameters("CreateZone")

                        inParamsZone("ZoneName") = strDomain
                        inParamsZone("ZoneType") = 0
                        inParamsZone("DsIntegrated") = 0
                        inParamsZone("IpAddr") = strIP

                        Dim outParamsZone = DnsZoneClass.InvokeMethod("CreateZone", inParamsZone, Nothing)

                        Try

                            objMgmntPath = New ManagementPath("MicrosoftDNS_AType")

                            Dim DnsARecord = New ManagementClass(objMgmntScope, objMgmntPath, objOptions)

                            inParamsZone = Nothing

                            inParamsZone = DnsARecord.GetMethodParameters("CreateInstanceFromPropertyData")

                            inParamsZone("DnsServerName") = "109.228.6.9"
                            inParamsZone("ContainerName") = strDomain
                            inParamsZone("OwnerName") = strDomain
                            inParamsZone("IPAddress") = "109.228.6.9"

                            outParamsZone = DnsARecord.InvokeMethod("CreateInstanceFromPropertyData", inParamsZone, Nothing)

                            '''''''''''''''''''

                            inParamsZone = Nothing

                            objMgmntPath = New ManagementPath("MicrosoftDNS_ResourceRecord")
                            inParamsZone = DnsARecord.GetMethodParameters("CreateInstanceFromTextRepresentation")

                            inParamsZone("DnsServerName") = "109.228.6.9"
                            inParamsZone("ContainerName") = strDomain
                            inParamsZone("TextRepresentation") = "www." & strDomain & " IN A 109.228.6.9"

                            outParamsZone = DnsARecord.InvokeMethod("CreateInstanceFromTextRepresentation", inParamsZone, Nothing)

                            '''''''''''''''''''

                            inParamsZone = Nothing

                            objMgmntPath = New ManagementPath("MicrosoftDNS_ResourceRecord")
                            inParamsZone = DnsARecord.GetMethodParameters("CreateInstanceFromTextRepresentation")

                            inParamsZone("DnsServerName") = "109.228.6.9"
                            inParamsZone("ContainerName") = strDomain
                            inParamsZone("TextRepresentation") = "mail." & strDomain & " IN A 109.228.6.9"

                            outParamsZone = DnsARecord.InvokeMethod("CreateInstanceFromTextRepresentation", inParamsZone, Nothing)


                            '''''''''''''''''''

                            inParamsZone = Nothing

                            objMgmntPath = New ManagementPath("MicrosoftDNS_ResourceRecord")
                            inParamsZone = DnsARecord.GetMethodParameters("CreateInstanceFromTextRepresentation")

                            inParamsZone("DnsServerName") = "109.228.6.9"
                            inParamsZone("ContainerName") = strDomain
                            inParamsZone("TextRepresentation") = strDomain & " IN MX 10 mail." & strDomain

                            outParamsZone = DnsARecord.InvokeMethod("CreateInstanceFromTextRepresentation", inParamsZone, Nothing)

                            ''''''''''''''''''

                            'TRY: "select * from MicrosoftDNS_Zone where containername = '" + domain + "';

                            Try
                                objMgmntSearch = New ManagementObjectSearcher("select * from microsoftdns_NStype where containername = '" & strDomain & "'")
                                objMgmntSearch.Scope = objMgmntScope
                                objMgmntCollection = objMgmntSearch.Get

                                For Each objMgmntObject In objMgmntCollection
                                    '  MsgBox("NS")
                                    'objMgmntObject.Delete()
                                Next

                                objMgmntSearch = New ManagementObjectSearcher("select * from MicrosoftDNS_Zone where containername = '" & strDomain & "'")
                                objMgmntSearch.Scope = objMgmntScope
                                objMgmntCollection = objMgmntSearch.Get

                                If objMgmntCollection.Count > 0 Then
                                    MsgBox("ZONE EXISTS")
                                End If

                                For Each objMgmntObject In objMgmntCollection
                                    'objMgmntObject.Delete()
                                Next

                                objMgmntSearch = New ManagementObjectSearcher("select * from microsoftdns_Atype where containername = '" & strDomain & "' and ownername = 'www." & strDomain & "'")
                                objMgmntSearch.Scope = objMgmntScope
                                objMgmntCollection = objMgmntSearch.Get

                                For Each objMgmntObject In objMgmntCollection
                                    ' MsgBox("A")
                                    'objMgmntObject.Delete()
                                Next

                                objMgmntSearch = New ManagementObjectSearcher("select * from microsoftdns_Atype where containername = '" & strDomain & "' and ownername = '" & strDomain & "'")
                                objMgmntSearch.Scope = objMgmntScope
                                objMgmntCollection = objMgmntSearch.Get

                                For Each objMgmntObject In objMgmntCollection
                                    ' MsgBox("A")
                                    ' objMgmntObject.Delete()
                                Next

                                Try

                                    Dim strSerial As String
                                    If Date.Now.Day.ToString.Length - 1 Then
                                        strSerial = Date.Now.Year.ToString & Date.Now.Month.ToString & "0" & Date.Now.Day.ToString & "01"
                                    Else
                                        strSerial = Date.Now.Year.ToString & Date.Now.Month.ToString & Date.Now.Day.ToString & "01"
                                    End If

                                    inParamsZone = Nothing

                                    objMgmntPath = New ManagementPath("MicrosoftDNS_ResourceRecord")
                                    inParamsZone = DnsARecord.GetMethodParameters("CreateInstanceFromTextRepresentation")

                                    inParamsZone("DnsServerName") = "109.228.6.9"
                                    inParamsZone("ContainerName") = strDomain
                                    inParamsZone("TextRepresentation") = strDomain & " IN SOA ns1.recallhosting.co.uk. domains.recallhosting.co.uk. (" & strSerial & " 172800 900 1209600 3600)"

                                    outParamsZone = DnsARecord.InvokeMethod("CreateInstanceFromTextRepresentation", inParamsZone, Nothing)

                                Catch ex As Exception
                                    MsgBox("CREATE SOA " & ex.Message)
                                End Try

                            Catch ex As Exception
                                MsgBox("DELETE SOA " & ex.Message)
                            End Try

                            inParamsZone = Nothing

                            objMgmntPath = New ManagementPath("MicrosoftDNS_ResourceRecord")
                            inParamsZone = DnsARecord.GetMethodParameters("CreateInstanceFromTextRepresentation")

                            inParamsZone("DnsServerName") = "109.228.6.9"
                            inParamsZone("ContainerName") = strDomain
                            inParamsZone("TextRepresentation") = strDomain & " IN NS ns1.recallhosting.co.uk."

                            outParamsZone = DnsARecord.InvokeMethod("CreateInstanceFromTextRepresentation", inParamsZone, Nothing)

                            '''''''''''''''''''

                            inParamsZone = Nothing

                            objMgmntPath = New ManagementPath("MicrosoftDNS_ResourceRecord")
                            inParamsZone = DnsARecord.GetMethodParameters("CreateInstanceFromTextRepresentation")

                            inParamsZone("DnsServerName") = "109.228.6.9"
                            inParamsZone("ContainerName") = strDomain
                            inParamsZone("TextRepresentation") = strDomain & " IN NS ns2.recallhosting.co.uk."

                            outParamsZone = DnsARecord.InvokeMethod("CreateInstanceFromTextRepresentation", inParamsZone, Nothing)

                            '''''''''''''''''''

                            Try
                                'Dim RegClass As ManagementClass

                                'objMgmntScope = New ManagementScope("\\.\Root\Default")
                                'objMgmntScope.Connect()

                                'objMgmntPath = New ManagementPath("StdRegProv")
                                'RegClass = New ManagementClass(objMgmntScope, objMgmntPath, objOptions)

                                'MsgBox("nofify")

                                'Dim inParams As ManagementBaseObject = RegClass.GetMethodParameters("SetMultiStringValue")
                                'inParams("hDefKey") = 2147483650
                                'inParams("sSubKeyName") = "SOFTWARE\Microsoft\Windows NT\CurrentVersion\DNS Server\Zones\" & strDomain
                                'inParams("sValueName") = "NotifyServers"
                                'inParams("sValue") = strNotifyServers
                                'outParamsZone = RegClass.InvokeMethod("SetMultiStringValue", inParams, Nothing)

                                'inParams = RegClass.GetMethodParameters("SetMultiStringValue")
                                'inParams("hDefKey") = 2147483650
                                'inParams("sSubKeyName") = "SOFTWARE\Microsoft\Windows NT\CurrentVersion\DNS Server\Zones\" & strDomain
                                'inParams("sValueName") = "SecondaryServers"
                                'inParams("sValue") = strSecondaryServers
                                'outParamsZone = RegClass.InvokeMethod("SetMultiStringValue", inParams, Nothing)

                                'inParams = RegClass.GetMethodParameters("SetDWORDValue")
                                'inParams("hDefKey") = 2147483650
                                'inParams("sSubKeyName") = "SOFTWARE\Microsoft\Windows NT\CurrentVersion\DNS Server\Zones\" & strDomain
                                'inParams("sValueName") = "AllowUpdate"
                                'inParams("uValue") = &H0
                                'outParamsZone = RegClass.InvokeMethod("SetDWORDValue", inParams, Nothing)

                                'inParams = RegClass.GetMethodParameters("SetDWORDValue")
                                'inParams("hDefKey") = 2147483650
                                'inParams("sSubKeyName") = "SOFTWARE\Microsoft\Windows NT\CurrentVersion\DNS Server\Zones\" & strDomain
                                'inParams("sValueName") = "NoRefreshInterval"
                                'inParams("uValue") = &HA8
                                'outParamsZone = RegClass.InvokeMethod("SetDWORDValue", inParams, Nothing)

                                'inParams = RegClass.GetMethodParameters("SetDWORDValue")
                                'inParams("hDefKey") = 2147483650
                                'inParams("sSubKeyName") = "SOFTWARE\Microsoft\Windows NT\CurrentVersion\DNS Server\Zones\" & strDomain
                                'inParams("sValueName") = "RefreshInterval"
                                'inParams("uValue") = &HA8
                                'outParamsZone = RegClass.InvokeMethod("SetDWORDValue", inParams, Nothing)

                                'inParams = RegClass.GetMethodParameters("SetDWORDValue")
                                'inParams("hDefKey") = 2147483650
                                'inParams("sSubKeyName") = "SOFTWARE\Microsoft\Windows NT\CurrentVersion\DNS Server\Zones\" & strDomain
                                'inParams("sValueName") = "NotifyLevel"
                                'inParams("uValue") = &H2
                                'outParamsZone = RegClass.InvokeMethod("SetDWORDValue", inParams, Nothing)

                                'inParams = RegClass.GetMethodParameters("SetDWORDValue")
                                'inParams("hDefKey") = 2147483650
                                'inParams("sSubKeyName") = "SOFTWARE\Microsoft\Windows NT\CurrentVersion\DNS Server\Zones\" & strDomain
                                'inParams("sValueName") = "SecureSecondaries"
                                'inParams("uValue") = &H2
                                'outParamsZone = RegClass.InvokeMethod("SetDWORDValue", inParams, Nothing)

                                'inParams = RegClass.GetMethodParameters("SetDWORDValue")
                                'inParams("hDefKey") = 2147483650
                                'inParams("sSubKeyName") = "SOFTWARE\Microsoft\Windows NT\CurrentVersion\DNS Server\Zones\" & strDomain
                                'inParams("sValueName") = "Aging"
                                'inParams("uValue") = &H0
                                'outParamsZone = RegClass.InvokeMethod("SetDWORDValue", inParams, Nothing)

                                Try


                                    objMgmntSearch = New ManagementObjectSearcher("select * from MicrosoftDNS_Zone where containername = '" & strDomain & "'")
                                    objMgmntSearch.Scope = objMgmntScope
                                    objMgmntCollection = objMgmntSearch.Get

                                    If objMgmntCollection.Count > 0 Then
                                        MsgBox("ZONE EXISTS")
                                    End If

                                    inParamsZone = Nothing


                                    For Each objMgmntObject In objMgmntCollection
                                        ' MsgBox("A")
                                        'objMgmntObject.InvokeMethod("ReloadZone", Nothing)
                                        inParamsZone = objMgmntObject.GetMethodParameters("ResetSecondaries")
                                        inParamsZone("SecondaryServers") = strSecondaryServers
                                        inParamsZone("SecureSecondaries") = 2
                                        inParamsZone("NotifyServers") = strNotifyServers
                                        inParamsZone("Notify") = 2
                                        objMgmntObject.InvokeMethod("ResetSecondaries", inParamsZone, Nothing)
                                    Next

                                    Try

                                        Try


                                            svrMgr = New ServerManager()
                                            objSite = svrMgr.Sites(strDomain)


                                            If objSite Is Nothing Then
                                                MsgBox("SITE NOT EXIST")
                                            Else
                                                MsgBox("SITE EXISTS")
                                            End If

                                            objBind = objSite.Bindings.CreateElement()
                                            objBind.Protocol = "https"
                                            objBind.BindingInformation = "109.228.6.9:441:subdomain." & strDomain
                                            objSite.Bindings.Add(objBind)
                                            svrMgr.CommitChanges()

                                        Catch ex As Exception

                                            MsgBox("add 2nd binding " & ex.Message)

                                        End Try

                                        Try


                                            svrMgr = New ServerManager()
                                            objSite = svrMgr.Sites("h" & strDomain)

                                            If objSite Is Nothing Then
                                                MsgBox("SITE NOT EXIST")
                                            Else
                                                MsgBox("SITE EXISTS")
                                            End If

                                        Catch ex As Exception

                                            MsgBox("wrong domain " & ex.Message)

                                        End Try

                                        Try
                                            svrMgr = New ServerManager()
                                            objSite = svrMgr.Sites(strDomain)
                                            'svrMgr.Sites(strdomain).Bindings.Add("*.443:",Byte(), "MyCert");

                                            For Each binding In objSite.Bindings
                                                If binding.BindingInformation = "109.228.6.9:80:" & strDomain Then
                                                    binding.Delete()
                                                End If
                                            Next

                                            svrMgr.CommitChanges()


                                        Catch ex As Exception

                                            MsgBox("remove binding " & ex.Message)

                                        End Try

                                        Try

                                            svrMgr = New ServerManager()
                                            objSite = svrMgr.Sites(strDomain)
                                            svrMgr.Sites.Remove(objSite)
                                            svrMgr.CommitChanges()

                                        Catch ex As Exception

                                            MsgBox("Delete site " & ex.Message)

                                        End Try

                                        Try
                                            objMgmntSearch = New ManagementObjectSearcher("SELECT * FROM MicrosoftDNS_Zone WHERE ContainerName='" & strDomain & "'")
                                            objMgmntSearch.Scope = objMgmntScope
                                            objMgmntCollection = objMgmntSearch.Get
                                            For Each objMgmntObject In objMgmntCollection
                                                ' MsgBox("A")
                                                objMgmntObject.Delete()
                                            Next
                                        Catch ex As Exception
                                            MsgBox("DELETE ZONE " & ex.Message)
                                        End Try

                                        objConnOptions = New ConnectionOptions()
                                        objConnOptions.Impersonation = ImpersonationLevel.Impersonate
                                        objConnOptions.Authentication = AuthenticationLevel.Connect
                                        objConnOptions.EnablePrivileges = True
                                        objConnOptions.Username = "domaintransfer"
                                        objConnOptions.Password = "testABC1234"

                                        objMgmntScope = New ManagementScope("\\212.227.52.140\Root\MicrosoftDNS")
                                        objMgmntScope.Options = objConnOptions
                                        objMgmntScope.Connect()

                                        objMgmntPath = New ManagementPath("MicrosoftDNS_Zone")
                                        DnsZoneClass = New ManagementClass(objMgmntScope, objMgmntPath, objOptions)

                                        inParamsZone = DnsZoneClass.GetMethodParameters("CreateZone")

                                        inParamsZone("ZoneName") = strDomain
                                        inParamsZone("ZoneType") = 1
                                        inParamsZone("DsIntegrated") = 0
                                        inParamsZone("IpAddr") = strIP

                                        outParamsZone = DnsZoneClass.InvokeMethod("CreateZone", inParamsZone, Nothing)

                                    Catch ex As Exception
                                        MsgBox("REMOTE SETUP " & ex.Message)
                                    End Try

                                Catch ex As Exception
                                    MsgBox("RESTART ZONE " & ex.Message)
                                End Try

                            Catch ex As Exception
                                MsgBox("EDIT ZONE TRANSFER " & ex.Message)
                            End Try

                        Catch ex As Exception
                            MsgBox("CREATE A " & ex.Message)
                        End Try

                    Catch ex As Exception
                        MsgBox("CREATE ZONE " & ex.Message)
                    End Try


                Catch ex As Exception
                    MsgBox("Class " & ex.Message)
                End Try
            Catch ex As Exception
                MsgBox("scope " & ex.Message)
            End Try
        Catch ex As Exception
            MsgBox("SETUP " & ex.Message)
        End Try

        ''set folder permissions
        Try
            Shell("ICACLS E:\hosting\" & strDomain & " /grant ""IIS AppPool\" & strDomain & """:M ")
            Shell("ICACLS E:\hosting\" & strDomain & " /grant ""IIS AppPool\" & strDomain & """:RX ")
            Shell("ICACLS E:\hosting\" & strDomain & " /grant ""IIS AppPool\" & strDomain & """:R ")
            Shell("ICACLS E:\hosting\" & strDomain & " /grant ""IIS AppPool\" & strDomain & """:W ")

        Catch ex As Exception
            MsgBox("5 " & ex.Message)
        End Try

    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

End Class
