<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCPrecedentePensioneCi.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCPrecedentePensioneCi" %>

<script type="text/javascript">

    $(document).ready(function() {
    var availableTags = document.getElementById("ctl00_ContentPlaceHolder1_HiddenFieldSedi").value.split(';');
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
    
    function CheckSede(val, args) {
        if (args.Value == "") {
            args.IsValid = false;
            return;
        }
        if (document.getElementById("ctl00_ContentPlaceHolder1_HiddenFieldSedi") != null) {
            var availableTags = document.getElementById("ctl00_ContentPlaceHolder1_HiddenFieldSedi").value.split(';');
            for (var i = 0; i < availableTags.length; i++) {
                if (args.Value.toUpperCase() == availableTags[i]) {
                    return;
                }
            }
        }
        args.IsValid = false;
        return;
    }
</script>

<asp:Panel runat="server" ID="pnlPrecedentePensione">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="width: 25%;">
                <label>
                    Codice Categoria:
                </label>
            </td>
            <td class="field" style="width: 25%;">
                <asp:DropDownList runat="server" ID="ddlCodiceP18" Width="75px" CssClass="tb8 txtUppercase" TabIndex="1">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="width: 25%;">
            </td>
            <td style="width: 25%;">
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Sede:</label>
            </td>
            <td class="field">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtSede" Width="140px" CssClass="txtUppercase tb8"
                    MaxLength="4" TabIndex="2"></asp:TextBox>
                <asp:CustomValidator runat="server" ID="CustomValidatorSede" ControlToValidate="txtSede"
                    ValidationGroup="UCTabPrecedentePensione" ErrorMessage="La sede selezionata non è valida" ClientValidationFunction="CheckSede"
                    Text="*" CssClass="field-is-required" Display="Dynamic" Enabled="false"></asp:CustomValidator>
            </td>
            <td class="Row1">
                <label>
                    Certificato:</label>
            </td>
            <td class="field">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtCertificato" Width="140px"
                    CssClass="txtUppercase tb8" MaxLength="8" TabIndex="3" onblur="extractNumber(this,0,false);"
                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateTxtCertificato" ControlToValidate="txtCertificato"
                    ErrorMessage="Numero di certificato non valido" ValidationExpression="^[0-9]{8}$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabPrecedentePensione" />
            </td>
        </tr>
        <tr id="TrDecOrig" runat="server">
            <td class="Row1" >
                <label>
                    Decorrenza Originaria:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDecOriginaria"
                    Width="95px" CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="4" Text="mm/aaaa"
                    MaxLength="7"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateDecOriginaria" ControlToValidate="txtDecOriginaria"
                    Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per Decorrenza Originaria"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabPrecedentePensione"
                    Text="*" CssClass="field-is-required" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecOriginaria" Display="Dynamic"
                    ErrorMessage="Decorrenza Originaria: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabPrecedentePensione"
                    ID="customCheckDataDecorrenzaOriginaria" ClientValidationFunction="checkCorrettezzaData" />  
            </td>
            
        </tr>
        <tr runat="server" id="TrDecCarico">
            <td class="Row1">
                <label>
                    Decorrenza Carico:</label>
            </td>
            <td class="field full-grid" colspan="3">
               <asp:TextBox Style="text-align: left" runat="server" ID="txtDecCarico"
                    Width="95px" CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="5" Text="mm/aaaa"
                    MaxLength="7"></asp:TextBox>
             <asp:RegularExpressionValidator runat="server" ID="validateDecCarico" ControlToValidate="txtDecCarico"
                    Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per Decorrenza Carico"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabPrecedentePensione"
                    Text="*" CssClass="field-is-required" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecCarico" Display="Dynamic"
                    ErrorMessage="Decorrenza Carico: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabPrecedentePensione"
                    ID="customCheckDataDecorrenzaCarico" ClientValidationFunction="checkCorrettezzaData" />  
            </td>
        
        </tr>
    </table>
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" class=" tab-actions-group">
            <tr>
                <td style="text-align: right" id="TdBtnSalvaPrecedentePensione" runat="server" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaPrecedentePensione" runat="server" SkinID="btnAzione1" CausesValidation="false" Enabled="true" Text="Salva Pens. Prov."
                        Width="170px" OnClick="SalvaPrecedentePensione_Click" OnClientClick="if(Page_ClientValidate('UCTabPrecedentePensione')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary"/>
                </td>
                <td style="text-align: left" id="TdBtnEliminaPrecedentePensione" runat="server">
                    <asp:Button ID="btnEliminaPrecedentePensione" runat="server" SkinID="btnAzione1" Enabled="true" Text="Elimina Pens. Prov." Width="170px"
                        CausesValidation="false" OnClick="EliminaPrecedentePensione_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Precedente Pensione?')) return false; else BlockUI();" CssClass="tertiary"/>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>

<asp:HiddenField runat="server" ID="HiddenFieldSedi"/>