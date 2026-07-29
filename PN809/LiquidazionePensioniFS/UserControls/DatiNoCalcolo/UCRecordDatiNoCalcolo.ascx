<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCRecordDatiNoCalcolo.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiNoCalcolo.UCRecordDatiNoCalcolo" %>
<asp:Panel runat="server" ID="pnlRecordNoCalcolo">
    <div class="bckGridViewElenco" style="margin: 10px; margin-top: 30px">
        <asp:GridView runat="server" ID="gvRecordNoCalcolo" SkinID="grdElenco1" AutoGenerateColumns="false"
            CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="100%" BorderColor="Black"
            AutoGenerateEditButton="false" PageSize="10" AllowPaging="true" OnPageIndexChanging="GvRecordNoCalcolo_onPageIndexChanging" OnRowCommand="gvRegistrazioniNoCalcolo_RowCommand"
            OnRowDataBound="gvRegistrazioniNoCalcolo_RowDataBound"  EnableViewState="true" PagerStyle-CssClass="default-pagination-tables">
            <EmptyDataTemplate>
                <center>
                    <asp:Label ID="lblNoData" runat="server" Text="Nessun record No Calcolo inserito."
                        SkinID="lblNoData" Visible="true"></asp:Label>
                </center>
            </EmptyDataTemplate>
            <Columns>
                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="2%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                    ItemStyle-CssClass="TblRecordset3" Visible="true">
                    <ItemTemplate>
                        <asp:HiddenField runat="server" ID="hdnIdRecordNoCalcolo" Value='<%#Bind("IdRecordNoCalcolo") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella"
                    ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblDecorrenza" Text='<%#Bind("Decorrenza")%>'> 
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Dati NoCalcolo" HeaderStyle-CssClass="intestazioneTabella"
                    ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Image runat="server" ID="imgRecordDatiNoCalcolo" Height="18px" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="" HeaderStyle-CssClass="intestazioneTabella" ItemStyle-CssClass="TblRecordset3"
                    ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Button runat="server" ID="btnModifica" CommandName="Modifica" Text="Modifica"
                            SkinID="btnAzione1" OnClientClick="BlockUI();" CssClass="tertiary editIconOnly" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="" HeaderStyle-CssClass="intestazioneTabella" ItemStyle-CssClass="TblRecordset3"
                    ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Button runat="server" ID="btnElimina" CommandName="Elimina" Text="Elimina" SkinID="btnAzione1" CssClass="ghost-delete trashIconOnly"
                            OnClientClick="BlockUI();" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: center" class="tab-actions-group__first">
                    <asp:Button ID="btnAggiungiRegistrazione" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Aggiungi Registrazione" Width="150px" OnClientClick="BlockUI();"
                        OnClick="btnAggiungiRegistrazione_Click" CssClass="primary force-right" />
                    <asp:Button ID="btnEliminaRegistrazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elimina" Width="150px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare le Registrazioni?')) return false; else BlockUI();"
                        OnClick="btnEliminaRegistrazioni_Click" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
