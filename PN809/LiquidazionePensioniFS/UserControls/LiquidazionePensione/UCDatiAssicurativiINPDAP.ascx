<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiAssicurativiINPDAP.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCDatiAssicurativiINPDAP" %>
<script type="text/javascript">
    $(document).ready(function () {
        SetCalendariInizioFineAssicurazione();

        <%--var availableTagsCausaCess = document.getElementById("ctl00_ContentPlaceHolder1_ucDatiAssicurativiINPDAP_HiddenFieldCausaCessazione").value.split(';');
        $("#<%=txtCausaCessazione.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTagsCausaCess
        });--%>

        var availableTags = document.getElementById("ctl00_ContentPlaceHolder1_ucDatiAssicurativiINPDAP_hiddenMicroqualifica").value.split(';');
        $("#<%=txtMicroqualificaINPDAP.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTags,
            open: function () {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            }
        });

        ddlDirittoIndennIntegrSpecOnChange();
        $(document.getElementById("<%= txtMicroqualificaINPDAP.ClientID %>")).change(function () {
            ddlDirittoIndennIntegrSpecOnChange();
        });

        initCompartoSettoreRuolo();
        $(document.getElementById("<%= ddlComparto.ClientID %>")).change(function () {
            ddlCompartoOnChange();
        });

        $(document.getElementById("<%= ddlSettore.ClientID %>")).change(function () {
            ddlSettoreOnChange();
        });      
    });


    function validateMese(source, args) {
        var mesi = args.Value;
        if (mesi < 0 || mesi > 11)
            args.IsValid = false;
        else
            args.IsValid = true;
        return false;
    }

    function validateGiorno(source, args) {
        var giorni = args.Value;
        if (giorni < 0 || giorni > 30)
            args.IsValid = false;
        else
            args.IsValid = true;
        return false;
    }

    function ddlDirittoIndennIntegrSpecOnChange() {
        var ddl = document.getElementById("<%= ddlDirittoIndennIntegrSpec.ClientID %>");
        if (ddl) {

            var iisRapportata = $("#<%= ddlIISAbbattimentoAnni.ClientID %>");
            var riduzioneL537 = $("#<%= ddlRiduzioneL537.ClientID %>");

            if (ddl.value == "NO") {


                if (iisRapportata) {
                    iisRapportata.val("NO");
                    iisRapportata.attr('disabled', true);
                }

                if (riduzioneL537) {
                    riduzioneL537.val("NO");
                    riduzioneL537.attr('disabled', true);
                }
            }
            else {


                if (iisRapportata) {
                    iisRapportata.attr('disabled', false);
                }

                if (riduzioneL537) {
                    riduzioneL537.attr('disabled', false);
                }
            }
        }

    }
    function CheckBeneficiDisabled() {


        var skip = $("#<%= hdnSKIP_ManageEnableBeneficiJS.ClientID %>").val();
        if (GetCodNatura3() == 'G' ||
            //GetCodNatura2() == 'J' ||
            skip == "TRUE") {
            return true;
        }

        return false;
    }

    function SetCalendariInizioFineAssicurazione() {
        if ($(document.getElementById("<%=pnlTxtPrimoVersamento.ClientID%>")).is(':disabled') == false) {
            if ($(document.getElementById("<%=txtPrimoVersamento.ClientID%>")).is(':disabled') == false) {
                $(document.getElementById("<%=txtPrimoVersamento.ClientID%>")).datepicker({
                    changeMonth: true,
                    changeYear: true,
                    changeDay: true,
                    showButtonPanel: true,
                    dateFormat: 'dd/mm/yy',
                    showOn: 'button',
                    buttonImageOnly: true,
                    buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                    minDate: '-100y',
                    maxDate: '0',
                    yearRange: '-100:' + '+0:'
                });
            }
            
            //$(document.getElementById("<%=txtPrimoVersamento.ClientID%>")).unmask();
            //$(document.getElementById("<%=txtPrimoVersamento.ClientID%>")).mask("99/99/9999");
        }
        if ($(document.getElementById("<%=pnlTxtUltimoVersamento.ClientID%>")).is(':disabled') == false) {
            if ($(document.getElementById("<%=txtUltimoVersamento.ClientID%>")).is(':disabled') == false) {
                $(document.getElementById("<%=txtUltimoVersamento.ClientID%>")).datepicker({
                    changeMonth: true,
                    changeYear: true,
                    changeDay: true,
                    showButtonPanel: true,
                    dateFormat: 'dd/mm/yy',
                    showOn: 'button',
                    buttonImageOnly: true,
                    buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                    minDate: '-100y',
                    maxDate: '0',
                    yearRange: '-100:' + '+0:'
                });
            }
            
            //$(document.getElementById("<%=txtUltimoVersamento.ClientID%>")).unmask();
            //$(document.getElementById("<%=txtUltimoVersamento.ClientID%>")).mask("99/99/9999");
        }
    }

    function initCompartoSettoreRuolo() {
        var ddlComparto = document.getElementById("<%= ddlComparto.ClientID %>");
        var ddlSettore = document.getElementById("<%= ddlSettore.ClientID %>");
        if (ddlComparto && ddlSettore) {
            //Nascondo tutti i valori di settore
            $("#<%=ddlSettore.ClientID%> > option[value!=]").wrap("<span/>").hide();
            //Se Comparto ha un valore, mostro i corrispondenti Settori
            if ($('#<%=ddlComparto.ClientID%> > option:selected[value!=]').val())
                $("#<%=ddlSettore.ClientID%> > span > option[value*='" + $('#<%=ddlComparto.ClientID%>').val() + ";']").unwrap().show();    

            var ddlRuolo = document.getElementById("<%= ddlRuolo.ClientID %>");
            if (ddlRuolo) {
                //Nascondo tutti i valori di Ruolo
                $("#<%=ddlRuolo.ClientID%> > option[value!=]").wrap("<span/>").hide();
                //Se settore ha un valore, mostro i corrispondenti Ruoli
                if ($('#<%=ddlSettore.ClientID%> > option:selected[value!=]').val()) 
                    $("#<%=ddlRuolo.ClientID%> > span > option[value*='" + $('#<%=ddlSettore.ClientID%>').val() + "']").unwrap().show();
            }
        }
    }


    function ddlCompartoOnChange() {
        var ddl = document.getElementById("<%= ddlComparto.ClientID %>");
        if (ddl) {
            var ddlSettore = document.getElementById("<%= ddlSettore.ClientID %>");
            //Se Il valore di Comparto è cambiato, devo sbiancare Settore e Ruolo
            $("#<%=ddlSettore.ClientID%> > option[value!=]").wrap("<span/>").hide();
            $("#<%=ddlRuolo.ClientID%> > option[value!=]").wrap("<span/>").hide();
            //Se Comparto ha un valore, mostro solo i Settori corrispondenti
            if (ddlSettore && $('#<%=ddlComparto.ClientID%> > option:selected[value!=]').val()) {
                $("#<%=ddlSettore.ClientID%> > span > option[value*='$" + $('#<%=ddlComparto.ClientID%>').val() + ";']").unwrap().show();                   
            }
        }
    }

    function ddlSettoreOnChange() {
        var ddl = document.getElementById("<%= ddlComparto.ClientID %>");
        var ddlSettore = document.getElementById("<%= ddlSettore.ClientID %>");
        if (ddl && ddlSettore) {            
            //Se Il valore di Settore è cambiato, devo sbiancare Ruolo
            $("#<%=ddlRuolo.ClientID%> > option[value!=]").wrap("<span/>").hide();
            var ddlRuolo = document.getElementById("<%= ddlRuolo.ClientID %>");
            //Se Settore ha un valore, mostro solo i Ruoli corrispondenti
            if (ddlRuolo && $('#<%=ddlSettore.ClientID%> > option:selected[value!=]').val()) {
                $("#<%=ddlRuolo.ClientID%> > span > option[value*='" + $('#<%=ddlSettore.ClientID%>').val() + "']").unwrap().show();
            }
        }
    }
