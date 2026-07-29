<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCPagamento.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.ModalitaPagamento.UCPagamento" %>
<style type="text/css">
    .fixed-dialog
    {
        position: fixed;
    }
</style>
<script type="text/javascript">
    $(document).ready(function () {

        var tipoPagamento = document.getElementById("<%=tipoPagamento.ClientID %>").value;
        var checkBanca = document.getElementById("<%= rdbBanca.ClientID %>").getAttribute("checked");
        var checkPosta = document.getElementById("<%= rdbPosta.ClientID %>").getAttribute("checked");
        var checkEstero = document.getElementById("<%= rdbEstero.ClientID %>").getAttribute("checked");
        var checkCassaSede = document.getElementById("<%= rdbCassaSede.ClientID %>").getAttribute("checked");
        CreatePopUp();
        if ((checkBanca == "checked") || (tipoPagamento == "B")) {                     //gestione visualizzazione pannello banca
            document.getElementById("<%=tipoPagamento.ClientID %>").value = "B";
            ShowBanca();
        }
        else if ((checkPosta == "checked") || (tipoPagamento == "P")) {                //gestione visualizzazione pannello posta
            tipoPagamento = "P"
            ShowPosta();
        }
        else if ((checkEstero == "checked") || (tipoPagamento == "E")) {               //gestione visualizzazione pannello estero
            tipoPagamento = "E"
            ShowEstero();
        }
        else if ((checkCassaSede == "checked") || (tipoPagamento == "C")) {            //gestione visualizzazione pannello cassa sede
            tipoPagamento = "C"
            ShowCassaSede();
        }
        else if (checkBanca == "" && checkPosta == "" && checkEstero == "" && checkCassaSede == "")  //primo caricamento
            ShowMain();
        else
            ShowMain();

        if (document.getElementById('<%=showPopUp.ClientID%>').value == "BS") {
            CreatePopUp();
            CleanForm();
            $('#dialogSportelloBanca').dialog('open');
        }
        else if (document.getElementById('<%=showPopUp.ClientID%>').value == "BC") {
            CreatePopUp();
            CleanForm();
            $('#dialogCCBanca').dialog('open');

        }
        else if (document.getElementById('<%=showPopUp.ClientID%>').value == "BL") {
            CreatePopUp();
            CleanForm();
            $('#dialogLibrettoBanca').dialog('open');
        }
        else if (document.getElementById('<%=showPopUp.ClientID%>').value == "BK") {
            CreatePopUp();
            CleanForm();
            $('#dialogPrepagataBanca').dialog('open');
        }
        else if (document.getElementById('<%=showPopUp.ClientID%>').value == "PS") {
            CreatePopUp();
            CleanForm();
            $('#dialogSportelloPosta').dialog('open');
        }
        else if (document.getElementById('<%=showPopUp.ClientID%>').value == "PC") {
            CreatePopUp();
            CleanForm();
            $('#dialogCCPosta').dialog('open');
        }
        else if (document.getElementById('<%=showPopUp.ClientID%>').value == "PL") {
            CreatePopUp();
            CleanForm();
            $('#dialogLibrettoPosta').dialog('open');
        }
        else if (document.getElementById('<%=showPopUp.ClientID%>').value == "PX") {
            CreatePopUp();
            CleanForm();
            $('#dialogCircPosta').dialog('open');
        }
        else if (document.getElementById('<%=showPopUp.ClientID%>').value == "PK") {
            CreatePopUp();
            CleanForm();
            $('#dialogPrepagataPosta').dialog('open');
        }

        else if (document.getElementById('<%=showPopUp.ClientID%>').value == "ES") {
            CreatePopUp();
            CleanForm();
            $('#dialogStatoEstero').dialog('open');
        }
        else if (document.getElementById('<%=showPopUp.ClientID%>').value == "EC") {
            CreatePopUp();
            CleanForm();
            $('#dialogCCEstero').dialog('open');
        }
        else if (document.getElementById('<%=showPopUp.ClientID%>').value == "EA") {
            CreatePopUp();
            CleanForm();
            $('#dialogStatoEstero').dialog('open');
        }
        else if (document.getElementById('<%=showPopUp.ClientID%>').value == "CS") {
            CreatePopUp();
            CleanForm();
            $('#dialogSportelloCassaSede').dialog('open');
        }
    });

    function SetRadio(rb) {

        $('input:radio').attr('checked', false); //Disabilita tutti i radio button
        $('.offClass').val(''); //Pulisce tutti i campi con la class "offClass"
        $('.' + rb.getAttribute("EnableClass")).removeAttr('disabled'); //Abilita gli oggetti con l'attributo specificato
        if (rb.getAttribute("EnableClass") == "onClassBanca") {
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "B";
            ShowBanca();
        }
        else if (rb.getAttribute("EnableClass") == "onClassPosta") {
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "P";
            ShowPosta();
        }
        else if (rb.getAttribute("EnableClass") == "onClassEstero") {
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "E";
            ShowEstero();
        }
        else if (rb.getAttribute("EnableClass") == "onClassCassaSede") {
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "C";
            ShowCassaSede();
        }
        else if (rb.getAttribute("EnableClass") == "onClassSportelloBanca") {
            document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value = document.getElementById('<%=modPagamentoB.ClientID %>').value;
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "B";
            document.getElementById('<%=modPagamentoB.ClientID %>').value = "S";
            ShowBanca();
            CleanForm();
            $('#dialogSportelloBanca').dialog('open');
        }
        else if (rb.getAttribute("EnableClass") == "onClassContoCorrenteBanca") {
            document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value = document.getElementById('<%=modPagamentoB.ClientID %>').value;
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "B";
            document.getElementById('<%=modPagamentoB.ClientID %>').value = "C";
            ShowBanca();
            CleanForm();
            $('#dialogCCBanca').dialog('open');
        }
        else if (rb.getAttribute("EnableClass") == "onClassLibrettoRisparmioBanca") {
            document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value = document.getElementById('<%=modPagamentoB.ClientID %>').value;
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "B";
            document.getElementById('<%=modPagamentoB.ClientID %>').value = "L";
            ShowBanca();
            CleanForm();
            $('#dialogLibrettoBanca').dialog('open');
        }
        else if (rb.getAttribute("EnableClass") == "onClassPrepagataBanca") {
            document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value = document.getElementById('<%=modPagamentoB.ClientID %>').value;
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "B";
            document.getElementById('<%=modPagamentoB.ClientID %>').value = "K";
            ShowBanca();
            CleanForm();
            $('#dialogPrepagataBanca').dialog('open');
        }
        else if (rb.getAttribute("EnableClass") == "onClassSportelloPosta") {
            document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value = document.getElementById('<%=modPagamentoP.ClientID %>').value;
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "P";
            document.getElementById('<%=modPagamentoP.ClientID %>').value = "S";
            ShowPosta();
            CleanForm();
            $('#dialogSportelloPosta').dialog('open');
        }
        else if (rb.getAttribute("EnableClass") == "onClassCCPosta") {
            document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value = document.getElementById('<%=modPagamentoP.ClientID %>').value;
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "P";
            document.getElementById('<%=modPagamentoP.ClientID %>').value = "C";
            ShowPosta();
            CleanForm();
            $('#dialogCCPosta').dialog('open');
        }
        else if (rb.getAttribute("EnableClass") == "onClassLibrettoPosta") {
            document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value = document.getElementById('<%=modPagamentoP.ClientID %>').value;
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "P";
            document.getElementById('<%=modPagamentoP.ClientID %>').value = "L";
            ShowPosta();
            CleanForm();
            $('#dialogLibrettoPosta').dialog('open');
        }
        else if (rb.getAttribute("EnableClass") == "onClassCircPosta") {
            document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value = document.getElementById('<%=modPagamentoP.ClientID %>').value;
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "P";
            document.getElementById('<%=modPagamentoP.ClientID %>').value = "X";
            ShowPosta();
            CleanForm();
            $('#dialogCircPosta').dialog('open');
        }
        else if (rb.getAttribute("EnableClass") == "onClassPrepagataPosta") {
            document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value = document.getElementById('<%=modPagamentoP.ClientID %>').value;
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "P";
            document.getElementById('<%=modPagamentoP.ClientID %>').value = "K";
            ShowPosta();
            CleanForm();
            $('#dialogPrepagataPosta').dialog('open');
        }

        else if (rb.getAttribute("EnableClass") == "onClassSportelloE") {
            document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value = document.getElementById('<%=modPagamentoE.ClientID %>').value;
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "E";
            document.getElementById('<%=modPagamentoE.ClientID %>').value = "S";
            ShowEstero();
            CleanForm();
            $('#dialogStatoEstero').dialog('open');

        }
        else if (rb.getAttribute("EnableClass") == "onClassContoCorrenteE") {
            document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value = document.getElementById('<%=modPagamentoE.ClientID %>').value;
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "E";
            document.getElementById('<%=modPagamentoE.ClientID %>').value = "C";
            ShowEstero();
            CleanForm();
            $('#dialogCCEstero').dialog('open');
        }
        else if (rb.getAttribute("EnableClass") == "onClassAssegnoE") {
            document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value = document.getElementById('<%=modPagamentoE.ClientID %>').value;
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "E";
            document.getElementById('<%=modPagamentoE.ClientID %>').value = "A";
            ShowEstero();
            CleanForm();
            $('#dialogStatoEstero').dialog('open');
        }
        else if (rb.getAttribute("EnableClass") == "onClassSportelloCassaSede") {
            document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value = document.getElementById('<%=modPagamentoC.ClientID %>').value;
            document.getElementById('<%=tipoPagamento.ClientID %>').value = "C";
            document.getElementById('<%=modPagamentoC.ClientID %>').value = "P";
            ShowCassaSede();
            CleanForm();
            $('#dialogSportelloCassaSede').dialog('open');
        }
        rb.checked = true;  //Seleziona il radioButton che ha scatenato l'evento
    }

    function ShowMain() {
        document.getElementById("<%=panMain.ClientID %>").style.display = 'block';
        document.getElementById("<%=panTipo.ClientID %>").style.display = 'block';
        document.getElementById("<%=panBanca.ClientID %>").style.display = 'none';
        document.getElementById("<%=panPosta.ClientID %>").style.display = 'none';
        document.getElementById("<%=panEstero.ClientID %>").style.display = 'none';
        document.getElementById("<%=panCassaSede.ClientID %>").style.display = 'none';
        var doAction = false;
        var cssClass;
        var tipoPagamento = document.getElementById("<%=tipoPagamento.ClientID%>").value;
        $('.offClass').val('');
        $('input:radio').attr('checked', false);
        if (doAction) {
            $(cssClass).removeAttr('disabled');
            SwitchValidator(cssClass, true);
        }
    }

    function ShowBanca() {

        document.getElementById("<%=panMain.ClientID %>").style.display = 'block';
        document.getElementById("<%=panTipo.ClientID %>").style.display = 'block';
        document.getElementById("<%=panBanca.ClientID %>").style.display = 'block';
        document.getElementById("<%=panPosta.ClientID %>").style.display = 'none';
        document.getElementById("<%=panEstero.ClientID %>").style.display = 'none';
        document.getElementById("<%=panCassaSede.ClientID %>").style.display = 'none';
        var modPagamentoB = document.getElementById("<%=modPagamentoB.ClientID %>").value;
        $(document.getElementById("<%=rdbBanca.ClientID %>")).attr("checked", true);
        //rdbBanca
        var checkCCBanca = document.getElementById("<%= rdbContoCorrenteBanca.ClientID %>").getAttribute("checked");
        var checkLibrettoBanca = document.getElementById("<%= rdbLibrettoRisparmioBanca.ClientID %>").getAttribute("checked");
        var checkSportelloBanca = document.getElementById("<%= rdbPagamSportelloBanca.ClientID %>").getAttribute("checked");
        var checkPrepagataBanca = document.getElementById("<%= rdbPrepagataBanca.ClientID %>").getAttribute("checked");
        if (/*checkCCBanca == "checked" ||*/modPagamentoB == "C")
            $(document.getElementById("<%=rdbContoCorrenteBanca.ClientID %>")).attr("checked", true);
        else if (/*checkLibrettoBanca == "checked" ||*/modPagamentoB == "L")
            $(document.getElementById("<%=rdbLibrettoRisparmioBanca.ClientID %>")).attr("checked", true);
        else if (/*checkSportelloBanca == "checked" ||*/modPagamentoB == "S")
            $(document.getElementById("<%=rdbPagamSportelloBanca.ClientID %>")).attr("checked", true);
        else if (/*checkPrepagataBanca == "checked" ||*/modPagamentoB == "K")
            $(document.getElementById("<%=rdbPrepagataBanca.ClientID %>")).attr("checked", true);
        document.getElementById("<%=tipoPagamento.ClientID %>").value = "B";
        return;
    }

    function ShowPosta() {

        document.getElementById("<%=panMain.ClientID %>").style.display = 'block';
        document.getElementById("<%=panTipo.ClientID %>").style.display = 'block';
        document.getElementById("<%=panBanca.ClientID %>").style.display = 'none';
        document.getElementById("<%=panPosta.ClientID %>").style.display = 'block';
        document.getElementById("<%=panEstero.ClientID %>").style.display = 'none';
        document.getElementById("<%=panCassaSede.ClientID %>").style.display = 'none';
        $(document.getElementById("<%=rdbPosta.ClientID %>")).attr("checked", true);
        var modPagamentoP = document.getElementById("<%=modPagamentoP.ClientID %>").value;
        //rdbPosta
        var checkSportelloPosta = document.getElementById("<%= rdbPagPostSportello.ClientID %>").getAttribute("checked");
        var checkLibrettoPosta = document.getElementById("<%= rdbPagPostLibretto.ClientID %>").getAttribute("checked");
        var checkCCPosta = document.getElementById("<%= rdbPagPostContoCorr.ClientID %>").getAttribute("checked");
        var checkPrepagataPosta = document.getElementById("<%= rdbPagPostPrepagata.ClientID %>").getAttribute("checked");

        if (document.getElementById("<%= rdbPagPostCircolarita.ClientID %>") != null)
            var checkCircPosta = document.getElementById("<%= rdbPagPostCircolarita.ClientID %>").getAttribute("checked");

        if (/*checkCircPosta == "checked" ||*/modPagamentoP == "X")
            $(document.getElementById("<%=rdbPagPostCircolarita.ClientID %>")).attr("checked", true);
        else if (/*checkSportelloPosta == "checked" ||*/modPagamentoP == "S")
            $(document.getElementById("<%=rdbPagPostSportello.ClientID %>")).attr("checked", true);
        else if (/*checkLibrettoPosta == "checked" ||*/modPagamentoP == "L")
            $(document.getElementById("<%=rdbPagPostLibretto.ClientID %>")).attr("checked", true);
        else if (/*checkCCPosta == "checked" ||*/modPagamentoP == "C")
            $(document.getElementById("<%=rdbPagPostContoCorr.ClientID %>")).attr("checked", true);
        else if (/*checkPrepagataPosta == "checked" ||*/modPagamentoP == "K")
            $(document.getElementById("<%=rdbPagPostPrepagata.ClientID %>")).attr("checked", true);
    }

    function ShowEstero() {
        document.getElementById("<%=panMain.ClientID %>").style.display = 'block';
        document.getElementById("<%=panTipo.ClientID %>").style.display = 'block';
        document.getElementById("<%=panBanca.ClientID %>").style.display = 'none';
        document.getElementById("<%=panPosta.ClientID %>").style.display = 'none';
        document.getElementById("<%=panEstero.ClientID %>").style.display = 'block';
        document.getElementById("<%=panCassaSede.ClientID %>").style.display = 'none';
        document.getElementById("<%=SelezioneStatoEstero.ClientID %>").style.display = 'block';
        var modPagamentoE = document.getElementById("<%=modPagamentoE.ClientID %>").value;
        $(document.getElementById("<%=rdbEstero.ClientID %>")).attr("checked", true);
        //rdbEstero
        var checkSportelloE = document.getElementById("<%= rdbSportelloE.ClientID %>").getAttribute("checked");
        var checkAssegnoE = document.getElementById("<%= rdbAssegnoE.ClientID %>").getAttribute("checked");
        var checkCCE = document.getElementById("<%= rdbContoCorrenteE.ClientID %>").getAttribute("checked");
        if (checkSportelloE == "checked" || modPagamentoE == "S") {
            $(document.getElementById("<%=rdbSportelloE.ClientID %>")).attr("checked", true);
            document.getElementById("<%=SelezioneStatoEstero.ClientID %>").style.display = 'block';
        }
        else if (checkAssegnoE == "checked" || modPagamentoE == "A") {
            $(document.getElementById("<%=rdbAssegnoE.ClientID %>")).attr("checked", true);
            document.getElementById("<%=SelezioneStatoEstero.ClientID %>").style.display = 'block';
        }
        else if (checkCCE == "checked" || modPagamentoE == "C") {
            $(document.getElementById("<%=rdbContoCorrenteE.ClientID %>")).attr("checked", true);
        }
    }

    function ShowCassaSede() {
        document.getElementById("<%=panMain.ClientID %>").style.display = 'block';
        document.getElementById("<%=panTipo.ClientID %>").style.display = 'block';
        document.getElementById("<%=panBanca.ClientID %>").style.display = 'none';
        document.getElementById("<%=panPosta.ClientID %>").style.display = 'none';
        document.getElementById("<%=panEstero.ClientID %>").style.display = 'none';
        document.getElementById("<%=panCassaSede.ClientID %>").style.display = 'block';
        var modPagamentoC = document.getElementById("<%=modPagamentoC.ClientID %>").value;
        $(document.getElementById("<%=rdbCassaSede.ClientID %>")).attr("checked", true);
        //rdbCassaSede
        var checkSportelloSede = document.getElementById("<%= rdbPagamSportelloCassaSede.ClientID %>").getAttribute("checked");

        //Aggiunta gestione per apertura popup cassa sede nel caso in cui non ci sono dati salvati a DB
        if (checkSportelloSede == "" && modPagamentoC == "") {
            CleanForm();
            $('#dialogSportelloCassaSede').dialog('open');
            $(document.getElementById("<%=rdbPagamSportelloCassaSede.ClientID %>")).attr("checked", true);
            document.getElementById("<%=modPagamentoC.ClientID %>").value = "P";
        }
        else {
            if (checkSportelloSede == "checked" || modPagamentoC == "P")
                $(document.getElementById("<%=rdbPagamSportelloCassaSede.ClientID %>")).attr("checked", true);
        }
        document.getElementById("<%=tipoPagamento.ClientID %>").value = "C";
        return;
    }

    function SwitchValidator(cssClass, onOff) {
        for (i = 0; i < $(cssClass).length; i++) {
            var control = $(cssClass)[i]
            var validatorid = control.id;
            val = document.getElementById(validatorid);
            if (val != null && val != 'undefined') {
                var s = val.id;
                if (s.indexOf("Validator") != -1) {
                    ValidatorEnable(val, onOff);
                }
            }
        }
    }

    function CheckValidator() {
        for (i = 0; i < $('input:radio').length; i++) {
            var control = $('input:radio')[i]
            if (control.checked) {
                SwitchValidator('.' + control.getAttribute("EnableClass"), true);
            }
        }
    }

    //Gestione Finestre di popup
    function CreatePopUp() {
        //-------------------- Banca ----------------------------------

        WireAutoTab('<%=txtAbiSportelloBanca.ClientID %>', '<%=txtCabSportelloBanca.ClientID %>', 5);
        CustomAutoTab('<%=txtCabSportelloBanca.ClientID %>', 5);
        $('#dialogSportelloBanca').dialog({
            autoOpen: false,

            show: 'blind',
            hide: 'blind',
            height: 300,
            width: 300,
            modal: true,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');


                },
                'Conferma': function () {
                    if (Page_ClientValidate("validDialogSportelloBanca")) {
                        document.getElementById('<%=paramRicerca1.ClientID%>').value = document.getElementById('<%=txtAbiSportelloBanca.ClientID %>').value;
                        document.getElementById('<%=paramRicerca2.ClientID%>').value = document.getElementById('<%=txtCabSportelloBanca.ClientID %>').value;
                        $(this).dialog('close');
                        document.getElementById('<%= btnConfermaSportelloBanca.ClientID %>').click();
                    }
                }
            },
            close: function () {
                ClosePopUp("B");

            }
        });

        WireAutoTab('<%=txtIbanCCBanca.ClientID %>', '<%=txtBicCCBanca.ClientID %>', 27);
        CustomAutoTab('<%=txtBicCCBanca.ClientID %>', 11);
        $('#dialogCCBanca').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            height: 350,
            width: 370,
            modal: true,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');

                },
                'Conferma': function () {
                    if (Page_ClientValidate("validDialogCCBanca")) {
                        document.getElementById('<%=paramRicerca1.ClientID %>').value = document.getElementById('<%=txtIbanCCBanca.ClientID %>').value;
                        document.getElementById('<%=paramRicerca2.ClientID %>').value = document.getElementById('<%=txtBicCCBanca.ClientID %>').value;
                        $(this).dialog('close');
                        document.getElementById('<%= btnConfermaCCBanca.ClientID %>').click();
                    }
                }
            },
            close: function () {
                ClosePopUp("B");
            }
        });

        CustomAutoTab('<%=txtIbanLibrettoBanca.ClientID %>', 27);
        $('#dialogLibrettoBanca').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            height: 300,
            width: 370,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            modal: true,
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');
                },
                'Conferma': function () {
                    if (Page_ClientValidate("validDialogLibrettoBanca")) {
                        document.getElementById('<%=paramRicerca1.ClientID %>').value = document.getElementById('<%=txtIbanLibrettoBanca.ClientID %>').value;
                        $(this).dialog('close');
                        document.getElementById('<%= btnConfermaLibrettoBanca.ClientID %>').click();
                    }
                }
            },
            close: function () {
                ClosePopUp("B");
            }

        });

        WireAutoTab('<%=txtIbanPrepagataBanca.ClientID %>', '<%=txtBicPrepagataBanca.ClientID %>', 27);
        CustomAutoTab('<%=txtBicPrepagataBanca.ClientID %>', 11);
        $('#dialogPrepagataBanca').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            height: 350,
            width: 370,
            modal: true,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');

                },
                'Conferma': function () {
                    if (Page_ClientValidate("validDialogPrepagataBanca")) {
                        document.getElementById('<%=paramRicerca1.ClientID %>').value = document.getElementById('<%=txtIbanPrepagataBanca.ClientID %>').value;
                        document.getElementById('<%=paramRicerca2.ClientID %>').value = document.getElementById('<%=txtBicPrepagataBanca.ClientID %>').value;
                        $(this).dialog('close');
                        document.getElementById('<%= btnConfermaPrepagataBanca.ClientID %>').click();
                    }
                }
            },
            close: function () {
                ClosePopUp("B");
            }
        });
        //-------------------- Posta ----------------------------------

        WireAutoTab('<%=txtAbiSportelloPosta.ClientID %>', '<%=txtFrazionarioSportelloPosta.ClientID %>', 5);
        CustomAutoTab('<%=txtFrazionarioSportelloPosta.ClientID %>', 5);
        $('#dialogSportelloPosta').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            height: 300,
            width: 300,
            modal: true,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');

                },
                'Conferma': function () {
                    if (Page_ClientValidate("validDialogSportelloPosta")) {
                        document.getElementById('<%=paramRicerca1.ClientID%>').value = document.getElementById('<%=txtAbiSportelloPosta.ClientID %>').value;
                        document.getElementById('<%=paramRicerca2.ClientID%>').value = document.getElementById('<%=txtFrazionarioSportelloPosta.ClientID %>').value;
                        $(this).dialog('close');
                        document.getElementById('<%= btnRicercaSportelloPosta.ClientID %>').click();
                    }
                }
            },
            close: function () {
                ClosePopUp("P");
            }

        });


        WireAutoTab('<%=txtIbanCCPosta.ClientID %>', '<%=txtFrazionarioCCPosta.ClientID %>', 27);
        CustomAutoTab('<%=txtFrazionarioCCPosta.ClientID %>', 5);
        $('#dialogCCPosta').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            height: 350,
            width: 370,
            modal: true,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');
                },
                'Conferma': function () {
                    if (Page_ClientValidate("validDialogCCPosta")) {
                        document.getElementById('<%=paramRicerca1.ClientID %>').value = document.getElementById('<%=txtIbanCCPosta.ClientID %>').value;
                        document.getElementById('<%=paramRicerca2.ClientID %>').value = document.getElementById('<%=txtFrazionarioCCPosta.ClientID %>').value;
                        $(this).dialog('close');
                        document.getElementById('<%= btnRicercaCCPosta.ClientID %>').click();
                    }
                }
            },
            close: function () {
                ClosePopUp("P");
            }

        });

        WireAutoTab('<%=txtIbanLibrettoPosta.ClientID %>', '<%=txtFrazionarioLibrettoPosta.ClientID %>', 27);
        CustomAutoTab('<%=txtFrazionarioLibrettoPosta.ClientID %>', 7);
        $('#dialogLibrettoPosta').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            height: 350,
            width: 370,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            modal: true,
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');
                },
                'Conferma': function () {
                    if (Page_ClientValidate("validDialogLibrettoPosta")) {
                        document.getElementById('<%=paramRicerca1.ClientID %>').value = document.getElementById('<%=txtIbanLibrettoPosta.ClientID %>').value;
                        document.getElementById('<%=paramRicerca2.ClientID %>').value = document.getElementById('<%=txtFrazionarioLibrettoPosta.ClientID %>').value;
                        $(this).dialog('close');
                        document.getElementById('<%= btnRicercaLibrettoPosta.ClientID %>').click();
                    }
                }
            },
            close: function () {
                ClosePopUp("P");
            }

        });

        WireAutoTab('<%=txtIbanCCPosta.ClientID %>', '<%=txtFrazionarioCCPosta.ClientID %>', 27);
        CustomAutoTab('<%=txtFrazionarioCCPosta.ClientID %>', 27);
        $('#dialogCCPosta').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            height: 350,
            width: 370,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            modal: true,
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');
                },
                'Conferma': function () {
                    if (Page_ClientValidate("validDialogCCPosta")) {
                        document.getElementById('<%=paramRicerca1.ClientID %>').value = document.getElementById('<%=txtIbanCCPosta.ClientID %>').value;
                        document.getElementById('<%=paramRicerca2.ClientID %>').value = document.getElementById('<%=txtFrazionarioCCPosta.ClientID %>').value;
                        $(this).dialog('close');
                        document.getElementById('<%= btnRicercaCCPosta.ClientID %>').click();
                    }
                }
            },
            close: function () {
                ClosePopUp("P");
            }
        });

        WireAutoTab('<%=txtAbiCircPosta.ClientID %>', '<%=txtFrazionarioCircPosta.ClientID %>', 7);
        CustomAutoTab('<%=txtFrazionarioCircPosta.ClientID %>', 7);
        $('#dialogCircPosta').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            height: 300,
            width: 300,
            modal: true,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');
                },
                'Conferma': function () {
                    if (Page_ClientValidate("validDialogCircPosta")) {
                        document.getElementById('<%=paramRicerca1.ClientID%>').value = document.getElementById('<%=txtAbiCircPosta.ClientID %>').value;
                        document.getElementById('<%=paramRicerca2.ClientID%>').value = document.getElementById('<%=txtFrazionarioCircPosta.ClientID %>').value;
                        $(this).dialog('close');
                        document.getElementById('<%= btnRicercaCircPosta.ClientID %>').click();
                    }
                }
            },
            close: function () {
                ClosePopUp("P");

            }
        });

        WireAutoTab('<%=txtIbanPrepagataPosta.ClientID %>', '<%=txtFrazionarioPrepagataPosta.ClientID %>', 27);
        CustomAutoTab('<%=txtFrazionarioPrepagataPosta.ClientID %>', 27);
        $('#dialogPrepagataPosta').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            height: 350,
            width: 370,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            modal: true,
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');
                },
                'Conferma': function () {
                    if (Page_ClientValidate("validDialogPrepagataPosta")) {
                        document.getElementById('<%=paramRicerca1.ClientID %>').value = document.getElementById('<%=txtIbanPrepagataPosta.ClientID %>').value;
                        document.getElementById('<%=paramRicerca2.ClientID %>').value = document.getElementById('<%=txtFrazionarioPrepagataPosta.ClientID %>').value;
                        $(this).dialog('close');
                        document.getElementById('<%= btnRicercaPrepagataPosta.ClientID %>').click();
                    }
                }
            },
            close: function () {
                ClosePopUp("P");
            }
        });

        //-------------------- Estero ----------------------------------

        WireAutoTab('<%=txtIbanCCEstero.ClientID %>', '<%=txtBicCCEstero.ClientID %>', 34);
        CustomAutoTab('<%=txtBicCCEstero.ClientID %>', 11);

        $('#dialogCCEstero').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            height: 370,
            width: 370,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            modal: true,
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');
                },
                'Conferma': function () {
                    if (Page_ClientValidate("validDialogCCEstero")) {
                        document.getElementById('<%=paramRicerca1.ClientID %>').value = document.getElementById('<%=txtIbanCCEstero.ClientID %>').value;
                        document.getElementById('<%=paramRicerca2.ClientID %>').value = document.getElementById('<%=txtBicCCEstero.ClientID %>').value;
                        var ddlCCStatoEstero = document.getElementById('<%=ddlStatoEsteroCCEstero.ClientID %>');
                        var descrizioneStato2 = ddlCCStatoEstero.options[ddlCCStatoEstero.selectedIndex].text;
                        document.getElementById('<%=paramRicerca3.ClientID %>').value = descrizioneStato2;
                        $(this).dialog('close');
                        document.getElementById('<%= btnRicercaCCEstero.ClientID %>').click();
                    }
                }
            },
            close: function () {
                ClosePopUp("E");
            }

        });

        $('#dialogStatoEstero').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            height: 350,
            width: 370,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            modal: true,
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');
                },
                'Conferma': function () {
                    if (Page_ClientValidate("validDialogStatoEstero")) {
                        var myDdl = document.getElementById('<%=ddlStatoEstero.ClientID %>');
                        var descrizioneStato = myDdl.options[myDdl.selectedIndex].text;
                        document.getElementById('<%=paramRicerca1.ClientID %>').value = descrizioneStato;
                        $(this).dialog('close');
                        document.getElementById('<%= btnRicercaStatoEstero.ClientID %>').click();
                    }
                }
            },
            close: function () {
                ClosePopUp("E");
            }

        });

        //-------------------- Cassa ----------------------------------

        WireAutoTab('<%=txtAbiSportelloCassaSede.ClientID %>', '<%=ddlCassaSede.ClientID %>', 7);
        CustomAutoTab('<%=ddlCassaSede.ClientID %>', 7);
        $('#dialogSportelloCassaSede').dialog({
            autoOpen: false,

            show: 'blind',
            hide: 'blind',
            height: 300,
            width: 700,
            modal: true,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');
                },
                'Conferma': function () {
                    if (Page_ClientValidate("validdialogSportelloCassaSede")) {
                        document.getElementById('<%=paramRicerca1.ClientID%>').value = document.getElementById('<%=txtAbiSportelloCassaSede.ClientID %>').value;
                        var myDdl = document.getElementById('<%=ddlCassaSede.ClientID %>');
                        var descrizioneCassaSede = myDdl.options[myDdl.selectedIndex].value;
                        document.getElementById('<%=paramRicerca2.ClientID %>').value = descrizioneCassaSede;
                        $(this).dialog('close');
                        document.getElementById('<%= btnConfermaSportelloSede.ClientID %>').click();
                    }
                }
            },
            close: function () {
                ClosePopUp("C");

            }
        });
    }

    function ClosePopUp(tipo) {
        if (tipo == "B") {
            document.getElementById('<%=modPagamentoB.ClientID %>').value = document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value;
            if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "C")
                $(document.getElementById("<%=rdbContoCorrenteBanca.ClientID %>")).attr("checked", true);
            else if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "S")
                $(document.getElementById("<%=rdbPagamSportelloBanca.ClientID %>")).attr("checked", true);
            else if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "L")
                $(document.getElementById("<%=rdbLibrettoRisparmioBanca.ClientID %>")).attr("checked", true);
            else if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "K")
                $(document.getElementById("<%=rdbPrepagataBanca.ClientID %>")).attr("checked", true);
            else if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "") {
                $(document.getElementById("<%=rdbContoCorrenteBanca.ClientID %>")).attr("checked", false);
                $(document.getElementById("<%=rdbLibrettoRisparmioBanca.ClientID %>")).attr("checked", false);
                $(document.getElementById("<%=rdbPagamSportelloBanca.ClientID %>")).attr("checked", false);
                $(document.getElementById("<%=rdbPrepagataBanca.ClientID %>")).attr("checked", false);
            }
        }
        else if (tipo == "P") {
            document.getElementById('<%=modPagamentoP.ClientID %>').value = document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value;
            if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "C")
                $(document.getElementById("<%=rdbPagPostContoCorr.ClientID %>")).attr("checked", true);
            else if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "S")
                $(document.getElementById("<%=rdbPagPostSportello.ClientID %>")).attr("checked", true);
            else if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "L")
                $(document.getElementById("<%=rdbPagPostLibretto.ClientID %>")).attr("checked", true);
            else if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "K")
                $(document.getElementById("<%=rdbPagPostPrepagata.ClientID %>")).attr("checked", true);
            else if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "C")
                $(document.getElementById("<%=rdbPagPostCircolarita.ClientID %>")).attr("checked", true);

            else if ((document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "")) {
                $(document.getElementById("<%=rdbPagPostContoCorr.ClientID %>")).attr("checked", false);
                $(document.getElementById("<%=rdbPagPostSportello.ClientID %>")).attr("checked", false);
                $(document.getElementById("<%=rdbPagPostLibretto.ClientID %>")).attr("checked", false);
                $(document.getElementById("<%=rdbPagPostCircolarita.ClientID %>")).attr("checked", false);
                $(document.getElementById("<%=rdbPagPostPrepagata.ClientID %>")).attr("checked", false);
            }
        }
        else if (tipo == "E") {
            document.getElementById('<%=modPagamentoE.ClientID %>').value = document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value;
            if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "C")
                $(document.getElementById("<%=rdbContoCorrenteE.ClientID %>")).attr("checked", true);
            else if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "S")
                $(document.getElementById("<%=rdbSportelloE.ClientID %>")).attr("checked", true);
            else if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "A")
                $(document.getElementById("<%=rdbAssegnoE.ClientID %>")).attr("checked", true);
            else if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "") {
                $(document.getElementById("<%=rdbContoCorrenteE.ClientID %>")).attr("checked", false);
                $(document.getElementById("<%=rdbSportelloE.ClientID %>")).attr("checked", false);
                $(document.getElementById("<%=rdbAssegnoE.ClientID %>")).attr("checked", false);
            }
        }
        else if (tipo == "C") {
            document.getElementById('<%=modPagamentoC.ClientID %>').value = document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value;
            if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "P")
                $(document.getElementById("<%=rdbPagamSportelloCassaSede.ClientID %>")).attr("checked", true);
            else if (document.getElementById('<%=modPagamentoPrecedente.ClientID %>').value == "") {
                $(document.getElementById("<%=rdbPagamSportelloCassaSede.ClientID %>")).attr("checked", false);
            }
        }
    }

    function CleanFields() {
        document.getElementById('<%=txtAbiSportelloBanca.ClientID %>').value = "";
        document.getElementById('<%=txtCabSportelloBanca.ClientID %>').value = "";
        document.getElementById('<%=txtIbanCCBanca.ClientID %>').value = "";
        document.getElementById('<%=txtBicCCBanca.ClientID %>').value = "";
        document.getElementById('<%=txtIbanLibrettoBanca.ClientID %>').value = "";
        document.getElementById('<%=txtIbanPrepagataBanca.ClientID %>').value = "";
        document.getElementById('<%=txtBicPrepagataBanca.ClientID %>').value = "";

        document.getElementById('<%=txtFrazionarioSportelloPosta.ClientID %>').value = "";

        document.getElementById('<%=txtIbanCCPosta.ClientID %>').value = "";
        document.getElementById('<%=txtFrazionarioCCPosta.ClientID %>').value = "";

        document.getElementById('<%=txtIbanLibrettoPosta.ClientID %>').value = "";
        document.getElementById('<%=txtFrazionarioLibrettoPosta.ClientID %>').value = "";

        document.getElementById('<%=txtIbanPrepagataPosta.ClientID %>').value = "";
        document.getElementById('<%=txtFrazionarioPrepagataPosta.ClientID %>').value = "";

        document.getElementById('<%=txtIbanCCEstero.ClientID %>').value = "";
        document.getElementById('<%=txtBicCCEstero.ClientID %>').value = "";
        document.getElementById('<%=ddlStatoEstero.ClientID %>').value = "";
        document.getElementById('<%=ddlStatoEsteroCCEstero.ClientID %>').value = "";


        document.getElementById('<%=ddlCassaSede.ClientID %>').value = "";
    }


    function CleanForm() {
        CleanFields();
        if (typeof (Page_Validators) != "undefined") {
            for (i = 0; i < Page_Validators.length; i++) {
                if (Page_Validators[i].style.visibility.length > 0 && Page_Validators[i].style.display.length == 0)
                    Page_Validators[i].style.visibility = 'hidden';
                else if (Page_Validators[i].style.display.length > 0 && Page_Validators[i].style.visibility.length == 0)
                    Page_Validators[i].style.display = 'none';
                else {
                    Page_Validators[i].style.visibility = 'hidden';
                }
            }
        }
        if (typeof (Page_ValidationSummaries) != "undefined") { //hide the validation summaries
            for (i = 0; i < Page_ValidationSummaries.length; i++) {
                Page_ValidationSummaries[i].style.display = 'none';
            }
        }
        return false;
    }


