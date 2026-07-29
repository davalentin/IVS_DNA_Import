<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiGenerici.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCDatiGenerici" %>
<script type="text/javascript">
    var uiDPchiuso = false;

    //datepicker che visualizza gg/mm/aa
    $(function () {
        $('.date-picker-dataCompletezza').datepicker({
            changeMonth: true,
            changeYear: true,
            changeDay: true,
            showButtonPanel: true,
            dateFormat: 'dd/mm/yy',
            showOn: 'button',
            buttonImageOnly: true,
            buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
            yearRange: '-100:' + '+0',
            minDate: '-100y',
            maxDate: '+0',
            onSelect: function (dateText, inst) { CalcolaDataInteressiLegali(dateText); }
        });
        //$(".date-picker-dataCompletezza").unmask();
        //$(".date-picker-dataCompletezza").mask("99/99/9999");
    });

    $(document).ready(function () {
        setDataInteressiLegali();
        SetCalendarioDataCalcolo();
        SetCalendarioDecorrenzaBonus();
        SetCalendarioRevSanitaria();
        var aoiChecked = $(document.getElementById("<%=chkTrasfAOI.ClientID %>")).attr("checked");
        var causaCarico = document.getElementById("<%=ddlCausaCarico.ClientID%>").value;
        if (aoiChecked || causaCarico == 3 || causaCarico == 9) {
            AbilitaTab();
        }
        else {
            DisabilitaTab();
        }
        getDDLCodNatura2Value();

        // gestione della visibilità della tab PrecedentePensione nel caso si provi a salvare dati generici senza il flag.
        if (document.getElementById("<%=HiddenPrecedentePensione.ClientID%>") != null && document.getElementById("<%=HiddenPrecedentePensione.ClientID%>").value == "true") {
            $(document.getElementById("<%=chkTrasfAOI.ClientID %>")).attr("checked", true);
            AbilitaTab();
        }

        //gestione del blocco check benefici se CodNatura3='G'
        SetCheckBenefici();
        $("#<%=ddlCodNatura3DG.ClientID%>").change(SetCheckBenefici);
        $("#<%=chkBenefici.ClientID%>").change(SetHiddenFieldBenefici);

        VisualizzaDataRipristino();

        if (document.getElementById("<%=HiddenAnnoBonusBooking.ClientID%>") != null && document.getElementById("<%=HiddenAnnoBonusBooking.ClientID%>").value == "SI")
            showHideAnnoBonus();

        var isRICPost20022022 = document.getElementById("<%=HiddenFieldIsRICPost20022022.ClientID%>").value;
        if (isRICPost20022022 == "SI") {
            var ControlName = document.getElementById("<%=ddlTrattINPDAP.ClientID%>");
            ControlName.remove("");
            if (ControlName.value == "NO")
                document.getElementById('<%= txtDecTrattINPDAP.ClientID %>').setAttribute("disabled", true);
        }
    });

    function SetHiddenFieldBenefici() {

        var chkbenefici = document.getElementById("<%=chkBenefici.ClientID%>");
        if (chkbenefici && chkbenefici.checked) {
            document.getElementById("<%=HiddenBenefici.ClientID%>").value = "true";
        } else {
            document.getElementById("<%=HiddenBenefici.ClientID%>").value = "false";
        }

    }

    function SetCheckBenefici() {

        var valddl = $("#<%=ddlCodNatura3DG.ClientID%>").val();
        var chkBenefici = $("#<%=chkBenefici.ClientID%>");
        var hdnOpzioneDonna = $("#<%=HiddenOpzioneDonnaLegge_197_2022.ClientID%>").val();

        if (chkBenefici) {
            if (valddl == 'G') {
                chkBenefici.attr('checked', true);
                chkBenefici.attr('disabled', true);
                $("#<%=HiddenBenefici.ClientID%>").val("true");
            }
            else {
                if (hdnOpzioneDonna == "false") {
                    chkBenefici.attr('disabled', false);
                }
            }
        }
    }

    function SetCheckBox(cb) {
        $('.offClass').val(''); //Pulisce tutti i campi con la class "offClass"
        $('.' + cb.getAttribute("EnableClass")).removeAttr('disabled'); //Abilita gli oggetti con l'attributo specificato
        if (cb.getAttribute("EnableClass") == "onClassTrasfAOI") {
            var aoiChecked = $(document.getElementById("<%=chkTrasfAOI.ClientID %>")).attr("checked");
            if (aoiChecked) {
                AbilitaTab();
            }
            else {
                DisabilitaTab();
            }
        }
    }

    function getDDLCodNatura2Value() {
        var IndexValue = document.getElementById('<%=ddlCodNatura2DG.ClientID %>').selectedIndex;
        var SelectedVal = document.getElementById('<%=ddlCodNatura2DG.ClientID %>').options[IndexValue].value;

        return SelectedVal;
    }

    function setDataInteressiLegali() {
        if (document.getElementById("<%=txtDataCompletezza.ClientID %>") != null) {
            var dataCompletezza = document.getElementById("<%=txtDataCompletezza.ClientID %>").value;

            if (dataCompletezza != "" && dataCompletezza.toUpperCase() != "GG/MM/AAAA")
                CalcolaDataInteressiLegali(dataCompletezza);
        }
    }

    function CalcolaDataInteressiLegali(dataCompletezza) {
        if (dataCompletezza != "") {
            var dataInteressiLegali = new Date();
            dataInteressiLegali = convertString2Date(dataCompletezza)
            dataInteressiLegali.setDate(dataInteressiLegali.getDate() + 121);
            var interessiLegali = convertDate2String(dataInteressiLegali);
            document.getElementById("<%=txtInteressiLegali.ClientID %>").value = interessiLegali;
        }
        else {
            document.getElementById("<%=txtInteressiLegali.ClientID %>").value = "";
        }
    }

    function SetCalendarioDataCalcolo() {
        if ($(document.getElementById("<%=txtDataCalcolo.ClientID%>")).is(':disabled') == false) {
            $(document.getElementById("<%=txtDataCalcolo.ClientID%>")).datepicker({
                changeMonth: true,
                changeYear: true,
                changeDay: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png'
            });
            //$(document.getElementById("<%=txtDataCalcolo.ClientID%>")).unmask();
            //$(document.getElementById("<%=txtDataCalcolo.ClientID%>")).mask("99/99/9999");
        }
    }

    function SetCalendarioDecorrenzaBonus() {
        if ($(document.getElementById("<%=txtDecorrenzaBonus.ClientID%>")).is(':disabled') == false) {
            $(document.getElementById("<%=txtDecorrenzaBonus.ClientID%>")).datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'mm/yy',
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                maxDate: '+100y',
                minDate: '-100y',
                //yearRange: 'c-50:' + 'c+50:',

                hideCalendar: 'ui-datepicker-calendar',
                onClose: function (dateText, inst) {
                    if (uiDPchiuso == true) {
                        var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                        var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                        $(this).datepicker('setDate', new Date(year, month, 1));
                        uiDPchiuso = false;
                    }
                }
            });
            //$(document.getElementById("<%=txtDecorrenzaBonus.ClientID%>")).unmask();
            //$(document.getElementById("<%=txtDecorrenzaBonus.ClientID%>")).mask("99/9999");
        }
    }

    function SetCalendarioRevSanitaria() {
        if ($(document.getElementById("<%=txtScadRevSanitaria.ClientID%>")).is(':disabled') == false) {
            $(document.getElementById("<%=txtScadRevSanitaria.ClientID%>")).datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'mm/yy',
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                maxDate: '+100y',
                minDate: '+0',
                hideCalendar: 'ui-datepicker-calendar',
                onClose: function (dateText, inst) {
                    if (uiDPchiuso == true) {
                        var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                        var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                        $(this).datepicker('setDate', new Date(year, month, 1));
                        uiDPchiuso = false;
                    }
                }
            });
            //$(document.getElementById("<%=txtScadRevSanitaria.ClientID%>")).unmask();
            //$(document.getElementById("<%=txtScadRevSanitaria.ClientID%>")).mask("99/9999");
        }
    }

    function onChangeCausaCarico() {
        var causaCarico = document.getElementById("<%=ddlCausaCarico.ClientID%>").value;
        if (causaCarico == 3 || causaCarico == 9) {
            AbilitaTab();
        }
        else {
            DisabilitaTab();
        }

        VisualizzaDataRipristino();
    }

    function VisualizzaDataRipristino() {
        if (document.getElementById("<%=ddlCausaCarico.ClientID%>") != null) {
            var causaCarico = document.getElementById("<%=ddlCausaCarico.ClientID%>").value;
            if (causaCarico == "9") {
                document.getElementById("TrDataRipristino").style.display = 'table-row';
            }
            else {
                document.getElementById("TrDataRipristino").style.display = 'none';
            }
        }
        else
            document.getElementById("TrDataRipristino").style.display = 'none';
    }

    function showHideAnnoBonus() {
        if (document.getElementById("<%=HiddenAnnoBonusBooking.ClientID%>") != null && document.getElementById("<%=HiddenAnnoBonusBooking.ClientID%>").value == "SI") {
            var checkBox = document.getElementById("<%= chkRichiestaBonus.ClientID %>");
            var textBox = document.getElementById("<%= txtAnnoBonus.ClientID %>");
            var label = document.getElementById("<%= lblAnnoBonus.ClientID %>");
            var hdn = document.getElementById("<%= hdnAnnoRichiestaBonus14.ClientID %>");

            if (checkBox.checked == true) {
                textBox.style.visibility = "visible";
                label.style.visibility = "visible";
                if (hdn != null && document.getElementById("<%=hdnAnnoRichiestaBonus14.ClientID%>").value != "") {
                    textBox.value = document.getElementById("<%=hdnAnnoRichiestaBonus14.ClientID%>").value;
                    textBox.setAttribute("disabled", true);
                }
            } else {
                textBox.value = "AAAA";
                textBox.style.visibility = "hidden";
                label.style.visibility = "hidden";
            }
        }
    }


    function ShowPopupTrattINPDAP() {
        CreatePopUpTrattINPDAP();
        $('#dialog-trattINPDAP').dialog('open');
    }

    function CreatePopUpTrattINPDAP() {
        var result;
        $('#dialog-trattINPDAP').dialog(
                {
                    autoOpen: false,
                    width: 400,
                    modal: true,
                    resizable: false,
                    draggable: false,

                    buttons:
                        {
                            "Si": function () {
                                $(this).dialog("close");
                                document.getElementById('<%= txtDecTrattINPDAP.ClientID %>').removeAttribute("disabled");
                                $("#<%=txtDecTrattINPDAP.ClientID%>").val('MM/AAAA');
                            },
                            "No": function () {
                                $(this).dialog("close");
                                $("#<%=txtDecTrattINPDAP.ClientID%>").val('MM/AAAA');
                                $("#<%=ddlTrattINPDAP.ClientID%>").val('NO');
                                document.getElementById('<%= txtDecTrattINPDAP.ClientID %>').setAttribute("disabled", true);
                                result = false;
                            }
                        }
                });
        $("#dialog-trattINPDAP").parent().appendTo($("form:first"));
    }

