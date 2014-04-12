<%@ Page Language="VB" AutoEventWireup="false" enableViewStateMac="false" CodeFile="Emails.aspx.vb" Inherits="Emails" %>

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
	
	<h2 id="domainname" runat="server"></h2>

    <h2>Email Box(es):</h2>
     <asp:Repeater ID="Repeater1" runat="server">
    <HeaderTemplate>
        <table border="1" >
        <tr><th>Name</th><th>Type</th><th>Size (MB)</th><th colspan="2">Actions</th></tr>
    </HeaderTemplate>
    
    <ItemTemplate>
    
        <tr>
        
            <td>&nbsp;&nbsp;<%#DataBinder.Eval(Container.DataItem, "emailName")%>&nbsp;&nbsp;</td><td>&nbsp;&nbsp;<%#DataBinder.Eval(Container.DataItem, "emailType")%>&nbsp;&nbsp;</td><td>&nbsp;&nbsp;&nbsp;<%#DataBinder.Eval(Container.DataItem, "emailSize")%>&nbsp;&nbsp;&nbsp;</td><td><a href="Email.aspx?emailID=<%#DataBinder.Eval(Container.DataItem, "emailID")%>&amp;domainID=<%=domainID %>&amp;Type=<%#DataBinder.Eval(Container.DataItem, "emailType")%>">Select</a>   </td><td><a href="Emails.aspx?emailID=<%#DataBinder.Eval(Container.DataItem, "emailID")%>&amp;domainID=<%=domainID %>&amp;type=<%#DataBinder.Eval(Container.DataItem, "emailType")%>&amp;delete=yes&amp;email=<%#DataBinder.Eval(Container.DataItem, "emailName")%>">Delete</a></td>
        
        </tr>
    
    </ItemTemplate>
    
    <FooterTemplate>
        </table>
    </FooterTemplate>
    
    </asp:Repeater>

<br />
    <div id="msg" runat="server" style="font-size: larger; color: #008000; font-weight: bold">
    </div>
   
        
    <form id="frmCreateEmail"  runat="server" visible="true">
        
    <div>
     <h2>
        <img alt="" src="images/createemail.GIF" class="fl cl" style="height: 33px; width: 159px" />
        <asp:ImageButton
            ID="btnForwarder" runat="server" class="fl" style="height: 33px; width: 159px" 
             ImageUrl="~/images/createforward2.GIF" />
        <asp:ImageButton
            ID="btnAlias" runat="server" class="fl" style="height: 33px; width: 159px" 
             ImageUrl="~/images/addaliasdomain2.GIF" />
            </h2>
            </div>
  <br /><br /><br />
    <div class="cl fl">
        <table>
        <tr>
            <td>  <input type="hidden" name="action" value="create" /><asp:Label ID="Label1" runat="server" Text="Email Name:"></asp:Label></td><td><asp:TextBox ID="txtEmail" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td><asp:Label ID="Label4" runat="server" Text="Password:"></asp:Label></td><td><asp:TextBox ID="txtPassword" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td><asp:Label ID="Label5" runat="server" Text="Confirm Password:"></asp:Label></td><td><asp:TextBox ID="txtConfirmPassword" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
        <td>
         <asp:Label ID="lblSize" runat="server" Text="Mailbox Size (KB):"></asp:Label></td>
             <td>
		<asp:RadioButton id="RadioButton1" 
		     AutoPostBack="False"
		     Checked="True"
		     GroupName="GroupName"
		     Text="50MB"
		     TextAlign="Left"
		     runat="server"/>
		<asp:RadioButton id="RadioButton2" 
		     AutoPostBack="False"
		     Checked="False"
		     GroupName="GroupName"
		     Text="1GB"
		     TextAlign="Left"
		     runat="server"/>
		<asp:RadioButton id="RadioButton3" 
		     AutoPostBack="False"
		     Checked="False"
		     GroupName="GroupName"
		     Text="2GB"
		     TextAlign="Left"
		     runat="server"/>
		<asp:RadioButton id="RadioButton4" 
		     AutoPostBack="False"
		     Checked="False"
		     GroupName="GroupName"
		     Text="5GB"
		     TextAlign="Left"
		     runat="server"/>
		<asp:RadioButton id="RadioButton5" 
		     AutoPostBack="False"
		     Checked="False"
		     GroupName="GroupName"
		     Text="10GB"
		     TextAlign="Left"
		     runat="server"/>	
		<asp:RadioButton id="RadioButton6" 
		     AutoPostBack="False"
		     Checked="False"
		     GroupName="GroupName"
		     Text="15GB"
		     TextAlign="Left"
		     runat="server"/>	
		<asp:RadioButton id="RadioButton7" 
		     AutoPostBack="False"
		     Checked="False"
		     GroupName="GroupName"
		     Text="20GB"
		     TextAlign="Left"
		     runat="server"/>	
            </td>
        </tr>
        <tr>
            <td colspan="2"><asp:Button class="submit" ID="btnCreate" runat="server" Text="Create Email" /></td>
        </tr>
        <tr>
        <td colspan="2">
        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
        ErrorMessage="Email Addresses are only allowed: A-Z, a-z, 0-9, ., _ and -" 
        ControlToValidate="txtEmail" ValidationExpression="^[a-zA-Z0-9_.-]{2,30}"></asp:RegularExpressionValidator>
     <br />
     <asp:CompareValidator ID="CompareValidator1" runat="server" 
         ControlToCompare="txtConfirmPassword" ControlToValidate="txtPassword" 
         ErrorMessage="Passwords Must Match"></asp:CompareValidator></td>
        </tr>
        </table>
            
    </div>

    </form>  
    
    
    <form id="frmCreateForwarder" runat="server" visible="false" enabled="false">
    
    <div><h2>
         <asp:ImageButton ID="btnLinkEmail" runat="server" 
             ImageUrl="~/images/createemail2.GIF" class="fl cl" style="height: 33px; width: 159px" />
             <img alt="" src="images/createforward.GIF" class="fl" style="height: 33px; width: 159px" />
             <asp:ImageButton
            ID="btnAlias2" runat="server" style="height: 33px; width: 159px" 
             class="fl" ImageUrl="~/images/addaliasdomain2.GIF" />
             </h2></div>
    <br /><br /><br />
    <table class="cl fl">
    <tr>
    <th><input type="hidden" name="action" value="createforwarder" /><label>Email Name:</label></th>
    <td><asp:TextBox ID="txtForwarder" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
    <th><label>Destintation:</label></th>
    <td><asp:TextBox ID="txtDestination" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
    <td colspan="2"><asp:Button class="submit" ID="btnCreateForwarder" runat="server" Text="Create Forwarder"  /></td>
    </tr>
    <tr>
    <td colspan="2">
    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
        ErrorMessage="Email Addresses are only allowed: A-Z, a-z, 0-9, ., _ and -" 
        ControlToValidate="txtForwarder" ValidationExpression="^[a-zA-Z0-9_.-]{2,30}"></asp:RegularExpressionValidator>
    <br /><asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
        ErrorMessage="Destination must be a full email address e.g. person@domain.com" 
        ControlToValidate="txtDestination" ValidationExpression="^[a-zA-Z0-9_.-]{2,30}\@[a-z0-9\-]{2,63}\.(co\.uk|com|org|org\.uk|edu|co|net)$"></asp:RegularExpressionValidator>
    </td>
    </tr>
    </table>
        
    </form>  
    
    
    
    <form id="frmAliasDomain" runat="server" visible="false" enabled="false">
    <div>
     <h2>
         <asp:ImageButton ID="btnLinkEmail2" runat="server" 
             class="fl cl" ImageUrl="~/images/createemail2.GIF" style="height: 33px; width: 159px" />
         <asp:ImageButton
            ID="btnForwarder2" runat="server" class="fl" style="height: 33px; width: 159px" 
             ImageUrl="~/images/createforward2.GIF" Width="122px" />
             <img alt="" src="images/addaliasdomain.GIF" class="fl" style="height: 33px; width: 159px" />
             </h2>    
             </div> 
