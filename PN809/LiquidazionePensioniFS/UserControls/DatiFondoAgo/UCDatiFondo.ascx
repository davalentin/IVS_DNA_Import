<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiFondo.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondoAgo.UCDatiFondo" %>
<script type="text/javascript">
    function getDecorrenzaRegistrazione() {
        // Se non è visibile la textbox allora sarà visibile la label
        var decorrenza = document.getElementById("<%= txtDecorrenzaRegistrazione.ClientID %>");
        if (decorrenza)
            return decorrenza.value;

        decorrenza = document.getElementById("<%= lblDecorrenzaRegistrazione.ClientID %>");
        if (decorrenza)
            return decorrenza.outerText;

        return "";
    }
</script>
<asp:Panel runat="server" ID="pnlDatiFondo">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="width: 30%">
                <label style="font-weight: bold">
                    Decorrenza Registrazione:</label>
            </td>
            <td class="field" style="text-align: left; width: 25%">
                <asp:TextBox runat="server" ID="txtDecorrenzaRegistrazione" Width="50%" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA"
                    MaxLength="10"></asp:TextBox>
                <asp:Label runat="server" ID="lblDecorrenzaRegistrazione" Visible="false"></asp:Label>
                <asp:RegularExpressionValidator ID="REVtxtDecorrenzaRegistrazione" ControlToValidate="txtDecorrenzaRegistrazione"
                    ErrorMessage="Decorrenza Registrazione in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiFondo" Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaRegistrazione"
                    Display="Dynamic" ErrorMessage="Decorrenza Registrazione: data illogica" Text="*"
                    ValidationGroup="UCTabDatiFondo" ID="customCheckDatatxtDecorrenzaRegistrazione"
                    ClientValidationFunction="checkCorrettezzaData" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtDecorrenzaRegistrazione" Display="Dynamic"
                    ErrorMessage="Decorrenza Registrazione: campo obbligatorio" Text="*" ValidationGroup="UCTabDatiFondo"
                    ControlToValidate="txtDecorrenzaRegistrazione"></asp:RequiredFieldValidator>
            </td>
            <td style="width: 45%">
            </td>
        </tr>
    </table>
    <div id="divBorder" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-bottom: 8px; margin-top: 4px;">
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Tipo Pensione:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:Label runat="server" ID="lblTipoPensione"></asp:Label>
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza Pensione:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:Label runat="server" ID="lblDecorrenzaPensione"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        Decorrenza Calcolo:</label>
                </td>
                <td class="field">
                    <asp:Label runat="server" ID="lblDecorrenzaCalcolo"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Tredicesima Mensilità:</label>
            </td>
            <td class="field" colspan="3">
                <asp:DropDownList runat="server" ID="ddlTredicesimaMens" Width="10%" CssClass="tb8 txtUppercase"
                    Enabled="false">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="RFVddlTredicesimaMens" Display="Dynamic"
                    ErrorMessage="Tredicesima Mensilità: campo obbligatorio" Text="*" ValidationGroup="UCTabDatiFondo"
                    ControlToValidate="ddlTredicesimaMens"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlIntegrazioneMinimo" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Integrazione Minimo:</label>
                </td>
                <td class="Row1" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlIntegrazioneMinimo" Width="10%" CssClass="tb8 txtUppercase">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Indennità Integrativa Speciale Conglobata:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:DropDownList runat="server" ID="ddlIndennIntegrSpecConglobata" Width="30.5%"
                    CssClass="tb8 txtUppercase" TabIndex="20" Enabled="false">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="Row1" style="width: 25%">
            </td>
            <td class="Row1" style="width: 25%">
            </td>
        </tr>
    </table>
    <div style="width: 720px; margin-top: 25px; margin-right: 40px;">
        <table width="100%">
            <tr>
                <td style="text-align: center">
                    <asp:Button ID="btnSalvaDatiFondo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Salva Dati Fondo" Width="150px" OnClick="SalvaDatiFondo_Click"
                        OnClientClick="if(Page_ClientValidate('UCTabDatiFondo')){aspnetForm.target ='_self'; BlockUI();}" />
                    <asp:Button ID="btnEliminaDatiFondo" SkinID="btnAzione1" runat="server" Width="150px"
                        Text="Elimina Dati Fondo" CausesValidation="False" OnClick="btnEliminaDatiFondo_Click"
                        OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Fondo?')) return false; else BlockUI();"
                        Enabled="true" />
                    <asp:Button ID="btnTornaElencoRegistrazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elenco Registrazioni" Width="150px" OnClick="TornaElencoRegistrazioni_Click"
                        OnClientClick="BlockUI();" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