</script>
<asp:Panel ID="panMain" runat="server">
    <table class="tabellaFormattazione grid grid-col-1" style="vertical-align: top; height: 8%; width: 100%">
        <tr>
            <td class="Row1 force-block">
                <div>
                    <asp:Label ID="lblAvvisoInvaliditaCivile" Text="ATTENZIONE: in caso di presenza di Invalidità civile parziale, incumulabile con AOI, localizzare il pagamento a Cassa Sede in attesa di espressa preferenza dichiarata dal soggetto"
                        runat="server" Visible="false" Style="font-weight: bold" ForeColor="Red"></asp:Label>
                </div>
                <br />
                <b>
                    <label class="section-label">Pagamento presso</label></b>
                <br />
                <asp:Panel ID="panTipo" runat="server" Visible="true">
                    <div style="padding: 5px">
                        <table border="0" style="vertical-align: top; height: 8%; width: 100%">
                            <tr>
                                <td style="height: 3%;">
                                    <asp:RadioButton runat="server" ID="rdbBanca" GroupName="TipoPagament" Font-Size="Small"
                                        CssClass="onClassBanca offClass" Text="Banca" TabIndex="1" />
                                </td>
                                <td style="height: 3%;">
                                    <asp:RadioButton runat="server" ID="rdbPosta" GroupName="TipoPagament" Font-Size="Small"
                                        CssClass="onClassPosta offClass" Text="Posta" TabIndex="2" />
                                </td>
                                <td style="height: 3%;">
                                    <asp:RadioButton runat="server" ID="rdbEstero" GroupName="TipoPagament" Font-Size="Small"
                                        CssClass="onClassEstero offClass" Text="Estero" TabIndex="3" />
                                </td>
                                <td style="height: 3%;">
                                    <asp:RadioButton runat="server" ID="rdbCassaSede" GroupName="TipoPagament" Font-Size="Small"
                                        CssClass="onClassCassaSede offClass" Text="Cassa Sede" TabIndex="3" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>
                <asp:Panel ID="panBanca" runat="server" Width="98%" BorderStyle="None">
                    <label style="font-weight: bold;" class="section-label">Tipo pagamento</label>
                    <span style="visibility: hidden">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                    <br />
                    <blockquote>
                        <asp:RadioButton ID="rdbPagamSportelloBanca" Text="Sportello" runat="server" CssClass="offClass onClassSportelloBanca"
                            GroupName="PAGBANC" TabIndex="4" />
                        <br />
                        <br />
                        <asp:RadioButton ID="rdbContoCorrenteBanca" runat="server" Text="Conto Corrente bancario nominativo"
                            GroupName="PAGBANC" CssClass="offClass onClassContoCorrenteBanca" TabIndex="5" />
                        <br />
                        <br />
                        <asp:RadioButton ID="rdbLibrettoRisparmioBanca" runat="server" GroupName="PAGBANC"
                            Text="Libretto di risparmio" CssClass="offClass onClassLibrettoRisparmioBanca"
                            TabIndex="6" />
                        <br />
                        <br />
                        <asp:RadioButton ID="rdbPrepagataBanca" runat="server" Text="Carta prepagata" GroupName="PAGBANC"
                            CssClass="offClass onClassPrepagataBanca" TabIndex="6" />
                        <br />
                        <br />
                        <!-- Campi da riempire con i dati provenienti dal WS  -->
                        <asp:Panel runat="server" ID="panDatibanca">
                            <div class="payment-mode-details" style="display: none">
                                <p class="payment-mode-details__title">
                                    Informazioni di pagamento
                                </p>
                            </div>
                            <table border="0" style="vertical-align: top; height: 8%; width: 100%" class="tabellaFormattazione grid grid-col-6">
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Codice IBAN</label>
                                    </td>
                                    <td style="height: 3%; width: 30%" colspan="3" class=" full-grid">
                                        <asp:TextBox ID="txtIban" runat="server" Enabled="false" Width="99.7%" CssClass="tb8 txtUppercase readonly"
                                            TabIndex="7"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Codice BIC</label>
                                    </td>
                                    <td style="height: 3%; width: 30%" colspan="3">
                                        <asp:TextBox ID="txtBicBanca" runat="server" Enabled="false" Width="100%" CssClass="tb8 txtUppercase readonly"
                                            TabIndex="8"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Codice ABI</label>
                                    </td>
                                    <td style="height: 3%; width: 30%">
                                        <asp:TextBox ID="txtCodiceAbi" runat="server" Enabled="false" Width="100%" CssClass="tb8 txtUppercase readonly"
                                            TabIndex="9"></asp:TextBox>
                                    </td>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Codice CAB
                                        </label>
                                    </td>
                                    <td style="height: 3%; width: 30%">
                                        <asp:TextBox ID="txtCodiceCab" runat="server" Enabled="false" Width="99%" CssClass="tb8 txtUppercase readonly"
                                            TabIndex="10"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Banca</label>
                                    </td>
                                    <td style="height: 3%; width: 30%" colspan="3" class=" full-grid">
                                        <asp:TextBox ID="txtBanca" runat="server" Enabled="false" Width="99.7%" CssClass="tb8 txtUppercase readonly"
                                            TabIndex="11"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Agenzia</label>
                                    </td>
                                    <td style="height: 3%; width: 30%" colspan="3" class=" full-grid">
                                        <asp:TextBox ID="txtAgenzia" runat="server" Enabled="false" Width="99.7%" CssClass="tb8 txtUppercase readonly"
                                            TabIndex="12"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Città</label>
                                    </td>
                                    <td style="height: 3%; width: 30%">
                                        <asp:TextBox ID="txtCittaBanca" runat="server" Enabled="false" Width="100%" CssClass="tb8 txtUppercase readonly"
                                            TabIndex="13"></asp:TextBox>
                                    </td>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Cap</label>
                                    </td>
                                    <td style="height: 3%; width: 30%">
                                        <asp:TextBox ID="txtCapBanca" runat="server" Enabled="false" Width="99%" CssClass="tb8 txtUppercase readonly"
                                            TabIndex="14"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right" class="payment-method-grid-address">
                                        <label>
                                            Indirizzo</label>
                                    </td>
                                    <td style="height: 3%; width: 30%" colspan="3" class=" full-grid">
                                        <asp:TextBox ID="txtIndirizzo" runat="server" Enabled="false" Width="99.7%" CssClass="tb8 txtUppercase readonly"
                                            TabIndex="15"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                            <table border="0" style="vertical-align: top; height: 8%; width: 100%">
                            </table>
                            <table border="0" style="vertical-align: top; height: 8%; width: 100%">
                            </table>
                        </asp:Panel>
                        <!--Fine pannello dati banca -->
                    </blockquote>
                </asp:Panel>
                <!-- Fine pannello banca-->
                <!-- Pannello poste -->
                <asp:Panel ID="panPosta" runat="server" Width="98%" BorderStyle="None">
                    <label style="font-weight: bold;" class="section-label">Tipo pagamento</label>
                    <span style="visibility: hidden">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                    <blockquote>
                        <asp:RadioButton ID="rdbPagPostSportello" runat="server" CssClass="offClass onClassSportelloPosta"
                            GroupName="PAGPOST" Text="Sportello" TabIndex="16" />
                        <br />
                        <br />
                        <asp:RadioButton ID="rdbPagPostLibretto" runat="server" CssClass="offClass onClassLibrettoPosta"
                            GroupName="PAGPOST" Text="Libretto postale nominativo" TabIndex="17" />
                        <asp:TextBox ID="txtNumLibrRispPoste" runat="server" MaxLength="12" Visible="false"
                            CssClass="tb8 txtUppercase" Width="20%" TabIndex="18"></asp:TextBox>
                        <br />
                        <br />
                        <asp:RadioButton ID="rdbPagPostContoCorr" runat="server" CssClass="offClass onClassCCPosta"
                            GroupName="PAGPOST" Text="Conto Corrente postale nominativo" TabIndex="19" />
                        <asp:Panel ID="pnlCircolarita" runat="server" Visible="false">
                            <br />
                            <asp:RadioButton ID="rdbPagPostCircolarita" runat="server" CssClass="offClass onClassCircPosta"
                                GroupName="PAGPOST" Text="Circolarità" TabIndex="20" />
                        </asp:Panel>
                        <br />
                        <br />
                        <asp:RadioButton ID="rdbPagPostPrepagata" runat="server" CssClass="offClass onClassPrepagataPosta"
                            GroupName="PAGPOST" Text="Carta prepagata (Postepay Evolution)" />
                        <!-- Campi da riempire con i dati provenienti dal WS -->
                        <asp:Panel ID="panDatiUfficioPostale" runat="server">
                            <div class="payment-mode-details" style="display: none">
                                <p class="payment-mode-details__title">
                                    Informazioni di pagamento
                                </p>
                            </div>
                            <br />
                            <table border="0" style="vertical-align: top; height: 8%; width: 100%" class="tabellaFormattazione grid">
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Codice IBAN</label>
                                    </td>
                                    <td style="height: 3%; width: 30%" colspan="3" class=" full-grid">
                                        <asp:TextBox ID="txtIbanPoste" runat="server" CssClass="tb8 txtUppercase readonly" Enabled="false" Width="99.7%"> </asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Libretto</label>
                                    </td>
                                    <td style="height: 3%;">
                                        <asp:TextBox ID="txtLibretto" runat="server" CssClass="tb8 txtUppercase readonly" Enabled="false" Width="100%"> </asp:TextBox>
                                    </td>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Codice BIC</label>
                                    </td>
                                    <td style="height: 3%; width: 30%">
                                        <asp:TextBox ID="txtBicPoste" runat="server" CssClass="tb8 txtUppercase readonly" Enabled="false" Width="99%"> </asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Ufficio Postale</label>
                                    </td>
                                    <td style="height: 3%; width: 30%" colspan="3" class=" full-grid">
                                        <asp:TextBox ID="txtUffPost" runat="server" CssClass="tb8 txtUppercase readonly" Enabled="false" Width="99.7%"
                                            TabIndex="20"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Agenzia
                                        </label>
                                    </td>
                                    <td style="height: 3%; width: 30%" colspan="3" class=" full-grid">
                                        <asp:TextBox ID="txtNumUffPost" runat="server" CssClass="tb8 txtUppercase readonly" Enabled="false" Width="99.7%"
                                            TabIndex="21"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Indirizzo
                                        </label>
                                    </td>
                                    <td style="height: 3%;" colspan="3" class=" full-grid">
                                        <asp:TextBox ID="txtIndirizzoUffPost" runat="server" CssClass="tb8 txtUppercase readonly" Enabled="false"
                                            Width="99.7%" TabIndex="22"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Città
                                        </label>
                                    </td>
                                    <td style="height: 3%; width: 30%">
                                        <asp:TextBox ID="txtCittaUffPost" runat="server" CssClass="tb8 txtUppercase readonly" Enabled="false" Width="100%"
                                            TabIndex="23"></asp:TextBox>
                                    </td>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            CAP
                                        </label>
                                    </td>
                                    <td style="height: 3%; width: 30%">
                                        <asp:TextBox ID="txtCapUffPost" runat="server" CssClass="tb8 txtUppercase readonly" Enabled="false" Width="99%"
                                            TabIndex="24"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Codice ABI</label>
                                    </td>
                                    <td style="height: 3%; width: 30%">
                                        <asp:TextBox ID="txtCodAbiUffPost" runat="server" CssClass="tb8 txtUppercase readonly" Enabled="false"
                                            Width="100%" TabIndex="26"></asp:TextBox>
                                    </td>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <asp:Label runat="server" ID="lblCabFrazionario" Text="Frazionario"></asp:Label>
                                    </td>
                                    <td style="height: 3%; width: 30%">
                                        <asp:TextBox ID="txtCabFrazionario" runat="server" CssClass="tb8 txtUppercase readonly" Enabled="false" Width="99%"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <!-- Fine Sezione dati Poste-->
                    </blockquote>
                </asp:Panel>
                <!-- Fine Sezione Poste  -->
                <!-- Inizio Pannello  Estero-->
                <asp:Panel ID="panEstero" runat="server" Width="98%" BorderStyle="None">
                    <label style="font-weight: bold;">
                        Pagamento Estero</label>
                    <span style="visibility: hidden">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
                    <blockquote>
                        <asp:Panel ID="panListaEstero" runat="server" Width="98%" BorderStyle="None">
                            <asp:RadioButton ID="rdbSportelloE" runat="server" GroupName="PAGESTERO" Text="Sportello"
                                CssClass="offClass onClassSportelloE" TabIndex="27" />
                            <br />
                            <br />
                            <asp:RadioButton ID="rdbAssegnoE" runat="server" GroupName="PAGESTERO" Text="Assegno"
                                CssClass="offClass onClassAssegnoE" TabIndex="28" />
                            <br />
                            <br />
                            <asp:RadioButton ID="rdbContoCorrenteE" runat="server" GroupName="PAGESTERO" Text="Conto Corrente"
                                CssClass="offClass onClassContoCorrenteE" TabIndex="29" />
                        </asp:Panel>
                        <asp:Panel runat="server" ID="SelezioneStatoEstero">
                            <div class="payment-mode-details" style="display: none">
                                <p class="payment-mode-details__title">
                                    Informazioni di pagamento
                                </p>
                            </div>
                            <table border="0" style="vertical-align: top; height: 8%; width: 100%" class="tabellaFormattazione grid">
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Nome</label>
                                    </td>
                                    <td style="height: 3%;" colspan="3" class=" full-grid">
                                        <asp:TextBox ID="txtNomeUfficioEstero" runat="server" CssClass="tb8 txtUppercase readonly"
                                            Enabled="false" Width="99.7%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Stato</label>
                                    </td>
                                    <td style="height: 3%; width: 30%" colspan="3" class=" full-grid">
                                        <asp:TextBox ID="txtCittaUfficioEstero" runat="server" CssClass="tb8 txtUppercase readonly" Enabled="false"
                                            Width="99.7%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Agenzia</label>
                                    </td>
                                    <td style="height: 3%; width: 30%" colspan="3" class=" full-grid">
                                        <asp:TextBox ID="txtAgenziaUfficioEstero" runat="server" CssClass="tb8 txtUppercase readonly"
                                            Enabled="false" Width="99.7%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Codice ABI</label>
                                    </td>
                                    <td style="height: 3%; width: 30%">
                                        <asp:TextBox ID="txtAbiUfficioEstero" runat="server" CssClass="tb8 txtUppercase readonly"
                                            Enabled="false" Width="100%"></asp:TextBox>
                                    </td>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Codice Cab</label>
                                    </td>
                                    <td style="height: 3%; width: 30%">
                                        <asp:TextBox ID="txtCabUfficioEstero" runat="server" CssClass="tb8 txtUppercase readonly"
                                            Enabled="false" Width="99%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Bic/Swift/Altro</label>
                                    </td>
                                    <td style="height: 3%; width: 30%">
                                        <asp:TextBox ID="txtBicUfficioEstero" runat="server" CssClass="tb8 txtUppercase readonly"
                                            Enabled="false" Width="100%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Iban/Conto</label>
                                    </td>
                                    <td style="height: 3%; width: 30%" colspan="3" class=" full-grid">
                                        <asp:TextBox ID="txtIbanUfficioEstero" runat="server" CssClass="tb8 txtUppercase readonly"
                                            Enabled="false" Width="99.7%"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <br />
                        <br />
                    </blockquote>
                </asp:Panel>
                <!-- Fine Pannello estero-->
                <!-- Pannello Cassa Sede-->
                <asp:Panel ID="panCassaSede" runat="server" Width="98%" BorderStyle="None">
                    <label style="font-weight: bold;" class="section-label">Tipo pagamento</label>
                    <br />
                    <blockquote>
                        <asp:RadioButton ID="rdbPagamSportelloCassaSede" Text="Cassa" runat="server" CssClass="offClass onClassSportelloCassaSede"
                            GroupName="PAGSEDE" TabIndex="4" />
                        <br />
                        <br />
                        <!-- Campi da riempire con i dati provenienti dal WS  -->
                            <div class="payment-mode-details" style="display: none">
                                <p class="payment-mode-details__title">
                                    Informazioni di pagamento
                                </p>
                            </div>
                        <asp:Panel runat="server" ID="panDatiSede">
                            <table border="0" style="vertical-align: top; height: 8%; width: 100%" class="tabellaFormattazione grid">
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Codice ABI</label>
                                    </td>
                                    <td style="height: 3%; width: 30%">
                                        <asp:TextBox ID="txtAbiCassa" runat="server" Enabled="false" Width="100%" CssClass="tb8 txtUppercase readonly"
                                            TabIndex="9"></asp:TextBox>
                                    </td>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Codice CAB
                                        </label>
                                    </td>
                                    <td style="height: 3%; width: 30%;">
                                        <asp:TextBox ID="txtCabCassa" runat="server" Enabled="false" Width="99%" CssClass="tb8 txtUppercase readonly"
                                            TabIndex="10"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 3%; width: 20%; text-align: right">
                                        <label>
                                            Descrizione</label>
                                    </td>
                                    <td colspan="3" style="height: 3%;" class=" full-grid">
                                        <asp:TextBox ID="txtDescrizioneSede" runat="server" Enabled="false" Width="99.7%"
                                            CssClass="tb8 txtUppercase readonly" TabIndex="7"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </blockquote>
                </asp:Panel>
                <!--Fine pannello Cassa Sede -->
            </td>
        </tr>
    </table>
