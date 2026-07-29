<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiCalcoloDZ_New.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondo.UCDatiCalcoloDZ_New" %>
<%@ Register Src="UCDoppioCalcolo_DZ.ascx" TagName="UCDoppioCalcolo" TagPrefix="UCDC" %>
<script type="text/javascript">
    function Confirm() {
        document.getElementById('<%= btnSalvaDatiCalcolo.ClientID %>').click();

        return false;

    }

    function checkPercentualeRiduzione(source, args) {
        var result = false;
        var ddl = document.getElementById('<%= ddlRiduzioneRetributiva.ClientID %>');
        if (ddl != null) {
            var selectedValue = ddl.options[ddl.selectedIndex].value;
            if (selectedValue.toUpperCase() == 'SI') {
                var txt = document.getElementById('<%= txtRiduzioneRetributiva.ClientID %>');
                if (txt.value == '')
                    result = false;
                else
                    result = true;
            }
            else
                result = true;
        }
        args.IsValid = result;
        return false;
    }

</script>
<!-- Pannello Dati Calcolo Header DZ -->
<asp:Panel ID="pnlHeader_DZ" runat="server" Visible="false">
    <div id="pdivHeader" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server">
        <table style="visibility: hidden">
            <tr>
                <td>
                    <label>
                        Tipo Pensione:
                    </label>
                </td>
                <td>
                    <asp:Label ID="lblTipoPensione" runat="server" />
                    <asp:HiddenField ID="hdnTipoPensione" runat="server" />
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox runat="server" ID="txtDecorrenzaRegistrazione" Width="60%" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA"
                        MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtDecorrenzaRegistrazione" ControlToValidate="txtDecorrenzaRegistrazione"
                        ErrorMessage="Decorrenza Registrazione in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ" Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaRegistrazione"
                        Display="Dynamic" ErrorMessage="Decorrenza Registrazione: data illogica" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiCalcoloDZ" ID="customCheckDatatxtDecorrenzaRegistrazione"
                        ClientValidationFunction="checkCorrettezzaData" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtDecorrenzaRegistrazione" Display="Dynamic"
                        ErrorMessage="Decorrenza Registrazione: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloDZ"
                        ControlToValidate="txtDecorrenzaRegistrazione"></asp:RequiredFieldValidator>

                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Sospensione:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtSospensione" Width="60%"
                        Text="" CssClass="txtUppercase tb8 date-picker dateMMaaaa" MaxLength="7"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtSospensione" ControlToValidate="txtSospensione"
                        ErrorMessage="Sospensione in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ"
                        Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtSospensione" Display="Dynamic"
                        ErrorMessage="Sospensione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloDZ"
                        ID="customCheckDataSospensione" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Pensione Base Annua:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtPensioneBaseAnnua" Width="60%"
                        CssClass="txtUppercase tb8" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtPensioneBaseAnnua" ControlToValidate="txtPensioneBaseAnnua"
                        ErrorMessage="Pension Base Annua in formato non valido" ValidationExpression="^\d{1,4}(,\d{1,5})?$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ"
                        Enabled="true" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<!-- Fine Pannello Dati Calcolo Header DZ -->
