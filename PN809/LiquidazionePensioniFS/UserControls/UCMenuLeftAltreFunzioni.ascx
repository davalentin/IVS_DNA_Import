<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCMenuLeftAltreFunzioni.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.UCMenuLeftAltreFunzioni" %>
<div id="menu">
    <div id="blue">
        <!--[if lte IE 7.0]> 
<script type="text/javascript">
            blue.style.display = "inline";
</script>
<![endif]-->
        <!--[if IE 8]>
<script type="text/javascript"> blue.style.display = "inline-table";</script>
<![endif]-->
        <ul id="listMenu" class="<%# GetlLstMenuClass() %>">
            <li id="liHome" runat="server" style="padding-bottom: 5px; border: 2px"><a href="../Default.aspx"
                onclick="BlockUI();">
                <img style="border: 0px" src="../App_Themes/<%= Page.Theme %>/Images/home.png" alt="Home" class="none"/>
                <asp:Label ID="lblHomePage" SkinID="lblVoceMenu" runat="server" Text="Menu Iniziale" />
            </a></li>
            <li id="liGestioneLiquidazioni" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/GestioneLiquidazioni.aspx" onclick="BlockUI();" id="aGestioneLiquidazioni"
                    runat="server">
                    <asp:Label ID="lblGestioneLiquidazioni" SkinID="lblVoceMenu" runat="server" Text="Gestione Liquidazioni" />
                </a></li>
            <li id="liTipologieNonAbilitate" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/TipologieNonAbilitate.aspx" onclick="BlockUI();" id="aTipologieNonAbilitate"
                    runat="server">
                    <asp:Label ID="lblTipologieNonAbilitate" SkinID="lblVoceMenu" runat="server" Text="Tipologie Non Abilitate" />
                </a></li>
            <li id="liSbloccoDomanda" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="~/AltreFunzioni/SbloccoDomanda.aspx" onclick="BlockUI();" id="aSbloccoDomanda"
                runat="server">
                <asp:Label ID="lblSbloccoDomanda" SkinID="lblVoceMenu" runat="server" Text="Sblocco Domanda" />
            </a></li>
            <li id="liRiassegnazioneDomanda" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/RiassegnazioneDomanda.aspx" onclick="BlockUI();" id="a1"
                    runat="server">
                    <asp:Label ID="lblRiassegnazioneDomanda" SkinID="lblVoceMenu" runat="server" Text="Riassegnazione Domanda" />
                </a></li>
            <li id="liMonitoraggio" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="~/AltreFunzioni/Monitoraggio.aspx" onclick="BlockUI();" id="aMonitoraggio"
                runat="server">
                <asp:Label ID="lblMonitoraggio" SkinID="lblVoceMenu" runat="server" Text="Monitoraggio" />
            </a></li>
            <li id="liAvvisi" runat="server" style="padding-bottom: 5px; border: 2px"><a href="~/AltreFunzioni/Avvisi.aspx"
                onclick="BlockUI();" id="a2" runat="server">
                <asp:Label ID="lblAvvisi" SkinID="lblVoceMenu" runat="server" Text="Avvisi" />
            </a></li>
            <li id="liMessaggiHermes" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="~/AltreFunzioni/MessaggiHermes.aspx" onclick="BlockUI();" id="a3" runat="server">
                <asp:Label ID="lblMessaggiHermes" SkinID="lblVoceMenu" runat="server" Text="Messaggi Hermes" />
            </a></li>
            <li id="liAggiornamenti" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="~/AltreFunzioni/Aggiornamenti.aspx" onclick="BlockUI();" id="aAggiornamenti"
                runat="server">
                <asp:Label ID="lblAggiornamenti" SkinID="lblVoceMenu" runat="server" Text="Aggiornamenti" />
            </a></li>
            <li id="liSbloccoCancellazione" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/SbloccoCancellazione.aspx" onclick="BlockUI();" id="a4"
                    runat="server">
                    <asp:Label ID="lblSbloccoCancellazione" SkinID="lblVoceMenu" runat="server" Text="Sblocco Cancellazione" />
                </a></li>
            <li id="liLavorazioneManualeAutomatiche" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="~/AltreFunzioni/LavorazioneManualeAutomatiche.aspx" onclick="BlockUI();" id="aLavorazioneManualeAutomatiche"
                runat="server">
                <asp:Label ID="lblLavorazioneManualeAutomatiche" SkinID="lblVoceMenu" runat="server" Text="Lavorazione Manuale" />
            </a></li>
            <li id="liBypassControlli" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="~/AltreFunzioni/BypassControlli.aspx" onclick="BlockUI();" id="aBypassControlli"
                runat="server">
                <asp:Label ID="lblBypassControlli" SkinID="lblVoceMenu" runat="server" Text="Bypass Controlli" />
            </a></li>
            <li id="liCambioDataSistema" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/CambioDataSistema.aspx" onclick="BlockUI();" id="a5" runat="server">
                    <asp:Label ID="lblCambioDataSistema" SkinID="lblVoceMenu" runat="server" Text="Cambio Data Sistema" />
                </a></li>
            <li id="liGestioneFAQ" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="~/AltreFunzioni/GestioneFAQ.aspx" onclick="BlockUI();" id="a6" runat="server">
                <asp:Label ID="lblGestioneFAQ" SkinID="lblVoceMenu" runat="server" Text="Gestione FAQ" />
            </a></li>
            <li id="liCambioStatoDomanda" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/CambioStatoDomanda.aspx" onclick="BlockUI();" id="a7" runat="server">
                    <asp:Label ID="lblCambioStatoDomanda" SkinID="lblVoceMenu" runat="server" Text="Cambio Dati Domanda" />
                </a></li>
            <li id="liPulisciDomanda" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="~/AltreFunzioni/PulisciDomanda.aspx" onclick="BlockUI();" id="a8" runat="server">
                <asp:Label ID="lblPulisciDomanda" SkinID="lblVoceMenu" runat="server" Text="Pulizia Domanda" />
            </a></li>
            <li id="liBypassTipologieNonAbilitate" runat="server" style="padding-bottom: 5px;
                border: 2px"><a href="~/AltreFunzioni/BypassTipologieNonAbilitate.aspx" onclick="BlockUI();"
                    id="a9" runat="server">
                    <asp:Label ID="lblBypassTipologieNonAbilitate" SkinID="lblVoceMenu" runat="server"
                        Text="Bypass Tipologie Non Abilitate" />
                </a></li>
            <li id="liAggiornamento" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="~/AltreFunzioni/Aggiornamento.aspx" onclick="BlockUI();" id="aAggiornamento"
                runat="server">
                <asp:Label ID="lblAggiornamento" SkinID="lblVoceMenu" runat="server" Text="Funzionalità di aggiornamento post calcolo" />
            </a></li>
            <li id="liGestioneTrasformazioni" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/GestioneAbilitazioneTrasformazioni.aspx" onclick="BlockUI();"
                    id="aGestioneTrasformazioni" runat="server">
                    <asp:Label ID="lblGestioneTrasformazioni" SkinID="lblVoceMenu" runat="server" Text="Gestione Trasformazioni" />
                </a></li>
            <li id="liGestioneBancheFideiussione" runat="server" style="padding-bottom: 5px;
                border: 2px"><a href="~/AltreFunzioni/GestioneBancheFideiussione.aspx" onclick="BlockUI();"
                    id="aGestBancheFid" runat="server">
                    <asp:Label ID="lblGestBancheFid" SkinID="lblVoceMenu" runat="server" Text="Gestione Aziende VESO92" />
                </a></li>
            <li id="liGestioneAziendeVESO33" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/GestioneAziendeVESO33.aspx" onclick="BlockUI();" id="a10"
                    runat="server">
                    <asp:Label ID="lblGestAziendeVESO33" SkinID="lblVoceMenu" runat="server" Text="Gestione Aziende VESO33" />
                </a></li>
            <li id="liGestioneAziendeCredito" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/GestioneAziendeCredito.aspx" onclick="BlockUI();" id="a11"
                    runat="server">
                    <asp:Label ID="lblGestAziendeCredito" SkinID="lblVoceMenu" runat="server" Text="Gestione Aziende Credito" />
                </a></li>
            <li id="liGestioneAziendeEditoriali" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/GestioneAziendeEditoriali.aspx" onclick="BlockUI();" id="a12"
                    runat="server">
                    <asp:Label ID="lblGestAziendeEditoriali" SkinID="lblVoceMenu" runat="server" Text="Gestione Aziende Editoriali art. 37 legge 416/1981, lettera (a)" />
                </a></li>
            <li id="liGestioneAziendeEditorialiLetteraB" runat="server" style="padding-bottom: 5px;
                border: 2px"><a href="~/PrepensionamentoArt37Legge416198LetteraB.aspx" onclick="BlockUI();"
                    id="a19" runat="server">
                    <asp:Label ID="Label3" SkinID="lblVoceMenu" runat="server" Text="Gestione Aziende Editoriali art. 37 legge 416/1981, lettera (b)" />
                </a></li>
            <li id="liGestioneAziendeEditorialiPerTipo0171" runat="server" style="padding-bottom: 5px;
                border: 2px"><a href="~/AltreFunzioni/GestioneAziendeEditorialiPerTipo0171.aspx"
                    onclick="BlockUI();" id="aGestioneAziendeEditorialiPerTipo0171" runat="server">
                    <asp:Label ID="lblGestioneAziendeEditorialiPerTipo0171" SkinID="lblVoceMenu" runat="server"
                        Text="Gestione Aziende Editoriali art.1 comma 154 legge 205/2017" />
                </a></li>
            <li id="liGestioneAziendeEditorialiPerTipo0179" runat="server" style="padding-bottom: 5px;
                border: 2px"><a href="~/AltreFunzioni/GestioneAziendeEditorialiPerTipo0179.aspx"
                    onclick="BlockUI();" id="a15" runat="server">
                    <asp:Label ID="Label1" SkinID="lblVoceMenu" runat="server" Text="Gestione Aziende Editoriali art. 1 comma 500 legge 160/2019" />
                </a></li>
            <li id="liGestioneAziendeVESO29" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/GestioneAziendeVESO29.aspx" onclick="BlockUI();" id="a13"
                    runat="server">
                    <asp:Label ID="lblGestAziendeVESO29" SkinID="lblVoceMenu" runat="server" Text="Gestione Aziende VESO29" />
                </a></li>
            <li id="liGestioneAziendeVOESO" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/GestioneAziendeVOESO.aspx" onclick="BlockUI();" id="aGestioneAziendeVOESO"
                    runat="server">
                    <asp:Label ID="lblGestioneAziendeVOESO" SkinID="lblVoceMenu" runat="server" Text="Gestione Aziende VOESO" />
                </a></li>
            <li id="liGestioneAziendeESOTEL" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/GestioneAziendeESOTEL.aspx" onclick="BlockUI();" id="aGestioneAziendeESOTEL"
                    runat="server">
                    <asp:Label ID="lblgestioneAziendeESOTEL" SkinID="lblVoceMenu" runat="server" Text="Gestione Aziende ESOTEL" />
                </a></li>
            <li id="liGestioneAziendeESOAMB" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/GestioneAziendeESOAMB.aspx" onclick="BlockUI();" id="a14"
                    runat="server">
                    <asp:Label ID="lblGestioneAziendeESOAMB" SkinID="lblVoceMenu" runat="server" Text="Gestione Aziende ESOAMB" />
                </a></li>
            <li id="liGestioneAziendeESPA" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/GestioneAziendeESPA.aspx" onclick="BlockUI();" id="a16"
                    runat="server">
                    <asp:Label ID="lblGestioneAziendeESPA" SkinID="lblVoceMenu" runat="server" Text="Gestione Aziende ESPA" />
                </a></li>
            <li id="liGestioneAziendeESOPMI" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/GestioneAziendeESOPMI.aspx" onclick="BlockUI();" id="a17"
                    runat="server">
                    <asp:Label ID="lblGestioneAziendeESOPMI" SkinID="lblVoceMenu" runat="server" Text="Gestione Aziende ESOPMI" />
                </a></li>
            <li id="liGestioneProvvisoriePerCoefficienti" runat="server" style="padding-bottom: 5px;
                border: 2px"><a href="~/AltreFunzioni/GestioneProvvisoriePerCoefficienti.aspx" onclick="BlockUI();"
                    id="aGestProvCoeff" runat="server">
                    <asp:Label ID="LabelGestProvvisorieCoeffic" SkinID="lblVoceMenu" runat="server" Text="Gestione Provvisorie Per Coefficienti" />
                </a></li>
            <li id="liGestioneAbilitazioneServizi" runat="server" style="padding-bottom: 5px;
                border: 2px"><a href="~/AltreFunzioni/GestioneAbilitazioneServizi.aspx" onclick="BlockUI();"
                    id="aGestAbilitServizi" runat="server">
                    <asp:Label ID="LabelGestioneAbilitazioneServizi" SkinID="lblVoceMenu" runat="server"
                        Text="Gestione Abilitazione polarizzazione ENPALS" />
                </a></li>
            <li id="lICambioDataINDCOM" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/AltreFunzioni/CambioDataINDCOM.aspx" onclick="BlockUI();" id="a18" runat="server">
                    <asp:Label ID="Label2" SkinID="lblVoceMenu" runat="server" Text="Cambio data limite domande INDCOM" />
                </a></li>
            <li id="liDataSistema" runat="server" visible="false" style="padding-top: 10px; padding-bottom: 5px;
                border: 32px">
                <asp:Label runat="server" ID="lblDataSistema" Style="color: Navy; font-weight: bold;" />
            </li>
        </ul>
    </div>
</div>