</asp:Panel>
<br />
<asp:Button ID="btnConfermaSportelloBanca" CausesValidation="true" ValidationGroup="validDialogSportelloBanca"
    Style="display: none" runat="server" OnClick="btnRicercaSportelloBanca_Click"
    OnClientClick="BlockUI();" Text=" " />
<asp:Button ID="btnConfermaCCBanca" CausesValidation="true" ValidationGroup="validDialogCCBanca"
    Style="display: none" runat="server" OnClick="btnRicercaCCBanca_Click" OnClientClick="BlockUI();"
    Text=" " />
<asp:Button ID="btnConfermaLibrettoBanca" CausesValidation="true" ValidationGroup="validDialogLibrettoBanca"
    Style="display: none" runat="server" OnClick="btnRicercaLibrettoBanca_Click"
    OnClientClick="BlockUI();" Text=" " />
<asp:Button ID="btnConfermaPrepagataBanca" CausesValidation="true" ValidationGroup="validDialogPrepagataBanca"
    Style="display: none" runat="server" OnClick="btnRicercaPrepagataBanca_Click"
    OnClientClick="BlockUI();" Text=" " />
<asp:Button ID="btnRicercaSportelloPosta" CausesValidation="true" ValidationGroup="validDialogSportelloPosta"
    Style="display: none" runat="server" OnClick="btnRicercaSportelloPosta_Click"
    OnClientClick="BlockUI();" Text=" " />
