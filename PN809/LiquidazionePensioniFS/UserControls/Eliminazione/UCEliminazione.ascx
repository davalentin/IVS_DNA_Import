<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCEliminazione.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Eliminazione.UCEliminazione" %>
<script type="text/javascript">
    $(document).ready(function () {
        if (document.getElementById("<%= HiddenFieldIsRicostituzione.ClientID %>").value == "NO") {
            SetCalendarioElContDataEvento();
        }
        var txtDec = document.getElementById("<%=txtElContDecorrenza.ClientID %>");
        txtDec.value = document.getElementById("<%=HiddenFieldDecorrenza.ClientID %>").value;
        //registro l'evento
        var txtDataEvento = document.getElementById("<%= txtElContDataEvento.ClientID %>")
        txtDataEvento.onblur = setDataDecorrenza;
        setDataDecorrenza();
        OnChangeddlElContCodice();

        $(document.getElementById("<%= ddlElContCodice.ClientID %>")).change(OnChangeddlElContCodice);
    });

    function SetCalendarioElContDataEvento() {
        //date-picker-base

var isCat = document.getElementById("<%= HiddenFieldIsPLAssegniStraordinari.ClientID %>").value === "SI";

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
            maxDate: isCat ? null : '+0',
            yearRange: '-100:' + '+0:',
            onSelect: function (text, ins) { setDataDecorrenza(); }
        });
    }

    function setDataDecorrenza() {
        if (document.getElementById("<%= HiddenFieldIsRic_TrFAssegniStraordinari.ClientID %>").value != "SI") {
            var dataEvento = document.getElementById("<%=txtElContDataEvento.ClientID %>") != null ? document.getElementById("<%=txtElContDataEvento.ClientID %>").value : "";
            var sDateDec = CalcolaDataDecorrenzaByDataEvento(dataEvento);
            document.getElementById("<%=txtElContDecorrenza.ClientID %>").value = sDateDec;
            document.getElementById("<%=HiddenFieldDecorrenza.ClientID %>").value = sDateDec;
        }
    }

    function setDataDecorrenzaByDecorrenzaOriginaria() {
        if (document.getElementById("<%= HiddenFieldIsPLAssegniStraordinari.ClientID %>").value == "SI") {
            var decorrenzaOriginaria = document.getElementById("<%=HiddenFieldDecorrenzaPensione.ClientID %>") != null ? document.getElementById("<%=HiddenFieldDecorrenzaPensione.ClientID %>").value : "";
            var sDateDec = CalcolaDataDecorrenzaByDecorrenzaOriginaria(decorrenzaOriginaria);
            document.getElementById("<%=txtElContDecorrenza.ClientID %>").value = sDateDec;
            document.getElementById("<%=HiddenFieldDecorrenza.ClientID %>").value = sDateDec;
        }
    }

    function CalcolaDataDecorrenzaByDecorrenzaOriginaria(sDecorrenzaOriginaria) {
        var parts = sDecorrenzaOriginaria.split("/");
        var year = parts[2];
        var month = parts[1];
        var day = parts[0];
        var formattedDataEvento = year + "/" + month + "/" + day;

        if (!Date.parse(formattedDataEvento)) {
            return "";
        }
        var decorrenzaOriginaria = convertString2Date(sDecorrenzaOriginaria);
        var dateDecorrenza = new Date(decorrenzaOriginaria.getFullYear(), decorrenzaOriginaria.getMonth(), 1);
        return convertDate2String(dateDecorrenza).substring(3);
    }

    function OnChangeddlElContCodice() {
        var codiceEliminazione = document.getElementById("<%= ddlElContCodice.ClientID %>");
        if (document.getElementById("<%= HiddenFieldIsMemo102Abilitato.ClientID %>").value == "SI") {
            if (codiceEliminazione && ($(codiceEliminazione).val() == 1) && document.getElementById("<%= HiddenFieldIsRic_TrFAssegniStraordinari.ClientID %>").value != "SI") {
                document.getElementById("<%= txtElContDataEvento.ClientID %>").disabled = false;
                var dataMorte = document.getElementById("<%= HiddenFieldDataMorte.ClientID %>");
                if (dataMorte && dataMorte.value !== "") {
                    document.getElementById("<%= txtElContDataEvento.ClientID %>").value = dataMorte.value;
                    document.getElementById("<%= HiddenFieldDataEvento.ClientID %>").value = document.getElementById("<%= txtElContDataEvento.ClientID %>").value;
                    $(document.getElementById("<%= txtElContDataEvento.ClientID %>")).datepicker("disable");

                    setDataDecorrenza();
                }
            }
            else if (codiceEliminazione && ($(codiceEliminazione).val() == 6) && document.getElementById("<%= HiddenFieldIsPLAssegniStraordinari.ClientID %>").value == "SI") {
                setDataDecorrenzaByDecorrenzaOriginaria();
                document.getElementById("<%= txtElContDataEvento.ClientID %>").value = "";
                document.getElementById("<%= txtElContDataEvento.ClientID %>").disabled = true;
                ValidatorEnable(document.getElementById('<%= RDVtxtElContDataEvento.ClientID %>'), false);
                ValidatorEnable(document.getElementById('<%= RDV2txtElContDataEvento.ClientID %>'), false);
                ValidatorEnable(document.getElementById('<%= customCheckDataDataEvento.ClientID %>'), false);
            }
            else if (document.getElementById("<%= HiddenFieldIsRic_TrFAssegniStraordinari.ClientID %>").value == "SI") {
                document.getElementById("<%= txtElContDataEvento.ClientID %>").disabled = true;
            }
            else if (codiceEliminazione && $(codiceEliminazione).val() != 1) {
                document.getElementById("<%= txtElContDataEvento.ClientID %>").disabled = false;
                $(document.getElementById("<%= txtElContDataEvento.ClientID %>")).datepicker("enable");
                if (document.getElementById("<%= HiddenFieldIsDomandaIndennitaUnaTantum_AGO.ClientID %>").value == "NO") {
                    document.getElementById("<%= HiddenFieldDataEvento.ClientID %>").value = "";
                }
            }
        }
        else {
            if (codiceEliminazione && ($(codiceEliminazione).val() == 1) && document.getElementById("<%= HiddenFieldIsRic_TrFAssegniStraordinari.ClientID %>").value != "SI") {
                var dataMorte = document.getElementById("<%= HiddenFieldDataMorte.ClientID %>");
                if (dataMorte && dataMorte.value !== "") {
                    document.getElementById("<%= txtElContDataEvento.ClientID %>").value = dataMorte.value;
                    document.getElementById("<%= HiddenFieldDataEvento.ClientID %>").value = document.getElementById("<%= txtElContDataEvento.ClientID %>").value;
                    $(document.getElementById("<%= txtElContDataEvento.ClientID %>")).datepicker("disable");

                    setDataDecorrenza();
                }
            }
            else if (codiceEliminazione && $(codiceEliminazione).val() != 1) {
                $(document.getElementById("<%= txtElContDataEvento.ClientID %>")).datepicker("enable");
                if (document.getElementById("<%= HiddenFieldIsDomandaIndennitaUnaTantum_AGO.ClientID %>").value == "NO") {
                    document.getElementById("<%= HiddenFieldDataEvento.ClientID %>").value = "";
                }
            }
        }
        if (document.getElementById("<%= HiddenFieldLockDataEvento.ClientID %>").value == "SI") {
            document.getElementById("<%= txtElContDataEvento.ClientID %>").disabled = true;
            $(document.getElementById("<%= txtElContDataEvento.ClientID %>")).datepicker("disable");
        }
    }

    function DisableValidator() {
        SwitchValidator('.offClass', false);
    }

    function SwitchValidator(cssClass, onOff) {
        for (i = 0; i < $(cssClass).length; i++) {
            var control = $(cssClass)[i]
            var validatorid = control.id;
            val = document.getElementById(validatorid);
            if (val != null && val != 'undefined') {
                var s = val.id;
                if (s.indexOf("RequiredField") != -1) {
                    ValidatorEnable(val, onOff);
                }
            }
        }
    }

