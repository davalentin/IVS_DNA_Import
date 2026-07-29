<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiGenerici.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCDatiGenerici" %>
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
            yearRange: '-100:' + '+0:',
            minDate: '-100y',
            maxDate: '+0',
            onSelect: function (dateText, inst) {                
                var hdnSetDataInteressiLegaliRicCumulo = document.getElementById("<%=HdnSetDataInteressiLegaliRicCumulo.ClientID%>").value;
                if (document.getElementById("<%=txtInteressiLegali.ClientID %>") != null) {
                    document.getElementById("<%=txtInteressiLegali.ClientID %>").value = CalcolaDataInteressiLegaliNew(dateText, document.getElementById("<%=lblDecorrenzaPensioneDatiGenerici.ClientID %>").innerText, document.getElementById("<%=txtDataDomanda.ClientID %>").value, hdnSetDataInteressiLegaliRicCumulo);
                    document.getElementById("<%=HiddenIntLeg.ClientID %>").value = document.getElementById("<%=txtInteressiLegali.ClientID %>").value;
                }
            }
        });
        //$(".date-picker-dataCompletezza").unmask();
        //$(".date-picker-dataCompletezza").mask("99/99/9999");
    });

    $(document).ready(function () {
        var siglaCategoria = document.getElementById("<%= HiddenFieldSiglaCategoria.ClientID %>").value;
        var filtro = document.getElementById("<%= HiddenFieldFiltro.ClientID %>").value;

        if ((document.getElementById("<%= HiddenFieldIsRicostituzione.ClientID %>").value == "NO" || siglaCategoria == 'INDCOM') && document.getElementById("<%= HdnRemoveScadSanCalendar.ClientID %>").value == "false") {
            SetCalendarioRevSanitaria();
        }

        if (document.getElementById("<%=txtInteressiLegali.ClientID %>"))
            setDataInteressiLegali();

        var aoiChecked = $(document.getElementById("<%=chkTrasfAOI.ClientID %>")).attr("checked");
        if (aoiChecked) {
            AbilitaTab();
        }
        else {
            DisabilitaTab();
        }
        //getDDLCodComunicazioni1Value();
        getDDLCodNatura2Value();
        // gestione della visibilità della tab PrecedentePensione nel caso si provi a salvare dati generici senza il flag.
        if (document.getElementById("<%=HiddenPrecedentePensione.ClientID%>") != null && document.getElementById("<%=HiddenPrecedentePensione.ClientID%>").value == "true") {
            $(document.getElementById("<%=chkTrasfAOI.ClientID %>")).attr("checked", true);
            AbilitaTab();
        }

        VisualizzaDataRipristino();

        if (document.getElementById("<%=HiddenENPALS.ClientID%>") != null && document.getElementById("<%=HiddenENPALS.ClientID%>").value == "false")
            setDataINPDAP();


        ManageEnteCassa();
        ManageCumuloEsterno();
        SetCodiciNatura3ByCodiceNatura1();

        $(document.getElementById('<%=ddlCodNatura1DG.ClientID %>')).change(function () {
            SetCodiciNatura3ByCodiceNatura1();
        });

        if (document.getElementById("<%=HiddenAnnoBonusBooking.ClientID%>") != null && document.getElementById("<%=HiddenAnnoBonusBooking.ClientID%>").value == "SI")
            showHideAnnoBonus();

        var isRICPost20022022 = document.getElementById("<%=HiddenFieldIsRICPost20022022.ClientID%>").value;
        if (isRICPost20022022 == "SI") {
            var ControlName = document.getElementById("<%=ddlTrattINPDAP.ClientID%>");
            ControlName.remove("");
            if (ControlName.value == "NO") {
                document.getElementById('<%= txtDecTrattINPDAP.ClientID %>').setAttribute("disabled", true);
            }
        }
    });

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

    function getDDLCodNatura1Value() {
        var IndexValue = document.getElementById('<%=ddlCodNatura1DG.ClientID %>').selectedIndex;
        var SelectedVal = document.getElementById('<%=ddlCodNatura1DG.ClientID %>').options[IndexValue].value;

        return SelectedVal;
    }

    function SetCodiciNatura3ByCodiceNatura1() {
        var ddlCodNatura1Value = getDDLCodNatura1Value();
        var siglaCategoria = document.getElementById("<%= HiddenFieldSiglaCategoria.ClientID %>").value;
        if (siglaCategoria == 'VOAUT' || siglaCategoria == 'IOAUT' || siglaCategoria == 'SOAUT') {
            var terzoByte = "";
            //resetto terza combo
            $(".ddlCodNaturaClass > span > option ").unwrap().show();

            if (siglaCategoria == 'VOAUT' && ddlCodNatura1Value == "5")
                terzoByte = "V";
            else if (((siglaCategoria == 'SOAUT' || siglaCategoria == 'IOAUT') || (siglaCategoria == 'VOAUT' && ddlCodNatura1Value != "5")))
                terzoByte = "D";
            if (terzoByte.length > 0)
                $(".ddlCodNaturaClass > option:contains('" + terzoByte + "')").wrap("<span/>").hide();
        }
    }

    function setDataInteressiLegali() {
        var siglaCategoria = document.getElementById("<%= HiddenFieldSiglaCategoria.ClientID %>").value;

        // ESOTEL => la data deve essere null (quindi stringa vuota + hidden vuoto)
        if (siglaCategoria && siglaCategoria.toUpperCase() === "ESOTEL") {
            if (document.getElementById("<%=txtInteressiLegali.ClientID %>") != null)
                document.getElementById("<%=txtInteressiLegali.ClientID %>").value = "";

            if (document.getElementById("<%=HiddenIntLeg.ClientID %>") != null)
                document.getElementById("<%=HiddenIntLeg.ClientID %>").value = "";

            return;
        }

        var dataCompletezza = document.getElementById("<%=txtDataCompletezza.ClientID %>") != null ? document.getElementById("<%=txtDataCompletezza.ClientID %>").value : "";
        var dataDecorrenza = document.getElementById("<%=lblDecorrenzaPensioneDatiGenerici.ClientID %>").innerText;
        var dataDomanda = document.getElementById("<%=txtDataDomanda.ClientID %>").value;
        if (document.getElementById("<%=txtInteressiLegali.ClientID %>") != null) {
            //Questi commenti sono stati lasciati per sviluppi futuri per rendere più visibile come avviene il cambio del valore di Data Interessi Locali a FE, rispetto al valore assegnato a BE
            //console.log("Qui stiamo riassegnando il valore di Data Interessi Legali");
            //console.log("Chiamo CalcolaDataInteressiLegaliNew con:");
            //console.log("Data Completezza: ", dataCompletezza);
            //console.log("Data Decorrenza: ", dataDecorrenza);
            //console.log("Data Domanda: ", dataDomanda);
            //console.log("Data Calcolata: ", CalcolaDataInteressiLegaliNew(dataCompletezza, dataDecorrenza, dataDomanda));            
            var hdnSetDataInteressiLegaliRicCumulo = document.getElementById("<%=HdnSetDataInteressiLegaliRicCumulo.ClientID%>").value;
            document.getElementById("<%=txtInteressiLegali.ClientID %>").value = CalcolaDataInteressiLegaliNew(dataCompletezza, dataDecorrenza, dataDomanda, hdnSetDataInteressiLegaliRicCumulo);
            document.getElementById("<%=HiddenIntLeg.ClientID %>").value = document.getElementById("<%=txtInteressiLegali.ClientID %>").value;
        }
    }

    function SetCalendarioRevSanitaria() {
        //        if ($(document.getElementById("<%=pnlScadRevSanitaria.ClientID%>")).is(':disabled') == false) 
        //        {
        $(document.getElementById("<%=txtScadRevSanitaria.ClientID%>")).datepicker({
            changeMonth: true,
            changeYear: true,
            showButtonPanel: true,
            dateFormat: 'mm/yy',
            showOn: 'button',
            buttonImageOnly: true,
            buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
            maxDate: '+100y',
            minDate: '-100y',
            yearRange: '-20:' + '+10:',

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
        //        }
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

    function GetCodNatura3() {
        var codNatura3 = document.getElementById("<%=ddlCodNatura3DG.ClientID%>").value;
        return codNatura3;
    }

    function setDataINPDAP() {
        var ControlName = document.getElementById("<%=ddlTrattINPDAP.ClientID%>");
        var ControlData = document.getElementById("<%=txtDecTrattINPDAP.ClientID%>");
        var isRICPost20022022 = document.getElementById("<%=HiddenFieldIsRICPost20022022.ClientID%>").value;

        if (isRICPost20022022 != "SI") {
            if (ControlName.value == "SI" && (ControlData.value == "MM/AAAA" || ControlData.value == "")) {
                document.getElementById("<%=txtDecTrattINPDAP.ClientID%>").value = document.getElementById("<%=lblDecorrenzaPensioneDatiGenerici.ClientID%>").innerText;
            }
            else if (ControlName.value != "SI") {
                document.getElementById("<%=txtDecTrattINPDAP.ClientID%>").value = 'MM/AAAA';

            }
        }
    }

    function GetCodNatura1() {
        var codNatura1 = document.getElementById("<%=ddlCodNatura1DG.ClientID%>").value;
        return codNatura1;
    }

    function GetCodNatura2() {
        var codNatura2 = document.getElementById("<%=ddlCodNatura2DG.ClientID%>").value;
        return codNatura2;
    }

    function EnableBenefici(isBeneficiEnabled) {
        
        var benefici = document.getElementById("<%= chkBenefici.ClientID %>");
        var hdnIsRicConTerzoCodNaturaZAttEconomica67ProfIndividuale11 = document.getElementById("<%=HiddenFieldIsRicConTerzoCodNaturaZAttEconomica67ProfIndividuale11.ClientID%>").value;
        var hdnHiddenFieldMemo28_2024 = document.getElementById("<%=HiddenFieldMemo28_2024.ClientID%>").value;
        var hdnChkBeneficiChecked = document.getElementById("<%=HiddenFieldChkBeneficiChecked.ClientID%>").value;
        var hdnChkBeneficiDisabled = document.getElementById("<%=HiddenFieldChkBeneficiDisabled.ClientID%>").value;
        var hdnSaltaCheckBenefici_0001_0002_0017 = document.getElementById("<%=HiddenFieldSaltaCheckBenefici0001_0002_0017.ClientID%>").value;
        if (benefici && hdnIsRicConTerzoCodNaturaZAttEconomica67ProfIndividuale11 == "false" && hdnHiddenFieldMemo28_2024 == "false") {
            if (isBeneficiEnabled) {
                benefici.disabled = true;
                benefici.checked = true;
            }
            else {

                if (hdnSaltaCheckBenefici_0001_0002_0017 == "false") {

                if (hdnChkBeneficiDisabled == "true")
                    benefici.disabled = true;
                else
                    benefici.disabled = false;
                if (hdnChkBeneficiChecked == "true")
                    benefici.checked = true;
                //                else
                //                    benefici.checked = false;
                }
            }
            document.getElementById("<%= HiddenFieldChkBenefici.ClientID %>").value = benefici.checked;
        }
        if (hdnHiddenFieldMemo28_2024 == "true") {
            benefici.disabled = false;
            document.getElementById("<%= HiddenFieldChkBenefici.ClientID %>").value = benefici.checked;
        }

        if (hdnSaltaCheckBenefici_0001_0002_0017 == "true") {           
            document.getElementById("<%= HiddenFieldChkBenefici.ClientID %>").value = benefici.checked;
        }
    }
    function SetHdnBenefici() {
        
        var benefici = document.getElementById("<%= chkBenefici.ClientID %>");
        document.getElementById("<%= HiddenFieldChkBenefici.ClientID %>").value = benefici.checked;
        setTimeout(function () {
            SetHdnBenefici();
        }, 250);
    }

    function GetEnteCassa() {
        var enteCassa = $("#<%= ddlEnteCassa.ClientID %> :selected");
        if (enteCassa) {
            return enteCassa.text().split(' - ')[0];
        }
        return 0;
    }

    function GetCheckBenefici() {
        return document.getElementById("<%= chkBenefici.ClientID %>");
    }

    function ManageCumuloEsterno() {
        var tipoCumulo = $('#<%= ddlTipoCumulo.ClientID %>');
        var cumuloEsterno = $('#<%= ddlCumuloEsterno.ClientID %>');
        if (tipoCumulo.length > 0 && cumuloEsterno.length > 0) {
            //tipo cumulo esterno
            if (tipoCumulo.val() == 'False') {
                var siglaCategoria = document.getElementById("<%= HiddenFieldSiglaCategoria.ClientID %>").value;
                if (siglaCategoria == 'IOCUM' || siglaCategoria == 'SOCUM') {
                    BlindEnteCassaToInps(false);
                    ManageEnteCassa();
                }
                $('.tdCumuloEsterno').show();
                ValidatorEnable(document.getElementById('<%= RFV_ddlCumuloEsterno.ClientID %>'), true);
                var blind = document.getElementById("<%= HiddenFieldBlindCumuloEsterno.ClientID %>").value;
                if (blind == "true") {
                    cumuloEsterno.find('[value=""]').remove();
                    cumuloEsterno.find('[value="M"]').remove();
                    cumuloEsterno.val('E');
                    ValidatorEnable(document.getElementById('<%= RFV_ddlCumuloEsterno.ClientID %>'), false);
                }
            }
            //tipo cumulo interno
            else {
                $('.tdCumuloEsterno').hide();
                ValidatorEnable(document.getElementById('<%= RFV_ddlCumuloEsterno.ClientID %>'), false);
                cumuloEsterno.val('');
                var siglaCategoria = document.getElementById("<%= HiddenFieldSiglaCategoria.ClientID %>").value;
                if ((siglaCategoria == 'IOCUM' || siglaCategoria == 'SOCUM') && tipoCumulo.val() == 'True') {
                    BlindEnteCassaToInps(true);
                }
            }
        }
    }

    function isCumuloEsternoObbligatorio(source, args) {
        var tipoCumulo = $('#<%= ddlTipoCumulo.ClientID %>');
        if (tipoCumulo.length > 0) {
            if (tipoCumulo.val() == 'False') {
                if (args.Value == '')
                    args.isValid = false;
            }
            else
                args.isValid = true;
        }
    }

    function getTipoCalcolo() {
        return $('#<%= ddlTipoCalcolo.ClientID %>').val();
    }

    function ManageEnteCassa() {
        var HiddenFieldIsRicTfrTotCum = document.getElementById("<%= HiddenFieldIsRicTfrTotCum.ClientID %>").value;
        if (HiddenFieldIsRicTfrTotCum != "SI") {
            var isEnteEx = $('#<%= ddlEnteIstruttoreFondoExInpdap.ClientID %>');
            var blind = document.getElementById("<%= HiddenFieldBlindEnteCassa.ClientID %>").value;
            var tipoCumulo = $('#<%= ddlTipoCumulo.ClientID %>');
            if (isEnteEx.length > 0 && blind == "true") {
                var ddlEnteCassa = $('#<%= ddlEnteCassa.ClientID %>');
                if (isEnteEx.val() == "false") {
                    BlindEnteCassaToInps(false);
                }
                else {
                    BlindEnteCassaToInps(true);
                }
            }
        }
    }

    function BlindEnteCassaToInps(blind) {
        var HiddenFieldIsRicTfrTotCum = document.getElementById("<%= HiddenFieldIsRicTfrTotCum.ClientID %>").value;
        if (HiddenFieldIsRicTfrTotCum != "SI") {
            var ddlEnteCassa = $('#<%= ddlEnteCassa.ClientID %>');
            if (blind == true) {
                ddlEnteCassa.val(1);
                ddlEnteCassa.attr("disabled", true);
                $('#<%= hdnEnteCassa.ClientID %>').val(1);
                SetAttEconomicaProfIndividualeCumulo();
            }
            else {
                ddlEnteCassa.removeAttr("disabled");
                $('#<%= hdnEnteCassa.ClientID %>').val('');
                SetAttEconomicaProfIndividualeCumulo();
            }
        }
    }

    function SetAttivitaAndProfessioneCum() {
        var siglaCategoria = document.getElementById("<%= HiddenFieldSiglaCategoria.ClientID %>").value;
        if (siglaCategoria == 'IOCUM' || siglaCategoria == 'SOCUM' ||
            siglaCategoria == 'VOTOT' || siglaCategoria == 'SOTOT' || siglaCategoria == 'IOTOT') {
            var ddlCtrlEnteCassaCodiceGestione = Get_ddlCtrlEnteCassaCodiceGestione();
            var enteCassa = GetEnteCassa();
            if (enteCassa && ddlCtrlEnteCassaCodiceGestione) {
                var attivita;
                var enableAttivita;
                var professione;
                var enableProfessione;
                if (enteCassa == "0801") {
                    enableAttivita = true;
                    enableProfessione = true;
                }
                else {
                    attivita = "71";
                    if (siglaCategoria == 'VOTOT' || siglaCategoria == 'SOTOT' || siglaCategoria == 'IOTOT') {
                        enableAttivita = true;
                        enableProfessione = true;
                    }
                    else {
                        enableAttivita = false;
                        enableProfessione = false;
                    }
                    var output = ddlCtrlEnteCassaCodiceGestione.options;
                    for (var i = 0; i < output.length; i++) {
                        if (output[i].value.indexOf(enteCassa) == 0) {
                            professione = output[i].text;
                        }
                    }
                }

                SetAttivitaEconomicaAndProfessione(attivita, professione, enableAttivita, enableProfessione)
            }
        }
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
<asp:HiddenField runat="server" ID="HiddenIntLeg" />
<asp:HiddenField runat="server" ID="HiddenENPALS" />
<asp:HiddenField runat="server" ID="HiddenFieldChkBenefici" />
<asp:HiddenField runat="server" ID="HiddenFieldIsRicostituzione" />
<asp:HiddenField runat="server" ID="HiddenTrattamentoDisagi" />
<asp:HiddenField runat="server" ID="HiddenAnnoBonusBooking" />
<asp:Panel runat="server" ID="pnlDatiGenerici">
    <table class="tabellaFormattazione grid grid-size-25">
        <tr id="trMessaggioInformativo" visible="false" runat="server">
            <td class="field" colspan="4">
                <asp:Label runat="server" ID="lblMessaggioInformativo" Text="" Style="font-weight: bold" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <asp:Label runat="server" ID="lblEtichettaDecorrenzaPensioneDatiGenerici" Text="">Decorrenza Pensione:</asp:Label>
            </td>
            <td class="field" style="width: 25%">
                <asp:Label runat="server" ID="lblDecorrenzaPensioneDatiGenerici" Text=""></asp:Label>
            </td>
            <td class="Row1" style="width: 25%"></td>
            <td class="field" style="width: 25%"></td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Natura:</label>
            </td>
            <td class="field cod-nat" colspan="3">
                <asp:DropDownList runat="server" ID="ddlCodNatura1DG" Width="10%" CssClass="txtUppercase tb8 xxs"
                    TabIndex="2">
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="ddlCodNatura2DG" Width="10%" CssClass="tb8 txtUppercase xxs"
                    TabIndex="3">
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="ddlCodNatura3DG" Width="10%" CssClass="tb8 txtUppercase xxs ddlCodNaturaClass"
                    TabIndex="4">
                </asp:DropDownList>
            </td>
            <td class="shift-right-full-grid"></td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Accantonamento Arretrati:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList runat="server" ID="ddlCodiciArretrati" Width="75px" CssClass="tb8 txtUppercase xxs"
                    TabIndex="5">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="1 - NO" Value="1"></asp:ListItem>
                    <asp:ListItem Text="8 - SI" Value="8"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlCodiceArretrati_RF" ControlToValidate="ddlCodiciArretrati"
                    Display="Dynamic" Enabled="true" ErrorMessage="Accantonamento arretrati: si prega di inserire il valore"
                    ValidationGroup="UCTabDatiGenerici" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Decorrenza Arretrati:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaArretrati"
                    Width="50%" CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="6"
                    Text="MM/AAAA" MaxLength="7"></asp:TextBox>
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
                <td class="Row1" style="width: 25%">
                    <label id="lblDataRevisioneSanitaria" runat="server">
                        Data Revisione Sanitaria:</label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtScadRevSanitaria" Width="50%"
                        CssClass="txtUppercase tb8" TabIndex="7" Text="MM/AAAA" MaxLength="7"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateScadRevSanitaria" ControlToValidate="txtScadRevSanitaria"
                        Display="Dynamic" ErrorMessage="Inserire la data nel formato valido per Data Revisione Sanitaria"
                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiGenerici" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtScadRevSanitaria" Display="Dynamic"
                        ErrorMessage="Data Revisione Sanitaria: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                        ID="customCheckDataDataRevisioneSanitaria" ClientValidationFunction="checkCorrettezzaData" />
                </td>
                <td class="Row1 iframe-dnone" colspan="2"></td>
            </tr>
        </asp:Panel>
        <tr runat="server" id="trCompletezzaAndInteressiLegali">
            <td class="Row1" style="width: 25%">
                <label id="lblDataCompletezza" runat="server">
                    Data Completezza:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataCompletezza" Width="50%"
                    CssClass="txtUppercase tb8 date-picker-dataCompletezza dateGGmmAAAA" TabIndex="8"
                    Text="GG/MM/AAAA" MaxLength="10"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateDataCompletezza" ControlToValidate="txtDataCompletezza"
                    Display="Dynamic" ErrorMessage="Data Completezza: Inserire la data nel formato valido"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$" ValidationGroup="UCTabDatiGenerici" />
                <asp:RequiredFieldValidator runat="server" ID="RFDataCompletezza" ControlToValidate="txtDataCompletezza"
                    Display="Dynamic" ErrorMessage="Data Completezza: campo obbligatorio" Text="*" CssClass="field-is-required"
                    ValidationGroup="UCTabDatiGenerici" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataCompletezza" Display="Dynamic"
                    ErrorMessage="Data Completezza: Data inserita posteriore a quella odierna" Text="*" CssClass="field-is-required"
                    ValidationGroup="UCTabDatiGenerici" ID="customDataCompletezza" ClientValidationFunction="checkDataPostOdiernaGGMMAAAA" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataCompletezza" Display="Dynamic"
                    ErrorMessage="Data Completezza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="customCheckDataDataCompletezza" ClientValidationFunction="checkCorrettezzaData" />
            </td>
            <td class="Row1" style="width: 25%">
                <label id="lblInteressiLegali" runat="server">
                    Data Interessi Legali:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtInteressiLegali" Width="50%"
                    CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="9" Enabled="false"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <asp:Panel runat="server" ID="pnlTipoCalcolo" Visible="true">
                <td class="Row1" style="width: 25%">
                    <asp:Label runat="server" ID="lblTipoCalcolo" Text="">Tipo Calcolo:</asp:Label>
                </td>
                <td class="field" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlTipoCalcolo" Width="90%" CssClass="tb8 txtUppercase xl"
                        TabIndex="10">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="requiredDdlTipoCalcolo" ControlToValidate="ddlTipoCalcolo"
                        Display="Dynamic" ErrorMessage="Scegliere il tipo di calcolo" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici" />
                </td>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlTipoCalcoloCum" Visible="false">
                <td class="Row1" style="width: 25%">
                    <label>
                        Contributivo:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlContributivoCum" Width="20%" CssClass="tb8 txtUppercase xxs"
                        TabIndex="10">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="8"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="2"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="rfvDdlContributivoCum" ControlToValidate="ddlContributivoCum"
                        Display="Dynamic" ErrorMessage="Contributivo è un campo obbligatorio" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiGenerici" />
                </td>
            </asp:Panel>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Causa Carico:</label>
            </td>
            <td class="Row1 full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlCausaCarico" Width="90%" CssClass="tb8 txtUppercase xl"
                    TabIndex="11" onchange="VisualizzaDataRipristino();">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Data Domanda:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataDomanda" Width="16.5%"
                    Enabled="false" Text="GG/MM/AAAA" TabIndex="10" CssClass="txtUppercase tb8 dateGGmmAAAA"
                    MaxLength="12"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateDataDomanda" ControlToValidate="txtDataDomanda"
                    ErrorMessage="Data Domanda in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiGenerici"
                    Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataDomanda" Display="Dynamic"
                    ErrorMessage="Data Domanda: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="customCheckDataDataDomanda" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
        <tr id="TrDataRipristino">
            <td class="Row1" style="width: 25%">
                <label id="lbldata">
                    Decorrenza Ripristino:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataCalcolo" Width="50%"
                    CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="13" Text="MM/AAAA"
                    MaxLength="7"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="RequiredDataCalcolo" ControlToValidate="txtDataCalcolo"
                    Display="Dynamic" ErrorMessage="Inserire la data calcolo" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici" />
                <asp:RegularExpressionValidator runat="server" ID="validateDataCalcolo" ControlToValidate="txtDataCalcolo"
                    Display="Dynamic" ErrorMessage="Inserire la data in un formato valido per Data Ripristino"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiGenerici" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataCalcolo" Display="Dynamic"
                    ErrorMessage="Decorrenza Ripristino: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="customCheckDataDecorrenzaRipristino" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
        <%--</div>--%>
        <%--</asp:Panel>--%>
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
        <asp:Panel ID="pnlCodiceLiquidazione" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Codice Liquidazione:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:TextBox ID="txtCodiceLiquidazione" runat="server" CssClass="tb8 txtUppercase"
                        Width="10%" TabIndex="2" MaxLength="1"></asp:TextBox>
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
        <asp:Panel ID="pnlCodiceMobilita" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Codice Mobilità:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlCodMobilita" Width="90%" CssClass="tb8 txtUppercase xl"
                        TabIndex="16">
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlCodDomRicorso">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Codice Domanda Ricorso:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlCodDomRicorso" Width="90%" CssClass="txtUppercase tb8"
                        TabIndex="17" Enabled="false">
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Cieco / Ex Combattente:</label>
            </td>
            <td class="chkField" style="width: 25%">
                <asp:CheckBox runat="server" ID="chkExCombattente" CssClass="tb8 offClass onClassExCombattente"
                    TabIndex="23" />
            </td>
            <td colspan="2" style="width: 50%" class="shift-partial-grid">
                <asp:Panel ID="pnlBenefici" runat="server">
                    <table style="width: 100%;" class="tabellaFormattazione grid grid-size-25">
                        <tr>
                            <td class="Row1" style="width: 49.5%;">
                                <label>
                                    Benefici:</label>
                            </td>
                            <td class="chkField" style="width: 50.5%;">
                                <asp:CheckBox runat="server" ID="chkBenefici" CssClass="tb8 offClass onClassBenefici"
                                    TabIndex="22" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Maggiorazioni Sociali:</label>
            </td>
            <td class="chkField" style="width: 25%">
                <asp:CheckBox runat="server" ID="chkMaggiorazioni" CssClass="tb8 offClass onClassMaggiorazioni"
                    TabIndex="22" />
            </td>
            <td colspan="2" style="width: 50%">
                <asp:Panel ID="pnlTrasformazioneAOI" runat="server" Visible="false">
                    <table style="width: 100%;">
                        <tr>
                            <td class="Row1" style="width: 49.5%;">
                                <label class="font-semibold">
                                    Trasformazione di AOI:</label>
                            </td>
                            <td class="chkField" style="width: 50.5%;">
                                <asp:CheckBox runat="server" ID="chkTrasfAOI" CssClass="tb8 offClass onClassTrasfAOI"
                                    TabIndex="19" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="4" style="width: 50%" >
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
        <asp:Panel runat="server" ID="pnlINPDAP">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Trattenuta Fondo Credito:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlTrattINPDAP" Width="30%" CssClass="txtUppercase tb8 xxs"
                        TabIndex="17" onchange="setDataINPDAP()">
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
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDecTrattINPDAP" Width="50%"
                        CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="18" Text="mm/aaaa"
                        MaxLength="7"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="txtDecTrattINPDAP"
                        Display="Dynamic" ErrorMessage="Inserire la data nel formato valido per Decorrenza Trattenuta Fondo Credito"
                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiGenerici" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecTrattINPDAP" Display="Dynamic"
                        ErrorMessage="Decorrenza Trattenuta Fondo Credito: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                        ID="customCheckDataDecorrenzaTrattINPDAP" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </tr>
        </asp:Panel>
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
        <asp:Panel ID="pnlDatiGenericiCum" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Ente / Cassa:</label>
                </td>
                <td class="field">
                    <asp:DropDownList runat="server" ID="ddlEnteCassa" Width="90%" CssClass="txtUppercase tb8"
                        onchange="SetAttEconomicaProfIndividualeCumulo();">
                    </asp:DropDownList>
                    <asp:HiddenField runat="server" ID="hdnEnteCassa" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVddlEnteCassa" ControlToValidate="ddlEnteCassa"
                        Display="Dynamic" ErrorMessage="Inserire Ente / Cassa" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label id="lblEnteIstruttore" runat="server">
                        Ente istruttore fondo ex INPDAP:</label>
                </td>
                <td class="field">
                    <asp:DropDownList runat="server" ID="ddlEnteIstruttoreFondoExInpdap" Width="30%"
                        CssClass="txtUppercase tb8 xxs" TabIndex="28" onchange="ManageEnteCassa();">
                        <asp:ListItem Value="" Text=""></asp:ListItem>
                        <asp:ListItem Value="true" Text="SI"></asp:ListItem>
                        <asp:ListItem Value="false" Text="NO"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="rfvddlEnteIstruttoreFondoExInpdap"
                        ControlToValidate="ddlEnteIstruttoreFondoExInpdap" Display="Dynamic" ErrorMessage="Inserire Ente istruttore fondo ex-Inpdap"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici" />
                </td>
            </tr>
            <tr runat="server" id="trTipologiaCumulo" visible="false">
                <td class="Row1" style="width: 25%">
                    <label>
                        Tipo Cumulo:</label>
                </td>
                <td class="field">
                    <asp:DropDownList runat="server" ID="ddlTipoCumulo" Width="90%" CssClass="txtUppercase tb8"
                        onchange="ManageCumuloEsterno();">
                        <asp:ListItem Value="" Text=""></asp:ListItem>
                        <asp:ListItem Value="False" Text="ESTERNO"></asp:ListItem>
                        <asp:ListItem Value="True" Text="INTERNO"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="RFV_ddlTipoCumulo" ControlToValidate="ddlTipoCumulo"
                        Display="Dynamic" ErrorMessage="Inserire Tipo Cumulo" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici" />
                </td>
                <td class="Row1 tdCumuloEsterno" style="width: 25%">
                    <label>
                        Cumulo Esterno:</label>
                </td>
                <td class="field tdCumuloEsterno" style="display: none;">
                    <asp:DropDownList runat="server" ID="ddlCumuloEsterno" Width="90%" CssClass="txtUppercase tb8">
                        <asp:ListItem Value="" Text=""></asp:ListItem>
                        <asp:ListItem Value="E" Text="COMPLETO"></asp:ListItem>
                        <asp:ListItem Value="M" Text="INCOMPLETO"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="RFV_ddlCumuloEsterno" ControlToValidate="ddlCumuloEsterno"
                        Display="Dynamic" ErrorMessage="Inserire Cumulo Esterno" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici" />
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
        E’ stata verificata presso l’area competente della D.C Credito e Welfare l’iscrizione
        del pensionato al Fondo credito?
    </p>
</div>
<asp:HiddenField runat="server" ID="HiddenFieldSiglaCategoria" />
<asp:HiddenField runat="server" ID="HiddenFieldFiltro" />
<asp:HiddenField runat="server" ID="HiddenFieldBlindEnteCassa" Value="false" />
<asp:HiddenField runat="server" ID="HiddenFieldBlindCumuloEsterno" Value="false" />
<asp:HiddenField runat="server" ID="HdnRemoveScadSanCalendar" Value="false" />
<asp:HiddenField runat="server" ID="hdnAnnoRichiestaBonus14" Value="" />
<asp:HiddenField runat="server" ID="HiddenFieldIsRICPost20022022" Value="" />
<asp:HiddenField runat="server" ID="HiddenFieldIsRicTfrTotCum" Value="false" />
<asp:HiddenField runat="server" ID="HiddenContributivoStorico" Value="" />
<asp:HiddenField ID="HiddenFieldIsRicConTerzoCodNaturaZAttEconomica67ProfIndividuale11" runat="server" Value="false" />
<asp:HiddenField runat="server" ID="HiddenFieldChkBeneficiDisabled" Value="false" />
<asp:HiddenField runat="server" ID="HiddenFieldChkBeneficiChecked" Value="false" />
<asp:HiddenField runat="server" ID="HiddenFieldMemo28_2024" Value="false" />
<asp:HiddenField runat="server" ID="HiddenFieldSaltaCheckBenefici0001_0002_0017" Value="false" />
<asp:HiddenField runat="server" ID="HiddenField1" Value="false" />
<asp:HiddenField runat="server" ID="HdnSetDataInteressiLegaliRicCumulo" Value="false" />
