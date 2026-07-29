<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master"
    AutoEventWireup="true" CodeBehind="AggiornamentoWebDom.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.AggiornamentoWebDom" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="720px">
        <tr>
            <td align="center" style="width: 720px">
                <label style="color: #336699; font-weight: bold; font-size: larger; width: 720px">
                    Aggiornamento WebDom</label>
                <br />
                <br />
            </td>
        </tr>
    </table>
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <br />
    <br />
    <asp:Panel ID="pnlElabora" runat="server" Style="border-style: solid; border-color: #000080;
        border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 0px">
        <table class="tabellaFormattazione" width="100%">
            <tr>
                <td class="Row1" style="text-align: left; font-weight: bold; padding-left: 10px">
                    <label>
                        Domande da aggiornare:
                    </label>
                    <asp:Label runat="server" ID="lblDomandeTotali"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="text-align: center">
                    <asp:Button runat="server" ID="btnElabora" OnClick="btnElabora_Click" SkinID="btnAzione1"
                        Text="Elabora" OnClientClick="BlockUI();" Width="150px" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="text-align: center">
                    <asp:Button runat="server" ID="btnPDF" OnClick="btnGeneraPDF_Click" SkinID="btnAzione1"
                        Text="Riepilogo Elaborazione" OnClientClick="BlockUI();" Width="150px" Visible="false" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlElaborazioneInCorso" Visible="false">
        <asp:Panel ID="pnlMessaggio" runat="server" Style="border-style: solid; border-color: #000080;
            border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 0px">
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="text-align: center">
                        <asp:Label ForeColor="Red" runat="server" ID="lblMessaggio">
                    E' in corso una elaborazione. L'operazione potrebbe richiedere diversi minuti.<br />
                    Aggiornare la pagina per verificare lo stato dell'operazione.
                        </asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="text-align: center">
                        <asp:Button runat="server" ID="btnAggiorna" OnClick="btnAggiorna_Click" SkinID="btnAzione1"
                            Text="Aggiorna" OnClientClick="BlockUI();" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlRiepilogo" runat="server" Style="border-style: solid; border-color: #000080;
            border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 0px"
            Visible="false">
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="font-weight: bold; width: 30%;">
                        Domande elaborate:
                    </td>
                    <td class="Row1" style="text-align: center; font-weight: bold; width: 10%;">
                        <asp:Label runat="server" ID="lblDomandeElaborate"></asp:Label>
                    </td>
                    <td style="width: 10%;">
                    </td>
                    <td class="Row1" style="font-weight: bold; width: 30%;">
                        Domande da elaborare:
                    </td>
                    <td class="Row1" style="text-align: center; font-weight: bold; width: 10%;">
                        <asp:Label runat="server" ID="lblDomandeNonElaborate"></asp:Label>
                    </td>
                    <td style="width: 10%;">
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
