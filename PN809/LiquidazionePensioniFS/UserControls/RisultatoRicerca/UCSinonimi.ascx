<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSinonimi.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.RisultatoRicerca.UCSinonimi" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
<asp:Panel ID="pnlRisultatoRicercaElaborazioneOmonimi" runat="server">
    <table class="tabellaFormattazione">
        <tr>
            <td class="titolo" style="padding-bottom: 10px;">
                <label>
                    Trovati
                </label>
                <asp:Label runat="server" ID="lblNOmonimi"></asp:Label>
                <label>
                    risultati per </label>
                <asp:Label runat="server" ID="lblParametriRicerca"></asp:Label>
                <asp:Label runat="server" ID="lblParametriRicerca2"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="gvSinonimi" runat="server" BorderWidth="1" BorderColor="Black"
                    AutoGenerateColumns="false" Visible="true" Width="100% " SkinID="grdElenco1"
                    OnRowCommand="ScegliSinonimo_onRowCommand" AllowPaging="true" PageSize="10" OnPageIndexChanging="gvSinonimi_onPageIndexChanging"
                    AllowSorting="true" OnSorting="gvSinonimi_onSorting" OnRowCreated="gvSinonimi_RowCreated" PagerSettings-Mode="NumericFirstLast"
                    CssClass="intestazioneTabella intestazioneTabella--sorting intestazioneTabella__with-pagination"  PagerStyle-CssClass="default-pagination-tables">
                    <EmptyDataTemplate>
                        <center>
                            <asp:Label ID="lblNoData" runat="server" Text="Nessuna posizione trovata per i criteri inseriti."
                                SkinID="lblNoData" Visible="true"></asp:Label>
                        </center>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField HeaderText="CodiceFiscale" DataField="CodiceFiscale" Visible="true"
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink intestazioneTabella__heading intestazioneTabella__heading--sort"
                            ItemStyle-CssClass="TblRecordset3" SortExpression="CodiceFiscale" />
                        <asp:BoundField HeaderText="Cognome" DataField="Cognome" Visible="true" ItemStyle-HorizontalAlign="Center"
                            ItemStyle-Width="21%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink intestazioneTabella__heading intestazioneTabella__heading--sort"
                            ItemStyle-CssClass="TblRecordset3" SortExpression="Cognome" />
                        <asp:BoundField HeaderText="Nome" DataField="Nome" Visible="true" ItemStyle-HorizontalAlign="Center"
                            ItemStyle-Width="16%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink intestazioneTabella__heading intestazioneTabella__heading--sort"
                            ItemStyle-CssClass="TblRecordset3" SortExpression="Nome" />
                        <asp:BoundField HeaderText="DataNascita" DataField="DataNascita" Visible="true" ItemStyle-HorizontalAlign="Center"
                            ItemStyle-Width="16%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink intestazioneTabella__heading intestazioneTabella__heading--sort"
                            ItemStyle-CssClass="TblRecordset3" DataFormatString="{0:dd/MM/yyyy}" SortExpression="DataNascita" />
                        <asp:TemplateField HeaderText="Operazione" ItemStyle-Width="26%" HeaderStyle-CssClass="intestazioneTabella Row1 intestazioneTabella__headin"
                            ControlStyle-CssClass="pulsante1 tertiary" ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Button runat="server" ID="btnRicerca" Text="Cerca Posizioni" CommandName="CercaPosizioni" OnClientClick="BlockUI()" CssClass="tertiary"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Panel>
