Imports System.Data.SqlClient
Imports System.Net.Mail
Imports System.ServiceProcess
Imports System.Management

Partial Class Emails
    Inherits System.Web.UI.Page

    Public domainID As Integer
    Private strDomainName As String

    Protected Sub btnCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreate.Click

        Dim oCon As New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        oCon.Open()
        
        dim intMailboxSize, strMailboxSize as string
	        	
	if radiobutton1.checked then
		intMailboxSize="51200"
	elseif radiobutton2.checked then
		intMailboxSize="1048576"
	elseif radiobutton3.checked then
		intMailboxSize="2097152"
	elseif radiobutton4.checked then
		intMailboxSize="5242880"
	elseif radiobutton5.checked then
		intMailboxSize="10485760"
	elseif radiobutton6.checked then
		intMailboxSize="15728640"
	elseif radiobutton7.checked then
		intMailboxSize="20971520"
        end if

        Dim oCom As New SqlCommand
        oCom.Connection = oCon
        oCom.CommandText = "sp_createemail '" & txtEmail.Text & "'," & domainID.ToString & ",''," & CInt(CInt(intMailboxSize) / 1024).ToString & ""

        Dim oRS As SqlDataReader
        oRS = oCom.ExecuteReader

        Dim blnCreateEmail As Boolean
        blnCreateEmail = False

        Dim txtEmailSize As TextBox

        If oRS.HasRows Then

            oRS.Read()
            If oRS.Item("emailID") = "0" Then
                msg.InnerHtml = "Email address already exsists"
                oRS.NextResult()
            Else
                oRS.NextResult()
                msg.InnerHtml = "Email address created"
                blnCreateEmail = True
            End If

            Repeater1.DataSource = oRS
            Repeater1.DataBind()

        End If

        oRS.Close()
        oCom.Dispose()
        oCon.Close()
        oCon.Dispose()

        txtEmailSize = CType(FindControl("txtSliderPos"), TextBox)

        If blnCreateEmail Then
            ''CREATE EMAIL

            oCon = New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
            oCom = New SqlCommand

            oCon.Open()

            ''sp_createcommand
            oCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','create mailbox','" & txtEmail.Text & "','" & intMailboxSize & "','" & txtPassword.Text & "','','','','','','','','','','','','','','','';"
            oCom.Connection = oCon

            oCom.ExecuteNonQuery()

            oCom.Dispose()
            oCon.Close()
            oCon.Dispose()

            ''SEND EMAIL
            Dim SmtpServer As New SmtpClient()
            Dim mail As New MailMessage()
            SmtpServer.Credentials = New  _
  Net.NetworkCredential("EMAILUSERNAME", "PASSWORD")
            SmtpServer.Port = 25
            SmtpServer.Host = "smtp.DOMAIN"
            mail = New MailMessage()
            mail.From = New MailAddress("EMAILADDRESS")
            mail.Subject = "New Mailbox"
            mail.Body = "I can hear the cash registers ring-a-ding-ling-a-ling! Mailbox: " & txtEmail.text & "@" & strDomainName & " Size: " & intMailboxSize
            SmtpServer.Send(mail)

        End If

    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        domainID = CInt(Request.QueryString("domainID"))

        Dim oCon As New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        Dim oCom As New SqlCommand
        Dim oRS As SqlDataReader
        Dim strEmailName As String

        oCon.Open()
        oCom.Connection = oCon
        oCom.CommandText = "sp_getemails " & domainID.ToString
        oRS = oCom.ExecuteReader

        If oRS.HasRows Then
            oRS.Read()
            domainname.InnerHtml = "Domain Name: " & oRS.Item("domainName")
            strDomainName = oRS.Item("domainName")
        End If

        oRS.Close()

        If Request.QueryString("delete") = "yes" Then

            strEmailName = Request.QueryString("email")

            If Not strEmailName.ToLower = "postmaster" Then

                strEmailName = Regex.Replace(strEmailName, "[^A-Za-z0-9_\.\-]", "")

                oCom.CommandText = "sp_deleteemail " & CStr(CInt(Request.QueryString("emailID"))) & "," & CStr(CInt(Request.QueryString("domainID")))
                oCom.ExecuteNonQuery()

                oCon = New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
                oCom = New SqlCommand
                oCon.Open()

                If Request.QueryString("type") = "Email" Then
                    ''DELETE EMAIL
                    oCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','delete mailbox','" & strEmailName & "','" & strEmailName & "','','','','','','','','','','','','','','','','';"
                    oCom.Connection = oCon
                Else
                    ''DELETE Forwarder
                    oCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','delete forwarder','" & strEmailName & "','FWBMAILBOX','','','','','','','','','','','','','','','','';"
                    oCom.Connection = oCon
                End If

                oCom.ExecuteNonQuery()

                oCom.Dispose()
                oCon.Close()
                oCon.Dispose()

                ''SEND EMAIL
                Dim SmtpServer As New SmtpClient()
                Dim mail As New MailMessage()
                    SmtpServer.Credentials = New  _
                Net.NetworkCredential("EMAILUSERNAME", "PASSWORD")
                SmtpServer.Port = 25
                SmtpServer.Host = "smtp.DOMAIN"
                mail = New MailMessage()
                mail.From = New MailAddress("EMAILADDRESS")
                mail.Subject = "Deleted mailbox"
                mail.Body = "Awwwww! Mailbox: " & strEmailName & "@" & strDomainName
                SmtpServer.Send(mail)

                msg.InnerHtml = "Email address deleted"
            Else
                msg.InnerHtml = "Cannot delete postmaster"
            End If
        End If

        If Request.QueryString("delete") = "alias" Then

            Dim strAliasDomain As String

            oCon = New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
            oCom = New SqlCommand
            oCon.Open()

            oCom.Connection = oCon
            oCom.CommandText = "sp_deletealiasdomain " & CStr(CInt(Request.QueryString("aliasdomainID")))
            oCom.ExecuteNonQuery()

            oCon = New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
            oCom = New SqlCommand
            oCon.Open()

            strAliasDomain = Request.QueryString("aliasdomain")

            oCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','remove alias domain','','" & strAliasDomain & "','','','','','','','','','','','','','','','','','';"
            oCom.Connection = oCon
            oCom.ExecuteNonQuery()

            oCon.Close()
            oCom.Dispose()
            oCon.Dispose()

            ''SEND EMAIL
            Dim SmtpServer As New SmtpClient()
            Dim mail As New MailMessage()
                                SmtpServer.Credentials = New  _
