<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCExCombattente.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBenefici.UCExCombattente" %>
<script type="text/javascript">
    var uiDPchiuso = false;
    $(document).ready(function () {
        if (document.getElementById("<%=pnlExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI.ClientID%>") != null) {
            if (($("#<%=radioL140.ClientID %>").is(':checked')) || ($("#<%=radioL336.ClientID %>").is(':checked'))) {
                SwitchValidator('.offClass', false);
                CheckValidator();
            }

            $("#<%=radioL140.ClientID %>").click(function () {
                if ($(this).is(':checked')) {
                    SwitchValidator('.offClass', false);
                    CheckValidator();
                }
            });

            $("#<%=radioL336.ClientID %>").click(function () {
                if ($(this).is(':checked')) {
                    SwitchValidator('.offClass', false);
                    CheckValidator();
                }
            });

            $('.offClass').attr('disabled', true);
            var doAction = false;
            var cssClass;
            var tipoRicerca = document.getElementById("<%=HiddenSelectedLegge.ClientID%>").value; //L'hidden field è valorizzato con il tipo di ricerca
            if (tipoRicerca == 'Legge140') { //Nel caso di un postback riabilito il blocco precedentemente selezionato
                doAction = true;
                cssClass = '.onClassLegge140';
                $(document.getElementById("<%=radioL140.ClientID %>")).attr("disabled", true);
                $(document.getElementById("<%=txtDecorrenza.ClientID%>")).datepicker({
                    changeMonth: true,
                    changeYear: true,
                    showButtonPanel: true,
                    dateFormat: 'mm/yy',
                    showOn: 'button',
                    buttonImageOnly: true,
                    buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                    maxDate: '+100y',
                    minDate: '-100y',
                    //yearRange: 'c-50:' + 'c+0:',

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
                //$(document.getElementById("<%=txtDecorrenza.ClientID%>")).unmask();
                //$(document.getElementById("<%=txtDecorrenza.ClientID%>")).mask("99/9999");
            }
            else if (tipoRicerca == 'Legge336') {
                doAction = true;
                cssClass = '.onClassLegge336';
                $(document.getElementById("<%=radioL336.ClientID %>")).attr("disabled", true);
            }

            else { //nel caso del primo caricamento della pagina
                $('.offClass').val('');
                $('input:radio').attr('checked', false);
            }
            if (doAction) {
                $(cssClass).removeAttr('disabled');

                SwitchValidator(cssClass, true);
            }
        }
    });

    function SetRadio(rb) {
        if (document.getElementById("<%=pnlExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI.ClientID%>") != null) {
            $('input:radio').attr('checked', false); //Disabilita tutti i radio button
            $('.offClass').attr('disabled', true); //Disabilita tutti gli oggetti con la class "offClass"

            //$('.offClass').val(''); //Pulisce tutti i campi con la class "offClass"


            $(document.getElementById("<%=radioL140.ClientID %>")).attr("disabled", false);
            $(document.getElementById("<%=radioL336.ClientID %>")).attr("disabled", false);
            $('.' + rb.getAttribute("EnableClass")).removeAttr('disabled'); //Abilita gli oggetti con l'attributo specificato

            if (rb.getAttribute("EnableClass") == "onClassLegge140") {

                document.getElementById("<%=HiddenSelectedLegge.ClientID%>").value = "Legge140"; //utilizzato per abilitare i pannelli in caso di messaggio di errore lato servizio

                $(document.getElementById("<%=radioL140.ClientID %>")).attr("checked", true);
                $(document.getElementById("<%=radioL140.ClientID %>")).attr("disabled", true);

                $(document.getElementById("<%=ddlExCombattente.ClientID %>")).focus();
                $(document.getElementById("<%=ddlExCombattente.ClientID%>")).removeAttr('disabled');

                if (document.getElementById("<%=txtDecorrenza.ClientID%>").value == 'MM/AAAA' || document.getElementById("<%=txtDecorrenza.ClientID%>").value == '')
                    document.getElementById("<%=txtDecorrenza.ClientID%>").value = 'MM/AAAA';

                $(document.getElementById("<%=txtDecorrenza.ClientID%>")).datepicker({
                    changeMonth: true,
                    changeYear: true,
                    showButtonPanel: true,
                    dateFormat: 'mm/yy',
                    showOn: 'button',
                    buttonImageOnly: true,
                    buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                    maxDate: '+100y',
                    minDate: '-100y',
                    //yearRange: 'c-50:' + 'c+0:',

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
            }
            else if (rb.getAttribute("EnableClass") == "onClassLegge336") {
                document.getElementById("<%=HiddenSelectedLegge.ClientID%>").value = "Legge336"; //utilizzato per abilitare i pannelli in caso di messaggio di errore lato servizio

                $(document.getElementById("<%=radioL336.ClientID %>")).attr("checked", true);
                $(document.getElementById("<%=radioL336.ClientID %>")).attr("disabled", true);

                $(document.getElementById("<%=ddlMaggExCombattente.ClientID %>")).focus();

                $(document.getElementById("<%=ddlMaggExCombattente.ClientID%>")).removeAttr('disabled');

                $(document.getElementById("<%=txtDecorrenza.ClientID%>")).datepicker("destroy");
                if (document.getElementById("<%=txtDecorrenza.ClientID%>").value == 'MM/AAAA')
                    document.getElementById("<%=txtDecorrenza.ClientID%>").value = '';
            }
            //nel RadioButton via codeBehind
            SwitchValidator('.offClass', false); //Disabilita tutti i validatori
            rb.checked = true; //Seleziona il radioButton che ha scatenato l'evento
        }
    }


    function SwitchValidator(cssClass, onOff) {
        if (document.getElementById("<%=pnlExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI.ClientID%>") != null) {
            for (i = 0; i < $(cssClass).length; i++) {
                var control = $(cssClass)[i]
                var validatorid = control.id;
                val = document.getElementById(validatorid);
                if (val != null && val != 'undefined') {
                    var s = val.id;
                    if (s.indexOf("Validator") != -1) {
                        //ValidatorEnable(val, onOff);
                        ValidatorEnableCustom(val, onOff);
                    }
                }
            }
        }
    }

    function ValidatorEnableCustom(val, enable) {
        if (document.getElementById("<%=pnlExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI.ClientID%>") != null) {
            var tipoFondoEL = document.getElementById("<%=HdnFondoEL.ClientID%>").value;
            var validatoreExCombattente = document.getElementById("<%=RequiredFieldValidator1.ClientID%>");
            if (tipoFondoEL == "NO" || (validatoreExCombattente != null && val.id != validatoreExCombattente.id)) {
                val.enabled = (enable != false);
                if (!val.enabled) {
                    ValidatorValidate(val);
                    ValidatorUpdateIsValid();
                }
            }
        }
    }

    function CheckValidator() {
        if (document.getElementById("<%=pnlExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI.ClientID%>") != null) {
            for (i = 0; i < $('input:radio').length; i++) {
                var control = $('input:radio')[i]
                if (control.checked) {
                    SwitchValidator('.' + control.getAttribute("EnableClass"), true);
                }
            }
        }
    }

    function EnableControls() {
        if (document.getElementById("<%=pnlExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI.ClientID%>") != null) {
            $('.offClass').attr('disabled', false);
            if (document.getElementById("<%=HiddenSelectedLegge.ClientID%>").value == "Legge140") {
                $('.onClassLegge336').css({ "background-color": "#D3D3D3" });
                $('.onClassLegge336').css("color", "#80808F");
            }
            else if (document.getElementById("<%=HiddenSelectedLegge.ClientID%>").value == "Legge336") {
                $('.onClassLegge140').css({ "background-color": "#D3D3D3" });
                $('.onClassLegge140').css("color", "#80808F");
            }
            else
                $('.offClass').attr('disabled', true);
        }
    }
</script>
<style type="text/css">
    div.form p
    {
        float: left;
        margin-top: 7px;
        margin-bottom: 6px;
        font-size: small;
    }
    div.form label
    {
        /*float: left;*/
        width: 50px;
        margin-right: 10px;
        text-align: right;
        padding-top: 1px;
        font-size: small;
    }
    div.form select
    {
        float: left;
        width: 10em;
        font-family: verdana, arial;
        font-size: small;
        padding-top: 1px;
    }
    /*input[disabled="disabled"], input.disabled, input[disabled]
        /*{
            background: #D3D3D3;
            color: #D3D3D3;
        }*/
    .disabledWithBackground input[type="text"][disabled="disabled"], .disabledWithBackground input[type="text"].disabled, .disabledWithBackground input[type="text"][disabled]
    {
        background: #D3D3D3;
        color: #D3D3D3;
    }
    .disabledWithBackground input[type="submit"][disabled="disabled"], .disabledWithBackground input[type="submit"].disabled, .disabledWithBackground input[type="submit"][disabled]
    {
        background: #D3D3D3;
        color: #D3D3D3;
    }
    
    .disabledWithBackground select[disabled="disabled"], .disabledWithBackground select.disabled, .disabledWithBackground select[disabled]
    {
        background: #D3D3D3;
        color: #D3D3D3;
    }
    .etichetta
    {
        width: 125px;
    }
    .radioButton
    {
        width: 21px;
    }
</style>
<asp:Panel ID="pnlExCombattenteEL_ET_TT_VL_FS_GAS_DZ_ES_PM_PI" runat="server" Visible="false"
    CssClass="disabledWithBackground">
    <div id="pdivL140" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-top: 15px; margin-bottom: 4px"
        runat="server">
        <table class="tabellaFormattazione grid grid-size-radio">
            <tr>
                <td class="radioButton" class="Row1" style="width: 5%">
                    <asp:RadioButton runat="server" ID="radioL140" CssClass="Legge140 radioButton like-enabled" />
                </td>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblTitoloLegge140" runat="server" Text="L. 140" Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione grid grid-size-20-col-2" cellpadding="3" cellspacing="1" border="0" width="100%">
            <tr>
                <td class="Row1" style="width: 30%">
                    <label>
                        Codice Ex Combattente:</label>
                </td>
                <td class="Row1" style="width: 70%">
                    <asp:DropDownList CssClass="txtUppercase tb8 offClass onClassLegge140" ID="ddlExCombattente"
                        runat="server" TabIndex="1" Width="480px">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="ddlExCombattente"
                        Enabled="false" ErrorMessage="Codice Ex Combattente obbligatorio" Text="*" Display="Dynamic"
                        ValidationGroup="UCTabExCombattente" CssClass="offClass  field-is-required onClassLegge140" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 30%">
                    <label>
                        Decorrenza:</label>
                </td>
                <td class="Row1" style="width: 70%">
                    <asp:TextBox runat="server" ID="txtDecorrenza" CssClass="tb8 txtUppercase offClass onClassLegge140"
                        MaxLength="7" Width="100px" TabIndex="2" Text="MM/AAAA"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="regexDecorrenzaValidator" ControlToValidate="txtDecorrenza"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}$" Enabled="false" Text="*" CssClass="field-is-required" ErrorMessage="Formato data non corretto"
                        Display="Dynamic" ValidationGroup="UCTabExCombattente" />
                    <asp:RegularExpressionValidator runat="server" ID="txtDecorrenzaValidator" ControlToValidate="txtDecorrenza"
                        Display="Dynamic" Enabled="false" ErrorMessage="Inserire un formato data valido per Decorrenza"
                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9\/]+|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabExCombattente" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabExCombattente"
                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </tr>
        </table>
    </div>
    <br />
    <div id="pdivL336" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px; margin-bottom: 4px"
        runat="server">
        <table class="tabellaFormattazione grid grid-size-radio">
            <tr>
                <td class="radioButton" class="Row1" style="width: 5%">
                    <asp:RadioButton runat="server" ID="radioL336" CssClass="Legge336 radioButton like-enabled" />
                </td>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblTitoloLegge336" runat="server" Text="L. 336" Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione grid grid-size-20" cellpadding="3" cellspacing="1" border="0" width="100%">
            <tr>
                <td class="shift-full-grid">
                    <asp:Panel ID="pnl336EL_ET_GAS_DZ_ES_PM_PI" runat="server" Visible="false" CssClass="full-width">
                        <table class="tabellaFormattazione grid grid-size-20-col-2">
                            <tr>
                                <td class="Row1" style="width: 30%">
                                    <label>
                                        Maggiorazione - Ex Combattente:</label>
                                </td>
                                <td class="Row1" style="width: 70%">
                                    <asp:DropDownList CssClass="txtUppercase tb8 offClass onClassLegge336" ID="ddlMaggExCombattente"
                                        runat="server" TabIndex="1" Width="480px">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="ddlMaggExCombattente"
                                        Enabled="false" ErrorMessage="Maggiorazione - Ex Combattente obbligatorio" Text="*" 
                                        Display="Dynamic" ValidationGroup="UCTabExCombattente" CssClass="offClass field-is-required  onClassLegge336" />
                                </td>
                            </tr>
                            <tr>
                                <td class="Row1" style="width: 30%">
                                    <label>
                                        RMS Senza Legge 336/70 Quota A:</label>
                                </td>
                                <td class="Row1" style="width: 70%">
                                    <asp:TextBox runat="server" ID="txtRMSL33670QuotaA" CssClass="txtUppercase tb8 offClass onClassLegge336"
                                        TabIndex="1" onblur="extractNumber(this,4,false);" onkeyup="extractNumber(this,4,false);"
                                        MaxLength="11" onkeypress="return blockNonNumbers(this, event, true, false);"
                                        Width="22%" />
                                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" ControlToValidate="txtRMSL33670QuotaA"
                                        Display="Dynamic" ErrorMessage="RMS Senza Legge 336/70 Quota A: Inserire massimo 6 cifre intere e 4 decimali"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,6}(\,\d{1,4})?" ValidationGroup="UCTabExCombattente" />
                                </td>
                            </tr>
                            <asp:Panel runat="server" ID="pnlRMSL33670QuotaB">
                                <tr>
                                    <td class="Row1" style="width: 30%">
                                        <label>
                                            RMS Senza Legge 336/70 Quota B:</label>
                                    </td>
                                    <td class="Row1" style="width: 70%">
                                        <asp:TextBox runat="server" ID="txtRMSL33670QuotaB" CssClass="txtUppercase tb8 offClass onClassLegge336"
                                            TabIndex="2" onblur="extractNumber(this,4,false);" onkeyup="extractNumber(this,4,false);"
                                            MaxLength="11" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            Width="22%" />
                                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator6" ControlToValidate="txtRMSL33670QuotaB"
                                            Display="Dynamic" ErrorMessage="RMS Senza Legge 336/70 Quota B: Inserire massimo 6 cifre intere e 4 decimali"
                                            Text="*" CssClass="field-is-required" ValidationExpression="\d{1,6}(\,\d{1,4})?" ValidationGroup="UCTabExCombattente" />
                                    </td>
                                </tr>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="pnlPercentualeMaggSL33670">
                                <tr>
                                    <td class="Row1" style="width: 30%">
                                        <label>
                                            Percentuale di maggiorazione Senza Legge 336/70:</label>
                                    </td>
                                    <td class="Row1" style="width: 70%">
                                        <asp:TextBox runat="server" ID="txtPercentualeMaggSL33670" CssClass="txtUppercase tb8 offClass onClassLegge336"
                                            TabIndex="3" Width="10%" MaxLength="2" onblur="extractNumber(this,0,false);"
                                            onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);" />
                                        <asp:RegularExpressionValidator runat="server" ID="txtPercentualeMaggSL33670Validator"
                                            ControlToValidate="txtPercentualeMaggSL33670" Display="Dynamic" ErrorMessage="Inserire il valore in un formato valido per Percentuale di maggiorazione Senza Legge 336/70"
                                            Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]+$" ValidationGroup="UCTabExCombattente" />
                                        <label>
                                            %</label>
                                    </td>
                                </tr>
                            </asp:Panel>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="HiddenSelectedLegge" />
    <asp:HiddenField runat="server" ID="HdnFondoEL" />
