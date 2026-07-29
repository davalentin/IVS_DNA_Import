<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiCalcoloEL_TT_ET.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiCalcoloEL_TT_ET" %>
<style type="text/css">
    .fixed-dialog
    {
        position: fixed;
    }
</style>
<script type="text/javascript">

    $(document).ready(function () {
        setRetribuzioneUltimoAnno();

    });

    function setRetribuzioneUltimoAnno() {
        if (document.getElementById("<%=FlagUnicarpe.ClientID%>") != null && document.getElementById("<%=FlagUnicarpe.ClientID%>").value == 'NO') {
            if (document.getElementById("<%=txtRMSA.ClientID%>") != null) {
                var RMSA = document.getElementById("<%=txtRMSA.ClientID%>").value;
                RMSA = RMSA.replace(',', '.');
                var num = parseFloat(RMSA);
                if (document.getElementById("<%=txtRetribUltimoAnnoRetrib.ClientID%>") != null) {
                    if (num.toString() == 'NaN')
                        document.getElementById("<%=htxtRetribUltimoAnnoRetrib.ClientID%>").value = document.getElementById("<%=txtRetribUltimoAnnoRetrib.ClientID%>").value = '';
                    else {
                        num = (num * 52).toFixed(4);
                        num = num.toString().replace('.', ',');
                        document.getElementById("<%=htxtRetribUltimoAnnoRetrib.ClientID%>").value = document.getElementById("<%=txtRetribUltimoAnnoRetrib.ClientID%>").value = num;
                    }
                }
            }
        }
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

    function ConfirmContributivi() {
        if (CheckAmmontareMaggioreDiMontante()) {
            $('#dialog-Contributivi').dialog('open');
        }
        else {
            if (document.getElementById('<%= btnSalvaDatiCalcoloNoRiduzione.ClientID %>'))
                document.getElementById('<%= btnSalvaDatiCalcoloNoRiduzione.ClientID %>').click();
            else if (document.getElementById('<%= btnSalvaDatiCalcolo.ClientID %>'))
                document.getElementById('<%= btnSalvaDatiCalcolo.ClientID %>').click();
        }

        return false;
    }

    function CheckAmmontareMaggioreDiMontante() {
        var montante = document.getElementById('<%= txtMontante.ClientID %>');
        var ammontare = document.getElementById('<%= txtImportoContributivoTotale.ClientID %>');

        if (montante && ammontare && parseFloat(ammontare.value) > parseFloat(montante.value))
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
                    if (document.getElementById('<%= btnSalvaDatiCalcoloNoRiduzione.ClientID %>'))
                        document.getElementById('<%= btnSalvaDatiCalcoloNoRiduzione.ClientID %>').click();
                    else if (document.getElementById('<%= btnSalvaDatiCalcolo.ClientID %>'))
                        document.getElementById('<%= btnSalvaDatiCalcolo.ClientID %>').click();
                    return true;
                }
            }
        });
    });

    function sommaSettimane() {
        var settimaneUtili = document.getElementById("<%=txtSettimaneUtiliDiritto.ClientID %>");
        var settimaneUtiliOI = document.getElementById("<%=txtSettimaneUtiliDirittoOI.ClientID %>");
        var totaleSettimane = document.getElementById("<%=txtSettimaneUtiliDirittoTot.ClientID %>");

        var valore1 = parseInt(settimaneUtili.value) || 0;
        var valore2 = parseInt(settimaneUtiliOI.value) || 0;

        totaleSettimane.value = valore1 + valore2;
    }
</script>
<asp:Panel ID="pnlSettimane_EL_TT_ET" runat="server" Visible="false">
    <div style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server">
        <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
            <tr>
                <td class="Row1" style="width: 33%">
                    <label runat="server" id="lblNumeroSettimane">
                        Settimane Utili al Diritto:</label>
                </td>
                <td class="Row1" style="width: 17%">
                    <asp:TextBox runat="server" ID="txtSettimaneUtiliDiritto" CssClass="tb8 txtUppercase" Width="60%"
                        MaxLength="4" onblur="sommaSettimane();"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="revSettimaneUtiliDiritto"
                        ControlToValidate="txtSettimaneUtiliDiritto" Display="Dynamic" ErrorMessage="Settimane Utili al Diritto non valide: inserire il numero di settimane in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                </td>
                <td class="Row1" style="width: 30%">
                </td>
                <td class="Row1" style="width: 20%">
                </td>
            </tr>
        </table>
        <asp:Panel runat="server" ID="pnlNSettimane_OrganizzazioniInternazionali">
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="width: 33%">
                        <label>
                            Settimane Utili al Diritto OI:</label><label runat="server" id="lbltest" visible="false"/>
                    </td>
                    <td class="Row1" style="width: 17%">
                        <asp:TextBox runat="server" ID="txtSettimaneUtiliDirittoOI" CssClass="tb8 txtUppercase" Width="60%"
                            MaxLength="4" onblur="sommaSettimane();"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator18"
                            ControlToValidate="txtSettimaneUtiliDirittoOI" Display="Dynamic" ErrorMessage="Settimane Utili al Diritto OI non valide: inserire il numero di settimane in un formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1"
                            ControlToValidate="txtSettimaneUtiliDirittoOI" Display="Dynamic" Enabled="true"
                            ErrorMessage="Settimane Utili al Diritto OI: campo obbligatorio" ValidationGroup="UCTabDatiCalcolo"
                            Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>

                    </td>
                    <td class="Row1" style="width: 30%">
                    </td>
                    <td class="Row1" style="width: 20%">
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 33%">
                        <label>
                            Settimane Utili al Diritto TOT:</label>
                    </td>
                    <td class="Row1" style="width: 17%">
                        <asp:TextBox runat="server" ID="txtSettimaneUtiliDirittoTot" CssClass="tb8 txtUppercase" Width="60%"
                            MaxLength="4" ReadOnly="true"></asp:TextBox>
                    </td>
                    <td class="Row1" style="width: 30%">
                    </td>
                    <td class="Row1" style="width: 20%">
                    </td>
                </tr>
            </table>
         </asp:Panel>
    </div>
