<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCAvviso.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.UCAvviso" %>
<div id="divAvviso" runat="server" class="box-avviso">
    <table cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td style="width: 70px; vertical-align: top; text-align: center">
                <asp:Image ID="imgIcon" runat="server" />
            </td>
            <td style="width: 650px; vertical-align: middle;">
                <asp:Label ID="lblTitle" runat="server" CssClass="toast-title"></asp:Label>
                <asp:Label ID="lblMsg" runat="server" Font-Size="Medium"></asp:Label>
                <asp:ImageButton ID="imgClose" runat="server" src="../App_Themes/iFrame/Images/x.svg" OnClick="closeToast" CssClass="close-img" Visible="false" />
            </td>
        </tr>
    </table>
</div>
<p>
</p>
