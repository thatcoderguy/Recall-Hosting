Imports System.Data.SqlClient
Imports System.Net.Mail
Imports System.ServiceProcess
Imports System.Management

Partial Class Domain
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Dim strAction As String = Request.Form("action")
        Dim strDomainName As String = Request.Form("txtDomain")

        Dim objCon As New SqlConnection(ConfigurationManager.ConnectionStrings("emailinterfacealphaConnectionString").ConnectionString)
        Dim objCom As New SqlCommand
        Dim objRS As SqlDataReader

        Dim blnCreateDomain As Boolean
        blnCreateDomain = False

        objCon.Open()

        Select Case strAction

            Case "find"

                objCom.CommandText = "sp_getdomain '" & strDomainName & "'"
                objCom.Connection = objCon

                objRS = objCom.ExecuteReader

                If objRS.HasRows Then

                    Repeater1.DataSource = objRS
                    Repeater1.DataBind()

                Else

                    msg.InnerHtml = "No Domain found." & "  <a href=""Default.aspx"">Go Back</a>"

                End If

                objRS.Close()
                objCom.Dispose()
                objCon.Close()
                objCon.Dispose()

            Case "create"

                objCom.CommandText = "sp_createdomain '" & strDomainName & "'"
                objCom.Connection = objCon

                objRS = objCom.ExecuteReader

                If objRS.HasRows Then

                    objRS.Read()
                    If objRS.Item("domainID") = "0" Then
                        msg.InnerHtml = "Domain Already Exists"
                        objRS.NextResult()
                    Else
                        objRS.NextResult()
                        msg.InnerHtml = "Domain Created"
                        blnCreateDomain = True
                    End If

                    Repeater1.DataSource = objRS
                    Repeater1.DataBind()

                Else

                    msg.InnerHtml = "Domain Not Created." & "  <a href=""Default.aspx"">Go Back</a>"

                End If

                objRS.Close()

                objCom.Dispose()
                objCon.Close()
                objCon.Dispose()


                If blnCreateDomain Then

                    ''CREATE EMAIL DOMAIN

                    objCon = New SqlConnection(ConfigurationManager.ConnectionStrings("workhorseConnectionString").ConnectionString)
                    objCom = New SqlCommand

                    objCon.Open()

                    ''sp_createcommand
                    objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','create postoffice','','','','','','','','','','','','','','','','','','';"
                    objCom.Connection = objCon

                    objCom.ExecuteNonQuery()

                    ''sp_createcommand
                    objCom.CommandText = "sp_insertcommand 1,'mailbox','" & strDomainName & "','add webmail','','','','','','','','','','','','','','','','','','';"
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
                    mail.Subject = "New Domain"
                    mail.Body = "Looks like a new domain just got setup yo! Domain name: " & strDomainName
                    SmtpServer.Send(mail)

                End If

        End Select

    End Sub


End Class