<asp:Button ID="btnRicercaCCPosta" CausesValidation="true" ValidationGroup="validDialogCCPosta"
    Style="display: none" runat="server" OnClick="btnRicercaCCPosta_Click" Text=" "
    OnClientClick="BlockUI();" />
<asp:Button ID="btnRicercaLibrettoPosta" CausesValidation="true" ValidationGroup="validDialogLibrettoPosta"
    Style="display: none" runat="server" OnClick="btnRicercaLibrettoPosta_Click"
    OnClientClick="BlockUI();" Text=" " />
<asp:Button ID="btnRicercaCircPosta" CausesValidation="true" ValidationGroup="validDialogCircPosta"
    Style="display: none" runat="server" OnClick="btnRicercaCircPosta_Click" OnClientClick="BlockUI();"
    Text=" " />
<asp:Button ID="btnRicercaPrepagataPosta" CausesValidation="true" ValidationGroup="validDialogPrepagataPosta"
    Style="display: none" runat="server" OnClick="btnRicercaPrepagataPosta_Click"
    OnClientClick="BlockUI();" Text=" " />
<asp:Button ID="btnRicercaCCEstero" CausesValidation="true" ValidationGroup="validDialogCCEstero"
    Style="display: none" runat="server" OnClick="btnRicercaCCEstero_Click" Text=" "
    OnClientClick="BlockUI();" />
