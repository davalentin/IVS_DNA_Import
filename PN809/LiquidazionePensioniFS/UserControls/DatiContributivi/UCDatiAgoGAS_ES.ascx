<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiAgoGAS_ES.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAgoGAS_ES" %>
<%@ Register Src="UCDoppioCalcolo_ES_DZ_GAS_PM.ascx" TagName="UCDoppioCalcolo" TagPrefix="UCDC" %>
<style type="text/css">
    .fixed-dialog
    {
        position: fixed;
    }
</style>
<script type="text/javascript">
    function Confirm() {
        var ddl = document.getElementById('<%= ddlRiduzioneRetributiva.ClientID %>');
        var selectedValue = ddl.options[ddl.selectedIndex].value;
        if (selectedValue.toUpperCase() == 'SI')
            document.getElementById('<%= btnSalvaDatiAgo.ClientID %>').click();
        else
            $('#dialog-confirm').dialog('open');

        return false;

    }

    function ConfirmContributivi() {
        if (CheckAmmontareMaggioreDiMontante()) {
            $('#dialog-Contributivi').dialog('open');
        }
        else {
            document.getElementById('<%= btnSalvaDatiAgoNoRiduzione.ClientID %>').click();
        }

        return false;
    }

    function CheckAmmontareMaggioreDiMontante() {
        var montante = document.getElementById('<%= txtMontante.ClientID %>');
        var ammontare = document.getElementById('<%= txtboxContributiTotali.ClientID %>');

        if (montante && ammontare && parseFloat(ammontare.value) > parseFloat(montante.value))
            return true;

        return false;
    }

    $(function () {
        $('#dialog-confirm').dialog({
            autoOpen: false,

            show: 'blind',
            hide: 'blind',
            height: 220,
            width: 450,
            modal: true,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            close: function (event, ui) { $('body').css('overflow', 'auto'); },
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');
                    return false;
                },
                'Ok': function () {
                    $(this).dialog('close');
                    document.getElementById('<%= btnSalvaDatiAgo.ClientID %>').click();
                    return true;
                }
            }
        });
    });

    $(function () {
        $('#dialog-Contributivi').dialog({
            autoOpen: false,

            show: 'blind',
            hide: 'blind',
            height: 220,
            width: 450,
            modal: true,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            close: function (event, ui) { $('body').css('overflow', 'auto'); },
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');
                    return false;
                },
                'Ok': function () {
                    $(this).dialog('close');
                    document.getElementById('<%= btnSalvaDatiAgoNoRiduzione.ClientID %>').click();
                    return true;
                }
            }
        });
    });

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
<!--Questo sezione è condivisa da ES e GAS -->
<div id="divDatiAgo" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server"
    visible="false">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="lblDatiAgo" runat="server" Text="Dati Ago" Style="font-weight: bold" CssClass="section-label"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="width: 25%; visibility:hidden">
                <label>
                    Tipo Pensione:
                </label>
            </td>
            <td class="field" style="width: 25%; visibility:hidden">
                <asp:Label ID="lblTipoPensione" runat="server" />
                <asp:HiddenField ID="hdnTipoPensione" runat="server" />
            </td>
            <asp:Panel runat="server" ID="pnlTipoLiquidazioneES" Visible="false">
                <td class="Row1" style="width: 25%">
                    <label>
                        Tipo Liquidazione:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtTipoLiquidazioneES" runat="server" MaxLength="1" CssClass="tb8 txtUppercase"
                        Width="20%"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtTipoLiquidazioneES" runat="server" ControlToValidate="txtTipoLiquidazioneES"
                        Display="Dynamic" Enabled="true" ErrorMessage="Tipo Liquidazione: Inserire valori interi"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="^[0-9]$" />
                </td>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlTipoLiquidazioneGAS" Visible="false">
                <td class="Row1" style="width: 25%">
                    <label>
                        Tipo Liquidazione:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlTipoLiquidazioneGAS" CssClass="tb8 txtUppercase"
                        Width="30%">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="RFVddlTipoLiquidazioneGAS" ControlToValidate="ddlTipoLiquidazioneGAS"
                        Display="Dynamic" ErrorMessage="Tipo Liquidazione obbligatorio" ValidationGroup="UCTabDatiAgoGAS"
                        Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
            </asp:Panel>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Decorrenza AGO:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaDatiAgo" Width="60%"
                    Text="" CssClass="txtUppercase tb8" TabIndex="1" MaxLength="7" Enabled="false"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtDecorrenzaDatiAgo" ControlToValidate="txtDecorrenzaDatiAgo"
                    ErrorMessage="Decorrenza AGO in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAgoGAS" Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaDatiAgo" Display="Dynamic"
                    ErrorMessage="Decorrenza AGO: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS"
                    ID="customCheckDataDecorrenzaAGO" ClientValidationFunction="checkCorrettezzaData" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Sospensione:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtSospensioneAGO" Width="60%"
                    Text="" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="1" MaxLength="7"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtSospensioneAGO" ControlToValidate="txtSospensioneAGO"
                    ErrorMessage="Sospensione AGO in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAgoGAS" Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtSospensioneAGO" Display="Dynamic"
                    ErrorMessage="Sospensione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS"
                    ID="customCheckDataSospensione" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
        <tr>
            <asp:Panel runat="server" ID="pnlDecorrenzaTeoricaGAS" Visible="false">
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza Teorica:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaTeorica" Width="60%"
                        Text="" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="3" MaxLength="7"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ID="txtDecorrenzaTeorica_RF" Display="Dynamic"
                        ErrorMessage="Decorrenza Teorica: Inserire la decorrenza teorica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS"
                        ControlToValidate="txtDecorrenzaTeorica" Enabled="false"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="validateTxtDecorrenzaTeorica" ControlToValidate="txtDecorrenzaTeorica"
                        ErrorMessage="Data decorrenza teorica in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAgoGAS" Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaTeorica" Display="Dynamic"
                        ErrorMessage="Decorrenza Teorica: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS"
                        ID="CustomValidator1" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlSettimaneVV" Visible="true">
                <td class="Row1" style="width: 25%">
                    <label>
                        Settimane VV:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtSettimaneAnzianitaEsclusiva" runat="server" MaxLength="4" Width="60%"
                        CssClass="tb8 txtUppercase"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtSettimaneAnzianitaEsclusiva" runat="server"
                        ControlToValidate="txtSettimaneAnzianitaEsclusiva" Display="Dynamic" Enabled="true"
                        ErrorMessage="Settimane VV: Inserire valori interi" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS"
                        ValidationExpression="^[0-9]*$" />
                </td>
            </asp:Panel>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Anni Dif:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtAnniDifferimento" runat="server" MaxLength="2" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtAnniDifferimento" runat="server" ControlToValidate="txtAnniDifferimento"
                    Display="Dynamic" Enabled="true" ErrorMessage="Anni Dif: Inserire valori interi"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="^[0-9]*$" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Età maturazione req.:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtEtaMaturazioneRequisiti" runat="server" MaxLength="2" Width="60%"
                    CssClass="tb8 txtUppercase"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtEtaMaturazioneRequisiti" runat="server"
                    ControlToValidate="txtEtaMaturazioneRequisiti" Display="Dynamic" Enabled="true"
                    ErrorMessage="Età maturazione req.: Inserire valori interi" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS"
                    ValidationExpression="^[0-9]*$" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Specifico:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtCodiceSpecifico" runat="server" MaxLength="1" Width="20%" CssClass="tb8 txtUppercase"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtCodiceSpecifico" runat="server" ControlToValidate="txtCodiceSpecifico"
                    Display="Dynamic" Enabled="true" ErrorMessage="Codice Specifico: Inserire un carattere"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="^[a-zA-Z]?$" />
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlDatiAgoES" Width="100%" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza Teorica:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtboxDecorrenzaTeorica" runat="server" Width="60%" CssClass="txtUppercase tb8 date-picker dateMMaaaa"
                        MaxLength="7"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator14" ControlToValidate="txtboxDecorrenzaTeorica"
                        ErrorMessage="Decorrenza Teorica in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAgoGAS" Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtboxDecorrenzaTeorica" Display="Dynamic"
                        ErrorMessage="Decorrenza Teorica: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS"
                        ID="customCheckDataDecorrenzaTeorica" ClientValidationFunction="checkCorrettezzaData" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Versamenti Volontari:
                    </label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtboxVersamentiVolontari" runat="server" Width="60%" MaxLength="5"
                        CssClass="tb8 txtUppercase"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator12" runat="server"
                        ControlToValidate="txtboxVersamentiVolontari" Display="Dynamic" Enabled="true"
                        ErrorMessage="Versamente Volontari: Inserire valori interi" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS"
                        ValidationExpression="^[0-9]*$" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Contributi diff. quota:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtboxContributiDiffQuota" runat="server" Width="60%" MaxLength="9"
                        CssClass="tb8 txtUppercase"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator13" runat="server"
                        ControlToValidate="txtboxContributiDiffQuota" Display="Dynamic" Enabled="true"
                        ErrorMessage="Contributi diff. quota: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,4}(,\d{1,4})?$" />
                </td>
            </tr>
        </asp:Panel>
    </table>
