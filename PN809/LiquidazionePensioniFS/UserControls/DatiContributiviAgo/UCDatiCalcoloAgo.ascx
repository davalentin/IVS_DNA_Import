<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiCalcoloAgo.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo.UCDatiCalcoloAgo" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<script type="text/javascript">

    function validateTab() {
        var flag = true;
        if (document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloAgo_pdivRetributivo") != null) {
            if (document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloAgo_modalitaEditRetributivi").value == "true")
                flag = Page_ClientValidate('UCTabDatiCalcoloAgoRetr');
        }
        if (flag) {
            if (document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloAgo_pdivContributivo") != null) {
                if (document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloAgo_modalitaEditContributivi").value == "true")
                    flag = Page_ClientValidate('UCTabDatiCalcoloAgoContr');
            }
        }
        if (flag) {
            if (document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloAgo_divPnlImportoLordoDecorrenza") != null ||
                document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloAgo_pnlDatiCalcoloRendita") != null ||
                document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloAgo_divPnlCoefficienteContributivo") != null) {
                flag = Page_ClientValidate('UCTabDatiCalcolo');
            }
        }
        return flag;
    }

    function disableValidators() {
        // Disabilita tutti i validator della pagina
        for (var i = 0; i < Page_Validators.length; i++) {
            var val = Page_Validators[i];
            if (val.validationGroup === 'UCTabDatiCalcoloAgoRetr' &&
            val.id.indexOf('RequiredField') !== -1) {
                ValidatorEnable(val, false);
            }
        }
    }

    function DisableValidator() {
        SwitchValidator('.offClass', false); //Disabilita tutti i validatori
    }

    function SwitchValidator(cssClass, onOff) {
        for (i = 0; i < $(cssClass).length; i++) {
            var control = $(cssClass)[i]
            var validatorid = control.id;
            val = document.getElementById(validatorid);
            if (val != null && val != 'undefined') {
                var s = val.id;
                if (s.indexOf("RequiredField") != -1) {
                    ValidatorEnable(val, onOff);
                }
            }
        }
    }

    function DisableGridViewValidators() {
        SwitchValidator('.disClass', false);
    }

    $(document).ready(function () {

        String.prototype.myStartsWith = function (str) {
            if (this.indexOf(str) === 0) {
                return true;
            } else {
                return false;
            }
        };

        var inabilitaConDecorrenzaPost122011 = document.getElementById("<%=hfInabilitaConDecorrenzaPost122011.ClientID %>").value;
        if (inabilitaConDecorrenzaPost122011 == "true") {
            DisableValidator();
        }

        var isDomandaVOPGI = document.getElementById("<%=HdnIsDomandaVOPGI.ClientID %>").value;
        var isDomandaIOPGI = document.getElementById("<%=HdnIsDomandaIOPGI.ClientID %>").value;
        var isDomandaSpacchettamentoSOPGI = document.getElementById("<%=HdnIsDomandaSpacchettamentoSOPGI.ClientID %>").value;


        if (isDomandaVOPGI == "true" || isDomandaIOPGI == "true" || isDomandaSpacchettamentoSOPGI == "true") {
            DisableGridViewValidators()
        }

        //Gestione per AUT
        if ($("#<%=HdnCodGestioneAUT.ClientID %>").val() || $("#<%=HdnCodGestioneRIC.ClientID %>").val()) {
            ManageCodGestForAUT(); //AUT
            //registro evento per gestiore decodifiche codGestione in base a FacoltaComputo
            $("#<%=ddlFacoltaComputo.ClientID %>").change(function () { ManageCodGestForAUT(); });
        }
    });

    //AUT
    function ManageCodGestForAUT() {
        var codGestAUT = $("#<%=HdnCodGestioneAUT.ClientID %>").val();
        //vecchia gestione
        //var codGestRIC = $("#<%=HdnCodGestioneRIC.ClientID %>").val();

        var array;
        //vecchia gestione
        //var arrayRIC;
        //if (codGestRIC.length > 0)
        //    arrayRIC = codGestRIC.split(';');
        if (codGestAUT.length > 0)
            array = codGestAUT.split(';')

        var isFacoltaComputo = ($("#<%=ddlFacoltaComputo.ClientID %>").val() == "SI");

        //vecchia gestione
        //if (arrayRIC != undefined) {
        //    for (var i = 0; i < arrayRIC.length; i++) {          
        //                        $(".classContribCodGestione > option").filter(function(){
        //                        return this.innerHTML.myStartsWith(arrayRIC[i])}).wrap("<span/> ").hide();           
        //    }
        //}

        if (array != undefined) {
            for (var i = 0; i < array.length; i++) {
                if (!isFacoltaComputo)
                    $(".classContribCodGestione > option").filter(function () {
                        return this.innerHTML.myStartsWith(array[i])
                    }).wrap("<span/> ").hide();
                else
                    $(".classContribCodGestione > span > option").filter(function () {
                        return this.innerHTML.myStartsWith(array[i])
                    }).unwrap().show();
            }
        }
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
                    document.getElementById('<%= btnSalvaDatiCalcolo.ClientID %>').click();

                    return true;
                }
            }
        });
    });
