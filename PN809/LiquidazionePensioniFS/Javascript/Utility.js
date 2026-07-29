function closeBrowser() {
    window.open('', '_parent', '');
    top.window.close();
}

function checkCorrettezzaMese(source, args) {
    var mesi = args.Value;
    if (mesi < 0 || mesi > 11)
        args.IsValid = false;
    else
        args.IsValid = true;
    return false;
}

function checkCorrettezzaGiorno(source, args) {
    var giorno = args.Value.split("/");
    var result = checkGiorno(giorno[0], giorno[1]);
    if (result)
        args.IsValid = true;
    else
        args.IsValid = false;
    return false;
}

function checkCorrettezzaData(source, args) {
    args.IsValid = true;
    var result;

    var data = args.Value.split("/");
    if (data[0] == 0 || data[1] == 0 || data[2] == 0) {
        args.IsValid = false;
        return false;
    }

    if (data.length == 3) {
        result = checkMese(data[1]);
        if (!result) {
            args.IsValid = false;
            return false;
        }

        result = checkDate(data[1], data[0], data[2]);
        if (!result) {
            args.IsValid = false;
            return false;
        }

        if (new Date(data[2], data[1] - 1, data[0]) < new Date(1753, 0, 1)) {
            args.IsValid = false;
            return false;
        }
    }
    else {
        result = checkMese(data[0]);
        if (!result) {
            args.IsValid = false;
            return false;
        }

        result = checkDate(data[0], 1, data[1]);
        if (!result) {
            args.IsValid = false;
            return false;
        }

        if (new Date(data[1], data[0] - 1, 1) < new Date(1753, 0, 1)) {
            args.IsValid = false;
            return false;
        }
    }

    return false;
}

function checkDataPostOdiernaMMAAAA(source, args) {
    var mon1 = parseInt(args.Value.substring(0, 2), 10);
    var yr1 = parseInt(args.Value.substring(3, 7), 10);
    var d1 = new Date(yr1, mon1 - 1, 01);
    var d2 = new Date();
    var mon2 = d2.getMonth();
    var yr2 = d2.getFullYear();
    var actualDate = new Date(yr2, mon2, 01);
    var milli_d1 = d1.getTime();
    var milli_d2 = actualDate.getTime();
    var diff = milli_d1 - milli_d2;
    var num_days = (((diff / 1000) / 60) / 60) / 24;
    if (num_days > 0) {
        args.IsValid = false;
    }
}

function checkDataPostOdiernaGGMMAAAA(source, args) {
    var day = parseInt(args.Value.substring(0, 2), 10);
    var month = parseInt(args.Value.substring(3, 5), 10);
    var year = parseInt(args.Value.substring(6, 10), 10);
    var dToCheck = new Date(year, month - 1, day);
    var actual = new Date();
    var milli_dToCheck = dToCheck.getTime();
    var milli_actual = actual.getTime();
    var diff = milli_dToCheck - milli_actual;
    var numDay = (((diff / 1000) / 60) / 60) / 24;
    if (numDay > 0) {
        args.IsValid = false;
    }
    else {
        args.IsValid = true;
    }
}

function validateDate(controlToValidate, compareFunction, dataCompare) {
    var result;
    var control = document.getElementById(controlToValidate);
    if (control.disabled)
        return true;
    else {
        control = document.getElementById(controlToValidate);
        var tokens = control.value.split("/");
        var month = parseInt(tokens[0], 10);
        var year = parseInt(tokens[1], 10);

        var tokensCompare = dataCompare.split("/");
        var monthCompare = parseInt(tokensCompare[0], 10);
        var yearCompare = parseInt(tokensCompare[1], 10);

        var decLimite = new Date(yearCompare, monthCompare, 01);
        var myData = new Date(year, month - 1, 01)

        if (checkDate(month, 01, year)) {
            result = checkTimeSequence(decLimite, myData, compareFunction);
        }
    }
    return result;
}

////////////////////////////////////////////////////////////
function validateDateDay(controlToValidate, compareFunction, dataCompare) {
    var result;
    var control = document.getElementById(controlToValidate);
    if (control.disabled)
        return true;
    else {
        control = document.getElementById(controlToValidate);
        var tokens = control.value.split("/");
        var day = parseInt(tokens[0], 10);
        var month = parseInt(tokens[1], 10);
        var year = parseInt(tokens[2], 10);

        var tokensCompare = dataCompare.split("/");
        var monthCompare = parseInt(tokensCompare[0], 10);
        var yearCompare = parseInt(tokensCompare[1], 10);

        var decLimite = new Date(yearCompare, monthCompare, 01);
        var myData = new Date(year, month - 1, day)

        if (checkDate(month, day, year)) {
            result = checkTimeSequence(decLimite, myData, compareFunction);
        }
    }
    return result;
}
///////////////////////////////////////////////////////////