</div>
<div id="divDatiRetributivi" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server"
    visible="false">
    <asp:Panel runat="server" ID="pnlDatiRetributivi" Visible="true">
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblDatiRetributivi" runat="server" Text="Dati Retributivi" Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblQuotaA" runat="server" Text="Quota A" Style="font-weight: bold"></asp:Label>
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
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,6}(,\d{1,4})?$" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Settimane totali:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtNSettimaneQuotaA" runat="server" MaxLength="4" CssClass="tb8 txtUppercase"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtNSettimaneQuotaA" runat="server" ControlToValidate="txtNSettimaneQuotaA"
                        Display="Dynamic" Enabled="true" ErrorMessage="Settimane totali Quota A: Inserire valori interi"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="^[0-9]*$" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Settimane esclusive:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtNSettimaneEsclusiveQuotaA" runat="server" MaxLength="4" CssClass="tb8 txtUppercase"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtNSettimaneEsclusiveQuotaA" runat="server"
                        ControlToValidate="txtNSettimaneEsclusiveQuotaA" Display="Dynamic" Enabled="true"
                        ErrorMessage="Settimane esclusive Quota A: Inserire valori interi" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS"
                        ValidationExpression="^[0-9]*$" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblQuotaB" runat="server" Text="Quota B" Style="font-weight: bold" CssClass="mt-16"></asp:Label>
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
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,6}(,\d{1,4})?$" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Settimane totali:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtNSettimaneQuotaB" runat="server" MaxLength="4" CssClass="tb8 txtUppercase"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtNSettimaneQuotaB" runat="server" ControlToValidate="txtNSettimaneQuotaB"
                        Display="Dynamic" Enabled="true" ErrorMessage="Settimane totali Quota B: Inserire valori interi"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="^[0-9]*$" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Settimane esclusive:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtNSettimaneEsclusiveQuotaB" runat="server" MaxLength="4" CssClass="tb8 txtUppercase"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtNSettimaneEsclusiveQuotaB" runat="server"
                        ControlToValidate="txtNSettimaneEsclusiveQuotaB" Display="Dynamic" Enabled="true"
                        ErrorMessage="Settimane esclusive Quota B: Inserire valori interi" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS"
                        ValidationExpression="^[0-9]*$" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlDatiRetributiviES" Visible="false">
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="Label1" runat="server" Text="Dati Retributivi" Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="Label2" runat="server" Text="Quota A" Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        RMS:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtboxQuotaA_RMS" runat="server" MaxLength="10" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtboxQuotaA_RMS" runat="server" ControlToValidate="txtboxQuotaA_RMS"
                        Display="Dynamic" Enabled="true" ErrorMessage="RMS Quota A: Inserire valori interi o decimali (max 5 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,5}(,\d{1,4})?$" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Sett. Anz. Tot:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtboxQuotaA_SettAnzTot" runat="server" MaxLength="4" CssClass="tb8 txtUppercase"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtboxQuotaA_SettAnzTot" runat="server" ControlToValidate="txtboxQuotaA_SettAnzTot"
                        Display="Dynamic" Enabled="true" ErrorMessage="Settimane totali Quota A: Inserire valori interi"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="^[0-9]*$" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Sett. Art. 24:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtboxSettArt24" runat="server" MaxLength="5" CssClass="tb8 txtUppercase"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtboxSettArt24" runat="server" ControlToValidate="txtboxSettArt24"
                        Display="Dynamic" Enabled="true" ErrorMessage="Sett. Art. 24: Inserire valori interi"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="^[0-9]*?$" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Sett. Art. 57:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtboxSettArt57" runat="server" MaxLength="5" CssClass="tb8 txtUppercase"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtboxSettArt57" runat="server" ControlToValidate="txtboxSettArt57"
                        Display="Dynamic" Enabled="true" ErrorMessage="Sett. Art. 57: Inserire valori interi"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="^[0-9]*?$" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Integr. Art. 11:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtboxIntegrArt11" runat="server" MaxLength="7" CssClass="tb8 txtUppercase"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtboxIntegrArt11" runat="server" ControlToValidate="txtboxIntegrArt11"
                        Display="Dynamic" Enabled="true" ErrorMessage="Integr. Art. 11: Inserire valori interi o decimali (max 2 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,2}(,\d{1,4})?$" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="Label3" runat="server" Text="Quota B" Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        RMS:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtboxQuotaB_RMS" runat="server" MaxLength="9" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtboxQuotaB_RMS" runat="server" ControlToValidate="txtboxQuotaB_RMS"
                        Display="Dynamic" Enabled="true" ErrorMessage="RMS Quota B: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,4}(,\d{1,4})?$" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Sett. Anz. Tot:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtboxQuotaB_SettAnzTot" runat="server" MaxLength="4" CssClass="tb8 txtUppercase"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtboxQuotaB_SettAnzTot" runat="server" ControlToValidate="txtboxQuotaB_SettAnzTot"
                        Display="Dynamic" Enabled="true" ErrorMessage="Settimane totali Quota A: Inserire valori interi"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="^[0-9]*$" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Sett. Art. 24:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtboxQuotaB_SettArt24" runat="server" MaxLength="4" CssClass="tb8 txtUppercase"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtboxQuotaB_SettArt24" runat="server" ControlToValidate="txtboxQuotaB_SettArt24"
                        Display="Dynamic" Enabled="true" ErrorMessage="Settimane Art 24: Inserire valori interi"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="^[0-9]*?$" />
                </td>
            </tr>
            <%--            <asp:Panel ID="pnlRiduzioneES" runat="server" Visible="false">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Riduzione Retributiva:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:DropDownList ID="ddlRiduzioneRetributivaES" CssClass="tb8" Width="30%" runat="server">
                            <asp:ListItem Text="NO" Value="false"></asp:ListItem>
                            <asp:ListItem Text="SI" Value="true"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Percentuale riduzione:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtPercentualeRiduzioneES" runat="server" CssClass="tb8 txtUppercase"
                            Width="30%" MaxLength="5"></asp:TextBox>
                             <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator15" Display="Dynamic"
                            ControlToValidate="txtPercentualeRiduzioneES" Enabled="true" ErrorMessage="Riduzione Retributiva: Inserire valori interi o decimali"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,2}(\,\d{1,2})?" />
                        <asp:CustomValidator runat="server" ControlToValidate="ddlRiduzioneRetributivaES" Display="Dynamic"
                            ErrorMessage="Riduzione Retributiva: La percentuale è obbligatoria avendo selezionato 'SI'"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ID="CustomValidator2" ClientValidationFunction="checkPercentualeRiduzione" />
                      
                        <label>
                            %</label>
                    </td>
                </tr>
            </asp:Panel>--%>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlRiduzioneRetributiva" runat="server" Visible="true">
        <table class="tabellaFormattazione  grid grid-size-20-col-5">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Riduzione Retributiva:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:DropDownList ID="ddlRiduzioneRetributiva" CssClass="tb8 txtUppercase xxs" Width="30%"
                        runat="server">
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Percentuale riduzione:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtRiduzioneRetributiva" runat="server" CssClass="tb8 txtUppercase"
                        Width="30%" MaxLength="5"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtRiduzioneRetributiva" Display="Dynamic"
                        ControlToValidate="txtRiduzioneRetributiva" Enabled="true" ErrorMessage="Riduzione Retributiva: Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,2}(\,\d{1,2})?" />
                    <asp:CustomValidator runat="server" ControlToValidate="ddlRiduzioneRetributiva" Display="Dynamic"
                        ErrorMessage="Riduzione Retributiva: La percentuale è obbligatoria avendo selezionato 'SI'"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ID="customRiduzione" ClientValidationFunction="checkPercentualeRiduzione" />
                    <label>
                        %</label>
                </td>
            </tr>
        </table>
    </asp:Panel>
