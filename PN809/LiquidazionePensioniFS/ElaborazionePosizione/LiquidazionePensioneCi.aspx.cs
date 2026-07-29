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
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class LiquidazionePensioneCi : CustomBasePage, IInfoLiquidazione, IQuadriSemafori, ITitolarePensione, ILiquidazionePensioneCi
    {
        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region ILiquidazionePensioneCi
        public AreaLiquidazionePensione areaLiquidazionePensioneCi { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda areaRiepilogoDomanda { get; set; }
        #endregion ILiquidazionePensioneCi

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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                CaricaDatiLiquidazione();
                //if (ucAvviso.Visible == true)
                //    ucAvviso.Visible = false;
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
                ValorizzaSemaforiTab(imgOpzione, this.areaQuadri.QuadroLiquidazionePensione.TabOpzione, pnlTabOpzione);
                ValorizzaSemaforiTab(imgPrecedentePensione, this.areaQuadri.QuadroLiquidazionePensione.TabPrecedentePensione, pnlTabPrecedentePensione);
                ValorizzaSemaforiTab(imgIstruttoria, this.areaQuadri.QuadroLiquidazionePensione.TabIstruttoria, pnlTabIstruttoria);
                ValorizzaSemaforiTab(imgInail, this.areaQuadri.QuadroLiquidazionePensione.TabInail, pnlTabInail);
            }
        }

        protected void SalvaLiquidazionePensioneCi_Click(Object sender, EventArgs e)
        {
            areaRiepilogoDomanda = new AreaRispostaRiepilogo.DatiRiepilogoDomanda();
            areaRiepilogoDomanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.domanda = areaRiepilogoDomanda;
            areaLiquidazionePensioneCi = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneCi.AreaLiquidazionePensione();
            Presenter.PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();

            areaLiquidazionePensioneCi.DatiGenerici = ucDatiGenerici.GetDatiGenerici();
            areaLiquidazionePensioneCi.DatiAssicurativi = ucDatiAssicurativi.GetDatiAssicurativi();
            areaLiquidazionePensioneCi.DatiOpzione = ucOpzione.GetDatiOpzione();
            areaLiquidazionePensioneCi.DatiProvenienza = ucPrecedentePensione.GetDatiProvenienza();
            areaLiquidazionePensioneCi.DatiIstruttoria = ucIstruttoria.GetDatiIstruttoria();
            areaLiquidazionePensioneCi.DatiInail = ucInail.GetDatiInail();

            presenterLiquidazione.SalvaLiquidazionePensioneCi(this);
            if (this.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
            }
            else
            {
                //if (areaLiquidazionePensioneCi.DatiProvenienza.Equals(new DatiProvenienza()))
                if (!areaLiquidazionePensioneCi.DatiProvenienza.CertificatoPrecedentePensione.HasValue && !areaLiquidazionePensioneCi.DatiProvenienza.CodiceP18PrecedentePensione.HasValue &&
                    !areaLiquidazionePensioneCi.DatiProvenienza.DecorrenzaOriginariaAltraPensione.HasValue && !areaLiquidazionePensioneCi.DatiProvenienza.SedePrecedentePensione.HasValue)
                    ucDatiGenerici.SetHiddenPrecedentePensioneValue("false");
                else
                    ucDatiGenerici.SetHiddenPrecedentePensioneValue("true");

                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Liquidazione Pensione salvati correttamente";
            }

            ucInail.ValorizzaEtichette(this);

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            elencoTab.Add(AreaQuadri.Tab.Bititolarita);
            elencoTab.Add(AreaQuadri.Tab.Detrazioni);
            elencoTab.Add(AreaQuadri.Tab.Eliminazione);
            elencoTab.Add(AreaQuadri.Tab.RichiestaBonus);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }
        
        private void ManageBtnPage()
        {
            bool isVisible = ucIstruttoria.ManageButtonRiduzioneRetributiva(this);
            //ENG - Miglioramento Popup TRF
            if (this.areaRiepilogoDomanda == null)
                this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            //in caso di usuranti o salvaguardia non va mostrato pop up su 62 anni
            if (isVisible && this.areaLiquidazionePensioneCi != null &&
                ((this.areaLiquidazionePensioneCi.IsUsuranti.HasValue && this.areaLiquidazionePensioneCi.IsUsuranti.Value) ||
                (this.areaLiquidazionePensioneCi.TipologiaSalvaguardia.HasValue) ||
                (this.areaLiquidazionePensioneCi.IsRiduzioneRetributivaEnabled.HasValue && !this.areaLiquidazionePensioneCi.IsRiduzioneRetributivaEnabled.Value)
                || this.areaRiepilogoDomanda.IsDomandaRiapertura))
                isVisible = false;
            btnSalvaLiquidazionePensioneCi.Visible = isVisible;
            btnSalvaLiquidazionePensioneCiNoRiduzione.Visible = !isVisible;
            btnPopUpPage.Visible = isVisible;
        }

        private void CaricaDatiLiquidazione()
        {
            HiddenFieldSedi.Value = CodeUtility.LoadSedi();



            //if (Session["DatiPensione"] == null)
            //{
            //    PresenterTitolare presenterTitolare = new PresenterTitolare();
            //    AreaTitolare titolare = new AreaTitolare();
            //    titolare = presenterTitolare.CaricaTitolare(this);
            //    Session["DatiPensione"] = (AreaTitolare.DatiPensione)titolare.Pensione;
            //}
            if (this.areaRiepilogoDomanda == null)
                this.areaRiepilogoDomanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            PresenterLiquidazionePensione presenterLiquidazione = new PresenterLiquidazionePensione();
            presenterLiquidazione.GetLiquidazionePensioneCi(this);
            if (HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ko;
                ucAvviso.Messaggio = ErrorMessage;
                pnlLiquidazionePensioneCi.Enabled = false;
                return;
            }
            ucDatiGenerici.ValorizzaEtichetteDatiGenerici(this);
            ucOpzione.ValorizzaEtichetteOpzione(this);
            ucPrecedentePensione.ValorizzaEtichettePrecedentePensione(this);
            if (this.areaLiquidazionePensioneCi.DatiProvenienza != null)
                ucDatiGenerici.SetHiddenPrecedentePensioneValue("true");
            ucIstruttoria.ValorizzaEtichetteIstruttoria(this);
            ucDatiAssicurativi.ValorizzaEtichetteDatiAssicurativi(this, datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione, datiPensione.IsDomandaVecchiaiaOrAnzianitaMaggiorazioneAmianto);
            ucInail.ValorizzaEtichette(this);
        }

        #region Avviso Dati Generici
        protected void event_ucShowAvvisoDatiGenerici(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCDatiGenerici tabDatiGenerici = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCDatiGenerici)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            elencoTab.Add(AreaQuadri.Tab.Bititolarita);
            elencoTab.Add(AreaQuadri.Tab.Detrazioni);
            elencoTab.Add(AreaQuadri.Tab.Eliminazione);
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
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCDatiGenerici tabDatiGenerici = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCDatiGenerici)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            elencoTab.Add(AreaQuadri.Tab.MaggiorazioniEBenefici);
            elencoTab.Add(AreaQuadri.Tab.Bititolarita);
            elencoTab.Add(AreaQuadri.Tab.Detrazioni);
            elencoTab.Add(AreaQuadri.Tab.Eliminazione);
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
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCDatiAssicurativiCi tabDatiAssicurativi = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCDatiAssicurativiCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
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
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCDatiAssicurativiCi tabDatiAssicurativi = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCDatiAssicurativiCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
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
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCOpzioneCi tabDatiOpzione = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCOpzioneCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

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
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCOpzioneCi tabDatiOpzione = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCOpzioneCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

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
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCPrecedentePensioneCi tabDatiPrecedentePensione = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCPrecedentePensioneCi)sender;
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
                ucAvviso.Messaggio = "Dati Precedente Pensione salvati correttamente";
                ucDatiGenerici.SetHiddenPrecedentePensioneValue("true");
            }
        }

        protected void event_ucShowAvvisoEliminaDatiPrecedentePensione(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCPrecedentePensioneCi tabDatiPrecedentePensione = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCPrecedentePensioneCi)sender;
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
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCIstruttoriaCi tabDatiIstruttoria = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCIstruttoriaCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
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
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCIstruttoriaCi tabDatiIstruttoria = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCIstruttoriaCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
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

        #region Avviso Dati Inail
        protected void event_ucShowAvvisoDatiInail(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCInailCi tabDatiInail = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCInailCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiInail.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiInail.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Inail salvati correttamente";
            }
        }

        protected void event_ucShowAvvisoEliminaDatiInail(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCInailCi tabDatiInail = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCInailCi)sender;
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.LiquidazionePensione);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);

            if (tabDatiInail.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = tabDatiInail.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Inail eliminati correttamente";
            }
        }
        #endregion Avviso Dati Inail
    }
}
