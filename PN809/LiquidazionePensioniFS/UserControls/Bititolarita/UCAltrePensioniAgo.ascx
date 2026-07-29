<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCAltrePensioniAgo.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Bititolarita.UCAltrePensioniAgo" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
<asp:Panel ID="pnlGridViewAltrePensioni" runat="server">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="lblTitoloAltrePensioni" runat="server" Text="Altre Pensioni" Style="font-weight: bold" CssClass="section-label"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="lblTestoBititolarita" runat="server" Text="Si ricorda che la bititolarità va segnalata solo ed esclusivamente nel caso di presenza di altre pensioni previdenziali e non per le assistenziali o per le prestazioni che accompagnano al pensionamento (es: assegni straordinari per il sostegno del reddito o Ape sociale)."></asp:Label>
                <br />
                <asp:Label ID="lblTestoVOAUT" runat="server" Visible="false" Text="Nel caso di soggetto già titolare di una pensione di categoria VOTOT 070 o VOCUM 170, verificare che la contribuzione versata nella gestione separata non sia stata utilizzata per la liquidazione di un’altra pensione."></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco mt-16" style="overflow-x: auto; width: 700px;">
                    <asp:GridView runat="server" ID="gvAltrePensioni" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella--scrollable intestazioneTabella__with-pagination" BorderWidth="1" Width="700px" BorderColor="Black"
                        AutoGenerateEditButton="true" PageSize="10" AllowPaging="true" OnRowCommand="gvAltrePensioni_RowCommand"
                        OnRowDataBound="gvAltrePensioni_RowDataBound" OnRowCancelingEdit="gvAltrePensioni_RowCancelingEdit"
                        OnRowEditing="gvAltrePensioni_RowEditing" OnRowUpdating="gvAltrePensioni_RowUpdating"
                        OnPageIndexChanging="gvAltrePensioni_onPageIndexChanging" EnableViewState="true" PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
                            <asp:TemplateField HeaderText="Codice Categoria" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="11%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceCategoria_item" Width="70%" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtCodiceCategoria" Width="70%" CssClass="txtUppercase tb8"
                                        MaxLength="3" Text='<%#Bind("CodiceCategoria") %>'>                                       
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtCodiceCategoria" runat="server" ErrorMessage="Codice Categoria: campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtCodiceCategoria" ValidationGroup="UCTabAltrePensioni"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtCodiceCategoria" ControlToValidate="txtCodiceCategoria"
                                        Display="Dynamic" ErrorMessage="Codice Categoria: inserire la categoria in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="[a-zA-Z0-9 ]{2}[a-zA-Z0-9 ]?$" ValidationGroup="UCTabAltrePensioni" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Certificato" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="15%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCertificato"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtCertificato" runat="server" MaxLength="8"
                                        Width="80%" Text='<%#Bind("Certificato") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtCertificato" ControlToValidate="txtCertificato"
                                        Display="Dynamic" ErrorMessage="Certificato: inserire il numero di Certificato in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{8}$" ValidationGroup="UCTabAltrePensioni" />
                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldtxtCertificato" runat="server" ErrorMessage="Certificato: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtCertificato" ValidationGroup="UCTabAltrePensioni"></asp:RequiredFieldValidator>--%>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Codice Ente" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="10%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceEnte"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <%-- <asp:DropDownList runat="server" ID="ddlCodiceEnte" Width="70%" CssClass="txtUppercase tb8">                                       
                                    </asp:DropDownList>--%>
                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldddlCodiceEnte" runat="server" ErrorMessage="Codice Ente: campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlCodiceEnte" ValidationGroup="UCTabAltrePensioni"></asp:RequiredFieldValidator>--%>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="16%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtDecorrenza" CssClass="txtUppercase tb8 date-picker-range70 dateMMaaaa"
                                        MaxLength="7" Text='<%# Bind("Decorrenza", "{0:MM/yyyy}")%>' />
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtDecorrenza" Display="Dynamic"
                                        ControlToValidate="txtDecorrenza" Enabled="true" ErrorMessage="Decorrenza: Inserire una data valida"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabAltrePensioni" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtDecorrenza" runat="server" ErrorMessage="Decorrenza: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtDecorrenza" ValidationGroup="UCTabAltrePensioni"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabAltrePensioni"
                                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Codice U/C" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="9%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceUC_item" Width="50%"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlCodiceUC" Width="70%" CssClass="txtUppercase tb8">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="U" Value="U"></asp:ListItem>
                                        <asp:ListItem Text="C" Value="C"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceUC" runat="server" ErrorMessage="Codice U/C: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlCodiceUC" ValidationGroup="UCTabAltrePensioni"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Codice Importo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="12%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceImporto"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlCodiceImporto" Width="80%" CssClass="txtUppercase tb8">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceImporto" runat="server" ErrorMessage="Codice Importo: campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlCodiceImporto" ValidationGroup="UCTabAltrePensioni"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cessazione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="16%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCessazione"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtCessazione" CssClass="txtUppercase tb8 date-picker-range70 dateMMaaaa"
                                        MaxLength="7" Text='<%# Bind("Cessazione", "{0:MM/yyyy}")%>' />
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtCessazione" Display="Dynamic"
                                        ControlToValidate="txtCessazione" Enabled="true" ErrorMessage="Cessazione: Inserire una data valida"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabAltrePensioni" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldtxtCessazione" runat="server" ErrorMessage="Cessazione: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtCessazione" ValidationGroup="UCTabAltrePensioni"></asp:RequiredFieldValidator> --%>
                                    <asp:CustomValidator runat="server" ControlToValidate="txtCessazione" Display="Dynamic"
                                        ErrorMessage="Cessazione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabAltrePensioni"
                                        ID="customCheckDataCessazione" ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;"
                                HeaderStyle-Width="3%">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteAltrePensioni" CommandName="Elimina" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:HiddenField runat="server" ID="hdnGUID" Visible="false" Value='<%# Eval("Id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <br />
                </div>
            </td>
        </tr>
    </table>
    <asp:HiddenField runat="server" ID="modalitaEditAltrePensioni" Value="false" />
</asp:Panel>
<div style="margin-right: 40px;" class="containerWidth xs">
    <table width="100%" style="min-height: 100px;" class="tab-actions-group">
        <tr valign="bottom">
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaAltrePensioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Altre Pensioni" Width="150px" OnClick="SalvaAltrePensioni_Click"
                    OnClientClick="BlockUI();" CssClass="primary"/>
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaAltrePensioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Altre Pensioni" Width="150px" OnClick="EliminaAltrePensioni_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i dati Altre Pensioni?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