<asp:Button ID="btnRicercaStatoEstero" CausesValidation="true" ValidationGroup="validDialogStatoEstero"
    Style="display: none" runat="server" OnClick="btnRicercaStatoEstero_Click" OnClientClick="BlockUI();"
    Text=" " />
<asp:Button ID="btnConfermaSportelloSede" CausesValidation="true" ValidationGroup="validdialogSportelloCassaSede"
    Style="display: none" runat="server" OnClick="btnRicercaSportelloCassaSede_Click"
    OnClientClick="BlockUI();" Text=" " />
<%--  Finestre di PopUp      --%>
<div id="dialogSportelloBanca" title="Inserimento Sportello" style="border-style: none;
    border-color: White;" class="flex-column">
    <asp:Panel ID="main" runat="server">
        <asp:ValidationSummary runat="server" ID="validSummarySportelloBanca" ValidationGroup="validDialogSportelloBanca"
            Font-Size="Small" CssClass="errorBox" Visible="true" />
        <label>
            Codice Abi</label>
        <span style="visibility: hidden">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span><br />
        <asp:TextBox runat="server" ID="txtAbiSportelloBanca" MaxLength="5" CssClass="tb8 txtUppercase"
            TabIndex="30"></asp:TextBox>
        <asp:RegularExpressionValidator ID="validateTxtAbiSportelloBanca" ControlToValidate="txtAbiSportelloBanca"
            ErrorMessage="Codice Abi non valido" ValidationExpression="^[0-9]{5}$" runat="server"
            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="validDialogSportelloBanca" Enabled="true" />
        <asp:RequiredFieldValidator runat="server" ID="validateAbiSportelloBanca" ControlToValidate="txtAbiSportelloBanca"
            Enabled="true" ErrorMessage="Codice abi obbligatorio" ValidationGroup="validDialogSportelloBanca"
            Text="*" CssClass="field-is-required" />
        <br />
        <br />
        <label class="mt-16">
            Codice Cab</label>
        <span style="visibility: hidden">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span><br />
        <asp:TextBox runat="server" ID="txtCabSportelloBanca" MaxLength="5" CssClass="tb8 txtUppercase"
            TabIndex="31"></asp:TextBox>
        <asp:RegularExpressionValidator ID="validateTxtCabSportellobanca" ControlToValidate="txtCabSportelloBanca"
            ErrorMessage="Codice Cab non valido" ValidationExpression="^[0-9]{5}$" runat="server"
            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="validDialogSportelloBanca" Enabled="true" />
        <asp:RequiredFieldValidator runat="server" ID="validateCabSportelloBanca" ControlToValidate="txtCabSportelloBanca"
            Enabled="true" ErrorMessage="Codice cab obbligatorio" ValidationGroup="validDialogSportelloBanca"
            Text="*" CssClass="field-is-required" />
        <asp:TextBox runat="server" ID="txtBtnSportelloBanca" MaxLength="5" Visible="false"
            CssClass="tb8 txtUppercase" TabIndex="32"></asp:TextBox>
    </asp:Panel>