</script>
<asp:Panel runat="server" ID="pnlEliminazione">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1 shift-full-grid" colspan="4">
                <label style="font-weight: bold" class="section-label">Eliminazione Contestuale</label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%; padding-left: 5px;">
                <label>
                    Codice:</label>
            </td>
            <td class="field full-grid" style="width: 25%; padding-left: 2px;" colspan="3">
                <asp:DropDownList runat="server" ID="ddlElContCodice" Width="88%" CssClass="tb8 txtUppercase wide"
                    TabIndex="25">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="RFVddlElContCodice" ControlToValidate="ddlElContCodice"
                    Enabled="true" ErrorMessage="Codice : E' un campo obbligatorio" ValidationGroup="UCELM" Display="Dynamic"
                    Text="*" CssClass="field-is-required" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%; padding-left: 5px;">
                <label id="lblDecorrenza" runat="server">
                    Decorrenza:</label>
            </td>
            <td class="field" style="width: 25%; padding-left: 2px;">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtElContDecorrenza" Width="95px"
                    CssClass="tb8 txtUppercase dateMMaaaa" MaxLength="7" TabIndex="26" Text="mm/aaaa"
                    Enabled="false"></asp:TextBox>
            </td>
            <td class="Row1" style="width: 25%; padding-left: 15px;">
                <label id="lblDataEvento" runat="server">
                    Data Evento:</label>
            </td>
            <td class="field" style="width: 25%; padding-left: 13px;">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtElContDataEvento" Width="95px"
                    MaxLength="10" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="27" Text="gg/mm/aaaa"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateTxtElContDataEvento" ControlToValidate="txtElContDataEvento"
                    Display="Dynamic" ErrorMessage="Data Evento Eliminazione Contestuale: Inserire la data in un formato valido"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    ValidationGroup="UCELM" />
                <asp:RequiredFieldValidator runat="server" ID="RDVtxtElContDataEvento" ControlToValidate="txtElContDataEvento"
                    Enabled="true" ErrorMessage="Data Evento: E' un campo obbligatorio" ValidationGroup="UCELM"
                    Text="*" Display="Dynamic" CssClass="offClass field-is-required" />
                <asp:RequiredFieldValidator runat="server" ID="RDV2txtElContDataEvento" InitialValue="gg/mm/aaaa"
                    ControlToValidate="txtElContDataEvento" Enabled="true" ErrorMessage="Data Evento: E' un campo obbligatorio "
                    ValidationGroup="UCELM" Text="*"  Display="Dynamic" CssClass="offClass field-is-required" />
                <asp:CustomValidator runat="server" ControlToValidate="txtElContDataEvento" Display="Dynamic"
                    ErrorMessage="Data Evento: data illogica" Text="*" ValidationGroup="UCELM" ID="customCheckDataDataEvento"
                    ClientValidationFunction="checkCorrettezzaData" CssClass="offClass field-is-required" />
            </td>
        </tr>
        <tr runat="server" id="trFineCalcoloArretrati" visible="false">
            <td class="Row1" style="width: 25%; padding-left: 5px;">
                <label>
                    Data Fine Calcolo Arretrati:</label>
            </td>
            <td class="field" style="width: 25%; padding-left: 2px;">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataFineCalcoloArretrati"
                    Width="95px" MaxLength="7" CssClass="txtUppercase tb8 dateMMaaaa date-picker"
                    Text="mm/aaaa"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtDataFineCalcoloArretrati"
                    ControlToValidate="txtDataFineCalcoloArretrati" Display="Dynamic" ErrorMessage="Data Fine Calcolo Arretrati: Inserire la data in un formato valido"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCELM" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtDataFineCalcoloArretrati" ControlToValidate="txtDataFineCalcoloArretrati"
                    Enabled="true" ErrorMessage="Data Fine Calcolo Arretrati: E' un campo obbligatorio"
                    ValidationGroup="UCELM" Text="*" CssClass="field-is-required" Display="Dynamic" />
                <asp:RequiredFieldValidator runat="server" ID="RFV2txtDataFineCalcoloArretrati" InitialValue="mm/aaaa"
                    ControlToValidate="txtDataFineCalcoloArretrati" Enabled="true" ErrorMessage="Data Fine Calcolo Arretrati: E' un campo obbligatorio"
                    ValidationGroup="UCELM" Text="*" CssClass="field-is-required" Display="Dynamic" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataFineCalcoloArretrati"
                    Display="Dynamic" ErrorMessage="Data Fine Calcolo Arretrati: data illogica" Text="*" CssClass="field-is-required"
                    ValidationGroup="UCELM" ID="CVtxtDataFineCalcoloArretrati" ClientValidationFunction="checkCorrettezzaData" />
            </td>
            <td class="Row1" style="width: 25%; padding-left: 15px;">
                <label id="lblDataFineCalcoloArretratiGP1AP2A" runat="server">
                    Data Fine Calcolo Arretrati (GP1AP2A):</label>
            </td>
            <td class="field" style="width: 25%; padding-left: 13px;">
                <asp:Label runat="server" ID="lblDataFineCalcoloArretratiStorico"></asp:Label>
            </td>
        </tr>
        <tr runat="server" id="trCampiRic" visible="false">
            <td class="Row1" style="width: 25%; padding-left: 5px;">
                <label>
                    Data Cessazione Diritto:</label>
            </td>
            <td class="field" style="width: 25%; padding-left: 2px;">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataCessazioneDiritto"
                    MaxLength="10" CssClass="txtUppercase tb8 dateGGmmAAAA date-picker-base" Text="gg/mm/aaaa"
                    Width="105px"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtDataCessazioneDiritto" ControlToValidate="txtDataCessazioneDiritto"
                    Display="Dynamic" ErrorMessage="Data Cessazione Diritto: Inserire la data in un formato valido"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    ValidationGroup="UCELM" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataCessazioneDiritto"
                    Display="Dynamic" ErrorMessage="Data Cessazione Diritto: data illogica" Text="*" CssClass="field-is-required"
                    ValidationGroup="UCELM" ID="CVtxtDataCessazioneDiritto" ClientValidationFunction="checkCorrettezzaData" />
            </td>
            <td class="Row1" style="width: 25%; padding-left: 15px;">
                <label>
                    Data Comunicazione Eliminazione:</label>
            </td>
            <td class="field" style="width: 25%; padding-left: 13px;">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataComunicazioneEliminazione"
                    Width="95px" MaxLength="7" CssClass="txtUppercase tb8 dateMMaaaa date-picker"
                    Text="mm/aaaa"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtDataComunicazioneEliminazione"
                    ControlToValidate="txtDataComunicazioneEliminazione" Display="Dynamic" ErrorMessage="Data Comunicazione Eliminazione: Inserire la data in un formato valido"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCELM" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataComunicazioneEliminazione"
                    Display="Dynamic" ErrorMessage="Data Comunicazione Eliminazione: data illogica"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCELM" ID="CVtxtDataComunicazioneEliminazione" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Pannello bottoni -->
