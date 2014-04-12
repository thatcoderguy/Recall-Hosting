Imports System.Data.SqlClient
Imports System.Net.Mail
Imports System.IO
Imports MailEnable.Administration
Imports System.ServiceProcess
Imports System.Management

Partial Class Email
    Inherits System.Web.UI.Page
    Private strEmailName As String
    Private strDomainName As String
    Private intMailboxSize As String

    Protected Sub btnLinkChangePasswd_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnLinkChangePasswd.Click

        btnLinkChangePasswd.ImageUrl = "images/changepassword2.GIF"
        btnLinkForwarder.ImageUrl = "images/forwarder2.GIF"
        btnLinkRedirector.ImageUrl = "images/redirector2.GIF"
        btnLinkRetreiver.ImageUrl = "images/popretriever.GIF"
        btnLinkAutoResponder.ImageUrl = "images/autoresponder2.GIF"
        btnLinkMailboxSize.ImageUrl = "images/mailboxspace2.GIF"
        btnLinkAliasAddress.imageurl = "images/addaliasaddress2.gif"

        mailboxspace.Visible = False
        Redirector.Visible = False
        ChangePasswd.Visible = False
        Forwarder.Visible = False
        Retriever.Visible = True
        AutoResponder.Visible = False
        aliasaddresses.visible = False

        successmsg.InnerHtml = ""

    End Sub

    Protected Sub btnLinkRetreiver_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnLinkRetreiver.Click

        btnLinkChangePasswd.ImageUrl = "images/changepassword2.GIF"
        btnLinkForwarder.ImageUrl = "images/forwarder2.GIF"
        btnLinkRedirector.ImageUrl = "images/redirector2.GIF"
        btnLinkRetreiver.ImageUrl = "images/popretriever.GIF"
        btnLinkAutoResponder.ImageUrl = "images/autoresponder2.GIF"
        btnLinkMailboxSize.ImageUrl = "images/mailboxspace2.GIF"
        btnLinkAliasAddress.imageurl = "images/addaliasaddress2.gif"

        mailboxspace.Visible = False
        Redirector.Visible = False
        ChangePasswd.Visible = False
        Forwarder.Visible = False
        Retriever.Visible = True
        AutoResponder.Visible = False
        aliasaddresses.visible = False

        successmsg.InnerHtml = ""

    End Sub

    Protected Sub btnLinkRedirector_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnLinkRedirector.Click

        btnLinkChangePasswd.ImageUrl = "images/changepassword2.GIF"
        btnLinkForwarder.ImageUrl = "images/forwarder2.GIF"
        btnLinkRedirector.ImageUrl = "images/redirector.GIF"
        btnLinkRetreiver.ImageUrl = "images/popretriever2.GIF"
        btnLinkAutoResponder.ImageUrl = "images/autoresponder2.GIF"
        btnLinkMailboxSize.ImageUrl = "images/mailboxspace2.GIF"
        btnLinkAliasAddress.imageurl = "images/addaliasaddress2.gif"

        mailboxspace.Visible = False
        Redirector.Visible = True
        ChangePasswd.Visible = False
        Forwarder.Visible = False
        Retriever.Visible = False
        AutoResponder.Visible = False
        aliasaddresses.visible = False

        successmsg.InnerHtml = ""

    End Sub

    Protected Sub btnLinkForwarder_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnLinkForwarder.Click

        btnLinkChangePasswd.ImageUrl = "images/changepassword2.GIF"
        btnLinkForwarder.ImageUrl = "images/forwarder2.GIF"
        btnLinkRedirector.ImageUrl = "images/redirector2.GIF"
        btnLinkRetreiver.ImageUrl = "images/popretriever.GIF"
        btnLinkAutoResponder.ImageUrl = "images/autoresponder2.GIF"
        btnLinkMailboxSize.ImageUrl = "images/mailboxspace2.GIF"
        btnLinkAliasAddress.imageurl = "images/addaliasaddress2.gif"

        mailboxspace.Visible = False
        Redirector.Visible = False
        ChangePasswd.Visible = False
        Forwarder.Visible = False
        Retriever.Visible = True
        AutoResponder.Visible = False
        aliasaddresses.visible = False

        successmsg.InnerHtml = ""

    End Sub

    Protected Sub btnLinkChangePasswd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnChangePasswd.Click


        Dim oCon As New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
        Dim oCom As New SqlCommand

        If txtConfirm.Text = txtPassword.Text Then

            oCon.Open()

            ''sp_createcommand
            ''change password
            oCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','change mailbox password','" & strEmailName & "','" & txtPassword.Text & "','','','','','','','','','','','','','','','','';"
            oCom.Connection = oCon

            oCom.ExecuteNonQuery()

            oCom.Dispose()
            oCon.Close()
            oCon.Dispose()

            ''SEND EMAIL
            successmsg.InnerHtml = "Password changed"
            successmsg.Style.Add("color", "#00ff00")

            Dim SmtpServer As New SmtpClient()
            Dim mail As New MailMessage()
                                SmtpServer.Credentials = New  _