function validateDateToDay(controlToValidate, compareFunction) {
    var result = false;
    var control = document.getElementById(controlToValidate);
    if (control.disabled)
        return true;
    else {
        var pattern = "^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$";
        result = false;
        if (RegExpValidator(controlToValidate, pattern)) {
            control = document.getElementById(controlToValidate);
            var tokens = control.value.split("/");
            var day = parseInt(tokens[0], 10);
            var month = parseInt(tokens[1], 10);
            var year = parseInt(tokens[2], 10);
            if (checkDate(month, day, year)) {
                var now = new Date();
                var today = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
                result = checkTimeSequence(new Date(year, month - 1, day), today, compareFunction);
            }
        }
    }
    return result;
}

function checkDate(month, day, year) {
    var monthLength = new Array(31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31);
    // check for bisestile year
    if ((year % 4 == 0 && year % 100 != 0) || year % 400 == 0)
        monthLength[1] = 29;
    if (month < 1 || month > 12)
        return false;
    if (day > monthLength[month - 1] || day < 1)
        return false;
    return true;
}

function checkMese(mese) {
    if (mese < 1 || mese > 12)
        return 0;
    else
        return 1;
}

function checkGiorno(giorno, mese) {
    var ngiorni;
    if (mese == 2) {
        ngiorni = 28;
    }
    else if (mese == '04' || mese == '4' || mese == '06' || mese == '6' || mese == '09' || mese == '9' ||
        mese == '11') {
        ngiorni = 30;
    }

    else
        ngiorni = 31;

    switch (ngiorni) {
        case 28:
            if (giorno > 28) {
                return 0;
            }
            else {
                return 1;
            }
        case 30:
            if (giorno > 30) {
                return 0;
            }
            else
                return 1;
        case 31:
            if (giorno > 31)
                return 0;
            else
                return 1;
    }
    return false;
}

function checkTimeSequence(date1, date2, compareFunction) {
    return compareFunction(date1, date2);
}

function validateDateInThePast(source, args) {
    var controlToValidate = source.id.replace("_CV", "");
    var compareFunction = function (a, b) { return a <= b; };
    args.IsValid = validateDate(controlToValidate, compareFunction, args.dataCompare);
    return false;
}

////////////////////////////////////////////////////////////////////////
function validateDateInThePastWithDay(source, args) {
    var controlToValidate = source.id.replace("_CV", "");
    var compareFunction = function (a, b) { return a <= b; };
    args.IsValid = validateDateDay(controlToValidate, compareFunction, args.dataCompare);
    return false;
}
////////////////////////////////////////////////////////////////////////

function validateDateForToDay(source, args) {
    var controlToValidate = source.id.replace("_CV", "");
    var compareFunction = function (a, b) { return a <= b; };
    args.IsValid = validateDateToDay(controlToValidate, compareFunction);
    return false;
}

function checkRegexp(o, regexp) {
    if (!(regexp.test(o.toString())))
        return false;
    else
        return true;
}

function convertString2Date(strDate) {
    var giorno = parseInt(strDate.substring(0, 2), 10);
    var mese = parseInt(strDate.substring(3, 5), 10);
    var anno = parseInt(strDate.substring(6, 10), 10);
    var d1 = new Date(anno, mese - 1, giorno);
    return d1;
}

function convertDate2String(date) {
    var giorno = date.getDay();
    var y = date.getFullYear();
    if (y.toString().length < 4)
        y = padLeft(y, 4);
    var m = date.getMonth() + 1;
    if (m.toString().length < 2)
        m = padLeft(m, 2);
    var day = date.getDate();
    if (day.toString().length < 2)
        day = padLeft(day, 2);
    var strDate = day + "/" + m + "/" + y;
    return strDate;
}

function padLeft(number, length) {

    var str = '' + number;
    while (str.length < length) {
        str = '0' + str;
    }

    return str;

}

