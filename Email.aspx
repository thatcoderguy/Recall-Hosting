<%@ Page Language="VB" AutoEventWireup="false" enableViewStateMac="false" CodeFile="Email.aspx.vb" Inherits="Email" %>

<%@ Register TagPrefix="uc1" TagName="ASPSlider" Src="~/ASPSlider.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Welcome to RecallHosting Email Control Panel</title>
    <link href="/styles/preview.css" rel="stylesheet" type="text/css" />
    
    
<script type="text/javascript">
<!--- 

    //for the slider!
    function updateText() {

        if (document.getElementById('ASPSlider3_TextBox1')) {
            var calculatedSize = parseInt(document.getElementById('ASPSlider3_TextBox1').value);

            if (calculatedSize > 1000) {
                calculatedSize = calculatedSize / 1024;

                if (calculatedSize > 1000) {

                    calculatedSize = calculatedSize / 1024;
                    var msg = document.getElementById('inmegabytes');
                    msg.value = ' ' + calculatedSize.toFixed(1) + ' GB';
                }
                else {
                    var msg = document.getElementById('inmegabytes');
                    msg.value = ' ' + calculatedSize.toFixed(1) + ' MB';
                }
            }
            else {

                var msg = document.getElementById('inmegabytes');
                msg.value = ' ' + calculatedSize.toFixed(1) + ' KB';
            }
        }
    }

    if (document.getElementById('ASPSlider3_TextBox1')) {
        var calculatedSize = parseInt(document.getElementById('ASPSlider3_TextBox1').value);

        if (calculatedSize > 1000) {
            calculatedSize = calculatedSize / 1024;

            if (calculatedSize > 1000) {

                calculatedSize = calculatedSize / 1024;
                var msg = document.getElementById('inmegabytes');
                msg.value = ' ' + calculatedSize.toFixed(1) + ' GB';
            }
            else {
                var msg = document.getElementById('inmegabytes');
                msg.value = ' ' + calculatedSize.toFixed(1) + ' MB';
            }
        }
        else {

            var msg = document.getElementById('inmegabytes');
            msg.value = ' ' + calculatedSize.toFixed(1) + ' KB';
        }
    }

    --->
</script>
</head>

<body>
 
<div id="wrapper">
<div id="outer">
<div id="topbar">
<a href=""><img src="/images/logo.gif" alt="" id="logo" /></a>
 
 
 
<div id="contactdetailstop">

</div>
</div>

<div id="main">
 
