<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSentenze.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCSentenze" %>
<asp:Panel runat="server" ID="pnlSentenze">
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1" colspan="6" style="text-align: left">
                <asp:Label ID="lblTitoloSentenzaCorteCostituzionale" runat="server" Text="Sentenza della Corte Costituzionale"
                    Style="font-weight: bold"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" colspan="6">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <asp:GridView runat="server" ID="gvSentenze" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" Width="100%"
                        PageSize="10" AllowPaging="true"  PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
                            <asp:TemplateField HeaderText="Cod. sentenza di merito" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodSentenzaMerito" Text='<%# Bind("CodSentenzaMerito")%>'
                                        CssClass="txtUppercase">      
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cod. sentenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodSentenza" Text='<%# Bind("CodSentenza")%>' CssClass="txtUppercase">      
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza dal" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenzaSentenzaDal" Text='<%# Bind("DecorrenzaDal", "{0:MM/yyyy}")%>'
                                        CssClass="txtUppercase">      
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza al" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenzaSentenzaAl" Text='<%# Bind("DecorrenzaAl", "{0:MM/yyyy}")%>'
                                        CssClass="txtUppercase">      
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
        <tr>
            <td class="Row1" colspan="6" style="text-align: left">
                <asp:Label ID="lblTitoloRicalcoloSentenza" runat="server" Text="Ricalcolo pensione per sentenza"
                    Style="font-weight: bold"></asp:Label>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td class="Row1" style="width: 18%">
                <label>
                    Sentenza 495/93:</label>
            </td>
            <td class="field" style="width: 12%; text-align: left;">
                <asp:Label runat="server" ID="lblSentenza49593Value" CssClass="txtUppercase"></asp:Label>
            </td>
            <td class="Row1" style="width: 20%">
                <label>
                    Sentenza 240/1994:</label>
            </td>
            <td class="field" style="width: 12%; text-align: left;">
                <asp:Label runat="server" ID="lblSentenza2401994Value" CssClass="txtUppercase"></asp:Label>
            </td>
            <td class="Row1" style="width: 30%">
                <label>
                    Sentenze 495/93 e 240/1994:</label>
            </td>
            <td class="field" style="width: 8; text-align: left">
                <asp:Label runat="server" ID="lblSentenze49593_2401994Value" CssClass="txtUppercase"></asp:Label>
            </td>
        </tr>
    </table>
    <div id="tastoAnnulla" style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%">
            <tr>
                <td style="text-align: right">
                    <asp:Button ID="btnSalva" runat="server" Enabled="true" SkinID="btnAzione1" Text="Salva Dati Sentenze"
                        Width="170px" OnClick="btnSalvaSentenze_Click" OnClientClick="if(Page_ClientValidate('UCTabSentenze')){aspnetForm.target ='_self'; BlockUI();}"
                        CausesValidation="false" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnElimina" runat="server" Enabled="true" SkinID="btnAzione1" Text="Elimina Dati Sentenze"
                        Width="170px" OnClick="btnEliminaSentenze_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Sentenze?')) return false; else BlockUI();" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
