<%@ Control Language="c#" AutoEventWireup="false"  Inherits="ASPSlider.ASPSlider" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<script runat="server">

    
    protected void TextBox1_TextChanged(object sender, EventArgs e)
    {

    }
</script>
<script type="text/javascript">
function updateValue(inpt) {
	document.getElementById('txtSliderPosTxt').value=inpt.value;
}
</script>
<!-- Copy rights Ashish Patil .Refer to License.txt for more information-->
<table onmousemove="divMouseOver()" id="tblMain" style="VISIBILITY: visible;POSITION: relative;HEIGHT: 20px;width:420px"
	onclick="toggleMouseFlag()" borderColor="gray" cellSpacing="0" cellPadding="0" runat="server" width="420px">
	<tr height="20">
		<td id="tdSliderImg" onmousemove="divMouseOver()" style="background-size: 300px;width:100%;" vAlign="top" width="420px" colSpan="3">
			<asp:image id="imgSlider" style="POSITION: relative" runat="server"></asp:image></td>
			
	</tr>
	<tr id="trSliderText">
		<td id="tdMin" style="font-size: 150%" class="" width="8%"></td>
		<td align="center"><asp:textbox style="display:none;" id="TextBox1" runat="server" width="1px" 
                ReadOnly="True" ontextchanged="TextBox1_TextChanged"></asp:textbox></td>
		<td id="tdMax" style="font-size: 150%" width="8%"></td>
	</tr>
	<tr>
		<td colspan="2" style="font-size: 150%">Actual Mailbox Size:</td><td colspan="2" ><input type="text" style="width: 100px;font-size: 150%;height:12px;" id="inmegabytes" name="inmegabytes" readonly="readonly" value="" /></td>
	</tr>
</table>
<asp:literal id="Literal1" runat="server"></asp:literal><INPUT id="txtSliderPos" type="hidden" runat="server" />
