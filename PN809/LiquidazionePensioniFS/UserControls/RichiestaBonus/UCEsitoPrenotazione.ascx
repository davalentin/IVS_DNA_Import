<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCEsitoPrenotazione.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.RichiestaBonus.UCEsitoPrenotazione" %>
<asp:Panel runat="server" ID="pnlEsitoPrenotazione">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1">
                <asp:Label ID="lblUltimaVariazione" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione tabellaContenuti grid-col-1" style="width: 100%;">
        <tr>
            <td>
                <asp:GridView ID="gvEsitoPrenotazione" runat="server" BorderWidth="1" BorderColor="Black"
                    AutoGenerateColumns="false" AllowSorting="true" Visible="true" Width="100% "
                    SkinID="grdElenco1" AllowPaging="false" PageSize="15">
                    <EmptyDataTemplate>
                        <center>
                            <asp:Label ID="lblNoData" runat="server" Text="Nessuna Elaborazione Prenotata." SkinID="lblNoData" Visible="true"></asp:Label>
                        </center>
                    </EmptyDataTemplate>
                    <Columns>                         
                        <asp:BoundField HeaderText="Anno Prenotazione" DataField="AnnoRichiesto" Visible="true" ItemStyle-HorizontalAlign="Center"
                            ItemStyle-Width="16%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                            ItemStyle-CssClass="TblRecordset3" />
                        <asp:BoundField HeaderText="Esito Prenotazione" DataField="DescrizioneEsito" Visible="true"
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="50%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                            ItemStyle-CssClass="TblRecordset3" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>    
</asp:Panel>