<!-- Pannello Dati Calcolo Servizio Utile DZ -->
<asp:Panel ID="pnlServizioUtile_DZ" runat="server" Visible="false">
    <div id="divServizioUtile_DZ" style="border-style: solid; border-color: #000080;
        border-collapse: collapse; border-width: 1px; width: 710px; margin-left: 4px;
        margin-top: 4px;" runat="server">
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblQuotaA" runat="server" Text="Quota A" Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="width: 15%">
                    <label>
                        Servizio Utile:
                    </label>
                </td>
                <td class="field" style="width: 17%">
                    <asp:TextBox ID="txtServizioUtileAA_QuotaA" runat="server" CssClass="tb8 txtUppercase"
                        Width="30px" MaxLength="2"></asp:TextBox>
                    <label>
                        AA</label>
                    <asp:RegularExpressionValidator ID="REVtxtServizioUtileAA_QuotaA" ControlToValidate="txtServizioUtileAA_QuotaA"
                        ErrorMessage="Servizio Utile Quota A: formato Anno non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ" />
                    <asp:TextBox ID="txtServizioUtileMM_QuotaA" runat="server" CssClass="tb8 txtUppercase"
                        Width="30px" MaxLength="2"></asp:TextBox>
                    <label>
                        MM</label>
                    <asp:RegularExpressionValidator ID="REVtxtServizioUtileMM_QuotaA" ControlToValidate="txtServizioUtileMM_QuotaA"
                        ErrorMessage="Servizio Utile Quota A: formato Mese non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ" />
                </td>
                <td class="Row1" style="width: 16%; text-align: right">
                    <label>
                        Retribuzione:</label>
                </td>
                <td class="field" style="width: 16%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtRetribuzionePensionabile_QuotaA"
                        Width="60%" CssClass="txtUppercase tb8" MaxLength="11"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtRetribuzionePensionabile_QuotaA" ControlToValidate="txtRetribuzionePensionabile_QuotaA"
                        ErrorMessage="Retribuzione Quota A in formato non valido" ValidationExpression="^\d{1,6}(,\d{1,4})?$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ"
                        Enabled="true" />
                </td>
                <td class="Row1" style="width: 16%">
                    <label>
                        Controcodice:</label>
                </td>
                <td class="field" style="width: 16%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtControcodice_QuotaA"
                        Width="60%" CssClass="txtUppercase tb8" MaxLength="3"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtControcodice_QuotaA" ControlToValidate="txtControcodice_QuotaA"
                        ErrorMessage="Controcodice Quota A in formato non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ"
                        Enabled="true" />
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblQuotaB" runat="server" Text="Quota B" Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="width: 15%">
                    <label>
                        Servizio Utile:
                    </label>
                </td>
                <td class="field" style="width: 17%">
                    <asp:TextBox ID="txtServizioUtileAA_QuotaB" runat="server" CssClass="tb8 txtUppercase"
                        Width="30px" MaxLength="2"></asp:TextBox>
                    <label>
                        AA</label>
                    <asp:RegularExpressionValidator ID="REVtxtServizioUtileAA_QuotaB" ControlToValidate="txtServizioUtileAA_QuotaB"
                        ErrorMessage="Servizio Utile Quota B: formato Anno non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ" />
                    <asp:TextBox ID="txtServizioUtileMM_QuotaB" runat="server" CssClass="tb8 txtUppercase"
                        Width="30px" MaxLength="2"></asp:TextBox>
                    <label>
                        MM</label>
                    <asp:RegularExpressionValidator ID="REVtxtServizioUtileMM_QuotaB" ControlToValidate="txtServizioUtileMM_QuotaB"
                        ErrorMessage="Servizio Utile Quota B: formato Mese non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ" />
                </td>
                <td class="Row1" style="width: 16%; text-align: right">
                    <label>
                        Retribuzione:</label>
                </td>
                <td class="field" style="width: 16%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtRetribuzionePensionabile_QuotaB"
                        Width="60%" CssClass="txtUppercase tb8" MaxLength="11"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtRetribuzionePensionabile_QuotaB" ControlToValidate="txtRetribuzionePensionabile_QuotaB"
                        ErrorMessage="Retribuzione Quota B in formato non valido" ValidationExpression="^\d{1,6}(,\d{1,4})?$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ"
                        Enabled="true" />
                </td>
                <td class="Row1" style="width: 16%">
                    <label>
                        Controcodice:</label>
                </td>
                <td class="field" style="width: 16%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtControcodice_QuotaB"
                        Width="60%" CssClass="txtUppercase tb8" MaxLength="3"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtControcodice_QuotaB" ControlToValidate="txtControcodice_QuotaB"
                        ErrorMessage="Controcodice Quota B in formato non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ"
                        Enabled="true" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<!-- Fine Pannello Dati Calcolo Servizio Utile DZ -->
