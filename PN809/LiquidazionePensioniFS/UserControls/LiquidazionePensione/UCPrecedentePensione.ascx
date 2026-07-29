<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCPrecedentePensione.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCPrecedentePensione" %>

<script type="text/javascript">
    $(document).ready(function() {
        var availableTags = document.getElementById("<%=HiddenFieldSedi.ClientID%>").value.split(';');
        //alert(availableTags);
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


    function CleanFields2() {
        document.getElementById("<%=ddlCodiceP18.ClientID%>").value = '';
        document.getElementById("<%=txtSede.ClientID%>").value = '';
        document.getElementById("<%=txtCertificato.ClientID%>").value = '';
        return false;
    }
  
</script>

<!-- Pannello Precedente Pensione Comune -->
<asp:Panel runat="server" ID="pnlPrecedentePensioneComune">
    <table class="tabellaFormattazione grid">
        <tr>
            <td class="Row1" style="width:25%;">
                <label>
                    Codice Categoria:
                </label>
            </td>
            <td class="field" style="width:25%;">
                <asp:DropDownList runat="server" ID="ddlCodiceP18" Width="75px" CssClass="tb8 txtUppercase" TabIndex="1">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                </asp:DropDownList>
                <%--<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="ddlCodiceP18"
                    ErrorMessage="il campo Codice Categoria è obbligatorio" Text="*" CssClass="field-is-required" Display="Dynamic"
                    ValidationGroup="UCTabPrecedentePensione"/>--%>
            </td>
            <td style="width:25%;">
            </td>
            <td style="width:25%;">
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Certificato:</label>
            </td>
            <td class="field">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtCertificato" Width="140px"
                    CssClass="txtUppercase tb8" MaxLength="8" TabIndex="3"
                    onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                    onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateTxtCertificato" ControlToValidate="txtCertificato"
                    ErrorMessage="Numero di certificato non valido" ValidationExpression="^[0-9]{8}$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabPrecedentePensione" />
                <%--<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtCertificato"
                    ErrorMessage="il campo Certificato è obbligatorio" Text="*" CssClass="field-is-required" Display="Dynamic"
                    ValidationGroup="UCTabPrecedentePensione"/>--%>
            </td>
            <td class="Row1">
            </td>
            <td class="field">
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Sede:</label>
            </td>
            <td class="field">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtSede" Width="290px" CssClass="txtUppercase tb8" TabIndex="2"></asp:TextBox>
                <%--<asp:RegularExpressionValidator ID="validateTxtSede" ControlToValidate="txtSede"
                    ErrorMessage="Numero di sede non valido" ValidationExpression="([0-9]^4[-]+ [a-zA-Z]) | ([a-zA-Z]+)" runat="server"
                    Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabPrecedentePensione" />--%>
                <%--<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtSede"
                    ErrorMessage="il campo Sede è obbligatorio" Text="*" CssClass="field-is-required" Display="Dynamic"
                    ValidationGroup="UCTabPrecedentePensione"/>--%>
            </td>
            <td class="Row1">
            </td>
            <td class="field">
            </td>
        </tr>
    </table>
    <div style="width: 100%; margin-top: 25px; margin-right: 40px;">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaPrecedentePensione" runat="server" SkinID="btnAzione1" CausesValidation="false" Enabled="true" 
                        Text="Salva Precedente Pensione" Width="180px" OnClientClick="if(Page_ClientValidate('UCTabPrecedentePensione')){aspnetForm.target ='_self'; BlockUI();}"
                        OnClick="SalvaPrecedentePensione_Click" CssClass="primary" />

            <%--                <td style="text-align: Center">
                    <asp:Button ID="btnAnnulla" runat="server" SkinID="btnAzione1" OnClientClick="javascript:return CleanFields2();"
                        Enabled="true" Text="Pulisci" Width="100px" />
--%>                </td>
                    <td style="text-align: left">
                    <asp:Button ID="btnEliminaDatiPrecedPensione" runat="server" SkinID="btnAzione1" style="padding-left: 0px; padding-right: 0px;"
                            Enabled="true" Text="Elimina Precedente Pensione" Width="180px"
                        CausesValidation="false" 
                            OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Precedente Pensione?')) return false; else BlockUI();" 
                            onclick="btnEliminaDatiPrecedPensione_Click" CssClass="ghost-delete"/>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>

<asp:HiddenField runat="server" ID="HiddenFieldSedi"/>
<!-- Fine Pannello Precedente Pensione Comune -->
