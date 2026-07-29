<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCRedditi.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Redditi.UCRedditi" %>
<asp:Panel runat="server" ID="pnlRedditi">
<table class="tabellaFormattazione">
<tr>
<td class="Row1">
    <asp:Label ID="lblUltimaVariazione" runat="server" Text="" CssClass="section-label"></asp:Label>
</td>
</tr>
</table>


    <table  class="tabellaFormattazione tabellaContenuti grid-col-1" style="width: 100%; ">
        <tr>
            <td>
                <asp:GridView ID="gvRedditi" runat="server" BorderWidth="1" BorderColor="Black" AutoGenerateColumns="false"
                    AllowSorting="true" Visible="true" Width="100% " SkinID="grdElenco1" AllowPaging="true"
                    OnPageIndexChanging="gvRedditi_onPageIndexChanging" PageSize="15" OnRowCommand="gvRedditi_onRowCommand"
                    OnRowCreated="gvRedditi_RowCreated">
                    <EmptyDataTemplate>
                        <center>
                            <asp:Label ID="lblNoData" runat="server" Text="Nessun reddito."
                                SkinID="lblNoData" Visible="true"></asp:Label>
                        </center>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField HeaderText="Anno Reddito" DataField="AnnoReddito" Visible="true" ItemStyle-HorizontalAlign="Center"
                            ItemStyle-Width="16%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                            ItemStyle-CssClass="TblRecordset3" />
                        <asp:BoundField HeaderText="Rilevanze" DataField="Rilevanze" Visible="true" ItemStyle-HorizontalAlign="Left"
                            ItemStyle-Width="16%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                            ItemStyle-CssClass="TblRecordset3" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
            <table width="100%" style="min-height:100px;" class="tab-actions-group position-left">
                <tr valign="bottom">
                    <td style="text-align: right ; " class="tab-actions-group__first">
                        <asp:Button ID="btnAggiorna" runat="server" Text="Aggiorna" SkinID="btnAzione1" CausesValidation="false"
                             OnClick="AggiornaRedditi" Width="150px" OnClientClick="mainValidate()"  CssClass="ghost-update"/>
                    </td>
                    <td style="text-align: left;">
                        <asp:Button ID="btnAcquisisci" runat="server" Text="Acquisisci" SkinID="btnAzione1" Width="150px" ValidationGroup="UCRedditi"
                            CausesValidation="true" OnClick="AcquisisciRedditi" OnClientClick="aspnetForm.target ='_blank';" CssClass="tertiary force-right tertiary-external"/>
                        <asp:Button ID="btnElimina" runat="server" Text="Elimina" SkinID="btnAzione1" Width="150px" OnClick="EliminaRedditi" Visible="false"
                            OnClientClick="BlockUI()" CssClass="ghost-delete" />
                    </td>
                </tr>
            </table>
        </div>
</asp:Panel>
