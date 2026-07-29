<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCTutore.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DelegatoTutore.UCTutore" %>

    <%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />



<script type="text/javascript">
	var lblCFTutore;

    $(document).ready(function() {
        
        lblCFTutore = document.getElementById("<%=lblCFTutore.ClientID%>");
        $(document.getElementById("<%=btnRicerca1Tutore.ClientID%>")).attr('disabled', true);
        $(document.getElementById("<%=btnRicerca2Tutore.ClientID%>")).attr('disabled', true);


        $('.offClass').attr('disabled', true);
        var doAction = false;
        var cssClass;
        var tipoRicerca = document.getElementById("<%=HiddenSelectedTipoRicercaTutore.ClientID%>").value; //L'hidden field è valorizzato con il tipo di ricerca
        if (tipoRicerca == 'DatiAnagrafici') { //Nel caso di un postback riabilito il blocco precedentemente selezionato
            doAction = true;
            cssClass = '.onClassAnagraficaTutore';
        }
        else if (tipoRicerca == 'CodiceFiscale') {
            doAction = true;
            cssClass = '.onClassCodiceFiscaleTutore';
        }

        else { //nel caso del primo caricamento della pagina
            $('.offClass').val('');
            $('input:radio').attr('checked', false);
        }
        if (doAction) {
            $(cssClass).removeAttr('disabled');
            $(document.getElementById("<%=btnRicerca1Tutore.ClientID%>")).removeAttr('disabled');
            $(document.getElementById("<%=btnRicerca2Tutore.ClientID%>")).removeAttr('disabled');
            SwitchValidator(cssClass, true);
        }
        
        CheckAmmSost();
    });

    function SetRadio_<%=this.ClientID %>(rb) {
    
        $('input:radio').attr('checked', false); //Disabilita tutti i radio button
        $('.offClass').attr('disabled', true); //Disabilita tutti gli oggetti con la class "offClass"
        $('.offClass').val(''); //Pulisce tutti i campi con la class "offClass"


        $(document.getElementById("<%=btnRicerca1Tutore.ClientID%>")).removeAttr('disabled'); //Abilita il pulsante btnRicerca
        $(document.getElementById("<%=btnRicerca2Tutore.ClientID%>")).removeAttr('disabled'); //Abilita il pulsante btnRicerca            
        $('.' + rb.getAttribute("EnableClass")).removeAttr('disabled'); //Abilita gli oggetti con l'attributo specificato
        if (rb.getAttribute("EnableClass") == "onClassAnagraficaTutore") {
            $(document.getElementById("<%=radioAnagraficaTutore.ClientID %>")).attr("checked", true);
            $(document.getElementById("<%=txtCognomeTutore.ClientID %>")).focus();
            $(document.getElementById("<%=txtDataNascitaTutore.ClientID%>")).datepicker({
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
            //$(document.getElementById("<%=txtDataNascitaTutore.ClientID%>")).unmask();
            //$(document.getElementById("<%=txtDataNascitaTutore.ClientID%>")).mask("99/99/9999");
            
            $(document.getElementById("ctl00_ContentPlaceHolder1_ucDelegato_txtDataNascitaDelegato")).datepicker("destroy");
            
        }
        else if (rb.getAttribute("EnableClass") == "onClassCodiceFiscaleTutore") {
            $(document.getElementById("<%=radioCodiceFiscaleTutore.ClientID %>")).attr("checked", true);
            $(document.getElementById("<%=txtCodiceFiscaleTutore.ClientID %>")).focus();
            $(document.getElementById("<%=txtDataNascitaTutore.ClientID%>")).datepicker("destroy");
            $(document.getElementById("ctl00_ContentPlaceHolder1_ucDelegato_txtDataNascitaDelegato")).datepicker("destroy");
        }

        //nel RadioButton via codeBehind
        SwitchValidator('.offClass', false); //Disabilita tutti i validatori
        // SwitchValidator('.' + rb.getAttribute("EnableClass"), true); //Abilita i validatori con l'attributo specificato
        //nel RadioButton via codeBehind
        rb.checked = true; //Seleziona il radioButton che ha scatenato l'evento
    }

    function CleanFields2() {
        document.getElementById("<%=ddlCodiceTutore.ClientID %>").value = '';
        document.getElementById("<%=txtTelTutore.ClientID %>").value = '';
        document.getElementById("<%=txtCellTutore.ClientID %>").value = '';
        document.getElementById("<%=txtEmailTutore.ClientID %>").value = '';
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
    
    function SetCodiceFiscaleTutore() {
        document.getElementById("<%=hdnCodiceFiscaleTutore.ClientID %>").value = document.getElementById("<%=txtCodiceFiscaleTutore.ClientID %>").value;
    }
    
    function CheckAmmSost()
    {
        var controlName = document.getElementById("<%=ddlCodiceTutore.ClientID %>");
        if(controlName != null)
        {
            if(controlName.value == 'A')
            {
                document.getElementById("rigaCessValAmmSost").style.display = 'table-row';
            }
            else
            {
                document.getElementById("rigaCessValAmmSost").style.display = 'none';
                document.getElementById("<%=txtCessValAmmSost.ClientID %>").value = "MM/AAAA";
            }
        }
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
<%--<asp:ValidationSummary runat="server" ID="validateSummary" ValidationGroup="UCDelegatoTutore"
    Font-Size="Small" CssClass="errorBox" />
--%>    
<asp:Panel runat="server" ID="pnlTutore">
    <asp:Panel runat="server" ID="pnlTutoreRicerca">
        <div class="deleghe-tutele-searcharea" style="display: none">
            <p class="deleghe-tutele-searcharea__title">
                Seleziona una modalità di ricerca tra <b>Codice fiscale</b> oppure <b>Cognome, Nome e Data di Nascita</b>
            </p>
        </div>
        <table class="tabellaFormattazione grid grid-specific-1">
            <tr>
                <td colspan="5" class="shift-full-grid">
                    <asp:ValidationSummary runat="server" ID="validSummarySchedaTutore" ValidationGroup="SchedaTutore"
                        Font-Size="Small" CssClass="errorBox" />
                </td>
            </tr>
            <tr>
                <td colspan="5" style="height: 5px;" class="none">
                </td>
            </tr>
            <tr>
                <td class="radioButton">
                    <asp:RadioButton runat="server" ID="radioCodiceFiscaleTutore" CssClass="CodiceFiscale radioButton"
                        TabIndex="1" />
                </td>
                <td class="Row1">
                    <label>
                        Codice fiscale:</label>
                </td>
                <td colspan="3" class="field shift-right-full-grid">
                    <div runat="server" id="divTxtCodiceFiscaleTutore" class="full-width">
                        <asp:TextBox Style="text-align: left" runat="server" ID="txtCodiceFiscaleTutore"
                            Width="37%" CssClass="txtUppercase tb8 offClass onClassCodiceFiscaleTutore" TabIndex="2"
                            MaxLength="16"></asp:TextBox>
                        <asp:CustomValidator ValidateEmptyText="True" ControlToValidate="txtCodiceFiscaleTutore"
                            EnableClientScript="true" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="SchedaTutore"
                            ID="txtCodiceFiscaleTutore_CV" ClientValidationFunction="validateCodiceFiscale"
                            ErrorMessage="Codice fiscale dell'incaricato alla tutela non valido" />
                        <%--                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ControlToValidate="txtCodiceFiscaleTutore"
                            ErrorMessage="Codice fiscale dell'incaricato alla tutela non valido" ValidationExpression="^[A-Za-z]{6}[0-9LMNPQRSTUV]{2}[A-Za-z]{1}[0-9LMNPQRSTUV]{2}[A-Za-z]{1}[0-9LMNPQRSTUV]{3}[A -Za-z]{1}$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCDelegatoTutore" CssClass="offClass  onClassCodiceFiscaleTutore"
                            Enabled="true" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtEmailTutore"
                            ErrorMessage="Indirizzo Email dell'incaricato alla tutela non valido" ValidationExpression="^[a-zA-Z0-9._%-]+@[a-zA-Z.-]+\.[a-zA-Z]{2,4}$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCDelegatoTutore" Enabled="true" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator13" ControlToValidate="txtCodiceFiscaleTutore"
                            Enabled="false" ErrorMessage="Inserire un codice fiscale dell'incaricato alla tutela" Text="*" CssClass="field-is-required" Display="Dynamic"
                            ValidationGroup="UCDelegatoTutore" CssClass="offClass  onClassCodiceFiscaleTutore" />
    --%>
                        <asp:ImageButton ValidationGroup="SchedaTutore" CausesValidation="true" ImageAlign="AbsMiddle"
                            runat="server" ID="btnRicerca1Tutore" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/search24.png"
                            AlternateText="Cerca" ToolTip="Cerca" OnClientClick="javascript: SetCodiceFiscaleTutore(); if(Page_ClientValidate('SchedaTutore')){aspnetForm.target ='_self'; BlockUI();}"
                            OnClick="RicercaTutore_Click" />
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
                    <asp:RadioButton runat="server" ID="radioAnagraficaTutore" CssClass="Anagrafica radioButton"
                        TabIndex="1" />
                </td>
                <td class="Row1" style="width: 16%;">
                    <label>
                        Cognome:</label>
                </td>
                <td style="width: 36%;" class="field">
                    <div runat="server" id="divTxtCognomeTutore" class="full-width">
                        <asp:TextBox Style="text-align: left" runat="server" ID="txtCognomeTutore" Width="83%"
                            CssClass="txtUppercase tb8 offClass onClassAnagraficaTutore " TabIndex="2" MaxLength="50"></asp:TextBox>
                        <asp:CustomValidator ValidateEmptyText="True" EnableClientScript="true" runat="server"
                            Display="None" Text="*" CssClass="field-is-required" ControlToValidate="txtCognomeTutore" ValidationGroup="SchedaTutore"
                            ID="txtCognomeTutore_CV" ClientValidationFunction="validateCognomeNome" ErrorMessage="Cognome dell'incaricato alla tutela non valido: inserire almeno 3 caratteri">
                        </asp:CustomValidator>
                        <%--                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtCognomeTutore"
                            ErrorMessage="Cognome dell'incaricato alla tutela non valido" ValidationExpression="^[\x20a-zA-Z ']+$" runat="server"
                            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCDelegatoTutore" CssClass="offClass  onClassAnagraficaTutore"
                            Enabled="false" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtCognomeTutore"
                            Enabled="false" ErrorMessage="Inserire un cognome dell'incaricato alla tutela" Text="*" CssClass="field-is-required" Display="Dynamic"
                            ValidationGroup="UCDelegatoTutore" CssClass="offClass  onClassAnagraficaTutore" />
    --%>
                    </div>
                </td>
                <td style="width: 15%;" class="Row1">
                    <label style="text-align: left; width: 85%">
                        Nome:</label>
                </td>
                <td class="field" align="left" style="width: 29%;">
                    <div class="p-relative full-width">
                        <asp:TextBox Style="text-align: left" runat="server" ID="txtNomeTutore" Width="37%"
                            CssClass="txtUppercase tb8 offClass  onClassAnagraficaTutore" MaxLength="50"
                            TabIndex="3"></asp:TextBox>
                        <asp:CustomValidator ValidateEmptyText="True" EnableClientScript="true" runat="server"
                            Display="None" Text="*" CssClass="field-is-required" ControlToValidate="txtNomeTutore" ValidationGroup="SchedaTutore"
                            ID="txtNomeTutore_CV" ClientValidationFunction="validateCognomeNome" ErrorMessage="Nome dell'incaricato alla tutela non valido: inserire almeno 3 caratteri">
                        </asp:CustomValidator>
                        <asp:ImageButton ValidationGroup="SchedaTutore" CausesValidation="true" ImageAlign="AbsMiddle"
                            runat="server" ID="btnRicerca2Tutore" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/search24.png"
                            AlternateText="Cerca" ToolTip="Cerca" OnClick="RicercaTutore_Click" OnClientClick="if(Page_ClientValidate('SchedaTutore')){aspnetForm.target ='_self'; BlockUI();}" />
                        <div class="deltut-cta-label" style="display: none">Cerca</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="width: 10px;">
                </td>
                <td class="Row1 grid-pos-2">
                    <label>
                        Data Nascita:</label>
                </td>
                <td colspan="3" align="left" class="field  grid-pos-3">
                    <asp:TextBox ID="txtDataNascitaTutore" CssClass="tb8 txtUppercase offClass onClassAnagraficaTutore dateGGmmAAAA"
                        runat="server" Text="gg/mm/aaaa" Width="60%" MaxLength="10" TabIndex="4"></asp:TextBox>
                    <asp:CustomValidator runat="server" ControlToValidate="txtDataNascitaTutore" Display="Dynamic"
                        ErrorMessage="Data Nascita: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="SchedaTutore"
                        ID="customCheckDataDataNascitaTutore" ClientValidationFunction="checkCorrettezzaData" />
                    <%--                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtNomeTutore"
                        ErrorMessage="Nome dell'incaricato alla tutela non valido" ValidationExpression="^[\x20a-zA-Z ']+$" runat="server"
                        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCDelegatoTutore" CssClass="offClass  onClassAnagraficaTutore"
                        Enabled="false" />
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="txtNomeTutore"
                        Enabled="false" ErrorMessage="Inserire un nome dell'incaricato alla tutela" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCDelegatoTutore"
                        CssClass="offClass  onClassAnagraficaTutore" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtNomeTutore" Display="Dynamic"
                        ErrorMessage="Nome dell'incaricato alla tutela: il campo deve essere lungo almeno tre caratteri" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCDelegatoTutore" ID="customCheckNome" ClientValidationFunction="checkLunghezzaNome" />
    --%>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <hr style="margin-left:15px;margin-right:15px; margin-top:24px" />
    <div runat="server" id="datiOmonimiTutore" visible="false">
        <table class="tabellaFormattazione no-grid">
            <tr>
                <td align="center">
                    <asp:GridView ID="gvSinonimiTutore" runat="server" BorderWidth="1" BorderColor="Black"
                        AutoGenerateColumns="false" Visible="true" Width="100% " SkinID="grdElenco1"
                        OnRowCommand="ScegliSinonimo_onRowCommand" AllowPaging="true" PageSize="10" OnPageIndexChanging="gvSinonimiTutore_onPageIndexChanging"
                        AllowSorting="true" OnSorting="gvSinonimiTutore_onSorting" OnRowCreated="gvSinonimiTutore_RowCreated"
                        CssClass="intestazioneTabella intestazioneTabella--sorting intestazioneTabella__with-pagination" PagerStyle-CssClass="default-pagination-tables">
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
                                    <asp:Button runat="server" ID="btnRicerca" Text="Seleziona soggetto" CommandName="CercaPosizioni" CssClass="tertiary" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <div runat="server" visible="true" id="divDatiTutore">
        <table class="tabellaFormattazione grid grid-specific-2">
            <caption style="display: none;">Risultato della ricerca</caption>
            <tr>
                <td class="none">
                </td>
                <td class="Row1 grid-row-9">
                    <label>Codice Tutela:</label>
                </td>
                <td class="field grid-row-9" colspan="3">
                    <asp:DropDownList runat="server" CssClass="tb8 txtUppercase" TabIndex="5" ID="ddlCodiceTutore" Enabled="false" Width="42%" OnChange="CheckAmmSost()">                       
                    </asp:DropDownList>
                    <asp:CustomValidator EnableClientScript="true" runat="server" Display="None" Text="*" CssClass="field-is-required"
                    ValidationGroup="UCDelegatoTutore" ID="ddlCodiceTutore_CV" ClientValidationFunction="validateDropDownList"
                    ErrorMessage="Scegliere il codice tutela"/>
                </td>
            </tr>
            <tr id="rigaCessValAmmSost">
                <td style="width:5px;" class="none">
                </td>
                <td class="Row1 grid-row-9">
                    <label>
                        Cess.Val.Amm.Sost.:</label>
                </td>
                <td class="field grid-row-9" colspan="3">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtCessValAmmSost"
                    Width="95px" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="6" Text="MM/AAAA" MaxLength="7"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateCessValAmmSost" ControlToValidate="txtCessValAmmSost"
                    Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per Cess.Val.Amm.Sost."
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCDelegatoTutore" Text="*" CssClass="field-is-required" />
                <asp:CustomValidator runat="server" ControlToValidate="txtCessValAmmSost" Display="Dynamic"
                    ErrorMessage="Cess.Val.Amm.Sost.: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCDelegatoTutore"
                    ID="customCheckDataCessValAmmSost" ClientValidationFunction="checkCorrettezzaData" />  
                </td>
            </tr>    
            <tr>
                <td colspan="5" style="height:15px;" class="none"></td>
            </tr>
            <tr>
                <td style="width:5px;" class="none">
                </td>
                <td class="Row1">
                    <label>
                        Codice Fiscale:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:Label runat="server" ID="lblCFTutore" Width="175px" Enabled="true" CssClass="txtUppercase"></asp:Label>
                </td>
            </tr>        
            <tr>
                <td style="width:5px;" class="none"></td>
                <td class="Row1" style="width:19%;">
                    <label>
                        Cognome:
                    </label>
                </td>
                <td class="Row1" style="width:31%;">
                    <asp:Label runat="server" ID="lblCognomeTutore" CssClass="txtUppercase"></asp:Label>
                </td>
                <td class="Row1" style="width:19%;">
                    <label>Nome:</label>
                </td>
                <td class="Row1" style="width:31%;">
                    <asp:Label runat="server" ID="lblNomeTutore" CssClass="txtUppercase"></asp:Label>
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
                    <asp:Label runat="server" ID="lblCognomeAcquisitoTutore"></asp:Label>
                </td>
            </tr>--%>
            <tr>
                <td style="width:5px;" class="none">
                </td>
                <td class="Row1">
                    <label>Sesso:</label>
                </td>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblSessoTutore" CssClass="txtUppercase"></asp:Label>
                </td>
                <td class="Row1">
                    <label>Data Nascita:</label>
                </td>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblDataNascitaTutore" CssClass="txtUppercase"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width:5px;" class="none">
                </td>
                <td class="Row1">
                    <label>Comune Nascita:</label>
                </td>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblComuneNascitaTutore" CssClass="txtUppercase"></asp:Label>
                </td>
                <td class="Row1">
                    <label>Provincia Nascita:</label>
                </td>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblProvinciaNascitaTutore" CssClass="txtUppercase"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width:5px;" class="none">
                </td>
                <td class="Row1">
                    <label>Indirizzo:</label>
                </td>
                <td class="field">
                    <asp:Label runat="server" ID="lblIndirizzoTutore" CssClass="txtUppercase"></asp:Label>
                </td>
                <td class="Row1">
                    <label>Numero:</label>
                </td>
                <td class="field">
                    <asp:Label runat="server" ID="lblNCivicoTutore" CssClass="txtUppercase"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width:5px;" class="none">
                </td>
                <td class="Row1">
                    <label>CAP:</label>
                </td>
                <td class="field">
                    <asp:Label runat="server" ID="lblCapTutore" CssClass="txtUppercase"></asp:Label>
                </td>
                <td class="Row1">
                    <label>Comune Residenza:</label>
                </td>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblComuneResidenzaTutore" CssClass="txtUppercase"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width:5px;" class="none">
                </td>
                <td class="Row1">
                    <label>Provincia:</label>
                </td>
                <td class="field">
                    <asp:Label runat="server" ID="lblProvinciaTutore" CssClass="txtUppercase"></asp:Label>
                </td>
                <asp:Panel runat="server" ID="pnlDataMorte" Visible="false">
                <td class="Row1">
                    <asp:Label ID="Label1" runat="server" ForeColor="Red">Data decesso:</asp:Label>
                </td>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblDataMorte" CssClass="txtUppercase" ForeColor="Red"></asp:Label>
                </td>
                </asp:Panel>
            </tr>
            <tr>
                <td style="width:5px;" class="none">
                </td>
                <td class="Row1 grid-row-10">
                    <label>Telefono:</label>
                </td>
                <td class="field grid-row-10" colspan="3">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtTelTutore" Width="35%" CssClass="txtUppercase tb8" Enabled="false"
                        MaxLength="18" TabIndex="6" onblur="extractPhoneChar(this);" onkeyup="extractPhoneChar(this);"
                        onkeypress="return blockNonPhone(this, event);"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validateTxtTel" ControlToValidate="txtTelTutore" ErrorMessage="Numero di telefono dell'incaricato alla tutela non valido (Formato corretto: +12/3456789)"
                        ValidationExpression="^\+?[0-9]+\/?[0-9]+|^\+?[0-9]+$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
                        ValidationGroup="UCDelegatoTutore" Enabled="true" />
                    <!-- Controllo campo obbligatorio -->
                    <%--<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtTelTutore"
                        Enabled="true" ErrorMessage="Inserire un numero di telefono dell'incaricato alla tutela" Text="*" CssClass="field-is-required" Display="Dynamic"
                        ValidationGroup="UCDelegatoTutore" />--%>
                </td>              
            </tr>
            <tr>
                <td style="width:5px;" class="none">
                </td>
                <td class="Row1 grid-row-10">
                    <label>Cellulare:</label>
                </td>
                <td class="field grid-row-10" colspan="3">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtCellTutore" Width="35%" CssClass="txtUppercase tb8" Enabled="false"
                        MaxLength="18" TabIndex="7" onblur="extractPhoneChar(this);" onkeyup="extractPhoneChar(this);"
                        onkeypress="return blockNonPhone(this, event);"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validateTxtCellTutore" ControlToValidate="txtCellTutore"
                        ErrorMessage="Numero di cellulare dell'incaricato alla tutela non valido (Formato corretto: +12/3456789)" ValidationExpression="^\+?[0-9]+\/?[0-9]+|^\+?[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCDelegatoTutore" Enabled="true" />
                </td>
            </tr>
            <tr>
                <td style="width:5px;" class="none">
                </td>
                <td class="Row1 grid-row-11">
                    <label>Email:</label>
                </td>
                <td class="field grid-row-11" colspan="3">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtEmailTutore" Width="35%" Enabled="false"
                        CssClass="tb8 txtUppercase" MaxLength="50" TabIndex="8"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validateTxtEmail" ControlToValidate="txtEmailTutore"
                        ErrorMessage="Indirizzo Email dell'incaricato alla tutela non valido" ValidationExpression="^[a-zA-Z0-9._%-]+@[a-zA-Z.-]+\.[a-zA-Z]{2,4}$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCDelegatoTutore" Enabled="true" />
                    <!-- Controllo campo obbligatorio-->
                    <%--  <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtEmailTutore"
                    Enabled="true" ErrorMessage="Inserire un indirizzo Email dell'incaricato alla tutela" Text="*" CssClass="field-is-required" Display="Dynamic"
                    ValidationGroup="UCDelegatoTutore" />--%>
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="hdnCodiceFiscaleTutore" />
    <asp:HiddenField runat="server" ID="HiddenSelectedTipoRicercaTutore" />
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right;" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaTabTutore" runat="server" SkinID="btnAzione1" Enabled="true" Text="Salva dati Tutela" Width="150px" 
                        onclick="btnSalvaTabTutore_Click" ValidationGroup="UCDelegatoTutore" 
                        OnClientClick="if(Page_ClientValidate('UCDelegatoTutore') && checkCFTutore()){aspnetForm.target ='_self'; BlockUI();} else return false;" CausesValidation="true" CssClass="primary"/>
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaTabTutore" runat="server" SkinID="btnAzione1" Enabled="true" Text="Elimina dati Tutela" Width="150px" 
                        onclick="btnEliminaTabTutore_Click" ValidationGroup="UCDelegatoTutore" 
                        OnClientClick="if (!window.confirm('Sei sicuro di voler eliminare la Tutela?')) return false; else BlockUI();" CausesValidation="true" CssClass="ghost-delete"/>
                </td>
                <%--<td style="text-align: center">
                    <asp:Button ID="btnAnnulla" runat="server" SkinID="btnAzione1" OnClientClick="javascript:return CleanFields2();"
                        Enabled="true" Text="Pulisci" Width="100px" />
                </td>--%>                
            </tr>
        </table>
    </div>
</asp:Panel>
