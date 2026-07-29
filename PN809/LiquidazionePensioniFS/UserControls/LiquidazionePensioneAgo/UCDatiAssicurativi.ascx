<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiAssicurativi.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.DatiAssicurativi" %>
<script type="text/javascript">
    $(document).ready(function() {
    SetCalendariInizioFineAssicurazione();
});
        function SetCalendariInizioFineAssicurazione() {
            if ($(document.getElementById("<%=pnlInizioFineAssicurazione.ClientID%>")).is(':disabled') == false) {
                if ($(document.getElementById("<%=txtInizioAssicurazione.ClientID%>")).is(':disabled') == false) {
                    $(document.getElementById("<%=txtInizioAssicurazione.ClientID%>")).datepicker({
                        changeMonth: true,
                        changeYear: true,
                        changeDay: true,
                        showButtonPanel: true,
                        dateFormat: 'dd/mm/yy',
                        showOn: 'button',
                        buttonImageOnly: true,
                        buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                        yearRange: '-70:' + '+0:',
                        minDate: '-70y',
                        maxDate: '+0',
                    });
                    //$(document.getElementById("<%=txtInizioAssicurazione.ClientID%>")).unmask();
                    //$(document.getElementById("<%=txtInizioAssicurazione.ClientID%>")).mask("99/99/9999");
                }
                if ($(document.getElementById("<%=txtFineAssicurazione.ClientID%>")).is(':disabled') == false) {
                    $(document.getElementById("<%=txtFineAssicurazione.ClientID%>")).datepicker({
                        changeMonth: true,
                        changeYear: true,
                        changeDay: true,
                        showButtonPanel: true,
                        dateFormat: 'dd/mm/yy',
                        showOn: 'button',
                        buttonImageOnly: true,
                        buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                        yearRange: '-70:' + '+0:',
                        minDate: '-70y',
                        maxDate: '+0',
                    });
                    //$(document.getElementById("<%=txtFineAssicurazione.ClientID%>")).unmask();
                    //$(document.getElementById("<%=txtFineAssicurazione.ClientID%>")).mask("99/99/9999");
                }
            }  
            if ($(document.getElementById("<%=pnlInizioFineUltimoLavoro.ClientID%>")).is(':disabled') == false) {
                if ($(document.getElementById("<%=txtInizioUltLav.ClientID%>")).is(':disabled') == false) {
                    $(document.getElementById("<%=txtInizioUltLav.ClientID%>")).datepicker({
                        changeMonth: true,
                        changeYear: true,
                        changeDay: true,
                        showButtonPanel: true,
                        dateFormat: 'dd/mm/yy',
                        showOn: 'button',
                        buttonImageOnly: true,
                        buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                        yearRange: '-70:' + '+0:',
                        minDate: '-70y',
                        maxDate: '+0',
                    });
                }
                if ($(document.getElementById("<%=txtFineUltLav.ClientID%>")).is(':disabled') == false) {
                    $(document.getElementById("<%=txtFineUltLav.ClientID%>")).datepicker({
                        changeMonth: true,
                        changeYear: true,
                        changeDay: true,
                        showButtonPanel: true,
                        dateFormat: 'dd/mm/yy',
                        showOn: 'button',
                        buttonImageOnly: true,
                        buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                        yearRange: '-70:' + '+0:',
                        minDate: '-70y',
                        maxDate: '+0',
                    });
                }
            }
        }


        $(document).ready(function() {
            $(".autotab").keyup(function() {
                if ($(this).attr("maxlength") == $(this).val().length) {
                    var index = $(".autotab").index(this);
                    var item = $($(".autotab")[++index]);
                    if (item.length > 0)
                        item.focus();
                }
            });

            setHiddenFieldAttivitaEconomicaProfessioneIndividuale();

//            $(".checkBenefici").keyup(function() {
//                SetChkBenefici();
//            });
        });


        function CheckLenghtAttEconomica2(source, args) {
            var attEcon = args.Value;
            if (attEcon.length != 2)
                args.IsValid = false;
            else
                args.IsValid = true;
            return false;
        }

        function CheckLenghtAttEconomica3(source, args) {
            var attEcon = args.Value;
            if (attEcon.length != 3)
                args.IsValid = false;
            else
                args.IsValid = true;
            return false;
        }

        function CheckLenghtProfIndividuale(source, args) {
            var profIndiv = args.Value;
            if (profIndiv.length != 3)
                args.IsValid = false;
            else
                args.IsValid = true;
            return false;
        }

        function CheckBeneficiDisabled() {

            var attivitaEconomica = $("#<%=hiddenFieldAttivitaEconomica.ClientID %>").val();
            var professioneIndividuale = $("#<%= hiddenFieldProfessioneIndividuale.ClientID %>").val();
            var skip = $("#<%= hdnSKIP_ManageEnableBeneficiJS.ClientID %>").val();
            var siglaCategoria = document.getElementById("<%= HiddenFieldSiglaCategoria.ClientID %>").value;

            if (//for Prepensionamento
                (attivitaEconomica == 92 && professioneIndividuale == 257) ||
                (attivitaEconomica == 3 && professioneIndividuale == 326) ||
                (attivitaEconomica == 3 && professioneIndividuale == 350) ||
                (attivitaEconomica == 4 && professioneIndividuale == 350) ||
                GetCodNatura3() == 'G' || GetCodNatura3() == 'Z' ||
                ((siglaCategoria == 'VOMIN' || siglaCategoria == 'SOMIN') && GetCodNatura3() == 'D') ||
                //for Beneficio Amianto
                (attivitaEconomica == 14 && professioneIndividuale == 190) ||
                (attivitaEconomica == 15 && professioneIndividuale == 208) ||
                //GetCodNatura2() == 'J' ||
                skip == "TRUE"
                ) {
                    return true;
                }

            return false;
        }

        function EnableAttEconomicaProfIndividualeCumulo(enteCassa){
            var siglaCategoria = document.getElementById("<%= HiddenFieldSiglaCategoria.ClientID %>").value;
            if (siglaCategoria == "IOCUM" || siglaCategoria == "SOCUM") {
                if (enteCassa) {
                    var attivitaEconomica = $("#<%=txtAttivitaEconomica.ClientID %>");
                    var professioneIndividuale = $("#<%= txtProfessioneIndividuale.ClientID %>");

                    attivitaEconomica.removeAttr("disabled");
                    professioneIndividuale.removeAttr("disabled");

                    if (enteCassa == "0805") {
                        var hdnAttivitaEconomica = $("#<%=hiddenFieldAttivitaEconomica.ClientID %>");
                        var hdnProfessioneIndividuale = $("#<%= hiddenFieldProfessioneIndividuale.ClientID %>");

                        if (attivitaEconomica.val() != "14" && professioneIndividuale.val() != "190") {
                            attivitaEconomica.val("71");
                            hdnAttivitaEconomica.val("71");
                            attivitaEconomica.attr("disabled", true);
                            professioneIndividuale.val("085");
                            hdnProfessioneIndividuale.val("085");
                            professioneIndividuale.attr("disabled", true);
                        }
                    }
                }             
            }
            if (siglaCategoria == 'IOCUM' || siglaCategoria == 'SOCUM' ||
                siglaCategoria == 'VOTOT' || siglaCategoria == 'SOTOT' || siglaCategoria == 'IOTOT') {
                SetAttivitaAndProfessioneCum();
            }
        }

        function setHiddenFieldAttivitaEconomicaProfessioneIndividuale(){
            var hdnAttivitaEconomica = $("#<%=hiddenFieldAttivitaEconomica.ClientID %>");
            var attivitaEconomica = $("#<%=txtAttivitaEconomica.ClientID %>");
            var hdnAttivitaEconomicaPrecedente = $("#<%= hdnAttivitaEconomicaPrecedente.ClientID %>");
            
            if (hdnAttivitaEconomica.val() != '')
                hdnAttivitaEconomicaPrecedente.val(hdnAttivitaEconomica.val());
            else
                hdnAttivitaEconomicaPrecedente.val(attivitaEconomica.val());

            hdnAttivitaEconomica.val(attivitaEconomica.val());

            var hdnProfessioneIndividuale = $("#<%= hiddenFieldProfessioneIndividuale.ClientID %>");
            var professioneIndividuale = $("#<%= txtProfessioneIndividuale.ClientID %>");
            var hdnProfessioneIndividualePrecedente = $("#<%= hdnProfessioneIndividualePrecedente.ClientID %>");

            if (hdnProfessioneIndividuale.val() != '')
                hdnProfessioneIndividualePrecedente.val(hdnProfessioneIndividuale.val());
            else
                hdnProfessioneIndividualePrecedente.val(professioneIndividuale.val());

            hdnProfessioneIndividuale.val(professioneIndividuale.val());
        }

        function CheckBeneficiChecked() {

            try {
                if (Get_SKIP_SetChkBenefici() != "TRUE") {               
                    var checkBenefici = GetCheckBenefici();
                    var attivitaEconomica = $("#<%=hiddenFieldAttivitaEconomica.ClientID %>").val();
                    var professioneIndividuale = $("#<%= hiddenFieldProfessioneIndividuale.ClientID %>").val();
                    var skip = $("#<%= hdnSKIP_ManageEnableBeneficiJS.ClientID %>").val();

                    if (//for Prepensionamento
                        (attivitaEconomica == 92 && professioneIndividuale == 257) ||
                        (attivitaEconomica == 3 && professioneIndividuale == 326) ||
                        (attivitaEconomica == 3 && professioneIndividuale == 350) ||
                        (attivitaEconomica == 4 && professioneIndividuale == 350) ||
                        GetCodNatura3() =='G' || 
                        //for Beneficio Amianto
                        (attivitaEconomica == 14 && professioneIndividuale == 190) ||
                        //GetCodNatura2() == 'J' ||
                        skip == "TRUE"
                        ) {
                            checkBenefici.checked = true;
                            return false;
                        }

                    var isNotUncheckBenefici = $("#<%= hdnNOTUncheckBenefici.ClientID %>").val();

                    if((!isNotUncheckBenefici || isNotUncheckBenefici !== "TRUE")) {
                        var isBeneficiSalvati = $("#<%=hdnIsDatiBeneficiSalvati.ClientID %>").val();
                        if (!isBeneficiSalvati || isBeneficiSalvati == "FALSE") {
                            var hdnAttivitaEconomicaPrecedente = $("#<%= hdnAttivitaEconomicaPrecedente.ClientID %>").val();
                            var hdnProfessioneIndividualePrecedente = $("#<%= hdnProfessioneIndividualePrecedente.ClientID %>").val();

                            if (//for Prepensionamento
                            (hdnAttivitaEconomicaPrecedente == 92 && hdnProfessioneIndividualePrecedente == 257) ||
                            (hdnAttivitaEconomicaPrecedente == 3 && hdnProfessioneIndividualePrecedente == 326) ||
                            (hdnAttivitaEconomicaPrecedente == 3 && hdnProfessioneIndividualePrecedente == 350) ||
                            (hdnAttivitaEconomicaPrecedente == 4 && hdnProfessioneIndividualePrecedente == 350) ||
                            //for Beneficio Amianto
                            (hdnAttivitaEconomicaPrecedente == 14 && hdnProfessioneIndividualePrecedente == 190)) {
                                checkBenefici.checked = false;
                            }
                        }
                    }
                }
            }
            catch(e)
            { }
        }

    function Get_ddlCtrlEnteCassaCodiceGestione() {
        return document.getElementById('<%= ddlCtrlEnteCassaCodiceGestione.ClientID %>');       
    }

    function SetAttivitaEconomicaAndProfessione(attivita, professione, enableAttivita, enableProfessione) {
        var attivitaEconomica = $("#<%=txtAttivitaEconomica.ClientID %>");  
        var professioneIndividuale = $("#<%= txtProfessioneIndividuale.ClientID %>");
        var hdnProfessioneIndividuale = $("#<%= hiddenFieldProfessioneIndividuale.ClientID %>");
        var hdnAttivitaEconomica = $("#<%=hiddenFieldAttivitaEconomica.ClientID %>");
        if (attivitaEconomica) {
            attivitaEconomica.val(attivita);
            hdnAttivitaEconomica.val(attivita);
            if (enableAttivita == true) {
                attivitaEconomica.removeAttr("disabled");
            }
            else if (enableAttivita == false) {
                attivitaEconomica.attr("disabled", true);
            }
        }
        if (professioneIndividuale) {
            professioneIndividuale.val(professione);
            hdnProfessioneIndividuale.val(professione);
            if (enableProfessione == true) {
                professioneIndividuale.removeAttr("disabled");
            }
            else if (enableProfessione == false) {
                professioneIndividuale.attr("disabled", true);
            }
        }
    }

    function Get_siglaCategoria() {
        return document.getElementById("<%= HiddenFieldSiglaCategoria.ClientID %>").value;
}

