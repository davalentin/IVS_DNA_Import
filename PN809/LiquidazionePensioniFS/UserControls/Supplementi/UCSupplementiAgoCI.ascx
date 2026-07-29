<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSupplementiAgoCI.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Supplementi.UCSupplementiAgoCI" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<script type="text/javascript">

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
                    document.getElementById('<%= btnSalvaTabSupplementi.ClientID %>').click();
                    return true;
                }
            }
        });
    });

</script>
<asp:Panel runat="server" ID="pnlSupplementi" Style="min-width: 720px">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblSuppFromPrelievo" runat="server" Text="Verificare i dati di tutti i supplementi prima della validazione."
                    Style="font-weight: bold" ForeColor="Red" Visible="false"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblRicNonContribNonSuppNonDoc" runat="server" Text="I dati dei supplementi sono disponibili per la sola visualizzazione.  Possono essere modificati con una Ricostituzione contributiva/documentale. Possono essere inseriti con una Ricostituzione per supplemento."
                    Style="font-weight: bold" ForeColor="Black" Visible="false"></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <asp:Panel runat="server" ID="pnlSupplementiAGOCIRetrib" Style="width: 720px">
        <!-- Grid Retributivi -->
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblSupplementiRetributivo" runat="server" Text="Supplementi retributivi"
                        Style="font-weight: bold" CssClass="section-label"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="tabellaContenuti">
            <tr>
                <td class="Row1">
                    <div class="bckGridViewElenco" style="width: 830px">
                        <asp:GridView runat="server" ID="gvSupplementi" SkinID="grdElenco1" AutoGenerateColumns="false"
                            CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                            OnRowEditing="gvSupplementi_RowEditing" Width="100%" PageSize="10" AllowPaging="true"
                            OnPageIndexChanging="gvSupplementi_onPageIndexChanging" OnRowCommand="gvSupplementi_RowCommand"
                            OnRowCancelingEdit="gvSupplementi_RowCancelingEdit" OnRowUpdating="gvSupplementi_RowUpdating"
                            OnRowDataBound="gvSupplementi_RowDataBound" OnRowDeleting="gvSupplementi_RowDeleting" PagerStyle-CssClass="default-pagination-tables">
                            <EmptyDataTemplate>
                                <center>
                                    <asp:Label ID="lblNoData" runat="server" Text="Nessun supplemento presente." SkinID="lblNoData"></asp:Label>
                                </center>
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:TemplateField HeaderText="Cod. Gestione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblTipoSupplementi" Text='<%#Bind("CodGestioneSupplemento")%>'
                                            Width="40px"> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlCodGestioneSupplementi"
                                            runat="server" Width="40px" AutoPostBack="true" OnSelectedIndexChanged="ddlCodGestioneSupplementi_SelectedIndexChanged"
                                            onchange="BlockUI();">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <%--<asp:CustomValidator runat="server" ID="requiredddlTipoSupplementi" ControlToValidate="ddlCodGestioneSupplementi"
                                            Display="Dynamic" ErrorMessage="Scegliere il tipo = 1" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiRetrib"
                                            ClientValidationFunction="checkddlTipoSupplementi" />--%>
                                        <asp:RequiredFieldValidator ID="RequiredFieldddlTipoSupplementi" runat="server" ErrorMessage="Tipo Supplementi: Campo obbligatorio"
                                            Text="*" CssClass="field-is-required" ControlToValidate="ddlCodGestioneSupplementi" ValidationGroup="UCTabSupplementiRetrib"></asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblQuotaSupplementi" Text='<%#Bind("QuotaSupplemento")%>'
                                            Width="50px"> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlQuotaSupplementi" runat="server"
                                            Width="50px" AutoPostBack="true" OnSelectedIndexChanged="ddlQuotaSupplementi_SelectedIndexChanged"
                                            onchange="BlockUI();">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                            <asp:ListItem Text="A" Value="A"></asp:ListItem>
                                            <asp:ListItem Text="B" Value="B"></asp:ListItem>
                                        </asp:DropDownList>
                                        <%--<asp:RequiredFieldValidator ID="RequiredFieldddlQuotaSupplementi" runat="server" ErrorMessage="Quota Supplementi: Campo obbligatorio"
                                            Text="*" CssClass="field-is-required" ControlToValidate="ddlQuotaSupplementi" ValidationGroup="UCTabSupplementiRetrib"></asp:RequiredFieldValidator>--%>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Tipo Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblTipoQuotaSupplementi" Width="50px"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList runat="server" ID="ddlTipoQuotaSupplementi" Width="50px" CssClass="txtUppercase tb8 xxs">
                                        </asp:DropDownList>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblDecorrenzaSupplementi" Text='<%# Bind("DecorrenzaSupplemento", "{0:MM/yyyy}")%>'
                                            Width="60px" CssClass="txtUppercase" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox CssClass="tb8 date-picker txtUppercase dateMMaaaa" runat="server" ID="txtDecorrenzaSupplementi"
                                            MaxLength="7" Text=' <%# Bind("DecorrenzaSupplemento", "{0:MM/yyyy}")%>'></asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ID="validateTxtDecorrenzaSupplementi"
                                            Display="Dynamic" ControlToValidate="txtDecorrenzaSupplementi" Enabled="true"
                                            ErrorMessage="Decorrenza Supplementi: Inserire una data valida" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiRetrib"
                                            ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldlblDecorrenzaSupplementi" runat="server"
                                            ErrorMessage="Decorrenza Supplementi: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtDecorrenzaSupplementi"
                                            ValidationGroup="UCTabSupplementiRetrib" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaSupplementi"
                                            Display="Dynamic" ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiRetrib"
                                            ID="customCheckDataDecorrenzaSupplementi" ClientValidationFunction="checkCorrettezzaData" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblSettimaneSupplementi" Text='<%#Bind("NSettimaneSupplemento")%>'> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSettimaneSupplementi" runat="server"
                                            Text='<%#Bind("NSettimaneSupplemento")%>' Width="50px" MaxLength="4">
                                        </asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ID="validatetxtSettimaneSupplementi"
                                            Display="Dynamic" ControlToValidate="txtSettimaneSupplementi" Enabled="true"
                                            ErrorMessage="Numero Settimane Supplementi: Inserire valori numerici" Text="*" CssClass="field-is-required"
                                            ValidationGroup="UCTabSupplementiRetrib" ValidationExpression="^[0-9]*$" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimaneSupplementi" runat="server"
                                            ErrorMessage="Numero Settimane Supplementi: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtSettimaneSupplementi"
                                            ValidationGroup="UCTabSupplementiRetrib"></asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="RMS" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblRMSSupplementi" Width="100px"> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox CssClass="tb8 txtUppercase" ID="txtRMSSupplementi" runat="server" Width="100px"
                                            MaxLength="14">
                                        </asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ID="validatetxtRMSSupplementiAGO"
                                            Display="Dynamic" ControlToValidate="txtRMSSupplementi" ErrorMessage="RMS Supplementi: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiRetrib" ValidationExpression="\d{0,7}(,\d{1,4})?"
                                            Enabled='<%# IsAGO() %>' />
                                        <asp:RegularExpressionValidator runat="server" ID="validatetxtRMSSupplementiCI" Display="Dynamic"
                                            ControlToValidate="txtRMSSupplementi" ErrorMessage="RMS Supplementi: inserire l'importo in formato valido (max 7 interi e 6 decimali)"
                                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiRetrib" ValidationExpression="\d{0,7}(,\d{1,6})?"
                                            Enabled='<%# IsCI() %>' />
                                        <%--<asp:RequiredFieldValidator ID="RequiredFieldtxtRMSSupplementi" runat="server" ErrorMessage="RMS: Campo obbligatorio"
                                            Text="*" CssClass="field-is-required" ControlToValidate="txtRMSSupplementi" ValidationGroup="UCTabSupplementiRetrib"></asp:RequiredFieldValidator>    --%>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cod. Liq." HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCodLiqSupplementi" Width="100px"> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlCodLiqSupplementi" runat="server"
                                            Width="35px">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                            <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                        </asp:DropDownList>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-CssClass="TblRecordset3"
                                    ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="1%" ItemStyle-Width="1%"
                                    FooterStyle-Width="1%">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDelete" CommandName="Delete" CommandArgument="Delete" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </td>
            </tr>
        </table>
        <!-- Fine Grid Retributivi -->
        <br />
        <br />
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlSupplementiAGOCIContrib" Style="width: 720px" class="mt-32">
        <!-- Grid Contributivi -->
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblSupplementiContributivi" runat="server" Text="Supplementi contributivi"
                        Style="font-weight: bold" CssClass="section-label"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="tabellaContenuti">
            <tr>
                <td class="Row1">
                    <div class="bckGridViewElenco" style="width: 830px">
                        <asp:GridView runat="server" ID="gvSupplementiContributivi" SkinID="grdElenco1" AutoGenerateColumns="false"
                            CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                            OnRowEditing="gvSupplementiContributivi_RowEditing" Width="100%" PageSize="10"
                            AllowPaging="true" OnPageIndexChanging="gvSupplementiContributivi_onPageIndexChanging"
                            OnRowCommand="gvSupplementiContributivi_RowCommand" OnRowCancelingEdit="gvSupplementiContributivi_RowCancelingEdit"
                            OnRowUpdating="gvSupplementiContributivi_RowUpdating" OnRowDataBound="gvSupplementiContributivi_RowDataBound"
                            OnRowDeleting="gvSupplementiContributivi_RowDeleting" OnDataBinding="gvSupplementiContributivi_DataBinding" PagerStyle-CssClass="default-pagination-tables">
                            <EmptyDataTemplate>
                                <center>
                                    <asp:Label ID="lblNoData" runat="server" Text="Nessun supplemento presente." SkinID="lblNoData"></asp:Label>
                                </center>
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:TemplateField HeaderText="Cod. Gestione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblTipoSupplementiContrib" Width="40px" Text='<%#Bind("CodGestioneSupplemento")%>'> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlCodGestioneSupplementiContrib"
                                            runat="server" Width="40px">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <%--<asp:CustomValidator runat="server" ID="requiredddlTipoSupplementi" ControlToValidate="ddlCodGestioneSupplementiContrib"
                                            Display="Dynamic" ErrorMessage="Scegliere il tipo = 1" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiContrib"
                                            ClientValidationFunction="checkddlTipoSupplementi" />--%>
                                        <asp:RequiredFieldValidator ID="RequiredFieldddlTipoSupplementi" runat="server" ErrorMessage="Tipo Supplementi: Campo obbligatorio"
                                            Text="*" CssClass="field-is-required" ControlToValidate="ddlCodGestioneSupplementiContrib" ValidationGroup="UCTabSupplementiContrib"></asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblQuotaSupplementi" Text='<%#Bind("QuotaSupplemento")%>'
                                            Width="50px"> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlQuotaSupplementi" runat="server"
                                            Width="50px" AutoPostBack="true" OnSelectedIndexChanged="ddlQuotaSupplementi_SelectedIndexChanged"
                                            onchange="BlockUI();">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                            <asp:ListItem Text="C" Value="C"></asp:ListItem>
                                            <asp:ListItem Text="D" Value="D"></asp:ListItem>
                                        </asp:DropDownList>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblDecorrenzaSupplementiContrib" Width="60px" Text='<%# Bind("DecorrenzaSupplemento", "{0:MM/yyyy}")%>'
                                            CssClass="txtUppercase" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox CssClass="tb8 date-picker txtUppercase dateMMaaaa" runat="server" ID="txtDecorrenzaSupplementiContrib"
                                            MaxLength="7" Text=' <%# Bind("DecorrenzaSupplemento", "{0:MM/yyyy}")%>'></asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ID="validateTxtDecorrenzaSupplementiContrib"
                                            Display="Dynamic" ControlToValidate="txtDecorrenzaSupplementiContrib" Enabled="true"
                                            ErrorMessage="Decorrenza Supplementi: Inserire una data valida" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiContrib"
                                            ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                        <%--<asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaSupplementi"
                                            Display="Dynamic" ErrorMessage="Decorrenza Supplementi: Data inserita posteriore a quella odierna"
                                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementi" ID="customDecorrenzaSupplementi" ClientValidationFunction="checkDataPostOdiernaMMAAAA" />--%>
                                        <%--<asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaSupplementiContrib"
                                            Display="Dynamic" ErrorMessage="Decorrenza Supplementi: Data inserita anteriore alla Data Decorrenza Pensione" Text="*" CssClass="field-is-required"
                                            ValidationGroup="UCTabSupplementiContrib" ID="customcheckDataDecorrenzaOriginaria" ClientValidationFunction="checkDataDecorrenzaOriginaria" />--%>
                                        <asp:RequiredFieldValidator ID="RequiredFieldlblDecorrenzaSupplementiContrib" runat="server"
                                            ErrorMessage="Decorrenza Supplementi: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtDecorrenzaSupplementiContrib"
                                            ValidationGroup="UCTabSupplementiContrib" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaSupplementiContrib"
                                            Display="Dynamic" ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiContrib"
                                            ID="customCheckDataDecorrenzaSupplemento" ClientValidationFunction="checkCorrettezzaData" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblSettimaneSupplementiContrib" Width="40px" Text='<%#Bind("NSettimaneSupplemento")%>'> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSettimaneSupplementiContrib" runat="server"
                                            Text='<%#Bind("NSettimaneSupplemento")%>' Width="40px" MaxLength="4">
                                        </asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ID="validatetxtSettimaneSupplementiContrib"
                                            Display="Dynamic" ControlToValidate="txtSettimaneSupplementiContrib" Enabled="true"
                                            ErrorMessage="Numero Settimane Supplementi: Inserire valori numerici" Text="*" CssClass="field-is-required"
                                            ValidationGroup="UCTabSupplementiContrib" ValidationExpression="^[0-9]*$" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimaneSupplementiContrib" runat="server"
                                            ErrorMessage="Numero Settimane Supplementi: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtSettimaneSupplementiContrib"
                                            ValidationGroup="UCTabSupplementiContrib"></asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Montante IVS " HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblMontanteIVS" Width="100px" Text='<%#Bind("MontanteSupplemento")%>'> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox CssClass="tb8 txtUppercase" ID="txtMontanteIVS" runat="server" Width="100px"
                                            MaxLength="12" Text='<%#Bind("MontanteSupplemento")%>'>
                                        </asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ID="validatetxtMontanteIVS" Display="Dynamic"
                                            ControlToValidate="txtMontanteIVS" Enabled="true" ErrorMessage="Montante IVS: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiContrib" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                                        <%--<asp:RequiredFieldValidator ID="RequiredFieldtxtMontanteIVS" runat="server" ErrorMessage="Montante IVS: Campo obbligatorio"
                                            Text="*" CssClass="field-is-required" ControlToValidate="txtMontanteIVS" ValidationGroup="UCTabSupplementiContrib"></asp:RequiredFieldValidator>  --%>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Ammontare contributi" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblAmmontareContributivo" Width="100px" Text='<%#Bind("AmmontareContributivo")%>'> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox CssClass="tb8 txtUppercase" ID="txtAmmontareContributivo" runat="server"
                                            Width="100px" MaxLength="12" Text='<%#Bind("AmmontareContributivo")%>'>
                                        </asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ID="validatetxtAmmontareContributivo"
                                            Display="Dynamic" ControlToValidate="txtAmmontareContributivo" Enabled="true"
                                            ErrorMessage="Ammontare dei contributi: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiContrib" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                                        <%--<asp:RequiredFieldValidator ID="RequiredFieldtxtAmmontareContributivo" runat="server" ErrorMessage="Ammontare dei contributi: Campo obbligatorio"
                                            Text="*" CssClass="field-is-required" ControlToValidate="txtAmmontareContributivo" ValidationGroup="UCTabSupplementiContrib"></asp:RequiredFieldValidator>   --%>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cod. Liq." HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCodiceLiquidazione" Width="35px" Text='<%#Bind("CodiceLiquidazione")%>'> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlCodiceLiquidazione" runat="server"
                                            Width="35px">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                            <asp:ListItem Text="C" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="D" Value="4"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceLiquidazione" runat="server"
                                            ErrorMessage="Codice Liquidazione: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="ddlCodiceLiquidazione"
                                            ValidationGroup="UCTabSupplementiContrib" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-CssClass="TblRecordset3"
                                    ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="1%" ItemStyle-Width="1%"
                                    FooterStyle-Width="1%">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDelete" CommandName="Delete" CommandArgument="Delete" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </td>
            </tr>
        </table>
        <!-- Fine Grid Contributivi -->
    </asp:Panel>

    <!-- Grid ante96 -->
    <asp:Panel runat="server" ID="pnlSupplementiAGOCIAnte96" Style="width: 720px">   
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblSupplementiAnte96" runat="server" Text=""
                        Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="tabellaContenuti">
            <tr>
                <td class="Row1">
                    <div class="bckGridViewElenco" style="width: 830px">
                        <asp:GridView runat="server" ID="gvSupplementiAnte96" SkinID="grdElenco1" AutoGenerateColumns="false"
                            CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                            OnRowEditing="gvSupplementiAnte96_RowEditing" Width="100%" PageSize="10" AllowPaging="true"
                            OnPageIndexChanging="gvSupplementiAnte96_PageIndexChanging" OnRowCommand="gvSupplementiAnte96_RowCommand"
                            OnRowCancelingEdit="gvSupplementiAnte96_RowCancelingEdit" OnRowUpdating="gvSupplementiAnte96_RowUpdating"
                            OnRowDataBound="gvSupplementiAnte96_RowDataBound" OnRowDeleting="gvSupplementiAnte96_RowDeleting" PagerStyle-CssClass="default-pagination-tables">
                            <EmptyDataTemplate>
                                <center>
                                    <asp:Label ID="lblNoData" runat="server" Text="Nessun supplemento presente." SkinID="lblNoData"></asp:Label>
                                </center>
                            </EmptyDataTemplate>
                            <Columns>
                                 <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblDecorrenzaSupplementi" Text='<%# Bind("DecorrenzaSupplemento", "{0:MM/yyyy}")%>'
                                            Width="60px" CssClass="txtUppercase" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox CssClass="tb8 date-picker txtUppercase dateMMaaaa" runat="server" ID="txtDecorrenzaSupplementi"
                                            MaxLength="7" Text=' <%# Bind("DecorrenzaSupplemento", "{0:MM/yyyy}")%>'></asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ID="validateTxtDecorrenzaSupplementi"
                                            Display="Dynamic" ControlToValidate="txtDecorrenzaSupplementi" Enabled="true"
                                            ErrorMessage="Decorrenza Supplementi: Inserire una data valida" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiContrib"
                                            ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldlblDecorrenzaSupplementi" runat="server"
                                            ErrorMessage="Decorrenza Supplementi: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtDecorrenzaSupplementi"
                                            ValidationGroup="UCTabSupplementiContrib" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaSupplementi"
                                            Display="Dynamic" ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiContrib"
                                            ID="customCheckDataDecorrenzaSupplementi" ClientValidationFunction="checkCorrettezzaData" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Codice Gestione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblTipoSupplementi" Text='<%#Bind("CodGestioneSupplemento")%>'
                                            Width="40px"> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlCodGestioneSupplementi"
                                            runat="server" Width="40px">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <%--<asp:CustomValidator runat="server" ID="requiredddlTipoSupplementi" ControlToValidate="ddlCodGestioneSupplementi"
                                            Display="Dynamic" ErrorMessage="Scegliere il tipo = 1" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiRetrib"
                                            ClientValidationFunction="checkddlTipoSupplementi" />--%>
                                        <asp:RequiredFieldValidator ID="RequiredFieldddlTipoSupplementi" runat="server" ErrorMessage="Tipo Supplementi: Campo obbligatorio"
                                            Text="*" CssClass="field-is-required" ControlToValidate="ddlCodGestioneSupplementi" ValidationGroup="UCTabSupplementiContrib"></asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblQuotaSupplementi" Text='<%#Bind("QuotaSupplemento")%>'
                                            Width="50px"> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlQuotaSupplementi" runat="server"
                                            Width="50px">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                            <asp:ListItem Text="A" Value="A"></asp:ListItem>
                                            <asp:ListItem Text="B" Value="B"></asp:ListItem>
                                        </asp:DropDownList>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="RMS Sentenza 72 IVS / Montante  " HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblMontanteIVS" Width="100px" Text='<%#Bind("MontanteSupplemento")%>'> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox CssClass="tb8 txtUppercase" ID="txtMontanteIVS" runat="server" Width="100px"
                                            MaxLength="12" Text='<%#Bind("MontanteSupplemento")%>'>
                                        </asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ID="validatetxtMontanteIVS" Display="Dynamic"
                                            ControlToValidate="txtMontanteIVS" Enabled="true" ErrorMessage="Montante IVS: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiContrib" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                                        <%--<asp:RequiredFieldValidator ID="RequiredFieldtxtMontanteIVS" runat="server" ErrorMessage="Montante IVS: Campo obbligatorio"
                                            Text="*" CssClass="field-is-required" ControlToValidate="txtMontanteIVS" ValidationGroup="UCTabSupplementiContrib"></asp:RequiredFieldValidator>  --%>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Settimane Giorni" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblSettimaneSupplementi" Text='<%#Bind("NSettimaneSupplemento")%>'> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSettimaneSupplementi" runat="server"
                                            Text='<%#Bind("NSettimaneSupplemento")%>' Width="50px" MaxLength="4">
                                        </asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ID="validatetxtSettimaneSupplementi"
                                            Display="Dynamic" ControlToValidate="txtSettimaneSupplementi" Enabled="true"
                                            ErrorMessage="Numero Settimane Supplementi: Inserire valori numerici" Text="*" CssClass="field-is-required"
                                            ValidationGroup="UCTabSupplementiContrib" ValidationExpression="^[0-9]*$" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimaneSupplementi" runat="server"
                                            ErrorMessage="Numero Settimane Supplementi: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtSettimaneSupplementi"
                                            ValidationGroup="UCTabSupplementiContrib"></asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                   <asp:TemplateField HeaderText="Retribuzione / Reddito medio " HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblRMSSupplementi" Width="100px" Text='<%#Bind("RMSSupplemento")%>'> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox CssClass="tb8 txtUppercase" ID="txtRMSSupplementi" runat="server" Width="100px"
                                            MaxLength="14" Text='<%#Bind("RMSSupplemento")%>'>
                                        </asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ID="validatetxtRMSSupplementiAGO"
                                            Display="Dynamic" ControlToValidate="txtRMSSupplementi" ErrorMessage="RMS Supplementi: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiContrib" ValidationExpression="\d{0,7}(,\d{1,4})?"
                                            Enabled='<%# IsAGO() %>' />                                     
                                        <%--<asp:RequiredFieldValidator ID="RequiredFieldtxtRMSSupplementi" runat="server" ErrorMessage="RMS: Campo obbligatorio"
                                            Text="*" CssClass="field-is-required" ControlToValidate="txtRMSSupplementi" ValidationGroup="UCTabSupplementiRetrib"></asp:RequiredFieldValidator>    --%>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                               
                               
                                <asp:TemplateField HeaderText="RMS articolo 2/ Importo Contributivo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblAmmontareContributivo" Width="100px" Text='<%#Bind("AmmontareContributivo")%>'> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox CssClass="tb8 txtUppercase" ID="txtAmmontareContributivo" runat="server"
                                            Width="100px" MaxLength="12" Text='<%#Bind("AmmontareContributivo")%>'>
                                        </asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ID="validatetxtAmmontareContributivo"
                                            Display="Dynamic" ControlToValidate="txtAmmontareContributivo" Enabled="true"
                                            ErrorMessage="Ammontare dei contributi: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiContrib" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                                        <%--<asp:RequiredFieldValidator ID="RequiredFieldtxtAmmontareContributivo" runat="server" ErrorMessage="Ammontare dei contributi: Campo obbligatorio"
                                            Text="*" CssClass="field-is-required" ControlToValidate="txtAmmontareContributivo" ValidationGroup="UCTabSupplementiContrib"></asp:RequiredFieldValidator>   --%>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cod. Liq." HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCodiceLiquidazione" Width="35px" Text='<%#Bind("CodiceLiquidazione")%>'> 
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlCodiceLiquidazione" runat="server"
                                            Width="35px">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                            <asp:ListItem Text="C" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="D" Value="4"></asp:ListItem>
                                        </asp:DropDownList>
                                     <%--   <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceLiquidazione" runat="server"
                                            ErrorMessage="Codice Liquidazione: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="ddlCodiceLiquidazione"
                                            ValidationGroup="UCTabSupplementiContrib" Display="Dynamic"></asp:RequiredFieldValidator>--%>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-CssClass="TblRecordset3"
                                    ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="1%" ItemStyle-Width="1%"
                                    FooterStyle-Width="1%">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDelete" CommandName="Delete" CommandArgument="Delete" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <!-- fine grid ante96 -->


    <asp:Panel ID="pnlIntegrazioneArt11" runat="server" Visible="false" Style="width: 720px">
        <br />
        <br />
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblIntegrazioneArt11" runat="server" Text="Integrazione Art.11 DPR N. 488/68"
                        Style="font-weight: bold" CssClass="section-label"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione grid grid-size-25">
            <tr>
                <td class="Row1" style="text-align: left; width: 25%">
                    <asp:Label ID="lblDecorrenza" runat="server" Text="Decorrenza:"></asp:Label>
                </td>
                <td class="field" style="text-align: left; width: 25%">
                    <asp:TextBox CssClass="tb8 date-picker txtUppercase dateMMaaaa" runat="server" ID="txtDecorrenza"
                        MaxLength="7" Text="MM/AAAA" Width="70%"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtDecorrenza" Display="Dynamic"
                        ControlToValidate="txtDecorrenza" Enabled="true" ErrorMessage="Decorrenza Integrazione Art.11: Inserire una data valida"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiAGO" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiAGO"
                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />
                </td>
                <td class="Row1" style="text-align: left; width: 25%">
                    <asp:Label ID="lblRenditafacolOrdinaria" runat="server" Text="Rendita facoltativa ordinaria: "></asp:Label>
                </td>
                <td class="field" style="text-align: left; width: 25%">
                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtRenditaFacolOrdinaria" runat="server"
                        Width="87%" MaxLength="15"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" Display="Dynamic"
                        ControlToValidate="txtRenditaFacolOrdinaria" Enabled="true" ErrorMessage="Rendita facoltativa ordinaria: Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiAGO" ValidationExpression="\d+(\,\d{1,4})?" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="text-align: left; width: 25%">
                    <asp:Label ID="lblImportoIVS" runat="server" Text="Importo IVS:"></asp:Label>
                </td>
                <td class="field" style="text-align: left; width: 25%">
                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtImportoIVS" runat="server" Width="87%"
                        MaxLength="15"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validatetxtImportoIVS" Display="Dynamic"
                        ControlToValidate="txtImportoIVS" Enabled="true" ErrorMessage="Importo IVS: Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiAGO" ValidationExpression="\d+(\,\d{1,4})?" />
                </td>
                <asp:Panel ID="pnlRenditafacolConv" runat="server" Visible="false">
                    <td class="Row1" style="text-align: left; width: 25%">
                        <asp:Label ID="lblRenditafacolConv" runat="server" Text="Rendita facoltativa convenzionale: "></asp:Label>
                    </td>
                    <td class="field" style="text-align: left; width: 25%">
                        <asp:TextBox CssClass="tb8 txtUppercase" ID="txtRenditafacolConv" runat="server"
                            Width="87%" MaxLength="15"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" Display="Dynamic"
                            ControlToValidate="txtRenditafacolConv" Enabled="true" ErrorMessage="Rendita facoltativa convenzionale: Inserire valori interi o decimali"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementiAGO" ValidationExpression="\d+(\,\d{1,4})?" />
                    </td>
                </asp:Panel>
            </tr>
        </table>
    </asp:Panel>
    <div id="pulsantiSaveDelete" class="containerWidth xl">
        <br />
        <br />
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: center" class="tab-actions-group__first">
                    <asp:Button ID="btnPopUp" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Style="display: none" Text="Salva Supplementi" Width="160px" OnClientClick="if(validatePage()){$('#dialog-confirm').dialog('open'); return false;}" CssClass="primary force-right" />
                    <asp:Button ID="btnSalvaTabSupplementi" runat="server" SkinID="btnAzione1" CommandArgument="CA_AgoCI"
                        CommandName="CN_AgoCI" Enabled="true" Text="Salva Supplementi" Width="160px"
                        OnClick="btnSalvaTabSupplementi_Click" OnClientClick="if(validatePage()){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary force-right" />
                    <asp:Button ID="btnEliminaTabSupplementi" runat="server" SkinID="btnAzione1" Enabled="true"
                        Text="Elimina Supplementi" Width="160px" OnClick="btnEliminaTabSupplementi_Click"
                        OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Supplementi?')) return false; else BlockUI();" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="modalitaEdit" Value="false" />
    <asp:HiddenField runat="server" ID="modalitaEditContrib" Value="false" />
    <asp:HiddenField runat="server" ID="modalitaEditAnte96" Value="false" />
</asp:Panel>
<div id="dialog-confirm" title="Confirm" style="border-style: none; border-color: White;">
    <p>
        <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
        Attenzione il Montante è inferiore all’Ammontare.<br />
        Confermare ?</p>
</div>
