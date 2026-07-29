<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiCalcoloPM.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiCalcoloPM" %>
<%@ Register Src="UCDoppioCalcolo_ES_DZ_GAS_PM.ascx" TagName="UCDoppioCalcolo" TagPrefix="UCDC" %>
<!-- Dati Retributivi -->
<div id="divDatiRetributivi" style="border-style: solid; border-color: #000080; border-collapse: collapse; 
border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server"
visible="true">
<table class="tabellaFormattazione">
    <tr>
        <td class="Row1" style="text-align: left">
            <asp:Label ID="lblDatiRetributivi" runat="server" Text="Dati Retributivi" Style="font-weight: bold"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="Row1" style="text-align: left">
            <asp:Label ID="Label1" runat="server" Text="Quota A" Style="font-weight: bold"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                RMS:</label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox ID="txtRMSQuotaA" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
            <asp:RegularExpressionValidator ID="REVtxtRMSQuotaA" runat="server" ControlToValidate="txtRMSQuotaA"
                Display="Dynamic" Enabled="true" ErrorMessage="RMS Quota A: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPM" ValidationExpression="\d{1,6}(,\d{1,4})?$" />
        </td>
    </tr>
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                Settimane totali:</label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox ID="txtSettimaneQuotaA" runat="server" MaxLength="5" CssClass="tb8 txtUppercase"
                Width="60%"></asp:TextBox>
            <asp:RegularExpressionValidator ID="REVtxtSettimaneQuotaA" runat="server" ControlToValidate="txtSettimaneQuotaA"
                Display="Dynamic" Enabled="true" ErrorMessage="Settimane totali Quota A: Inserire valori interi"
                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPM" ValidationExpression="^[0-9]*$" />
        </td>
        <td class="Row1" style="width: 25%">
            <label id="lblSettEsclusiveQuotaA" runat="server">
                Settimane esclusive:</label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox ID="txtSettimaneEsclusiveQuotaA" runat="server" MaxLength="5" CssClass="tb8 txtUppercase"
                Width="60%"></asp:TextBox>
            <asp:RegularExpressionValidator ID="REVtxtSettimaneEsclusiveQuotaA" runat="server"
                ControlToValidate="txtSettimaneEsclusiveQuotaA" Display="Dynamic" Enabled="true"
                ErrorMessage="Settimane esclusive Quota A: Inserire valori interi" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPM"
                ValidationExpression="^[0-9]*$" />
        </td>
    </tr>
    <tr>
        <td class="Row1" style="text-align: left">
            <asp:Label ID="Label2" runat="server" Text="Quota B" Style="font-weight: bold"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                RMS:</label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox ID="txtRMSQuotaB" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
            <asp:RegularExpressionValidator ID="REVtxtRMSQuotaB" runat="server" ControlToValidate="txtRMSQuotaB"
                Display="Dynamic" Enabled="true" ErrorMessage="RMS Quota B: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPM" ValidationExpression="\d{1,6}(,\d{1,4})?$" />
        </td>
    </tr>
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                Settimane totali:</label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox ID="txtSettimaneQuotaB" runat="server" MaxLength="4" CssClass="tb8 txtUppercase"
                Width="60%"></asp:TextBox>
            <asp:RegularExpressionValidator ID="REVtxtSettimaneQuotaB" runat="server" ControlToValidate="txtSettimaneQuotaB"
                Display="Dynamic" Enabled="true" ErrorMessage="Settimane totali Quota B: Inserire valori interi"
                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPM" ValidationExpression="^[0-9]*$" />
        </td>
        <td class="Row1" style="width: 25%">
            <label id="lblSettEsclusiveQuotaB" runat="server">
                Settimane esclusive:</label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox ID="txtSettimaneEsclusiveQuotaB" runat="server" MaxLength="4" CssClass="tb8 txtUppercase"
                Width="60%"></asp:TextBox>
            <asp:RegularExpressionValidator ID="REVtxtSettimaneEsclusiveQuotaB" runat="server"
                ControlToValidate="txtSettimaneEsclusiveQuotaB" Display="Dynamic" Enabled="true"
                ErrorMessage="Settimane esclusive Quota B: Inserire valori interi" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloPM"
                ValidationExpression="^[0-9]*$" />
        </td>
    </tr>
</table>

</div>

<div id="divDatiContributiviL335" style="border-style: solid; border-color: #000080; border-collapse: collapse; 
border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server"
visible="true">
<table class="tabellaFormattazione">
    <tr>
        <td class="Row1" style="text-align: left">
            <asp:Label ID="Label3" runat="server" Text="Dati contributivi L.335"
                Style="font-weight: bold"></asp:Label>
        </td>
    </tr>
</table>
<table class="tabellaFormattazione">
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                Montante Totale:
            </label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox Style="text-align: left" runat="server" ID="txtMontateTotaleL335" Width="60%"
                CssClass="txtUppercase tb8" MaxLength="12"></asp:TextBox>
            <asp:RegularExpressionValidator ID="REVMontanteTotaleL335" ControlToValidate="txtMontateTotaleL335"
                ErrorMessage="Montante L.315: Inserire valori interi o decimali (max 7 interi e 4 decimali)" ValidationExpression="^\d{1,7}(,\d{1,4})?$"
                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloPM"
                Enabled="true" />
            <asp:RequiredFieldValidator runat="server" ID="RFVMontanteTotaleL555" ControlToValidate="txtMontateTotaleL335"
                Display="Dynamic" Enabled="true" ErrorMessage="Montante L. 335: campo obbligatorio" ValidationGroup="UCTabDatiCalcoloPM"
                Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
        </td>
        <td class="Row1" style="width: 25%">
            <label>
                N Settimane:</label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox ID="txtSettimaneL335" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                MaxLength="4"></asp:TextBox>
            <asp:RegularExpressionValidator ID="REVtxtSettimaneL335" ControlToValidate="txtSettimaneL335"
                ErrorMessage="N Settimane in formato non valido" ValidationExpression="^[0-9]+$"
                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloPM"
                Enabled="true" />
            <asp:RequiredFieldValidator runat="server" ID="RFVtxtSettimaneL335" ControlToValidate="txtSettimaneL335"
                Display="Dynamic" Enabled="true" ErrorMessage="Numero Settimane L. 335: campo obbligatorio" ValidationGroup="UCTabDatiCalcoloPM"
                Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
        </td>
    </tr>