</asp:Panel>
<br />
<asp:Panel ID="pnlExCombattentePT" runat="server" Visible="false">
    <table>
        <tr>
            <td class="Row1" style="width: 30%">
                <label>
                    Numero scatti:</label>
            </td>
            <td class="Row1" style="width: 70%">
                <asp:TextBox runat="server" ID="txtDirittoScatti" CssClass="txtUppercase tb8 offClass onClassLegge336"
                    TabIndex="3" Width="90px" MaxLength="2" />
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="txtDirittoScatti"
                    Display="Dynamic" ErrorMessage="Inserire il valore in un formato valido per Numero scatti"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]+$" ValidationGroup="UCTabExCombattente" />
            </td>
        </tr>
    </table>
</asp:Panel>
<div style="margin-top: 100px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaExCombattente" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Ex Combattente" Width="180px" OnClick="SalvaExCombattente_Click"
                    OnClientClick="javascript:CheckValidator(); if(Page_ClientValidate('UCTabExCombattente')){aspnetForm.target ='_self'; EnableControls(); BlockUI();}"  CssClass="primary"/>
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaExCombattente" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Ex Combattente" Width="180px" OnClick="EliminaExCombattente_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare Ex Combattente?')) return false; else BlockUI();" CssClass="ghost-delete"/>
            </td>
        </tr>
    </table>
</div>
