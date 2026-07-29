<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCAltreDomandeCollegate.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreDomandeCollegate.UCAltreDomandeCollegate" %>
<div id="pdivAltreDomandeCollegate" runat="server" style="margin-left: 10px; margin-right: 10px;" class="reset-style">
    <table class="tabellaFormattazione grid-col-1" style="padding-left: 10px; padding-bottom: 10px;
        width: 99%">
        <tr>
            <td>
                <asp:Label runat="server" ID="lblDatiAltreDomandeCollegate" Style="font-weight: bold" CssClass="section-label"> Dettaglio domande collegate:</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="gvDatiAltreDomandeCollegate" runat="server" SkinID="grdElenco2"
                    AutoGenerateColumns="false" Width="95%" OnRowDataBound="gvDatiAltreDomandeCollegate_RowDataBound"
                    OnRowCommand="gvDatiAltreDomandeCollegate_onRowCommand" Style="margin: auto;"
                    CssClass="intestazioneTabella intestazioneTabella__with-pagination intestazioneTabella--scrollable"
                    PagerStyle-CssClass="default-pagination-tables">
                    <EmptyDataTemplate>
                        <center>
                            <asp:Label ID="lblNoData" runat="server" Text="Nessun'altra domanda collegata presente."
                                SkinID="lblNoData" CssClass="no-content-container section-label"></asp:Label>
                        </center>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="Numero Domanda" ItemStyle-HorizontalAlign="Center"
                            HeaderStyle-CssClass="intestazioneTabella Row1 is-200" ItemStyle-CssClass="TblRecordset3 is-200">
                            <ItemTemplate>
                                <asp:LinkButton CssClass="is-text" runat="server" Text='<%#Eval("NumeroDomanda")%>' ID="lnkDomanda"
                                    OnClientClick="BlockUI()" CommandName="Selezione" CommandArgument='<%#Eval("NumeroDomanda")%>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Prodotto" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblProdotto" Text='<%#Eval("Prodotto")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Stato LIQPENS" ItemStyle-HorizontalAlign="Center"
                            HeaderStyle-CssClass="intestazioneTabella Row1" ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblStatoLiqPens" Text='<%#Eval("StatoLiqPens")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Stato WEBDOM" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblStatoWebDom" Text='<%#Eval("StatoWebDom")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Chiave Pensione" ItemStyle-HorizontalAlign="Center"
                            HeaderStyle-CssClass="intestazioneTabella Row1 noPadding is-200" ItemStyle-CssClass="TblRecordset3 noPadding is-200">
                            <%-- Ho messo un altezza fissa per fare in modo che funzione l'height: 100% della table nell'ItemTemplate --%>
                            <ItemStyle Height="50px" />
                            <HeaderTemplate>
                                <table style="border-collapse: collapse;" class="innerBorder intestazioneTabella intestazioneTabella--scrollable no-border-style">
                                    <tr>
                                        <th colspan="2" class="intestazioneSubColumns">
                                            <asp:Label runat="server" ID="lblChiavePensione" Text="Chiave Pensione"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <th class="intestazioneSubColumns is-200 no-border-style" style="width: 50%;">
                                            <asp:Label runat="server" ID="lblPensioneGenerata" Text="Pensione generata dell'avente diritto"></asp:Label>
                                        </th>
                                        <th class="intestazioneSubColumns is-200 no-border-style has-left" style="width: 50%;">
                                            <asp:Label runat="server" ID="lblPensioneRiferimento" Text="Pensione di riferimento del dante causa"></asp:Label>
                                        </th>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <table style="border-collapse: collapse; width: 100%; height: 100%;" class="innerBorder intestazioneTabella intestazioneTabella--scrollable no-border-style">
                                    <tr>
                                        <td style="width: 50%; text-align: center;" class="is-200 no-border-style">
                                            <asp:Label runat="server" ID="lblPensioneGenerataAventeDiritto"></asp:Label>
                                        </td>
                                        <td style="width: 50%; text-align: center;" class="is-200 no-border-style has-left">
                                            <asp:Label runat="server" ID="lblPensioneRiferimentoDanteCausa"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <asp:Panel runat="server" ID="pnlAventiDiritto" Visible="false">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblDatiAventiDiritto" Style="font-weight: bold" CssClass="section-label mt-32"> Elenco degli aventi diritto</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="gvAventiDiritto" runat="server" SkinID="grdElenco1" AutoGenerateColumns="false"
                        Width="95%" OnRowDataBound="gvAventiDiritto_RowDataBound" Style="margin: auto;" 
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination intestazioneTabella--scrollable"
                        PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataTemplate>
                        <center>
                            <asp:Label ID="lblNoData" runat="server" Text="Nessun avente diritto presente."
                                SkinID="lblNoData"></asp:Label>
                        </center>
                    </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderText="Nome" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="intestazioneTabella Row1 is-200"
                                ItemStyle-CssClass="TblRecordset3 is-200">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblNome"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cognome" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCognome"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Data di nascita" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-CssClass="intestazioneTabella Row1" ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDataNascita"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Codice Fiscale" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-CssClass="intestazioneTabella Row1" ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceFiscale"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Parentela con il dante causa" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-CssClass="intestazioneTabella Row1  is-200" ItemStyle-CssClass="TblRecordset3  is-200">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblParentelaDC" Text='<%#Bind("DecParentelaDA")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </asp:Panel>
</div>

<div style="margin-top: 25px;">
</div>
