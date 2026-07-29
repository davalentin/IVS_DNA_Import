<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCBenefici.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBenefici.UCBenefici" %>
<script type="text/javascript">
    $(document).ready(function () {
        $("#<%= ddlTipoSettimaneBeneficio.ClientID %>").change(ddlTipoSettimaneBeneficioOnChange);

        ddlTipoSettimaneBeneficioOnChange();
    });

    function ddlTipoSettimaneBeneficioOnChange() {
        if (document.getElementById("<%= hdnIsRicostituzione.ClientID %>").value == "NO") {
            if ($("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "05" || $("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "11" ||
                $("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "14" ||
                $("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "12" ||
                $("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "15" ||
                $("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "18" ||
                $("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "19" ||
                $("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "24") {
                $("#<%= txtNumeroSettimaneBeneficio.ClientID%>").val('');
                $("#<%= txtNumeroSettimaneBeneficio.ClientID%>").attr('disabled', true);
            }
            else if ($("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() != "01")
                $("#<%= txtNumeroSettimaneBeneficio.ClientID%>").removeAttr('disabled');
        }

        if ($("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "01") {
            $("#<%= lblSettAnzContPost311295.ClientID%>").show();
            $("#<%= txtSettAnzContPost311295.ClientID%>").show();
            $("#<%= lblSettAnzContPost311295.ClientID %>").text("Settimane anz. contrib. quota contributiva:");
            setValueHiddenFieldVerificaAperturaPopup("1");
        }
        else {
            $("#<%= lblSettAnzContPost311295.ClientID%>").hide();
            $("#<%= txtSettAnzContPost311295.ClientID%>").hide();
            setValueHiddenFieldVerificaAperturaPopup("0");
        }
    };

    function verificaAperturaPopupSettimanaContributiva() {

        if (document.getElementById("<%= hdnNumeroSettimaneUtiliDiritto.ClientID %>").value != null && document.getElementById("<%= hdnNumeroSettimaneUtiliDiritto.ClientID %>").value != "") {

            if (document.getElementById("<%= txtSettAnzContPost311295.ClientID %>").value != null && document.getElementById("<%= txtSettAnzContPost311295.ClientID %>").value.trim() != "") {
                var settimaneUtiliDiritto = parseFloat(document.getElementById("<%= hdnNumeroSettimaneUtiliDiritto.ClientID %>").value);
                var settimaneQuotaContributiva = parseInt(document.getElementById("<%= txtSettAnzContPost311295.ClientID %>").value);

                if (settimaneQuotaContributiva > Math.ceil(settimaneUtiliDiritto / 3) && document.getElementById("<%= hdnDecorrenzaPost012017.ClientID %>").value == "SI") {
                    return true;
                }
            }
        }

        return false;

    }

    $(function () {
        $('#dialog-confirm').dialog({
            autoOpen: false,

            show: 'blind',
            hide: 'blind',
            height: 230,
            width: 455,
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
                    setValueHiddenFieldVerificaAperturaPopup("0");
                    document.getElementById('<%= btnBenefici.ClientID %>').click();
                    return true;
                }
            }
        });
    });

    function GestisciSalvataggioConPopup() {


        if ($("#<%= hdnVerificaAperturaPopup.ClientID %>").val() == "1") { //se è selezionato il tipo beneficio Non Vedente, verifico se devo effettivamente aprire il Popup

            var isPopupVisible = verificaAperturaPopupSettimanaContributiva();

            if (isPopupVisible == true) {
                $('#dialog-confirm').dialog('open');
                return false;
            }
            else {

                aspnetForm.target = '_self';
                BlockUI();
                return true;
            }


        } else {

            aspnetForm.target = '_self';
            BlockUI();
            return true;

        }
    }

    function getHiddenFieldVerificaAperturaPopup() {

        return $("#<%= hdnVerificaAperturaPopup.ClientID %>").val();
    }

    function setValueHiddenFieldVerificaAperturaPopup(verifica) {

        $("#<%= hdnVerificaAperturaPopup.ClientID %>").val(verifica);
    }

</script>
<asp:Panel ID="pnlBeneficiCommon" runat="server">
    <table class="tabellaFormattazione grid grid-size-20" cellpadding="3" cellspacing="1" border="0" width="100%">
        <tr>
            <td colspan="4" style="height: 5px" class="none">
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 20%">
                <asp:Label runat="server" ID="lblTipoSettimaneBeneficio">
                    Tipo beneficio:</asp:Label>
            </td>
            <td class="Row1 full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlTipoSettimaneBeneficio" CssClass="tb8 txtUppercase"
                    TabIndex="1" Width="95%">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <asp:Panel runat="server" ID="pnlDataNonVedenteDal" Visible="false">
                <td class="Row1" style="width: 25%">
                    <label>
                        Data 'Non vedente dal':</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDataNonVedenteDal" Width="100px"
                        CssClass="txtUppercase tb8 dateGGmmAAAA" MaxLength="10" Enabled="false"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validatetxtDataNonVedenteDal" ControlToValidate="txtDataNonVedenteDal"
                        ErrorMessage="Data 'Non vedente dal' in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$|^GG/MM/AAAA$|^gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabBenefici" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDataNonVedenteDal" Display="Dynamic"
                        ErrorMessage="Data 'Non vedente dal': data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabBenefici"
                        ID="customCheckDataNonVedenteDal" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </asp:Panel>
        </tr>
        <tr>
            <asp:Panel ID="pnlSettBeneficio" runat="server" Visible="false">
                <td class="Row1" style="width: 20%">
                    <asp:Label runat="server" ID="lblNumeroSettimaneBeneficio">Numero settimane beneficio:</asp:Label>
                </td>
                <td class="Row1" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtNumeroSettimaneBeneficio" CssClass="tb8 txtUppercase"
                        TabIndex="2" Width="38%" MaxLength="4" onblur="extractNumber(this,0,false);"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtNumeroSettimaneBeneficio"
                        ControlToValidate="txtNumeroSettimaneBeneficio" Display="Dynamic" ErrorMessage="Inserire il valore in un formato valido per Numero settimane beneficio"
                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]+$" ValidationGroup="UCTabBenefici" />
                </td>
            </asp:Panel>
            <td class="Row1" style="width: 25%">
                <asp:Label ID="lblSettAnzContPost311295" runat="server" Text="Settimane anz. contrib. successive al 31/12/1995:"
                    Style="display: none;"></asp:Label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:TextBox runat="server" ID="txtSettAnzContPost311295" CssClass="tb8 txtUppercase"
                    Width="50px" MaxLength="4" onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                    onkeypress="return blockNonNumbers(this, event, false, false);" Style="display: none;"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateTxtSettAnzContPost311295"
                    ControlToValidate="txtNumeroSettimaneBeneficio" Display="Dynamic" ErrorMessage="Inserire il valore in un formato valido per Settimane anz. contrib. successive al 31/12/1995"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]+$" ValidationGroup="UCTabBenefici" />
            </td>
        </tr>
        <tr>
            <td class="grid-col-2" colspan="3">
                <asp:Panel ID="pnlBenefTemporale" runat="server" Visible="false">
                    <table class="tabellaFormattazione grid grid-col-6">
                        <tr>
                            <td class="Row1 shift-full-grid" style="width: 15.5%">
                                <asp:Label runat="server" ID="Label3">Beneficio temporale:</asp:Label>
                            </td>
                            <td class="field fileds-date-input" style="width: 5%">
                                <asp:TextBox runat="server" ID="txtAABeneficioTemporale" CssClass="tb8 txtUppercase"
                                    Width="95%" TabIndex="3" MaxLength="2"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="txtAABeneficioTemporale_RV" ControlToValidate="txtAABeneficioTemporale"
                                    Display="Dynamic" ErrorMessage="Inserire il valore in un formato valido per Beneficio temporale AA"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]+$" ValidationGroup="UCTabBenefici" />
                                <label class="font-semibold">
                                    AA</label>
                                <asp:TextBox runat="server" ID="txtMMBeneficioTemporale" CssClass="tb8 txtUppercase"
                                    Width="95%" TabIndex="4" MaxLength="2"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="txtMMBeneficioTemporale_RV" ControlToValidate="txtMMBeneficioTemporale"
                                    Display="Dynamic" ErrorMessage="Inserire il valore in un formato valido per Beneficio temporale MM"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]+$" ValidationGroup="UCTabBenefici" />
                                <label class="font-semibold">
                                    MM</label>
                                <asp:TextBox runat="server" ID="txtGGBeneficioTemporale" CssClass="tb8 txtUppercase"
                                    Width="95%" TabIndex="5" MaxLength="2"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="txtGGBeneficioTemporale_RV" ControlToValidate="txtGGBeneficioTemporale"
                                    Display="Dynamic" ErrorMessage="Inserire il valore in un formato valido per Beneficio temporale GG"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]+$" ValidationGroup="UCTabBenefici" />
                                <label class="font-semibold">
                                    GG</label>
                            </td>
                            <td style="width: 47%" class="none">
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Panel>
<br />
<asp:Panel ID="pnlMaggiorazioneSociale" runat="server" Visible="false">
    <div id="divMaggiorazioneSociale" style="border-style: solid; border-color: #000080;
        border-collapse: collapse; border-width: 1px; margin: 4px">
        <table class="tabellaFormattazione  grid grid-size-20">
            <tr>
                <td class="Row1 shift-full-grid" style="width: 100%" colspan="4">
                    <asp:Label runat="server" ID="Label1" Style="font-style: italic" CssClass="section-label">Maggiorazione Sociale</asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 16%">
                    <label>
                        Decorrenza:</label>
                </td>
                <td class="Row1" style="width: 28%">
                    <asp:TextBox runat="server" ID="txtDecorrenza" CssClass="tb8 txtUppercase date-picker dateMMaaaa"
                        MaxLength="7" Width="30%" TabIndex="6" Text="MM/AAAA"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateDecorrenza" ControlToValidate="txtDecorrenza"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}$" Enabled="true" Text="*" CssClass="field-is-required" ErrorMessage="Decorrenza: Formato data non corretto"
                        Display="Dynamic" ValidationGroup="UCTabBenefici" />
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtDecorrenza" ControlToValidate="txtDecorrenza"
                        Display="Dynamic" ErrorMessage="Inserire un formato data valido per Decorrenza"
                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9\/]+|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabBenefici" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabBenefici"
                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />
                </td>
                <td class="Row1" style="width: 16%">
                    <label>
                        Cessazione:</label>
                </td>
                <td class="Row1" style="width: 40%">
                    <asp:TextBox runat="server" ID="txtCessazione" CssClass="tb8 txtUppercase date-picker dateMMaaaa"
                        MaxLength="7" Width="22%" TabIndex="7" Text="MM/AAAA"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" ControlToValidate="txtCessazione"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}$" Enabled="true" Text="*" CssClass="field-is-required" ErrorMessage="Cessazione: Formato data non corretto"
                        Display="Dynamic" ValidationGroup="UCTabBenefici" />
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="txtCessazione"
                        Display="Dynamic" ErrorMessage="Inserire un formato data valido per Cessazione"
                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9\/]+|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabBenefici" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtCessazione" Display="Dynamic"
                        ErrorMessage="Cessazione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabBenefici"
                        ID="customCheckDataCessazione" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<br />
<asp:Panel ID="pnlOneriTerrorismo" runat="server" Visible="false">
    <div id="pdivOneriTerrorismo" style="border-style: solid; border-color: #000080;
        border-collapse: collapse; border-width: 1px; margin: 4px">
        <table class="tabellaFormattazione  grid grid-size-20">
            <tr>
                <td class="Row1 shift-full-grid" colspan="4">
                    <asp:Label runat="server" ID="Label2" Style="font-style: italic" CssClass="section-label">Oneri terrorismo</asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 16%">
                    <label>
                        Importi:</label>
                </td>
                <td class="field" style="width: 28%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtOneriTerrorismoUno" Width="80%"
                        CssClass="txtUppercase tb8 " MaxLength="7" TabIndex="8"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator4" ControlToValidate="txtOneriTerrorismoUno"
                        Display="Dynamic" ErrorMessage="Oneri Terrorismo Uno: Inserire massimo 4 cifre intere e 2 decimali"
                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,4}(\,\d{1,2})?" ValidationGroup="UCTabBenefici" />
                </td>
                <td class="field" style="width: 28%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtOneriTerrorismoDue" Width="80%"
                        CssClass="txtUppercase tb8 " MaxLength="7" TabIndex="9"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator5" ControlToValidate="txtOneriTerrorismoDue"
                        Display="Dynamic" ErrorMessage="Oneri Terrorismo Due: Inserire massimo 4 cifre intere e 2 decimali"
                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,4}(\,\d{1,2})?" ValidationGroup="UCTabBenefici" />
                </td>
                <td class="field" style="width: 28%;">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtOneriTerrorismoTre" Width="80%"
                        CssClass="txtUppercase tb8 " MaxLength="7" TabIndex="10"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator6" ControlToValidate="txtOneriTerrorismoTre"
                        Display="Dynamic" ErrorMessage="Oneri Terrorismo Tre: Inserire massimo 4 cifre intere e 2 decimali"
                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,4}(\,\d{1,2})?" ValidationGroup="UCTabBenefici" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<div style="margin-top: 100px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnBenefici" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Benefici" TabIndex="11" Width="180px" OnClick="SalvaBenefici_Click"
                    OnClientClick="if(Page_ClientValidate('UCTabBenefici')){return GestisciSalvataggioConPopup();}" CssClass="primary" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaBenefici" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Benefici" TabIndex="12" Width="180px" OnClick="EliminaBenefici_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare Benefici?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
<asp:HiddenField runat="server" ID="hdnIsRicostituzione" />
<asp:HiddenField runat="server" ID="hdnNumeroSettimaneUtiliDiritto" />
<asp:HiddenField runat="server" ID="hdnVerificaAperturaPopup" />
<asp:HiddenField runat="server" ID="hdnDecorrenzaPost012017" />
<div id="dialog-confirm" title="Confirm" style="border-style: none; border-color: White;">
    <p>
        <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
        Attenzione: verificare la corretta attribuzione delle settimane di beneficio sulla
        quota di pensione calcolata con il sistema contributivo. Si rinvia al messaggio
        2114/2018.
    </p>
</div>
