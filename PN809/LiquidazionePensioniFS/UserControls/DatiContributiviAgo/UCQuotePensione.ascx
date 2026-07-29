<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCQuotePensione.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo.UCQuotePensione" %>
<asp:Panel runat="server" ID="pnlQuotenPensione">
    <div id="divQuotePensione" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <br />
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblQuotePensione"> Quote Pensione:</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                </td>
            </tr>
            <tr>
                <td style="text-align: center;">
                    <asp:GridView runat="server" ID="gvQuotePensione" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella" BorderWidth="1" Width="100%" BorderColor="Black"
                        OnRowCommand="gvQuotePensione_RowCommand" OnRowDataBound="gvQuotePensione_RowDataBound"
                        OnRowCancelingEdit="gvQuotePensione_RowCancelingEdit" OnRowEditing="gvQuotePensione_RowEditing"
                        OnDataBound="gvQuotePensione_DataBound" OnDataBinding="gvQuotePensione_DataBinding"
                        OnRowUpdating="gvQuotePensione_RowUpdating" OnRowDeleting="gvQuotePensione_RowDeleting"
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
                                        Display="Dynamic" Text="*" CssClass="field-is-required" ControlToValidate="ddlEnteGestioneFondo" ValidationGroup="UCGvQuotePensione"></asp:RequiredFieldValidator>
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
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="17%" Visible="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenzaQuota_item" CssClass="txtUppercase"></asp:Label>
                                    <asp:Label runat="server" ID="lblValueDecorrenzaQuota" CssClass="txtUppercase" Visible="false"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtDecorrenzaQuota" Text=' <%# Bind("Decorrenza", "{0:dd/MM/yyyy}")%>'
                                        CssClass="tb8 txtUppercase date-picker-base dateGGmmAAAA" MaxLength="10"></asp:TextBox>
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaQuota" Display="Dynamic"
                                        ErrorMessage="La Decorrenza Quota inserita non è corretta" Text="*" CssClass="field-is-required" ValidationGroup="UCGvQuotePensione"
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
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtSettimane" ValidationGroup="UCGvQuotePensione"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator runat="server" ID="REVtxtSettimane" ControlToValidate="txtSettimane"
                                        Display="Dynamic" ErrorMessage="Settimane: inserire il numero di settimane  in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabQuotePensioni" />
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
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,8}(,\d{1,7})?" ValidationGroup="UCGvQuotePensione" />
                                    <asp:RequiredFieldValidator ID="RFVtxtImportoQuota" runat="server" ErrorMessage="Importo Quota: Campo obbligatorio"
                                        Display="Dynamic" Text="*" CssClass="field-is-required" ControlToValidate="txtImportoQuota" ValidationGroup="UCGvQuotePensione"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField HeaderStyle-CssClass="intestazioneTabella Row1" ItemStyle-CssClass="TblRecordset3"
                                ItemStyle-Width="2%" ShowDeleteButton="true" />
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" ItemStyle-CssClass="TblRecordset3"
                                ItemStyle-Width="4%" Visible="false">
                                <ItemTemplate>
                                    <asp:Image ID="imgVisualizzaTrattenute" alt="Visualizza dati trattenute" title="Visualizza dati trattenute"
                                        Style="cursor: pointer" src="../App_Themes/<%= Page.Theme %>/Images/plus.png" runat="server" />
                                    <asp:HiddenField ID="hdnVisualizzaTrattenute" runat="server" />
                                    </td></tr><tr style="display: none">
                                        <td>
                                            <table width="100%">
                                                <td style="width: 22%">
                                                    <label style="font-weight: bold">
                                                        Trattenute:</label>
                                                </td>
                                                <td style="margin: 15px auto;">
                                                    <asp:GridView runat="server" ID="gvTrattenute" SkinID="grdElenco1" CssClass="intestazioneTabella"
                                                        BorderWidth="1" Width="100%" BorderColor="Black" AutoGenerateColumns="false"
                                                        OnRowDataBound="gvTrattenute_RowDataBound" OnRowEditing="gvTrattenute_RowEditing"
                                                        OnRowUpdating="gvTrattenute_RowUpdating" OnRowCancelingEdit="gvTrattenute_RowCancelingEdit"
                                                        OnRowDeleting="gvTrattenute_RowDeleting" OnDataBound="gvTrattenute_DataBound"
                                                        OnDataBinding="gvTrattenute_DataBinding" OnRowCommand="gvTrattenute_RowCommand"
                                                        EnableViewState="true">
                                                        <Columns>
                                                            <asp:CommandField ItemStyle-Width="6%" ShowEditButton="true" />
                                                            <asp:TemplateField HeaderText="Anno competenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                                                                <ItemTemplate>
                                                                    <asp:Label runat="server" ID="lblAnnoCompetenza" Width="100px" CssClass="txtUppercase"
                                                                        Text='<%#Bind("AnnoCompetenza") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <EditItemTemplate>
                                                                    <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtAnnoCompetenza" MaxLength="4"
                                                                        Text='<%# Bind("AnnoCompetenza")%>' Width="40px">
                                                                    </asp:TextBox>
                                                                    <asp:RegularExpressionValidator runat="server" ID="REVtxtAnnoCompetenza" ControlToValidate="txtAnnoCompetenza"
                                                                        Display="Dynamic" ErrorMessage="Inserire l'Anno di competenza in un formato valido (numerico)"
                                                                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{4}$" />
                                                                    <asp:RequiredFieldValidator ID="RFVtxtAnnoCompetenza" runat="server" ErrorMessage="Anno competenza: Campo obbligatorio"
                                                                        Display="Dynamic" Text="*" CssClass="field-is-required" ControlToValidate="txtAnnoCompetenza"></asp:RequiredFieldValidator>
                                                                </EditItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:LinkButton ID="btnAggiungiTrattenute" CommandName="Aggiungi" runat="server" />
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Codice trattenute" HeaderStyle-CssClass="intestazioneTabella Row1"
                                                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                                                                <ItemTemplate>
                                                                    <asp:Label runat="server" ID="lblCodiceTrattenute" Width="100px" CssClass="txtUppercase"
                                                                        Text='<%#Bind("CodiceTrattenute") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <EditItemTemplate>
                                                                    <asp:DropDownList runat="server" ID="ddlCodiceTrattenute" Width="100px" CssClass="txtUppercase tb8 xxs">
                                                                    </asp:DropDownList>
                                                                    <asp:RequiredFieldValidator ID="RFVddlCodiceTrattenute" runat="server" ErrorMessage="Codice Trattenute : Campo obbligatorio"
                                                                        Display="Dynamic" Text="*" CssClass="field-is-required" ControlToValidate="ddlCodiceTrattenute"></asp:RequiredFieldValidator>
                                                                </EditItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Importo trattenute" HeaderStyle-CssClass="intestazioneTabella Row1"
                                                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                                                                <ItemTemplate>
                                                                    <asp:Label runat="server" ID="lblImportoTrattenute" Width="100px" CssClass="txtUppercase"
                                                                        Text='<%#Bind("ImportoTrattenute")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <EditItemTemplate>
                                                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtImportoTrattenute" Width="140px"
                                                                        CssClass="txtUppercase tb8 " MaxLength="16" Text='<%#Bind("ImportoTrattenute")%>' />
                                                                    <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoTrattenute" ControlToValidate="txtImportoTrattenute"
                                                                        Display="Dynamic" ErrorMessage="Importo Trattenute: inserire l'importo in formato valido (max 8 interi e 2 decimali)"
                                                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,8}(,\d{1,2})?" />
                                                                    <asp:RequiredFieldValidator ID="RFVtxtImportoTrattenute" runat="server" ErrorMessage="Importo Trattenute: Campo obbligatorio"
                                                                        Display="Dynamic" Text="*" CssClass="field-is-required" ControlToValidate="txtImportoTrattenute"></asp:RequiredFieldValidator>
                                                                </EditItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField Visible="false">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblIdQuota" Text='<%#Bind("IdQuota") %>' runat="server" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:CommandField HeaderStyle-CssClass="intestazioneTabella Row1" ItemStyle-CssClass="TblRecordset3"
                                                                ItemStyle-Width="2%" ShowDeleteButton="true" />
                                                        </Columns>
                                                    </asp:GridView>
                                                </td>
                                            </table>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        <br />
        <asp:Panel runat="server" ID="pnlContributiItaEdEsteriAl1295" Visible="false">
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="width: 100%; text-align: left">
                        <asp:Label ID="lblContributiItalianiEsteri" Text="Contributi Italiani ed Esteri al 31/12/95:"
                            runat="server"></asp:Label>
                        <asp:TextBox ID="txtContributiItalianiEsteri" runat="server" CssClass="tb8 txtUppercase"
                            Width="70" MaxLength="6"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="validateTxtContributiItalianiEsteri"
                            ControlToValidate="txtContributiItalianiEsteri" Display="Dynamic" ErrorMessage="Contributi Italiani ed Esteri al 31/12/95 non valido: inserire il numero di Contributi Italiani ed Esteri al 31/12/95 in un formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloCI" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    <div style="margin-top: 25px;">
        <table width="100%">
            <tr>
                <td style="text-align: right">
                    <asp:Button ID="btnSalvaQuotePensione" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Salva Quote Pensione" Width="190px" OnClientClick="BlockUI()"
                        OnClick="btnSalvaQuotePensione_Click" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaQuotePensione" Style="text-align: center; padding-left: 0px;
                        padding-right: 0px" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elimina Quote Pensione" Width="190px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Calcolo?')) return false; else BlockUI();"
                        OnClick="btnEliminaQuotePensione_Click" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
