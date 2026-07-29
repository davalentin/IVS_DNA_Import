<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSupplementiENPALS.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Supplementi.UCSupplementiENPALS" %>
<asp:Panel runat="server" ID="pnlRecordSupplementiEnpals">
    <div style="min-height: 150px;">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblDatiSupplementi" Style="font-weight: bold">Dati Supplementi:</asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:GridView runat="server" ID="gvRecordSupplementiEnpals" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="900px" BorderColor="Black"
                        AutoGenerateEditButton="true" PageSize="10" AllowPaging="true" OnRowDataBound="gvRecordSupplementiENPALS_RowDataBound"
                        OnRowCommand="gvRecordSupplementiENPALS_RowCommand" OnRowCancelingEdit="gvRecordSupplementiENPALS_RowCancelingEdit"
                        OnRowEditing="gvRecordSupplementiENPALS_RowEditing" OnRowDeleting="gvRecordSupplementiENPALS_RowDeleting"
                        OnPageIndexChanging="gvRecordSupplementiENPALS_onPageIndexChanging" OnDataBound="gvRecordSupplementiENPALS_DataBound"
                        EnableViewState="true" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <Columns>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="2%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                                ItemStyle-CssClass="TblRecordset3" Visible="false">
                                <ItemTemplate>
                                    <asp:Image runat="server" ID="img" Visible="false" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="140px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtDecorrenza" MaxLength="7" CssClass="tb8 date-picker txtUppercase dateMMaaaa"></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="REVtxtDecorrenza" Display="Dynamic"
                                        ControlToValidate="txtDecorrenza" Enabled="true" ErrorMessage="Decorrenza: Inserire una data valida"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabRecordSupplementiENPALS" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabRecordSupplementiENPALS"
                                        ID="CVtxtDecorrenza" ClientValidationFunction="checkCorrettezzaData" />
                                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtDecorrenza" ControlToValidate="txtDecorrenza"
                                        Enabled="true" ErrorMessage="Decorrenza: campo obbligatorio" Text="*" CssClass="field-is-required" Display="Dynamic"
                                        ValidationGroup="UCTabRecordSupplementiENPALS" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Inizio Supplemento" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="140px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblInizioSupplemento"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtInizioSupplemento" MaxLength="14" CssClass="tb8 date-picker-base txtUppercase dateGGmmAAAA"></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="REVtxtInizioSupplemento" Display="Dynamic"
                                        ControlToValidate="txtInizioSupplemento" Enabled="true" ErrorMessage="Inizio Supplemento: Inserire una data valida"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabRecordSupplementiENPALS" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtInizioSupplemento" Display="Dynamic"
                                        ErrorMessage="Inizio Supplemento: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabRecordSupplementiENPALS"
                                        ID="CVtxtInizioSupplemento" ClientValidationFunction="checkCorrettezzaData" />
                                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtInizioSupplemento" ControlToValidate="txtInizioSupplemento"
                                        Enabled="true" ErrorMessage="Inizio Supplemento: campo obbligatorio" Text="*" CssClass="field-is-required"
                                        Display="Dynamic" ValidationGroup="UCTabRecordSupplementiENPALS" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Fine Supplemento" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="140px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblFineSupplemento"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtFineSupplemento" MaxLength="14" CssClass="tb8 date-picker-base txtUppercase dateGGmmAAAA"></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="REVtxtFineSupplemento" Display="Dynamic"
                                        ControlToValidate="txtFineSupplemento" Enabled="true" ErrorMessage="Fine Supplemento: Inserire una data valida"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabRecordSupplementiENPALS" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtFineSupplemento" Display="Dynamic"
                                        ErrorMessage="Fine Supplemento: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabRecordSupplementiENPALS"
                                        ID="CVtxtFineSupplemento" ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Importo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="130px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblImporto" Text='<%#Bind("Importo")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label runat="server" ID="lblImporto" Text='<%#Bind("Importo")%>'></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-CssClass="TblRecordset3"
                                ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="1%" ItemStyle-Width="1%"
                                FooterStyle-Width="1%" Visible="false">
                                <ItemTemplate>
                                    <asp:Button runat="server" ID="btnDettaglio" CommandName="Dettaglio" Text="Dettaglio"
                                        SkinID="btnAzione1" OnClientClick="BlockUI();" Visible="false"  CssClass="tertiary viewIconOnly" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-CssClass="TblRecordset3"
                                ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="1%" ItemStyle-Width="1%"
                                FooterStyle-Width="1%" Visible="false">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDelete" CommandName="Delete" CommandArgument="Delete" runat="server"
                                        Visible="false" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <div id="pulsanteEliminaTutto" style="width: 720px">
        <table width="100%">
            <tr>
                <td style="text-align: center">
                    <asp:Button ID="btnEliminaTutto" runat="server" SkinID="btnAzione1" Enabled="true"
                        Text="Elimina Tutto" Width="150px" OnClick="btnEliminaRecordSupplementi_Click"
                        OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Supplementi?')) return false; else BlockUI();" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<asp:Panel runat="server" ID="pnlSupplementiEnpals" Visible="false">
    <asp:Panel runat="server" ID="pnlGridSupplementiRetribEnpals">
        <!-- Grid Retributivi -->
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblDatiRetributivi" Style="font-weight: bold">Supplementi Retributivi:</asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:GridView runat="server" ID="gvSupplementiENPALS" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="900px" BorderColor="Black"
                        AutoGenerateEditButton="true" PageSize="10" AllowPaging="true" OnRowDataBound="gvSupplementiENPALS_RowDataBound"
                        OnRowCommand="gvSupplementiENPALS_RowCommand" OnRowCancelingEdit="gvSupplementiENPALS_RowCancelingEdit"
                        OnRowEditing="gvSupplementiENPALS_RowEditing" OnRowDeleting="gvSupplementiENPALS_RowDeleting"
                        OnPageIndexChanging="gvSupplementiENPALS_onPageIndexChanging" EnableViewState="true" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <Columns>
                            <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="60px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuota"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlQuota" Width="35px" CssClass="tb8 txtUppercase xxs">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="A" Value="A"></asp:ListItem>
                                        <asp:ListItem Text="B" Value="B"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ID="RFVddlQuota" ControlToValidate="ddlQuota"
                                        Enabled="true" ErrorMessage="Quota: campo obbligatorio" Text="*" CssClass="field-is-required" Display="Dynamic"
                                        ValidationGroup="UCTabSupplementiENPALSRetrib" InitialValue="" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="140px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza" Text='<%#Bind("Decorrenza", "{0:MM/yyyy}")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza"></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Numero Contributi" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="60px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblNSettimane"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtNSettimane" CssClass="tb8 txtUppercase" MaxLength="4"
                                        Width="45px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="REVtxtNSettimane" ControlToValidate="txtNSettimane"
                                        ErrorMessage="Numero Contributi in formato non valido" ValidationExpression="^[0-9]+$"
                                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabSupplementiENPALSRetrib"
                                        Enabled="true" />
                                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtNSettimane" ControlToValidate="txtNSettimane"
                                        Enabled="true" ErrorMessage="Numero Contributi: campo obbligatorio" Text="*" CssClass="field-is-required"
                                        Display="Dynamic" ValidationGroup="UCTabSupplementiENPALSRetrib" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Numero Totale Contrib. Calcolo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="140px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblNTotaleContributiCalcolo"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtNTotaleContributiCalcolo" CssClass="tb8 txtUppercase"
                                        MaxLength="4" Width="55px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="REVtxtNTotaleContributiCalcolo" ControlToValidate="txtNTotaleContributiCalcolo"
                                        ErrorMessage="Numero Totale Contrib. Calcolo in formato non valido" ValidationExpression="^[0-9]+$"
                                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabSupplementiENPALSRetrib"
                                        Enabled="true" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Retribuzione Media" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="120px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRM"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtRM" CssClass="tb8 txtUppercase" MaxLength="12"
                                        Width="100px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="REVtxtRM" ControlToValidate="txtRM" ErrorMessage="Retribuzione Media in formato non valido (7 interi e 4 decimali)"
                                        ValidationExpression="\d{1,7}(\,\d{1,4})?$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
                                        ValidationGroup="UCTabSupplementiENPALSRetrib" Enabled="true" />
                                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtRM" ControlToValidate="txtRM"
                                        Enabled="true" ErrorMessage="Retribuzione Media: campo obbligatorio" Text="*" CssClass="field-is-required"
                                        Display="Dynamic" ValidationGroup="UCTabSupplementiENPALSRetrib" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Importo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="130px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblImporto"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtImporto" CssClass="tb8 txtUppercase" MaxLength="13"
                                        Width="110px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="REVtxtImporto" ControlToValidate="txtImporto"
                                        ErrorMessage="Importo in formato non valido (9 interi e 3 decimali)" ValidationExpression="\d{1,9}(\,\d{1,3})?$"
                                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabSupplementiENPALSRetrib"
                                        Enabled="true" />
                                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtImporto" ControlToValidate="txtImporto"
                                        Enabled="true" ErrorMessage="Importo: campo obbligatorio" Text="*" CssClass="field-is-required" Display="Dynamic"
                                        ValidationGroup="UCTabSupplementiENPALSRetrib" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Importo pro rata temporis" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="130px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblImportoProRataTemporis"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtImportoProRataTemporis" CssClass="tb8 txtUppercase"
                                        MaxLength="13" Width="110px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="REVtxtImportoProRataTemporis" ControlToValidate="txtImportoProRataTemporis"
                                        ErrorMessage="Importo pro rata temporis in formato non valido (9 interi e 3 decimali)"
                                        ValidationExpression="\d{1,9}(\,\d{1,3})?$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
                                        ValidationGroup="UCTabSupplementiENPALSRetrib" Enabled="true" />
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
                </td>
            </tr>
        </table>
        <!-- Fine Grid Retributivi -->
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlGridSupplementiContribEnpals">
        <!-- Grid Contributivi -->
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblCalcoloContributivo" Style="font-weight: bold">Supplementi Contributivi:</asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:GridView ID="gvSupplementiContributiviENPALS" runat="server" AllowPaging="true"
                        AutoGenerateColumns="false" AutoGenerateEditButton="true" BorderColor="Black"
                        BorderWidth="1" CssClass="intestazioneTabella intestazioneTabella__with-pagination" EnableViewState="true" OnRowDataBound="gvSupplementiContributiviENPALS_RowDataBound"
                        OnRowCommand="gvSupplementiContributiviENPALS_RowCommand" OnRowCancelingEdit="gvSupplementiContributiviENPALS_RowCancelingEdit"
                        OnRowEditing="gvSupplementiContributiviENPALS_RowEditing" OnRowDeleting="gvSupplementiContributiviENPALS_RowDeleting"
                        OnPageIndexChanging="gvSupplementiContributiviENPALS_onPageIndexChanging" PageSize="10"
                        SkinID="grdElenco1" Width="900px" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <Columns>
                            <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="60px" >
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuota" Width="40px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlQuota" Width="40px" CssClass="txtUppercase tb8 xxs">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="C" Value="C"></asp:ListItem>
                                        <asp:ListItem Text="D" Value="D"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlQuotaContrib" runat="server" ErrorMessage="Quota: Campo obbligatorio"
                                        Text="*" ControlToValidate="ddlQuota" ValidationGroup="UCTabSupplementiENPALSContrib"
                                        CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="150px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza" Text='<%#Bind("Decorrenza", "{0:MM/yyyy}")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza"></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Importo Quota Contributivo"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="215px">
                                <ItemTemplate>
                                    <asp:Label ID="lblImportoContributivoTotale" runat="server"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtImportoContributivoTotale" CssClass="tb8 txtUppercase"
                                        MaxLength="12" Width="100px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="REVtxtImportoContributivoTotale" ControlToValidate="txtImportoContributivoTotale"
                                        ErrorMessage="Importo Quota Contributivo in formato non valido (7 interi e 4 decimali)"
                                        ValidationExpression="\d{1,7}(\,\d{1,4})?$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
                                        ValidationGroup="UCTabSupplementiENPALSContrib" Enabled="true" />
                                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtImportoContributivoTotale" ControlToValidate="txtImportoContributivoTotale"
                                        Enabled="true" ErrorMessage="Importo Quota Contributivo: campo obbligatorio"
                                        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabSupplementiENPALSContrib" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Montante Complessivo"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="165px">
                                <ItemTemplate>
                                    <asp:Label ID="lblMontante" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtMontante" CssClass="tb8 txtUppercase" MaxLength="12"
                                        Width="100px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="REVtxtMontante" ControlToValidate="txtMontante"
                                        ErrorMessage="Montante Complessivo in formato non valido (7 interi e 4 decimali)"
                                        ValidationExpression="\d{1,7}(\,\d{1,4})?$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
                                        ValidationGroup="UCTabSupplementiENPALSContrib" Enabled="true" />
                                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtMontante" ControlToValidate="txtMontante"
                                        Enabled="true" ErrorMessage="Montante Complessivo: campo obbligatorio" Text="*" CssClass="field-is-required"
                                        Display="Dynamic" ValidationGroup="UCTabSupplementiENPALSContrib" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Coefficiente di Trasformazione"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="205px">
                                <ItemTemplate>
                                    <asp:Label ID="lblCoefficienteTrasformazione" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtCoefficienteTrasformazione" CssClass="tb8 txtUppercase"
                                        MaxLength="11" Width="95px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="REVtxtCoefficienteTrasformazione" ControlToValidate="txtCoefficienteTrasformazione"
                                        ErrorMessage="Coefficiente di Trasformazione in formato non valido (4 interi e 6 decimali)"
                                        ValidationExpression="\d{1,4}(\,\d{1,6})?$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
                                        ValidationGroup="UCTabSupplementiENPALSContrib" Enabled="true" />
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
                </td>
            </tr>
        </table>
        <!-- Fine Grid Contributivi -->
    </asp:Panel>
    <!-- Il Pannello non è visibile perchè al momento non deve essere gestito. Sarà gestito in futuro -->
    <asp:Panel ID="pnlIntegrazioneArt11" runat="server" Style="width: 720px" Visible="false">
        <br />
        <br />
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblIntegrazioneArt11" runat="server" Text="Integrazione Art.11 DPR N. 488/68"
                        Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione">
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
                <asp:Panel ID="pnlRenditafacolConv" runat="server">
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
                    <asp:Button ID="btnSalvaDettaglioSupplementi" runat="server" SkinID="btnAzione1"
                        CommandArgument="CA_AgoCI" CommandName="CN_AgoCI" Enabled="true" Text="Salva Supplementi"
                        Width="160px" OnClick="btnSalvaDettaglioSupplementi_Click" OnClientClick="if(validatePage()){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary force-right" />
                    <asp:Button ID="btnEliminaDettaglioSupplementi" runat="server" SkinID="btnAzione1"
                        Enabled="true" Text="Elimina Supplementi" Width="160px" OnClick="btnEliminaDettaglioSupplementi_Click"
                        OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Supplementi?')) return false; else BlockUI();" CssClass="ghost-delete" />
                    <asp:Button ID="btnTornaElencoSupplementi" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elenco Supplementi" Width="160px" OnClick="TornaElencoSupplementi_Click"
                        OnClientClick="BlockUI();" CssClass="tertiary" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<asp:HiddenField runat="server" ID="modalitaEditRecordSupplementi" Value="false" />
<asp:HiddenField runat="server" ID="modalitaEditENPALS" Value="false" />
<asp:HiddenField runat="server" ID="modalitaEditContribENPALS" Value="false" />
