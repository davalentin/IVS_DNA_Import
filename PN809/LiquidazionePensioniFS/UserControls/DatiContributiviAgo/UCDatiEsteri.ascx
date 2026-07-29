<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiEsteri.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo.UCDatiEsteri" %>
<script type="text/javascript">

    function GetHdnIsInProrata() {
        var isInProrata = "false";
        if (document.getElementById("<%=hdnIsInProrata.ClientID%>") != null) {
            isInProrata = document.getElementById("<%=hdnIsInProrata.ClientID%>").value;
        }
        return isInProrata;
    }

</script>
<asp:Panel ID="pnlTable" runat="server">
    <%-- Tabella visualizzazione elenco contribuzioni estere--%>
    <asp:Panel ID="pnlDatiEsteriEditMode" runat="server" Visible="false">
        <table class="tabellaContenuti">
            <tr>
                <td class="Row1">
                    <asp:GridView ID="gvDatiEsteri" Visible="true" runat="server" AllowPaging="true"
                        AutoGenerateColumns="false" AutoGenerateEditButton="true" BorderColor="Black"
                        BorderWidth="1" CssClass="intestazioneTabella intestazioneTabella__with-pagination" EnableViewState="true" OnRowCancelingEdit="gvDatiEsteri_RowCancelingEdit"
                        OnRowCommand="gvDatiEsteri_RowCommand" OnRowDataBound="gvDatiEsteri_RowDataBound"
                        OnRowEditing="gvDatiEsteri_RowEditing" OnRowUpdating="gvDatiEsteri_RowUpdating"
                        PageSize="10" SkinID="grdElenco1" Width="100%" OnPageIndexChanging="gvDatiEsteri_onPageIndexChanging"  PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
                            <asp:TemplateField HeaderText="Codice Stato" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceStato" Width="70%" CssClass="txtUppercase"
                                        Text='<%#Bind("strCodiceStato") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtCodiceStato" Width="70%" CssClass="txtUppercase tb8"
                                        MaxLength="2" Text='<%#Bind("strCodiceStato") %>'>                                       
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtCodiceStato" runat="server" ErrorMessage="Codice Stato: campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtCodiceStato" ValidationGroup="UCTabDatiEsteriGrid2"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtCodiceStato" ControlToValidate="txtCodiceStato"
                                        Display="Dynamic" ErrorMessage="Codice Stato: inserire la categoria in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiEsteriGrid2" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Codice Istituzione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceIstituzione" Width="70%" CssClass="txtUppercase"
                                        Text='<%#Bind("strCodiceIstituzione") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtCodiceIstituzione" Width="70%" CssClass="txtUppercase tb8"
                                        MaxLength="4" Text='<%#Bind("strCodiceIstituzione") %>'>                                       
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtCodiceIstituzione" runat="server"
                                        ErrorMessage="Codice Istituzione: campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtCodiceIstituzione"
                                        ValidationGroup="UCTabDatiEsteriGrid2" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtCodiceIstituzione" ControlToValidate="txtCodiceIstituzione"
                                        Display="Dynamic" ErrorMessage="Codice Istituzione: inserire la categoria in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiEsteriGrid2" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="2%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Image runat="server" ID="img" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella">
                                <ItemTemplate>
                                    <asp:Button runat="server" ID="btnRicerca" Enabled="false" Text="Dettaglio" CommandName="Ricerca"
                                        CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" SkinID="btnAzione1"  CssClass="tertiary viewIconOnly" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteStati" CommandName="Elimina" CommandArgument="<% # ((GridViewRow) Container).RowIndex %>"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:HiddenField runat="server" ID="modalitaEditStatiEsteri" Value="false" />
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div id="divStati" class="bckGridViewElenco" style="width: 700px">
                    <asp:GridView ID="gvIstituzioniEstere" SkinID="grdElenco1" BorderWidth="1" BorderColor="Black"
                        DataKeyNames="Id" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                        CellPadding="4" ForeColor="#333333" GridLines="None" Width="100%" OnDataBinding="gvIstituzioniEstere_DataBinding"
                        OnRowCommand="gvIstituzioniEstere_RowCommand" OnRowDataBound="gvIstituzioniEstere_RowDataBound">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessun dato trovato." SkinID="lblNoData"
                                    Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="2%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Image runat="server" ID="img" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField HeaderText="Stato" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="38%"
                                HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                                DataField="nomeStato"></asp:BoundField>
                            <asp:BoundField HeaderText="Istituzione" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="32%"
                                HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                                DataField="codiceIstituzione" DataFormatString="{0:dd/MM/yyyy}"></asp:BoundField>
                            <asp:BoundField DataField="id" HeaderText="Id" Visible="False" />
                            <asp:TemplateField HeaderText="Operazione" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="30%"
                                ControlStyle-CssClass="pulsante1 editIconOnly tertiary">
                                <ItemTemplate>
                                    <asp:Button runat="server" ID="btnModifica" Text="Modifica" CommandName="modifica"
                                        CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" SkinID="btnAzione1" class="editIconOnly tertiary" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="intestazioneTabella Row1" />
                                <ItemStyle Width="26%" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    <%--Fine tabelle visualizzazione elenco contribuzioni estere--%>
