<%@ Page Language="VB" AutoEventWireup="false" enableViewStateMac="false" CodeFile="Domain.aspx.vb" Inherits="Domain" %>


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
<a href="><img src="/images/logo.gif" alt="" id="logo" /></a>
 
 
 
<div id="contactdetailstop">

</div>
</div>

<div id="main">
 
<div id="primary">
 
	<h1 id="welcomemessage">RecallHosting Email Control Panel v0.2 Beta</h1>
 
	<div class="textcontainer">
 
       <form id="form1" runat="server">
    
    <h2>Domain(s):</h2>
    
    <asp:Repeater ID="Repeater1" runat="server">
    <HeaderTemplate>
        <table border="1" >
    </HeaderTemplate>
    
    <ItemTemplate>
    
        <tr>
        
            <th><%#DataBinder.Eval(Container.DataItem, "domainName")%></th><td><a href="Emails.aspx?domainID=<%# DataBinder.Eval(Container.DataItem, "domainID") %>">Select</a></td>
        
        </tr>
    
    </ItemTemplate>
    
    <FooterTemplate>
        </table>
    </FooterTemplate>
    
    </asp:Repeater>

    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString="<%$ ConnectionStrings:emailinterfacealphaConnectionString %>" 
        SelectCommand="sp_getdomain" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:FormParameter FormField="txtDomain" Name="strDomain" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    </form>

    <br />
    <div id="msg" runat="server" style="font-size: large; color: #008000">
    </div>  
 
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

