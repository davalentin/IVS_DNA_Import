<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCAventiDiritto.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AventiDiritto.UCAventiDiritto" %>
<script type="text/javascript">

    function validateTab() {
        var flag = true;

        if (flag)
            flag = Page_ClientValidate("Selezione");

        if (flag)
            flag = Page_ClientValidate('UCAventiDiritto');

        if (flag)
            flag = Page_ClientValidate('UCAventiDirittoGrid');

        return flag;
    }

    function validateSelezione(source, args) {
        args.IsValid = true;
        source.errormessage = "";

        var listGroupNameRadio = [];
        $("[id$='chkSelect']").each(function () {
            var name = $(this).parent().attr("class");
            if ($.inArray(name, listGroupNameRadio) == -1)
                listGroupNameRadio.push(name);
        });

        var count = 0;
        for (var i = 0; i < listGroupNameRadio.length; i++) {
            if ($("span[class='" + listGroupNameRadio[i] + "'] > input[id$='chkSelect']:checked").size() == 0) {
                args.IsValid = false;
                if (count > 0)
                    source.errormessage += "<li>";
                source.errormessage += "Selezionare almeno una checkbox per l'avente diritto " + listGroupNameRadio[i] + "</li>";
                count++;
            }
        }
    }

    $(document).ready(function () {
        $("#dialog-confirm").html("");
    });

    $(function () {
        $('#dialog-confirm').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            width: 550,
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
                },
                'Ok': function () {
                    $(this).dialog('close');
                    document.getElementById('<%= btnSalvaAventiDiritto.ClientID %>').click();
                }
            }
        });
    });

    $(document).ready(function () {
        $("#dialog-confirm2").html("");
    });

    $(function () {
        $('#dialog-confirm2').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            width: 550,
            modal: true,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            close: function (event, ui) { $('body').css('overflow', 'auto'); },
            buttons: {
                'NO': function () {
                    $(this).dialog('close');
                },
                'SI': function () {
                    $(this).dialog('close');
                    document.getElementById('<%= btnSalvaAventiDiritto.ClientID %>').click();
                }
            }
        });
    });

    function confirmUC() {
        if (isConfirmPopUp())
            $('#dialog-confirm').dialog('open');
        else if (isConfirmPopUpNucleo()) {
            $('#dialog-confirm2').dialog('open');
        }
        else
            document.getElementById('<%= btnSalvaAventiDiritto.ClientID %>').click();
    }

    function isConfirmPopUp() {
        var returnValue = false;
        $("#dialog-confirm").html("");

        $("[id$='chkSelect']:checked").each(function () {
            if ($(this).closest("tr").find("input[type='hidden'][id$='hdnChiavePensione']").val() == "") {
                var codiceFiscale = $(this).closest("tr").find("span[id$='lblCodiceFiscale']").text();
                $("#dialog-confirm").append('<p><span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>ATTENZIONE!! Per il soggetto ' + codiceFiscale + ' vi è un’incongruenza tra i codici nucleo di WebDom e dell’Archivio Pensione. Confermare la scelta dell’avente diritto?</p></br>');
                returnValue = true;
            }
        });
        return returnValue;
    }

    function isConfirmPopUpNucleo() {
        var returnValue = false;
        $("#dialog-confirm2").html("");

        $("[id$='ddlNucleo']").each(function () {
            if ($(this).closest("tr").find("input[type='hidden'][id$='hdnChiavePensione']").val() != "") {
                var nucleoDaArchivioPensione = $(this).closest("tr").find("span[id$='lblNucleoDaArchivioPensione']").text();
                if (nucleoDaArchivioPensione != $(this).val()) {
                    $("#dialog-confirm2").append('<p><span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>Attenzione si sta modificando il codice nucleo di un avente diritto per il quale è già stata liquidata una pensione. Procedere?</p></br>');
                    returnValue = true;
                    return false;
                }
            }
        });
        return returnValue;
    }

    function SetUniqueRadioButton(classGroup, current) {
        for (var i = 0; i < document.forms[0].elements.length; i++) {
            var elm = document.forms[0].elements[i];
            if (elm.type == 'checkbox') {
                if ($(elm.parentElement).hasClass(classGroup)) {
                    elm.checked = false;
                }
            }
        }
        current.checked = true;
    }