<div id="primary">
 
	<h1 id="welcomemessage">RecallHosting Email Control Panel v0.2 Beta</h1>
 
	<div class="textcontainer">
 
 
    <div style="clear: left;float: left;" id="message" runat="server">
    
        <h2 id="emailbox" runat="server">Email Box:</h2>
    
        <form id="frmChangePassword" runat="server">
    
        <asp:ImageButton CssClass="fl cl " ID="btnLinkChangePasswd" runat="server" 
            ImageUrl="images/changepassword.GIF" Height="36px" Width="141px" />
        <asp:ImageButton CssClass="fl"  ID="btnLinkRetreiver" runat="server" 
            ImageUrl="images/popretriever2.GIF" Height="36px" Width="197px" />
        <asp:ImageButton CssClass="fl" ID="btnLinkRedirector" runat="server" 
            ImageUrl="images/redirector2.GIF" Height="36px" Width="168px" />
        <asp:ImageButton CssClass="fl" ID="btnLinkAutoResponder" runat="server" Height="36px" 
            ImageUrl="~/images/autoresponder2.GIF" Width="150px" /> 
        <asp:ImageButton CssClass="fl" ID="btnLinkForwarder" runat="server" 
            ImageUrl="images/forwarder2.GIF" Height="36px" Width="180px" />
        <asp:ImageButton CssClass="fl" ID="btnLinkMailboxSize" runat="server" 
            ImageUrl="images/mailboxspace2.GIF" Height="36px" Width="180px" />
         <asp:ImageButton CssClass="fl" ID="btnLinkAliasAddress" runat="server" 
            ImageUrl="images/addaliasaddress2.GIF" Height="36px" Width="180px" />
        <div id="ChangePasswd" runat="server">
        <br /><br /><br /><br /><br />
        <table>
        <tr>
            <td><asp:Label ID="Label1" runat="server" Text="Password:"></asp:Label></td><td><asp:TextBox ID="txtPassword" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td><asp:Label ID="Label2" runat="server" Text="Confirm:"></asp:Label></td><td><asp:TextBox ID="txtConfirm" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td colspan="2"><asp:Button class="submit" ID="btnChangePasswd" runat="server" 
                    Text="Change Password" style="height: 26px" /></td>
        </tr>
        </table>
        </div>
    
        <div style="clear: left;float: left;" id="Retriever" runat="server"  visible="false">
            <br />
            <table>
            <tr><th>Retriever POP Address</th><th>Retriever Username</th><th>Retriever Password</th><th>Keep Copy</th><th></th></tr>
       
            <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate><tr><td><%# DataBinder.Eval(Container.DataItem, "popAddress")%></td> <td><%# DataBinder.Eval(Container.DataItem, "popUsername")%></td> <td><%# DataBinder.Eval(Container.DataItem, "popPassword")%></td><td><%# DataBinder.Eval(Container.DataItem, "status")%></td><td><a href="email.aspx?emailID=<%# DataBinder.Eval(Container.DataItem, "emailID")%>&amp;domainID=<%# DataBinder.Eval(Container.DataItem, "domainID")%>&amp;RetrieverID=<%# DataBinder.Eval(Container.DataItem, "redirectorID")%>">Delete</a></td></tr></ItemTemplate>
            </asp:Repeater>
        
            </table>
            <br />
            <table>
            <tr>
                <td><asp:Label ID="Label3" runat="server" Text="POP Address"></asp:Label></td>
                <td><asp:TextBox ID="txtPOPAddress" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="Label4" runat="server" Text="POP Username"></asp:Label></td>
                <td><asp:TextBox ID="txtPOPUsername" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="Label5" runat="server" Text="POP Password"></asp:Label></td>
                <td><asp:TextBox ID="txtPOPPassword" runat="server"></asp:TextBox></td>
            </tr>   
                 
            <tr>
                <td><asp:Label ID="Label9" runat="server" Text="Status"></asp:Label></td>
                <td><asp:DropDownList ID="dlStatus" runat="server"><asp:ListItem Text="Enabled - Do Not Keep Copy" Value="0"></asp:ListItem><asp:ListItem Text="Enabled - Keep Copy" Value="1"></asp:ListItem></asp:DropDownList></td>
            </tr>
            <tr>
                <td colspan="2"><asp:Button class="submit" ID="btnRetriver" runat="server" Text="Create Retriever" /></td>
            </tr>
            </table>

        </div>
        
        <div style="float: left; clear: left;" id="AutoResponder" runat="server" visible="false">
         <br /><span style="font-weight:bold">Subject:</span>
            <asp:TextBox ID="txtAutoResponderSubject" runat="server" Width="514px"></asp:TextBox>
            <br />
            <span style="font-weight:bold">Body:</span><br />
            <asp:TextBox ID="txtAutoResponderBody" runat="server" Height="199px" 
                Width="561px" TextMode="MultiLine"></asp:TextBox>
            <br />
            <asp:Button  class="submit" ID="btnUpdateAutoResponder" runat="server" Text="Submit" 
                Width="72px" style="height: 26px" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox ID="cbStatus" runat="server" />
            <br />
        
        </div>
    
        <div style="clear: left;float: left;" id="Redirector" runat="server" visible="false">
             <br />
             
            <table>
            <tr>
            <th>Redirector Destination</th><th></th>
            </tr>
       
            <asp:Repeater ID="Repeater2" runat="server">
            <ItemTemplate><tr><td><%# DataBinder.Eval(Container.DataItem, "redirectorEmail")%></td><th><a href="email.aspx?emailID=<%# DataBinder.Eval(Container.DataItem, "emailID")%>&amp;domainID=<%# DataBinder.Eval(Container.DataItem, "domainID")%>&amp;RedirectorID=<%# DataBinder.Eval(Container.DataItem, "redirectorID")%>&amp;RedirectorDest=<%# DataBinder.Eval(Container.DataItem, "redirectorEmail")%>&amp;Status=<%# DataBinder.Eval(Container.DataItem, "Status")%>">Delete</a></th></tr></ItemTemplate>
            </asp:Repeater>
                               <tr>
                <td>&nbsp;</td>
            </tr>
                   <tr>
                <td id="redirectstatus" runat="server" ></td>
            </tr>
            </table>
            
            <br /><br />
            <table>

            <tr>
                <td><asp:Label ID="Label6" runat="server" Text="Redirector Destination"></asp:Label></td>
                <td><asp:TextBox ID="txtRedirectDestination" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="Label7" runat="server" Text="Status"></asp:Label></td>
                <td>
                    <asp:DropDownList ID="dlRedirectionStatus" runat="server">
                    <asp:ListItem Text="Enabled - Do Not Keep Copy" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Enabled - Keep Copy" Value="1"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2"><asp:Button class="submit" ID="btnCreateRedirect" runat="server" 
                        Text="Create Redirector" style="height: 26px" /></td>
            </tr>
            </table>
                         <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
        ErrorMessage="Destination must be a full email address e.g. person@domain.com" 
        ControlToValidate="txtRedirectDestination" ValidationExpression="^[a-zA-Z0-9_.-]{2,30}\@[a-z0-9\-]{2,63}\.(co\.uk|com|org|org\.uk|edu|co|net)$"></asp:RegularExpressionValidator>
        </div>
    
        <div style="clear: left;float: left;" id="Forwarder" runat="server" visible="false">
 <br />
            
            <table>
            <tr>
            <th>Forwarder Destination</th><th></th>
            </tr>
       
            <asp:Repeater ID="Repeater3" runat="server">
            <ItemTemplate><tr><td><%# DataBinder.Eval(Container.DataItem, "forwarderEmail")%></td><th><a href="email.aspx?emailID=<%# DataBinder.Eval(Container.DataItem, "emailID")%>&amp;domainID=<%# DataBinder.Eval(Container.DataItem, "domainID")%>&amp;ForwarderID=<%# DataBinder.Eval(Container.DataItem, "forwarderID")%>&amp;ForwarderDest=<%# DataBinder.Eval(Container.DataItem, "forwarderEmail")%>">Delete</a></th></tr></ItemTemplate>
            </asp:Repeater>
        
            </table>
            
                <br /><br />
            <table>
            <tr>
                <td><asp:Label ID="Label8" runat="server" Text="Forwarder Destination"></asp:Label></td>
                <td><asp:TextBox ID="txtForwarderDestination" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td colspan="2"><asp:Button  class="submit" ID="btnCreateForward" runat="server" Text="Create Forwarder" /></td>
            </tr>
            </table>
    
             <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
        ErrorMessage="Destination must be a full email address e.g. person@domain.com" 
        ControlToValidate="txtForwarderDestination" ValidationExpression="^[a-zA-Z0-9_.-]{2,30}\@[a-z0-9\-]{2,63}\.(co\.uk|com|org|org\.uk|edu|co|net)$"></asp:RegularExpressionValidator>
    
        </div>
    
        <div style="clear: left;float: left;" id="mailboxspace" runat="server" visible="false">
        
                <h2>Space Usage:</h2>
                
                
                <table style="width: 765px">
                <tr>
                <td colspan="2" id="usagetext" runat="server"></td>
                </tr>
                <tr>
                <td style="border: 1px solid black;width:300px" class="style1"><img runat="server" id="usage" src="~/images/progress.GIF" /></td><td style="width: 465px"></td>
                </tr>
                </table>
                <hr />
                <h2>Purge Emails:</h2>
                
            <asp:RadioButton ID="RadioButton1" runat="server" Text="All Emails" Checked="True" 
                    GroupName="deletemessages" /><br />
            <asp:RadioButton ID="RadioButton2" runat="server" text="Messages Older Than" 
                    GroupName="deletemessages"/>&nbsp;
                <asp:TextBox ID="txtDeleteDays" runat="server" Width="29px">30</asp:TextBox>    
                
                &nbsp;<asp:Label  class="standard" ID="Label10" runat="server" Text="Days"></asp:Label><br  />
                <asp:Button ID="btnPurgeEmails" class="submit" runat="server" Text="Purge Emails" /><br />
                 <div style="float: left; clear: left; font-weight: bold;" id="successmsg2" runat="server"></div>
                <hr />
                
                <h2>Upgrade Mailbox Size:</h2>
       
                   <table>
        <tr>
        <td>
         <asp:Label ID="lblSize" runat="server" Text="Mailbox Size:"></asp:Label></td>
             <td width="100%">
		<asp:RadioButton id="RadioButton3" 
		     AutoPostBack="False"
		     Checked="True"
		     GroupName="GroupName"
		     Text="50MB"
		     TextAlign="Left"
		     runat="server"/>
		<asp:RadioButton id="RadioButton4" 
		     AutoPostBack="False"
		     Checked="False"
		     GroupName="GroupName"
		     Text="1GB"
		     TextAlign="Left"
		     runat="server"/>
		<asp:RadioButton id="RadioButton5" 
		     AutoPostBack="False"
		     Checked="False"
		     GroupName="GroupName"
		     Text="2GB"
		     TextAlign="Left"
		     runat="server"/>
		<asp:RadioButton id="RadioButton6" 
		     AutoPostBack="False"
		     Checked="False"
		     GroupName="GroupName"
		     Text="5GB"
		     TextAlign="Left"
		     runat="server"/>
		<asp:RadioButton id="RadioButton7" 
		     AutoPostBack="False"
		     Checked="False"
		     GroupName="GroupName"
		     Text="10GB"
		     TextAlign="Left"
		     runat="server"/>	
		<asp:RadioButton id="RadioButton8" 
		     AutoPostBack="False"
		     Checked="False"
		     GroupName="GroupName"
		     Text="15GB"
		     TextAlign="Left"
		     runat="server"/>	
		<asp:RadioButton id="RadioButton9" 
		     AutoPostBack="False"
		     Checked="False"
		     GroupName="GroupName"
		     Text="20GB"
		     TextAlign="Left"
		     runat="server"/>
            </td>
        </tr>
                  <tr>
                  <td colspan="2">
                      <asp:Button  class="submit" ID="btnChangeMailboxSize" runat="server" Text="Change Mailbox Size" /></td>
                  </tr>
                   </table>
        </div>
        
          <div style="clear: left;float: left;" id="aliasaddresses" runat="server" visible="false">
    <br />
                <table>
            <tr>
            <th>Alias Addresses</th><th></th>
            </tr>
       
            <asp:Repeater ID="rptAliasAddresses" runat="server">
            <ItemTemplate><tr><td>&nbsp;&nbsp;<%#DataBinder.Eval(Container.DataItem, "aliasAddress")%>&nbsp;&nbsp;</td><td><a href="Email.aspx?emailID=<%#DataBinder.Eval(Container.DataItem, "mailboxID")%>&amp;domainID=<%#DataBinder.Eval(Container.DataItem, "domainID")%>&amp;delete=yes&amp;aliasaddressid=<%#DataBinder.Eval(Container.DataItem, "aliasaddressID")%>&amp;aliasaddress=<%#DataBinder.Eval(Container.DataItem, "aliasAddress")%>">Delete</a></td></tr></ItemTemplate>
            </asp:Repeater>
        
            </table>
            
                <br /><br />
            <table>
            <tr>
                <td colspan="3"><asp:Label ID="Label11" runat="server" Text="Alias Email Address"></asp:Label></td>
                <td><asp:TextBox ID="txtAliasAddress" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td colspan="2"><asp:Button  class="submit" ID="btnCreateAliasAddress" runat="server" Text="Add Alias Address" /></td>
            </tr>
            </table>
    
 
        </div>
        
        </form>
        
        <div style="float: left; clear: left; font-weight: bold;" id="successmsg" runat="server"></div>
        
        <br />

    </div>
 
	</div>
	</div>
<div id="secondary">

<img src="/images/side.jpg" alt="" id="lifestyle" />

</div>
 
</div>
<div id="navbar">
<ul id="nav">
<li class="first"><a href="emails.aspx?domainID=<%= request.querystring("domainID") %>">Back to Emails</a></li>
</ul>
</div>
 
<div id="footer">
Copyright <%#Year(Now)%> RecallHosting - Developed by <a href="" title="">RecallHosting</a> - RecallHosting</div>
</div>
</div>
 
</body>

</html>

