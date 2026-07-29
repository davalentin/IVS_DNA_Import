<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCMaggiorazioni.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiAgo.UCMaggiorazioni" %>
<asp:Panel ID="pnlMaggiorazioneSociale" Visible="true" runat="server">
    <div id="Div1" style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; margin: 4px">
        <table class="tabellaFormattazione grid grid-size-20">
            <tr>
                <td class="Row1 shift-full-grid" style="width:100%" colspan="4">
                    <asp:Label runat="server" ID="lblMaggSociale" style="font-style: italic" CssClass="section-label">Maggiorazione Sociale</asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width:25%">
                    <label>Decorrenza:</label>
                </td>
                <td class="Row1" style="width:25%">
                    <asp:TextBox runat="server" ID="txtDecorrenza" CssClass="tb8 txtUppercase date-picker dateMMaaaa"
                        MaxLength="7" Width="100px" TabIndex="1" Text="MM/AAAA"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ID="txtDecorrenza_RF" ControlToValidate="txtDecorrenza"
                        Display="Dynamic" Enabled="true" ErrorMessage="Decorrenza Maggiorazioni: Campo obbligatorio"
                        ValidationGroup="UCTabMaggiorazioni" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator runat="server" ID="validateDecorrenza" ControlToValidate="txtDecorrenza"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}$" Enabled="true" Text="*" CssClass="field-is-required" ErrorMessage="Decorrenza Maggiorazioni: Formato non valido"
                        Display="Dynamic" ValidationGroup="UCTabMaggiorazioni" />                     
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabMaggiorazioni"
                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />  
                </td>
                <td class="Row1" style="width:25%" runat="server" id="tdLblCessazione">
                    <label>Cessazione:</label>
                </td>
                <td class="Row1" style="width:25%" runat="server" id="tdCessazione">
                    <asp:TextBox runat="server" ID="txtCessazione" CssClass="tb8 txtUppercase date-picker dateMMaaaa"
                        MaxLength="7" Width="100px" TabIndex="2" Text="MM/AAAA"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" ControlToValidate="txtCessazione"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" Enabled="true" Text="*" CssClass="field-is-required" ErrorMessage="Cessazione: Formato data non corretto"
                        Display="Dynamic" ValidationGroup="UCTabMaggiorazioni" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtCessazione" Display="Dynamic"
                        ErrorMessage="Cessazione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabMaggiorazioni"
                        ID="customCheckDataCessazione" ClientValidationFunction="checkCorrettezzaData" />  
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<asp:Panel ID="pnlAnniRid" runat="server" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width:25%">
                <label>Anni Riduzione benefici Art.38 finanziaria 2002:</label>
            </td>
            <td class="Row1 full-grid" colspan="3">
                <asp:TextBox runat="server" ID="txtAARidbenArt38" CssClass="tb8 txtUppercase" MaxLength="1" Width="10%" TabIndex="3"
                onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="validateTxtAARidbenArt38"
                ControlToValidate="txtAARidbenArt38" Display="Dynamic" ErrorMessage="Inserire il valore in un formato valido per Anni Riduzione benefici Art.38 finanziaria 2002"
                Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]+$" ValidationGroup="UCTabMaggiorazioni" />
            </td>
        </tr>
    </table>
</asp:Panel>

<div style="margin-top: 100px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnMaggiorazioni" runat="server" SkinID="btnAzione1" CausesValidation="false" Enabled="true" Text="Salva Maggiorazioni" 
                    Width="160px" OnClick="SalvaMaggiorazioni_Click" OnClientClick="if(Page_ClientValidate('UCTabMaggiorazioni')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary"/>
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaMaggiorazioni" runat="server" SkinID="btnAzione1" CausesValidation="false" Enabled="true" Text="Elimina Maggiorazioni" 
                    Width="160px" OnClick="EliminaMaggiorazioni_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare Maggiorazioni?')) return false; else BlockUI();" CssClass="ghost-delete"/>
            </td>
        </tr>
    </table>
</div>