Net.NetworkCredential("EMAILUSERNAME", "PASSWORD")
            SmtpServer.Port = 25
            SmtpServer.Host = "smtp.DOMAIN"
            mail = New MailMessage()
            mail.From = New MailAddress("EMAILADDRESS")
            mail.Subject = "Password Changed"
            mail.Body = "Mailbox: " & strEmailName & "@" & strDomainName & "  Password: " & txtPassword.Text
            SmtpServer.Send(mail)

        Else

            successmsg.InnerHtml = "Passwords must match"
            successmsg.Style.Add("color", "#ff0000")

        End If

    End Sub

    Protected Sub btnRetriver_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRetriver.Click
        Dim objCon As New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        Dim objCom As New SqlCommand
        Dim intEmailID, intDomainID As Integer
        Dim objRS As SqlDataReader
        intEmailID = CInt(Request.QueryString("emailID"))
        intDomainID = CInt(Request.QueryString("domainID"))

        objCon.Open()

        objCom.CommandText = "sp_createpopretriever " & intEmailID.ToString & ",'" & txtPOPAddress.Text & "','" & txtPOPUsername.Text & "','" & txtPOPPassword.Text & "'," & CInt(dlRedirectionStatus.SelectedValue).ToString
        objCom.Connection = objCon
        objCom.ExecuteNonQuery()


        ''get forwarders
        objCom.CommandText = "sp_getpopretrievers " & intEmailID.ToString & "," & intDomainID.ToString
        objCom.Connection = objCon

        objRS = objCom.ExecuteReader

        If objRs.HasRows Then

            Repeater1.DataSource = objRS
            Repeater1.DataBind()

        End If

        objRs.Close()
        objCom.Dispose()
        objCon.Close()
        objCon.Dispose()


        objCon = New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
        objCom = New SqlCommand

        objCon.Open()

        ''sp_createcommand
        objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','add pop connector','" & strEmailName & "','" & txtPOPUsername.Text & "','" & txtPOPPassword.Text & "','" & txtPOPAddress.Text & "','" & dlStatus.SelectedValue & "','110','0','','','','','','','','','','','';"
        objCom.Connection = objCon

        objCom.ExecuteNonQuery()

        objCom.Dispose()
        objCon.Close()
        objCon.Dispose()

        ''SEND EMAIL
        Dim SmtpServer As New SmtpClient()
        Dim mail As New MailMessage()
                            SmtpServer.Credentials = New  _
Net.NetworkCredential("EMAILUSERNAME", "PASSWORD")
        SmtpServer.Port = 25
        SmtpServer.Host = "smtp.DOMAIN"
        mail = New MailMessage()
        mail.From = New MailAddress("EMAILADDRESS")
        mail.Subject = "New POP Connector"
        mail.Body = "Mailbox: " & strEmailName & "@" & strDomainName & "  POP Server: " & txtPOPAddress.Text & "  Username:" & txtPOPUsername.Text & "  Password:" & txtPOPPassword.Text
        SmtpServer.Send(mail)

        successmsg.InnerHtml = "Redirector Added"
        successmsg.Style.Add("color", "#00ff00")


        successmsg.InnerHtml = "Retriever Added"
        successmsg.Style.Add("color", "#00ff00")


    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Repeater1.DataSource = Nothing
        Repeater2.DataSource = Nothing
        Repeater3.DataSource = Nothing
        Repeater1.DataBind()
        Repeater2.DataBind()
        Repeater3.DataBind()
        Dim intExtraParam, intEmailID, intDomainID As Integer
        Dim strSQL As String
        intEmailID = CInt(Request.QueryString("emailID"))
        intDomainID = CInt(Request.QueryString("domainID"))
        strSQL = ""
        intExtraParam = 0

        Dim objCon As New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        Dim objCom As New SqlCommand
        Dim objRS As SqlDataReader

        objCon.Open()

        If Not strSQL = "" Then
            objCom.CommandText = strSQL
            objCom.Connection = objCon
            objCom.ExecuteNonQuery()
        End If

        ''''''''get email address and valid IDs
        objCom.CommandText = "sp_getemailaddress " & intEmailID.ToString & "," & intDomainID.ToString
        objCom.Connection = objCon

        objRS = objCom.ExecuteReader

        If objRS.HasRows Then

            objRS.Read()
            emailbox.InnerHtml = "Email Box: " & objRS.Item("emailAddress")
            strEmailName = Split(objRS.Item("emailAddress"), "@")(0)
            strDomainName = Split(objRS.Item("emailAddress"), "@")(1)
            intMailboxSize = objRS.Item("emailSize")

        Else

            Response.Redirect("emails.aspx?domainID=" & intDomainID.ToString & "&amp;Msg=ForwarderDeleted")


        End If

        objRS.Close()
        objCom.Dispose()
        objCon.Close()
        objCon.Dispose()

        objCon = New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
        objCom = New SqlCommand
        objCon.Open()

        If Not Request.QueryString("ForwarderID") = "" Then

            intExtraParam = CInt(Request.QueryString("ForwarderID"))
            strSQL = "sp_deleteforwarder " & intEmailID.ToString & "," & intExtraParam.ToString

            btnLinkChangePasswd.ImageUrl = "images/changepassword2.GIF"
            btnLinkForwarder.ImageUrl = "images/forwarder.GIF"
            btnLinkRedirector.ImageUrl = "images/redirector2.GIF"
            btnLinkRetreiver.ImageUrl = "images/popretriever2.GIF"

            Redirector.Visible = False
            ChangePasswd.Visible = False
            Forwarder.Visible = True
            Retriever.Visible = False

            ''delete forwarder
            ''sp_createcommand
            objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','delete forwarder','" & strEmailName & "','" & Request.QueryString("ForwarderDest") & "','','','','','','','','','','','','','','','','';"
            objCom.Connection = objCon

            objCom.ExecuteNonQuery()

            objCom.Dispose()
            objCon.Close()
            objCon.Dispose()

            ''SEND EMAIL
            Dim SmtpServer As New SmtpClient()
            Dim mail As New MailMessage()
                                SmtpServer.Credentials = New  _
