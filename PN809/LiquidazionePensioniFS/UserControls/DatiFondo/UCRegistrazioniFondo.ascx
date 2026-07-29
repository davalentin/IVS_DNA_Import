<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCRegistrazioniFondo.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondo.UCRegistrazioniFondo" %>
<asp:Panel runat="server" ID="pnlRegistrazioniFondo">
    <div class="bckGridViewElenco tableWithChips" style="margin: 10px; margin-top: 30px">
        <asp:GridView runat="server" ID="gvRegistrazioniFondo" SkinID="grdElenco1" AutoGenerateColumns="false"
            CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="100%" BorderColor="Black"
            AutoGenerateEditButton="false" PageSize="10" AllowPaging="true" OnRowCommand="gvRegistrazioniFondo_RowCommand"
            OnRowDataBound="gvRegistrazioniFondo_RowDataBound" EnableViewState="true" PagerStyle-CssClass="default-pagination-tables">
            <Columns>
                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="2%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                    ItemStyle-CssClass="TblRecordset3">
                    <ItemTemplate>
                        <asp:HiddenField runat="server" ID="hdnIdRecordFondo" Value='<%#Bind("IdRecordFondo") %>' />
                        <asp:Image runat="server" ID="imgSemaforo" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella"
                    ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblDecorrenza" Text='<%#Bind("DecorrenzaValiditaDati", "{0:dd/MM/yyyy}")%>'> 
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Legge 4/60" HeaderStyle-CssClass="intestazioneTabella"
                    ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Image runat="server" ID="imgLegge460" Height="18px" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Art.2 Comma 12 L.335" HeaderStyle-CssClass="intestazioneTabella"
                    ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Image runat="server" ID="imgArticolo2" Height="18px" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Privilegiata" HeaderStyle-CssClass="intestazioneTabella"
                    ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Image runat="server" ID="imgPrivilegiata" Height="18px" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="" HeaderStyle-CssClass="intestazioneTabella" ItemStyle-CssClass="TblRecordset3"
                    ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Button runat="server" ID="btnModifica" CommandName="Modifica" Text="Modifica"
                            SkinID="btnAzione1" OnClientClick="BlockUI();"  CssClass="tertiary editIconOnly" />
                        <asp:Button runat="server" ID="btnConsulta" CommandName="Consulta" Text="Consulta"
                            SkinID="btnAzione1" OnClientClick="BlockUI();" Visible="false" CssClass="tertiary viewIconOnly" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="" HeaderStyle-CssClass="intestazioneTabella" ItemStyle-CssClass="TblRecordset3"
                    ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Button runat="server" ID="btnElimina" CommandName="Elimina" Text="Elimina" SkinID="btnAzione1"
                            OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare la registrazione?')) return false; else BlockUI();" CssClass="ghost-delete trashIconOnly" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Panel>
<div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: center" class="tab-actions-group__first">
                <asp:Button ID="btnAggiungiRegistrazione" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Aggiungi Registrazione" Width="180px" OnClientClick="BlockUI();"
                    OnClick="btnAggiungiRegistrazione_Click" CssClass="primary force-right" />
                <asp:Button ID="btnEliminaRegistrazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina" Width="180px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare le Registrazioni?')) return false; else BlockUI();"
                    OnClick="btnEliminaRegistrazioni_Click" CssClass="ghost-delete"/>
            </td>
        </tr>
        <tr></tr>
    </table>
</div>