</asp:Panel>
<!-- Pannello Dati Calcolo Retributivi EL-TT-ET -->
<asp:Panel ID="pnlDatiCalcoloRetributivi_EL_TT_ET" runat="server" Visible="false">
    <div id="pdivRetributivo" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server">
        <!-- Pannello Dati Calcolo Retributivi Fondo ET -->
        <asp:Panel ID="pnlDatiCalcoloRetributiviET" runat="server" Visible="false">
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <label style="font-weight: bold" class="section-label mt-32">
                            Dati Ante 01/01/93 (Quota A)</label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
                <tr>
                    <td class="Row1" style="width: 15%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 inline-fields" style="width: 23%">
                        <asp:TextBox ID="txtServizioUtileAAQtaA" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" TabIndex="1" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator9" ControlToValidate="txtServizioUtileAAQtaA"
                            ErrorMessage="Servizio Utile Quota A: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:TextBox ID="txtServizioUtileMMQtaA" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" TabIndex="2" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator10" ControlToValidate="txtServizioUtileMMQtaA"
                            ErrorMessage="Servizio Utile Quota A: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:TextBox ID="txtServizioUtileGGQtaA" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" TabIndex="3" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator11" ControlToValidate="txtServizioUtileGGQtaA"
                            ErrorMessage="Servizio Utile Quota A: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                    </td>
                    <td class="Row1" style="width: 13%">
                        <label>
                            Retribuzione Pensionabile:</label>
                    </td>
                    <td class="Row1" style="width: 21%">
                        <asp:TextBox ID="txtRetribPensionabileQtaA" runat="server" CssClass="tb8 txtUppercase"
                            Width="90%" TabIndex="4" MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" Display="Dynamic"
                            ControlToValidate="txtRetribPensionabileQtaA" Enabled="true" ErrorMessage="Retribuzione Pensionabile Quota A: Inserire valori interi o decimali"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
                    </td>
                    <td id="Td1" class="Row1" style="width: 28%" runat="server">
                        <asp:Panel ID="pnlControCodiceRetribQtaA" runat="server" Visible="true">
                            <table>
                                <tr>
                                    <td class="Row1" style="width: 64%" runat="server" id="lblControCodQtaA">
                                        <label>
                                            ControCodice Retr.:</label>
                                    </td>
                                    <td class="Row1" style="width: 36%" runat="server" id="rowControCodQtaA">
                                        <asp:TextBox ID="txtControCodiceRetrQtaA" runat="server" CssClass="tb8 txtUppercase"
                                            Width="30px" TabIndex="5" MaxLength="3"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator12" ControlToValidate="txtControCodiceRetrQtaA"
                                            ErrorMessage="ControCodice Retr. Quota A: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <label style="font-weight: bold" class="section-label mt-32">
                            Dati Post 31/12/92 (Quota B)</label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
                <tr>
                    <td class="Row1" style="width: 15%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 inline-fields" style="width: 23%">
                        <asp:TextBox ID="txtServizioUtileAAQtaB" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" TabIndex="6" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator15" ControlToValidate="txtServizioUtileAAQtaB"
                            ErrorMessage="Servizio Utile Quota B: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:TextBox ID="txtServizioUtileMMQtaB" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" TabIndex="7" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ControlToValidate="txtServizioUtileMMQtaB"
                            ErrorMessage="Servizio Utile Quota B: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:TextBox ID="txtServizioUtileGGQtaB" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" TabIndex="8" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtServizioUtileGGQtaB"
                            ErrorMessage="Servizio Utile Quota B: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                    </td>
                    <td class="Row1" style="width: 13%">
                        <label>
                            Retribuzione Pensionabile:</label>
                    </td>
                    <td class="Row1" style="width: 21%">
                        <asp:TextBox ID="txtRetribPensionabileQtaB" runat="server" CssClass="tb8 txtUppercase"
                            Width="90%" TabIndex="9" MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" Display="Dynamic"
                            ControlToValidate="txtRetribPensionabileQtaB" Enabled="true" ErrorMessage="Retribuzione Pensionabile Quota B: Inserire valori interi o decimali"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
                    </td>
                    <td class="Row1" style="width: 28%" runat="server">
                        <asp:Panel ID="pnlControCodiceRetribQtaB" runat="server" Visible="true">
                            <table>
                                <tr>
                                    <td class="Row1" style="width: 64%" runat="server" id="lblControCodQtaB">
                                        <label>
                                            ControCodice Retr.:</label>
                                    </td>
                                    <td class="Row1" style="width: 36%" runat="server" id="rowControCodQtaB">
                                        <asp:TextBox ID="txtControCodiceRetrQtaB" runat="server" CssClass="tb8 txtUppercase"
                                            Width="30px" TabIndex="10" MaxLength="3"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" ControlToValidate="txtControCodiceRetrQtaB"
                                            ErrorMessage="ControCodice Retr. Quota B: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <label style="font-weight: bold" class="section-label mt-32">
                            Dati Post 31/12/94 (Quota C)</label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
                <tr>
                    <td class="Row1" style="width: 15%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 inline-fields">
                        <asp:TextBox ID="txtServizioUtileAAQtaC" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" TabIndex="11" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator6" ControlToValidate="txtServizioUtileAAQtaC"
                            ErrorMessage="Servizio Utile Quota C: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:TextBox ID="txtServizioUtileMMQtaC" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" TabIndex="12" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator7" ControlToValidate="txtServizioUtileMMQtaC"
                            ErrorMessage="Servizio Utile Quota C: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:TextBox ID="txtServizioUtileGGQtaC" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" TabIndex="13" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator8" ControlToValidate="txtServizioUtileGGQtaC"
                            ErrorMessage="Servizio Utile Quota C: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- Fine Pannello Dati Calcolo Retributivi Fondo ET-->
        <asp:Panel ID="pnlDecretoCross" runat="server">
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblTitoloDatiRetributivi" runat="server" Text="" Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
                <asp:Panel ID="pnlRigaA" runat="server" Visible="true">
                    <tr>
                        <td class="Row1" style="width: 33%">
                            <label>
                                Retribuzione Media Settimanale A:</label>
                        </td>
                        <td class="Row1" style="width: 30%">
                            <asp:TextBox runat="server" ID="txtRMSA" CssClass="tb8 txtUppercase" MaxLength="11"
                                Width="90%" TabIndex="14" OnBlur="setRetribuzioneUltimoAnno();"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="validateTxtRMSA" Display="Dynamic"
                                ControlToValidate="txtRMSA" Enabled="true" ErrorMessage="Retribuzione Media Settimanale A: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,6}(,\d{1,4})?" />
                        </td>
                        <td class="Row1" style="width: 3%">
                            <label>
                                €</label>
                        </td>
                        <td class="Row1" style="width: 13%">
                            <label class="etichettaBold">
                                Settimane A:</label>
                        </td>
                        <td class="Row1" style="width: 15%">
                            <asp:TextBox runat="server" ID="txtSettimaneA" CssClass="tb8 txtUppercase" Width="80%"
                                MaxLength="4" TabIndex="15"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneA" ControlToValidate="txtSettimaneA"
                                Display="Dynamic" ErrorMessage="Numero settimane A non valido: inserire il numero di settimane in un formato valido"
                                Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                        </td>
                        <td class="Row1 none" style="width: 5%">
                        </td>
                    </tr>
                </asp:Panel>
                <tr>
                    <td class="Row1" style="width: 33%">
                        <label>
                            Retribuzione Media Settimanale B:</label>
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox runat="server" ID="txtRMSB" CssClass="tb8 txtUppercase" Width="90%"
                            MaxLength="11" TabIndex="16"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="validateTxtRMSB" Display="Dynamic"
                            ControlToValidate="txtRMSB" Enabled="true" ErrorMessage="Retribuzione Media Settimanale B: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,6}(,\d{1,4})?" />
                    </td>
                    <td class="Row1" style="width: 3%">
                        <label>
                            €</label>
                    </td>
                    <td class="Row1" style="width: 14%">
                        <label class="etichettaBold">
                            Settimane B:</label>
                    </td>
                    <td class="Row1" style="width: 15%">
                        <asp:TextBox runat="server" ID="txtSettimaneB" CssClass="tb8 txtUppercase" Width="80%"
                            MaxLength="4" TabIndex="17"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneB" ControlToValidate="txtSettimaneB"
                            Display="Dynamic" ErrorMessage="Numero settimane B non valido: inserire il numero di settimane in un formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                    </td>
                    <td class="Row1" style="width: 5%">
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlDatiCalcoloRetributivi_EL_TT" runat="server" Visible="false">
                <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
                    <tr>
                        <td class="Row1" style="width: 33%">
                        </td>
                        <td class="Row1" style="width: 30%">
                        </td>
                        <td class="Row1" style="width: 3%">
                        </td>
                        <td class="Row1" style="width: 14%">
                            <label>
                                Settimane C:</label>
                        </td>
                        <td class="Row1" style="width: 15%">
                            <asp:TextBox runat="server" ID="txtSettimaneC" CssClass="tb8 txtUppercase" Width="80%"
                                MaxLength="4" TabIndex="18"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneC" ControlToValidate="txtSettimaneC"
                                Display="Dynamic" ErrorMessage="Numero settimane C non valido: inserire il numero di settimane in un formato valido"
                                Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                        </td>
                        <td class="Row1 none" style="width: 5%">
                        </td>
                    </tr>
                    <tr runat="server" id="rigaD">
                        <td class="Row1" style="width: 33%">
                            <label>
                                Retribuzione Media Settimanale D:</label>
                        </td>
                        <td class="Row1" style="width: 30%">
                            <asp:TextBox runat="server" ID="txtRMSD" CssClass="tb8 txtUppercase" Width="90%"
                                MaxLength="11" TabIndex="19"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="validateTxtRMSD" Display="Dynamic"
                                ControlToValidate="txtRMSD" Enabled="true" ErrorMessage="Retribuzione Media Settimanale D: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,6}(,\d{1,4})?" />
                        </td>
                        <td class="Row1" style="width: 3%">
                            <label>
                                €</label>
                        </td>
                        <td class="Row1" style="width: 14%">
                            <label>
                                Settimane D:</label>
                        </td>
                        <td class="Row1" style="width: 15%">
                            <asp:TextBox runat="server" ID="txtSettimaneD" CssClass="tb8 txtUppercase" Width="80%"
                                MaxLength="4" TabIndex="20"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneD" ControlToValidate="txtSettimaneD"
                                Display="Dynamic" ErrorMessage="Numero settimane D non valido: inserire il numero di settimane in un formato valido"
                                Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                        </td>
                        <td class="Row1" style="width: 5%">
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
                <tr>
                    <td class="Row1" style="width: 33%">
                        <label>
                            Retribuzione AGO annua:</label>
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox runat="server" ID="txtRetribuzioneAgoAnnua" CssClass="tb8 txtUppercase"
                            Width="90%" MaxLength="11" TabIndex="21"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ID="validateTxtRetribuzioneAgoAnnuaObbl"
                            ControlToValidate="txtRetribuzioneAgoAnnua" Display="Dynamic" Enabled="true"
                            ErrorMessage="Retribuzione AGO annua: campo obbligatorio" ValidationGroup="UCTabDatiCalcolo"
                            Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator runat="server" ID="validateTxtRetribuzioneAgoAnnua"
                            Display="Dynamic" ControlToValidate="txtRetribuzioneAgoAnnua" Enabled="true"
                            ErrorMessage="Retribuzione AGO annua: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,6}(,\d{1,4})?" />
                    </td>
                    <td class="Row1" style="width: 3%">
                        <label>
                            €</label>
                    </td>
                    <td class="Row1" style="width: 14%">
                    </td>
                    <td class="Row1" style="width: 15%">
                    </td>
                    <td class="Row1" style="width: 5%">
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- Pannello Dati Calcolo Retributivi Fondo Telefonici -->
        <asp:Panel ID="pnlDatiCalcoloRetributiviTT" runat="server" Visible="false">
            <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
                <tr>
                    <td class="Row1" style="width: 33%">
                        <label>
                            Retribuzione ultimo anno:</label>
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox ID="txtRetribUltimoAnnoRetrib" runat="server" CssClass="tb8 txtUppercase"
                            Width="90%" TabIndex="22" MaxLength="11" Enabled="false"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="validateTxtRetribUltimoAnnoRetrib"
                            Display="Dynamic" ControlToValidate="txtRetribUltimoAnnoRetrib" Enabled="true"
                            ErrorMessage="Retribuzione ultimo anno: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,6}(,\d{1,4})?" />
                        <input type="hidden" name="htxtRetribUltimoAnnoRetrib" id="htxtRetribUltimoAnnoRetrib"
                            value="" runat="server" />
                    </td>
                    <td class="Row1" style="width: 3%">
                        <label>
                            €</label>
                    </td>
                    <td class="Row1" style="width: 14%">
                    </td>
                    <td class="Row1" style="width: 15%">
                    </td>
                    <td class="Row1" style="width: 5%">
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 33%">
                        <label>
                            Retribuzione biennio:</label>
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox ID="txtRetribuzioneBiennio" runat="server" CssClass="tb8 txtUppercase"
                            Width="90%" TabIndex="23" MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="validateTxtRetribuzioneBiennio"
                            Display="Dynamic" ControlToValidate="txtRetribuzioneBiennio" Enabled="true" ErrorMessage="Retribuzione biennio: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,6}(,\d{1,4})?" />
                    </td>
                    <td class="Row1" style="width: 3%">
                        <label>
                            €</label>
                    </td>
                    <td class="Row1" style="width: 14%">
                    </td>
                    <td class="Row1" style="width: 15%">
                    </td>
                    <td class="Row1" style="width: 5%">
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- Fine Pannello Dati Calcolo Retributivi Fondo Telefonici-->
        <!-- Pannello Riduzione Retributiva-->
        <asp:Panel ID="pnlRiduzioneRetributiva" runat="server" Visible="true">
            <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
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
                            ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{1,2}(\,\d{1,2})?" />
                        <asp:CustomValidator runat="server" ControlToValidate="ddlRiduzioneRetributiva" Display="Dynamic"
                            ErrorMessage="Riduzione Retributiva: La percentuale è obbligatoria avendo selezionato 'SI'"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ID="customRiduzione" ClientValidationFunction="checkPercentualeRiduzione" />
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
    </div>