function CalcolaDataInteressiLegaliNew(dataCompletezza, dataDecorrenza, dataDomanda, hdnSetDataInteressiLegaliRicCumulo) {
    
    var dataPrevista = "";
    dataDomanda = convertString2Date(dataDomanda);
    if (dataDecorrenza.length == 10)
        dataDecorrenza = convertString2Date(dataDecorrenza);
    else
        dataDecorrenza = convertString2Date('01/' + dataDecorrenza);

    if ((dataDomanda.getTime() >= dataDecorrenza.getTime())) {
        dataPrevista = dataDomanda;
    }
    else {
        dataPrevista = dataDecorrenza;
    }

    if (dataCompletezza != "" && dataCompletezza.toUpperCase() != "GG/MM/AAAA") {
        dataCompletezza = convertString2Date(dataCompletezza);
        if ((dataCompletezza.getTime() >= dataDecorrenza.getTime()) || hdnSetDataInteressiLegaliRicCumulo == "true") {
            dataPrevista = dataCompletezza;
        }
    }

    if (hdnSetDataInteressiLegaliRicCumulo == "true")
        dataPrevista.setDate(dataPrevista.getDate() + 30);
    else
        dataPrevista.setDate(dataPrevista.getDate() + 121);

    return convertDate2String(dataPrevista);
}

function CalcolaDataInteressiLegaliNewINPDAP(dataCompletezza, dataDecorrenza) {
    var dataPrevista = "";
    dataDecorrenza = convertString2Date(dataDecorrenza);

    dataPrevista = dataDecorrenza;

    if (dataCompletezza != "" && dataCompletezza.toUpperCase() != "GG/MM/AAAA") {
        dataCompletezza = convertString2Date(dataCompletezza);
        if ((dataCompletezza.getTime() >= dataDecorrenza.getTime())) {
            dataPrevista = dataCompletezza;
        }
    }
    dataPrevista.setDate(dataPrevista.getDate() + 31);
    return convertDate2String(dataPrevista);
}

function checkLunghezzaNome(source, args) {
    if (args.Value.length < 3)
        args.IsValid = false;
    else
        args.IsValid = true;
    return false;
}

function validateDropDownList(source, args) {
    var result = false;
    var controlToValidate = source.id.replace("_CV", "");
    var control = document.getElementById(controlToValidate);
    if (control.disabled)
        result = true;
    else {
        var pattern = '^[\S]*$';
        result = !(RegExpValidator(controlToValidate, pattern));
    }
    args.IsValid = result;
    return false;
}

function RegExpValidator(controlToValidate, pattern) {
    var control = document.getElementById(controlToValidate);
    var value = control.value;
    var rx = new RegExp(pattern);
    var matches = rx.exec(value);
    return (matches != null && value == matches[0]);
}

function validateCodiceFiscale(source, args) {
    var result = false;
    var controlToValidate = source.id.split("_CV");
    var control = document.getElementById(controlToValidate[0]);
    if (control.disabled)
        result = true;
    else {
        var pattern = '^([A-Za-z]{6}[0-9lmnpqrstuvLMNPQRSTUV]{2}[abcdehlmprstABCDEHLMPRST]{1}[0-9lmnpqrstuvLMNPQRSTUV]{2}[A-Za-z]{1}[0-9lmnpqrstuvLMNPQRSTUV]{3}[A-Za-z]{1})$';
        result = RegExpValidator(controlToValidate[0], pattern);
    }
    args.IsValid = result;
    return false;
}

function validateDataSequence(source, args) {
    var result = false;
    var controlToValidate = source.id.replace("_CV", "");
    var controlMin = document.getElementById(controlToValidate + "Min");
    var controlMax = document.getElementById(controlToValidate + "Max");
    if (controlMin.disabled && controlMax.disabled)
        result = true;
    else {
        var tokensMin = controlMin.value.split("/");
        var dayMin = parseInt(tokensMin[0], 10);
        var monthMin = parseInt(tokensMin[1], 10);
        var yearMin = parseInt(tokensMin[2], 10);
        if (!checkDate(monthMin, dayMin, yearMin))
            result = true;
        else {
            var tokensMax = controlMax.value.split("/");
            var dayMax = parseInt(tokensMax[0], 10);
            var monthMax = parseInt(tokensMax[1], 10);
            var yearMax = parseInt(tokensMax[2], 10);
            if (!checkDate(monthMax, dayMax, yearMax))
                result = true;
            else {
                var compareFunction = function (a, b) { return a <= b; }
                result = checkTimeSequence(new Date(yearMin, monthMin - 1, dayMin), new Date(yearMax, monthMax - 1, dayMax), compareFunction);
            }
        }
    }
    args.IsValid = result;
    return false;
}

