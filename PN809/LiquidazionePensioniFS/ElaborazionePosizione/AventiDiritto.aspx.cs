using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AventiDiritto;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    [NavigationRules(AllowedReferrer = AllowedReferrer.Application, CheckSequenceOnPostBack = false)]
    public partial class AventiDiritto : CustomBasePage, IInfoLiquidazione, IQuadriSemafori, IAventiDiritto
    {
        #region IInfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion IInfoLiquidazione

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IQuadriSemafori
        public AreaQuadri areaQuadri { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        public AreaInfoPratica areaInfoPratica { get; set; }
        #endregion IQuadriSemafori

        #region IAventiDiritto
        public AreaAventiDiritto AreaAventiDiritto { get; set; }
        #endregion IAventiDiritto

        #region protected methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            if (!IsPostBack)
            {
                this.InfoLiquidazione = ValorizzaInfoLiquidazione(ucInfoLiquidazione);
                CaricaDati();
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
                    ValorizzaSemaforiTab(imgAventiDiritto, this.areaQuadri.QuadroAventiDiritto.TabAventiDiritto, pnlTabAventiDiritto);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("AventiDiritto, Errore nel metodo Page_PreRender " + ex);
            }
        }

        protected void btnSalvaTutto_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.AreaAventiDiritto = ucAventiDiritto.RecuperaCampi();
            PresenterAventiDiritto presenter = new PresenterAventiDiritto();
            presenter.SalvaDatiAventiDiritto(this);

            if (this.HasError)
            {
                ucAvviso.Tipo = TipoAvviso.Warning;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = this.ErrorMessage;
                return;
            }
            else
            {
                ucAventiDiritto.ValorizzaEtichette(this);

                ucAvviso.Tipo = TipoAvviso.Ok;
                ucAvviso.Visible = true;
                ucAvviso.Messaggio = "Dati Aventi Diritto salvati correttamente";
            }

            event_ucAggiornaSemaforo(this, null);
        }

        protected void event_ucShowAvviso(object sender, EventArgs e)
        {
            if (((IAventiDiritto)sender).HasError)
                ucAvviso.Tipo = TipoAvviso.Warning;
            else
                ucAvviso.Tipo = TipoAvviso.Ok;

            ucAvviso.Visible = true;
            ucAvviso.Messaggio = ((IAventiDiritto)sender).ErrorMessage;
        }

        protected void event_ucAggiornaSemaforo(object sender, EventArgs e)
        {
            this.areaInfoPratica = new AreaInfoPratica();
            List<AreaQuadri.Tab> elencoTab = new List<AreaQuadri.Tab>();
            elencoTab.Add(AreaQuadri.Tab.AventiDiritto);
            elencoTab.Add(AreaQuadri.Tab.Periodi);
            this.areaInfoPratica.ElencoTab = elencoTab.ToArray();

            CodeUtility.AggiornaSemafori(this, this, ucInfoLiquidazione);
        }
        #endregion protected methods

        #region private methods
        private void CaricaDati()
        {
            PresenterAventiDiritto presenter = new PresenterAventiDiritto();
            presenter.GetAventiDiritto(this);
            if (!this.HasError)
            {
                ucAventiDiritto.ValorizzaEtichette(this);
            }
            else
            {
                ucAvviso.Visible = true;
                ucAvviso.Tipo = TipoAvviso.Ko;
                ucAvviso.Messaggio = ErrorMessage;
                pnlAventiPeriodo.Enabled = false;
                return;
            }
        }

        #endregion private methods
    }
}