</div>
<div id="divDatiContributivi" style="border-style: solid; border-color: #000080;
    border-collapse: collapse; border-width: 1px; width: 710px; margin-left: 4px;
    margin-top: 4px;" runat="server" visible="false">
    <asp:Panel runat="server" ID="pnlDatiContributivi" Visible="false">
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblDatiContributivi" runat="server" Text="Dati Contributivi" Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
            <asp:Panel runat="server" ID="pnlDatiContributiviES" Visible="false">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Contributi Totali:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox runat="server" ID="txtboxContributiTotali" CssClass="tb8 txtUppercase"
                            MaxLength="10" Width="60%"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" Display="Dynamic"
                            ControlToValidate="txtboxContributiTotali" Enabled="true" ErrorMessage="Contributi Totali: Inserire valori interi o decimali (max 5 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,5}(\,\d{1,4})?" />
                    </td>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Contributi Art. 24:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtboxContributiArt24" runat="server" CssClass="tb8 txtUppercase"
                            MaxLength="9" Width="60%"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3" Display="Dynamic"
                            ControlToValidate="txtboxContributiArt24" Enabled="true" ErrorMessage="Contributi Art. 24: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,4}(\,\d{1,4})?" />
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Contributi Art. 57:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtboxContributiArt57" runat="server" CssClass="tb8 txtUppercase"
                            MaxLength="8" Width="60%"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator4" Display="Dynamic"
                            ControlToValidate="txtboxContributiArt57" Enabled="true" ErrorMessage="Contributi Art. 57: Inserire valori interi o decimali (max 3 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,3}(\,\d{1,4})?" />
                    </td>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Supplemento Art. 14:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtboxSupplementoArt14" runat="server" CssClass="tb8 txtUppercase"
                            Width="60%" MaxLength="8"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator5" Display="Dynamic"
                            ControlToValidate="txtboxSupplementoArt14" Enabled="true" ErrorMessage="Supplemento Art. 14: Inserire valori interi o decimali (max 3 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,3}(\,\d{1,4})?" />
                    </td>
                </tr>
            </asp:Panel>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Montante totale:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox runat="server" ID="txtMontante" CssClass="tb8 txtUppercase" MaxLength="12"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtMontante" Display="Dynamic"
                        ControlToValidate="txtMontante" Enabled="true" ErrorMessage="Montante totale: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,7}(\,\d{1,4})?" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Montante esclusivo:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox runat="server" ID="txtMontanteEsclusivo" CssClass="tb8 txtUppercase"
                        MaxLength="12" Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtMontanteEsclusivo" Display="Dynamic"
                        ControlToValidate="txtMontanteEsclusivo" Enabled="true" ErrorMessage="Montante esclusivo: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,7}(\,\d{1,4})?" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Settimane esclusive:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtNSettimane" runat="server" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtNSettimane" Display="Dynamic"
                        ControlToValidate="txtNSettimane" Enabled="true" ErrorMessage="Settimane esclusive: Inserire valori interi"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="^[0-9]*$" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <!-- DATI L214 -->
    <asp:Panel runat="server" ID="pnlDatiContributiviL214" Visible="false">
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblDatiContributiviL214" runat="server" Text="Dati Contributivi L.214"
                        Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Montante:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox runat="server" ID="txtMontanteQuotaDL214" CssClass="tb8 txtUppercase"
                        MaxLength="11" Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtMontanteQuotaDL214_ES" Display="Dynamic"
                        ControlToValidate="txtMontanteQuotaDL214" Enabled="false" ErrorMessage="Montante L.214: Inserire valori interi o decimali (max 7 interi e 2 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,7}(\,\d{1,2})?" />
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtMontanteQuotaDL214" Display="Dynamic"
                        ControlToValidate="txtMontanteQuotaDL214" Enabled="true" ErrorMessage="Montante L.214: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,7}(\,\d{1,4})?" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtMontanteDL214" ControlToValidate="txtMontanteQuotaDL214"
                        Display="Dynamic" Enabled="true" ErrorMessage="Montante L.214: campo obbligatorio"
                        ValidationGroup="UCTabDatiAgoGAS" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Settimane:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtNSettimaneQuotaDL214" runat="server" CssClass="tb8 txtUppercase"
                        MaxLength="4" Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtNSettimaneQuotaDL214" Display="Dynamic"
                        ControlToValidate="txtNSettimaneQuotaDL214" Enabled="true" ErrorMessage="Settimane L.214: Inserire valori interi"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="^[0-9]*$" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtNSettimaneQuotaDL214" ControlToValidate="txtNSettimaneQuotaDL214"
                        Display="Dynamic" Enabled="true" ErrorMessage="Settimane L.214: campo obbligatorio"
                        ValidationGroup="UCTabDatiAgoGAS" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <asp:Panel runat="server" ID="pnlImportoContributivo" Visible="false">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Montante esclusivo L.214:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtboxL214_ImportoContributivo" runat="server" CssClass="tb8 txtUppercase"
                            MaxLength="11" Width="60%"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="validateTxtRetribuzione" Display="Dynamic"
                            ControlToValidate="txtboxL214_ImportoContributivo" Enabled="true" ErrorMessage="Montante esclusivo L.214: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,6}(\,\d{1,4})?" />                        
                    </td>
                </tr>
            </asp:Panel>
            <asp:Panel ID="pnlMontanteEscusivoGAS" runat="server" Visible="false">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Montante esclusivo:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox runat="server" ID="txtMontanteEsclusivoQuotaDL214" CssClass="tb8 txtUppercase"
                            MaxLength="12" Width="60%"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="REVtxtMontanteEsclusivoQuotaDL214"
                            Display="Dynamic" ControlToValidate="txtMontanteEsclusivoQuotaDL214" Enabled="true"
                            ErrorMessage="Montante escusivo L.214: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,7}(\,\d{1,4})?" />
                    </td>
                </tr>
            </asp:Panel>
        </table>
    </asp:Panel>
