<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiCalcoloCi.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCDatiCalcoloCi1" %>
<script type="text/javascript">

    $(document).ready(function () {
        var maternitaAcnaChecked = $(document.getElementById("<%=ckbCTRMaternitaExAcna.ClientID %>")).attr("checked");
        if (maternitaAcnaChecked) {
            AbilitaTab();
        }
        else {
            DisabilitaTab();
        }
    });

    function SetCheckBox(cb) {
        $('.offClass').val(''); //Pulisce tutti i campi con la class "offClass"
        $('.' + cb.getAttribute("EnableClass")).removeAttr('disabled'); //Abilita gli oggetti con l'attributo specificato
        if (cb.getAttribute("EnableClass") == "onClassMaternitaExAcna") {
            var maternitaAcnaChecked = $(document.getElementById("<%=ckbCTRMaternitaExAcna.ClientID %>")).attr("checked");
            if (maternitaAcnaChecked) {
                AbilitaTab();   //funzione implementata nella pagina padre (DatiContributiviCi.aspx)
            }
            else {
                DisabilitaTab();    //funzione implementata nella pagina padre (DatiContributiviCi.aspx)
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
<!-- Pannello Comune (due) -->
<asp:Panel ID="pnlComuneDue" runat="server" Visible="true">
    <table class="tabellaFormattazione grid grid-size-25">
        <tr>
            <td class="Row1 if-empty-none" style="text-align: left" colspan="2">
                <asp:Label ID="lblQuotaD" runat="server" Text="Per domande con data fine assicurazione pari o successiva al 01/01/2012 è necessario inserire la quota D"
                    Style="font-weight: bold" ForeColor="Black" Visible="false"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <asp:Label ID="lblCTRMaternitaExAcna" runat="server" Text="CTR Maternità / Ex Acna Cengio:"></asp:Label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:CheckBox ID="ckbCTRMaternitaExAcna" runat="server" TabIndex="15" CssClass="tb8" />
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Comune (due) -->
<!-- Pannello relativo al GridView Dati Contributivi -->
<asp:Panel ID="pnlGridViewDatiContributivi" runat="server">
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <table class="tabellaFormattazione">
                        <tr>
                            <td class="Row1" style="text-align: left">
                                <asp:Label ID="lblTitoloDatiContributivi" runat="server" Text="Dati Contributivi"
                                    Style="font-weight: bold" CssClass="section-label"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:GridView ID="gvDatiContributivi" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                        AutoGenerateEditButton="true" BorderColor="Black" BorderWidth="1" CssClass="intestazioneTabella intestazioneTabella__with-pagination"
                        EnableViewState="true" OnRowCancelingEdit="gvDatiContributivi_RowCancelingEdit"
                        OnRowCommand="gvDatiContributivi_RowCommand" OnRowDataBound="gvDatiContributivi_RowDataBound"
                        OnRowEditing="gvDatiContributivi_RowEditing" OnRowUpdating="gvDatiContributivi_RowUpdating"
                        PageSize="10" SkinID="grdElenco1" Width="100%" RowStyle-HorizontalAlign="Center" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessun dato contributivo inserito."
                                    SkinID="lblNoData" Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Codice Gestione"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="20%" ItemStyle-Width="20%"
                                FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label ID="lblCodiceGestione_item" runat="server" CssClass="txtUppercase" Width="100px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlCodiceGestione" runat="server" CssClass="txtUppercase tb8"
                                        Width="100px">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceGestione" runat="server" ErrorMessage="Codice Gestione: campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlCodiceGestione" ValidationGroup="UCTabDatiCalcoloContrCI"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="10%" ItemStyle-Width="10%"
                                FooterStyle-Width="10%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuota_item" Width="40px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlQuota" Width="40px" CssClass="txtUppercase tb8">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="C" Value="C"></asp:ListItem>
                                        <asp:ListItem Text="D" Value="D"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlQuotaContrib" runat="server" ErrorMessage="Quota: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlQuota" ValidationGroup="UCTabDatiCalcoloContrCI"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Settimane"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="15%" ItemStyle-Width="15%"
                                FooterStyle-Width="15%">
                                <ItemTemplate>
                                    <asp:Label ID="lblSettimane" runat="server" Text='<%#Bind("Settimane") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtSettimaneContributive" runat="server" CssClass="tb8 txtUppercase"
                                        MaxLength="4" Text='<%#Bind("Settimane") %>' Width="50px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regularTxtSettimaneContributive" runat="server"
                                        ControlToValidate="txtSettimaneContributive" Display="Dynamic" ErrorMessage="Settimane: inserire il numero di settimane  in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloContrCI" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimaneContributive" runat="server"
                                        ErrorMessage="Settimane: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtSettimaneContributive"
                                        ValidationGroup="UCTabDatiCalcoloContrCI" Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Ammontare Contributivo"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="21%" ItemStyle-Width="21%"
                                FooterStyle-Width="21%">
                                <ItemTemplate>
                                    <asp:Label ID="lblAmmontareContributivo" runat="server"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAmmontareContributivo" runat="server" CssClass="txtUppercase tb8 "
                                        MaxLength="15" Style="text-align: left" Text='<%#Bind("AmmontareContributivo") %>'
                                        Width="100px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regularTxtAmmontareContributivo" runat="server"
                                        ControlToValidate="txtAmmontareContributivo" Display="Dynamic" ErrorMessage="Ammontare Contributivo: Inserire valori interi o decimali (max 9 interi e 4 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{0,9}(,\d{1,4})?" ValidationGroup="UCTabDatiCalcoloContrCI" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtAmmontareContributivo" runat="server"
                                        ErrorMessage="Ammontare contributivo: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtAmmontareContributivo"
                                        ValidationGroup="UCTabDatiCalcoloContrCI" Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Montante Contributivo"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="21%" ItemStyle-Width="21%"
                                FooterStyle-Width="21%">
                                <ItemTemplate>
                                    <asp:Label ID="lblMontanteContributivo" runat="server"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtMontanteContributivo" runat="server" CssClass="txtUppercase tb8 "
                                        MaxLength="15" Style="text-align: left" Text=' <%# Bind("MontanteContributivo")%>'
                                        Width="100px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regularTxtMontanteContributivo" runat="server"
                                        ControlToValidate="txtMontanteContributivo" Display="Dynamic" ErrorMessage="Montante Contributivo: Inserire valori interi o decimali (max 9 interi e 4 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{0,9}(,\d{1,4})?" ValidationGroup="UCTabDatiCalcoloContrCI" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtMontanteContributivo" runat="server"
                                        ErrorMessage="Montante contributivo: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtMontanteContributivo"
                                        ValidationGroup="UCTabDatiCalcoloContrCI" Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;"
                                HeaderStyle-Width="5%" ItemStyle-Width="5%" FooterStyle-Width="5%">
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
                </div>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione mt-32">
        <tr class="single-input-field">
            <td class="Row1" style="width: 100%;">
                <asp:Label ID="lblCMSM" Text="CMSM:" runat="server"></asp:Label>
            </td>
            <td class="Row1 full-grid" style="width:75%;" colspan="3">
                <asp:TextBox ID="txtCMSM" runat="server" CssClass="tb8 txtUppercase" Width="130"
                    MaxLength="14"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateTxtCMSM" Display="Dynamic"
                    ControlToValidate="txtCMSM" Enabled="true" ErrorMessage="CMSM: Inserire valori interi o decimali (max 9 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloCI" ValidationExpression="\d{0,9}(,\d{1,4})?" />
            </td>
        </tr>
    </table>
    <asp:HiddenField runat="server" ID="modalitaEditContributivi" Value="false" />
</asp:Panel>
<!-- Fine Pannello relativo al GridView Dati Contributivi-->
<!-- Pannello relativo al GridView Dati Retributivi -->
<asp:Panel ID="pnlGridViewDatiRetributivi" runat="server">
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <table class="tabellaFormattazione">
                        <tr>
                            <td class="Row1" style="text-align: left">
                                <asp:Label ID="lblTitoloDatiRetributivi" runat="server" Text="Dati Retributivi" Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:GridView runat="server" ID="gvDatiRetributivi" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="100%" BorderColor="Black"
                        AutoGenerateEditButton="true" PageSize="10" AllowPaging="true" OnRowCommand="gvDatiRetributivi_RowCommand"
                        OnRowDataBound="gvDatiRetributivi_RowDataBound" OnRowCancelingEdit="gvDatiRetributivi_RowCancelingEdit"
                        OnRowEditing="gvDatiRetributivi_RowEditing" OnRowUpdating="gvDatiRetributivi_RowUpdating"
                        OnLoad="gvDatiRetributivi_LoadEvent" EnableViewState="true" PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
                            <asp:TemplateField HeaderText="Codice Gestione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="21%" ItemStyle-Width="21%"
                                FooterStyle-Width="21%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceGestione_item" Width="100px" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlCodiceGestione" Width="100px" CssClass="txtUppercase tb8">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceGestione" runat="server" ErrorMessage="Codice Gestione: campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlCodiceGestione" ValidationGroup="UCTabDatiCalcoloRetrCI"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="12%" ItemStyle-Width="12%"
                                FooterStyle-Width="12%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuota_item" Width="40px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlQuota" Width="40px" CssClass="txtUppercase tb8">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="A" Value="A"></asp:ListItem>
                                        <asp:ListItem Text="B" Value="B"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlQuotaSupplementi" runat="server"
                                        Display="Dynamic" ErrorMessage="Quota: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="ddlQuota"
                                        ValidationGroup="UCTabDatiCalcoloRetrCI"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="17%" ItemStyle-Width="17%"
                                FooterStyle-Width="17%" Visible="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="16%" ItemStyle-Width="16%"
                                FooterStyle-Width="16%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSettimaneRetributive" runat="server"
                                        MaxLength="4" Width="50px" Text='<%#Bind("Settimane") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtSettimaneRetributive"
                                        ControlToValidate="txtSettimaneRetributive" Display="Dynamic" ErrorMessage="Settimane: inserire il numero di settimane in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloRetrCI" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimaneRetributive" runat="server"
                                        ErrorMessage="Settimane: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtSettimaneRetributive"
                                        ValidationGroup="UCTabDatiCalcoloRetrCI" Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Reddito / Retribuzione Media" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="27%" ItemStyle-Width="27%"
                                FooterStyle-Width="27%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRetribuzioneMedia"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtRetribuzioneMedia" Width="110px"
                                        CssClass="txtUppercase tb8 " MaxLength="15" Text=' <%# Bind("RedditoRetribuzioneMedia")%>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtRetribuzioneMedia" ControlToValidate="txtRetribuzioneMedia"
                                        Display="Dynamic" ErrorMessage="Retribuzione Media: Inserire valori interi o decimali (max 7 interi e 6 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{0,7}(,\d{1,6})?" ValidationGroup="UCTabDatiCalcoloRetrCI" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtRetribuzioneMedia" runat="server"
                                        ErrorMessage="Reddito/Retribuzione media: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtRetribuzioneMedia"
                                        ValidationGroup="UCTabDatiCalcoloRetrCI" Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Sett. 707" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="10%" ItemStyle-Width="10%"
                                FooterStyle-Width="10%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane707CI"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSettimaneRetributive707CI" runat="server"
                                        MaxLength="4" Width="50px" Text='<%#Bind("Settimane707") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="REVtxtSettimaneRetributive707"
                                        ControlToValidate="txtSettimaneRetributive707CI" Display="Dynamic" ErrorMessage="Sett. 707: inserire il numero di settimane  in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloRetrCI" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;"
                                HeaderStyle-Width="5%" ItemStyle-Width="5%" FooterStyle-Width="5%">
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
                </div>
            </td>
        </tr>
    </table>
    <asp:HiddenField runat="server" ID="modalitaEditRetributivi" Value="false" />
</asp:Panel>
<!-- Fine Pannello relativo al GridView Dati Retributivi-->
<!-- Pannello relativo al GridView Dati Contributivi Esteri D.L. 503/92-->
<asp:Panel ID="pnlGridViewDatiContributiviEsteri" runat="server">
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <table class="tabellaFormattazione">
                        <tr>
                            <td class="Row1" style="text-align: left">
                                <asp:Label ID="lblTitoloDatiContributiviEsteri" runat="server" Text="Contributi Esteri D.L. 503/92 e L 335/95"
                                    Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <table class="tabellaFormattazione">
                        <tr class="single-input-field">
                            <td class="Row1" style="width: 100%; text-align: left">
                                <asp:Label ID="lblContributiItalianiEsteri" Text="Contributi Italiani ed Esteri al 31/12/95:"
                                    runat="server"></asp:Label>
                            </td>
                            <td class="Row1 full-grid" style="width:75%;" colspan="3">
                                <asp:TextBox ID="txtContributiItalianiEsteri" runat="server" CssClass="tb8 txtUppercase"
                                    Width="130" MaxLength="4"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="validateTxtContributiItalianiEsteri"
                                    ControlToValidate="txtContributiItalianiEsteri" Display="Dynamic" ErrorMessage="Contributi Italiani ed Esteri al 31/12/95 non valido: inserire il numero di Contributi Italiani ed Esteri al 31/12/95 in un formato valido"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloCI" />
                            </td>
                        </tr>
                    </table>
                    <asp:GridView runat="server" ID="gvDatiContributiviEsteri" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="100%" BorderColor="Black"
                        AutoGenerateEditButton="true" PageSize="10" AllowPaging="true" OnRowCommand="gvDatiContributiviEsteri_RowCommand"
                        OnRowDataBound="gvDatiContributiviEsteri_RowDataBound" OnRowCancelingEdit="gvDatiContributiviEsteri_RowCancelingEdit"
                        OnRowEditing="gvDatiContributiviEsteri_RowEditing" OnRowUpdating="gvDatiContributiviEsteri_RowUpdating"
                        EnableViewState="true" Visible="false" PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
                            <asp:TemplateField HeaderText="Codice Gestione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="30%" ItemStyle-Width="30%"
                                FooterStyle-Width="30%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceGestione_item" Width="120px" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlCodiceGestione" Width="120px" CssClass="txtUppercase tb8"
                                        Enabled="false">
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="29%" ItemStyle-Width="29%"
                                FooterStyle-Width="29%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtDecorrenza" CssClass="txtUppercase tb8 date-picker dateMMaaaa"
                                        MaxLength="7" Text='<%# Bind("Decorrenza", "{0:MM/yyyy}")%>' Width="70px" />
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtDecorrenza" Display="Dynamic"
                                        ControlToValidate="txtDecorrenza" Enabled="true" ErrorMessage="Decorrenza: Inserire una data valida"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloContrEsteriCI" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloContrEsteriCI"
                                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="29%" ItemStyle-Width="29%"
                                FooterStyle-Width="29%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSettimane" runat="server" MaxLength="4"
                                        Width="50px" Text='<%#Bind("Settimane") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtSettimane" ControlToValidate="txtSettimane"
                                        Display="Dynamic" ErrorMessage="Settimane: inserire il numero di settimane in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloContrEsteriCI" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;"
                                HeaderStyle-Width="5%" ItemStyle-Width="5%" FooterStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteContributiEsteri" CommandName="Elimina" CommandArgument="<% # ((GridViewRow)Container).RowIndex %>"
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
                </div>
            </td>
        </tr>
    </table>
    <asp:HiddenField runat="server" ID="modalitaEditContributiviEsteri" Value="false" />
</asp:Panel>
<!-- Fine Pannello relativo al GridView Dati Contributivi Esteri D.L. 503/92-->
<div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnPopUp" Style="display: none" runat="server" SkinID="btnAzione1"
                    CausesValidation="false" OnClick="btnSalvaDatiCalcolo_Click" Text="Salva Dati Calcolo"
                    Width="160px" OnClientClick="if(Page_ClientValidate('UCTabDatiCalcoloCI')){$('#dialog-confirm').dialog('open');return false;}" CssClass="primary" />
                <asp:Button ID="btnSalvaDatiCalcolo" runat="server" Enabled="true" SkinID="btnAzione1"
                    Text="Salva Dati Calcolo" Width="160px" OnClick="btnSalvaDatiCalcolo_Click" OnClientClick="if(Page_ClientValidate('UCTabDatiCalcoloCI')){aspnetForm.target ='_self'; BlockUI();}"
                    CausesValidation="false" CssClass="primary" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaDatiCalcolo" runat="server" Enabled="true" SkinID="btnAzione1"
                    Text="Elimina Dati Calcolo" Width="160px" OnClick="btnEliminaDatiCalcolo_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Calcolo?')) return false; else BlockUI();"
                    CausesValidation="false" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
<div id="dialog-confirm" title="Confirm" style="border-style: none; border-color: White;">
    <p>
        <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
        Attenzione il montante è inferiore all'ammontare - Confermare ?</p>
</div>
