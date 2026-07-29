<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCPrivilegiate.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondoAgo.UCPrivilegiate" %>
<asp:Panel ID="pnlPrivilegiate" runat="server">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="width: 30%">
                <label style="font-weight: bold">
                    Decorrenza Registrazione:</label>
            </td>
            <td class="field" style="text-align: left; width: 25%">
                <asp:Label runat="server" ID="lblDecorrenzaRegistrazione" Width="50%"></asp:Label>
            </td>
            <td style="width: 45%">
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione" cellpadding="3" cellspacing="1" border="0" width="100%">
        <tr>
            <td class="Row1" style="width: 28%">
                <label>
                    Indennità di ausiliaria:</label>
            </td>
            <td class="Row1" style="width: 72%">
                <asp:DropDownList runat="server" ID="ddlIndennitaAusiliaria" Width="20%" CssClass="tb8 txtUppercase"
                    TabIndex="1">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 28%">
                <label>
                    Indennità paraplegici:</label>
            </td>
            <td class="Row1" style="width: 72%">
                <asp:DropDownList runat="server" ID="ddlIndennitaParaplegici" Width="20%" CssClass="tb8 txtUppercase"
                    TabIndex="1">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 28%">
                <label>
                    Indennità speciale:</label>
            </td>
            <td class="Row1" style="width: 72%">
                <asp:DropDownList runat="server" ID="ddlIndennitaSpeciale" Width="20%" CssClass="tb8 txtUppercase"
                    TabIndex="1">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
    </table>
    <div style="width: 720px; margin-top: 100px; margin-right: 40px;">
        <table width="100%">
            <tr>
                <td style="text-align: center">
                    <asp:Button ID="btnSalvaPrivilegiate" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Salva Privilegiate" Width="150px" OnClick="btnSalvaPrivilegiate_Click"
                        OnClientClick="if(Page_ClientValidate('UCTabPrivilegiate')){aspnetForm.target ='_self'; BlockUI();}" />
                    <asp:Button ID="btnEliminaPrivilegiate" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elimina Privilegiate" TabIndex="10" Width="150px" OnClick="btnEliminaPrivilegiate_Click"
                        OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare Privilegiate?')) return false; else BlockUI();" />
                    <asp:Button ID="btnTornaElencoRegistrazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elenco Registrazioni" Width="150px" OnClick="TornaElencoRegistrazioni_Click"
                        OnClientClick="BlockUI();" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
