<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCLegge460.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondo.UCLegge460" %>
<script type="text/javascript">
    $(document).ready(function () {
        var availableTags = document.getElementById("<%=HiddenFieldSedi.ClientID%>").value.split(';');
        $("#<%=txtSede.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTags,
            open: function () {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            }
        });
    });
</script>
<asp:Panel runat="server" ID="pnlLegge460">
    <table class="tabellaFormattazione grid grid-size-25">
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
    <div id="divBorder" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-bottom: 8px; margin-top: 4px;">
        <table class="tabellaFormattazione grid grid-size-25">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Categoria:</label>
                </td>
                <td class="Row1 full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlCodiceCategoria" Width="75px" CssClass="tb8"
                        TabIndex="26">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Sede:</label>
                </td>
                <td class="Row1 full-grid" colspan="3">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtSede" Width="50%" Text=""
                        CssClass="txtUppercase tb8" TabIndex="27" MaxLength="10"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Certificato:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtCertificato" Width="50%"
                        CssClass="txtUppercase tb8" MaxLength="8" TabIndex="28" onblur="extractNumber(this,0,false);"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validateTxtCertificato" ControlToValidate="txtCertificato"
                        ErrorMessage="Numero di certificato non valido" ValidationExpression="^[0-9]{8}$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabLegge460" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Numero Mesi Riscatti:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtnumMesiRiscatti" Width="50%"
                        Text="" CssClass="txtUppercase tb8" TabIndex="30" MaxLength="2"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionNumeroMesiRiscatti" ControlToValidate="txtnumMesiRiscatti"
                        ErrorMessage="Numero Mesi Riscatti: formato Mesi non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabLegge460" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Numero Mesi Totali:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtNumMesiTotali" Width="50%"
                        Text="" CssClass="txtUppercase tb8" TabIndex="31" MaxLength="2"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionNumeroMesiTotali" ControlToValidate="txtNumMesiTotali"
                        ErrorMessage="Numero Mesi Totali: formato Mesi non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabLegge460" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: center" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaDatiLegge460" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Dati Legge 4/60" Width="180px" OnClick="btnSalvaDatiLegge460_Click"
                    OnClientClick="if(Page_ClientValidate('UCTabLegge460')){aspnetForm.target ='_self'; BlockUI();}" CssClass="force-right primary" />
                <asp:Button ID="btnEliminaDatiLegge460" SkinID="btnAzione1" runat="server" Width="180px"
                    Text="Elimina Dati Legge 4/60" CausesValidation="False" OnClick="btnEliminaDatiLegge460_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Legge 4/60?')) return false; else BlockUI();" CssClass="ghost-delete"/>
                <asp:Button ID="btnTornaElencoRegistrazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elenco Registrazioni" Width="180px" OnClick="TornaElencoRegistrazioni_Click"
                    OnClientClick="BlockUI();" />
            </td>
        </tr>
    </table>
</div>
<asp:HiddenField runat="server" ID="HiddenFieldSedi" />