<!-- Pannello Dati Calcolo Retributivi DZ -->
<asp:Panel ID="pnlRetributivi_DZ" runat="server" Visible="false">
    <div id="divRetributivi_DZ" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server">
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Retribuzione Media Settimanale A:
                    </label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtRMSQuotaA" Width="60%"
                        CssClass="txtUppercase tb8" MaxLength="11"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtRMSQuotaA" ControlToValidate="txtRMSQuotaA"
                        ErrorMessage="Retribuzione Media Settimanale A in formato non valido" ValidationExpression="^\d{1,6}(,\d{1,4})?$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ"
                        Enabled="true" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label class="etichettaBold">
                        Settimane A:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtNSettimaneQuotaA" runat="server" CssClass="tb8 txtUppercase"
                        Width="70%" MaxLength="4"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtNSettimaneQuotaA" ControlToValidate="txtNSettimaneQuotaA"
                        ErrorMessage="Settimane Quota A in formato non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ"
                        Enabled="true" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Retribuzione Media Settimanale B:
                    </label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtRMSQuotaB" Width="60%"
                        CssClass="txtUppercase tb8" MaxLength="11"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtRMSQuotaB" ControlToValidate="txtRMSQuotaB"
                        ErrorMessage="Retribuzione Media Settimanale B in formato non valido" ValidationExpression="^\d{1,6}(,\d{1,4})?$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ"
                        Enabled="true" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Settimane B:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtNSettimaneQuotaB" runat="server" CssClass="tb8 txtUppercase"
                        Width="70%" MaxLength="4"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtNSettimaneQuotaB" ControlToValidate="txtNSettimaneQuotaB"
                        ErrorMessage="Settimane Quota B in formato non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ"
                        Enabled="true" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<!-- Fine Pannello Dati Calcolo Retributivi DZ -->
<!-- Pannello Dati Calcolo Contributivi Legge 214 DZ -->
<asp:Panel ID="pnlContributiviL214_DZ" runat="server" Visible="false">
    <div id="divContributiviL214_DZ" style="border-style: solid; border-color: #000080;
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
                        ErrorMessage="Importo Contributivo Totale in formato non valido" ValidationExpression="^\d{1,6}(,\d{1,4})?$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ"
                        Enabled="true" />
                    <asp:RequiredFieldValidator runat="server" ID="txtImportoContribTotaleQuotaDL214RF"
                        ControlToValidate="txtImportoContribTotaleQuotaDL214" Display="Dynamic" Enabled="true"
                        ErrorMessage="Importo Contributivo Totale L. 214: campo obbligatorio" ValidationGroup="UCTabDatiCalcoloDZ"
                        Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        N Settimane:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtNSettimaneQuotaDL214" runat="server" CssClass="tb8 txtUppercase"
                        Width="30px" MaxLength="4"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtNSettimaneQuotaDL214" ControlToValidate="txtNSettimaneQuotaDL214"
                        ErrorMessage="N Settimane in formato non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ"
                        Enabled="true" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtNSettimaneQuotaDL214" ControlToValidate="txtNSettimaneQuotaDL214"
                        Display="Dynamic" Enabled="true" ErrorMessage="Numero Settimane L. 214: campo obbligatorio"
                        ValidationGroup="UCTabDatiCalcoloDZ" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
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
                        ErrorMessage="Montante in formato non valido" ValidationExpression="^\d{1,7}(,\d{1,4})?$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloDZ"
                        Enabled="true" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtMontanteQuotaDL214" ControlToValidate="txtMontanteQuotaDL214"
                        Display="Dynamic" Enabled="true" ErrorMessage="Montante L. 214: campo obbligatorio"
                        ValidationGroup="UCTabDatiCalcoloDZ" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<!-- Fine Pannello Dati Calcolo Contributivi Legge 214 DZ -->