</asp:Panel>
<asp:Panel ID="pnlELAnteArmonizzazione" runat="server" Visible="false">
    <table class="tabellaFormattazione" width="100%">
        <tr>
            <td class="Row1" style="text-align: left">
                <label style="font-weight: bold" class="section-label mt-32">
                    Dati Ante 01/01/93 (Quota A)</label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
        <tr>
            <td class="Row1" style="width: 15%">
                <label>
                    Servizio Utile:</label>
            </td>
            <td class="Row1" style="width: 23%">
                <asp:TextBox ID="txtELAnteArmQtaA_AA" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="1" MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtELAnteArmQtaA_AA" ControlToValidate="txtELAnteArmQtaA_AA"
                    ErrorMessage="Servizio Utile Quota A: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                <asp:TextBox ID="txtELAnteArmQtaA_MM" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="2" MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtELAnteArmQtaA_MM" ControlToValidate="txtELAnteArmQtaA_MM"
                    ErrorMessage="Servizio Utile Quota A: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
            </td>
            <td class="Row1" style="width: 13%">
                <label>
                    Retribuzione Pensionabile:</label>
            </td>
            <td class="Row1" style="width: 21%">
                <asp:TextBox ID="txtELAnteArmQtaA_RetrPens" runat="server" CssClass="tb8 txtUppercase"
                    Width="70%" TabIndex="4" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtELAnteArmQuotaA_RetrPens"
                    Display="Dynamic" ControlToValidate="txtELAnteArmQtaA_RetrPens" Enabled="true"
                    ErrorMessage="Retribuzione Pensionabile Quota A: Inserire valori interi o decimali"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
            </td>
            <td id="Td2" class="Row1" style="width: 28%" runat="server">
                <asp:Panel ID="Panel2" runat="server" Visible="true">
                    <table>
                        <tr>
                            <td class="Row1" style="width: 64%" runat="server" id="Td3">
                                <label>
                                    ControCodice Retr.:</label>
                            </td>
                            <td class="Row1" style="width: 36%" runat="server" id="Td4">
                                <asp:TextBox ID="txtELAnteArmQtaA_CC" runat="server" CssClass="tb8 txtUppercase"
                                    Width="30px" TabIndex="5" MaxLength="3"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="REVtxtELAnteArmQtaA_CC" ControlToValidate="txtELAnteArmQtaA_CC"
                                    ErrorMessage="ControCodice Retr. Quota A: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione" width="100%">
        <tr>
            <td class="Row1" style="text-align: left">
                <label style="font-weight: bold" class="section-label mt-32">
                    Dati Post 31/12/92 (Quota B)</label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
        <tr>
            <td class="Row1" style="width: 15%">
                <label>
                    Servizio Utile:</label>
            </td>
            <td class="Row1" style="width: 23%">
                <asp:TextBox ID="txtELAnteArmQtaB_AA" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="6" MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtELAnteArmQtaB_AA" ControlToValidate="txtELAnteArmQtaB_AA"
                    ErrorMessage="Servizio Utile Quota B: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                <asp:TextBox ID="txtELAnteArmQtaB_MM" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="7" MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtELAnteArmQtaB_MM" ControlToValidate="txtELAnteArmQtaB_MM"
                    ErrorMessage="Servizio Utile Quota B: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
            </td>
            <td class="Row1" style="width: 13%">
                <label>
                    Retribuzione Pensionabile:</label>
            </td>
            <td class="Row1" style="width: 21%">
                <asp:TextBox ID="txtELAnteArmQtaB_RetrPens" runat="server" CssClass="tb8 txtUppercase"
                    Width="70%" TabIndex="9" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtELAnteArmQtaB_RetrPens"
                    Display="Dynamic" ControlToValidate="txtELAnteArmQtaB_RetrPens" Enabled="true"
                    ErrorMessage="Retribuzione Pensionabile Quota B: Inserire valori interi o decimali"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
            </td>
            <td id="Td5" class="Row1" style="width: 28%" runat="server">
                <table>
                    <tr>
                        <td class="Row1" style="width: 64%" runat="server" id="Td6">
                            <label>
                                ControCodice Retr.:</label>
                        </td>
                        <td class="Row1" style="width: 36%" runat="server" id="Td7">
                            <asp:TextBox ID="txtELAnteArmQtaB_CC" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" TabIndex="10" MaxLength="3"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="REVtxtELAnteArmQtaB_CC" ControlToValidate="txtELAnteArmQtaB_CC"
                                ErrorMessage="ControCodice Retr. Quota B: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione" width="100%">
        <tr>
            <td class="Row1" style="text-align: left">
                <label style="font-weight: bold" class="section-label mt-32">
                    Dati Post 31/12/94 (Quota C)</label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
        <tr>
            <td class="Row1" style="width: 15%">
                <label>
                    Servizio Utile:</label>
            </td>
            <td class="Row1">
                <asp:TextBox ID="txtELAnteArmQtaC_AA" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="11" MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REV" ControlToValidate="txtELAnteArmQtaC_AA"
                    ErrorMessage="Servizio Utile Quota C: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                <asp:TextBox ID="txtELAnteArmQtaC_MM" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="12" MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtELAnteArmQtaC_MM" ControlToValidate="txtELAnteArmQtaC_MM"
                    ErrorMessage="Servizio Utile Quota C: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlTTAnteArmonizzazione" runat="server" Visible="false">
    <table class="tabellaFormattazione" width="100%">
        <tr>
            <td class="Row1" style="text-align: left">
                <label style="font-weight: bold" class="section-label mt-32">
                    Dati Ante 01/01/93 (Quota A)</label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Servizio Utile:</label>
            </td>
            <td class="field fileds-date-input fileds-date-input--col-2" style="width: 25%">
                <asp:TextBox ID="txtTTAnteArmQtaA_AA" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="1" MaxLength="2"></asp:TextBox>
                AA
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtaA_AA" ControlToValidate="txtTTAnteArmQtaA_AA"
                    ErrorMessage="Servizio Utile Quota A: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                <asp:TextBox ID="txtTTAnteArmQtaA_MM" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="2" MaxLength="2"></asp:TextBox>
                MM
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtaA_MM" ControlToValidate="txtTTAnteArmQtaA_MM"
                    ErrorMessage="Servizio Utile Quota A: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Servizio Utile Ridotto:</label>
            </td>
            <td class="field fileds-date-input fileds-date-input--col-2" style="width: 25%">
                <asp:TextBox ID="txtTTAnteArmQtaARid_AA" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" MaxLength="2"></asp:TextBox>
                AA
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtaARid_AA" ControlToValidate="txtTTAnteArmQtaARid_AA"
                    ErrorMessage="S. U. Ridotto Quota A: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                <asp:TextBox ID="txtTTAnteArmQtaARid_MM" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" MaxLength="2"></asp:TextBox>
                MM
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtaARid_MM" ControlToValidate="txtTTAnteArmQtaARid_MM"
                    ErrorMessage="S. U. Ridotto Quota A: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Pensione al '53:</label>
            </td>
            <td class="field">
                <asp:TextBox ID="txtTTAnteArmPensioneAl53" runat="server" CssClass="tb8 txtUppercase"
                    Width="80%" MaxLength="9"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtTTAnteArmPensioneAl53" Display="Dynamic"
                    ControlToValidate="txtTTAnteArmPensioneAl53" Enabled="true" ErrorMessage="Pensione al '53: Inserire valori interi o decimali (4 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="^\d{1,4}(\,\d{1,4})?$" />
            </td>
            <td class="Row1">
                <label>
                    Retribuzione Ultimo Anno:</label>
            </td>
            <td class="field">
                <asp:TextBox ID="txtTTAnteArmRetrUAnno" runat="server" CssClass="tb8 txtUppercase"
                    Width="80%" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmRetrUAnno" ControlToValidate="txtTTAnteArmRetrUAnno"
                    ErrorMessage="Retr. U. Anno: Inserire valori interi o decimali (6 interi e 4 decimali)"
                    ValidationExpression="^\d{1,6}(\,\d{1,4})?$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
                    ValidationGroup="UCTabDatiCalcolo" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtTTAnteArmRetrUAnno" ControlToValidate="txtTTAnteArmRetrUAnno"
                    Display="Dynamic" Enabled="true" ErrorMessage="Retribuzione Ultimo Anno: campo obbligatorio"
                    ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Retribuzione Biennio:</label>
            </td>
            <td class="field">
                <asp:TextBox ID="txtTTAnteArmRetrBiennio" runat="server" CssClass="tb8 txtUppercase"
                    Width="80%" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmRetrBiennio" ControlToValidate="txtTTAnteArmRetrBiennio"
                    ErrorMessage="Retr. Bien.: Inserire valori interi o decimali (6 interi e 4 decimali)"
                    ValidationExpression="^\d{1,6}(\,\d{1,4})?$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
                    ValidationGroup="UCTabDatiCalcolo" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtTTAnteArmRetrBiennio" ControlToValidate="txtTTAnteArmRetrBiennio"
                    Display="Dynamic" Enabled="true" ErrorMessage="Retribuzione Biennio: campo obbligatorio"
                    ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
            <td class="Row1">
                <label>
                    Elementi Accessori:</label>
            </td>
            <td class="field">
                <asp:TextBox ID="txtTTAnteArmElAccess" runat="server" CssClass="tb8 txtUppercase"
                    Width="80%" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtTTAnteArmElAccess" Display="Dynamic"
                    ControlToValidate="txtTTAnteArmElAccess" Enabled="true" ErrorMessage="El. Access.: Inserire valori interi o decimali (6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Retribuzione Supplementi:</label>
            </td>
            <td class="field">
                <asp:TextBox ID="txtTTAnteArmRetrSup" runat="server" CssClass="tb8 txtUppercase"
                    Width="80%" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmRetrSup" ControlToValidate="txtTTAnteArmRetrSup"
                    ErrorMessage="Retr. Sup.: Inserire valori interi o decimali (6 interi e 4 decimali)"
                    ValidationExpression="^\d{1,6}(\,\d{1,4})?$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
                    ValidationGroup="UCTabDatiCalcolo" />
            </td>
            <td class="Row1">
                <label>
                    Controcodice Retributivo:</label>
            </td>
            <td class="field">
                <asp:TextBox ID="txtTTAnteArmControCodiceRetrQtA" runat="server" CssClass="tb8 txtUppercase"
                    Width="30%" MaxLength="3"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmControCodiceRetrQtA" ControlToValidate="txtTTAnteArmControCodiceRetrQtA"
                    ErrorMessage="ControCodice Retr. Quota A: formato non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtTTAnteArmControCodiceRetrQtA"
                    ControlToValidate="txtTTAnteArmControCodiceRetrQtA" Display="Dynamic" Enabled="true"
                    ErrorMessage="Controcodice retributivo Quota A: campo obbligatorio" ValidationGroup="UCTabDatiCalcolo"
                    Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione" width="100%">
        <tr>
            <td class="Row1" style="text-align: left">
                <label style="font-weight: bold" class="section-label mt-32">
                    Dati Post 31/12/92 (Quota B)</label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Servizio Utile:</label>
            </td>
            <td class="field fileds-date-input fileds-date-input--col-2" style="width: 25%">
                <asp:TextBox ID="txtTTAnteArmQtB_AA" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                    MaxLength="2"></asp:TextBox>
                AA
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtB_AA" ControlToValidate="txtTTAnteArmQtB_AA"
                    ErrorMessage="Servizio Utile Quota B: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                <asp:TextBox ID="txtTTAnteArmQtB_MM" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                    MaxLength="2"></asp:TextBox>
                MM
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtB_MM" ControlToValidate="txtTTAnteArmQtB_MM"
                    ErrorMessage="Servizio Utile Quota B: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Servizio Utile Ridotto:</label>
            </td>
            <td class="field fileds-date-input fileds-date-input--col-2" style="width: 25%">
                <asp:TextBox ID="txtTTAnteArmQtBRid_AA" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" MaxLength="2"></asp:TextBox>
                AA
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtBRid_AA" ControlToValidate="txtTTAnteArmQtBRid_AA"
                    ErrorMessage="S. U. Ridotto Quota B: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                <asp:TextBox ID="txtTTAnteArmQtBRid_MM" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" MaxLength="2"></asp:TextBox>
                MM
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtBRid_MM" ControlToValidate="txtTTAnteArmQtBRid_MM"
                    ErrorMessage="S. U. Ridotto Quota B: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Retribuzione Pensionabile:</label>
            </td>
            <td class="field">
                <asp:TextBox ID="txtTTAnteArmRetrPensionabileQtB" runat="server" CssClass="tb8 txtUppercase"
                    Width="80%" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmRetrPensionabileQtB" ControlToValidate="txtTTAnteArmRetrPensionabileQtB"
                    ErrorMessage="Retr. Pensionabile quota B: Inserire valori interi o decimali (6 interi e 4 decimali)"
                    ValidationExpression="^\d{1,6}(\,\d{1,4})?$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
                    ValidationGroup="UCTabDatiCalcolo" />
            </td>
            <td class="Row1">
                <label>
                    Controcodice Retributivo:</label>
            </td>
            <td class="field" style="width: 16%">
                <asp:TextBox ID="txtTTAnteArmControCodiceRetrQtB" runat="server" CssClass="tb8 txtUppercase"
                    Width="30%" MaxLength="3"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmControCodiceRetrQtB" ControlToValidate="txtTTAnteArmControCodiceRetrQtB"
                    ErrorMessage="ControCodice Retr. Quota B: formato non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione" width="100%">
        <tr>
            <td class="Row1" style="text-align: left">
                <label style="font-weight: bold" class="section-label mt-32">
                    Dati Post 31/12/94 (Quota C)</label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Servizio Utile:</label>
            </td>
            <td class="field fileds-date-input fileds-date-input--col-2" style="width: 25%">
                <asp:TextBox ID="txtTTAnteArmQtC_AA" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                    MaxLength="2"></asp:TextBox>
                AA
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtC_AA" ControlToValidate="txtTTAnteArmQtC_AA"
                    ErrorMessage="Servizio Utile Quota C: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                <asp:TextBox ID="txtTTAnteArmQtC_MM" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                    MaxLength="2"></asp:TextBox>
                MM
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtC_MM" ControlToValidate="txtTTAnteArmQtC_MM"
                    ErrorMessage="Servizio Utile Quota C: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Servizio Utile Ridotto:</label>
            </td>
            <td class="field fileds-date-input fileds-date-input--col-2" style="width: 25%">
                <asp:TextBox ID="txtTTAnteArmQtCRid_AA" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" MaxLength="2"></asp:TextBox>
                AA
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtCRid_AA" ControlToValidate="txtTTAnteArmQtCRid_AA"
                    ErrorMessage="S. U. Ridotto Quota C: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                <asp:TextBox ID="txtTTAnteArmQtCRid_MM" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" MaxLength="2"></asp:TextBox>
                MM
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtCRid_MM" ControlToValidate="txtTTAnteArmQtCRid_MM"
                    ErrorMessage="S. U. Ridotto Quota C: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione" width="100%">
        <tr>
            <td class="Row1" style="text-align: left">
                <label style="font-weight: bold" class="section-label mt-32">
                    Dati Post 31/12/96 (Quota D)</label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Servizio Utile:</label>
            </td>
            <td class="field fileds-date-input fileds-date-input--col-2" style="width: 25%">
                <asp:TextBox ID="txtTTAnteArmQtD_AA" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                    MaxLength="2"></asp:TextBox>
                AA
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtD_AA" ControlToValidate="txtTTAnteArmQtD_AA"
                    ErrorMessage="Servizio Utile Quota D: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                <asp:TextBox ID="txtTTAnteArmQtD_MM" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                    MaxLength="2"></asp:TextBox>
                MM
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtD_MM" ControlToValidate="txtTTAnteArmQtD_MM"
                    ErrorMessage="Servizio Utile Quota D: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Servizio Utile Ridotto:</label>
            </td>
            <td class="field fileds-date-input fileds-date-input--col-2" style="width: 25%">
                <asp:TextBox ID="txtTTAnteArmQtDRid_AA" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" MaxLength="2"></asp:TextBox>
                AA
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtDRid_AA" ControlToValidate="txtTTAnteArmQtDRid_AA"
                    ErrorMessage="S. U. Ridotto Quota D: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                <asp:TextBox ID="txtTTAnteArmQtDRid_MM" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" MaxLength="2"></asp:TextBox>
                MM
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmQtDRid_MM" ControlToValidate="txtTTAnteArmQtDRid_MM"
                    ErrorMessage="S. U. Ridotto Quota D: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Retribuzione Pensionabile:</label>
            </td>
            <td class="field">
                <asp:TextBox ID="txtTTAnteArmRetrPensionabileQtD" runat="server" CssClass="tb8 txtUppercase"
                    Width="80%" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtTTAnteArmRetrPensionabileQtD" ControlToValidate="txtTTAnteArmRetrPensionabileQtD"
                    ErrorMessage="Retr. Pensionabile quota D: Inserire valori interi o decimali (6 interi e 4 decimali)"
                    ValidationExpression="^\d{1,6}(\,\d{1,4})?$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
                    ValidationGroup="UCTabDatiCalcolo" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel runat="server" ID="pnlAnteArmonizzazioneCommon" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
        <tr>
            <td class="Row1" style="width: 30%">
                <label>
                    Retr. Ponderata Annua AGO per determinazione limite :</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:TextBox ID="txtAnteArm_RetrPondAGO" runat="server" CssClass="tb8 txtUppercase"
                    Style="width: 30%" TabIndex="11" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtAnteArm_RetrPondAGO" Display="Dynamic"
                    ControlToValidate="txtAnteArm_RetrPondAGO" Enabled="true" ErrorMessage="Retr. Ponderata Annua AGO per determinazione limite: Inserire 6 interi e 4 decimali"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Dati Calcolo Retributivi EL-TT-ET -->
