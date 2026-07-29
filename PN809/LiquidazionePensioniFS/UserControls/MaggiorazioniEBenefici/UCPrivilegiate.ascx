<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCPrivilegiate.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBenefici.UCPrivilegiate" %>

<asp:Panel ID="pnlPrivilegiateCommon" runat="server">
    <table class="tabellaFormattazione" cellpadding="3" cellspacing="1" border="0" width="100%">
        <tr>
            <td colspan="2" style="height:5px"\>                
        </tr>
        <tr>
            <td class="Row1" style="width:28%">
                <label>Super Invalidità:</label>
            </td>
            <td class="Row1" style="width:72%">
                <asp:DropDownList runat="server" ID="ddlInvalidita" Width="97%" CssClass="tb8 txtUppercase" TabIndex="1">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlInvalidita_RF" ControlToValidate="ddlInvalidita"
                 Display=Dynamic Enabled="true" ErrorMessage="Super Invalidità: dato obbligatorio" ValidationGroup="UCTabPrivilegiate"
                  Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width:28%">
                <label>Assegno Integrativo:</label>
            </td>
            <td class="Row1" style="width:72%">
                <asp:DropDownList runat="server" ID="ddlAssegnoIntegrativo" Width="97%" CssClass="tb8 txtUppercase" TabIndex="2">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlAssegnoIntegrativo_RF" ControlToValidate="ddlAssegnoIntegrativo"
                 Display="Dynamic" Enabled="true" ErrorMessage="Assegno Integrativo: dato obbligatorio" ValidationGroup="UCTabPrivilegiate"
                  Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width:28%">
                <label>Integrazione Indennità Assistenza:</label>
            </td>
            <td class="Row1" style="width:72%">
                <asp:DropDownList runat="server" ID="ddlIntegrazioneIndennita" Width="97%" CssClass="tb8 txtUppercase" TabIndex="3">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlIntegrazioneIndennita_RF" ControlToValidate="ddlIntegrazioneIndennita"
                 Display="Dynamic" Enabled="true" ErrorMessage="Integrazione Indennità Assistenza: dato obbligatorio" ValidationGroup="UCTabPrivilegiate"
                  Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width:28%">
                <label>Indennità Accompagnamento Aggiuntiva:</label>
            </td>
            <td class="Row1" style="width:72%">
                <asp:DropDownList runat="server" ID="ddlIndennitaAccompagno" Width="10%" CssClass="tb8 txtUppercase" TabIndex="4">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlIndennitaAccompagno_RF" ControlToValidate="ddlIndennitaAccompagno"
                 Display="Dynamic" Enabled="true" ErrorMessage="Indennità Accompagnamento Aggiuntiva: dato obbligatorio" ValidationGroup="UCTabPrivilegiate"
                  Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width:28%">
                <label>Cumulo Infermità:</label>
            </td>
            <td class="Row1" style="width:72%">
                <asp:DropDownList runat="server" ID="ddlCumulo" Width="97%" CssClass="tb8 txtUppercase" TabIndex="5">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlCumulo_RF" ControlToValidate="ddlCumulo"
                 Display=Dynamic Enabled="true" ErrorMessage="Cumulo Infermità: dato obbligatorio" ValidationGroup="UCTabPrivilegiate"
                  Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width:28%">
                <label>Categoria 2° Infermità:</label>
            </td>
            <td class="Row1" style="width:72%">
                <asp:DropDownList runat="server" ID="ddlInfermita" Width="97%" CssClass="tb8 txtUppercase" TabIndex="6">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlInfermita_RF" ControlToValidate="ddlInfermita"
                 Display=Dynamic Enabled="true" ErrorMessage="Categoria 2° Infermità: dato obbligatorio" ValidationGroup="UCTabPrivilegiate"
                  Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width:28%">
                <label>Assegno di Cura:</label>
            </td>
            <td class="Row1" style="width:72%">
                <asp:DropDownList runat="server" ID="ddlAssegnoCura" Width="97%" CssClass="tb8 txtUppercase" TabIndex="7">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlAssegnoCura_RF" ControlToValidate="ddlAssegnoCura"
                 Display=Dynamic Enabled="true" ErrorMessage="Assegno di Cura: dato obbligatorio" ValidationGroup="UCTabPrivilegiate"
                  Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width:28%">
                <label>Indennità Speciale Annua:</label>
            </td>
            <td class="Row1" style="width:72%">
                <asp:DropDownList runat="server" ID="ddlIndennitaSpeciale" Width="10%" CssClass="tb8 txtUppercase" TabIndex="8">
                </asp:DropDownList>
                 <asp:RequiredFieldValidator runat="server" ID="ddlIndennitaSpeciale_RF" ControlToValidate="ddlIndennitaSpeciale"
                 Display=Dynamic Enabled="true" ErrorMessage="Indennità Speciale Annua: dato obbligatorio" ValidationGroup="UCTabPrivilegiate"
                  Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
</asp:Panel>

<div style="margin-top: 100px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%">
        <tr>
            <td style="text-align: right">
                <asp:Button ID="btnSalvaPrivilegiate" runat="server" SkinID="btnAzione1" CausesValidation="false" Enabled="true" Text="Salva Privilegiate" Width="180px" 
                    onclick="btnSalvaPrivilegiate_Click" OnClientClick="if(Page_ClientValidate('UCTabPrivilegiate')){aspnetForm.target ='_self'; BlockUI();}" TabIndex="9" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaPrivilegiate" runat="server" SkinID="btnAzione1" CausesValidation="false" Enabled="true" Text="Elimina Privilegiate" TabIndex="10"
                    Width="180px" onclick="btnEliminaPrivilegiate_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare Benefici?')) return false; else BlockUI();"/>
            </td>
        </tr>
    </table>
</div>