</script>

<asp:Panel runat="server" ID="pnlTotale">
<!-- Pannello Common Header -->
<asp:Panel runat="server" ID="pnlCommonHeader">
    <div id="divBorder" style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; width: 710px; margin-left: 4px; margin-bottom: 8px; margin-top: 4px;">
        <table class="tabellaFormattazione grid grid-size-20">
            <tr>
                <td class="Row1" style="width: 25%; display:none">
                    <label>
                        Tipo Pensione:</label>
                </td>
                <td class="Row1" style="width: 25%; display:none">
                    <asp:Label runat="server" ID="lblTipoPensione"></asp:Label>
                    <asp:HiddenField ID="hdnTipoPensione" runat="server" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza Pensione:</label>
                </td>
                <td class="Row1" style="width: 75%">
                    <asp:Label runat="server" ID="lblDecorrenzaPensioneDatiAssicurativi" />
                </td>
            </tr>
            <asp:Panel runat="server" ID="pnlDecorrenzaCalcoloNuovaGestioneDatiFondoFSPT" Visible="false">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Decorrenza Calcolo:</label>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <asp:Label runat="server" ID="lblDecorrenzaCalcolo"></asp:Label>
                    </td>
                </tr>
            </asp:Panel>
        </table>
    </div>
    <table class="tabellaFormattazione grid grid-size-20">
        <tr runat="server" ID="trDateAssicurazione">
            <td class="Row1" style="width: 25%">
                <label>
                    Primo Versamento:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:Panel runat="server" ID="pnlTxtPrimoVersamento">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtPrimoVersamento" Width="50%"
                        Text="" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="1" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtPrimoVersamento"
                        ErrorMessage="Data primo versamento in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
                    <asp:RequiredFieldValidator runat="server" ID="requiredPrimoVersamento" Display="Dynamic"
                        ErrorMessage="Primo versamento: Inserire la data del primo versamento" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiAssicurativiFS" ControlToValidate="txtPrimoVersamento"></asp:RequiredFieldValidator>
                    <asp:CustomValidator runat="server" ControlToValidate="txtPrimoVersamento" Display="Dynamic"
                        ErrorMessage="Primo Versamento: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                        ID="customCheckDataPrimoVersamento" ClientValidationFunction="checkCorrettezzaData" />
                </asp:Panel>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Ultimo Versamento:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:Panel runat="server" ID="pnlTxtUltimoVersamento">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtUltimoVersamento" Width="50%"
                        Text="" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="2" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validatetxtUltimoVersamento" ControlToValidate="txtUltimoVersamento"
                        ErrorMessage="Data ultimo versamento in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
                    <asp:RequiredFieldValidator runat="server" ID="RFUltimoVersamento" Display="Dynamic"
                        ErrorMessage="Ultimo versamento: Inserire la data dell'ultimo versamento" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiAssicurativiFS" ControlToValidate="txtUltimoVersamento"></asp:RequiredFieldValidator>
                    <asp:CustomValidator runat="server" ControlToValidate="txtUltimoVersamento" Display="Dynamic"
                        ErrorMessage="Ultimo Versamento: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                        ID="customCheckDataUltimoVersamento" ClientValidationFunction="checkCorrettezzaData" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <asp:Panel runat="server" ID="pnlCodiceSpecifico">
                <td class="Row1" style="width: 25%">
                    <label>
                        Codice Specifico:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlCodiceSpecifico" CssClass="txtUppercase tb8"
                        TabIndex="3" Width="90%">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="ddlCodiceSpecifico_RF" Display="Dynamic"
                        Text="*" CssClass="field-is-required" ErrorMessage="Codice Specifico: Si prega di inserire il codice specifico"
                        ControlToValidate="ddlCodiceSpecifico" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
                </td>
            </asp:Panel>
        </tr>
        <asp:Panel runat="server" ID="pnlAmministrazione" Visible="false">
	    <tr>
		    <td class="Row1" style="width: 25%">
			    <label>
				    Codice Fiscale amministrazione appartenenza:</label>
		    </td>
		    <td class="Row1" style="width: 25%">
				    <asp:TextBox Style="text-align: left" runat="server" ID="txtCfAmministrazione" Width="70%"
					    Text="" CssClass="txtUppercase tb8" TabIndex="1" MaxLength="11"></asp:TextBox>
                 <asp:RegularExpressionValidator ID="REVtxtCfAmministrazione" ControlToValidate="txtCfAmministrazione"
                        ErrorMessage="CF non valido" ValidationExpression="^[0-9]{11}$"
                        runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                        CssClass="offClass  field-is-required onClassDomanda" Enabled="true" />
		    </td>
		    <td class="Row1" style="width: 25%">
			    <label>
				    Progressivo amministrazione appartenenza:</label>
		    </td>
		    <td class="Row1" style="width: 25%">
				    <asp:TextBox Style="text-align: left" runat="server" ID="txtProgAmministrazione" Width="70%"
					    Text="" CssClass="txtUppercase tb8" TabIndex="2" MaxLength="5"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtProgAmministrazione" ControlToValidate="txtProgAmministrazione"
                        ErrorMessage="Progressivo non valido" ValidationExpression="^[0-9]{5}$"
                        runat="server" Text="*"  Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                        CssClass="offClass field-is-required  onClassDomanda" Enabled="true" />
		    </td>
	    </tr>
        </asp:Panel>
    </table>
