<%@ Page Language="C#" MasterPageFile="~/ProcedureOperatore.Master" AutoEventWireup="true"
    CodeBehind="ConfermaAcquisizione.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.ConfermaAcquisizione" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <script type="text/javascript" src="Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="Javascript/supposition.js"></script>
    <style type="text/css">
        .fixed-dialog
        {
            position: fixed;
        }
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
        input[disabled="disabled"], input.disabled, input[disabled]
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
    <script type="text/javascript">
        $(document).ready(function () {
            document.getElementById("<%=validSummary.ClientID%>").style.display = "none";
            $('.offClass').attr('disabled', true);
            $('.offClass').val('');
            $('input:radio').attr('checked', false);
        });

        function SetRadio(rb) {
            $('input:radio').attr('checked', false); //Disabilita tutti i radio button
            $('.offClass').attr('disabled', true); //Disabilita tutti gli oggetti con la class "offClass"

            document.getElementById("<%=validSummary.ClientID%>").style.display = "none"; //Nasconde il validator summary
            $('.' + rb.getAttribute("EnableClass")).removeAttr('disabled'); //Abilita gli oggetti con l'attributo specificato
            if (rb.getAttribute("EnableClass") == "onClassAnagrafica") {
                $(document.getElementById("<%=radioAnagrafica.ClientID %>")).attr("checked", true);
                $(document.getElementById("<%=txtCognome.ClientID %>")).focus();
                $(document.getElementById("<%=txtDataNascita.ClientID%>")).datepicker({
                    changeMonth: true,
                    changeYear: true,
                    changeDay: true,
                    showButtonPanel: true,
                    dateFormat: 'dd/mm/yy',
                    showOn: 'button',
                    buttonImageOnly: true,
                    buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                    minDate: '-110y',
                    maxDate: '0',
                    yearRange: 'c-80:' + 'c+80:'
                });
                //$(document.getElementById("<%=txtDataNascita.ClientID%>")).unmask();
                //$(document.getElementById("<%=txtDataNascita.ClientID%>")).mask("99/99/9999");

                $('.onClassCodiceFiscale').val(''); //Pulisce tutti i campi con la class "onClassCodiceFiscale"
            }
            else if (rb.getAttribute("EnableClass") == "onClassCodiceFiscale") {
                $(document.getElementById("<%=radioCodiceFiscale.ClientID %>")).attr("checked", true);
                $(document.getElementById("<%=txtCodiceFiscale.ClientID %>")).focus();
                $(document.getElementById("<%=txtDataNascita.ClientID%>")).datepicker("destroy");

                $('.onClassAnagrafica').val(''); //Pulisce tutti i campi con la class "onClassAnagrafica"
            }

            //nel RadioButton via codeBehind
            SwitchValidator('.offClass', false); //Disabilita tutti i validatori
            //SwitchValidator('.' + rb.getAttribute("EnableClass"), true); //Abilita i validatori con l'attributo specificato
            //nel RadioButton via codeBehind
            rb.checked = true; //Seleziona il radioButton che ha scatenato l'evento
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

        function CleanFields() {
            document.getElementById("<%=txtNome.ClientID%>").value = '';
            document.getElementById("<%=txtCognome.ClientID%>").value = '';
            document.getElementById("<%=txtDataNascita.ClientID %>").value = '';
            document.getElementById("<%=txtCodiceFiscale.ClientID%>").value = '';
            return false;
        }

        function checkLunghezzaCognome(source, args) {
            if (args.Value.length < 2)
                args.IsValid = false;
            else
                args.IsValid = true;
            return false;
        }

        function Confirm() {
            $('#dialog-confirm').dialog('open');
            return false;

        }

        $(function () {
            $('#dialog-confirm').dialog({
                autoOpen: false,

                show: 'blind',
                hide: 'blind',
                height: 280,
                width: 450,
                modal: true,
                resizable: false,
                draggable: true,
                centerX: true,
                centerY: true,
                dialogClass: 'fixed-dialog',
                open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
                close: function (event, ui) { $('body').css('overflow', 'auto'); },
                buttons: {
                    'Annulla': function () {
                        $(this).dialog('close');
                        return false;
                    },
                    'Ok': function () {
                        $(this).dialog('close');
                        document.getElementById('<%= btnContinua.ClientID %>').click();
                        return true;
                    }
                }
            });
        });

        function CreatePopUpSede() {
            // jQuery UI Dialog
            var sedeDomanda = document.getElementById('<%=HdnSede.ClientID %>').value;
            $('#changeSedeUtente').text("La sede della domanda è " + sedeDomanda + ". Cambiare sede per proseguire?");
            var result;
            $('#changeSedeUtente').dialog(
                {
                    autoOpen: false,
                    width: 400,
                    modal: true,
                    resizable: false,
                    draggable: false,

                    buttons:
                        {
                            "Annulla": function () {
                                $(this).dialog("close");
                                $('#<%= HdnNDom.ClientID %>').val(''); //sbianco l'hdn filed
                                result = false;
                            },
                            "Conferma": function () {
                                $(this).dialog("close");
                                document.getElementById('<%= btnConfermaPopUp.ClientID %>').click();
                            }
                        }
                    ,
                    close: function (event, ui) {
                        if (event.originalEvent) {
                            $('#<%= HdnNDom.ClientID %>').val(''); //sbianco l'hdn filed 
                        }
                    }

                });
            $("#changeSedeUtente").parent().appendTo($("form:first"));
        }

        function ShowPopUpSede() {
            CreatePopUpSede();
            $('#changeSedeUtente').dialog('open');
        }

        function ShowPopUpGenerazioneCertificato() {
            CreatePopUpGenerazioneCertificato();
            $('#divGenerazioneNuovoCertificatoEnpals').dialog('open');
        }

        function CreatePopUpGenerazioneCertificato() {

            var categoria = document.getElementById('<%=HdnCodiceCategoria.ClientID %>').value;
            $('#divGenerazioneNuovoCertificatoEnpals').text("E' stato generato un nuovo certificato per la categoria " + categoria + ".");
            $('#divGenerazioneNuovoCertificatoEnpals').dialog(
                {
                    autoOpen: false,
                    width: 400,
                    modal: true,
                    resizable: false,
                    draggable: false,

                    buttons:
                        {
                            "OK": function () {
                                $(this).dialog("close");
                                document.getElementById('<%= btnConfermaMessaggioGenerazioneCertificato.ClientID %>').click();
                            },                           
                        }
                    ,
                    close: function (event, ui) {
                      document.getElementById('<%= btnConfermaMessaggioGenerazioneCertificato.ClientID %>').click();
                     }

                });

            $("#divGenerazioneNuovoCertificatoEnpals").parent().appendTo($("form:first"));
        }


          function ShowPopUpMemo239() {
            CreatePopUpMemo239();
            $('#divPopupMemo239').dialog('open');
        }

         function CreatePopUpMemo239() {

            $('#divPopupMemo239').dialog(
                {
                    autoOpen: false,
                    width: 400,
                    modal: true,
                    resizable: false,
                    draggable: false,
                    centerX: true,
                    centerY: true,
                    dialogClass: 'fixed-dialog',
                    buttons:
                        {
                            "Continua": function () {
                                $(this).dialog("close");                                
                                document.getElementById('<%= btnConfermaPopupMemo239.ClientID %>').click();
                            }                     
                        }
                    ,
                    close: function (event, ui) {
                            document.getElementById('<%= btnConfermaPopupMemo239.ClientID %>').click();
                     }

                });

            $("#divPopupMemo239").parent().appendTo($("form:first"));
        }

            function ShowPopUpMemo312023() {
            CreatePopUpMemo312023();
            $('#divPopupMemo312023').dialog('open');
        }

         function CreatePopUpMemo312023() {

            $('#divPopupMemo312023').dialog(
                {
                    autoOpen: false,
                    width: 400,
                    modal: true,
                    resizable: false,
                    draggable: false,
                    centerX: true,
                    centerY: true,
                    dialogClass: 'fixed-dialog',
                    buttons:
                        {
                            "Continua": function () {
                                $(this).dialog("close");                                
                                document.getElementById('<%= btnConfermaPopupMemo312023.ClientID %>').click();
                            }                     
                        }
                    ,
                    close: function (event, ui) {
                            document.getElementById('<%= btnConfermaPopupMemo312023.ClientID %>').click();
                     }

                });

            $("#divPopupMemo312023").parent().appendTo($("form:first"));
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="containerWidth xs is-contents">
        <tr>
            <td>
                <table width="100%" align="center">
                    <tr class="iframe-dnone">
                        <td align="center">
                            <label style="color: #336699; font-weight: bold; font-size: larger; width: 100%;">
                                Dettaglio domanda da acquisire</label>
                            <br />
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1" style="text-align: left" colspan="2">
                            <asp:Label ID="lblOpzDonna2023" runat="server" Text="ATTENZIONE le condizioni previste dalle lettere a), b) e c) dell’articolo 1, comma 292, della legge n. 197 del 2022 devono sussistere al momento della presentazione della domanda"
                                Style="font-weight: bold; font-size: 100%;" Visible="false"></asp:Label>
                        </td>
                    </tr>
                </table>
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
                <div align="right" style="margin-top: 12px;">
                <asp:Button runat="server" ID="btnRichiestaLavorazioneManuale" Visible="false" 
                    Text="Richiesta Lavorazione Manuale" CausesValidation="false"
                    OnCommand="btnRichiestaLavorazioneManuale_Click" class="pulsante1 tertiary" OnClientClick="if (!window.confirm('Si vuole definire una domanda senza passaggio da Unicarpe. Si conferma la richiesta di lavorazione manuale?')) return false; else BlockUI();"/>
                    </div>
                <asp:ValidationSummary runat="server" ID="validSummary" ValidationGroup="ricercaDanteCausa"
                    Font-Size="Small" CssClass="errorBox" />
                <br />
                <asp:Panel ID="pnlRicercaDanteCausa" runat="server" Style="border-style: solid; border-color: #000080;
                    border-collapse: collapse; border-width: 1px; width: 100%; margin-left: 0px;"
                    Visible="false">
                    <div class="form" style="padding: 10px">
                        <table width="100%">
                            <tr>
                                <td style="width: 50px; vertical-align: top; text-align: center">
                                    <asp:Image ID="imgIcon" runat="server" />
                                </td>
                                <td style="width: 670px; vertical-align: middle;">
                                    <asp:Label ID="lblMsg" runat="server" Font-Size="Medium"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <table width="100%">
                            <tr>
                                <td class="etichettaBold">
                                    <label>
                                        E' possibile effettuare la ricerca manuale del Dante Causa:</label>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <table width="100%">
                            <tr>
                                <td class="radioButton" width="5%">
                                    <asp:RadioButton runat="server" ID="radioCodiceFiscale" CssClass="CodiceFiscale radioButton" />
                                </td>
                                <td class="etichetta" width="17%">
                                    <label>
                                        Codice fiscale:</label>
                                </td>
                                <td class="field" width="78%" colspan="4">
                                    <div runat="server" id="divTxtCodiceFiscale">
                                        <asp:TextBox Style="text-align: left" runat="server" ID="txtCodiceFiscale" Width="175px"
                                            CssClass="txtUppercase tb8 offClass onClassCodiceFiscale" TabIndex="1" MaxLength="16"></asp:TextBox>
                                        <asp:CustomValidator ControlToValidate="txtCodiceFiscale" runat="server" Text="*" CssClass="field-is-required"
                                            Display="Dynamic" ValidationGroup="ricercaDanteCausa" ID="txtCodiceFiscale_CV"
                                            ClientValidationFunction="validateCodiceFiscale" ErrorMessage="Codice fiscale non valido" />
                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="txtCodiceFiscale"
                                            Enabled="false" ErrorMessage="Inserire un codice fiscale" Text="*" Display="Dynamic"
                                            ValidationGroup="ricercaDanteCausa" CssClass="offClass  onClassCodiceFiscale field-is-required" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="radioButton" width="5%">
                                    <asp:RadioButton runat="server" ID="radioAnagrafica" CssClass="Anagrafica radioButton" />
                                </td>
                                <td class="etichetta" width="17%">
                                    <label>
                                        Cognome:</label>
                                </td>
                                <td class="field" width="33%">
                                    <div runat="server" id="divTxtCognome">
                                        <asp:TextBox Style="text-align: left" runat="server" ID="txtCognome" Width="175px"
                                            CssClass="txtUppercase tb8 offClass onClassAnagrafica " TabIndex="1" MaxLength="50"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtCognome"
                                            ErrorMessage="Cognome non  valido" ValidationExpression="^[\x20a-zA-Z ']+$" runat="server"
                                            Text="*" Display="Dynamic" ValidationGroup="ricercaDanteCausa" CssClass="offClass field-is-required  onClassAnagrafica"
                                            Enabled="false" />
                                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtCognome"
                                            Enabled="false" ErrorMessage="Inserire un cognome" Text="*" Display="Dynamic"
                                            ValidationGroup="ricercaDanteCausa" CssClass="offClass  onClassAnagrafica field-is-required" />
                                        <asp:CustomValidator runat="server" ControlToValidate="txtCognome" Display="Dynamic"
                                            ErrorMessage="Cognome: il campo deve essere lungo almeno due caratteri" Text="*"
                                            ValidationGroup="ricercaDanteCausa" ID="customCheckCognomeValidator" ClientValidationFunction="checkLunghezzaCognome"
                                            CssClass="offClass  onClassAnagrafica field-is-required" />
                                    </div>
                                </td>
                                <td width="3%">
                                </td>
                                <td class="etichetta" width="12%">
                                    <label>
                                        Nome:</label>
                                </td>
                                <td class="field" width="30%">
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtNome" Width="175px" CssClass="txtUppercase tb8 offClass  onClassAnagrafica"
                                        MaxLength="50" TabIndex="2"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtNome"
                                        ErrorMessage="Nome non valido" ValidationExpression="^[\x20a-zA-Z ']+$" runat="server"
                                        Text="*" Display="Dynamic" ValidationGroup="ricercaDanteCausa" CssClass="offClass  onClassAnagrafica field-is-required"
                                        Enabled="false" />
                                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtNome"
                                        Enabled="false" ErrorMessage="Inserire un nome" Text="*" Display="Dynamic" ValidationGroup="ricercaDanteCausa"
                                        CssClass="offClass  onClassAnagrafica field-is-required" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtNome" Display="Dynamic"
                                        ErrorMessage="Nome: il campo deve essere lungo almeno tre caratteri" Text="*"
                                        ValidationGroup="ricercaDanteCausa" ID="customCheckNomeValidator" ClientValidationFunction="checkLunghezzaNome"
                                        CssClass="offClass  onClassAnagrafica field-is-required" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td class="etichetta">
                                    <label>
                                        Data di nascita:</label>
                                </td>
                                <td class="field" colspan="4">
                                    <asp:TextBox ID="txtDataNascita" CssClass="tb8 txtUppercase offClass onClassAnagrafica"
                                        runat="server" Text="gg/mm/aaaa" Width="175px" MaxLength="10" TabIndex="3"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator5" ControlToValidate="txtDataNascita"
                                        ErrorMessage="Data Nascita: inserire la data nel formato gg/mm/aaaa" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                        runat="server" Text="*" Display="Dynamic" ValidationGroup="ricercaDanteCausa"
                                        CssClass="offClass  onClassAnagrafica field-is-required" Enabled="false" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDataNascita" Display="Dynamic"
                                        ErrorMessage="Data di nascita: Data inserita posteriore a quella odierna" Text="*"
                                        ValidationGroup="ricercaDanteCausa" ID="customDataNascitaValidator" ClientValidationFunction="checkDataPostOdiernaGGMMAAAA"
                                        CssClass="offClass  onClassAnagrafica field-is-required" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDataNascita" Display="Dynamic"
                                        ErrorMessage="Data di nascita: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="ricercaDanteCausa"
                                        ID="customCheckDataDataDiNascita" ClientValidationFunction="checkCorrettezzaData" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>
                <br />

                <asp:Panel ID="pnlConferma" runat="server" Style="border-style: solid; border-color: #000080;
                    border-collapse: collapse; border-width: 1px; width: 100%; margin-left: 0px;
                    background-position: right top; background-repeat: no-repeat; background-image: url('../App_Themes/BlueINPS1/Images/detail.png');" CssClass="iframe-dnone">
                    <table class="tabellaFormattazione">
                        <tr>
                            <td>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1" style="width: 20%;">
                                <label class="etichettaBold">
                                    Numero Domanda:</label>
                            </td>
                            <td class="Row1" style="width: 30%; text-align: left">
                                <asp:Label ID="lblNumeroDomanda" runat="server"></asp:Label>
                            </td>
                            <td class="Row1" style="width: 20%;">
                                <label class="etichettaBold">
                                    Categoria:</label>
                            </td>
                            <td class="Row1" style="width: 30%; text-align: left">
                                <asp:Label ID="lblCategoria" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1" style="width: 20%;">
                                <label class="etichettaBold">
                                    Cognome:</label>
                            </td>
                            <td class="Row1" style="width: 30%; text-align: left">
                                <asp:Label ID="lblCognome" runat="server"></asp:Label>
                            </td>
                            <td class="Row1" style="width: 20%;">
                                <label class="etichettaBold">
                                    Nome:</label>
                            </td>
                            <td class="Row1" style="width: 30%; text-align: left">
                                <asp:Label ID="lblNome" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1" style="width: 20%;">
                                <label class="etichettaBold">
                                    Codice Fiscale:</label>
                            </td>
                            <td class="Row1 full-grid" colspan="3" width="80%">
                                <asp:Label ID="lblCodiceFiscale" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1">
                                <label class="etichettaBold">
                                    Gruppo:
                                </label>
                                <asp:Label ID="lblGruppo" runat="server"></asp:Label>
                            </td>
                            <td class="Row1">
                                <label class="etichettaBold">
                                    Prodotto:
                                </label>
                                <asp:Label ID="lblProdotto" runat="server"></asp:Label>
                            </td>
                            <td class="Row1">
                                <label class="etichettaBold">
                                    Tipo:
                                </label>
                                <asp:Label ID="lblTipo" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1" colspan="999">
                                <label class="etichettaBold">
                                    Descrizione:
                                </label>
                                <asp:Label ID="lblDescrizione" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr id="trChiavePensione" runat="server">
                            <td class="Row1" colspan="999">
                                <label class="etichettaBold">
                                    Chiave Pensione:
                                </label>
                                <asp:Label ID="lblChiavePensione" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <asp:Panel ID="pnlPensioniOvunque" runat="server" Visible="false">
                            <tr>
                                <td class="Row1" colspan="999">
                                    <label class="etichettaBold">
                                        Chiave Pensione:
                                    </label>
                                    <asp:Label ID="lblChiaveDomandaPensioniOvunque" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="Row1" colspan="999">
                                    <label class="etichettaBold">
                                        Sigla Pensione:
                                    </label>
                                    <asp:Label ID="lblSiglaCategoriaPensioniOvunque" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="Row1" colspan="999">
                                    <label class="etichettaBold">
                                        Sede Gestione Pensione:
                                    </label>
                                    <asp:Label ID="lblSedeGestionePensioniOvunque" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </asp:Panel>

                        <tr id="trInformativaRicEnpals" runat="server" visible="false">
                            <td class="Row1" colspan="999" style="padding-top: 20px">
                                <asp:Label ID="lblInformativaRicEnpals" runat="server" Text="SI RICORDA CHE SE E’ PRESENTE UNA PRESTAZIONE COLLEGATA AL REDDITO, PER LE PENSIONI DELLA GESTIONE SPETTACOLO E SPORT VANNO SEMPRE ACQUISITI I REDDITI, ANCHE IN CASO DI RICOSTITUZIONE NON REDDITUALE.</br></br>IL MANCATO INSERIMENTO DI REDDITI DAL 1995, OVVERO DALLA DECORRENZA ORIGINARIA, PUO’ COMPORTARE UN CONGUAGLIO A DEBITO O CREDITO NON CORRETTO."></asp:Label>
                            </td>
                        </tr>
                        <tr id="trINDCOM" runat="server" visible="false">
                            <td class="Row1" colspan="999" style="padding-top: 20px">
                                <asp:Label ID="lblInformativaIndcom" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />
                </asp:Panel>

                <div class="iframe-conferma-summary" style="display: none">
                    <div class="field-row">
                        <div class="field">
                            <div class="label">Nome Cognome</div>
                            <div><asp:Label ID="lblNomeNew" runat="server"></asp:Label> <asp:Label ID="lblCognomeNew" runat="server"></asp:Label></div>
                        </div>
                        <div class="field">
                            <div class="label">Codice Fiscale</div>
                            <div><asp:Label ID="lblCodiceFiscaleNew" runat="server"></asp:Label></div>
                        </div>
                        <div class="field">
                            <div class="label">Numero domanda</div>
                            <div><asp:Label ID="lblNumeroDomandaNew" runat="server"></asp:Label></div>
                        </div>
                    </div>
                    <div class="field-row">
                        <div class="field">
                            <div class="label">Categoria</div>
                            <div><asp:Label ID="lblCategoriaNew" runat="server"></asp:Label></div>
                        </div>
                        <div class="field">
                            <div class="label">Descrizione</div>
                            <div><asp:Label ID="lblDescrizioneNew" runat="server"></asp:Label></div>
                        </div>
                        <div class="field">
                            <div class="label">Tipo</div>
                            <div><asp:Label ID="lblTipoNew" runat="server"></asp:Label></div>
                        </div>
                    </div>
                    <div class="field-row">
                        <div class="field">
                            <div class="label">Gruppo</div>
                            <div><asp:Label ID="lblGruppoNew" runat="server"></asp:Label></div>
                        </div>
                        <div class="field">
                            <div class="label">Prodotto</div>
                            <div><asp:Label ID="lblProdottoNew" runat="server"></asp:Label></div>
                        </div>
                        <div class="field">
                        </div>
                    </div>

                    <div class="field-row">
                        <div class="field">
                            <div class="info-banner">
                                <img src="App_Themes/iFrame/Images/circle-exclamation.svg" /><span>Se hai utilizzato le procedure di diritto/misura Unicarpe/Asi, consigliamo di verificare che l'archivio Felpe sia stato correttamente scritto prima di procedere.</span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="iframe-conferma-summary-buttons content-center">
                    <asp:Button ID="Button1" runat="server" Text="Acquisisci domanda" SkinID="btnAzione1"
                                CausesValidation="false" OnCommand="btnContinua_Click" OnClientClick="BlockUI();" CssClass="primary" />
                </div>
                <br />
                <table width="100%" class="iframe-dnone">
                    <tr style="text-align: center;">
                        <td>
                            <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                                CausesValidation="false" OnClientClick="BlockUI()" PostBackUrl="~/ElaborazionePosizione.aspx"
                                Width="165px"  CssClass="tertiary"/>
                        </td>
                        <td>
                            <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate"
                                SkinID="btnAzione1" CausesValidation="false" Width="165px" PostBackUrl="~/RisultatoRicercaElaborazione.aspx?Conferma=true"
                                OnClientClick="BlockUI()" Visible="false" Style="padding-left: 0px; padding-right: 0px;"  CssClass="tertiary"/>
                        </td>
                        <td style="display: none">
                            <asp:Button ID="btnContinua" runat="server" Text="" SkinID="btnAzione1" Width="180px"
                                CausesValidation="false" OnCommand="btnContinua_Click" OnClientClick="BlockUI();"  CssClass="tertiary"/>
                        </td>
                        <td>
                            <asp:Button ID="btnpopup" runat="server" Text="Continua" SkinID="btnAzione1" Width="165px"
                                CausesValidation="false" OnClientClick="return Confirm();"  CssClass="tertiary"/>
                        </td>
                        <td>
                            <asp:Button ID="btnConsultazione" runat="server" Text="Consulta" SkinID="btnAzione1"
                                CausesValidation="false" Width="165px" OnCommand="btnContinua_Click" CommandArgument="Consulta"
                                OnClientClick="BlockUI()" Visible="false" CssClass="tertiary" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5" style="text-align: center;" class="shift-full-grid">
                            <a href="~/ElaborazionePosizione/Stampa.aspx" target="_blank" class="linkLikeButton"
                                runat="server" id="aVisualizzaPensione" visible="false" style="width: 165px;">Visualizza
                                Pensione </a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div id="dialog-confirm" title="Conferma" style="display: none; border-style: none;
        border-color: White;">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
            Domanda in attesa di prelievo da WebDom. Confermi l'acquisizione?
        </p>
        <p style="font-size: 15px; margin-left: 25px; margin-right: 20px">
            <br />Se la lavorazione è stata fatta sulle procedure di diritto/misura UNICARPE/ASI, assicurarsi che sia stato correttamente scritto l'archivio FELPE prima di procedere.
        </p>
    </div>
    <!-- Cambio Sede -->
    <div id="changeSedeUtente" title="Cambia sede" style="display: none;">
        <p>
        </p>
    </div>
    <div id="divGenerazioneNuovoCertificatoEnpals" title="Generazione Nuovo Certificato"
        style="display: none;">
        <p>
        </p>
    </div>

    <div id="divPopupMemo239" title="Avviso" 
        style="display: none;">
        <p>
        "Attenzione. Verificare l’esito del giudizio medico legale e acquisirlo in Webdom."
        </p>
    </div>

    
    <div id="divPopupMemo312023" title="Avviso" 
        style="display: none;">
        <p>
        "Attenzione, per trasformare da provvisoria a definitiva una domanda di precoci in cumulo contattare la casella Unipens@inps.it per avere le informazioni necessarie per la lavorazione in modalità manuale."
        </p>
    </div>

    <asp:Button ID="btnConfermaPopUp" CausesValidation="true" Style="display: none" runat="server"
        OnCommand="btnConfermaPopUp_Click" CommandArgument="Consulta" OnClientClick="BlockUI();"
        Text=""  CssClass="tertiary"/>
    <asp:Button ID="btnConfermaMessaggioGenerazioneCertificato" CausesValidation="true"
        Style="display: none" runat="server" OnCommand="btnConfermaMessaggioGenerazioneCertificato_Click"
        OnClientClick="BlockUI();" Text="" />
    <asp:Button ID="btnConfermaPopupMemo239" CausesValidation="true"
        Style="display: none" runat="server" OnCommand="btnConfermaPopupMemo239_Click"
        OnClientClick="BlockUI();" Text="" />
    <asp:Button ID="btnConfermaPopupMemo312023" CausesValidation="true"
        Style="display: none" runat="server" OnCommand="btnConfermaPopupMemo312023_Click"
        OnClientClick="BlockUI();" Text="" />
    <asp:HiddenField ID="HdnSede" runat="server"></asp:HiddenField>
    <asp:HiddenField ID="HdnNDom" runat="server"></asp:HiddenField>
    <asp:HiddenField ID="HdnCodiceCategoria" runat="server"></asp:HiddenField>
    <asp:HiddenField ID="HDecorrenzaFinestra" runat="server"></asp:HiddenField>
</asp:Content>