</script>
<asp:Panel runat="server" ID="pnlDatiCalcolo">
    <UCA:UCAvviso Visible="false" ID="ucAvviso" runat="server" />
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblQuotaD" runat="server" Text="Per domande con data fine assicurazione pari o successiva al 01/01/2012 è necessario inserire la quota D."
                    Style="font-weight: bold" ForeColor="Black" Visible="false"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblRicNonContrib" runat="server" Text="I dati di calcolo sono disponibili per la sola visualizzazione.  Possono essere modificati con una Ricostituzione contributiva."
                    Style="font-weight: bold" ForeColor="Black" Visible="false"></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <!--panel retributivo-->
    <div id="pdivRetributivo" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td class="full-grid">
                    <asp:Label runat="server" ID="lblDatiRetributivi" CssClass="section-label">Dati Retributivi</asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center" class="full-grid">
                    <asp:GridView runat="server" ID="gvDatiRetributivi" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="100%" BorderColor="Black"
                        AutoGenerateEditButton="true" PageSize="10" AllowPaging="true" OnRowCommand="gvDatiRetributivi_RowCommand"
                        OnRowDataBound="gvDatiRetributivi_RowDataBound" OnRowCancelingEdit="gvDatiRetributivi_RowCancelingEdit"
                        OnRowEditing="gvDatiRetributivi_RowEditing" OnRowUpdating="gvDatiRetributivi_RowUpdating"
                        EnableViewState="true" OnLoad="gvDatiRetributivi_Load" OnDataBound="gvDatiRetributivi_DataBound"
                        OnPageIndexChanging="gvDatiRetributivi_PageIndexChanging" PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
                            <asp:TemplateField HeaderText="Codice Gestione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceGestione_item" Width="100px" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlCodiceGestione" Width="100px" CssClass="txtUppercase tb8 xs">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceGestione" runat="server" ErrorMessage="Codice Gestione: campo obbligatorio"
                                        Text="*" ControlToValidate="ddlCodiceGestione" ValidationGroup="UCTabDatiCalcoloAgoRetr"
                                        CssClass="disClass field-is-required"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuota_item" Width="50px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlQuota" Width="50px" CssClass="txtUppercase tb8 xxs">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="A" Value="A"></asp:ListItem>
                                        <asp:ListItem Text="B" Value="B"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlQuotaSupplementi" runat="server"
                                        ErrorMessage="Quota: Campo obbligatorio" Text="*" ControlToValidate="ddlQuota"
                                        ValidationGroup="UCTabDatiCalcoloAgoRetr" CssClass="field-is-required disClass"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" Visible="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza" Text='<%#Bind("Decorrenza", "{0:dd/MM/yyyy}") %>'></asp:Label>
                                </ItemTemplate>
                                <%--                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenza" Width="80px"
                                        CssClass="txtUppercase tb8 date-picker dateMMaaaa" MaxLength="7" Text='<%#Bind("Decorrenza", "{0:dd/MM/yyyy}") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="REV_txtDecorrenza" ControlToValidate="txtDecorrenza"
                                        ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$" Enabled="true" Text="*" CssClass="field-is-required"
                                        ErrorMessage="Formato data non corretto" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloAgoRetr" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza"
                                        Display="Dynamic" ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required"
                                        ValidationGroup="UCTabDatiCalcoloAgoRetr" ID="customCheckDatatxtDecorrenza"
                                        ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>--%>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSettimaneRetributive" runat="server"
                                        MaxLength="4" Width="50px" Text='<%#Bind("Settimane") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtSettimaneRetributive"
                                        ControlToValidate="txtSettimaneRetributive" Display="Dynamic" ErrorMessage="Settimane: inserire il numero di settimane  in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloAgoRetr" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimaneRetributive" runat="server"
                                        ErrorMessage="Settimane: Campo obbligatorio" Text="*" ControlToValidate="txtSettimaneRetributive"
                                        ValidationGroup="UCTabDatiCalcoloAgoRetr" CssClass="disClass field-is-required"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Reddito / Retribuzione Media" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRetribuzioneMedia" Width="150px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtRetribuzioneMedia" Width="150px"
                                        CssClass="txtUppercase tb8 " MaxLength="12" Text=' <%# Bind("RetribuzioneMedia")%>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtRetribuzioneMedia" ControlToValidate="txtRetribuzioneMedia"
                                        Display="Dynamic" ErrorMessage="Retribuzione Media: inserire l'importo in formato valido (max 7 interi e 6 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,6})?" ValidationGroup="UCTabDatiCalcoloAgoRetr" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtRetribuzioneMedia" runat="server"
                                        ErrorMessage="Reddito/Retribuzione media: Campo obbligatorio" Text="*" ControlToValidate="txtRetribuzioneMedia"
                                        ValidationGroup="UCTabDatiCalcoloAgoRetr" CssClass="disClass field-is-required"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Sett. 707" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane707"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSettimaneRetributive707" runat="server"
                                        MaxLength="4" Width="50px" Text='<%#Bind("Settimane707") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="REVtxtSettimaneRetributive707"
                                        ControlToValidate="txtSettimaneRetributive707" Display="Dynamic" ErrorMessage="Sett. 707: inserire il numero di settimane  in un formato valido"
                                        Text="*" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloAgoRetr"
                                        CssClass="disClass field-is-required" />
                                    <!--COMMENTATI CONTROLLI SETTIMANE 707 PER TEST SULLE SALVAGUARDIE LEGGE 135/2012 -->
                                    <%--<asp:RequiredFieldValidator ID="RFVtxtSettimaneRetributive707" runat="server"
                                        ErrorMessage="Sett. 707: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtSettimaneRetributive707"
                                        ValidationGroup="UCTabDatiCalcoloAgoRetr" ></asp:RequiredFieldValidator>--%>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quote Retributivo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" Visible="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuoteRetributivo" Width="150px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtQuoteRetributivo" Width="150px"
                                        CssClass="txtUppercase tb8 " MaxLength="12" Text=' <%# Bind("PL_Quotar")%>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtQuoteRetributivo" ControlToValidate="txtQuoteRetributivo"
                                        Display="Dynamic" ErrorMessage="Quote Retributivo: inserire l'importo in formato valido (max 7 interi e 6 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,6})?" ValidationGroup="UCTabDatiCalcoloAgoRetr" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtQuoteRetributivo" runat="server"
                                        ErrorMessage="Quote Retributivo: Campo obbligatorio" Text="*" ControlToValidate="txtQuoteRetributivo"
                                        ValidationGroup="UCTabDatiCalcoloAgoRetr" CssClass="disClass field-is-required"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quote Retributivo 707" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" Visible="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuoteRetributivo707" Width="150px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtQuoteRetributivo707"
                                        Width="150px" CssClass="txtUppercase tb8 " MaxLength="12" Text=' <%# Bind("PL_Quotar707")%>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtQuoteRetributivo707"
                                        ControlToValidate="txtQuoteRetributivo707" Display="Dynamic" ErrorMessage="Quote Retributivo 707: inserire l'importo in formato valido (max 7 interi e 6 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,6})?" ValidationGroup="UCTabDatiCalcoloAgoRetr" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtQuoteRetributivo707" runat="server"
                                        ErrorMessage="Quote Retributivo 707: Campo obbligatorio" Text="*" ControlToValidate="txtQuoteRetributivo707"
                                        ValidationGroup="UCTabDatiCalcoloAgoRetr" CssClass="disClass field-is-required"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteRetributivi" CommandName="Elimina" CommandArgument="<% # ((GridViewRow) Container).RowIndex %>"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblIdCodeGestione" runat="server" />
                                    <asp:HiddenField runat="server" ID="hdnGUID" Visible="false" Value='<%# Eval("Id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        <br />
        <br />
    </div>
    <!-- fine panel retributivo--->
    <!--panel contributivo-->
    <div id="pdivContributivo" runat="server" style="margin-left: 10px; margin-right: 10px;" class="mt-32">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblCalcoloContributivo" CssClass="section-label">Dati Contributivi</asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:GridView ID="gvDatiContributivi" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                        AutoGenerateEditButton="true" BorderColor="Black" BorderWidth="1" CssClass="intestazioneTabella intestazioneTabella__with-pagination"
                        EnableViewState="true" OnRowCancelingEdit="gvDatiContributivi_RowCancelingEdit"
                        OnRowCommand="gvDatiContributivi_RowCommand" OnRowDataBound="gvDatiContributivi_RowDataBound"
                        OnRowEditing="gvDatiContributivi_RowEditing" OnRowUpdating="gvDatiContributivi_RowUpdating"
                        OnLoad="gvDatiContributivi_Load" PageSize="10" SkinID="grdElenco1" Width="100%"
                        OnDataBound="gvDatiContributivi_DataBound" OnPageIndexChanging="gvDatiContributivi_PageIndexChanging" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessun dato retributivo inserito."
                                    SkinID="lblNoData" Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Codice Gestione"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblCodiceGestione_item" runat="server" CssClass="txtUppercase" Width="150px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlCodiceGestione" runat="server" CssClass="txtUppercase tb8 classContribCodGestione xs"
                                        Width="150px">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceGestione" runat="server" ErrorMessage="Codice Gestione: campo obbligatorio"
                                        Text="*" ControlToValidate="ddlCodiceGestione" ValidationGroup="UCTabDatiCalcoloAgoContr"
                                        CssClass="offClass field-is-required disClass"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuota_item" Width="40px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlQuota" Width="40px" CssClass="txtUppercase tb8 xxs">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="C" Value="C"></asp:ListItem>
                                        <asp:ListItem Text="D" Value="D"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlQuotaContrib" runat="server" ErrorMessage="Quota: Campo obbligatorio"
                                        Text="*" ControlToValidate="ddlQuota" ValidationGroup="UCTabDatiCalcoloAgoContr"
                                        CssClass="offClass field-is-required disClass"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Settimane"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblSettimane" runat="server" Text='<%#Bind("Settimane") %>' Width="40px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtSettimaneContributive" runat="server" CssClass="tb8 txtUppercase"
                                        MaxLength="4" Text='<%#Bind("Settimane") %>' Width="40px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimaneContributive" runat="server"
                                        ErrorMessage="Settimane: Campo obbligatorio" Text="*" ControlToValidate="txtSettimaneContributive"
                                        ValidationGroup="UCTabDatiCalcoloAgoContr" CssClass="offClass field-is-required disClass"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtSettimaneContributive" runat="server"
                                        ControlToValidate="txtSettimaneContributive" Display="Dynamic" ErrorMessage="Settimane: inserire il numero di settimane  in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloAgoContr" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Ammontare"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblAmmontareContributivo" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAmmontareContributivo" runat="server" CssClass="txtUppercase tb8 "
                                        MaxLength="12" Style="text-align: left" Text='<%#Bind("AmmontareContributivo") %>'
                                        Width="100px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtAmmontareContributivo" runat="server"
                                        ErrorMessage="Ammontare contributivo: Campo obbligatorio" Text="*" ControlToValidate="txtAmmontareContributivo"
                                        ValidationGroup="UCTabDatiCalcoloAgoContr" CssClass="offClass field-is-required disClass"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtAmmontareContributivo" runat="server"
                                        ControlToValidate="txtAmmontareContributivo" Display="Dynamic" ErrorMessage="Ammontare Contributivo: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,4})?" ValidationGroup="UCTabDatiCalcoloAgoContr" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Montante"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblMontanteContributivo" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtMontanteContributivo" runat="server" CssClass="txtUppercase tb8 "
                                        MaxLength="12" Style="text-align: left" Text=' <%# Bind("MontanteContributivo")%>'
                                        Width="100px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtMontanteContributivo" runat="server"
                                        ErrorMessage="Montante contributivo: Campo obbligatorio" Text="*" ControlToValidate="txtMontanteContributivo"
                                        ValidationGroup="UCTabDatiCalcoloAgoContr" CssClass="offClass field-is-required disClass"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtMontanteContributivo" runat="server"
                                        ControlToValidate="txtMontanteContributivo" Enabled="false" Display="Dynamic"
                                        ErrorMessage="Montante Contributivo: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,4})?" ValidationGroup="UCTabDatiCalcoloAgoContr" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Quota Contributiva"
                                ItemStyle-CssClass="TblRecordset3" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblQuotaContributiva" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtQuotaContributiva" runat="server" CssClass="txtUppercase tb8 "
                                        MaxLength="12" Style="text-align: left" Text=' <%# Bind("PL_Quotac")%>' Width="100px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtQuotaContributiva" runat="server"
                                        ErrorMessage="Quota Contributiva: Campo obbligatorio" Text="*" ControlToValidate="txtQuotaContributiva"
                                        ValidationGroup="UCTabDatiCalcoloAgoContr" CssClass="offClass field-is-required disClass" Enabled="false"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtQuotaContributiva" runat="server" ControlToValidate="txtQuotaContributiva"
                                        Display="Dynamic" ErrorMessage="Quota Contributiva: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,4})?" ValidationGroup="UCTabDatiCalcoloAgoContr"
                                        Enabled="false" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteContributivi" ToolTip="cancella" runat="server" Text=""
                                        CommandArgument="<%#((GridViewRow)Container).RowIndex %>" CommandName="Elimina" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblIdCodeGestione" runat="server" />
                                    <asp:HiddenField runat="server" ID="hdnGUID" Visible="false" Value='<%# Eval("Id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <!-- fine panel contributivo--->
    <asp:Panel ID="pnlDomandeAUT" runat="server" Visible="false">
        <div id="divPnlDomandeAut" runat="server" style="margin-left: 10px; margin-right: 10px;">
            <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
                width: 99%">
                <tr>
                    <td colspan="2">
                        <br />
                    </td>
                </tr>
                <tr class="Row1">
                    <td style="width: 25%">
                        <asp:Label ID="lblFacoltaComputo" runat="server"> Facoltà di computo: </asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlFacoltaComputo" CssClass="tb8 txtUppercase xxs"
                            Width="10%">
                            <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                            <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlImportoLordoDecorrenza" Visible="false">
        <div id="divPnlImportoLordoDecorrenza" runat="server" style="margin-left: 10px; margin-right: 10px;">
            <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
                width: 99%">
                <tr>
                    <td colspan="4">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Codice gestione:
                        </label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:Label ID="lblGestioneImportoLordoAllaDec" runat="server"></asp:Label>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Importo lordo alla decorrenza:
                        </label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox runat="server" ID="txtImportoLordoAllaDecorrenza" CssClass="tb8 txtUppercase"
                            MaxLength="16" Width="90%"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoLordoAllaDecorrenza"
                            Display="Dynamic" ControlToValidate="txtImportoLordoAllaDecorrenza" Enabled="true"
                            ErrorMessage="Importo lordo alla decorrenza: Inserire valori interi o decimali (max 8 interi e 7 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{1,8}(\,\d{1,7})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RFVtxtImportoLordoAllaDecorrenza"
                            ControlToValidate="txtImportoLordoAllaDecorrenza" Display="Dynamic" Enabled="true"
                            ErrorMessage="Importo lordo alla Decorrenza: si prega di inserire il valore"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlDatiCalcoloAPESociale" Visible="false">
        <table>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Importo Lordo:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox runat="server" ID="txtImportoLordo" CssClass="tb8 txtUppercase" MaxLength="16"
                        Width="90%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoLordo" Display="Dynamic"
                        ControlToValidate="txtImportoLordo" Enabled="true" ErrorMessage="Importo lordo: Inserire valori interi o decimali (max 8 interi e 7 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{1,8}(\,\d{1,7})?" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtImportoLordo" ControlToValidate="txtImportoLordo"
                        Display="Dynamic" Enabled="true" ErrorMessage="Importo lordo: si prega di inserire il valore"
                        ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlDatiCalcoloRendita" Visible="false">
        <table>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Importo Mensile alla Decorrenza Originaria:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox runat="server" ID="txtImportoMensileAllaDecorrenzaOriginaria" CssClass="tb8 txtUppercase"
                        MaxLength="16" Width="90%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoMensileAllaDecorrenzaOriginaria"
                        Display="Dynamic" ControlToValidate="txtImportoMensileAllaDecorrenzaOriginaria"
                        Enabled="true" ErrorMessage="Importo Mensile alla Decorrenza Originaria: Inserire valori interi o decimali (max 8 interi e 7 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{1,8}(\,\d{1,7})?" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtImportoMensileAllaDecorrenzaOriginaria"
                        ControlToValidate="txtImportoMensileAllaDecorrenzaOriginaria" Display="Dynamic"
                        Enabled="true" ErrorMessage="Importo Mensile alla Decorrenza Originaria: si prega di inserire il valore"
                        ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
                <td class="Row1" style="width: 25%" runat="server" id="trLblImportoMensileAlGennaio2001">
                    <label>
                        Importo Mensile al Gennaio 2001:</label>
                </td>
                <td class="field" style="width: 25%" runat="server" id="trTxtImportoMensileAlGennaio2001">
                    <asp:TextBox runat="server" ID="txtImportoMensileAlGennaio2001" CssClass="tb8 txtUppercase"
                        MaxLength="16" Width="90%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoMensileAlGennaio2001"
                        Display="Dynamic" ControlToValidate="txtImportoMensileAlGennaio2001" Enabled="true"
                        ErrorMessage="Importo Mensile al Gennaio 2001: Inserire valori interi o decimali (max 8 interi e 7 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{1,8}(\,\d{1,7})?" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtImportoMensileAlGennaio2001"
                        ControlToValidate="txtImportoMensileAlGennaio2001" Display="Dynamic" Enabled="true"
                        ErrorMessage="Importo Mensile al Gennaio 2001: si prega di inserire il valore"
                        ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlCoefficienteContributivo" runat="server" Visible="false">
        <div id="divPnlCoefficienteContributivo" runat="server" style="margin-left: 10px;
            margin-right: 10px;">
            <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
                width: 70%">
                <tr>
                    <td colspan="2">
                        <br />
                    </td>
                </tr>
                <tr class="Row1">
                    <td style="width: 50%">
                        <asp:Label ID="Label1" runat="server"> Coefficiente di Trasformazione: </asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtCoefficienteContributivo" CssClass="tb8 txtUppercase"
                            MaxLength="16" Width="20%"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="REVtxtCoefficienteContributivo"
                            Display="Dynamic" ControlToValidate="txtCoefficienteContributivo" Enabled="true"
                            ErrorMessage="Coefficiente: inserire l'importo in formato valido (max 2 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{1,2}(\,\d{1,4})?" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlContributiItaEdEsteriAl1295" Visible="false">
        <div id="divContributiItaEdEsteriAl1295" runat="server" style="margin-left: 10px;
            margin-right: 10px;">
            <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
                width: 99%">
                <tr class="Row1">
                    <td style="width: 40%">
                        <asp:Label ID="lblContributiItalianiEsteri" runat="server"> Contributi Italiani ed Esteri al 31/12/95: </asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtContributiItalianiEsteri" runat="server" CssClass="tb8 txtUppercase xxs"
                            Width="10%" MaxLength="6"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="validateTxtContributiItalianiEsteri"
                            ControlToValidate="txtContributiItalianiEsteri" Display="Dynamic" ErrorMessage="Contributi Italiani ed Esteri al 31/12/95 non valido: inserire il numero di Contributi Italiani ed Esteri al 31/12/95 in un formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloCI" />
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="text-align: left" colspan="2">
                        <asp:Label ID="lblContrItaEsteri" runat="server" Text="Attenzione per tale tipologia di pensione, possono essere valorizzati solo contributi esteri ante 1996 o periodi di riscatto in GS"
                            Style="font-weight: bold" ForeColor="green"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <div style="margin-top: 25px;">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right" class="tab-actions-group__first">
                    <asp:Button ID="btnPopUp" Style="display: none" runat="server" SkinID="btnAzione1"
                        CausesValidation="false" Text="Salva Dati Calcolo" Width="190px" OnClientClick="if(validateTab()){$('#dialog-confirm').dialog('open');}return false;" CssClass="primary" />
                    <asp:Button ID="btnSalvaDatiCalcolo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Salva Dati Calcolo" Width="190px" OnClientClick="if(validateTab()){aspnetForm.target = '_self'; BlockUI();}"
                        OnClick="btnSalvaDatiCalcolo_Click" CssClass="primary" />
                    <%--                    <asp:Button ID="btnAnnulla" runat="server" SkinID="btnAzione1" OnClientClick="javascript:return CleanFields4();"
                        Enabled="true" Text="Pulisci" Width="100px" />
                    --%>
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaDatiCalcolo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elimina Dati Calcolo" Width="190px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Calcolo?')) return false; else BlockUI();"
                        OnClick="btnEliminaDatiCalcolo_Click" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<div id="dialog-confirm" title="Confirm" style="border-style: none; border-color: White;">
    <p>
        <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
        Attenzione il montante è inferiore all'ammontare - Confermare ?</p>
</div>
<asp:HiddenField ID="hfInabilitaConDecorrenzaPost122011" runat="server" Value="false" />
<asp:HiddenField ID="HdnCodGestioneAUT" runat="server" Value="" />
<asp:HiddenField runat="server" ID="modalitaEditContributivi" Value="false" />
<asp:HiddenField runat="server" ID="modalitaEditRetributivi" Value="false" />
<asp:HiddenField ID="HdnCodGestioneRIC" runat="server" Value="" />
<asp:HiddenField runat="server" ID="HdnIsDomandaVOPGI" Value="false" />
<asp:HiddenField runat="server" ID="HdnIsDomandaIOPGI" Value="false" />
<asp:HiddenField runat="server" ID="HdnIsDomandaSpacchettamentoSOPGI" Value="false" />
