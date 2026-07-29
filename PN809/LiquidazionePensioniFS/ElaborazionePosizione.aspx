<%@ Page Title="" Language="C#" MasterPageFile="~/ProcedureOperatore.Master" AutoEventWireup="true"
    CodeBehind="ElaborazionePosizione.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.ElaborazionePosizione" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">
        $(document).ready(function() {
            $(document.getElementById("<%=btnRicerca.ClientID%>")).attr('disabled', true);
            $(document.getElementById("<%=btnAnnulla.ClientID%>")).attr('disabled', true);
            document.getElementById("<%=validSummary.ClientID%>").style.display = "none";
            $('.offClass').attr('disabled', true);
            $('.offClass').val('');
            $('input:radio').attr('checked', false);
        });

        function SetRadio(rb) {
            $('input:radio').attr('checked', false); //Disabilita tutti i radio button
            $('.offClass').attr('disabled', true); //Disabilita tutti gli oggetti con la class "offClass"

            document.getElementById("<%=validSummary.ClientID%>").style.display = "none"; //Nasconde il validator summary
            $(document.getElementById("<%=btnRicerca.ClientID%>")).removeAttr('disabled'); //Abilita il pulsante btnRicerca
            $(document.getElementById("<%=btnAnnulla.ClientID%>")).removeAttr('disabled'); //Abilita il pulsante btnRicerca
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

                $('.onClassDomanda').val(''); //Pulisce tutti i campi con la class "onClassDomanda"
                $('.onClassCodiceFiscale').val(''); //Pulisce tutti i campi con la class "onClassCodiceFiscale"
            }
            else if (rb.getAttribute("EnableClass") == "onClassDomanda") {
                $(document.getElementById("<%=radioDomanda.ClientID %>")).attr("checked", true);
                $(document.getElementById("<%=txtNumeroDomanda.ClientID %>")).focus();
                $(document.getElementById("<%=txtDataNascita.ClientID%>")).datepicker("destroy");

                $('.onClassAnagrafica').val(''); //Pulisce tutti i campi con la class "onClassAnagrafica"
                $('.onClassCodiceFiscale').val(''); //Pulisce tutti i campi con la class "onClassCodiceFiscale"
            }
            else if (rb.getAttribute("EnableClass") == "onClassCodiceFiscale") {
                $(document.getElementById("<%=radioCodiceFIscale.ClientID %>")).attr("checked", true);
                $(document.getElementById("<%=txtCodiceFiscale.ClientID %>")).focus();
                $(document.getElementById("<%=txtDataNascita.ClientID%>")).datepicker("destroy");

                $('.onClassAnagrafica').val(''); //Pulisce tutti i campi con la class "onClassAnagrafica"
                $('.onClassDomanda').val(''); //Pulisce tutti i campi con la class "onClassDomanda"
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
            document.getElementById("<%=txtNumeroDomanda.ClientID%>").value = '';
            return false;
        }

        function checkLunghezzaCognome(source, args) {
            if (args.Value.length < 2)
                args.IsValid = false;
            else
                args.IsValid = true;
            return false;
        }

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
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <asp:ValidationSummary runat="server" ID="validSummary" ValidationGroup="anagrafica"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel class="pnlElaborazionePosizione" runat="server" ID="panel" DefaultButton="btnRicerca" Width="720px">
        <div class="page-title" style="display: none">
            <h2 class="page-title-secondlevel">Ricerca domanda</h2>
            <h6 class="page-subtitle">Seleziona il metodo di ricerca</h6>
        </div>

        <div class="form div_container grid-contents grid-contents--1-row" style="height: 40px; padding-top: 10px; ">
            <table width="100%">
                <tr>
                    <td class="radioButton">
                        <asp:RadioButton runat="server" ID="radioDomanda" CssClass=" radioButton" />
                        <label style="display: none">
                            Numero Domanda</label>
                    </td>
                    <td class="etichetta">
                        <label>
                            Numero Domanda:</label>
                    </td>
                    <td class="field">
                        <div runat="server" id="divTxtNumeroDomanda">
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtNumeroDomanda" Width="175px"
                                CssClass="txtUppercase tb8 offClass onClassDomanda" TabIndex="1" MaxLength="13"
                                onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                                onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtNumeroDomanda"
                                ErrorMessage="Numero domanda deve essere lungo 13" ValidationExpression="^[0-9]{13}$" runat="server"
                                Text="*" Display="Dynamic" ValidationGroup="anagrafica" CssClass="offClass field-is-required onClassDomanda"
                                Enabled="false" />
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator6" ControlToValidate="txtNumeroDomanda"
                                ErrorMessage="Il Numero di Domanda non può avere come prima cifra 0" ValidationExpression="^[1-9]{1}[0-9]{12}$" runat="server"
                                Text="*" Display="Dynamic" ValidationGroup="anagrafica" CssClass="offClass field-is-required onClassDomanda"
                                Enabled="false" />                                
                                
                                
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtNumeroDomanda"
                                Enabled="false" ErrorMessage="Inserire un numero Domanda" Text="*" Display="Dynamic"
                                ValidationGroup="anagrafica" CssClass="offClass field-is-required onClassDomanda" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div class="form div_container grid-contents grid-contents--1-row" style="height: 40px; padding-top: 10px;">
            <table width="100%">
                <tr>
                    <td class="radioButton">
                        <asp:RadioButton runat="server" ID="radioCodiceFIscale" CssClass="CodiceFiscale radioButton" />
                        <label style="display: none">
                            Codice fiscale</label>
                    </td>
                    <td class="etichetta">
                        <label>
                            Codice fiscale:</label>
                    </td>
                    <td class="field">
                        <div runat="server" id="divTxtCodiceFiscale">
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtCodiceFiscale" Width="175px"
                                CssClass="txtUppercase tb8 offClass onClassCodiceFiscale" TabIndex="1" MaxLength="16"></asp:TextBox>
                            <asp:CustomValidator ControlToValidate="txtCodiceFiscale" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="anagrafica"
                                ID="txtCodiceFiscale_CV" ClientValidationFunction="validateCodiceFiscale"
                                ErrorMessage="Codice fiscale non valido" />
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="txtCodiceFiscale"
                                Enabled="false" ErrorMessage="Inserire un codice fiscale" Text="*" Display="Dynamic"
                                ValidationGroup="anagrafica" CssClass="offClass field-is-required onClassCodiceFiscale" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div class="form div_container form grid-contents grid-contents--2-row" style="height: 70px; padding-top: 10px;">
            <table width="100%">
                <tr>
                    <td class="radioButton">
                        <asp:RadioButton runat="server" ID="radioAnagrafica" CssClass="Anagrafica radioButton" />
                        <label style="display: none">
                            Cognome, nome, data di nascita</label>
                    </td>
                    <td class="etichetta">
                        <label>
                            Cognome:</label>
                    </td>
                    <td class="field">
                        <div runat="server" id="divTxtCognome">
                        <asp:TextBox Style="text-align: left" runat="server" ID="txtCognome" Width="175px"
                            CssClass="txtUppercase tb8 offClass onClassAnagrafica " TabIndex="1" MaxLength="50"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtCognome"
                            ErrorMessage="Cognome non  valido" ValidationExpression="^[\x20a-zA-Z ']+$" runat="server"
                            Text="*" Display="Dynamic" ValidationGroup="anagrafica" CssClass="offClass field-is-required onClassAnagrafica"
                            Enabled="false" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtCognome"
                            Enabled="false" ErrorMessage="Inserire un cognome" Text="*" Display="Dynamic"
                            ValidationGroup="anagrafica" CssClass="offClass field-is-required onClassAnagrafica" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtCognome" Display="Dynamic"
                                ErrorMessage="Cognome: il campo deve essere lungo almeno due caratteri" Text="*" ValidationGroup="anagrafica"
                                ID="customCheckCognomeValidator" ClientValidationFunction="checkLunghezzaCognome" CssClass="offClass field-is-required onClassAnagrafica" />
                    </div>
                    </td>
                        <td class="etichetta">
                        <label>
                            Nome:</label>
                    </td>
                    <td class="field">
                        
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtNome" Width="175px" CssClass="txtUppercase tb8 offClass  onClassAnagrafica"
                                MaxLength="50" TabIndex="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtNome"
                                ErrorMessage="Nome non valido" ValidationExpression="^[\x20a-zA-Z ']+$" runat="server"
                                Text="*" Display="Dynamic" ValidationGroup="anagrafica" CssClass="offClass field-is-required onClassAnagrafica"
                                Enabled="false" />
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtNome"
                                Enabled="false" ErrorMessage="Inserire un nome" Text="*" Display="Dynamic" ValidationGroup="anagrafica"
                                CssClass="offClass field-is-required onClassAnagrafica" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtNome" Display="Dynamic"
                                ErrorMessage="Nome: il campo deve essere lungo almeno tre caratteri" Text="*" ValidationGroup="anagrafica"
                                ID="customCheckNomeValidator" ClientValidationFunction="checkLunghezzaNome" CssClass="offClass field-is-required onClassAnagrafica" />                                
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td class="etichetta">
                        <label>
                            Data di nascita:</label>
                    </td>
                    <td class="field" style="width: 33%">
                        <asp:TextBox ID="txtDataNascita" CssClass="tb8 txtUppercase offClass onClassAnagrafica"
                            runat="server" Text="gg/mm/aaaa" Width="175px" MaxLength="10" TabIndex="3"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" ControlToValidate="txtDataNascita"
                            ErrorMessage="Data Nascita: inserire la data nel formato gg/mm/aaaa" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="anagrafica" CssClass="offClassfield-is-required  onClassAnagrafica"
                            Enabled="false" />
                        <asp:CustomValidator runat="server" ControlToValidate="txtDataNascita" Display="Dynamic"
                            ErrorMessage="Data di nascita: Data inserita posteriore a quella odierna" Text="*"
                            ValidationGroup="anagrafica" ID="customDataNascitaValidator" ClientValidationFunction="checkDataPostOdiernaGGMMAAAA" CssClass="offClassfield-is-required  onClassAnagrafica" />
                        <asp:CustomValidator runat="server" ControlToValidate="txtDataNascita" Display="Dynamic"
                            ErrorMessage="Data di nascita: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="anagrafica"
                            ID="customCheckDataDataDiNascita" ClientValidationFunction="checkCorrettezzaData" />  
                    </td>
                </tr>
            </table>
        </div>
        <div class="form grid-contents grid-contents--form-row" style="width: 720px">
            <table width="100%">
                <tr>
                    <td style="text-align: right">
                        <asp:Button ID="btnRicerca" runat="server" Text="Cerca" SkinID="btnAzione1" OnClick="btnRicerca_Click"
                            CssClass="primary"
                            OnClientClick="javascript:CheckValidator(); if(Page_ClientValidate('anagrafica')){aspnetForm.target ='_self'; BlockUI();}" 
                            CausesValidation="false" TabIndex="4"
                              />
                        <%--PostBackUrl="~/RisultatoRicercaElaborazione.aspx"--%>
                    </td>
                    <td style="text-align: left">
                        <asp:Button ID="btnAnnulla" runat="server" SkinID="btnAzione1" OnClientClick="javascript:return CleanFields();"
                            Text="Annulla" />
                    </td>
                </tr>
            </table>
        </div>

        <!-- Cambio Sede -->
          <div id="changeSedeUtente" title="Cambia sede" style="display: none;">
            <p></p>
        </div>
        <asp:Button ID="btnConfermaPopUp" CausesValidation="true" Style="display: none" runat="server" 
            OnClick="btnConfermaPopUp_Click" OnClientClick="BlockUI();" Text="" />
    <asp:HiddenField ID="HdnSede" runat="server"></asp:HiddenField>
    <asp:HiddenField ID="HdnNDom" runat="server"></asp:HiddenField>
    </asp:Panel>
</asp:Content>



