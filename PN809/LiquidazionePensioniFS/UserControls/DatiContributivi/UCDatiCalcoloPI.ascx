<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiCalcoloPI.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiCalcoloPI" %>
<asp:Panel runat="server" ID="pnlCatV" Visible="false">
    <table class="tabellaFormattazione grid grid-size-25">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Retribuzione Media Settimanale A:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtRMSQuotaA" MaxLength="11" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtRMSQuotaA" runat="server" ControlToValidate="txtRMSQuotaA"
                    Display="Dynamic" Enabled="true" ErrorMessage="RMS Quota A: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^\d{1,6}(,\d{1,4})?$" />
            </td>
            <td class="Row1" style="width: 25%">
                <label class="etichettaBold">
                    Settimane A:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtNSettimaneQuotaA" MaxLength="4" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtNSettimaneQuotaA" runat="server" ControlToValidate="txtNSettimaneQuotaA"
                    Display="Dynamic" Enabled="true" ErrorMessage="Settimane A: Inserire valori interi"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^[0-9]*$" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Retribuzione Media Settimanale B:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtRMSQuotaB" MaxLength="11" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtRMSQuotaB" runat="server" ControlToValidate="txtRMSQuotaB"
                    Display="Dynamic" Enabled="true" ErrorMessage="RMS Quota B: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^\d{1,6}(,\d{1,4})?$" />
            </td>
            <td class="Row1" style="width: 25%">
                <label class="etichettaBold">
                    Settimane B:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtNSettimaneQuotaB" MaxLength="4" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtNSettimaneQuotaB" runat="server" ControlToValidate="txtNSettimaneQuotaB"
                    Display="Dynamic" Enabled="true" ErrorMessage="Settimane B: Inserire valori interi"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^[0-9]*$" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel runat="server" ID="pnlCommon">
    <table class="tabellaFormattazione grid grid-size-25">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Elemento Retributivo:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtStipendioAnnuo" MaxLength="11" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtStipendioAnnuo" runat="server" ControlToValidate="txtStipendioAnnuo"
                    Display="Dynamic" Enabled="true" ErrorMessage="Elemento Retributivo: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^\d{1,6}(,\d{1,4})?$" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtStipendioAnnuo" ControlToValidate="txtStipendioAnnuo"
                    Display="Dynamic" Enabled="true" ErrorMessage="Elemento Retributivo: campo obbligatorio"
                    ValidationGroup="UCTabDatiCalcoloPI" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                   Assegno personale 36/bis:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtStipendioBase" MaxLength="11" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtStipendioBase" runat="server" ControlToValidate="txtStipendioBase"
                    Display="Dynamic" Enabled="true" ErrorMessage="Stipendio Base: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^\d{1,6}(,\d{1,4})?$" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtStipendioBase" ControlToValidate="txtStipendioBase"
                    Display="Dynamic" Enabled="true" ErrorMessage="Stipendio Base: campo obbligatorio"
                    ValidationGroup="UCTabDatiCalcoloPI" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Contingenza:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtImportoIIS" MaxLength="6" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtImportoIIS" runat="server" ControlToValidate="txtImportoIIS"
                    Display="Dynamic" Enabled="true" ErrorMessage="Contingenza: Inserire valori interi"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^\d{1,6}(,\d{2})?$" />
            </td>
            <td class="Row1" runat="server" id="trLblPensioneFacoltativaMensile" visible="false"
                style="width: 25%">
                <label>
                    Pensione al 1/12/98:</label>
            </td>
            <td class="field" runat="server" id="trTxtPensioneFacoltativaMensile" visible="false"
                style="width: 25%">
                <asp:TextBox runat="server" ID="txtPensioneFacoltativaMensile" MaxLength="9" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtPensioneFacoltativaMensile" runat="server"
                    ControlToValidate="txtPensioneFacoltativaMensile" Display="Dynamic" Enabled="true"
                    ErrorMessage="Pensione al 1/12/98: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^\d{1,4}(,\d{1,4})?$" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel runat="server" ID="pnlCatAB" Visible="false">
    <table class="tabellaFormattazione grid grid-size-25" >
        <tr>
            <td class="Row1" style="width: 25%">
              <label>Percentuale di Capitalizzazione:</label>
            </td>
            <td class="field" style="width: 25%" >
                  <asp:TextBox runat="server" ID="txtPercentualeCapitalizzazione" MaxLength="7" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox> %
					<asp:RegularExpressionValidator ID="REVtxtPercentualeCapitalizzazione" runat="server" ControlToValidate="txtPercentualeCapitalizzazione"
                    Display="Dynamic" Enabled="true" ErrorMessage="Percentuale di Capitalizzazione: Inserire valori interi o decimali (max 2 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^\d{1,2}(,\d{1,4})?$" />
            </td>
              <td class="Row1" style="width: 50%">
            </td>
        </tr>
	</table>
</asp:Panel>
<asp:Panel runat="server" ID="pnlCatU" Visible="false">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="width: 25%">
                 <label>
                    Att. Con.:</label>
            </td>
            <td class="field" style="width: 25%">
                  <asp:DropDownList runat="server" ID="ddlAttCon" CssClass="tb8 txtUppercase" Width="60%">
                </asp:DropDownList>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice di Maggiorazione:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList runat="server" ID="ddlCodiceMaggiorazione" CssClass="tb8 txtUppercase" Width="30%">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="0" Value="0"></asp:ListItem>
                    <asp:ListItem Text="1" Value="1"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Pens.Compl.Riv 1/95:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtPensComplRiv1_95" MaxLength="11" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtPensComplRiv1_95" runat="server" ControlToValidate="txtPensComplRiv1_95"
                    Display="Dynamic" Enabled="true" ErrorMessage="Pens.Compl.Riv 1/95: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^\d{1,6}(,\d{1,4})?$" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Controcodice retribuzione:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtControCodiceRetribuzione" MaxLength="3" CssClass="tb8 txtUppercase" Width="70%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REV" runat="server" ControlToValidate="txtControCodiceRetribuzione"
                    Display="Dynamic" Enabled="true" ErrorMessage="Controcodice retribuzione: Inserire valori interi"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPI" ValidationExpression="^[0-9]*$" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtControCodiceRetribuzione" ControlToValidate="txtControCodiceRetribuzione"
                    Display="Dynamic" Enabled="true" ErrorMessage="Controcodice retribuzione: campo obbligatorio"
                    ValidationGroup="UCTabDatiCalcoloPI" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
</asp:Panel>
<div style="margin-right: 40px;" class="containerWidth xs">
    <table width="100%" style="min-height: 100px;" class="tab-actions-group">
        <tr>
            <td style="text-align: right; vertical-align: bottom;" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaDatiCalcolo" runat="server" CausesValidation="false" ValidationGroup="UCTabDatiCalcoloPI"
                    SkinID="btnAzione1" Width="150px" OnClick="btnSalvaDatiCalcolo_Click" Text="Salva Dati Calcolo"
                    OnClientClick="if(Page_ClientValidate('UCTabDatiCalcoloPI')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
            </td>
            <td style="text-align: left; vertical-align: bottom;">
                <asp:Button ID="btnEliminaDatiCalcolo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Dati Calcolo" Width="150px" OnClick="btnEliminaDatiCalcolo_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Calcolo?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
<asp:HiddenField runat="server" ID="hdnAttCon" Value="" />