Net.NetworkCredential("EMAILUSERNAME", "PASSWORD")
            SmtpServer.Port = 25
            SmtpServer.Host = "smtp.DOMAIN"
            mail = New MailMessage()
            mail.From = New MailAddress("EMAILADDRESS")
            mail.Subject = "Alias Domain Removed"
            mail.Body = "Domain: " & strDomainName & "  Alias: " & strAliasDomain
            SmtpServer.Send(mail)

            frmAliasDomain.Visible = True
            frmCreateEmail.Visible = False
            frmCreateForwarder.Visible = False

        End If

        oCon = New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        oCom = New SqlCommand
        oCon.Open()
        oCom.Connection = oCon
        oCom.CommandText = "sp_getemails " & domainID.ToString
        oRS = oCom.ExecuteReader

        'If Not IsPostBack Then
        oRS.NextResult()
        If oRS.HasRows Then

            Repeater1.DataSource = oRS
            Repeater1.DataBind()

        End If

        'End If

        oRS.Close()
        oCom.Dispose()
        oCon.Close()
        oCon.Dispose()

        oCon = New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        oCom = New SqlCommand
        oCon.Open()
        oCom.Connection = oCon
        oCom.CommandText = "sp_getaliasdomains " & domainID.ToString
        oRS = oCom.ExecuteReader

        If oRS.HasRows Then

            rptAlias.DataSource = oRS
            rptAlias.DataBind()

        End If

        oRS.Close()
        oCom.Dispose()
        oCon.Close()
        oCon.Dispose()

        If Request.QueryString("Msg") = "ForwarderDeleted" Then
            msg.InnerHtml = "Forwarder has been deleted"
        End If


    End Sub

    Protected Sub btnCreateForwarder_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreateForwarder.Click
        Dim oCon As New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        oCon.Open()

        Dim oCom As New SqlCommand
        oCom.Connection = oCon
        oCom.CommandText = "sp_createemail '" & txtForwarder.Text & "'," & domainID.ToString & ",'" & txtDestination.Text & "',0"

        Dim oRS As SqlDataReader
        oRS = oCom.ExecuteReader

        Dim blnCreateEmail As Boolean
        blnCreateEmail = False

        If oRS.HasRows Then

            oRS.Read()
            If oRS.Item("emailID") = "0" Then
                msg.InnerHtml = "Email address already exsists"
                oRS.NextResult()
            Else
                oRS.NextResult()
                msg.InnerHtml = "Email address created"
                blnCreateEmail = True
            End If

            Repeater1.DataSource = oRS
            Repeater1.DataBind()

        End If

        oRS.Close()
        oCom.Dispose()
        oCon.Close()
        oCon.Dispose()

        If blnCreateEmail Then
            ''CREATE FORWARDER

            oCon = New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
            oCom = New SqlCommand

            oCon.Open()

            ''sp_createcommand
            oCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','add forwarder','" & txtForwarder.Text & "','" & txtDestination.Text & "','','','','','','','','','','','','','','','','';"
            oCom.Connection = oCon

            oCom.ExecuteNonQuery()

            oCom.Dispose()
            oCon.Close()
            oCon.Dispose()

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
            mail.Body = "That wont earn me money! Mailbox: " & txtEmail.text & "@" & strDomainName & "  Destination: " & txtDestination.text
            SmtpServer.Send(mail)

        End If
    End Sub

    Protected Sub btnForwarder_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnForwarder.Click
        frmCreateEmail.Visible = False
        frmCreateForwarder.Visible = True
        frmAliasDomain.Visible = False
    End Sub

    Protected Sub btnLinkEmail_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnLinkEmail.Click
        frmCreateEmail.Visible = True
        frmCreateForwarder.Visible = False
        frmAliasDomain.Visible = False
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnForwarder2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnForwarder2.Click
        frmCreateEmail.Visible = False
        frmCreateForwarder.Visible = True
        frmAliasDomain.Visible = False
    End Sub

    Protected Sub btnLinkEmail2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnLinkEmail2.Click
        frmCreateEmail.Visible = True
        frmCreateForwarder.Visible = False
        frmAliasDomain.Visible = False
    End Sub

    Protected Sub btnAlias2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnAlias2.Click
        frmCreateEmail.Visible = False
        frmCreateForwarder.Visible = False
        frmAliasDomain.Visible = True
    End Sub

    Protected Sub btnAlias_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnAlias.Click
        frmCreateEmail.Visible = False
        frmCreateForwarder.Visible = False
        frmAliasDomain.Visible = True
    End Sub

    Protected Sub btnCreateAlias_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreateAlias.Click
        Dim oCon As New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        oCon.Open()

        Dim oCom As New SqlCommand
        oCom.Connection = oCon
        oCom.CommandText = "sp_createaliasdomain '" & txtAliasDomain.Text & "'," & domainID.ToString

        oCom.ExecuteNonQuery()


        oCon = New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
        oCom = New SqlCommand

        oCon.Open()

        ''sp_createcommand
        oCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','add alias domain','','" & txtAliasDomain.Text & "','','','','','','','','','','','','','','','','','';"
        oCom.Connection = oCon

        oCom.ExecuteNonQuery()

        oCom.Dispose()
        oCon.Close()
        oCon.Dispose()

        ''SEND EMAIL
        Dim SmtpServer As New SmtpClient()
        Dim mail As New MailMessage()
                            SmtpServer.Credentials = New  _
Net.NetworkCredential("EMAILUSERNAME", "PASSWORD")
        SmtpServer.Port = 25
        SmtpServer.Host = "smtp.DOMAIN"
        mail = New MailMessage()
        mail.From = New MailAddress("EMAILADDRESS")
        mail.Subject = "New Alias Domain"
        mail.Body = "Domain: " & strDomainName & "  Alias: " & txtAliasDomain.Text
        SmtpServer.Send(mail)

        oCon = New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        oCom = New SqlCommand
        Dim oRS As sqldatareader

        oCon.Open()
        oCom.Connection = oCon
        oCom.CommandText = "sp_getaliasdomains " & domainID.ToString
        oRS = oCom.ExecuteReader

        If oRS.HasRows Then

            rptAlias.DataSource = oRS
            rptAlias.DataBind()

        End If

        oRS.Close()
        oCom.Dispose()
        oCon.Close()
        oCon.Dispose()

    End Sub

End Class