<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCRichiestaBonus.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.RichiestaBonus.UCRichiestaBonus" %>
<asp:Panel runat="server" ID="pnlRichiestaBonus">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
            <asp:Label ID="lblStatoPreCalcolo" runat="server" Text="La colonna 'Stato pre-calcolo' descrive lo stato presente nel booking ante definizione
                    della ricostituzione" ForeColor="Red" Visible="false"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <asp:Label ID="lblUltimaVariazione" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione tabellaContenuti grid-col-1" style="width: 100%;">
        <tr>
            <td>
                <asp:GridView ID="gvRichiestaBonus" runat="server" BorderWidth="1" BorderColor="Black"
                    AutoGenerateColumns="false" AllowSorting="true" Visible="true" Width="100% "
                    SkinID="grdElenco1" AllowPaging="false" OnPageIndexChanging="gvRichiestaBonus_onPageIndexChanging"
                    PageSize="15" OnRowDataBound="gvRichiestaBonus_RowDataBound">
                    <EmptyDataTemplate>
                        <center>
                            <asp:Label ID="lblNoData" runat="server" Text="Nessun anno." SkinID="lblNoData" Visible="true"></asp:Label>
                        </center>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="Richiesta Bonus" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkRichiediBonus" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Anno Bonus" DataField="Anno" Visible="true" ItemStyle-HorizontalAlign="Center"
                            ItemStyle-Width="16%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                            ItemStyle-CssClass="TblRecordset3" />
                        <asp:BoundField HeaderText="Prescrizione" DataField="Prescrizione" Visible="true"
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="16%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                            ItemStyle-CssClass="TblRecordset3" />
                        <asp:BoundField HeaderText="Stato pre-calcolo" DataField="DescrizioneEsitoMessaggio"
                            Visible="true" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                            ItemStyle-CssClass="TblRecordset3" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" style="min-height: 100px;">
            <tr valign="bottom">
                <td style="text-align: right;">
                    <asp:Button ID="btnSalvaAnniRichiestaBonus" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Salva Anni Richiesta Bonus" Width="190px" OnClientClick="mainValidate()"
                        OnClick="btnSalvaAnniRichiestaBonus_Click" />
                </td>
                <td style="text-align: left;">
                    <asp:Button ID="btnEliminaAnniRichiestaBonus" runat="server" SkinID="btnAzione1"
                        CausesValidation="false" Enabled="true" Text="Elimina Anni Richiesta Bonus" Width="190px"
                        OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare gli Anni Richiesta Bonus?')) return false; else BlockUI();"
                        OnClick="btnEliminaAnniRichiestaBonus_Click" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