function validateData(source, args) {
    var tokens = args.Value.split("/");
    var day = parseInt(tokens[0], 10);
    var month = parseInt(tokens[1], 10);
    var year = parseInt(tokens[2], 10);
    args.IsValid = checkDate(month, day, year);
    return false;
}

function validateCognomeNome(source, args) {
    var result = false;
    var controlToValidate = source.id.replace("_CV", "");
    var control = document.getElementById(controlToValidate);
    if (control.disabled)
        result = true;
    else {
        var pattern = "^[\x20a-zA-Z ']{3,}$";
        result = RegExpValidator(controlToValidate, pattern);
    }
    args.IsValid = result;
    return false;
}

function validateNumeroDomanda(source, args) {
    var result = false;
    var controlToValidate = source.id.replace("_CV", "");
    var control = document.getElementById(controlToValidate);
    if (control.disabled)
        result = true;
    else {
        var pattern = '^[1-9]{1}[0-9]{12}$';
        result = RegExpValidator(controlToValidate, pattern);
    }
    args.IsValid = result;
    return false;
}

function validateNumeroCertificato(source, args) {
    var result = false;
    var controlToValidate = source.id.replace("_CV", "");
    var control = document.getElementById(controlToValidate);
    if (control.disabled)
        result = true;
    else {
        var pattern = '^[0-9]{1,8}$';
        result = RegExpValidator(controlToValidate, pattern);
    }
    args.IsValid = result;
    return false;
}

function validateMatricola(source, args) {
    var result = false;
    var controlToValidate = source.id.replace("_CV", "");
    var control = document.getElementById(controlToValidate);
    if (control.disabled)
        result = true;
    else {
        var pattern = '^[0-9A-Za-z]{8}$';
        result = RegExpValidator(controlToValidate, pattern);
    }
    args.IsValid = result;
    return false;
}

function checkddlTipoSupplementi(source, args) {
    if (args.Value == 1)
        args.IsValid = true;
    else
        args.IsValid = false;
    return false;
}

function checkddlQuotaSupplementi(source, args) {
    if (args.Value == "B")
        args.IsValid = true;
    else
        args.IsValid = false;
    return false;
}

function validateSede(source, args) {
    var controlToValidate = source.id.replace("_CV", "");
    var control = document.getElementById(controlToValidate);
    if (control.disabled) {
        args.IsValid = true;
        return;
    }
    else {
        if (control.value == "") {
            args.IsValid = false;
            return;
        }

        var idHiddenFieldSedi = document.getElementById("hfClientID").value;
        if (document.getElementById(idHiddenFieldSedi) != null) {
            var availableTags = document.getElementById(idHiddenFieldSedi).value.split(';');
            for (var i = 0; i < availableTags.length; i++) {
                if (control.value.toUpperCase() == availableTags[i]) {
                    args.IsValid = true;
                    return;
                }
            }
        }
    }
    args.IsValid = false;
    return;
}

function validateSedeCodeOptional(source, args) {
    var controlToValidate = source.id.replace("_CV", "");
    var control = document.getElementById(controlToValidate);
    if (control.disabled || control.value == "") {
        args.IsValid = true;
        return;
    }
    else {
        var hidden = "ctl00_ContentPlaceHolder1_HiddenFieldSedi";
        if (document.getElementById(hidden) != null && document.getElementById(hidden).value != "") {
            var availableTags = document.getElementById(hidden).value.split(';');
            for (var i = 0; i < availableTags.length; i++) {
                if (control.value.toUpperCase() == availableTags[i]) {
                    args.IsValid = true;
                    return;
                }
            }
        }
    }
    args.IsValid = false;
    return;
}

function validateSedeCodeMandatory(source, args) {
    var controlToValidate = source.id.replace("_CV", "");
    var control = document.getElementById(controlToValidate);
    if (control.disabled) {
        args.IsValid = true;
        return;
    }
    else {
        if (control.value == "") {
            args.IsValid = false;
            return;
        }
        var hidden = "ctl00_ContentPlaceHolder1_HiddenFieldSedi";
        if (document.getElementById(hidden) != null && document.getElementById(hidden).value != "") {
            var availableTags = document.getElementById(hidden).value.split(';');
            for (var i = 0; i < availableTags.length; i++) {
                if (control.value.toUpperCase() == availableTags[i]) {
                    args.IsValid = true;
                    return;
                }
            }
        }
    }
    args.IsValid = false;
    return;
}


