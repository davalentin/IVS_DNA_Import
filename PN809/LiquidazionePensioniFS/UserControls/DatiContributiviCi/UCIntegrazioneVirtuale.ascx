<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCIntegrazioneVirtuale.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCIntegrazioneVirtuale" %>
<asp:Panel ID="pnlTitolare" runat="server">
    <table class="tabellaContenuto">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <table class="tabellaFormattazione">
                        <tr>
                            <td class="Row1" style="text-align: left">
                                <asp:Label ID="lblTitolare" runat="server" Text="Titolare" Style="font-weight: bold" CssClass="section-label"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:GridView runat="server" ID="gvRedditiPerIntegrazioneVirtualeTitolare" SkinID="grdElenco1"
                        AutoGenerateColumns="false" CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="100%"
                        BorderColor="Black" AutoGenerateEditButton="true" PageSize="10" AllowPaging="true"
                        OnRowCommand="gvRedditiPerIntegrazioneVirtualeTitolare_RowCommand" OnRowDataBound="gvRedditiPerIntegrazioneVirtualeTitolare_RowDataBound"
                        OnRowCancelingEdit="gvRedditiPerIntegrazioneVirtualeTitolare_RowCancelingEdit"
                        OnRowEditing="gvRedditiPerIntegrazioneVirtualeTitolare_RowEditing" OnRowUpdating="gvRedditiPerIntegrazioneVirtualeTitolare_RowUpdating"
                        RowStyle-HorizontalAlign="Center" EnableViewState="true" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessun dato inserito." SkinID="lblNoData"
                                    Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Anno Reddito"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblAnnoReddTitolare" Width="100px" runat="server" Text='<%#Bind("Anno") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAnnoReddTitolare" runat="server" CssClass="tb8 txtUppercase"
                                        MaxLength="4" Text='<%#Bind("Anno") %>' Width="100px" Enabled="false" Style="text-align: center;"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldTxtAnnoReddTitolare" runat="server"
                                        ErrorMessage="Anno Reddito Titolare: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtAnnoReddTitolare"
                                        ValidationGroup="UCTabIntegrazioneVirtuale"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtAnnoReddTitolare" runat="server" ControlToValidate="txtAnnoReddTitolare"
                                        Display="Dynamic" ErrorMessage="Anno Reddito Titolare: inserire l'anno in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{4}$" ValidationGroup="UCTabIntegrazioneVirtuale" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Reddito"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblRedditoTitolare" Width="100px" runat="server" Text='<%#Bind("Reddito") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtRedditoTitolare" runat="server" CssClass="txtUppercase tb8 "
                                        MaxLength="15" Text=' <%# Bind("Reddito")%>' Width="150px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regularTxtRedditoTitolare" runat="server" ControlToValidate="txtRedditoTitolare"
                                        Display="Dynamic" ErrorMessage="Reddito Titolare: inserire il reddito in formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,}\,?\d{0,}" ValidationGroup="UCTabIntegrazioneVirtuale" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;"
                                HeaderStyle-Width="5%" ItemStyle-Width="5%" FooterStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteTitolare" CommandName="Elimina" CommandArgument="<% # ((GridViewRow)Container).RowIndex %>"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    <asp:HiddenField runat="server" ID="modalitaEditTitolare" Value="false" />
</asp:Panel>
<asp:Panel ID="pnlConiuge" runat="server">
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <table class="tabellaFormattazione">
                        <tr>
                            <td class="Row1" style="text-align: left">
                                <asp:Label ID="lblConiuge" runat="server" Text="Coniuge" Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:GridView runat="server" ID="gvRedditiPerIntegrazioneVirtualeConiuge" SkinID="grdElenco1"
                        AutoGenerateColumns="false" CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="100%"
                        BorderColor="Black" AutoGenerateEditButton="true" PageSize="10" AllowPaging="true"
                        OnRowCommand="gvRedditiPerIntegrazioneVirtualeConiuge_RowCommand" OnRowDataBound="gvRedditiPerIntegrazioneVirtualeConiuge_RowDataBound"
                        OnRowCancelingEdit="gvRedditiPerIntegrazioneVirtualeConiuge_RowCancelingEdit"
                        OnRowEditing="gvRedditiPerIntegrazioneVirtualeConiuge_RowEditing" OnRowUpdating="gvRedditiPerIntegrazioneVirtualeConiuge_RowUpdating"
                        RowStyle-HorizontalAlign="Center" EnableViewState="true" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessun dato inserito." SkinID="lblNoData"
                                    Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Anno Reddito"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblAnnoRedditoConiuge" runat="server" Width="100px" Text='<%#Bind("Anno") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAnnoRedditoConiuge" runat="server" CssClass="tb8 txtUppercase"
                                        MaxLength="4" Text='<%#Bind("Anno") %>' Width="100px" Enabled="false" Style="text-align: center;"></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regulartxtAnnoRedditoConiuge"
                                        ControlToValidate="txtAnnoRedditoConiuge" Display="Dynamic" ErrorMessage="Inserire l'Anno Reddito in un formato valido (numerico)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{4}$" ValidationGroup="GrigliaBanche" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Reddito"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblRedditoConiuge" Width="100px" runat="server" Text='<%#Bind("Reddito") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtRedditoConiuge" runat="server" CssClass="txtUppercase tb8 " MaxLength="15"
                                        Text=' <%# Bind("Reddito")%>' Width="150px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regularTxtRedditoConiuge" runat="server" ControlToValidate="txtRedditoConiuge"
                                        Display="Dynamic" ErrorMessage="Reddito Coniuge: inserire il reddito in formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,}\,?\d{0,}" ValidationGroup="UCTabIntegrazioneVirtuale" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;"
                                HeaderStyle-Width="5%" ItemStyle-Width="5%" FooterStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteConiuge" CommandName="Elimina" CommandArgument="<% # ((GridViewRow)Container).RowIndex %>"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    <asp:HiddenField runat="server" ID="modalitaEditConiuge" Value="false" />
</asp:Panel>
<div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaRedditiPerIntegrazioneVirtuale" runat="server" Enabled="true"
                    SkinID="btnAzione1" Text="Salva Integrazione Virtuale" Width="185px" OnClick="btnSalvaIntegrazioneVirtuale_Click"
                    OnClientClick="if(Page_ClientValidate('UCTabIntegrazioneVirtuale')){aspnetForm.target ='_self'; BlockUI();}"
                    CausesValidation="false" CssClass="primary" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaRedditiPerIntegrazioneVirtuale" runat="server" Enabled="true"
                    SkinID="btnAzione1" Text="Elimina Integrazione Virtuale" Width="185px" OnClick="btnEliminaIntegrazioneVirtuale_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Redditi per integrazione virtuale?')) return false; else BlockUI();"
                    CausesValidation="false" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