</asp:Panel>
<!-- Fine Pannello Common Header -->
<table class="tabellaFormattazione grid grid-size-20">
    <tr>
        <td class="Row1" style="width: 25%">
            <asp:Label ID="lblMicroqualifica" runat="server" Text="Microqualifica:"></asp:Label>
        </td>
        <asp:Panel runat="server" ID="pnlDDLMicroqualificaINPDAP" Visible="false">
            <td class="Row1 full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlMicroqualificaINPDAP" Width="90%" CssClass="txtUppercase tb8"
                    TabIndex="4">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="REQFddlMicroqualificaINPDAP" Display="Dynamic"
                    Text="*" CssClass="field-is-required" ErrorMessage="" ControlToValidate="ddlMicroqualificaINPDAP" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
            </td>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlTXTMicroqualificaINPDAP" Visible="false">
            <td class="Row1 full-grid" colspan="3">
                <asp:TextBox runat="server" ID="txtMicroqualificaINPDAP" Width="90%" CssClass="txtUppercase tb8"
                    TabIndex="4">
                </asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="REQFtxtMicroqualificaINPDAP" Display="Dynamic"
                    Text="*" CssClass="field-is-required" ErrorMessage="" ControlToValidate="txtMicroqualificaINPDAP" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
            </td>
        </asp:Panel>
    </tr>
    <asp:Panel runat="server" ID="pnlComparto" Visible="true">
        <tr>
            <td class="Row1" style="width: 25%">
            <asp:Label ID="lblComparto" runat="server" Text="Comparto:"></asp:Label>
            </td>
            <td class="Row1 full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlComparto" Width="90%" CssClass="txtUppercase tb8"
                    TabIndex="4" >
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
            <asp:Label ID="lblSettore" runat="server" Text="Settore:"></asp:Label>
            </td>
            <td class="Row1 full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlSettore" Width="90%" CssClass="txtUppercase tb8"
                    TabIndex="4">
                </asp:DropDownList>              
            </td>
            </tr>
        <tr>
             <td class="Row1" style="width: 25%">
            <asp:Label ID="lblRuolo" runat="server" Text="Ruolo:"></asp:Label>
            </td>
            <td class="Row1 full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlRuolo" Width="90%" CssClass="txtUppercase tb8"
                    TabIndex="4">
                </asp:DropDownList>
            </td>
          </tr>
    </asp:Panel>
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                Causa di cessazione:</label>
        </td>
        <td class="Row1" colspan="2">
            <%--<asp:TextBox ID="txtCausaCessazione" runat="server" Width="90%" Text="" CssClass="txtUppercase tb8"></asp:TextBox>--%>
            <asp:DropDownList runat="server" ID="ddlCausaCessazione" Width="90%" CssClass="tb8 txtUppercase"
                    TabIndex="19">
                </asp:DropDownList>
            <asp:RequiredFieldValidator runat="server" ID="RequiredCausaCessazione" Display="Dynamic"
                        ErrorMessage="Causa di cessazione: campo obbligatorio." Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiAssicurativiFS" ControlToValidate="ddlCausaCessazione" Enabled ="false"></asp:RequiredFieldValidator>
        </td>
    </tr>
