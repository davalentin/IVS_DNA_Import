<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCOneriStoricoGP.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Oneri.UCOneriStoricoGP" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<asp:Panel runat="server" ID="pnlOneriStoricoGP">
    <br />
    <!-- GridView Oneri -->
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblTitoloGV0neriStoricoGP" runat="server" Text="Oneri" Style="font-weight: bold" CssClass="section-label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
            </td>
        </tr>
    </table>
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco" style="width: 700px; margin: 7px;">
                    <asp:GridView runat="server" ID="gvOneriStoricoGP" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="false"
                        Width="100%" PageSize="10" AllowPaging="true" OnPageIndexChanging="gvOneriStoricoGP_onPageIndexChanging"
                        OnRowDataBound="gvOneriStoricoGP_RowDataBound" OnLoad="gvOneriStoricoGP_Load" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessun dato 'Oneri' trovato." SkinID="lblNoData"
                                    Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderText="Gruppo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblGruppo" Text='<%#Bind("IdCodeGruppo")%>' Width="100px"> 
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="SottoGruppo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSottoGruppo" Text='<%#Bind("IdCodeSottoGruppo")%>'
                                        Width="100px"> 
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Dec. Ben." HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza" CssClass="txtUppercase" Width="70px" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cess. Ben." HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCessazione" CssClass="txtUppercase" Width="75px" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Sett." HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane" Text='<%#Bind("Settimane")%>' Width="40px"> 
                                    </asp:Label>
                                </ItemTemplate>                               
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Onere" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblOnere" Text='<%#Bind("Onere")%>' Width="100px"> </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="Cess. incumulabilità" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCessBenIncumul" CssClass="txtUppercase" Width="75px" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    <!-- Fine GridView Oneri -->
    <br />
    <br />
    <br />
    <asp:Panel runat="server" ID="pnlBeneficiParticolariStoricoGP">
    <!-- GridView Benefici Particolari -->
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblTitoloBeneficiParticolariStoricoGP" runat="server" Text="Benefici Particolari"
                    Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco" style="width: 700px; margin: 7px;">
                    <asp:GridView runat="server" ID="gvBeneficiStoricoGP" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="false"
                        Width="100%" PageSize="10" AllowPaging="true" OnPageIndexChanging="gvBeneficiStoricoGP_onPageIndexChanging"
                        OnRowDataBound="gvBeneficiStoricoGP_RowDataBound" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessun dato 'Benefici Particolari' trovato."
                                    SkinID="lblNoData" Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderText="Codice Benefici" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceBeneficiStoricoGP" Text='<%#Bind("CodiceBenefici")%>'
                                        Width="150px"> 
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimaneStoricoGP" Text='<%#Bind("Settimane")%>' Width="150px"> 
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    <!-- Fine GridView Benefici Particolari -->
    </asp:Panel>
     <div style="margin-top: 200px; margin-right: 40px;" class="containerWidth xs">
     </div>
</asp:Panel>
