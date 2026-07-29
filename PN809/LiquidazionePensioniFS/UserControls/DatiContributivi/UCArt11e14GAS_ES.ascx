<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCArt11e14GAS_ES.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCArt11e14GAS_ES" %>
<style type="text/css">
    .fixed-dialog
    {
        position: fixed;
    }
</style>

<div id="divDatiArt11e14" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server" class="reset-style">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="lblSupplementiArt14" runat="server" Text="Supplementi Art.14" Style="font-weight: bold" CssClass="section-label"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width:25%">
                <label>
                    Contrib. totali:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtContributiTotaliSupplementoDPR143271" MaxLength="10" CssClass="tb8 txtUppercase" Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtContributiTotaliSupplementoDPR143271" runat="server" ControlToValidate="txtContributiTotaliSupplementoDPR143271"
                    Display="Dynamic" Enabled="true" ErrorMessage="Contrib. totali: Inserire valori interi o decimali (max 5 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabArt11_14GAS" ValidationExpression="\d{1,5}(,\d{1,4})?$$" />
            </td>
            <td class="Row1" style="width:25%">
                <label>
                    Contrib. esclusiva:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtContribuzioneEsclusivaDPR143271" MaxLength="10" CssClass="tb8 txtUppercase" Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtContribuzioneEsclusivaDPR143271" runat="server" ControlToValidate="txtContribuzioneEsclusivaDPR143271"
                    Display="Dynamic" Enabled="true" ErrorMessage="Contrib. esclusiva: Inserire valori interi o decimali (max 5 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabArt11_14GAS" ValidationExpression="\d{1,5}(,\d{1,4})?$$" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width:25%">
                <label>
                    CC totali:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtCCTotaliArt14" MaxLength="8" CssClass="tb8 txtUppercase" Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtCCTotaliArt14" runat="server" ControlToValidate="txtCCTotaliArt14"
                    Display="Dynamic" Enabled="true" ErrorMessage="CC totali Art.14: Inserire valori interi o decimali (max 3 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabArt11_14GAS" ValidationExpression="\d{1,3}(,\d{1,4})?$$" />
            </td>
            <td class="Row1" style="width:25%">
                <label>
                    CC esclusiva:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtContribuzioneEsclusiva" MaxLength="8" CssClass="tb8 txtUppercase" Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtContribuzioneEsclusiva" runat="server" ControlToValidate="txtContribuzioneEsclusiva"
                    Display="Dynamic" Enabled="true" ErrorMessage="CC esclusiva Art.14: Inserire valori interi o decimali (max 3 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabArt11_14GAS" ValidationExpression="\d{1,3}(,\d{1,4})?$$" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Dec. DPCM:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDecDPCM" Width="60%"
                    Text="" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="1" MaxLength="10"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtDecDPCM" ControlToValidate="txtDecDPCM"
                    ErrorMessage="Dec. DPCM in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabArt11_14GAS"
                    Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecDPCM" Display="Dynamic"
                    ErrorMessage="Dec. DPCM: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabArt11_14GAS"
                    ID="customCheckDataDecDPCM" ClientValidationFunction="checkCorrettezzaData" />  
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    RMS:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtRMSArt14" MaxLength="10" CssClass="tb8 txtUppercase" Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtRMSArt14" runat="server" ControlToValidate="txtRMSArt14"
                    Display="Dynamic" Enabled="true" ErrorMessage="RMS: Inserire valori interi o decimali (max 5 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabArt11_14GAS" ValidationExpression="\d{1,5}(,\d{1,4})?$$" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Sent. 72 RMS:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtRMSSent72" MaxLength="10" CssClass="tb8 txtUppercase" Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtRMSSent72" runat="server" ControlToValidate="txtRMSSent72"
                    Display="Dynamic" Enabled="true" ErrorMessage="Sent. 72 RMS: Inserire valori interi o decimali (max 5 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabArt11_14GAS" ValidationExpression="\d{1,5}(,\d{1,4})?$$" />
            </td>
        </tr>
    </table>
