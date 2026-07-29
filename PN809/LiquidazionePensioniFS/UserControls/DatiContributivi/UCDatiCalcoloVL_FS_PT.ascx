<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiCalcoloVL_FS_PT.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiCalcoloVL_FS_PT" %>
<style type="text/css">
    .fixed-dialog
    {
        position: fixed;
    }
</style>
<script type="text/javascript">

    function validateMese(source, args) {
        var mesi = args.Value;
        if (mesi < 0 || mesi > 12)
            args.IsValid = false;
        else
            args.IsValid = true;
        return false;
    }

    function validateGiorno(source, args) {
        var giorni = args.Value;
        if (giorni < 0 || giorni > 366)
            args.IsValid = false;
        else
            args.IsValid = true;
        return false;
    }

    function Confirm() {
        var ddl = document.getElementById('<%= ddlRiduzioneRetributiva.ClientID %>');
        var selectedValue = ddl.options[ddl.selectedIndex].value;
        if (selectedValue.toUpperCase() == 'SI')
            document.getElementById('<%= btnSalvaDatiCalcolo.ClientID %>').click();
        else
            $('#dialog-confirm').dialog('open');

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
                    document.getElementById('<%= btnSalvaDatiCalcolo.ClientID %>').click();
                    return true;
                }
            }
        });
    });


    function ConfirmContributivi() {
        if (CheckAmmontareMaggioreDiMontante()) {
            $('#dialog-Contributivi').dialog('open');
        }
        else {
            document.getElementById('<%= btnSalvaDatiCalcoloNoRiduzione.ClientID %>').click();
        }
        return false;
    }

    function CheckAmmontareMaggioreDiMontante() {
        var montante, ammontare;
        if ($('#<%=HdnFondo.ClientID %>').val() == "FS" || $('#<%=HdnFondo.ClientID %>').val() == "PT") {
            txtMontante = document.getElementById('<%= txtMontanteFS_PT.ClientID %>');
            txtAmmontare = document.getElementById('<%= txtImportoContributivoTotaleFS_PT.ClientID %>');
            if (txtMontante)
                montante = parseFloat(txtMontante.value);
            if (txtAmmontare)
                ammontare = parseFloat(txtAmmontare.value);
        }
        else {
            //FONDO VL
            montante = 0;
            if ($('#<%= txtMontanteDa0697_VL.ClientID %>'))
                montante += parseFloat($('#<%= txtMontanteDa0697_VL.ClientID %>').val());
            if ($('#<%= txtMontanteDa0196a0697_VL.ClientID %>'))
                montante += parseFloat($('#<%= txtMontanteDa0196a0697_VL.ClientID %>').val());

            if ($('#<%= txtImportTotale335_VL.ClientID %>'))
                ammontare = parseFloat($('#<%= txtImportTotale335_VL.ClientID %>').val());
        }
        if (montante && ammontare && ammontare > montante)
            return true;
        return false;
    }

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
                    document.getElementById('<%= btnSalvaDatiCalcoloNoRiduzione.ClientID %>').click();
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

    function sommaSettimane() {
        var settimaneUtili = document.getElementById("<%=txtSettimaneUtiliDiritto.ClientID %>");
        var settimaneUtiliOI = document.getElementById("<%=txtNumeroSettimaneOI.ClientID %>");
        var totaleSettimane = document.getElementById("<%=txtNumeroSettimaneTot.ClientID %>");

        var valore1 = parseInt(settimaneUtili.value) || 0;
        var valore2 = parseInt(settimaneUtiliOI.value) || 0;

        totaleSettimane.value = valore1 + valore2;
    }
</script>
<asp:Panel ID="pnlSettimane_VL" runat="server" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20" width="100%">
        <tr>
            <td class="Row1" style="width: 33%">
                <label runat="server" id="lblNumeroSettimane">
                    Settimane Utili al Diritto:</label>
            </td>
            <td class="Row1" style="width: 17%">
                <asp:TextBox runat="server" ID="txtSettimaneUtiliDiritto" CssClass="tb8 txtUppercase"
                    Width="60%" MaxLength="4" onblur="sommaSettimane();"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="revSettimaneUtiliDiritto" ControlToValidate="txtSettimaneUtiliDiritto"
                    Display="Dynamic" ErrorMessage="Settimane Utili al Diritto non valide: inserire il numero di settimane in un formato valido"
                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
            </td>
            <td class="Row1" style="width: 30%">
            </td>
            <td class="Row1" style="width: 20%">
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlNSettimane_OrganizzazioniInternazionali">
            <tr>
                <td class="Row1">
                    <label>
                        Numero Settimane OI:</label><label runat="server" id="lbltest" visible="false"/>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtNumeroSettimaneOI" Width="60%" CssClass="txtUppercase tb8" TabIndex="7" MaxLength="4"
                        onblur="sommaSettimane();"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator36" ControlToValidate="txtNumeroSettimaneOI"
                    Display="Dynamic" ErrorMessage="Settimane Utili al Diritto non valide: inserire il numero di settimane in un formato valido"
                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        Numero Settimane Utili:</label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtNumeroSettimaneTot" Width="60%"
                        ReadOnly="true" MaxLength="4" CssClass="txtUppercase tb8" TabIndex="8"></asp:TextBox>
                </td>
            </tr>
        </asp:Panel>
    </table>
