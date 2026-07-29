<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCExCombattente.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCExCombattente" %>

<asp:Panel ID="pnlExCombattente" runat="server">
    <div id="pdivL140" style="border-style: solid; border-color: #000080; border-collapse:collapse; border-width:1px; 
        width: 710px; margin-left:4px; margin-top:4px; margin-bottom:4px" runat="server">
        <table class="tabellaFormattazione grid grid-size-20">
            <tr>
                <td class="Row1" style="text-align:left" colspan="2">
                    <asp:Label ID="lblTitoloLegge140" runat="server" Text="L. 140" style="font-weight: bold" CssClass="section-label"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione grid grid-size-20" cellpadding="3" cellspacing="1" border="0" width="100%">
            <tr>
                <td class="Row1" style="width:30%">
                    <label>Cieco / Ex Combattente:</label>
                </td>
                <td class="Row1" style="width:70%">
                    <asp:DropDownList CssClass="txtUppercase tb8" ID="ddlExCombattente" runat="server" TabIndex="1" Width="480px">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="ddlExCombattente"
                        Enabled="false" ErrorMessage="Codice Ex Combattente obbligatorio" Text="*" CssClass="field-is-required" Display="Dynamic"
                        ValidationGroup="UCTabExCombattente"/>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width:30%">
                    <label>Decorrenza:</label>
                </td>
                <td class="Row1" style="width:70%">
                    <asp:TextBox runat="server" ID="txtDecorrenza" CssClass="tb8 txtUppercase date-picker dateMMaaaa"
                        MaxLength="7" Width="100px" TabIndex="2" Text="MM/AAAA"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="txtDecorrenzaValidator"
                        ControlToValidate="txtDecorrenza" Display="Dynamic" Enabled="true" ErrorMessage="Inserire un formato data valido per Decorrenza Cieco / Ex Combattente"
                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabExCombattente" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabExCombattente"
                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />  
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>

<div style="margin-top: 100px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaExCombattente" runat="server" SkinID="btnAzione1" CausesValidation="false" Enabled="true" Text="Salva Ex Combattente" 
                    Width="170px" OnClick="SalvaExCombattente_Click" OnClientClick="if(Page_ClientValidate('UCTabExCombattente')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary"/>
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaExCombattente" runat="server" SkinID="btnAzione1" CausesValidation="false" Enabled="true" Text="Elimina Ex Combattente" 
                    Width="170px" OnClick="EliminaExCombattente_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare Ex Combattente?')) return false; else BlockUI();" CssClass="ghost-delete"/>
            </td>
        </tr>
    </table>
</div>
