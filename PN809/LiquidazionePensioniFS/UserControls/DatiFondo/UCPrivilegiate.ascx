<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCPrivilegiate.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondo.UCPrivilegiate" %>

<table class="tabellaFormattazione grid grid-size-25">
    <tr>
        <td class="Row1" style="width: 30%">
            <label style="font-weight: bold">
                Decorrenza Registrazione:</label>
        </td>
        <td class="field" style="text-align: left; width: 25%">
            <asp:Label runat="server" ID="lblDecorrenzaRegistrazione" Width="50%"></asp:Label>
        </td>
        <td style="width: 45%"></td>
    </tr>
</table>
   <asp:Panel runat="server" ID="pnlEquoIndenizzo" Visible="false">
        <table class="tabellaFormattazione grid grid-size-25" cellpadding="3" cellspacing="1" border="0" width="100%">
            <tr>
                <td class="Row1 shift-full-grid" style="width: 30%" colspan="2">
                    <label style="font-weight: bold">
                        Equo Indennizzo:</label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 28%">
                    <label>
                        Ente Equo Indennizzo:</label>
                </td>
                <td class="Row1" style="width: 22%">
                    <asp:TextBox runat="server" ID="txtEnteEquoIndennizzo" CssClass="txtUppercase tb8 offClass onClassLegge336"
                        TabIndex="18" MaxLength="11" Width="75%" />
                </td>
                <td class="Row1" style="width: 28%">
                    <label id="label1" runat="server">
                        Importo 50% Equo Indenizzo:</label>
                </td>
                <td class="Row1" style="width: 22%">
                    <asp:TextBox runat="server" ID="txtImportoEquoIndennizzo" CssClass="txtUppercase tb8 offClass onClassLegge336"
                        TabIndex="18" MaxLength="20" Width="75%" />
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoEquoIndenizzo" Display="Dynamic"
                        ControlToValidate="txtImportoEquoIndennizzo" Enabled="true" ErrorMessage="Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabPrivilegiate" ValidationExpression="\d+(\,\d{1,4})?" />
                </td>
            </tr>
            </table>
        </asp:Panel>
    <asp:Panel ID="pnlPrivilegiate" runat="server" Visible="false">
    <table class="tabellaFormattazione grid grid-size-25" cellpadding="3" cellspacing="1" border="0" width="100%">
             <tr>
                <td class="Row1 shift-full-grid" style="width: 30%" colspan="2">
                    <label style="font-weight: bold">
                        Particolari Disposizioni :</label>
                </td>
            </tr>
        <tr>
            <td class="Row1" style="width: 28%">
                <label>
                    Super Invalidità:</label>
            </td>
            <td class="Row1" style="width: 72%">
                <asp:DropDownList runat="server" ID="ddlInvalidita" Width="97%" CssClass="tb8 txtUppercase"
                    TabIndex="1">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlInvalidita_RF" ControlToValidate="ddlInvalidita"
                    Display="Dynamic" Enabled="true" ErrorMessage="Super Invalidità: dato obbligatorio"
                    ValidationGroup="UCTabPrivilegiate" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 28%">
                <label>
                    Assegno Integrativo:</label>
            </td>
            <td class="Row1" style="width: 72%">
                <asp:DropDownList runat="server" ID="ddlAssegnoIntegrativo" Width="97%" CssClass="tb8 txtUppercase"
                    TabIndex="2">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlAssegnoIntegrativo_RF" ControlToValidate="ddlAssegnoIntegrativo"
                    Display="Dynamic" Enabled="true" ErrorMessage="Assegno Integrativo: dato obbligatorio"
                    ValidationGroup="UCTabPrivilegiate" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 28%">
                <label>
                    Integrazione Indennità Assistenza:</label>
            </td>
            <td class="Row1" style="width: 72%">
                <asp:DropDownList runat="server" ID="ddlIntegrazioneIndennita" Width="97%" CssClass="tb8 txtUppercase"
                    TabIndex="3">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlIntegrazioneIndennita_RF" ControlToValidate="ddlIntegrazioneIndennita"
                    Display="Dynamic" Enabled="true" ErrorMessage="Integrazione Indennità Assistenza: dato obbligatorio"
                    ValidationGroup="UCTabPrivilegiate" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 28%">
                <label>
                    Indennità Accompagnamento Aggiuntiva:</label>
            </td>
            <td class="Row1" style="width: 72%">
                <asp:DropDownList runat="server" ID="ddlIndennitaAccompagno" Width="10%" CssClass="tb8 txtUppercase"
                    TabIndex="4">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlIndennitaAccompagno_RF" ControlToValidate="ddlIndennitaAccompagno"
                    Display="Dynamic" Enabled="true" ErrorMessage="Indennità Accompagnamento Aggiuntiva: dato obbligatorio"
                    ValidationGroup="UCTabPrivilegiate" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 28%">
                <label>
                    Cumulo Infermità:</label>
            </td>
            <td class="Row1" style="width: 72%">
                <asp:DropDownList runat="server" ID="ddlCumulo" Width="97%" CssClass="tb8 txtUppercase"
                    TabIndex="5">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlCumulo_RF" ControlToValidate="ddlCumulo"
                    Display="Dynamic" Enabled="true" ErrorMessage="Cumulo Infermità: dato obbligatorio"
                    ValidationGroup="UCTabPrivilegiate" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 28%">
                <label>
                    Categoria 2° Infermità:</label>
            </td>
            <td class="Row1" style="width: 72%">
                <asp:DropDownList runat="server" ID="ddlInfermita" Width="97%" CssClass="tb8 txtUppercase"
                    TabIndex="6">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlInfermita_RF" ControlToValidate="ddlInfermita"
                    Display="Dynamic" Enabled="true" ErrorMessage="Categoria 2° Infermità: dato obbligatorio"
                    ValidationGroup="UCTabPrivilegiate" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 28%">
                <label>
                    Assegno di Cura:</label>
            </td>
            <td class="Row1" style="width: 72%">
                <asp:DropDownList runat="server" ID="ddlAssegnoCura" Width="97%" CssClass="tb8 txtUppercase"
                    TabIndex="7">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlAssegnoCura_RF" ControlToValidate="ddlAssegnoCura"
                    Display="Dynamic" Enabled="true" ErrorMessage="Assegno di Cura: dato obbligatorio"
                    ValidationGroup="UCTabPrivilegiate" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 28%">
                <label>
                    Indennità Speciale Annua:</label>
            </td>
            <td class="Row1" style="width: 72%">
                <asp:DropDownList runat="server" ID="ddlIndennitaSpeciale" Width="10%" CssClass="tb8 txtUppercase"
                    TabIndex="8">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlIndennitaSpeciale_RF" ControlToValidate="ddlIndennitaSpeciale"
                    Display="Dynamic" Enabled="true" ErrorMessage="Indennità Speciale Annua: dato obbligatorio"
                    ValidationGroup="UCTabPrivilegiate" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlPrivilegiateINPDAP" runat="server" Visible="false">
    <table class="tabellaFormattazione grid grid-size-25" cellpadding="3" cellspacing="1" border="0" width="100%">
        <tr>
            <td class="Row1" style="width: 28%">
                <label>
                    Indennità di ausiliaria:</label>
            </td>
            <td class="Row1" style="width: 72%">
                <asp:DropDownList runat="server" ID="ddlIndennitaAusiliaria" Width="20%" CssClass="tb8 txtUppercase xxs"
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
                <asp:DropDownList runat="server" ID="ddlIndennitaParaplegici" Width="20%" CssClass="tb8 txtUppercase xxs"
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
                <asp:DropDownList runat="server" ID="ddlIndennitaSpecialeINPDAP" Width="20%" CssClass="tb8 txtUppercase xxs"
                    TabIndex="1">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Panel>
<div style="margin-top: 100px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: center" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaPrivilegiate" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Privilegiate" Width="180px" OnClick="btnSalvaPrivilegiate_Click"
                    OnClientClick="if(Page_ClientValidate('UCTabPrivilegiate')){aspnetForm.target ='_self'; BlockUI();}"
                    TabIndex="9" CssClass="force-right primary" />
                <asp:Button ID="btnEliminaPrivilegiate" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Privilegiate" TabIndex="10" Width="180px" OnClick="btnEliminaPrivilegiate_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare Privilegiate?')) return false; else BlockUI();" CssClass="ghost-delete"/>
                <asp:Button ID="btnTornaElencoRegistrazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elenco Registrazioni" Width="180px" OnClick="TornaElencoRegistrazioni_Click"
                    OnClientClick="BlockUI();" CssClass="tertiary" />
            </td>
        </tr>
    </table>
</div>