<!-- Pannello doppio calcolo -->
<UCDC:UCDoppioCalcolo runat="server" ID="ucDoppioCalcolo" Visible="false" />
<!-- Fine Pannello doppio calcolo -->
<!-- Pannello Riduzione Retributiva-->
<asp:Panel ID="pnlRiduzioneRetributiva" runat="server" Visible="false">
    <table class="tabellaFormattazione grid" width="100%">
        <tr style="min-height: 50px; vertical-align: bottom">
            <td class="Row1" style="width: 33%">
                <label>
                    Riduzione Retributiva:</label>
            </td>
            <td class="Row1" style="width: 30%">
                <asp:DropDownList ID="ddlRiduzioneRetributiva" CssClass="tb8 txtUppercase xxs" Width="25%"
                    runat="server">
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="txtRiduzioneRetributiva" runat="server" CssClass="tb8 txtUppercase"
                    Width="61%" TabIndex="24" MaxLength="5"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator13"
                    Display="Dynamic" ControlToValidate="txtRiduzioneRetributiva" Enabled="true"
                    ErrorMessage="Riduzione Retributiva: Inserire valori interi o decimali" Text="*" CssClass="field-is-required"
                    ValidationGroup="UCTabDatiCalcoloDZ" ValidationExpression="\d{1,2}(\,\d{1,2})?" />
                <asp:CustomValidator runat="server" ControlToValidate="ddlRiduzioneRetributiva" Display="Dynamic"
                    ErrorMessage="Riduzione Retributiva: La percentuale è obbligatoria avendo selezionato 'SI'"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloDZ" ID="customRiduzione" ClientValidationFunction="checkPercentualeRiduzione" />
            </td>
            <td class="Row1" style="width: 3%">
                <label>
                    %</label>
            </td>
            <td>
            </td>
        </tr>
        <tr>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Riduzione Retributiva-->
<div style="margin-right: 40px;" class="containerWidth xs">
    <table width="100%" style="min-height: 100px;" class="tab-actions-group">
        <tr>
            <td style="text-align: right; vertical-align: bottom;" class="tab-actions-group__first">
                <asp:Button ID="btnPopUp" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Visible="false" Text="Salva Dati Calcolo" Width="150px" OnClientClick="if(Page_ClientValidate('UCTabDatiCalcoloDZ')){return Confirm();}" CssClass="force-right primary" />
                <asp:Button ID="btnSalvaDatiCalcolo" runat="server" CausesValidation="false" Style="display: none"
                    ValidationGroup="UCTabDatiCalcoloDZ" SkinID="btnAzione1" Width="150px" OnClick="btnSalvaDatiCalcolo_Click"
                    Text="Salva Dati Calcolo" Visible="false" OnClientClick="if(Page_ClientValidate('UCTabDatiCalcoloDZ')){aspnetForm.target ='_self'; BlockUI();}" CssClass="force-right primary" />
                <asp:Button ID="btnSalvaDatiCalcoloNoRiduzione" runat="server" CausesValidation="false"
                    ValidationGroup="UCTabDatiCalcoloDZ" SkinID="btnAzione1" Width="150px" OnClick="btnSalvaDatiCalcolo_Click"
                    Text="Salva Dati Calcolo" Visible="true" OnClientClick="if(Page_ClientValidate('UCTabDatiCalcoloDZ')){aspnetForm.target ='_self'; BlockUI();}" CssClass="force-right primary" />
            </td>
            <td style="text-align: left; vertical-align: bottom;">
                <asp:Button ID="btnEliminaDatiCalcolo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="false" Text="Elimina Dati Calcolo" Width="150px" OnClick="btnEliminaDatiCalcolo_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Calcolo?')) return false; else BlockUI();" CssClass="ghost-delete"/>
                <asp:Button ID="btnTornaElencoRegistrazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elenco Registrazioni" Width="180px" OnClick="TornaElencoRegistrazioni_Click"
                    OnClientClick="BlockUI();"  CssClass="tertiary"/>
            </td>
        </tr>
    </table>
</div>
