<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCIstruttoria.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.Istruttoria" %>
<style type="text/css">
    .fixed-dialog
    {
        position: fixed;
    }
</style>
<script type="text/javascript">
    $(document).ready(function () {
        var availableTags = document.getElementById("ctl00_ContentPlaceHolder1_ucIstruttoria_HiddenFieldAziende").value.split(';');
        var siglaCategoria = document.getElementById("<%= HiddenFieldSiglaCategoria.ClientID %>").value;
        var filtro = document.getElementById("<%= HiddenFieldFiltro.ClientID %>").value;

        $("#<%=txtAzienda.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTags,
            select: function (event, ui) {
                if (siglaCategoria == "VESO92" || siglaCategoria == "ESOAMB" || siglaCategoria == "ESPA") {

                    $("#<%=txtAzienda.ClientID%>").val(ui.item.value);
                    __doPostBack('txtAzienda', '');
                    BlockUI();
                }
            },
            open: function () {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            }
        });

        VisualizzaRowAzienda();
        if (document.getElementById("<%= HiddenFieldAziendaVisible.ClientID %>").value == "true") {
            document.getElementById("ctl00_ContentPlaceHolder1_ucIstruttoria_rowAzienda").style.display = "table-row";
        }
        else {
            document.getElementById("ctl00_ContentPlaceHolder1_ucIstruttoria_rowAzienda").style.display = "none";
        }

        VisualizzaRowAttivitaUsuranti();
        if (document.getElementById("<%= HiddenFieldAttivitaUsurantiVisible.ClientID %>").value == "true") {
            document.getElementById("ctl00_ContentPlaceHolder1_ucIstruttoria_rowAttivitaUsuranti").style.display = "table-row";
        }
        else {
            document.getElementById("ctl00_ContentPlaceHolder1_ucIstruttoria_rowAttivitaUsuranti").style.display = "none";
        }

    });


    function Confirm() {
        var ddl = document.getElementById('<%= ddlRiduzioneRetributiva.ClientID %>');
        var selectedValue = ddl.options[ddl.selectedIndex].value;
        var tipoCalcolo = typeof getTipoCalcolo === "function" ? getTipoCalcolo() : undefined;
        var hfDataTitolareAdd62 = document.getElementById('<%= HiddenDataTitolareAdd62.ClientID %>').value;
        var noShow = false;
        if (hfDataTitolareAdd62 != null && hfDataTitolareAdd62 != "")
        {
            var codNat1 = typeof GetCodNatura1 === "function" ? GetCodNatura1() : undefined;
            var dataScadenza = document.getElementById('<%= txtScadenza.ClientID %>').value;
            if (dataScadenza != null && dataScadenza != "") {
                var dateApp = hfDataTitolareAdd62.split("/");
                var d1 = new Date(dateApp[2], dateApp[1] - 1, dateApp[0]);
                dateApp = dataScadenza.split("/");
                var d2 = new Date(dateApp[1], dateApp[0] - 1, 1);
                if (d1 < d2) {
                    noShow = true;
                }
            }
            if (codNat1 != '1')
                noShow = true;

            var ddlSoggettoDerogato = document.getElementById('<%= ddlSoggettoDerogato.ClientID %>');          
            if (!(ddlSoggettoDerogato!= null && $('#<%= ddlSoggettoDerogato.ClientID %>').is(':visible') == true && $('#<%= ddlSoggettoDerogato.ClientID %>').val() == ""))
                noShow = true;

            var siglaCategoria = document.getElementById("<%= HiddenFieldSiglaCategoria.ClientID %>").value;
            var ddlRiduzioneAssegno = document.getElementById('<%= ddlRiduzioneAssegno.ClientID %>');          
            if (siglaCategoria == "VOCRED" && !(ddlRiduzioneAssegno != null &&  $('#<%= ddlRiduzioneAssegno.ClientID %>').is(':visible') == true && $('#<%= ddlRiduzioneAssegno.ClientID %>').val() == ""))            
                noShow = true;
        }
        if (selectedValue.toUpperCase() == 'SI' || tipoCalcolo == 1 || noShow)
            document.getElementById('<%= btnSalvaIstruttoria.ClientID %>').click();
        else
            $('#dialog-confirm').dialog('open');

        return false;

    }

    $(function () {
        $('#dialog-confirm').dialog({
            autoOpen: false,

            show: 'blind',
            hide: 'blind',
            height: 220,
            width: 450,
            modal: true,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            close: function (event, ui) { $('body').css('overflow', 'auto'); },
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');
                    return false;
                },
                'Ok': function () {
                    $(this).dialog('close');
                    document.getElementById('<%= btnSalvaIstruttoria.ClientID %>').click();
                    return true;
                }
            }
        });
    });


    function checkPercentualeRiduzione(source, args) {
        var result = false;
        var ddl = document.getElementById('<%= ddlRiduzioneRetributiva.ClientID %>');
        if (ddl != null) {
            var selectedValue = ddl.options[ddl.selectedIndex].value;
            if (selectedValue.toUpperCase() == 'SI') {
                var txt = document.getElementById('<%= txtRiduzioneRetributiva.ClientID %>');
                if (txt.value == '')
                    result = false;
                else
                    result = true;
            }
            else
                result = true;
        }
        args.IsValid = result;
        return false;
    }

    function VisualizzaRowAzienda() {
        var isVisible;
        var codNatura3 = GetCodNatura3();
        var siglaCategoria = document.getElementById("<%= HiddenFieldSiglaCategoria.ClientID %>").value;
        var isPrepensionamentoEditoriaVisible = document.getElementById("<%= HiddenFieldIsPrepensionamentoEditoria.ClientID%>").value;
        if ((codNatura3 != null && codNatura3 == "O" && siglaCategoria != null && siglaCategoria == "VO" && isPrepensionamentoEditoriaVisible == "NO") ||
            (siglaCategoria == "VESO33" || siglaCategoria == "VESO92" || siglaCategoria == "VOCRED" || siglaCategoria == "CRED27" || siglaCategoria == "VOCOOP" ||
             siglaCategoria == "COOP28" || siglaCategoria == "VESO29" || siglaCategoria == "VOESO" || siglaCategoria == "ESOTEL" || siglaCategoria == "ESOAMB" ||
             siglaCategoria == "ESPA")) {
            document.getElementById("ctl00_ContentPlaceHolder1_ucIstruttoria_rowAzienda").style.display = "table-row";
            isVisible = document.getElementById("<%= HiddenFieldAziendaVisible.ClientID %>").value = "true";
        }
        else {
            document.getElementById("ctl00_ContentPlaceHolder1_ucIstruttoria_rowAzienda").style.display = "none";
            isVisible = document.getElementById("<%= HiddenFieldAziendaVisible.ClientID %>").value = "false";
        }
    }

    function VisualizzaRowAttivitaUsuranti() {
        var isVisible;
        var codNatura1 = GetCodNatura1();
        var siglaCategoria = document.getElementById("<%= HiddenFieldSiglaCategoria.ClientID %>").value;
        if (codNatura1 != null && (codNatura1 == "0" || codNatura1 == "6" || codNatura1 == "8" || codNatura1 == "9") &&
            siglaCategoria != null && (siglaCategoria == "VO" || siglaCategoria == "VOBANC" || siglaCategoria == "VOP" || siglaCategoria == "VR" || siglaCategoria == "VOART" ||
                siglaCategoria == "VOCOM" || siglaCategoria == "VDAI")) {
            document.getElementById("ctl00_ContentPlaceHolder1_ucIstruttoria_rowAttivitaUsuranti").style.display = "table-row";
            isVisible = document.getElementById("<%= HiddenFieldAttivitaUsurantiVisible.ClientID %>").value = "true";
        }
        else {
            document.getElementById("ctl00_ContentPlaceHolder1_ucIstruttoria_rowAttivitaUsuranti").style.display = "none";
            isVisible = document.getElementById("<%= HiddenFieldAttivitaUsurantiVisible.ClientID %>").value = "false";
        }
    }

    function getAzienda() {
        try {
            return $("#<%= txtAzienda.ClientID %>").val().split(' -')[0];
        } catch (e) {
            return 0;
        }
    }