</table>

<!-- Pannello Custom INPDAP -->
<asp:Panel runat="server" ID="pnlCustomINPDAP" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <asp:Panel ID="pnlDecAnteAgosto95" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Diritto Indennità Integrativa Speciale:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlDirittoIndennIntegrSpec" Width="30.5%" CssClass="tb8 txtUppercase xxs"
                        TabIndex="24">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Riduzione L.537:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlRiduzioneL537" Width="30.5%" CssClass="tb8 txtUppercase xxs"
                        TabIndex="26">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        I.I.S. RAP. ad Anni:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlIISAbbattimentoAnni" Width="30.5%" CssClass="tb8 txtUppercase xxs"
                        TabIndex="27">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <asp:Panel runat="server" ID="pnlVVUtiliDiritto" Visible="false">
                <td class="Row1 label-fields-high" style="width: 25%">
                    <label>
                        VV utili diritto:</label>
                </td>
                <td class="Row1 fileds-date-input" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtVVUtiliDirittoAA" Width="15%"
                        Text="" CssClass="txtUppercase tb8" Enabled="false" />
                    <label>AA</label>
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtVVUtiliDirittoMM" Width="15%"
                        Text="" CssClass="txtUppercase tb8" Enabled="false" />
                    <label>MM</label>
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtVVUtiliDirittoGG" Width="15%"
                        Text="" CssClass="txtUppercase tb8" Enabled="false" />
                    <label>GG</label>
                </td>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlVVUtiliMisura" Visible="false">
                <td class="Row1 label-fields-high" style="width: 25%">
                    <label>
                        VV utili misura:</label>
                </td>
                <td class="Row1 fileds-date-input" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtVVUtiliMisuraAA" Width="15%"
                        Text="" CssClass="txtUppercase tb8" Enabled="false" />
                    <label>AA</label>
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtVVUtiliMisuraMM" Width="15%"
                        Text="" CssClass="txtUppercase tb8" Enabled="false" />
                    <label>MM</label>
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtVVUtiliMisuraGG" Width="15%"
                        Text="" CssClass="txtUppercase tb8" Enabled="false" />
                    <label>GG</label>
                </td>
            </asp:Panel>
        </tr>
        <tr>
            <asp:Panel runat="server" ID="pnlAttivitaEconomica" Visible="false">
                <td class="Row1" style="width: 25%">
                    <label>
                        Attività Economica:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtAttivitaEconomica" Width="68%"
                        Text="" CssClass="txtUppercase tb8" TabIndex="29" Enabled="false" />
                </td>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlProfessioneIndividuale" Visible="false">
                <td class="Row1" style="width: 25%">
                    <label>
                        Professione individuale:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtProfessioneIndividuale" Width="68%"
                        Text="" CssClass="txtUppercase tb8" TabIndex="29" Enabled="false" />
                </td>
            </asp:Panel>
        </tr>
        <%--**Revisione Campi INPDAP**--%>
        <%--<tr>
            <<td class="Row1" style="width: 25%">
                <label>
                    Anni Max:               
                </label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtAnniMax" CssClass="txtUppercase tb8" MaxLength="2" Width="30%"/>
                <asp:RegularExpressionValidator runat="server" ID="REV_txtAnniMax" ControlToValidate="txtAnniMax" Display="Dynamic"
                    ErrorMessage="Anni Max in formato non valido" Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]*$" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Anni Utili:               
                </label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtAnniUtili" CssClass="txtUppercase tb8" MaxLength="2" Width="30%"/>
                <asp:RegularExpressionValidator runat="server" ID="REV_txtAnniUtili" ControlToValidate="txtAnniUtili" Display="Dynamic"
                    ErrorMessage="Anni Utili in formato non valido" Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]*$" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
            <td class="Row1" colspan="2">
                <label>             
                </label>
            </td>
        </tr>--%>
    </table>