<div style="margin-right: 40px;" class="containerWidth xs">
    <table width="100%" style="min-height: 100px;" class="tab-actions-group">
        <tr>
            <td style="text-align: right; vertical-align: bottom;" class="tab-actions-group__first">
                <asp:Button ID="btnSalva" runat="server" CausesValidation="false" ValidationGroup="UCELM"
                    SkinID="btnAzione1" Width="190px" OnClick="btnSalva_Click" Text="Salva Dati Eliminazione"
                    OnClientClick="if(Page_ClientValidate('UCELM')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
            </td>
            <td style="text-align: left; vertical-align: bottom;">
                <asp:Button ID="btnElimina" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Dati Eliminazione" Width="190px" OnClick="btnElimina_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Eliminazione?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
<asp:HiddenField runat="server" ID="HiddenFieldIsRicostituzione" />
<asp:HiddenField runat="server" ID="HiddenFieldDecorrenza" />
<asp:HiddenField runat="server" ID="HiddenFieldDataMorte" />
<asp:HiddenField runat="server" ID="HiddenFieldDataEvento" />
<asp:HiddenField runat="server" ID="HiddenFieldIsRic_TrFAssegniStraordinari" Value="NO" />
<asp:HiddenField runat="server" ID="HiddenFieldIsPLAssegniStraordinari" Value="NO" />
<asp:HiddenField runat="server" ID="HiddenFieldDecorrenzaPensione" />
<asp:HiddenField runat="server" ID="HiddenFieldIsMemo102Abilitato" Value="NO" />
<asp:HiddenField runat="server" ID="HiddenFieldLockDataEvento" Value="NO" />
<asp:HiddenField runat="server" ID="HiddenFieldIsDomandaIndennitaUnaTantum_AGO" Value="NO" />
