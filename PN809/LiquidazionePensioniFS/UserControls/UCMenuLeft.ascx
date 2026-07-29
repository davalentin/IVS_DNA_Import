<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCMenuLeft.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.UCMenuLeft" %>
<div id="menu">
    <div id="blue">
        <ul id="listMenu" class="iconMenu">
            <li id="liHome" runat="server" style="padding-bottom: 5px; border: 2px"><a href="../Default.aspx"
                onclick="BlockUI();">
                <img style="border: 0px" src="../App_Themes/<%= Page.Theme %>/Images/home.png" alt="Home" class="none"/>
                <asp:Label ID="lblHomePage" SkinID="lblVoceMenu" runat="server" Text="Menu Iniziale" />
            </a></li>
            <li id="liTitolare" runat="server" style="padding-bottom: 5px; border: 2px"><a href="Titolare.aspx"
                onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgTitolare" runat="server" AlternateText="Stato Titolare" />
                <asp:Label ID="lblTitolare" SkinID="lblVoceMenu" runat="server" Text="Titolare" />
            </a></li>
            <li id="liPeriodi" runat="server" style="padding-bottom: 5px; border: 2px"><a href="Periodi.aspx"
                onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgPeriodi" runat="server" AlternateText="Stato Periodi" />
                <asp:Label ID="lblPeriodi" SkinID="lblVoceMenu" runat="server" Text="Periodi" />
            </a></li>
            <li id="liFamiliare" runat="server" style="padding-bottom: 5px; border: 2px"><a href="Familiare.aspx"
                onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgFamiliare" runat="server" AlternateText="Stato Familiare" />
                <asp:Label ID="lblFamiliare" SkinID="lblVoceMenu" runat="server" Text="Familiari" />
            </a></li>
            <li id="liDanteCausa" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="DanteCausa.aspx" onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgDanteCausa" runat="server" AlternateText="Stato DanteCausa" />
                <asp:Label ID="lblDanteCausa" SkinID="lblVoceMenu" runat="server" Text="Dante Causa" />
            </a></li>
            <li id="liAventiDiritto" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="AventiDiritto.aspx" onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgAventiDiritto" runat="server" AlternateText="Stato Aventi Diritto" />
                <asp:Label ID="lblAventiDiritto" SkinID="lblVoceMenu" runat="server" Text="Aventi Diritto" />
            </a></li>
            <li id="liAltreDomandeCollegate" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="AltreDomandeCollegate.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgAltreDomandeCollegate" runat="server" AlternateText="Stato Altre Domande Collegate"
                        Visible="false" />
                    <asp:Label ID="lblAltreDomandeCollegate" SkinID="lblVoceMenu" runat="server" Text="Altre Domande Collegate" />
                </a></li>
            <li id="liLiquidazionePensione" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="LiquidazionePensione.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgLiquidazionePensione" runat="server" AlternateText="Stato liquidazione pensione" />
                    <asp:Label ID="lblLiquidazionePensione" SkinID="lblVoceMenu" runat="server" Text="Liquidazione Pensione" />
                </a></li>
            <li id="liLiquidazionePensioneAgo" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="LiquidazionePensioneAgo.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgLiquidazionePensioneAgo" runat="server" AlternateText="Stato liquidazione pensione Ago" />
                    <asp:Label ID="lblLiquidazionePensioneAgo" SkinID="lblVoceMenu" runat="server" Text="Liquidazione Pensione" />
                </a></li>
            <li id="liLiquidazionePensioneCi" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="LiquidazionePensioneCi.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgLiquidazionePensioneCi" runat="server" AlternateText="Stato liquidazione pensione Ci" />
                    <asp:Label ID="lblLiquidazionePensioneCi" SkinID="lblVoceMenu" runat="server" Text="Liquidazione Pensione" />
                </a></li>
            <li id="liDatiFondo" runat="server" style="padding-bottom: 5px; border: 2px"><a href="DatiFondo.aspx"
                onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgDatiFondo" runat="server" AlternateText="Stato dati fondo" />
                <asp:Label ID="lblDatiFondo" SkinID="lblVoceMenu" runat="server" Text="Dati Fondo" />
            </a></li>
            <li id="liDatiFondoAgo" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="DatiFondoAgo.aspx" onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgDatiFondoAgo" runat="server" AlternateText="Stato dati fondo" />
                <asp:Label ID="lblDatiFondoAgo" SkinID="lblVoceMenu" runat="server" Text="Dati Fondo" />
            </a></li>
            <li id="liDatiContributivi" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="DatiContributivi.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgDatiContributivi" runat="server" AlternateText="Stato dati calcolo" />
                    <asp:Label ID="lblDatiContributivi" SkinID="lblVoceMenu" runat="server" Text="Dati Calcolo" />
                </a></li>
            <li id="liDatiContributiviAgo" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="DatiContributiviAgo.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgDatiContributiviAgo" runat="server" AlternateText="Stato dati calcolo" />
                    <asp:Label ID="lblDatiContributiviAgo" SkinID="lblVoceMenu" runat="server" Text="Dati Calcolo" />
                </a></li>
            <li id="liDatiContributiviCi" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="DatiContributiviCi.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgDatiContributiviCi" runat="server" AlternateText="Stato dati calcolo CI" />
                    <asp:Label ID="lblDatiContributiviCi" SkinID="lblVoceMenu" runat="server" Text="Dati Calcolo" />
                </a></li>
            <li id="liDatiNoCalcolo" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="DatiNoCalcolo.aspx" onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgDatiNoCalcolo" runat="server" AlternateText="Dati No Calcolo" />
                <asp:Label ID="lblDatiNoCalcolo" SkinID="lblVoceMenu" runat="server" Text="Dati No Calcolo" />
            </a></li>
            <li id="liMaggiorazioniBenefici" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="MaggiorazioniEBenefici.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgMaggiorazioniEBenefici" runat="server" AlternateText="Stato maggiorazioni e benefici" />
                    <asp:Label ID="lblmaggiorazioniEBenefici" SkinID="lblVoceMenu" runat="server" Text="Maggiorazioni/Benefici" />
                </a></li>
            <li id="liMaggiorazioniBeneficiAgo" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="MaggiorazioniEBeneficiAgo.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgMaggiorazioniEBeneficiAgo" runat="server"
                        AlternateText="Stato maggiorazioni e benefici" />
                    <asp:Label ID="lblmaggiorazioniEBeneficiAgo" SkinID="lblVoceMenu" runat="server"
                        Text="Maggiorazioni/Benefici" />
                </a></li>
            <li id="liMaggiorazioniBeneficiCi" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="MaggiorazioniEBeneficiCi.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgMaggiorazioniEBeneficiCi" runat="server" AlternateText="Stato maggiorazioni e benefici" />
                    <asp:Label ID="lblmaggiorazioniEBeneficiCi" SkinID="lblVoceMenu" runat="server" Text="Maggiorazioni/Benefici" />
                </a></li>
            <li id="liOneri" runat="server" style="padding-bottom: 5px; border: 2px"><a href="Oneri.aspx"
                onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgOneri" runat="server" AlternateText="Stato oneri" />
                <asp:Label ID="lblOneri" SkinID="lblVoceMenu" runat="server" Text="Oneri" />
            </a></li>
            <li id="liSupplementi" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="Supplementi.aspx" onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgSupplementi" runat="server" AlternateText="Supplementi" />
                <asp:Label ID="lblSupplementi" SkinID="lblVoceMenu" runat="server" Text="Supplementi" />
            </a></li>
            <li id="liDetrazioni" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="Detrazioni.aspx" onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgDetrazioni" runat="server" AlternateText="Stato Detrazioni" />
                <asp:Label ID="lblDetrazioni" SkinID="lblVoceMenu" runat="server" Text="Detrazioni" />
            </a></li>
            <li id="liRedditi" runat="server" style="padding-bottom: 5px; border: 2px"><a href="Redditi.aspx"
                onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgRedditi" runat="server" AlternateText="Stato Redditi" />
                <asp:Label ID="lblRedditi" SkinID="lblVoceMenu" runat="server" Text="Redditi" />
            </a></li>
            <li id="liRichiestaBonus" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="RichiestaBonus.aspx" onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgRichiestaBonus" runat="server" AlternateText="Stato Richiesta Bonus" />
                <asp:Label ID="lblRichiestaBonus" SkinID="lblVoceMenu" runat="server" Text="Richiesta Bonus" />
            </a></li>
            <li id="liModalitaPagamento" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="ModalitaPagamento.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgPagamento" runat="server" AlternateText="Stato Pagamento" />
                    <asp:Label ID="lblPagamento" SkinID="lblVoceMenu" runat="server" Text="Modalità Pagamento" />
                </a></li>
            <li id="liBititolarita" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="Bititolarita.aspx" onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgBititolarita" runat="server" AlternateText="Stato Bititolarità" />
                <asp:Label ID="lblBititolarita" SkinID="lblVoceMenu" runat="server" Text="Bititolarità" />
            </a></li>
            <li id="liDelegatoTutore" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="DelegatoTutore.aspx" onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgDelegatoTutore" runat="server" AlternateText="Stato Deleghe/Tutele" />
                <asp:Label ID="lblDelegatoTutore" SkinID="lblVoceMenu" runat="server" Text="Deleghe/Tutele" />
            </a></li>
            <li id="liEliminazione" runat="server" style="padding-bottom: 5px; border: 2px"><a
                href="Eliminazione.aspx" onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgEliminazione" runat="server" AlternateText="Stato Eliminazione" />
                <asp:Label ID="lblEliminazione" SkinID="lblVoceMenu" runat="server" Text="Eliminazione" />
            </a></li>
            <li id="liSindacatoPatronato" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="SindacatoPatronato.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="Image1" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/rosso_tab.png"
                        AlternateText="Stato Sindacato e Patronato" />
                    <asp:Label ID="lblSindacatoPatronato" SkinID="lblVoceMenu" runat="server" Text="Sindacato e Patronato" />
                </a></li>
            <li id="liInviaCalcolo" runat="server" style="padding-top: 10px; padding-bottom: 5px;
                border: 32px"><a href="InvioCalcolo.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgInviaAlCalcolo" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
                        AlternateText="" />
                    <asp:Label ID="lblInvioCalcolo" SkinID="lblVoceMenu" runat="server" Text="Invio al Calcolo" />
                </a></li>
            <li id="liStampa" runat="server" style="padding-top: 10px; padding-bottom: 5px; border: 32px">
                <a href="Stampa.aspx" target="_blank">
                    <asp:Image ImageAlign="TextTop" ID="imgStampa" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
                        AlternateText="" />
                    <asp:Label ID="lblStampa" SkinID="lblVoceMenu" runat="server" Text="Visualizza stampa" />
                </a></li>
            <%--Aggiorna href per puntare al nuovo menù AggiornaWebDom--%>
            <li id="liAggiornaCalcoloNoInd" runat="server" style="padding-top: 10px; padding-bottom: 5px; border: 32px">
                <a href="AggiornaCalcoloNoInd.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="Image3" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
                        AlternateText="" />
                    <asp:Label ID="Label1" SkinID="lblVoceMenu" runat="server" Text="Valutazione causali di debito" />
                </a>
            </li>
            <li id="liAggCI05" runat="server" style="padding-top: 10px; padding-bottom: 5px;
                border: 32px"><a href="AggiornaCI05.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgAggCI05" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
                        AlternateText="" />
                    <asp:Label ID="lblAggCI05" SkinID="lblVoceMenu" runat="server" Text="Aggiorna Staz. Lavoro" />
                </a></li>
            <li id="liAggWebDom" runat="server" style="padding-top: 10px; padding-bottom: 5px;
                border: 32px"><a href="AggiornaWebDom.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgAggWebDom" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
                        AlternateText="" />
                    <asp:Label ID="lblAggWebDom" SkinID="lblVoceMenu" runat="server" Text="Aggiorna WebDom" />
                </a></li>
            <li id="liAggFelpe" runat="server" style="padding-top: 10px; padding-bottom: 5px;
                border: 32px"><a href="AggiornaFelpe.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgAggFelpe" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
                        AlternateText="" />
                    <asp:Label ID="lblAggFelpe" SkinID="lblVoceMenu" runat="server" Text="Aggiorna Felpe" />
                </a></li>
            <li id="liAggOneri" runat="server" style="padding-top: 10px; padding-bottom: 5px;
                border: 32px"><a href="AggiornaOneri.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgAggOneri" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
                        AlternateText="" />
                    <asp:Label ID="lblAggOneri" SkinID="lblVoceMenu" runat="server" Text="Aggiorna Oneri" />
                </a></li>
            <li id="liAggSai" runat="server" style="padding-top: 10px; padding-bottom: 5px; border: 32px">
                <a href="AggiornaSai.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgAggSai" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
                        AlternateText="" />
                    <asp:Label ID="lblAggSai" SkinID="lblVoceMenu" runat="server" Text="Aggiorna SAI" />
                </a></li>
            <li id="liAggINPDAP" runat="server" style="padding-top: 10px; padding-bottom: 5px;
                border: 32px"><a href="AggiornaINPDAP.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgAggINPDAP" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
                        AlternateText="" />
                    <asp:Label ID="lblAggINPDAP" SkinID="lblVoceMenu" runat="server" Text="Aggiorna SIN" />
                </a></li>
            <li id="liAggTotal" runat="server" style="padding-top: 10px; padding-bottom: 5px;
                border: 32px"><a href="AggiornaTotal.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgAggTotal" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
                        AlternateText="" />
                    <asp:Label ID="lblAggTotal" SkinID="lblVoceMenu" runat="server" Text="Aggiorna TOTAL" />
                </a></li>
            <li id="liAggBooking" runat="server" style="padding-top: 10px; padding-bottom: 5px; border: 32px">
                <a href="AggiornaBooking.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgAggBooking" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
                        AlternateText="" />
                    <asp:Label ID="lblAggBooking" SkinID="lblVoceMenu" runat="server" Text="Aggiorna Booking" />
                </a></li>
            <li id="liAggTot" runat="server" style="padding-top: 10px; padding-bottom: 5px; border: 32px">
                <a href="AggiornaTot.aspx" onclick="BlockUI();">
                    <asp:Image ImageAlign="TextTop" ID="imgAggTot" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
                        AlternateText="" />
                    <asp:Label ID="lblAggTot" SkinID="lblVoceMenu" runat="server" Text="Aggiorna Totalizzazioni" />
                </a></li>
            <li id="liAggNoteDebito" runat="server" style="padding-top: 10px; padding-bottom: 5px; border: 32px">
	            <a href="AggiornaNoteDebito.aspx" onclick="BlockUI();">
	            <asp:Image ImageAlign="TextTop" ID="imgAggNoteDebito" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
		            AlternateText="" />
	            <asp:Label ID="lblNoteDebito" SkinID="lblVoceMenu" runat="server" Text="Aggiorna Note di Debito" />
            </a></li>
             <li id="liAggiornaPianiDiPagamento" runat="server" style="padding-top: 10px; padding-bottom: 5px; border: 32px">
	            <a href="AggiornaPianiDiPagamento.aspx" onclick="BlockUI();">
	            <asp:Image ImageAlign="TextTop" ID="imgAggPianiDiPagamento" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
		            AlternateText="" />
	            <asp:Label ID="lblPianiDiPagamento" SkinID="lblVoceMenu" runat="server" Text="Aggiorna Piani Di Pagamento" />
            </a></li>

              <li id="liAggiornaEquoInd" runat="server" style="padding-top: 10px; padding-bottom: 5px; border: 32px">
	            <a href="AggiornaEquoInd.aspx" onclick="BlockUI();">
	            <asp:Image ImageAlign="TextTop" ID="Image2" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
		            AlternateText="" />
	            <asp:Label ID="lblEquoInd" SkinID="lblVoceMenu" runat="server" Text="Aggiorna Piani Di Pagamento" />
            </a></li>

            <li id="liAggiornaIndennitaSpeciale" runat="server" style="padding-top: 10px; padding-bottom: 5px; border: 32px">
	            <a href="AggiornaIndennitaSpeciale.aspx" onclick="BlockUI();">
	            <asp:Image ImageAlign="TextTop" ID="Image" runat="server" ImageUrl="~/App_Themes/BlueINPS1/Images/arrow-right2.png"
		            AlternateText="" />
	            <asp:Label ID="lblIndennSpec" SkinID="lblVoceMenu" runat="server" Text="Aggiorna Piani Di Pagamento" />
            </a></li>

            <li id="liPresaInCarico" runat="server" style="padding-bottom: 5px; border: 32px"><a
                href="PresaInCarico.aspx" onclick="BlockUI();">
                <asp:Image ImageAlign="TextTop" ID="imgPresaInCarico" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/arrow-right2.png"
                    AlternateText="" />
                <asp:Label ID="lblPresaInCarico" SkinID="lblVoceMenu" runat="server" Text="Presa In Carico" />
            </a></li>
            <%--
                    <li id="liExit" runat="server" style="padding-top: 10px; padding-bottom: 5px; border: 32px">
                <a href="#" onclick="if(!window.confirm('Sei sicuro di voler chiudere il browser?')) return false; else closeBrowser();">
                    <asp:Image ImageAlign="Middle" ID="imgExit" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/exit.png"
                        AlternateText="" />
                    <asp:Label ID="lblExit" SkinID="lblVoceMenu" runat="server" Text="Uscita" />
                </a></li>
            --%>
            <li id="liDataSistema" runat="server" visible="false" style="padding-top: 10px; padding-bottom: 5px;
                border: 32px">
                <asp:Label runat="server" ID="lblDataSistema" Style="color: Navy; font-weight: bold;" />
            </li>
        </ul>
    </div>
</div>