<!-- Pannello Dati Calcolo Contributivi EL-TT-ET -->
<asp:Panel ID="pnlDatiCalcoloContributivi_EL_TT_ET" runat="server" Visible="false">
    <div id="pdivContributivo" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server">
        <asp:Panel ID="pnlDatiCalcoloContributiviLegge335_EL_TT_ET" runat="server" Visible="false">
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblTitoloContributiviL335" Text="Dati Contributivi da L. 335" runat="server"
                            Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
                <tr>
                    <td class="Row1" style="width: 33%">
                        <label>
                            Importo contributivo totale:</label>
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox runat="server" ID="txtImportoContributivoTotale" CssClass="tb8 txtUppercase"
                            Width="90%" MaxLength="12" TabIndex="25"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="validateTxtImportoContributivoTotale"
                            Display="Dynamic" ControlToValidate="txtImportoContributivoTotale" Enabled="true"
                            ErrorMessage="Importo Contributivo Totale: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="txtImportoContributivoTotale"
                            Display="Dynamic" Enabled="true" ErrorMessage="Importo Contributivo Totale: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                    <td class="Row1" style="width: 3%">
                        <label>
                            €</label>
                    </td>
                    <td class="Row1" style="width: 14%">
                    </td>
                    <td class="Row1" style="width: 15%">
                    </td>
                    <td class="Row1 none" style="width: 5%">
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 33%">
                        <label>
                            Montante:</label>
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox runat="server" ID="txtMontante" CssClass="tb8 txtUppercase" Width="90%"
                            MaxLength="12" TabIndex="26"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="validateTxtMontante" Display="Dynamic"
                            ControlToValidate="txtMontante" Enabled="true" ErrorMessage="Montante: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtMontante"
                            Display="Dynamic" Enabled="true" ErrorMessage="Montante: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                    <td class="Row1" style="width: 3%">
                        <label>
                            €</label>
                    </td>
                    <td class="Row1" style="width: 14%">
                        <label class="etichettaBold">
                            Settimane:</label>
                    </td>
                    <td class="Row1" style="width: 15%">
                        <asp:TextBox runat="server" ID="txtSettimane" CssClass="tb8 txtUppercase" Width="80%"
                            MaxLength="4" TabIndex="27"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimane" ControlToValidate="txtSettimane"
                            Display="Dynamic" ErrorMessage="Numero Settimane non valido: inserire il numero di settimane in un formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator5" ControlToValidate="txtSettimane"
                            Display="Dynamic" Enabled="true" ErrorMessage="Numero Settimane: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                    <td class="Row1 none" style="width: 5%">
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlDatiCalcoloContributiviLegge214_EL_TT_ET" runat="server" Visible="false">
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblDatiContributiviL214" runat="server" Text="Dati Contributivi da L. 214"
                            Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
                <tr>
                    <td class="Row1" style="width: 33%">
                        <label>
                            Importo contributivo totale:</label>
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox runat="server" ID="txtImportoContribTotaleQuotaDL214" CssClass="tb8 txtUppercase"
                            Width="90%" MaxLength="12" TabIndex="28"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator14"
                            Display="Dynamic" ControlToValidate="txtImportoContribTotaleQuotaDL214" Enabled="true"
                            ErrorMessage="Importo Contributivo Totale L. 214: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="txtImportoContribTotaleQuotaDL214RF"
                            ControlToValidate="txtImportoContribTotaleQuotaDL214" Display="Dynamic" Enabled="true"
                            ErrorMessage="Importo Contributivo Totale L. 214: campo obbligatorio" ValidationGroup="UCTabDatiCalcolo"
                            Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                    <td class="Row1" style="width: 3%">
                        <label>
                            €</label>
                    </td>
                    <td class="Row1" style="width: 14%">
                    </td>
                    <td class="Row1" style="width: 15%">
                    </td>
                    <td class="Row1 none" style="width: 5%">
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 33%">
                        <label>
                            Montante:</label>
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox runat="server" ID="txtMontanteQuotaDL214" CssClass="tb8 txtUppercase"
                            Width="90%" MaxLength="12" TabIndex="29"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator16"
                            Display="Dynamic" ControlToValidate="txtMontanteQuotaDL214" Enabled="true" ErrorMessage="Montante L. 214: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RFV_txtMontanteQuotaDL214" ControlToValidate="txtMontanteQuotaDL214"
                            Display="Dynamic" Enabled="true" ErrorMessage="Montante L. 214: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                    <td class="Row1" style="width: 3%">
                        <label>
                            €</label>
                    </td>
                    <td class="Row1" style="width: 14%">
                        <label class="etichettaBold">
                            Settimane:</label>
                    </td>
                    <td class="Row1" style="width: 15%">
                        <asp:TextBox runat="server" ID="txtNSettimaneQuotaDL214" CssClass="tb8 txtUppercase"
                            Width="80%" MaxLength="4" TabIndex="30"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator17"
                            ControlToValidate="txtNSettimaneQuotaDL214" Display="Dynamic" ErrorMessage="Numero Settimane L. 214 non valido: inserire il numero di settimane in un formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RFV_txtNSettimaneQuotaDL214" ControlToValidate="txtNSettimaneQuotaDL214"
                            Display="Dynamic" Enabled="true" ErrorMessage="Numero Settimane L. 214: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                    <td class="Row1" style="width: 5%">
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</asp:Panel>
<!-- Fine Pannello Dati Calcolo Contributivi EL-TT-ET -->
<!-- Pannello dati comma 707 -->
<asp:Panel runat="server" ID="pnlComma707" Visible="false">
    <div id="divComma707" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server">
        <table class="tabellaFormattazione" width="100%">
            <tr>
                <td class="section-label mt-32" style="text-align: left; font-weight: bold">
                    Calcolo ex comma 707
                </td>
            </tr>
        </table>
        <table runat="server" id="tblComma707EL_TT" class="tabellaFormattazione grid grid-size-20-col-5" width="100%"
            visible="false">
            <tr>
                <td class="Row1" style="width: 20%">
                    Quota A:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaAComma707EL_TT" CssClass="tb8 txtUppercase"
                        MaxLength="4" Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaAComma707EL_TT" ControlToValidate="txtQuotaAComma707EL_TT"
                        Display="Dynamic" ErrorMessage="Quota A del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                </td>
                <td class="Row1" style="width: 20%">
                    Quota B:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaBComma707EL_TT" CssClass="tb8 txtUppercase"
                        MaxLength="4" Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaBComma707EL_TT" ControlToValidate="txtQuotaBComma707EL_TT"
                        Display="Dynamic" ErrorMessage="Quota B del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 20%">
                    Quota C:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaCComma707EL_TT" CssClass="tb8 txtUppercase"
                        MaxLength="4" Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaCComma707EL_TT" ControlToValidate="txtQuotaCComma707EL_TT"
                        Display="Dynamic" ErrorMessage="Quota C del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                </td>
                <td class="Row1" style="width: 20%">
                    Quota D:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaDComma707EL_TT" CssClass="tb8 txtUppercase"
                        MaxLength="4" Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaDComma707EL_TT" ControlToValidate="txtQuotaDComma707EL_TT"
                        Display="Dynamic" ErrorMessage="Quota D del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                </td>
            </tr>
        </table>
        <table runat="server" id="tblComma707ET" class="tabellaFormattazione grid grid-size-20-col-5" width="100%"
            visible="false">
            <tr>
                <td class="Row1">
                    Quota A Fondo:
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtQuotaAComma707ETAA" CssClass="tb8 txtUppercase"
                        MaxLength="2" Width="18%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaAComma707ETAA" ControlToValidate="txtQuotaAComma707ETAA"
                        Display="Dynamic" ErrorMessage="Quota A Fondo AA del Calcolo ex comma 707: Inserire valori interi (max 2 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                    <asp:TextBox runat="server" ID="txtQuotaAComma707ETMM" CssClass="tb8 txtUppercase"
                        MaxLength="2" Width="18%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaAComma707ETMM" ControlToValidate="txtQuotaAComma707ETMM"
                        Display="Dynamic" ErrorMessage="Quota A Fondo MM del Calcolo ex comma 707: Inserire valori interi (max 2 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                    <asp:TextBox runat="server" ID="txtQuotaAComma707ETGG" CssClass="tb8 txtUppercase"
                        MaxLength="2" Width="18%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaAComma707ETGG" ControlToValidate="txtQuotaAComma707ETGG"
                        Display="Dynamic" ErrorMessage="Quota A Fondo GG del Calcolo ex comma 707: Inserire valori interi (max 2 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                </td>
                <td class="Row1" runat="server" id="tdRigaA707ET_lbl">
                    Settimane quota A AGO:
                </td>
                <td class="field" runat="server" id="tdRigaA707ET_txt">
                    <asp:TextBox runat="server" ID="txtQuotaAComma707ET" CssClass="tb8 txtUppercase"
                        MaxLength="4" Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaAComma707ET" ControlToValidate="txtQuotaAComma707ET"
                        Display="Dynamic" ErrorMessage="Quota A AGO del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 20%">
                    Quota B Fondo:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaBComma707ETAA" CssClass="tb8 txtUppercase"
                        MaxLength="2" Width="18%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaBComma707ETAA" ControlToValidate="txtQuotaBComma707ETAA"
                        Display="Dynamic" ErrorMessage="Quota B Fondo AA del Calcolo ex comma 707: Inserire valori interi (max 2 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                    <asp:TextBox runat="server" ID="txtQuotaBComma707ETMM" CssClass="tb8 txtUppercase"
                        MaxLength="2" Width="18%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaBComma707ETMM" ControlToValidate="txtQuotaBComma707ETMM"
                        Display="Dynamic" ErrorMessage="Quota B Fondo MM del Calcolo ex comma 707: Inserire valori interi (max 2 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                    <asp:TextBox runat="server" ID="txtQuotaBComma707ETGG" CssClass="tb8 txtUppercase"
                        MaxLength="2" Width="18%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaBComma707ETGG" ControlToValidate="txtQuotaBComma707ETGG"
                        Display="Dynamic" ErrorMessage="Quota B Fondo GG del Calcolo ex comma 707: Inserire valori interi (max 2 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                </td>
                <td class="Row1" style="width: 30%">
                    Settimane quota B AGO:
                </td>
                <td class="field" style="width: 20%">
                    <asp:TextBox runat="server" ID="txtQuotaBComma707ET" CssClass="tb8 txtUppercase"
                        MaxLength="4" Width="60%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaBComma707ET" ControlToValidate="txtQuotaBComma707ET"
                        Display="Dynamic" ErrorMessage="Quota B Fondo del Calcolo ex comma 707: Inserire valori interi (max 4 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    Quota C Fondo:
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtQuotaCComma707ETAA" CssClass="tb8 txtUppercase"
                        MaxLength="2" Width="18%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaCComma707ETAA" ControlToValidate="txtQuotaCComma707ETAA"
                        Display="Dynamic" ErrorMessage="Quota C Fondo AA del Calcolo ex comma 707: Inserire valori interi (max 2 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                    <asp:TextBox runat="server" ID="txtQuotaCComma707ETMM" CssClass="tb8 txtUppercase"
                        MaxLength="2" Width="18%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaCComma707ETMM" ControlToValidate="txtQuotaCComma707ETMM"
                        Display="Dynamic" ErrorMessage="Quota C Fondo MM del Calcolo ex comma 707: Inserire valori interi (max 2 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                    <asp:TextBox runat="server" ID="txtQuotaCComma707ETGG" CssClass="tb8 txtUppercase"
                        MaxLength="2" Width="18%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaCComma707ETGG" ControlToValidate="txtQuotaCComma707ETGG"
                        Display="Dynamic" ErrorMessage="Quota C Fondo GG del Calcolo ex comma 707: Inserire valori interi (max 2 interi)"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione grid grid-size-20-col-5" width="100%">
            <tr>
                <td class="Row1" style="width: 45%">
                    Retribuzione ponderata AGO per calcolo limite:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtRetribuzionePonderataComma707" CssClass="tb8 txtUppercase"
                        MaxLength="12" Width="90%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REV_txtRetribuzionePonderataComma707"
                        Display="Dynamic" ControlToValidate="txtRetribuzionePonderataComma707" Enabled="true"
                        ErrorMessage="Retribuzione ponderata AGO per calcolo limite: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtRetribuzionePonderataComma707"
                        ControlToValidate="txtRetribuzionePonderataComma707" Display="Dynamic" Enabled="true"
                        ErrorMessage="Retribuzione ponderata AGO per calcolo limite: campo obbligatorio"
                        ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
                <td class="Row1" style="width: 20%">
                    <label>
                        €</label>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<!-- Fine Pannello dati comma 707 -->