function checkSede(val, args) {
    if (args.Value == "") {
        args.IsValid = false;
        return;
    }
    if (document.getElementById("<%=HiddenFieldSedi.ClientID%>") != null) {
        var availableTags = document.getElementById("<%=HiddenFieldSedi.ClientID%>").value.split(';');
        for (var i = 0; i < availableTags.length; i++) {
            if (args.Value.toUpperCase() == availableTags[i]) {
                return;
            }
        }
    }
    args.IsValid = false;
    return;
}

var documentAttendere = {
    wait: function (idDiv, idDivImg) {
        var color = 'white';
        var opacity = 0.8;
        //var o1 = document.getElementById(idDiv);
        var o1 = idDiv;
        o1.cloneNode(0);

        var o = o1.cloneNode(0);
        o.style.background = color;
        document.createElement('div');
        o.id = "divWait1";
        documentAttendere.style(o, {
            position: 'absolute',
            //width: document.getElementById(idDiv).offsetWidth + 'px',
            //height: document.getElementById(idDiv).offsetHeight + 'px',
            width: idDiv.offsetWidth + 'px',
            height: idDiv.offsetHeight + 'px',
            background: color,
            zIndex: 1000,
            opacity: opacity,

            filter: 'alpha(opacity=' + opacity * 100 + ')'
        });


        o1.style.zIndex = "-1";
        o1.parentNode.insertBefore(o, o1.parentNode.firstChild);

        //document.getElementById(idDivImg).style.display = 'block';
        //document.getElementById(idDivImg).innerHTML = document.getElementById(idDivImg).innerHTML;
        idDivImg.style.display = 'block';
        //idDivImg.innerHTML = idDivImg.innerHTML;
    },
    style: function (obj, s) {
        for (var i in s) {
            obj.style[i] = s[i];
        }
    }
}

function mainValidate() {
    if (!validatePage())
        return;
    aspnetForm.target = '_self';
    BlockUI();
}

function mainValidateForConfirm() {
    if (!validatePage())
        return false;
    return true;
}

function BlockUI() {
    $.blockUI({ message: "<br /><p>Attendere Elaborazione in corso...</p><br />" });
}


function LoadSelectedTab(firstTab) {
    //When page loads...
    var selectedTab = $('input[id$="hdnSelected"]').val();

    $(".tab_content").hide(); //Hide all content
    if (firstTab) {
        $("ul.tabs li:first").addClass("active").show(); //Activate first tab
        $(".tab_content:first").show(); //Show first tab content
    }
    $("a[href=" + "" + selectedTab + "" + "]").parent().addClass("active");
    $(selectedTab).show();
}

function LoadClickTab(source, isFlex = false, pageTheme = "") {
    $("ul.tabs li").removeClass("active"); //Remove any "active" class
    $(source).addClass("active"); //Add "active" class to selected tab
    $(".tab_content").hide(); //Hide all tab content
    var activeTab = $(source).find("a").attr("href"); //Find the href attribute value to identify the active tab + content
    $(activeTab).fadeIn('fast'); //Fade in the active ID content
    if (isFlex) {
        $(activeTab).css("display", pageTheme == "iFrame" ? "flex" : "block");
    }
    $('input[id$="hdnSelected"]').val(activeTab);
    return activeTab;
}

function OpenNewPage(page) {
    window.open(page, '', '');
}

// inizio Liquidazione Pensione

function SetCheckBoxCentralizzata(cb, aoiChecked) {
    $('.offClass').val(''); //Pulisce tutti i campi con la class "offClass"
    $('.' + cb.getAttribute("EnableClass")).removeAttr('disabled'); //Abilita gli oggetti con l'attributo specificato
    if (cb.getAttribute("EnableClass") == "onClassTrasfAOI")
        EnableDisableTab(aoiChecked);
}

function EnableDisableTab(aoiChecked) {
    if (aoiChecked)
        AbilitaTab();       //funzione implementata nella pagina padre (LiquidazionePensione.aspx)
    else
        DisabilitaTab();    //funzione implementata nella pagina padre (LiquidazionePensione.aspx)
}