</div>
<div id="dialogCCBanca" title="Inserimento Dati CC Bancario" style="border-style: none;
    border-color: White; width: 370px" class="flex-column">
    <asp:ValidationSummary runat="server" ID="dialogCCBancaSummary1" ValidationGroup="validDialogCCBanca"
        Font-Size="Small" CssClass="errorBox" Visible="true" />
    <label>
        Codice IBAN</label>
    <asp:TextBox runat="server" ID="txtIbanCCBanca" MaxLength="27" Width="315px" CssClass="tb8 txtUppercase"
        TabIndex="33"></asp:TextBox>
    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorTxtIbanCCBanca"
        ControlToValidate="txtIbanCCBanca" Enabled="true" ErrorMessage="Inserire il codice Iban del conto corrente"
        ValidationGroup="validDialogCCBanca" Text="*" CssClass="field-is-required" />
    <br />
    <br />
    <label class="mt-16">
        Codice BIC
    </label>
    <asp:TextBox runat="server" ID="txtBicCCBanca" MaxLength="11" Width="315px" CssClass="tb8 txtUppercase"
        TabIndex="34"></asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionValidatorTxtBicCCBanca" ControlToValidate="txtBicCCBanca"
        ErrorMessage="Codice Bic non valido" ValidationExpression="^[A-Z a-z 0-9]{8,11}$"
        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="validDialogCCBanca"
        Enabled="true" />
