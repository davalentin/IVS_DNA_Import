<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiGenericiEL_TT_ET.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCDatiGenericiEL_TT_ET" %>
<script type="text/javascript">

    $(document).ready(function () {        
        //al momento codComunicazione non visualizzato
        //document.getElementById('<%=ddlCodComunicazioni2.ClientID %>').style.display = "none";
        var aoiChecked = $(document.getElementById("<%=chkTrasfAOI.ClientID %>")).attr("checked");
        EnableDisableTab(aoiChecked);
        getDDLCodNatura2Value();
        if (document.getElementById("<%=HiddenAnnoBonusBooking.ClientID%>") != null && document.getElementById("<%=HiddenAnnoBonusBooking.ClientID%>").value == "SI")
            showHideAnnoBonus();

        if (document.getElementById("<%=pnlRequisitiAnte247.ClientID%>") != null && document.getElementById("<%=pnlRequisitiAnte247.ClientID%>") != undefined) {
            SetTrimestreRequisiti();
            $("#<%=ddlReqAnte247.ClientID %>").change(function () { SetTrimestreRequisiti(); });
        }

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

    function setDataInteressiLegali() {
        var dataCompletezza = document.getElementById("<%=txtDataCompletezza.ClientID %>").value;
        if (dataCompletezza != "") {
            var dataInteressiLegali = new Date();
            dataInteressiLegali = convertString2Date(dataCompletezza)
            dataInteressiLegali.setDate(dataInteressiLegali.getDate() + 121);
            var interessiLegali = convertDate2String(dataInteressiLegali);
            document.getElementById("<%=txtInteressiLegali.ClientID %>").value = interessiLegali;
        }
    }

    function setTipoCalcoloRetributivoDisabled() {
        var ddlTipoCalcolo = $("#<%= ddlTipoCalcolo.ClientID %>");
        if (ddlTipoCalcolo) {
            ddlTipoCalcolo.val('18');
            ddlTipoCalcolo.attr("disabled", true);
        }
    }

    function unlockTipoCalcolo() {
        var ddlTipoCalcolo = $("#<%= ddlTipoCalcolo.ClientID %>");
        var isTipoCalcoloUnlockable = $("#<%= hdnIsTipoCalcoloUnlockable.ClientID %>");
        if (ddlTipoCalcolo && isTipoCalcoloUnlockable && isTipoCalcoloUnlockable.val() == "SI") {
            ddlTipoCalcolo.removeAttr("disabled");
        }
    }

    function setAllDisabledControlsInHiddenField() {
        var hdnTipoCalcolo = $("#<%= hdnTipoCalcolo.ClientID %>");
        var ddlTipoCalcolo = $("#<%= ddlTipoCalcolo.ClientID %>");
        hdnTipoCalcolo.val(ddlTipoCalcolo.val());
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
<asp:HiddenField runat="server" ID="HiddenTrattamentoDisagi" />
<asp:HiddenField runat="server" ID="HiddenAnnoBonusBooking" />
<!-- Pannello Dati Generici Comune -->
<asp:Panel runat="server" ID="pnlDatiGenericiCommon">
    <table class="tabellaFormattazione grid grid-size-25">
        <tr runat="server" id="trDeroga" visible="false">
            <td class="Row1">
                <label>
                    Deroga:
                </label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:DropDownList CssClass="tb8 txtUppercase" ID="ddlDeroga" runat="server" Enabled="false"
                    Width="513px" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Decorrenza Pensione:</label>
            </td>
            <td class="field">
                <asp:Label runat="server" ID="lblDecorrenzaPensioneDatiGenerici" Text=""></asp:Label>
            </td>
            <td class="Row1">
            </td>
            <td class="field">
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Decorrenza Arretrati:</label>
            </td>
            <td class="field">
                <%--<asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaArretrati" Width="95px" 
                    CssClass="txtUppercase tb8 date-picker-maxActual" TabIndex="4" Text="" MaxLength="7"></asp:TextBox>--%>
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaArretrati"
                    Width="95px" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="4"
                    Text="" MaxLength="7"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateDecorrenzaArretrati" ControlToValidate="txtDecorrenzaArretrati"
                    Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per  Decorrenza Arretrati "
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiGenerici"
                    Text="*" CssClass="field-is-required" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaArretrati" Display="Dynamic"
                    ErrorMessage="Decorrenza Arretrati: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="customCheckDataDecorrenzaArretrati" ClientValidationFunction="checkCorrettezzaData" />
            </td>
            <td class="Row1">
                <label>
                    Codici Arretrati:</label>
            </td>
            <td class="field">
                <asp:DropDownList runat="server" ID="ddlCodiciArretrati" Width="75px" CssClass="tb8 txtUppercase xxs"
                    TabIndex="5">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="1 - NO" Value="1"></asp:ListItem>
                    <asp:ListItem Text="8 - SI" Value="8"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlCodiceArretrati_RF" ControlToValidate="ddlCodiciArretrati"
                    Display="Dynamic" Enabled="true" ErrorMessage="Codice Arretrati: si prega di inserire il codice"
                    ValidationGroup="UCTabDatiGenerici" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr runat="server" id="TrScadenzaRevisioneSanitaria" visible="false">
            <td class="Row1">
                <label>
                    Scadenza Revisione Sanitaria:</label>
            </td>
            <td class="field">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtScadRevSanitaria" Width="95px"
                    CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="6" Text="mm/aaaa"
                    MaxLength="7"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateScadRevSanitaria" ControlToValidate="txtScadRevSanitaria"
                    Display="Dynamic" ErrorMessage="Inserire la data nel formato valido per Scadenza Revisione Sanitaria"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiGenerici" />
                <asp:CustomValidator runat="server" ControlToValidate="txtScadRevSanitaria" Display="Dynamic"
                    ErrorMessage="Scadenza Revisione Sanitaria: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="customCheckDataScadenzaRevisioneSanitaria" ClientValidationFunction="checkCorrettezzaData" />
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Data Completezza:</label>
            </td>
            <td class="field">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataCompletezza" Width="95px"
                    CssClass="txtUppercase tb8 date-picker-base-maxActual dateGGmmAAAA" TabIndex="7"
                    Text="" MaxLength="10"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateDataCompletezza" ControlToValidate="txtDataCompletezza"
                    Display="Dynamic" ErrorMessage="Inserire la data nel formato valido per Data Completezza"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    ValidationGroup="UCTabDatiGenerici" />
                <%--                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3" ControlToValidate="txtDataCompletezza"
                    Display="Dynamic" ErrorMessage="Data Completezza: si prega di inserire la data"
                    Text="*" CssClass="field-is-required" ValidationExpression="^|GG/MM/AAAA|gg/mm/aaaa$"
                    ValidationGroup="UCTabDatiGenerici" />--%>
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
            <td class="Row1">
                <label>
                    Data Interessi Legali:</label>
            </td>
            <td class="field">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtInteressiLegali" Width="95px"
                    CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" TabIndex="8" Text=""
                    MaxLength="10"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateInteressiLegali" ControlToValidate="txtInteressiLegali"
                    Display="Dynamic" ErrorMessage="Inserire la data in un formato valido per Data Interessi Legali"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    ValidationGroup="UCTabDatiGenerici" />
                <%--Controllo non necessario. Il controllo da effettuare è rispetto a: PL - 121 giorni dalla data di completezza; Ric-  decorrenza della pensione (non può essere antecedente).--%>
                <%--                <asp:CustomValidator runat="server" ControlToValidate="txtInteressiLegali" Display="Dynamic"
                    ErrorMessage="Data Interessi Legali: Data inserita posteriore a quella odierna" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="customDataInteressiLegali" ClientValidationFunction="checkDataPostOdiernaGGMMAAAA" />
                --%>
                <asp:CustomValidator runat="server" ControlToValidate="txtInteressiLegali" Display="Dynamic"
                    ErrorMessage="Data Interessi Legali: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                    ID="customCheckDataDataInteressiLegali" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Tipo Calcolo:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlTipoCalcolo" Width="90%" CssClass="tb8 txtUppercase xl"
                    TabIndex="9">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="requiredDdlTipoCalcolo" ControlToValidate="ddlTipoCalcolo"
                    Display="Dynamic" ErrorMessage="Scegliere il tipo di calcolo" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Codice Natura:</label>
            </td>
            <td class="field cod-nat">
                <asp:DropDownList runat="server" ID="ddlCodNatura1DG" Width="50px" CssClass="txtUppercase tb8 xxs"
                    TabIndex="10">
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="ddlCodNatura2DG" Width="50px" CssClass="tb8 txtUppercase xxs"
                    TabIndex="11">
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="ddlCodNatura3DG" Width="50px" CssClass="tb8 txtUppercase xxs"
                    TabIndex="12">
                </asp:DropDownList>
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlBonus2432004">
            <tr id="trBonus2432004" runat="server">
                <td colspan="4" class="shift-full-grid">
                    <div id="Div1" style="border-style: solid; border-color: #000080; border-collapse: collapse;
                        border-width: 1px; width: 100%; margin-left: 0px">
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
                                        TabIndex="13">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 150px;">
                                </td>
                                <td style="width: 150px;">
                                </td>
                            </tr>
                            <tr>
                                <td class="Row1" style="width: 180px; padding-left: 5px;">
                                    <label>
                                        Data Inizio:</label>
                                </td>
                                <td class="field" style="width: 150px; padding-left: 2px;">
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDataInizioBonus" Width="95px"
                                        CssClass="txtUppercase tb8 date-picker dateMMaaaa" MaxLength="7" Text="mm/aaaa"
                                        TabIndex="14"></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="txtDataInizioBonus"
                                        Display="Dynamic" ErrorMessage="Inserire la data in un formato valido per la data inizio bonus"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiGenerici" />
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
                                        MaxLength="7" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="15"
                                        Text="MM/AAAA"></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" ControlToValidate="txtDataFineBonus"
                                        Display="Dynamic" ErrorMessage="Data Fine Bonus: Inserire la data in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiGenerici" />
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
        <asp:Panel runat="server" ID="pnlCausaCarico">
            <tr>
                <td class="Row1 pre__full-grid">
                    <label>
                        Causa Carico:</label>
                </td>
                <td class="Row1 full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlCausaCarico" Width="90%" CssClass="tb8 txtUppercase xl"
                        TabIndex="16">
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="Row1 pre__full-grid">
                <label>
                    Codice Comunicazioni / Provvisoria:</label>
            </td>
            <td class="field full-grid inline-fields" colspan="3">
                <asp:DropDownList runat="server" ID="ddlCodComunicazioni1" Width="25%" CssClass="tb8 txtUppercase"
                    TabIndex="17" Visible="false">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="Sede" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Pensionato" Value="2"></asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="ddlCodComunicazioni2" Width="5%" CssClass="tb8 txtUppercase"
                    TabIndex="18" Visible="false">
                </asp:DropDownList>
                <asp:TextBox runat="server" ID="txtCodComunicazioni2" Width="5%" CssClass="tb8 txtUppercase"
                    Visible="false"></asp:TextBox>
                <asp:DropDownList runat="server" ID="ddlCodComunicazioni3" Width="30%" CssClass="tb8 txtUppercase"
                    TabIndex="19">
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="ddlCodComunicazioni4" Width="59%" CssClass="tb8 txtUppercase"
                    TabIndex="20">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Ripristino:</label>
            </td>
            <td class="chkField">
                <asp:CheckBox runat="server" ID="chkTrasfAOI" CssClass="tb8 offClass onClassTrasfAOI"
                    TabIndex="21" />
            </td>
            <td class="Row1" style="width: 20%">
                <label>
                    Benefici:</label>
            </td>
            <td class="chkField">
                <asp:CheckBox runat="server" ID="chkBenefici" CssClass="tb8 offClass onClassBenefici"
                    TabIndex="22" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Ex Combattente:</label>
            </td>
            <td class="chkField">
                <asp:CheckBox runat="server" ID="chkExCombattente" CssClass="tb8 offClass onClassExCombattente"
                    TabIndex="23" />
            </td>
            <td class="Row1">
                <asp:Label ID="lblChkDL407" Text="D.L. 407" runat="server"></asp:Label>
            </td>
            <td class="chkField">
                <asp:CheckBox runat="server" ID="chkDL407" CssClass="tb8 offClass onClassExCombattente"
                    TabIndex="24" />
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
                        TabIndex="17" >
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
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3" ControlToValidate="txtDecTrattINPDAP"
                        Display="Dynamic" ErrorMessage="Inserire la data nel formato valido per Decorrenza Trattenuta Fondo Credito"
                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiGenerici" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecTrattINPDAP" Display="Dynamic"
                        ErrorMessage="Decorrenza Trattenuta Fondo Credito: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                        ID="customCheckDataDecorrenzaTrattINPDAP" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td colspan="4" style="width: 50%">
                <asp:Panel ID="pnlRichiestaBonus" runat="server" Visible="false">
                    <table style="width: 100%;">
                        <tr>
                            <td class="Row1" style="width: 24.5%;">
                                <asp:Label id="lblRichiestaBonus" runat="server" />
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
            <td colspan="4" class="shift-full-grid">
                <div id="pdivElimContestuale" style="border-style: solid; border-color: #000080;
                    border-collapse: collapse; border-width: 1px; width: 100%; margin-left: 0px"
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
                                    TabIndex="25">
                                </asp:DropDownList>
                            </td>
                            <td style="width: 150px;">
                            </td>
                            <td style="width: 150px;">
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1" style="width: 180px; padding-left: 5px;">
                                <label>
                                    Decorrenza:</label>
                            </td>
                            <td class="field" style="width: 150px; padding-left: 2px;">
                                <asp:TextBox Style="text-align: left" runat="server" ID="txtElContDecorrenza" Width="95px"
                                    CssClass="tb8 txtUppercase date-picker dateMMaaaa" MaxLength="7" TabIndex="26"
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
                                <%--<asp:TextBox Style="text-align: left" runat="server" ID="txtElContDataEvento" Width="95px" MaxLength="10"
                                    CssClass="txtUppercase tb8 date-picker-base-maxActual" TabIndex="27" Text=""></asp:TextBox>--%>
                                <asp:TextBox Style="text-align: left" runat="server" ID="txtElContDataEvento" Width="95px"
                                    MaxLength="10" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" TabIndex="27"
                                    Text=""></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtElContDataEvento" ControlToValidate="txtElContDataEvento"
                                    Display="Dynamic" ErrorMessage="Data Evento Eliminazione Contestuale: Inserire la data in un formato valido"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                    ValidationGroup="UCTabDatiGenerici" />
                                <%--<asp:CustomValidator runat="server" ControlToValidate="txtElContDataEvento" Display="Dynamic"
                                    ErrorMessage="Data Evento Eliminazione Contestuale : Data inserita posteriore a quella odierna"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici" ID="customElContDataEvento" ClientValidationFunction="checkDataPostOdiernaGGMMAAAA" />--%>
                                <asp:CustomValidator runat="server" ControlToValidate="txtElContDataEvento" Display="Dynamic"
                                    ErrorMessage="Data Evento: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiGenerici"
                                    ID="customCheckDataDataEventoEliminazione" ClientValidationFunction="checkCorrettezzaData" />
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlRequisitiAnte247">
            <tr>
                <td class="Row1">
                    <label>
                        Requisiti Ante 247:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlReqAnte247" Width="50px" CssClass="tb8 txtUppercase xxs"
                        TabIndex="28">
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
                <td class="Row1">
                    <label>
                        Trimestre Requisiti:</label>
                </td>
                <td>
                    <table style="border-collapse: collapse; border-style: none; border-width: 0;" cellspacing="0"
                        cellpadding="0" border="0">
                        <tr>
                            <td class="field">
                                <asp:DropDownList ID="ddlTrimestreRequisiti" Width="50px" CssClass="tb8 txtUppercase"
                                    TabIndex="29" runat="server">
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
                                    Text="aaaa" CssClass="txtUppercase tb8" MaxLength="4" TabIndex="30" onblur="extractNumber(this,0,false);"
                                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtTrimestreRequisiti"
                                    ControlToValidate="txtTrimestreRequisiti" Display="Dynamic" ErrorMessage="Inserire l'anno del trimestre requisiti in un formato valido"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{4}|AAAA|aaaa$" ValidationGroup="UCTabDatiGenerici" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="Row1">
                    <label>
                        Anzianità Anni:</label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtAnzAnni" Width="95px"
                        CssClass="txtUppercase tb8" MaxLength="2" Text="aa" TabIndex="31" onblur="extractNumber(this,0,false);"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtAnzAnni" ControlToValidate="txtAnzAnni"
                        Display="Dynamic" ErrorMessage="Inserire gli anni di anzianità in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+|AA|aa)$" ValidationGroup="UCTabDatiGenerici" />
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlSperimentaleDonna">
            <tr>
                <td class="Row1">
                    <label>
                        Legge 243:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlSperimentaleDonna" Width="50px" CssClass="tb8 txtUppercase xxs"
                        TabIndex="32">
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
                <td class="Row1">
                    <label>
                        Semestre Requisiti:</label>
                </td>
                <td>
                    <table style="border-collapse: collapse; border-style: none; border-width: 0;" cellspacing="0"
                        cellpadding="0" border="0">
                        <tr>
                            <td class="field">
                                <asp:DropDownList ID="ddlSemestreRequisiti" Width="50px" CssClass="tb8 txtUppercase"
                                    TabIndex="33" runat="server">
                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                    <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                                <span style="visibility: hidden">&nbsp;</span>
                            </td>
                            <td class="field">
                                <asp:TextBox Style="text-align: left" runat="server" ID="txtSemestreRequisiti" Width="45px"
                                    Text="aaaa" CssClass="txtUppercase tb8" MaxLength="4" TabIndex="34" onblur="extractNumber(this,0,false);"
                                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator5" ControlToValidate="txtSemestreRequisiti"
                                    Display="Dynamic" ErrorMessage="Inserire l'anno del semestre requisiti in un formato valido"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{4}|AAAA|aaaa$" ValidationGroup="UCTabDatiGenerici" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="Row1">
                    <label>
                        Anzianità Anni:</label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtAnzAnniSperDonna" Width="95px"
                        CssClass="txtUppercase tb8" MaxLength="2" Text="aa" TabIndex="35" onblur="extractNumber(this,0,false);"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator6" ControlToValidate="txtAnzAnniSperDonna"
                        Display="Dynamic" ErrorMessage="Inserire gli anni di anzianità in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+|AA|aa)$" ValidationGroup="UCTabDatiGenerici" />
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlRequisitiVecchiaia">
            <tr>
                <td class="Row1" style="width: 27%">
                    <label>
                        Requisiti Vecchiaia al 12/94:</label>
                </td>
                <td class="chkField">
                    <asp:CheckBox runat="server" ID="chkReqVecch1294" Style="text-align: left" CssClass="tb8"
                        TabIndex="36" />
                </td>
                <td class="Row1">
                    <label>
                        Requisiti Anzianità al 12/94:</label>
                </td>
                <td class="chkField">
                    <asp:CheckBox runat="server" ID="chkReqAnz1294" Style="text-align: left" CssClass="tb8"
                        TabIndex="36" />
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        Requisiti Vecchiaia al 9/96:</label>
                </td>
                <td class="chkField">
                    <asp:CheckBox runat="server" ID="chkReqVecch996" Style="text-align: left" CssClass="tb8"
                        TabIndex="38" />
                </td>
                <td>
                </td>
            </tr>
        </asp:Panel>
    </table>
</asp:Panel>
<asp:Panel ID="pnlButton" runat="server">
    <div style="width: 100%; margin-top: 25px; margin-right: 40px;">
        <table style="width:100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaDatiGenerici" runat="server" SkinID="btnAzione1" Enabled="true"
                        Text="Salva Dati Generici" Width="180px" CausesValidation="false" OnClick="SalvaDatiGenerici_Click"
                        OnClientClick="setAllDisabledControlsInHiddenField(); if(Page_ClientValidate('UCTabDatiGenerici')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
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
<!-- Fine Pannello Dati Generici Comune -->
<asp:HiddenField runat="server" ID="hdnTipoCalcolo" />
<asp:HiddenField runat="server" ID="hdnIsTipoCalcoloUnlockable" Value="SI" />
<asp:HiddenField runat="server" ID="hdnTrimesteRequisiti" Value="" />
<asp:HiddenField runat="server" ID="hdnTrimesteRequisitiAnno" Value="" />
<asp:HiddenField runat="server" ID="hdnAnnoRichiestaBonus14" Value="" />
<asp:HiddenField runat="server" ID="HiddenFieldIsRICPost20022022" Value="" />
<asp:HiddenField runat="server" ID="HiddenPrecedentePensione" />