</div>
<!-- Pannello doppio calcolo -->
<UCDC:UCDoppioCalcolo runat="server" ID="ucDoppioCalcolo" Visible="false" />
<!-- Fine Pannello doppio calcolo -->
<div id="divAltraPensione" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server"
    visible="false">
    <asp:Panel runat="server" ID="pnlAltraPensione" Visible="false">
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="Label4" runat="server" Text="Altra Pensione" Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Base:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox runat="server" ID="txtAltraPensione" CssClass="tb8 txtUppercase" MaxLength="9"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtAltraPensione" Display="Dynamic"
                        ControlToValidate="txtAltraPensione" Enabled="true" ErrorMessage="Base: Inserire valori interi o decimali (max 3 interi e 5 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{3,7}(\,\d{1,5})?" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Categoria:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:DropDownList runat="server" TabIndex="1" ID="ddlCategoriaPensione" CssClass="tb8 txtUppercase"
                        Width="100px" Enabled="true">
                    </asp:DropDownList>
                    <asp:CustomValidator EnableClientScript="true" runat="server" Display="Dynamic" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCPensioneDirettaDC" ID="ddlCategoriaPensione_CV" ClientValidationFunction="validateDropDownList"
                        ErrorMessage="Selezionare la categoria" />
                </td>
            </tr>
            <%--        <asp:Panel runat="server" ID="Panel2" Visible="false">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Importo Contributibo:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtAltraPensImpContrib" runat="server" CssClass="tb8" MaxLength="4"
                            Width="60%"></asp:TextBox>
                        <%-- <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" Display="Dynamic"
                        ControlToValidate="txtNSettimaneQuotaDL214" Enabled="true" ErrorMessage="Settimane L.214: Inserire valori interi"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="^[0-9]*$" />
                    </td>
                </tr>
            </asp:Panel>--%>
        </table>
    </asp:Panel>
