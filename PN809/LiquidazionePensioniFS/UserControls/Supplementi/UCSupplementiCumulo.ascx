<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSupplementiCumulo.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Supplementi.UCSupplementiCumulo" %>
<asp:Panel runat="server" ID="pnlSupplementiCumulo">
    <div id="divSupplementiCumulo" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <br />
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblSupplementiCumulo"> Quote Supplementi:</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                </td>
            </tr>
            <tr>
                <td style="text-align: center;">
                    <asp:GridView runat="server" ID="gvQuoteSupplementi" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella" BorderWidth="1" Width="100%" BorderColor="Black"
                        OnRowCommand="gvQuoteSupplementi_RowCommand" OnRowDataBound="gvQuoteSupplementi_RowDataBound"
                        OnRowCancelingEdit="gvQuoteSupplementi_RowCancelingEdit" OnRowEditing="gvQuoteSupplementi_RowEditing"
                        OnDataBound="gvQuoteSupplementi_DataBound" OnDataBinding="gvQuoteSupplementi_DataBinding"
                        OnRowUpdating="gvQuoteSupplementi_RowUpdating" OnRowDeleting="gvQuoteSupplementi_RowDeleting"
                        EnableViewState="true">
                        <Columns>
                            <asp:CommandField ItemStyle-Width="6%" ShowEditButton="true" />
                            <asp:TemplateField HeaderText="Ente/Gestione Fondo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblEnteGestioneFondo_item" Width="100px" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlEnteGestioneFondo" Width="100px" CssClass="txtUppercase tb8 xxs">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RFVddlEnteGestioneFondo" runat="server" ErrorMessage="Ente/Gestione - Fondo : campo obbligatorio"
                                        Display="Dynamic" Text="*" CssClass="field-is-required" ControlToValidate="ddlEnteGestioneFondo" ValidationGroup="UCGvQuoteSupplementi"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:LinkButton ID="btnAggiungiQuote" CommandName="Aggiungi" runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Descrizione Fondo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="27%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDescrizioneFondo_item" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="17%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenzaQuota_item" CssClass="txtUppercase"></asp:Label>
                                    <asp:Label runat="server" ID="lblValueDecorrenzaQuota" CssClass="txtUppercase" Visible="false"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtDecorrenzaQuota" Text=' <%# Bind("Decorrenza", "{0:MM/yyyy}")%>'
                                        CssClass="tb8 txtUppercase date-picker dateMMaaaa" MaxLength="10"></asp:TextBox>
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaQuota" Display="Dynamic"
                                        ErrorMessage="La Decorrenza Quota inserita non è corretta" Text="*" CssClass="field-is-required" ValidationGroup="UCGvQuoteSupplementi"
                                        ID="customCheckDecorrenzaQuota" ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="10%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane_item" Width="50px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtSettimane" Width="50%" CssClass="txtUppercase tb8"
                                        Text=' <%# Bind("Settimane")%>' MaxLength="5" />
                                    <asp:RequiredFieldValidator ID="RFVtxtSettimane" runat="server" ErrorMessage="Settimane: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtSettimane" ValidationGroup="UCGvQuoteSupplementi"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator runat="server" ID="REVtxtSettimane" ControlToValidate="txtSettimane"
                                        Display="Dynamic" ErrorMessage="Settimane: inserire il numero di settimane  in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCGvQuoteSupplementi" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Importo Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblImportoQuota" Width="100px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtImportoQuota" Width="140px"
                                        CssClass="txtUppercase tb8 " MaxLength="16" Text=' <%# Bind("ImportoQuota")%>' />
                                    <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoQuota" ControlToValidate="txtImportoQuota"
                                        Display="Dynamic" ErrorMessage="Importo Quota: inserire l'importo in formato valido (max 8 interi e 7 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,8}(,\d{1,7})?" ValidationGroup="UCGvQuoteSupplementi" />
                                    <asp:RequiredFieldValidator ID="RFVtxtImportoQuota" runat="server" ErrorMessage="Importo Quota: Campo obbligatorio"
                                        Display="Dynamic" Text="*" CssClass="field-is-required" ControlToValidate="txtImportoQuota" ValidationGroup="UCGvQuoteSupplementi"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Adeguamento Pro Quota Casse" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblAdeguamentoProQuotaCasse_item" Width="90px" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlAdeguamentoProQuotaCasse" Width="90px" CssClass="txtUppercase tb8 xxs">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RFVddlAdeguamentoProQuotaCasse" runat="server" ErrorMessage="Adeguamento Pro Quota Casse : campo obbligatorio"
                                        Display="Dynamic" Text="*" CssClass="field-is-required" ControlToValidate="ddlAdeguamentoProQuotaCasse" ValidationGroup="UCGvQuoteSupplementi"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tipo Variazione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="15%"  Visible="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblTipoVariazione_item" Width="90px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField HeaderStyle-CssClass="intestazioneTabella Row1" ItemStyle-CssClass="TblRecordset3"
                                ItemStyle-Width="2%" ShowDeleteButton="true" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <div style="margin-top: 25px;">
        <table width="100%">
            <tr>
                <td style="text-align: right">
                    <asp:Button ID="btnSalvaSupplementiCumulo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Salva Quote Supplementi" Width="190px" OnClientClick="BlockUI()"
                        OnClick="btnSalvaSupplementiCumulo_Click" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaSupplementiCumulo" Style="text-align: center; padding-left: 0px;
                        padding-right: 0px" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elimina Quote Supplementi" Width="190px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Supplementi?')) return false; else BlockUI();"
                        OnClick="btnEliminaSupplementiCumulo_Click" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
