<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSupplementiCumuloStorico.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Supplementi.UCSupplementiCumuloStorico" %>
<asp:Panel runat="server" ID="pnlSupplementiCumuloStorico">
    <div id="divSupplementiCumuloStorico" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <br />
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblSupplementiCumuloStorico"> Quote Supplementi Storico:</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                </td>
            </tr>
            <tr>
                <td style="text-align: center;">
                    <asp:GridView runat="server" ID="gvSupplementiStorico" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella" BorderWidth="1" Width="100%" BorderColor="Black"
                        EnableViewState="true" OnRowDataBound="gvSupplementiStorico_RowDataBound">
                        <Columns>
                            <asp:TemplateField HeaderText="Ente/Gestione Fondo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblEnteGestioneFondoStorico_item" Width="100px" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Descrizione Fondo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="27%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDescrizioneFondoStorico_item" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="17%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenzaQuotaStorico_item" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane_Item" Width="100px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Importo Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblImportoQuotaStorico_Item" Width="100px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