Net.NetworkCredential("EMAILUSERNAME", "PASSWORD")
            SmtpServer.Port = 25
            SmtpServer.Host = "smtp.DOMAIN"
            mail = New MailMessage()
            mail.From = New MailAddress("EMAILADDRESS")
            mail.Subject = "Forwarder Deleted"
            mail.Body = "Mailbox: " & strEmailName & "@" & strDomainName & "  Forwarder: " & Request.QueryString("ForwarderDest")
            SmtpServer.Send(mail)

            successmsg.InnerHtml = "Forwarder Deleted"
            successmsg.Style.Add("color", "#ff0000")

        ElseIf Not Request.QueryString("RedirectorID") = "" Then

            intExtraParam = CInt(Request.QueryString("RedirectorID"))
            strSQL = "sp_deleteredirector " & intEmailID.ToString & "," & intExtraParam.ToString

            btnLinkChangePasswd.ImageUrl = "images/changepassword2.GIF"
            btnLinkForwarder.ImageUrl = "images/forwarder2.GIF"
            btnLinkRedirector.ImageUrl = "images/redirector.GIF"
            btnLinkRetreiver.ImageUrl = "images/popretriever2.GIF"

            Redirector.Visible = True
            ChangePasswd.Visible = False
            Forwarder.Visible = False
            Retriever.Visible = False

            ''sp_createcommand
            objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','delete redirector','" & strEmailName & "','" & Request.QueryString("RedirectorDest") & "','" & CInt(Request.QueryString("Status")).ToString & "','','','','','','','','','','','','','','','';"
            objCom.Connection = objCon

            objCom.ExecuteNonQuery()

            objCom.Dispose()
            objCon.Close()
            objCon.Dispose()

            ''SEND EMAIL
            Dim SmtpServer As New SmtpClient()
            Dim mail As New MailMessage()
                                SmtpServer.Credentials = New  _
Net.NetworkCredential("EMAILUSERNAME", "PASSWORD")
            SmtpServer.Port = 25
            SmtpServer.Host = "smtp.DOMAIN"
            mail = New MailMessage()
            mail.From = New MailAddress("EMAILADDRESS")
            mail.Subject = "Redirector Deleted"
            mail.Body = "Mailbox: " & strEmailName & "@" & strDomainName & "  Redirector: " & Request.QueryString("RedirectorDest")
            SmtpServer.Send(mail)

            successmsg.InnerHtml = "Redirector Deleted"
            successmsg.Style.Add("color", "#ff0000")

        ElseIf Not Request.QueryString("RetrieverID") = "" Then

            intExtraParam = CInt(Request.QueryString("RetrieverID"))

            strSQL = "sp_getpopretriever " & intEmailID.ToString & "," & intExtraParam.ToString

            objCom = New SqlCommand
            objCon = New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
            objCon.Open()
            objCom.Connection = objCon
            objCom.CommandText = strSQL

            Dim strMailServer, strUsername, strPassword, intStatus As String

            objRS = objCom.ExecuteReader

            If objRS.HasRows Then
                objRS.Read()

                strMailServer = objRS("popAddress")
                strUsername = objRS("popUsername")
                strPassword = objRS("popPassword")
                intStatus = objRS("status")

                objRS.Close()

                strSQL = "sp_deletepopretriever " & intEmailID.ToString & "," & intExtraParam.ToString

                objCom.CommandText = strSQL
                objCom.ExecuteNonQuery()

                objCon.Close()

                btnLinkChangePasswd.ImageUrl = "images/changepassword2.GIF"
                btnLinkForwarder.ImageUrl = "images/forwarder2.GIF"
                btnLinkRedirector.ImageUrl = "images/redirector2.GIF"
                btnLinkRetreiver.ImageUrl = "images/popretriever.GIF"

                Redirector.Visible = False
                ChangePasswd.Visible = False
                Forwarder.Visible = False
                Retriever.Visible = True

                ''delete retriever
                objCon = New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
                objCom = New SqlCommand

                objCon.Open()

                ''sp_createcommand
                objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','remove pop connector','" & strEmailName & "','" & strUsername & "','" & strPassword & "','" & strMailServer & "','" & intStatus & "','110','0','','','','','','','','','','','';"
                objCom.Connection = objCon

                objCom.ExecuteNonQuery()

                objCom.Dispose()
                objCon.Close()
                objCon.Dispose()

                ''SEND EMAIL
                Dim SmtpServer As New SmtpClient()
                Dim mail As New MailMessage()
                                    SmtpServer.Credentials = New  _
