<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Welcome to RecallHosting Email Control Panel</title>
    <link href="/styles/preview.css" rel="stylesheet" type="text/css" />
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
 
	<h1 id="welcomemessage">Welcome to the RecallHosting Email Control Panel v0.2 Beta</h1>
 
	<div class="textcontainer">
 
        <form id="frmDomain" action="Domain.aspx" method="post" class="standard">
        <input type="hidden" name="action" value="find" />
        
        <table>
        <tr>
        <th><label id="lblSearch">Domain Name:</label></th>
        <td><input type="text" name="txtDomain" value="" /></td>
         </tr>
         <tr>
         <td colspan="2"><input type="submit" name="submit" style="float:right" class="submit" value="Find Domain" /></td>
                </tr>
        </table>
        </form>
 
         <form id="frmCreateDomain"  runat="server" action="Domain.aspx" method="post"  class="standard">
        <input type="hidden" name="action" value="create" />
        <table>
        <tr>
        <th><label id="Label2">Create Domain Name:</label></th>
        <td><asp:TextBox ID="txtDomain" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
        <td colspan="2"><asp:Button ID="btnCreate" style="float:right" class="submit" runat="server" Text="Create Domain" /></td>
        </tr>
        </table>
            

        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
            ErrorMessage="Domains must be in the format <name>.<TLD> e.g recallhosting.com" 
            ControlToValidate="txtDomain" ValidationExpression="^[a-z0-9\-]{2,63}\.(co\.uk|com|org|org\.uk|edu|co|net)$"></asp:RegularExpressionValidator>
        </form>  
 
	</div>
	</div>
<div id="secondary">
 
<img src="/images/side.jpg" alt="" id="lifestyle" />
 
</div>
 
</div>
<div id="navbar">

</div>
 
<div id="footer">
Copyright <%#Year(Now)%> RecallHosting - Developed by <a href="" title="">RecallHosting</a> - RecallHosting</div>
</div>
</div>
 
</body>

</html>