</div>

<div id="divES_DatiArt11e14" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server" class="reset-style">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="Label1" runat="server" Text="Supplementi Art.11 e 14" Style="font-weight: bold"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Dec. DPCM:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtboxES_DecDPCM" Width="60%"
                    Text="" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="1" MaxLength="10"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator5" ControlToValidate="txtboxES_DecDPCM"
                    ErrorMessage="Dec. DPCM in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabArt11_14GAS"
                    Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtboxES_DecDPCM" Display="Dynamic"
                    ErrorMessage="Dec. DPCM: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabArt11_14GAS"
                    ID="customCheckDataES_DecDPCM" ClientValidationFunction="checkCorrettezzaData" />  
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    RMS DPCM:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtboxES_RmsDCPM" MaxLength="10" CssClass="tb8 txtUppercase" Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ControlToValidate="txtboxES_RmsDCPM"
                    Display="Dynamic" Enabled="true" ErrorMessage="RMS: Inserire valori interi o decimali (max 5 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabArt11_14GAS" ValidationExpression="\d{1,5}(,\d{1,4})?$$" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Sent. 72 RMS:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtboxES_Sent74Rms" MaxLength="10" CssClass="tb8 txtUppercase" Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" ControlToValidate="txtboxES_Sent74Rms"
                    Display="Dynamic" Enabled="true" ErrorMessage="Sent. 72 RMS: Inserire valori interi o decimali (max 5 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabArt11_14GAS" ValidationExpression="\d{1,5}(,\d{1,4})?$$" />
            </td>
        </tr>
    </table>
</div>

<div id="divSuppArt11" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server" visible="false" class="reset-style">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="lblSupplementiArt11" runat="server" Text="Supplementi Art.11" Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width:25%">
                <label>
                    CC totali:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtCCTotaliArt11" MaxLength="8" CssClass="tb8 txtUppercase" Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtCCTotaliArt11" runat="server" ControlToValidate="txtCCTotaliArt11"
                    Display="Dynamic" Enabled="true" ErrorMessage="CC totali Art.11: Inserire valori interi o decimali (max 3 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabArt11_14GAS" ValidationExpression="\d{1,3}(,\d{1,4})?$$" />
            </td>
            <td class="Row1" style="width:25%">
                <label>
                    CC esclusiva:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtCCEsclusivaArt11" MaxLength="8" CssClass="tb8 txtUppercase" Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtCCEsclusivaArt11" runat="server" ControlToValidate="txtCCEsclusivaArt11"
                    Display="Dynamic" Enabled="true" ErrorMessage="CC esclusiva Art.11: Inserire valori interi o decimali (max 5 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabArt11_14GAS" ValidationExpression="\d{1,5}(,\d{1,4})?$$" />
            </td>
        </tr>
    </table>
</div>

<div style="margin-right: 40px;" class="containerWidth xs">
    <table width="100%" style="min-height:100px;" class="tab-actions-group">
        <tr>
            <td style="text-align: right; vertical-align:bottom;" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaArt11_14" runat="server" CausesValidation="false" ValidationGroup="UCTabArt11_14GAS" 
                    SkinID="btnAzione1" Width="150px" OnClick="btnSalvaArt11_14_Click" Text="Salva Art.11 e 14"
                    OnClientClick="if(Page_ClientValidate('UCTabArt11_14GAS')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary"/>
            </td>
            <td style="text-align: left;vertical-align:bottom;">
                <asp:Button ID="btnEliminaArt11_14" runat="server" SkinID="btnAzione1" CausesValidation="false" Enabled="true" Text="Elimina Art.11 e 14" 
                    Width="150px" OnClick="btnEliminaArt11_14_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Art.11 e 14?')) return false; else BlockUI();" CssClass="ghost-delete"/>
            </td>
        </tr>
    </table>
</div>