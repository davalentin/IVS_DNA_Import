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
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.CrossSuppLiqAgo;
using INPS.DNA.Presenter.Interface;


namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class Supplementi : CustomBasePage, ISupplementi, IQuadriSemafori, ICrossContribuzioneEnpals
    {

        #region ISup
        public long numDomanda { get; set; }
        public AreaSupplementi lstSupplementi { get; set; }
        public Presenter.SvrLiquidazione.AreaSupplementi risposta { get; set; }
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }

        #endregion ISup

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region ICrossContribuzioneEnpals
        public long NumeroDomanda { get; set; }
        public DatiContribuzioneEnpals DatiContribuzioneEnpals { get; set; }
        public TipologiaContribuzioneEnpals Tipologia { get; set; }
        public bool IsContribuzioneEnpalsRetributivaVisible { get; set; }
        public bool IsContribuzioneEnpalsContributivaVisible { get; set; }
        #endregion ICrossContribuzioneEnpals

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!Page.IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);

                InitData();
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                //ucSupplementiENPALS
                if (this.domanda.IsDomandaENPALS)
                {
                    //ENG - Memo 32_a/2018
                    ValorizzaSemaforiTab(imgStorico, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabStorico);

                    if (ucSupplementiENPALS.IsPageDettaglioVisible())
                    {
                        AreaQuadri.Semaforo semaforo = ucSupplementiENPALS.IsDettaglioSalvato() ? AreaQuadri.Semaforo.Verde : AreaQuadri.Semaforo.Rosso_Abilitato;
                        ValorizzaSemaforiTab(imgSupplementi, semaforo, pnlTabSupplementi);
                        if (ucSupplementiENPALS.IsDettaglioDalSAISelezionato())
                            ValorizzaSemaforiTab(imgContribuzioneEnpals, this.areaQuadri.QuadroSupplementi.TabDatiContribuzioneEnpals, pnlTabContribuzioneEnpals);
                    }
                    else
                    {
                        ValorizzaSemaforiTab(imgSupplementi, this.areaQuadri.QuadroSupplementi.TabSupplementi, pnlTabSupplementi);
                        ValorizzaSemaforiTab(imgContribuzioneEnpals, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabContribuzioneEnpals);
                    }
                }
                else
                {
                    if (Utility.IsDomandaCumulo(this.domanda.Categoria) || Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
                    {
                        ValorizzaSemaforiTab(imgQuoteSupplementi, this.areaQuadri.QuadroSupplementi.TabSupplementi, pnlTabQuoteSupplementi);
                        ValorizzaSemaforiTab(imgSupplementi, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabSupplementi);
                        //ENG - Memo 32_a/2018
                        if (Utility.IsRicostituzioneMotiviContributiviVariazioneDatiSupplementiCumulo(datiPensione, this.domanda.Categoria))
                        {
                            ValorizzaSemaforiTab(imgStorico, this.areaQuadri.QuadroSupplementi.TabStoricoSupplementi, pnlTabStorico);
                        }
                        else
                        {
                            ValorizzaSemaforiTab(imgStorico, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabStorico);
                        }

                        hdnSelected.Value = "#quoteSupplementi";

                        if (Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
                        {
                            lblTitleQuoteSupplementi.Text = "Quote Supplementi Totalizzazione";
                        }
                        else if (Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione))
                        {
                            lblTitleQuoteSupplementi.Text = "Adeguamento Pro Quota";
                        }
                    }
                    else
                    {
                        ValorizzaSemaforiTab(imgQuoteSupplementi, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabQuoteSupplementi);
                        ValorizzaSemaforiTab(imgSupplementi, this.areaQuadri.QuadroSupplementi.TabSupplementi, pnlTabSupplementi);
                        //ENG - Memo 32_a/2018
                        ValorizzaSemaforiTab(imgStorico, AreaQuadri.Semaforo.Rosso_NonAbilitato, pnlTabStorico);
                    }
                    ValorizzaSemaforiTab(imgContribuzioneEnpals, this.areaQuadri.QuadroSupplementi.TabDatiContribuzioneEnpals, pnlTabContribuzioneEnpals);
                }
            }
        }

        protected void SalvaSupplementi_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
            try
            {
                PresenterSupplementi presenterSupplementi = new PresenterSupplementi();
                this.lstSupplementi = new AreaSupplementi();

                this.lstSupplementi = GetDatiSupplementi();

                if (this.domanda.TipoAppartenenza == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO &&
                   this.domanda.IsDomandaENPALS)
                {
                    this.lstSupplementi.DatiContribuzioneEnpalsSAS = ucContribEnpals.GetDatiContribuzioneEnpals();
                }


                presenterSupplementi.SalvaSupplementiByDomanda(this);

                if (this.HasError)
                {
                    ucAvviso.Tipo = TipoAvviso.Warning;
                    ucAvviso.Visible = true;
                    ucAvviso.Messaggio = this.ErrorMessage;
                }
                else
                {
                    ucAvviso.Tipo = TipoAvviso.Ok;
                    ucAvviso.Visible = true;
                    if (Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione))
                    {
                        ucAvviso.Messaggio = "Dati Adeguamento Pro Quota salvati correttamente";
                    }
                    else
                    {
                        ucAvviso.Messaggio = "Dati Supplementi salvati correttamente";
                    }
                }
            }
            catch (INPS.DNA.DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Supplementi, Errore nel metodo SalvaDati" + ex);
            }
            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Supplementi);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }



        protected void event_ucSalvaSupplementi(object sender, Utility.CustomEventArgs e)
        {
            bool hasError;
            string errorMsg;
            GetDataEvents(sender, e, out hasError, out errorMsg);
            AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];

            if (hasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = errorMsg;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                if (Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(datiPensione))
                {
                    ucAvviso.Messaggio = "Dati Adeguamento Pro Quota salvati correttamente";
                }
                else
                {
                    ucAvviso.Messaggio = "Dati Supplementi salvati correttamente";
                }
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Supplementi);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucEliminaSupplementi(object sender, Utility.CustomEventArgs e)
        {
            bool hasError;
            string errorMsg;
            GetDataEvents(sender, e, out hasError, out errorMsg);
            if (hasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = errorMsg;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Supplementi eliminati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Supplementi);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucErrorSalvaSupplementi(object sender, EventArgs e)
        {
            string errorMsg;
            if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
                errorMsg = "Inserire il Tipo, la Decorrenza, il numero di Settimane, RMS e la Quota";
            else
                errorMsg = "Inserire il Tipo, la Decorrenza, il numero di Settimane, RMS e la Quota per Ago e CI";

            ucAvviso.Tipo = TipoAvviso.Warning;
            ucAvviso.Visible = true;
            ucAvviso.Messaggio = errorMsg;
        }

        protected void event_ucAbilitaTastoSalva(object sender, EventArgs e)
        {
            if (!btnSalva.Enabled)
                btnSalva.Enabled = true;
            if (!btnPopUpPage.Enabled)
                btnPopUpPage.Enabled = true;
        }

        protected void event_ucDisabilitaTastoSalva(object sender, EventArgs e)
        {
            if (btnSalva.Enabled)
                btnSalva.Enabled = false;
            if (btnPopUpPage.Enabled)
                btnPopUpPage.Enabled = false;
        }

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
            ucAvviso.Messaggio = "Contributi ENPALS salvati correttamente.";


            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Supplementi);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowPopUp(object sender, EventArgs args)
        {
            btnPopUpPage.Style.Remove("display");
            btnSalva.Style.Remove("display");
            btnSalva.Style.Add("display", "none");
        }

        protected void event_ucHidePopUp(object sender, EventArgs args)
        {
            btnPopUpPage.Style.Remove("display");
            btnSalva.Style.Remove("display");
            btnPopUpPage.Style.Add("display", "none");
        }

        protected void event_ucHideTastoSalva(object sender, EventArgs args)
        {
            btnSalva.Style.Remove("display");
            btnSalva.Style.Add("display", "none");
        }

        protected void event_ucShowTastoSalva(object sender, EventArgs args)
        {
            btnSalva.Style.Remove("display");
        }

        protected void event_ucInitData(object sender, EventArgs args)
        {
            InitData();
        }

        protected void event_ucHideAvviso(object sender, EventArgs args)
        {
            ucAvviso.Messaggio = string.Empty;
            ucAvviso.Visible = false;
        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            if (((IViewUI)sender).HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ((IViewUI)sender).ErrorMessage;
            }
        }

        #region Gestione UserControls

        private void InitData()
        {
            PresenterSupplementi presenterSupplementi = new PresenterSupplementi();
            presenterSupplementi.RicercaSupplementiByNumDomanda(this);
            this.DatiContribuzioneEnpals = this.risposta.DatiContribuzioneEnpalsSAS;
            this.IsContribuzioneEnpalsRetributivaVisible = this.risposta.IsContribuzioneEnpalsRetributivaVisible;
            this.IsContribuzioneEnpalsContributivaVisible = this.risposta.IsContribuzioneEnpalsContributivaVisible;

            SwitchUserControls();
        }

        private void SwitchUserControls()
        {
            AreaTitolare.DatiPensione datiPensione = CodeUtility.GetDatiPensioneFromSession();

            if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
            {
                this.ucSupplementi.Visible = true;
                this.ucSupplementiAgoCI.Visible = false;
                this.ucSupplementiENPALS.Visible = false;
                this.ucSupplementiCumulo.Visible = false;
                this.ucSupplementiCumuloStorico.Visible = false;
                ucSupplementi.ValorizzaEtichette(this);
            }
            else
            {
                this.ucSupplementi.Visible = false;

                if (this.domanda.IsDomandaENPALS)
                {
                    this.ucSupplementiAgoCI.Visible = false;
                    this.ucSupplementiCumulo.Visible = false;
                    this.ucSupplementiENPALS.Visible = true;
                    pnlTabQuoteSupplementi.Visible = false;
                    ucSupplementiENPALS.ValorizzaEtichette(this);
                    ucContribEnpals.ValorizzaEtichette(this);
                    this.ucSupplementiCumuloStorico.Visible = false;
                    ucContribEnpals.SetHiddenField(TipologiaContribuzioneEnpals.SAS);
                }
                else if (Utility.IsDomandaCumulo(this.domanda.Categoria) || Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
                {
                    this.ucSupplementiENPALS.Visible = false;
                    this.ucSupplementiCumulo.Visible = true;
                    this.ucSupplementiAgoCI.Visible = false;
                    this.ucSupplementiCumuloStorico.Visible = false;
                    ucSupplementiCumulo.ValorizzaEtichette(this);

                    //ENG Memo 32_a/2018
                    if (Utility.IsRicostituzioneMotiviContributiviVariazioneDatiSupplementiCumulo(datiPensione, this.domanda.Categoria))
                    {
                        this.ucSupplementiCumuloStorico.Visible = true;
                        ucSupplementiCumuloStorico.ValorizzaEtichette(this);
                    }
                }
                else
                {
                    this.ucSupplementiENPALS.Visible = false;
                    this.ucSupplementiCumulo.Visible = false;
                    this.ucSupplementiAgoCI.Visible = true;
                    this.ucSupplementiCumuloStorico.Visible = false;
                    ucSupplementiAgoCI.ValorizzaEtichette(this);
                }
            }
        }

        private AreaSupplementi GetDatiSupplementi()
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda.TipoAppartenenza.HasValue && this.domanda.TipoAppartenenza.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS)
                return ucSupplementi.GetDatiUcSupplementi();
            else
            {
                if (this.domanda.IsDomandaENPALS)
                    return ucSupplementiENPALS.GetEtichetteDettaglio();
                else if (Utility.IsDomandaCumulo(this.domanda.Categoria) || Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
                    return ucSupplementiCumulo.GetDatiUcSupplementi();
                else
                    return ucSupplementiAgoCI.GetDatiUcSupplementi();
            }
        }

        private void GetDataEvents(object sender, Utility.CustomEventArgs e, out bool hasError, out string errorMsg)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            hasError = false;
            errorMsg = string.Empty;

            AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp? tipoApp = (AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp?)e.TipoApp;
            switch (tipoApp.Value)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS:
                    UserControls.Supplementi.UCSupplementi tabSupplementi = (UserControls.Supplementi.UCSupplementi)sender;
                    hasError = tabSupplementi.HasError;
                    errorMsg = tabSupplementi.ErrorMessage;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO:
                    if (this.domanda.IsDomandaENPALS)
                    {
                        UserControls.Supplementi.UCSupplementiENPALS tabSupplementiENPALS = (UserControls.Supplementi.UCSupplementiENPALS)sender;
                        hasError = tabSupplementiENPALS.HasError;
                        errorMsg = tabSupplementiENPALS.ErrorMessage;
                    }
                    else if (Utility.IsDomandaCumulo(this.domanda.Categoria) || Utility.IsDomandaTotalizzazione(this.domanda.Categoria))
                    {
                        UserControls.Supplementi.UCSupplementiCumulo tabSupplementiCumulo = (UserControls.Supplementi.UCSupplementiCumulo)sender;
                        hasError = tabSupplementiCumulo.HasError;
                        errorMsg = tabSupplementiCumulo.ErrorMessage;
                    }
                    else
                    {
                        UserControls.Supplementi.UCSupplementiAgoCI tabSupplementiAgoCI = (UserControls.Supplementi.UCSupplementiAgoCI)sender;
                        hasError = tabSupplementiAgoCI.HasError;
                        errorMsg = tabSupplementiAgoCI.ErrorMessage;
                    }
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI:
                    {
                        UserControls.Supplementi.UCSupplementiAgoCI tabSupplementiAgoCI = (UserControls.Supplementi.UCSupplementiAgoCI)sender;
                        hasError = tabSupplementiAgoCI.HasError;
                        errorMsg = tabSupplementiAgoCI.ErrorMessage;
                    }
                    break;
            }
        }

        #endregion Gestione UserControls
    }
}