</script>
<asp:HiddenField runat="server" ID="HiddenPrecedentePensione" />
<asp:HiddenField runat="server" ID="HiddenTrattamentoDisagi" />
<asp:HiddenField runat="server" ID="HiddenAnnoBonusBooking" />
<asp:Panel runat="server" ID="pnlDatiGenerici">
    <table class="tabellaFormattazione grid grid-size-25">
        <tr>
            <td class="Row1">
                <label>
                    Decorrenza Pensione:</label>
            </td>
            <td class="field">
                <asp:Label runat="server" ID="lblDecorrenzaPensioneDatiGenerici" Text=""></asp:Label>
            </td>
            <td class="Row1"></td>
            <td class="field"></td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Codice Natura:</label>
            </td>
            <td class="field full-grid cod-nat" colspan="3">
                <asp:DropDownList runat="server" ID="ddlCodNatura1DG" Width="50px" CssClass="txtUppercase tb8 xxs"
                    TabIndex="2">
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="ddlCodNatura2DG" Width="50px" CssClass="tb8 txtUppercase xxs"
                    TabIndex="3">
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="ddlCodNatura3DG" Width="50px" CssClass="tb8 txtUppercase xxs"
                    TabIndex="4">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Accantonamento Arretrati:</label>
            </td>
            <td class="field">
                <asp:DropDownList runat="server" ID="ddlCodiciArretrati" Width="75px" CssClass="tb8 txtUppercase xxs"
                    TabIndex="5">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="8 - SI" Value="8"></asp:ListItem>
                    <asp:ListItem Text="1 - NO" Value="1"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlCodiceArretrati_RF" ControlToValidate="ddlCodiciArretrati"
                    Display="Dynamic" Enabled="true" ErrorMessage="Accantonamento arretrati: si prega di inserire il valore"
                    ValidationGroup="UCTabDatiGenerici" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
            <td class="Row1">
                <label>
                    Decorrenza Arretrati:</label>
            </td>
            <td class="field">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaArretrati"
                    Width="95px" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="6"
                    Text="mm/aaaa" MaxLength="7"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateDecorrenzaArretrati" ControlToValidate="txtDecorrenzaArretrati"
                    Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per  Decorrenza Arretrati "
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiGenerici"
                    Text="*" CssClass="field-is-required" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaArretrati" Display="Dynamic"
                    ErrorMessage="Decorrenza Arretrati: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="customCheckDataDecorrenzaArretrati" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
        <asp:Panel ID="pnlScadRevSanitaria" runat="server" Visible="false">
            <tr>
                <td class="Row1">
                    <label>
                        Data Revisione Sanitaria:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtScadRevSanitaria" Width="95px"
                        CssClass="txtUppercase tb8" TabIndex="7" Text="MM/AAAA" MaxLength="7"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateScadRevSanitaria" ControlToValidate="txtScadRevSanitaria"
                        Display="Dynamic" ErrorMessage="Inserire la data nel formato valido per Data Revisione Sanitaria"
                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiGenerici" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtScadRevSanitaria" Display="Dynamic"
                        ErrorMessage="Data Revisione Sanitaria: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                        ID="customCheckDataDataRevisioneSanitaria" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="Row1">
                <label id="lblDataCompletezza" runat="server">
                    Data Completezza:</label>
            </td>
            <td class="field">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataCompletezza" Width="95px"
                    CssClass="txtUppercase tb8 date-picker-dataCompletezza dateGGmmAAAA" TabIndex="8"
                    Text="gg/mm/aaaa" MaxLength="10"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateDataCompletezza" ControlToValidate="txtDataCompletezza"
                    Display="Dynamic" ErrorMessage="Inserire la data nel formato valido per Data Completezza"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$" ValidationGroup="UCTabDatiGenerici" />
                <asp:RequiredFieldValidator runat="server" ID="RFDataCompletezza" ControlToValidate="txtDataCompletezza"
                    Display="Dynamic" ErrorMessage="Inserire la data completezza" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataCompletezza" Display="Dynamic"
                    ErrorMessage="Data Completezza: Data inserita posteriore a quella odierna" Text="*" CssClass="field-is-required"
                    ValidationGroup="UCTabDatiGenerici" ID="customDataCompletezza" ClientValidationFunction="checkDataPostOdiernaGGMMAAAA" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataCompletezza" Display="Dynamic"
                    ErrorMessage="Data Completezza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="customCheckDataDataCompletezza" ClientValidationFunction="checkCorrettezzaData" />
            </td>
            <td class="Row1">
                <label id="lblInteressiLegali" runat="server">
                    Data Interessi Legali:</label>
            </td>
            <td class="field">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtInteressiLegali" Width="95px"
                    CssClass="txtUppercase tb8" TabIndex="9" Enabled="false"></asp:TextBox>
            </td>
        </tr>
        <tr id="TrDataRipristino">
            <td class="Row1">
                <label id="lblDecorrenzaRipristino" runat="server">
                    Decorrenza Ripristino:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataCalcolo" Width="95px"
                    CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="10" Text="mm/aaaa"
                    MaxLength="7"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="RequiredDataCalcolo" ControlToValidate="txtDataCalcolo"
                    Display="Dynamic" ErrorMessage="Inserire la data calcolo" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici" />
                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtDataCalcolo"
                    ErrorMessage="Data Calcolo in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiGenerici"
                    Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataCalcolo" Display="Dynamic"
                    ErrorMessage="Decorrenza Ripristino: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="customCheckDataDecorrenzaRipristino" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Causa Carico:</label>
            </td>
            <td class="Row1 full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlCausaCarico" Width="90%" CssClass="tb8 txtUppercase xl"
                    TabIndex="12">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="RFVddlCausaCarico" ControlToValidate="ddlCausaCarico"
                    Display="Dynamic" ErrorMessage="Causa Carico obbligatoria" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici" />
            </td>
        </tr>
        <asp:Panel ID="pnlEsenzioneFiscale" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Esenzione Fiscale:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlEsenzioneFiscale" Width="90%" CssClass="tb8 txtUppercase"
                        TabIndex="14">
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlModalitaLiquidazione" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Modalità Liquidazione:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlModalitaLiquidazione" Width="90%" CssClass="tb8 txtUppercase"
                        TabIndex="15">
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlProvvisoria" runat="server" Visible="true">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Provvisoria:</label>
                </td>
                <td class="chkField full-grid" colspan="3">
                    <asp:CheckBox runat="server" ID="chkProvvisoria" CssClass="tb8 offClass onClassProvvisoria"
                        TabIndex="22" />
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="Row1">
                <label>
                    Codice Mobilità:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlCodMobilita" Width="90%" CssClass="tb8 txtUppercase xl"
                    TabIndex="13">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Codice Domanda Ricorso</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlCodDomandaRicorso" Width="90%" CssClass="tb8 txtUppercase"
                    TabIndex="14" Enabled="false">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Cieco / Ex Combattente:</label>
            </td>
            <td class="chkField">
                <asp:CheckBox runat="server" ID="chkExCombattente" CssClass="tb8 offClass onClassExCombattente"
                    TabIndex="15" />
            </td>
            <td class="Row1" style="width: 20%">
                <label>
                    Benefici:</label>
            </td>
            <td class="chkField">
                <asp:CheckBox runat="server" ID="chkBenefici" CssClass="tb8 offClass onClassBenefici"
                    TabIndex="16" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Maggiorazioni Sociali:</label>
            </td>
            <td class="chkField" style="width: 25%">
                <asp:CheckBox runat="server" ID="chkMaggiorazioni" CssClass="tb8 offClass onClassMaggiorazioni"
                    TabIndex="17" />
            </td>
            <td colspan="2" style="width: 50%">
                <asp:Panel ID="pnlTrasformazioneAOI" runat="server" Visible="false">
                    <table style="width: 100%;">
                        <tr>
                            <td class="Row1" style="width: 50%;">
                                <label>
                                    Trasformazione di AOI:</label>
                            </td>
                            <td class="chkField" style="width: 50%;">
                                <asp:CheckBox runat="server" ID="chkTrasfAOI" CssClass="tb8 offClass onClassTrasfAOI"
                                    TabIndex="18" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
            <td style="width: 50%" class="none"></td>
        </tr>
        <tr>
            <td colspan="4" style="width: 50%">
                <asp:Panel ID="pnlRichiestaBonus" runat="server" Visible="false">
                    <table style="width: 100%;">
                        <tr>
                            <td class="Row1" style="width: 24.5%;">
                                <asp:Label ID="lblRichiestaBonus" runat="server" />
                            </td>
                            <td class="chkField" style="width: 24.5%;">
                                <asp:CheckBox runat="server" ID="chkRichiestaBonus" CssClass="tb8 offClass onClassBonus"
                                    TabIndex="19" onclick="showHideAnnoBonus()" />
                            </td>
                            <td class="Row1" style="width: 24.5%;">
                                <asp:Label ID="lblAnnoBonus" runat="server" Text="Anno Richiesta Bonus:" Style="visibility: hidden;" />
                            </td>
                            <td class="chkField" style="width: 24.5%;">
                                <asp:TextBox Style="text-align: left; visibility: hidden;" runat="server" ID="txtAnnoBonus"
                                    Width="50%" CssClass="txtUppercase tb8" TabIndex="18" Text="AAAA" MaxLength="4"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Trattenuta Fondo Credito:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList runat="server" ID="ddlTrattINPDAP" Width="50px" CssClass="txtUppercase tb8 xxs"
                    TabIndex="19">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Decorrenza Trattenuta Fondo Credito:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDecTrattINPDAP" Width="95px"
                    CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="20" Text="mm/aaaa"
                    MaxLength="7"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="txtDecTrattINPDAP"
                    Display="Dynamic" ErrorMessage="Inserire la data nel formato valido per Decorrenza Trattenuta Fondo Credito"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiGenerici" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecTrattINPDAP" Display="Dynamic"
                    ErrorMessage="Decorrenza Trattenuta Fondo Credito: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="customCheckDataDecorrenzaTrattenutaINPDAP" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Decorrenza Bonus:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaBonus" Width="95px"
                    CssClass="txtUppercase tb8 dateMMaaaa" TabIndex="21" Text="mm/aaaa" MaxLength="7"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" ControlToValidate="txtDecorrenzaBonus"
                    Display="Dynamic" ErrorMessage="Inserire la data nel formato valido per Decorrenza Bonus"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiGenerici" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaBonus" Display="Dynamic"
                    ErrorMessage="Decorrenza Bonus: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="customCheckDataDecorrenzaBonus" ClientValidationFunction="checkCorrettezzaData" />
            </td>
            <asp:Panel ID="pnlDataPrenotazione" runat="server" Visible="false">
                <td class="Row1" style="width: 25%">
                    <label>
                        Data Prenotazione:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDataPrenotazione" Width="95px"
                        Text="gg/mm/aaaa" TabIndex="22" CssClass="txtUppercase tb8 date-picker-base-maxActual dateGGmmAAAA"
                        MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ControlToValidate="txtDataPrenotazione"
                        ErrorMessage="Data Prenotazione in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiGenerici"
                        Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDataPrenotazione" Display="Dynamic"
                        ErrorMessage="Data Prenotazione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                        ID="customCheckDataDataPrenotazione" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </asp:Panel>
        </tr>
        <%--<tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Detrazioni Estero:</label>
            </td>
            <td class="chkField">
                <asp:CheckBox runat="server" ID="chkDetrazioniEstero" CssClass="tb8 offClass onClassdetrEstero"
                    TabIndex="23" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Vittime Terrorismo:</label>
            </td>
            <td class="chkField">
                <asp:CheckBox runat="server" ID="chkVittimeTerrorismo" CssClass="tb8 offClass onClassVittimeTerr"
                    TabIndex="24" />
            </td>
        </tr>--%>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Opzione Contributiva:</label>
            </td>
            <td class="chkField" colspan="3">
                <asp:CheckBox runat="server" ID="chkContributiva" CssClass="tb8 offClass onClassContributiva"
                    TabIndex="25" />
            </td>
        </tr>
        <asp:Panel ID="pnlConfermeInvalidita" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Conferme Invalidità:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlConfermeInvalidita" Width="90%" CssClass="txtUppercase tb8"
                        TabIndex="28">
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
    </table>
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaDatiGenerici" runat="server" SkinID="btnAzione1" Enabled="true"
                        Text="Salva Dati Generici" Width="170px" CausesValidation="false" OnClick="SalvaDatiGenerici_Click"
                        OnClientClick="if(Page_ClientValidate('UCTabDatiGenerici')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaDatiGenerici" runat="server" SkinID="btnAzione1" Enabled="true"
                        Text="Elimina Dati Generici" Width="170px" CausesValidation="false" OnClick="EliminaDatiGenerici_Click"
                        OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Generici?')) return false; else BlockUI();" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<div id="dialog-trattINPDAP" title="Conferma" style="display: none; border-style: none; border-color: White;">
    <p>
        <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
        E’ stata verificata presso l’area competente della D.C Credito e Welfare l’iscrizione del pensionato al Fondo credito?
    </p>
</div>
<asp:HiddenField ID="HiddenBenefici" runat="server" />
<asp:HiddenField ID="HiddenOpzioneDonnaLegge_197_2022" runat="server" Value="false" />
<asp:HiddenField runat="server" ID="hdnAnnoRichiestaBonus14" Value="" />
<asp:HiddenField runat="server" ID="HiddenFieldIsRICPost20022022" Value="" />