</table>
</div>
<!-- Pannello Dati Calcolo Contributivi Legge 214 DZ -->
<div id="divContributiviL214" style="border-style: solid; border-color: #000080;
border-collapse: collapse; border-width: 1px; width: 710px; margin-left: 4px;
margin-top: 4px;" runat="server">
<table class="tabellaFormattazione">
    <tr>
        <td class="Row1" style="text-align: left">
            <asp:Label ID="lblContributiviL214" runat="server" Text="Dati contributivi da Legge 214"
                Style="font-weight: bold"></asp:Label>
        </td>
    </tr>
</table>
<table class="tabellaFormattazione">
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                Importo Contributivo Totale:
            </label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox Style="text-align: left" runat="server" ID="txtImportoContribTotaleQuotaDL214"
                Width="60%" CssClass="txtUppercase tb8" MaxLength="11"></asp:TextBox>
            <asp:RegularExpressionValidator ID="REVtxtImportoContribTotaleQuotaDL214" ControlToValidate="txtImportoContribTotaleQuotaDL214"
                ErrorMessage="Importo Contributivo Totale L.214: Inserire valori interi o decimali (max 6 interi e 4 decimali)" ValidationExpression="^\d{1,6}(,\d{1,4})?$"
                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloPM"
                Enabled="true" />
            <asp:RequiredFieldValidator runat="server" ID="txtImportoContribTotaleQuotaDL214RF" ControlToValidate="txtImportoContribTotaleQuotaDL214"
                Display="Dynamic" Enabled="true" ErrorMessage="Importo Contributivo Totale L. 214: campo obbligatorio" ValidationGroup="UCTabDatiCalcoloPM"
                Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
        </td>
        <td class="Row1" style="width: 25%">
            <label>
                N Settimane:</label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox ID="txtNSettimaneQuotaDL214" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                MaxLength="4"></asp:TextBox>
            <asp:RegularExpressionValidator ID="REVtxtNSettimaneQuotaDL214" ControlToValidate="txtNSettimaneQuotaDL214"
                ErrorMessage="N Settimane L.214: in formato non valido" ValidationExpression="^[0-9]+$"
                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloPM"
                Enabled="true" />
            <asp:RequiredFieldValidator runat="server" ID="RFVtxtNSettimaneQuotaDL214" ControlToValidate="txtNSettimaneQuotaDL214"
                Display="Dynamic" Enabled="true" ErrorMessage="Numero Settimane L. 214: campo obbligatorio" ValidationGroup="UCTabDatiCalcoloPM"
                Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                Montante:
            </label>
        </td>
        <td class="field" style="width: 25%">
            <asp:TextBox Style="text-align: left" runat="server" ID="txtMontanteQuotaDL214" Width="60%"
                CssClass="txtUppercase tb8" MaxLength="12"></asp:TextBox>
            <asp:RegularExpressionValidator ID="REVtxtMontanteQuotaDL214" ControlToValidate="txtMontanteQuotaDL214"
                ErrorMessage="Montante L. 214: Inserire valori interi o decimali (max 7 interi e 4 decimali)" ValidationExpression="^\d{1,7}(,\d{1,4})?$"
                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloPM"
                Enabled="true" />
            <asp:RequiredFieldValidator runat="server" ID="RFVtxtMontanteQuotaDL214" ControlToValidate="txtMontanteQuotaDL214"
                Display="Dynamic" Enabled="true" ErrorMessage="Montante L. 214: campo obbligatorio" ValidationGroup="UCTabDatiCalcoloPM"
                Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
        </td>
    </tr>
</table>
</div>
<!-- Pannello doppio calcolo -->
<UCDC:UCDoppioCalcolo runat="server" ID="ucDoppioCalcolo" Visible="false" />
<!-- Fine Pannello doppio calcolo -->


<div style="margin-right: 40px;" class="containerWidth xs">
<table width="100%" style="min-height: 100px;" class="tab-actions-group">
<tr>
    <td style="text-align: right; vertical-align: bottom;" class="tab-actions-group__first">
        <asp:Button ID="btnSalvaDatiCalcolo" runat="server" CausesValidation="false" 
            ValidationGroup="UCTabDatiCalcoloPM" SkinID="btnAzione1" Width="150px" OnClick="btnSalvaDatiCalcolo_Click"
            Text="Salva Dati Calcolo"  OnClientClick="if(Page_ClientValidate('UCTabDatiCalcoloPM')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
    </td>
    <td style="text-align: left; vertical-align: bottom;">
        <asp:Button ID="btnEliminaDatiCalcolo" runat="server" SkinID="btnAzione1" CausesValidation="false"
            Enabled="true" Text="Elimina Dati Calcolo" Width="150px" OnClick="btnEliminaDatiCalcolo_Click"
            OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Calcolo?')) return false; else BlockUI();" CssClass="ghost-delete" />
    </td>
</tr>
</table>
</div>