</asp:Panel>
<asp:Panel ID="editpan" runat="server" Visible="False">
    <table id="editab" width="100%" class="tabellaFormattazione">
        <tr>
            <td class="Row1" colspan="4">
                <asp:Label ID="lblIdPrestazioneEE" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Stato:</label>
            </td>
            <td class="Row1 full-grid" colspan="3">
                <asp:Label ID="lblCodiceStatoEE" runat="server" />
                <asp:Label ID="lblNomeStato" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Istituzione:</label>
            </td>
            <td class="Row1 full-grid" colspan="3">
                <asp:Label ID="lblCodiceIstituzione" runat="server" />
                <asp:Label ID="lblSigla" runat="server" />
                <asp:Label ID="lblCitta" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Settimane misura:</label>
            </td>
            <td class="Row1 ">
                <asp:TextBox ID="txtSettimaneMisuraDecorrenzaPensione" Width="70px" runat="server"
                    CssClass="tb8 txtUppercase" MaxLength="4" TabIndex="3"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RESettimaneMisuraDecorrenzaPensione" ControlToValidate="txtSettimaneMisuraDecorrenzaPensione"
                    ErrorMessage="Settimane misura a decorrenza pensione: settimane in formato non valido"
                    ValidationExpression="^[0-9]+" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiEsteri"
                    Enabled="true" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Contributi Diritto:</label>
            </td>
            <td class="Row1 ">
                <asp:TextBox ID="txtSettimaneDiritto" runat="server" Width="70px" CssClass="tb8 txtUppercase"
                    MaxLength="4" TabIndex="5"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RESettimaneDiritto" ControlToValidate="txtSettimaneDiritto"
                    ErrorMessage="Settimane diritto: settimane in formato non valido" ValidationExpression="^[0-9]+"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiEsteri" Enabled="true" />
            </td>
        </tr>
    </table>
    <asp:Panel ID="pnlImportiEsteri" runat="server">
        <table class="tabellaContenuti">
            <tr>
                <td class="Row1">
                    <div id="divImportiEsteri" class="bckGridViewElenco" style="width: 700px">
                        <asp:GridView runat="server" ID="gvImportiEsteri" SkinID="grdElenco1" AutoGenerateColumns="false"
                            CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="100%" BorderColor="Black"
                            AutoGenerateEditButton="true" PageSize="10" AllowPaging="true" OnRowCommand="gvImportiEsteri_RowCommand"
                            OnRowDataBound="gvImportiEsteri_RowDataBound" OnRowCancelingEdit="gvImportiEsteri_RowCancelingEdit"
                            OnRowEditing="gvImportiEsteri_RowEditing" OnRowUpdating="gvImportiEsteri_RowUpdating"
                            OnPageIndexChanging="gvImportiEsteri_onPageIndexChanging" EnableViewState="true"  PagerStyle-CssClass="default-pagination-tables">
                            <Columns>
                                <asp:TemplateField HeaderText="Decorrenza" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblDecorrenzaPrestazioneEE" Text='<%#Bind("strDecorrenzaPrestazione", "{0:MM/yyyy}")%>'
                                            Width="100px"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox runat="server" ID="txtDecorrenzaPrestazioneEE" CssClass="tb8 date-picker txtUppercase dateMMaaaa"
                                            Text='<%#Bind("strDecorrenzaPrestazione", "{0:MM/yyyy}")%>' Width="100px" MaxLength="7">      
                                        </asp:TextBox>
                                        <asp:RequiredFieldValidator ID="validateDecorrenzaPrestazioneEE" runat="server" ErrorMessage="Decorrenza Prestazione Estera: dato obbligatorio"
                                            Display="Dynamic" Text="*" CssClass="field-is-required" ControlToValidate="txtDecorrenzaPrestazioneEE" ValidationGroup="UCTabDatiEsteriGrid"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator runat="server" ID="REDecorrenzaPrestazioneEE" ControlToValidate="txtDecorrenzaPrestazioneEE"
                                            Display="Dynamic" Enabled="true" ErrorMessage="Decorrenza Prestazione Estera: inserire la data nel formato valido"
                                            ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiEsteriGrid"
                                            Text="*" CssClass="field-is-required" />
                                        <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaPrestazioneEE"
                                            Display="Dynamic" ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiEsteriGrid"
                                            ID="customCheckDataDecorrenzaPrestazioneEE" ClientValidationFunction="checkCorrettezzaData" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Importo Prestazione" ItemStyle-HorizontalAlign="Center"
                                    HeaderStyle-CssClass="intestazioneTabella Row1" ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblImportoPrestazioneEE" Text='<%#Bind("strImportoPrestazione")%>'
                                            Width="120px"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox runat="server" ID="txtImportoPrestazioneEE" Text='<%#Bind("strImportoPrestazione")%>'
                                            CssClass="tb8 txtUppercase" Width="120px" MaxLength="26"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="validateImportoPrestazioneEE" runat="server" ErrorMessage="Importo Prestazione Estera: dato obbligatorio"
                                            Display="Dynamic" Text="*" CssClass="field-is-required" ControlToValidate="txtImportoPrestazioneEE" ValidationGroup="UCTabDatiEsteriGrid"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="REImportoPrestazioneEE" ControlToValidate="txtImportoPrestazioneEE"
                                            ErrorMessage="Importo Prestazione Estera: importo in formato non valido" ValidationExpression="^[0-9]+\,[0-9]+|[0-9]+$"
                                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiEsteriGrid"
                                            Enabled="true" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cessazione" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCessazionePrestazioneEE" Text='<%#Bind("strCessazionePrestazione", "{0:MM/yyyy}")%>'
                                            Width="100px"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox runat="server" ID="txtCessazionePrestazioneEE" Text='<%#Bind("strCessazionePrestazione", "{0:MM/yyyy}")%>'
                                            CssClass="tb8 txtUppercase date-picker dateMMaaaa" Width="100px" MaxLength="7"></asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ID="RECessazionePrestazioneEE" ControlToValidate="txtCessazionePrestazioneEE"
                                            Display="Dynamic" Enabled="true" ErrorMessage="Cessazione Prestazione Estera: inserire la data nel formato valido"
                                            ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiEsteriGrid"
                                            Text="*" CssClass="field-is-required" />
                                        <asp:CustomValidator runat="server" ControlToValidate="txtCessazionePrestazioneEE"
                                            Display="Dynamic" ErrorMessage="Cessazione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiEsteriGrid"
                                            ID="customCheckDataCessazionePrestazioneEE" ClientValidationFunction="checkCorrettezzaData" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDelete" CommandName="Elimina" CommandArgument="<% # ((GridViewRow) Container).RowIndex %>"
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:HiddenField runat="server" ID="modalitaEditImporti" Value="false" />
    <div style="margin-top: 25px;">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right;" class="tab-actions-group__first">
                    <asp:Button ID="btnConfermaModifiche" SkinID="btnAzione1" runat="server" Text="Salva Stato"
                        OnClick="ConfermaModifiche_Click" ValidationGroup="UCTabDatiEsteri" Width="160px"
                        OnClientClick="if(Page_ClientValidate('UCTabDatiEsteri')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
                </td>
                <td style="text-align: left;">
                    <asp:Button ID="btnAnnullaModifiche" SkinID="btnAzione1" runat="server" Text="Indietro"
                        OnClick="AnnullaModifiche_Click" Width="160px" OnClientClick="BlockUI()" />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="hdnNRecordProrata" Value="" />
    <asp:HiddenField runat="server" ID="hdnIsInProrata" Value="" />
</asp:Panel>
<div runat="server" style="margin-top: 25px;" id="divEliminaProrata" visible="false">
    <table width="100%">
        <tr>
            <td colspan="4" style="text-align: center;">
                <asp:Button ID="btnCancelProRata" SkinID="btnAzione1" runat="server" Text="Elimina e Riacquisisci Stati"
                    Style="padding-left: 0px; padding-right: 0px;" CausesValidation="false" OnClick="btnCancelProRata_Click"
                    Width="200px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i dati di Pro Rata Estera?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
