<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCAnagrafica.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Titolare.UCAnagrafica" %>
<script type="text/javascript">

    //    $(document).ready(function() {
    //        var availableTags = document.getElementById("<%=HiddenFieldSedi.ClientID%>").value.split(';');
    //        //alert(availableTags);
    //        $("#<%=txtSedeDestinazione.ClientID%>").autocomplete({
    //            minLength: 0,
    //            source: availableTags
    //        });

    //    });

    $(document).ready(function () {
        var availableTags = document.getElementById("<%=HiddenFieldSedi.ClientID%>").value.split(';');
        //alert(availableTags);
        $("#<%=txtSedeDestinazione.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTags,
            open: function() {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            }
        });

        //        htxtPerfRequisiti       contiene il valore a db 
        //        htxtDecorrenzaPensione  contiene il valore a db

        var fondo = document.getElementById("<%=hiddenFieldTipoFondo.ClientID%>").value;
        var idDecDisabledSuperstiti = document.getElementById("<%= hiddenFieldDecDisabledSuperstiti.ClientID %>").value;
        var isEsodati = document.getElementById("<%= hdnIsEsodati.ClientID %>").value;
        var isINPDAP = document.getElementById("<%=hdnIsINPDAP.ClientID %>").value;
        var isLikeFSPT = document.getElementById("<%=hdnIsLikeFSPT.ClientID %>").value;

        var IsDPRVisibleBefore2011 = document.getElementById("<%=hdnIsDPRVisibleBefore2011.ClientID %>").value;

        var dataPerfRequisiti = document.getElementById("<%=htxtPerfRequisiti.ClientID%>").value;

        if (fondo == "FS" || fondo == "PT" || isINPDAP == "SI" || isLikeFSPT == "SI")
            var dataDecorrenzaPensione = document.getElementById("<%=txtDecorrenzaPensioneFSPT.ClientID%>").value;
        else
            var dataDecorrenzaPensione = document.getElementById("<%=txtDecorrenzaPensione.ClientID%>").value;

        var flagUnicarpe = document.getElementById("<%=hFlagUnicarpe.ClientID%>").value;
        var visibilitaPerf = document.getElementById("<%=hiddenFieldPerf.ClientID%>").value;

        if (flagUnicarpe == "True" || idDecDisabledSuperstiti == "SI") {
            if (fondo == "FS" || fondo == "PT" || isINPDAP == "SI" || isLikeFSPT == "SI")
                document.getElementById("<%=pnlTxtDecorrenzaPensioneFSPT.ClientID%>").style.display = 'none';
            else
                document.getElementById("<%=pnlTxtDecorrenzaPensione.ClientID%>").style.display = 'none';
        }
        else {
            if (fondo == "FS" || fondo == "PT" || isINPDAP == "SI" || isLikeFSPT == "SI")
                document.getElementById("<%=pnlTxtDecorrenzaPensioneFSPT.ClientID %>").style.display = 'block';
            else
                document.getElementById("<%=pnlTxtDecorrenzaPensione.ClientID %>").style.display = 'block';

            document.getElementById("<%=lblDecorrenzaPensione.ClientID %>").style.display = 'none';
        }
        if (visibilitaPerf == "SI") {
            if (isEsodati == "SI") {
                if (flagUnicarpe == "True" && dataPerfRequisiti != "") {
                    document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'none';
                    document.getElementById("<%=lbltxtPerfezRequisiti.ClientID %>").style.display = 'block';
                }
                else {
                    document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'block';
                    document.getElementById("<%=lblPerfezionamentoReq.ClientID %>").style.display = 'block';
                }

            }
            <%--else if (isINPDAP == "SI") {
                document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'none';
                document.getElementById("<%=lblPerfezionamentoReq.ClientID %>").style.display = 'block';
            }--%>
            else if (dataDecorrenzaPensione != "") {
                if (fondo == "FS" || fondo == "PT" || isINPDAP == "SI" || isLikeFSPT == "SI")
                    var anno = parseInt(dataDecorrenzaPensione.split('/')[2]);
                else
                    var anno = parseInt(dataDecorrenzaPensione.split('/')[1]);

                if (anno >= 2011) {
                    document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'block';
                    document.getElementById("<%=lblPerfezionamentoReq.ClientID %>").style.display = 'block';

                    if (flagUnicarpe == "True" && dataPerfRequisiti != "") {
                        document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'none';
                        document.getElementById("<%=lbltxtPerfezRequisiti.ClientID %>").style.display = 'block';
                    }
                    else {
                        document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'block';
                        document.getElementById("<%=lbltxtPerfezRequisiti.ClientID %>").style.display = 'none';
                    }
                }
                else {
                    document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'none';
                    document.getElementById("<%=lblPerfezionamentoReq.ClientID %>").style.display = 'none';
                    if (IsDPRVisibleBefore2011 == "SI") {
                        document.getElementById("<%=lblPerfezionamentoReq.ClientID %>").style.display = 'block';
                        if (flagUnicarpe == "True" && dataPerfRequisiti != "") {
                            document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'none';
                            document.getElementById("<%=lbltxtPerfezRequisiti.ClientID %>").style.display = 'block';
                        }
                        else {
                            document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'block';
                            document.getElementById("<%=lbltxtPerfezRequisiti.ClientID %>").style.display = 'none';
                        }
                    }
                }
            }
            else {
                document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'none';
                document.getElementById("<%=lblPerfezionamentoReq.ClientID %>").style.display = 'none';
            }
        }
        else {
            document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'none';
            document.getElementById("<%=lblPerfezionamentoReq.ClientID %>").style.display = 'none';
        }


        var hiddenField = document.getElementById('<%= hiddInfoMessage.ClientID %>');
            if (hiddenField) {
                var value = hiddenField.value;
                if (value != '') {
                    openModal(value);
                    document.getElementById('<%= hiddInfoMessage.ClientID %>').value = '';
                }
            }
    });

    function checkTabPress(e) {
        // pick passed event of global event object
        e = e || event;
        if (event.keyCode == 9) {
            var hdnTabPressed = document.getElementById("<%=hdnTabPressed.ClientID %>").value
             if (hdnTabPressed == "NO") {
                 document.getElementById("<%=hdnTabPressed.ClientID %>").value = "SI";
                setpnlTxtPerfRequisitiVisibility(e);
            }
        }
    }

    function CleanFields() {
        document.getElementById("<%=txtTel.ClientID%>").value = '';
        document.getElementById("<%=txtCell.ClientID%>").value = '';
        document.getElementById("<%=txtEmail.ClientID%>").value = '';
        document.getElementById("<%=txtDecorrenzaPensione.ClientID%>").value = '';
        document.getElementById("<%=ddlSindacato.ClientID %>").value = '';

        return false;
    }

    function changeDdlSeltaLM() {
        var hdnAbilitazioneMemo28_2024 = document.getElementById("<%=hdnAbilitazioneMemo28_2024.ClientID %>").value;
        if(hdnAbilitazioneMemo28_2024 == "SI")
        {
            if(document.getElementById("<%=ddlFigli.ClientID %>").value == '0' || document.getElementById("<%=ddlFigli.ClientID %>").value == '')
            {
                document.getElementById("<%=ddlSeltaLM.ClientID %>").value = '0';
                document.getElementById("<%=ddlSeltaLM.ClientID %>").disabled = true;
            }
            else
            {
                document.getElementById("<%=ddlSeltaLM.ClientID %>").disabled = false;
            }
        }
    }

    function setpnlTxtPerfRequisitiVisibility(e) {
        var hdnTabPressed = document.getElementById("<%=hdnTabPressed.ClientID %>").value;
        var fondo = document.getElementById("<%=hiddenFieldTipoFondo.ClientID%>").value;
        var isEsodati = document.getElementById("<%= hdnIsEsodati.ClientID %>").value;
        var isINPDAP = document.getElementById("<%=hdnIsINPDAP.ClientID %>").value;
        var isLikeFSPT = document.getElementById("<%=hdnIsLikeFSPT.ClientID %>").value;
        var IsDPRVisibleBefore2011 = document.getElementById("<%=hdnIsDPRVisibleBefore2011.ClientID %>").value;
        var visibilitaPerf = document.getElementById("<%=hiddenFieldPerf.ClientID%>").value;
        if (visibilitaPerf == "SI") {
            if (isEsodati == "SI") {
                document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'block';
                document.getElementById("<%=lblPerfezionamentoReq.ClientID %>").style.display = 'block';
            }
            <%--else if (isINPDAP == "SI") {
                document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'none';
                document.getElementById("<%=lblPerfezionamentoReq.ClientID %>").style.display = 'block';
            }--%>
            else {
                if (fondo == "FS" || fondo == "PT" || isINPDAP == "SI" || isLikeFSPT == "SI") {
                    var data = document.getElementById("<%=txtDecorrenzaPensioneFSPT.ClientID %>").value;
                    var posizione = 2;
                }
                else {
                    var data = document.getElementById("<%=txtDecorrenzaPensione.ClientID %>").value;
                    var posizione = 1;
                }

                if (data != '' && data.split('/')[posizione] != 'AAAA' && data.split('/')[posizione] != 'aaaa') {
                    var anno = parseInt(data.split('/')[posizione]);

                    if (anno < 2011) {
                        document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'none';
                        document.getElementById("<%=lblPerfezionamentoReq.ClientID %>").style.display = 'none';
                        if (IsDPRVisibleBefore2011 == "SI") {
                            document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'block';
                            document.getElementById("<%=lblPerfezionamentoReq.ClientID %>").style.display = 'block';
                            if (document.getElementById("<%=htxtPerfRequisiti.ClientID %>").value != "" && (document.getElementById("<%=txtPerfRequisiti.ClientID %>").value == "" || document.getElementById("<%=txtPerfRequisiti.ClientID %>").value == "GG/MM/AAAA"))
                                document.getElementById("<%=txtPerfRequisiti.ClientID %>").value = document.getElementById("<%=htxtPerfRequisiti.ClientID %>").value;
                            else
                                if (document.getElementById("<%=txtPerfRequisiti.ClientID %>").value == "")
                                    document.getElementById("<%=txtPerfRequisiti.ClientID %>").value = 'GG/MM/AAAA';
                        }
                        else {
                            document.getElementById("<%=txtPerfRequisiti.ClientID %>").value = 'GG/MM/AAAA';
                        }
                    }
                    else {
                        document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'block';
                        document.getElementById("<%=lblPerfezionamentoReq.ClientID %>").style.display = 'block';

                        if (document.getElementById("<%=htxtPerfRequisiti.ClientID %>").value != "" && (document.getElementById("<%=txtPerfRequisiti.ClientID %>").value == "" || document.getElementById("<%=txtPerfRequisiti.ClientID %>").value == "GG/MM/AAAA"))
                            document.getElementById("<%=txtPerfRequisiti.ClientID %>").value = document.getElementById("<%=htxtPerfRequisiti.ClientID %>").value;
                        else
                            if (document.getElementById("<%=txtPerfRequisiti.ClientID %>").value == "")
                                document.getElementById("<%=txtPerfRequisiti.ClientID %>").value = 'GG/MM/AAAA';
                    }
                }
                else {
                    document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'none';
                    document.getElementById("<%=lblPerfezionamentoReq.ClientID %>").style.display = 'none';
                }
            }
        }
        else {
            document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display = 'none';
            document.getElementById("<%=lblPerfezionamentoReq.ClientID %>").style.display = 'none';
        }
        if (hdnTabPressed == "SI") {
            $(e).trigger(jQuery.Event('keydown', { which: 9 }));
            document.getElementById("<%=hdnTabPressed.ClientID %>").value = "NO";
        }
    }

    function checkCorrettezzaPerfRequisiti(source, args) {
        if (document.getElementById("<%=pnlTxtPerfRequisiti.ClientID %>").style.display != 'none') {
            args.value = document.getElementById("<%=txtPerfRequisiti.ClientID %>").value;
            validateData(null, args);
            return args.IsValid;
        }
        else
            return true;

    }

    function GetDecPensione() {
        var fondo = document.getElementById("<%=hiddenFieldTipoFondo.ClientID%>").value;
        var isINPDAP = document.getElementById("<%=hdnIsINPDAP.ClientID %>").value;
        var isLikeFSPT = document.getElementById("<%=hdnIsLikeFSPT.ClientID %>").value;
        var decPens = "";
        if (fondo == "FS" || fondo == "PT" || isINPDAP == "SI" || isLikeFSPT == "SI") {
            var date = document.getElementById("<%=txtDecorrenzaPensioneFSPT.ClientID%>").value;
            if (date != null && date != "") {
                var decPens = date.split('/');
                decPens = decPens[1] + "/" + decPens[2];
            }
        }
        else
            decPens = document.getElementById("<%=txtDecorrenzaPensione.ClientID%>").value;
        return decPens;
    }

    function PrevalorizzaCittadinanza() {
        $('#<%= ddlCittadinanza.ClientID %>').val("Z000");
    }

  