<div style="margin-right: 40px;" class="containerWidth xs">
    <table width="100%" style="min-height: 100px;" class="tab-actions-group">
        <tr>
            <td style="text-align: right; vertical-align: bottom;" class="tab-actions-group__first">
                <asp:Button ID="btnPopUpContributivi" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Style="display: none" Text="Salva Dati Calcolo" Width="150px" OnClientClick="if(Page_ClientValidate('UCTabDatiCalcolo')){return ConfirmContributivi();}" CssClass="primary" />
                <asp:Button ID="btnPopUp" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Visible="false" Text="Salva Dati Calcolo" Width="150px" OnClientClick="if(Page_ClientValidate('UCTabDatiCalcolo')){return Confirm();}" CssClass="primary" />
                <asp:Button ID="btnSalvaDatiCalcolo" runat="server" CausesValidation="false" Style="display: none"
                    ValidationGroup="UCTabDatiCalcolo" SkinID="btnAzione1" Width="150px" OnClick="btnSalvaDatiCalcolo_Click"
                    Text="Salva Dati Calcolo" Visible="false" OnClientClick="if(Page_ClientValidate('UCTabDatiCalcolo')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
                <asp:Button ID="btnSalvaDatiCalcoloNoRiduzione" runat="server" CausesValidation="false"
                    ValidationGroup="UCTabDatiCalcolo" SkinID="btnAzione1" Width="150px" OnClick="btnSalvaDatiCalcolo_Click"
                    Text="Salva Dati Calcolo" Visible="true" OnClientClick="if(Page_ClientValidate('UCTabDatiCalcolo')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
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
<div id="dialog-confirm" title="Confirm" style="border-style: none; border-color: White;">
    <p>
        <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
        Età titolare inferiore a 62 anni. Confermi la mancanza della percentuale di Riduzione?
    </p>
</div>
<div id="dialog-Contributivi" title="Confirm" style="border-style: none; border-color: White;">
    <p>
        <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
        Attenzione il Montante è inferiore all’Ammontare.<br />
        Confermare ?
    </p>
</div>
