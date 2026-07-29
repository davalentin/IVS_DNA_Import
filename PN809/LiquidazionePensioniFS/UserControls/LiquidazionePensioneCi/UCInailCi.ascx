<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCInailCi.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCInailCi" %>
<asp:Panel ID="pnlGridViewRenditaINAIL" runat="server">
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <table class="tabellaFormattazione">
                        <tr>
                            <td class="Row1" style="text-align: left">
                                <asp:Label ID="lblTitoloRenditaINAIL" runat="server" Text="Rendita INAIL" Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <asp:GridView runat="server" ID="gvRenditaINAIL" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="100%" BorderColor="Black"
                        AutoGenerateEditButton="true" PageSize="10" EnableViewState="true" OnRowDataBound="gvRenditaINAIL_RowDataBound"
                        AllowPaging="true" OnRowCommand="gvRenditaINAIL_RowCommand" OnRowCancelingEdit="gvRenditaINAIL_RowCancelingEdit"
                        OnRowEditing="gvRenditaINAIL_RowEditing" OnPageIndexChanging="gvRenditaINAIL_onPageIndexChanging"  PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
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
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabGridINAIL" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtDecorrenzaCE" runat="server" ErrorMessage="Decorrenza: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtDecorrenza" ValidationGroup="UCTabGridINAIL" Display="Dynamic"
                                        Enabled="true"></asp:RequiredFieldValidator>
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabGridINAIL"
                                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Importo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="29%" ItemStyle-Width="29%"
                                FooterStyle-Width="29%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblImporto"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtImporto" runat="server" MaxLength="11"
                                        Width="50%" Text='<%#Bind("Importo") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtSettimane" ControlToValidate="txtImporto"
                                        Display="Dynamic" ErrorMessage="Importo: inserire l'importo in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d+(\,\d{1,4})?" ValidationGroup="UCTabGridINAIL"
                                        Enabled="true" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtImporto" runat="server" ErrorMessage="Importo: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtImporto" ValidationGroup="UCTabGridINAIL" Display="Dynamic"
                                        Enabled="true"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Evento" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="30%" ItemStyle-Width="30%"
                                FooterStyle-Width="30%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblEvento_item" Width="120px" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlEvento" Width="120px" CssClass="txtUppercase tb8 xxs">
                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                        <asp:ListItem Value="SI" Text="SI"></asp:ListItem>
                                        <asp:ListItem Value="NO" Text="NO"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlEvento" runat="server" ErrorMessage="Evento: campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlEvento" ValidationGroup="UCTabGridINAIL" Display="Dynamic"
                                        Enabled="true"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;"
                                HeaderStyle-Width="5%" ItemStyle-Width="5%" FooterStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteRenditaINAIL" CommandName="Elimina" CommandArgument="<% # ((GridViewRow)Container).RowIndex %>"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblIdCodeGestione" runat="server" />
                                    <asp:HiddenField runat="server" ID="hdnGUID" Visible="false" Value='<%# Eval("Id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    <asp:HiddenField runat="server" ID="modalitaEditRenditaINAIL" Value="false" />
</asp:Panel>
<div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaBititolaritaInail" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Inail/Accomp." Width="170px" OnClientClick="if(Page_ClientValidate('UCTabInail')){aspnetForm.target ='_self'; BlockUI();}"
                    OnClick="SalvaInail_Click" CssClass="primary" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaBititolaritaInail" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Inail/Accomp." Width="170px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i dati Inail/Accomp.?')) return false; else BlockUI();"
                    OnClick="EliminaInail_Click" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