function sommaSettimane() {
        var settimaneUtili = document.getElementById("<%=txtNumeroSettimaneOBG.ClientID %>");
        var settimaneUtiliOI = document.getElementById("<%=txtNumeroSettimaneOI.ClientID %>");
        var totaleSettimane = document.getElementById("<%=txtNumeroSettimaneTot.ClientID %>");

        var valore1 = parseInt(settimaneUtili.value) || 0;
        var valore2 = parseInt(settimaneUtiliOI.value) || 0;

        totaleSettimane.value = valore1 + valore2;

        extractNumber(this,0,false);
    }

</script>
<asp:Panel runat="server" ID="pnlDatiAssicurativi">
    <table class="tabellaFormattazione grid grid-size-25">
        <tr runat="server" id="trMessaggioStaticoENPALS" visible="false">
            <td colspan="4">
                <asp:Label runat="server" ID="lblMessaggioStaticoENPALS" SkinID="lblBoldBlack" Text="Si evidenzia che tutti i contributi sono espressi in giorni, a meno del campo &#34;Anzianità Contributiva&#34; il cui dato è espresso in settimane." />
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlDecorrenzaPensioneENPALS" Visible="false">
            <tr>
                <td class="Row1" style="width: 23%">
                    <label>
                        Decorrenza Pensione:</label>
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtDecorrenzaPensione" CssClass="tb8 txtUppercase"
                        Enabled="false" Width="100px"></asp:TextBox>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlInizioFineAssicurazione">
            <tr>
                <td class="Row1" style="width: 23%">
                    <label>
                        Inizio Assicurazione:</label>
                </td>
                <td class="field" style="width: 27%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtInizioAssicurazione"
                        Width="100px" Text="gg/mm/aaaa" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="1"
                        MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtInizioAssicurazione"
                        ErrorMessage="Data Inizio Assicurazione in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$|^GG/MM/AAAA$|^gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:RequiredFieldValidator runat="server" ID="requiredInizioAssicurazione" Display="Dynamic"
                        ErrorMessage="Inizio Assicurazione: Inserire la data di Inizio Assicurazione"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi" ControlToValidate="txtInizioAssicurazione"></asp:RequiredFieldValidator>
                    <asp:CustomValidator runat="server" ControlToValidate="txtInizioAssicurazione" Display="Dynamic"
                        ErrorMessage="Inizio Assicurazione: data inserita posteriore a quella odierna"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi" ID="customInizioAssicurazione"
                        ClientValidationFunction="checkDataPostOdiernaGGMMAAAA" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtInizioAssicurazione" Display="Dynamic"
                        ErrorMessage="Inizio Assicurazione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi"
                        ID="customCheckDataInizioAssicurazione" ClientValidationFunction="checkCorrettezzaData" />
                </td>
                <td class="Row1" style="width: 23%">
                    <label>
                        Fine Assicurazione:</label>
                </td>
                <td class="field" style="width: 27%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtFineAssicurazione" Width="100px"
                        Text="gg/mm/aaaa" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="2" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validatetxtFineAssicurazione" ControlToValidate="txtFineAssicurazione"
                        ErrorMessage="Data Fine Assicurazione in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$|^GG/MM/AAAA$|^gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtFineAssicurazione" Display="Dynamic"
                        ErrorMessage="Fine Assicurazione: Data inserita posteriore a quella odierna"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi" ID="customFineAssicurazione"
                        ClientValidationFunction="checkDataPostOdiernaMMAAAA" />
                    <asp:RequiredFieldValidator runat="server" ID="RFFineAssicurazione" Display="Dynamic"
                        ErrorMessage="Fine Assicurazione: Inserire la data di Fine Assicurazione" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiAssicurativi" ControlToValidate="txtFineAssicurazione"></asp:RequiredFieldValidator>
                    <asp:CustomValidator runat="server" ControlToValidate="txtFineAssicurazione" Display="Dynamic"
                        ErrorMessage="Fine Assicurazione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi"
                        ID="customCheckDataFineAssicurazione" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlInizioFineUltimoLavoro" Visible="false">
            <tr runat="server" id="trInizioFineUltimoLavoro">
                <td class="Row1">
                    <asp:Label runat="server" ID="lblInizioUltLAv" Text="Inizio Ultimo Lavoro:"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtInizioUltLav" Width="100px"
                        Text="gg/mm/aaaa" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="3" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validateInizioUltLav" ControlToValidate="txtInizioUltLav"
                        ErrorMessage="Inizio Ultimo Lavoro in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtInizioUltLav" Display="Dynamic"
                        ErrorMessage="Inizio Ultimo Lavoro: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi"
                        ID="customCheckDataInizioUltimoLavoro" ClientValidationFunction="checkCorrettezzaData" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtInizioUltLav" Display="Dynamic"
                        ErrorMessage="Inizio Ultimo Lavoro è un dato obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi"
                        ControlToValidate="txtInizioUltLav" Enabled="false" />
                </td>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblFineUltLav" Text="Fine Ultimo Lavoro:"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtFineUltLav" Width="100px"
                        Text="gg/mm/aaaa" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="4" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validateFineUltLav" ControlToValidate="txtFineUltLav"
                        ErrorMessage="Fine Ultimo Lavoro in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtFineUltLav" Display="Dynamic"
                        ErrorMessage="Fine Ultimo Lavoro: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi"
                        ID="customCheckDataFineUltimoLavoro" ClientValidationFunction="checkCorrettezzaData" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtFineUltLav" Display="Dynamic"
                        ErrorMessage="Fine Ultimo Lavoro è un dato obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi"
                        ControlToValidate="txtFineUltLav" Enabled="false" />
                </td>
            </tr>
            <asp:Panel runat="server" ID="pnlImportoUltimaRetribuzione">
                <tr>
                    <td class="Row1">
                        <label>
                            Importo Ultima Retribuzione Mensile:</label>
                    </td>
                    <td class="field">
                        <asp:TextBox ID="txtImportaUltimaRetribuzione" runat="server" MaxLength="12" Width="120px"
                            CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtImportaUltimaRetribuzione" runat="server"
                            ControlToValidate="txtImportaUltimaRetribuzione" Display="Dynamic" Enabled="true"
                            ErrorMessage="Importa Ultima Retribuzione Mensile: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi" ValidationExpression="\d{1,7}(,\d{1,4})?$" />
                        <asp:RequiredFieldValidator runat="server" ID="RFVtxtImportaUltimaRetribuzione" Display="Dynamic"
                            ErrorMessage="Importo Ultima Retribuzione Mensile è un dato obbligatorio" Text="*" CssClass="field-is-required"
                            ValidationGroup="UCTabDatiAssicurativi" ControlToValidate="txtImportaUltimaRetribuzione" />
                    </td>
                </tr>
            </asp:Panel>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlAttEconomProfInd">
            <%--<div runat="server" id="divAttEconomProfInd">--%>
            <tr>
                <td class="Row1">
                    <label>
                        Attività Economica:</label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtAttivitaEconomica" Width="120px"
                        CssClass="txtUppercase tb8 onClassDomanda autotab" TabIndex="5" MaxLength="2"
                        onblur="extractNumber(this,0,false); setHiddenFieldAttivitaEconomicaProfessioneIndividuale(); CheckBeneficiChecked();"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtAttivitaEconomica"
                        ErrorMessage="Attivita Economica non valido" ValidationExpression="^[0-9]{2}$"
                        runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        CssClass="offClass field-is-required  onClassDomanda" Enabled="false" />
                    <asp:RequiredFieldValidator runat="server" ID="RFAttivitaEconimica" ControlToValidate="txtAttivitaEconomica"
                        Display="Dynamic" Enabled="true" ErrorMessage="Attività Economica Obbligatoria"
                        ValidationGroup="UCTabDatiAssicurativi" Text="*" CssClass="field-is-required" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtAttivitaEconomica" Display="Dynamic"
                        ErrorMessage="Attività Economica: il campo deve essere lungo 2" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi"
                        ID="checkLenghtAttEconomica" ClientValidationFunction="CheckLenghtAttEconomica2" />
                </td>
                <td class="Row1">
                    <label>
                        Professione Individuale:</label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtProfessioneIndividuale"
                        Width="120px" CssClass="txtUppercase tb8 onClassDomanda autotab" TabIndex="6"
                        MaxLength="3" onblur="extractNumber(this,0,false); setHiddenFieldAttivitaEconomicaProfessioneIndividuale(); CheckBeneficiChecked();"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ControlToValidate="txtProfessioneIndividuale"
                        ErrorMessage="Professione Individuale non valido" ValidationExpression="^[0-9]{3}$"
                        runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        CssClass="offClass field-is-required  onClassDomanda" Enabled="false" />
                    <asp:RequiredFieldValidator runat="server" ID="RFProfessioneIndividuale" ControlToValidate="txtProfessioneIndividuale"
                        Display="Dynamic" Enabled="true" ErrorMessage="Professione Individuale Obbligatoria"
                        ValidationGroup="UCTabDatiAssicurativi" Text="*" CssClass="field-is-required" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtProfessioneIndividuale"
                        Display="Dynamic" ErrorMessage="Professione Individuale: il campo deve essere lungo 3"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi" ID="checkLenghtProfIndividuale"
                        ClientValidationFunction="CheckLenghtProfIndividuale" />
                </td>
            </tr>
        </asp:Panel>
        <%--</div>--%>
        <asp:Panel runat="server" ID="pnlNSettimane_NContributiVolontariDiritto">
            <tr>
                <td class="Row1">
                    <label runat="server" id="lblNumeroSettimane">
                        Numero Settimane:</label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtNumeroSettimaneOBG" Width="120px"
                        CssClass="txtUppercase tb8" TabIndex="7" MaxLength="4" onblur="sommaSettimane();"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ID="RFNumeroSettimane" ControlToValidate="txtNumeroSettimaneOBG"
                        Display="Dynamic" Enabled="false" ErrorMessage="Numero Settimane Obbligatorio"
                        ValidationGroup="UCTabDatiAssicurativi" Text="*" CssClass="field-is-required" />
                    <asp:RegularExpressionValidator ID="validateTxtNumeroSettimaneOBG" ControlToValidate="txtNumeroSettimaneOBG"
                        ErrorMessage="Numero di settimane non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                </td>
                <td class="Row1">
                    <label>
                        Numero Contributi Volontari Diritto:</label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtNumContrVolontari" Width="120px"
                        MaxLength="4" CssClass="txtUppercase tb8" TabIndex="8" onblur="extractNumber(this,0,false);"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validateNumContrVolontari" ControlToValidate="txtNumContrVolontari"
                        ErrorMessage="Numero di contributi volontari diritto non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlNSettimane_OrganizzazioniInternazionali">
            <tr>
                <td class="Row1">
                    <label runat="server" id="lblnSettimaneOI">
                        Numero Settimane OI:</label> <label runat="server" id="Label1" visible="false" />
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtNumeroSettimaneOI" Width="120px"
                        CssClass="txtUppercase tb8" TabIndex="7" MaxLength="4" onblur="sommaSettimane();"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <%--<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtNumeroSettimaneOI"
                        Display="Dynamic" Enabled="false" ErrorMessage="Numero Settimane Organizzazioni Internazionali"
                        ValidationGroup="UCTabDatiAssicurativi" Text="*" CssClass="field-is-required" />--%>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtNumeroSettimaneOI"
                        ErrorMessage="Numero di settimane OI non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        Numero Settimane Utili:</label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtNumeroSettimaneTot" Width="120px" ReadOnly="true"
                        MaxLength="4" CssClass="txtUppercase tb8" TabIndex="8"></asp:TextBox>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlTotSettEstereUtiliPerDirittoEContrEsteraTotale"
            Visible="false">
            <tr>
                <td class="Row1">
                    <label>
                        Totale settimane estere utili per diritto:</label>
                </td>
                <td class="field">
                      <asp:TextBox Style="text-align: left" runat="server" ID="txtCTotSettEstereUtiliPerDiritto"
                        MaxLength="4" Width="120px" CssClass="txtUppercase tb8" TabIndex="9" onblur="extractNumber(this,0,false);"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                </td>
                <td class="Row1">
                    <label>
                        Contribuzione estera totale:</label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtContribuzioneEsteraTotale"
                        MaxLength="3" Width="120px" CssClass="txtUppercase tb8" TabIndex="9" onblur="extractNumber(this,0,false);"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlNumContrVolontariAnz">
            <tr>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblNumContrVolontariAnz" Text="Numero Contributi Volontari per Anzianità:"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtNumContrVolontariAnz"
                        MaxLength="4" Width="120px" CssClass="txtUppercase tb8" TabIndex="9" onblur="extractNumber(this,0,false);"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validateNumContrVolontariAnz" ControlToValidate="txtNumContrVolontariAnz"
                        ErrorMessage="Numero di contributi volontari per anzianità non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlAnzVecch" runat="server" Visible="false">
            <tr>
                <td class="Row1">
                    <label>
                        Requisiti Vecchiaia al 12/94:</label>
                </td>
                <td class="chkField">
                    <asp:DropDownList runat="server" ID="ddlReqVecch1294" Width="50px" CssClass="tb8 txtUppercase xxs"
                        TabIndex="10">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="Row1">
                    <label>
                        Requisiti Anzianità al 12/94:</label>
                </td>
                <td class="chkField">
                    <asp:DropDownList runat="server" ID="ddlReqAnz1294" Width="50px" CssClass="tb8 txtUppercase xxs"
                        TabIndex="11">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        Requisiti Anzianità al 9/96:</label>
                </td>
                <td class="chkField">
                    <asp:DropDownList runat="server" ID="ddlReqAnz996" Width="50px" CssClass="tb8 txtUppercase xxs"
                        TabIndex="12">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlContenitoreReqArt2Dl503" runat="server" Visible="false">
            <tr>
                <asp:Panel ID="pnlReqArt2Dl503" runat="server">
                    <td class="Row1">
                        <label>
                            Requisiti Art. 2 c.3 DL 503/92:</label>
                    </td>
                    <td class="chkField">
                        <asp:DropDownList runat="server" ID="ddlReqArt2Dl503" Width="150px" CssClass="tb8 txtUppercase xxs"
                            TabIndex="11" Enabled="false">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="NO" Value="0"></asp:ListItem>
                            <asp:ListItem Text="SI Accert. Sede" Value="1"></asp:ListItem>
                            <asp:ListItem Text="SI Accert. Auto." Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </asp:Panel>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlBonus" runat="server" Visible="false">
            <tr>
                <td class="Row1">
                    <label>
                        Inizio Bonus:</label>
                </td>
                <td class="chkField">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtInizioBonus" Width="100px"
                        MaxLength="10" Text="gg/mm/aaaa" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA"
                        TabIndex="13" Enabled="false"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="revValidateInizioBonus" ControlToValidate="txtInizioBonus"
                        ErrorMessage="Data Inizio Bonus in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtInizioBonus" Display="Dynamic"
                        ErrorMessage="Data Inizio Bonus: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi"
                        ID="customCheckDataInizioBonus" ClientValidationFunction="checkCorrettezzaData" />
                </td>
                <td class="Row1">
                    <label>
                        Fine Bonus:</label>
                </td>
                <td class="chkField">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtFineBonus" Width="100px"
                        MaxLength="10" Text="gg/mm/aaaa" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA"
                        TabIndex="14" Enabled="false"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="revValidateFineBonus" ControlToValidate="txtFineBonus"
                        ErrorMessage="Data Fine Bonus in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtFineBonus" Display="Dynamic"
                        ErrorMessage="Data Fine Bonus: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi"
                        ID="CustomValidator1" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlDatiAssicurativiENPALS" Visible="false">
            <tr>
                <td class="Row1">
                    <label>
                        Anni e mesi maturati per il diritto:</label>
                </td>
                <td class="field fileds-date-input fileds-date-input--col-2">
                    <asp:TextBox runat="server" ID="txtAADiritto" CssClass="tb8 txtUppercase" Enabled="false"
                        Width="30px" MaxLength="2"></asp:TextBox>
                    <label>
                        AA</label>
                    <asp:RegularExpressionValidator ID="REV_txtAADiritto" ControlToValidate="txtAADiritto"
                        ErrorMessage="Anni e mesi maturati per il diritto AA non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:TextBox runat="server" ID="txtMMDiritto" CssClass="tb8 txtUppercase" Enabled="false"
                        Width="30px" MaxLength="2"></asp:TextBox>
                    <label>
                        MM</label>
                    <asp:RegularExpressionValidator ID="REV_txtMMDiritto" ControlToValidate="txtMMDiritto"
                        ErrorMessage="Anni e mesi maturati per il diritto MM non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtMMDiritto" Display="Dynamic"
                        ErrorMessage="Anni e mesi maturati per il diritto MM: mese non valido" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiAssicurativi" ID="CV_txtMMDiritto" ClientValidationFunction="checkCorrettezzaMese" />
                </td>
                <td class="Row1">
                    <label>
                        Età maturata per il diritto:</label>
                </td>
                <td class="field fileds-date-input fileds-date-input--col-2">
                    <asp:TextBox runat="server" ID="txtEtaDirittoAA" CssClass="tb8 txtUppercase" Enabled="false"
                        Width="30px" MaxLength="2"></asp:TextBox>
                    <label>
                        AA</label>
                    <asp:RegularExpressionValidator ID="REV_txtEtaDirittoAA" ControlToValidate="txtEtaDirittoAA"
                        ErrorMessage="Età maturata per il diritto AA non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:TextBox runat="server" ID="txtEtaDirittoMM" CssClass="tb8 txtUppercase" Enabled="false"
                        Width="30px" MaxLength="2"></asp:TextBox>
                    <label>
                        MM</label>
                    <asp:RegularExpressionValidator ID="REV_txtEtaDirittoMM" ControlToValidate="txtEtaDirittoMM"
                        ErrorMessage="Età maturata per il diritto MM non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtEtaDirittoMM" Display="Dynamic"
                        ErrorMessage="Età maturata per il diritto MM: mese non valido" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi"
                        ID="CV_txtEtaDirittoMM" ClientValidationFunction="checkCorrettezzaMese" />
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        Numero Contributi per il diritto:</label>
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtNTotDiritto" CssClass="tb8 txtUppercase" Enabled="false"
                        Width="120px" MaxLength="5"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REV_txtNTotDiritto" ControlToValidate="txtNTotDiritto"
                        ErrorMessage="Numero Contributi per il diritto non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                </td>
                <td class="Row1">
                    <label>
                        Anzianità Contributiva:</label>
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtAnzianitaContributiva" CssClass="tb8 txtUppercase"
                        Enabled="false" Width="120px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        Gruppo prevalente:</label>
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtGruppoPrevalente" CssClass="tb8 txtUppercase"
                        Enabled="false" Width="120px" MaxLength="1"></asp:TextBox>
                </td>
                <td class="Row1">
                    <label>
                        Raggruppamento con il quale ha conseguito il diritto:</label>
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtGruppoDiritto" CssClass="tb8 txtUppercase" Enabled="false"
                        Width="120px" MaxLength="1"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        Raggruppamento prevalente:</label>
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtRaggruppamentoPrevalente" CssClass="tb8 txtUppercase"
                        Enabled="false" Width="120px" MaxLength="1"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        Qualifica:</label>
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtQualifica" CssClass="tb8 txtUppercase" Enabled="false"
                        Width="120px" MaxLength="3"></asp:TextBox>
                </td>
                <td class="Row1">
                    <label>
                        Numero Totale Contributi nella Qualifica:</label>
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtNTotQualifica" CssClass="tb8 txtUppercase" Enabled="false"
                        Width="120px" MaxLength="9"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REV_txtNTotQualifica" ControlToValidate="txtNTotQualifica"
                        ErrorMessage="Numero Totale Contributi nella Qualifica non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        Numero Totale Contributi:</label>
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtNTotContributi" CssClass="tb8 txtUppercase" Enabled="false"
                        Width="120px" MaxLength="9"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REV_txtNTotContributi" ControlToValidate="txtNTotContributi"
                        ErrorMessage="Numero Totale Contributi non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                </td>
                <td class="Row1">
                    <label>
                        Numero Totale Contributi ENPALS:</label>
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtNTotContributiEnpals" CssClass="tb8 txtUppercase"
                        Enabled="false" Width="120px" MaxLength="9"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REV_txtNTotContributiEnpals" ControlToValidate="txtNTotContributiEnpals"
                        ErrorMessage="Numero Totale Contributi ENPALS non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        Età maturata per la misura:</label>
                </td>
                <td class="field fileds-date-input fileds-date-input--col-2">
                    <asp:TextBox runat="server" ID="txtEtaMisuraAA" CssClass="tb8 txtUppercase" Enabled="false"
                        Width="30px" MaxLength="2"></asp:TextBox>
                    <label>
                        AA</label>
                    <asp:RegularExpressionValidator ID="REV_txtEtaMisuraAA" ControlToValidate="txtEtaMisuraAA"
                        ErrorMessage="Età maturata per la misura AA non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:TextBox runat="server" ID="txtEtaMisuraMM" CssClass="tb8 txtUppercase" Enabled="false"
                        Width="30px" MaxLength="2"></asp:TextBox>
                    <label>
                        MM</label>
                    <asp:RegularExpressionValidator ID="REV_txtEtaMisuraMM" ControlToValidate="txtEtaMisuraMM"
                        ErrorMessage="Età maturata per la misura MM non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtEtaMisuraMM" Display="Dynamic"
                        ErrorMessage="Età maturata per la misura MM: mese non valido" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi"
                        ID="CV_txtEtaMisuraMM" ClientValidationFunction="checkCorrettezzaMese" />
                </td>
                <td class="Row1">
                    <label>
                        Numero Contributi per la misura:</label>
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtNContributiMisura" CssClass="tb8 txtUppercase"
                        Enabled="false" Width="120px" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REV_txtNContributiMisura" ControlToValidate="txtNContributiMisura"
                        ErrorMessage="Numero Contributi per la misura non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        Numero Totale Contributi nel triennio per la Qualifica:</label>
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtNContributiTriennio" CssClass="tb8 txtUppercase"
                        Enabled="false" Width="120px" MaxLength="9"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REV_txtNContributiTriennio" ControlToValidate="txtNContributiTriennio"
                        ErrorMessage="Numero Totale Contributi nel triennio per la Qualifica non valido"
                        ValidationExpression="^[0-9]+$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                </td>
                <td class="Row1">
                    <label>
                        Numero Totale Contributi nel quinquennio per la Qualifica:</label>
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtNContributiQuinquennio" CssClass="tb8 txtUppercase"
                        Enabled="false" Width="120px"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REV_txtNContributiQuinquennio" ControlToValidate="txtNContributiQuinquennio"
                        ErrorMessage="Numero Totale Contributi nel quinquennio per la Qualifica non valido"
                        ValidationExpression="^[0-9]+$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        Numero Contributi N.L.155:</label>
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtNContributiNL155" CssClass="tb8 txtUppercase"
                        Enabled="false" Width="120px" MaxLength="9"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REV_txtNContributiNL155" ControlToValidate="txtNContributiNL155"
                        ErrorMessage="Numero Contributi N.L.155 non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                </td>
                <td class="Row1">
                    <label>
                        Numero Contributi N.L.222:</label>
                </td>
                <td class="field">
                    <asp:TextBox runat="server" ID="txtNContributiNL222" CssClass="tb8 txtUppercase"
                        Enabled="false" Width="120px" MaxLength="9"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REV_txtNContributiNL222" ControlToValidate="txtNContributiNL222"
                        ErrorMessage=" Numero Contributi N.L.222 non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlCodiceAttivitaLavorativa" Visible="false">
            <tr>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblCodiceAttivitaLavorativa" Text="Codice Attività Lavorativa:"></asp:Label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:TextBox Style="text-align: left" runat="server" Width="90%" ID="txtCodiceAttivitaLavorativa"
                        class="txtUppercase tb8 onClassDomanda autotab" Enabled="false"></asp:TextBox>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlCodiceConvenzione" Visible="false">
            <tr>
                <td class="Row1">
                    <label>
                        Codice Convenzione:</label>
                </td>
                <td class="field">
                    <asp:TextBox ID="txtCodiceConvenzione" runat="server" CssClass="tb8 txtUppercase"
                        Width="30"></asp:TextBox>
                </td>
            </tr>
        </asp:Panel>
    </table>
    <asp:DropDownList runat="server" ID="ddlCtrlEnteCassaCodiceGestione" Width="10%"
        CssClass="txtUppercase tb8 xxs" Style="visibility: hidden" Visible="false">
    </asp:DropDownList>
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaDatiAssicurativi" runat="server" SkinID="btnAzione1" Enabled="true"
                        Text="Salva Dati Assicurativi" CausesValidation="false" Width="170px" OnClick="SalvaDatiAssicurativi_Click"
                        OnClientClick="if(Page_ClientValidate('UCTabDatiAssicurativi')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaDatiAssicurativi" runat="server" SkinID="btnAzione1" Enabled="true"
                        Text="Elimina Dati Assicurativi" Width="170px" Style="padding-left: 0px; padding-right: 0px;" CssClass="ghost-delete"
                        CausesValidation="false" OnClick="EliminaDatiAssicurativi_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Assicurativi?')) return false; else BlockUI();" />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="modalitaEdit" Value="false" />
</asp:Panel>
<asp:HiddenField runat="server" ID="hiddenFieldAttivitaEconomica" />
<asp:HiddenField runat="server" ID="hiddenFieldProfessioneIndividuale" />
<asp:HiddenField runat="server" ID="hdnAttivitaEconomicaPrecedente" />
<asp:HiddenField runat="server" ID="hdnProfessioneIndividualePrecedente" />
<asp:HiddenField runat="server" ID="hdnIsDatiBeneficiSalvati" />
<asp:HiddenField runat="server" ID="hdnNOTUncheckBenefici" />
<asp:HiddenField runat="server" ID="hdnSKIP_ManageEnableBeneficiJS" Value="FALSE" />
<asp:HiddenField runat="server" ID="HiddenFieldSiglaCategoria" />