</div>
<div style="margin-right: 40px;" class="containerWidth xs">
    <table width="100%" style="min-height: 100px;" class="tab-actions-group">
        <tr>
            <td style="text-align: right; vertical-align: bottom;"  class="tab-actions-group__first">
                <asp:Button ID="btnPopUpContributivi" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Style="display: none" Text="Salva Dati Ago" Width="150px" OnClientClick="if(Page_ClientValidate('UCTabDatiAgoGAS')){return ConfirmContributivi();}" CssClass="primary" />
                <asp:Button ID="btnPopUp" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Visible="false" Text="Salva Dati Ago" Width="150px" OnClientClick="if(Page_ClientValidate('UCTabDatiAgoGAS')){return Confirm();}" CssClass="primary" />
                <asp:Button ID="btnSalvaDatiAgo" runat="server" CausesValidation="false" Style="display: none"
                    ValidationGroup="UCTabDatiAgoGAS" SkinID="btnAzione1" Width="150px" OnClick="btnSalvaDatiAgo_Click"
                    Text="Salva Dati Ago" Visible="false" OnClientClick="if(Page_ClientValidate('UCTabDatiAgoGAS')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
                <asp:Button ID="btnSalvaDatiAgoNoRiduzione" runat="server" CausesValidation="false"
                    ValidationGroup="UCTabDatiAgoGAS" SkinID="btnAzione1" Width="150px" OnClick="btnSalvaDatiAgo_Click"
                    Text="Salva Dati Ago" Visible="true" OnClientClick="if(Page_ClientValidate('UCTabDatiAgoGAS')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
            </td>
            <td style="text-align: left; vertical-align: bottom;">
                <asp:Button ID="btnEliminaDatiAgo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Dati Ago" Width="150px" OnClick="btnEliminaDatiAgo_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Ago?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
<div id="dialog-confirm" title="Confirm" style="border-style: none; border-color: White;">
    <p>
        <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
        Età titolare inferiore a 62 anni. Confermi la mancanza della percentuale di Riduzione?</p>
</div>
<div id="dialog-Contributivi" title="Confirm" style="border-style: none; border-color: White;">
    <p>
        <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
        Attenzione il Montante è inferiore all’Ammontare.<br />
        Confermare ?</p>
</div>
