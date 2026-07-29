<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiGenericiINPDAP.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCDatiGenericiINPDAP" %>
<!--script-->
<script type="text/javascript">
    $(document).ready(function () {
        getDDLCodNatura2Value();
        DisabilitaTab();

        if (document.getElementById("<%=txtInteressiLegali.ClientID %>").value == "" || document.getElementById("<%=HiddenIntLeg.ClientID %>").value == "")
            setDataInteressiLegali();

        var aoiChecked = $(document.getElementById("<%=chkTrasfAOI.ClientID %>")).attr("checked");
        EnableDisableTab(aoiChecked);

        if (document.getElementById("<%=pnlRequisitiAnte247.ClientID%>") != null && document.getElementById("<%=pnlRequisitiAnte247.ClientID%>") != undefined)
        {
            SetTrimestreRequisiti();
            $("#<%=ddlReqAnte247.ClientID %>").change(function () { SetTrimestreRequisiti(); });
        }

        if (document.getElementById("<%= HiddenFieldIsRicostituzione.ClientID %>").value == "NO") {
            SetCalendarioElContDataEvento();
        }

        var txtDataEvento = document.getElementById("<%= txtElContDataEvento.ClientID %>")
        txtDataEvento.onblur = setDataDecorrenza;
        setDataDecorrenza();

        var isRICPost20022022 = document.getElementById("<%=HiddenFieldIsRICPost20022022.ClientID%>").value;
        if (isRICPost20022022 == "SI") {
            var ControlName = document.getElementById("<%=ddlTrattINPDAP.ClientID%>");
            ControlName.remove("");
            if (ControlName.value == "NO")
                document.getElementById('<%= txtDecTrattINPDAP.ClientID %>').setAttribute("disabled", true);
        }

        if (document.getElementById("<%=HiddenPrecedentePensione.ClientID%>") != null && document.getElementById("<%=HiddenPrecedentePensione.ClientID%>").value == "true") {
            $(document.getElementById("<%=chkTrasfAOI.ClientID %>")).attr("checked", true);
            AbilitaTab();
        }
    });

    function getRequisitiAnte247() {
        return getRequisitiAnte247Centralizzata('<%=ddlReqAnte247.ClientID %>');
    }

    function setDDLRequisitiAnte247Value(valore, RequisitiAnte247) {
        var elem =  getElemCentralizzata("<%=ddlTrimestreRequisiti.ClientID %>");     
        if (RequisitiAnte247 == "NO") {   
            $('#<%= hdnTrimesteRequisiti.ClientID %>').val(valore);
            setTxtCentralizzata(elem, valore);
            elem.disabled=true;
        }
        else {
            $('#<%= hdnTrimesteRequisiti.ClientID %>').val('');
            elem.disabled=false;
        }
    }

    function setTxtRequisitiAnte247Value(valore, RequisitiAnte247) {
        var elem =  getElemCentralizzata("<%=txtTrimestreRequisiti.ClientID %>");       
        if (RequisitiAnte247 == "NO") {
            $('#<%= hdnTrimesteRequisitiAnno.ClientID %>').val(valore);
            setTxtCentralizzata(elem, valore);
            elem.disabled=true;
        }
        else {
            $('#<%= hdnTrimesteRequisitiAnno.ClientID %>').val('');
            elem.disabled=false;
        }
    }

    function SetCheckBox(cb) {
        var aoiChecked = $(document.getElementById("<%=chkTrasfAOI.ClientID %>")).attr("checked");
        SetCheckBoxCentralizzata(cb, aoiChecked);
    }

    function getDDLCodComunicazioni1Value() {
        var IndexValue = document.getElementById('<%=ddlCodComunicazioni1.ClientID %>').selectedIndex;
        var SelectedVal = document.getElementById('<%=ddlCodComunicazioni1.ClientID %>').options[IndexValue].value;
        if (SelectedVal == "1") {
            //modificare a none non appena si hanno info sui valori che può assumere il secondo campo di cod comunicazioni
            //in caso di textbox 
            document.getElementById('<%=ddlCodComunicazioni2.ClientID %>').style.display = "block";

            //modificare a block non appena si hanno info sui valori che può assumere il secondo campo di cod comunicazioni
            //in caso di textbox 
            document.getElementById('<%=txtCodComunicazioni2.ClientID %>').style.display = "none";
        }
        else {
            document.getElementById('<%=ddlCodComunicazioni2.ClientID %>').style.display = "block";
            document.getElementById('<%=txtCodComunicazioni2.ClientID %>').style.display = "none";
        }
    }

    function getDDLCodNatura1Value() {

        return getDDLCodNaturaCentralizzata('<%=ddlCodNatura1DG.ClientID %>');
    }

    function getDDLCodNatura2Value() {
        var SelectedVal = getDDLCodNaturaCentralizzata('<%=ddlCodNatura2DG.ClientID %>');
        if (SelectedVal == "Y") {
            document.getElementById('<%=trBonus2432004.ClientID %>').style.display = "table-row";
        }
        else {
            document.getElementById('<%=trBonus2432004.ClientID %>').style.display = "none";
        }
        return SelectedVal;
    }

    function getDDLCodNatura3Value() {
        return getDDLCodNaturaCentralizzata('<%=ddlCodNatura3DG.ClientID %>');
    }

    function setDDLCodNatura1Value(valore) {
        setSelectedIndexCentralizzata(setDDLCodNaturaCentralizzata("<%=ddlCodNatura1DG.ClientID %>"), valore);
    }

    function setDDLCodNatura2Value(valore) {
        setSelectedIndexCentralizzata(setDDLCodNaturaCentralizzata("<%=ddlCodNatura2DG.ClientID %>"), valore);
    }

    function setDDLCodNatura3Value(valore) {
        setSelectedIndexCentralizzata(setDDLCodNaturaCentralizzata("<%=ddlCodNatura3DG.ClientID %>"), valore);
    }

    function getDataInizioBonus() {
        if ($("#<%=txtDataInizioBonus.ClientID %>")) {
            return $("#<%=txtDataInizioBonus.ClientID %>").val();
        }
    }

    function GetCodNatura3() {
        var codNatura3 = document.getElementById("<%=ddlCodNatura3DG.ClientID%>").value;
        return codNatura3;
    }

    function GetCodNatura2() {
        var codNatura2 = document.getElementById("<%=ddlCodNatura2DG.ClientID%>").value;
            return codNatura2;
        }

    function GetCodNatura1() {
        var codNatura1 = document.getElementById("<%=ddlCodNatura1DG.ClientID%>").value;
        return codNatura1;
    }

    function EnableBenefici(isBeneficiEnabled) {
        var benefici = document.getElementById("<%= chkBenefici.ClientID %>");
        if (benefici) {
            if (isBeneficiEnabled) {
                benefici.disabled = true;
                benefici.checked = true;
            }
            else {
                if (hdnOpzioneDonna == "false") {
                    benefici.disabled = false;
                }
            }
            document.getElementById("<%= HiddenFieldChkBenefici.ClientID %>").value = benefici.checked;
        }
    }

    function GetEnteCassa() {
        return 0;
    }

    function SetCheckBox(cb) {
        $('.offClass').val(''); //Pulisce tutti i campi con la class "offClass"
        $('.' + cb.getAttribute("EnableClass")).removeAttr('disabled'); //Abilita gli oggetti con l'attributo specificato
    }

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
                document.getElementById("<%=txtInteressiLegali.ClientID %>").value = CalcolaDataInteressiLegaliNewINPDAP(dateText, document.getElementById("<%=lblDecorrenzaPensioneData.ClientID %>").innerText);
                document.getElementById("<%=HiddenIntLeg.ClientID %>").value = document.getElementById("<%=txtInteressiLegali.ClientID %>").value;
            }
        });
    });

    function setDataInteressiLegali() {
        var dataCompletezza = document.getElementById("<%=txtDataCompletezza.ClientID %>") != null ? document.getElementById("<%=txtDataCompletezza.ClientID %>").value : "";
        var dataDecorrenza = document.getElementById("<%=lblDecorrenzaPensioneData.ClientID %>").innerText;
        document.getElementById("<%=txtInteressiLegali.ClientID %>").value = CalcolaDataInteressiLegaliNewINPDAP(dataCompletezza, dataDecorrenza);
        document.getElementById("<%=HiddenIntLeg.ClientID %>").value = document.getElementById("<%=txtInteressiLegali.ClientID %>").value;
    }

    function setHdnInteressiLegali() {
        var hdnInteressiLegali = $("#<%= HiddenIntLeg.ClientID %>");
        var txtInteressiLegali = $("#<%= txtInteressiLegali.ClientID %>");

        hdnInteressiLegali.val(txtInteressiLegali.val());
    }


    function SetCalendarioElContDataEvento() {
        //date-picker-base
        $(document.getElementById("<%=txtElContDataEvento.ClientID%>")).datepicker({
             changeMonth: true,
             changeYear: true,
             changeDay: true,
             showButtonPanel: true,
             dateFormat: 'dd/mm/yy',
             showOn: 'button',
             buttonImageOnly: true,
             buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
             minDate: '-100y',
             maxDate: '+0',
             yearRange: '-100:' + '+0:',
             onSelect: function (text, ins) { setDataDecorrenza(); }
         });
    }

    function setDataDecorrenza() {
        var dataEvento = document.getElementById("<%=txtElContDataEvento.ClientID %>") != null ? document.getElementById("<%=txtElContDataEvento.ClientID %>").value : "";
        var sDateDec = CalcolaDataDecorrenzaByDataEvento(dataEvento);
        document.getElementById("<%=txtElContDecorrenza.ClientID %>").value = sDateDec;
        document.getElementById("<%=HiddenFieldDecorrenza.ClientID %>").value = sDateDec;
    }

    function setDataINPDAP() {
        var ControlName = document.getElementById("<%=ddlTrattINPDAP.ClientID%>");
        var isRICPost20022022 = document.getElementById("<%=HiddenFieldIsRICPost20022022.ClientID%>").value;

        if (isRICPost20022022 == "SI") {
            if (ControlName.value == "SI")
                ShowPopupTrattINPDAP();
            else if (ControlName.value == "NO") {
                document.getElementById('<%= txtDecTrattINPDAP.ClientID %>').value = "MM/AAAA";
                document.getElementById('<%= txtDecTrattINPDAP.ClientID %>').setAttribute("disabled", true);
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

    
    function CreatePopUp(callbackOk) {
        $('#dialog-esenzione').text("Attenzione, si sta rimuovendo l’esenzione fiscale per vittime del dovere");

        // pulizia
        if ($('#dialog-esenzione').data('ui-dialog')) {
            $('#dialog-esenzione').dialog('destroy');
        }

        $('#dialog-esenzione').dialog({
            autoOpen: false,
            width: 400,
            modal: true,
            resizable: false,
            draggable: false,
            buttons: {
                "Annulla": function () {
                    $(this).dialog("close");
                },
                "Ok": function () {
                    $(this).dialog("close");
                    if (typeof callbackOk === "function") callbackOk();
                }
            }
        });

        $("#dialog-esenzione").parent().appendTo($("form:first"));
    }

    function ShowPopUpEsenzione() {
        var ddl = document.getElementById('<%= ddlCodComunicazioni4.ClientID %>');
        var isRic = document.getElementById('<%= hdnIsRicEsenzioneFiscaleVittimeDelDovere.ClientID %>').value;

        var showPopup = (isRic === "SI" && ddl && ddl.value == 0);

        if (showPopup) {
            
            CreatePopUp(function () {
                // callback eseguito quando l'utente clicca "Ok"
                if (Page_ClientValidate('UCTabDatiGenerici')) {
                    aspnetForm.target = '_self';
                    BlockUI();
                    __doPostBack('<%= btnSalvaDatiGenerici.UniqueID %>', '');
                } 
            });

            $('#dialog-esenzione').dialog('open');
            return false;
        }
        else
        {
            if (Page_ClientValidate('UCTabDatiGenerici')) {
                aspnetForm.target = '_self';
                BlockUI();
                return true; 
            }
            else {
                return false; 
            }
            }
    }


</script>
<!-- Fine script-->
<!-- Pannello common header-->
<asp:Panel runat="server" ID="pnlCommonHeader" Visible="false">
    <asp:Panel runat="server" ID="pnlDecorrenzaGiuridica" Visible="false">
        <table class="tabellaFormattazione grid grid-size-25-col-2" style="width: 100%">
            <tr>
                <td class="Row1" style="width: 25%">
                    <asp:Label runat="server" ID="lblDecorrenzaPensione" Text=""></asp:Label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:Label runat="server" ID="lblDecorrenzaPensioneData" Text=""></asp:Label>
                </td>
                <td class="Row1" style="width: 25%"></td>
                <td class="field" style="width: 25%"></td>
            </tr>
        </table>
    </asp:Panel>
</asp:Panel>
<!--Fine pannello common header-->
<!--Pannello Common-->
<asp:Panel ID="pnlCommon" runat="server" Visible="false">
    <table class="tabellaFormattazione grid grid-size-25" style="width: 100%">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Decorrenza Arretrati:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaArretrati"
                    Width="50%" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="3"
                    Text="" MaxLength="10"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateDecorrenzaArretrati" ControlToValidate="txtDecorrenzaArretrati"
                    ErrorMessage="Inserire la data nel formato valido per  Decorrenza Arretrati"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiGenerici"
                    Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaArretrati" Display="Dynamic"
                    ErrorMessage="Decorrenza Arretrati: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="customCheckDataDecorrenzaArretrati" ClientValidationFunction="checkCorrettezzaData" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Accantonamento Arretrati:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList runat="server" ID="ddlCodiciArretrati" Width="75px" CssClass="tb8 txtUppercase xxs"
                    TabIndex="4">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="1 - NO" Value="1"></asp:ListItem>
                    <asp:ListItem Text="8 - SI" Value="8"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlCodiceArretrati_RF" ControlToValidate="ddlCodiciArretrati"
                    Display="Dynamic" Enabled="true" ErrorMessage="Accantonamento Arretrati: si prega di inserire il codice"
                    ValidationGroup="UCTabDatiGenerici" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr runat="server" id="TrScadenzaRevisioneSanitaria" visible="false">
            <td class="Row1" style="width: 25%">
                <label>
                    Scadenza Revisione Sanitaria:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtScadRevSanitaria" Width="16.5%"
                    CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" TabIndex="5" Text="gg/mm/aaaa"
                    MaxLength="10"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateScadRevSanitaria" ControlToValidate="txtScadRevSanitaria"
                    ErrorMessage="Inserire la data nel formato valido per Scadenza Revisione Sanitaria"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiGenerici"
                    Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtScadRevSanitaria" Display="Dynamic"
                    ErrorMessage="Scadenza Revisione Sanitaria: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="customCheckDataScadRevSanitaria" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Data Completezza:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataCompletezza" Width="50%"
                    CssClass="txtUppercase tb8 date-picker-dataCompletezza dateGGmmAAAA" TabIndex="6"
                    Text="" MaxLength="10"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateDataCompletezza" ControlToValidate="txtDataCompletezza"
                    Display="Dynamic" ErrorMessage="Inserire la data nel formato valido per Data Completezza"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    ValidationGroup="UCTabDatiGenerici" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataCompletezza" Display="Dynamic"
                    ErrorMessage="Data Completezza: Data inserita posteriore a quella odierna" Text="*" CssClass="field-is-required"
                    ValidationGroup="UCTabDatiGenerici" ID="customDataCompletezza" ClientValidationFunction="checkDataPostOdiernaGGMMAAAA" />
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldtxtDataCompletezza" ControlToValidate="txtDataCompletezza"
                    Display="Dynamic" Enabled="true" ErrorMessage="Data completezza: si prega di inserire la data"
                    ValidationGroup="UCTabDatiGenerici" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                <asp:CustomValidator runat="server" ControlToValidate="txtDataCompletezza" Display="Dynamic"
                    ErrorMessage="Data Completezza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="customCheckDataDataCompletezza" ClientValidationFunction="checkCorrettezzaData" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Data Interessi Legali:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtInteressiLegali" Width="50%"
                    CssClass="txtUppercase tb8 dateGGmmAAAA date-picker-base" onblur="setHdnInteressiLegali();"
                    MaxLength="10"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REV_txtInteressiLegali" ControlToValidate="txtInteressiLegali"
                    ErrorMessage="Inserire la data nel formato valido per Data Interessi Legali"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiGenerici"
                    Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtInteressiLegali" Display="Dynamic"
                    ErrorMessage="Data Interessi Legali: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="CV_txtInteressiLegali" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
        <tr runat="server">
            <td class="Row1" style="width: 25%">
                <label>
                    Data per Rivalsa:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataRivalsa" MaxLength="10"
                    Width="50%" CssClass="txtUppercase tb8 dateGGmmAAAA date-picker-base"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REV_txtDataRivalsa" ControlToValidate="txtDataRivalsa"
                    ErrorMessage="Inserire la data nel formato valido per Data per Rivalsa" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiGenerici"
                    Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataRivalsa" Display="Dynamic"
                    ErrorMessage="Data per Rivalsa: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="CV_txtDataRivalsa" ClientValidationFunction="checkCorrettezzaData" />
            </td>
            <%--<td class="Row1" style="width: 50%" colspan="2">
            </td>--%>
            <%--<td class="Row1" style="width: 25%">
                <label>
                    Aliquota media:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtAliquotaMedia" CssClass="txtUppercase tb8" MaxLength="8"
                    Width="50%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REV_txtAliquotaMedia" ControlToValidate="txtAliquotaMedia"
                    Display="Dynamic" ErrorMessage="Aliquota media: inserire la percentuale in formato valido (max 3 interi e 4 decimali con valore massimo 100)"
                    Text="*" CssClass="field-is-required" ValidationExpression="^\d{1,2}(,\d{1,4})?$|^100(,0{1,4})$" ValidationGroup="UCTabDatiGenerici" />
            </td>--%>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%"  runat="server" ID="lblTipoCalcolo">
                <label>
                    Tipo Calcolo:</label>
            </td>
            <td class="field" colspan="3">
                <asp:DropDownList runat="server" ID="ddlTipoCalcolo" Width="90%" CssClass="tb8 txtUppercase xl"
                    TabIndex="8">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="requiredDdlTipoCalcolo" ControlToValidate="ddlTipoCalcolo"
                    Display="Dynamic" ErrorMessage="Scegliere il tipo di calcolo" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Natura:</label>
            </td>
            <td class="field full-grid cod-nat" colspan="3">
                <asp:DropDownList runat="server" ID="ddlCodNatura1DG" Width="10%" CssClass="txtUppercase tb8 xxs"
                    TabIndex="9">
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="ddlCodNatura2DG" Width="10%" CssClass="tb8 txtUppercase xxs"
                    TabIndex="10">
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="ddlCodNatura3DG" Width="10%" CssClass="tb8 txtUppercase xxs"
                    TabIndex="11">
                </asp:DropDownList>
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlPersonaleViaggiante" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Personale Viaggiante:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlPersonaleViaggiante" Width="80%" CssClass="tb8 txtUppercase"
                        Enabled="false">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="RFVddlPersonaleViaggiante" Display="Dynamic"
                        ControlToValidate="ddlPersonaleViaggiante" ErrorMessage="Personale Viaggiante obbligatorio"
                        ValidationGroup="UCTabDatiGenericiFS" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlBonus2432004">
            <tr id="trBonus2432004" runat="server">
                <td colspan="4" style="width: 100%" class="shift-full-grid">
                    <div id="Div1" style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; width: 100%; margin-left: 0px">
                        <table cellpadding="3" cellspacing="1" border="0" width="100%" class="tabellaFormattazione grid grid-size-25">
                            <tr>
                                <td class="Row1 shift-full-grid" colspan="4">
                                    <label style="font-style: italic" class="section-label mt-32">
                                        Bonus L. 243/2004:</label>
                                </td>
                            </tr>
                            <tr>
                                <td class="Row1" style="width: 180px; padding-left: 5px;">
                                    <label>
                                        Attribuzione Bonus:</label>
                                </td>
                                <td class="field" style="width: 150px; padding-left: 2px;">
                                    <asp:DropDownList runat="server" ID="ddlAttribuzioneBonus" Width="50px" CssClass="tb8 txtUppercase xxs"
                                        TabIndex="12">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 150px;"></td>
                                <td style="width: 150px;"></td>
                            </tr>
                            <tr>
                                <td class="Row1" style="width: 180px; padding-left: 5px;">
                                    <label>
                                        Data Inizio:</label>
                                </td>
                                <td class="field" style="width: 150px; padding-left: 2px;">
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDataInizioBonus" Width="95px"
                                        CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" MaxLength="10" Text="gg/mm/aaaa"
                                        TabIndex="13" onblur="setDecorrenzaCalcoloFSPT()"></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="REV_txtDataInizioBonus" ControlToValidate="txtDataInizioBonus"
                                        Display="Dynamic" ErrorMessage="Inserire la data in un formato valido per la data inizio bonus"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                        ValidationGroup="UCTabDatiGenerici" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDataInizioBonus" Display="Dynamic"
                                        ErrorMessage="Data Inizio: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                                        ID="customCheckDataDataInizioBonus" ClientValidationFunction="checkCorrettezzaData" />
                                </td>
                                <td class="Row1" style="width: 160px; padding-left: 15px;">
                                    <label>
                                        Data Fine:</label>
                                </td>
                                <td class="field" style="width: 160px; padding-left: 13px;">
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDataFineBonus" Width="95px"
                                        MaxLength="10" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" TabIndex="14"
                                        Text="GG/MM/AAAA"></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="REV_txtDataFineBonus" ControlToValidate="txtDataFineBonus"
                                        Display="Dynamic" ErrorMessage="Data Fine Bonus: Inserire la data in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                        ValidationGroup="UCTabDatiGenerici" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDataFineBonus" Display="Dynamic"
                                        ErrorMessage="Data Fine: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                                        ID="customCheckDataDataFineBonus" ClientValidationFunction="checkCorrettezzaData" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </asp:Panel>
        <tr id="rowCausaCarico" runat="server" visible="false">
            <td class="Row1" style="width: 25%">
                <label>
                    Causa Carico:</label>
            </td>
            <td class="Row1 full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlCausaCarico" Width="90%" CssClass="tb8 txtUppercase xl"
                    TabIndex="15">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Comunicazioni / Provvisoria:</label>
            </td>
            <td class="field full-grid inline-fields" colspan="3">
                <asp:DropDownList runat="server" ID="ddlCodComunicazioni1" Width="25%" CssClass="tb8 txtUppercase"
                    TabIndex="16" Visible="false" Enabled="false">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="Sede" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Pensionato" Value="2"></asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="ddlCodComunicazioni2" Width="5%" CssClass="tb8 txtUppercase"
                    TabIndex="17" Visible="false" Enabled="false">
                </asp:DropDownList>
                <asp:TextBox runat="server" ID="txtCodComunicazioni2" Width="5%" CssClass="tb8 txtUppercase"
                    Visible="false" Enabled="false">
                </asp:TextBox>
                <asp:DropDownList runat="server" ID="ddlCodComunicazioni3" Width="30%" CssClass="tb8 txtUppercase">
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="ddlCodComunicazioni4" Width="59%" CssClass="tb8 txtUppercase">
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Panel>
<!--Fine Pannello Common-->
<!-- Pannello Common Check-->
<asp:Panel ID="pnlCommonCheck" runat="server" Visible="false">
    <table class="tabellaFormattazione grid grid-size-25" style="width: 100%">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Ex Combattente:</label>
            </td>
            <td class="chkField" style="width: 25%">
                <asp:CheckBox runat="server" ID="chkExCombattente" CssClass="tb8 offClass onClassExCombattente"
                    TabIndex="21" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Benefici:</label>
            </td>
            <td class="chkField" style="width: 25%">
                <asp:CheckBox runat="server" ID="chkBenefici" CssClass="tb8 offClass onClassBenefici"
                    TabIndex="22" />
            </td>
        </tr>
        <tr id="trTrasfAOI" runat="server">
            <td class="Row1" style="width: 25%">
                <label>
                    Ripristino:</label>
            </td>
            <td class="chkField full-grid" colspan="3">
                <asp:CheckBox runat="server" ID="chkTrasfAOI" CssClass="tb8 offClass onClassTrasfAOI"
                    TabIndex="23" />
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlINPDAP" Visible="false">
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
                        CssClass="txtUppercase tb8 date-picker-maxOneYear dateMMaaaa" TabIndex="18" Text="mm/aaaa"
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
    </table>
</asp:Panel>
<!-- Fine Pannello Common Check-->
<!-- Pannello Custom Eliminazione Contestuale -->
<asp:Panel ID="pnlEliminazioneContestuale" runat="server">
    <table class="tabellaFormattazione grid grid-col-1" style="width: 100%">
        <tr>
            <td colspan="4" style="width: 100%">
                <div id="pdivElimContestuale" class="mt-32" style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; width: 100%; margin-left: 0px"
                    runat="server">
                    <table class="tabellaFormattazione grid grid-size-25" cellpadding="3" cellspacing="1" border="0" width="100%">
                        <tr>
                            <td class="Row1 shift-full-grid" colspan="4">
                                <label style="font-style: italic" class="section-label mt-32">
                                    Eliminazione Contestuale</label>
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1" style="width: 180px; padding-left: 5px;">
                                <label>
                                    Codice:</label>
                            </td>
                            <td class="field" style="width: 150px; padding-left: 2px;">
                                <asp:DropDownList runat="server" ID="ddlElContCodice" Width="110px" CssClass="tb8 txtUppercase"
                                    TabIndex="26">
                                </asp:DropDownList>
                            </td>
                            <td style="width: 150px;"></td>
                            <td style="width: 150px;"></td>
                        </tr>
                        <tr>
                            <td class="Row1" style="width: 180px; padding-left: 5px;">
                                <label>
                                    Decorrenza Eliminazione:</label>
                            </td>
                            <td class="field" style="width: 150px; padding-left: 2px;">
                                <asp:TextBox Style="text-align: left" runat="server" ID="txtElContDecorrenza" Width="95px"
                                    CssClass="tb8 txtUppercase dateMMaaaa" MaxLength="7" TabIndex="27"
                                    Text=""></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtElContDecorrenza" ControlToValidate="txtElContDecorrenza"
                                    Display="Dynamic" ErrorMessage="Inserire la data in un formato valido per Decorrenza Eliminazione Contestuale"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiGenerici" />
                                <asp:CustomValidator runat="server" ControlToValidate="txtElContDecorrenza" Display="Dynamic"
                                    ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                                    ID="customCheckDataDecorrenzaEliminazione" ClientValidationFunction="checkCorrettezzaData" />
                            </td>
                            <td class="Row1" style="width: 160px; padding-left: 15px;">
                                <label>
                                    Data Evento:</label>
                            </td>
                            <td class="field" style="width: 160px; padding-left: 13px;">
                                <asp:TextBox Style="text-align: left" runat="server" ID="txtElContDataEvento" Width="95px"
                                    MaxLength="10" CssClass="txtUppercase tb8 date-picker-evt dateGGmmAAAA" TabIndex="28"
                                    Text=""></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtElContDataEvento" ControlToValidate="txtElContDataEvento"
                                    Display="Dynamic" ErrorMessage="Data Evento Eliminazione Contestuale: Inserire la data in un formato valido"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                    ValidationGroup="UCTabDatiGenerici" />
                                <asp:CustomValidator runat="server" ControlToValidate="txtElContDataEvento" Display="Dynamic"
                                    ErrorMessage="Data Evento: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                                    ID="customCheckDataDataEventoEliminazione" ClientValidationFunction="checkCorrettezzaData" />
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Custom Eliminazione Contestuale VL - FS -->
<!-- Pannello Common Requsiti Ante-SperimentaleDonna-->
<asp:Panel ID="pnlCommonRequisitiAnteSperDonna" runat="server" Visible="false">
    <table class="tabellaFormattazione grid grid-size-25" style="width: 100%">
        <asp:Panel runat="server" ID="pnlRequisitiAnte247">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Requisiti Ante 247:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlReqAnte247" Width="50px" CssClass="tb8 txtUppercase xxs"
                        TabIndex="29">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="ddlReqAnte247_RF" ControlToValidate="ddlReqAnte247"
                        Display="Dynamic" Enabled="true" ErrorMessage="Requisiti ante 247: si prega di inserire il codice"
                        ValidationGroup="UCTabDatiGenerici" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlTrimestreAnzianitaRequisitiNoInvalidita">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Trimestre Requisiti:</label>
                </td>
                <td class="field" style="width: 25%">
                    <table style="border-collapse: collapse; border-style: none; border-width: 0;" cellspacing="0"
                        cellpadding="0" border="0">
                        <tr>
                            <td class="field">
                                <asp:DropDownList ID="ddlTrimestreRequisiti" Width="50px" CssClass="tb8 txtUppercase"
                                    TabIndex="30" runat="server">
                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                    <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                </asp:DropDownList>
                                <span style="visibility: hidden">&nbsp;</span>
                            </td>
                            <td class="field">
                                <asp:TextBox Style="text-align: left" runat="server" ID="txtTrimestreRequisiti" Width="45px"
                                    Text="aaaa" CssClass="txtUppercase tb8" MaxLength="4" TabIndex="31" onblur="extractNumber(this,0,false);"
                                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtTrimestreRequisiti"
                                    ControlToValidate="txtTrimestreRequisiti" Display="Dynamic" ErrorMessage="Inserire l'anno del trimestre requisiti in un formato valido"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{4}|AAAA|aaaa$" ValidationGroup="UCTabDatiGenerici" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:Label ID="lblAnzAnniSperDonnaTrimestre" runat="server" Text="Anzianità Anni:" />
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtAnzAnni" Width="95px"
                        CssClass="txtUppercase tb8" MaxLength="2" Text="aa" TabIndex="32" onblur="extractNumber(this,0,false);"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtAnzAnni" ControlToValidate="txtAnzAnni"
                        Display="Dynamic" ErrorMessage="Inserire gli anni di anzianità in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+|AA|aa)$" ValidationGroup="UCTabDatiGenerici" />
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlSperimentaleDonna">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Legge 243:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlSperimentaleDonna" Width="50px" CssClass="tb8 txtUppercase xxs"
                        TabIndex="33">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="ddlSperimentaleDonna"
                        Display="Dynamic" Enabled="true" ErrorMessage="Sperimentale Donna: si prega di inserire il codice"
                        ValidationGroup="UCTabDatiGenerici" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Semestre Requisiti:</label>
                </td>
                <td class="field" style="width: 25%">
                    <table style="border-collapse: collapse; border-style: none; border-width: 0;" cellspacing="0"
                        cellpadding="0" border="0">
                        <tr>
                            <td class="field">
                                <asp:DropDownList ID="ddlSemestreRequisiti" Width="50px" CssClass="tb8 txtUppercase"
                                    TabIndex="34" runat="server">
                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                    <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                                <span style="visibility: hidden">&nbsp;</span>
                            </td>
                            <td class="field">
                                <asp:TextBox Style="text-align: left" runat="server" ID="txtSemestreRequisiti" Width="45px"
                                    Text="aaaa" CssClass="txtUppercase tb8" MaxLength="4" TabIndex="35" onblur="extractNumber(this,0,false);"
                                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator5" ControlToValidate="txtSemestreRequisiti"
                                    Display="Dynamic" ErrorMessage="Inserire l'anno del semestre requisiti in un formato valido"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{4}|AAAA|aaaa$" ValidationGroup="UCTabDatiGenerici" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:Label ID="lblAnzAnniSperDonnaSemestre" runat="server" Text="Anzianità Anni:" />
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtAnzAnniSperDonna" Width="95px"
                        CssClass="txtUppercase tb8" MaxLength="2" Text="aa" TabIndex="36" onblur="extractNumber(this,0,false);"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator6" ControlToValidate="txtAnzAnniSperDonna"
                        Display="Dynamic" ErrorMessage="Inserire gli anni di anzianità in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+|AA|aa)$" ValidationGroup="UCTabDatiGenerici" />
                </td>
            </tr>
        </asp:Panel>
    </table>
</asp:Panel>
<!-- Fine Pannello Common Requsiti Ante-SperimentaleDonna-->
<!-- Panel button-->
<asp:Panel ID="pnlButton" runat="server">
    <div style="width: 100%; margin-top: 25px; margin-right: 40px;">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaDatiGenerici" runat="server" SkinID="btnAzione1" Enabled="true"
                        Text="Salva Dati Generici" Width="180px" CausesValidation="false" OnClick="SalvaDatiGenerici_Click"
                        OnClientClick="return ShowPopUpEsenzione();" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaDatiGenerici" runat="server" SkinID="btnAzione1" Enabled="true"
                        Text="Elimina Dati Generici" Width="180px" CausesValidation="false" OnClick="EliminaDatiGenerici_Click"
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

<div id="dialog-esenzione" title="Conferma" style="display: none;">
    <p></p>
</div>
<!-- Panel button-->
<asp:HiddenField runat="server" ID="HiddenFieldChkBenefici" />
<asp:HiddenField runat="server" ID="HiddenIntLeg" />
<asp:HiddenField runat="server" ID="hdnTrimesteRequisiti" Value="" />
<asp:HiddenField runat="server" ID="hdnTrimesteRequisitiAnno" Value="" />
<asp:HiddenField runat="server" ID="HiddenFieldDecorrenza" />
<asp:HiddenField runat="server" ID="HiddenFieldDataEvento" />
<asp:HiddenField runat="server" ID="HiddenFieldIsRicostituzione" />
<asp:HiddenField runat="server" ID="HiddenFieldIsRICPost20022022" Value="" />
<asp:HiddenField runat="server" ID="HiddenPrecedentePensione" />
<asp:HiddenField ID="HiddenOpzioneDonnaLegge_197_2022" runat="server" Value="false" />
<asp:HiddenField ID="hdnIsRicEsenzioneFiscaleVittimeDelDovere" runat="server" Value="false" />