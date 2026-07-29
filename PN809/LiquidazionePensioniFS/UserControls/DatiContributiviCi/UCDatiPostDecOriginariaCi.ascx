<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiPostDecOriginariaCi.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCDatiPostDecOriginariaCi" %>

<asp:Panel ID="pnlGridViewDatiPostDecOriginaria" runat="server">
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <table class="tabellaFormattazione">
                        <tr>
                            <td class="Row1" style="text-align: left">
                                <asp:Label ID="lblTitoloDatiContributivi" runat="server" Text="Dati contributivi o retributivi post decorrenza originaria"
                                    Style="font-weight: bold"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:GridView ID="gvDatiPostDecOriginaria" runat="server" AllowPaging="true" AutoGenerateColumns="false" AutoGenerateEditButton="true"
                        BorderColor="Black" BorderWidth="1" CssClass="intestazioneTabella intestazioneTabella__with-pagination" EnableViewState="true"
                        PageSize="10" SkinID="grdElenco1" Width="100%" RowStyle-HorizontalAlign="Center"
                        OnRowDataBound="gvDatiPostDecOriginaria_RowDataBound" 
                        OnRowCommand="gvDatiPostDecOriginaria_RowCommand"
                        OnRowEditing="gvDatiPostDecOriginaria_RowEditing" PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="16%" ItemStyle-Width="16%"
                                FooterStyle-Width="16%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza" Width="55%" CssClass="txtUppercase" Text='<%# Bind("Decorrenza", "{0:MM/yyyy}")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtDecorrenza" CssClass="tb8 txtUppercase date-picker dateMMaaaa"
                                        MaxLength="7" Text='<%# Bind("Decorrenza", "{0:MM/yyyy}")%>' Width="55%" />
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtDecorrenza" Display="Dynamic"
                                        ControlToValidate="txtDecorrenza" Enabled="true" ErrorMessage="Decorrenza: Inserire una data valida"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiPostDecOriginariaCI" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiPostDecOriginariaCI"
                                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" /> 
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CTR" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="12%" ItemStyle-Width="12%"
                                FooterStyle-Width="12%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCTR" Width="70%" CssClass="txtUppercase" Text='<%# Bind("CTR")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtCTR" CssClass="tb8 txtUppercase" Text='<%# Bind("CTR")%>' Width="70%" />
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtCTR" Display="Dynamic"
                                        ControlToValidate="txtCTR" Enabled="true" ErrorMessage="CTR: E' possibile inserire solo caratteri numerici"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiPostDecOriginariaCI" ValidationExpression="[0-9]*$" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="IVS" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="14%" ItemStyle-Width="14%"
                                FooterStyle-Width="14%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblIVS" Width="70%" CssClass="txtUppercase" Text='<%# Bind("IVS")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtIVS" CssClass="tb8 txtUppercase" Text='<%# Bind("IVS")%>' Width="70%" MaxLength="17" />
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtIVS" Display="Dynamic"
                                        ControlToValidate="txtIVS" Enabled="true" ErrorMessage="IVS: E' possibile inserire solo caratteri numerici"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiPostDecOriginariaCI" ValidationExpression="[0-9]*,?[0-9]*$" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane Retributive" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="15%" ItemStyle-Width="15%"
                                FooterStyle-Width="15%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimaneRetributive" Width="80%" CssClass="txtUppercase" Text='<%# Bind("SettimaneRetributive")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtSettimaneRetributive" CssClass="tb8 txtUppercase" Text='<%# Bind("SettimaneRetributive")%>' Width="80%" />
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneRetributive" Display="Dynamic"
                                        ControlToValidate="txtSettimaneRetributive" Enabled="true" ErrorMessage="Settimane Retributive: E' possibile inserire solo caratteri numerici"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiPostDecOriginariaCI" ValidationExpression="[0-9]*$" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane VV" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="15%" ItemStyle-Width="15%"
                                FooterStyle-Width="15%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimaneVV" Width="80%" CssClass="txtUppercase" Text='<%# Bind("SettimaneVV")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtSettimaneVV" CssClass="tb8 txtUppercase" Text='<%# Bind("SettimaneVV")%>' Width="80%" />
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneVV" Display="Dynamic"
                                        ControlToValidate="txtSettimaneVV" Enabled="true" ErrorMessage="Settimane VV: E' possibile inserire solo caratteri numerici"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiPostDecOriginariaCI" ValidationExpression="[0-9]*$" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="RMS" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="17%" ItemStyle-Width="17%"
                                FooterStyle-Width="17%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRMS" Width="80%" CssClass="txtUppercase" Text='<%# Bind("RMS")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtRMS" CssClass="tb8 txtUppercase" Text='<%# Bind("RMS")%>' Width="80%" MaxLength="17" />
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtRMS" Display="Dynamic"
                                        ControlToValidate="txtRMS" Enabled="true" ErrorMessage="RMS: E' possibile inserire solo caratteri numerici"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiPostDecOriginariaCI" ValidationExpression="[0-9]*,?[0-9]*$" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella">
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
    <asp:HiddenField runat="server" ID="modalitaEditDatiPostDecOriginaria" Value="false" /> 
</asp:Panel>

<div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%">
        <tr>
            <td style="text-align: right">
                <asp:Button ID="btnSalvaDatiPostDecOriginaria" runat="server" Enabled="true" SkinID="btnAzione1" Text="Salva Post. Dec. Orig."
                    Width="160px" OnClick="btnSalvaDatiPostDecOriginaria_Click" OnClientClick="if(Page_ClientValidate('UCTabDatiPostDecOriginariaCI')){aspnetForm.target ='_self'; BlockUI();}" 
                    CausesValidation="false" CssClass="primary" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaDatiPostDecOriginaria" runat="server" Enabled="true" SkinID="btnAzione1" Text="Elimina Post Dec. Orig."
                    Width="160px" OnClick="btnEliminaDatiPostDecOriginaria_Click" 
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i dati di Post Dec. Orig.?')) return false; else BlockUI();" CausesValidation="false" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>