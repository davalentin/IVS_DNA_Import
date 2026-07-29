<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCArticolo2.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBenefici.UCArticolo2" %>

<asp:Panel ID="pnlPrivilegiateCommon" runat="server">
    <table class="tabellaFormattazione" cellpadding="3" cellspacing="1" border="0" width="100%">
         <tr>
            <td align="left" colspan="2" style="padding-top:10px;">
                <asp:Label runat="server" ID="lblRecordFondo" Font-Bold="true">Art.2 Comma 12 L.335/95</asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="height:5px" />                
        </tr>
        <tr>
            <td class="Row1" style="width:22%">
                <label>Data inizio beneficio:</label>
            </td>
            <td class="Row1" style="width:78%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataInizioBeneficio" Width="15%" 
                    CssClass="txtUppercase tb8 date-picker-base-maxActual dateGGmmAAAA" TabIndex="7" Text=""
                    MaxLength="10"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldtxtDataInizioBeneficio" ControlToValidate="txtDataInizioBeneficio"
                 Display="Dynamic" Enabled="true" ErrorMessage="Data inizio beneficio: si prega di inserire la data" ValidationGroup="UCTabArticolo2"
                  Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator runat="server" ID="validateDataCompletezza" ControlToValidate="txtDataInizioBeneficio"
                    Display="Dynamic" ErrorMessage="Inserire una data nel formato valido per Data inizio beneficio"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    ValidationGroup="UCTabArticolo2" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataInizioBeneficio" Display="Dynamic"
                    ErrorMessage="Data Inizio Beneficio: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabArticolo2"
                    ID="customCheckDataDataInizioBeneficio" ClientValidationFunction="checkCorrettezzaData" />  
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width:22%">
                <label>Data fine beneficio:</label>
            </td>
            <td class="Row1" style="width:78%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataFineBeneficio" Width="15%" 
                    CssClass="txtUppercase tb8 date-picker-base-maxActual dateGGmmAAAA" TabIndex="7" Text=""
                    MaxLength="10"></asp:TextBox>
                <%--<asp:RequiredFieldValidator runat="server" ID="RequiredFieldtxtDataFineBeneficio" ControlToValidate="txtDataFineBeneficio"
                 Display=Dynamic Enabled="true" ErrorMessage="Data fine beneficio: si prega di inserire la data" ValidationGroup="UCTabArticolo2"
                  Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>--%>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="txtDataFineBeneficio"
                    Display="Dynamic" ErrorMessage="Inserire una data nel formato valido per Data fine beneficio"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    ValidationGroup="UCTabArticolo2" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataFineBeneficio" Display="Dynamic"
                    ErrorMessage="Data Fine Beneficio: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabArticolo2"
                    ID="customCheckDataDataFineBeneficio" ClientValidationFunction="checkCorrettezzaData" />  
            </td>
        </tr>
    </table>
</asp:Panel>
<div style="margin-top: 100px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%">
        <tr>
            <td style="text-align: right">
                <asp:Button ID="btnSalvaArt2" runat="server" SkinID="btnAzione1" CausesValidation="false" Enabled="true" Text="Salva Articolo 2" Width="180px" 
                    OnClientClick="if(Page_ClientValidate('UCTabArticolo2')){aspnetForm.target ='_self'; BlockUI();}" TabIndex="9" onclick="btnSalvaArt2_Click" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaArt2" runat="server" SkinID="btnAzione1" CausesValidation="false" Enabled="true" Text="Elimina Articolo 2" TabIndex="10"
                    Width="180px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare Benefici?')) return false; else BlockUI();" onclick="btnEliminaArt2_Click"/>
            </td>
        </tr>
    </table>
</div>