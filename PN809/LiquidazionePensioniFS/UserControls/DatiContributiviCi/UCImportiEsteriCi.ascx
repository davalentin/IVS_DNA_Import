<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCImportiEsteriCi.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCImportiEsteriCi" %>

<!-- Pannello relativo al GridView Importi Esteri in euro -->
<asp:Panel ID="pnlGridViewImportiEsteri" runat="server">
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <table class="tabellaFormattazione">
                        <tr>
                            <td class="Row1" style="text-align:left">
                                <asp:Label ID="lblTitoloImportiEsteri" runat="server" Text="Importi Esteri in euro" style="font-weight: bold" CssClass="section-label"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:GridView ID="gvImportiEsteri" runat="server" AllowPaging="true" AutoGenerateColumns="false" AutoGenerateEditButton="true" BorderColor="Black" 
                        BorderWidth="1" CssClass="intestazioneTabella intestazioneTabella__with-pagination" EnableViewState="true" 
                        OnRowCancelingEdit="gvImportiEsteri_RowCancelingEdit" 
                        OnRowCommand="gvImportiEsteri_RowCommand" 
                        OnRowDataBound="gvImportiEsteri_RowDataBound" 
                        OnRowEditing="gvImportiEsteri_RowEditing" 
                        OnRowUpdating="gvImportiEsteri_RowUpdating" PageSize="10" 
                        SkinID="grdElenco1" Width="100%" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                        <center>
                            <asp:Label ID="lblNoData" runat="server" Text="Nessun dato retributivo inserito." SkinID="lblNoData" Visible="true"></asp:Label>
                        </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1" 
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="44%" ItemStyle-Width="44%" FooterStyle-Width="44%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtDecorrenza" CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" MaxLength="7" Text='<%# Bind("Decorrenza", "{0:MM/yyyy}")%>' 
                                        Width="70px"/>
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtDecorrenza" Display="Dynamic"
                                        ControlToValidate="txtDecorrenza" Enabled="true" ErrorMessage="Decorrenza: Inserire una data valida"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabImportiEsteriCI" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />  
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabImportiEsteriCI"
                                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />                             
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Importo" 
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="44%" ItemStyle-Width="44%" FooterStyle-Width="44%">
                                <ItemTemplate>
                                    <asp:Label ID="lblImporto" runat="server" ></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtImporto" runat="server"  CssClass="txtUppercase tb8 " MaxLength="15" 
                                    Style="text-align: left" Text=' <%# Bind("Importo")%>' Width="100px"></asp:TextBox>                                   
                                    <asp:RegularExpressionValidator ID="regularTxtImporto" runat="server" ControlToValidate="txtImporto" 
                                        Display="Dynamic" ErrorMessage="Importo: inserire l'importo in formato valido" 
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,}\,?\d{0,}" ValidationGroup="UCTabImportiEsteriCI" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;"
                                HeaderStyle-Width="5%" ItemStyle-Width="5%" FooterStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteImportiEsteri" ToolTip="cancella" runat="server" text=""
                                        CommandArgument="<%#((GridViewRow)Container).RowIndex %>" CommandName="Elimina" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblIdCodeGestione" runat="server"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>  
    <asp:HiddenField runat="server" ID="modalitaEditImportiEsteri" Value="false" /> 
</asp:Panel>
<!-- Fine Pannello relativo al GridView Importi Esteri in euro-->

<div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs no-margin">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaImportiEsteri" runat="server" Enabled="true" SkinID="btnAzione1" Text="Salva Importi Esteri"
                    Width="160px" OnClick="btnSalvaImportiEsteri_Click" OnClientClick="if(Page_ClientValidate('UCTabImportiEsteriCI')){aspnetForm.target ='_self'; BlockUI();}" 
                    CausesValidation="false" CssClass="primary" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaImportiEsteri" runat="server" Enabled="true" SkinID="btnAzione1" Text="Elimina Importi Esteri"
                    Width="160px" OnClick="btnEliminaImportiEsteri_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i dati di Importi Esteri?')) return false; else BlockUI();" 
                    CausesValidation="false" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>