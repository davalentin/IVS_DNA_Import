<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCQuotaFondoINPGIStoricoGP.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo.UCQuotaFondoINPGIStoricoGP" %>
<script type="text/javascript">

    function setPeriodoRetr(i) {
        var row = i.parentNode.parentNode;
        var rowIndex = row.rowIndex;
        if (i.selectedIndex > 0) {
            var periodi = document.getElementById("<%=hdnPeriodiRetrib.ClientID%>").value.split(";");
            document.getElementById('<%=gvRetributiviINPGI.ClientID %>').rows[rowIndex].cells[2].childNodes[1].innerHTML = periodi[i.selectedIndex - 1];
        }
        else
            document.getElementById('<%=gvRetributiviINPGI.ClientID %>').rows[rowIndex].cells[2].childNodes[1].innerHTML = "";
    }

    function setPeriodoContr(i) {
        var row = i.parentNode.parentNode;
        var rowIndex = row.rowIndex;
        if (i.selectedIndex > 0) {
            var periodi = document.getElementById("<%=hdnPeriodiContrib.ClientID%>").value.split(";");
            document.getElementById('<%=gvContributiviINPGI.ClientID %>').rows[rowIndex].cells[2].childNodes[1].innerHTML = periodi[i.selectedIndex - 1];
        }
        else
            document.getElementById('<%=gvContributiviINPGI.ClientID %>').rows[rowIndex].cells[2].childNodes[1].innerHTML = "";
    }

</script>
<asp:Panel runat="server" ID="pnlQuotaFondoInpgiStorico">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
            </td>
        </tr>
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblRicNonContrib" runat="server" Text="I dati di calcolo sono disponibili per la sola visualizzazione.  Possono essere modificati con una Ricostituzione contributiva."
                    Style="font-weight: bold" ForeColor="Black" Visible="false"></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <div id="divRetributiviINPGI" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblDatiRetributivi">Dati Retributivi:</asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:GridView runat="server" ID="gvRetributiviINPGI" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella" BorderWidth="1" Width="100%" BorderColor="Black"
                        AutoGenerateEditButton="false" AllowPaging="false" OnRowDataBound="gvRetributiviINPGI_RowDataBound"
                        EnableViewState="true" OnLoad="gvRetributiviINPGI_Load">
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Quote"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblCodiceGestioneRetribQuotaFondo_item" runat="server" CssClass="txtUppercase"
                                        Width="80px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Periodo"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblPeriodoRetr" runat="server" Width="150px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane" Width="40px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Retribuzione Media Settimanale" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRetribuzioneMediaSettimanale" Width="100px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Importo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblImportoCalcolato" Width="100px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane Comma 707" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimaneComma707" Width="40px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Importo Comma 707" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblImportoComma707" Width="100px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <div id="divContributiviINPGI" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblDatiContributivi">Dati Contributivi:</asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:GridView ID="gvContributiviINPGI" runat="server" AllowPaging="false" AutoGenerateColumns="false"
                        AutoGenerateEditButton="false" BorderColor="Black" BorderWidth="1" CssClass="intestazioneTabella"
                        EnableViewState="true" OnRowDataBound="gvContributiviINPGI_RowDataBound" SkinID="grdElenco1"
                        Width="100%">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessuna quota inserita." SkinID="lblNoData"
                                    Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Quota"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblCodiceGestioneQuotaFondo_item" runat="server" CssClass="txtUppercase"
                                        Width="80px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Periodo"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblPeriodoContr" runat="server" Width="150px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Settimane"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblSettimaneContr" runat="server" Text='<%#Bind("Settimane") %>' Width="40px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Montante"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblMontante" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Quota Contributivo"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblQuota" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<div style="min-height: 50px;">
</div>
<asp:HiddenField runat="server" ID="hdnPeriodiRetrib" Value="" />
<asp:HiddenField runat="server" ID="hdnPeriodiContrib" Value="" />
