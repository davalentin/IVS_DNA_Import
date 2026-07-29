<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCRegistrazioniFondo.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondoAgo.UCRegistrazioniFondo" %>
<asp:Panel runat="server" ID="pnlRegistrazioniFondo">
    <div class="bckGridViewElenco" style="margin: 10px; margin-top: 30px">
        <asp:GridView runat="server" ID="gvRegistrazioniFondo" SkinID="grdElenco1" AutoGenerateColumns="false"
            CssClass="intestazioneTabella" BorderWidth="1" Width="100%" BorderColor="Black"
            AutoGenerateEditButton="false" PageSize="10" AllowPaging="true" OnRowCommand="gvRegistrazioniFondo_RowCommand"
            OnRowDataBound="gvRegistrazioniFondo_RowDataBound" EnableViewState="true">
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
                            SkinID="btnAzione1" OnClientClick="BlockUI();" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="" HeaderStyle-CssClass="intestazioneTabella" ItemStyle-CssClass="TblRecordset3"
                    ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Button runat="server" ID="btnElimina" CommandName="Elimina" Text="Elimina" SkinID="btnAzione1"
                            OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare la registrazione?')) return false; else BlockUI();" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    <div style="width: 720px; margin-top: 25px; margin-right: 40px;">
        <table width="100%">
            <tr>
                <td style="text-align: center">
                    <asp:Button ID="btnAggiungiRegistrazione" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Aggiungi Registrazione" Width="150px" OnClientClick="BlockUI();"
                        OnClick="btnAggiungiRegistrazione_Click" />
                    <asp:Button ID="btnEliminaRegistrazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elimina" Width="150px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare le Registrazioni?')) return false; else BlockUI();"
                        OnClick="btnEliminaRegistrazioni_Click" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