</div>
<div id="dialogLibrettoBanca" title="Inserimento Dati Libretto" style="border-style: none;
    border-color: White; width: 370px" class="flex-column">
    <asp:ValidationSummary runat="server" ID="ValidationSummaryLibrettoBanca" ValidationGroup="validDialogLibrettoBanca"
        Font-Size="Small" CssClass="errorBox" Visible="true" />
    <label>
        Codice IBAN
    </label>
    <asp:TextBox runat="server" ID="txtIbanLibrettoBanca" MaxLength="27" Width="315px"
        CssClass="tb8 txtUppercase" TabIndex="35"></asp:TextBox>
    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtIbanLibrettoBanca"
        Enabled="true" ErrorMessage="Inserire il codice Iban del conto corrente" ValidationGroup="validDialogLibrettoBanca"
        Text="*" CssClass="field-is-required" />
</div>
<div id="dialogPrepagataBanca" title="Inserimento Dati Carta Prepagata" style="border-style: none;
    border-color: White; width: 370px" class="flex-column">
    <asp:ValidationSummary runat="server" ID="dialogPrepagataBancaSummary1" ValidationGroup="validDialogPrepagataBanca"
        Font-Size="Small" CssClass="errorBox" Visible="true" />
    <label>
        Codice IBAN</label>
    <asp:TextBox runat="server" ID="txtIbanPrepagataBanca" MaxLength="27" Width="315px"
        CssClass="tb8 txtUppercase" TabIndex="33"></asp:TextBox>
    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorTxtIbanPrepagataBanca"
        ControlToValidate="txtIbanPrepagataBanca" Enabled="true" ErrorMessage="Inserire il codice Iban della carta prepagata"
        ValidationGroup="validDialogPrepagataBanca" Text="*" CssClass="field-is-required" />
    <br />
    <br />
    <label class="mt-16">
        Codice BIC
    </label>
    <asp:TextBox runat="server" ID="txtBicPrepagataBanca" MaxLength="11" Width="315px"
        CssClass="tb8 txtUppercase" TabIndex="34"></asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionValidatorTxtBicPrepagataBanca"
        ControlToValidate="txtBicPrepagataBanca" ErrorMessage="Codice Bic non valido"
        ValidationExpression="^[A-Z a-z 0-9]{8,11}$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
        ValidationGroup="validDialogPrepagataBanca" Enabled="true" />
</div>
<div id="dialogSportelloPosta" title="Inserimento Sportello" style="border-style: none;
    border-color: White;" class="flex-column">
    <asp:ValidationSummary runat="server" ID="validateSportelloPosta" ValidationGroup="validDialogSportelloPosta"
        Font-Size="Small" CssClass="errorBox" Visible="true" />
    <label>
        Codice Abi
    </label>
    <asp:TextBox runat="server" ID="txtAbiSportelloPosta" MaxLength="5" Enabled="false"
        CssClass="tb8 txtUppercase" TabIndex="36"></asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionSportelloPosta" ControlToValidate="txtAbiSportelloPosta"
        ErrorMessage="Codice Abi non valido" ValidationExpression="^[0-9]{5}$" runat="server"
        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="validDialogSportelloPosta" Enabled="true" />
    <asp:RequiredFieldValidator runat="server" ID="RequiredAbiSportelloPosta" ControlToValidate="txtAbiSportelloPosta"
        Enabled="true" ErrorMessage="Inserire il codice abi dello sportello" ValidationGroup="validDialogSportelloPosta"
        Text="*" CssClass="field-is-required" />
    <br />
    <br />
    <label class="mt-16">
        Frazionario
    </label>
    <asp:TextBox runat="server" ID="txtFrazionarioSportelloPosta" MaxLength="5" CssClass="tb8 txtUppercase"></asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionValidatorSportelloPosta" ControlToValidate="txtFrazionarioSportelloPosta"
        ErrorMessage="Codice Frazionario non valido" ValidationExpression="^[0-9]{5}$"
        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="validDialogSportelloPosta"
        Enabled="true" />
    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldFrazionarioSportelloPosta"
        ControlToValidate="txtFrazionarioSportelloPosta" Enabled="true" ErrorMessage="Inserire il frazionario dello sportello"
        ValidationGroup="validDialogSportelloPosta" Text="*" CssClass="field-is-required" />
</div>
<div id="dialogLibrettoPosta" title="Inserimento Dati Libretto Postale" style="border-style: none;
    border-color: White;" class="flex-column">
    <asp:ValidationSummary runat="server" ID="ValidationSummaryLibrettoPosta" ValidationGroup="validDialogLibrettoPosta"
        Font-Size="Small" CssClass="errorBox" Visible="true" />
    <label>
        Codice IBAN
    </label>
    <asp:TextBox runat="server" ID="txtIbanLibrettoPosta" MaxLength="27" Width="315px"
        CssClass="tb8 txtUppercase"></asp:TextBox>
    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorIbanLibPosta"
        ControlToValidate="txtIbanLibrettoPosta" Enabled="true" ErrorMessage="Inserire il codice Iban del libretto"
        ValidationGroup="validDialogLibrettoPosta" Text="*" CssClass="field-is-required" Display="Dynamic" />
    <br />
    <br />
    <label class="mt-16">
        Frazionario
    </label>
    <asp:TextBox runat="server" ID="txtFrazionarioLibrettoPosta" MaxLength="5" Width="315px"
        CssClass="tb8 txtUppercase"></asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionValidatorFrazionarioLibPosta"
        ControlToValidate="txtFrazionarioLibrettoPosta" ErrorMessage="Frazionario non valido"
        ValidationExpression="^[0-9]{5}$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="validDialogLibrettoPosta"
        Enabled="true" />
    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorFrazionarioLibPosta"
        ControlToValidate="txtFrazionarioLibrettoPosta" Enabled="true" ErrorMessage="Inserire il frazionario del libretto"
        ValidationGroup="validDialogLibrettoPosta" Text="*" CssClass="field-is-required" />
