<%@ Page Language="C#" MasterPageFile="~/ProcedureOperatore.Master" AutoEventWireup="true"
    CodeBehind="UtilitySistema.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UtilitySistema" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlWelcome" runat="server">
        <UCA:UCAvviso Visible="false" ID="ucAvviso" runat="server" />

        <div style="margin-left: 50px; width: 650px;" class="utility-list">
            <div style="display: none;" class="force-block utility-title">Utility di sistema</div>
            <span class="none">E' possibile effettuare le seguenti operazioni:</span>
            <br />
            <br />
            <ul>
                <li id="liGestioneLiquidazioni" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneLiquidazioni.aspx" onclick="BlockUI();">Gestione Liquidazioni<span class="none">:</span></a></strong> Abilitazione e disabilitazione liquidazione sedi</li>
                <li id="liTipologieNonAbilitate" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/TipologieNonAbilitate.aspx" onclick="BlockUI();">Tipologie Non Abilitate<span class="none">:</span></a></strong> Abilitazione e disabilitazione tipologie</li>
                <li id="liSbloccoDomanda" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/SbloccoDomanda.aspx" onclick="BlockUI();">Sblocco Domanda<span class="none">:</span></a></strong> Sblocco domanda WebDom</li>
                <li id="liRiassegnazioneDomanda" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/RiassegnazioneDomanda.aspx" onclick="BlockUI();">Riassegnazione Domanda<span class="none">:</span></a></strong> Riassegnazione domanda ad un altro utente</li>
                <li id="liMonitoraggio" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/Monitoraggio.aspx" onclick="BlockUI();">Monitoraggio<span class="none">:</span></a></strong> Monitoraggio</li>

                <li id="liAvvisi" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/Avvisi.aspx" onclick="BlockUI();">Avvisi<span class="none">:</span></a></strong> Avvisi</li>
                <li id="liMessaggiHermes" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/MessaggiHermes.aspx" onclick="BlockUI();">Messaggi Hermes<span class="none">:</span></a></strong> Messaggi hermes</li>
                <li id="liAggiornamenti" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/Aggiornamenti.aspx" onclick="BlockUI();">Aggiornamenti<span class="none">:</span></a></strong> Aggiornamenti</li>

                <li id="liSbloccoCancellazione" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/SbloccoCancellazione.aspx" onclick="BlockUI();">Sblocco Cancellazione<span class="none">:</span></a></strong> Sblocco cancellazione domanda</li>
                <li id="liLavorazioneManualeAutomatiche" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/LavorazioneManualeAutomatiche.aspx" onclick="BlockUI();">Lavorazione Manuale<span class="none">:</span></a></strong> Autorizzazione lavorazione manuale domande</li>
                <li id="liBypassControlli" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/BypassControlli.aspx" onclick="BlockUI();">Bypass Controlli<span class="none">:</span></a></strong> Bypass controlli per domanda</li>
                <li id="liCambioDataSistema" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/CambioDataSistema.aspx" onclick="BlockUI();">Cambio Data Sistema<span class="none">:</span></a></strong> Cambio data del sistema</li>
                <li id="liGestioneFAQ" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneFAQ.aspx" onclick="BlockUI();">Gestione FAQ<span class="none">:</span></a></strong> Gestione delle FAQ</li>

                <li id="liCambioStatoDomanda" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/CambioStatoDomanda.aspx" onclick="BlockUI();">Cambio Dati Domanda<span class="none">:</span></a></strong> Cambio dati domanda</li>

                <li id="liPulisciDomanda" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/PulisciDomanda.aspx" onclick="BlockUI();">Pulisci Domanda<span class="none">:</span></a></strong> Pulisci domanda</li>

                <li id="liBypassTipologieNonAbilitate" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/BypassTipologieNonAbilitate.aspx" onclick="BlockUI();">Bypass Tipologie Non Abilitate<span class="none">:</span></a></strong> Bypass tipologie non abilitate</li>
                <li id="liAggiornamento" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/Aggiornamento.aspx" onclick="BlockUI();">Funzionalità di aggiornamento post calcolo<span class="none">:</span></a></strong> Permette di effettuare l'aggiornamento post calcolo per tutte le domande che non sono riuscite a completarlo</li>

                <li id="liGestioneTrasformazioni" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneAbilitazioneTrasformazioni.aspx" onclick="BlockUI();">Gestione Trasformazioni<span class="none">:</span></a></strong> Abilitazione e disabilitazione trasformazioni da provvisoria a definitiva per sedi</li>
                <li id="liGestioneBanchefideiussione" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneBancheFideiussione.aspx" onclick="BlockUI();">Gestione Aziende VESO92<span class="none">:</span></a></strong> Visualizzazione, inserimento, modifica e cancellazione di fideiussioni bancarie e Aziende per la Categoria VESO92</li>
                <li id="liGestioneAziendeVESO33" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneAziendeVESO33.aspx" onclick="BlockUI();">Gestione Aziende VESO33<span class="none">:</span></a></strong> Visualizzazione, inserimento, modifica e cancellazione di Aziende VESO33</li>
                <li id="liGestioneAziendeCredito" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneAziendeCredito.aspx" onclick="BlockUI();">Gestione Aziende Credito<span class="none">:</span></a></strong> Visualizzazione, inserimento, modifica e cancellazione di Aziende Credito</li>
                <li id="liGestioneAziendeEditoriali" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneAziendeEditoriali.aspx" onclick="BlockUI();">Gestione Aziende Editoriali art. 37 legge 416/1981, lettera (a)<span class="none">:</span></a></strong> Visualizzazione, inserimento, modifica e cancellazione di Aziende Editoriali art. 37 legge 416/1981, lettera (a)</li>
                <li id="liGestioneAziendeEditorialiLetteraB" style="margin-bottom: 30px;" runat="server"><strong><a href="PrepensionamentoArt37Legge416198LetteraB.aspx" onclick="BlockUI();">Gestione Aziende Editoriali art. 37 legge 416/1981, lettera (b)<span class="none">:</span></a></strong> Visualizzazione, inserimento, modifica e cancellazione di Aziende Editoriali art. 37 legge 416/1981, lettera (b)</li>
                <li id="liGestioneAziendeEditorialiPerTipo0171" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneAziendeEditorialiPerTipo0171.aspx" onclick="BlockUI();">Gestione Aziende Editoriali art.1 comma 154 legge 205/2017<span class="none">:</span></a></strong> Visualizzazione, inserimento, modifica e cancellazione di Aziende Editoriali art.1 comma 154 legge 205/2017</li>
                <li id="liGestioneAziendeEditorialiPerTipo0179" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneAziendeEditorialiPerTipo0179.aspx" onclick="BlockUI();">Gestione Aziende Editoriali art.1 comma 500 legge 160/2019<span class="none">:</span></a></strong> Visualizzazione, inserimento, modifica e cancellazione di Aziende Editoriali art.1 comma 500 legge 160/2019</li>
                <li id="liGestioneAziendeVESO29" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneAziendeVESO29.aspx" onclick="BlockUI();">Gestione Aziende VESO29<span class="none">:</span></a></strong> Visualizzazione, inserimento, modifica e cancellazione di Aziende VESO29</li>
                <li id="liGestioneAziendeVOESO" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneAziendeVOESO.aspx" onclick="BlockUI();">Gestione Aziende VOESO<span class="none">:</span></a></strong> Visualizzazione, inserimento, modifica e cancellazione di Aziende VOESO</li>
                <li id="liGestioneAziendeESOTEL" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneAziendeESOTEL.aspx" onclick="BlockUI();">Gestione Aziende ESOTEL<span class="none">:</span></a></strong> Visualizzazione, inserimento, modifica e cancellazione di Aziende ESOTEL</li>
                <li id="liGestioneAziendeESOAMB" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneAziendeESOAMB.aspx" onclick="BlockUI();">Gestione Aziende ESOAMB<span class="none">:</span></a></strong> Visualizzazione, inserimento, modifica e cancellazione di Aziende ESOAMB</li>
                <li id="liGestioneAziendeESPA" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneAziendeESPA.aspx" onclick="BlockUI();">Gestione Aziende ESPA<span class="none">:</span></a></strong> Visualizzazione, inserimento, modifica e cancellazione di Aziende ESPA</li>
                <li id="liGestioneAziendeESOPMI" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneAziendeESOPMI.aspx" onclick="BlockUI();">Gestione Aziende ESOPMI<span class="none">:</span></a></strong> Visualizzazione, inserimento, modifica e cancellazione di Aziende ESOPMI</li>
                <li id="liGestioneProvvisoriePerCoefficienti" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneProvvisoriePerCoefficienti.aspx" onclick="BlockUI();">Gestione Provvisorie Per Coefficienti<span class="none">:</span></a></strong> Gestione della Data di Decorrenza Provvisoria Obbligatoria</li>
                <li id="liGestioneAbilitazioneServizi" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/GestioneAbilitazioneServizi.aspx" onclick="BlockUI();">Gestione Abilitazione polarizzazione ENPALS<span class="none">:</span></a></strong> Gestione per le chiavi di abilitazione</li>
                <li id="liCambioDataINDCOM" style="margin-bottom: 30px;" runat="server"><strong><a href="AltreFunzioni/CambioDataINDCOM.aspx" onclick="BlockUI();">Cambio data limite domande INDCOM<span class="none">:</span></a></strong> Cambio data limite indennizzi ai commercianti da legge 145/2018</li>
            </ul>
        </div>
    </asp:Panel>
</asp:Content>