</script>
<div id="pdivAventiDiritto" runat="server" style="margin-left: 10px; margin-right: 10px;">
    <asp:CustomValidator runat="server" Display="Dynamic" ValidationGroup="Selezione"
        ID="customCheckSelezione" ClientValidationFunction="validateSelezione" />
    <table class="tabellaFormattazione grid-col-1" style="padding-left: 10px; padding-bottom: 10px; width: 99%">
        <tr>
            <td>
                <asp:Label runat="server" ID="lblDatiAventiDiritto" Style="font-weight: bold" CssClass="section-label"> Elenco degli aventi diritto</asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="text-align: center">
                <asp:Label Style="font-weight: bold; color: Red; font-size: x-large" runat="server" ID="lblNoAventiDiritto"
                    Visible="false">


                    Nessun Avente Diritto presente.</asp:Label>
            </td>
        </tr>
        <tr>
            <td style="text-align: center">
                <asp:Repeater ID="repAventiDiritto" runat="server" OnItemDataBound="repAventiDiritto_ItemDataBound">
                    <HeaderTemplate>
                        <table style="vertical-align: top; width: 100%; height: 90%; margin: auto; margin-top: 1px; border-collapse: collapse; border: 1px solid #C0A9D6;" class="table table-striped display dataTable">
                            <tr class="TblRecordset">
                                <th class="intestazioneTabella" style="border: 1px solid #C0A9D6; width: 2%;" runat="server"
                                    id="headerRadioButton" visible="false"></th>
                                <th class="intestazioneTabella" style="border: 1px solid #C0A9D6; width: 2%;"></th>
                                <th class="intestazioneTabella Row1" style="border: 1px solid #C0A9D6;">
                                    <label>
                                        Nome</label>
                                </th>
                                <th class="intestazioneTabella Row1" style="border: 1px solid #C0A9D6;">
                                    <label>
                                        Cognome</label>
                                </th>
                                <th class="intestazioneTabella Row1" style="border: 1px solid #C0A9D6;">
                                    <label>
                                        Codice Fiscale</label>
                                </th>
                                <th class="intestazioneTabella Row1" style="border: 1px solid #C0A9D6;">
                                    <label>
                                        Relazione con il dante causa</label>
                                </th>
                                <th class="intestazioneTabella Row1" style="border: 1px solid #C0A9D6;">
                                    <label>
                                        Nucleo da WebDom</label>
                                </th>
                                <th class="intestazioneTabella Row1" style="border: 1px solid #C0A9D6;">
                                    <label>
                                        Nucleo da Archivio Pensione</label>
                                </th>
                                <th class="intestazioneTabella Row1" style="border: 1px solid #C0A9D6; width: 70px;">
                                    <label>
                                        Nucleo</label>
                                </th>
                                <th class="intestazioneTabella Row1" style="border: 1px solid #C0A9D6; width: 150px;">
                                    <label>
                                        Pensione</label>
                                </th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="TblRecordset3 breakword" style="width: 2%; border: 1px solid #C0A9D6;"
                                runat="server" id="itemRadioButton" visible="false">
                                <asp:CheckBox runat="server" ID="chkSelect" Visible="false" Checked="false" />
                            </td>
                            <td class="TblRecordset3 breakword" style="width: 2%; border: 1px solid #C0A9D6;">
                                <img alt="Visualizza dati periodi" title="Visualizza dati periodi" style="cursor: pointer"
                                    src="../App_Themes/<%= Page.Theme %>/Images/plus.png" />
                            </td>
                            <td class="TblRecordset3" style="border: 1px solid #C0A9D6;">
                                <asp:Label runat="server" ID="lblNome"></asp:Label>
                                <asp:HiddenField runat="server" ID="hdnIdAnagrafica" Value='<%#Bind("IdAnagrafica")%>'></asp:HiddenField>
                                <asp:HiddenField runat="server" ID="hdnId" Value='<%#Bind("Id")%>'></asp:HiddenField>
                                <asp:HiddenField runat="server" ID="hdnChiavePensione" Value='<%#String.Format("{0:000}{1:0000}{2:00000000}", Eval("CategoriaPensione"), Eval("SedePensione"), Eval("CertificatoPensione")) %>' />
                            </td>
                            <td class="TblRecordset3" style="border: 1px solid #C0A9D6;">
                                <asp:Label runat="server" ID="lblCognome"></asp:Label>
                            </td>
                            <td class="TblRecordset3" style="border: 1px solid #C0A9D6;">
                                <asp:Label runat="server" ID="lblCodiceFiscale"></asp:Label>
                            </td>
                            <td class="TblRecordset3" style="border: 1px solid #C0A9D6;">
                                <asp:Label runat="server" ID="lblRelazioneDA" Text='<%#Bind("DecParentelaDA")%>'></asp:Label>
                            </td>
                            <td class="TblRecordset3" style="border: 1px solid #C0A9D6;">
                                <asp:Label runat="server" ID="lblNucleoDaWebDom"></asp:Label>
                            </td>
                            <td class="TblRecordset3" style="border: 1px solid #C0A9D6;">
                                <asp:Label runat="server" ID="lblNucleoDaArchivioPensione"></asp:Label>
                            </td>
                            <td class="field" style="border: 1px solid #C0A9D6;">
                                <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlNucleo" runat="server" TabIndex="1" Width="45px">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorDdlNucleo"
                                    ControlToValidate="ddlNucleo" Enabled="true" ErrorMessage="Inserire il Nucleo"
                                    ValidationGroup="UCAventiDirittoGrid" Text="*" CssClass="field-is-required" />
                            </td>
                            <td class="TblRecordset3" style="border: 1px solid #C0A9D6;">
                                <asp:Label runat="server" ID="lblPensione"></asp:Label>
                            </td>
                        </tr>
                        <tr style="display: none">
                            <td></td>
                            <td>
                                <label style="font-weight: bold">
                                    Periodi:</label>
                            </td>
                            <td colspan="999">
                                <div style="margin: 15px auto;">
                                    <asp:GridView ID="gvDatiPeriodi" runat="server" AutoGenerateColumns="false" Width="95%"
                                        OnRowDataBound="gvDatiPeriodi_RowDataBound" Style="margin: auto;" SkinID="grdElenco1">
                                        <EmptyDataTemplate>
                                            <center>
                                                <asp:Label ID="lblNoData" runat="server" Text="Nessun periodo presente."
                                                    SkinID="lblNoData"></asp:Label>
                                            </center>
                                        </EmptyDataTemplate>
                                        <Columns>
                                            <asp:TemplateField HeaderText="Grado di parentela" HeaderStyle-CssClass="intestazioneTabellaInnestata"
                                                ItemStyle-CssClass="TblRecordset3 breakword" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblGradoParentela"></asp:Label>
                                                    <asp:Label runat="server" ID="lblIdPeriodo" Text='<%#Bind("Id")%>' Visible="false"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="% spettante" HeaderStyle-CssClass="intestazioneTabellaInnestata"
                                                ItemStyle-CssClass="TblRecordset3 breakword" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblPercSpettante" Text='<%#Bind("PercSpettante", "{0:F2}")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Coeff. di riduzione" HeaderStyle-CssClass="intestazioneTabellaInnestata"
                                                ItemStyle-CssClass="TblRecordset3 breakword" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblCoeffRiduzione" Text='<%#Bind("CoeffRiduzione", "{0:F2}")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="% giudice" HeaderStyle-CssClass="intestazioneTabellaInnestata"
                                                ItemStyle-CssClass="TblRecordset3 breakword" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblPercGiudice" Text='<%#Bind("PercGiudice", "{0:F2}")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Decorrenza periodo" HeaderStyle-CssClass="intestazioneTabellaInnestata"
                                                ItemStyle-CssClass="TblRecordset3 breakword" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblDecPeriodo" Text='<%#Bind("DecorrenzaPeriodo", "{0:MM/yyyy}")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Cessazione periodo" HeaderStyle-CssClass="intestazioneTabellaInnestata"
                                                ItemStyle-CssClass="TblRecordset3 breakword" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtCessazionePeriodo" Width="80px"
                                                        CssClass="txtUppercase tb8 date-picker dateMMaaaa" MaxLength="7" Text='<%#Bind("CessazionePeriodo", "{0:MM/yyyy}") %>'></asp:TextBox>
                                                    <asp:RegularExpressionValidator runat="server" ID="validateCessazione" ControlToValidate="txtCessazionePeriodo"
                                                        Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per Cessazione Periodo"
                                                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCAventiDirittoGrid"
                                                        Text="*" CssClass="field-is-required" />
                                                    <asp:CustomValidator runat="server" ControlToValidate="txtCessazionePeriodo" Display="Dynamic"
                                                        ErrorMessage="Cessazione periodo: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCAventiDirittoGrid"
                                                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <asp:Label Style="font-weight: bold; color: Red" runat="server" ID="lblMsgSelezione"
                    Visible="false">
                    Selezionare l'Avente Diritto corretto.</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <div style="margin: 25px auto; width: 100%; text-align: center;">
                    <table style="margin: auto;">
                        <tr>
                            <td align="right">
                                <asp:Button ID="btnAggiornaDaWebDom" runat="server" Text="Aggiorna da Webdom" SkinID="btnAzione1"
                                    CausesValidation="false" Width="230px" Height="50px" OnClick="AggiornaDaWebDom_Click"
                                    OnClientClick="BlockUI();"  CssClass="ghost-update"/>
                            </td>
                            <td align="left">
                                <asp:Button ID="btnAggiornaDaArchivioPensione" runat="server" Text="Aggiorna da Archivio Pensione"
                                    SkinID="btnAzione1" CausesValidation="false" Width="230px" Height="50px" OnClick="AggiornaDaArchivioPensione_Click"
                                    OnClientClick="BlockUI();" CssClass="ghost-update"/>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    <div style="margin-top: 25px;">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: center;">
                    <asp:Button ID="btnPopUp" runat="server" Text="Salva" SkinID="btnAzione1" CausesValidation="false"
                        Width="180px" OnClientClick="if(validateTab()){ confirmUC();} return false;" CssClass="primary"/>
                    <asp:Button ID="btnSalvaAventiDiritto" runat="server" Text="Salva" SkinID="btnAzione1"
                        CausesValidation="false" Width="180px" OnClick="SalvaAventiDiritto_Click" OnClientClick="if(validateTab()){aspnetForm.target = '_self'; BlockUI();}"
                        Style="display: none" CssClass="primary"/>
                </td>
            </tr>
        </table>
    </div>
</div>
<div id="dialog-confirm" title="Confirm" style="border-style: none; border-color: White;"></div>
<div id="dialog-confirm2" title="Confirm" style="border-style: none; border-color: White;"></div>


