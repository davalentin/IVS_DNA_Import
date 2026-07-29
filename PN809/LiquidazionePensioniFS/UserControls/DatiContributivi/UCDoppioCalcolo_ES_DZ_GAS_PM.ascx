<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDoppioCalcolo_ES_DZ_GAS_PM.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDoppioCalcolo_ES_DZ_GAS_PM" %>
<div id="divComma707" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server">
    <table class="tabellaFormattazione" width="100%">
        <tr>
            <td class="section-label mt-32" style="text-align: left; font-weight: bold;">
                Calcolo ex comma 707
            </td>
        </tr>
    </table>
    <!-- Pannello ES DZ PM -->
    <asp:Panel runat="server" ID="pnlFondi_ES_DZ_PM" Visible="false">
        <table class="tabellaFormattazione" width="100%">
            <tr>
                <td class="Row1" style="width: 20%">
                    Quota A:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaAComma707" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaAComma707" ControlToValidate="txtQuotaAComma707"
                        Display="Dynamic" ErrorMessage="Quota A del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" />
                </td>
                <td class="Row1" style="width: 20%">
                    Quota B:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaBComma707" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaBComma707" ControlToValidate="txtQuotaBComma707"
                        Display="Dynamic" ErrorMessage="Quota B del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" />
                </td>
            </tr>
            <tr runat="server" id="trRetribuzionePonderata707">
                <td class="Row1" colspan="2">
                    Retribuzione ponderata AGO per calcolo limite:
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtRetribuzionePonderataComma707" CssClass="tb8 txtUppercase"
                        MaxLength="12" Width="80%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtRetribuzionePonderataComma707"
                        Display="Dynamic" ControlToValidate="txtRetribuzionePonderataComma707" Enabled="true"
                        ErrorMessage="Retribuzione ponderata AGO per calcolo limite: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtRetribuzionePonderataComma707"
                        ControlToValidate="txtRetribuzionePonderataComma707" Display="Dynamic" Enabled="true"
                        ErrorMessage="Retribuzione ponderata AGO per calcolo limite: campo obbligatorio"
                        Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
                <td class="Row1">
                    <label>
                        €</label>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <!-- Fine Pannello ES DZ PM -->
        
    <!-- Pannello GAS -->
    <asp:Panel runat="server" ID="pnlFondi_GAS" Visible="false">
        <table class="tabellaFormattazione" width="100%">
             <tr>
                <td class="Row1" style="font-weight: bold;" colspan="4">
                    Quota A
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    Settimane 707 totali:
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox runat="server" ID="txtQuotaAComma707_GAS" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaAComma707_GAS" ControlToValidate="txtQuotaAComma707_GAS"
                        Display="Dynamic" ErrorMessage="Quota A del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" />
                </td>
                <td class="Row1" style="width: 25%">
                    Settimane 707 esclusive:
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox runat="server" ID="txtQuotaAComma707Esclusive_GAS" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaAComma707Esclusive_GAS" ControlToValidate="txtQuotaAComma707Esclusive_GAS"
                        Display="Dynamic" ErrorMessage="Quota A del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="font-weight: bold;" colspan="4">
                    Quota B
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    Settimane 707 totali:
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox runat="server" ID="txtQuotaBComma707_GAS" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaBComma707_GAS" ControlToValidate="txtQuotaBComma707_GAS"
                        Display="Dynamic" ErrorMessage="Quota B del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" />
                </td>
                <td class="Row1" style="width: 25%">
                    Settimane 707 esclusive:
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox runat="server" ID="txtQuotaBComma707Esclusive_GAS" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaBComma707Esclusive_GAS" ControlToValidate="txtQuotaBComma707Esclusive_GAS"
                        Display="Dynamic" ErrorMessage="Quota B del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <!-- Fine Pannello GAS -->
</div>
