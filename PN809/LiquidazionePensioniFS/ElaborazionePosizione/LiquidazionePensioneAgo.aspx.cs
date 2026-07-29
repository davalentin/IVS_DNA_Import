using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.Security;
using INPS.DNA.Security.Idm;
using INPS.DNA.UI.Web;
using INPS.DNA.UI.Web.Intranet;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.View.Web;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.CrossSuppLiqAgo;
using INPS.DNA.Presenter.Interface;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class LiquidazionePensioneAgo : CustomBasePage, IInfoLiquidazione, IQuadriSemafori, ITitolarePensione, ILiquidazionePensioneAgo, ICrossContribuzioneEnpals
    {
        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region ILiquidazionePensioneAgo
        public AreaLiquidazionePensione areaLiquidazionePensioneAgo { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ILiquidazionePensioneAgo

        #region ITitolarePensione
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolarePensione

        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region ICrossContribuzioneEnpals
        public Presenter.SvrLiquidazione.DatiContribuzioneEnpals DatiContribuzioneEnpals { get; set; }
        public Presenter.SvrLiquidazione.TipologiaContribuzioneEnpals Tipologia { get; set; }
        public bool IsContribuzioneEnpalsRetributivaVisible { get; set; }
        public bool IsContribuzioneEnpalsContributivaVisible { get; set; }
        #endregion ICrossContribuzioneEnpals

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                this.GetDatiPensione(this);
                CaricaDatiLiquidazione();
                ManageBtnPage();
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                ValorizzaSemaforiTab(imgDatiGenerici, this.areaQuadri.QuadroLiquidazionePensione.TabDatiGenerici, pnlTabDatiGenerici);
                ValorizzaSemaforiTab(imgDatiAssicurativi, this.areaQuadri.QuadroLiquidazionePensione.TabDatiAssicurativi, pnlTabDatiAssicurativi);
                ValorizzaSemaforiTab(imgIstruttoria, this.areaQuadri.QuadroLiquidazionePensione.TabIstruttoria, pnlTabIstruttoria);
                ValorizzaSemaforiTab(imgOpzione, this.areaQuadri.QuadroLiquidazionePensione.TabOpzione, pnlTabOpzione);
                ValorizzaSemaforiTab(imgPrecedentePensione, this.areaQuadri.QuadroLiquidazionePensione.TabPrecedentePensione, pnlTabPrecedentePensione);
                ValorizzaSemaforiTab(imgINAIL, this.areaQuadri.QuadroLiquidazionePensione.TabInail, pnlTabINAIL);
                ValorizzaSemaforiTab(imgContribuzioneEnpals, this.areaQuadri.QuadroLiquidazionePensione.TabDatiContributiviEnpals, pnlTabContribuzioneEnpals);
                ValorizzaSemaforiTab(imgStorico, this.areaQuadri.QuadroLiquidazionePensione.TabStorico, pnlTabStorico);
                ValorizzaSemaforiTab(imgSentenzaArt4, this.areaQuadri.QuadroLiquidazionePensione.TabSentenzaArt4, pnlTabSentenzaArt4);
                ValorizzaSemaforiTab(imgSentenze, this.areaQuadri.QuadroLiquidazionePensione.TabSentenze, pnlTabSentenze);
            }
        }

        protected void SalvaLiquidazionePensioneAgo_Click(Object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            areaLiquidazionePensioneAgo = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo.AreaLiquidazionePensione();
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();

            areaLiquidazionePensioneAgo.DatiGenerici = ucDatiGenerici.GetDatiGenerici();
            areaLiquidazionePensioneAgo.DatiAssicurativi = ucDatiAssicurativi.GetDatiAssicurativi();
            areaLiquidazionePensioneAgo.DatiOpzione = ucOpzione.GetDatiOpzione();
            areaLiquidazionePensioneAgo.DatiProvenienza = ucPrecedentePensione.GetDatiProvenienza();
            areaLiquidazionePensioneAgo.DatiIstruttoria = ucIstruttoria.GetDatiIstruttoria();
            areaLiquidazionePensioneAgo.DatiInail = ucInail.GetValoriInail();
            areaLiquidazionePensioneAgo.DatiSentenzaArt4 = ucSentenzaArt4.GetDatiUcSentenzaArt4();
            //areaLiquidazionePensioneAgo.DatiSentenze = ucSentenze.GetDatiUcSentenze();
            if (this.domanda.IsDomandaENPALS && this.areaLiquidazionePensioneAgo != null)
            {
                areaLiquidazionePensioneAgo.DatiContribuzioneEnpals = Utility.GetDatiContribuzioneEnpalsSvrLiquidazioneAgo(ucContribEnpals.GetDatiContribuzioneEnpals());
            }

            presenterLiquidazione.SalvaLiquidazionePensioneAgo(this);
            if (this.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;

                ucDatiGenerici.ManageCodNatura();
            }
            else
            {
                //if (areaLiquidazionePensioneAgo.DatiProvenienza.Equals(new DatiProvenienza()))
                if (!areaLiquidazionePensioneAgo.DatiProvenienza.CertificatoPrecedentePensione.HasValue && !areaLiquidazionePensioneAgo.DatiProvenienza.CodiceP18PrecedentePensione.HasValue &&
                    !areaLiquidazionePensioneAgo.DatiProvenienza.DecorrenzaCaricoPrecedentePensione.HasValue && !areaLiquidazionePensioneAgo.DatiProvenienza.DecorrenzaOriginariaAltraPensione.HasValue &&
                    !areaLiquidazionePensioneAgo.DatiProvenienza.SedePrecedentePensione.HasValue)
                    ucDatiGenerici.SetHiddenPrecedentePensioneValue("false");
                else
                    ucDatiGenerici.SetHiddenPrecedentePensioneValue("true");

                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Liquidazione Pensione salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            elencoTab.Add(AreaQuadri.Tab.Bititolarita);
            elencoTab.Add(AreaQuadri.Tab.Detrazioni);
            elencoTab.Add(AreaQuadri.Tab.Eliminazione);
            elencoTab.Add(AreaQuadri.Tab.Oneri);
            elencoTab.Add(AreaQuadri.Tab.Supplementi);
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            elencoTab.Add(AreaQuadri.Tab.Familiare);
            elencoTab.Add(AreaQuadri.Tab.Redditi);
            elencoTab.Add(AreaQuadri.Tab.Titolare);
            elencoTab.Add(AreaQuadri.Tab.RichiestaBonus);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        private void ManageBtnPage()
        {
            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);
            bool isVisible = ucIstruttoria.ManageButtonRiduzioneRetributiva(this);
            //in caso di usuranti o salvaguardia o Ante96 non va mostrato pop up su 62 anni
            if (isVisible && this.areaLiquidazionePensioneAgo != null &&
                ((areaLiquidazionePensioneAgo.IsAnte96 != null) || (this.areaLiquidazionePensioneAgo.IsUsuranti.HasValue && this.areaLiquidazionePensioneAgo.IsUsuranti.Value) ||
                (this.areaLiquidazionePensioneAgo.TipologiaSalvaguardia.HasValue) ||
                (Utility.IsDomandaVOMIN(this.domanda.Categoria) && Utility.IsDomandaAnzianitaAnticipata(datiPensione)) ||
                // tipo calcolo contributivo (gestito lato Javascript)
                (datiPensione.DecorrenzaOriginaria.HasValue && Utility.DataSuccessivaA(datiPensione.DecorrenzaOriginaria.Value, new DateTime(2015, 1, 1)))
                ))
                isVisible = false;
            btnSalvaLiquidazionePensioneAgo.Visible = isVisible;
            btnSalvaLiquidazionePensioneAgoNoRiduzione.Visible = !isVisible;
            btnPopUpPage.Visible = isVisible;
        
        }

        private void CaricaDatiLiquidazione()
        {
            AreaTitolare.DatiPensione datiPensione = new AreaTitolare.DatiPensione();
            datiPensione = GetDatiPensione(this);
            bool IsDomandaSperDonna = CodeUtility.IsDomandaSperimentaleDonna(datiPensione);
            HiddenFieldSedi.Value = CodeUtility.LoadSedi();
            PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.GetLiquidazionePensioneAgo(this);

            if (HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ko;
                ucAvviso.Messaggio = ErrorMessage;
                pnlLiquidazionePensioneAgo.Enabled = false;
                return;
            }

            ucDatiGenerici.SetHiddenENPALS();
            ucDatiGenerici.SetHiddenFieldIsRicostituzione();
            ucDatiGenerici.ValorizzaEtichetteDatiGenerici(this);
            ucDatiAssicurativi.ValorizzaEtichetteDatiAssicurativi(this, datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione, datiPensione.IsDomandaVecchiaiaOrAnzianitaMaggiorazioneAmianto);
            ucOpzione.ValorizzaEtichetteOpzione(this);
            ucPrecedentePensione.ValorizzaEtichettePrecedentePensione(this);
            if (this.areaLiquidazionePensioneAgo.DatiProvenienza != null)
                ucDatiGenerici.SetHiddenPrecedentePensioneValue("true");
            ucIstruttoria.ValorizzaEtichetteIstruttoria(this);

            ucInail.ValorizzaEtichetteInail(this);
            ucStorico.ValorizzaEtichette(this);
            ucSentenzaArt4.ValorizzaEtichette(this.areaLiquidazionePensioneAgo);
            ucSentenze.ValorizzaEtichette(this.areaLiquidazionePensioneAgo);

            if (this.domanda.IsDomandaENPALS && this.areaLiquidazionePensioneAgo != null && this.areaLiquidazionePensioneAgo.DatiContribuzioneEnpals != null)
            {
                this.DatiContribuzioneEnpals = Utility.GetDatiContribuzioneEnpalsSvrLiquidazione(this.areaLiquidazionePensioneAgo.DatiContribuzioneEnpals);
                this.IsContribuzioneEnpalsRetributivaVisible = this.areaLiquidazionePensioneAgo.IsContribuzioneEnpalsRetributivaVisible;
                this.IsContribuzioneEnpalsContributivaVisible = this.areaLiquidazionePensioneAgo.IsContribuzioneEnpalsContributivaVisible;
                ucContribEnpals.ValorizzaEtichette(this);
                ucContribEnpals.SetHiddenField(Presenter.SvrLiquidazione.TipologiaContribuzioneEnpals.SAI);
            }

            if (Utility.IsDomandaIndennitaUnaTantum_AGO(datiPensione) || Utility.IsDomandaSupplementare(datiPensione) ||
                (Utility.IsDomandaEsodo(this.domanda.Categoria) && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                || Utility.IsDomandaRipristino(datiPensione) || Utility.IsDomandaINDCOM(this.domanda.Categoria) || Utility.IsDomandaUsuranti(datiPensione)
                || (Utility.IsDomandaSPED(this.domanda.Categoria) && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                || ((Utility.IsDomandaCumulo(this.domanda.Categoria) || Utility.IsDomandaTotalizzazione(this.domanda.Categoria)) && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                || Utility.IsDomandaPMO(this.domanda.Categoria))
                hdnSKIP_SetChkBenefici.Value = "TRUE";

            if (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura) && (Utility.IsDomandaTotalizzazione(this.domanda.Categoria) || Utility.IsDomandaCumulo(this.domanda.Categoria)))
            {
                HiddenFieldIsRicTfrTotCum_Liq.Value = "SI";
            }
        }

        #region events
        #region Avviso Dati Generici
        protected void event_ucShowAvvisoDatiGenerici(object sender, EventArgs e)
        {
            IViewUI tabDatiGenerici = (IViewUI)sender;
            //if (this.domanda == null)
            //    this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            elencoTab.Add(AreaQuadri.Tab.Bititolarita);
            elencoTab.Add(AreaQuadri.Tab.Detrazioni);
            elencoTab.Add(AreaQuadri.Tab.Eliminazione);
            elencoTab.Add(AreaQuadri.Tab.Oneri);
            elencoTab.Add(AreaQuadri.Tab.Supplementi);
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            elencoTab.Add(AreaQuadri.Tab.Redditi);
            elencoTab.Add(AreaQuadri.Tab.Familiare);
            elencoTab.Add(AreaQuadri.Tab.Titolare);
            elencoTab.Add(AreaQuadri.Tab.RichiestaBonus);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiGenerici.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiGenerici.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Generici salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaDatiGenerici(object sender, EventArgs e)
        {
            IViewUI tabDatiGenerici = (IViewUI)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            elencoTab.Add(AreaQuadri.Tab.Bititolarita);
            elencoTab.Add(AreaQuadri.Tab.Detrazioni);
            elencoTab.Add(AreaQuadri.Tab.Eliminazione);
            elencoTab.Add(AreaQuadri.Tab.Oneri);
            elencoTab.Add(AreaQuadri.Tab.Supplementi);
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            elencoTab.Add(AreaQuadri.Tab.Familiare);
            elencoTab.Add(AreaQuadri.Tab.Titolare);
            elencoTab.Add(AreaQuadri.Tab.RichiestaBonus);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (!tabDatiGenerici.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Generici eliminati correttamente";
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiGenerici.ErrorMessage;
            }
        }

        //ENG - Aggiornamento Memo86
        protected void event_ucShowAvvisoTrattenutaFondoCredito(object sender, EventArgs e)
        {
            ucAvviso.Tipo = TipoAvviso.Warning;
            ucAvviso.Visible = true;
            ucAvviso.Messaggio = "Attenzione, il campo 'Trattenuta fondo credito' e il campo 'Decorrenza trattenuta fondo credito' sono stati modificati. Verificare il conguaglio impostando opportunamente il campo 'Decorrenza arretrati'";
        }

        #endregion Avviso Dati Generici

        #region Avviso Dati Assicurativi
        protected void event_ucShowAvvisoDatiAssicurativi(object sender, EventArgs e)
        {
            IViewUI tabDatiAssicurativi = (IViewUI)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            elencoTab.Add(AreaQuadri.Tab.Oneri);
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiAssicurativi.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiAssicurativi.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Assicurativi salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaDatiAssicurativi(object sender, EventArgs e)
        {
            IViewUI tabDatiAssicurativi = (IViewUI)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            elencoTab.Add(AreaQuadri.Tab.Oneri);
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if ((tabDatiAssicurativi.HasError) || (!tabDatiAssicurativi.HasError && !String.IsNullOrEmpty(tabDatiAssicurativi.ErrorMessage)))
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiAssicurativi.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Assicurativi eliminati correttamente";
            }

        }
        #endregion Avviso Dati Assicurativi

        #region Avviso Dati Opzione
        protected void event_ucShowAvvisoDatiOpzione(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCOpzione tabDatiOpzione = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCOpzione)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiOpzione.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiOpzione.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Opzione salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaDatiOpzione(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCOpzione tabDatiOpzione = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCOpzione)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiOpzione.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiOpzione.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Opzione eliminati correttamente";
            }

        }
        #endregion Avviso Dati Opzione

        #region Avviso Dati Precedente Pensione
        protected void event_ucShowAvvisoDatiPrecedentePensione(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCPrecedentePensione tabDatiPrecedentePensione = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCPrecedentePensione)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiPrecedentePensione.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiPrecedentePensione.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Precedente Pensione salvati correttamente";
                ucDatiGenerici.SetHiddenPrecedentePensioneValue("true");
            }
        }

        protected void event_ucShowAvvisoEliminaDatiPrecedentePensione(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCPrecedentePensione tabDatiPrecedentePensione = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCPrecedentePensione)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiPrecedentePensione.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiPrecedentePensione.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Precedente Pensione eliminati correttamente";
                ucDatiGenerici.SetHiddenPrecedentePensioneValue("false");
            }
        }
        #endregion Avviso Dati Precedente Pensione

        #region Avviso Dati Istruttoria
        protected void event_ucShowAvvisoDatiIstruttoria(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.Istruttoria tabDatiIstruttoria = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.Istruttoria)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            elencoTab.Add(AreaQuadri.Tab.Eliminazione);
            elencoTab.Add(AreaQuadri.Tab.Oneri);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiIstruttoria.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiIstruttoria.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Istruttoria salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaDatiIstruttoria(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.Istruttoria tabDatiIstruttoria = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.Istruttoria)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            elencoTab.Add(AreaQuadri.Tab.Eliminazione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiIstruttoria.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiIstruttoria.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Istruttoria eliminati correttamente";
            }
        }
        #endregion Avviso Dati Istruttoria

        #region Avviso Dati INAIL
        protected void event_ucShowAvvisoInail(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCInail tabINAIL = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCInail)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabINAIL.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabINAIL.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Inail/Accompagnamento salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaInail(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCInail tabINAIL = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCInail)sender;

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabINAIL.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabINAIL.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Inail/Accompagnamento eliminati correttamente";
            }
        }
        #endregion Avviso Dati INAIL

        #region Avviso Contributi Enpals
        protected void event_ucSalvaContribuzioneEnpals(object sender, EventArgs args)
        {
            UCContribuzioneEnpals uc = (UCContribuzioneEnpals)sender;
            if (uc.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = uc.ErrorMessage;
                return;
            }
            ucAvviso.Tipo = TipoAvviso.Ok;
            ucAvviso.Visible = true;
            ucAvviso.Messaggio = "Contributi ENPALS salvati correttamente";


            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }
        #endregion Avviso Contributi Enpals

        #region Avviso Interessi Legali
        protected void event_ucShowAvvisoDatiInteressiLegali(object sender, EventArgs args)
        {
            IViewUI uc = (IViewUI)sender;
            if (uc.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = uc.ErrorMessage;
                return;
            }
            ucAvviso.Tipo = TipoAvviso.Ok;
            ucAvviso.Visible = true;
            ucAvviso.Messaggio = "Dati Interessi Legali salvati correttamente";

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoEliminaDatiLegali(object sender, EventArgs e)
        {
            IViewUI uc = (IViewUI)sender;

            if (uc.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = uc.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Interessi Legali eliminati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }
        #endregion Avviso Interessi Legali

        #region Sentenza Art. 4
        protected void event_ucShowAvvisoSentenzaArt4(object sender, EventArgs args)
        {
            IViewUI uc = (IViewUI)sender;
            if (uc.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = uc.ErrorMessage;
                return;
            }
            ucAvviso.Tipo = TipoAvviso.Ok;
            ucAvviso.Visible = true;
            ucAvviso.Messaggio = "Dati Sentenza Art. 4 salvati correttamente";

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoEliminaSentenzaArt4(object sender, EventArgs e)
        {
            IViewUI uc = (IViewUI)sender;

            if (uc.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = uc.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Sentenza Art. 4 eliminati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        #endregion Sentenza Art. 4

        #region Sentenze

        protected void event_ucShowAvvisoSentenze(object sender, EventArgs args)
        {
            IViewUI uc = (IViewUI)sender;
            if (uc.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = uc.ErrorMessage;
                return;
            }
            ucAvviso.Tipo = TipoAvviso.Ok;
            ucAvviso.Visible = true;
            ucAvviso.Messaggio = "Dati Sentenze salvati correttamente";

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoEliminaSentenze(object sender, EventArgs e)
        {
            IViewUI uc = (IViewUI)sender;

            if (uc.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = uc.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Sentenze eliminati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        #endregion Sentenze
        protected void event_ucHideAvviso(object sender, EventArgs e)
        {
            ucAvviso.Messaggio = string.Empty;
            ucAvviso.Visible = false;
        }

        #endregion events

    }
}