<br /><br /><br />
<asp:Repeater ID="rptAlias" runat="server">
    <HeaderTemplate>
        <table border="1" class="cl fl">
        <tr><th>Alias Domain</th><th>Actions</th></tr>
    </HeaderTemplate>
    
    <ItemTemplate>
    
        <tr>
        
            <td>&nbsp;&nbsp;<%#DataBinder.Eval(Container.DataItem, "aliasDomain")%>&nbsp;&nbsp;</td><td><a href="Emails.aspx?domainID=<%=domainID %>&amp;aliasdomainID=<%#DataBinder.Eval(Container.DataItem, "aliasdomainid")%>&amp;delete=alias&amp;aliasdomain=<%#DataBinder.Eval(Container.DataItem, "aliasDomain")%>">Delete</a></td>
        
        </tr>
    
    </ItemTemplate>
    
    <FooterTemplate>
    <tr>
    <td><input type="hidden" name="action" value="createforwarder" /></td> 
    </tr>
        </table>
    </FooterTemplate>
    
    </asp:Repeater>

    <br /><br />
    
   <table class="cl fl">
   <tr>
   <th><label>Alias Domain:</label></th>
   <td><asp:TextBox ID="txtAliasDomain" runat="server"></asp:TextBox></td>
   </tr>
   <tr>
   <td colspan="2"><asp:Button class="submit" ID="btnCreateAlias" runat="server" Text="Create Alias Domain"  /></td>
   </tr>
   <tr>
    <td colspan="2"><asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" 
        ErrorMessage="This must be a valid domain e.g. RecallHosting.com" 
        ControlToValidate="txtAliasDomain" ValidationExpression="^[a-z0-9\-]{2,63}\.(co\.uk|com|org|org\.uk|edu|co|net)$"></asp:RegularExpressionValidator></td>
   </tr>
   </table>
         </form>   

	</div>
	</div>
<div id="secondary">
 
<img src="/images/side.jpg" alt="" id="lifestyle" />
 
</div>
 
</div>
<div id="navbar">
<ul id="nav">
<li class="first"><a href="default.aspx">Back to Domains</a></li>
</ul>
</div>
 
<div id="footer">
Copyright <%#Year(Now)%> RecallHosting - Developed by <a href="" title="">RecallHosting</a> - RecallHosting</div>
</div>
 
</body>

</html>

