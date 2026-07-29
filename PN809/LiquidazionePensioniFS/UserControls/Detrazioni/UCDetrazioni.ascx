<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDetrazioni.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Detrazioni.UCDetrazioni" %>
<asp:Panel runat="server" ID="pnlDetrazioni">
    <table class="tabellaFormattazione grid grid-col-2" width="100%">
        <tr>
            <td class="Row1" style="width: 62%;">
                <label style="font-weight: bold">
                    Detrazioni fiscali relative a:
                    <asp:Label runat="server" ID="lblDecorrenzaImposte"></asp:Label>
                    <asp:Label runat="server" ID="lblCodiceFiscale"></asp:Label>
                </label>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <asp:Label runat="server">Detrazioni per reddito da lavoro o esenzioni</asp:Label>
            </td>
            <td class="Row1" align="left">
                <asp:Label runat="server" ID="lblAgevolazReddLavAut"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Agevolazioni per pensionati articolo 11 del TUIR</label>
            </td>
            <td class="Row1">
                <asp:Label runat="server" ID="lblAgevolazioniPensionati"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Coniuge / Unito ovvero primo figlio al posto del coniuge</label>
            </td>
            <td class="Row1">
                <asp:Label runat="server" ID="lblConiugeoPrimoFiglio"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Numero di figli &lt = 3 anni senza handicap al 100%
                </label>
            </td>
            <td class="Row1">
                <asp:Label runat="server" ID="lblNFigliMin3NoHandicap100"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Numero di figli &lt = 3 anni senza handicap al 50%
                </label>
            </td>
            <td class="Row1">
                <asp:Label runat="server" ID="lblNFigliMin3NoHandicap50"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Numero di figli &lt = 3 anni con handicap al 100%
                </label>
            </td>
            <td class="Row1">
                <asp:Label runat="server" ID="lblNFigliMin3Handicap100"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Numero di figli &lt = 3 anni con handicap al 50%
                </label>
            </td>
            <td class="Row1">
                <asp:Label runat="server" ID="lblNFigliMin3Handicap50"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Numero di figli &gt 3 anni senza handicap al 100%
                </label>
            </td>
            <td class="Row1">
                <asp:Label runat="server" ID="lblNFigliMagg3NoHandicap100"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Numero di figli &gt 3 anni senza handicap al 50%
                </label>
            </td>
            <td class="Row1">
                <asp:Label runat="server" ID="lblNFigliMagg3NoHandicap50"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Numero di figli &gt 3 anni con handicap al 100%
                </label>
            </td>
            <td class="Row1">
                <asp:Label runat="server" ID="lblNFigliMagg3Handicap100"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Numero di figli &gt 3 anni con handicap al 50%
                </label>
            </td>
            <td class="Row1">
                <asp:Label runat="server" ID="lblNFigliMagg3Handicap50"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Numero degli altri familiari al 100%</label>
            </td>
            <td class="Row1">
                <asp:Label runat="server" ID="lblNAltriFamiliari100"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Numero degli altri familiari al 50%</label>
            </td>
            <td class="Row1">
                <asp:Label runat="server" ID="lblNAltriFamiliari50"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Addizionale regionale per residenti di Lombardia e Veneto</label>
            </td>
            <td class="Row1">
                <asp:Label runat="server" ID="lblAddizionaleLombardiaVeneto"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Non Residente Schumacker</label>
            </td>
            <td class="Row1">
                <asp:Label runat="server" ID="lblNonResidenteSchumacker"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Convenzioni Doppie Imposizioni</label>
            </td>
            <td class="Row1">
                <asp:Label runat="server" ID="lblConvDoppieImposizioni"></asp:Label>
            </td>
        </tr>
    </table>
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table style="margin-left: auto; margin-right: auto;" class="tab-actions-group position-left">
            <tr>
                <td class="tab-actions-group__first">
                    <asp:Button ID="btnAggiorna" runat="server" Text="Aggiorna" SkinID="btnAzione1" Width="150px"
                        CausesValidation="false" OnClick="AggiornaDetrazioni" OnClientClick="mainValidate()" CssClass="ghost-update" />
                    <%--CausesValidation="true" PostBackUrl="~/ElaborazionePosizione/Detrazioni.aspx"   Width="150px" OnClientClick="aspnetForm.target ='_self';"/>--%>
                </td>
                <td>
                    <asp:Button ID="btnAcquisisci" runat="server" Text="Acquisisci" SkinID="btnAzione1"
                        ValidationGroup="UCDetrazioni" CausesValidation="true" OnClick="AcquisisciDetrazioni"
                        Width="150px" OnClientClick="aspnetForm.target ='_blank';" CssClass="tertiary tertiary-external" />
                </td>
                <td>
                    <asp:Button ID="btnTornaAiSoggetti" runat="server" Text="Torna ai soggetti" SkinID="btnAzione1"
                        Visible="false" ValidationGroup="UCDetrazioni" CausesValidation="true" OnClick="TornaAiSoggetti"
                        OnClientClick="BlockUI()" Width="150px" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<asp:Panel runat="server" ID="pnlSoggetti" Visible="false">
    <div style="width: 95%; margin-left: auto; margin-right: auto; margin-top: 10px;">
        <asp:GridView ID="gv_Soggetti" SkinID="grdElenco1" runat="server" AllowSorting="False"
            AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" Width="100%"
            OnRowCommand="gv_Soggetti_RowCommand" OnRowDataBound="gv_Soggetti_RowDataBound" CssClass="intestazioneTabella">
            <EmptyDataRowStyle ForeColor="Red" />
            <EmptyDataTemplate>
                <center>
                    <asp:Label ID="lblNoData" runat="server" Text="Nessun contitolare trovato." SkinID="lblNoData"
                        Visible="true"></asp:Label>
                </center>
            </EmptyDataTemplate>
            <Columns>
                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="2%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                    ItemStyle-CssClass="TblRecordset3">
                    <ItemTemplate>
                        <asp:Image runat="server" ID="img" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Codice Fiscale" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%"
                    HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                    DataField="CodiceFiscale"></asp:BoundField>
                <asp:BoundField HeaderText="Cognome" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%"
                    HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                    DataField="Cognome"></asp:BoundField>
                <asp:BoundField HeaderText="Nome" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%"
                    HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                    DataField="Nome"></asp:BoundField>
                <asp:BoundField HeaderText="Data Nascita" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%"
                    HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                    DataField="DataNascita" DataFormatString="{0:dd/MM/yyyy}"></asp:BoundField>
                <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%"
                    HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3">
                    <ItemTemplate>
                        <asp:Button runat="server" SkinID="btnAzione1" ID="btnDettaglio" Text="Dettaglio"
                            CommandName="Dettaglio" CommandArgument="<%# ((INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.GestioneDetrazioniSoggetto)Container.DataItem).IdAnagrafica %>"
                            OnClientClick="BlockUI()" CssClass="tertiary viewIconOnly"/>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Panel>
