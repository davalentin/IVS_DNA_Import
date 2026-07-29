<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCBenefici.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiCi.UCBenefici" %>
<script type="text/javascript">
    $(document).ready(function () {
        $("#<%= ddlTipoSettimaneBeneficio.ClientID %>").change(ddlTipoSettimaneBeneficioOnChange);

        ddlTipoSettimaneBeneficioOnChange();
    });

    function ddlTipoSettimaneBeneficioOnChange() {
        if (document.getElementById("<%= hdnIsRicostituzione.ClientID %>").value == "NO") {
            if ($("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "10" || $("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "11" ||
                $("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "12" ||
                $("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "15") {
                $("#<%= txtNumeroSettimaneBeneficio.ClientID%>").val('');
                $("#<%= txtNumeroSettimaneBeneficio.ClientID%>").attr('disabled', true);
            }
            else if (document.getElementById("<%= hdnIsMaggiorazioneAmiantoLegge208_2015.ClientID %>").value != "SI" && $("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() != "01")
                $("#<%= txtNumeroSettimaneBeneficio.ClientID%>").removeAttr('disabled');
        }

        if ($("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "01" ||
            $("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "04" && document.getElementById("<%= hdnIsMaggiorazioneAmiantoLegge208_2015.ClientID %>").value == "SI") {


            $("#<%= lblSettAnzContPost311295.ClientID%>").show();
            $("#<%= txtSettAnzContPost311295.ClientID%>").show();


            if ($("#<%= ddlTipoSettimaneBeneficio.ClientID %>").val() == "01") {
                $("#<%= lblSettAnzContPost311295.ClientID %>").text("Settimane anz. contrib. quota contributiva:");
                setValueHiddenFieldVerificaAperturaPopup("1");
            }
            else {
                $("#<%= lblSettAnzContPost311295.ClientID %>").text("Settimane anz. contrib. successive al 31/12/1995:");
                setValueHiddenFieldVerificaAperturaPopup("0");
            }

        }
        else {
            $("#<%= lblSettAnzContPost311295.ClientID%>").hide();
            $("#<%= txtSettAnzContPost311295.ClientID%>").hide();
            setValueHiddenFieldVerificaAperturaPopup("0");
        }




    };



    function verificaAperturaPopupSettimanaContributiva() {


        if (document.getElementById("<%= hdnNumeroSettimaneDatiAssicurativi.ClientID %>").value != null && document.getElementById("<%= hdnNumeroSettimaneDatiAssicurativi.ClientID %>").value != "") {

            if (document.getElementById("<%= txtSettAnzContPost311295.ClientID %>").value != null && document.getElementById("<%= txtSettAnzContPost311295.ClientID %>").value.trim() != "") {
                var settimaneDatiAssicurativi = parseFloat(document.getElementById("<%= hdnNumeroSettimaneDatiAssicurativi.ClientID %>").value);
                var settimaneQuotaContributiva = parseInt(document.getElementById("<%= txtSettAnzContPost311295.ClientID %>").value);
                if (settimaneQuotaContributiva > Math.ceil(settimaneDatiAssicurativi / 3)) {
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
<asp:Panel ID="pnlBenefici" runat="server">
    <table class="tabellaFormattazione" cellpadding="3" cellspacing="1" border="0" width="100%">
        <tr>
            <td class="Row1" style="width: 25%">
                <asp:Label runat="server" ID="lblTipoSettimaneBeneficio">
                    Tipo beneficio:</asp:Label>
            </td>
            <td class="Row1 full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlTipoSettimaneBeneficio" CssClass="tb8 txtUppercase"
                    TabIndex="1" Width="94.5%">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
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
            <td class="Row1" style="width: 25%">
                <asp:Label runat="server" ID="lblNumeroSettimaneBeneficio">
                    Numero settimane beneficio:</asp:Label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:TextBox runat="server" ID="txtNumeroSettimaneBeneficio" CssClass="tb8 txtUppercase"
                    TabIndex="2" Width="26.5%" MaxLength="4" onblur="extractNumber(this,0,false);"
                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateTxtNumeroSettimaneBeneficio"
                    ControlToValidate="txtNumeroSettimaneBeneficio" Display="Dynamic" ErrorMessage="Inserire il valore in un formato valido per Numero settimane beneficio"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]+$" ValidationGroup="UCTabBenefici" />
            </td>
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
    </table>
</asp:Panel>
<br />
<asp:Panel ID="pnlOneriTerrorismo" Visible="false" runat="server">
    <div id="pdivOneriTerrorismo" style="border-style: solid; border-color: #000080;
        border-collapse: collapse; border-width: 1px; margin: 4px">
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="width: 25%">
                    <asp:Label runat="server" ID="Label2" Style="font-style: italic">Oneri terrorismo</asp:Label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtOneriTerrorismoUno" Width="80%"
                        CssClass="txtUppercase tb8 " MaxLength="7" TabIndex="7"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator4" ControlToValidate="txtOneriTerrorismoUno"
                        Display="Dynamic" ErrorMessage="Oneri Terrorismo Uno: Inserire massimo 4 cifre intere e 2 decimali"
                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,4}(\,\d{1,2})?" ValidationGroup="UCTabBenefici" />
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtOneriTerrorismoDue" Width="80%"
                        CssClass="txtUppercase tb8 " MaxLength="7" TabIndex="8"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator5" ControlToValidate="txtOneriTerrorismoDue"
                        Display="Dynamic" ErrorMessage="Oneri Terrorismo Due: Inserire massimo 4 cifre intere e 2 decimali"
                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,4}(\,\d{1,2})?" ValidationGroup="UCTabBenefici" />
                </td>
                <td class="field" style="width: 25%;">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtOneriTerrorismoTre" Width="80%"
                        CssClass="txtUppercase tb8 " MaxLength="7" TabIndex="9"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator6" ControlToValidate="txtOneriTerrorismoTre"
                        Display="Dynamic" ErrorMessage="Oneri Terrorismo Tre: Inserire massimo 4 cifre intere e 2 decimali"
                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,4}(\,\d{1,2})?" ValidationGroup="UCTabBenefici" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<asp:Panel ID="pnlSentenze" runat="server">
    <br />
    <table class="tabellaFormattazione" cellpadding="3" cellspacing="1" border="0" width="100%">
        <tr>
            <!-- Campo non visibile come richiesto dalla specifica della mail ReEng - modifiche CI fase 4 in data 10/06/13 -->
            <td class="Row1" style="width: 25%; display: none;">
                <label>
                    Sentenze:</label>
            </td>
            <td class="Row1 full-grid" colspan="3" style="display: none;">
                <asp:TextBox runat="server" ID="txtSentenza495240" CssClass="tb8 txtUppercase" TabIndex="6"
                    Width="26.5%" MaxLength="1" onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                    onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Settimane con incremento 1%:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:TextBox runat="server" ID="txtSettimane1Percento" CssClass="tb8 txtUppercase"
                    TabIndex="7" Width="26.5%" MaxLength="3" onblur="extractNumber(this,0,false);"
                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="txtSettimane1Percento"
                    Display="Dynamic" ErrorMessage="Inserire il valore in un formato valido per Settimane con incremento 1%"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]+$" ValidationGroup="UCTabBenefici" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Settimane con incremento 0.5%:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:TextBox runat="server" ID="txtSettimane05Percento" CssClass="tb8 txtUppercase"
                    TabIndex="8" Width="26.5%" MaxLength="3" onblur="extractNumber(this,0,false);"
                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" ControlToValidate="txtSettimane05Percento"
                    Display="Dynamic" ErrorMessage="Inserire il valore in un formato valido per Settimane con incremento 0.5%"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]+$" ValidationGroup="UCTabBenefici" />
            </td>
        </tr>
    </table>
</asp:Panel>
<div style="margin-top: 100px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnBenefici" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Benefici" Width="170px" OnClick="SalvaBenefici_Click"
                    OnClientClick="if(Page_ClientValidate('UCTabBenefici')){return GestisciSalvataggioConPopup();}" CssClass="primary" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaBenefici" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Benefici" Width="170px" OnClick="EliminaBenefici_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare Benefici?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
<asp:HiddenField runat="server" ID="hdnIsRicostituzione" />
<asp:HiddenField runat="server" ID="hdnIsMaggiorazioneAmiantoLegge208_2015" />
<asp:HiddenField runat="server" ID="hdnNumeroSettimaneDatiAssicurativi" />
<asp:HiddenField runat="server" ID="hdnVerificaAperturaPopup" />
<div id="dialog-confirm" title="Confirm" style="border-style: none; border-color: White;">
    <p>
        <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
        Attenzione: verificare la corretta attribuzione delle settimane di beneficio sulla
        quota di pensione calcolata con il sistema contributivo. Si rinvia al messaggio
        2114/2018.
    </p>
</div>