</script>
<asp:Panel runat="server" ID="pnlAnagrafica">
    <table class="tabellaFormattazione grid grid-size-20" width="100%">
        <tr>
            <td class="Row1" style="width: 22%;">
                <label>
                    Numero Domanda:</label>
            </td>
            <td class="field" style="width: 28%">
                <asp:Label runat="server" ID="lblNumeroDomanda" />
            </td>
            <td class="Row1" style="width: 25%; text-align: left;">
                <label>
                    Codice Fiscale:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:Label runat="server" ID="lblCodiceFiscale" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 22%;">
                <label>
                    Cognome:</label>
            </td>
            <td class="field" style="width: 28%">
                <asp:Label runat="server" ID="lblCognome" />
            </td>
            <td class="Row1" style="width: 25%; text-align: left;">
                <label>
                    Nome:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:Label runat="server" ID="lblNome" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 22%;">
                <label>
                    Sesso:</label>
            </td>
            <td class="field" style="width: 28%">
                <asp:Label runat="server" ID="lblSesso" />
            </td>
            <td class="Row1" style="width: 25%; text-align: left;">
                <label>
                    Data di Nascita:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:Label runat="server" ID="lblDataDiNascita"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 22%;">
                <label>
                    Comune di Nascita:</label>
            </td>
            <td class="field" style="width: 28%">
                <asp:Label runat="server" ID="lblComuneNascita"></asp:Label>
            </td>
            <td class="Row1" style="width: 25%; text-align: left;">
                <asp:Label runat="server" ID="etichettaProvinciaStatoNascita" />
            </td>
            <td class="field" style="width: 25%">
                <asp:Label runat="server" ID="lblProvinciaStatoNascita" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 22%;">
                <label>
                    Indirizzo:</label>
            </td>
            <td class="field" style="width: 28%">
                <asp:Label runat="server" ID="lblIndirizzo" />
            </td>
            <td class="Row1" style="width: 25%; text-align: left;">
                <label>
                    N. Civico:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:Label runat="server" ID="lblNCivico" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 22%;">
                <label>
                    CAP:</label>
            </td>
            <td class="field" style="width: 28%">
                <asp:Label runat="server" ID="lblCAP" />
            </td>
            <td class="Row1" style="width: 25%; text-align: left;">
                <asp:Label runat="server" ID="etichettaComuneStatoResidenza" />
            </td>
            <td class="field" style="width: 25%">
                <asp:Label runat="server" ID="lblComuneStatoResidenza" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 22%;">
                <label>
                    Provincia:</label>
            </td>
            <td class="field" style="width: 28%">
                <asp:Label runat="server" ID="lblProvincia" />
            </td>
            <td class="Row1" style="width: 25%; text-align: left;">
                <label>
                    Residente all'estero:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:Label ID="lblResidenteEstero" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <asp:Panel runat="server" ID="pnlFrazioneEstero" Visible="false">
                <td class="Row1" style="width: 22%;">
                    <label>
                        Frazione:</label>
                </td>
                <td class="field" style="width: 28%;">
                    <asp:Label runat="server" ID="lblFrazione" />
                </td>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlDataMorte" Visible="false">
                <td class="Row1" style="width: 22%;">
                    <label>
                        Data Morte:</label>
                </td>
                <td class="field" style="width: 28%;">
                    <asp:Label runat="server" ID="lblDataMorte" />
                </td>
            </asp:Panel>
        </tr>
        <tr>
            <td class="Row1" style="width: 22%;">
                <label>
                    Cittadinanza:</label>
            </td>
            <td class="field full-grid flex-space" colspan="3">
                <asp:DropDownList runat="server" ID="ddlCittadinanza" CssClass="tb8 txtUppercase md width-72-percent"
                    TabIndex="1" Width="81%" />
                <asp:Button runat="server" ID="btnCittadinanza" SkinID="btnAzione1" OnClientClick="PrevalorizzaCittadinanza(); return false;" class="tertiary"
                    Text="<< Italia" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 22%;">
                <label>
                    Telefono:</label>
            </td>
            <td class="field" style="width: 28%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtTel" Width="140px" CssClass="txtUppercase tb8"
                    MaxLength="18" TabIndex="2" onblur="extractPhoneChar(this);" onkeyup="extractPhoneChar(this);"
                    onkeypress="return blockNonPhone(this, event);"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateTxtTel" ControlToValidate="txtTel" ErrorMessage="Numero di telefono non valido (Formato corretto: +12/3456789)"
                    ValidationExpression="^\+?[0-9]+\/?[0-9]+|^\+?[0-9]+$" runat="server" Text="*" CssClass="field-is-required"
                    Display="Dynamic" ValidationGroup="UCTabAnagrafica" Enabled="true" />
            </td>
            <td class="Row1" style="width: 25%; text-align: left;">
                <label>
                    Cellulare:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtCell" Width="140px" CssClass="txtUppercase tb8"
                    MaxLength="18" TabIndex="3" onblur="extractPhoneChar(this);" onkeyup="extractPhoneChar(this);"
                    onkeypress="return blockNonPhone(this, event);"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateTxtCell" ControlToValidate="txtCell"
                    ErrorMessage="Numero di cellulare non valido (Formato corretto: +12/3456789)"
                    ValidationExpression="^\+?[0-9]+\/?[0-9]+|^\+?[0-9]+$" runat="server" Text="*" CssClass="field-is-required"
                    Display="Dynamic" ValidationGroup="UCTabAnagrafica" Enabled="true" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 22%;">
                <label>
                    Email:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtEmail" Width="250px"
                    CssClass="tb8 txtUppercase" MaxLength="50" TabIndex="4"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateTxtEmail" ControlToValidate="txtEmail"
                    ErrorMessage="Indirizzo Email non valido" ValidationExpression="^[a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabAnagrafica" Enabled="true" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 22%;">
                <asp:Label runat="server" ID="lblEtichettaDataPresentazioneDomanda">
                    Data Presentazione:
                </asp:Label>
            </td>
            <td class="field">
                <asp:Label runat="server" ID="lblDataPresentazioneDomanda" />
            </td>
            <td class="Row1" runat="server" id="tdLabelLavoratorePubblico" visible="false">
                <label>
                    Lavoratore pubblico:
                </label>
            </td>
            <td class="field" runat="server" id="tdFieldLavoratorePubblico" visible="false">
                <asp:DropDownList runat="server" ID="ddlLavoratorePubblico" CssClass="txtUppercase tb8 xs"
                    Width="30%">
                    <asp:ListItem Value="" Text=""></asp:ListItem>
                    <asp:ListItem Value="NO" Text="NO"></asp:ListItem>
                    <asp:ListItem Value="SI" Text="SI"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="RFVddlLavoratorePubblico" Display="Dynamic"
                    ErrorMessage="Lavoratore pubblico: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabAnagrafica"
                    ControlToValidate="ddlLavoratorePubblico" InitialValue=""></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 22%;">
                <asp:Label runat="server" ID="lblEtichettaDecorrenzaPensione">
                    Decorrenza Pensione:
                </asp:Label>
            </td>
            <td class="field" style="width: 28%;">
                <asp:Label runat="server" ID="lblDecorrenzaPensione" Style="text-align: left;"></asp:Label>
                <asp:Panel runat="server" ID="pnlTxtDecorrenzaPensioneFSPT" Visible="false">
                    <asp:TextBox Style="text-align: left" runat="server" onblur="setpnlTxtPerfRequisitiVisibility(this);"
                        onkeydown="checkTabPress(this)" ID="txtDecorrenzaPensioneFSPT" Width="55%" Text="gg/mm/aaaa"
                        CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" TabIndex="5" MaxLength="10"
                        DataFormatString="{0:dd/MM/yyyy}"> </asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateDecorrenzaFSPT" ControlToValidate="txtDecorrenzaPensioneFSPT"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$" Enabled="true" Text="*" CssClass="field-is-required"
                        ErrorMessage="Formato data non corretto" Display="Dynamic" ValidationGroup="UCTabAnagrafica" />
                    <asp:RequiredFieldValidator runat="server" ID="validateDecorrenzaReqFSPT" ControlToValidate="txtDecorrenzaPensioneFSPT"
                        Enabled="true" ErrorMessage="Inserire la data di decorrenza della pensione" ValidationGroup="UCTabAnagrafica"
                        Text="*" CssClass="field-is-required" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaPensioneFSPT"
                        Display="Dynamic" ErrorMessage="Decorrenza Pensione: data illogica" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabAnagrafica" ID="customCheckDataDecorrenzaPensioneFSPT"
                        ClientValidationFunction="checkCorrettezzaData" />
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTxtDecorrenzaPensione" Visible="true">
                    <asp:TextBox Style="text-align: left" runat="server" onblur="setpnlTxtPerfRequisitiVisibility(this);"
                        onkeydown="checkTabPress(this)" ID="txtDecorrenzaPensione" Width="55%" Text="mm/aaaa"
                        CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="5" MaxLength="7"
                        DataFormatString="{0:MM/yyyy}"> </asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateDecorrenza" ControlToValidate="txtDecorrenzaPensione"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}$" Enabled="true" Text="*" CssClass="field-is-required" ErrorMessage="Formato data non corretto"
                        Display="Dynamic" ValidationGroup="UCTabAnagrafica" />
                    <asp:RequiredFieldValidator runat="server" ID="validateDecorrenzaReq" ControlToValidate="txtDecorrenzaPensione"
                        Enabled="true" ErrorMessage="Inserire la data di decorrenza della pensione" ValidationGroup="UCTabAnagrafica"
                        Text="*" CssClass="field-is-required" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaPensione" Display="Dynamic"
                        ErrorMessage="Decorrenza Pensione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabAnagrafica"
                        ID="customCheckDecorrenzaPensione" ClientValidationFunction="checkCorrettezzaData" />
                </asp:Panel>
                <input type="hidden" id="htxtDecorrenzaPensione" name="htxtDecorrenzaPensione" runat="server" />
                <input type="hidden" id="hFlagUnicarpe" name="hFlagUnicarpe" runat="server" />
            </td>
            <td class="Row1" style="width: 25%; text-align: left;">
                <label runat="server" id="lblPerfezionamentoReq">
                    Perfezionamento Requisiti:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:Label ID="lbltxtPerfezRequisiti" runat="server" Style="text-align: left;"></asp:Label>
                <asp:Panel runat="server" ID="pnlTxtPerfRequisiti">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtPerfRequisiti" Width="100px"
                        CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" MaxLength="10" TabIndex="6"></asp:TextBox>
                    <asp:CustomValidator runat="server" ControlToValidate="txtPerfRequisiti" Display="Dynamic"
                        ErrorMessage="Data Perfezionamento Requisiti: Inserire una data in formato valido"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabAnagrafica" ID="CustomValidator2" ClientValidationFunction="checkCorrettezzaPerfRequisiti" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtPerfRequisiti" Display="Dynamic"
                        ErrorMessage="Perfezionamento Requisiti: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabAnagrafica"
                        ID="customCheckDataPerfezionamentoRequisiti" ClientValidationFunction="checkCorrettezzaData" />
                    <input type="hidden" id="htxtPerfRequisiti" name="htxtPerfRequisiti" runat="server" />
                </asp:Panel>
            </td>
        </tr>
        <tr id="trOpzione" runat="server" visible="false">
            <td class="Row1" style="width: 22%;" id="tdOpz" visible="false" runat="server">
                <label>
                    Data Opzione:</label>
            </td>
            <td class="field" style="width: 28%">
                <asp:Label runat="server" ID="lblDataOpzione" />
            </td>
            <td class="Row1" style="width: 25%; text-align: left;" id="tdRaggOpz" visible="false"
                runat="server">
                <label>
                    Raggiungimento Opzione:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:Label runat="server" ID="lblDataRaggOpzione"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 22%">
                <label runat="server" id="lblDataCondizioniPerComputo" visible="false">
                    Condizioni per il Computo:</label>
            </td>
            <td class="field" style="width: 28%">
                <asp:Panel runat="server" ID="pnlDataCondizioniPerComputo" Visible="false">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDataCondizioniPerComputo"
                        Width="55%" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validatetxtDataCondizioniPerComputo" ControlToValidate="txtDataCondizioniPerComputo"
                        ErrorMessage="Condizioni per il Computo in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabAnagrafica" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDataCondizioniPerComputo"
                        Display="Dynamic" ErrorMessage="Condizioni per il Computo : data illogica" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabAnagrafica" ID="customCheckDataCondizioniPerComputo" ClientValidationFunction="checkCorrettezzaData" />
                </asp:Panel>
            </td>
        </tr>
        <tr id="trNumFigli" runat="server" visible="false">
            <td class="Row1" style="width: 22%;">
                <label>
                    Numero figli:</label>
            </td>
            <td class="field" style="width: 28%">
                <asp:DropDownList runat="server" ID="ddlFigli" CssClass="txtUppercase tb8 xs" Width="50%"
                    onchange="changeDdlSeltaLM();">
                    <%--<asp:ListItem Value="0" Text=""></asp:ListItem>
                    <asp:ListItem Value="1" Text="1"></asp:ListItem>
                    <asp:ListItem Value="2" Text="2"></asp:ListItem>
                    <asp:ListItem Value="3" Text="più di 2"></asp:ListItem>--%>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorDdlFigli" Display="Dynamic"
                    ErrorMessage="Numero Figli: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabAnagrafica"
                    ControlToValidate="ddlFigli" InitialValue="" Enabled="false"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr id="trNumFigliOpzioneDonna" runat="server" visible="false">
            <td class="Row1" style="width: 22%;">
                <label>
                    Numero figli:</label>
            </td>
            <td class="field" style="width: 28%">
                <asp:DropDownList runat="server" ID="ddlFigliOpzDonna" CssClass="txtUppercase tb8 xs"
                    Width="50%">
                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                    <asp:ListItem Value="1" Text="1"></asp:ListItem>
                    <asp:ListItem Value="2" Text="2 o più figli"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr id="trSceltaLM" runat="server" visible="false">
            <td class="Row1" style="width: 22%; text-align: left;">
                <label>
                    Lavoratrici madri:</label>
            </td>
            <td class="field" colspan="2">
                <asp:DropDownList runat="server" ID="ddlSeltaLM" CssClass="txtUppercase tb8 xs" Width="90%">
                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                    <asp:ListItem Value="1" Text="anticipo età pensionabile"></asp:ListItem>
                    <asp:ListItem Value="2" Text="coefficiente più favorevole"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 22%;">
                <label>
                    Patronato:
                </label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:Label runat="server" ID="lblPatronato" Width="350px">
                </asp:Label>
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlSindacato">
            <tr>
                <td class="Row1" style="width: 22%;">
                    <label>
                        Sindacato:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlSindacato" Width="95%" CssClass="tb8 txtUppercase xl"
                        TabIndex="7">
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="certOpz" Visible="false">
            <tr>
                <td class="Row1" style="width: 22%;" runat="server">
                    <label>
                        Certificato:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="textBoxCert" Width="140px"
                        CssClass="txtUppercase tb8" MaxLength="8" TabIndex="3" onblur="extractPhoneChar(this);"
                        onkeyup="extractPhoneChar(this);" onkeypress="return blockNonPhone(this, event);"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtCell"
                        ErrorMessage="Certificato non valido" ValidationExpression="^?[0-9]+$" runat="server"
                        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabAnagrafica" Enabled="true" />
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorCert" Display="Dynamic"
                        ErrorMessage="Certificato: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabAnagrafica"
                        ControlToValidate="textBoxCert" InitialValue=""></asp:RequiredFieldValidator>
                </td>
            </tr>
        </asp:Panel>
        <!-- pannello sede destinazione--->
        <asp:Panel runat="server" ID="pnlSedeDestinazione">
            <tr>
                <td class="Row1" style="width: 22%;">
                    <asp:Label ID="lblSedeDestinazione" Text="Sede destinazione:" runat="server"></asp:Label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtSedeDestinazione" Width="251px"
                        CssClass="txtUppercase tb8" TabIndex="8"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtSedeDestinazione"
                        Enabled="true" ErrorMessage="Sede Destinazione: campo obbligatorio" ValidationGroup="UCTabAnagrafica"
                        Text="*" CssClass="field-is-required" />
                </td>
            </tr>
        </asp:Panel>
        <!---fine pannello sede destinazione-->
    </table>
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: Center" class="tab-actions-group__first">
                    <asp:Button ID="btnSalva" runat="server" Enabled="true" SkinID="btnAzione1" Text="Salva Anagrafica"
                        Width="130px" OnClick="btnSalva_Click" OnClientClick="if(Page_ClientValidate('UCTabAnagrafica')){aspnetForm.target ='_self'; BlockUI();}"
                        CausesValidation="false" CssClass="primary" />
                </td>
                <td style="text-align: Center">
                    <asp:Button ID="btnAnnulla" runat="server" SkinID="btnAzione1" OnClientClick="javascript:return CleanFields();"
                        Enabled="true" Text="Pulisci" Width="100px" Visible="false" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<asp:HiddenField runat="server" ID="HiddenFieldSedi" />
<asp:HiddenField runat="server" ID="hiddenFieldTipoFondo" />
<asp:HiddenField runat="server" ID="hiddenFieldPerf" />
<asp:HiddenField runat="server" ID="hiddenFieldDecDisabledSuperstiti" Value="NO" />
<asp:HiddenField runat="server" ID="hdnIsEsodati" Value="NO" />
<asp:HiddenField runat="server" ID="hdnIsINPDAP" Value="NO" />
<asp:HiddenField runat="server" ID="hdnIsLikeFSPT" Value="NO" />
<asp:HiddenField runat="server" ID="hdnIsDPRVisibleBefore2011" Value="NO" />
<asp:HiddenField runat="server" ID="hdnTabPressed" Value="NO" />
<asp:HiddenField runat="server" ID="hdnIsFromService" />
<asp:HiddenField runat="server" ID="hdnAbilitazioneMemo28_2024" Value="NO" />
<asp:HiddenField ID="hiddInfoMessage" runat="server" />
