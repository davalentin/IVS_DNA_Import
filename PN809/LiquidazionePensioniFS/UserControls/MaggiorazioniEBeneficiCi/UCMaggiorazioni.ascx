<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCMaggiorazioni.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCMaggiorazioni" %>
<asp:Panel ID="pnlMaggiorazioneSociale" Visible="true" runat="server">
    <div id="Div1" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; margin: 4px">
        <table class="tabellaFormattazione grid grid-size-20">
            <tr>
                <td class="Row1 shift-full-grid" style="width: 100%" colspan="4">
                    <asp:Label runat="server" ID="lblMaggSociale" Style="font-style: italic" CssClass="section-label">Maggiorazione Sociale</asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza:</label>
                </td>
                <td class="Row1" style="width: 25%">
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
                <td class="Row1" style="width: 25%" runat="server" id="tdLblCessazione">
                    <label>
                        Cessazione:</label>
                </td>
                <td class="Row1" style="width: 25%" runat="server" id="tdCessazione">
                    <asp:TextBox runat="server" ID="txtCessazione" CssClass="tb8 txtUppercase date-picker dateMMaaaa"
                        MaxLength="7" Width="100px" TabIndex="2" Text="MM/AAAA"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" ControlToValidate="txtCessazione"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" Enabled="true"
                        Text="*" CssClass="field-is-required" ErrorMessage="Cessazione: Formato data non corretto" Display="Dynamic"
                        ValidationGroup="UCTabMaggiorazioni" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtCessazione" Display="Dynamic"
                        ErrorMessage="Cessazione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabMaggiorazioni"
                        ID="customCheckDataCessazione" ClientValidationFunction="checkCorrettezzaData" />  
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%" runat="server" id="tdLblAnniRiduzioneEta">
                <label>
                    Anni Riduzione Età:</label>
                </td>
                <td class="Row1" style="width: 25%" runat="server" id="tdAnniRiduzioneEta">
                    <asp:TextBox runat="server" ID="txtAnniRiduzioneEta" CssClass="tb8 txtUppercase"
                        MaxLength="1" Width="12px" TabIndex="4" Text=""></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidatorAnniRiduzioneEta" ControlToValidate="txtAnniRiduzioneEta"
                        ValidationExpression="^[0-9]{1}$" Enabled="true" Text="*" CssClass="field-is-required" ErrorMessage="Anni Riduzione Età: inserire il numero di anni in un formato valido" 
                        Display="Dynamic" ValidationGroup="UCTabMaggiorazioni" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%" runat="server">
                <label>
                    Req.Art.2 Com.3 DL.503/92:</label>
                </td>
                <td class="Row1 full-grid" colspan="3" runat="server">
                    <asp:DropDownList runat="server" ID="ddlReqArt2Com3DL50392" Width="90%" CssClass="tb8 txtUppercase" TabIndex="3"></asp:DropDownList>
                </td>
            </tr>
        </table>
    </div>
    <div id="Div2" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; margin: 4px">
        <table class="tabellaFormattazione grid grid-size-20">
            <tr>
                <td class="Row1" style="width: 100%" colspan="4">
                    <asp:Label runat="server" ID="lblMaggL140" Style="font-style: italic">Maggiorazione Legge 140</asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox runat="server" ID="txtDecorrenzaMaggiorazioneLegge140" CssClass="tb8 txtUppercase date-picker dateMMaaaa"
                        MaxLength="7" Width="100px" TabIndex="5" Text="MM/AAAA"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="txtDecorrenza"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" Enabled="true" Text="*" CssClass="field-is-required" ErrorMessage="Decorrenza Maggiorazioni: Formato non valido"
                        Display="Dynamic" ValidationGroup="UCTabMaggiorazioni" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaMaggiorazioneLegge140" Display="Dynamic"
                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabMaggiorazioni"
                        ID="customCheckDataDecorrenzaMaggiorazioneLegge140" ClientValidationFunction="checkCorrettezzaData" />  
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<div style="margin-top: 100px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnMaggiorazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Maggiorazioni" Width="170px" OnClick="SalvaMaggiorazioni_Click"
                    OnClientClick="if(Page_ClientValidate('UCTabMaggiorazioni')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary"/>
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaMaggiorazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Maggiorazioni" Width="170px" OnClick="EliminaMaggiorazioni_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare Maggiorazioni?')) return false; else BlockUI();"  CssClass="ghost-delete"/>
            </td>
        </tr>
    </table>
</div>