Net.NetworkCredential("EMAILUSERNAME", "PASSWORD")
                SmtpServer.Port = 25
                SmtpServer.Host = "smtp.DOMAIN"
                mail = New MailMessage()
                mail.From = New MailAddress("EMAILADDRESS")
                mail.Subject = "POP Retriever Deleted"
                mail.Body = "Mailbox: " & strEmailName & "@" & strDomainName & "  Redirector: " & Request.QueryString("RedirectorDest")
                SmtpServer.Send(mail)

                successmsg.InnerHtml = "Retreiver Deleted"
                successmsg.Style.Add("color", "#ff0000")

            End If

        ElseIf Not Request.QueryString("aliasaddressid") = "" Then

            intExtraParam = CInt(Request.QueryString("aliasaddressid"))
            strSQL = "sp_deletealiasaddress " & CStr(CInt(Request.QueryString("domainID"))) & "," & intEmailID.ToString & "," & intExtraParam.ToString

            btnLinkChangePasswd.ImageUrl = "images/changepassword2.GIF"
            btnLinkForwarder.ImageUrl = "images/forwarder2.GIF"
            btnLinkRedirector.ImageUrl = "images/redirector2.GIF"
            btnLinkRetreiver.ImageUrl = "images/popretriever2.GIF"
            btnLinkAliasAddress.imageUrl = "images/addaliasaddress.GIF"

            Redirector.Visible = False
            ChangePasswd.Visible = False
            Forwarder.Visible = False
            Retriever.Visible = False
            aliasaddresses.visible = True

            ''sp_createcommand
            objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','remove alias address','" & strEmailName & "','" & Request.QueryString("aliasaddress") & "','','','','','','','','','','','','','','','','';"
            objCom.Connection = objCon

            objCom.ExecuteNonQuery()

            objCom.Dispose()
            objCon.Close()
            objCon.Dispose()

            ''SEND EMAIL
            Dim SmtpServer As New SmtpClient()
            Dim mail As New MailMessage()
                    SmtpServer.Credentials = New  _
            Net.NetworkCredential("EMAILUSERNAME", "PASSWORD")
            SmtpServer.Port = 25
            SmtpServer.Host = "smtp.DOMAIN"
            mail = New MailMessage()
            mail.From = New MailAddress("EMAILADDRESS")
            mail.Subject = "Alias Address Deleted"
            mail.Body = "Mailbox: " & strEmailName & "@" & strDomainName & "  Alias Address: " & Request.QueryString("aliasaddress")
            SmtpServer.Send(mail)

            successmsg.InnerHtml = "Alias Address Deleted"
            successmsg.Style.Add("color", "#ff0000")

        End If

        objCon = New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        objCom = New SqlCommand

        objCon.Open()

        If Not strSQL = "" Then
            objCom.CommandText = strSQL
            objCom.Connection = objCon
            objCom.ExecuteNonQuery()
        End If

        ''''''''get email address and valid IDs
        objCom.CommandText = "sp_getemailaddress " & intEmailID.ToString & "," & intDomainID.ToString
        objCom.Connection = objCon

        objRS = objCom.ExecuteReader

        If objRS.HasRows Then

            objRS.Read()
            emailbox.InnerHtml = "Email Box: " & objRS.Item("emailAddress")
            strEmailName = Split(objRS.Item("emailAddress"), "@")(0)
            strDomainName = Split(objRS.Item("emailAddress"), "@")(1)

            If objRS.Item("emailType") = "Forwarder" Then

                btnLinkForwarder.Enabled = True
                btnLinkForwarder.Visible = True
                btnLinkChangePasswd.Enabled = False
                btnLinkChangePasswd.Visible = False
                btnLinkRetreiver.Enabled = False
                btnLinkRetreiver.Visible = False
                btnLinkRedirector.Enabled = False
                btnLinkRedirector.Visible = False
                btnLinkAutoResponder.Visible = False
                btnLinkMailboxSize.Visible = False
                btnLinkAliasAddress.visible = False

                If intExtraParam = 0 Then

                    Redirector.Visible = False
                    ChangePasswd.Visible = False
                    Forwarder.Visible = True
                    Retriever.Visible = False
                    AutoResponder.Visible = False

                    btnLinkChangePasswd.ImageUrl = "images/changepassword2.GIF"
                    btnLinkForwarder.ImageUrl = "images/forwarder.GIF"
                    btnLinkRedirector.ImageUrl = "images/redirector2.GIF"
                    btnLinkRetreiver.ImageUrl = "images/popretriever2.GIF"
                    btnLinkAutoResponder.ImageUrl = "images/autoresponder2.GIF"

                End If


            ElseIf objRS.Item("emailType") = "Email" Then

                If intExtraParam = 0 Then

                    Redirector.Visible = False
                    ChangePasswd.Visible = True
                    Forwarder.Visible = False
                    Retriever.Visible = False
                    AutoResponder.Visible = False


                    btnLinkChangePasswd.ImageUrl = "images/changepassword.GIF"
                    btnLinkForwarder.ImageUrl = "images/forwarder2.GIF"
                    btnLinkRedirector.ImageUrl = "images/redirector2.GIF"
                    btnLinkRetreiver.ImageUrl = "images/popretriever2.GIF"
                    btnLinkAutoResponder.ImageUrl = "images/autoresponder2.GIF"

                End If

                btnLinkForwarder.Enabled = False
                btnLinkForwarder.Visible = False
                btnLinkChangePasswd.Enabled = True
                btnLinkChangePasswd.Visible = True
                btnLinkRetreiver.Enabled = True
                btnLinkRetreiver.Visible = True
                btnLinkRedirector.Enabled = True
                btnLinkRedirector.Visible = True
                btnLinkAutoResponder.Visible = True
                btnLinkMailboxSize.Visible = True
                btnLinkAliasAddress.visible = True

            End If

        Else

            message.InnerHtml = "Sorry, emailbox not found  <a href=""Default.aspx"">Go Back</a>"

        End If

        objRS.Close()

        ''get retreivers
        objCom.CommandText = "sp_getpopretrievers " & intEmailID.ToString & "," & intDomainID.ToString
        objCom.Connection = objCon

        objRS = objCom.ExecuteReader

        If objRS.HasRows Then

            Repeater1.DataSource = objRS
            Repeater1.DataBind()

        End If

        objRS.Close()

        ''get redirectors
        objCom.CommandText = "sp_getredirectdesinations " & intEmailID.ToString & "," & intDomainID.ToString
        objCom.Connection = objCon

        objRS = objCom.ExecuteReader

        If objRS.HasRows Then

            Repeater2.DataSource = objRS
            Repeater2.DataBind()

        End If

        objRS.Close()

        ''get redirectors
        objCom.CommandText = "sp_getredirectdesinations " & intEmailID.ToString & "," & intDomainID.ToString
        objCom.Connection = objCon

        objRS = objCom.ExecuteReader

        If objRS.HasRows Then

            Do While objRS.Read()
                redirectstatus.InnerHtml = "Current Redirect Status: " & Replace(Replace(objRS.Item("status"), "0", "Enabled - Do not keep copy"), "1", "Enabled - Keep copy on server")
            Loop


        End If

        objRS.Close()


        ''get forwarders
        objCom.CommandText = "sp_getforwarddestinations " & intEmailID.ToString & "," & intDomainID.ToString
        objCom.Connection = objCon

        objRS = objCom.ExecuteReader

        If objRS.HasRows Then

            Repeater3.DataSource = objRS
            Repeater3.DataBind()

        End If

        objRS.Close()



        ''get alias addresses
        objCom.CommandText = "sp_getaliasaddresses " & intDomainID.ToString & "," & intEmailID.ToString
        objCom.Connection = objCon

        objRS = objCom.ExecuteReader

        If objRS.HasRows Then

            rptAliasAddresses.DataSource = objRS
            rptAliasAddresses.DataBind()

        End If

        objRS.Close()
        objCom.Dispose()
        objCon.Close()
        objCon.Dispose()

    End Sub

    Protected Sub btnCreateForward_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreateForward.Click

        Dim objCon As New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        Dim objCom As New SqlCommand
        Dim intEmailID As Integer
        Dim intDomainID As Integer
        Dim objRs As SqlDataReader
        intEmailID = CInt(Request.QueryString("emailID"))
        intDomainID = CInt(Request.QueryString("domainID"))

        objCon.Open()

        objCom.CommandText = "sp_createforwarder " & intEmailID.ToString & ",'" & txtForwarderDestination.Text & "'"
        objCom.Connection = objCon
        objCom.ExecuteNonQuery()

        ''get forwarders
        objCom.CommandText = "sp_getforwarddestinations " & intEmailID.ToString & "," & intDomainID.ToString
        objCom.Connection = objCon

        objRS = objCom.ExecuteReader

        If objRs.HasRows Then

            Repeater3.DataSource = objRs
            Repeater3.DataBind()

        End If

        objRs.Close()
        objCom.Dispose()
        objCon.Close()
        objCon.Dispose()


        objCon = New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
        objCom = New SqlCommand

        objCon.Open()

        ''sp_createcommand
        objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','add forwarder','" & strEmailName & "','" & txtForwarderDestination.Text & "','','','','','','','','','','','','','','','','';"
        objCom.Connection = objCon

        objCom.ExecuteNonQuery()

        objCom.Dispose()
        objCon.Close()
        objCon.Dispose()

        ''SEND EMAIL
        Dim SmtpServer As New SmtpClient()
        Dim mail As New MailMessage()
                    SmtpServer.Credentials = New  _
        Net.NetworkCredential("EMAILUSERNAME", "PASSWORD")
        SmtpServer.Port = 25
        SmtpServer.Host = "smtp.DOMAIN"
        mail = New MailMessage()
        mail.From = New MailAddress("EMAILADDRESS")
        mail.Subject = "New Forwarder"
        mail.Body = "That wont earn me money! Mailbox: " & strEmailName & "@" & strDomainName & "  Destination: " & txtForwarderDestination.Text
        SmtpServer.Send(mail)

        successmsg.InnerHtml = "Forwarder Added"
        successmsg.Style.Add("color", "#00ff00")

    End Sub

    Protected Sub btnCreateRedirect_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreateRedirect.Click
        Dim objCon As New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        Dim objCom As New SqlCommand
        Dim objRS As SqlDataReader
        Dim intEmailID As Integer
        Dim intDomainID As Integer
        intEmailID = CInt(Request.QueryString("emailID"))
        intDomainID = CInt(Request.QueryString("domainID"))

        objCon.Open()

        objCom.CommandText = "sp_createredirector " & intEmailID.ToString & ",'" & txtRedirectDestination.Text & "'," & dlRedirectionStatus.SelectedValue
        objCom.Connection = objCon
        objCom.ExecuteNonQuery()

        ''get redirectors
        objCom.CommandText = "sp_getredirectdesinations " & intEmailID.ToString & "," & intDomainID.ToString
        objCom.Connection = objCon

        objRS = objCom.ExecuteReader

        If objRS.HasRows Then

            Repeater2.DataSource = objRS
            Repeater2.DataBind()

        End If

        objRS.Close()

        ''get redirectors
        objCom.CommandText = "sp_getredirectdesinations " & intEmailID.ToString & "," & intDomainID.ToString
        objCom.Connection = objCon

        objRS = objCom.ExecuteReader

        If objRS.HasRows Then

            Do While objRS.Read()
                redirectstatus.InnerHtml = "Current Redirect Status: " & Replace(Replace(objRS.Item("status"), "0", "Enabled - Do not keep copy"), "1", "Enabled - Keep copy on server")
            Loop


        End If

        objRS.Close()

        objCon.Close()
        objCom.Dispose()
        objCon.Dispose()

        objCon = New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
        objCom = New SqlCommand

        objCon.Open()

        ''sp_createcommand
        objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','add redirector','" & strEmailName & "','" & txtRedirectDestination.Text & "','" & dlRedirectionStatus.SelectedValue & "','','','','','','','','','','','','','','','';"
        objCom.Connection = objCon

        objCom.ExecuteNonQuery()

        objCom.Dispose()
        objCon.Close()
        objCon.Dispose()

        ''SEND EMAIL
        Dim SmtpServer As New SmtpClient()
        Dim mail As New MailMessage()
                    SmtpServer.Credentials = New  _
        Net.NetworkCredential("EMAILUSERNAME", "PASSWORD")
        SmtpServer.Port = 25
        SmtpServer.Host = "smtp.DOMAIN"
        mail = New MailMessage()
        mail.From = New MailAddress("EMAILADDRESS")
        mail.Subject = "New Redirector"
        mail.Body = "Mailbox: " & strEmailName & "@" & strDomainName & "  Destination: " & txtRedirectDestination.Text
        SmtpServer.Send(mail)

        successmsg.InnerHtml = "Redirector Added"
        successmsg.Style.Add("color", "#00ff00")

    End Sub

    Protected Sub btnLinkAutoResponder_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnLinkAutoResponder.Click

        btnLinkChangePasswd.ImageUrl = "images/changepassword2.GIF"
        btnLinkForwarder.ImageUrl = "images/forwarder2.GIF"
        btnLinkRedirector.ImageUrl = "images/redirector2.GIF"
        btnLinkRetreiver.ImageUrl = "images/popretriever2.GIF"
        btnLinkAutoResponder.ImageUrl = "images/autoresponder.GIF"
        btnLinkMailboxSize.ImageUrl = "images/mailboxspace2.GIF"

        Redirector.Visible = False
        ChangePasswd.Visible = False
        Forwarder.Visible = False
        Retriever.Visible = False
        AutoResponder.Visible = True
        mailboxspace.Visible = False
        
        successmsg.InnerHtml = ""

        Dim objCon As New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        Dim objCom As New SqlCommand
        Dim rs As SqlDataReader
        Dim intEmailID As Integer

        intEmailID = CInt(Request.QueryString("emailID"))

        objCon.Open()
        objCom.Connection = objCon
        objCom.CommandText = "sp_getAutoResponder " & intEmailID.ToString

        rs = objCom.ExecuteReader

        If rs.HasRows Then

            rs.Read()

            txtAutoResponderBody.Text = rs.Item("autoResponserBodyText")
            txtAutoResponderSubject.Text = rs.Item("autoResponderSubject")

            If rs.Item("autoResponderStatus") = "1" Then
                cbStatus.Checked = True
            Else
                cbStatus.Checked = False
            End If

        End If

        objCon.Close()
        objCom.Dispose()
        objCon.Dispose()

    End Sub

    Protected Sub btnUpdateAutoResponder_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateAutoResponder.Click

        Dim objCon As New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        Dim objCom As New SqlCommand
        Dim intEmailID As Integer

        intEmailID = CInt(Request.QueryString("emailID"))

        objCon.Open()
        objCom.Connection = objCon
        objCom.CommandText = "sp_alterAutoResponser " & intEmailID.ToString & ",'" & txtAutoResponderSubject.Text & "','" & txtAutoResponderBody.Text & "'," & cbStatus.Checked.ToString

        objCom.ExecuteNonQuery()

        objCon.Close()
        objCom.Dispose()
        objCon.Dispose()

        objCon = New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
        objCom = New SqlCommand

        objCon.Open()
        objCom.Connection = objCon

        If cbStatus.Checked Then
            objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','add autoresponder','" & strEmailName & "','" & txtAutoResponderSubject.Text & "','" & txtAutoResponderBody.Text & "','1','','','','','','','','','','','','','','';"
        Else
            objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','delete autoresponder','" & strEmailName & "','" & txtAutoResponderSubject.Text & "','" & txtAutoResponderBody.Text & "','0','','','','','','','','','','','','','','';"
        End If
        objCom.ExecuteNonQuery()

        objCon.Close()
        objCom.Dispose()
        objCon.Dispose()

        successmsg.InnerHtml = "Auto Responder Updated"

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnLinkMailboxSize_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnLinkMailboxSize.Click

        ''get latest size info
        Dim objCon As New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
        Dim objCom As New SqlCommand
        objCon.Open()

        objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','mailbox usage','" & strEmailName & "','<PUT A LOCAL PATH HERE>" & strEmailName & "-" & strDomainName & ".txt','','','','','','','','','','','','','','','','';"

        objCom.Connection = objCon
        objCom.ExecuteNonQuery()

        objCon.Close()
        objCom.Dispose()
        
        successmsg.InnerHtml = ""

        btnLinkChangePasswd.ImageUrl = "images/changepassword2.GIF"
        btnLinkForwarder.ImageUrl = "images/forwarder2.GIF"
        btnLinkRedirector.ImageUrl = "images/redirector2.GIF"
        btnLinkRetreiver.ImageUrl = "images/popretriever2.GIF"
        btnLinkAutoResponder.ImageUrl = "images/autoresponder2.GIF"
        btnLinkMailboxSize.ImageUrl = "images/mailboxspace.GIF"
        btnLinkAliasAddress.imageurl = "images/addaliasaddress2.gif"

        mailboxspace.Visible = True
        Redirector.Visible = False
        ChangePasswd.Visible = False
        Forwarder.Visible = False
        Retriever.Visible = False
        AutoResponder.Visible = False
        aliasaddresses.visible = False

        Dim objMailBox As New Mailbox
        objMailBox.Postoffice = strDomainName
        objMailBox.MailboxName = strEmailName
        objMailBox.Size = -3
        objMailBox.Limit = -1

        System.Threading.Thread.Sleep(2000)

        If objMailBox.GetMailbox = 1 Then

            Dim dblOnePercent As Double = CDbl(100) / CDbl(objMailBox.Limit.ToString)

            ''adjust image to represent size
            Dim dblPercent As Double
            dblPercent = (dblOnePercent * CDbl(objMailBox.Size / 1024)) * 3

            usage.Width = CStr(CInt(dblPercent))
            usage.Height = 21

            usagetext.InnerHtml = Replace(Replace(Replace("There are {X} emails in this mailbox, using {Y} MB of the {Z} MB allocated", "{X}", System.IO.Directory.GetFiles("D:\email\system\Mail Enable\Postoffices\" & strDomainName & "\MAILROOT\" & strEmailName & "\inbox", "*.mai").Length().ToString()), "{Y}", Decimal.Round(CDec(CDbl(objMailBox.Size / 1024)), 2).ToString), "{Z}", Decimal.Round(CDec(CDbl(objMailBox.Limit / 1024)), 2).ToString)

		if objMailBox.Limit.ToString="51200" then
			 radiobutton3.checked=true
		elseif objMailBox.Limit.ToString="1048576" then
			radiobutton4.checked=true
		elseif objMailBox.Limit.ToString="2097152" then
			radiobutton5.checked=true
		elseif objMailBox.Limit.ToString="5242880" then
			radiobutton6.checked=true
		elseif objMailBox.Limit.ToString="10485760" then
			radiobutton7.checked=true
		elseif objMailBox.Limit.ToString="15728640" then
			radiobutton8.checked=true
		elseif objMailBox.Limit.ToString="20971520" then
			radiobutton9.checked=true
		end if

        Else

            usage.Width = 100
            usage.Height = 21

            usagetext.InnerHtml = "Error retrieving mailbox size, please try again."

            radiobutton3.checked=true

        End If

    End Sub

    Protected Sub btnPurgeEmails_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPurgeEmails.Click

        btnLinkChangePasswd.ImageUrl = "images/changepassword2.GIF"
        btnLinkForwarder.ImageUrl = "images/forwarder2.GIF"
        btnLinkRedirector.ImageUrl = "images/redirector2.GIF"
        btnLinkRetreiver.ImageUrl = "images/popretriever2.GIF"
        btnLinkAutoResponder.ImageUrl = "images/autoresponder2.GIF"
        btnLinkMailboxSize.ImageUrl = "images/mailboxspace.GIF"
        btnLinkAliasAddress.imageurl = "images/addaliasaddress2.gif"

        mailboxspace.Visible = True
        Redirector.Visible = False
        ChangePasswd.Visible = False
        Forwarder.Visible = False
        Retriever.Visible = False
        AutoResponder.Visible = False
        aliasaddresses.visible = False

        Dim objCon As New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
        Dim objCom As New SqlCommand
        objCon.Open()

        If RadioButton1.Checked Then
            objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','clean mailbox','" & strEmailName & "','','0','','','','','','','','','','','','','','','';"
        ElseIf RadioButton2.Checked Then
            objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','clean mailbox','" & strEmailName & "','','" & CInt(txtDeleteDays.Text).ToString & "','','','','','','','','','','','','','','','';"
        End If

        objCom.Connection = objCon
        objCom.ExecuteNonQuery()

        objCon.Close()
        objCom.Dispose()

        successmsg2.InnerHtml = "Emails are being purged, this may take a few minutes. "

        Dim objMailBox As New Mailbox
        objMailBox.Postoffice = strDomainName
        objMailBox.MailboxName = strEmailName
        objMailBox.Size = -3
        objMailBox.Limit = -1

        System.Threading.Thread.Sleep(2000)

        If objMailBox.GetMailbox = 1 Then

            Dim dblOnePercent As Double = CDbl(100) / CDbl(objMailBox.Limit.ToString)

            ''adjust image to represent size
            Dim dblPercent As Double
            dblPercent = (dblOnePercent * CDbl(objMailBox.Size / 1024)) * 3

            usage.Width = CStr(CInt(dblPercent))
            usage.Height = 21

            usagetext.InnerHtml = Replace(Replace(Replace("There are {X} emails in this mailbox, using {Y} MB of the {Z} MB allocated", "{X}", System.IO.Directory.GetFiles("D:\email\system\Mail Enable\Postoffices\" & strDomainName & "\MAILROOT\" & strEmailName & "\inbox", "*.mai").Length().ToString()), "{Y}", Decimal.Round(CDec(CDbl(objMailBox.Size / 1024)), 2).ToString), "{Z}", Decimal.Round(CDec(CDbl(objMailBox.Limit / 1024)), 2).ToString)

	if objMailBox.Limit.ToString="51200" then
		 radiobutton3.checked=true
	elseif objMailBox.Limit.ToString="1048576" then
		radiobutton4.checked=true
	elseif objMailBox.Limit.ToString="2097152" then
		radiobutton5.checked=true
	elseif objMailBox.Limit.ToString="5242880" then
		radiobutton6.checked=true
	elseif objMailBox.Limit.ToString="10485760" then
		radiobutton7.checked=true
	elseif objMailBox.Limit.ToString="15728640" then
		radiobutton8.checked=true
	elseif objMailBox.Limit.ToString="20971520" then
		radiobutton9.checked=true
	end if

        Else

            usage.Width = 100
            usage.Height = 21

            usagetext.InnerHtml = "Error retrieving mailbox size, please try again."

            radiobutton3.checked=true
        End If

        ''SEND EMAIL
        Dim SmtpServer As New SmtpClient()
        Dim mail As New MailMessage()
                    SmtpServer.Credentials = New  _
        Net.NetworkCredential("EMAILUSERNAME", "PASSWORD")
        SmtpServer.Port = 25
        SmtpServer.Host = "smtp.DOMAIN"
        mail = New MailMessage()
        mail.From = New MailAddress("EMAILADDRESS")
        mail.Subject = "Emails Purged"
        mail.Body = "Mailbox: " & strEmailName & "@" & strDomainName & " Days: " & txtDeleteDays.Text
        SmtpServer.Send(mail)

    End Sub

    Protected Sub btnChangeMailboxSize_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnChangeMailboxSize.Click
        Dim txtEmailSize As TextBox
        Dim intEmailID, intDomainID As Integer

        txtEmailSize = CType(FindControl("txtSliderPos"), TextBox)
        intEmailID = CInt(Request.QueryString("emailID"))
        intDomainID = CInt(Request.QueryString("domainID"))
        
	dim intMailboxSize, strMailboxSize as string

	if radiobutton3.checked then
		intMailboxSize="51200"
	elseif radiobutton4.checked then
		intMailboxSize="1048576"
	elseif radiobutton5.checked then
		intMailboxSize="2097152"
	elseif radiobutton6.checked then
		intMailboxSize="5242880"
	elseif radiobutton7.checked then
		intMailboxSize="10485760"
	elseif radiobutton8.checked then
		intMailboxSize="15728640"
	elseif radiobutton9.checked then
		intMailboxSize="20971520"
	end if

        Dim objCon As New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
        Dim objCom As New SqlCommand
        objCon.Open()

        objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','change mailbox size','" & strEmailName & "','" & intMailboxSize & "','','','','','','','','','','','','','','','','';"

        objCom.Connection = objCon
        objCom.ExecuteNonQuery()

        objCon.Close()
        objCom.Dispose()

        objCon = New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        objCon.Open()

        objCom = New SqlCommand
        objCom.Connection = objCon
        objCom.CommandText = "sp_updatemailboxsize " & intEmailID.ToString & "," & intDomainID.ToString & "," & CInt(CInt(intMailboxSize) / 1024).ToString
        objCom.ExecuteNonQuery()

        objCon.Close()
        objCom.Dispose()
        objCon.Dispose()


        Dim objMailBox As New Mailbox
        objMailBox.Postoffice = strDomainName
        objMailBox.MailboxName = strEmailName
        objMailBox.Size = -3
        objMailBox.Limit = -1

        System.Threading.Thread.Sleep(2000)

        If objMailBox.GetMailbox = 1 Then

            Dim dblOnePercent As Double = CDbl(100) / CDbl(objMailBox.Limit.ToString)

            ''adjust image to represent size
            Dim dblPercent As Double
            dblPercent = (dblOnePercent * CDbl(objMailBox.Size / 1024)) * 3

            usage.Width = CStr(CInt(dblPercent))
            usage.Height = 21

            usagetext.InnerHtml = Replace(Replace(Replace("There are {X} emails in this mailbox, using {Y} MB of the {Z} MB allocated", "{X}", System.IO.Directory.GetFiles("D:\email\system\Mail Enable\Postoffices\" & strDomainName & "\MAILROOT\" & strEmailName & "\inbox", "*.mai").Length().ToString()), "{Y}", Decimal.Round(CDec(CDbl(objMailBox.Size / 1024)), 2).ToString), "{Z}", Decimal.Round(CDec(CDbl(objMailBox.Limit / 1024)), 2).ToString)

	if objMailBox.Limit.ToString="51200" then
		 radiobutton3.checked=true
	elseif objMailBox.Limit.ToString="1048576" then
		radiobutton4.checked=true
	elseif objMailBox.Limit.ToString="2097152" then
		radiobutton5.checked=true
	elseif objMailBox.Limit.ToString="5242880" then
		radiobutton6.checked=true
	elseif objMailBox.Limit.ToString="10485760" then
		radiobutton7.checked=true
	elseif objMailBox.Limit.ToString="15728640" then
		radiobutton8.checked=true
	elseif objMailBox.Limit.ToString="20971520" then
		radiobutton9.checked=true
	end if

        Else

            usage.Width = 100
            usage.Height = 21

            usagetext.InnerHtml = "Error retrieving mailbox size, please try again."

            radiobutton3.checked=true

        End If

        ''SEND EMAIL
        Dim SmtpServer As New SmtpClient()
        Dim mail As New MailMessage()
                    SmtpServer.Credentials = New  _
        Net.NetworkCredential("EMAILUSERNAME", "PASSWORD")
        SmtpServer.Port = 25
        SmtpServer.Host = "smtp.DOMAIN"
        mail = New MailMessage()
        mail.From = New MailAddress("EMAILADDRESS")
        mail.Subject = "Mailbox Size Changed"
        mail.Body = "WHOOOOOO! Mooor monehz!! Mailbox: " & strEmailName & "@" & strDomainName & " New Size: " & intMailboxSize
        SmtpServer.Send(mail)


    End Sub

    Protected Sub RadioButton2_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadioButton2.CheckedChanged
        RadioButton1.Checked = False
    End Sub

    Protected Sub RadioButton1_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        RadioButton2.Checked = False
    End Sub

    Protected Sub btnLinkAliasAddress_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnLinkAliasAddress.Click

        btnLinkChangePasswd.ImageUrl = "images/changepassword2.GIF"
        btnLinkForwarder.ImageUrl = "images/forwarder2.GIF"
        btnLinkRedirector.ImageUrl = "images/redirector2.GIF"
        btnLinkRetreiver.ImageUrl = "images/popretriever2.GIF"
        btnLinkAutoResponder.ImageUrl = "images/autoresponder2.GIF"
        btnLinkMailboxSize.ImageUrl = "images/mailboxspace2.GIF"
        btnLinkAliasAddress.imageurl = "images/addaliasaddress.gif"

        mailboxspace.Visible = False
        Redirector.Visible = False
        ChangePasswd.Visible = False
        Forwarder.Visible = False
        Retriever.Visible = False
        AutoResponder.Visible = False
        aliasaddresses.visible = True

        successmsg.InnerHtml = ""

        Dim mailboxID, domainID As Integer
        domainID = CInt(Request.QueryString("domainID"))
        mailboxID = CInt(Request.QueryString("emailID"))

        Dim oCon As SqlConnection
        Dim oCom As SqlCommand
        Dim oRS As SqlDataReader

        oCon = New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        oCom = New SqlCommand
        oCon.Open()
        oCom.Connection = oCon
        oCom.CommandText = "sp_getaliasaddresses " & domainID.ToString & "," & mailboxID.ToString
        oRS = oCom.ExecuteReader

        If oRS.HasRows Then

            rptAliasAddresses.DataSource = oRS
            rptAliasAddresses.DataBind()

        End If

        oRS.Close()
        oCom.Dispose()
        oCon.Close()
        oCon.Dispose()

    End Sub

    Protected Sub btnCreateAliasAddress_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreateAliasAddress.Click


        Dim objCon As New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        Dim objCom As New SqlCommand
        Dim intEmailID As Integer
        Dim intDomainID As Integer
        Dim objRs As SqlDataReader
        intEmailID = CInt(Request.QueryString("emailID"))
        intDomainID = CInt(Request.QueryString("domainID"))

        objCon.Open()

        objCom.CommandText = "sp_createaliasaddress " & intDomainID.ToString & "," & intEmailID.ToString & ",'" & txtAliasAddress.Text & "'"
        objCom.Connection = objCon
        objCom.ExecuteNonQuery()

        ''get forwarders
        objCom.CommandText = "sp_getaliasaddresses " & intDomainID.ToString & "," & intEmailID.ToString
        objCom.Connection = objCon

        objRs = objCom.ExecuteReader

        If objRs.HasRows Then

            rptAliasAddresses.DataSource = objRs
            rptAliasAddresses.DataBind()

        End If

        objRs.Close()
        objCom.Dispose()
        objCon.Close()
        objCon.Dispose()


        objCon = New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
        objCom = New SqlCommand

        objCon.Open()

        ''sp_createcommand
        objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','add alias address','" & strEmailName & "','" & txtAliasAddress.Text & "','','','','','','','','','','','','','','','','';"
        objCom.Connection = objCon

        objCom.ExecuteNonQuery()

        objCom.Dispose()
        objCon.Close()
        objCon.Dispose()

        successmsg.InnerHtml = "Alias Address Added"
        successmsg.Style.Add("color", "#00ff00")


        ''SEND EMAIL
        Dim SmtpServer As New SmtpClient()
        Dim mail As New MailMessage()
                    SmtpServer.Credentials = New  _
        Net.NetworkCredential("EMAILUSERNAME", "PASSWORD")
        SmtpServer.Port = 25
        SmtpServer.Host = "smtp.DOMAIN"
        mail = New MailMessage()
        mail.From = New MailAddress("EMAILADDRESS")
        mail.Subject = "Alias Address Added"
        mail.Body = "Mailbox: " & strEmailName & "@" & strDomainName & " Alias: " & txtAliasAddress.text
        SmtpServer.Send(mail)

    End Sub

End Class