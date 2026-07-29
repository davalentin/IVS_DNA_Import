<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCOpzioneCi.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCOpzioneCi" %>

<asp:Panel runat="server" ID="pnlOpzione">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1">
                <label>
                    Data Domanda Opzione:</label>
            </td>
            <td class="field">
                 <asp:TextBox Style="text-align: left" runat="server" ID="txtDataDomandaOpzione"
                    Width="95px" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" TabIndex="1" Text="gg/mm/aaaa"
                    MaxLength="10"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateDataDomandaOpzione" ControlToValidate="txtDataDomandaOpzione"
                    Display="Dynamic" ErrorMessage="Inserire la data in un formato valido per Data Domanda Opzione"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    ValidationGroup="UCTabOpzione"/>
                <asp:CustomValidator runat="server" ControlToValidate="txtDataDomandaOpzione" Display="Dynamic"
                    ErrorMessage="Data Domanda Opzione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabOpzione"
                    ID="customCheckDataDataDomandaOpzione" ClientValidationFunction="checkCorrettezzaData" />  
            </td>
            <td class="Row1">
                <label>
                    Decorrenza Opzione:</label>
            </td>
            <td class="field">
                 <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaOpzione"
                    Width="95px" CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="2" Text="mm/aaaa"
                    MaxLength="7"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateDecorrenzaOpzione" ControlToValidate="txtDecorrenzaOpzione"
                    Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per Decorrenza Opzione"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabOpzione"
                    Text="*" CssClass="field-is-required" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaOpzione" Display="Dynamic"
                    ErrorMessage="Decorrenza Opzione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabOpzione"
                    ID="customCheckDataDecorrenzaOpzione" ClientValidationFunction="checkCorrettezzaData" />  
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Decorrenza DPCM:</label>
            </td>
            <td class="field full-grid" colspan="3">
                 <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaDCPM" MaxLength="7"
                    Width="95px" CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="3" Text="mm/aaaa"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" ControlToValidate="txtDecorrenzaDCPM"
                    Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per Decorrenza DCPM"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabOpzione" Text="*" CssClass="field-is-required" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaDCPM" Display="Dynamic"
                    ErrorMessage="Decorrenza DPCM: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabOpzione"
                    ID="customCheckDataDecorrenzaDPCM" ClientValidationFunction="checkCorrettezzaData" />  
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Codice Opzione Riliquidazione:</label>
            </td>
            <td class="field full-grid" colspan="3">
                 <asp:DropDownList runat="server" ID="ddlCodiceOpzioneRiliquidazione" Width="99%" CssClass="tb8 txtUppercase" TabIndex="4"></asp:DropDownList>
            </td>
        </tr>
    </table>
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%">
            <tr>
                <td style="text-align: right">
                    <asp:Button ID="btnSalvaOpzione" runat="server" SkinID="btnAzione1" Enabled="true" Text="Salva Opzione" Width="170px"
                        CausesValidation="false" OnClick="SalvaOpzione_Click" OnClientClick="if(Page_ClientValidate('UCTabOpzione')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary"/>
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaOpzione" runat="server" SkinID="btnAzione1" Enabled="true" Text="Elimina Opzione" Width="170px"
                        CausesValidation="false" OnClick="EliminaOpzione_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Opzione?')) return false; else BlockUI();" CssClass="ghost-delete"/>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>