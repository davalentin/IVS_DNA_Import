using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Periodi;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class Periodi : CustomBasePage, IInfoLiquidazione, IQuadriSemafori, IPeriodi
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IPeriodi
        public AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public AreaPeriodi areaPeriodi { get; set; }
        #endregion IPeriodi

        #region protected methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);

                // GET
                CaricaDatiPeriodi();
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
                    domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
                    this.areaQuadri = (AreaQuadri)Session["Semaforo"];
                    ValorizzaSemaforiTab(imgPeriodi, this.areaQuadri.QuadroPeriodi.TabPeriodi, pnlTabPeriodi);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("Periodi, Errore nel metodo Page_PreRender " + ex);
            }
        }

        protected void SalvaPeriodi_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaPeriodi = ucPeriodi.RecuperaCampi();
            PresenterPeriodi presenter = new PresenterPeriodi();
            presenter.SalvaDatiPeriodi(this);

            if (this.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                return;
            }
            else
            {
                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Periodi salvati correttamente";
            }

            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Periodi);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            if (((IPeriodi)sender).HasError)
                ucAvviso.Tipo = TipoAvviso.Warning;
            else
                ucAvviso.Tipo = TipoAvviso.Ok;

            ucAvviso.Visible = true;
            ucAvviso.Messaggio = ((IPeriodi)sender).ErrorMessage;
        }

        protected void event_ucHideAvviso(object sender, EventArgs e)
        {
            ucAvviso.Visible = false;
            ucAvviso.Messaggio = string.Empty;
        }

        protected void event_ucAggiornaSemaforo(object sender, EventArgs e)
        {
            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.Periodi);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }

        protected void event_ucAbilitaPulsanti(object sender, EventArgs e)
        {
            btnSalvaPeriodi.Enabled = true;
        }

        protected void event_ucDisabilitaPulsanti(object sender, EventArgs e)
        {
            btnSalvaPeriodi.Enabled = false;
        }
        #endregion protected methods

        #region private methods
        private void CaricaDatiPeriodi()
        {
            PresenterPeriodi presenter = new PresenterPeriodi();
            presenter.GetAreaPeriodi(this);

            if (this.HasError)
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ko;
                ucAvviso.Messaggio = ErrorMessage;
                pnlPeriodi.Enabled = false;
                return;
            }

            ucPeriodi.InitData(this.areaPeriodi);
        }
        #endregion private methods
    }
}