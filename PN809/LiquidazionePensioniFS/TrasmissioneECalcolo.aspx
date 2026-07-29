<%@ Page Title="" Language="C#" MasterPageFile="~/ProcedureOperatore.Master" AutoEventWireup="true"
    CodeBehind="TrasmissioneECalcolo.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.TrasmissioneECalcolo" %>

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
            var doAction = false;
            var cssClass;
            var tipoOperazione = document.getElementById("<%=HiddenSelectedTipoOperazione.ClientID%>").value; //L'hidden field è valorizzato con il tipo di ricerca
            if (tipoOperazione == 'invioPosizione') { //Nel caso di un postback riabilito il blocco precedentemente selezionato
                doAction = true;
                cssClass = '.onClassInvioPosizione';

            }
            else if (tipoOperazione == 'RicercaPosizione') {

                doAction = true;
                cssClass = '.onClassRicercaPosizione';

            }
            else { //nel caso del primo caricamento della pagina
                $('.offClass').val('');
                $('input:radio').attr('checked', false);
            }
            if (doAction) {
                $(cssClass).removeAttr('disabled');
                $(document.getElementById("<%=btnRicerca.ClientID%>")).removeAttr('disabled');
                $(document.getElementById("<%=btnAnnulla.ClientID%>")).removeAttr('disabled');
                SwitchValidator(cssClass, true);
            }
        });

        function SetRadio(rb) {
            $('input:radio').attr('checked', false);                                        //Disabilita tutti i radio button
            $('.offClass').attr('disabled', true);                                          //Disabilita tutti gli oggetti con la class "offClass"
            $('.offClass').val('');                                                         //Pulisce tutti i campi con la class "offClass"

            document.getElementById("<%=validSummary.ClientID%>").style.display = "none";   //Nasconde il validator summary
            $('.' + rb.getAttribute("EnableClass")).removeAttr('disabled');                 //Abilita gli oggetti con l'attributo specificato
            if (rb.getAttribute("EnableClass") == "onClassRicercaPosizione") {              //Controllo se è stato selezionato il radio button ricerca posizione
                $(document.getElementById("<%=radioRicercaPosizioneDaTrasmettere.ClientID %>")).attr("checked", true);
                $(document.getElementById("<%=txtRicercaPosizioneCriterio1.ClientID %>")).focus();



                $(document.getElementById("<%=btnRicerca.ClientID%>")).removeAttr('disabled');    //Abilita il pulsante btnRicerca
                $(document.getElementById("<%=btnAnnulla.ClientID%>")).removeAttr('disabled');    //Abilita il pulsante btnRicerca
                $(document.getElementById("<%=txtDataLavorazioneDal.ClientID%>")).datepicker({
                    changeMonth: true,
                    changeYear: true,
                    changeDay: true,
                    showButtonPanel: true,
                    dateFormat: 'dd/mm/yy',
                    showOn: 'button',
                    buttonImageOnly: true,
                    buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',

                    maxDate: '+0',
                    minDate: '-50y'

                    //yearRange: 'c-50:' + 'c+0:'

                });
                //$(document.getElementById("<%=txtDataLavorazioneDal.ClientID%>")).unmask();
                //$(document.getElementById("<%=txtDataLavorazioneDal.ClientID%>")).mask("99/99/9999");
                
                $(document.getElementById("<%=txtDataLavorazioneAl.ClientID%>")).datepicker({
                    changeMonth: true,
                    changeYear: true,
                    changeDay: true,
                    showButtonPanel: true,
                    dateFormat: 'dd/mm/yy',
                    showOn: 'button',
                    buttonImageOnly: true,
                    buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                    yearRange: 'c-50:' + 'c+0:'
                });
                //$(document.getElementById("<%=txtDataLavorazioneAl.ClientID%>")).unmask();
                //$(document.getElementById("<%=txtDataLavorazioneAl.ClientID%>")).mask("99/99/9999");
            }
            else {

                $(document.getElementById("<%=radioInvioPosizione.ClientID %>")).attr("checked", true);
                $(document.getElementById("<%=txtInvioPosizione.ClientID %>")).focus();


                $(document.getElementById("<%=txtDataLavorazioneDal.ClientID%>")).datepicker("destroy");
                $(document.getElementById("<%=txtDataLavorazioneAl.ClientID%>")).datepicker("destroy");
                $(document.getElementById("<%=btnRicerca.ClientID%>")).attr('disabled', true);  //disabilita il pulsante btnRicerca
                $(document.getElementById("<%=btnAnnulla.ClientID%>")).attr('disabled', true);  //disabilita il pulsante btnRicerca
            }
            //nel RadioButton via codeBehind
            SwitchValidator('.offClass', false);                                            //Disabilita tutti i validatori
            //            SwitchValidator('.' + rb.getAttribute("EnableClass"), true);                    //Abilita i validatori con l'attributo specificato 
            //nel RadioButton via codeBehind
            rb.checked = true;                                                              //Seleziona il radioButton che ha scatenato l'evento
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
            document.getElementById("<%=txtRicercaPosizioneCriterio1.ClientID%>").value = '';
            document.getElementById("<%=ddlRicercaPosizioneCriterio1.ClientID %>").value = '';
            document.getElementById("<%=ddlRicercaPosizioneCriterio2.ClientID %>").value = '';
            document.getElementById("<%=txtDataLavorazioneDal.ClientID %>").value = '';
            document.getElementById("<%=txtDataLavorazioneAl.ClientID %>").value = '';
            return false;
        }
        
    </script>

    <div>
        <style type="text/css">
            input[disabled="disabled"], input.disabled, input[disabled]
            {
                background: #D3D3D3;
                color: #D3D3D3;
            }
            select[disabled="disabled"], select.disabled, select[disabled]
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
    </div>
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <asp:ValidationSummary runat="server" ID="validSummary" ValidationGroup="TrasmissioneECalcolo"
        Font-Size="Small" />
    <asp:Panel class="pnlElaborazionePosizione" runat="server" ID="panel" CssClass="search-grid__container">
        <div class="div_container search-grid" style="height: 45px; padding-top: 10px;">
            <table width="100%" class="search-grid__position search-grid__position--single">
                <tr>
                    <td class="radioButton">
                        <asp:RadioButton runat="server" ID="radioInvioPosizione" CssClass=" radioButton" />
                    </td>
                    <td class="Row1"  style="width: 20%; text-align:left">
                        <label>
                            Trasmissione singola posizione:</label>
                    </td>
                    <td class="field" style="width: 45%;">
                        <div runat="server" id="divTxtInvioPosizione" >
                            <asp:TextBox Width="235px" CssClass="tb8 txtUppercase offClass onClassInvioPosizione"
                                runat="server" ID="txtInvioPosizione" MaxLength="13" onblur="extractNumber(this,0,false);"
                                onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtInvioPosizione"
                                ErrorMessage="Numero domanda non valido" ValidationExpression="^[0-9]{13}$" runat="server"
                                Text="*" Display="Dynamic" ValidationGroup="TrasmissioneECalcolo" CssClass="offClass field-is-required  onClassInvioPosizione"
                                Enabled="false" />
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtInvioPosizione"
                                Enabled="false" ErrorMessage="Inserire il numero di domanda della posizione da inviare al calcolo"
                                Text="*" Display="Dynamic" ValidationGroup="TrasmissioneECalcolo" CssClass="offClass field-is-required  onClassInvioPosizione" />
                        </div>
                    </td>
                    <td style="width: 25%"></td>
                    <td style="width: 10%">
                        <asp:ImageButton ID="btnInvioPensione" ImageUrl="~/App_Themes/BlueINPS1/Images/arrow-right.gif"
                            CssClass=" onClassInvioPosizione" runat="server" OnClientClick="javascript:CheckValidator();"
                            ToolTip="invia posizione al calcolo" ValidationGroup="TrasmissioneECalcolo" CausesValidation="true" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="div_container search-grid" style="height: 120px; padding-top: 10px;">
            <table width="100%" class="search-grid__position search-grid__position--multiple">
                <tr>
                    <td class="radioButton">
                        <asp:RadioButton runat="server" ID="radioRicercaPosizioneDaTrasmettere" CssClass=" radioButton" />
                    </td>
                    <td class="Row1" align="left" style="text-align: right; width:22%;">
                        <label>
                            Ricerca posizione:</label>
                    </td>
                    
                    <td class="field" style=" width:45%;">
                        <div runat="server" id="divTxtRicercaPosizione">
                            <asp:TextBox Width="235px" CssClass="tb8 txtUppercase offClass onClassRicercaPosizione"
                                runat="server" ID="txtRicercaPosizioneCriterio1"></asp:TextBox>
                        </div>
                    </td>
                    <td class="field" style=" width:20%">
                        <asp:DropDownList runat="server" Width="180px" CssClass="tb8 txtUppercase offClass onClassRicercaPosizione"
                            ID="ddlRicercaPosizioneCriterio1">
                            <asp:ListItem Text="Categoria" Value="Categoria" />
                            <asp:ListItem Text="Sede" Value="Sede" Selected="True" />
                            <asp:ListItem Text="Titolare" Value="Titolare" />
                            <asp:ListItem Text="Numero Domanda" Value="NumeroDomanda" />
                            <asp:ListItem Text="Certificato" Value="Certificato" />
                            <asp:ListItem Text="Data Presentazione" Value="DataPresentazione" />
                            <asp:ListItem Text="Data Lavorazione" Value="DataLavorazione" />
                            <asp:ListItem Text="Periodo Giacenza" Value="PeriodoGiacenza" />
                        </asp:DropDownList>
                    </td>
                    <td style=" width:12%">
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td class="Row1" style="text-align: right;">
                        <label>
                            Dal:</label>
                    </td>
                    <td>
                        <table style="border-collapse: collapse; border-style: none; border-width: 0;" cellspacing="0"
                            cellpadding="0" border="0" width="100%" class="search-grid__position search-grid__position--date">
                            <tr>
                                <td style=" width:38%">
                                    <asp:TextBox runat="server" ID="txtDataLavorazioneDal" CssClass="tb8  offClass onClassRicercaPosizione"
                                        Width="80px"></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtDataLavorazioneDal"
                                        Display="Dynamic" ControlToValidate="txtDataLavorazioneDal" Enabled="true" ErrorMessage="Dal: Inserire la data in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationGroup="TrasmissioneECalcolo" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"></asp:RegularExpressionValidator>
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDataLavorazioneDal" Display="Dynamic"
                                        ErrorMessage="Data Lavorazione dal: Data inserita posteriore a quella odierna" Text="*" CssClass="field-is-required" ValidationGroup="TrasmissioneECalcolo"
                                        ID="customDataLavorazioneDal" ClientValidationFunction="checkDataPostOdiernaGGMMAAAA" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDataLavorazioneDal" Display="Dynamic"
                                        ErrorMessage="Data lavorazione dal: giorno non valido" Text="*" CssClass="field-is-required" ValidationGroup="TrasmissioneECalcolo"
                                        ID="customCheckGiornoDatalavorazioneDal" ClientValidationFunction="checkCorrettezzaGiorno" />    
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDataLavorazioneDal" Display="Dynamic"
                                        ErrorMessage="Dal: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="TrasmissioneECalcolo"
                                        ID="customCheckDataDal" ClientValidationFunction="checkCorrettezzaData" />                            

                                        
                                </td>
                                <td class="Row1" style=" width:8%">
                                    <label>
                                        Al:</label>
                                </td>
                                <td class="field" style=" text-align:left; width:40%">
                                    <asp:TextBox runat="server" ID="txtDataLavorazioneAl" CssClass="tb8 offClass onClassRicercaPosizione"
                                        Width="80px"></asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ID="validateDataLavorazioneAl"
                                        Display="Dynamic" ControlToValidate="txtDataLavorazioneAl" Enabled="true" ErrorMessage="Al: Inserire la data in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationGroup="TrasmissioneECalcolo" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"></asp:RegularExpressionValidator>
                                        <asp:CustomValidator runat="server" ControlToValidate="txtDataLavorazioneAl" Display="Dynamic"
                                        ErrorMessage="Data lavorazione al: giorno non valido" Text="*" CssClass="field-is-required" ValidationGroup="TrasmissioneECalcolo"
                                        ID="checkGiornoDataLavorazioneAl" ClientValidationFunction="checkCorrettezzaGiorno" />    
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDataLavorazioneAl" Display="Dynamic"
                                        ErrorMessage="Data Lavorazione al: Data inserita posteriore a quella odierna" Text="*" CssClass="field-is-required" ValidationGroup="TrasmissioneECalcolo"
                                        ID="customDataLavorazioneAl" ClientValidationFunction="checkDataPostOdiernaGGMMAAAA" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDataLavorazioneAl" Display="Dynamic"
                                        ErrorMessage="Al: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="TrasmissioneECalcolo"
                                        ID="customCheckDataAl" ClientValidationFunction="checkCorrettezzaData" />  
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="field" style=" width:20%">
                        <asp:DropDownList runat="server" Width="180px" CssClass="tb8 txtUppercase offClass onClassRicercaPosizione"
                            ID="ddlRicercaPosizioneCriterio2">
                            <asp:ListItem Text="Categoria" Value="Categoria" />
                            <asp:ListItem Text="Sede" Value="Sede" />
                            <asp:ListItem Text="Titolare" Value="Titolare" />
                            <asp:ListItem Text="Numero Domanda" Value="NumeroDomanda" />
                            <asp:ListItem Text="Certificato" Value="Certificato" />
                            <asp:ListItem Text="Data Presentazione" Value="DataPresentazione" />
                            <asp:ListItem Text="Data Lavorazione" Value="DataLavorazione" Selected="True" />
                            <asp:ListItem Text="Periodo Giacenza" Value="PeriodoGiacenza" />
                        </asp:DropDownList>
                    </td>
                    <td style="width: 12%">
                        <asp:Image ID="imgAggiungiCriterio" ImageUrl="~/App_Themes/BlueINPS1/Images/add24.png"
                            runat="server" ToolTip="Aggiungi Criterio" />
                    </td>
                </tr>
            </table>
            <br />
            <table width="100%" class="search-grid__position search-grid__position--actions">
                <tr>
                    <td style="text-align: right">
                        <asp:Button ID="btnRicerca" CssClass="onClassRicercaPosizione primary" runat="server"
                            Text="Ricerca" SkinID="btnAzione1" OnClick="btnRicerca_Click" OnClientClick="javascript:CheckValidator();"
                            ValidationGroup="TrasmissioneECalcolo" CausesValidation="true" />
                    </td>
                    <td style="text-align: left">
                        <asp:Button ID="btnAnnulla" runat="server" SkinID="btnAzione1" OnClientClick="javascript:return CleanFields();"
                            Text="Annulla" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:HiddenField runat="server" ID="HiddenSelectedTipoOperazione" />
</asp:Content>