</asp:Panel>
<asp:Panel ID="pnlDatiCalcolo" runat="server">
    <div id="divBorder" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-bottom: 8px; margin-top: 4px;">
        <!-- Inizio Pannello Common FS_PT -->
        <asp:Panel ID="pnlDatiCommonFS_PT" runat="server" Visible="false">
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Pensione Annua Lorda:</label>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <asp:TextBox ID="txtPensioneAnnuaLorda" runat="server" CssClass="tb8 txtUppercase"
                            Width="60%" TabIndex="1" MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" Display="Dynamic"
                            ControlToValidate="txtPensioneAnnuaLorda" Enabled="true" ErrorMessage="Pensione Annua Lorda: Inserire valori interi o decimali"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d+(\,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtPensioneAnnuaLorda"
                            Display="Dynamic" Enabled="true" ErrorMessage="Pensione Annua Lorda: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcoloVL" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                    <td class="Row1" style="width: 24%">
                        <label>
                            Anni Servizio Utili Diritto:</label>
                    </td>
                    <td class="Row1 fileds-date-input" style="width: 29%">
                        <asp:TextBox ID="txtAnniServUtiliDirittoAA" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" TabIndex="2" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" ControlToValidate="txtAnniServUtiliDirittoAA"
                            Display="Dynamic" ErrorMessage="Anni di Servizio Utili per il Diritto: inserire il numero di anni in un formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator5" ControlToValidate="txtAnniServUtiliDirittoAA"
                            Display="Dynamic" Enabled="true" ErrorMessage="Anni Servizio Utili Diritto: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcoloVL" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <asp:Panel runat="server" ID="pnlAnniServizioUtiliDirittoPerAutomatiche" Visible="false"
                            Style="display: inline;">
                            <label>
                                AA</label>
                            <asp:TextBox ID="txtAnniServUtiliDirittoMM" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="REV_txtAnniServUtiliDirittoMM"
                                ControlToValidate="txtAnniServUtiliDirittoMM" Display="Dynamic" ErrorMessage="Anni di Servizio Utili per il Diritto: inserire il numero di mesi in un formato valido"
                                Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                            <asp:RequiredFieldValidator runat="server" ID="RFV_txtAnniServUtiliDirittoMM" ControlToValidate="txtAnniServUtiliDirittoMM"
                                Display="Dynamic" Enabled="true" ErrorMessage="Anni Servizio Utili Diritto: campo obbligatorio"
                                ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                            <label>
                                MM</label>
                            <asp:TextBox ID="txtAnniServUtiliDirittoGG" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="REV_txtAnniServUtiliDirittoGG"
                                ControlToValidate="txtAnniServUtiliDirittoGG" Display="Dynamic" ErrorMessage="Anni di Servizio Utili per il Diritto: inserire il numero di giorni in un formato valido"
                                Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                            <asp:RequiredFieldValidator runat="server" ID="RFV_txtAnniServUtiliDirittoGG" ControlToValidate="txtAnniServUtiliDirittoGG"
                                Display="Dynamic" Enabled="true" ErrorMessage="Anni Servizio Utili Diritto: campo obbligatorio"
                                ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                            <label>
                                GG</label>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- Fine Pannello Common FS_PT -->
        <br />
        <asp:Panel ID="pnlDatiRetributivi" runat="server" Visible="false">
            <!-- Pannello Dati Calcolo Retributivi VL-->
            <asp:Panel ID="pnlDatiRetributiviVL" runat="server" Visible="false">
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1 shift-full-grid" style="text-align: left">
                            <asp:Label ID="lblDecretoLegislativo164DatiRetrib" runat="server" Text="Decreto Legislativo 164"
                                Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <asp:Label ID="lblRetribuzioneMediaSettADatiRetrib" runat="server" Text="Retribuzione Media Settimanale A:"></asp:Label>
                        </td>
                        <td class="Row1" style="width: 25%">
                            <asp:TextBox ID="txtRetribuzioneMediaSettADatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                                Width="130" TabIndex="3" MaxLength="11"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="validateTxtRetribuzioneMediaSettADatiRetrib"
                                Display="Dynamic" ControlToValidate="txtRetribuzioneMediaSettADatiRetrib" Enabled="true"
                                ErrorMessage="Retribuzione Media Settimanale A: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d{0,6}(,\d{1,4})?" />
                        </td>
                        <td class="Row1" style="text-align: right; width: 20%">
                            <asp:Label ID="lblSettimaneA1DatiRetrib" runat="server" Text="Settimane A1:"></asp:Label>
                        </td>
                        <td style="width: 35px" class="none">
                        </td>
                        <td class="Row1" style="width: 30%">
                            <asp:TextBox ID="txtSettimaneA1DatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                                Width="130" TabIndex="4" MaxLength="4"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneA1DatiRetrib"
                                ControlToValidate="txtSettimaneA1DatiRetrib" Display="Dynamic" ErrorMessage="Numero settimane A1 non valido: inserire il numero di settimane in un formato valido"
                                Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1 none" style="width: 25%">
                        </td>
                        <td class="Row1 none" style="width: 25%">
                        </td>
                        <td class="Row1" style="text-align: right; width: 20%">
                            <asp:Label ID="lblSettimaneA2DatiRetrib" runat="server" Text="Settimane A2:"></asp:Label>
                        </td>
                        <td style="width: 20px" class="none">
                        </td>
                        <td class="Row1" style="width: 30%">
                            <asp:TextBox ID="txtSettimaneA2DatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                                Width="130" TabIndex="5" MaxLength="4"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneA2DatiRetrib"
                                ControlToValidate="txtSettimaneA2DatiRetrib" Display="Dynamic" ErrorMessage="Numero settimane A2 non valido: inserire il numero di settimane in un formato valido"
                                Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <asp:Label ID="lblRetribuzioneMediaSettBDatiRetrib" runat="server" Text="Retribuzione Media Settimanale B:"></asp:Label>
                        </td>
                        <td class="Row1" style="width: 25%">
                            <asp:TextBox ID="txtRetribuzioneMediaSettBDatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                                Width="130" TabIndex="6" MaxLength="11"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="validateTxtRetribuzioneMediaSettBDatiRetrib"
                                Display="Dynamic" ControlToValidate="txtRetribuzioneMediaSettBDatiRetrib" Enabled="true"
                                ErrorMessage="Retribuzione Media Settimanale B: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d{0,6}(,\d{1,4})?" />
                        </td>
                        <td class="Row1" style="text-align: right; width: 20%">
                            <asp:Label ID="lblSettimaneBDatiRetrib" runat="server" Text="Settimane B:"></asp:Label>
                        </td>
                        <td style="width: 20px" class="none">
                        </td>
                        <td class="Row1" style="width: 30%">
                            <asp:TextBox ID="txtSettimaneBDatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                                Width="130" TabIndex="7" MaxLength="4"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneBDatiRetrib"
                                ControlToValidate="txtSettimaneBDatiRetrib" Display="Dynamic" ErrorMessage="Numero settimane B non valido: inserire il numero di settimane in un formato valido"
                                Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1 none" style="width: 25%">
                        </td>
                        <td class="Row1 none" style="width: 25%">
                        </td>
                        <td class="Row1" style="text-align: right; width: 20%">
                            <asp:Label ID="lblSettimaneC1DatiRetrib" runat="server" Text="Settimane C1:"></asp:Label>
                        </td>
                        <td style="width: 20px" class="none">
                        </td>
                        <td class="Row1" style="width: 30%">
                            <asp:TextBox ID="txtSettimaneC1DatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                                Width="130" TabIndex="8" MaxLength="4"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneC1DatiRetrib"
                                ControlToValidate="txtSettimaneC1DatiRetrib" Display="Dynamic" ErrorMessage="Numero settimane C1 non valido: inserire il numero di settimane in un formato valido"
                                Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                        </td>
                    </tr>
                </table>
                <asp:Panel ID="pnlDatiRetributiviCustomVL" runat="server" Visible="false">
                    <table class="tabellaFormattazione grid grid-size-20">
                        <tr>
                            <td class="Row1 none" style="width: 25%">
                            </td>
                            <td class="Row1 none" style="width: 25%">
                            </td>
                            <td class="Row1" style="text-align: right; width: 20%">
                                <asp:Label ID="lblSettimaneC2DatiRetrib" runat="server" Text="Settimane C2:"></asp:Label>
                            </td>
                            <td style="width: 20px" class="none">
                            </td>
                            <td class="Row1" style="width: 30%">
                                <asp:TextBox ID="txtSettimaneC2DatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                                    Width="130" TabIndex="9" MaxLength="4"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneC2DatiRetrib"
                                    ControlToValidate="txtSettimaneC2DatiRetrib" Display="Dynamic" ErrorMessage="Numero settimane C2 non valido: inserire il numero di settimane in un formato valido"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1" style="width: 25%">
                                <asp:Label ID="lblRetribuzioneMediaSettDDatiRetrib" runat="server" Text="Retribuzione Media Settimanale D:"></asp:Label>
                            </td>
                            <td class="Row1" style="width: 25%">
                                <asp:TextBox ID="txtRetribuzioneMediaSettDDatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                                    Width="130" TabIndex="10" MaxLength="11"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtRetribuzioneMediaSettDDatiRetrib"
                                    Display="Dynamic" ControlToValidate="txtRetribuzioneMediaSettDDatiRetrib" Enabled="true"
                                    ErrorMessage="Retribuzione Media Settimanale D: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d{0,6}(,\d{1,4})?" />
                            </td>
                            <td class="Row1" style="text-align: right; width: 20%">
                                <asp:Label ID="lblSettimaneDDatiRetrib" runat="server" Text="Settimane D:"></asp:Label>
                            </td>
                            <td style="width: 20px" class="none">
                            </td>
                            <td class="Row1" style="width: 30%">
                                <asp:TextBox ID="txtSettimaneDDatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                                    Width="130" TabIndex="11" MaxLength="4"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneDDatiRetrib"
                                    ControlToValidate="txtSettimaneDDatiRetrib" Display="Dynamic" ErrorMessage="Numero settimane D non valido: inserire il numero di settimane in un formato valido"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </asp:Panel>
            <!-- Fine Pannello Dati Calcolo Retributivi VL-->
            <!-- Pannello Dati Calcolo Retributivi FS_PT-->
            <asp:Panel ID="pnlDatiRetributiviFS_PT" runat="server" Visible="false">
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr style="min-height: 50px; vertical-align: bottom">
                        <td class="Row1" style="text-align: left">
                            <asp:Label ID="lblDatiRetributivi" runat="server" Text="Dati Retributivi:" Style="font-weight: bold;
                                font-size: 15px;"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1" style="text-align: left">
                            <asp:Label ID="lblQuotaA" runat="server" Text="QUOTA A" Style="font-weight: bold"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1" style="text-align: left">
                            <asp:Label ID="lblData92" runat="server" Text="Dati al 31/12/92" Style="font-weight: bold"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1" style="width: 22%">
                            <label>
                                Servizio Utile:</label>
                        </td>
                        <td class="Row1 fileds-date-input" style="width: 34%">
                            <asp:TextBox ID="txtServizioUtileAAQtaA" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="12" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator9" ControlToValidate="txtServizioUtileAAQtaA"
                                ErrorMessage="Servizio Utile al 31/12/92: formato Anno non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloVL" />
                            <label>
                                AA</label>
                            <asp:TextBox ID="txtServizioUtileMMQtaA" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="13" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator10" ControlToValidate="txtServizioUtileMMQtaA"
                                ErrorMessage="Servizio Utile al 31/12/92: formato Mese non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloVL" />
                            <label>
                                MM</label>
                            <asp:TextBox ID="txtServizioUtileGGQtaA" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="14" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator11" ControlToValidate="txtServizioUtileGGQtaA"
                                ErrorMessage="Servizio Utile al 31/12/92: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloVL" />
                            <label>
                                GG</label>
                        </td>
                        <td class="Row1" style="width: 24%">
                            <label>
                                Retribuzione:</label>
                        </td>
                        <td class="Row1" style="width: 20%">
                            <asp:TextBox ID="txtRetribuzioneQtaA" runat="server" CssClass="tb8 txtUppercase"
                                Width="75%" TabIndex="15" MaxLength="11"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator6" Display="Dynamic"
                                ControlToValidate="txtRetribuzioneQtaA" Enabled="true" ErrorMessage="Retribuzione: Inserire valori interi o decimali"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d+(\,\d{1,4})?" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1" colspan="2">
                        </td>
                        <td class="Row1" style="width: 24%">
                            <label>
                                Quota Art. 14:</label>
                        </td>
                        <td class="Row1" style="width: 20%">
                            <asp:TextBox ID="txtQuotaArt14QtaA" runat="server" CssClass="tb8 txtUppercase" Width="75%"
                                TabIndex="16" MaxLength="9"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RVtxtQuotaArt14QtaA" ControlToValidate="txtQuotaArt14QtaA"
                                Display="Dynamic" ErrorMessage="Quota Art. 14: Inserire massimo 4 cifre intere e 4 decimali"
                                Text="*" CssClass="field-is-required" ValidationExpression="\d{1,4}(\,\d{1,4})?" ValidationGroup="UCTabDatiCalcoloVL" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1" colspan="2">
                        </td>
                        <td class="Row1" style="width: 24%">
                            <label>
                                Importo Indennità Integrativa Speciale:</label>
                        </td>
                        <td class="Row1" style="width: 20%">
                            <asp:TextBox ID="txtImpIndenIntegrSpecQtaA" runat="server" CssClass="tb8 txtUppercase"
                                Width="75%" TabIndex="17" MaxLength="11"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator7" Display="Dynamic"
                                ControlToValidate="txtImpIndenIntegrSpecQtaA" Enabled="true" ErrorMessage="Importo Indennità Integrativa Speciale: Inserire valori interi o decimali"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d+(\,\d{1,4})?" />
                        </td>
                    </tr>
                    <asp:Panel ID="pnl336FS" runat="server" Visible="false">
                        <tr>
                            <td class="Row1" colspan="2">
                            </td>
                            <td class="Row1" style="width: 24%">
                                <label>
                                    Retribuzione senza benefici L.336/70:</label>
                            </td>
                            <td class="Row1" style="width: 20%">
                                <asp:TextBox runat="server" ID="txtRetribuzioneSenzaBenefici336" CssClass="txtUppercase tb8 offClass onClassLegge336"
                                    TabIndex="18" MaxLength="11" Width="75%" />
                                <asp:RegularExpressionValidator runat="server" ID="txtRetribuzioneSenzaBenefici336_RV"
                                    ControlToValidate="txtRetribuzioneSenzaBenefici336" Display="Dynamic" ErrorMessage="Retribuzione senza benefici L.336/70: Inserire massimo 6 cifre intere e 4 decimali"
                                    Text="*" CssClass="field-is-required" ValidationExpression="\d{1,6}(\,\d{1,4})?" ValidationGroup="UCTabDatiCalcoloVL" />
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnlQuotaRetributivaAnnua" runat="server" Visible="false">
                        <tr>
                            <td class="Row1" colspan="2">
                            </td>
                            <td class="Row1" style="width: 24%">
                                <label>
                                    Quota pensione retributiva annua:</label>
                            </td>
                            <td class="Row1" style="width: 20%">
                                <asp:TextBox ID="txtQuotaRetributivaAnnua" runat="server" CssClass="tb8 txtUppercase"
                                    Width="75%" TabIndex="17" MaxLength="11"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator23"
                                    Display="Dynamic" ControlToValidate="txtQuotaRetributivaAnnua" Enabled="true"
                                    ErrorMessage="QuotaRetributivaAnnua: Inserire valori interi o decimali" Text="*" CssClass="field-is-required"
                                    ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d+(\,\d{1,4})?" />
                            </td>
                        </tr>
                    </asp:Panel>
                </table>
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1" style="text-align: left">
                            <asp:Label ID="lblQuotaB" runat="server" Text="QUOTA B" Style="font-weight: bold"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1" style="text-align: left">
                            <asp:Label ID="lblData94" runat="server" Text="Dati al 31/12/94" Style="font-weight: bold"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1" style="width: 22%">
                            <label>
                                Servizio Utile:</label>
                        </td>
                        <td class="Row1 fileds-date-input" style="width: 34%">
                            <asp:TextBox ID="txtServizioUtileAAQtaB1" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="19" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator8" ControlToValidate="txtServizioUtileAAQtaB1"
                                ErrorMessage="Servizio Utile al 31/12/94: formato Anno non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloVL" />
                            <label>
                                AA</label>
                            <asp:TextBox ID="txtServizioUtileMMQtaB1" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="20" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator12" ControlToValidate="txtServizioUtileMMQtaB1"
                                ErrorMessage="Servizio Utile al 31/12/94: formato Mese non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloVL" />
                            <label>
                                MM</label>
                            <asp:TextBox ID="txtServizioUtileGGQtaB1" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="21" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator14" ControlToValidate="txtServizioUtileGGQtaB1"
                                ErrorMessage="Servizio Utile al 31/12/94: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloVL" />
                            <label>
                                GG</label>
                        </td>
                        <td class="Row1" style="width: 24%">
                            <label>
                                Retribuzione Media:</label>
                        </td>
                        <td class="Row1" style="width: 20%">
                            <asp:TextBox ID="txtRMSQtaB1" runat="server" CssClass="tb8 txtUppercase" Width="75%"
                                TabIndex="22" MaxLength="11"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator15"
                                Display="Dynamic" ControlToValidate="txtRMSQtaB1" Enabled="true" ErrorMessage="Retribuzione Media Quota B: Inserire valori interi o decimali"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d+(\,\d{1,4})?" />
                        </td>
                        <asp:Panel ID="pnlQuotaRetributivaAnnuaB94" runat="server" Visible="false">
                            <tr>
                                <td class="Row1" colspan="2">
                                </td>
                                <td class="Row1" style="width: 24%">
                                    <label>
                                        Quota pensione retributiva annua:</label>
                                </td>
                                <td class="Row1" style="width: 20%">
                                    <asp:TextBox ID="txtQuotaPensioneRetributivaAnnuaB94" runat="server" CssClass="tb8 txtUppercase"
                                        Width="75%" TabIndex="17" MaxLength="11"></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator24"
                                        Display="Dynamic" ControlToValidate="txtQuotaPensioneRetributivaAnnuaB94" Enabled="true"
                                        ErrorMessage="QuotaRetributivaAnnua: Inserire valori interi o decimali" Text="*" CssClass="field-is-required"
                                        ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d+(\,\d{1,4})?" />
                                </td>
                            </tr>
                        </asp:Panel>
                    </tr>
                </table>
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1" style="text-align: left">
                            <asp:Label ID="Label2" runat="server" Text="Dati al 31/12/95" Style="font-weight: bold"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1" style="width: 22%">
                            <label>
                                Servizio Utile:</label>
                        </td>
                        <td class="Row1 fileds-date-input">
                            <asp:TextBox ID="txtServizioUtileAAQtaB2" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="23" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator16" ControlToValidate="txtServizioUtileAAQtaB2"
                                ErrorMessage="Servizio Utile al 31/12/95: formato Anno non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloVL" />
                            <label>
                                AA</label>
                            <asp:TextBox ID="txtServizioUtileMMQtaB2" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="24" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator17" ControlToValidate="txtServizioUtileMMQtaB2"
                                ErrorMessage="Servizio Utile al 31/12/95: formato Mese non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloVL" />
                            <label>
                                MM</label>
                            <asp:TextBox ID="txtServizioUtileGGQtaB2" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="25" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator18" ControlToValidate="txtServizioUtileGGQtaB2"
                                ErrorMessage="Servizio Utile al 31/12/95: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloVL" />
                            <label>
                                GG</label>
                        </td>
                        <asp:Panel ID="pnlQuotaPensioneRetributivaAnnuaB95" runat="server" Visible="false">
                            <td class="Row1" colspan="2">
                            </td>
                            <td class="Row1" style="width: 24%">
                                <label>
                                    Quota pensione retributiva annua:</label>
                            </td>
                            <td class="Row1" style="width: 20%">
                                <asp:TextBox ID="txtQuotaPensioneRetributivaAnnuaB95" runat="server" CssClass="tb8 txtUppercase"
                                    Width="75%" TabIndex="17" MaxLength="11"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator33"
                                    Display="Dynamic" ControlToValidate="txtQuotaPensioneRetributivaAnnuaB95" Enabled="true"
                                    ErrorMessage="QuotaRetributivaAnnua: Inserire valori interi o decimali" Text="*" CssClass="field-is-required"
                                    ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d+(\,\d{1,4})?" />
                            </td>
                        </asp:Panel>
                    </tr>
                </table>
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1" style="text-align: left">
                            <asp:Label ID="lblData97" runat="server" Text="Dati al 31/12/97" Style="font-weight: bold"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1" style="width: 22%">
                            <label>
                                Servizio Utile:</label>
                        </td>
                        <td class="Row1 fileds-date-input">
                            <asp:TextBox ID="txtServizioUtileAAQtaB3" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="26" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator19" ControlToValidate="txtServizioUtileAAQtaB3"
                                ErrorMessage="Servizio Utile al 31/12/97: formato Anno non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloVL" />
                            <label>
                                AA</label>
                            <asp:TextBox ID="txtServizioUtileMMQtaB3" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="27" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator20" ControlToValidate="txtServizioUtileMMQtaB3"
                                ErrorMessage="Servizio Utile al 31/12/97: formato Mese non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloVL" />
                            <label>
                                MM</label>
                            <asp:TextBox ID="txtServizioUtileGGQtaB3" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="28" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator21" ControlToValidate="txtServizioUtileGGQtaB3"
                                ErrorMessage="Servizio Utile al 31/12/97: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloVL" />
                            <label>
                                GG</label>
                        </td>
                        <asp:Panel ID="pnlQuotaPensioneRetributivaAnnuaB97" runat="server" Visible="false">
                            <td class="Row1" colspan="2">
                            </td>
                            <td class="Row1" style="width: 24%">
                                <label>
                                    Quota pensione retributiva annua:</label>
                            </td>
                            <td class="Row1" style="width: 20%">
                                <asp:TextBox ID="txtQuotaPensioneRetributivaAnnuaB97" runat="server" CssClass="tb8 txtUppercase"
                                    Width="75%" TabIndex="17" MaxLength="11"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator34"
                                    Display="Dynamic" ControlToValidate="txtQuotaPensioneRetributivaAnnuaB97" Enabled="true"
                                    ErrorMessage="QuotaRetributivaAnnua: Inserire valori interi o decimali" Text="*" CssClass="field-is-required"
                                    ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d+(\,\d{1,4})?" />
                            </td>
                        </asp:Panel>
                    </tr>
                </table>
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1" style="text-align: left">
                            <asp:Label ID="lblCessazione" runat="server" Text="Dati Cessazione" Style="font-weight: bold"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1" style="width: 22%">
                            <label>
                                Servizio Utile:</label>
                        </td>
                        <td class="Row1 fileds-date-input">
                            <asp:TextBox ID="txtServizioUtileCessazioneAA" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="29" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator25" ControlToValidate="txtServizioUtileCessazioneAA"
                                ErrorMessage="Servizio Utile Cessazione: formato Anno non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloVL" />
                            <%--<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator6" ControlToValidate="txtServizioUtileCessazioneAA"
                                Display="Dynamic" Enabled="true" ErrorMessage="Servizio Utile Cessazione AA: campo obbligatorio" ValidationGroup="UCTabDatiCalcoloVL"
                                Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>--%>
                            <label>
                                AA</label>
                            <asp:TextBox ID="txtServizioUtileCessazioneMM" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="30" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator26" ControlToValidate="txtServizioUtileCessazioneMM"
                                ErrorMessage="Servizio Utile Cessazione: formato Mese non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloVL" />
                            <%--<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator7" ControlToValidate="txtServizioUtileCessazioneMM"
                                Display="Dynamic" Enabled="true" ErrorMessage="Servizio Utile Cessazione MM: campo obbligatorio" ValidationGroup="UCTabDatiCalcoloVL"
                                Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>--%>
                            <label>
                                MM</label>
                            <asp:TextBox ID="txtServizioUtileCessazioneGG" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="31" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator27" ControlToValidate="txtServizioUtileCessazioneGG"
                                ErrorMessage="Servizio Utile Cessazione: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloVL" />
                            <%--<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator8" ControlToValidate="txtServizioUtileCessazioneGG"
                                Display="Dynamic" Enabled="true" ErrorMessage="Servizio Utile Cessazione GG: campo obbligatorio" ValidationGroup="UCTabDatiCalcoloVL"
                                Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>--%>
                            <label>
                                GG</label>
                        </td>
                        <asp:Panel ID="pnlQuotaPensioneRetributivaAnnuaCessazione" runat="server" Visible="false">
                            <td class="Row1" colspan="2">
                            </td>
                            <td class="Row1" style="width: 24%">
                                <label>
                                    Quota pensione retributiva annua:</label>
                            </td>
                            <td class="Row1" style="width: 20%">
                                <asp:TextBox ID="txtQuotaPensioneRetributivaAnnuaCessazione" runat="server" CssClass="tb8 txtUppercase"
                                    Width="75%" TabIndex="17" MaxLength="11"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator35"
                                    Display="Dynamic" ControlToValidate="txtQuotaPensioneRetributivaAnnuaCessazione"
                                    Enabled="true" ErrorMessage="QuotaRetributivaAnnua: Inserire valori interi o decimali"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d+(\,\d{1,4})?" />
                            </td>
                        </asp:Panel>
                    </tr>
                </table>
            </asp:Panel>
            <!-- Fine Pannello Dati Calcolo Retributivi FS_PT-->
            <!-- Pannello Riduzione Retributiva-->
            <asp:Panel ID="pnlRiduzioneRetributiva" runat="server" Visible="false">
                <table class="tabellaFormattazione grid grid-size-20" width="100%">
                    <tr style="min-height: 50px; vertical-align: bottom">
                        <td class="Row1" style="width: 33%">
                            <label>
                                Riduzione Retributiva:</label>
                        </td>
                        <td class="Row1" style="width: 30%">
                            <asp:DropDownList ID="ddlRiduzioneRetributiva" CssClass="tb8 txtUppercase xxs width-50" Width="25%"
                                runat="server">
                                <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                                <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="txtRiduzioneRetributiva" runat="server" CssClass="tb8 txtUppercase width-50"
                                Width="61%" TabIndex="32" MaxLength="5"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator32"
                                Display="Dynamic" ControlToValidate="txtRiduzioneRetributiva" Enabled="true"
                                ErrorMessage="Riduzione Retributiva: Inserire valori interi o decimali" Text="*" CssClass="field-is-required"
                                ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d{1,2}(\,\d{1,2})?" />
                            <asp:CustomValidator runat="server" ControlToValidate="ddlRiduzioneRetributiva" Display="Dynamic"
                                ErrorMessage="Riduzione Retributiva: La percentuale è obbligatoria avendo selezionato 'SI'"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ID="customRiduzione" ClientValidationFunction="checkPercentualeRiduzione" />
                        </td>
                        <td class="Row1" style="width: 3%">
                            <label>
                                %</label>
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <!-- Fine Pannello Riduzione Retributiva-->
        </asp:Panel>
        <asp:Panel ID="pnlDatiContributivi" runat="server" Visible="false">
            <!-- Pannello Dati Calcolo Contributivi L.335 VL -->
            <asp:Panel ID="pnlDatiContributiviVL" runat="server" Visible="false">
                <table class="tabellaFormattazione grid grid-size-20" width="100%">
                    <tr>
                        <td class="Row1" style="text-align: left">
                            <asp:Label ID="lblTitoloContributiviL335" runat="server" Text="Dati Contributivi da Legge 335"
                                Style="font-weight: bold"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Importo contributivo totale:</label>
                        </td>
                        <td class="Row1">
                            <asp:TextBox ID="txtImportTotale335_VL" runat="server" CssClass="tb8 txtUppercase"
                                Width="74%" TabIndex="33" MaxLength="11"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator31"
                                Display="Dynamic" ControlToValidate="txtImportTotale335_VL" Enabled="true" ErrorMessage="Importo Contributivo Totale: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                        </td>
                    </tr>
                    <asp:Panel runat="server" ID="pnlDatiContributiviVLFelpe" Visible="false">
                        <tr>
                            <td class="Row1" style="width: 25%">
                                <asp:Label ID="lblMontante_VL" runat="server" Text="Montante:"></asp:Label>
                            </td>
                            <td class="Row1" style="width: 25%" colspan="2">
                                <asp:TextBox ID="txtMontante_VL" runat="server" CssClass="tb8 txtUppercase" Width="130"
                                    TabIndex="34" MaxLength="13"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtMontante_VL" Display="Dynamic"
                                    ControlToValidate="txtMontante_VL" Enabled="true" ErrorMessage="Montante: Inserire valori interi o decimali (max 8 interi e 4 decimali)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d{0,8}(,\d{1,4})?" />
                            </td>
                            <td class="Row1" style="width: 20%">
                            </td>
                            <td class="Row1" style="width: 20px">
                            </td>
                            <td class="Row1" style="width: 30%">
                            </td>
                            <td class="Row1" style="width: 10px">
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlDatiContributiviVLNoFelpe" Visible="true">
                        <tr>
                            <td class="Row1" style="width: 25%">
                                <asp:Label ID="lblMontanteDa0196a0697_VL" runat="server" Text="Montante da 01/96 a 06/97:"></asp:Label>
                            </td>
                            <td class="Row1" style="width: 25%">
                                <asp:TextBox ID="txtMontanteDa0196a0697_VL" runat="server" CssClass="tb8 txtUppercase"
                                    Width="130" TabIndex="35" MaxLength="13"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtMontanteDa0196a0697_VL"
                                    Display="Dynamic" ControlToValidate="txtMontanteDa0196a0697_VL" Enabled="true"
                                    ErrorMessage="Montante da 01/96 a 06/97: Inserire valori interi o decimali (max 8 interi e 4 decimali)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d{0,8}(,\d{1,4})?" />
                            </td>
                            <td class="Row1" style="text-align: right; width: 20%">
                                <asp:Label ID="lblAnzianita96_VL" runat="server" Text="Anzianità:"></asp:Label>
                            </td>
                            <td style="width: 20px">
                            </td>
                            <td class="Row1" style="width: 30%">
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtA96_VL" ControlToValidate="txtA96_VL"
                                    Display="Dynamic" ErrorMessage="Anno non valido: inserire il numero di anni in un formato valido"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                                <asp:TextBox ID="txtA96_VL" runat="server" CssClass="tb8 txtUppercase" Width="30"
                                    TabIndex="36" MaxLength="2"></asp:TextBox>
                                <asp:Label ID="lblA96_VL" runat="server" Text="a"></asp:Label>
                                <span style="visibility: hidden">&nbsp;</span>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtM96_VL" ControlToValidate="txtM96_VL"
                                    Display="Dynamic" ErrorMessage="Mese non valido: inserire il numero di mesi in un formato valido"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                                <asp:CustomValidator runat="server" ControlToValidate="txtM96_VL" Display="Dynamic"
                                    ErrorMessage="Mese non valido: inserire un numero minore o uguale a 12" Text="*" CssClass="field-is-required"
                                    ValidationGroup="UCTabDatiCalcoloVL" ID="txtM96_VL_CV" ClientValidationFunction="validateMese" />
                                <asp:TextBox ID="txtM96_VL" runat="server" CssClass="tb8 txtUppercase" Width="30"
                                    TabIndex="37" MaxLength="2"></asp:TextBox>
                                <asp:Label ID="lblM96_VL" runat="server" Text="m"></asp:Label>
                                <span style="visibility: hidden">&nbsp;</span>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtG96_VL" ControlToValidate="txtG96_VL"
                                    Display="Dynamic" ErrorMessage="Giorno non valido: inserire il numero di giorni in un formato valido"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                                <asp:CustomValidator runat="server" ControlToValidate="txtG96_VL" Display="Dynamic"
                                    ErrorMessage="Giorno non valido: inserire un giorno minore o uguale a 365" Text="*" CssClass="field-is-required"
                                    ValidationGroup="UCTabDatiCalcoloVL" ID="txtG96_VL_CV" ClientValidationFunction="validateGiorno" />
                                <asp:TextBox ID="txtG96_VL" runat="server" CssClass="tb8 txtUppercase" Width="30"
                                    TabIndex="38" MaxLength="3"></asp:TextBox>
                                <asp:Label ID="lblG96_VL" runat="server" Text="g"></asp:Label>
                            </td>
                            <td style="width: 10px">
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1" style="width: 25%">
                                <asp:Label ID="lblMontanteDal0797_VL" runat="server" Text="Montante dal 07/97:"></asp:Label>
                            </td>
                            <td class="Row1" style="width: 25%">
                                <asp:TextBox ID="txtMontanteDa0697_VL" runat="server" CssClass="tb8 txtUppercase"
                                    Width="130" TabIndex="39" MaxLength="13"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtMontanteDa0697_VL"
                                    Display="Dynamic" ControlToValidate="txtMontanteDa0697_VL" Enabled="true" ErrorMessage="Montante dal 07/97 in poi: Inserire valori interi o decimali (max 8 interi e 4 decimali)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d{0,8}(,\d{1,4})?" />
                            </td>
                            <td class="Row1" style="text-align: right; width: 20%">
                                <asp:Label ID="lblAnzianita97_VL" runat="server" Text="Anzianità:"></asp:Label>
                            </td>
                            <td style="width: 20px">
                            </td>
                            <td class="Row1" style="width: 30%">
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtA97_VL" ControlToValidate="txtA97_VL"
                                    Display="Dynamic" ErrorMessage="Anno non valido: inserire il numero di anni in un formato valido"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                                <asp:TextBox ID="txtA97_VL" runat="server" CssClass="tb8 txtUppercase" Width="30"
                                    TabIndex="40" MaxLength="2"></asp:TextBox>
                                <asp:Label ID="lblA97_VL" runat="server" Text="a"></asp:Label>
                                <span style="visibility: hidden">&nbsp;</span>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtM97_VL" ControlToValidate="txtM97_VL"
                                    Display="Dynamic" ErrorMessage="Mese non valido: inserire il numero di mesi in un formato valido"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                                <asp:CustomValidator runat="server" ControlToValidate="txtM97_VL" Display="Dynamic"
                                    ErrorMessage="Mese non valido: inserire un numero minore o uguale a 12" Text="*" CssClass="field-is-required"
                                    ValidationGroup="UCTabDatiCalcoloVL" ID="txtM97_VL_CV" ClientValidationFunction="validateMese" />
                                <asp:TextBox ID="txtM97_VL" runat="server" CssClass="tb8 txtUppercase" Width="30"
                                    TabIndex="41" MaxLength="2"></asp:TextBox>
                                <asp:Label ID="lblM97_VL" runat="server" Text="m"></asp:Label>
                                <span style="visibility: hidden">&nbsp;</span>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtG97_VL" ControlToValidate="txtG97_VL"
                                    Display="Dynamic" ErrorMessage="Giorno non valido: inserire il numero di giorni in un formato valido"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                                <asp:CustomValidator runat="server" ControlToValidate="txtG97_VL" Display="Dynamic"
                                    ErrorMessage="Giorno non valido: inserire un giorno minore o uguale a 365" Text="*" CssClass="field-is-required"
                                    ValidationGroup="UCTabDatiCalcoloVL" ID="txtG97_VL_CV" ClientValidationFunction="validateGiorno" />
                                <asp:TextBox ID="txtG97_VL" runat="server" CssClass="tb8 txtUppercase" Width="30"
                                    TabIndex="42" MaxLength="3"></asp:TextBox>
                                <asp:Label ID="lblG97_VL" runat="server" Text="g"></asp:Label>
                            </td>
                            <td style="width: 10px">
                            </td>
                        </tr>
                    </asp:Panel>
                </table>
            </asp:Panel>
            <!-- Fine Pannello Dati Calcolo Contributivi VL -->
            <!-- Pannello Dati Calcolo Contributivi FS_PT -->
            <asp:Panel ID="pnlDatiContributiviFS_PT" runat="server" Visible="false">
                <table class="tabellaFormattazione grid grid-size-20" width="100%">
                    <tr style="min-height: 50px; vertical-align: bottom">
                        <td class="Row1" style="text-align: left">
                            <asp:Label ID="lblContributiviFS_PT" runat="server" Text="Dati Contributivi da Legge 335:"
                                Style="font-weight: bold; font-size: 15px;"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Importo Contributivo Totale:</label>
                        </td>
                        <td class="Row1">
                            <asp:TextBox ID="txtImportoContributivoTotaleFS_PT" runat="server" CssClass="tb8 txtUppercase"
                                Width="53%" TabIndex="43" MaxLength="12"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3" Display="Dynamic"
                                ControlToValidate="txtImportoContributivoTotaleFS_PT" Enabled="true" ErrorMessage="Importo Contributivo Totale: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator9" ControlToValidate="txtImportoContributivoTotaleFS_PT"
                                Display="Dynamic" Enabled="true" ErrorMessage="Importo Contributivo Totale: campo obbligatorio"
                                ValidationGroup="UCTabDatiCalcoloVL" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        </td>
                        <td class="Row1" style="width: 20%">
                            <label>
                                Settimane:</label>
                        </td>
                        <td class="Row1" style="width: 20%">
                            <asp:TextBox runat="server" ID="txtSettimaneFS_PT" CssClass="tb8 txtUppercase" Width="75%"
                                MaxLength="11" TabIndex="48"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator13"
                                ControlToValidate="txtSettimaneFS_PT" Display="Dynamic" ErrorMessage="Numero Settimane L. 335 non valido: inserire il numero di settimane in un formato valido"
                                Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="txtSettimaneFS_PT"
                                Display="Dynamic" Enabled="true" ErrorMessage="Numero Settimane L. 335: campo obbligatorio"
                                ValidationGroup="UCTabDatiCalcoloVL" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Montante:</label>
                        </td>
                        <td class="Row1">
                            <asp:TextBox ID="txtMontanteFS_PT" runat="server" CssClass="tb8 txtUppercase" Width="53%"
                                MaxLength="12"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator4" Display="Dynamic"
                                ControlToValidate="txtMontanteFS_PT" Enabled="true" ErrorMessage="Montante: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Importo Quota C:</label>
                        </td>
                        <td class="Row1">
                            <asp:TextBox ID="txtImportoQuotaCFS_PT" runat="server" CssClass="tb8 txtUppercase"
                                Width="53%" TabIndex="45" MaxLength="12"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator5" Display="Dynamic"
                                ControlToValidate="txtImportoQuotaCFS_PT" Enabled="true" ErrorMessage="Importo Quota C: Inserire valori interi o decimali"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d+(\,\d{1,4})?" />
                            <%--<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator11" ControlToValidate="txtImportoQuotaCFS_PT"
                                Display="Dynamic" Enabled="true" ErrorMessage="Importo Quota C: campo obbligatorio" ValidationGroup="UCTabDatiCalcoloVL"
                                Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>--%>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <!-- Fine Pannello Dati Calcolo Contributivi FS_PT -->
            <br />
            <!-- Pannello Dati Calcolo Contributivi L.214 -->
            <asp:Panel ID="pnlDatiCalcoloContributiviLegge214_VL_FS_PT" runat="server" Visible="false">
                <table class="tabellaFormattazione grid grid-size-20" width="100%">
                    <tr>
                        <td class="Row1 shift-full-grid" style="text-align: left">
                            <asp:Label ID="lblDatiContributiviL214" runat="server" Text="Dati Contributivi da L. 214"
                                Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione grid grid-size-20" width="100%">
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Importo contributivo totale:</label>
                        </td>
                        <td class="Row1">
                            <asp:TextBox runat="server" ID="txtImportoContribTotaleQuotaDL214" CssClass="tb8 txtUppercase"
                                Width="53%" MaxLength="12" TabIndex="46"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator28"
                                Display="Dynamic" ControlToValidate="txtImportoContribTotaleQuotaDL214" Enabled="true"
                                ErrorMessage="Importo Contributivo Totale L. 214: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                            <asp:RequiredFieldValidator runat="server" ID="txtImportoContribTotaleQuotaDL214RF"
                                ControlToValidate="txtImportoContribTotaleQuotaDL214" Display="Dynamic" Enabled="true"
                                ErrorMessage="Importo Contributivo Totale L. 214: campo obbligatorio" ValidationGroup="UCTabDatiCalcoloVL"
                                Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        </td>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Settimane:</label>
                        </td>
                        <td class="Row1" style="width: 20%">
                            <asp:TextBox runat="server" ID="txtNSettimaneQuotaDL214" CssClass="tb8 txtUppercase"
                                Width="75%" MaxLength="11" TabIndex="48"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator30"
                                ControlToValidate="txtNSettimaneQuotaDL214" Display="Dynamic" ErrorMessage="Numero Settimane L. 214 non valido: inserire il numero di settimane in un formato valido"
                                Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtNSettimaneQuotaDL214"
                                Display="Dynamic" Enabled="true" ErrorMessage="Numero Settimane L. 214: campo obbligatorio"
                                ValidationGroup="UCTabDatiCalcoloVL" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Montante:</label>
                        </td>
                        <td class="Row1" style="width: 35%">
                            <asp:TextBox runat="server" ID="txtMontanteQuotaDL214" CssClass="tb8 txtUppercase"
                                Width="53%" MaxLength="12" TabIndex="47"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator29"
                                Display="Dynamic" ControlToValidate="txtMontanteQuotaDL214" Enabled="true" ErrorMessage="Montante L. 214: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtMontanteQuotaDL214"
                                Display="Dynamic" Enabled="true" ErrorMessage="Montante L. 214: campo obbligatorio"
                                ValidationGroup="UCTabDatiCalcoloVL" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        </td>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Quota pensione contributiva annua:</label>
                        </td>
                        <td class="Row1" style="width: 20%">
                            <asp:TextBox ID="txtQuotaPensioneContributivaAnnuaDL214" runat="server" CssClass="tb8 txtUppercase"
                                Width="75%" MaxLength="11"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator22"
                                Display="Dynamic" ControlToValidate="txtQuotaPensioneContributivaAnnuaDL214"
                                Enabled="true" ErrorMessage="QuotaPensioneRetributivaAnnuaDL214: Inserire valori interi o decimali"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </asp:Panel>
        <!-- Inizio Pannello Common -->
        <asp:Panel ID="pnlDatiCommon" runat="server">
            <br />
            <!-- Inizio Pannello Common VL -->
            <asp:Panel ID="pnlDatiCommonVL" runat="server" Visible="false">
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1" style="text-align: right; width: 20%">
                            <asp:Label ID="lblLavoratorePrecoceDati" runat="server" Text="Lavoratore Precoce:"></asp:Label>
                        </td>
                        <td class="Row1" style="width: 30%">
                            <asp:CheckBox ID="chkLavoratorePrecoce" runat="server" TabIndex="49" CssClass="tb8" />
                        </td>
                        <td style="width: 10px">
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <!-- Fine Pannello Common VL -->
        </asp:Panel>
        <!-- Fine Pannello Common -->
    </div>
</asp:Panel>
<!-- Pannello dati comma 707 -->
<asp:Panel runat="server" ID="pnlComma707" Visible="false">
    <div id="divComma707" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server" class="reset-style">
        <table class="tabellaFormattazione grid grid-size-20" width="100%">
            <tr>
                <td class="section-label mt-32 shift-full-grid" style="text-align: left; font-weight: bold">
                    Calcolo ex comma 707
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione grid grid-size-20" width="100%">
            <tr>
                <td class="Row1" style="width: 20%">
                    Quota A1:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaA1Comma707" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaA1Comma707" ControlToValidate="txtQuotaA1Comma707"
                        Display="Dynamic" ErrorMessage="Quota A1 del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                </td>
                <td class="Row1" style="width: 20%">
                    Quota A2:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaA2Comma707" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaA2Comma707" ControlToValidate="txtQuotaA2Comma707"
                        Display="Dynamic" ErrorMessage="Quota A2 del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 20%">
                    Quota B:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaBComma707" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaBComma707" ControlToValidate="txtQuotaBComma707"
                        Display="Dynamic" ErrorMessage="Quota B del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                </td>
                <td class="Row1" style="width: 20%">
                    Quota C1:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaC1Comma707" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaC1Comma707" ControlToValidate="txtQuotaC1Comma707"
                        Display="Dynamic" ErrorMessage="Quota C1 del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 20%">
                    Quota C2:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaC2Comma707" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaC2Comma707" ControlToValidate="txtQuotaC2Comma707"
                        Display="Dynamic" ErrorMessage="Quota C2 del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                </td>
                <td class="Row1" style="width: 20%">
                    Quota D:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaDComma707" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaDComma707" ControlToValidate="txtQuotaDComma707"
                        Display="Dynamic" ErrorMessage="Quota D del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVL" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<!-- Fine Pannello dati comma 707 -->
<!-- Pannell dati Calcolo Ante Armonizzazione VL -->
<asp:Panel runat="server" ID="pnlAnteArmonizzazioneVL" Visible="false">
    <div id="divAnteArmonizzazioneVL" style="border-style: solid; border-color: #000080;
        border-collapse: collapse; border-width: 1px; width: 710px; margin-left: 4px;
        margin-top: 4px;" runat="server">
        <table class="tabellaFormattazione grid grid-size-20" width="100%">
            <tr>
                <td class="Row1 shift-full-grid" style="text-align: left; font-weight: bold" class="section-label mt-32">
                    Dati Ante 01/01/93 (quota A)
                </td>
            </tr>
            <tr>
                <td class="shift-full-grid">
                    <table class="tabellaFormattazione grid grid-size-20" width="100%">
                        <tr>
                            <td class="Row1" style="width: 17%">
                                <label>
                                    Retrib. Pens. Annua:</label>
                            </td>
                            <td class="field" style="width: 33%">
                                <asp:TextBox ID="txtRetrPensAnnuaQuotaA" runat="server" CssClass="tb8 txtUppercase"
                                    Width="70%" MaxLength="10"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtRetrPensAnnuaQuotaA" Display="Dynamic"
                                    ControlToValidate="txtRetrPensAnnuaQuotaA" Enabled="true" ErrorMessage="Retrib. Pens. Annua quota A: Inserire valori interi o decimali (max 5 interi e 4 decimali)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^\d{0,5}(,\d{1,4})?$" />
                                <asp:RequiredFieldValidator runat="server" ID="RFVtxtRetrPensAnnuaQuotaA" ControlToValidate="txtRetrPensAnnuaQuotaA"
                                    Display="Dynamic" Enabled="true" ErrorMessage="Retrib. Pens. Annua quota A: campo obbligatorio"
                                    ValidationGroup="UCTabDatiCalcoloVL" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                            </td>
                            <td class="Row1" style="width: 17%">
                                <label>
                                    Controcodice retributivo:</label>
                            </td>
                            <td class="field" style="width: 33%">
                                <asp:TextBox ID="txtControcodiceRetributivoQuotaA" runat="server" CssClass="tb8 txtUppercase"
                                    Width="40%" MaxLength="3"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtControcodiceRetributivoQuotaA"
                                    Display="Dynamic" ControlToValidate="txtControcodiceRetributivoQuotaA" Enabled="true"
                                    ErrorMessage="Controcodice retributivo quota A: Inserire valori interi (max 3 interi)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^[0-9]+$" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table class="tabellaFormattazione grid grid-size-20" width="100%">
                        <tr>
                            <td class="Row1" style="width: 17%">
                                <label>
                                    Servizio Utile ante 27/11/88:</label>
                            </td>
                            <td class="field" style="width: 33%">
                                <asp:TextBox ID="txtServizioUtileAnte271188AA" runat="server" CssClass="tb8 txtUppercase"
                                    Width="13%" MaxLength="2"></asp:TextBox>
                                AA
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtServizioUtileAnte271188AA"
                                    Display="Dynamic" ControlToValidate="txtServizioUtileAnte271188AA" Enabled="true"
                                    ErrorMessage="Servizio Utile ante 27/11/88 AA: Inserire valori interi (max 2 interi)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^[0-9]+$" />
                                <asp:RequiredFieldValidator runat="server" ID="RFV" ControlToValidate="txtServizioUtileAnte271188AA"
                                    Display="Dynamic" Enabled="true" ErrorMessage="Servizio Utile ante 27/11/88 AA: campo obbligatorio"
                                    ValidationGroup="UCTabDatiCalcoloVL" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                                <asp:TextBox ID="txtServizioUtileAnte271188MM" runat="server" CssClass="tb8 txtUppercase"
                                    Width="13%" MaxLength="2"></asp:TextBox>
                                MM
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtServizioUtileAnte271188MM"
                                    Display="Dynamic" ControlToValidate="txtServizioUtileAnte271188MM" Enabled="true"
                                    ErrorMessage="Servizio Utile ante 27/11/88 MM: Inserire valori interi (max 2 interi)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^[0-9]+$" />
                                <asp:RequiredFieldValidator runat="server" ID="RFVtxtServizioUtileAnte271188MM" ControlToValidate="txtServizioUtileAnte271188MM"
                                    Display="Dynamic" Enabled="true" ErrorMessage="Servizio Utile ante 27/11/88 MM: campo obbligatorio"
                                    ValidationGroup="UCTabDatiCalcoloVL" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                                <asp:TextBox ID="txtServizioUtileAnte271188GG" runat="server" CssClass="tb8 txtUppercase"
                                    Width="13%" MaxLength="2"></asp:TextBox>
                                GG
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtServizioUtileAnte271188GG"
                                    Display="Dynamic" ControlToValidate="txtServizioUtileAnte271188GG" Enabled="true"
                                    ErrorMessage="Servizio Utile ante 27/11/88 GG: Inserire valori interi (max 2 interi)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^[0-9]+$" />
                                <asp:RequiredFieldValidator runat="server" ID="RFVtxtServizioUtileAnte271188GG" ControlToValidate="txtServizioUtileAnte271188GG"
                                    Display="Dynamic" Enabled="true" ErrorMessage="Servizio Utile ante 27/11/88 GG: campo obbligatorio"
                                    ValidationGroup="UCTabDatiCalcoloVL" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                            </td>
                            <td class="Row1" style="width: 17%">
                                <label>
                                    Servizio Utile ante '93:</label>
                            </td>
                            <td class="field" style="width: 33%">
                                <asp:TextBox ID="txtServizioUtileAnte93AA" runat="server" CssClass="tb8 txtUppercase"
                                    Width="13%" MaxLength="2"></asp:TextBox>
                                AA
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtServizioUtileAnte93AA" Display="Dynamic"
                                    ControlToValidate="txtServizioUtileAnte93AA" Enabled="true" ErrorMessage="Servizio Utile ante '93 AA: Inserire valori interi (max 2 interi)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^[0-9]+$" />
                                <asp:RequiredFieldValidator runat="server" ID="RFVtxtServizioUtileAnte93AA" ControlToValidate="txtServizioUtileAnte93AA"
                                    Display="Dynamic" Enabled="true" ErrorMessage="Servizio Utile ante '93 AA: campo obbligatorio"
                                    ValidationGroup="UCTabDatiCalcoloVL" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                                <asp:TextBox ID="txtServizioUtileAnte93MM" runat="server" CssClass="tb8 txtUppercase"
                                    Width="13%" MaxLength="2"></asp:TextBox>
                                MM
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtServizioUtileAnte93MM" Display="Dynamic"
                                    ControlToValidate="txtServizioUtileAnte93MM" Enabled="true" ErrorMessage="Servizio Utile ante '93 MM: Inserire valori interi (max 2 interi)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^[0-9]+$" />
                                <asp:RequiredFieldValidator runat="server" ID="RFVtxtServizioUtileAnte93MM" ControlToValidate="txtServizioUtileAnte93MM"
                                    Display="Dynamic" Enabled="true" ErrorMessage="Servizio Utile ante '93 MM: campo obbligatorio"
                                    ValidationGroup="UCTabDatiCalcoloVL" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                                <asp:TextBox ID="txtServizioUtileAnte93GG" runat="server" CssClass="tb8 txtUppercase"
                                    Width="13%" MaxLength="2"></asp:TextBox>
                                GG
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtServizioUtileAnte93GG" Display="Dynamic"
                                    ControlToValidate="txtServizioUtileAnte93GG" Enabled="true" ErrorMessage="Servizio Utile ante '93 GG: Inserire valori interi (max 2 interi)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^[0-9]+$" />
                                <asp:RequiredFieldValidator runat="server" ID="RFVtxtServizioUtileAnte93GG" ControlToValidate="txtServizioUtileAnte93GG"
                                    Display="Dynamic" Enabled="true" ErrorMessage="Servizio Utile ante '93 GG: campo obbligatorio"
                                    ValidationGroup="UCTabDatiCalcoloVL" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="section-label mt-32" style="text-align: left; font-weight: bold; padding-top: 20px">
                    Dati Post 31/12/92 (quota B)
                </td>
            </tr>
            <tr>
                <td>
                    <table class="tabellaFormattazione grid grid-size-20" width="100%">
                        <tr>
                            <td class="Row1" style="width: 17%">
                                <label>
                                    Retrib. Pens. Annua:</label>
                            </td>
                            <td class="field" style="width: 33%">
                                <asp:TextBox ID="txtRetrPensAnnuaQuotaB" runat="server" CssClass="tb8 txtUppercase"
                                    Width="70%" MaxLength="10"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtRetrPensAnnuaQuotaB" Display="Dynamic"
                                    ControlToValidate="txtRetrPensAnnuaQuotaB" Enabled="true" ErrorMessage="Retrib. Pens. Annua quota B: Inserire valori interi o decimali (max 5 interi e 4 decimali)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^\d{0,5}(,\d{1,4})?$" />
                            </td>
                            <td class="Row1" style="width: 17%">
                                <label>
                                    Controcodice retributivo:</label>
                            </td>
                            <td class="field" style="width: 33%">
                                <asp:TextBox ID="txtControcodiceRetributivoQuotaB" runat="server" CssClass="tb8 txtUppercase"
                                    Width="40%" MaxLength="3"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtControcodiceRetributivoQuotaB"
                                    Display="Dynamic" ControlToValidate="txtControcodiceRetributivoQuotaB" Enabled="true"
                                    ErrorMessage="Controcodice retributivo quota B: Inserire valori interi (max 3 interi)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^[0-9]+$" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table class="tabellaFormattazione grid grid-size-20" width="100%">
                        <tr>
                            <td class="Row1" style="width: 17%">
                                <label>
                                    Servizio Utile post 31/12/92:</label>
                            </td>
                            <td class="field" style="width: 33%">
                                <asp:TextBox ID="txtServizioUtilePost311292AA" runat="server" CssClass="tb8 txtUppercase"
                                    Width="13%" MaxLength="2"></asp:TextBox>
                                AA
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtServizioUtilePost311292AA"
                                    Display="Dynamic" ControlToValidate="txtServizioUtilePost311292AA" Enabled="true"
                                    ErrorMessage="Servizio Utile post 31/12/92 AA: Inserire valori interi (max 2 interi)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^[0-9]+$" />
                                <asp:TextBox ID="txtServizioUtilePost311292MM" runat="server" CssClass="tb8 txtUppercase"
                                    Width="13%" MaxLength="2"></asp:TextBox>
                                MM
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtServizioUtilePost311292MM"
                                    Display="Dynamic" ControlToValidate="txtServizioUtilePost311292MM" Enabled="true"
                                    ErrorMessage="Servizio Utile post 31/12/92 MM: Inserire valori interi (max 2 interi)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^[0-9]+$" />
                                <asp:TextBox ID="txtServizioUtilePost311292GG" runat="server" CssClass="tb8 txtUppercase"
                                    Width="13%" MaxLength="2"></asp:TextBox>
                                GG
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtServizioUtilePost311292GG"
                                    Display="Dynamic" ControlToValidate="txtServizioUtilePost311292GG" Enabled="true"
                                    ErrorMessage="Servizio Utile post 31/12/92 GG: Inserire valori interi (max 2 interi)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^[0-9]+$" />
                            </td>
                            <td class="Row1" style="width: 50%">
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="section-label mt-32" style="text-align: left; font-weight: bold; padding-top: 20px">
                    Dati Post 31/12/94 (quota C)
                </td>
            </tr>
            <tr>
                <td>
                    <table class="tabellaFormattazione grid grid-size-20" width="100%">
                        <tr>
                            <td class="Row1" style="width: 17%">
                                <label>
                                    Servizio Utile post '94:</label>
                            </td>
                            <td class="field" style="width: 33%">
                                <asp:TextBox ID="txtServizioUtilePost94AA" runat="server" CssClass="tb8 txtUppercase"
                                    Width="13%" MaxLength="2"></asp:TextBox>
                                AA
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtServizioUtilePost94AA" Display="Dynamic"
                                    ControlToValidate="txtServizioUtilePost94AA" Enabled="true" ErrorMessage="Servizio Utile post '94 AA: Inserire valori interi (max 2 interi)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^[0-9]+$" />
                                <asp:TextBox ID="txtServizioUtilePost94MM" runat="server" CssClass="tb8 txtUppercase"
                                    Width="13%" MaxLength="2"></asp:TextBox>
                                MM
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtServizioUtilePost94MM" Display="Dynamic"
                                    ControlToValidate="txtServizioUtilePost94MM" Enabled="true" ErrorMessage="Servizio Utile post '94 MM: Inserire valori interi (max 2 interi)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^[0-9]+$" />
                                <asp:TextBox ID="txtServizioUtilePost94GG" runat="server" CssClass="tb8 txtUppercase"
                                    Width="13%" MaxLength="2"></asp:TextBox>
                                GG
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtServizioUtilePost94GG" Display="Dynamic"
                                    ControlToValidate="txtServizioUtilePost94GG" Enabled="true" ErrorMessage="Servizio Utile post '94 GG: Inserire valori interi (max 2 interi)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVL" ValidationExpression="^[0-9]+$" />
                            </td>
                            <td class="Row1" style="width: 50%">
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<!-- Fine Pannell dati Calcolo Ante Armonizzazione VL -->
<div style="margin-right: 40px;" class="containerWidth xs">
    <table width="100%" style="min-height: 100px;" class="tab-actions-group">
        <tr>
            <td style="text-align: right; vertical-align: bottom;" class="tab-actions-group__first">
                <asp:Button ID="btnPopUpContributivi" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Style="display: none" Text="Salva Dati Calcolo" Width="150px" OnClientClick="if(Page_ClientValidate('UCTabDatiCalcoloVL')){return ConfirmContributivi();}" CssClass="primary" />
                <asp:Button ID="btnPopUp" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Visible="false" Text="Salva Dati Calcolo" Width="150px" OnClientClick="if(Page_ClientValidate('UCTabDatiCalcoloVL')){return Confirm();}" CssClass="primary" />
                <asp:Button ID="btnSalvaDatiCalcolo" runat="server" CausesValidation="false" Style="display: none"
                    ValidationGroup="UCTabDatiCalcoloVL" SkinID="btnAzione1" Width="150px" OnClick="btnSalvaDatiCalcolo_Click"
                    Text="Salva Dati Calcolo" Visible="false" OnClientClick="if(Page_ClientValidate('UCTabDatiCalcoloVL')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
                <asp:Button ID="btnSalvaDatiCalcoloNoRiduzione" runat="server" CausesValidation="false"
                    ValidationGroup="UCTabDatiCalcoloVL" SkinID="btnAzione1" Width="150px" OnClick="btnSalvaDatiCalcolo_Click"
                    Text="Salva Dati Calcolo" Visible="true" OnClientClick="if(Page_ClientValidate('UCTabDatiCalcoloVL')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
            </td>
            <td style="text-align: left; vertical-align: bottom;">
                <asp:Button ID="btnEliminaDatiCalcolo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Dati Calcolo" Width="150px" OnClick="btnEliminaDatiCalcolo_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Calcolo?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
<asp:HiddenField ID="FlagUnicarpe" runat="server" />
<asp:HiddenField ID="HdnFondo" runat="server" />
<div id="dialog-confirm" title="Confirm" style="border-style: none; border-color: White;">
    <p>
        <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
        Età titolare inferiore a 57 anni. Confermi la mancanza della percentuale di Riduzione?</p>
</div>
<div id="dialog-Contributivi" title="Confirm" style="border-style: none; border-color: White;">
    <p>
        <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
        Attenzione il Montante è inferiore all’Ammontare.<br />
        Confermare ?</p>
</div>
