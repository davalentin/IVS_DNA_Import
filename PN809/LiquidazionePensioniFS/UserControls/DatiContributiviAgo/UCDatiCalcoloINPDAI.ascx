<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiCalcoloINPDAI.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo.UCDatiCalcoloINPDAI" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<script type="text/javascript">

    function validateTabINPDAI() {
        var flag = true;
        if (document.getElementById("<%=pdivRetributivo.ClientID %>") != null) {
            if (document.getElementById("<%=modalitaEditRetributivi.ClientID%>").value == "true")
                flag = Page_ClientValidate('UCTabDatiCalcoloAgoRetr');
        }
        if (flag) {
            if (document.getElementById("<%=pdivContributivo.ClientID %>") != null) {
                if (document.getElementById("<%=modalitaEditContributivi.ClientID %>").value == "true")
                    flag = Page_ClientValidate('UCTabDatiCalcoloAgoContr');
            }
        }
        if (flag) {
            if (document.getElementById("<%=pnlContributoSolidarieta.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabDatiCalcoloINPDAI');
            }
        }
        return flag;
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

    $(document).ready(function () {
        var inabilitaConDecorrenzaPost122011 = document.getElementById("<%=hfInabilitaConDecorrenzaPost122011.ClientID %>").value;
        if (inabilitaConDecorrenzaPost122011 == "true") {
            DisableValidator();
        }


        $("table[id$='gvDatiRetributivi']").ready(function () {
            $(this).find("select[id$='ddlCodiceGestione']").change(function () {
                switchCodiceTipoQuota(this);
            });
        });
        $("table[id$='gvDatiRetributivi'] select[id$='ddlCodiceGestione']").each(function () {
            switchCodiceTipoQuota($(this));
        });
    });

    function switchCodiceTipoQuota(itemCodiceGestione) {
        var ddlTipoQuotaGestioneA = $(itemCodiceGestione).closest("tr").find("select[id$='ddlTipoQuotaGestioneA']");
        var ddlTipoQuotaGestioneAltre = $(itemCodiceGestione).closest("tr").find("select[id$='ddlTipoQuotaGestioneAltre']");

        if ($(itemCodiceGestione).find('option:selected').text().split(' - ')[0] == 'A') {
            // riporto il valore dall'altra dropdownlist
            $(ddlTipoQuotaGestioneA).val($(ddlTipoQuotaGestioneAltre).val());
            // svuoto il valore dell'altra dropdownlist
            $(ddlTipoQuotaGestioneAltre).val('');
            $(ddlTipoQuotaGestioneAltre).hide();
            $(ddlTipoQuotaGestioneA).show();
        }
        else {
            // riporto il valore dall'altra dropdownlist
            $(ddlTipoQuotaGestioneAltre).val($(ddlTipoQuotaGestioneA).val());
            // svuoto il valore dell'altra dropdownlist
            $(ddlTipoQuotaGestioneA).val('');
            $(ddlTipoQuotaGestioneAltre).show();
            $(ddlTipoQuotaGestioneA).hide();
        }
    }

</script>
<style type="text/css">
    .hideGridColumn {
        display: none;
    }
</style>
<asp:Panel runat="server" ID="pnlDatiCalcolo">
    <UCA:UCAvviso Visible="false" ID="ucAvviso" runat="server" />
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblRicNonContrib" runat="server" Text="I dati di calcolo sono disponibili per la sola visualizzazione. Possono essere modificati con una Ricostituzione contributiva." Style="font-weight: bold" ForeColor="Black" Visible="false"></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <div id="pdivRetributivo" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px; width: 99%">
            <tr>
                <td class=" full-grid">
                    <asp:Label runat="server" ID="lblDatiRetributivi"> Dati Retributivi:</asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center;" class=" full-grid">
                    <asp:GridView runat="server" ID="gvDatiRetributivi" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella" BorderWidth="1" Width="100%" BorderColor="Black"
                        AutoGenerateEditButton="true" PageSize="10" AllowPaging="false" OnRowCommand="gvDatiRetributivi_RowCommand"
                        OnRowDataBound="gvDatiRetributivi_RowDataBound" OnRowCancelingEdit="gvDatiRetributivi_RowCancelingEdit"
                        OnRowEditing="gvDatiRetributivi_RowEditing" EnableViewState="true" OnLoad="gvDatiRetributivi_Load"
                        OnDataBound="gvDatiRetributivi_DataBound">
                        <Columns>
                            <asp:TemplateField HeaderText="Codice Gestione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceGestione_item" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlCodiceGestione" Width="100px" CssClass="txtUppercase tb8 xs">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceGestione" runat="server" ErrorMessage="Codice Gestione: campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlCodiceGestione" ValidationGroup="UCTabDatiCalcoloAgoRetr"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
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
                                        <asp:ListItem Text="A" Value="A"></asp:ListItem>
                                        <asp:ListItem Text="B" Value="B"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlQuotaSupplementi" runat="server"
                                        ErrorMessage="Quota: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="ddlQuota"
                                        ValidationGroup="UCTabDatiCalcoloAgoRetr" Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tipo Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblTipoQuota_item" Width="130px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlTipoQuotaGestioneA" Width="130px" CssClass="txtUppercase tb8 xs">
                                    </asp:DropDownList>
                                    <asp:DropDownList runat="server" ID="ddlTipoQuotaGestioneAltre" Width="40px" CssClass="txtUppercase tb8 xs">
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Giorni / Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="100px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSettimaneRetributive" runat="server"
                                        MaxLength="5" Width="50px" Text='<%#Bind("Settimane") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtSettimaneRetributive"
                                        ControlToValidate="txtSettimaneRetributive" Display="Dynamic" ErrorMessage="Settimane: inserire il numero di settimane  in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloAgoRetr" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimaneRetributive" runat="server"
                                        ErrorMessage="Settimane: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtSettimaneRetributive"
                                        ValidationGroup="UCTabDatiCalcoloAgoRetr" Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="RMS / RMG" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRetribuzioneMedia" Width="120px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtRetribuzioneMedia" Width="120px"
                                        CssClass="txtUppercase tb8 " MaxLength="14" Text=' <%# Bind("RetribuzioneMedia")%>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtRetribuzioneMedia" ControlToValidate="txtRetribuzioneMedia"
                                        Display="Dynamic" ErrorMessage="Retribuzione Media: inserire l'importo in formato valido (max 7 interi e 6 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,6})?" ValidationGroup="UCTabDatiCalcoloAgoRetr" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtRetribuzioneMedia" runat="server"
                                        ErrorMessage="Reddito/Retribuzione media: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtRetribuzioneMedia"
                                        ValidationGroup="UCTabDatiCalcoloAgoRetr" Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Giorni / Settimane 707" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="100px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane707"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSettimaneRetributive707" runat="server"
                                        MaxLength="5" Width="40px" Text='<%#Bind("Settimane707") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="REVtxtSettimaneRetributive707"
                                        ControlToValidate="txtSettimaneRetributive707" Display="Dynamic" ErrorMessage="Sett. 707: inserire il numero di settimane  in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloAgoRetr" />
                                    <asp:RequiredFieldValidator ID="RFVtxtSettimaneRetributive707" runat="server" ErrorMessage="Sett. 707: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtSettimaneRetributive707" ValidationGroup="UCTabDatiCalcoloAgoRetr"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="PL_Quotar" ItemStyle-CssClass="hideGridColumn" HeaderStyle-CssClass="hideGridColumn" ItemStyle-Width="100px">
                                <ItemTemplate>
                                    <asp:HiddenField runat="server" ID="lblPL_Quotar"></asp:HiddenField>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="PL_Quotar707" ItemStyle-CssClass="hideGridColumn" HeaderStyle-CssClass="hideGridColumn" ItemStyle-Width="100px">
                                <ItemTemplate>
                                    <asp:HiddenField runat="server" ID="lblPL_Quotar707"></asp:HiddenField>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteRetributivi" CommandName="Elimina" CommandArgument="<% # ((GridViewRow)Container).RowIndex %>"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblIdCodeGestione" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="modalitaEditRetributivi" Value="false" />
    <br />
    <div id="pdivContributivo" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px; width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblCalcoloContributivo">Dati Contributivi:</asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center;">
                    <asp:GridView ID="gvDatiContributivi" runat="server" AllowPaging="false" AutoGenerateColumns="false"
                        AutoGenerateEditButton="true" BorderColor="Black" BorderWidth="1" CssClass="intestazioneTabella"
                        EnableViewState="true" OnRowCancelingEdit="gvDatiContributivi_RowCancelingEdit"
                        OnRowCommand="gvDatiContributivi_RowCommand" OnRowDataBound="gvDatiContributivi_RowDataBound"
                        OnRowEditing="gvDatiContributivi_RowEditing" PageSize="10" SkinID="grdElenco1"
                        OnDataBound="gvDatiContributivi_DataBound" Width="100%">
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
                                    <asp:DropDownList ID="ddlCodiceGestione" runat="server" CssClass="txtUppercase tb8 xs"
                                        Width="150px">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceGestione" runat="server" ErrorMessage="Codice Gestione: campo obbligatorio"
                                        Text="*"  ControlToValidate="ddlCodiceGestione" ValidationGroup="UCTabDatiCalcoloAgoContr"
                                        CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
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
                                        CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
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
                                        ValidationGroup="UCTabDatiCalcoloAgoContr" CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
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
                                        ValidationGroup="UCTabDatiCalcoloAgoContr" CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
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
                                        ValidationGroup="UCTabDatiCalcoloAgoContr" CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtMontanteContributivo" runat="server"
                                        ControlToValidate="txtMontanteContributivo" Display="Dynamic" ErrorMessage="Montante Contributivo: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,4})?" ValidationGroup="UCTabDatiCalcoloAgoContr" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="PL_Quotac" ItemStyle-CssClass="hideGridColumn" HeaderStyle-CssClass="hideGridColumn" ItemStyle-Width="100px">
                                <ItemTemplate>
                                    <asp:HiddenField runat="server" ID="lblPL_Quotac"></asp:HiddenField>
                                </ItemTemplate>
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
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <br />
    <table runat="server" ID="ImportoAl200312" class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px; width: 80%">
       <tr>
                <td align="left" style="width: 25%; padding-left: 10px;" class="Row1" >
                    <label>
                       Importo al 12/2003:</label>
                </td>
                <td align="left" style="width: 55%" class="field">
                    <asp:TextBox runat="server" ID="txtImportoAl200312" CssClass="txtUppercase tb8" Width="20%"
                        MaxLength="9"></asp:TextBox>
                        </td>
            </tr>
        </table>
    <asp:Panel ID="pnlContributoSolidarieta" Style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;"
        runat="server">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px; width: 99%">
            <tr>
                <td class="Row1" colspan="4">
                    <b>
                        <label>
                            Contributo di solidarietà L. 214/2011</label></b>
                </td>
            </tr>
            <tr>
                <td align="left" style="width: 25%; padding-left: 10px" class="Row1">
                    <label>
                        Anzianità al '95:</label>
                </td>
                <td align="left" style="width: 25%" class="field">
                    <asp:TextBox runat="server" ID="txtAnzAl95" CssClass="txtUppercase tb8" Width="70%"
                        MaxLength="9"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtAnzAl95" ControlToValidate="txtAnzAl95"
                        Display="Dynamic" Text="*" CssClass="field-is-required" ErrorMessage="Anzianità al '95 in formato non corretto (max 4 interi e 4 decimali)"
                        ValidationExpression="\d{1,4}(\,\d{1,4})?" ValidationGroup="UCTabDatiCalcoloINPDAI"></asp:RegularExpressionValidator>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAnzAl95" Text="*" CssClass="field-is-required"
                        ID="RFVtxtAnzAl95" ErrorMessage="Anzianità al '95 obbligatoria" Display="Dynamic"
                        ValidationGroup="UCTabDatiCalcoloINPDAI"></asp:RequiredFieldValidator>
                </td>
                <td align="left" style="width: 25%" class="Row1">
                    <label>
                        Quota al '95:</label>
                </td>
                <td align="left" style="width: 25%" class="field">
                    <asp:TextBox runat="server" ID="txtQuotaAl95" CssClass="txtUppercase tb8" Width="70%"
                        MaxLength="12"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtQuotaAl95" ControlToValidate="txtQuotaAl95"
                        Display="Dynamic" Text="*" CssClass="field-is-required" ErrorMessage="Quota al '95 in formato non corretto (max 7 interi e 4 decimali)"
                        ValidationExpression="\d{1,7}(\,\d{1,4})?" ValidationGroup="UCTabDatiCalcoloINPDAI"></asp:RegularExpressionValidator>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtQuotaAl95" Text="*" CssClass="field-is-required"
                        ID="RFVtxtQuotaAl95" ErrorMessage="Quota al '95 obbligatoria" Display="Dynamic"
                        ValidationGroup="UCTabDatiCalcoloINPDAI"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:HiddenField runat="server" ID="modalitaEditContributivi" Value="false" />
    <!---panel tipo calcolo vincente-->
    <asp:Panel ID="panelTipoCalcoloVincente" runat="server" Visible="false">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px; width: 99%">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Tipo Calcolo Vincente:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:Label ID="labelTipoCalcoloVincente" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <!---fine panel tipo calcolo vincente-->
    <div style="margin-top: 25px;">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaDatiCalcolo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Salva Dati Calcolo" Width="190px" OnClientClick="if(validateTabINPDAI()){aspnetForm.target ='_self'; BlockUI();}"
                        OnClick="btnSalvaDatiCalcolo_Click" CssClass="primary" />
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
<asp:HiddenField ID="hfInabilitaConDecorrenzaPost122011" runat="server" Value="false" />