</div>
<div id="dialogCCPosta" title="Inserimento Dati " style="border-style: none; border-color: White;
    width: 370px" class="flex-column">
    <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="validDialogCCPosta"
        Font-Size="Small" CssClass="errorBox" Visible="true" />
    <label>
        Codice IBAN
    </label>
    <asp:TextBox runat="server" ID="txtIbanCCPosta" MaxLength="27" Width="315px" CssClass="tb8 txtUppercase"
        TabIndex="40"></asp:TextBox>
    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtIbanCCPosta"
        Enabled="true" ErrorMessage="Inserire il codice Iban del conto corrente" ValidationGroup="validDialogCCPosta"
        Text="*" CssClass="field-is-required" Display="Dynamic" />
    <br />
    <br />
    <label class="mt-16">
        Frazionario
    </label>
    <asp:TextBox runat="server" ID="txtFrazionarioCCPosta" MaxLength="5" Width="315px"
        CssClass="tb8 txtUppercase" TabIndex="41"></asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtFrazionarioCCPosta"
        ErrorMessage="Frazionario non valido" ValidationExpression="^[0-9]{5}$" runat="server"
        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="validDialogCCPosta" Enabled="true" />
    <asp:RequiredFieldValidator runat="server" ID="RequiredFrazionarioCCPosta" ControlToValidate="txtFrazionarioCCPosta"
        Enabled="true" ErrorMessage="Inserire il frazionario del conto corrente" ValidationGroup="validDialogCCPosta"
        Text="*" CssClass="field-is-required" />
</div>
<div id="dialogCircPosta" title="Inserimento Sportello" style="border-style: none;
    border-color: White;" class="flex-column">
    <asp:ValidationSummary runat="server" ID="ValidationSummary2" ValidationGroup="validDialogCircPosta"
        Font-Size="Small" CssClass="errorBox" Visible="true" />
    <label>
        Codice Abi</label>
    <span style="visibility: hidden">&nbsp;&nbsp;&nbsp;</span><br />
    <asp:TextBox runat="server" ID="txtAbiCircPosta" MaxLength="5" CssClass="tb8 txtUppercase"
        TabIndex="30" Enabled="false"></asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ControlToValidate="txtAbiCircPosta"
        ErrorMessage="Codice Abi non valido" ValidationExpression="^[0-9]{5}$" runat="server"
        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="validDialogCircPosta" Enabled="true" />
    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator6" ControlToValidate="txtAbiCircPosta"
        Enabled="true" ErrorMessage="Codice abi obbligatorio" ValidationGroup="validDialogCircPosta"
        Text="*" CssClass="field-is-required" />
    <br />
    <br />
    <label class="mt-16">
        Frazionario</label>
    <span style="visibility: hidden">&nbsp;&nbsp;&nbsp;</span><br />
    <asp:TextBox runat="server" ID="txtFrazionarioCircPosta" MaxLength="7" CssClass="tb8 txtUppercase"
        TabIndex="31" Enabled="false"></asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator5" ControlToValidate="txtFrazionarioCircPosta"
        ErrorMessage="Codice Cab non valido" ValidationExpression="^[0-9]{7}$" runat="server"
        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="validDialogCircPosta" Enabled="true" />
    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator7" ControlToValidate="txtFrazionarioCircPosta"
        Enabled="true" ErrorMessage="Codice cab obbligatorio" ValidationGroup="validDialogCircPosta"
        Text="*" CssClass="field-is-required" />
</div>
<div id="dialogPrepagataPosta" title="Inserimento Dati " style="border-style: none;
    border-color: White; width: 370px" class="flex-column">
    <asp:ValidationSummary runat="server" ID="ValidationSummary3" ValidationGroup="validDialogPrepagataPosta"
        Font-Size="Small" CssClass="errorBox" Visible="true" />
    <label>
        IBAN
    </label>
    <asp:TextBox runat="server" ID="txtIbanPrepagataPosta" MaxLength="27" Width="315px"
        CssClass="tb8 txtUppercase"></asp:TextBox>
    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorTxtIbanPrepagataPosta"
        ControlToValidate="txtIbanPrepagataPosta" Enabled="true" ErrorMessage="Inserire il codice Iban della Postepay Evolution"
        ValidationGroup="validDialogPrepagataPosta" Text="*" CssClass="field-is-required" Display="Dynamic" />
    <br />
    <br />
    <label class="mt-16">
        Frazionario
    </label>
    <asp:TextBox runat="server" ID="txtFrazionarioPrepagataPosta" MaxLength="5" Width="315px"
        CssClass="tb8 txtUppercase"></asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionValidatorTxtFrazionarioPrepagataPosta"
        ControlToValidate="txtFrazionarioPrepagataPosta" ErrorMessage="Frazionario non valido"
        ValidationExpression="^[0-9]{5}$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="validDialogPrepagataPosta"
        Enabled="true" />
</div>
<div id="dialogCCEstero" title="Inserimento Dati CC Estero" style="border-style: none;
    border-color: White; width: 370px" class="flex-column">
    <asp:ValidationSummary runat="server" ID="ValidationSummaryCCEstero" ValidationGroup="validDialogCCEstero"
        Font-Size="Small" CssClass="errorBox" Visible="true" />
    <label>
        Stato Estero
    </label>
    <asp:DropDownList runat="server" ID="ddlStatoEsteroCCEstero" Width="320px" CssClass="tb8 txtUppercase"
        TabIndex="42">
    </asp:DropDownList>
    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorddlCCStatoEstero"
        ControlToValidate="ddlStatoEsteroCCEstero" Enabled="true" ErrorMessage="Inserire lo Stato Estero"
        ValidationGroup="validDialogCCEstero" Text="*" CssClass="field-is-required" />
    <br />
    <br />
    <label class="mt-16">
        Codice IBAN/Numero di Conto
    </label>
    <asp:TextBox runat="server" ID="txtIbanCCEstero" MaxLength="34" Width="320px" CssClass="tb8 txtUppercase"
        TabIndex="43"></asp:TextBox>
    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorIbanCCEstero"
        ControlToValidate="txtIbanCCEstero" Enabled="true" ErrorMessage="Inserire il codice Iban del conto corrente"
        ValidationGroup="validDialogCCEstero" Text="*" CssClass="field-is-required" />
    <br />
    <br />
    <label class="mt-16">
        Codice BIC/SWIFT/Altro
    </label>
    <asp:TextBox runat="server" ID="txtBicCCEstero" MaxLength="11" CssClass="tb8 txtUppercase"
        Width="320px" TabIndex="44"></asp:TextBox>
</div>
<div id="dialogStatoEstero" title="Inserimento Stato Estero" style="border-style: none;
    border-color: White; width: 370px" class="flex-column">
    <asp:ValidationSummary runat="server" ID="ValidationSummaryStatoEstero" ValidationGroup="validDialogStatoEstero"
        Font-Size="Small" CssClass="errorBox" Visible="true" />
    <label>
        Stato Estero
    </label>
    <asp:DropDownList runat="server" ID="ddlStatoEstero" Width="320px" CssClass="tb8 txtUppercase"
        TabIndex="45">
    </asp:DropDownList>
    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorDdlStatoEstero"
        ControlToValidate="ddlStatoEstero" Enabled="true" ErrorMessage="Inserire lo Stato Estero"
        ValidationGroup="validDialogStatoEstero" Text="*" CssClass="field-is-required" />
</div>
<div id="dialogSportelloCassaSede" title="Inserimento Cassa Sede" style="border-style: none;
    border-color: White;" class="flex-column">
    <asp:ValidationSummary runat="server" ID="validSummarySportelloSede" ValidationGroup="validdialogSportelloCassaSede"
        Font-Size="Small" CssClass="errorBox" Visible="true" />
    <label>
        Codice Abi</label>
    <span style="visibility: hidden">&nbsp;&nbsp;&nbsp;</span><br />
    <asp:TextBox runat="server" ID="txtAbiSportelloCassaSede" MaxLength="5" CssClass="tb8 txtUppercase" Width="89%"
        TabIndex="30" Enabled="false"></asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtAbiSportelloCassaSede"
        ErrorMessage="Codice Abi non valido" ValidationExpression="^[0-9]{5}$" runat="server"
        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="validdialogSportelloCassaSede" Enabled="true" />
    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="txtAbiSportelloCassaSede"
        Enabled="true" ErrorMessage="Codice abi obbligatorio" ValidationGroup="validdialogSportelloCassaSede"
        Text="*" CssClass="field-is-required" />
    <br />
    <br />
    <label class="mt-16">
        Cassa</label>
    <span style="visibility: hidden">&nbsp;&nbsp;&nbsp;</span><br />
    <asp:DropDownList ID="ddlCassaSede" runat="server" Width="90%" CssClass="tb8 txtUppercase xl">
    </asp:DropDownList>
    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator5" ControlToValidate="ddlCassaSede"
        Enabled="true" ErrorMessage="Codice Cassa obbligatorio" ValidationGroup="validdialogSportelloCassaSede"
        Text="*" CssClass="field-is-required" />
</div>
<asp:HiddenField runat="server" ID="tipoPagamento" />
<asp:HiddenField runat="server" ID="modPagamentoPrecedente" />
<asp:HiddenField runat="server" ID="modPagamentoB" />
<asp:HiddenField runat="server" ID="modPagamentoP" />
<asp:HiddenField runat="server" ID="modPagamentoE" />
<asp:HiddenField runat="server" ID="modPagamentoC" />
<asp:HiddenField runat="server" ID="paramRicerca1" />
<asp:HiddenField runat="server" ID="paramRicerca2" />
<asp:HiddenField runat="server" ID="paramRicerca3" />
<asp:HiddenField runat="server" ID="showPopUp" />
<asp:HiddenField runat="server" ID="CodMeccanizzazioneB" />
<asp:HiddenField runat="server" ID="CodMeccanizzazioneP" />
<asp:HiddenField runat="server" ID="CodMeccanizzazioneE" />
<asp:HiddenField runat="server" ID="CodMeccanizzazioneC" />
