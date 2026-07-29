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
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.CrossDatiFondoContr;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class DatiContributivi : CustomBasePage, IInfoLiquidazione, IDatiContributivi, IQuadriSemafori, ITitolarePensione
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IDatiContributivi
        public AreaDatiContributivi areaDatiContributivi { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion IDatiContributivi

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        #endregion ITitolare

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (this.domanda == null) 
            {
                this.HasError = true;
                this.ErrorMessage = "Sessione scaduta o Domanda non presente in Session.";
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Messaggio = this.ErrorMessage;
                return;
            }

            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);

                this.GetDatiTitolare(this);

                GetDatiContributivi();
                if (HasError) return;
                SwitchUserControls();

                if (this.domanda != null &&  this.domanda.Tipo != null && (this.domanda.Tipo == "RIC" || ControllaTipoCalcolo()))
                    SetDatiCalcolo();

                ValorizzaEtichette();

                RenderSemafori();
                if (Session["Pratiche"] != null)
                {
                    btnTornaPosizioni.Visible = true;
                    btnTornaARicerca.Visible = false;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                if ((AreaQuadri)Session["Semaforo"] != null)
                {
                    if (this.domanda == null)
                        this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                    if (this.TitolarePensione == null)
                        this.TitolarePensione = new AreaTitolare();
                    if (this.TitolarePensione.Pensione == null)
                        this.TitolarePensione.Pensione = GetDatiPensione(this);
                    if (this.areaDatiContributivi == null)
                        this.areaDatiContributivi = (AreaDatiContributivi)ViewState[EnumViewState.AreaDatiContributivi.ToString()];

                    this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                    ValorizzaSemaforiTab(imgDatiCalcolo, this.areaQuadri.QuadroDatiContributivi.TabDatiCalcolo, pnlTabDatiCalcolo);
                    ValorizzaSemaforiTab(imgDatiFondo, this.areaQuadri.QuadroDatiContributivi.TabDatiFondo, pnlTabDatiFondo);
                    ValorizzaSemaforiTab(imgDatiAgo, this.areaQuadri.QuadroDatiContributivi.TabDatiAgo, pnlTabDatiAgo);
                    ValorizzaSemaforiTab(imgArt11_14, this.areaQuadri.QuadroDatiContributivi.TabArt11e14, pnlTabArt11e14);
                    ValorizzaSemaforiTab(imgAnte67, this.areaQuadri.QuadroDatiContributivi.TabAnte67, pnlTabAnte67);
                    ValorizzaSemaforiTab(imgSL336, this.areaQuadri.QuadroDatiContributivi.TabSL33670, pnlTabSL336);
                    if (this.areaDatiContributivi != null && this.areaDatiContributivi.IsSettimane707Visible.GetValueOrDefault() && this.domanda.Tipofondo.HasValue &&
                        new List<AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo> { AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS, AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT }.Contains(this.domanda.Tipofondo.Value) &&
                        Utility.IsDomandaUnicarpe(this.TitolarePensione.Pensione, true) == Utility.TipoUnicarpe.Automatica &&
                        this.areaQuadri.QuadroDatiContributivi.TabDatiCalcolo707 == AreaQuadri.Semaforo.Rosso_NonAbilitato)
                        ValorizzaSemaforiTab(imgDatiCalcolo707, AreaQuadri.Semaforo.Rosso_Abilitato, pnlTabDatiCalcolo707);
                    else
                        ValorizzaSemaforiTab(imgDatiCalcolo707, this.areaQuadri.QuadroDatiContributivi.TabDatiCalcolo707, pnlTabDatiCalcolo707);
                    ValorizzaSemaforiTab(imgStorico, this.areaQuadri.QuadroDatiContributivi.TabStorico, pnlTabStorico);

                }
            }
            catch (DnaExceptionBase exx)
            {
                throw new INPS.DNA.DnaApplicationException("DatiContributivi, Errore nel metodo Page_PreRender " + exx); ;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("DatiContributivi, Errore nel metodo Page_PreRender " + ex);
            }
        }

        protected void SalvaDati_Click(object sender, EventArgs e)
        {
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();

            if (this.areaDatiContributivi == null)
                this.areaDatiContributivi = new AreaDatiContributivi();

            if (this.areaDatiContributivi.DatiCalcolo == null)
                this.areaDatiContributivi.DatiCalcolo = new GestioneContribDatiCalcolo();

            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    if (this.areaDatiContributivi.DatiCalcolo.fondoTT == null)
                        this.areaDatiContributivi.DatiCalcolo.fondoTT = new GestioneContribFondoTT();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    if (this.areaDatiContributivi.DatiCalcolo.fondoVL == null)
                        this.areaDatiContributivi.DatiCalcolo.fondoVL = new GestioneContribFondoVL();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    if (this.areaDatiContributivi.DatiCalcolo.fondoFST == null)
                        this.areaDatiContributivi.DatiCalcolo.fondoFST = new GestioneContribFondoFST();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    if (this.areaDatiContributivi.DatiCalcolo.fondoPT == null)
                        this.areaDatiContributivi.DatiCalcolo.fondoPT = new GestioneContribFondoPT();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    if (this.areaDatiContributivi.DatiCalcolo.fondoGAS == null)
                        this.areaDatiContributivi.DatiCalcolo.fondoGAS = new GestioneContribFondoGAS();
                    if (this.areaDatiContributivi.DatiFondo == null)
                        this.areaDatiContributivi.DatiFondo = new GestioneContribEntityDatiFondo();
                    if (this.areaDatiContributivi.DatiArt11e14 == null)
                        this.areaDatiContributivi.DatiArt11e14 = new GestioneContribDatiArt11e14();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    if (this.areaDatiContributivi.DatiCalcolo.fondoDZ == null)
                        this.areaDatiContributivi.DatiCalcolo.fondoDZ = new GestioneContribFondoDZ();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    if (this.areaDatiContributivi.DatiFondo == null)
                        this.areaDatiContributivi.DatiFondo = new GestioneContribEntityDatiFondo();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                    if (this.areaDatiContributivi.DatiCalcolo == null)
                        this.areaDatiContributivi.DatiCalcolo = new GestioneContribDatiCalcolo();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                    if (this.areaDatiContributivi.DatiCalcolo.fondoPI == null)
                        this.areaDatiContributivi.DatiCalcolo.fondoPI = new GestioneContribFondoPI();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    if (this.areaDatiContributivi.DatiAgoAltraPensione == null)
                        this.areaDatiContributivi.DatiAgoAltraPensione = new GestioneContribDatiAgoAltraPensione();
                    break;

            }

            RecuperaCampiDatiCalcolo();

            presenterDatiContributivi.SalvaDatiContributivi(this);
            if (HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ErrorMessage;
                ucAvviso.Tipo = TipoAvviso.Warning;
            }
            else
            {
                //commentata per evitare di gestire non correttamente i dati associati alle crossproperties
                //SetDatiCalcolo();
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Messaggio = "Dati Calcolo salvati correttamente.";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucCaricaDatiCalcolo(object sender, EventArgs e)
        {
            this.areaDatiContributivi = (AreaDatiContributivi)sender;
            this.GetDatiTitolare(this);
            SetDatiCalcolo();
        }

        protected void event_ucShowAvvisoDatiCalcolo(object sender, Utility.CustomEventArgs e)
        {
            bool hasError;
            string errorMsg;
            GetDataEventsTabDatiCalcolo(sender, e, out hasError, out errorMsg);

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
                ucAvviso.Messaggio = "Dati Calcolo salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoEliminaDatiCalcolo(object sender, Utility.CustomEventArgs e)
        {
            bool hasError;
            string errorMsg;
            GetDataEventsTabDatiCalcolo(sender, e, out hasError, out errorMsg);

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
                ucAvviso.Messaggio = "Dati Calcolo eliminati correttamente</br> " + errorMsg;
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoDatiCalcolo707(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.CrossDatiFondoContr.UCDatiCalcolo707 ucTabDatiCalcolo707 = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.CrossDatiFondoContr.UCDatiCalcolo707)sender;

            if (ucTabDatiCalcolo707.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabDatiCalcolo707.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Calcolo 707 salvati correttamente.";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoEliminaDatiCalcolo707(object sender, EventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.CrossDatiFondoContr.UCDatiCalcolo707 ucTabDatiCalcolo707 = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.CrossDatiFondoContr.UCDatiCalcolo707)sender;

            if (ucTabDatiCalcolo707.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabDatiCalcolo707.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Calcolo 707 eliminati correttamente.";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoDatiFondoGAS(object sender, Utility.CustomEventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiFondoGAS_ES ucTabDatiFondoGAS = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiFondoGAS_ES)sender;

            if (ucTabDatiFondoGAS.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabDatiFondoGAS.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Fondo salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoEliminaDatiFondoGAS(object sender, Utility.CustomEventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiFondoGAS_ES ucTabDatiFondoGAS = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiFondoGAS_ES)sender;

            if (ucTabDatiFondoGAS.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabDatiFondoGAS.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Fondo eliminati correttamente</br> " + ucTabDatiFondoGAS.ErrorMessage;
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoDatiAgoGAS(object sender, Utility.CustomEventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAgoGAS_ES ucTabDatiAgoGAS = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAgoGAS_ES)sender;

            if (ucTabDatiAgoGAS.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabDatiAgoGAS.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Ago salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }
        protected void event_ucShowAvvisoEliminaDatiAgoPI(object sender, Utility.CustomEventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAGO_PI ucTabDatiAgoPI = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAGO_PI)sender;

            if (ucTabDatiAgoPI.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabDatiAgoPI.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Ago eliminati correttamente</br> " + ucTabDatiAgoPI.ErrorMessage;
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoDatiFondoPI(object sender, Utility.CustomEventArgs e)
        {

            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiFondo_PI ucTabDatiFondoPI = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiFondo_PI)sender;

            if (ucTabDatiFondoPI.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabDatiFondoPI.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Fondo salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoEliminaDatiFondoPI(object sender, Utility.CustomEventArgs e)
        {
            var uc = (UserControls.DatiContributivi.UCDatiFondo_PI)sender;

            ucAvviso.Visible = true;
            ucAvviso.Tipo = uc.HasError ? TipoAvviso.Warning : TipoAvviso.Ok;
            ucAvviso.Messaggio = uc.HasError
                ? uc.ErrorMessage
                : "Dati Fondo PI eliminati correttamente";

            this.areaInfoPratica = new AreaInfoPratica();

            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiFondo);

            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }
        protected void event_ucShowAvvisoDatiAgoPI(object sender, Utility.CustomEventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAGO_PI ucTabDatiAgoPI = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAGO_PI)sender;

            if (ucTabDatiAgoPI.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabDatiAgoPI.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Ago salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }


        protected void event_ucShowAvvisoEliminaDatiAgoGAS(object sender, Utility.CustomEventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAgoGAS_ES ucTabDatiAgoGAS = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAgoGAS_ES)sender;

            if (ucTabDatiAgoGAS.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabDatiAgoGAS.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Ago eliminati correttamente</br> " + ucTabDatiAgoGAS.ErrorMessage;
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoDatiArt11e14GAS(object sender, Utility.CustomEventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCArt11e14GAS_ES ucTabDatiArt11e14GAS = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCArt11e14GAS_ES)sender;

            if (ucTabDatiArt11e14GAS.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabDatiArt11e14GAS.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Art 11 e 14 salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoEliminaDatiArt11e14GAS(object sender, Utility.CustomEventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCArt11e14GAS_ES ucTabDatiArt11e14GAS = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCArt11e14GAS_ES)sender;

            if (ucTabDatiArt11e14GAS.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabDatiArt11e14GAS.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Art. 11 e 14 eliminati correttamente</br> " + ucTabDatiArt11e14GAS.ErrorMessage;
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoAnte67(object sender, Utility.CustomEventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCAnte67ES ucTabAnte67 = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCAnte67ES)sender;

            if (ucTabAnte67.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabAnte67.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Ante 67 salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoEliminaAnte67(object sender, Utility.CustomEventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCAnte67ES ucTabAnte67 = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCAnte67ES)sender;

            if (ucTabAnte67.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabAnte67.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Ante 67 eliminati correttamente</br> " + ucTabAnte67.ErrorMessage;
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoSL336(object sender, Utility.CustomEventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCSL336_ES ucTabSL336 = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCSL336_ES)sender;

            if (ucTabSL336.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabSL336.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati S.L. 336 salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoEliminaSL336(object sender, Utility.CustomEventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCSL336_ES ucTabSL336 = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCSL336_ES)sender;

            if (ucTabSL336.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabSL336.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati S.L. 336 eliminati correttamente</br> " + ucTabSL336.ErrorMessage;
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowPopUp(object sender, EventArgs e)
        {
            if (domanda != null && (domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI || domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL))
                return;

            btnPopUpContributivi.Style.Remove("display");
            btnPopUpPage.Style.Remove("display");
            btnSalva.Style.Remove("display");
            btnSalvaNoRiduzione.Style.Remove("display");

            btnPopUpPage.Style.Add("display", "none");
            btnSalva.Style.Add("display", "none");
            btnSalvaNoRiduzione.Style.Add("display", "none");
        }

        protected void event_ucHidePopUp(object sender, EventArgs e)
        {
            if (domanda!= null && (domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI || domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL))
                return;

            btnPopUpContributivi.Style.Remove("display");
            btnPopUpPage.Style.Remove("display");
            btnSalva.Style.Remove("display");
            btnSalvaNoRiduzione.Style.Remove("display");

            btnPopUpContributivi.Style.Add("display", "none");
            btnSalva.Style.Add("display", "none");
        }

        protected void event_ucHideAvviso(object sender, EventArgs e)
        {
            ucAvviso.Visible = false; 
        }

        protected void event_ucShowAvvisoDatiAgoAltraPensione(object sender, Utility.CustomEventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAgoAltraPensione_ET ucTabDatiAgoAltraPensione = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAgoAltraPensione_ET)sender;
            if (ucTabDatiAgoAltraPensione.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabDatiAgoAltraPensione.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Ago / Altra Pensione salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvvisoEliminaDatiAgoAltraPensione(object sender, Utility.CustomEventArgs e)
        {
            INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAgoAltraPensione_ET ucTabDatiAgoAltraPensione = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAgoAltraPensione_ET)sender;

            if (ucTabDatiAgoAltraPensione.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = ucTabDatiAgoAltraPensione.ErrorMessage;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Ago / Altra Pensione eliminati correttamente</br> " + ucTabDatiAgoAltraPensione.ErrorMessage;
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.DatiCalcolo);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        #region private method

        private void GetDatiContributivi()
        {
            PresenterDatiContributivi presenterDatiContributivi = new PresenterDatiContributivi();
            presenterDatiContributivi.GetDatiContributivi(this);

            if (!HasError)
            {
                ViewState[EnumViewState.AreaDatiContributivi.ToString()] = this.areaDatiContributivi;
                Session["AreaDatiContributivi"] = this.areaDatiContributivi;

                ucAvviso.Visible = false;
                ucAvviso.Messaggio = "";
            }
            else
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Messaggio = ErrorMessage;
            }
        }

        private void SwitchUserControls()
        {
            this.ucDatiCalcoloEL_TT_ET.Visible = false;
            this.ucDatiCalcoloVL_FS_PT.Visible = false;
            this.ucDatiAgoGAS_ES.Visible = false;
            this.ucDatiFondoGAS_ES.Visible = false;
            this.ucArt11e14GAS_ES.Visible = false;
            this.ucDatiCalcoloDZ.Visible = false;
            this.ucSL336.Visible = false;
            this.ucDatiCalcoloPM.Visible = false;
          //  this.ucDatiCalcoloPI.Visible = false;
            this.ucDatiAgoAltraPensione.Visible = false;
            this.ucDatiCalcolo707.Visible = false;
            this.ucStorico.Visible = false;
            this.ucDatiAgo_PI.Visible = false;
            this.ucDatiFondo_PI.Visible = false;

            if (this.domanda != null && this.domanda.Tipofondo.HasValue)
            {
                switch (this.domanda.Tipofondo)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                        this.ucDatiCalcoloEL_TT_ET.Visible = true;
                        this.ucDatiCalcoloEL_TT_ET.areaDatiContributivi = this.areaDatiContributivi;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        this.ucDatiCalcoloEL_TT_ET.Visible = true;
                        this.ucDatiCalcoloEL_TT_ET.areaDatiContributivi = this.areaDatiContributivi;
                        this.ucDatiAgoAltraPensione.Visible = true;
                        this.ucDatiAgoAltraPensione.areaDatiContributivi = this.areaDatiContributivi;
                        spanDatiAgo.InnerText = "Altra Pensione - Dati AGO";
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                        this.ucDatiCalcoloVL_FS_PT.Visible = true;
                        this.ucDatiCalcoloVL_FS_PT.areaDatiContributivi = this.areaDatiContributivi;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        this.ucDatiCalcoloVL_FS_PT.Visible = true;
                        this.ucDatiCalcoloVL_FS_PT.areaDatiContributivi = this.areaDatiContributivi;
                        this.ucDatiCalcolo707.Visible = true;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                        this.ucDatiFondoGAS_ES.Visible = true;
                        this.ucDatiAgoGAS_ES.Visible = true;
                        this.ucArt11e14GAS_ES.Visible = true;
                        this.ucDatiFondoGAS_ES.areaDatiContributivi = this.areaDatiContributivi;
                        this.ucDatiAgoGAS_ES.areaDatiContributivi = this.areaDatiContributivi;
                        this.ucArt11e14GAS_ES.areaDatiContributivi = this.areaDatiContributivi;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                        this.ucDatiCalcoloDZ.Visible = true;
                        this.ucDatiCalcoloDZ.areaDatiContributivi = this.areaDatiContributivi;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                        this.ucDatiFondoGAS_ES.Visible = true;
                        this.ucDatiFondoGAS_ES.areaDatiContributivi = this.areaDatiContributivi;
                        this.ucDatiAgoGAS_ES.Visible = true;
                        this.ucDatiAgoGAS_ES.areaDatiContributivi = this.areaDatiContributivi;
                        this.ucArt11e14GAS_ES.Visible = true;
                        this.ucArt11e14GAS_ES.areaDatiContributivi = this.areaDatiContributivi;
                        this.ucAnte67Es.Visible = true;
                        this.ucAnte67Es.areaDatiContributivi = this.areaDatiContributivi;
                        this.ucSL336.Visible = true;
                        this.ucSL336.areaDatiContributivi = this.areaDatiContributivi;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        this.ucDatiCalcoloPM.Visible = true;
                        this.ucDatiCalcoloPM.areaDatiContributivi = this.areaDatiContributivi;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                        //this.ucDatiCalcoloPI.Visible = true;
                        //this.ucDatiCalcoloPI.areaDatiContributivi = this.areaDatiContributivi;

                        this.ucDatiAgo_PI.Visible = true;
                        this.ucDatiAgo_PI.areaDatiContributivi = this.areaDatiContributivi;
                        this.ucDatiFondo_PI.Visible = true;
                        this.ucDatiFondo_PI.areaDatiContributivi = this.areaDatiContributivi;

                        btnSalva.Visible = false;
                        btnSalvaNoRiduzione.Visible = false;
                        btnPopUpPage.Visible = false;
                        btnPopUpContributivi.Visible = false;
                        break;

                }
            }

            if (this.areaDatiContributivi != null && this.areaDatiContributivi.DatiCalcoloStorico != null)
            {
                if (this.domanda.Tipofondo.HasValue)
                {
                    switch (this.domanda.Tipofondo)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                            this.ucStorico.Visible = true;
                            this.ucStorico.areaDatiContributivi = this.areaDatiContributivi;
                            break;
                    }
                }
            }
        }

        private void SetDatiCalcolo()
        {
            if (this.areaDatiContributivi.DatiCalcolo != null || this.areaDatiContributivi.DatiCalcolo707 != null)
            {
                if (this.domanda.Tipofondo.HasValue)
                {
                    switch (this.domanda.Tipofondo)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                            ucDatiCalcoloEL_TT_ET.ValorizzaEtichetteDatiCalcolo();
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                            ucDatiCalcoloVL_FS_PT.ValorizzaEtichetteDatiCalcolo();
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                            ucDatiCalcoloVL_FS_PT.ValorizzaEtichetteDatiCalcolo();
                            ucDatiCalcolo707.ValorizzaEtichette(this.areaDatiContributivi.DatiCalcolo707, null, UCDatiCalcolo707.PaginaChiamante.DatiContributivi);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                            ucDatiAgoGAS_ES.ValorizzaEtichette(this.TitolarePensione.Pensione);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                            ucDatiCalcoloDZ.ValorizzaEtichetteDatiCalcolo(this.TitolarePensione.Pensione);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                            ucDatiCalcoloPM.ValorizzaEtichette(this.TitolarePensione.Pensione);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                          //  ucDatiCalcoloPI.ValorizzaEtichette();
                            ucDatiAgo_PI.ValorizzaEtichette();
                            break;

                    }
                }
            }

            if (this.areaDatiContributivi.DatiCalcoloStorico != null)
            {
                if (this.domanda.Tipofondo.HasValue)
                {
                    switch (this.domanda.Tipofondo)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                            ucStorico.ValorizzaEtichette();
                            break;
                    }
                }
            }
        }

        private void ValorizzaEtichette()
        {
            if (this.areaDatiContributivi != null)
            {
                if (this.domanda.Tipofondo.HasValue)
                {
                    switch (this.domanda.Tipofondo)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                            ucArt11e14GAS_ES.ValorizzaEtichette();
                            ucDatiFondoGAS_ES.ValorizzaEtichette();
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                            ucDatiFondoGAS_ES.ValorizzaEtichette();
                            ucArt11e14GAS_ES.ValorizzaEtichette();
                            ucAnte67Es.ValorizzaEtichette();
                            ucSL336.ValorizzaEtichette();
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                            ucDatiAgoAltraPensione.ValorizzaEtichette(this.areaDatiContributivi);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                            ucDatiFondo_PI.ValorizzaEtichette();
                            break;

                    }
                }
            }
        }

        private void RecuperaCampiDatiCalcolo()
        {
            if (this.areaDatiContributivi.DatiCalcolo != null)
            {
                if (this.domanda.Tipofondo.HasValue)
                {
                    switch (this.domanda.Tipofondo)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                            ucDatiCalcoloEL_TT_ET.RecuperaCampi(this.domanda.Tipofondo);
                            this.areaDatiContributivi.DatiCalcolo = ucDatiCalcoloEL_TT_ET.areaDatiContributivi.DatiCalcolo;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                            ucDatiCalcoloEL_TT_ET.RecuperaCampi(this.domanda.Tipofondo);
                            this.areaDatiContributivi.DatiCalcolo = ucDatiCalcoloEL_TT_ET.areaDatiContributivi.DatiCalcolo;
                            this.areaDatiContributivi.DatiAgoAltraPensione = ucDatiAgoAltraPensione.RecuperaCampi();
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                            ucDatiCalcoloVL_FS_PT.RecuperaCampi(this.domanda.Tipofondo);
                            this.areaDatiContributivi.DatiCalcolo = ucDatiCalcoloVL_FS_PT.areaDatiContributivi.DatiCalcolo;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                            ucDatiCalcoloVL_FS_PT.RecuperaCampi(this.domanda.Tipofondo);
                            this.areaDatiContributivi.DatiCalcolo = ucDatiCalcoloVL_FS_PT.areaDatiContributivi.DatiCalcolo;
                            this.areaDatiContributivi.DatiCalcolo707 = ucDatiCalcolo707.RecuperaCampi();
                            break;

                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                            ucDatiFondoGAS_ES.RecuperaCampi();
                            this.areaDatiContributivi.DatiFondo = ucDatiFondoGAS_ES.areaDatiContributivi.DatiFondo;
                            ucDatiAgoGAS_ES.RecuperaCampi();
                            this.areaDatiContributivi.DatiCalcolo = ucDatiAgoGAS_ES.areaDatiContributivi.DatiCalcolo;
                            ucArt11e14GAS_ES.RecuperaCampi();
                            this.areaDatiContributivi.DatiArt11e14 = ucArt11e14GAS_ES.areaDatiContributivi.DatiArt11e14;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                            ucDatiCalcoloDZ.RecuperaCampi(this.domanda.Tipofondo);
                            this.areaDatiContributivi.DatiCalcolo = ucDatiCalcoloDZ.areaDatiContributivi.DatiCalcolo;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                            ucDatiFondoGAS_ES.RecuperaCampi();
                            this.areaDatiContributivi.DatiFondo = ucDatiFondoGAS_ES.areaDatiContributivi.DatiFondo;
                            ucDatiAgoGAS_ES.RecuperaCampi();
                            this.areaDatiContributivi.DatiCalcolo = ucDatiAgoGAS_ES.areaDatiContributivi.DatiCalcolo;
                            ucArt11e14GAS_ES.RecuperaCampi();
                            this.areaDatiContributivi.DatiArt11e14 = ucArt11e14GAS_ES.areaDatiContributivi.DatiArt11e14;
                            ucAnte67Es.RecuperaCampi();
                            this.areaDatiContributivi.DatiAnte67 = ucAnte67Es.areaDatiContributivi.DatiAnte67;
                            ucSL336.RecuperaCampi();
                            this.areaDatiContributivi.DatiSL336 = ucSL336.areaDatiContributivi.DatiSL336;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                            ucDatiCalcoloPM.RecuperaCampi();
                            this.areaDatiContributivi.DatiCalcolo = ucDatiCalcoloPM.areaDatiContributivi.DatiCalcolo;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                         //   ucDatiCalcoloPI.RecuperaCampi();
                         //   this.areaDatiContributivi.DatiCalcolo = ucDatiCalcoloPI.areaDatiContributivi.DatiCalcolo;
                            //TODO
                            //   ucDatiFondo_PI.BuildAreaFromUI();
                            //ucDatiAgo_PI.RecuperaCampi();
                         
                            this.areaDatiContributivi.DatiCalcolo = ucDatiAgo_PI.areaDatiContributivi.DatiCalcolo;
                            break;
                    }
                }
            }
        }

        private bool ControllaTipoCalcolo()
        {
            bool bReturn = false;
            switch (this.areaDatiContributivi.DatiCalcolo.TipoCalcolo)
            {
                case GestioneContribTipoCalcolo.Contributivo:
                    switch (this.domanda.Tipofondo)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                            if (!IsDatiRetributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi))
                            {
                                bReturn = false;
                                ucDatiCalcoloEL_TT_ET.EnableDisableBtnSalva(bReturn);
                            }
                            else
                                bReturn = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                            if (!IsDatiRetributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi))
                            {
                                bReturn = false;
                                ucDatiCalcoloVL_FS_PT.EnableDisableBtnSalva(bReturn);
                            }
                            else
                                bReturn = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                            if (!IsDatiRetributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi))
                            {
                                bReturn = false;
                                ucDatiAgoGAS_ES.EnableDisableBtnSalva(bReturn);
                            }
                            else
                                bReturn = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                            if (!IsDatiRetributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi))
                            {
                                bReturn = false;
                                ucDatiCalcoloPM.EnableDisableBtnSalva(bReturn);
                            }
                            else
                                bReturn = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                            //if (this.areaDatiContributivi.CategoriaFondoPI.HasValue && (this.areaDatiContributivi.CategoriaFondoPI.Value == UtilityCategoriaFondoPI.U ||
                            //                                                            this.areaDatiContributivi.CategoriaFondoPI.Value == UtilityCategoriaFondoPI.V))
                            // Il tipo calcolo è ininfluente rispetto ai dati
                            bReturn = true;
                            break;
                    }
                    if (!bReturn)
                    {
                        ucAvviso.Visible = !bReturn;
                        ucAvviso.Tipo = TipoAvviso.Warning;
                        ucAvviso.Messaggio = "'Tipo Calcolo' incongruente; verificare la sezione 'Liquidazione Pensione'.";
                        ManageBtnPage(bReturn);
                    }
                    break;

                case GestioneContribTipoCalcolo.Retributivo:
                    switch (this.domanda.Tipofondo)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:

                            ManageRiduzioneRetributiva();

                            if (!IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi))
                            {
                                bReturn = false;
                                ucDatiCalcoloEL_TT_ET.EnableDisableBtnSalva(bReturn);
                            }
                            else
                                bReturn = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:

                            ManageRiduzioneRetributiva();

                            if (!IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi))
                            {
                                bReturn = false;
                                ucDatiCalcoloVL_FS_PT.EnableDisableBtnSalva(bReturn);
                            }
                            else
                                bReturn = true;
                            break;

                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                            if (!IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi))
                            {
                                bReturn = false;
                                ucDatiCalcoloVL_FS_PT.EnableDisableBtnSalva(bReturn);
                            }
                            else
                                bReturn = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:

                            ManageRiduzioneRetributiva();

                            if (!IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi))
                            {
                                bReturn = false;
                                ucDatiAgoGAS_ES.EnableDisableBtnSalva(bReturn);
                            }
                            else
                                bReturn = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:

                            ManageRiduzioneRetributiva();

                            if (!IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi))
                            {
                                bReturn = false;
                                ucDatiCalcoloDZ.EnableDisableBtnSalva(bReturn);
                            }
                            else
                                bReturn = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                            if (!IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi))
                            {
                                bReturn = false;
                                ucDatiCalcoloPM.EnableDisableBtnSalva(bReturn);
                            }
                            else
                                bReturn = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                            //if (this.areaDatiContributivi.CategoriaFondoPI.HasValue && (this.areaDatiContributivi.CategoriaFondoPI.Value == UtilityCategoriaFondoPI.U ||
                            //                                                            this.areaDatiContributivi.CategoriaFondoPI.Value == UtilityCategoriaFondoPI.V))
                            // Il tipo calcolo è ininfluente rispetto ai dati
                            bReturn = true;
                            break;
                    }

                    if (!bReturn)
                    {
                        ucAvviso.Visible = !bReturn;
                        ucAvviso.Tipo = TipoAvviso.Warning;
                        ucAvviso.Messaggio = "'Tipo Calcolo' incongruente; verificare la sezione 'Liquidazione Pensione'.";
                        ManageBtnPage(bReturn);
                    }
                    break;

                case GestioneContribTipoCalcolo.Misto:
                case GestioneContribTipoCalcolo.RetributivoMonti:
                    switch (this.domanda.Tipofondo)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                            ManageRiduzioneRetributiva();

                            if (!((IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi) && IsDatiRetributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi)) ||
                                (!IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi) && !IsDatiRetributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi))))
                            {
                                bReturn = false;
                                ucDatiCalcoloEL_TT_ET.EnableDisableBtnSalva(bReturn);
                            }
                            else
                                bReturn = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:

                            ManageRiduzioneRetributiva();

                            if (!((IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi) && IsDatiRetributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi)) ||
                                (!IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi) && !IsDatiRetributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi))))
                            {
                                bReturn = false;
                                ucDatiCalcoloVL_FS_PT.EnableDisableBtnSalva(bReturn);
                            }
                            else
                                bReturn = true;
                            break;

                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                            if (!((IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi) && IsDatiRetributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi)) ||
                                (!IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi) && !IsDatiRetributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi))))
                            {
                                bReturn = false;
                                ucDatiCalcoloVL_FS_PT.EnableDisableBtnSalva(bReturn);
                            }
                            else
                                bReturn = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                            //ATTENZIONE : mancano i requisiti
                            bReturn = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:

                            ManageRiduzioneRetributiva();

                            if (!((IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi) && IsDatiRetributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi)) ||
                                (!IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi) && !IsDatiRetributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi))))
                            {
                                bReturn = false;
                                ucDatiAgoGAS_ES.EnableDisableBtnSalva(bReturn);
                            }
                            else
                                bReturn = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:

                            ManageRiduzioneRetributiva();

                            if (!((IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi) && IsDatiRetributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi)) ||
                                (!IsDatiContributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi) && !IsDatiRetributiviNull(this.domanda.Tipofondo, this.areaDatiContributivi))))
                            {
                                bReturn = false;
                                ucDatiCalcoloDZ.EnableDisableBtnSalva(bReturn);
                            }
                            else
                                bReturn = true;
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                            //if (this.areaDatiContributivi.CategoriaFondoPI.HasValue && (this.areaDatiContributivi.CategoriaFondoPI.Value == UtilityCategoriaFondoPI.U ||
                            //                                                            this.areaDatiContributivi.CategoriaFondoPI.Value == UtilityCategoriaFondoPI.V))
                            // Il tipo calcolo è ininfluente rispetto ai dati
                            bReturn = true;
                            break;
                    }

                    if (!bReturn)
                    {
                        ucAvviso.Visible = !bReturn;
                        ucAvviso.Tipo = TipoAvviso.Warning;
                        ucAvviso.Messaggio = "'Tipo Calcolo' incongruente; verificare la sezione 'Liquidazione Pensione'.";

                        ManageBtnPage(false);
                    }
                    break;

                case GestioneContribTipoCalcolo.NonValido:
                    switch (this.domanda.Tipofondo)
                    {
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                            ucDatiCalcoloEL_TT_ET.EnableDisableBtnSalva(false);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                            ucDatiCalcoloVL_FS_PT.EnableDisableBtnSalva(false);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                            ucDatiAgoGAS_ES.EnableDisableBtnSalva(false);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                            ucDatiCalcoloDZ.EnableDisableBtnSalva(false);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                            ucDatiCalcoloPM.EnableDisableBtnSalva(false);
                            break;
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                        case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                            //if (this.areaDatiContributivi.CategoriaFondoPI.HasValue && (this.areaDatiContributivi.CategoriaFondoPI.Value == UtilityCategoriaFondoPI.U ||
                            //                                                            this.areaDatiContributivi.CategoriaFondoPI.Value == UtilityCategoriaFondoPI.V))
                                // Il tipo calcolo è ininfluente rispetto ai dati
                                return true;
                            break;
                    }
                    ucAvviso.Visible = true;
                    ucAvviso.Tipo = TipoAvviso.Warning;
                    if (String.IsNullOrEmpty(ucAvviso.Messaggio))
                        ucAvviso.Messaggio = "E' necessario salvare il 'Tipo Calcolo' dal menu 'Liquidazione Pensione' prima di poter inserire i dati calcolo";

                    ManageBtnPage(false);
                    bReturn = false;
                    break;
            }

            return bReturn;
        }

        private void ManageRiduzioneRetributiva()
        {
            bool IsRiduzionePresent = false;
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    IsRiduzionePresent = this.ucDatiCalcoloEL_TT_ET.ManageButtonRiduzioneRetributiva();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    IsRiduzionePresent = this.ucDatiCalcoloVL_FS_PT.ManageButtonRiduzioneRetributiva(this.domanda.Tipofondo);
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    IsRiduzionePresent = this.ucDatiAgoGAS_ES.ManageButtonRiduzioneRetributiva();
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    IsRiduzionePresent = this.ucDatiCalcoloDZ.ManageButtonRiduzioneRetributiva(this.domanda.Tipofondo);
                    break;

            }
            //in caso di usuranti o salvaguardia non va mostrato pop up su 62 anni
            if (IsRiduzionePresent && this.areaDatiContributivi != null &&
                ((this.areaDatiContributivi.IsUsuranti.HasValue && this.areaDatiContributivi.IsUsuranti.Value) ||
                (this.areaDatiContributivi.TipologiaSalvaguardia.HasValue) ||
                (this.areaDatiContributivi.IsRiduzioneRetributivaEnabled.HasValue && !this.areaDatiContributivi.IsRiduzioneRetributivaEnabled.Value)))
                IsRiduzionePresent = false;

            btnSalvaNoRiduzione.Visible = !IsRiduzionePresent;
            btnPopUpPage.Visible = IsRiduzionePresent;
            btnSalva.Visible = IsRiduzionePresent;
            SetLabelRiduzioneRetributiva();
        }

        private void SetLabelRiduzioneRetributiva()
        {
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                    lblEtaTit.Text = "Età titolare inferiore a 62 anni. Confermi la mancanza della percentuale di Riduzione?";
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    lblEtaTit.Text = "Età titolare inferiore a 57 anni. Confermi la mancanza della percentuale di Riduzione?";
                    break;
            }
        }

        private void ManageBtnPage(bool isVisible)
        {
            btnSalva.Enabled = isVisible;
            btnSalvaNoRiduzione.Enabled = isVisible;
            btnPopUpPage.Enabled = isVisible;
        }

        private bool IsDatiContributiviNull(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, AreaDatiContributivi dati)
        {
            if (dati == null || dati.DatiCalcolo == null)
                return true;

            bool breturn = false;
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                    if (!dati.DatiCalcolo.Montante.HasValue && !dati.DatiCalcolo.ImportoContributivoTotale.HasValue && !dati.DatiCalcolo.NSettimane.HasValue &&
                        !dati.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue && !dati.DatiCalcolo.MontanteQuotaDL214.HasValue && !dati.DatiCalcolo.NSettimaneQuotaDL214.HasValue)
                        breturn = true;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    if (!dati.DatiCalcolo.Montante.HasValue && !dati.DatiCalcolo.ImportoContributivoTotale.HasValue && !dati.DatiCalcolo.NSettimane.HasValue &&
                        !dati.DatiCalcolo.ImportoContribTotaleQuotaDL214.HasValue && !dati.DatiCalcolo.MontanteQuotaDL214.HasValue && !dati.DatiCalcolo.NSettimaneQuotaDL214.HasValue)
                        breturn = true;
                    break;
                //case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                //case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                //    if (!dati.DatiCalcolo.Montante.HasValue && !dati.DatiCalcolo.ImportoContributivoTotale.HasValue && !dati.DatiCalcolo.MontanteContributivo.HasValue)
                //        breturn = true;
                //    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                    if (!dati.DatiCalcolo.Montante.HasValue && !dati.DatiCalcolo.MontanteEsclusivo.HasValue && !dati.DatiCalcolo.NSettimane.HasValue && !dati.DatiCalcolo.MontanteQuotaDL214.HasValue &&
                        !dati.DatiCalcolo.NSettimaneQuotaDL214.HasValue)
                        breturn = true;
                    break;


            }
            return breturn;
        }

        private bool IsDatiRetributiviNull(AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo, AreaDatiContributivi dati)
        {
            if (dati == null || dati.DatiCalcolo == null)
                return true;
            bool breturn = false;
            switch (this.domanda.Tipofondo)
            {
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    if (dati.IsAnteArmonizzazione.GetValueOrDefault())
                    {
                        if (!(dati.DatiCalcolo.fondoEL != null && dati.DatiCalcolo.fondoEL.LServizioUtile != null && dati.DatiCalcolo.fondoEL.LServizioUtile.Count() > 0) &&
                            !dati.DatiCalcolo.RMSQuotaA.HasValue && !dati.DatiCalcolo.RMSQuotaB.HasValue && !dati.DatiCalcolo.RMSQuotaD.HasValue && !dati.DatiCalcolo.NSettimaneQuotaA.HasValue &&
                            !dati.DatiCalcolo.NSettimaneQuotaB.HasValue && !dati.DatiCalcolo.NSettimaneQuotaC.HasValue && !dati.DatiCalcolo.NSettimaneQuotaD.HasValue && !dati.DatiCalcolo.RetribuzionePonderataAnnua.HasValue)
                            breturn = true;
                    }
                    else
                    {
                        if (!dati.DatiCalcolo.RMSQuotaA.HasValue && !dati.DatiCalcolo.RMSQuotaB.HasValue && !dati.DatiCalcolo.RMSQuotaD.HasValue && !dati.DatiCalcolo.NSettimaneQuotaA.HasValue &&
                            !dati.DatiCalcolo.NSettimaneQuotaB.HasValue && !dati.DatiCalcolo.NSettimaneQuotaC.HasValue && !dati.DatiCalcolo.NSettimaneQuotaD.HasValue && !dati.DatiCalcolo.RetribuzionePonderataAnnua.HasValue)
                            breturn = true;
                    }
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    if (dati.IsAnteArmonizzazione.GetValueOrDefault())
                    {
                        if (!(dati.DatiCalcolo.fondoTT != null && dati.DatiCalcolo.fondoTT.lDatiServizioUtile != null && dati.DatiCalcolo.fondoTT.lDatiServizioUtile.Count() > 0) &&
                            !dati.DatiCalcolo.RMSQuotaA.HasValue && !dati.DatiCalcolo.RMSQuotaB.HasValue && !dati.DatiCalcolo.RMSQuotaD.HasValue && !dati.DatiCalcolo.NSettimaneQuotaA.HasValue &&
                            !dati.DatiCalcolo.NSettimaneQuotaB.HasValue && !dati.DatiCalcolo.NSettimaneQuotaC.HasValue && !dati.DatiCalcolo.NSettimaneQuotaD.HasValue && !dati.DatiCalcolo.RetribuzionePonderataAnnua.HasValue)
                            breturn = true;
                    }
                    else
                    {
                        if (!dati.DatiCalcolo.RMSQuotaA.HasValue && !dati.DatiCalcolo.RMSQuotaB.HasValue && !dati.DatiCalcolo.RMSQuotaD.HasValue && !dati.DatiCalcolo.NSettimaneQuotaA.HasValue &&
                            !dati.DatiCalcolo.NSettimaneQuotaB.HasValue && !dati.DatiCalcolo.NSettimaneQuotaC.HasValue && !dati.DatiCalcolo.NSettimaneQuotaD.HasValue && !dati.DatiCalcolo.RetribuzionePonderataAnnua.HasValue)
                            breturn = true;
                    }
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                    if (!(dati.DatiCalcolo.fondoET != null && dati.DatiCalcolo.fondoET.lDatiServizioUtile != null && dati.DatiCalcolo.fondoET.lDatiServizioUtile.Count() > 0) &&
                        !dati.DatiCalcolo.RMSQuotaA.HasValue && !dati.DatiCalcolo.RMSQuotaB.HasValue && !dati.DatiCalcolo.RMSQuotaD.HasValue && !dati.DatiCalcolo.NSettimaneQuotaA.HasValue &&
                        !dati.DatiCalcolo.NSettimaneQuotaB.HasValue && !dati.DatiCalcolo.NSettimaneQuotaC.HasValue && !dati.DatiCalcolo.NSettimaneQuotaD.HasValue && !dati.DatiCalcolo.RetribuzionePonderataAnnua.HasValue)
                        breturn = true;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    if (!dati.DatiCalcolo.RMSQuotaA.HasValue && !dati.DatiCalcolo.RMSQuotaB.HasValue && !dati.DatiCalcolo.RMSQuotaD.HasValue && !dati.DatiCalcolo.NSettimaneQuotaA.HasValue &&
                        !dati.DatiCalcolo.NSettimaneQuotaB.HasValue && !dati.DatiCalcolo.NSettimaneQuotaC.HasValue && !dati.DatiCalcolo.NSettimaneQuotaD.HasValue &&
                        !dati.DatiCalcolo.NSettimaneQuotaA2.HasValue && !dati.DatiCalcolo.NSettimaneQuotaC2.HasValue)
                        breturn = true;
                    break;

                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    if (!dati.DatiCalcolo.IdPensione.HasValue)
                        breturn = true;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                    if (!dati.DatiCalcolo.RMSQuotaA.HasValue && !dati.DatiCalcolo.RMSQuotaB.HasValue && !dati.DatiCalcolo.NSettimaneQuotaA.HasValue && !dati.DatiCalcolo.NSettimaneEsclusiveQuotaA.HasValue &&
                        !dati.DatiCalcolo.NSettimaneQuotaB.HasValue && !dati.DatiCalcolo.NSettimaneEsclusiveQuotaB.HasValue)
                        breturn = true;
                    break;
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                    if (!dati.DatiCalcolo.RMSQuotaA.HasValue && !dati.DatiCalcolo.RMSQuotaB.HasValue && !dati.DatiCalcolo.NSettimaneQuotaA.HasValue && !dati.DatiCalcolo.NSettimaneQuotaB.HasValue)
                        breturn = true;
                    break;
            }
            return breturn;
        }

        private void GetDataEventsTabDatiCalcolo(object sender, Utility.CustomEventArgs e, out bool hasError, out string errorMsg)
        {
            hasError = false;
            errorMsg = string.Empty;

            AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo? tipoFondo = (AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo?)e.TipoFondo;
            if (tipoFondo.HasValue)
            {
                switch (tipoFondo.Value)
                {
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.EL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.TT:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ET:
                        INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiCalcoloEL_TT_ET tabDatiCalcoloEL_TT_ET = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiCalcoloEL_TT_ET)sender;
                        hasError = tabDatiCalcoloEL_TT_ET.HasError;
                        errorMsg = tabDatiCalcoloEL_TT_ET.ErrorMessage;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.VL:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT:
                        INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiCalcoloVL_FS_PT tabDatiCalcoloVL = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiCalcoloVL_FS_PT)sender;
                        hasError = tabDatiCalcoloVL.HasError;
                        errorMsg = tabDatiCalcoloVL.ErrorMessage;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.ES:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.GAS:
                        INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAgoGAS_ES tabDatiCalcoloGAS = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAgoGAS_ES)sender;
                        hasError = tabDatiCalcoloGAS.HasError;
                        errorMsg = tabDatiCalcoloGAS.ErrorMessage;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.DZ:
                        INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiCalcoloDZ tabDatiCalcoloDZ = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiCalcoloDZ)sender;
                        hasError = tabDatiCalcoloDZ.HasError;
                        errorMsg = tabDatiCalcoloDZ.ErrorMessage;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PM:
                        INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiCalcoloPM tabDatiCalcoloPM = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiCalcoloPM)sender;
                        hasError = tabDatiCalcoloPM.HasError;
                        errorMsg = tabDatiCalcoloPM.ErrorMessage;
                        break;
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PI:
                    case AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PL:
                        INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiCalcoloPI tabDatiCalcoloPI = (INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiCalcoloPI)sender;
                        hasError = tabDatiCalcoloPI.HasError;
                        errorMsg = tabDatiCalcoloPI.ErrorMessage;
                        break;
                }
            }
        }

        private void RenderSemafori()
        {
            if ((AreaQuadri)Session["Semaforo"] != null)
            {
                domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                this.areaQuadri = (AreaQuadri)Session["Semaforo"];

                if (this.areaQuadri.QuadroDatiContributivi.TabDatiCalcolo != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#dati_calcolo";
                else if (this.areaQuadri.QuadroDatiContributivi.TabDatiFondo != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#dati_fondo";
                else if (this.areaQuadri.QuadroDatiContributivi.TabDatiAgo != AreaQuadri.Semaforo.Rosso_NonAbilitato)
                    hdnSelected.Value = "#dati_ago";
            }
        }

        private void ResetMessaggiPagina()
        {
            ucAvviso.Visible = false;
            ucAvviso.Messaggio = "";

            HasError = false;
            ErrorMessage = "";
        }

        #endregion private methods

        #region Enum

        public enum EnumViewState
        {
            AreaDatiContributivi
        }

        #endregion Enum
    }
}