</asp:Panel>
<!-- Fine Pannello Custom INPDAP -->
<!-- Pannello ripartizioni inpadap -->
<asp:Panel runat="server" ID="pnlRipartizioni" CssClass="mt-32">
    <div id="div1" style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; margin: 4px auto;">
        <table class="tabellaContenuti" style="width: 100%">
            <tr>
                <td align="left">
                    <asp:Label runat="server" ID="lblRipartizioniInpdap" Font-Bold="true">&nbsp; Ripartizioni INPDAP</asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <asp:GridView runat="server" ID="gvRipartizioni" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="100%" BorderColor="Black"
                        AutoGenerateEditButton="true" PageSize="10" AllowPaging="true" OnRowCommand="gvRipartizioni_RowCommand"
                        OnRowDataBound="gvRipartizioni_RowDataBound" OnRowCancelingEdit="gvRipartizioni_RowCancelingEdit"
                        OnRowEditing="gvRipartizioni_RowEditing" EnableViewState="true" OnPageIndexChanging="gvRipartizioni_onPageIndexChanging" PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
                            <asp:TemplateField HeaderText="Ente" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="46%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblEnte"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="%" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="46%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblPercentuale" Text='<%# Bind("Importo")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtPercentuale" Width="13%" CssClass="tb8 txtUppercase" MaxLength="5"
                                        Text='<%# Bind("Importo")%>'></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="REV_txtPercentuale" runat="server" ErrorMessage="% (Percentuale) inserita in formato non corretto"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtPercentuale" ValidationGroup="GrigliaRipartizioni"
                                        Display="Dynamic" ValidationExpression="^100(,00)?$|^\d{1,2}(,\d{1,2})?$">
                                    </asp:RegularExpressionValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<!-- Fine Pannello ripartizioni inpdap -->
</asp:Panel>
<!--div bottoni-->
<div style="width: 100%; margin-top: 25px; margin-right: 40px;">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaDatiAssicurativi" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Dati Assicurativi" Width="180px" OnClick="SalvaDatiAssicurativi_Click"
                    OnClientClick="if(Page_ClientValidate('UCTabDatiAssicurativiFS')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaDatiAssicurativi" SkinID="btnAzione1" runat="server" Width="180px"
                    Text="Elimina Dati Assicurativi" CausesValidation="False" OnClick="btnEliminaDatiAssicurativi_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Assicurativi?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
<!--fine div bottoni-->
<asp:HiddenField runat="server" ID="modalitaEdit" Value="false" />
<asp:HiddenField runat="server" ID="HiddenFieldCausaCessazione" />
<asp:HiddenField runat="server" ID="hiddenMicroqualifica" />
<asp:HiddenField runat="server" ID="hdnDecorrenzaCalcolo" />
<asp:HiddenField runat="server" ID="hdnDecorrenzaCalcoloOriginale" />
<asp:HiddenField runat="server" ID="hdnSKIP_ManageEnableBeneficiJS" value="FALSE" />
