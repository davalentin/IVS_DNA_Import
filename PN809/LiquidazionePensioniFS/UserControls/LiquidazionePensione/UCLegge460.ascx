<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCLegge460.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCLegge460" %>

<script type="text/javascript">
    $(document).ready(function() {
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

<asp:Panel runat="server" ID="pnlLegge460" Visible="true">
    <div id="divBorder" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-bottom: 8px; margin-top: 4px;">
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Categoria:</label>
                </td>
                <td class="Row1 full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlCodiceCategoria" Width="75px" CssClass="tb8 txtUppercase"
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
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaSecondaria"
                        Width="50%" MaxLength="10" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" TabIndex="29" />
                    <asp:RegularExpressionValidator ID="RegularExpressionValidatorDecorrenzaSencodaria" ControlToValidate="txtDecorrenzaSecondaria"
                        ErrorMessage="Decorrenza: data in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabLegge460"
                        Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaSecondaria" Display="Dynamic"
                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabLegge4_60"
                        ID="customCheckDataDecorrenzaSecondaria" ClientValidationFunction="checkCorrettezzaData" />  
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

<div style="width: 100%; margin-top: 25px; margin-right: 40px;">
    <table width="100%">
        <tr>
            <td style="text-align: right">
                <asp:Button ID="btnSalvaDatiLegge460" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Dati Legge 4/60" Width="180px" OnClick="btnSalvaDatiLegge460_Click"
                    OnClientClick="if(Page_ClientValidate('UCTabLegge4_60')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaDatiLegge460" SkinID="btnAzione1" runat="server" Width="180px"
                    Text="Elimina Dati Legge 4/60" CausesValidation="False" OnClick="btnEliminaDatiLegge460_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Legge 4/60?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>

<asp:HiddenField runat="server" ID="HiddenFieldSedi" />