function getDDLCodNaturaCentralizzata(value) {
    //controllo che l'elemento esista
    var element = document.getElementById(value);
    if (!element)
        return element;
    var IndexValue = document.getElementById(value).selectedIndex;
    var SelectedVal = document.getElementById(value).options[IndexValue].value;
    return SelectedVal;
}

function getDDLCodNaturaValueCentralizzata(ddl) {
    var codNatura1Value = $($("table[id*=gvRecordFondo] select[id*=" + ddl + "]")).val();
    return codNatura1Value;
}

function setDDLCodNaturaCentralizzata(value) {
    return document.getElementById(value);
}


function getElemCentralizzata(value) {
    return document.getElementById(value);
}

function setSelectedIndexCentralizzata(s, v) {
    for (var i = 0; i < s.options.length; i++) {
        if (s.options[i].value == v) {
            s.options[i].selected = true;
            return;
        }
    }
}

function setTxtCentralizzata(t, v) {
    if (t != null && t != undefined)
        t.value = v;
}



// fine Liquidazione Pensione

function CalcolaDataDecorrenzaByDataEvento(sDataEvento) {

    var parts = sDataEvento.split("/");
    var year = parts[2];
    var month = parts[1];
    var day = parts[0];
    var formattedDataEvento = year + "/" + month + "/" + day;

    if (!Date.parse(formattedDataEvento)) {
        return "";
    }
    var dataEvento = convertString2Date(sDataEvento);
    var dateDecorrenza = new Date(dataEvento.getFullYear(), dataEvento.getMonth(), 1);
    dateDecorrenza.setMonth(dateDecorrenza.getMonth() + 1);
    return convertDate2String(dateDecorrenza).substring(3);
}

function createPagination(nPage, pageNumber, pageSize, nElem, cssClass, nomeFunctionSetPage) {
    var firstElem = (pageSize * (pageNumber - 1)) + 1;
    var lastElem = pageSize * pageNumber;
    if (lastElem > nElem)
        lastElem = nElem;
    var elenco = "<div class='" + cssClass + "NavPage' style='display: inline; font-size: 15px; padding-right: 13px'>" + firstElem + " - " + lastElem + " di " + nElem + "</div>";
    var i;
    if (nPage > 7) {
        if (pageNumber < 5) {
            for (i = 1; i < 6; i++) {
                elenco = elenco + "<input type='button' class='" + cssClass + "' name='" + i + "' value='" + i + "' onclick='" + nomeFunctionSetPage + "(" + i + ")' ></input>";
            }
            elenco = elenco + "<input type='button' class='" + cssClass + "' value='...' style='disabled'></input>";
            elenco = elenco + "<input type='button' class='" + cssClass + "' name='" + nPage + "' value='" + nPage + "' onclick='" + nomeFunctionSetPage + "(" + nPage + ")'></input>";
        }

        if (pageNumber > +nPage - 4) {
            elenco = elenco + "<input type='button' class='" + cssClass + "' name='1' value='1' onclick='" + nomeFunctionSetPage + "(1)' ></input>";
            elenco = elenco + "<input type='button' class='" + cssClass + "' value='...' style='disabled'></input>";
            for (i = +nPage - 4; i < +nPage + 1; i++) {
                elenco = elenco + "<input type='button' class='" + cssClass + "' name='" + i + "' value='" + i + "' onclick='" + nomeFunctionSetPage + "(" + i + ")' ></input>";
            }
        }

        if (pageNumber > 4 && pageNumber < +nPage - 3) {
            elenco = elenco + "<input type='button' class='" + cssClass + "' name='1' value='1' onclick='" + nomeFunctionSetPage + "(1)' ></input>";
            if (pageNumber > 4)
                elenco = elenco + "<input type='button' class='" + cssClass + "' value='...' style='disabled'></input>";
            if (pageNumber == 4)
                elenco = elenco + "<input type='button' class='" + cssClass + "' name='2' value='2' onclick='" + nomeFunctionSetPage + "(2)' ></input>";
            for (i = +pageNumber - 1; i < +pageNumber + 2; i++) {
                elenco = elenco + "<input type='button' class='" + cssClass + "' name='" + i + "' value='" + i + "' onclick='" + nomeFunctionSetPage + "(" + i + ")' ></input>";
            }
            if (pageNumber < +nPage - 3)
                elenco = elenco + "<input type='button' class='" + cssClass + "' value='...' style='disabled'></input>";
            if (pageNumber == +nPage - 3)
                elenco = elenco + "<input type='button' class='" + cssClass + "' name='" + (+nPage - 1) + "' value='" + (+nPage - 1) + "' onclick='" + nomeFunctionSetPage + "(" + (+nPage - 1) + ")'></input>";
            elenco = elenco + "<input type='button' class='" + cssClass + "' name='" + nPage + "' value='" + nPage + "' onclick='" + nomeFunctionSetPage + "(" + nPage + ")'></input>";
        }
    }
    else {
        for (i = 1; i < +nPage + 1; i++) {
            elenco = elenco + "<input type='button' class='" + cssClass + "' name='" + i + "' value='" + i + "' onclick='" + nomeFunctionSetPage + "(" + i + ")' ></input>";
        }
    }

    return elenco;
}

