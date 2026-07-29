<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master"
    AutoEventWireup="true" CodeBehind="Aggiornamento.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.Aggiornamento" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/AltreFunzioni/Aggiornamento/UCAggiornamento.ascx"
    TagName="UCAggiornamento" TagPrefix="UCAGG" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        hr {
            border: 0;
            width: 100%;
            color: #336699;
            background-color: #336699;
            height: 2px;
            margin-bottom: 15px;
            margin-top: 1px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Funzionalità di aggiornamento post calcolo</h2>
        <h6 class="page-subtitle">Permette di effettuare l'aggiornamento post calcolo per tutte le domande che non sono riuscite a completarlo</h6>
    </div>

    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" ShowClose="true" OnCloseToastEvt="event_ucHideAvviso" />

    <asp:Panel runat="server" ID="pnlInfo" CssClass="cards-container">
        <UCAGG:UCAggiornamento runat="server" ID="ucAggiornamentoWebDom" Titolo="WebDom" OnShowElaborazioneInCorso="event_ucShowElaborazioneInCorso"
            OnRecuperaInformazioni="event_ucRecuperaInformazioni" OnHideAvviso="event_ucHideAvviso"></UCAGG:UCAggiornamento>

        <UCAGG:UCAggiornamento runat="server" ID="ucAggiornamentoFelpe" Titolo="Felpe" OnShowElaborazioneInCorso="event_ucShowElaborazioneInCorso"
            OnRecuperaInformazioni="event_ucRecuperaInformazioni" OnHideAvviso="event_ucHideAvviso"></UCAGG:UCAggiornamento>

        <UCAGG:UCAggiornamento runat="server" ID="ucAggiornamentoOneri" Titolo="Oneri" OnShowElaborazioneInCorso="event_ucShowElaborazioneInCorso"
            OnRecuperaInformazioni="event_ucRecuperaInformazioni" OnHideAvviso="event_ucHideAvviso"></UCAGG:UCAggiornamento>

        <asp:Panel runat="server" ID="pnlAgoVisible" Visible="false" CssClass="cards-container">
            <UCAGG:UCAggiornamento runat="server" ID="ucAggiornamentoSAI" Titolo="SAI" OnShowElaborazioneInCorso="event_ucShowElaborazioneInCorso"
                OnRecuperaInformazioni="event_ucRecuperaInformazioni" OnHideAvviso="event_ucHideAvviso"></UCAGG:UCAggiornamento>

            <UCAGG:UCAggiornamento runat="server" ID="ucAggiornamentoCumulo" Titolo="Cumulo" OnShowElaborazioneInCorso="event_ucShowElaborazioneInCorso"
                OnRecuperaInformazioni="event_ucRecuperaInformazioni" OnHideAvviso="event_ucHideAvviso"></UCAGG:UCAggiornamento>

            <UCAGG:UCAggiornamento runat="server" ID="ucAggiornamentoTot" Titolo="Totalizzazione" OnShowElaborazioneInCorso="event_ucShowElaborazioneInCorso"
                OnRecuperaInformazioni="event_ucRecuperaInformazioni" OnHideAvviso="event_ucHideAvviso"></UCAGG:UCAggiornamento>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlFsVisible" Visible="false" CssClass="cards-container">
            <UCAGG:UCAggiornamento runat="server" ID="ucAggiornamentoINPDAP" Titolo="SIN" OnShowElaborazioneInCorso="event_ucShowElaborazioneInCorso"
                OnRecuperaInformazioni="event_ucRecuperaInformazioni" OnHideAvviso="event_ucHideAvviso"></UCAGG:UCAggiornamento>

            <UCAGG:UCAggiornamento runat="server" ID="ucAggiornamentoNoteDebito" Titolo="Note di Debito" OnShowElaborazioneInCorso="event_ucShowElaborazioneInCorso"
                OnRecuperaInformazioni="event_ucRecuperaInformazioni" OnHideAvviso="event_ucHideAvviso"></UCAGG:UCAggiornamento>

            <UCAGG:UCAggiornamento runat="server" ID="ucAggiornamentoPianiDiPagamento" Titolo="Piani di Pagamento" OnShowElaborazioneInCorso="event_ucShowElaborazioneInCorso"
                OnRecuperaInformazioni="event_ucRecuperaInformazioni" OnHideAvviso="event_ucHideAvviso"></UCAGG:UCAggiornamento>
        </asp:Panel>
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlElaborazioneInCorso" Visible="false">
        <asp:Panel ID="pnlMessaggio" runat="server" Style="border: 1px solid #f2f6fc" CssClass="p-16">
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td style="text-align: center">
                        <asp:Label runat="server" ID="lblMessaggio">
                    E' in corso una elaborazione. L'operazione potrebbe richiedere diversi minuti.<br />
                    Aggiornare la pagina per verificare lo stato dell'operazione.
                        </asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="justify-end">
                        <asp:Button runat="server" ID="btnAggiorna" OnClick="btnAggiorna_Click" SkinID="btnAzione1"
                            Text="Aggiorna" OnClientClick="BlockUI();" CssClass="ghost-update mt-16" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlRiepilogo" runat="server" Style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 0px"
            Visible="false">
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="font-weight: bold; width: 30%;">Domande elaborate:
                    </td>
                    <td class="Row1" style="text-align: center; font-weight: bold; width: 10%;">
                        <asp:Label runat="server" ID="lblDomandeElaborate"></asp:Label>
                    </td>
                    <td style="width: 10%;"></td>
                    <td class="Row1" style="font-weight: bold; width: 30%;">Domande da elaborare:
                    </td>
                    <td class="Row1" style="text-align: center; font-weight: bold; width: 10%;">
                        <asp:Label runat="server" ID="lblDomandeNonElaborate"></asp:Label>
                    </td>
                    <td style="width: 10%;"></td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
