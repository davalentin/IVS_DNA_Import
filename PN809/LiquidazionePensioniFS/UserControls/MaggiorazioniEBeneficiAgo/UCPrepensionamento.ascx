<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCPrepensionamento.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiAgo.UCPrepensionamento" %>

<table class="tabellaFormattazione">
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                Codice Legge:</label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox runat="server" ID="txtCodiceLegge" Width="100px" CssClass="tb8 txtUppercase" MaxLength="4" Enabled="false" ></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ID="RFVtxtCodiceLegge" ControlToValidate="txtCodiceLegge" ErrorMessage="Codice Legge obbligatorio" 
                Text="*" Display="Dynamic" ValidationGroup="UCTabPrepensionamento" />
            <asp:RegularExpressionValidator runat="server" ID="REVtxtCodiceLegge" ControlToValidate="txtCodiceLegge" ErrorMessage="Codice Legge in formato non valido" 
                ValidationExpression="^[0-9]{4}$" Text="*" Display="Dynamic" ValidationGroup="UCTabPrepensionamento" />
        </td>
    </tr>
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                Settimane utili diritto:</label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox runat="server" ID="txtSettimaneUtiliDiritto" Width="100px" CssClass="tb8 txtUppercase" MaxLength="4" ></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ID="RFVtxtSettimaneUtiliDiritto" ControlToValidate="txtSettimaneUtiliDiritto" ErrorMessage="Settimane utili diritto obbligatorie" 
                Text="*" Display="Dynamic" ValidationGroup="UCTabPrepensionamento" />
            <asp:RegularExpressionValidator runat="server" ID="REVtxtSettimaneUtiliDiritto" ControlToValidate="txtSettimaneUtiliDiritto" ErrorMessage="Settimane utili diritto in formato non valido" 
                ValidationExpression="^[0-9]+$" Text="*" Display="Dynamic" ValidationGroup="UCTabPrepensionamento" />
        </td>
        <td class="Row1" style="width: 25%">
            <label>
                Settimane utili misura:</label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox runat="server" ID="txtSettimaneUtiliMisura" Width="100px" CssClass="tb8 txtUppercase" MaxLength="4" ></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ID="RFVtxtSettimaneUtiliMisura" ControlToValidate="txtSettimaneUtiliMisura" ErrorMessage="Settimane utili misura obbligatorie" 
                Text="*" Display="Dynamic" ValidationGroup="UCTabPrepensionamento" />
            <asp:RegularExpressionValidator runat="server" ID="REVtxtSettimaneUtiliMisura" ControlToValidate="txtSettimaneUtiliMisura" ErrorMessage="Settimane utili misura in formato non valido" 
                ValidationExpression="^[0-9]+$" Text="*" Display="Dynamic" ValidationGroup="UCTabPrepensionamento" />
        </td>
    </tr>
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                Settimane maggiore anzianità:</label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox runat="server" ID="txtSettimaneMaggioreAnzianita" Width="100px" CssClass="tb8 txtUppercase" MaxLength="4" ></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="REVtxtSettimaneMaggioreAnzianita" ControlToValidate="txtSettimaneMaggioreAnzianita" ErrorMessage="Settimane maggiore anzianità in formato non valido" 
                ValidationExpression="^[0-9]+$" Text="*" Display="Dynamic" ValidationGroup="UCTabPrepensionamento" />
        </td>
    </tr>
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                Onere mancata contribuzione:</label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox runat="server" ID="txtOnereMancataContribuzione" Width="100px" CssClass="tb8 txtUppercase" MaxLength="13" ></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="REVtxtOnereMancataContribuzione" ControlToValidate="txtOnereMancataContribuzione" ErrorMessage="Onere mancata contribuzione in formato non valido" 
                ValidationExpression="\d{1,8}(,\d{1,4})?$" Text="*" Display="Dynamic" ValidationGroup="UCTabPrepensionamento" />
        </td>
    </tr>
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                Codice Azienda:</label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox runat="server" ID="txtCodiceAzienda" Width="100px" CssClass="tb8 txtUppercase" MaxLength="10" ></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="REVtxtCodiceAzienda" ControlToValidate="txtSettimaneMaggioreAnzianita" ErrorMessage="Codice Azienda in formato non valido" 
                ValidationExpression="^[0-9]+$" Text="*" Display="Dynamic" ValidationGroup="UCTabPrepensionamento" />
        </td>
    </tr>
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                Cessazione beneficio prepensionamento:</label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox runat="server" ID="txtCessazioneBeneficioPrepensionamento" Width="80px" CssClass="tb8 txtUppercase date-picker" MaxLength="7" ></asp:TextBox>
            <asp:RegularExpressionValidator ID="REVtxtCessazioneBeneficioPrepensionamento" ControlToValidate="txtCessazioneBeneficioPrepensionamento"
                    ErrorMessage="Cessazione beneficio prepensionamento in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}$"
                    runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabPrepensionamento"
                    Enabled="true" />
            <asp:CustomValidator runat="server" ControlToValidate="txtCessazioneBeneficioPrepensionamento" Display="Dynamic"
                    ErrorMessage="Cessazione beneficio prepensionamento: data illogica" Text="*" ValidationGroup="UCTabPrepensionamento"
                    ID="customCheckDataCessazioneBeneficioPrepensionamento" ClientValidationFunction="checkCorrettezzaData" />  
        </td>
    </tr>
    <tr>
        <asp:Panel runat="server" ID="pnlAmianto" Visible="false">
            <td class="Row1" style="width: 25%">
                <label>
                    Settimane amianto:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtSettimaneAmianto" Width="100px" CssClass="tb8 txtUppercase" MaxLength="4" ></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtSettimaneAmianto" ControlToValidate="txtSettimaneAmianto" ErrorMessage="Settimane amianto in formato non valido" 
                    ValidationExpression="^[0-9]+$" Text="*" Display="Dynamic" ValidationGroup="UCTabPrepensionamento" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtSettimaneAmianto" ControlToValidate="txtSettimaneAmianto" ErrorMessage="Settimane amianto obbligatorie" 
                    Text="*" Display="Dynamic" ValidationGroup="UCTabPrepensionamento" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Cessazione amianto:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtCessazioneAmianto" Width="80px" CssClass="tb8 txtUppercase date-picker" MaxLength="7" ></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtCessazioneAmianto" ControlToValidate="txtCessazioneAmianto"
                        ErrorMessage="Cessazione amianto in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}$"
                        runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabPrepensionamento"
                        Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtCessazioneAmianto" Display="Dynamic"
                    ErrorMessage="Cessazione Amianto: data illogica" Text="*" ValidationGroup="UCTabPrepensionamento"
                    ID="customCheckDataCessazioneAmianto" ClientValidationFunction="checkCorrettezzaData" />  
            </td>
        </asp:Panel>
    </tr>
</table>

<div style="width: 720px; margin-top: 100px; margin-right: 40px;">
    <table width="100%">
        <tr>
            <td style="text-align: right">
                <asp:Button ID="btnPrepensionamento" runat="server" SkinID="btnAzione1" CausesValidation="false" Enabled="true" Text="Salva Prepensionamento" 
                    Width="160px" OnClick="SalvaPrepensionamento_Click" OnClientClick="if(Page_ClientValidate('UCTabPrepensionamento')){aspnetForm.target ='_self'; BlockUI();}"/>
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaPrepensionamento" runat="server" SkinID="btnAzione1" CausesValidation="false" Enabled="true" Text="Elimina Prepensionamento" 
                    Width="160px" OnClick="EliminaPrepensionamento_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare Prepensionamento?')) return false; else BlockUI();"/>
            </td>
        </tr>
    </table>
</div>