function cancelBack() {
    if (event.keyCode == 8 || (event.keyCode == 37 && event.altKey) || (event.keyCode == 39 && event.altKey)) {
        if (event.srcElement != null && (event.srcElement.type == "text" || event.srcElement.type == "textarea") && !event.srcElement.readOnly)
            return;
        else if (event.srcElement.form == null || event.srcElement.isContentEditable == false) {
            event.cancelBubble = true;
            event.returnValue = false;
        }
    }
}

function cancelBackFF(event) {
    if (event.keyCode == 8 || (event.keyCode == 37 && event.altKey) || (event.keyCode == 39 && event.altKey)) {
        if (event.originalTarget != null && (event.originalTarget.type == "text" || event.originalTarget.type == "textarea") && !event.originalTarget.readOnly)
            return;
        else if (event.originalTarget.form == null || event.originalTarget.isContentEditable == false) {
            //event.cancelBubble = true;
            event.stopPropagation();
            event.returnValue = false;
        }
    }
}

function DisabilitaValidatore(validator) {
    validator.isvalid = true;
    validator.enabled = false;
    ValidatorUpdateDisplay(validator);
}

function AbilitaValidatore(validator) {
    validator.isvalid = true;
    validator.enabled = true;
    ValidatorUpdateDisplay(validator);
}

function GestioneVisibilitaPannelliRepeater() {
    const currentTheme = document.getElementById('ctl00_UcTestata_hCurrentTheme').value;
    $("[src*=plus]").live("click", function () {
        $(this).closest("tr").next().show(); //after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>");
        $(this).attr("src", "../App_Themes/" + currentTheme + "/Images/minus.png");
    });
    $("[src*=minus]").live("click", function () {
        $(this).attr("src", "../App_Themes/" + currentTheme + "/Images/plus.png");
        $(this).closest("tr").next().hide();
    });
}

function GestioneVisibilitaPannelliGridView() {
    var elem = $("input[type='hidden'][id*=hdnVisualizzaTrattenute]");
    if (elem.length > 0) {
        var i;
        for (i = 0; i < elem.length; i++) {
            if ($(elem[i]).val() == 'SI') {
                $(elem[i]).closest("tr").next().show();
                $(">td", $(elem[i]).closest("tr").next()).attr("colspan", $(">td", $(elem[i]).closest("tr")).length);
                $("[src*=plus]", $(elem[i]).closest("tr")).attr("src", "../App_Themes/BlueINPS1/Images/minus.png");
            }
        }
    }
    $("[src*=plus]").live("click", function () {
        $("input[type='hidden'][id*=hdnVisualizzaTrattenute]", $(this).closest("tr")).val("SI");
        $(this).closest("tr").next().show();
        $(">td", $(this).closest("tr").next()).attr("colspan", $(">td", $(this).closest("tr")).length); //after("<tr><td colspan = '" + ($(">td", $(this).closest("tr")).length) + "'>" + $(this).next().html() + "</td></tr>");
        $(this).attr("src", "../App_Themes/BlueINPS1/Images/minus.png");
    });
    $("[src*=minus]").live("click", function () {
        $("input[type='hidden'][id*=hdnVisualizzaTrattenute]", $(this).closest("tr")).val("NO");
        $(this).attr("src", "../App_Themes/BlueINPS1/Images/plus.png");
        $(this).closest("tr").next().hide();
    });
}

function getRequisitiAnte247Centralizzata(value) {
    //controllo che l'elemento esista
    var element = document.getElementById(value);
    if (!element)
        return element;
    var IndexValue = document.getElementById(value).selectedIndex;
    var SelectedVal = document.getElementById(value).options[IndexValue].value;
    return SelectedVal;
}

