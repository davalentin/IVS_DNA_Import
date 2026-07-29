<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDelegato.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DelegatoTutore.UCDelegato" %>
    <%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />

<script type="text/javascript">

	var lblCFDelegato;

    $(document).ready(function() {
		
		lblCFDelegato = document.getElementById("<%=lblCFDelegato.ClientID%>");
        $(document.getElementById("<%=btnRicerca1Delegato.ClientID%>")).attr('disabled', true);
        $(document.getElementById("<%=btnRicerca2Delegato.ClientID%>")).attr('disabled', true);

        $('.offClass').attr('disabled', true);
        var doAction = false;
        var cssClass;
        var tipoRicerca = document.getElementById("<%=HiddenSelectedTipoRicercaDelegato.ClientID%>").value; //L'hidden field è valorizzato con il tipo di ricerca
        if (tipoRicerca == 'DatiAnagrafici') { //Nel caso di un postback riabilito il blocco precedentemente selezionato
            doAction = true;
            cssClass = '.onClassAnagraficaDelegato';
        }
        else if (tipoRicerca == 'CodiceFiscale') {
            doAction = true;
            cssClass = '.onClassCodiceFiscaleDelegato';
        }

        else { //nel caso del primo caricamento della pagina
            $('.offClass').val('');
            $('input:radio').attr('checked', false);
        }
        if (doAction) {
            $(cssClass).removeAttr('disabled');
            $(document.getElementById("<%=btnRicerca1Delegato.ClientID%>")).removeAttr('disabled');
            $(document.getElementById("<%=btnRicerca2Delegato.ClientID%>")).removeAttr('disabled');
            SwitchValidator(cssClass, true);
        }
    });

    function SetRadio_<%=this.ClientID %>(rb) {
        $('input:radio').attr('checked', false); //Disabilita tutti i radio button
        $('.offClass').attr('disabled', true); //Disabilita tutti gli oggetti con la class "offClass"
        $('.offClass').val(''); //Pulisce tutti i campi con la class "offClass"

        $(document.getElementById("<%=btnRicerca1Delegato.ClientID%>")).removeAttr('disabled'); //Abilita il pulsante btnRicerca
        $(document.getElementById("<%=btnRicerca2Delegato.ClientID%>")).removeAttr('disabled'); //Abilita il pulsante btnRicerca            
        $('.' + rb.getAttribute("EnableClass")).removeAttr('disabled'); //Abilita gli oggetti con l'attributo specificato
        if (rb.getAttribute("EnableClass") == "onClassAnagraficaDelegato") {
            $(document.getElementById("<%=radioAnagraficaDelegato.ClientID %>")).attr("checked", true);
            $(document.getElementById("<%=txtCognomeDelegato.ClientID %>")).focus();
            $(document.getElementById("<%=txtDataNascitaDelegato.ClientID%>")).datepicker({
                changeMonth: true,
                changeYear: true,
                changeDay: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                yearRange: 'c-50:' + 'c+0'
            });
            //$(document.getElementById("<%=txtDataNascitaDelegato.ClientID%>")).unmask();
            //$(document.getElementById("<%=txtDataNascitaDelegato.ClientID%>")).mask("99/99/9999");
            
            $(document.getElementById("ctl00_ContentPlaceHolder1_ucTutore_txtDataNascitaTutore")).datepicker("destroy");
            
        }
        else if (rb.getAttribute("EnableClass") == "onClassCodiceFiscaleDelegato") {
            $(document.getElementById("<%=radioCodiceFiscaleDelegato.ClientID %>")).attr("checked", true);
            $(document.getElementById("<%=txtCodiceFiscaleDelegato.ClientID %>")).focus();
            $(document.getElementById("<%=txtDataNascitaDelegato.ClientID%>")).datepicker("destroy");
            $(document.getElementById("ctl00_ContentPlaceHolder1_ucTutore_txtDataNascitaTutore")).datepicker("destroy");
        }

        //nel RadioButton via codeBehind
        SwitchValidator('.offClass', false); //Disabilita tutti i validatori
        // SwitchValidator('.' + rb.getAttribute("EnableClass"), true); //Abilita i validatori con l'attributo specificato
        //nel RadioButton via codeBehind
        rb.checked = true; //Seleziona il radioButton che ha scatenato l'evento
    }


    function CleanFields() {
        document.getElementById("<%=ddlCodiceDelegato.ClientID %>").value = '';
        document.getElementById("<%=txtTelDelegato.ClientID %>").value = '';
        document.getElementById("<%=txtCellDelegato.ClientID %>").value = '';
        document.getElementById("<%=txtEmailDelegato.ClientID %>").value = '';
        return false;
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

    function SetCodiceFiscaleDelegato() {
        document.getElementById("<%=hdnCodiceFiscaleDelegato.ClientID %>").value = document.getElementById("<%=txtCodiceFiscaleDelegato.ClientID %>").value;
    }
</script>

<style type="text/css">
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
<asp:Panel runat="server" ID="pnlDelegato">
    <asp:Panel runat="server" ID="pnlDelegatoRicerca">
        <div class="deleghe-tutele-searcharea" style="display: none">
            <p class="deleghe-tutele-searcharea__title">
                Seleziona una modalità di ricerca tra <b>Codice fiscale</b> oppure <b>Cognome, Nome e Data di Nascita</b>
            </p>
        </div>
        <table class="tabellaFormattazione grid grid-specific-1">
            <tr>
                <td colspan="5" class="shift-full-grid">
                    <asp:ValidationSummary runat="server" ID="validSummarySchedaDelegato" ValidationGroup="RicercaPerCodiceFiscale"
                        Font-Size="Small" CssClass="errorBox" />
                    <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="RicercaPerAnagrafica"
                        Font-Size="Small" CssClass="errorBox" />
                </td>
            </tr>
            <tr>
                <td colspan="5" style="height: 5px;" class="none">
                </td>
            </tr>
            <tr>
                <td class="radioButton">
                    <asp:RadioButton runat="server" ID="radioCodiceFiscaleDelegato" CssClass="CodiceFiscale radioButton"
                        TabIndex="1" />
                </td>
                <td class="Row1">
                    <label>
                        Codice fiscale:</label>
                </td>
                <td colspan="1" class="field shift-right-full-grid">
                    <div runat="server" id="divTxtCodiceFiscaleDelegato" class="full-width">
                        <asp:TextBox Style="text-align: left" runat="server" ID="txtCodiceFiscaleDelegato"
                            Width="37%" CssClass="txtUppercase tb8 offClass onClassCodiceFiscaleDelegato"
                            TabIndex="2" MaxLength="16"></asp:TextBox>
                        <asp:CustomValidator ValidateEmptyText="True" ControlToValidate="txtCodiceFiscaleDelegato"
                            EnableClientScript="true" runat="server" Text="*" CssClass="field-is-required" Display="None" ValidationGroup="RicercaPerCodiceFiscale"
                            ID="txtCodiceFiscaleDelegato_CV" ClientValidationFunction="validateCodiceFiscale"
                            ErrorMessage="Codice fiscale dell'incaricato alla delega non valido" />
                        <asp:ImageButton ValidationGroup="RicercaPerCodiceFiscale" CausesValidation="true"
                            ImageAlign="AbsMiddle" runat="server" ID="btnRicerca1Delegato" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/search24.png"
                            AlternateText="Cerca" ToolTip="Cerca" OnClientClick="SetCodiceFiscaleDelegato(); if(Page_ClientValidate('RicercaPerCodiceFiscale')){aspnetForm.target ='_self'; BlockUI();}"
                            OnClick="RicercaDelegato_Click" CssClass="offClass" />
                        <div class="deltut-cta-label" style="display: none">Cerca</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="5" style="height: 15px;" class="shift-full-grid">
                </td>
            </tr>
            <tr>
                <td class="radioButton" style="width: 3%;">
                    <asp:RadioButton runat="server" ID="radioAnagraficaDelegato" CssClass="Anagrafica radioButton"
                        TabIndex="1" />
                </td>
                <td class="Row1" style="width: 16%;">
                    <label>
                        Cognome:</label>
                </td>
                <td style="width: 36%;" class="field">
                    <div runat="server" id="divTxtCognomeDelegato" class="full-width">
                        <asp:TextBox Style="text-align: left" runat="server" ID="txtCognomeDelegato" Width="83%"
                            CssClass="txtUppercase tb8 offClass onClassAnagraficaDelegato " TabIndex="2"
                            MaxLength="50"></asp:TextBox>
                        <asp:CustomValidator ValidateEmptyText="True" EnableClientScript="true" runat="server"
                            Display="None" Text="*" CssClass="field-is-required" ControlToValidate="txtCognomeDelegato" ValidationGroup="RicercaPerAnagrafica"
                            ID="txtCognomeDelegato_CV" ClientValidationFunction="validateCognomeNome" ErrorMessage="Cognome dell'incaricato alla delega non valido: inserire almeno 3 caratteri">
                        </asp:CustomValidator>
                    </div>
                </td>
                <td style="width: 15%;" class="Row1">
                    <label style="text-align: left; width: 85%">
                        Nome:</label>
                </td>
                <td class="field" align="left" style="width: 29%;">
                    <div class="p-relative full-width">
                        <asp:TextBox Style="text-align: left" runat="server" ID="txtNomeDelegato" Width="37%"
                            CssClass="txtUppercase tb8 offClass onClassAnagraficaDelegato" MaxLength="50"
                            TabIndex="3"></asp:TextBox>
                        <asp:CustomValidator ValidateEmptyText="True" EnableClientScript="true" runat="server"
                            Display="None" Text="*" CssClass="field-is-required" ControlToValidate="txtNomeDelegato" ValidationGroup="RicercaPerAnagrafica"
                            ID="txtNomeDelegato_CV" ClientValidationFunction="validateCognomeNome" ErrorMessage="Nome dell'incaricato alla delega non valido: inserire almeno 3 caratteri">
                        </asp:CustomValidator>
                        <asp:ImageButton ValidationGroup="RicercaPerAnagrafica" CausesValidation="true" ImageAlign="AbsMiddle"
                            runat="server" ID="btnRicerca2Delegato" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/search24.png" CssClass="offClass"
                            AlternateText="Cerca" ToolTip="Cerca" OnClick="RicercaDelegato_Click" OnClientClick="if(Page_ClientValidate('RicercaPerAnagrafica')){aspnetForm.target ='_self'; BlockUI();}" />
                        <div class="deltut-cta-label" style="display: none">Cerca</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="width: 10px;">
                </td>
                <td class="Row1 grid-pos-2">
                    <label>
                        Data di nascita:</label>
                </td>
                <td colspan="1" align="left" class="field  grid-pos-3">
                    <asp:TextBox ID="txtDataNascitaDelegato" CssClass="tb8 txtUppercase offClass onClassAnagraficaDelegato dateGGmmAAAA"
                        runat="server" Text="gg/mm/aaaa" Width="83%" MaxLength="10" TabIndex="4"></asp:TextBox>
                    <asp:CustomValidator runat="server" ControlToValidate="txtDataNascitaDelegato" Display="Dynamic"
                        ErrorMessage="Data Nascita: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="RicercaPerAnagrafica"
                        ID="customCheckDataDataNascitaDelegato" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <hr style="margin-left:15px;margin-right:15px; margin-top:24px" />
    <div runat="server" id="datiOmonimi" visible="false">
        <table class="tabellaFormattazione no-grid">
            <tr>
                <td align="center">
                    <asp:GridView ID="gvSinonimiDelegato" runat="server" BorderWidth="1" BorderColor="Black"
                        AutoGenerateColumns="false" Visible="true" Width="100% " SkinID="grdElenco1"
                        OnRowCommand="ScegliSinonimo_onRowCommand" AllowPaging="true" PageSize="10" OnPageIndexChanging="gvSinonimiDelegato_onPageIndexChanging"
                        AllowSorting="true" OnSorting="gvSinonimiDelegato_onSorting" OnRowCreated="gvSinonimiDelegato_RowCreated"
                        CssClass="intestazioneTabella intestazioneTabella--sorting intestazioneTabella__with-pagination"  PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessuna posizione trovata per i criteri inseriti."
                                    SkinID="lblNoData" Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:BoundField HeaderText="CodiceFiscale" DataField="CodiceFiscale" Visible="true"
                                ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink intestazioneTabella__heading intestazioneTabella__heading--sort"
                                ItemStyle-CssClass="TblRecordset3" SortExpression="CodiceFiscale" />
                            <asp:BoundField HeaderText="Cognome" DataField="Cognome" Visible="true" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-Width="21%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink intestazioneTabella__heading intestazioneTabella__heading--sort"
                                ItemStyle-CssClass="TblRecordset3" SortExpression="Cognome" />
                            <asp:BoundField HeaderText="Nome" DataField="Nome" Visible="true" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-Width="16%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink intestazioneTabella__heading intestazioneTabella__heading--sort"
                                ItemStyle-CssClass="TblRecordset3" SortExpression="Nome" />
                            <asp:BoundField HeaderText="DataNascita" DataField="DataNascita" Visible="true" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-Width="16%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink intestazioneTabella__heading intestazioneTabella__heading--sort"
                                ItemStyle-CssClass="TblRecordset3" DataFormatString="{0:dd/MM/yyyy}" SortExpression="DataNascita" />
                            <asp:TemplateField HeaderText="Operazione" ItemStyle-Width="26%" HeaderStyle-CssClass="intestazioneTabella Row1 intestazioneTabella__heading"
                                ControlStyle-CssClass="pulsante1 tertiary viewIconOnly" ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Button runat="server" ID="btnRicerca" Text="Seleziona soggetto" CommandName="CercaPosizioni" CssClass="tertiary viewIconOnly" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView> 
                </td>
            </tr>
         </table>
    </div>
    <div runat="server" visible="true" id="divDatiDelegato">
        <table class="tabellaFormattazione  grid grid-specific-2">
            <caption style="display: none;">Risultato della ricerca</caption>
            <tr>
                <td class="none">
                </td>
                <td class="Row1 grid-row-8">
                    <label>Codice Delega:</label>
                </td>
                <td class="field full-grid grid-row-8" colspan="3">
                    <asp:DropDownList runat="server" CssClass="tb8 txtUppercase" TabIndex="7" 
                        ID="ddlCodiceDelegato" Enabled="false" Width="42%">
                    </asp:DropDownList>
                    <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required" ValidationGroup="UCDelegatoTutore" 
                        ID="ddlCodiceDelegato_CV" ClientValidationFunction="validateDropDownList" ErrorMessage="Scegliere il codice delega" />
                </td>
            </tr>
            <tr>
                <td colspan="5" style="height:15px;"  class="none""></td>
            </tr>
           <tr>
                <td style="width:5px;" class="none">
                </td>
                <td class="Row1">
                    <label>Codice Fiscale:</label>
                </td>
                <td class="field" colspan="3">
                    <asp:Label runat="server" ID="lblCFDelegato" Width="175px" Enabled="true" CssClass="txtUppercase "></asp:Label>
                </td>
            </tr>
           <tr>
               <td style="width:5px;"  class="none"></td>
                <td class="Row1" style="width:19%;">
                    <label>Cognome:</label>
                </td>
                <td class="Row1" style="width:31%;">
                    <asp:Label runat="server" ID="lblCognomeDelegato" CssClass="txtUppercase"></asp:Label>
                </td>
                <td class="Row1" style="width:19%;">
                    <label>Nome:</label>
                </td>
                <td class="Row1" style="width:31%;">
                    <asp:Label runat="server" ID="lblNomeDelegato" CssClass="txtUppercase"></asp:Label>
                </td>
            </tr>
            <%--            <tr>
                <td>
                </td>
                <td class="Row1">
                    <label>
                        Cognome Acquisito:
                    </label>
                </td>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblCognomeAcquisitoDelegato"></asp:Label>
                </td>
            </tr>
--%>
            <tr>
                <td style="width:5px;" class="none">
                </td>
                <td class="Row1">
                    <label>Sesso:</label>
                </td>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblSessoDelegato" CssClass="txtUppercase"></asp:Label>
                </td>
                <td class="Row1">
                    <label>Data di Nascita:</label>
                </td>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblDataNascitaDelegato" CssClass="txtUppercase"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width:5px;" class="none">
                </td>
                <td class="Row1">
                    <label>Comune Nascita:</label>
                </td>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblComuneNascitaDelegato"  CssClass="txtUppercase"></asp:Label>
                </td>
                <td class="Row1">
                    <label>Provincia Nascita:</label>
                </td>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblProvinciaNascitaDelegato" CssClass="txtUppercase"></asp:Label>
                </td>
            </tr>
            <tr>
                 <td style="width:5px;" class="none">
                </td>
                <td class="Row1">
                    <label>Indirizzo:</label>
                </td>
                <td class="field">
                    <asp:Label runat="server" ID="lblIndirizzoDelegato" CssClass="txtUppercase"></asp:Label>
                </td>
                <td class="Row1">
                    <label>Numero:</label>
                </td>
                <td class="field">
                    <asp:Label runat="server" ID="lblNCivicoDelegato" CssClass="txtUppercase"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width:5px;" class="none">
                </td>
                <td class="Row1">
                    <label>CAP:</label>
                </td>
                <td class="field">
                    <asp:Label runat="server" ID="lblCapDelegato" CssClass="txtUppercase"></asp:Label>
                </td>
                <td class="Row1">
                    <label>Comune Residenza:</label>
                </td>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblComuneResidenzaDelegato" CssClass="txtUppercase"></asp:Label>
                </td>
            </tr>
            <tr>
                 <td style="width:5px;" class="none">
                </td>
                <td class="Row1">
                    <label>Provincia:</label>
                </td>
                <td class="field">
                    <asp:Label runat="server" ID="lblProvinciaDelegato" CssClass="txtUppercase"></asp:Label>
                </td>
                <asp:Panel runat="server" ID="pnlDataMorte" Visible="false">
                <td class="Row1">
                    <asp:Label runat="server" ForeColor="Red">Data decesso:</asp:Label>
                </td>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblDataMorte" CssClass="txtUppercase" ForeColor="Red"></asp:Label>
                </td>
                </asp:Panel>
            </tr>
            <tr>
                <td style="width:5px;" class="none">
                </td>
                <td class="Row1">
                    <label>Telefono:</label>
                </td>
                <td class="field"  colspan="3">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtTelDelegato" Width="35%"
                        CssClass="txtUppercase tb8" Enabled="false" MaxLength="18" TabIndex="8" onblur="extractPhoneChar(this);"
                        onkeyup="extractPhoneChar(this);" 
                        onkeypress="return blockNonPhone(this, event);"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validateTxtTel" ControlToValidate="txtTelDelegato"
                        ErrorMessage="Numero di telefono dell'incaricato alla delega non valido (Formato corretto: +12/3456789)" ValidationExpression="^\+?[0-9]+\/?[0-9]+|^\+?[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="None" ValidationGroup="UCDelegatoTutore" Enabled="true" />
                    <!-- Controllo campo obbligatorio -->
                    <%--<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtTelDelegato"
                        Enabled="true" ErrorMessage="Inserire un numero di telefono dell'incaricato alla delega" Text="*" CssClass="field-is-required" Display="None"
                        ValidationGroup="UCDelegatoTutore" />--%>
                </td>
                </tr>
            <tr>
                 <td style="width:5px;" class="none">
                    </td>
                <td class="Row1">
                    <label>Cellulare:</label>
                </td>
                <td class="field" colspan="3">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtCellDelegato" Width="35%"
                        CssClass="txtUppercase tb8" Enabled="false" MaxLength="18" TabIndex="9" onblur="extractPhoneChar(this);"
                        onkeyup="extractPhoneChar(this);" 
                        onkeypress="return blockNonPhone(this, event);"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validateTxtCell" ControlToValidate="txtCellDelegato"
                        ErrorMessage="Numero di cellulare dell'incaricato alla delega non valido (Formato corretto: +12/3456789)" ValidationExpression="^\+?[0-9]+\/?[0-9]+|^\+?[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="None" ValidationGroup="UCDelegatoTutore" Enabled="true" />
                </td>
            </tr>
            <tr>
               <td style="width:5px;" class="none">
                </td>
                <td class="Row1">
                    <label>Email:</label>
                </td>
                <td class="field" colspan="3">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtEmailDelegato" Width="35%"
                        Enabled="false" CssClass="txtUppercase tb8" MaxLength="50" TabIndex="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validateTxtEmail" ControlToValidate="txtEmailDelegato"
                        ErrorMessage="Indirizzo Email dell'incaricato alla delega non valido" ValidationExpression="^[a-zA-Z0-9._%-]+@[a-zA-Z.-]+\.[a-zA-Z]{2,4}$"
                        runat="server" Text="*" CssClass="field-is-required" Display="None" ValidationGroup="UCDelegatoTutore" Enabled="true" />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="hdnCodiceFiscaleDelegato" />
    <asp:HiddenField runat="server" ID="HiddenSelectedTipoRicercaDelegato" />
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaTabDelegato" runat="server" SkinID="btnAzione1" Enabled="true" Text="Salva dati Delega" Width="150px" 
                        onclick="btnSalvaTabDelegato_Click" OnClientClick="if(Page_ClientValidate('UCDelegatoTutore') && checkCFDelegato()){aspnetForm.target ='_self'; BlockUI();} else return false;" 
                        ValidationGroup="UCDelegatoTutore" CausesValidation="true" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaTabDelegato" runat="server" SkinID="btnAzione1" Enabled="true" Text="Elimina dati Delega" Width="150px" 
                        onclick="btnEliminaTabDelegato_Click" OnClientClick="if (!window.confirm('Sei sicuro di voler eliminare i dati Delega?')) return false; else BlockUI();" 
                        ValidationGroup="UCDelegatoTutore" CausesValidation="true" CssClass="ghost-delete" />
                </td>
                <%--<td style="text-align: center">
                    <asp:Button ID="btnAnnulla" runat="server" SkinID="btnAzione1" OnClientClick="javascript:return CleanFields();"
                        Enabled="true" Text="Pulisci" Width="100px" CausesValidation="true" ValidationGroup="" />
                </td>--%>
            </tr>
        </table>
    </div>
</asp:Panel>