</script>
<asp:Panel runat="server" ID="pnlIstruttoria">
    <table class="tabellaFormattazione grid grid-size-20" width="100%">
        <tr>
            <td class="Row1 if-empty-none">
                <asp:Label ID="lblCodCD_CM_MR" Text="Codice CD/CM/MR:" runat="server" Visible="false"></asp:Label>
            </td>
            <td class="field if-empty-none">
                <asp:DropDownList runat="server" ID="ddlCodCD_CM_MR" Width="35%" CssClass="txtUppercase tb8"
                    TabIndex="1" Visible="false">
                </asp:DropDownList>
            </td>
            <td class="Row1 none">
            </td>
            <td class="field none">
            </td>
        </tr>
        <asp:Panel ID="pnlCodiceRequisitoRidotto" runat="server">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Codice Requisiti ridotti:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlCodReqRidotti" Width="43%" CssClass="tb8 txtUppercase">
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlBancari" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Codice Banca:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlCodiceBanca" Width="43%" CssClass="tb8 txtUppercase">
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlSoggettoDerogato" runat="server" Visible="false">
            <tr>
                <td class="Row1">
                    <label>
                        Soggetto Derogato:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlSoggettoDerogato" Width="90%" CssClass="tb8 txtUppercase"
                        TabIndex="3" Enabled="false">
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <tr id="rowAzienda" runat="server">
            <td class="Row1" style="width: 25%">
                <label>
                    Azienda:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox runat="server" ID="txtAzienda" TabIndex="2" CssClass="tb8 txtUppercase"
                    Width="70%"></asp:TextBox>
            </td>
        </tr>
        <asp:Panel ID="pnlAliquotaTfrEsodati" runat="server" Visible="false">
            <tr>
                <td class="Row1">
                    <label>
                        Aliquota TFR Esodati:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:TextBox Style="text-align: right" runat="server" ID="txtAliquotaTFREsodatiInt"
                        Width="40px" CssClass="txtUppercase tb8" TabIndex="7" MaxLength="2" onblur="extractNumber(this,0,false);"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <label>
                        ,
                    </label>
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtAliquotaTFREsodatiDec"
                        Width="40px" CssClass="txtUppercase tb8" TabIndex="8" MaxLength="2" Text="00"
                        onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                        onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <label>
                        %</label>
                </td>
            </tr>
        </asp:Panel>
        <tr id="rowAttivitaUsuranti" runat="server">
            <td class="Row1" style="width: 25%">
                <label>
                    Attività Usuranti:</label>
            </td>
            <td class="field">
                <asp:DropDownList runat="server" ID="ddlAttivitaUsuranti" Width="15%" CssClass="tb8 txtUppercase xxs"
                    TabIndex="1">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="Row1">
            </td>
            <td class="field">
            </td>
        </tr>
        <tr>
            <td colspan="4" class="shift-full-grid">
                <!-- Pannello Riduzione Retributiva-->
                <asp:Panel ID="pnlRiduzioneRetributiva" runat="server" CssClass="full-width">
                    <table width="100%" class="tabellaFormattazione grid grid-size-20">
                        <tr style="vertical-align: bottom">
                            <td class="Row1" style="width: 25%">
                                <label>
                                    Riduz. ex L.214:</label>
                            </td>
                            <td class="Row1 flex-align-center gap-4" style="width: 65%">
                                <asp:DropDownList ID="ddlRiduzioneRetributiva" CssClass="tb8 txtUppercase xxs width-33" Width="15%"
                                    runat="server">
                                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox ID="txtRiduzioneRetributiva" runat="server" CssClass="tb8 txtUppercase width-33"
                                    Width="15%" TabIndex="14" MaxLength="5"></asp:TextBox>
                                <label>
                                    %</label>
                                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator13"
                                    Display="Dynamic" ControlToValidate="txtRiduzioneRetributiva" Enabled="true"
                                    ErrorMessage="Riduzione Retributiva: Inserire valori interi o decimali" Text="*" CssClass="field-is-required"
                                    ValidationGroup="UCTabIstruttoria" ValidationExpression="\d{1,2}(\,\d{1,2})?" />
                                <asp:CustomValidator runat="server" ControlToValidate="ddlRiduzioneRetributiva" Display="Dynamic"
                                    ErrorMessage="Riduzione Retributiva: La percentuale è obbligatoria avendo selezionato 'SI'"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabIstruttoria" ID="customRiduzione" ClientValidationFunction="checkPercentualeRiduzione" />
                            </td>
                            <td style="width: 15%">
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <!-- Fine Pannello Riduzione Retributiva-->
                <!-- Inizio Pannello Prepensionamento Editoria-->
                <asp:Panel ID="pnlPrepensionamentoEditoria" runat="server" Visible="false">
                    <table width="100%">
                        <tr style="vertical-align: bottom">
                            <td class="Row1" style="width: 35%">
                                <label ID="lblCodice" runat="server">
                                    Codice:
                                </label>
                                <asp:TextBox ID="txtCodicePrepensionamentoEditoria" runat="server" CssClass="tb8 txtUppercase"
                                    Width="20%" TabIndex="14" MaxLength="5"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REVCodicePrepensionamentoEditoria"
                                    ControlToValidate="txtCodicePrepensionamentoEditoria" Display="Dynamic" ErrorMessage="Inserire il Codice azienda editoria in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]+$" ValidationGroup="UCTabIstruttoria" />
                                <asp:RequiredFieldValidator runat="server" ID="RFVCodicePrepensionamentoEditoria"
                                    Display="Dynamic" ErrorMessage="Codice: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabIstruttoria"
                                    ControlToValidate="txtCodicePrepensionamentoEditoria"></asp:RequiredFieldValidator>
                                <asp:Button ID="btnAggiorna" runat="server" SkinID="btnAzione1" CausesValidation="false"
                                    OnClick="AggiornaCampiCodice_Click" Text="Aggiorna" Width="90px"  CssClass="ghost-update"/>
                            </td>
                            <td class="Row1" style="width: 45%">
                                <asp:Label ID="lblDenominazioneAzienda" runat="server">Denominazione Azienda: </asp:Label>
                                <asp:TextBox ID="txtDenominazioneAzienda" runat="server" CssClass="tb8 txtUppercase"
                                    Width="40%" TabIndex="14" MaxLength="5"></asp:TextBox>
                            </td>
                        </tr>
                        <tr style="vertical-align: bottom">
                            <td class="Row1" style="width: 35%">
                                <asp:Label ID="lblDataAccordi" runat="server">Data Accordi: </asp:Label>
                                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataAccordi" Width="50%"
                                    CssClass="tb8 txtUppercase" TabIndex="8" Text="GG/MM/AAAA" MaxLength="10"></asp:TextBox>
                            </td>
                            <td class="Row1" style="width: 45%">
                                <asp:Label ID="lblDecreto" runat="server">Decreto: </asp:Label>
                                <asp:TextBox ID="txtDecreto" runat="server" CssClass="tb8 txtUppercase" Width="40%"
                                    TabIndex="14" MaxLength="5"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <!-- Fine Pannello Prepensionamento Editoria-->
            </td>
        </tr>
        <asp:Panel ID="pnlCodiciDerogaENPLAS" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Descrizione Deroga:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlCodiceDeroga1" Width="13%" CssClass="tb8 txtUppercase xxs"
                        Enabled="false">
                    </asp:DropDownList>
                    <asp:DropDownList runat="server" ID="ddlCodiceDeroga2" Width="13%" CssClass="tb8 txtUppercase xxs"
                        Enabled="false">
                    </asp:DropDownList>
                    <asp:DropDownList runat="server" ID="ddlCodiceDeroga3" Width="13%" CssClass="tb8 txtUppercase xxs"
                        Enabled="false">
                    </asp:DropDownList>
                    <asp:DropDownList runat="server" ID="ddlCodiceDeroga4" Width="13%" CssClass="tb8 txtUppercase xxs"
                        Enabled="false">
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlRiduzioneAssegno" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Riduzione Assegno:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList ID="ddlRiduzioneAssegno" CssClass="tb8 txtUppercase" Width="40px"
                        runat="server">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="8" Value="8"></asp:ListItem>
                        <asp:ListItem Text="11" Value="11"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlBancaFideiussione" Visible="false">
            <tr>
                <td colspan="4">
                    <asp:Label runat="server" ID="lblNoPianoEsodo" Font-Bold="true" ForeColor="Red" Font-Size="Small"
                        Visible="false">
                        Non è previsto al momento un piano esodo per l’azienda selezionata.</asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Anno:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlAnnoBancaFideiussione" Width="40%" CssClass="tb8 txtUppercase"
                        OnSelectedIndexChanged="ddlAnnoBancaFideiussione_OnSelectedIndexChanged" AutoPostBack="true"
                        onchange="BlockUI();" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVddlAnnoBancaFideiussione" Display="Dynamic"
                        ErrorMessage="Anno: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabIstruttoria"
                        ControlToValidate="ddlAnnoBancaFideiussione" InitialValue=""></asp:RequiredFieldValidator>
                    <asp:Button runat="server" ID="btnAggiornaAnnoBancaFideiussione" OnClick="btnAggiornaAnnoBancaFideiussione_OnClick"
                        OnClientClick="BlockUI()" Text="Aggiorna" SkinID="btnAzione1" Width="45%"  CssClass="ghost-update"/>
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Progressivo:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlProgressivoBancaFideiussione" Width="40%"
                        CssClass="tb8 txtUppercase" OnSelectedIndexChanged="ddlProgressivoBancaFideiussione_OnSelectedIndexChanged"
                        AutoPostBack="true" onchange="BlockUI();" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVddlProgressivoBancaFideiussione"
                        Display="Dynamic" ErrorMessage="Progressivo: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabIstruttoria"
                        ControlToValidate="ddlProgressivoBancaFideiussione" InitialValue=""></asp:RequiredFieldValidator>
                    <asp:Button runat="server" ID="btnAggiornaProgressivoBancaFideiussione" OnClick="ddlAnnoBancaFideiussione_OnSelectedIndexChanged"
                        OnClientClick="BlockUI()" Text="Aggiorna" SkinID="btnAzione1" Width="45%"  CssClass="ghost-update"/>
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        ABI:</label>
                </td>
                <td class="field">
                    <asp:Label runat="server" ID="lblABIBancaFideiussione"></asp:Label>
                </td>
                <td class="Row1">
                    <label>
                        CAB:</label>
                </td>
                <td class="field">
                    <asp:Label runat="server" ID="lblCABBancaFideiussione"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        Banca:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:Label runat="server" ID="lblBancaFideiussione"></asp:Label>
                </td>
            </tr>
        </asp:Panel>
        <asp:Label ID="lblIstruttoriaAPESociale" runat="server" Text="La data di scadenza dell’indennità deve essere pari alla data della decorrenza della pensione Monti/Fornero"
            Style="font-weight: bold" ForeColor="Red" Visible="false"></asp:Label>
        <br />
        <asp:Label ID="lblScadenzaIndennitaAPESociale" runat="server" Text="Per sbloccare il campo data scadenza indennità è necessario cancellare e ri-prelevare la domanda"
            ForeColor="Black" Visible="false"></asp:Label>
        <!--pannello scadenza assegno-->
        <asp:Panel ID="pnlScadenzaAssegno" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <asp:Label runat="server" ID="lblTextScadenzaAssegno" Text="Data scadenza assegno:"></asp:Label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtScadenza" Width="50%"
                        CssClass="txtUppercase tb8" Text="mm/aaaa"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtScadenzaMMAAAA" ControlToValidate="txtScadenza"
                        Display="Dynamic" ErrorMessage="Inserire la data nel formato valido per scadenza assegno"
                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}$" ValidationGroup="UCTabIstruttoria"
                        Enabled="true" />
                    <asp:RegularExpressionValidator ID="REVtxtScadenzaGGMMAAAA" ControlToValidate="txtScadenza"
                        ErrorMessage="Inserire la data nel formato valido per scadenza assegno" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabIstruttoria"
                        Enabled="false" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtScadenza" Display="Dynamic"
                        ErrorMessage="Data scadenza assegno: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabIstruttoria"
                        ID="CVtxtScadenza" ClientValidationFunction="checkCorrettezzaData" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtScadenza" ControlToValidate="txtScadenza"
                        Display="Dynamic" ErrorMessage="Inserire Data scadenza assegno" Text="*" CssClass="field-is-required" ValidationGroup="UCTabIstruttoria" />
                </td>
                <td class="Row1">
                </td>
                <td class="field">
                </td>
            </tr>
        </asp:Panel>
        <!-- fine panello scadenza assegno-->
        <asp:Panel ID="pnlCodiceEntePSO" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Codice Ente:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlCodiceEnte" Width="43%" CssClass="tb8 txtUppercase">
                    <asp:ListItem Text="ENPAS – 1" Value="1"></asp:ListItem>
                    <asp:ListItem Text="POSTELEGRAFONICI – 2" Value="2"></asp:ListItem>
                    <asp:ListItem Text="INADEL – 3" Value="3"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
    </table>
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs no-margin">
        <table width="100%" class="tab-actions-group footer-actions-group--istruttoria">
            <tr>
                <td style="text-align: right; vertical-align: bottom;" class="tab-actions-group__first">
                    <asp:Button ID="btnPopUp" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Visible="false" Text="Salva Istruttoria" Width="170px" OnClientClick="if(Page_ClientValidate('UCTabIstruttoria')){return Confirm();}" CssClass="primary" />
                    <asp:Button ID="btnSalvaIstruttoria" runat="server" CausesValidation="false" Style="display: none"
                        ValidationGroup="UCTabIstruttoria" SkinID="btnAzione1" Width="170px" OnClick="SalvaIstruttoria_Click"
                        Text="Salva Istruttoria" Visible="false" OnClientClick="if(Page_ClientValidate('UCTabIstruttoria')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
                    <asp:Button ID="btnSalvaIstruttoriaNoRiduzione" runat="server" CausesValidation="false"
                        ValidationGroup="UCTabIstruttoria" SkinID="btnAzione1" Width="170px" OnClick="SalvaIstruttoria_Click"
                        Text="Salva Istruttoria" Visible="true" OnClientClick="if(Page_ClientValidate('UCTabIstruttoria')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaIstruttoria" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elimina Istruttoria" Width="170px" OnClick="EliminaIstruttoria_Click"
                        OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Istruttoria?')) return false; else BlockUI();" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<asp:HiddenField runat="server" ID="HiddenFieldAziende" />
<asp:HiddenField ID="FlagUnicarpe" runat="server" />
<asp:HiddenField runat="server" ID="HiddenFieldAziendaVisible" Value="false" />
<asp:HiddenField runat="server" ID="HiddenFieldAttivitaUsurantiVisible" Value="false" />
<asp:HiddenField runat="server" ID="HiddenFieldSiglaCategoria" />
<asp:HiddenField runat="server" ID="HiddenFieldDecorrenzaOriginaria" />
<asp:HiddenField runat="server" ID="HiddenFieldDataNascitaTitolare" />
<asp:HiddenField runat="server" ID="HiddenFieldFiltro" />
<asp:HiddenField runat="server" ID="HiddenFieldIsPrepensionamentoEditoria" />
<asp:HiddenField runat="server" ID="HiddenDataTitolareAdd62" />
<div id="dialog-confirm" title="Confirm" style="border-style: none; border-color: White;">
    <p>
        <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
        Età titolare inferiore a 62 anni. Confermi la mancanza della percentuale di Riduzione?
    </p>
</div>
